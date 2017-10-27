using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;

namespace ImageProcessing.Models.Algorithms
{
    public class Contrast : IImageProcessAlgorithm
    {
        private int threshold = 0;

        public Contrast(int threshold)
        {
            this.threshold = threshold;
        }

        public void Execute(byte[] bytes, int width, int height)
        {
            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            BitmapWrapper bmpWrapper = new BitmapWrapper(bytes, height, width);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Color col = bmpWrapper.GetPixel(j, i);

                    int r = (int)(((((col.R / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0);
                    int g = (int)(((((col.G / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0);
                    int b = (int)(((((col.B / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0);



                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    bmpWrapper.SetPixel(j, i, Color.FromArgb(col.A, r, g, b));

                }
            }
            
        }
    }
}
