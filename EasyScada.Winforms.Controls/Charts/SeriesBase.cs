using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls.Charts
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class SeriesBase
    {
        #region Fields
        private EasyChart chart;
        private string title = "Series";
        private ChartPointCollection points = new ChartPointCollection();
        private int scalesXAt;
        private int scalesYAt;
        private Color color;
        private bool visible = true;
        private Label legend;
        private bool legenVisible = true;
        #endregion

        #region Public properties
        [Browsable(false)]
        public EasyChart Chart
        {
            get => chart;
            internal set => chart = value;
        }

        [Category("Easy Chart"), DefaultValue("Series")]
        public string Title
        {
            get => title;
            set
            {
                title = value;
                legend.Text = title;
            }
        }

        [Category("Easy Chart"), TypeConverter(typeof(CollectionConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartPointCollection Points
        {
            get => points;
            private set => points = value;
        }

        [Browsable(false)]
        public double? MaxValueX
        {
            get
            {
                if (points == null || points.Count < 1)
                    return null;
                return new double?((from cp in points select cp.X).Max());
            }
        }

        [Browsable(false)]
        public double? MaxValueY
        {
            get
            {
                if (points == null || points.Count < 1)
                    return null;
                return new double?((from cp in points select cp.Y).Max());
            }
        }

        [Browsable(false)]
        public double? MinValueX
        {
            get
            {
                if (points == null || points.Count < 1)
                    return null;
                return new double?((from cp in points select cp.X).Min());
            }
        }

        [Browsable(false)]
        public double? MinValueY
        {
            get
            {
                if (points == null || points.Count < 1)
                    return null;
                return new double?((from cp in points select cp.Y).Min());
            }
        }

        [Category("Easy Chart"), DefaultValue(0)]
        public int ScalesXAt
        {
            get => scalesXAt;
            set
            {
                if (value < 0)
                    scalesXAt = 0;
                else
                    scalesXAt = value;
            }
        }

        [Category("Easy Chart"), DefaultValue(0)]
        public int ScalesYAt
        {
            get => scalesYAt;
            set
            {
                if (value < 0)
                    scalesYAt = 0;
                else
                    scalesYAt = value;
            }
        }

        [Browsable(false)]
        public Axis ParentAxisX
        {
            get
            {
                if (chart != null && scalesXAt < chart.AxisX.Count)
                    return chart.AxisX[scalesXAt];
                return null;
            }
        }

        [Browsable(false)]
        public Axis ParentAxisY
        {
            get
            {
                if (chart != null && scalesYAt < chart.AxisY.Count)
                    return chart.AxisY[scalesYAt];
                return null;
            }
        }

        [Category("Easy Chart")]
        public Color Color
        {
            get => color;
            set => color = value;
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        [Browsable(false)]
        internal Label Legend
        {
            get => legend;
            set => legend = value;
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool LegendVisible
        {
            get => legenVisible;
            set
            {
                legenVisible = value;
                legend.Visible = value;
            }
        }
        #endregion

        #region Constructors
        public SeriesBase()
        {
            color = GetColorByIndex(EasyChart.ColorIndex);
            EasyChart.ColorIndex++;
            legend = new Label()
            {
                AutoSize = true,
                Padding = new Padding(12, 0, 0, 0),
                Text = Title
            };
            legend.Paint += PaintLegend;
        }
        #endregion

        #region Methods
        public float CountLeft(double xValue)
        {
            double width = chart.Border.Width;
            double avg = width / ParentAxisX.Range;
            double left = (xValue - ParentAxisX.MinValue) * avg;
            if (ParentAxisX.IsReverse)
                left = width - left;
            left += chart.Border.Left;
            return Convert.ToSingle(left);
        }

        public float CountTop(double yValue)
        {
            double height = chart.Border.Height;
            double avg = height / ParentAxisY.Range;
            double top = (yValue - ParentAxisY.MinValue) * avg;
            if (ParentAxisY.IsReverse)
                top = height - top;
            top += chart.Border.Top;
            return Convert.ToSingle(top);
        }

        internal virtual void Draw(Graphics g)
        {

        }

        public static Color GetColorByIndex(int index)
        {
            return EasyChart.Colors[index - (EasyChart.Colors.Count * index / EasyChart.Colors.Count)];
        }

        public PointF[] GetLocationArray()
        {
            if (points != null && points.Count > 0)
            {
                PointF[] pts = new PointF[points.Count];
                for (int i = 0; i < pts.Length; i++)
                {
                    pts[i] = new PointF(points[i].Left, points[i].Top);
                }
                return pts;
            }
            return null;
        }

        protected virtual void PaintLegend(object sender, PaintEventArgs e)
        {

        }

        public ChartPoint Select(int x, int y, float range = 0f)
        {
            ChartPoint[] pointArray = this.SelectAll(x, y, range);
            if (pointArray == null)
                return null;
            int temp = int.MaxValue;
            ChartPoint point = null;
            foreach (var item in pointArray)
            {
                int n = Math.Abs(x - item.Left + Math.Abs(y - item.Top));
                if (n < temp)
                {
                    point = item;
                    temp = n;
                }
            }
            return point;
        }

        public ChartPoint[] SelectAll(int x, int y, float range = 0f)
        {
            IEnumerable<ChartPoint> source = from cp in points
                                             where (((cp.Left >= (x - range)) && (cp.Left <= (x + range))) && (cp.Top >= (y - range))) && (cp.Top <= (y + range))
                                             select cp;
            if (source == null)
            {
                return null;
            }
            return source.ToArray<ChartPoint>();
        }

        public ChartPoint[] SelectAllByX(int x, float range = 0f)
        {
            IEnumerable<ChartPoint> source = from cp in points
                                             where (cp.Left >= (x - range)) && (cp.Left <= (x + range))
                                             select cp;
            if (source == null)
            {
                return null;
            }
            return source.ToArray<ChartPoint>();
        }

        public ChartPoint[] SelectAllByY(int y, float range = 0f)
        {
            IEnumerable<ChartPoint> source = from cp in points
                                             where (cp.Top >= (y - range)) && (cp.Top <= (y + range))
                                             select cp;
            if (source == null)
            {
                return null;
            }
            return source.ToArray<ChartPoint>();
        }

        public ChartPoint SelectByX(int x, float range = 0f)
        {
            ChartPoint[] pointArray = this.SelectAllByX(x, range);
            if (pointArray == null)
            {
                return null;
            }
            int num = int.MaxValue;
            ChartPoint point2 = null;
            foreach (ChartPoint point3 in pointArray)
            {
                int num3 = Math.Abs(x - point3.Left);
                if (num3 < num)
                {
                    point2 = point3;
                    num = num3;
                }
            }
            return point2;
        }

        public ChartPoint SelectByY(int y, float range = 0f)
        {
            ChartPoint[] pointArray = this.SelectAllByY(y, range);
            if (pointArray == null)
            {
                return null;
            }
            int num = int.MaxValue;
            ChartPoint point2 = null;
            foreach (ChartPoint point3 in pointArray)
            {
                int num3 = Math.Abs(y - point3.Top);
                if (num3 < num)
                {
                    point2 = point3;
                    num = num3;
                }
            }
            return point2;
        }

        public void SetValuesX(double[] values, double startValue = 0.0, double valueStep = 0.1)
        {
            points.Clear();
            if ((values != null) && (values.Length != 0))
            {
                chart.BeginUpdate();
                for (int i = 0; i < values.Length; i++)
                {
                    points.Add(new ChartPoint(values[i], (i * valueStep) + startValue));
                }
                chart.EndUpdate();
            }
        }

        public void SetValuesY(double[] values, double startValue = 0.0, double valueStep = 0.1)
        {
            points.Clear();
            if ((values != null) && (values.Length != 0))
            {
                chart.BeginUpdate();
                for (int i = 0; i < values.Length; i++)
                {
                    points.Add(new ChartPoint((i * valueStep) + startValue, values[i]));
                }
                chart.EndUpdate();
            }
        }
        #endregion
    }
}
