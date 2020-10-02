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
using System.Text.RegularExpressions;

namespace EasyScada.Winforms.Controls
{
    public partial class ColumnsConfigDesignerForm : EasyForm
    {
        #region Inner
        private enum EditState
        {
            None,
            Add,
            Edit
        }
        #endregion

        #region Constructors
        public ColumnsConfigDesignerForm(LogColumnCollection columnsSource, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceProvider = serviceProvider;
            this.columnsSource = columnsSource;
            Columns = new BindingList<LogColumn>(columnsSource);
            gridView.AutoGenerateColumns = false;
            gridView.DataSource = Columns;

            gridView.SelectionChanged += GridView_SelectionChanged;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            btnBrowseTagPath.Click += BtnBrowseTagPath_Click;

            foreach (var item in Enum.GetValues(typeof(LogColumnMode)))
                cobMode.Items.Add(item.ToString());

            UpdateButtons();
        }

        #endregion

        #region Public properties
        private LogColumn editItem;
        private LogColumnCollection columnsSource;
        private EditState state;
        private EditState State
        {
            get => state;
            set
            {
                state = value;
                UpdateButtons();
            }
        }
        public BindingList<LogColumn> Columns { get; set; }
        private LogColumn selectedItem;
        public LogColumn SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                DisplayLogColumnProperties(value);
            }
        }
        public IServiceProvider ServiceProvider { get; set; }
        #endregion

        #region Event handlers
        private void BtnBrowseTagPath_Click(object sender, EventArgs e)
        {
            if (State != EditState.None)
            {
                SelectTagPathDesignerForm form = new SelectTagPathDesignerForm(ServiceProvider);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    txbTag.Text = form.SelectedTagPath;
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            State = EditState.None;
            DisplayLogColumnProperties(SelectedItem);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Regex.IsMatch(txbColumnName.Text?.Trim(), $"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                {
                    MessageBox.Show("The column name was not in correct format.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txbColumnName.Focus();
                }

                Enum.TryParse(cobMode.Text, out LogColumnMode colMode);
                editItem.Enabled = ckbEnabled.Checked;
                editItem.ColumnName = txbColumnName.Text?.Trim();
                editItem.DefaultValue = txbDefaultValue.Text?.Trim();
                editItem.Description = txbDescription.Text?.Trim();
                editItem.Mode = colMode;
                editItem.TagPath = txbTag.Text?.Trim();

                if (State == EditState.Add)
                {
                    //columnsSource.Add(editItem);
                    Columns.Add(editItem);
                }

                gridView.ClearSelection();
                State = EditState.None;
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (row.DataBoundItem == editItem)
                        row.Selected = true;
                }
                editItem = null;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (SelectedItem != null)
            {
                var mbr = MessageBox.Show("Do you want to delete selected object ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    Columns.Remove(SelectedItem);
                }
                UpdateButtons();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            State = EditState.Edit;
            editItem = SelectedItem;
            txbColumnName.Focus();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            State = EditState.Add;
            editItem = new LogColumn() { Enabled = true };
            DisplayLogColumnProperties(editItem);
            txbColumnName.Focus();
        }

        private void GridView_SelectionChanged(object sender, EventArgs e)
        {
            if (gridView.SelectedRows.Count > 0)
            {
                SelectedItem = gridView.SelectedRows[0].DataBoundItem as LogColumn;
            }
            else
                SelectedItem = null;
        }
        #endregion

        #region Methods
        private void DisplayLogColumnProperties(LogColumn column)
        {
            if (column != null && State == EditState.None)
            {
                ckbEnabled.Checked = column.Enabled;
                txbColumnName.Text = column.ColumnName?.Trim();
                txbDefaultValue.Text = column.DefaultValue;
                txbDescription.Text = column.Description;
                txbTag.Text = column.TagPath;
                cobMode.Text = column.Mode.ToString();
            }
            else
            {
                if (editItem != null)
                {
                    ckbEnabled.Checked = editItem.Enabled;
                    txbColumnName.Text = editItem.ColumnName?.Trim();
                    txbDefaultValue.Text = editItem.DefaultValue;
                    txbDescription.Text = editItem.Description;
                    txbTag.Text = editItem.TagPath;
                    cobMode.Text = editItem.Mode.ToString();
                }
            }
        }

        private void UpdateButtons()
        {
            switch (State)
            {
                case EditState.None:
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = SelectedItem != null;
                    btnDelete.Enabled = selectedItem != null;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    break;
                case EditState.Add:
                case EditState.Edit:
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
