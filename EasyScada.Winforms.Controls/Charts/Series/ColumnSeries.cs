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
    public class ColumnSeries : SeriesBase
    {
        private int width = 24;

        [Category("Series")]
        public int Width
        {
            get => width;
            set => width = value;
        }

        internal static void Draw(Graphics g, Rectangle border, int sum, ColumnSeries[] series)
        {
            int count = series.Count();
            float totalWidth = 0f;
            for (int i = 0; i < count; i++)
            {
                totalWidth += series[i].width;
            }
            if (totalWidth > 0)
            {
                totalWidth += (count - 1) * 3f;

                float width = border.Width;
                float num5 = width / sum;
                float num6 = border.Left + num5 - (totalWidth / 2f);
                for (int i = 0; i < count; i++)
                {
                    if (series[i].Visible)
                    {
                        PointF[] locationArray = series[i].GetLocationArray();
                        if (locationArray != null && locationArray.Length > 0)
                        {
                            using (SolidBrush brush = new SolidBrush(series[i].Color))
                            {
                                for (int k = 0; k < locationArray.Length; k++)
                                {
                                    float x = num6 + (num5 * k);
                                    float y = locationArray[k].Y;
                                    float w = series[i].width;
                                    float h = (border.Bottom - locationArray[k].Y) - 1f;
                                    RectangleF rect = new RectangleF(x, y, w, h);
                                    g.FillRectangle(brush, rect);
                                }
                            }
                        }
                        num6 += series[i].Width + 3f;
                    }
                }
            }
        }

        protected override void PaintLegend(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (SolidBrush brush = new SolidBrush(Color))
            {
                e.Graphics.FillRectangle(brush, 2f, (Legend.Height - 16f) / 2f, 12f, 16f);
            }
        }
    }
}
