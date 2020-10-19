namespace EasyScada.Winforms.Controls
{
    partial class SelectTagPathDesignerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectTagPathDesignerForm));
            this.easyPanel1 = new EasyScada.Winforms.Controls.EasyPanel();
            this.btnOk = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnCancel = new EasyScada.Winforms.Controls.ThemedButton();
            this.easyPanel2 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easySplitContainer1 = new EasyScada.Winforms.Controls.EasySplitContainer();
            this.groupProjectTree = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.projectTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupTagCollection = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.easyPanel3 = new EasyScada.Winforms.Controls.EasyPanel();
            this.searchTagControl1 = new EasyScada.Winforms.Controls.SearchTagControl();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).BeginInit();
            this.easyPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel2)).BeginInit();
            this.easyPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel1)).BeginInit();
            this.easySplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel2)).BeginInit();
            this.easySplitContainer1.Panel2.SuspendLayout();
            this.easySplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree.Panel)).BeginInit();
            this.groupProjectTree.Panel.SuspendLayout();
            this.groupProjectTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection.Panel)).BeginInit();
            this.groupTagCollection.Panel.SuspendLayout();
            this.groupTagCollection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel3)).BeginInit();
            this.easyPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // easyPanel1
            // 
            this.easyPanel1.Controls.Add(this.btnOk);
            this.easyPanel1.Controls.Add(this.btnCancel);
            this.easyPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.easyPanel1.Location = new System.Drawing.Point(0, 548);
            this.easyPanel1.Name = "easyPanel1";
            this.easyPanel1.Size = new System.Drawing.Size(829, 28);
            this.easyPanel1.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(640, 0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 25);
            this.btnOk.TabIndex = 1;
            this.btnOk.Values.Text = "Ok";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(736, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 25);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Values.Text = "Cancel";
            // 
            // easyPanel2
            // 
            this.easyPanel2.Controls.Add(this.easySplitContainer1);
            this.easyPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel2.Location = new System.Drawing.Point(0, 0);
            this.easyPanel2.Name = "easyPanel2";
            this.easyPanel2.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel2.Size = new System.Drawing.Size(829, 548);
            this.easyPanel2.TabIndex = 1;
            // 
            // easySplitContainer1
            // 
            this.easySplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easySplitContainer1.Location = new System.Drawing.Point(4, 4);
            this.easySplitContainer1.Name = "easySplitContainer1";
            // 
            // easySplitContainer1.Panel1
            // 
            this.easySplitContainer1.Panel1.Controls.Add(this.groupProjectTree);
            // 
            // easySplitContainer1.Panel2
            // 
            this.easySplitContainer1.Panel2.Controls.Add(this.groupTagCollection);
            this.easySplitContainer1.Size = new System.Drawing.Size(821, 540);
            this.easySplitContainer1.SplitterDistance = 273;
            this.easySplitContainer1.TabIndex = 1;
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
            this.groupProjectTree.Size = new System.Drawing.Size(273, 540);
            this.groupProjectTree.TabIndex = 0;
            this.groupProjectTree.ValuesPrimary.Heading = "Project Tree";
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
            this.projectTree.Size = new System.Drawing.Size(271, 487);
            this.projectTree.TabIndex = 1;
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
            // groupTagCollection
            // 
            this.groupTagCollection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupTagCollection.Location = new System.Drawing.Point(0, 0);
            this.groupTagCollection.Name = "groupTagCollection";
            // 
            // groupTagCollection.Panel
            // 
            this.groupTagCollection.Panel.Controls.Add(this.easyPanel3);
            this.groupTagCollection.Size = new System.Drawing.Size(543, 540);
            this.groupTagCollection.TabIndex = 0;
            this.groupTagCollection.ValuesPrimary.Heading = "Tags Collection";
            this.groupTagCollection.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("groupTagCollection.ValuesPrimary.Image")));
            this.groupTagCollection.ValuesSecondary.Heading = "Total: 0";
            // 
            // easyPanel3
            // 
            this.easyPanel3.Controls.Add(this.searchTagControl1);
            this.easyPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel3.Location = new System.Drawing.Point(0, 0);
            this.easyPanel3.Name = "easyPanel3";
            this.easyPanel3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.easyPanel3.Size = new System.Drawing.Size(541, 487);
            this.easyPanel3.TabIndex = 0;
            // 
            // searchTagControl1
            // 
            this.searchTagControl1.CoreItemSource = null;
            this.searchTagControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTagControl1.IsInSearchMode = false;
            this.searchTagControl1.Location = new System.Drawing.Point(0, 3);
            this.searchTagControl1.Name = "searchTagControl1";
            this.searchTagControl1.SelectedItem = null;
            this.searchTagControl1.Size = new System.Drawing.Size(541, 484);
            this.searchTagControl1.TabIndex = 2;
            this.searchTagControl1.TagPathSource = null;
            this.searchTagControl1.UseTagPath = false;
            // 
            // SelectTagPathDesignerForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(829, 576);
            this.Controls.Add(this.easyPanel2);
            this.Controls.Add(this.easyPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectTagPathDesignerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Tag";
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).EndInit();
            this.easyPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel2)).EndInit();
            this.easyPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel1)).EndInit();
            this.easySplitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel2)).EndInit();
            this.easySplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1)).EndInit();
            this.easySplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree.Panel)).EndInit();
            this.groupProjectTree.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupProjectTree)).EndInit();
            this.groupProjectTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection.Panel)).EndInit();
            this.groupTagCollection.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupTagCollection)).EndInit();
            this.groupTagCollection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel3)).EndInit();
            this.easyPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private EasyPanel easyPanel1;
        private ThemedButton btnOk;
        private ThemedButton btnCancel;
        private EasyPanel easyPanel2;
        private EasySplitContainer easySplitContainer1;
        private EasyHeaderGroup groupProjectTree;
        private EasyHeaderGroup groupTagCollection;
        private EasyPanel easyPanel3;
        private System.Windows.Forms.TreeView projectTree;
        private System.Windows.Forms.ImageList imageList1;
        private SearchTagControl searchTagControl1;
    }
}