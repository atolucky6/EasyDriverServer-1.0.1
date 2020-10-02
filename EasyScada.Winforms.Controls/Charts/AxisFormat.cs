using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class AxisFormat
    {
        public const int TitleAngleAxisY = -90;
        public static StringFormat TitleFormatRightAxisY { get; set; }
        public static StringFormat TitleFormatLeftAxisY { get; set; }
        public static StringFormat TitleFormatTopAxisX { get; set; }
        public static StringFormat TitleFormatBottomAxisX { get; set; }
        public static StringFormat LabelFormatRightAxisY { get; set; }
        public static StringFormat LableFormatLeftAxisY { get; set; }
        public static StringFormat LabelFormatTopAxisX { get; set; }
        public static StringFormat LabelFormatBottomAxisX { get; set; }

        static AxisFormat()
        {
            TitleFormatRightAxisY = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Far
            };

            TitleFormatLeftAxisY = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            LableFormatLeftAxisY = new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            LabelFormatRightAxisY = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            TitleFormatTopAxisX = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            TitleFormatBottomAxisX = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Far
            };

            LabelFormatTopAxisX = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Far
            };

            LabelFormatBottomAxisX = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };
        }
    }
}
