using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;

namespace ImageProcessing.Models.Algorithms
{
    public class SobelEdgeDetection : IImageProcessAlgorithm
    {
        private int[] mask;
        private int[] cordinatesX;
        private int[] cordinatesY;

        public SobelEdgeDetection()
        {
            mask = new[] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            cordinatesY = new[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
            cordinatesX = new[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        }

        public void Execute(byte[] bytes, int width, int height)
        {
            BitmapWrapper bmpWrapper = new BitmapWrapper(bytes, height, width);
            Color[,] tmpBuffer = new Color[height, width];
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        var col = bmpWrapper.GetPixel(j + cordinatesY[k], i + cordinatesX[k]);
                        r += col.R * mask[k];
                        g += col.G * mask[k];
                        b += col.B * mask[k];

                    }

                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    tmpBuffer[j, i] = Color.FromArgb(255, r , g , b);
                }
            }

            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    bmpWrapper.SetPixel(j, i, tmpBuffer[j, i]);
                }
            }
        }
    }
}
