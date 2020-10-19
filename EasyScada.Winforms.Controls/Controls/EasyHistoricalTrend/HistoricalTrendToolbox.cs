using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EasyScada.Core;
using EasyScada.Winforms.Controls.Charts;

namespace EasyScada.Winforms.Controls
{
    public partial class HistoricalTrendToolbox : UserControl, ISupportInitialize
    {
        #region Constructors
        public HistoricalTrendToolbox()
        {
            InitializeComponent();
        }
        #endregion

        #region Public properties

        private HistoricalTrend _HistoricalTrend;
        public HistoricalTrend HistoricalTrend
        {
            get => _HistoricalTrend;
            set
            {
                if (_HistoricalTrend != value)
                {
                    _HistoricalTrend = value;
                }
            }
        }

        protected bool IsBusy { get; set; }

        protected LogProfile Database => HistoricalTrend?.Database;

        protected TrendLineCollection Lines => HistoricalTrend?.Lines;

        #endregion

        #region Methods
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            //HistoricalTrend.SetScaleByXAxis(trackBar.Value / 10.0f);    _HistoricalTrend.RenderCurveUI();
            _HistoricalTrend.RenderCurveUI();

            btnComfirm.Click += BtnComfirm_Click;
        }

        public void Reload()
        {
            IsBusy = true;
            Database.GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adp, true);
            try
            {
                if (_HistoricalTrend != null && Database != null && Lines != null && Lines.Count > 0)
                {
                    _HistoricalTrend.Text = "Loading...";
                    _HistoricalTrend.RemoveAllCurve();
                    List<string> columns = new List<string>();
                    columns.Add("DateTime");
                    columns.AddRange(Lines.Where(x => x.Enabled && !string.IsNullOrEmpty(x.ColumnName)).Select(x => x.ColumnName).ToArray());
                    if (columns.Count > 0)
                    {
                        string from = $"{dtpFromDate.Value.ToString("yyyy-MM-dd")} {dtpFromTime.Value.ToString("HH:mm:ss")}";
                        string to = $"{dtpToDate.Value.ToString("yyyy-MM-dd")} {dtpToTime.Value.ToString("HH:mm:ss")}";

                        conn.Open();
                        cmd.CommandText = Database.GetSelectQuery(columns.ToArray(), $"where DateTime > '{from}' and DateTime < '{to}';");
                        DataTable dt = new DataTable();
                        adp.SelectCommand = cmd;
                        adp.Fill(dt);

                        List<float[]> datas = new List<float[]>();
                        DateTime[] times = new DateTime[dt.Rows.Count];
                        for (int i = 0; i < dt.Columns.Count - 1; i++)
                            datas.Add(new float[dt.Rows.Count]);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var currentRow = dt.Rows[i];
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                try
                                {
                                    if (j == 0)
                                        times[i] = Convert.ToDateTime(currentRow[j]);
                                    else
                                        datas[j - 1][i] = Convert.ToSingle(currentRow[j]);
                                }
                                catch { }
                            }
                        }

                        new Thread(new ThreadStart(() =>
                        {
                            Invoke(new System.Action(() =>
                            {
                                float maxLeft = _HistoricalTrend.LeftAxisMaxValue;
                                float minLeft = _HistoricalTrend.LeftAxisMinValue;
                                float maxRight = _HistoricalTrend.RightAxisMaxValue;
                                float minRight = _HistoricalTrend.RightAxisMinValue;
                                for (int i = 1; i < dt.Columns.Count; i++)
                                {
                                    TrendLine line = Lines.FirstOrDefault(x => x.ColumnName == dt.Columns[i].ColumnName);
                                    if (line != null)
                                    {
                                        switch (line.Alignment)
                                        {
                                            case TrendLineAlignment.Left:
                                                if (datas[i - 1].Any())
                                                {
                                                    var max = datas[i - 1].Max();
                                                    if (max > maxLeft)
                                                        maxLeft = max;

                                                    var min = datas[i - 1].Min();
                                                    if (min < minLeft)
                                                        minLeft = min;
                                                }
                                                _HistoricalTrend.SetLeftCurve(line.ColumnName, datas[i - 1], line.Color, false);
                                                break;
                                            case TrendLineAlignment.Right:
                                                if (datas[i - 1].Any())
                                                {
                                                    var maxr = datas[i - 1].Max();
                                                    if (maxr > maxRight)
                                                        maxRight = maxr;

                                                    var minr = datas[i - 1].Min();
                                                    if (minr < minRight)
                                                        minRight = minr;
                                                }
                                                _HistoricalTrend.SetRightCurve(line.ColumnName, datas[i - 1], line.Color, false);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }

                                _HistoricalTrend.LeftAxisMaxValue = maxLeft;
                                _HistoricalTrend.LeftAxisMinValue = minLeft;
                                _HistoricalTrend.RightAxisMaxValue = maxRight;
                                _HistoricalTrend.RightAxisMinValue = minRight;
                                _HistoricalTrend.RoundAxisValue = true;
                                _HistoricalTrend.SetDateTimes(times);
                                _HistoricalTrend.SetScaleByXAxis(1.0f);
                                _HistoricalTrend.RenderCurveUI();
                                HistoricalTrend.Focus();
                            }));
                        }))
                        { IsBackground = true }.Start();
                    }
                }
            }
            catch { }
            finally
            {
                _HistoricalTrend.Text = "";
                conn.Dispose();
                cmd.Dispose();
                adp.Dispose();
                IsBusy = false;
            }
        }

        private void BtnComfirm_Click(object sender, System.EventArgs e)
        {
            Reload();
        }
        #endregion
    }
}
