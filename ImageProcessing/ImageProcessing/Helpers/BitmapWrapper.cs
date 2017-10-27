using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Helpers
{
    public class BitmapWrapper
    {
        private byte[] bytes;
        private int height;
        private int width;

        public BitmapWrapper(byte[] bytes, int height, int width)
        {
            this.height = height;
            this.width = width;
            this.bytes = bytes;
        }

        public Color GetPixel(int j, int i)
        {
            var baseIndex = j * width * 4 + i*4;
            return Color.FromArgb(bytes[baseIndex+3], bytes[baseIndex + 2], bytes[baseIndex + 1], bytes[baseIndex]);
        }

        public void SetPixel(int j, int i, Color color)
        {
            var baseIndex = j * width * 4 + i*4;
            bytes[baseIndex] = color.B;
            bytes[baseIndex + 1] = color.G;
            bytes[baseIndex + 2] = color.R;
            bytes[baseIndex + 3] = color.A;
        }
    }
}
