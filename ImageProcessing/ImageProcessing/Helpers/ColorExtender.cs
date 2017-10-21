using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Helpers
{
    public static class ColorExtender
    {
        public static Color Combine(this Color color, params Color[] colors)
        {
            return Color.FromArgb(Math.Max(color.R + colors.Sum(x=>x.R), 255),
            Math.Max(color.G + colors.Sum(x => x.G), 255),
            Math.Max(color.B + colors.Sum(x => x.B), 255));
        }
    }
}
