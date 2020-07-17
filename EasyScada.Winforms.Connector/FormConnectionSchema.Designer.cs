namespace EasyScada.Winforms.Connector
{
    partial class FormConnectionSchema
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConnectionSchema));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnReloadAll = new System.Windows.Forms.ToolStripButton();
            this.btnTransfer = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.projectTreeViewContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnContextRefreshServer = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextExpandAllServer = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextCollapseAllServer = new System.Windows.Forms.ToolStripMenuItem();
            this.localProjectTreeContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnContextRefreshLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextExpandAllProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContextCollapseAllProject = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.localProjectTree = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.serverProjectTree = new EasyScada.Winforms.Connector.ProjectTreeView();
            this.toolStrip.SuspendLayout();
            this.projectTreeViewContext.SuspendLayout();
            this.localProjectTreeContext.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnReloadAll,
            this.btnTransfer,
            this.btnSave});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(700, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnReloadAll
            // 
            this.btnReloadAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnReloadAll.Image = ((System.Drawing.Image)(resources.GetObject("btnReloadAll.Image")));
            this.btnReloadAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReloadAll.Name = "btnReloadAll";
            this.btnReloadAll.Size = new System.Drawing.Size(23, 22);
            this.btnReloadAll.Text = "toolStripButton1";
            this.btnReloadAll.ToolTipText = "Reload";
            this.btnReloadAll.Click += new System.EventHandler(this.btnReloadAll_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTransfer.Image = ((System.Drawing.Image)(resources.GetObject("btnTransfer.Image")));
            this.btnTransfer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(23, 22);
            this.btnTransfer.Text = "btnTransfer";
            this.btnTransfer.ToolTipText = "Transfer server tag file to project";
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnSave
            // 
            this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "toolStripButton2";
            this.btnSave.ToolTipText = "Save project tag file";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "remote_station_24px.png");
            this.imageList1.Images.SetKeyName(1, "local_station_16px.png");
            this.imageList1.Images.SetKeyName(2, "channel_24px.png");
            this.imageList1.Images.SetKeyName(3, "device_24px.png");
            this.imageList1.Images.SetKeyName(4, "tag_24px.png");
            // 
            // projectTreeViewContext
            // 
            this.projectTreeViewContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnContextRefreshServer,
            this.btnContextExpandAllServer,
            this.btnContextCollapseAllServer});
            this.projectTreeViewContext.Name = "projectTreeViewContext";
            this.projectTreeViewContext.Size = new System.Drawing.Size(137, 70);
            // 
            // btnContextRefreshServer
            // 
            this.btnContextRefreshServer.Name = "btnContextRefreshServer";
            this.btnContextRefreshServer.Size = new System.Drawing.Size(136, 22);
            this.btnContextRefreshServer.Text = "Refresh";
            this.btnContextRefreshServer.Click += new System.EventHandler(this.btnContextRefreshServer_Click);
            // 
            // btnContextExpandAllServer
            // 
            this.btnContextExpandAllServer.Name = "btnContextExpandAllServer";
            this.btnContextExpandAllServer.Size = new System.Drawing.Size(136, 22);
            this.btnContextExpandAllServer.Text = "Expand All";
            this.btnContextExpandAllServer.Click += new System.EventHandler(this.btnContextExpandAllServer_Click);
            // 
            // btnContextCollapseAllServer
            // 
            this.btnContextCollapseAllServer.Name = "btnContextCollapseAllServer";
            this.btnContextCollapseAllServer.Size = new System.Drawing.Size(136, 22);
            this.btnContextCollapseAllServer.Text = "Collapse All";
            this.btnContextCollapseAllServer.Click += new System.EventHandler(this.btnContextCollapseAllServer_Click);
            // 
            // localProjectTreeContext
            // 
            this.localProjectTreeContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnContextRefreshLocal,
            this.btnContextExpandAllProject,
            this.btnContextCollapseAllProject});
            this.localProjectTreeContext.Name = "localProjectTreeContext";
            this.localProjectTreeContext.Size = new System.Drawing.Size(137, 70);
            // 
            // btnContextRefreshLocal
            // 
            this.btnContextRefreshLocal.Name = "btnContextRefreshLocal";
            this.btnContextRefreshLocal.Size = new System.Drawing.Size(136, 22);
            this.btnContextRefreshLocal.Text = "Refresh";
            this.btnContextRefreshLocal.Click += new System.EventHandler(this.btnContextRefreshLocal_Click);
            // 
            // btnContextExpandAllProject
            // 
            this.btnContextExpandAllProject.Name = "btnContextExpandAllProject";
            this.btnContextExpandAllProject.Size = new System.Drawing.Size(136, 22);
            this.btnContextExpandAllProject.Text = "Expand All";
            this.btnContextExpandAllProject.Click += new System.EventHandler(this.btnContextExpandAllProject_Click);
            // 
            // btnContextCollapseAllProject
            // 
            this.btnContextCollapseAllProject.Name = "btnContextCollapseAllProject";
            this.btnContextCollapseAllProject.Size = new System.Drawing.Size(136, 22);
            this.btnContextCollapseAllProject.Text = "Collapse All";
            this.btnContextCollapseAllProject.Click += new System.EventHandler(this.btnContextCollapseAllProject_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.localProjectTree);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(351, 409);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project Connection Schema";
            // 
            // localProjectTree
            // 
            this.localProjectTree.ContextMenuStrip = this.localProjectTreeContext;
            this.localProjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localProjectTree.FullRowSelect = true;
            this.localProjectTree.ImageIndex = 0;
            this.localProjectTree.ImageList = this.imageList1;
            this.localProjectTree.Location = new System.Drawing.Point(3, 16);
            this.localProjectTree.Name = "localProjectTree";
            this.localProjectTree.SelectedImageIndex = 0;
            this.localProjectTree.Size = new System.Drawing.Size(345, 390);
            this.localProjectTree.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.serverProjectTree);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(345, 409);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server Connection Schema";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(700, 409);
            this.splitContainer1.SplitterDistance = 345;
            this.splitContainer1.TabIndex = 1;
            // 
            // serverProjectTree
            // 
            this.serverProjectTree.BackColor = System.Drawing.SystemColors.Window;
            this.serverProjectTree.ContextMenuStrip = this.projectTreeViewContext;
            this.serverProjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverProjectTree.ImageIndex = 0;
            this.serverProjectTree.ImageList = this.imageList1;
            this.serverProjectTree.Location = new System.Drawing.Point(3, 16);
            this.serverProjectTree.Name = "serverProjectTree";
            this.serverProjectTree.SelectedImageIndex = 0;
            this.serverProjectTree.Size = new System.Drawing.Size(339, 390);
            this.serverProjectTree.TabIndex = 0;
            // 
            // FormConnectionSchema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 434);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip);
            this.Name = "FormConnectionSchema";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection Schema";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.projectTreeViewContext.ResumeLayout(false);
            this.localProjectTreeContext.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton btnReloadAll;
        private System.Windows.Forms.ToolStripButton btnTransfer;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ContextMenuStrip projectTreeViewContext;
        private System.Windows.Forms.ToolStripMenuItem btnContextRefreshServer;
        private System.Windows.Forms.ContextMenuStrip localProjectTreeContext;
        private System.Windows.Forms.ToolStripMenuItem btnContextRefreshLocal;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView localProjectTree;
        private System.Windows.Forms.GroupBox groupBox2;
        private ProjectTreeView serverProjectTree;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem btnContextExpandAllServer;
        private System.Windows.Forms.ToolStripMenuItem btnContextCollapseAllServer;
        private System.Windows.Forms.ToolStripMenuItem btnContextExpandAllProject;
        private System.Windows.Forms.ToolStripMenuItem btnContextCollapseAllProject;
    }
}