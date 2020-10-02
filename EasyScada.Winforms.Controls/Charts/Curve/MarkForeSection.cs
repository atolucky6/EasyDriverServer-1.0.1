using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class MarkForeSection : MarkSectionBase
    {
        public MarkForeSection()
        {
            LinePen = Pens.Cyan;
            FontBrush = Brushes.Yellow;
            IsRenderTimeText = true;
            CursorTexts = new Dictionary<string, string>();
        }

        public float StartHeight { get; set; }
        public float Height { get; set; }
        public bool IsRenderTimeText { get; set; }
        public Pen LinePen { get; set; }
        public Brush FontBrush { get; set; }
        public string MarkText { get; set; }
        public Dictionary<string, string> CursorTexts { get; set; }
    }
}
