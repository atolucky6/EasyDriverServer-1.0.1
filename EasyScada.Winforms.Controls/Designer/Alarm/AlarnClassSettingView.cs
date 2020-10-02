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
using System.IO;

namespace EasyScada.Winforms.Controls
{
    public partial class AlarnClassSettingView : UserControl
    {
        #region Constructors
        public AlarnClassSettingView()
        {
            InitializeComponent();
            btnImport.Click += BtnImport_Click;
            btnExport.Click += BtnExport_Click;
            btnCopy.Click += BtnCopy_Click;
            btnCut.Click += BtnCut_Click;
            btnPaste.Click += BtnPaste_Click;
            btnDelete.Click += BtnDelete_Click;
            btnContextCopy.Click += BtnCopy_Click;
            btnContextCut.Click += BtnCut_Click;
            btnContextPaste.Click += BtnPaste_Click;
            btnContextDelete.Click += BtnDelete_Click;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;

            gridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            gridView.UserDeletingRow += GridView_UserDeletingRow;
            gridView.CellValidating += GridView_CellValidating;
            gridView.SelectionChanged += GridView_SelectionChanged;
            gridView.CellEndEdit += GridView_CellEndEdit;
            gridView.CellBeginEdit += GridView_CellBeginEdit;
            gridView.KeyUp += GridView_KeyUp;
            gridView.AllowUserToAddRows = true;
            gridView.AllowUserToDeleteRows = true;
            gridView.ShowCellErrors = true;
            gridView.CellValueChanged += GridView_CellValueChanged;
            gridView.RowsAdded += GridView_RowsAdded;
            Load += OnLoaded;
            for (int i = 0; i < 10; i++)
            {
                gridView.Rows.Add($"Alarm_Group_{i + 1}", "#FF0000", "#FFFFFF", "#FF00FF", "");
                //gridView.Rows[gridView.Rows.Count - 2].ReadOnly = false;
            }
        }

