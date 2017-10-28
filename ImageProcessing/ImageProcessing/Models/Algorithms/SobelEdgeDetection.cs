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
        private int[] maskX;
        private int[] maskY;
        private int[] cordinatesX;
        private int[] cordinatesY;

        public SobelEdgeDetection()
        {
            maskX = new[] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            maskY = new[] { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
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
                    int rx = 0;
                    int gx = 0;
                    int bx = 0;
                    int ry = 0;
                    int gy = 0;
                    int by = 0;


                    for (int k = 0; k < 9; k++)
                    {
                        var col = bmpWrapper.GetPixel(j + cordinatesY[k], i + cordinatesX[k]);
                        rx += col.R * maskX[k];
                        gx += col.G * maskX[k];
                        bx += col.B * maskX[k];
                        ry += col.R * maskY[k];
                        gy += col.G * maskY[k];
                        by += col.B * maskY[k];
                    }

                    int r = Math.Abs(rx + ry);
                    int g = Math.Abs(gx + gy);
                    int b = Math.Abs(bx + by);

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
