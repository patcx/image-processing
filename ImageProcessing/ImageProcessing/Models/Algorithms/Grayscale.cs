using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;

namespace ImageProcessing.Models.Algorithms
{
    public class Grayscale : IImageProcessAlgorithm
    {
        public void Execute(byte[] bytes, int width, int height)
        {
            BitmapWrapper bmpWrapper = new BitmapWrapper(bytes, height, width);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Color col = bmpWrapper.GetPixel(j, i);
                    int grayScale = (int)((col.R * 0.3) + (col.G * 0.59) + (col.B * 0.11));
                    bmpWrapper.SetPixel(j, i, Color.FromArgb(col.A, grayScale, grayScale, grayScale));

                }
            }
        }
    }
}
