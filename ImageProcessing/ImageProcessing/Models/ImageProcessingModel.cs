using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;

namespace ImageProcessing.Models
{
    public class ImageProcessingModel
    {
        private int contrast = 0;
        private float brightness = 1;
        private bool isGray = false;
        private bool isNegative = false;

        private Bitmap baseImageSource;
        private object bitmapLock = new object();

        public Bitmap ImageSource { get; set; }
        public Bitmap Histogram { get; set; }
        public Bitmap Projection { get; set; }

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

        public void SetBrightness(float brightness)
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
                    ImageSource.ApplyNegative();
                if (isGray)
                    ImageSource.ApplyGray();

                if(Math.Abs(brightness - 1) > float.Epsilon)
                    ImageSource.AdjustBrightness(brightness);
                if(contrast != 0)
                     ImageSource.AdjustContrast(contrast);
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
                        select x).Skip(0).First();
            int maxG = (from x in colorG
                        orderby x descending
                        select x).Skip(0).First();
            int maxB = (from x in colorB
                        orderby x descending
                        select x).Skip(0).First();

            int max = Math.Max(Math.Max(colorR.Max(), colorB.Max()), colorG.Max());

            Bitmap bm = new Bitmap(255, 300);
            using (var g = Graphics.FromImage(bm))
            {
                using (var b = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(b, 0, 0, bm.Width, bm.Height);
                }
            }
          

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
            int[] projection = new int[ImageSource.Width];
            for (int i = 0; i < ImageSource.Width; i++)
            {
                for (int j = 0; j < ImageSource.Height; j++)
                {
                    var pixel = ImageSource.GetPixel(i, j);
                    if (pixel.GetBrightness() > 0.5)
                        projection[i]++;
                }
            }

            var bm = new Bitmap(ImageSource.Width, projection.Max()+1);
            for (int i = 0; i < projection.Length; i++)
            {
                bm.DrawBar(i, 0, projection[i], Color.Black);
            }

            Projection = bm;
        }
    }
}
