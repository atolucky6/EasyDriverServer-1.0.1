namespace EasyScada.Winforms.Controls
{
    partial class ColumnsConfigDesignerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnsConfigDesignerForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnContextCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextCut = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.easyPanel1 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easyPanel2 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easyHeaderGroup2 = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.gridView = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.easyHeaderGroup1 = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.btnAdd = new EasyScada.Winforms.Controls.EasyButton();
            this.btnEdit = new EasyScada.Winforms.Controls.EasyButton();
            this.btnDelete = new EasyScada.Winforms.Controls.EasyButton();
            this.btnSave = new EasyScada.Winforms.Controls.EasyButton();
            this.btnCancel = new EasyScada.Winforms.Controls.EasyButton();
            this.txbDescription = new EasyScada.Winforms.Controls.EasyTextBox();
            this.easyLabel5 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyLabel4 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyLabel3 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyLabel2 = new EasyScada.Winforms.Controls.EasyLabel();
            this.txbDefaultValue = new EasyScada.Winforms.Controls.EasyTextBox();
            this.cobMode = new EasyScada.Winforms.Controls.EasyComboBox();
            this.ckbEnabled = new EasyScada.Winforms.Controls.EasyCheckBox();
            this.txbColumnName = new EasyScada.Winforms.Controls.EasyTextBox();
            this.txbTag = new EasyScada.Winforms.Controls.EasyTextBox();
            this.btnBrowseTagPath = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.colEnabled = new EasyScada.Winforms.Controls.EasyDataGridViewCheckBoxColumn();
            this.colName = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colTriggerTag = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colDefaultValue = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colMode = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colDescription = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).BeginInit();
            this.easyPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel2)).BeginInit();
            this.easyPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2.Panel)).BeginInit();
            this.easyHeaderGroup2.Panel.SuspendLayout();
            this.easyHeaderGroup2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1.Panel)).BeginInit();
            this.easyHeaderGroup1.Panel.SuspendLayout();
            this.easyHeaderGroup1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobMode)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.btnContextCopy,
            this.btnContextCut,
            this.btnContextPaste,
            this.btnContextDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 98);
            // 
            // btnContextCopy
            // 
            this.btnContextCopy.Name = "btnContextCopy";
            this.btnContextCopy.Size = new System.Drawing.Size(107, 22);
            this.btnContextCopy.Text = "Copy";
            // 
            // btnContextCut
            // 
            this.btnContextCut.Name = "btnContextCut";
            this.btnContextCut.Size = new System.Drawing.Size(107, 22);
            this.btnContextCut.Text = "Cut";
            // 
            // btnContextPaste
            // 
            this.btnContextPaste.Name = "btnContextPaste";
            this.btnContextPaste.Size = new System.Drawing.Size(107, 22);
            this.btnContextPaste.Text = "Paste";
            // 
            // btnContextDelete
            // 
            this.btnContextDelete.Name = "btnContextDelete";
            this.btnContextDelete.Size = new System.Drawing.Size(107, 22);
            this.btnContextDelete.Text = "Delete";
            // 
            // easyPanel1
            // 
            this.easyPanel1.Controls.Add(this.easyPanel2);
            this.easyPanel1.Controls.Add(this.easyHeaderGroup1);
            this.easyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel1.Location = new System.Drawing.Point(0, 0);
            this.easyPanel1.Name = "easyPanel1";
            this.easyPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel1.Size = new System.Drawing.Size(772, 626);
            this.easyPanel1.TabIndex = 1;
            // 
            // easyPanel2
            // 
            this.easyPanel2.Controls.Add(this.easyHeaderGroup2);
            this.easyPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel2.Location = new System.Drawing.Point(4, 188);
            this.easyPanel2.Name = "easyPanel2";
            this.easyPanel2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.easyPanel2.Size = new System.Drawing.Size(764, 434);
            this.easyPanel2.TabIndex = 1;
            // 
            // easyHeaderGroup2
            // 
            this.easyHeaderGroup2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyHeaderGroup2.HeaderVisibleSecondary = false;
            this.easyHeaderGroup2.Location = new System.Drawing.Point(0, 4);
            this.easyHeaderGroup2.Name = "easyHeaderGroup2";
            // 
            // easyHeaderGroup2.Panel
            // 
            this.easyHeaderGroup2.Panel.Controls.Add(this.gridView);
            this.easyHeaderGroup2.Size = new System.Drawing.Size(764, 430);
            this.easyHeaderGroup2.TabIndex = 0;
            this.easyHeaderGroup2.ValuesPrimary.Heading = "Columns Collection";
            this.easyHeaderGroup2.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("easyHeaderGroup2.ValuesPrimary.Image")));
            // 
            // gridView
            // 
            this.gridView.AllowUserToAddRows = false;
            this.gridView.AllowUserToDeleteRows = false;
            this.gridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEnabled,
            this.colName,
            this.colTriggerTag,
            this.colDefaultValue,
            this.colMode,
            this.colDescription});
            this.gridView.ContextMenuStrip = this.contextMenuStrip1;
            this.gridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView.GridStyles.Style = EasyScada.Winforms.Controls.DataGridViewStyle.Mixed;
            this.gridView.GridStyles.StyleBackground = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.gridView.HideOuterBorders = true;
            this.gridView.Location = new System.Drawing.Point(0, 0);
            this.gridView.MultiSelect = false;
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.RowHeadersVisible = false;
            this.gridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.gridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridView.Size = new System.Drawing.Size(762, 398);
            this.gridView.TabIndex = 25;
            // 
            // easyHeaderGroup1
            // 
            this.easyHeaderGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.easyHeaderGroup1.HeaderVisibleSecondary = false;
            this.easyHeaderGroup1.Location = new System.Drawing.Point(4, 4);
            this.easyHeaderGroup1.Name = "easyHeaderGroup1";
            // 
            // easyHeaderGroup1.Panel
            // 
            this.easyHeaderGroup1.Panel.Controls.Add(this.btnAdd);
            this.easyHeaderGroup1.Panel.Controls.Add(this.btnEdit);
            this.easyHeaderGroup1.Panel.Controls.Add(this.btnDelete);
            this.easyHeaderGroup1.Panel.Controls.Add(this.btnSave);
            this.easyHeaderGroup1.Panel.Controls.Add(this.btnCancel);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbDescription);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel5);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel4);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel3);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel2);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbDefaultValue);
            this.easyHeaderGroup1.Panel.Controls.Add(this.cobMode);
            this.easyHeaderGroup1.Panel.Controls.Add(this.ckbEnabled);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbColumnName);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbTag);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel1);
            this.easyHeaderGroup1.Size = new System.Drawing.Size(764, 184);
            this.easyHeaderGroup1.TabIndex = 0;
            this.easyHeaderGroup1.ValuesPrimary.Heading = "Column properties";
            this.easyHeaderGroup1.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("easyHeaderGroup1.ValuesPrimary.Image")));
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(371, 120);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 25);
            this.btnAdd.TabIndex = 15;
            this.btnAdd.Values.Text = "Add";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(449, 120);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(72, 25);
            this.btnEdit.TabIndex = 14;
            this.btnEdit.Values.Text = "Edit";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(527, 120);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 25);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Values.Text = "Delete";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(605, 120);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 25);
            this.btnSave.TabIndex = 12;
            this.btnSave.Values.Text = "Save";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(683, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 25);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Values.Text = "Cancel";
            // 
            // txbDescription
            // 
            this.txbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbDescription.Location = new System.Drawing.Point(371, 65);
            this.txbDescription.Multiline = true;
            this.txbDescription.Name = "txbDescription";
            this.txbDescription.Size = new System.Drawing.Size(384, 49);
            this.txbDescription.TabIndex = 10;
            // 
            // easyLabel5
            // 
            this.easyLabel5.Location = new System.Drawing.Point(290, 65);
            this.easyLabel5.Name = "easyLabel5";
            this.easyLabel5.Size = new System.Drawing.Size(75, 20);
            this.easyLabel5.TabIndex = 9;
            this.easyLabel5.Values.Text = "Description:";
            // 
            // easyLabel4
            // 
            this.easyLabel4.Location = new System.Drawing.Point(7, 93);
            this.easyLabel4.Name = "easyLabel4";
            this.easyLabel4.Size = new System.Drawing.Size(45, 20);
            this.easyLabel4.TabIndex = 8;
            this.easyLabel4.Values.Text = "Mode:";
            // 
            // easyLabel3
            // 
            this.easyLabel3.Location = new System.Drawing.Point(7, 65);
            this.easyLabel3.Name = "easyLabel3";
            this.easyLabel3.Size = new System.Drawing.Size(85, 20);
            this.easyLabel3.TabIndex = 7;
            this.easyLabel3.Values.Text = "Default value:";
            // 
            // easyLabel2
            // 
            this.easyLabel2.Location = new System.Drawing.Point(290, 36);
            this.easyLabel2.Name = "easyLabel2";
            this.easyLabel2.Size = new System.Drawing.Size(33, 20);
            this.easyLabel2.TabIndex = 6;
            this.easyLabel2.Values.Text = "Tag:";
            // 
            // txbDefaultValue
            // 
            this.txbDefaultValue.Location = new System.Drawing.Point(98, 64);
            this.txbDefaultValue.Name = "txbDefaultValue";
            this.txbDefaultValue.Size = new System.Drawing.Size(186, 23);
            this.txbDefaultValue.TabIndex = 5;
            // 
            // cobMode
            // 
            this.cobMode.DropDownWidth = 186;
            this.cobMode.Location = new System.Drawing.Point(98, 93);
            this.cobMode.Name = "cobMode";
            this.cobMode.Size = new System.Drawing.Size(186, 21);
            this.cobMode.TabIndex = 4;
            // 
            // ckbEnabled
            // 
            this.ckbEnabled.Location = new System.Drawing.Point(98, 9);
            this.ckbEnabled.Name = "ckbEnabled";
            this.ckbEnabled.Size = new System.Drawing.Size(67, 20);
            this.ckbEnabled.TabIndex = 3;
            this.ckbEnabled.Values.Text = "Enabled";
            // 
            // txbColumnName
            // 
            this.txbColumnName.Location = new System.Drawing.Point(98, 35);
            this.txbColumnName.Name = "txbColumnName";
            this.txbColumnName.Size = new System.Drawing.Size(186, 23);
            this.txbColumnName.TabIndex = 2;
            // 
            // txbTag
            // 
            this.txbTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTag.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseTagPath});
            this.txbTag.Location = new System.Drawing.Point(371, 34);
            this.txbTag.Name = "txbTag";
            this.txbTag.Size = new System.Drawing.Size(384, 24);
            this.txbTag.TabIndex = 1;
            // 
            // btnBrowseTagPath
            // 
            this.btnBrowseTagPath.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseTagPath.Text = "...";
            this.btnBrowseTagPath.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseTagPath.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseTagPath.UniqueName = "9BE5762D890B4A8A338BD53854A497CC";
            // 
            // easyLabel1
            // 
            this.easyLabel1.Location = new System.Drawing.Point(7, 36);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Size = new System.Drawing.Size(89, 20);
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.Values.Text = "Column name:";
            // 
            // colEnabled
            // 
            this.colEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colEnabled.DataPropertyName = "Enabled";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = false;
            this.colEnabled.DefaultCellStyle = dataGridViewCellStyle1;
            this.colEnabled.FalseValue = null;
            this.colEnabled.HeaderText = "Enabled";
            this.colEnabled.IndeterminateValue = null;
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.ReadOnly = true;
            this.colEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colEnabled.TrueValue = null;
            this.colEnabled.Width = 59;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colName.DataPropertyName = "ColumnName";
            this.colName.HeaderText = "Column Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.Width = 114;
            // 
            // colTriggerTag
            // 
            this.colTriggerTag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTriggerTag.DataPropertyName = "TagPath";
            this.colTriggerTag.HeaderText = "Tag";
            this.colTriggerTag.Name = "colTriggerTag";
            this.colTriggerTag.ReadOnly = true;
            this.colTriggerTag.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTriggerTag.Width = 54;
            // 
            // colDefaultValue
            // 
            this.colDefaultValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDefaultValue.DataPropertyName = "DefaultValue";
            this.colDefaultValue.HeaderText = "Default Value";
            this.colDefaultValue.Name = "colDefaultValue";
            this.colDefaultValue.ReadOnly = true;
            this.colDefaultValue.Width = 105;
            // 
            // colMode
            // 
            this.colMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colMode.DataPropertyName = "Mode";
            this.colMode.HeaderText = "Mode";
            this.colMode.Name = "colMode";
            this.colMode.ReadOnly = true;
            this.colMode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colMode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMode.Width = 48;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescription.DataPropertyName = "Description";
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 381;
            // 
            // ColumnsConfigDesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 626);
            this.Controls.Add(this.easyPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ColumnsConfigDesignerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Columns Configuration";
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).EndInit();
            this.easyPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel2)).EndInit();
            this.easyPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2.Panel)).EndInit();
            this.easyHeaderGroup2.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2)).EndInit();
            this.easyHeaderGroup2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1.Panel)).EndInit();
            this.easyHeaderGroup1.Panel.ResumeLayout(false);
            this.easyHeaderGroup1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1)).EndInit();
            this.easyHeaderGroup1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cobMode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem btnContextDelete;
        private System.Windows.Forms.ToolStripMenuItem btnContextPaste;
        private System.Windows.Forms.ToolStripMenuItem btnContextCut;
        private System.Windows.Forms.ToolStripMenuItem btnContextCopy;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private EasyPanel easyPanel1;
        private EasyPanel easyPanel2;
        private EasyHeaderGroup easyHeaderGroup2;
        private EasyDataGridView gridView;
        private EasyHeaderGroup easyHeaderGroup1;
        private EasyButton btnCancel;
        private EasyTextBox txbDescription;
        private EasyLabel easyLabel5;
        private EasyLabel easyLabel4;
        private EasyLabel easyLabel3;
        private EasyLabel easyLabel2;
        private EasyTextBox txbDefaultValue;
        private EasyComboBox cobMode;
        private EasyCheckBox ckbEnabled;
        private EasyTextBox txbColumnName;
        private EasyTextBox txbTag;
        private ButtonSpecAny btnBrowseTagPath;
        private EasyLabel easyLabel1;
        private EasyButton btnAdd;
        private EasyButton btnEdit;
        private EasyButton btnDelete;
        private EasyButton btnSave;
        private EasyDataGridViewCheckBoxColumn colEnabled;
        private EasyDataGridViewTextBoxColumn colName;
        private EasyDataGridViewTextBoxColumn colTriggerTag;
        private EasyDataGridViewTextBoxColumn colDefaultValue;
        private EasyDataGridViewTextBoxColumn colMode;
        private EasyDataGridViewTextBoxColumn colDescription;
    }
}