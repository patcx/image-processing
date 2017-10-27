using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models.Algorithms
{
    public class Brightness : IImageProcessAlgorithm
    {
        private int value;
        public Brightness(int value)
        {
            this.value = value;
        }

        public void Execute(byte[] bytes, int width, int height)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)Math.Min(255, Math.Max(0, bytes[i] + value));
            }
        }
    }
}
