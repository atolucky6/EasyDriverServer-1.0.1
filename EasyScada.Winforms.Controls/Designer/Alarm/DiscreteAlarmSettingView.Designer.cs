﻿namespace EasyScada.Winforms.Controls
{
    partial class DiscreteAlarmSettingView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnImport = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnCut = new System.Windows.Forms.ToolStripButton();
            this.btnPaste = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.gridView = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.colName = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAlarmText = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAlarmClass = new EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn();
            this.colAlarmGroup = new EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn();
            this.colTriggerTag = new EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn();
            this.colTriggerValue = new EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn();
            this.colDescription = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnContextCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextCut = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnBrowserTag1 = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.btnBrowserTag2 = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(8);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImport,
            this.btnExport,
            this.toolStripSeparator2,
            this.btnCopy,
            this.btnCut,
            this.btnPaste,
            this.btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1138, 29);
            this.toolStrip1.TabIndex = 12;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnImport
            // 
            this.btnImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(23, 22);
            this.btnImport.Text = "toolStripButton1";
            this.btnImport.ToolTipText = "Import";
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(23, 22);
            this.btnExport.Text = "toolStripButton2";
            this.btnExport.ToolTipText = "Export";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(23, 22);
            this.btnCopy.Text = "toolStripButton5";
            this.btnCopy.ToolTipText = "Copy";
            // 
            // btnCut
            // 
            this.btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(23, 22);
            this.btnCut.Text = "toolStripButton6";
            this.btnCut.ToolTipText = "Cut";
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(23, 22);
            this.btnPaste.Text = "toolStripButton7";
            this.btnPaste.ToolTipText = "Paste";
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "toolStripButton8";
            this.btnDelete.ToolTipText = "Delete";
            // 
            // gridView
            // 
            this.gridView.AllowUserToOrderColumns = true;
            this.gridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colAlarmText,
            this.colAlarmClass,
            this.colAlarmGroup,
            this.colTriggerTag,
            this.colTriggerValue,
            this.colDescription});
            this.gridView.ContextMenuStrip = this.contextMenuStrip1;
            this.gridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView.Location = new System.Drawing.Point(0, 29);
            this.gridView.Name = "gridView";
            this.gridView.PaletteMode = EasyScada.Winforms.Controls.PaletteMode.Office2010Silver;
            this.gridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.gridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridView.Size = new System.Drawing.Size(1138, 681);
            this.gridView.TabIndex = 14;
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.Width = 68;
            // 
            // colAlarmText
            // 
            this.colAlarmText.HeaderText = "Alarm Text";
            this.colAlarmText.Name = "colAlarmText";
            this.colAlarmText.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colAlarmText.Width = 92;
            // 
            // colAlarmClass
            // 
            this.colAlarmClass.DropDownWidth = 121;
            this.colAlarmClass.HeaderText = "Alarm class";
            this.colAlarmClass.Name = "colAlarmClass";
            this.colAlarmClass.Width = 77;
            // 
            // colAlarmGroup
            // 
            this.colAlarmGroup.DropDownHeight = 300;
            this.colAlarmGroup.DropDownWidth = 400;
            this.colAlarmGroup.HeaderText = "Alarm group";
            this.colAlarmGroup.MaxDropDownItems = 12;
            this.colAlarmGroup.Name = "colAlarmGroup";
            this.colAlarmGroup.Width = 84;
            // 
            // colTriggerTag
            // 
            this.colTriggerTag.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.colTriggerTag.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.colTriggerTag.DropDownHeight = 300;
            this.colTriggerTag.DropDownWidth = 400;
            this.colTriggerTag.HeaderText = "Trigger Tag";
            this.colTriggerTag.MaxDropDownItems = 12;
            this.colTriggerTag.Name = "colTriggerTag";
            this.colTriggerTag.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTriggerTag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colTriggerTag.Width = 93;
            // 
            // colTriggerValue
            // 
            this.colTriggerValue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.colTriggerValue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.colTriggerValue.DropDownHeight = 300;
            this.colTriggerValue.DropDownWidth = 400;
            this.colTriggerValue.HeaderText = "Trigger Value";
            this.colTriggerValue.MaxDropDownItems = 12;
            this.colTriggerValue.Name = "colTriggerValue";
            this.colTriggerValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTriggerValue.Width = 84;
            // 
            // colDescription
            // 
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Width = 96;
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.toolTip1.InitialDelay = 0;
            this.toolTip1.ReshowDelay = 100;
            // 
            // btnBrowserTag1
            // 
            this.btnBrowserTag1.Text = "...";
            this.btnBrowserTag1.UniqueName = "84B7BBE0BF2C4D91548C30362ACB3317";
            // 
            // btnBrowserTag2
            // 
            this.btnBrowserTag2.Text = "...";
            this.btnBrowserTag2.UniqueName = "8701FE19830C40C0789020E52F5785D1";
            // 
            // DiscreteAlarmSettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DiscreteAlarmSettingView";
            this.Size = new System.Drawing.Size(1138, 710);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnImport;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnCut;
        private System.Windows.Forms.ToolStripButton btnPaste;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private EasyScada.Winforms.Controls.EasyDataGridView gridView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem btnContextCopy;
        private System.Windows.Forms.ToolStripMenuItem btnContextCut;
        private System.Windows.Forms.ToolStripMenuItem btnContextPaste;
        private System.Windows.Forms.ToolStripMenuItem btnContextDelete;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private EasyScada.Winforms.Controls.ButtonSpecAny btnBrowserTag1;
        private EasyScada.Winforms.Controls.ButtonSpecAny btnBrowserTag2;
        private EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn colName;
        private EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn colAlarmText;
        private EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn colAlarmClass;
        private EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn colAlarmGroup;
        private EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn colTriggerTag;
        private EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn colTriggerValue;
        private EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn colDescription;
    }
}