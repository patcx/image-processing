using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ImageProcessing.Models.Algorithms;

namespace ImageProcessing.Helpers
{
    public  static class BitmapHelper
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    
        public static void ApplyTransformation(this Bitmap image, IImageProcessAlgorithm imageProcess)
        {
            BitmapData sourceData = image.LockBits(new Rectangle(0, 0,
                                       image.Width, image.Height),
                                       ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            image.UnlockBits(sourceData);

            imageProcess.Execute(pixelBuffer, image.Width, image.Height);

            BitmapData resultData = image.LockBits(new Rectangle(0, 0,
                                        image.Width, image.Height),
                                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            image.UnlockBits(resultData);
        }
        public static void Fill(this Bitmap image, Color color)
        {
            using (var g = Graphics.FromImage(image))
            {
                using (var b = new SolidBrush(color))
                {
                    g.FillRectangle(b, 0, 0, image.Width, image.Height);
                }
            }
        }
        public static void DrawBar(this Bitmap image, int x, int from, int to, Color color)
        {
            using (var g = Graphics.FromImage(image))
            {
                using (var pen = new Pen(color))
                {
                    g.DrawLine(pen, x, image.Height-to, x, image.Height-from);
                }
            }
        }
        public static void DrawBarH(this Bitmap image, int y, int from, int to, Color color)
        {
            using (var g = Graphics.FromImage(image))
            {
                using (var pen = new Pen(color))
                {
                    g.DrawLine(pen, from, y, to, y);
                }
            }
        }
    }
}
