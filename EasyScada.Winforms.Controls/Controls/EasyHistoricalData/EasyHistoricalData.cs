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
using System.Data.Common;
using System.IO;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyHistoricalDataDesigner))]
    [ToolboxItem(true)]
    public partial class EasyHistoricalData : UserControl, ISupportInitialize
    {
        #region Public properties
        public string Title
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string CustomQuery { get; set; }
        
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public bool UseCustomQuery { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string ReportTitle { get; set; }

        LogProfileCollection _databases = new LogProfileCollection();
        [Browsable(false)]
        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogProfileCollection Databases { get => _databases; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public DataGridView DataGrid { get => dataGrid; }

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
        #endregion

        #region Members
        bool _lockTable;
        DataTable _dataSource;
        #endregion

        public EasyHistoricalData()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                dataGrid.AutoGenerateColumns = true;
                dataGrid.AllowUserToAddRows = false;
                dataGrid.AllowUserToDeleteRows = false;
                dataGrid.AllowUserToOrderColumns = false;

                dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                dataGrid.VirtualMode = true;
                dataGrid.CellValueNeeded += OnCellValueNeeded;
                dataGrid.CellValuePushed += OnCellValuePushed;

                dtpFromDate.ValueChanged += OnSelectTimeChanged;
                dtpFromTime.ValueChanged += OnSelectTimeChanged;
                dtpToDate.ValueChanged += OnSelectTimeChanged;
                dtpToTime.ValueChanged += OnSelectTimeChanged;
                btnExport.Click += OnExecuteReport;
                btnRefresh.Click += OnRefreshView;

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

        #region Event handlers

        private void OnCellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void OnRefreshView(object sender, EventArgs e)
        {
            LogProfile Database = GetDatabase();
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

                    _dataSource = data;
                    this.SetInvoke(x => x.RefreshDataGrid());
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

        private void OnCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_lockTable || _dataSource == null)
                return;

            // If this is the row for new records, no values are needed.
            if (e.RowIndex == dataGrid.RowCount)
                return;

            if (e.RowIndex >= _dataSource.Rows.Count)
                return;

            DataRow row = _dataSource.Rows[e.RowIndex];

            // Set the cell value to paint using the Customer object retrieved.
            e.Value = row[dataGrid.Columns[e.ColumnIndex].HeaderText];
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            dataGrid.BackgroundColor = BackColor;
        }
        #endregion

        #region Methods
        private void RefreshDataGrid()
        {
            dataGrid.Columns.Clear();

            if (_dataSource != null && _dataSource.Columns.Count > 0)
            {
                foreach (DataColumn col in _dataSource.Columns)
                {
                    DataGridViewTextBoxColumn dataGridCol = new DataGridViewTextBoxColumn() { HeaderText = col.ColumnName, ReadOnly = true };
                    dataGrid.Columns.Add(dataGridCol);
                }
            }

            _lockTable = true;
            dataGrid.RowCount += 1;
            _lockTable = false;
            dataGrid.RowCount = _dataSource.Rows.Count;
        }

        public void Export(string path, ExportType exportType)
        {
            LogProfile Database = GetDatabase();
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

        private LogProfile GetDatabase()
        {
            return cobTable.SelectedItem as LogProfile;
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

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                foreach (var item in Databases)
                {
                    cobTable.Items.Add(item);
                }
                cobTable.DisplayMember = "TableName";
                if (Databases.Count > 0)
                    cobTable.SelectedIndex = 0;
            }
        }
        #endregion
    }
}
