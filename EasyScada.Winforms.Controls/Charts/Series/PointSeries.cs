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
    public class PointSeries : SeriesBase
    {
        private int width = 8;
        private int height = 8;

        [Category("Series"), DefaultValue(8)]
        public int Width
        {
            get => width;
            set => width = value;
        }

        [Category("Series"), DefaultValue(8)]
        public int Height
        {
            get => height;
            set => height = value;
        }

        internal override void Draw(Graphics g)
        {
            if (Points.Count > 0)
            {
                using (SolidBrush brush = new SolidBrush(Color))
                {
                    foreach (ChartPoint point in Points)
                    {
                        float x = point.Left - (width / 2f);
                        float y = point.Top - (height / 2f);
                        g.FillEllipse(brush, x, y, (float)width, (float)height);
                    }
                }
            }
        }

        protected override void PaintLegend(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (SolidBrush brush = new SolidBrush(Color))
            {
                e.Graphics.FillEllipse(brush, 2f, (Legend.Height - 8) / 2f, 8f, 8f);
            }
        }
    }
}
