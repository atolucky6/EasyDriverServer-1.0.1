using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    public class BezierSeries : SeriesBase
    {
        float thickness = 1.8f;

        [Category("Series")]
        public float Thickness
        {
            get => thickness;
            set => thickness = value;
        }

        internal override void Draw(Graphics g)
        {
            PointF[] locationArray = GetLocationArray();
            if (locationArray != null && locationArray.Length > 0)
            {
                int num = locationArray.Length % 3;
                int num2 = locationArray.Length / 3;
                int num3 = num == 0 ? -2 : 1;
                PointF[] points = locationArray.Take((3 * num2) + num3).ToArray();
                using (Pen pen = new Pen(Color, Thickness))
                {
                    g.DrawBeziers(pen, points);
                }
            }
        }

        protected override void PaintLegend(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            PointF[] points = new PointF[]
            {
                new PointF(0f, Legend.Height /2f),
                new PointF(3f, 0f),
                new PointF(0f, (float)Legend.Height),
                new PointF(12f, (float)Legend.Height / 2f)
            };

            using (Pen pen = new Pen(Color, Thickness))
            {
                e.Graphics.DrawBeziers(pen, points);
            }
        }
    }
}
