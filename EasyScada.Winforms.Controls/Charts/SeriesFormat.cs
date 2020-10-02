using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class SeriesFormat
    {
        public static StringFormat LabelFormat { get; set; }

        static SeriesFormat()
        {
            LabelFormat = new StringFormat();
            LabelFormat.Alignment = StringAlignment.Center;
            LabelFormat.LineAlignment = StringAlignment.Far;
        }
    }
}
