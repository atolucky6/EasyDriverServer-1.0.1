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
    public class LineSeries : SeriesBase
    {
        private float thickness = 1.8f;
        private float curveTension = 0f;
        private bool fillGraph = true;
        private bool pointVisible = false;
        private bool valueVisible = false;
        private Font font = new Font(FontFamily.GenericSansSerif, 10f);

        [Category("Series"), DefaultValue(1.8f)]
        public float Thickness
        {
            get => thickness;
            set => thickness = value;
        }

        [Category("Series"), DefaultValue(0f)]
        public float CurveTension
        {
            get => curveTension;
            set => curveTension = value;
        }

        [Category("Series"), DefaultValue(true)]
        public bool FillGraph
        {
            get => fillGraph;
            set => fillGraph = value;
        }

        [Category("Series"), DefaultValue(false)]
        public bool PointVisible
        {
            get => pointVisible;
            set => pointVisible = value;
        }

        [Category("Series"), DefaultValue(false)]
        public bool ValueVisible
        {
            get => valueVisible;
            set => valueVisible = value;
        }

        [Category("Series")]
        public Font Font
        {
            get => font;
            set => font = value;
        }

        internal override void Draw(Graphics g)
        {
            PointF[] locationArray = GetLocationArray();
            if (locationArray != null && locationArray.Length > 1)
            {
                using (Pen pen = new Pen(Color, Thickness))
                {
                    if (CurveTension > 0f)
                    {
                        g.DrawCurve(pen, locationArray, CurveTension);
                    }
                    else
                    {
                        g.DrawLines(pen, locationArray);
                    }
                }

                if (FillGraph && ParentAxisY != null)
                {
                    double yValue = ParentAxisY.IsReverse ? ParentAxisY.MaxValue : ParentAxisY.MinValue;
                    int y = (int)Math.Round((double)CountTop(yValue));
                    PointF[] array = new PointF[locationArray.Length + 2];
                    locationArray.CopyTo(array, 0);
                    array[array.Length - 1] = new PointF(locationArray[0].X, y);
                    array[array.Length - 2] = new PointF(locationArray[locationArray.Length - 1].X, y);
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color)))
                    {
                        g.FillPolygon(brush, array);
                    }
                }
            }
            if (Points.Count > 0 && (pointVisible || valueVisible))
            {
                using (SolidBrush brush = new SolidBrush(Color))
                {
                    if (pointVisible)
                    {
                        float width = 4f * thickness;
                        foreach (ChartPoint point in Points)
                        {
                            int x = (int)Math.Round(point.Left - (2f * thickness));
                            int y = (int)Math.Round(point.Top - (2f * thickness));
                            g.FillEllipse(brush, x, y, width, width);
                        }
                    }
                    if (valueVisible)
                    {
                        ChartPoint point = Points.Last();
                        string yStr = point.Y.ToString();
                        if (!string.IsNullOrEmpty(yStr))
                        {
                            g.DrawString(yStr, Font, brush, new Point(point.Left, point.Top), SeriesFormat.LabelFormat);
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
