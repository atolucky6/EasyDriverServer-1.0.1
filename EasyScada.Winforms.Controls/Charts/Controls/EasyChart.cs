using EasyScada.Winforms.Controls.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [ToolboxItem(false)]
    public class EasyChart : Control
    {

        #region Fields
        public const int AxisMargin = 3;
        public const int MiniGridSum = 5;

        private bool isDraging = false;
        private Point markLineLocation;
        private Point dragLocation;
        private double zoomTimes = 0.0f;
        private bool updateFlag = true;
        private ToolTip mainToolTip;
        private FlowLayoutPanel legendPanel;
        private Pen markLinePen;
        private bool needDrawMarkLine;
        private int horizontalSeparatorSum = 10;
        private int verticalSeparatorSum = 10;
        private AxisCollection axisX = new AxisCollection();
        private AxisCollection axisY = new AxisCollection();
        private SeriesCollection series = new SeriesCollection();
        private Rectangle border;
        private int offsetX;
        private int offsetY;
        private bool horizontalGridVisible = true;
        private bool verticalGridVisible = true;
        private bool miniGridVisible = false;
        private DashStyle miniGridDashStyle = DashStyle.Dot;
        private bool legendVisible = true;
        private bool zoomEnabled = true;
        private bool allowDrag = true;
        private bool markLineVisible = false;
        private Color markLineColor = Color.FromArgb(80, 126, 211);
        private bool selectPointEnabled = true;
        #endregion

        #region Public properties
        public static List<Color> Colors { get; set; }

        public static int ColorIndex { get; set; }

        internal ToolTip MainToolTip
        {
            get => mainToolTip;
            set => mainToolTip = value;
        }

        internal FlowLayoutPanel LegendPanel
        {
            get => legendPanel;
            set => legendPanel = value;
        }

        [Browsable(false)]
        public Rectangle Border => border;

        [Category("Easy Chart"), DefaultValue(10)]
        public int HorizontalSeperatorSum
        {
            get => horizontalSeparatorSum;
            set
            {
                horizontalSeparatorSum = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(10)]
        public int VerticalSeperatorSum
        {
            get => verticalSeparatorSum;
            set
            {
                verticalSeparatorSum = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), TypeConverter(typeof(CollectionConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AxisCollection AxisX
        {
            get => axisX;
        }

        [Category("Easy Chart"), TypeConverter(typeof(CollectionConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AxisCollection AxisY
        {
            get => axisY;
        }

        [Category("Easy Chart"), TypeConverter(typeof(CollectionConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SeriesCollection Series
        {
            get => series;
        }

        [Browsable(false)]
        public int OffsetX
        {
            get => offsetX;
            set
            {
                offsetX = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public int OffsetY
        {
            get => offsetY;
            set
            {
                offsetY = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool HorizontalGridVisible
        {
            get => horizontalGridVisible;
            set
            {
                horizontalGridVisible = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool VerticalGridVisible
        {
            get => verticalGridVisible;
            set
            {
                verticalGridVisible = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(false)]
        public bool MiniGridVisible
        {
            get => miniGridVisible;
            set
            {
                miniGridVisible = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(DashStyle.Dot)]
        public DashStyle MiniGridDashStyle
        {
            get => miniGridDashStyle;
            set
            {
                miniGridDashStyle = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool LegendVisible
        {
            get => legendVisible;
            set
            {
                legendVisible = value;
                LegendPanel.Visible = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(DockStyle.Top)]
        public DockStyle LegendDock
        {
            get => LegendPanel.Dock;
            set
            {
                if ((value != DockStyle.None) && (value != DockStyle.Fill))
                {
                    if ((value == DockStyle.Top) || (value == DockStyle.Bottom))
                    {
                        LegendPanel.FlowDirection = FlowDirection.LeftToRight;
                    }
                    else if ((value == DockStyle.Left) || (value == DockStyle.Right))
                    {
                        LegendPanel.FlowDirection = FlowDirection.TopDown;
                    }
                    LegendPanel.Dock = value;
                    Invalidate();
                }
            }
        }

        [Category("Easy Chart")]
        public Font LegendFont
        {
            get => LegendPanel.Font;
            set
            {
                LegendPanel.Font = value;
                Invalidate();
            }
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool ZoomEnabled
        {
            get => zoomEnabled;
            set
            {
                zoomEnabled = value;
            }
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool AllowDrag
        {
            get => allowDrag;
            set
            {
                allowDrag = value;
            }
        }

        [Category("Easy Chart"), DefaultValue(false)]
        public bool MarkLineVisible
        {
            get => markLineVisible;
            set
            {
                markLineVisible = value;
            }
        }

        [Category("Easy Chart"), DefaultValue(typeof(Color), "80, 126, 211")]
        public Color MarkLineColor
        {
            get => markLineColor;
            set
            {
                markLineColor = value;
                markLinePen = new Pen(value);
            }
        }

        [Category("Easy Chart"), DefaultValue(true)]
        public bool SelectPointEnabled
        {
            get => selectPointEnabled;
            set
            {
                selectPointEnabled = value;
            }
        }

        [Category("Easy Chart"), DefaultValue(SelectMode.Both)]
        public SelectMode SelectMode { get; set; }

        [Browsable(false)]
        public int AxisXIndexForSelect { get; set; }

        [Browsable(false)]
        public int AxisYIndexForSelect { get; set; }

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout { get => base.BackgroundImageLayout; set => base.BackgroundImageLayout = value; }

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override RightToLeft RightToLeft { get => base.RightToLeft; set => base.RightToLeft = value; }
        #endregion

        #region Events
        public event ChartPointEventHandler GotChartPoint;
        #endregion

        #region Constructors
        static EasyChart()
        {
            Colors = new List<Color>()
            {
                Color.FromArgb(41, 127, 184),
                Color.FromArgb(230, 76, 60),
                Color.FromArgb(240, 195, 15),
                Color.FromArgb(26, 187, 155),
                Color.FromArgb(87, 213, 140),
                Color.FromArgb(154, 89, 181),
                Color.FromArgb(92, 109, 126),
                Color.FromArgb(22, 159, 132),
                Color.FromArgb(39, 173, 96),
                Color.FromArgb(92, 171, 225),
                Color.FromArgb(141, 68, 172),
                Color.FromArgb(229, 126, 34),
                Color.FromArgb(210, 84, 0),
                Color.FromArgb(191, 57, 43)
            };
        }

        public EasyChart()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            BackColor = Color.White;
            Font = new Font(FontFamily.GenericSansSerif, 12f);
            ForeColor = Color.FromArgb(51, 51, 51);
            Padding = new Padding(6);
            Size = new Size(400, 300);
            TabStop = false;
            axisX.Chart = this;
            axisX.Type = AxisType.X;
            axisY.Chart = this;
            axisY.Type = AxisType.Y;
            series.Chart = this;
            markLinePen = new Pen(markLineColor);
            MainToolTip = new ToolTip();
            legendPanel = new FlowLayoutPanel()
            {
                AutoSize = true,
                Dock = DockStyle.Top
            };
            Controls.Add(legendPanel);
            PaddingChanged += (s, e) => Invalidate();
        }
        #endregion

        #region Methods

        public void BeginUpdate()
        {
            updateFlag = false;
        }

        public float CountLeft(int scalesXAt, double xValue)
        {
            double width = border.Width;
            double avg = width / axisX[scalesXAt].Range;
            double left = (xValue - axisX[scalesXAt].MinValue) * avg;
            if (axisX[scalesXAt].IsReverse)
                left = width - left;
            left += border.Left;
            return Convert.ToSingle(left);
        }

        public float CountTop(int scalesYAt, double yValue)
        {
            double height = border.Height;
            double avg = height / axisY[scalesYAt].Range;
            double top = (yValue - axisY[scalesYAt].MinValue) * avg;
            if (!axisY[scalesYAt].IsReverse)
                top = height - top;
            top += border.Top;
            return Convert.ToSingle(top);
        }

        public double CountValueX(int scalesXAt, double xLocation)
        {
            double width = border.Width;
            double avg = axisX[scalesXAt].Range / width;
            double valueX = (xLocation - border.Left) * avg;
            if (axisX[scalesXAt].IsReverse)
                valueX = axisX[scalesXAt].Range - valueX;
            return valueX - Math.Abs(axisX[scalesXAt].MinValue);
        }

        public double CountValueY(int scalesYAt, double yLocation)
        {
            double height = border.Height;
            double avg = axisY[scalesYAt].Range / height;
            double valueY = (yLocation - border.Top) * avg;
            if (!axisY[scalesYAt].IsReverse)
                valueY = axisY[scalesYAt].Range - valueY;
            return valueY - Math.Abs(axisY[scalesYAt].MinValue);
        }

        private void DrawAxis(Graphics g)
        {
            DrawAxisX(g);
            DrawAxisY(g);
        }

        private void DrawAxisX(Graphics g)
        {
            float top = Padding.Top;
            float bottom = 0f;
            if (LegendPanel.Visible)
            {
                switch (LegendPanel.Dock)
                {
                    case DockStyle.Top:
                        top += LegendPanel.Height + 3f;
                        break;
                }
            }
            foreach (Axis axis in AxisX)
            {
                axis.DrawAxisX(g, Font, border, offsetX, ref top, ref bottom);
            }
        }

        private void DrawAxisY(Graphics g)
        {
            float left = Padding.Left;
            float right = 0f;
            if (LegendPanel.Visible)
            {
                switch (LegendPanel.Dock)
                {
                    case DockStyle.Left:
                        left += LegendPanel.Width + 3f;
                        break;
                }
            }

            foreach (Axis axis in AxisY)
            {
                axis.DrawAxisY(g, Font, border, offsetY, ref left, ref right);
            }
        }

        internal void DrawFrame(Graphics g)
        {
            Pen pen = new Pen(Color.FromArgb(111, ForeColor));
            Pen hvPenLine = new Pen(Color.FromArgb(58, ForeColor));
            Pen miniGridPen = new Pen(Color.FromArgb(53, ForeColor))
            {
                DashStyle = miniGridDashStyle
            };
            g.DrawRectangle(pen, border);
            pen.Dispose();

            if (horizontalGridVisible)
            {
                int left = border.Left;
                int right = border.Right;
                float height = border.Height;
                float distance = height / horizontalSeparatorSum;
                float offset = offsetY % distance;
                float horizonLineY = border.Top + offset;
                for (int i = 0; i < horizontalSeparatorSum + 1; i++)
                {
                    if (border.Contains(border.Left, (int)horizonLineY))
                    {
                        g.DrawLine(hvPenLine, left, horizonLineY, right, horizonLineY);
                    }
                    horizonLineY += distance;
                }

                if (MiniGridVisible)
                {
                    float smallDistance = distance / 5f;
                    offset = offsetY % smallDistance;
                    horizonLineY = border.Top + offset;
                    for (int j = 0; j < horizontalSeparatorSum * 5 + 1; j++)
                    {
                        if (border.Contains(border.Left, (int)horizonLineY))
                        {
                            g.DrawLine(miniGridPen, left, horizonLineY, right, horizonLineY);
                        }
                        horizonLineY += smallDistance;
                    }
                }
            }

            if (verticalGridVisible)
            {
                int top = border.Top;
                int bottom = border.Bottom;
                float width = border.Width;
                float distance = width / verticalSeparatorSum;
                float offset = offsetX % distance;
                float verticalX = border.Left + offset;

                for (int i = 0; i < verticalSeparatorSum + 1; i++)
                {
                    if (border.Contains((int)verticalX, border.Top))
                    {
                        g.DrawLine(hvPenLine, verticalX, top, verticalX, bottom);
                    }
                    verticalX += offset;
                }

                if (miniGridVisible)
                {
                    float smallDistance = distance / 5f;
                    offset = offsetX % smallDistance;
                    verticalX = border.Left + offset;
                    for (int j = 0; j < verticalSeparatorSum * 5 + 1; j++)
                    {
                        if (border.Contains((int)verticalX, border.Top))
                        {
                            g.DrawLine(hvPenLine, verticalX, top, verticalX, bottom);
                        }
                        verticalX += smallDistance;
                    }
                }
            }

            hvPenLine.Dispose();
            miniGridPen.Dispose();
        }

        private void DrawMarkLine(Graphics g)
        {
            if (needDrawMarkLine && !isDraging)
            {
                g.DrawLine(markLinePen, markLineLocation.X, border.Top, markLineLocation.X, border.Bottom);
            }
        }

        private void DrawSeries(Graphics g)
        {
            foreach (SeriesBase series in series)
            {
                if (series.Visible)
                    series.Draw(g);
            }

            List<ColumnSeries> columnSeries = new List<ColumnSeries>();
            foreach (SeriesBase series in series)
            {
                if (series is ColumnSeries col)
                    columnSeries.Add(col);
            }
            if (columnSeries.Count > 0)
                ColumnSeries.Draw(g, border, verticalSeparatorSum, columnSeries.ToArray());
        }

        public void DrawToBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                Size size = Size;
                Point location = Location;
                DockStyle dock = Dock;
                Size = bitmap.Size;
                DrawToBitmap(bitmap, ClientRectangle);
                Size = size;
                Location = location;
                Dock = dock;
            }
        }

        public void EndUpdate()
        {
            updateFlag = true;
            Invalidate();
        }

        public string GetMouseValueString(Point mouseLocation)
        {
            string str = "";
            int count = axisX.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (!axisX[i].IsCustomLabels)
                    {
                        double value = CountValueX(i, mouseLocation.X);
                        str = str + $"{axisX[i].Title}: {Math.Round(value, 2)}";
                    }
                }
            }

            count = axisY.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (!axisY[i].IsCustomLabels)
                    {
                        double value = CountValueY(i, mouseLocation.Y);
                        str = str + $"{axisY[i].Title}: {Math.Round(value, 2)}";
                    }
                }
            }
            return str;
        }

        public PointF GetPointLocation(int scalesXAt, int scalesYAt, ChartPoint cp)
        {
            return new PointF(CountLeft(scalesXAt, cp.X), CountTop(scalesYAt, cp.Y));
        }

        public string GetPointsContentBoth(ChartPoint[] pts)
        {
            string str = "";
            foreach (ChartPoint point in pts)
            {
                if (!string.IsNullOrEmpty(str))
                    str += "\r";
                string[] strArray = new string[]
                {
                    str,
                    point.Series.Title,
                    "(",
                    point.GetLabelFromAxisX(),
                    ",",
                    point.GetLabelFromAxisY(),
                    ")"
                };
                str = string.Concat(strArray);
            }
            return str;
        }

        public string GetPointsContentX(ChartPoint[] pts)
        {
            string str = pts[0].GetLabelFromAxisX() ?? "";

            foreach (ChartPoint point in pts)
            {
                string[] strArray = new string[]
                {
                    str,
                    "\r",
                    point.Series.Title,
                    ":",
                    point.GetLabelFromAxisY(),
                };
                str = string.Concat(strArray);
            }
            return str;
        }

        public string GetPointsContentY(ChartPoint[] pts)
        {
            string str = pts[0].GetLabelFromAxisY() ?? "";

            foreach (ChartPoint point in pts)
            {
                string[] strArray = new string[]
                {
                    str,
                    "\r",
                    point.Series.Title,
                    ":",
                    point.GetLabelFromAxisX(),
                };
                str = string.Concat(strArray);
            }
            return str;
        }

        private void InitializeChart()
        {
            foreach (SeriesBase series in series)
            {
                series.Chart = this;
                series.Points.Chart = this;
                series.Points.Series = series;
                foreach (ChartPoint point in series.Points)
                {
                    point.Chart = this;
                    point.Series = series;
                }
            }

            foreach (Axis axis in axisX)
            {
                axis.Chart = this;
                axis.Type = AxisType.X;
            }

            foreach (Axis axis in axisY)
            {
                axis.Chart = this;
                axis.Type = AxisType.Y;
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            InitializeChart();
        }

        private void OnGotChartPoint(ChartPointEventArgs e)
        {
            GotChartPoint?.Invoke(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (allowDrag && e.Button == MouseButtons.Left)
            {
                dragLocation = e.Location;
                isDraging = true;
                Cursor = Cursors.Hand;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mainToolTip.SetToolTip(this, null);
            if (needDrawMarkLine)
            {
                needDrawMarkLine = false;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool flag = false;
            if (allowDrag && isDraging)
            {
                int x = e.X - dragLocation.X;
                int y = e.Y - dragLocation.Y;
                dragLocation = e.Location;
                if (x != 0 || y != 0)
                {
                    OffsetX += x;
                    OffsetY += y;
                    flag = true;
                }
                if (markLineVisible)
                {
                    if (border.Contains(e.Location))
                    {
                        markLineLocation = e.Location;
                        needDrawMarkLine = true;
                        flag = true;
                    }
                    else if (needDrawMarkLine)
                    {
                        needDrawMarkLine = false;
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (selectPointEnabled && border.Contains(e.Location) && e.Button == MouseButtons.Left)
            {
                SelectPoints(e.Location);
            }
            if (allowDrag && e.Button == MouseButtons.Left)
            {
                isDraging = false;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (zoomEnabled && border.Contains(e.Location))
            {
                if (e.Delta > 0 && zoomTimes < 0.45)
                {
                    zoomTimes += 0.05;
                }
                else if (e.Delta < 0 && zoomTimes > -1.95f)
                {
                    zoomTimes -= 0.05;
                }
                zoomTimes = Math.Round(zoomTimes, 2);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (updateFlag)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Graphics g = e.Graphics;
                g.Clear(BackColor);
                UpdateAxisRange();
                UpdateChartBorder(g, base.Size);
                UpdateAxisSize();
                g.SmoothingMode = SmoothingMode.HighQuality;
                DrawFrame(g);
                DrawAxis(g);
                g.Clip = new Region(border);
                DrawSeries(g);
                DrawMarkLine(g);
                sw.Stop();
                Debug.WriteLine($"It took {sw.ElapsedMilliseconds} milliseconds to redraw.");
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        [Bindable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetRightToLeft()
        {
            base.ResetRightToLeft();
        }

        private void SelectPoints(Point location)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ChartPoint[] pts = null;
            string content = null;
            switch (SelectMode)
            {
                case SelectMode.Both:
                    pts = SelectPointsByLocation(location, 4);
                    if (pts != null)
                        content = GetPointsContentBoth(pts);
                    break;
                case SelectMode.X:
                    pts = SelectPointsByX(AxisXIndexForSelect, location.X, 4);
                    if (pts != null)
                        content = GetPointsContentX(pts);
                    break;
                case SelectMode.Y:
                    pts = SelectPointsByY(AxisYIndexForSelect, location.Y, 4);
                    if (pts != null)
                        content = GetPointsContentY(pts);
                    break;
                default:
                    break;
            }
            if (pts != null)
            {
                ChartPointEventArgs e = new ChartPointEventArgs(pts, content);
                OnGotChartPoint(e);
                MainToolTip.SetToolTip(this, e.Content);
            }
            sw.Stop();
            Debug.WriteLine($"It took {sw.ElapsedMilliseconds} to find the data.");
        }

        public ChartPoint[] SelectPointsByLocation(Point location, int range)
        {
            List<ChartPoint> list = new List<ChartPoint>();
            foreach (SeriesBase series in series)
            {
                ChartPoint cp = series.Select(location.X, location.Y, (float)range);
                if (cp != null)
                    list.Add(cp);
            }
            if (list.Count > 0)
                return list.ToArray();
            return null;
        }

        public ChartPoint[] SelectPointsByX(int indexOfAxisX, int x, int range)
        {
            List<ChartPoint> list = new List<ChartPoint>();
            foreach (SeriesBase series in series)
            {
                if (series.ScalesXAt == indexOfAxisX)
                {
                    ChartPoint cp = series.SelectByX(x, (float)range);
                    if (cp != null)
                    {
                        if (list.Count == 0)
                            list.Add(cp);
                        else if (cp.X == list[0].X)
                            list.Add(cp);
                    }
                }
            }
            if (list.Count > 0)
                return list.ToArray();
            return null;
        }

        public ChartPoint[] SelectPointsByY(int indexOfAxisY, int y, int range)
        {
            List<ChartPoint> list = new List<ChartPoint>();
            foreach (SeriesBase series in series)
            {
                if (series.ScalesYAt == indexOfAxisY)
                {
                    ChartPoint cp = series.SelectByY(y, (float)range);
                    if (cp != null)
                    {
                        if (list.Count == 0)
                            list.Add(cp);
                        else if (cp.Y == list[0].Y)
                            list.Add(cp);
                    }
                }
            }
            if (list.Count > 0)
                return list.ToArray();
            return null;
        }

        public void SetOffset(int offsetX, int offsetY)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            Invalidate();
        }

        public void SetToolTip(string caption)
        {
            MainToolTip.SetToolTip(this, caption);
        }

        public void SetZoomTimes(double times)
        {
            if (times < -2)
                zoomTimes = -2.0;
            else if (times > 0.5)
                zoomTimes = 0.5;
            else
                zoomTimes = times;
            zoomTimes = Math.Round(zoomTimes, 2);
            Invalidate();
        }

        private void UpdateAxisRange()
        {
            foreach (Axis axis in axisX)
            {
                axis.ResetRange();
            }
            foreach (Axis axis in axisY)
            {
                axis.ResetRange();
            }
            foreach (SeriesBase series in series)
            {
                if (series.ScalesXAt >= 0 && series.ScalesXAt < axisX.Count)
                    axisX[series.ScalesXAt].UpdateRange(series, AxisType.X, verticalSeparatorSum);
                if (series.ScalesYAt >= 0 && series.ScalesYAt < axisY.Count)
                    axisY[series.ScalesYAt].UpdateRange(series, AxisType.Y, horizontalSeparatorSum);
            }
            foreach (Axis axis in axisX)
            {
                axis.UpdateZoom(zoomTimes);
            }
            foreach (Axis axis in axisY)
            {
                axis.UpdateZoom(zoomTimes);
            }
        }

        private void UpdateAxisSize()
        {
            foreach (Axis axis in axisX)
            {
                axis.UpdateSize2(border, verticalSeparatorSum);
                axis.UpdateOffset(verticalSeparatorSum, offsetX);
            }
            foreach (Axis axis in axisY)
            {
                axis.UpdateSize2(border, horizontalSeparatorSum);
                axis.UpdateOffset(horizontalSeparatorSum, -offsetY);
            }
        }

        private void UpdateChartBorder(Graphics g, Size size)
        {
            float top = 0f;
            float bottom = 0f;
            float left = 0f;
            float right = 0f;

            foreach (Axis axis in axisX)
            {
                axis.UpdateLabel(verticalSeparatorSum);
                axis.UpdateSize1(g, Font);
                if (axis.Position == AxisPosition.LeftBottom)
                    bottom += axis.Height;
                else
                    top += axis.Height;
            }
            foreach (Axis axis in axisY)
            {
                axis.UpdateLabel(horizontalSeparatorSum);
                axis.UpdateSize1(g, Font);
                if (axis.Position == AxisPosition.LeftBottom)
                    left += axis.Width;
                else
                    right += axis.Width;
            }
            top += Padding.Top;
            bottom += Padding.Bottom;
            left += Padding.Left;
            right += Padding.Right;

            int x = (int)left;
            int y = (int)top;
            int width = (int)(size.Width - left - right);
            int height = (int)(size.Height - top - bottom);

            if (LegendPanel.Visible)
            {
                switch (LegendPanel.Dock)
                {
                    case DockStyle.Top:
                        LegendPanel.Padding = new Padding(x + 3, Padding.Top, (int)(right + 3), 0);
                        y += LegendPanel.Height + 3;
                        height -= LegendPanel.Height + 3;
                        break;
                    case DockStyle.Bottom:
                        LegendPanel.Padding = new Padding(x + 3, 0, (int)(right + 3), Padding.Bottom);
                        height -= LegendPanel.Height + 3;
                        break;
                    case DockStyle.Left:
                        LegendPanel.Padding = new Padding(Padding.Left, y + 3, 0, (int)(bottom + 3));
                        x += LegendPanel.Width + 3;
                        width -= LegendPanel.Height + 3;
                        break;
                    case DockStyle.Right:
                        LegendPanel.Padding = new Padding(0, y + 3, Padding.Right, (int)(bottom + 3));
                        width -= LegendPanel.Width + 3;
                        break;
                    case DockStyle.Fill:
                        break;
                    default:
                        break;
                }
            }
            border = new Rectangle(x, y, width, height);
        }

        #endregion
    }
}
