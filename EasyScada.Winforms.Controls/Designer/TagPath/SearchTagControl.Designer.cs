namespace EasyScada.Winforms.Controls
{
    partial class SearchTagControl
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
            this.easyPanel1 = new EasyScada.Winforms.Controls.EasyPanel();
            this.gridView = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.colSTT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txbSearch = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnClearSearch = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.timerDelay = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).BeginInit();
            this.easyPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // easyPanel1
            // 
            this.easyPanel1.Controls.Add(this.gridView);
            this.easyPanel1.Controls.Add(this.txbSearch);
            this.easyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel1.Location = new System.Drawing.Point(0, 0);
            this.easyPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.easyPanel1.Name = "easyPanel1";
            this.easyPanel1.Size = new System.Drawing.Size(384, 465);
            this.easyPanel1.TabIndex = 0;
            // 
            // gridView
            // 
            this.gridView.AllowUserToAddRows = false;
            this.gridView.AllowUserToDeleteRows = false;
            this.gridView.AllowUserToResizeRows = false;
            this.gridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSTT,
            this.colName,
            this.colAddress,
            this.colDataType});
            this.gridView.GridStyles.Style = EasyScada.Winforms.Controls.DataGridViewStyle.Mixed;
            this.gridView.GridStyles.StyleBackground = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.gridView.HideOuterBorders = true;
            this.gridView.Location = new System.Drawing.Point(0, 26);
            this.gridView.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.gridView.MultiSelect = false;
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.RowHeadersVisible = false;
            this.gridView.RowHeadersWidth = 25;
            this.gridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridView.Size = new System.Drawing.Size(384, 439);
            this.gridView.TabIndex = 3;
            this.gridView.VirtualMode = true;
            // 
            // colSTT
            // 
            this.colSTT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSTT.FillWeight = 40F;
            this.colSTT.HeaderText = "";
            this.colSTT.MinimumWidth = 25;
            this.colSTT.Name = "colSTT";
            this.colSTT.ReadOnly = true;
            this.colSTT.Width = 25;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colAddress
            // 
            this.colAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAddress.HeaderText = "Address";
            this.colAddress.Name = "colAddress";
            this.colAddress.ReadOnly = true;
            this.colAddress.Width = 78;
            // 
            // colDataType
            // 
            this.colDataType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDataType.HeaderText = "DataType";
            this.colDataType.Name = "colDataType";
            this.colDataType.ReadOnly = true;
            this.colDataType.Width = 84;
            // 
            // txbSearch
            // 
            this.txbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbSearch.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnClearSearch});
            this.txbSearch.Location = new System.Drawing.Point(3, 0);
            this.txbSearch.Name = "txbSearch";
            this.txbSearch.Size = new System.Drawing.Size(378, 23);
            this.txbSearch.TabIndex = 1;
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.ButtonSpec;
            this.btnClearSearch.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnClearSearch.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Close;
            this.btnClearSearch.UniqueName = "E446752422C1428D98AFD47198C5E4B4";
            // 
            // SearchTagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.easyPanel1);
            this.Name = "SearchTagControl";
            this.Size = new System.Drawing.Size(384, 465);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).EndInit();
            this.easyPanel1.ResumeLayout(false);
            this.easyPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private EasyPanel easyPanel1;
        private ThemedTextBox txbSearch;
        private ButtonSpecAny btnClearSearch;
        private EasyDataGridView gridView;
        private System.Windows.Forms.Timer timerDelay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSTT;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDataType;
    }
}
