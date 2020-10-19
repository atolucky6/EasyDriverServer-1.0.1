namespace EasyScada.Winforms.Controls
{
    partial class EasyAlarmViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EasyAlarmViewer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.easyDataGridView1 = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.colName = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colIncommingTime = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colState = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAlarmText = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colValue = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colLimit = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colCompareMode = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colOutgoingTime = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAckTime = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAlarmType = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAlarmClass = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAlarmGroup = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cSVFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xLSXFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.btnRefresh,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.toolStripTextBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1064, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // easyDataGridView1
            // 
            this.easyDataGridView1.AllowUserToAddRows = false;
            this.easyDataGridView1.AllowUserToDeleteRows = false;
            this.easyDataGridView1.AllowUserToOrderColumns = true;
            this.easyDataGridView1.AllowUserToResizeRows = false;
            this.easyDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.easyDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.easyDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colIncommingTime,
            this.colState,
            this.colAlarmText,
            this.colValue,
            this.colLimit,
            this.colCompareMode,
            this.colOutgoingTime,
            this.colAckTime,
            this.colAlarmType,
            this.colAlarmClass,
            this.colAlarmGroup});
            this.easyDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyDataGridView1.GridStyles.Style = EasyScada.Winforms.Controls.DataGridViewStyle.Mixed;
            this.easyDataGridView1.GridStyles.StyleBackground = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.easyDataGridView1.Location = new System.Drawing.Point(0, 27);
            this.easyDataGridView1.Name = "easyDataGridView1";
            this.easyDataGridView1.ReadOnly = true;
            this.easyDataGridView1.RowHeadersVisible = false;
            this.easyDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.easyDataGridView1.Size = new System.Drawing.Size(1064, 616);
            this.easyDataGridView1.TabIndex = 2;
            this.easyDataGridView1.VirtualMode = true;
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 68;
            // 
            // colIncommingTime
            // 
            this.colIncommingTime.HeaderText = "Incomming time";
            this.colIncommingTime.Name = "colIncommingTime";
            this.colIncommingTime.ReadOnly = true;
            this.colIncommingTime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colIncommingTime.Width = 125;
            // 
            // colState
            // 
            this.colState.HeaderText = "State";
            this.colState.Name = "colState";
            this.colState.ReadOnly = true;
            this.colState.Width = 62;
            // 
            // colAlarmText
            // 
            this.colAlarmText.HeaderText = "Alarm text";
            this.colAlarmText.Name = "colAlarmText";
            this.colAlarmText.ReadOnly = true;
            this.colAlarmText.Width = 91;
            // 
            // colValue
            // 
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Width = 64;
            // 
            // colLimit
            // 
            this.colLimit.HeaderText = "Limit";
            this.colLimit.Name = "colLimit";
            this.colLimit.ReadOnly = true;
            this.colLimit.Width = 63;
            // 
            // colCompareMode
            // 
            this.colCompareMode.HeaderText = "Compare mode";
            this.colCompareMode.Name = "colCompareMode";
            this.colCompareMode.ReadOnly = true;
            this.colCompareMode.Width = 119;
            // 
            // colOutgoingTime
            // 
            this.colOutgoingTime.HeaderText = "Outgoing time";
            this.colOutgoingTime.Name = "colOutgoingTime";
            this.colOutgoingTime.ReadOnly = true;
            this.colOutgoingTime.Width = 114;
            // 
            // colAckTime
            // 
            this.colAckTime.HeaderText = "Ack time";
            this.colAckTime.Name = "colAckTime";
            this.colAckTime.ReadOnly = true;
            this.colAckTime.Width = 83;
            // 
            // colAlarmType
            // 
            this.colAlarmType.HeaderText = "Alarm type";
            this.colAlarmType.Name = "colAlarmType";
            this.colAlarmType.ReadOnly = true;
            this.colAlarmType.Width = 94;
            // 
            // colAlarmClass
            // 
            this.colAlarmClass.HeaderText = "Class";
            this.colAlarmClass.Name = "colAlarmClass";
            this.colAlarmClass.ReadOnly = true;
            this.colAlarmClass.Width = 63;
            // 
            // colAlarmGroup
            // 
            this.colAlarmGroup.HeaderText = "Group";
            this.colAlarmGroup.Name = "colAlarmGroup";
            this.colAlarmGroup.ReadOnly = true;
            this.colAlarmGroup.Width = 69;
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 24);
            this.btnRefresh.Text = "toolStripButton1";
            this.btnRefresh.ToolTipText = "Refresh";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cSVFileToolStripMenuItem,
            this.xLSXFileToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // cSVFileToolStripMenuItem
            // 
            this.cSVFileToolStripMenuItem.Name = "cSVFileToolStripMenuItem";
            this.cSVFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cSVFileToolStripMenuItem.Text = "CSV file";
            // 
            // xLSXFileToolStripMenuItem
            // 
            this.xLSXFileToolStripMenuItem.Name = "xLSXFileToolStripMenuItem";
            this.xLSXFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.xLSXFileToolStripMenuItem.Text = "XLSX file";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(69, 24);
            this.toolStripLabel1.Text = "Display top:";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.AutoSize = false;
            this.toolStripTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            // 
            // EasyAlarmViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.easyDataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "EasyAlarmViewer";
            this.Size = new System.Drawing.Size(1064, 643);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private EasyDataGridView easyDataGridView1;
        private EasyDataGridViewTextBoxColumn colName;
        private EasyDataGridViewTextBoxColumn colIncommingTime;
        private EasyDataGridViewTextBoxColumn colState;
        private EasyDataGridViewTextBoxColumn colAlarmText;
        private EasyDataGridViewTextBoxColumn colValue;
        private EasyDataGridViewTextBoxColumn colLimit;
        private EasyDataGridViewTextBoxColumn colCompareMode;
        private EasyDataGridViewTextBoxColumn colOutgoingTime;
        private EasyDataGridViewTextBoxColumn colAckTime;
        private EasyDataGridViewTextBoxColumn colAlarmType;
        private EasyDataGridViewTextBoxColumn colAlarmClass;
        private EasyDataGridViewTextBoxColumn colAlarmGroup;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem cSVFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xLSXFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
    }
}
