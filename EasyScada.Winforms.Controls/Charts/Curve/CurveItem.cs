using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public class CurveItem
    {
        public float[] Data;
        public string[] Text;

        public float LineThickness { get; set; }
        public bool IsSmoothCurve { get; set; }
        public Color LineColor { get; set; }
        public bool IsLeftFrame { get; set; }
        public bool Visible { get; set; }
        public bool LineRenderVisible { get; set; }
        public RectangleF TitleRegion { get; set; }
        public string RenderFormat { get; set; }

        public CurveItem()
        {
            LineThickness = 1f;
            IsLeftFrame = true;
            Visible = true;
            LineRenderVisible = true;
            TitleRegion = new RectangleF(0, 0, 0, 0);
        }
    }
}
