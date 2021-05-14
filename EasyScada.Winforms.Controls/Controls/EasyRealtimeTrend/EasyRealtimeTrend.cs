using EasyScada.Core;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyRealtimeTrendDesigner))]
    public class EasyRealtimeTrend : PlotView, ISupportConnector, ISupportInitialize
    {
        #region Constructors
        public EasyRealtimeTrend() : base()
        {
        }
        #endregion

        #region Public properties
        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public int UpdateInterval { get; set; } = 100;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public string Title { get; set; } = "Title";

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public string TimeAxisTitle { get; set; } = "Time";

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color TimeAxisTitleColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color TimeAxisLabelColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color TimeAxisColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public string LeftAxisTitle { get; set; } = "Y1";

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color LeftAxisTitleColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color LeftAxisLabelColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color LeftAxisColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public bool LeftAxisVisible { get; set; } = true;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public string RightAxisTitle { get; set; } = "Y2";

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public bool RightAxisVisible { get; set; } = false;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color RightAxisTitleColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color RightAxisLabelColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color RightAxisColor { get; set; } = Color.Black;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public bool VerticalGridLinesVisible { get; set; } = true;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public bool HorizontalGridLinesVisible { get; set; } = true;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public bool LegendVisible { get; set; } = true;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public LegendPlacement LegendPlacement { get; set; } = LegendPlacement.Outside;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public LegendPosition LegendPosition { get; set; } = LegendPosition.RightTop;

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public TimeSpan TimeRange { get; set; } = TimeSpan.FromSeconds(60);

        protected LineDefinitionCollection _lines = new LineDefinitionCollection();
        [Category(DesignerCategory.EASYSCADA), Browsable(true), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineDefinitionCollection Lines { get => _lines; }
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
        #endregion

        #region Members
        DateTimeAxis _timeAxis;
        LinearAxis _valueAxisLeft;
        LinearAxis _valueAxisRight;

        Dictionary<LineDefinition, LineSeries> _seriesDictionary = new Dictionary<LineDefinition, LineSeries>();
        Timer _refreshTimer;
        #endregion

        #region Event handlers
        private void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _refreshTimer.Stop();
            try
            {
                if (Connector.IsStarted)
                {
                    DateTime currentTime = DateTime.Now;
                    DateTime minTime = currentTime.Subtract(TimeRange);

                    lock (Model.SyncRoot)
                    {
                        foreach (var line in Lines)
                        {
                            if (line.LinkedTag != null && line.LinkedTag.Quality == Quality.Good && double.TryParse(line.LinkedTag.Value, out double result))
                                line.Points.Add(new DateTimePoint(result));
                            else
                                line.Points.Add(DateTimePoint.Empty);

                            if (line.LinkedTag != null)
                                _seriesDictionary[line].Title = $"{line.Title}: {line.LinkedTag.Value}";

                            if (line.Points[0].Time < minTime)
                                line.Points.RemoveAt(0);
                        }
                    }

                    this.SetInvoke((x) =>
                    {
                        lock (x.Model.SyncRoot)
                        {
                            x._timeAxis.Minimum = DateTimeAxis.ToDouble(minTime);
                            x._timeAxis.Maximum = DateTimeAxis.ToDouble(currentTime);
                            x.Model.InvalidatePlot(true);
                        }
                    });
                }
            }
            catch { }
            _refreshTimer.Start();
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                Model = new PlotModel();
                Model.Title = Title;
                Model.TitleFontSize = Font.Size;
                Model.TitleColor = ConvertToOxyColor(ForeColor);

                _timeAxis = new DateTimeAxis()
                {
                    FirstDayOfWeek = DayOfWeek.Monday,
                    Position = AxisPosition.Bottom,
                    Title = TimeAxisTitle,
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    FontSize = this.Font.Size,
                    TitleColor = ConvertToOxyColor(TimeAxisTitleColor),
                    AxislineColor = ConvertToOxyColor(TimeAxisColor),
                    TextColor = ConvertToOxyColor(TimeAxisLabelColor),
                };

                if (VerticalGridLinesVisible)
                {
                    _timeAxis.MajorGridlineStyle = LineStyle.LongDash;
                    _timeAxis.MinorGridlineStyle = LineStyle.Dash;
                }

                _valueAxisLeft = new LinearAxis()
                {
                    Title =  LeftAxisTitle,
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    Position = AxisPosition.Left,
                    IsAxisVisible = LeftAxisVisible,
                    FontSize = this.Font.Size,
                    TitleColor = ConvertToOxyColor(LeftAxisTitleColor),
                    AxislineColor = ConvertToOxyColor(LeftAxisColor),
                    TextColor = ConvertToOxyColor(LeftAxisLabelColor),
                };

                _valueAxisRight = new LinearAxis
                {
                    Position = AxisPosition.Right,
                    Title = RightAxisTitle,
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    IsAxisVisible = RightAxisVisible,
                    FontSize = this.Font.Size,
                    TitleColor = ConvertToOxyColor(RightAxisTitleColor),
                    AxislineColor = ConvertToOxyColor(RightAxisColor),
                    TextColor = ConvertToOxyColor(RightAxisLabelColor),
                };

                if (HorizontalGridLinesVisible)
                {
                    _valueAxisLeft.MajorGridlineStyle = LineStyle.LongDash;
                    _valueAxisLeft.MinorGridlineStyle = LineStyle.Dash;

                    _valueAxisRight.MajorGridlineStyle = LineStyle.LongDash;
                    _valueAxisRight.MinorGridlineStyle = LineStyle.Dash;
                }

                Model.Axes.Add(_timeAxis);
                Model.Axes.Add(_valueAxisLeft);
                Model.Axes.Add(_valueAxisRight);

                InitializeSeries();

                Model.IsLegendVisible = LegendVisible;
                Model.LegendPlacement = LegendPlacement;
                Model.LegendPosition = LegendPosition;

                _valueAxisRight.IsAxisVisible = false;
                _refreshTimer = new Timer();
                _refreshTimer.Elapsed += _refreshTimer_Elapsed;
                _refreshTimer.Interval = UpdateInterval;
                _refreshTimer.Start();
            }
        }

        private void InitializeSeries()
        {
            foreach (var item in Lines)
            {
                LineSeries series = new LineSeries()
                {
                    Color = ConvertToOxyColor(item.LineColor),
                    FontSize = item.FontSize,
                    MarkerFill = ConvertToOxyColor(item.MarkerFill),
                    MarkerStroke = ConvertToOxyColor(item.MarkerStroke),
                    MarkerType = item.MarkerType,
                    StrokeThickness = item.StrokeThickness,
                    DataFieldX = "Time",
                    DataFieldY = "Value",
                    ItemsSource = item.Points,
                    BrokenLineStyle = item.EmptyLineStyle,
                    LineLegendPosition = LineLegendPosition.Start,
                    RenderInLegend = item.ShowInLegend,
                    LineStyle = item.LineStyle,
                    Title = item.Title,
                };

                _seriesDictionary[item] = series;
                Model.Series.Add(series);
            }
        }

        private OxyColor ConvertToOxyColor(Color color)
        {
            return OxyColor.FromArgb(color.A, color.R, color.G, color.B);
        }
        #endregion
    }
}
