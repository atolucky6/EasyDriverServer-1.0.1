namespace EasyScada.ServerApp
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnNew = new DevExpress.XtraBars.BarButtonItem();
            this.btnOpen = new DevExpress.XtraBars.BarButtonItem();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveAs = new DevExpress.XtraBars.BarButtonItem();
            this.btnUndo = new DevExpress.XtraBars.BarButtonItem();
            this.btnRedo = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddChannel = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddDevice = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddTag = new DevExpress.XtraBars.BarButtonItem();
            this.btnCopy = new DevExpress.XtraBars.BarButtonItem();
            this.btnCut = new DevExpress.XtraBars.BarButtonItem();
            this.btnPaste = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.subFile = new DevExpress.XtraBars.BarSubItem();
            this.btnExit = new DevExpress.XtraBars.BarButtonItem();
            this.subEdit = new DevExpress.XtraBars.BarSubItem();
            this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.projectTree1 = new EasyScada.ServerApp.ProjectTree();
            this.projectTree2 = new EasyScada.ServerApp.ProjectTree();
            this.projectTree3 = new EasyScada.ServerApp.ProjectTree();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar2,
            this.bar3});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.subFile,
            this.subEdit,
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.btnSaveAs,
            this.btnExit,
            this.btnAddChannel,
            this.btnAddDevice,
            this.btnAddTag,
            this.btnCopy,
            this.btnCut,
            this.btnPaste,
            this.btnDelete,
            this.btnUndo,
            this.btnRedo,
            this.barSubItem1,
            this.barStaticItem1});
            this.barManager.MainMenu = this.bar2;
            this.barManager.MaxItemId = 24;
            this.barManager.StatusBar = this.bar3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 1;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.FloatLocation = new System.Drawing.Point(498, 163);
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnOpen),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSave, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveAs),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRedo),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddChannel, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddDevice),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddTag),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCopy, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCut),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPaste),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btnNew
            // 
            this.btnNew.Caption = "New";
            this.btnNew.Id = 8;
            this.btnNew.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.new_16x161;
            this.btnNew.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.new_32x321;
            this.btnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.btnNew.Name = "btnNew";
            // 
            // btnOpen
            // 
            this.btnOpen.Caption = "Open";
            this.btnOpen.Id = 9;
            this.btnOpen.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.open_16x161;
            this.btnOpen.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.open_32x321;
            this.btnOpen.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O));
            this.btnOpen.Name = "btnOpen";
            // 
            // btnSave
            // 
            this.btnSave.Caption = "Save";
            this.btnSave.Id = 10;
            this.btnSave.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.save_16x161;
            this.btnSave.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.save_32x321;
            this.btnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.btnSave.Name = "btnSave";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Caption = "Save as...";
            this.btnSaveAs.Id = 11;
            this.btnSaveAs.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.saveas_16x16;
            this.btnSaveAs.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.saveas_32x32;
            this.btnSaveAs.Name = "btnSaveAs";
            // 
            // btnUndo
            // 
            this.btnUndo.Caption = "Undo";
            this.btnUndo.Id = 20;
            this.btnUndo.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.undo_16x16;
            this.btnUndo.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.undo_32x32;
            this.btnUndo.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z));
            this.btnUndo.Name = "btnUndo";
            // 
            // btnRedo
            // 
            this.btnRedo.Caption = "Redo";
            this.btnRedo.Id = 21;
            this.btnRedo.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.redo_16x16;
            this.btnRedo.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.redo_32x32;
            this.btnRedo.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y));
            this.btnRedo.Name = "btnRedo";
            // 
            // btnAddChannel
            // 
            this.btnAddChannel.Caption = "Add Channel";
            this.btnAddChannel.Id = 13;
            this.btnAddChannel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddChannel.ImageOptions.Image")));
            this.btnAddChannel.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1));
            this.btnAddChannel.Name = "btnAddChannel";
            // 
            // btnAddDevice
            // 
            this.btnAddDevice.Caption = "Add Device";
            this.btnAddDevice.Id = 14;
            this.btnAddDevice.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.device_16px;
            this.btnAddDevice.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2));
            this.btnAddDevice.Name = "btnAddDevice";
            // 
            // btnAddTag
            // 
            this.btnAddTag.Caption = "Add Tag";
            this.btnAddTag.Id = 15;
            this.btnAddTag.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.tag_16px;
            this.btnAddTag.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3));
            this.btnAddTag.Name = "btnAddTag";
            // 
            // btnCopy
            // 
            this.btnCopy.Caption = "Copy";
            this.btnCopy.Id = 16;
            this.btnCopy.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.copy_16x16;
            this.btnCopy.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.copy_32x32;
            this.btnCopy.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C));
            this.btnCopy.Name = "btnCopy";
            // 
            // btnCut
            // 
            this.btnCut.Caption = "Cut";
            this.btnCut.Id = 17;
            this.btnCut.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.cut_16x16;
            this.btnCut.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.cut_32x32;
            this.btnCut.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X));
            this.btnCut.Name = "btnCut";
            // 
            // btnPaste
            // 
            this.btnPaste.Caption = "Paste";
            this.btnPaste.Id = 18;
            this.btnPaste.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.paste_16x16;
            this.btnPaste.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.paste_32x32;
            this.btnPaste.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V));
            this.btnPaste.Name = "btnPaste";
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Delete";
            this.btnDelete.Id = 19;
            this.btnDelete.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.removepivotfield_16x16;
            this.btnDelete.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.removepivotfield_32x32;
            this.btnDelete.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Delete);
            this.btnDelete.Name = "btnDelete";
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.subFile),
            new DevExpress.XtraBars.LinkPersistInfo(this.subEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // subFile
            // 
            this.subFile.Caption = "File";
            this.subFile.Id = 0;
            this.subFile.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnNew, DevExpress.XtraBars.BarItemPaintStyle.Standard),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnOpen),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSave, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveAs),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExit, true)});
            this.subFile.Name = "subFile";
            this.subFile.ShortcutKeyDisplayString = "F";
            // 
            // btnExit
            // 
            this.btnExit.Caption = "Exit";
            this.btnExit.Id = 12;
            this.btnExit.ImageOptions.Image = global::EasyScada.ServerApp.Properties.Resources.close_16x16;
            this.btnExit.ImageOptions.LargeImage = global::EasyScada.ServerApp.Properties.Resources.close_32x32;
            this.btnExit.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4));
            this.btnExit.Name = "btnExit";
            // 
            // subEdit
            // 
            this.subEdit.Caption = "Edit";
            this.subEdit.Id = 1;
            this.subEdit.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddChannel),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddDevice),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddTag),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCopy, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCut),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPaste),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRedo)});
            this.subEdit.Name = "subEdit";
            this.subEdit.ShortcutKeyDisplayString = "E";
            // 
            // barSubItem1
            // 
            this.barSubItem1.Caption = "Tool";
            this.barSubItem1.Id = 22;
            this.barSubItem1.Name = "barSubItem1";
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem1)});
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "Server running on port 9090";
            this.barStaticItem1.Id = 23;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(1228, 53);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 673);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(1228, 27);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 53);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 620);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1228, 53);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 620);
            // 
            // projectTree1
            // 
            this.projectTree1.Channels = null;
            this.projectTree1.Location = new System.Drawing.Point(0, 0);
            this.projectTree1.Name = "projectTree1";
            this.projectTree1.Size = new System.Drawing.Size(816, 637);
            this.projectTree1.TabIndex = 0;
            // 
            // projectTree2
            // 
            this.projectTree2.Channels = null;
            this.projectTree2.Location = new System.Drawing.Point(0, 0);
            this.projectTree2.Name = "projectTree2";
            this.projectTree2.Size = new System.Drawing.Size(816, 637);
            this.projectTree2.TabIndex = 0;
            // 
            // projectTree3
            // 
            this.projectTree3.Location = new System.Drawing.Point(145, 90);
            this.projectTree3.Name = "projectTree3";
            this.projectTree3.Size = new System.Drawing.Size(816, 637);
            this.projectTree3.TabIndex = 4;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1228, 700);
            this.Controls.Add(this.projectTree3);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmMain";
            this.Text = "Easy Scada Server V1.0.1";
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarSubItem subFile;
        private DevExpress.XtraBars.BarSubItem subEdit;
        private DevExpress.XtraBars.BarButtonItem btnNew;
        private DevExpress.XtraBars.BarButtonItem btnOpen;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.BarButtonItem btnSaveAs;
        private DevExpress.XtraBars.BarButtonItem btnExit;
        private DevExpress.XtraBars.BarButtonItem btnUndo;
        private DevExpress.XtraBars.BarButtonItem btnRedo;
        private DevExpress.XtraBars.BarButtonItem btnAddChannel;
        private DevExpress.XtraBars.BarButtonItem btnAddDevice;
        private DevExpress.XtraBars.BarButtonItem btnAddTag;
        private DevExpress.XtraBars.BarButtonItem btnCopy;
        private DevExpress.XtraBars.BarButtonItem btnCut;
        private DevExpress.XtraBars.BarButtonItem btnPaste;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.BarSubItem barSubItem1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private ProjectTree projectTree1;
        private ProjectTree projectTree2;
        private ProjectTree projectTree3;
    }
}