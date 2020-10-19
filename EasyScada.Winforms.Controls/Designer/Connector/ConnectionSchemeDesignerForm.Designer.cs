namespace EasyScada.Winforms.Controls
{
    partial class ConnectionSchemeDesignerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionSchemeDesignerForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.easyPanel1 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easySplitContainer1 = new EasyScada.Winforms.Controls.EasySplitContainer();
            this.easyHeaderGroup3 = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.easyLabel4 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel3 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.cobCommunicationMode = new EasyScada.Winforms.Controls.EasyComboBox();
            this.txbServerAddress = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.easyLabel2 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel1 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.txbPort = new EasyScada.Winforms.Controls.EasyNumericUpDown();
            this.txbRefreshRate = new EasyScada.Winforms.Controls.EasyNumericUpDown();
            this.easySplitContainer2 = new EasyScada.Winforms.Controls.EasySplitContainer();
            this.groupProjectTree = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.projectTree = new EasyScada.Winforms.Controls.HieraticalTreeView();
            this.groupTagCollection = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.searchTagControl = new EasyScada.Winforms.Controls.SearchTagControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getConnectionSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnGetConnectionSchema = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).BeginInit();
            this.easyPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel1)).BeginInit();
            this.easySplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel2)).BeginInit();
            this.easySplitContainer1.Panel2.SuspendLayout();
            this.easySplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup3.Panel)).BeginInit();
            this.easyHeaderGroup3.Panel.SuspendLayout();
            this.easyHeaderGroup3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCommunicationMode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer2.Panel1)).BeginInit();
            this.easySplitContainer2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer2.Panel2)).BeginInit();
            this.easySplitContainer2.Panel2.SuspendLayout();
            this.easySplitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree.Panel)).BeginInit();
            this.groupProjectTree.Panel.SuspendLayout();
            this.groupProjectTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection.Panel)).BeginInit();
            this.groupTagCollection.Panel.SuspendLayout();
            this.groupTagCollection.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.easyPanel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1030, 592);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1030, 641);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // easyPanel1
            // 
            this.easyPanel1.Controls.Add(this.easySplitContainer1);
            this.easyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel1.Location = new System.Drawing.Point(0, 0);
            this.easyPanel1.Name = "easyPanel1";
            this.easyPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel1.Size = new System.Drawing.Size(1030, 592);
            this.easyPanel1.TabIndex = 0;
            // 
            // easySplitContainer1
            // 
            this.easySplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.easySplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easySplitContainer1.Location = new System.Drawing.Point(4, 4);
            this.easySplitContainer1.Name = "easySplitContainer1";
            this.easySplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // easySplitContainer1.Panel1
            // 
            this.easySplitContainer1.Panel1.Controls.Add(this.easyHeaderGroup3);
            // 
            // easySplitContainer1.Panel2
            // 
            this.easySplitContainer1.Panel2.Controls.Add(this.easySplitContainer2);
            this.easySplitContainer1.Size = new System.Drawing.Size(1022, 584);
            this.easySplitContainer1.SplitterDistance = 107;
            this.easySplitContainer1.TabIndex = 0;
            // 
            // easyHeaderGroup3
            // 
            this.easyHeaderGroup3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyHeaderGroup3.HeaderVisibleSecondary = false;
            this.easyHeaderGroup3.Location = new System.Drawing.Point(0, 0);
            this.easyHeaderGroup3.Name = "easyHeaderGroup3";
            // 
            // easyHeaderGroup3.Panel
            // 
            this.easyHeaderGroup3.Panel.Controls.Add(this.easyLabel4);
            this.easyHeaderGroup3.Panel.Controls.Add(this.easyLabel3);
            this.easyHeaderGroup3.Panel.Controls.Add(this.cobCommunicationMode);
            this.easyHeaderGroup3.Panel.Controls.Add(this.txbServerAddress);
            this.easyHeaderGroup3.Panel.Controls.Add(this.easyLabel2);
            this.easyHeaderGroup3.Panel.Controls.Add(this.easyLabel1);
            this.easyHeaderGroup3.Panel.Controls.Add(this.txbPort);
            this.easyHeaderGroup3.Panel.Controls.Add(this.txbRefreshRate);
            this.easyHeaderGroup3.Size = new System.Drawing.Size(1022, 107);
            this.easyHeaderGroup3.TabIndex = 0;
            this.easyHeaderGroup3.ValuesPrimary.Heading = "Connector Parameters";
            this.easyHeaderGroup3.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("easyHeaderGroup3.ValuesPrimary.Image")));
            // 
            // easyLabel4
            // 
            this.easyLabel4.Location = new System.Drawing.Point(412, 9);
            this.easyLabel4.Name = "easyLabel4";
            this.easyLabel4.Size = new System.Drawing.Size(105, 20);
            this.easyLabel4.TabIndex = 6;
            this.easyLabel4.Values.Text = "Refresh rate (ms):";
            // 
            // easyLabel3
            // 
            this.easyLabel3.Location = new System.Drawing.Point(252, 9);
            this.easyLabel3.Name = "easyLabel3";
            this.easyLabel3.Size = new System.Drawing.Size(135, 20);
            this.easyLabel3.TabIndex = 5;
            this.easyLabel3.Values.Text = "Communication Mode:";
            // 
            // cobCommunicationMode
            // 
            this.cobCommunicationMode.DropDownWidth = 155;
            this.cobCommunicationMode.Location = new System.Drawing.Point(252, 36);
            this.cobCommunicationMode.Name = "cobCommunicationMode";
            this.cobCommunicationMode.Size = new System.Drawing.Size(155, 21);
            this.cobCommunicationMode.TabIndex = 4;
            // 
            // txbServerAddress
            // 
            this.txbServerAddress.Location = new System.Drawing.Point(7, 35);
            this.txbServerAddress.Name = "txbServerAddress";
            this.txbServerAddress.Size = new System.Drawing.Size(156, 23);
            this.txbServerAddress.TabIndex = 2;
            this.txbServerAddress.Text = "127.0.0.1";
            // 
            // easyLabel2
            // 
            this.easyLabel2.Location = new System.Drawing.Point(169, 9);
            this.easyLabel2.Name = "easyLabel2";
            this.easyLabel2.Size = new System.Drawing.Size(36, 20);
            this.easyLabel2.TabIndex = 1;
            this.easyLabel2.Values.Text = "Port:";
            // 
            // easyLabel1
            // 
            this.easyLabel1.Location = new System.Drawing.Point(7, 9);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Size = new System.Drawing.Size(94, 20);
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.Values.Text = "Server Address:";
            // 
            // txbPort
            // 
            this.txbPort.Location = new System.Drawing.Point(169, 35);
            this.txbPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.txbPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txbPort.Name = "txbPort";
            this.txbPort.Size = new System.Drawing.Size(77, 22);
            this.txbPort.TabIndex = 8;
            this.txbPort.Value = new decimal(new int[] {
            8800,
            0,
            0,
            0});
            // 
            // txbRefreshRate
            // 
            this.txbRefreshRate.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txbRefreshRate.Location = new System.Drawing.Point(413, 35);
            this.txbRefreshRate.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.txbRefreshRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txbRefreshRate.Name = "txbRefreshRate";
            this.txbRefreshRate.Size = new System.Drawing.Size(117, 22);
            this.txbRefreshRate.TabIndex = 9;
            this.txbRefreshRate.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // easySplitContainer2
            // 
            this.easySplitContainer2.Cursor = System.Windows.Forms.Cursors.Default;
            this.easySplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easySplitContainer2.Location = new System.Drawing.Point(0, 0);
            this.easySplitContainer2.Name = "easySplitContainer2";
            // 
            // easySplitContainer2.Panel1
            // 
            this.easySplitContainer2.Panel1.Controls.Add(this.groupProjectTree);
            // 
            // easySplitContainer2.Panel2
            // 
            this.easySplitContainer2.Panel2.Controls.Add(this.groupTagCollection);
            this.easySplitContainer2.Size = new System.Drawing.Size(1022, 472);
            this.easySplitContainer2.SplitterDistance = 340;
            this.easySplitContainer2.TabIndex = 1;
            // 
            // groupProjectTree
            // 
            this.groupProjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupProjectTree.Location = new System.Drawing.Point(0, 0);
            this.groupProjectTree.Name = "groupProjectTree";
            // 
            // groupProjectTree.Panel
            // 
            this.groupProjectTree.Panel.Controls.Add(this.projectTree);
            this.groupProjectTree.Size = new System.Drawing.Size(340, 472);
            this.groupProjectTree.TabIndex = 1;
            this.groupProjectTree.ValuesPrimary.Heading = "Project tree";
            this.groupProjectTree.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("groupProjectTree.ValuesPrimary.Image")));
            this.groupProjectTree.ValuesSecondary.Heading = "Total tags: 0";
            // 
            // projectTree
            // 
            this.projectTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.projectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectTree.HideSelection = false;
            this.projectTree.ImageIndex = 0;
            this.projectTree.ImageList = this.imageList1;
            this.projectTree.ItemHeight = 21;
            this.projectTree.Location = new System.Drawing.Point(0, 0);
            this.projectTree.Name = "projectTree";
            this.projectTree.PathSeparator = "/";
            this.projectTree.SelectedImageIndex = 0;
            this.projectTree.Size = new System.Drawing.Size(338, 419);
            this.projectTree.TabIndex = 2;
            // 
            // groupTagCollection
            // 
            this.groupTagCollection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupTagCollection.GroupBackStyle = EasyScada.Winforms.Controls.PaletteBackStyle.ControlGroupBox;
            this.groupTagCollection.Location = new System.Drawing.Point(0, 0);
            this.groupTagCollection.Name = "groupTagCollection";
            // 
            // groupTagCollection.Panel
            // 
            this.groupTagCollection.Panel.Controls.Add(this.searchTagControl);
            this.groupTagCollection.Panel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.groupTagCollection.Size = new System.Drawing.Size(677, 472);
            this.groupTagCollection.TabIndex = 1;
            this.groupTagCollection.ValuesPrimary.Heading = "Tag Collection";
            this.groupTagCollection.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("groupTagCollection.ValuesPrimary.Image")));
            this.groupTagCollection.ValuesSecondary.Heading = "Total: 0";
            // 
            // searchTagControl
            // 
            this.searchTagControl.CoreItemSource = null;
            this.searchTagControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTagControl.IsInSearchMode = false;
            this.searchTagControl.Location = new System.Drawing.Point(0, 3);
            this.searchTagControl.Name = "searchTagControl";
            this.searchTagControl.SelectedItem = null;
            this.searchTagControl.Size = new System.Drawing.Size(675, 416);
            this.searchTagControl.TabIndex = 7;
            this.searchTagControl.TagPathSource = null;
            this.searchTagControl.UseTagPath = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1030, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.sToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.sToolStripMenuItem1,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // sToolStripMenuItem
            // 
            this.sToolStripMenuItem.Name = "sToolStripMenuItem";
            this.sToolStripMenuItem.Size = new System.Drawing.Size(118, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            // 
            // sToolStripMenuItem1
            // 
            this.sToolStripMenuItem1.Name = "sToolStripMenuItem1";
            this.sToolStripMenuItem1.Size = new System.Drawing.Size(118, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getConnectionSchemaToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // getConnectionSchemaToolStripMenuItem
            // 
            this.getConnectionSchemaToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("getConnectionSchemaToolStripMenuItem.Image")));
            this.getConnectionSchemaToolStripMenuItem.Name = "getConnectionSchemaToolStripMenuItem";
            this.getConnectionSchemaToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.getConnectionSchemaToolStripMenuItem.Text = "Get Connection Schema";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.toolStripSeparator1,
            this.btnSave,
            this.toolStripButton3,
            this.btnGetConnectionSchema});
            this.toolStrip1.Location = new System.Drawing.Point(3, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(93, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "toolStripButton1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnGetConnectionSchema
            // 
            this.btnGetConnectionSchema.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGetConnectionSchema.Image = ((System.Drawing.Image)(resources.GetObject("btnGetConnectionSchema.Image")));
            this.btnGetConnectionSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGetConnectionSchema.Name = "btnGetConnectionSchema";
            this.btnGetConnectionSchema.Size = new System.Drawing.Size(23, 22);
            this.btnGetConnectionSchema.Text = "toolStripButton4";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "remote_station_24px-2.png");
            this.imageList1.Images.SetKeyName(1, "local_station_16px.png");
            this.imageList1.Images.SetKeyName(2, "remote_station_24px.png");
            this.imageList1.Images.SetKeyName(3, "channel_24px.png");
            this.imageList1.Images.SetKeyName(4, "device_24px.png");
            this.imageList1.Images.SetKeyName(5, "folder_48px.png");
            this.imageList1.Images.SetKeyName(6, "tag_24px.png");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ConnectionSchemeDesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 641);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ConnectionSchemeDesignerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Connection Schema";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).EndInit();
            this.easyPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel1)).EndInit();
            this.easySplitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel2)).EndInit();
            this.easySplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1)).EndInit();
            this.easySplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup3.Panel)).EndInit();
            this.easyHeaderGroup3.Panel.ResumeLayout(false);
            this.easyHeaderGroup3.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup3)).EndInit();
            this.easyHeaderGroup3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cobCommunicationMode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer2.Panel1)).EndInit();
            this.easySplitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer2.Panel2)).EndInit();
            this.easySplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer2)).EndInit();
            this.easySplitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree.Panel)).EndInit();
            this.groupProjectTree.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree)).EndInit();
            this.groupProjectTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.projectTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection.Panel)).EndInit();
            this.groupTagCollection.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection)).EndInit();
            this.groupTagCollection.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private EasyPanel easyPanel1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getConnectionSchemaToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripButton3;
        private System.Windows.Forms.ToolStripButton btnGetConnectionSchema;
        private EasySplitContainer easySplitContainer1;
        private EasyHeaderGroup easyHeaderGroup3;
        private EasySplitContainer easySplitContainer2;
        private ThemedLabel easyLabel1;
        private ThemedTextBox txbServerAddress;
        private ThemedLabel easyLabel2;
        private EasyComboBox cobCommunicationMode;
        private ThemedLabel easyLabel3;
        private ThemedLabel easyLabel4;
        private EasyHeaderGroup groupProjectTree;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private EasyScada.Winforms.Controls.HieraticalTreeView projectTree;
        private EasyNumericUpDown txbPort;
        private EasyNumericUpDown txbRefreshRate;
        private System.Windows.Forms.ImageList imageList1;
        private EasyHeaderGroup groupTagCollection;
        private SearchTagControl searchTagControl;
    }
}