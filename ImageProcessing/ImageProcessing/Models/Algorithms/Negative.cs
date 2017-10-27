using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;

namespace ImageProcessing.Models.Algorithms
{
    public class Negative : IImageProcessAlgorithm
    {
        public void Execute(byte[] bytes, int width, int height)
        {
            BitmapWrapper bmpWrapper = new BitmapWrapper(bytes, height, width);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Color col = bmpWrapper.GetPixel(j, i);
                    bmpWrapper.SetPixel(j, i, Color.FromArgb(col.A, 255-col.R, 255-col.G, 255-col.B));

                }
            }
        }
    }
}
