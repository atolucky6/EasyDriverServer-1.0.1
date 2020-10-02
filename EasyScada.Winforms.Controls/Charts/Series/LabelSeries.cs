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
    public class LabelSeries : SeriesBase
    {
        private int width = 12;
        private int height = 12;
        private Font font = new Font(FontFamily.GenericSansSerif, 10f);

        [Category("Series")]
        public int Width
        {
            get => width;
            set => width = value;
        }

        [Category("Series")]
        public int Height
        {
            get => height;
            set => height = value;
        }

        [Category("Series")]
        public Font Font
        {
            get => font;
            set => font = value;
        }

        internal override void Draw(Graphics g)
        {
            if (Points.Count > 0)
            {
                using (SolidBrush brush = new SolidBrush(Color))
                {
                    foreach (ChartPoint point in Points)
                    {
                        int left = point.Left;
                        int top = point.Top;
                        Point[] points = new Point[]
                        {
                            new Point(left, top),
                            new Point(left - (Width / 2), top - Height),
                            new Point(left + (Width / 2), top - Height)
                        };

                        g.FillPolygon(brush, points);
                        if (!string.IsNullOrEmpty(point.Label))
                        {
                            Point pt = new Point(left, top - height);
                            g.DrawString(point.Label, font, brush, (PointF)pt, SeriesFormat.LabelFormat);
                        }
                    }
                }
            }
        }

        protected override void PaintLegend(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            float y = (Legend.Height - 12f) / 2f;
            PointF[] points = new PointF[]
            {
                new PointF(0f, y),
                new PointF(12f, y),
                new PointF(6f, y + 12f)
            };
            using (SolidBrush brush = new SolidBrush(Color))
            {
                e.Graphics.FillPolygon(brush, points);
            }
        }
    }
}
