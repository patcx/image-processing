using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;
using ImageProcessing.Models.Algorithms;

namespace ImageProcessing.Models
{
    public class ImageProcessingModel
    {
        private int contrast = 0;
        private int brightness = 0;
        private bool isGray = false;
        private bool isNegative = false;

        private Bitmap baseImageSource;
        private object bitmapLock = new object();

        public Bitmap ImageSource { get; set; }
        public Bitmap Histogram { get; set; }
        public Bitmap VerticalProjection { get; set; }
        public Bitmap HorizontalProjection { get; set; }

        public ImageProcessingModel(string path)
        {
            lock (bitmapLock)
            {
                ImageSource = (Bitmap) Image.FromFile(path);
                baseImageSource = (Bitmap)ImageSource.Clone();
                UpdateHistogram();
                UpdateProjection();
            }
        }

        public void SetConstrast(int threshold)
        {
            contrast = threshold;
            UpdateBitmap();
        }

        public void SetBrightness(int brightness)
        {
            this.brightness = brightness;
            UpdateBitmap();
        }

        public void SetNegative(bool isEnabled)
        {
            isNegative = isEnabled;
            UpdateBitmap();
        }

        public void SetGray(bool isEnabled)
        {
            isGray = isEnabled;
            UpdateBitmap();
        }

        private void UpdateBitmap()
        {
            lock (bitmapLock)
            {
                ImageSource = (Bitmap)baseImageSource.Clone();

                if (isNegative)
                    ImageSource.ApplyTransformation(new Negative());
                if (isGray)
                    ImageSource.ApplyTransformation(new Grayscale());

                if (brightness != 0)
                    ImageSource.ApplyTransformation(new Brightness(brightness));
                if(contrast != 0)
                    ImageSource.ApplyTransformation(new Contrast(contrast));

                UpdateHistogram();
                UpdateProjection();
            }
        }

        private void UpdateHistogram()
        {
            int[] colorR = new int[256];
            int[] colorG = new int[256];
            int[] colorB = new int[256];
            for (int j = 0; j < ImageSource.Height; j++)
            {
                for (int i = 0; i < ImageSource.Width; i++)
                {
                    var pixel = ImageSource.GetPixel(i, j);
                    colorR[pixel.R]++;
                    colorB[pixel.B]++;
                    colorG[pixel.G]++;
                }
            }

            int maxR = (from x in colorR
                        orderby x descending
                        select x).Skip(1).First();
            int maxG = (from x in colorG
                        orderby x descending
                        select x).Skip(1).First();
            int maxB = (from x in colorB
                        orderby x descending
                        select x).Skip(1).First();

            int max = Math.Max(Math.Max(colorR.Max(), colorB.Max()), colorG.Max());

            Bitmap bm = new Bitmap(255, 300);
            bm.Fill(Color.Black);
          

            for (int i = 0; i < 256; i++)
            {
                colorR[i] = (int)((double)colorR[i]/max * 250);
                colorG[i] = (int)((double)colorG[i]/max * 250);
                colorB[i] = (int)((double)colorB[i] / max * 250);


                var colors = (new[]
                {
                    (colorR[i], Color.FromArgb(255, 0, 0)),
                    (colorG[i], Color.FromArgb(0, 255, 0)),
                    (colorB[i], Color.FromArgb(0, 0, 255)),
                }).OrderBy(x=>x.Item1).ToArray();

                int j = 0;
                bm.DrawBar(i, j, colors[0].Item1, colors[0].Item2.Combine(colors[1].Item2, colors[2].Item2));

                if (colors[0].Item1 != colors[1].Item1)
                {
                    bm.DrawBar(i, colors[0].Item1, colors[1].Item1, colors[1].Item2.Combine(colors[2].Item2));
                }
                if (colors[1].Item1 != colors[2].Item1)
                {
                    bm.DrawBar(i, colors[1].Item1, colors[2].Item1, colors[2].Item2);
                }
            }

            Histogram = bm;
        }

        private void UpdateProjection()
        {
            int[] hprojection = new int[ImageSource.Height];
            int[] vprojection = new int[ImageSource.Width];
            for (int i = 0; i < ImageSource.Width; i++)
            {
                for (int j = 0; j < ImageSource.Height; j++)
                {
                    var pixel = ImageSource.GetPixel(i, j);
                    if (pixel.GetBrightness() < 0.5)
                    {
                        vprojection[i]++;
                        hprojection[j]++;
                    }
                }
            }

            var bmv = new Bitmap(ImageSource.Width, vprojection.Max()+1);
            for (int i = 0; i < vprojection.Length; i++)
            {
                bmv.DrawBar(i, 0, vprojection[i], Color.Black);
            }
          

            var bmh = new Bitmap(hprojection.Max() + 1, ImageSource.Height);
            for (int i = 0; i < hprojection.Length; i++)
            {
                bmh.DrawBarH(i, 0, hprojection[i], Color.Black);
            }


            VerticalProjection = bmv;
            HorizontalProjection = bmh;
        }
    }
}
