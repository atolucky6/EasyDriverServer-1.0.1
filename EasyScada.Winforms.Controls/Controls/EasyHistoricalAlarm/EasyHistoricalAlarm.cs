using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyHistoricalAlarmDesigner))]
    [ToolboxItem(true)]
    public partial class EasyHistoricalAlarm : UserControl
    {
        #region Public properties
        public string Title
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string ReportTitle { get; set; } = "History Alarm";

        [Browsable(false)]
        public LogProfile Database { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public DataGridView AlarmGrid { get => alarmGrid; }
        #endregion

        #region Members
        AlarmSetting _alarmSetting;
        bool _lockTable;
        DataTable _alarmSource;
        Dictionary<string, AlarmClass> _alarmClassCache = new Dictionary<string, AlarmClass>();
        #endregion

        public EasyHistoricalAlarm()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                alarmGrid.AllowUserToAddRows = false;
                alarmGrid.AllowUserToDeleteRows = false;
                alarmGrid.AllowUserToOrderColumns = false;

                alarmGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                alarmGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                alarmGrid.VirtualMode = true;
                alarmGrid.CellValueNeeded += AlarmGrid_CellValueNeeded;
                alarmGrid.CellValuePushed += AlarmGrid_CellValuePushed;

                alarmGrid.CellContentClick += AlarmGrid_CellContentClick;

                dtpFromDate.ValueChanged += OnSelectTimeChanged;
                dtpFromTime.ValueChanged += OnSelectTimeChanged;
                dtpToDate.ValueChanged += OnSelectTimeChanged;
                dtpToTime.ValueChanged += OnSelectTimeChanged;
                btnExport.Click += OnExecuteReport;
                btnRefresh.Click += OnRefreshView;
                btnAckAll.Click += BtnAckAll_Click;
                btnChangeAckComment.Click += BtnChangeAckComment_Click;

                dtpFromDate.Value = DateTime.Now;
                dtpFromTime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                dtpToDate.Value = DateTime.Now;
                dtpToTime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                foreach (var item in Enum.GetValues(typeof(ExportType)))
                {
                    cobType.Items.Add(item.ToString());
                }
                cobType.SelectedIndex = 0;

                _alarmSetting = DesignerHelper.GetAlarmSetting(null);

                if (_alarmSetting != null)
                {
                    if (_alarmSetting.AlarmClasses != null && _alarmSetting.AlarmClasses.Count > 0)
                    {
                        foreach (var item in _alarmSetting.AlarmClasses)
                        {
                            _alarmClassCache.Add(item.Name, item);
                        }
                        alarmGrid.CellFormatting += AlarmGrid_CellFormatting;
                    }

                    cobAlarmClass.Items.Add("All");
                    foreach (var item in _alarmSetting.AlarmClasses)
                    {
                        cobAlarmClass.Items.Add(item.Name);
                    }
                    cobAlarmClass.SelectedIndex = 0;

                    cobAlarmState.Items.Add("All");
                    cobAlarmState.Items.Add("In");
                    cobAlarmState.Items.Add("Out");
                    cobAlarmState.Items.Add("Ack");
                    cobAlarmState.SelectedIndex = 0;

                    cobAlarmType.Items.Add("All");
                    cobAlarmType.Items.Add("DiscreteAlarm");
                    cobAlarmType.Items.Add("AnalogAlarm");
                    cobAlarmType.Items.Add("QualityAlarm");
                    cobAlarmType.SelectedIndex = 0;

                    cobFindBy.Items.Add("IncommingTime");
                    cobFindBy.Items.Add("OutgoingTime");
                    cobFindBy.Items.Add("AckTime");
                    cobFindBy.SelectedIndex = 0;
                }
            }
        }

        #region Event handlers
        private void BtnAckAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (_alarmSource != null && _alarmSource.Rows.Count > 0)
                {
                    AckAlarmComfirm form = new AckAlarmComfirm();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        int result = EasyAlarmLogger.AckAll(new LogProfileCollection() { Database }, cobFindBy.Text, GetFromTime(), GetToTime(), form.Comment);
                        if (result > 0)
                        {
                            OnRefreshView(null, null);
                        }
                    }
                }
            }
            catch { }
        }

        private void BtnChangeAckComment_Click(object sender, EventArgs e)
        {
            if (alarmGrid.SelectedCells.Count > 0)
            {
                AckAlarmComfirm form = new AckAlarmComfirm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (DataGridViewCell cell in alarmGrid.SelectedCells)
                    {
                        DataGridViewRow row = cell.OwningRow;
                        if (row != null && row.Cells["colState"].Value?.ToString() == "Ack")
                        {

                            string alarmName = row.Cells["colName"].Value?.ToString();
                            string alarmType = row.Cells["colAlarmType"].Value?.ToString();
                            string inTimeStr = row.Cells["colInTime"].Value?.ToString();
                            if (DateTime.TryParse(inTimeStr, out DateTime inTime))
                            {
                                int result = EasyAlarmLogger.ChangeAckComment(new LogProfileCollection() { Database }, alarmName, alarmType, inTime, form.Comment);
                                if (result > 0)
                                {
                                    DataRow dtRow = _alarmSource.Rows[cell.RowIndex];
                                    dtRow["State"] = "Ack";
                                    dtRow["AckTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    dtRow["AckComment"] = form.Comment;
                                    alarmGrid.InvalidateRow(cell.RowIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AlarmGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _alarmSource.Rows.Count)
            {
                if (alarmGrid.Columns[e.ColumnIndex].HeaderText == "Action")
                {
                    DataGridViewRow row = alarmGrid.Rows[e.RowIndex];
                    if (row != null && row.Cells["colState"].Value?.ToString() == "Out")
                    {
                        AckAlarmComfirm form = new AckAlarmComfirm();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            string alarmName = row.Cells["colName"].Value?.ToString();
                            string alarmType = row.Cells["colAlarmType"].Value?.ToString();
                            string inTimeStr = row.Cells["colInTime"].Value?.ToString();
                            if (DateTime.TryParse(inTimeStr, out DateTime inTime))
                            {
                                int result = EasyAlarmLogger.AckAlarmItem(new LogProfileCollection() { Database }, alarmName, alarmType, inTime, form.Comment);
                                if (result > 0)
                                {
                                    DataRow dtRow = _alarmSource.Rows[e.RowIndex];
                                    dtRow["State"] = "Ack";
                                    dtRow["AckTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    dtRow["AckComment"] = form.Comment;
                                    alarmGrid.InvalidateRow(e.RowIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AlarmGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void OnRefreshView(object sender, EventArgs e)
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
                    string filterTime = cobFindBy.Text;

                    string where = $"where {filterTime} >= '{GetFromTime():yyyy-MM-dd HH:mm:ss}' and {filterTime} <= '{GetToTime():yyyy-MM-dd HH:mm:ss}'";

                    if (cobAlarmClass.Text != "All")
                    {
                        where += $" and AlarmClass = '{cobAlarmClass.Text}'";
                    }

                    if (cobAlarmState.Text != "All")
                    {
                        where += $" and State = '{cobAlarmState.Text}'";
                    }

                    if (cobAlarmType.Text != "All")
                    {
                        where += $" and AlarmType = '{cobAlarmType.Text}'";
                    }

                    selectQuery = Database.GetSelectQuery(where);

                    cmd.CommandText = selectQuery;
                    adp.SelectCommand = cmd;
                    DataTable data = new DataTable();
                    adp.Fill(data);

                    DataTable alarmData = data;
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

        private void OnSelectTimeChanged(object sender, EventArgs e)
        {
            if (GetFromTime() <= GetToTime())
                btnExport.Enabled = true;
            else
                btnExport.Enabled = false;
        }

        private void OnExecuteReport(object sender, EventArgs e)
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
                case "Outgoing Time":
                    e.Value = row["OutgoingTime"];
                    break;
                case "Ack Time":
                    e.Value = row["AckTime"];
                    break;
                case "Class":
                    e.Value = row["AlarmClass"];
                    break;
                case "Type":
                    e.Value = row["AlarmType"];
                    break;
                case "Group":
                    e.Value = row["AlarmGroup"];
                    break;
                case "Tag":
                    e.Value = row["TriggerTag"];
                    break;
                case "Ack Comment":
                    if (_alarmSource.Columns.Contains("AckComment"))
                        e.Value = row["AckComment"];
                    break;
                case "Action":
                    if (row["State"].ToString() == "Out")
                    {
                        e.Value = "Ack";
                    }
                    else
                    {
                        e.Value = "";
                    }
                    break;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            alarmGrid.BackgroundColor = BackColor;
        }
        #endregion

        #region Methods
        private void RefreshAlarmGrid()
        {
            _lockTable = true;
            alarmGrid.RowCount += 1;
            _lockTable = false;
            alarmGrid.RowCount = _alarmSource.Rows.Count;
        }

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
                    string filterTime = cobFindBy.Text;

                    string where = $"where {filterTime} >= '{GetFromTime():yyyy-MM-dd HH:mm:ss}' and {filterTime} <= '{GetToTime():yyyy-MM-dd HH:mm:ss}'";

                    if (cobAlarmClass.Text != "All")
                    {
                        where += $" and AlarmClass = '{cobAlarmClass.Text}'";
                    }

                    if (cobAlarmState.Text != "All")
                    {
                        where += $" and State = '{cobAlarmState.Text}'";
                    }

                    if (cobAlarmType.Text != "All")
                    {
                        where += $" and AlarmType = '{cobAlarmType.Text}'";
                    }

                    selectQuery = Database.GetSelectQuery(where);

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
                    conn.Close();
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
        #endregion
    }
}