        private void GridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                var row = gridView.Rows[e.RowIndex];
                if (!row.IsNewRow)
                {
                    Color color = ColorTranslator.FromHtml(row.Cells[colBackColorOut.Index].Value?.ToString());
                    row.Cells[colBackColorOut.Index].Style.BackColor = color;
                    row.Cells[colBackColorOut.Index].Style.SelectionBackColor = color;

                    color = ColorTranslator.FromHtml(row.Cells[colBackColorIn.Index].Value?.ToString());
                    row.Cells[colBackColorIn.Index].Style.BackColor = color;
                    row.Cells[colBackColorIn.Index].Style.SelectionBackColor = color;

                    color = ColorTranslator.FromHtml(row.Cells[colBackColorAck.Index].Value?.ToString());
                    row.Cells[colBackColorAck.Index].Style.BackColor = color;
                    row.Cells[colBackColorAck.Index].Style.SelectionBackColor = color;
                }
            }
            catch { }
        }
        #endregion

        #region Public properties
        public AlarmSetting AlarmSetting { get; set; }
        #endregion

        #region Methods
        #endregion

        #region Event handlers
        private void OnLoaded(object sender, EventArgs e)
        {
            if (gridView.CanPaste())
            {
                btnPaste.Enabled = true;
                btnContextPaste.Enabled = true;
            }
            else
            {
                btnPaste.Enabled = false;
                btnContextPaste.Enabled = false;
            }
        }

        private void GridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
            {
                BtnCut_Click(btnCut, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                BtnPaste_Click(btnPaste, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                if (gridView.CanPaste())
                {
                    btnPaste.Enabled = true;
                    btnContextPaste.Enabled = true;
                }
                else
                {
                    btnPaste.Enabled = false;
                    btnContextPaste.Enabled = false;
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (gridView.CanPaste())
            {
                btnPaste.Enabled = true;
                btnContextPaste.Enabled = true;
            }
            else
            {
                btnPaste.Enabled = false;
                btnContextPaste.Enabled = false;
            }
        }

        private void GridView_SelectionChanged(object sender, EventArgs e)
        {
            if (gridView.SelectedRows.Count > 0 &&
                !gridView.SelectedRows[0].IsNewRow)
            {
                btnCopy.Enabled = true;
                btnCut.Enabled = true;
                btnDelete.Enabled = true;

                btnContextCopy.Enabled = true;
                btnContextCut.Enabled = true;
                btnContextDelete.Enabled = true;
            }
            else
            {
                btnCopy.Enabled = false;
                btnCut.Enabled = false;
                btnDelete.Enabled = false;

                btnContextCopy.Enabled = false;
                btnContextCut.Enabled = false;
                btnContextDelete.Enabled = false;
            }
        }

        private void GridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var column = gridView.Columns[e.ColumnIndex];
            if (column != null)
            {
                if (column.HeaderText == "Name")
                {
                    string errorText = "";
                    string currentName = e.FormattedValue?.ToString();
                    if (!string.IsNullOrEmpty(currentName))
                    {
                        bool isUnique = true;
                        for (int i = 0; i < gridView.Rows.Count; i++)
                        {
                            if (i == e.RowIndex)
                                continue;
                            if (currentName == gridView.Rows[i].Cells[e.ColumnIndex].Value?.ToString())
                            {
                                isUnique = false;
                                break;
                            }
                        }
                        if (isUnique)
                        {
                            gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = null;
                            toolTip1.Hide(this);
                            e.Cancel = false;
                            return;
                        }
                        else
                        {
                            errorText = $"The alarm group with this name '{currentName}' is already exists.";
                        }
                    }
                    else
                    {
                        errorText = "The alarm group name can't be empty.";
                    }

                    if (gridView.Rows[e.RowIndex].IsNewRow)
                    {
                        gridView.CancelEdit();
                        return;
                    }

                    if (gridView.EditingControl is EasyDataGridViewTextBoxEditingControl editControl)
                    {
                        editControl.StateCommon.Back.Color1 = Color.FromArgb(247, 198, 198);
                        toolTip1.Show(errorText, gridView.EditingControl, 0, editControl.Height + 3);
                        e.Cancel = true;
                    }
                }
            }
        }

        private void GridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            toolTip1.Hide(this);
            var row = gridView.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            cell.Value = cell.Value?.ToString().Trim();

            if (row.IsNewRow)
            {
                if (!string.IsNullOrEmpty(row.Cells["colName"].Value?.ToString()))
                {
                    gridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    gridView.NotifyCurrentCellDirty(true);
                }
            }
        }

        private void GridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = gridView.Rows[e.RowIndex].ReadOnly;
            DataGridViewRow row = gridView.Rows[e.RowIndex];

            if (row.IsNewRow)
            {
                string name = gridView.GetUniqueNameInDataGridView("Alarm_Class_1", out int colNameIndex);
                if (colNameIndex > -1 && string.IsNullOrWhiteSpace(gridView.Rows[e.RowIndex].Cells[colNameIndex].Value?.ToString()))
                {
                    gridView.Rows[e.RowIndex].Cells[colNameIndex].Value = name;
                    gridView.Rows[e.RowIndex].Cells["colBackColorIn"].Value = "#FF0000";
                    gridView.Rows[e.RowIndex].Cells["colBackColorOut"].Value = "#FFFFFF";
                    gridView.Rows[e.RowIndex].Cells["colBackColorAck"].Value = "#FFFFFF";
                }
            }
            else
            {
                if (e.ColumnIndex == colBackColorOut.Index ||
                   e.ColumnIndex == colBackColorIn.Index ||
                   e.ColumnIndex == colBackColorAck.Index)
                {
                    if (colorDialog1.ShowDialog() == DialogResult.OK)
                    {
                        gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = colorDialog1.Color;
                        gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = colorDialog1.Color;
                        gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = $"#{colorDialog1.Color.R.ToString("X2")}{colorDialog1.Color.G.ToString("X2")}{colorDialog1.Color.B.ToString("X2")}";
                    }
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void GridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var row = gridView.Rows[e.RowIndex];
                if (!row.IsNewRow)
                {
                    if (e.ColumnIndex == colBackColorOut.Index ||
                       e.ColumnIndex == colBackColorIn.Index ||
                       e.ColumnIndex == colBackColorAck.Index)
                    {
                        Color color = ColorTranslator.FromHtml(row.Cells[e.ColumnIndex].Value?.ToString());
                        row.Cells[e.ColumnIndex].Style.BackColor = color;
                        row.Cells[e.ColumnIndex].Style.SelectionBackColor = color;
                    }
                }
            }
            catch { }
        }

        private void GridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = true;
            if (gridView.SelectedRows.Count > 0 && gridView.SelectedRows.Contains(e.Row))
            {
                var mbr = MessageBox.Show("Do you want to delete all selected objects ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in gridView.SelectedRows)
                        selectedRows.Add(row);
                    selectedRows.ForEach(x =>
                    {
                        if (!x.IsNewRow && !x.ReadOnly)
                            gridView.Rows.Remove(x);
                    });
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gridView.SelectedRows.Count > 0)
            {
                var mbr = MessageBox.Show("Do you want to delete all selected objects ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in gridView.SelectedRows)
                        selectedRows.Add(row);
                    selectedRows.ForEach(x =>
                    {
                        if (!x.IsNewRow && !x.ReadOnly)
                            gridView.Rows.Remove(x);
                    });
                }
            }
        }

        private void BtnPaste_Click(object sender, EventArgs e)
        {
            gridView.Paste();
        }

        private void BtnCut_Click(object sender, EventArgs e)
        {
            gridView.CopySelectedRowToClipboard();
            if (gridView.CanPaste())
            {
                btnPaste.Enabled = true;
                btnContextPaste.Enabled = true;
            }
            else
            {
                btnPaste.Enabled = false;
                btnContextPaste.Enabled = false;
            }
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in gridView.SelectedRows)
                selectedRows.Add(row);
            selectedRows.ForEach(x =>
            {
                if (!x.IsNewRow && !x.ReadOnly)
                    gridView.Rows.Remove(x);
            });
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            gridView.CopySelectedRowToClipboard();
            if (gridView.CanPaste())
            {
                btnPaste.Enabled = true;
                btnContextPaste.Enabled = true;
            }
            else
            {
                btnPaste.Enabled = false;
                btnContextPaste.Enabled = false;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "CSV File (*.csv)|*.csv";
                saveFileDialog1.Title = "Export";
                saveFileDialog1.FileName = "Alarm_Class";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog1.FileName;

                    CsvBuilder builder = new CsvBuilder();
                    builder
                        .AddColumn("Name")
                        .AddColumn("BackgroundColorIncoming")
                        .AddColumn("BackgroundColorOutgoing")
                        .AddColumn("BackgroundColorAcknowledged")
                        .AddColumn("Description");
                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        builder.AddRow(
                            row.Cells["colName"].Value?.ToString(), 
                            row.Cells["colBackColorIn"].Value?.ToString(),
                            row.Cells["colBackColorOut"].Value?.ToString(),
                            row.Cells["colBackColorAck"].Value?.ToString(),
                            row.Cells["colDescription"].Value?.ToString());
                    }
                    File.WriteAllText(filePath, builder.ToString());
                }
            }
            catch
            {
                MessageBox.Show($"Can't export to file '{saveFileDialog1.FileName}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "CSV File (*.csv)|*.csv";
                openFileDialog1.Title = "Import";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(openFileDialog1.FileName);
                    if (lines.Length > 1)
                    {
                        string[] columns = lines[0].Split(',');
                        if (columns.Length == 5)
                        {
                            int colNameIndex = -1;
                            int colDescriptionIndex = -1;
                            int colBackColorInIndex = -1;
                            int colBackColorOutIndex = -1;
                            int colBackColorAckIndex = -1;
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (columns[i] == "Name")
                                    colNameIndex = i;
                                else if (columns[i] == "BackgroundColorIncoming")
                                    colBackColorInIndex = i;
                                else if (columns[i] == "BackgroundColorOutgoing")
                                    colBackColorOutIndex = i;
                                else if (columns[i] == "BackgroundColorAcknowledged")
                                    colBackColorAckIndex = i;
                                else if (columns[i] == "Description")
                                    colDescriptionIndex = i;
                            }
                            if (colNameIndex > -1 && colDescriptionIndex > -1)
                            {
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    string[] rowValues = lines[i].Split(',');
                                    if (rowValues.Length == 5)
                                    {
                                        string name = gridView.GetUniqueNameInDataGridView(rowValues[colNameIndex], out int nameIndex);
                                        gridView.Rows.Add(
                                            name, 
                                            rowValues[colBackColorInIndex],
                                            rowValues[colBackColorOutIndex],
                                            rowValues[colBackColorAckIndex],
                                            rowValues[colDescriptionIndex]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Can't import file '{openFileDialog1.FileName}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
