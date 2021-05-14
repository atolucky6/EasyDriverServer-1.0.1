using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using OxyPlot;
using EasyScada.Core;
using System.Drawing.Design;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel.Design;
using System.IO;
using System.Diagnostics;
using System.Data.Common;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyHistoricalTrendDesigner))]
    [ToolboxItem(true)]
    public partial class EasyHistoricalTrend : UserControl, ISupportInitialize
    {
        public static Color GetRandomColor()
        {
            if (ColorIndex >= ColorPalette.Count)
                ColorIndex = 0;

            var color = ColorPalette[ColorIndex];
            ColorIndex++;
            return color;
        }
        public static int ColorIndex = 0;
        public static List<Color> ColorPalette = new List<Color>();
        static EasyHistoricalTrend()
        {
            ColorPalette.Add(Color.Red);
            ColorPalette.Add(Color.Blue);
            ColorPalette.Add(Color.Green);
            ColorPalette.Add(Color.Black);
            ColorPalette.Add(Color.Orange);
            ColorPalette.Add(Color.Purple);
            ColorPalette.Add(Color.Yellow);
            ColorPalette.Add(Color.Violet);
            ColorPalette.Add(Color.Lime);
            ColorPalette.Add(Color.Linen);
            ColorPalette.Add(Color.Maroon);
            ColorPalette.Add(Color.MediumSpringGreen);
            ColorPalette.Add(Color.MintCream);
            ColorPalette.Add(Color.PaleTurquoise);
            ColorPalette.Add(Color.Pink);
            ColorPalette.Add(Color.PaleVioletRed);
            ColorPalette.Add(Color.Firebrick);
        }

        public EasyHistoricalTrend()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                dtpFromDate.ValueChanged += OnSelectTimeChanged;
                dtpFromTime.ValueChanged += OnSelectTimeChanged;
                dtpToDate.ValueChanged += OnSelectTimeChanged;
                dtpToTime.ValueChanged += OnSelectTimeChanged;
                btnRefresh.Click += BtnRefresh_Click;
                btnExport.Click += BtnExport_Click;
                btnReset.Click += BtnReset_Click;

                dtpFromDate.Value = DateTime.Now;
                dtpFromTime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                dtpToDate.Value = DateTime.Now;
                dtpToTime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                foreach (var item in Enum.GetValues(typeof(ExportType)))
                {
                    cobType.Items.Add(item.ToString());
                }
                cobType.SelectedIndex = 0;
            }
        }

        #region Public properties
        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public string Title { get => groupBox1.Text; set => groupBox1.Text = value; }

        [Category(DesignerCategory.EASYSCADA), Browsable(true)]
        public Color BackColorTrend { get => plotView1.BackColor; set => plotView1.BackColor = value; }

        public string ReportTitle { get; set; } = "Report";

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
        public string TimeAxisFieldName { get; set; } = "DateTime";

        [Browsable(false)]
        public LogProfile Database { get; set; }

        protected LineDefinitionCollection _lines = new LineDefinitionCollection();
        [Category(DesignerCategory.EASYSCADA), Browsable(true), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineDefinitionCollection Lines { get => _lines; }

        List<string> _columns = new List<string>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public List<string> Columns
        {
            get => _columns;
            set
            {
                if (_columns != value)
                {
                    _columns = value;
                }
            }
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string CustomQuery { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public bool UseCustomQuery { get; set; }
        #endregion

        #region ISupportConnector
        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();
        #endregion

        #region Members
        DateTimeAxis _timeAxis;
        LinearAxis _valueAxisLeft;
        Dictionary<LineDefinition, LineSeries> _seriesDictionary = new Dictionary<LineDefinition, LineSeries>();
        #endregion

        #region Event handlers
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Title = $"Export to {cobType.Text}";
                saveFileDialog1.Filter = $"{cobType.Text} file|*.{cobType.Text.ToLower()}";
                saveFileDialog1.FileName = string.Empty;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ExportType type = (ExportType)Enum.Parse(typeof(ExportType), cobType.Text);
                    Export(saveFileDialog1.FileName, type);

                    if (File.Exists(saveFileDialog1.FileName))
                    {
                        var mbr = MessageBox.Show($"Export successfully! Do you want to open it ?", "Message", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (mbr == DialogResult.Yes)
                        {
                            Process.Start(saveFileDialog1.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Export failed.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTrend();
        }

        private void OnSelectTimeChanged(object sender, EventArgs e)
        {
            if (GetFromTime() <= GetToTime())
                btnExport.Enabled = true;
            else
                btnExport.Enabled = false;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _timeAxis.Reset();
            _valueAxisLeft.Reset();
            plotView1.InvalidatePlot(true);
        }
        #endregion

        #region Methods
        public void Export(string path, ExportType exportType)
        {
            if (Database == null)
            {
                MessageBox.Show($"Database or table doesn't exists.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Database.GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adp, true);
                try
                {
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    string selectQuery = null;
                    if (UseCustomQuery)
                    {
                        selectQuery = CustomQuery;
                    }
                    else
                    {
                        string where = $"where DateTime >= '{GetFromTime():yyyy-MM-dd HH:mm:ss}' and DateTime <= '{GetToTime():yyyy-MM-dd HH:mm:ss} ordery by DateTime asc'";
                        if (_columns.Count == 0)
                        {
                            selectQuery = Database.GetSelectQuery(where);
                        }
                        else
                        {
                            selectQuery = Database.GetSelectQuery(_columns.ToArray(), where);
                        }
                    }

                    cmd.CommandText = selectQuery;
                    adp.SelectCommand = cmd;
                    DataTable data = new DataTable();
                    adp.Fill(data);
                    Export(data, path, exportType);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Dispose();
                    cmd.Dispose();
                    adp.Dispose();
                }
            }
        }

        public void Export(DataTable data, string path, ExportType type)
        {
            try
            {
                string reportTime = $"From: {GetFromTime():dd/MM/yyyy HH:mm:ss} - To: {GetToTime():dd/MM/yyyy HH:mm:ss}";

                if (type == ExportType.Csv)
                {
                    ReportHelper.ExportToCsv(data, path);
                }
                else if (type == ExportType.Xlsx)
                {
                    ReportHelper.ExportToXlsx(data, path, ReportTitle, reportTime);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime GetFromTime()
        {
            return new DateTime(dtpFromDate.Value.Year, dtpFromDate.Value.Month, dtpFromDate.Value.Day, dtpFromTime.Value.Hour, dtpFromTime.Value.Minute, dtpFromTime.Value.Second);
        }

        private DateTime GetToTime()
        {
            return new DateTime(dtpToDate.Value.Year, dtpToDate.Value.Month, dtpToDate.Value.Day, dtpToTime.Value.Hour, dtpToTime.Value.Minute, dtpToTime.Value.Second);
        }

        private void RefreshTrend()
        {
            try
            {
                Database.GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adp, true);
                try
                {
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    string selectQuery = null;
                    DateTime fromTime = GetFromTime();
                    DateTime toTime = GetToTime();
                    if (UseCustomQuery)
                    {
                        selectQuery = CustomQuery;
                    }
                    else
                    {
                        string where = $"where DateTime >= '{fromTime:yyyy-MM-dd HH:mm:ss}' and DateTime <= '{toTime:yyyy-MM-dd HH:mm:ss} order by DateTime asc'";
                        if (_columns.Count == 0)
                        {
                            selectQuery = Database.GetSelectQuery(where);
                        }
                        else
                        {
                            selectQuery = Database.GetSelectQuery(_columns.ToArray(), where);
                        }
                    }

                    cmd.CommandText = selectQuery;
                    adp.SelectCommand = cmd;
                    DataTable data = new DataTable();
                    adp.Fill(data);

                    foreach (LineDefinition line in Lines)
                    {
                        line.Points.Clear();
                    }

                    if (data.Columns.Count > 0)
                    {
                        if (Lines.Count == 0)
                        {
                            foreach (DataColumn col in data.Columns)
                            {
                                if (col.ColumnName != TimeAxisFieldName)
                                {
                                    LineDefinition line = new LineDefinition()
                                    {
                                        ColumnName = col.ColumnName,
                                        LineColor = GetRandomColor(),
                                        Title = col.ColumnName,
                                        
                                    };
                                    Lines.Add(line);
                                }
                            }
                            InitializeSeries();
                        }

                        bool containTimeColumns = data.Columns.Contains(TimeAxisFieldName);
                        if (containTimeColumns)
                        {
                            if (data.Rows.Count > 0)
                            {
                                foreach (DataRow row in data.Rows)
                                {
                                    if (DateTime.TryParse(row[TimeAxisFieldName].ToString(), out DateTime time))
                                    {
                                        foreach (LineDefinition line in Lines)
                                        {
                                            if (data.Columns.Contains(line.ColumnName))
                                            {
                                                if (double.TryParse(row[line.ColumnName].ToString(), out double value))
                                                {
                                                    line.Points.Add(new DateTimePoint(time, value));
                                                }
                                                else
                                                {
                                                    line.Points.Add(DateTimePoint.Empty);
                                                }
                                            }

                                            if (_seriesDictionary.ContainsKey(line))
                                                _seriesDictionary[line].Title = $"{line.Title}";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    this.SetInvoke((x) =>
                    {
                        lock (x.plotView1.Model.SyncRoot)
                        {
                            x._timeAxis.Minimum = DateTimeAxis.ToDouble(fromTime);
                            x._timeAxis.Maximum = DateTimeAxis.ToDouble(toTime);
                            x._timeAxis.AbsoluteMinimum = DateTimeAxis.ToDouble(fromTime);
                            x._timeAxis.AbsoluteMaximum = DateTimeAxis.ToDouble(toTime);
                            x._timeAxis.Reset();
                            x._valueAxisLeft.Reset();
                            x.plotView1.InvalidatePlot(true);
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                    adp.Dispose();
                }
            }
            catch { }
        }


        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                plotView1.Model = new PlotModel();
                plotView1.Model.Title = Title;
                plotView1.Model.TitleFontSize = Font.Size;
                plotView1.Model.TitleColor = ConvertToOxyColor(ForeColor);

                _timeAxis = new DateTimeAxis()
                {
                    FirstDayOfWeek = DayOfWeek.Monday,
                    Position = AxisPosition.Bottom,
                    Title = TimeAxisTitle,
                    IsPanEnabled = true,
                    IsZoomEnabled = true,
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
                    Title = LeftAxisTitle,
                    IsPanEnabled = true,
                    IsZoomEnabled = false,
                    Position = AxisPosition.Left,
                    IsAxisVisible = LeftAxisVisible,
                    FontSize = this.Font.Size,
                    TitleColor = ConvertToOxyColor(LeftAxisTitleColor),
                    AxislineColor = ConvertToOxyColor(LeftAxisColor),
                    TextColor = ConvertToOxyColor(LeftAxisLabelColor),
                };


                if (HorizontalGridLinesVisible)
                {
                    _valueAxisLeft.MajorGridlineStyle = LineStyle.LongDash;
                    _valueAxisLeft.MinorGridlineStyle = LineStyle.Dash;
                }

                plotView1.Model.Axes.Add(_timeAxis);
                plotView1.Model.Axes.Add(_valueAxisLeft);

                InitializeSeries();

                plotView1.Model.IsLegendVisible = LegendVisible;
                plotView1.Model.LegendPlacement = LegendPlacement;
                plotView1.Model.LegendPosition = LegendPosition;
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
                plotView1.Model.Series.Add(series);
            }
        }

        private OxyColor ConvertToOxyColor(Color color)
        {
            return OxyColor.FromArgb(color.A, color.R, color.G, color.B);
        }
        #endregion
    }
}
