using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public class RGB
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public static RGB Scale(RGB rgb)
        {
            if (rgb.R >= 0 && rgb.R <= 255 && rgb.G >= 0 && rgb.G <= 255 && rgb.B >= 0 && rgb.B <= 255)
                return rgb;

            if (rgb.R > 255)
            {
                int outRange = rgb.R - 255;
                outRange = outRange / 2;
                rgb.R = 255;
                rgb.B += outRange;
                rgb.G += outRange;
                return Scale(rgb);
            }
            else if (rgb.B > 255)
            {
                int outRange = rgb.B - 255;
                outRange = outRange / 2;
                rgb.B = 255;
                rgb.R += outRange;
                rgb.G += outRange;
                return Scale(rgb);
            }
            else if (rgb.G > 255)
            {
                int outRange = rgb.G - 255;
                outRange = outRange / 2;
                rgb.G = 255;
                rgb.R += outRange;
                rgb.B += outRange;
                return Scale(rgb);
            }
            return rgb;
        }
    }
}
