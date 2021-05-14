using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyScada.Core;
using System.ComponentModel.Design;
using System.Data.Common;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyAlarmViewerDesigner))]
    public partial class EasyAlarmViewer : UserControl
    {
        #region Constructors
        public EasyAlarmViewer()
        {
            InitializeComponent();            
        }
        #endregion

        #region Public properties
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public int MaxDisplayRowCount { get; set; } = 50;

        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public LogProfile Database { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public DataGridView AlarmGrid { get => alarmGrid; }

        int _refreshInterval = 1000;
        public int RefreshInterval
        {
            get => _refreshInterval;
            set
            {
                if (_refreshInterval != value)
                {
                    if (value < 100)
                        _refreshInterval = 100;
                    else
                        _refreshInterval = value;
                }
            }
        }
        #endregion

        #region Members
        System.Timers.Timer _refreshTimer;
        DataTable _alarmSource;
        bool _lockTable;
        AlarmSetting _alarmSetting;
        Dictionary<string, AlarmClass> _alarmClassCache = new Dictionary<string, AlarmClass>();
        #endregion

        #region Event handlers
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                alarmGrid.AllowUserToAddRows = false;
                alarmGrid.AllowUserToDeleteRows = false;
                alarmGrid.AllowUserToOrderColumns = false;

                alarmGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                alarmGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                alarmGrid.VirtualMode = true;
                alarmGrid.CellValueNeeded += AlarmGrid_CellValueNeeded;
                cobMaxDisplayRow.Items.AddRange(new object[] { 10, 50, 100, 200 });
                cobMaxDisplayRow.TextChanged += CobMaxDisplayRow_TextChanged;
                cobMaxDisplayRow.Text = MaxDisplayRowCount.ToString();

                _alarmSetting = DesignerHelper.GetAlarmSetting(Site);
                if (_alarmSetting != null && _alarmSetting.AlarmClasses != null && _alarmSetting.AlarmClasses.Count > 0)
                {
                    foreach (var item in _alarmSetting.AlarmClasses)
                    {
                        _alarmClassCache.Add(item.Name, item);
                    }
                    alarmGrid.CellFormatting += AlarmGrid_CellFormatting;
                }

                if (_refreshTimer == null)
                {
                    _refreshTimer = new System.Timers.Timer();
                    _refreshTimer.Elapsed += RefreshTimer_Elapsed;
                    _refreshTimer.Interval = _refreshInterval;
                    _refreshTimer.Start();
                }
            }
        }

        private void AlarmGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                string headerName = alarmGrid.Columns[e.ColumnIndex].HeaderText;
                if (headerName == "Class" || headerName == "State")
                {
                    DataGridViewRow currentRow = alarmGrid.Rows[e.RowIndex];
                    AlarmClass alarmClass = _alarmClassCache[currentRow.Cells["colClass"].Value?.ToString()];
                    if (alarmClass != null)
                    {
                        string state = currentRow.Cells["colState"].Value?.ToString();
                        switch (state)
                        {
                            case "In":
                                currentRow.DefaultCellStyle.BackColor = alarmClass.GetBackColorIncomming();
                                break;
                            case "Out":
                                currentRow.DefaultCellStyle.BackColor = alarmClass.GetBackColorOutgoing();
                                break;
                            case "Ack":
                                currentRow.DefaultCellStyle.BackColor = alarmClass.GetBackColorAcknowledged();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch { }
        }

        private void CobMaxDisplayRow_TextChanged(object sender, EventArgs e)
        {
            if (uint.TryParse(cobMaxDisplayRow.Text, out uint maxRowCount))
            {
                if (maxRowCount < 500 && maxRowCount > 0)
                    MaxDisplayRowCount = (int)maxRowCount;
            }
        }

        private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _refreshTimer.Stop();
            try
            {
                DataTable alarmData = GetTopAlarmData(MaxDisplayRowCount, "In");
                bool _needUpdate = false;
                if (_alarmSource == null)
                {
                    _alarmSource = alarmData;
                    _needUpdate = true;
                }
                else
                {
                    if (_alarmSource.Rows.Count != alarmData.Rows.Count)
                    {
                        var oldSource = _alarmSource;
                        _alarmSource = alarmData;
                        _needUpdate = true;
                        oldSource.Dispose();
                    }
                    else
                    {
                        if (_alarmSource.Rows.Count > 0)
                        {
                            DataRow sourceRow = _alarmSource.Rows[0];
                            DataRow newRow = alarmData.Rows[0];

                            for (int i = 0; i < _alarmSource.Columns.Count; i++)
                            {
                                if (sourceRow[i]?.ToString() != newRow[i]?.ToString())
                                {
                                    var oldSource = _alarmSource;
                                    _alarmSource = alarmData;
                                    _needUpdate = true;
                                    oldSource.Dispose();
                                    break;
                                }
                            }
                        }
                    }
                }

                if (_needUpdate)
                {
                    this.SetInvoke(x => x.RefreshAlarmGrid());
                }
            }
            catch { }
            finally { _refreshTimer.Start(); }
        }

        private void AlarmGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            
        }

        private void AlarmGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_lockTable || _alarmSource == null)
                return;

            // If this is the row for new records, no values are needed.
            if (e.RowIndex == alarmGrid.RowCount) 
                return;

            if (e.RowIndex >= _alarmSource.Rows.Count)
                return;

            DataRow row = _alarmSource.Rows[e.RowIndex];

            // Set the cell value to paint using the Customer object retrieved.
            switch (alarmGrid.Columns[e.ColumnIndex].HeaderText)
            {
                case "Name":
                    e.Value = row["Name"];
                    break;
                case "Incomming Time":
                    e.Value = row["IncommingTime"];
                    break;
                case "State":
                    e.Value = row["State"];
                    break;
                case "Alarm Text":
                    e.Value = row["AlarmText"];
                    break;
                case "Value":
                    e.Value = row["Value"];
                    break;
                case "Limit":
                    e.Value = row["Limit"];
                    break;
                case "Class":
                    e.Value = row["AlarmClass"];
                    break;
                case "Group":
                    e.Value = row["AlarmGroup"];
                    break;
                case "Tag":
                    e.Value = row["TriggerTag"];
                    break;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            alarmGrid.BackgroundColor = BackColor;
        }

        private void RefreshAlarmGrid()
        {
            _lockTable = true;
            alarmGrid.RowCount += 1;
            _lockTable = false;
            alarmGrid.RowCount = _alarmSource.Rows.Count;
        }
        #endregion

        #region Methods
        public DataTable GetTopAlarmData(int limit = 100, string filterState = "")
        {
            DataTable dt = new DataTable();
            try
            {
                if (Database != null && limit > 0)
                {
                    Database.GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adapter);
                    conn.Open();
                    if (Database.DatabaseType == Core.DbType.MySql)
                    {
                        if (string.IsNullOrEmpty(filterState))
                            cmd.CommandText = $"select * from {Database.TableName} order by IncommingTime desc limit {limit}";
                        else
                            cmd.CommandText = $"select * from {Database.TableName} where State = '{filterState}' order by IncommingTime desc limit {limit}";
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();
                    }
                    else if (Database.DatabaseType == Core.DbType.MSSQL)
                    {
                        if (string.IsNullOrEmpty(filterState))
                            cmd.CommandText = $"select top {limit} * from {Database.TableName} order by IncommingTime desc";
                        else
                            cmd.CommandText = $"select top {limit} * from {Database.TableName} where State = '{filterState}' order by IncommingTime desc";
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();
                    }
                }
            }
            catch { }
            return dt;
        }
        #endregion
    }
}
