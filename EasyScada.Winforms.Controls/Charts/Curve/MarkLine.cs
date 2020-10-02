using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class MarkLine
    {
        public MarkLine()
        {
            TextBrush = Brushes.DodgerBlue;
            CircleBrush = Brushes.DodgerBlue;
            LinePen = Pens.DodgerBlue;
            IsLeftFrame = true;
        }

        public Brush TextBrush { get; set; }
        public Brush CircleBrush { get; set; }
        public Pen LinePen { get; set; }
        public PointF[] Points { get; set; }
        public string[] Marks { get; set; }
        public bool IsLineClosed { get; set; }
        public bool IsLeftFrame { get; set; }
    }
}
