using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    public class StandardLineSeries : SeriesBase
    {
        private float thickness = 1f;
        private bool horizontalLineVisible = true;
        private bool verticalLineVisible = true;

        [Category("Series"), DefaultValue(1f)]
        public float Thickness
        {
            get => thickness;
            set => thickness = value;
        }

        [Category("Series"), DefaultValue(true)]
        public bool HorizontalLineVisible
        {
            get => horizontalLineVisible;
            set => horizontalLineVisible = value;
        }

        [Category("Series"), DefaultValue(true)]
        public bool VerticalLineVisible
        {
            get => verticalLineVisible;
            set => verticalLineVisible = value;
        }

        internal override void Draw(Graphics g)
        {
            PointF[] locationArray = GetLocationArray();
            if (locationArray != null && locationArray.Length != 0)
            {
                using (Pen pen = new Pen(Color, thickness))
                {
                    if (horizontalLineVisible && ParentAxisX != null)
                    {
                        float x1 = CountLeft(ParentAxisX.MinValue);
                        float x2 = CountLeft(ParentAxisX.MaxValue);
                        foreach (PointF pf in locationArray)
                        {
                            g.DrawLine(pen, x1, pf.Y, x2, pf.Y);
                        }
                    }
                    if (verticalLineVisible && ParentAxisY != null)
                    {
                        float y1 = CountTop(ParentAxisY.MinValue);
                        float y2 = CountTop(ParentAxisY.MaxValue);

                        foreach (PointF pf in locationArray)
                        {
                            g.DrawLine(pen, pf.X, y1, pf.X, y2);
                        }
                    }
                }
            }
        }

        protected override void PaintLegend(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            float y = (Legend.Height - thickness) / 2f;
            using (Pen pen = new Pen(Color, thickness))
            {
                e.Graphics.DrawLine(pen, 0f, y, 12f, y);
            }
        }
    }
}
