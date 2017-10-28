using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Helpers;

namespace ImageProcessing.Models.Algorithms
{
    public class RobertsEdgeDetection : IImageProcessAlgorithm
    {

        private int[] firstMask;
        private int[] secondMask;
        private int[] cordinatesX;
        private int[] cordinatesY;

        public RobertsEdgeDetection()
        {
            firstMask = new[] {1, -1, 0, 0};
            secondMask = new[] {1, 0, -1, 0};
            cordinatesX = new[] {0, 1, 0, 1};
            cordinatesY = new[] {0, 0, 1, 1};
        }

        public void Execute(byte[] bytes, int width, int height)
        {
            BitmapWrapper bmpWrapper = new BitmapWrapper(bytes, height, width);
            Color[,] tmpBuffer = new Color[height, width];
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    int fr = 0;
                    int fg = 0;
                    int fb = 0;
                    int sr = 0;
                    int sg = 0;
                    int sb = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        var col = bmpWrapper.GetPixel(j + cordinatesY[k], i + cordinatesX[k]);
                        fr += col.R * firstMask[k];
                        fg += col.G * firstMask[k];
                        fb += col.B * firstMask[k];
                        sr += col.R * secondMask[k];
                        sg += col.G * secondMask[k];
                        sb += col.B * secondMask[k];

                    }

                    fr = Math.Abs(fr);
                    fg = Math.Abs(fg);
                    fb = Math.Abs(fb);

                    sr = Math.Abs(sr);
                    sg = Math.Abs(sg);
                    sb = Math.Abs(sb);


                    tmpBuffer[j, i] = Color.FromArgb(255, TruncateColor(fr + sr), TruncateColor(fg  + sg), TruncateColor(fb + sb));
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

        private int TruncateColor(int v)
        {
            return Math.Min(255, Math.Max(0, v));
        }
    }
}
