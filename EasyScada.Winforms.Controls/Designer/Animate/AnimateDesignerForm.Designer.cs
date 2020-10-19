namespace EasyScada.Winforms.Controls
{
    partial class AnimateDesignerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimateDesignerForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.easyPanel1 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easySplitContainer1 = new EasyScada.Winforms.Controls.EasySplitContainer();
            this.easyNavigator1 = new EasyScada.Winforms.Controls.Navigator.EasyNavigator();
            this.easyPage1 = new EasyScada.Winforms.Controls.Navigator.EasyPage();
            this.easyPanel2 = new EasyScada.Winforms.Controls.EasyPanel();
            this.numDelayAnalog = new EasyScada.Winforms.Controls.EasyNumericUpDown();
            this.ckbEnabledAnalog = new EasyScada.Winforms.Controls.EasyCheckBox();
            this.txbMinValueAnalog = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnBrowseMinValueAnalog = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.txbMaxValueAnalog = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnBrowseMaxValueAnalog = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.txbDescriptionAnalog = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnAddAnalog = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnEditAnalog = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnDeleteAnalog = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnSaveAnalog = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnCancelAnalog = new EasyScada.Winforms.Controls.ThemedButton();
            this.easyLabel7 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel6 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel5 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel4 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.cobCompareModeAnalog = new EasyScada.Winforms.Controls.EasyComboBox();
            this.txbTriggerTagAnalog = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnBrowseTriggerTagAnalog = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.easyLabel2 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel1 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.analogGridView = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.colAnalogEnabled = new EasyScada.Winforms.Controls.EasyDataGridViewCheckBoxColumn();
            this.colAnalogTriggerTag = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAnalogMinValue = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAnalogMaxValue = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAnalogCompareMode = new EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn();
            this.colAnalogDelay = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.colAnalogDescription = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyPage2 = new EasyScada.Winforms.Controls.Navigator.EasyPage();
            this.easyPanel3 = new EasyScada.Winforms.Controls.EasyPanel();
            this.numDelayDiscrete = new EasyScada.Winforms.Controls.EasyNumericUpDown();
            this.ckbEnabledDiscrete = new EasyScada.Winforms.Controls.EasyCheckBox();
            this.txbTriggerValueDiscrete = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnBrowseTriggerValueDiscrete = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.txbDescriptionDiscrete = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnAddDiscrete = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnEditDiscrete = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnDeleteDiscrete = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnSaveDiscrete = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnCancelDiscrete = new EasyScada.Winforms.Controls.ThemedButton();
            this.easyLabel3 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel8 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel9 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.cobCompareModeDiscrete = new EasyScada.Winforms.Controls.EasyComboBox();
            this.txbTriggerTagDiscrete = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnBrowseTriggerTagDiscrete = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.easyLabel11 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel12 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.discreteGridView = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.easyDataGridViewCheckBoxColumn1 = new EasyScada.Winforms.Controls.EasyDataGridViewCheckBoxColumn();
            this.easyDataGridViewTextBoxColumn1 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewTextBoxColumn2 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewComboBoxColumn1 = new EasyScada.Winforms.Controls.EasyDataGridViewComboBoxColumn();
            this.easyDataGridViewTextBoxColumn4 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewTextBoxColumn5 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyPage3 = new EasyScada.Winforms.Controls.Navigator.EasyPage();
            this.easyPanel4 = new EasyScada.Winforms.Controls.EasyPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.numDelayQuality = new EasyScada.Winforms.Controls.EasyNumericUpDown();
            this.cobCompareModeQuality = new EasyScada.Winforms.Controls.EasyComboBox();
            this.cobQuality = new EasyScada.Winforms.Controls.EasyComboBox();
            this.themedLabel5 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.themedLabel3 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.themedLabel2 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.themedLabel1 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.ckbEnabledQuality = new EasyScada.Winforms.Controls.EasyCheckBox();
            this.txbTriggerTagQuality = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnBrowseTriggerTagQuality = new EasyScada.Winforms.Controls.ButtonSpecAny();
            this.themedLabel4 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.txbDescriptionQuality = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnAddQuality = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnEditQuality = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnDeleteQuality = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnSaveQuality = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnCancelQuality = new EasyScada.Winforms.Controls.ThemedButton();
            this.qualityGridView = new EasyScada.Winforms.Controls.EasyDataGridView();
            this.easyDataGridViewCheckBoxColumn2 = new EasyScada.Winforms.Controls.EasyDataGridViewCheckBoxColumn();
            this.easyDataGridViewTextBoxColumn3 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewTextBoxColumn6 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewComboBoxColumn2 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewTextBoxColumn7 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyDataGridViewTextBoxColumn8 = new EasyScada.Winforms.Controls.EasyDataGridViewTextBoxColumn();
            this.easyHeaderGroup2 = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.btnResetProperty = new EasyScada.Winforms.Controls.ButtonSpecHeaderGroup();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.easyContextMenuItem1 = new EasyScada.Winforms.Controls.EasyContextMenuItem();
            this.easyContextMenuItem2 = new EasyScada.Winforms.Controls.EasyContextMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel1)).BeginInit();
            this.easyPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel1)).BeginInit();
            this.easySplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easySplitContainer1.Panel2)).BeginInit();
            this.easySplitContainer1.Panel2.SuspendLayout();
            this.easySplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyNavigator1)).BeginInit();
            this.easyNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPage1)).BeginInit();
            this.easyPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel2)).BeginInit();
            this.easyPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCompareModeAnalog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.analogGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyPage2)).BeginInit();
            this.easyPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel3)).BeginInit();
            this.easyPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCompareModeDiscrete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discreteGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyPage3)).BeginInit();
            this.easyPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel4)).BeginInit();
            this.easyPanel4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCompareModeQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2.Panel)).BeginInit();
            this.easyHeaderGroup2.Panel.SuspendLayout();
            this.easyHeaderGroup2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.easyPanel1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(980, 610);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(980, 635);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // easyPanel1
            // 
            this.easyPanel1.Controls.Add(this.easySplitContainer1);
            this.easyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel1.Location = new System.Drawing.Point(0, 0);
            this.easyPanel1.Name = "easyPanel1";
            this.easyPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel1.Size = new System.Drawing.Size(980, 610);
            this.easyPanel1.TabIndex = 0;
            // 
            // easySplitContainer1
            // 
            this.easySplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.easySplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easySplitContainer1.Location = new System.Drawing.Point(4, 4);
            this.easySplitContainer1.Name = "easySplitContainer1";
            // 
            // easySplitContainer1.Panel1
            // 
            this.easySplitContainer1.Panel1.Controls.Add(this.easyNavigator1);
            // 
            // easySplitContainer1.Panel2
            // 
            this.easySplitContainer1.Panel2.Controls.Add(this.easyHeaderGroup2);
            this.easySplitContainer1.Size = new System.Drawing.Size(972, 602);
            this.easySplitContainer1.SplitterDistance = 664;
            this.easySplitContainer1.TabIndex = 0;
            // 
            // easyNavigator1
            // 
            this.easyNavigator1.Bar.BarMapExtraText = EasyScada.Winforms.Controls.Navigator.MapEasyPageText.None;
            this.easyNavigator1.Bar.BarMapImage = EasyScada.Winforms.Controls.Navigator.MapEasyPageImage.Small;
            this.easyNavigator1.Bar.BarMapText = EasyScada.Winforms.Controls.Navigator.MapEasyPageText.TextTitle;
            this.easyNavigator1.Bar.CheckButtonStyle = EasyScada.Winforms.Controls.ButtonStyle.Standalone;
            this.easyNavigator1.Bar.ItemSizing = EasyScada.Winforms.Controls.Navigator.BarItemSizing.SameWidthAndHeight;
            this.easyNavigator1.Bar.TabBorderStyle = EasyScada.Winforms.Controls.TabBorderStyle.RoundedEqualSmall;
            this.easyNavigator1.Bar.TabStyle = EasyScada.Winforms.Controls.TabStyle.HighProfile;
            this.easyNavigator1.Button.ButtonDisplayLogic = EasyScada.Winforms.Controls.Navigator.ButtonDisplayLogic.None;
            this.easyNavigator1.Button.CloseButtonAction = EasyScada.Winforms.Controls.Navigator.CloseButtonAction.None;
            this.easyNavigator1.Button.CloseButtonDisplay = EasyScada.Winforms.Controls.Navigator.ButtonDisplay.Hide;
            this.easyNavigator1.Button.ContextButtonAction = EasyScada.Winforms.Controls.Navigator.ContextButtonAction.SelectPage;
            this.easyNavigator1.Button.ContextButtonDisplay = EasyScada.Winforms.Controls.Navigator.ButtonDisplay.Logic;
            this.easyNavigator1.Button.ContextMenuMapImage = EasyScada.Winforms.Controls.Navigator.MapEasyPageImage.Small;
            this.easyNavigator1.Button.ContextMenuMapText = EasyScada.Winforms.Controls.Navigator.MapEasyPageText.TextTitle;
            this.easyNavigator1.Button.NextButtonAction = EasyScada.Winforms.Controls.Navigator.DirectionButtonAction.ModeAppropriateAction;
            this.easyNavigator1.Button.NextButtonDisplay = EasyScada.Winforms.Controls.Navigator.ButtonDisplay.Logic;
            this.easyNavigator1.Button.PreviousButtonAction = EasyScada.Winforms.Controls.Navigator.DirectionButtonAction.ModeAppropriateAction;
            this.easyNavigator1.Button.PreviousButtonDisplay = EasyScada.Winforms.Controls.Navigator.ButtonDisplay.Logic;
            this.easyNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyNavigator1.Group.GroupBackStyle = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.easyNavigator1.Group.GroupBorderStyle = EasyScada.Winforms.Controls.PaletteBorderStyle.ControlClient;
            this.easyNavigator1.Header.HeaderStyleBar = EasyScada.Winforms.Controls.HeaderStyle.Secondary;
            this.easyNavigator1.Header.HeaderStylePrimary = EasyScada.Winforms.Controls.HeaderStyle.Primary;
            this.easyNavigator1.Header.HeaderStyleSecondary = EasyScada.Winforms.Controls.HeaderStyle.Secondary;
            this.easyNavigator1.Location = new System.Drawing.Point(0, 0);
            this.easyNavigator1.Name = "easyNavigator1";
            this.easyNavigator1.NavigatorMode = EasyScada.Winforms.Controls.Navigator.NavigatorMode.BarTabGroup;
            this.easyNavigator1.PageBackStyle = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.easyNavigator1.Pages.AddRange(new EasyScada.Winforms.Controls.Navigator.EasyPage[] {
            this.easyPage1,
            this.easyPage2,
            this.easyPage3});
            this.easyNavigator1.Panel.PanelBackStyle = EasyScada.Winforms.Controls.PaletteBackStyle.PanelClient;
            this.easyNavigator1.SelectedIndex = 0;
            this.easyNavigator1.Size = new System.Drawing.Size(664, 602);
            this.easyNavigator1.TabIndex = 0;
            this.easyNavigator1.Text = "easyNavigator1";
            // 
            // easyPage1
            // 
            this.easyPage1.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.easyPage1.Controls.Add(this.easyPanel2);
            this.easyPage1.Flags = 65534;
            this.easyPage1.LastVisibleSet = true;
            this.easyPage1.MinimumSize = new System.Drawing.Size(50, 50);
            this.easyPage1.Name = "easyPage1";
            this.easyPage1.Size = new System.Drawing.Size(662, 576);
            this.easyPage1.Text = "Analog triggers";
            this.easyPage1.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.easyPage1.ToolTipTitle = "Page ToolTip";
            this.easyPage1.UniqueName = "1D93E0CD36F340930598BC0B36C4DEF1";
            // 
            // easyPanel2
            // 
            this.easyPanel2.Controls.Add(this.numDelayAnalog);
            this.easyPanel2.Controls.Add(this.ckbEnabledAnalog);
            this.easyPanel2.Controls.Add(this.txbMinValueAnalog);
            this.easyPanel2.Controls.Add(this.txbMaxValueAnalog);
            this.easyPanel2.Controls.Add(this.txbDescriptionAnalog);
            this.easyPanel2.Controls.Add(this.btnAddAnalog);
            this.easyPanel2.Controls.Add(this.btnEditAnalog);
            this.easyPanel2.Controls.Add(this.btnDeleteAnalog);
            this.easyPanel2.Controls.Add(this.btnSaveAnalog);
            this.easyPanel2.Controls.Add(this.btnCancelAnalog);
            this.easyPanel2.Controls.Add(this.easyLabel7);
            this.easyPanel2.Controls.Add(this.easyLabel6);
            this.easyPanel2.Controls.Add(this.easyLabel5);
            this.easyPanel2.Controls.Add(this.easyLabel4);
            this.easyPanel2.Controls.Add(this.cobCompareModeAnalog);
            this.easyPanel2.Controls.Add(this.txbTriggerTagAnalog);
            this.easyPanel2.Controls.Add(this.easyLabel2);
            this.easyPanel2.Controls.Add(this.easyLabel1);
            this.easyPanel2.Controls.Add(this.analogGridView);
            this.easyPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel2.Location = new System.Drawing.Point(0, 0);
            this.easyPanel2.Name = "easyPanel2";
            this.easyPanel2.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel2.Size = new System.Drawing.Size(662, 576);
            this.easyPanel2.TabIndex = 0;
            // 
            // numDelayAnalog
            // 
            this.numDelayAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDelayAnalog.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numDelayAnalog.Location = new System.Drawing.Point(515, 70);
            this.numDelayAnalog.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numDelayAnalog.Name = "numDelayAnalog";
            this.numDelayAnalog.Size = new System.Drawing.Size(140, 22);
            this.numDelayAnalog.TabIndex = 41;
            // 
            // ckbEnabledAnalog
            // 
            this.ckbEnabledAnalog.Location = new System.Drawing.Point(97, 12);
            this.ckbEnabledAnalog.Name = "ckbEnabledAnalog";
            this.ckbEnabledAnalog.Size = new System.Drawing.Size(67, 20);
            this.ckbEnabledAnalog.TabIndex = 37;
            this.ckbEnabledAnalog.Values.Text = "Enabled";
            // 
            // txbMinValueAnalog
            // 
            this.txbMinValueAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbMinValueAnalog.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseMinValueAnalog});
            this.txbMinValueAnalog.Location = new System.Drawing.Point(515, 10);
            this.txbMinValueAnalog.Name = "txbMinValueAnalog";
            this.txbMinValueAnalog.Size = new System.Drawing.Size(140, 23);
            this.txbMinValueAnalog.TabIndex = 35;
            // 
            // btnBrowseMinValueAnalog
            // 
            this.btnBrowseMinValueAnalog.Image = global::EasyScada.Winforms.Controls.Properties.Resources.script_edit;
            this.btnBrowseMinValueAnalog.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseMinValueAnalog.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseMinValueAnalog.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseMinValueAnalog.UniqueName = "17BA7218BD974E7F87B64F3A03F1FC49";
            // 
            // txbMaxValueAnalog
            // 
            this.txbMaxValueAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbMaxValueAnalog.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseMaxValueAnalog});
            this.txbMaxValueAnalog.Location = new System.Drawing.Point(515, 40);
            this.txbMaxValueAnalog.Name = "txbMaxValueAnalog";
            this.txbMaxValueAnalog.Size = new System.Drawing.Size(140, 23);
            this.txbMaxValueAnalog.TabIndex = 34;
            // 
            // btnBrowseMaxValueAnalog
            // 
            this.btnBrowseMaxValueAnalog.Image = global::EasyScada.Winforms.Controls.Properties.Resources.script_edit;
            this.btnBrowseMaxValueAnalog.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseMaxValueAnalog.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseMaxValueAnalog.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseMaxValueAnalog.UniqueName = "17BA7218BD974E7F87B64F3A03F1FC49";
            // 
            // txbDescriptionAnalog
            // 
            this.txbDescriptionAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbDescriptionAnalog.Location = new System.Drawing.Point(97, 70);
            this.txbDescriptionAnalog.Multiline = true;
            this.txbDescriptionAnalog.Name = "txbDescriptionAnalog";
            this.txbDescriptionAnalog.Size = new System.Drawing.Size(309, 48);
            this.txbDescriptionAnalog.TabIndex = 31;
            // 
            // btnAddAnalog
            // 
            this.btnAddAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAnalog.Location = new System.Drawing.Point(271, 127);
            this.btnAddAnalog.Name = "btnAddAnalog";
            this.btnAddAnalog.Size = new System.Drawing.Size(72, 25);
            this.btnAddAnalog.TabIndex = 29;
            this.btnAddAnalog.Values.Text = "Add";
            // 
            // btnEditAnalog
            // 
            this.btnEditAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditAnalog.Location = new System.Drawing.Point(349, 127);
            this.btnEditAnalog.Name = "btnEditAnalog";
            this.btnEditAnalog.Size = new System.Drawing.Size(72, 25);
            this.btnEditAnalog.TabIndex = 28;
            this.btnEditAnalog.Values.Text = "Edit";
            // 
            // btnDeleteAnalog
            // 
            this.btnDeleteAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAnalog.Location = new System.Drawing.Point(427, 127);
            this.btnDeleteAnalog.Name = "btnDeleteAnalog";
            this.btnDeleteAnalog.Size = new System.Drawing.Size(72, 25);
            this.btnDeleteAnalog.TabIndex = 27;
            this.btnDeleteAnalog.Values.Text = "Delete";
            // 
            // btnSaveAnalog
            // 
            this.btnSaveAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAnalog.Location = new System.Drawing.Point(505, 127);
            this.btnSaveAnalog.Name = "btnSaveAnalog";
            this.btnSaveAnalog.Size = new System.Drawing.Size(72, 25);
            this.btnSaveAnalog.TabIndex = 26;
            this.btnSaveAnalog.Values.Text = "Save";
            // 
            // btnCancelAnalog
            // 
            this.btnCancelAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelAnalog.Location = new System.Drawing.Point(584, 127);
            this.btnCancelAnalog.Name = "btnCancelAnalog";
            this.btnCancelAnalog.Size = new System.Drawing.Size(72, 25);
            this.btnCancelAnalog.TabIndex = 25;
            this.btnCancelAnalog.Values.Text = "Cancel";
            // 
            // easyLabel7
            // 
            this.easyLabel7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel7.Location = new System.Drawing.Point(412, 97);
            this.easyLabel7.Name = "easyLabel7";
            this.easyLabel7.Size = new System.Drawing.Size(98, 20);
            this.easyLabel7.TabIndex = 19;
            this.easyLabel7.Values.Text = "Compare mode:";
            // 
            // easyLabel6
            // 
            this.easyLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel6.Location = new System.Drawing.Point(412, 70);
            this.easyLabel6.Name = "easyLabel6";
            this.easyLabel6.Size = new System.Drawing.Size(70, 20);
            this.easyLabel6.TabIndex = 18;
            this.easyLabel6.Values.Text = "Delay (ms):";
            // 
            // easyLabel5
            // 
            this.easyLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel5.Location = new System.Drawing.Point(412, 42);
            this.easyLabel5.Name = "easyLabel5";
            this.easyLabel5.Size = new System.Drawing.Size(68, 20);
            this.easyLabel5.TabIndex = 17;
            this.easyLabel5.Values.Text = "Max value:";
            // 
            // easyLabel4
            // 
            this.easyLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel4.Location = new System.Drawing.Point(412, 12);
            this.easyLabel4.Name = "easyLabel4";
            this.easyLabel4.Size = new System.Drawing.Size(66, 20);
            this.easyLabel4.TabIndex = 16;
            this.easyLabel4.Values.Text = "Min value:";
            // 
            // cobCompareModeAnalog
            // 
            this.cobCompareModeAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cobCompareModeAnalog.DropDownWidth = 140;
            this.cobCompareModeAnalog.Location = new System.Drawing.Point(515, 97);
            this.cobCompareModeAnalog.Name = "cobCompareModeAnalog";
            this.cobCompareModeAnalog.Size = new System.Drawing.Size(140, 21);
            this.cobCompareModeAnalog.TabIndex = 11;
            // 
            // txbTriggerTagAnalog
            // 
            this.txbTriggerTagAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTriggerTagAnalog.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseTriggerTagAnalog});
            this.txbTriggerTagAnalog.Location = new System.Drawing.Point(97, 40);
            this.txbTriggerTagAnalog.Name = "txbTriggerTagAnalog";
            this.txbTriggerTagAnalog.Size = new System.Drawing.Size(309, 23);
            this.txbTriggerTagAnalog.TabIndex = 8;
            // 
            // btnBrowseTriggerTagAnalog
            // 
            this.btnBrowseTriggerTagAnalog.Image = global::EasyScada.Winforms.Controls.Properties.Resources.script_edit;
            this.btnBrowseTriggerTagAnalog.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseTriggerTagAnalog.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseTriggerTagAnalog.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseTriggerTagAnalog.UniqueName = "17BA7218BD974E7F87B64F3A03F1FC49";
            // 
            // easyLabel2
            // 
            this.easyLabel2.Location = new System.Drawing.Point(7, 70);
            this.easyLabel2.Name = "easyLabel2";
            this.easyLabel2.Size = new System.Drawing.Size(75, 20);
            this.easyLabel2.TabIndex = 7;
            this.easyLabel2.Values.Text = "Description:";
            // 
            // easyLabel1
            // 
            this.easyLabel1.Location = new System.Drawing.Point(7, 42);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Size = new System.Drawing.Size(73, 20);
            this.easyLabel1.TabIndex = 6;
            this.easyLabel1.Values.Text = "Trigger tag:";
            // 
            // analogGridView
            // 
            this.analogGridView.AllowUserToAddRows = false;
            this.analogGridView.AllowUserToDeleteRows = false;
            this.analogGridView.AllowUserToResizeRows = false;
            this.analogGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.analogGridView.ColumnHeadersHeight = 25;
            this.analogGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.analogGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAnalogEnabled,
            this.colAnalogTriggerTag,
            this.colAnalogMinValue,
            this.colAnalogMaxValue,
            this.colAnalogCompareMode,
            this.colAnalogDelay,
            this.colAnalogDescription});
            this.analogGridView.GridStyles.Style = EasyScada.Winforms.Controls.DataGridViewStyle.Mixed;
            this.analogGridView.GridStyles.StyleBackground = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.analogGridView.HideOuterBorders = true;
            this.analogGridView.Location = new System.Drawing.Point(7, 158);
            this.analogGridView.Name = "analogGridView";
            this.analogGridView.RowHeadersVisible = false;
            this.analogGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.analogGridView.Size = new System.Drawing.Size(648, 411);
            this.analogGridView.TabIndex = 4;
            // 
            // colAnalogEnabled
            // 
            this.colAnalogEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colAnalogEnabled.DataPropertyName = "Enabled";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.NullValue = false;
            this.colAnalogEnabled.DefaultCellStyle = dataGridViewCellStyle10;
            this.colAnalogEnabled.FalseValue = null;
            this.colAnalogEnabled.HeaderText = "Enabled";
            this.colAnalogEnabled.IndeterminateValue = null;
            this.colAnalogEnabled.Name = "colAnalogEnabled";
            this.colAnalogEnabled.ReadOnly = true;
            this.colAnalogEnabled.TrueValue = null;
            this.colAnalogEnabled.Width = 59;
            // 
            // colAnalogTriggerTag
            // 
            this.colAnalogTriggerTag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAnalogTriggerTag.DataPropertyName = "TriggerTagPath";
            this.colAnalogTriggerTag.HeaderText = "Trigger Tag";
            this.colAnalogTriggerTag.Name = "colAnalogTriggerTag";
            this.colAnalogTriggerTag.ReadOnly = true;
            this.colAnalogTriggerTag.Width = 93;
            // 
            // colAnalogMinValue
            // 
            this.colAnalogMinValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAnalogMinValue.DataPropertyName = "MinValue";
            this.colAnalogMinValue.HeaderText = "Min Value";
            this.colAnalogMinValue.Name = "colAnalogMinValue";
            this.colAnalogMinValue.ReadOnly = true;
            this.colAnalogMinValue.Width = 88;
            // 
            // colAnalogMaxValue
            // 
            this.colAnalogMaxValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAnalogMaxValue.DataPropertyName = "MaxValue";
            this.colAnalogMaxValue.HeaderText = "Max Value";
            this.colAnalogMaxValue.Name = "colAnalogMaxValue";
            this.colAnalogMaxValue.ReadOnly = true;
            this.colAnalogMaxValue.Width = 90;
            // 
            // colAnalogCompareMode
            // 
            this.colAnalogCompareMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colAnalogCompareMode.DataPropertyName = "CompareMode";
            this.colAnalogCompareMode.DropDownWidth = 121;
            this.colAnalogCompareMode.HeaderText = "Compare Mode";
            this.colAnalogCompareMode.Name = "colAnalogCompareMode";
            this.colAnalogCompareMode.ReadOnly = true;
            this.colAnalogCompareMode.Width = 100;
            // 
            // colAnalogDelay
            // 
            this.colAnalogDelay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAnalogDelay.DataPropertyName = "Delay";
            this.colAnalogDelay.HeaderText = "Delay";
            this.colAnalogDelay.Name = "colAnalogDelay";
            this.colAnalogDelay.ReadOnly = true;
            this.colAnalogDelay.Width = 65;
            // 
            // colAnalogDescription
            // 
            this.colAnalogDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAnalogDescription.DataPropertyName = "Descripntion";
            this.colAnalogDescription.HeaderText = "Description";
            this.colAnalogDescription.Name = "colAnalogDescription";
            this.colAnalogDescription.ReadOnly = true;
            this.colAnalogDescription.Width = 96;
            // 
            // easyPage2
            // 
            this.easyPage2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.easyPage2.Controls.Add(this.easyPanel3);
            this.easyPage2.Flags = 65534;
            this.easyPage2.LastVisibleSet = true;
            this.easyPage2.MinimumSize = new System.Drawing.Size(50, 50);
            this.easyPage2.Name = "easyPage2";
            this.easyPage2.Size = new System.Drawing.Size(662, 576);
            this.easyPage2.Text = "Discrete triggers";
            this.easyPage2.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.easyPage2.ToolTipTitle = "Page ToolTip";
            this.easyPage2.UniqueName = "AD8592C1B3C84C6032934415EA08020E";
            // 
            // easyPanel3
            // 
            this.easyPanel3.Controls.Add(this.numDelayDiscrete);
            this.easyPanel3.Controls.Add(this.ckbEnabledDiscrete);
            this.easyPanel3.Controls.Add(this.txbTriggerValueDiscrete);
            this.easyPanel3.Controls.Add(this.txbDescriptionDiscrete);
            this.easyPanel3.Controls.Add(this.btnAddDiscrete);
            this.easyPanel3.Controls.Add(this.btnEditDiscrete);
            this.easyPanel3.Controls.Add(this.btnDeleteDiscrete);
            this.easyPanel3.Controls.Add(this.btnSaveDiscrete);
            this.easyPanel3.Controls.Add(this.btnCancelDiscrete);
            this.easyPanel3.Controls.Add(this.easyLabel3);
            this.easyPanel3.Controls.Add(this.easyLabel8);
            this.easyPanel3.Controls.Add(this.easyLabel9);
            this.easyPanel3.Controls.Add(this.cobCompareModeDiscrete);
            this.easyPanel3.Controls.Add(this.txbTriggerTagDiscrete);
            this.easyPanel3.Controls.Add(this.easyLabel11);
            this.easyPanel3.Controls.Add(this.easyLabel12);
            this.easyPanel3.Controls.Add(this.discreteGridView);
            this.easyPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel3.Location = new System.Drawing.Point(0, 0);
            this.easyPanel3.Name = "easyPanel3";
            this.easyPanel3.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel3.Size = new System.Drawing.Size(662, 576);
            this.easyPanel3.TabIndex = 1;
            // 
            // numDelayDiscrete
            // 
            this.numDelayDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDelayDiscrete.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numDelayDiscrete.Location = new System.Drawing.Point(515, 70);
            this.numDelayDiscrete.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numDelayDiscrete.Name = "numDelayDiscrete";
            this.numDelayDiscrete.Size = new System.Drawing.Size(140, 22);
            this.numDelayDiscrete.TabIndex = 40;
            // 
            // ckbEnabledDiscrete
            // 
            this.ckbEnabledDiscrete.Location = new System.Drawing.Point(97, 12);
            this.ckbEnabledDiscrete.Name = "ckbEnabledDiscrete";
            this.ckbEnabledDiscrete.Size = new System.Drawing.Size(67, 20);
            this.ckbEnabledDiscrete.TabIndex = 37;
            this.ckbEnabledDiscrete.Values.Text = "Enabled";
            // 
            // txbTriggerValueDiscrete
            // 
            this.txbTriggerValueDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTriggerValueDiscrete.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseTriggerValueDiscrete});
            this.txbTriggerValueDiscrete.Location = new System.Drawing.Point(515, 40);
            this.txbTriggerValueDiscrete.Name = "txbTriggerValueDiscrete";
            this.txbTriggerValueDiscrete.Size = new System.Drawing.Size(140, 23);
            this.txbTriggerValueDiscrete.TabIndex = 34;
            // 
            // btnBrowseTriggerValueDiscrete
            // 
            this.btnBrowseTriggerValueDiscrete.Image = global::EasyScada.Winforms.Controls.Properties.Resources.script_edit;
            this.btnBrowseTriggerValueDiscrete.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseTriggerValueDiscrete.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseTriggerValueDiscrete.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseTriggerValueDiscrete.UniqueName = "17BA7218BD974E7F87B64F3A03F1FC49";
            // 
            // txbDescriptionDiscrete
            // 
            this.txbDescriptionDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbDescriptionDiscrete.Location = new System.Drawing.Point(97, 70);
            this.txbDescriptionDiscrete.Multiline = true;
            this.txbDescriptionDiscrete.Name = "txbDescriptionDiscrete";
            this.txbDescriptionDiscrete.Size = new System.Drawing.Size(309, 48);
            this.txbDescriptionDiscrete.TabIndex = 31;
            // 
            // btnAddDiscrete
            // 
            this.btnAddDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDiscrete.Location = new System.Drawing.Point(271, 127);
            this.btnAddDiscrete.Name = "btnAddDiscrete";
            this.btnAddDiscrete.Size = new System.Drawing.Size(72, 25);
            this.btnAddDiscrete.TabIndex = 29;
            this.btnAddDiscrete.Values.Text = "Add";
            // 
            // btnEditDiscrete
            // 
            this.btnEditDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditDiscrete.Location = new System.Drawing.Point(349, 127);
            this.btnEditDiscrete.Name = "btnEditDiscrete";
            this.btnEditDiscrete.Size = new System.Drawing.Size(72, 25);
            this.btnEditDiscrete.TabIndex = 28;
            this.btnEditDiscrete.Values.Text = "Edit";
            // 
            // btnDeleteDiscrete
            // 
            this.btnDeleteDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteDiscrete.Location = new System.Drawing.Point(427, 127);
            this.btnDeleteDiscrete.Name = "btnDeleteDiscrete";
            this.btnDeleteDiscrete.Size = new System.Drawing.Size(72, 25);
            this.btnDeleteDiscrete.TabIndex = 27;
            this.btnDeleteDiscrete.Values.Text = "Delete";
            // 
            // btnSaveDiscrete
            // 
            this.btnSaveDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDiscrete.Location = new System.Drawing.Point(505, 127);
            this.btnSaveDiscrete.Name = "btnSaveDiscrete";
            this.btnSaveDiscrete.Size = new System.Drawing.Size(72, 25);
            this.btnSaveDiscrete.TabIndex = 26;
            this.btnSaveDiscrete.Values.Text = "Save";
            // 
            // btnCancelDiscrete
            // 
            this.btnCancelDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelDiscrete.Location = new System.Drawing.Point(584, 127);
            this.btnCancelDiscrete.Name = "btnCancelDiscrete";
            this.btnCancelDiscrete.Size = new System.Drawing.Size(72, 25);
            this.btnCancelDiscrete.TabIndex = 25;
            this.btnCancelDiscrete.Values.Text = "Cancel";
            // 
            // easyLabel3
            // 
            this.easyLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel3.Location = new System.Drawing.Point(412, 97);
            this.easyLabel3.Name = "easyLabel3";
            this.easyLabel3.Size = new System.Drawing.Size(98, 20);
            this.easyLabel3.TabIndex = 19;
            this.easyLabel3.Values.Text = "Compare mode:";
            // 
            // easyLabel8
            // 
            this.easyLabel8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel8.Location = new System.Drawing.Point(412, 70);
            this.easyLabel8.Name = "easyLabel8";
            this.easyLabel8.Size = new System.Drawing.Size(70, 20);
            this.easyLabel8.TabIndex = 18;
            this.easyLabel8.Values.Text = "Delay (ms):";
            // 
            // easyLabel9
            // 
            this.easyLabel9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel9.Location = new System.Drawing.Point(412, 42);
            this.easyLabel9.Name = "easyLabel9";
            this.easyLabel9.Size = new System.Drawing.Size(84, 20);
            this.easyLabel9.TabIndex = 17;
            this.easyLabel9.Values.Text = "Trigger value:";
            // 
            // cobCompareModeDiscrete
            // 
            this.cobCompareModeDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cobCompareModeDiscrete.DropDownWidth = 140;
            this.cobCompareModeDiscrete.Location = new System.Drawing.Point(515, 97);
            this.cobCompareModeDiscrete.Name = "cobCompareModeDiscrete";
            this.cobCompareModeDiscrete.Size = new System.Drawing.Size(140, 21);
            this.cobCompareModeDiscrete.TabIndex = 11;
            // 
            // txbTriggerTagDiscrete
            // 
            this.txbTriggerTagDiscrete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTriggerTagDiscrete.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseTriggerTagDiscrete});
            this.txbTriggerTagDiscrete.Location = new System.Drawing.Point(97, 40);
            this.txbTriggerTagDiscrete.Name = "txbTriggerTagDiscrete";
            this.txbTriggerTagDiscrete.Size = new System.Drawing.Size(309, 23);
            this.txbTriggerTagDiscrete.TabIndex = 8;
            // 
            // btnBrowseTriggerTagDiscrete
            // 
            this.btnBrowseTriggerTagDiscrete.Image = global::EasyScada.Winforms.Controls.Properties.Resources.script_edit;
            this.btnBrowseTriggerTagDiscrete.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseTriggerTagDiscrete.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseTriggerTagDiscrete.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseTriggerTagDiscrete.UniqueName = "17BA7218BD974E7F87B64F3A03F1FC49";
            // 
            // easyLabel11
            // 
            this.easyLabel11.Location = new System.Drawing.Point(7, 70);
            this.easyLabel11.Name = "easyLabel11";
            this.easyLabel11.Size = new System.Drawing.Size(75, 20);
            this.easyLabel11.TabIndex = 7;
            this.easyLabel11.Values.Text = "Description:";
            // 
            // easyLabel12
            // 
            this.easyLabel12.Location = new System.Drawing.Point(7, 42);
            this.easyLabel12.Name = "easyLabel12";
            this.easyLabel12.Size = new System.Drawing.Size(73, 20);
            this.easyLabel12.TabIndex = 6;
            this.easyLabel12.Values.Text = "Trigger tag:";
            // 
            // discreteGridView
            // 
            this.discreteGridView.AllowUserToAddRows = false;
            this.discreteGridView.AllowUserToDeleteRows = false;
            this.discreteGridView.AllowUserToResizeRows = false;
            this.discreteGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.discreteGridView.ColumnHeadersHeight = 25;
            this.discreteGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.discreteGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.easyDataGridViewCheckBoxColumn1,
            this.easyDataGridViewTextBoxColumn1,
            this.easyDataGridViewTextBoxColumn2,
            this.easyDataGridViewComboBoxColumn1,
            this.easyDataGridViewTextBoxColumn4,
            this.easyDataGridViewTextBoxColumn5});
            this.discreteGridView.GridStyles.Style = EasyScada.Winforms.Controls.DataGridViewStyle.Mixed;
            this.discreteGridView.GridStyles.StyleBackground = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.discreteGridView.HideOuterBorders = true;
            this.discreteGridView.Location = new System.Drawing.Point(7, 158);
            this.discreteGridView.Name = "discreteGridView";
            this.discreteGridView.RowHeadersVisible = false;
            this.discreteGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.discreteGridView.Size = new System.Drawing.Size(648, 411);
            this.discreteGridView.TabIndex = 4;
            // 
            // easyDataGridViewCheckBoxColumn1
            // 
            this.easyDataGridViewCheckBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.easyDataGridViewCheckBoxColumn1.DataPropertyName = "Enabled";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.NullValue = false;
            this.easyDataGridViewCheckBoxColumn1.DefaultCellStyle = dataGridViewCellStyle11;
            this.easyDataGridViewCheckBoxColumn1.FalseValue = null;
            this.easyDataGridViewCheckBoxColumn1.HeaderText = "Enabled";
            this.easyDataGridViewCheckBoxColumn1.IndeterminateValue = null;
            this.easyDataGridViewCheckBoxColumn1.Name = "easyDataGridViewCheckBoxColumn1";
            this.easyDataGridViewCheckBoxColumn1.ReadOnly = true;
            this.easyDataGridViewCheckBoxColumn1.TrueValue = null;
            this.easyDataGridViewCheckBoxColumn1.Width = 59;
            // 
            // easyDataGridViewTextBoxColumn1
            // 
            this.easyDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.easyDataGridViewTextBoxColumn1.DataPropertyName = "TriggerTagPath";
            this.easyDataGridViewTextBoxColumn1.HeaderText = "Trigger Tag";
            this.easyDataGridViewTextBoxColumn1.Name = "easyDataGridViewTextBoxColumn1";
            this.easyDataGridViewTextBoxColumn1.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn1.Width = 93;
            // 
            // easyDataGridViewTextBoxColumn2
            // 
            this.easyDataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.easyDataGridViewTextBoxColumn2.DataPropertyName = "TriggerValue";
            this.easyDataGridViewTextBoxColumn2.HeaderText = "Trigger Value";
            this.easyDataGridViewTextBoxColumn2.Name = "easyDataGridViewTextBoxColumn2";
            this.easyDataGridViewTextBoxColumn2.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn2.Width = 103;
            // 
            // easyDataGridViewComboBoxColumn1
            // 
            this.easyDataGridViewComboBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.easyDataGridViewComboBoxColumn1.DataPropertyName = "CompareMode";
            this.easyDataGridViewComboBoxColumn1.DropDownWidth = 121;
            this.easyDataGridViewComboBoxColumn1.HeaderText = "Compare Mode";
            this.easyDataGridViewComboBoxColumn1.Name = "easyDataGridViewComboBoxColumn1";
            this.easyDataGridViewComboBoxColumn1.ReadOnly = true;
            this.easyDataGridViewComboBoxColumn1.Width = 100;
            // 
            // easyDataGridViewTextBoxColumn4
            // 
            this.easyDataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.easyDataGridViewTextBoxColumn4.DataPropertyName = "Delay";
            this.easyDataGridViewTextBoxColumn4.HeaderText = "Delay";
            this.easyDataGridViewTextBoxColumn4.Name = "easyDataGridViewTextBoxColumn4";
            this.easyDataGridViewTextBoxColumn4.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn4.Width = 65;
            // 
            // easyDataGridViewTextBoxColumn5
            // 
            this.easyDataGridViewTextBoxColumn5.DataPropertyName = "Description";
            this.easyDataGridViewTextBoxColumn5.HeaderText = "Description";
            this.easyDataGridViewTextBoxColumn5.Name = "easyDataGridViewTextBoxColumn5";
            this.easyDataGridViewTextBoxColumn5.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn5.Width = 188;
            // 
            // easyPage3
            // 
            this.easyPage3.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.easyPage3.Controls.Add(this.easyPanel4);
            this.easyPage3.Flags = 65534;
            this.easyPage3.LastVisibleSet = true;
            this.easyPage3.MinimumSize = new System.Drawing.Size(50, 50);
            this.easyPage3.Name = "easyPage3";
            this.easyPage3.Size = new System.Drawing.Size(662, 576);
            this.easyPage3.Text = "Quality triggers";
            this.easyPage3.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.easyPage3.ToolTipTitle = "Page ToolTip";
            this.easyPage3.UniqueName = "E376D4CB51F64DC1C2A5362DF83A3C97";
            // 
            // easyPanel4
            // 
            this.easyPanel4.Controls.Add(this.tableLayoutPanel1);
            this.easyPanel4.Controls.Add(this.btnAddQuality);
            this.easyPanel4.Controls.Add(this.btnEditQuality);
            this.easyPanel4.Controls.Add(this.btnDeleteQuality);
            this.easyPanel4.Controls.Add(this.btnSaveQuality);
            this.easyPanel4.Controls.Add(this.btnCancelQuality);
            this.easyPanel4.Controls.Add(this.qualityGridView);
            this.easyPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel4.Location = new System.Drawing.Point(0, 0);
            this.easyPanel4.Name = "easyPanel4";
            this.easyPanel4.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel4.Size = new System.Drawing.Size(662, 576);
            this.easyPanel4.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(206)))), ((int)(((byte)(230)))));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.Controls.Add(this.numDelayQuality, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.cobCompareModeQuality, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.cobQuality, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.themedLabel5, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.themedLabel3, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.themedLabel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ckbEnabledQuality, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txbTriggerTagQuality, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.themedLabel4, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.txbDescriptionQuality, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.themedLabel2, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 9);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(649, 112);
            this.tableLayoutPanel1.TabIndex = 40;
            // 
            // numDelayQuality
            // 
            this.numDelayQuality.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numDelayQuality.Location = new System.Drawing.Point(532, 56);
            this.numDelayQuality.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numDelayQuality.Name = "numDelayQuality";
            this.numDelayQuality.Size = new System.Drawing.Size(114, 22);
            this.numDelayQuality.TabIndex = 45;
            // 
            // cobCompareModeQuality
            // 
            this.cobCompareModeQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cobCompareModeQuality.DropDownWidth = 140;
            this.cobCompareModeQuality.Location = new System.Drawing.Point(532, 84);
            this.cobCompareModeQuality.Name = "cobCompareModeQuality";
            this.cobCompareModeQuality.Size = new System.Drawing.Size(114, 21);
            this.cobCompareModeQuality.TabIndex = 41;
            // 
            // cobQuality
            // 
            this.cobQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cobQuality.DropDownWidth = 140;
            this.cobQuality.Location = new System.Drawing.Point(532, 29);
            this.cobQuality.Name = "cobQuality";
            this.cobQuality.Size = new System.Drawing.Size(114, 21);
            this.cobQuality.TabIndex = 41;
            // 
            // themedLabel5
            // 
            this.themedLabel5.Location = new System.Drawing.Point(428, 84);
            this.themedLabel5.Name = "themedLabel5";
            this.themedLabel5.Size = new System.Drawing.Size(98, 20);
            this.themedLabel5.TabIndex = 41;
            this.themedLabel5.Values.Text = "Compare mode:";
            // 
            // themedLabel3
            // 
            this.themedLabel3.Location = new System.Drawing.Point(428, 29);
            this.themedLabel3.Name = "themedLabel3";
            this.themedLabel3.Size = new System.Drawing.Size(92, 20);
            this.themedLabel3.TabIndex = 41;
            this.themedLabel3.Values.Text = "Trigger quality:";
            // 
            // themedLabel2
            // 
            this.themedLabel2.Location = new System.Drawing.Point(3, 84);
            this.themedLabel2.Name = "themedLabel2";
            this.themedLabel2.Size = new System.Drawing.Size(75, 20);
            this.themedLabel2.TabIndex = 41;
            this.themedLabel2.Values.Text = "Description:";
            // 
            // themedLabel1
            // 
            this.themedLabel1.Location = new System.Drawing.Point(3, 29);
            this.themedLabel1.Name = "themedLabel1";
            this.themedLabel1.Size = new System.Drawing.Size(73, 20);
            this.themedLabel1.TabIndex = 41;
            this.themedLabel1.Values.Text = "Trigger tag:";
            // 
            // ckbEnabledQuality
            // 
            this.ckbEnabledQuality.Location = new System.Drawing.Point(84, 3);
            this.ckbEnabledQuality.Name = "ckbEnabledQuality";
            this.ckbEnabledQuality.Size = new System.Drawing.Size(67, 20);
            this.ckbEnabledQuality.TabIndex = 41;
            this.ckbEnabledQuality.Values.Text = "Enabled";
            // 
            // txbTriggerTagQuality
            // 
            this.txbTriggerTagQuality.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecAny[] {
            this.btnBrowseTriggerTagQuality});
            this.txbTriggerTagQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbTriggerTagQuality.Location = new System.Drawing.Point(84, 29);
            this.txbTriggerTagQuality.Multiline = true;
            this.txbTriggerTagQuality.Name = "txbTriggerTagQuality";
            this.tableLayoutPanel1.SetRowSpan(this.txbTriggerTagQuality, 2);
            this.txbTriggerTagQuality.Size = new System.Drawing.Size(338, 49);
            this.txbTriggerTagQuality.TabIndex = 42;
            // 
            // btnBrowseTriggerTagQuality
            // 
            this.btnBrowseTriggerTagQuality.Image = global::EasyScada.Winforms.Controls.Properties.Resources.script_edit;
            this.btnBrowseTriggerTagQuality.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.InputControl;
            this.btnBrowseTriggerTagQuality.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnBrowseTriggerTagQuality.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnBrowseTriggerTagQuality.UniqueName = "17BA7218BD974E7F87B64F3A03F1FC49";
            // 
            // themedLabel4
            // 
            this.themedLabel4.Location = new System.Drawing.Point(428, 56);
            this.themedLabel4.Name = "themedLabel4";
            this.themedLabel4.Size = new System.Drawing.Size(70, 20);
            this.themedLabel4.TabIndex = 41;
            this.themedLabel4.Values.Text = "Delay (ms):";
            // 
            // txbDescriptionQuality
            // 
            this.txbDescriptionQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbDescriptionQuality.Location = new System.Drawing.Point(84, 84);
            this.txbDescriptionQuality.Multiline = true;
            this.txbDescriptionQuality.Name = "txbDescriptionQuality";
            this.txbDescriptionQuality.Size = new System.Drawing.Size(338, 25);
            this.txbDescriptionQuality.TabIndex = 43;
            // 
            // btnAddQuality
            // 
            this.btnAddQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddQuality.Location = new System.Drawing.Point(271, 127);
            this.btnAddQuality.Name = "btnAddQuality";
            this.btnAddQuality.Size = new System.Drawing.Size(72, 25);
            this.btnAddQuality.TabIndex = 29;
            this.btnAddQuality.Values.Text = "Add";
            // 
            // btnEditQuality
            // 
            this.btnEditQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditQuality.Location = new System.Drawing.Point(349, 127);
            this.btnEditQuality.Name = "btnEditQuality";
            this.btnEditQuality.Size = new System.Drawing.Size(72, 25);
            this.btnEditQuality.TabIndex = 28;
            this.btnEditQuality.Values.Text = "Edit";
            // 
            // btnDeleteQuality
            // 
            this.btnDeleteQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteQuality.Location = new System.Drawing.Point(427, 127);
            this.btnDeleteQuality.Name = "btnDeleteQuality";
            this.btnDeleteQuality.Size = new System.Drawing.Size(72, 25);
            this.btnDeleteQuality.TabIndex = 27;
            this.btnDeleteQuality.Values.Text = "Delete";
            // 
            // btnSaveQuality
            // 
            this.btnSaveQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveQuality.Location = new System.Drawing.Point(505, 127);
            this.btnSaveQuality.Name = "btnSaveQuality";
            this.btnSaveQuality.Size = new System.Drawing.Size(72, 25);
            this.btnSaveQuality.TabIndex = 26;
            this.btnSaveQuality.Values.Text = "Save";
            // 
            // btnCancelQuality
            // 
            this.btnCancelQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelQuality.Location = new System.Drawing.Point(584, 127);
            this.btnCancelQuality.Name = "btnCancelQuality";
            this.btnCancelQuality.Size = new System.Drawing.Size(72, 25);
            this.btnCancelQuality.TabIndex = 25;
            this.btnCancelQuality.Values.Text = "Cancel";
            // 
            // qualityGridView
            // 
            this.qualityGridView.AllowUserToAddRows = false;
            this.qualityGridView.AllowUserToDeleteRows = false;
            this.qualityGridView.AllowUserToResizeRows = false;
            this.qualityGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualityGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.qualityGridView.ColumnHeadersHeight = 25;
            this.qualityGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.qualityGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.easyDataGridViewCheckBoxColumn2,
            this.easyDataGridViewTextBoxColumn3,
            this.easyDataGridViewTextBoxColumn6,
            this.easyDataGridViewComboBoxColumn2,
            this.easyDataGridViewTextBoxColumn7,
            this.easyDataGridViewTextBoxColumn8});
            this.qualityGridView.GridStyles.Style = EasyScada.Winforms.Controls.DataGridViewStyle.Mixed;
            this.qualityGridView.GridStyles.StyleBackground = EasyScada.Winforms.Controls.PaletteBackStyle.ControlClient;
            this.qualityGridView.HideOuterBorders = true;
            this.qualityGridView.Location = new System.Drawing.Point(7, 158);
            this.qualityGridView.MultiSelect = false;
            this.qualityGridView.Name = "qualityGridView";
            this.qualityGridView.ReadOnly = true;
            this.qualityGridView.RowHeadersVisible = false;
            this.qualityGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.qualityGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.qualityGridView.Size = new System.Drawing.Size(648, 411);
            this.qualityGridView.TabIndex = 4;
            // 
            // easyDataGridViewCheckBoxColumn2
            // 
            this.easyDataGridViewCheckBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.easyDataGridViewCheckBoxColumn2.DataPropertyName = "Enabled";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.NullValue = false;
            this.easyDataGridViewCheckBoxColumn2.DefaultCellStyle = dataGridViewCellStyle12;
            this.easyDataGridViewCheckBoxColumn2.FalseValue = null;
            this.easyDataGridViewCheckBoxColumn2.HeaderText = "Enabled";
            this.easyDataGridViewCheckBoxColumn2.IndeterminateValue = null;
            this.easyDataGridViewCheckBoxColumn2.Name = "easyDataGridViewCheckBoxColumn2";
            this.easyDataGridViewCheckBoxColumn2.ReadOnly = true;
            this.easyDataGridViewCheckBoxColumn2.TrueValue = null;
            this.easyDataGridViewCheckBoxColumn2.Width = 59;
            // 
            // easyDataGridViewTextBoxColumn3
            // 
            this.easyDataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.easyDataGridViewTextBoxColumn3.DataPropertyName = "TriggerTagPath";
            this.easyDataGridViewTextBoxColumn3.HeaderText = "Trigger Tag";
            this.easyDataGridViewTextBoxColumn3.Name = "easyDataGridViewTextBoxColumn3";
            this.easyDataGridViewTextBoxColumn3.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn3.Width = 93;
            // 
            // easyDataGridViewTextBoxColumn6
            // 
            this.easyDataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.easyDataGridViewTextBoxColumn6.DataPropertyName = "TriggerQuality";
            this.easyDataGridViewTextBoxColumn6.HeaderText = "Trigger Quality";
            this.easyDataGridViewTextBoxColumn6.Name = "easyDataGridViewTextBoxColumn6";
            this.easyDataGridViewTextBoxColumn6.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn6.Width = 113;
            // 
            // easyDataGridViewComboBoxColumn2
            // 
            this.easyDataGridViewComboBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.easyDataGridViewComboBoxColumn2.DataPropertyName = "CompareMode";
            this.easyDataGridViewComboBoxColumn2.HeaderText = "Compare Mode";
            this.easyDataGridViewComboBoxColumn2.Name = "easyDataGridViewComboBoxColumn2";
            this.easyDataGridViewComboBoxColumn2.ReadOnly = true;
            this.easyDataGridViewComboBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.easyDataGridViewComboBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.easyDataGridViewComboBoxColumn2.Width = 100;
            // 
            // easyDataGridViewTextBoxColumn7
            // 
            this.easyDataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.easyDataGridViewTextBoxColumn7.DataPropertyName = "Delay";
            this.easyDataGridViewTextBoxColumn7.HeaderText = "Delay";
            this.easyDataGridViewTextBoxColumn7.Name = "easyDataGridViewTextBoxColumn7";
            this.easyDataGridViewTextBoxColumn7.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn7.Width = 65;
            // 
            // easyDataGridViewTextBoxColumn8
            // 
            this.easyDataGridViewTextBoxColumn8.DataPropertyName = "Description";
            this.easyDataGridViewTextBoxColumn8.HeaderText = "Description";
            this.easyDataGridViewTextBoxColumn8.Name = "easyDataGridViewTextBoxColumn8";
            this.easyDataGridViewTextBoxColumn8.ReadOnly = true;
            this.easyDataGridViewTextBoxColumn8.Width = 96;
            // 
            // easyHeaderGroup2
            // 
            this.easyHeaderGroup2.ButtonSpecs.AddRange(new EasyScada.Winforms.Controls.ButtonSpecHeaderGroup[] {
            this.btnResetProperty});
            this.easyHeaderGroup2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyHeaderGroup2.HeaderVisibleSecondary = false;
            this.easyHeaderGroup2.Location = new System.Drawing.Point(0, 0);
            this.easyHeaderGroup2.Name = "easyHeaderGroup2";
            // 
            // easyHeaderGroup2.Panel
            // 
            this.easyHeaderGroup2.Panel.Controls.Add(this.propertyGrid);
            this.easyHeaderGroup2.Size = new System.Drawing.Size(303, 602);
            this.easyHeaderGroup2.TabIndex = 2;
            this.easyHeaderGroup2.ValuesPrimary.Heading = "Animate Properties";
            this.easyHeaderGroup2.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("easyHeaderGroup2.ValuesPrimary.Image")));
            // 
            // btnResetProperty
            // 
            this.btnResetProperty.Image = ((System.Drawing.Image)(resources.GetObject("btnResetProperty.Image")));
            this.btnResetProperty.Style = EasyScada.Winforms.Controls.PaletteButtonStyle.Inherit;
            this.btnResetProperty.ToolTipStyle = EasyScada.Winforms.Controls.LabelStyle.ToolTip;
            this.btnResetProperty.Type = EasyScada.Winforms.Controls.PaletteButtonSpecStyle.Generic;
            this.btnResetProperty.UniqueName = "0387A61B28E14029768E456ECB1F2791";
            // 
            // propertyGrid
            // 
            this.propertyGrid.BackColor = System.Drawing.SystemColors.Window;
            this.propertyGrid.ContextMenuStrip = this.contextMenuStrip1;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(301, 570);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.ViewBorderColor = System.Drawing.SystemColors.Window;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToDefaultToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(157, 26);
            // 
            // resetToDefaultToolStripMenuItem
            // 
            this.resetToDefaultToolStripMenuItem.Name = "resetToDefaultToolStripMenuItem";
            this.resetToDefaultToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.resetToDefaultToolStripMenuItem.Text = "Reset to default";
            // 
            // easyContextMenuItem1
            // 
            this.easyContextMenuItem1.Text = "Menu Item";
            // 
            // easyContextMenuItem2
            // 
            this.easyContextMenuItem2.Text = "Reset to default";
            // 
            // AnimateDesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 635);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "AnimateDesignerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Animates Configuration";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)(this.easyNavigator1)).EndInit();
            this.easyNavigator1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPage1)).EndInit();
            this.easyPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel2)).EndInit();
            this.easyPanel2.ResumeLayout(false);
            this.easyPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCompareModeAnalog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.analogGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyPage2)).EndInit();
            this.easyPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel3)).EndInit();
            this.easyPanel3.ResumeLayout(false);
            this.easyPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCompareModeDiscrete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discreteGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyPage3)).EndInit();
            this.easyPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyPanel4)).EndInit();
            this.easyPanel4.ResumeLayout(false);
            this.easyPanel4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobCompareModeQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2.Panel)).EndInit();
            this.easyHeaderGroup2.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2)).EndInit();
            this.easyHeaderGroup2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private EasyPanel easyPanel1;
        private EasySplitContainer easySplitContainer1;
        private Navigator.EasyNavigator easyNavigator1;
        private Navigator.EasyPage easyPage1;
        private Navigator.EasyPage easyPage2;
        private Navigator.EasyPage easyPage3;
        private EasyPanel easyPanel2;
        private EasyDataGridView analogGridView;
        private ThemedLabel easyLabel7;
        private ThemedLabel easyLabel6;
        private ThemedLabel easyLabel5;
        private ThemedLabel easyLabel4;
        private EasyComboBox cobCompareModeAnalog;
        private ThemedTextBox txbTriggerTagAnalog;
        private ButtonSpecAny btnBrowseTriggerTagAnalog;
        private ThemedLabel easyLabel2;
        private ThemedLabel easyLabel1;
        private ThemedButton btnAddAnalog;
        private ThemedButton btnEditAnalog;
        private ThemedButton btnDeleteAnalog;
        private ThemedButton btnSaveAnalog;
        private ThemedButton btnCancelAnalog;
        private ThemedTextBox txbMinValueAnalog;
        private ButtonSpecAny btnBrowseMinValueAnalog;
        private ThemedTextBox txbMaxValueAnalog;
        private ButtonSpecAny btnBrowseMaxValueAnalog;
        private ThemedTextBox txbDescriptionAnalog;
        private EasyCheckBox ckbEnabledAnalog;
        private EasyPanel easyPanel3;
        private EasyCheckBox ckbEnabledDiscrete;
        private ThemedTextBox txbTriggerValueDiscrete;
        private ButtonSpecAny btnBrowseTriggerValueDiscrete;
        private ThemedTextBox txbDescriptionDiscrete;
        private ThemedButton btnAddDiscrete;
        private ThemedButton btnEditDiscrete;
        private ThemedButton btnDeleteDiscrete;
        private ThemedButton btnSaveDiscrete;
        private ThemedButton btnCancelDiscrete;
        private ThemedLabel easyLabel3;
        private ThemedLabel easyLabel8;
        private ThemedLabel easyLabel9;
        private EasyComboBox cobCompareModeDiscrete;
        private ThemedTextBox txbTriggerTagDiscrete;
        private ButtonSpecAny btnBrowseTriggerTagDiscrete;
        private ThemedLabel easyLabel11;
        private ThemedLabel easyLabel12;
        private EasyDataGridView discreteGridView;
        private EasyPanel easyPanel4;
        private ThemedButton btnAddQuality;
        private ThemedButton btnDeleteQuality;
        private EasyDataGridView qualityGridView;
        private EasyHeaderGroup easyHeaderGroup2;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private EasyNumericUpDown numDelayDiscrete;
        private EasyNumericUpDown numDelayAnalog;
        private ButtonSpecHeaderGroup btnResetProperty;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem resetToDefaultToolStripMenuItem;
        private EasyContextMenuItem easyContextMenuItem1;
        private EasyContextMenuItem easyContextMenuItem2;
        private EasyDataGridViewCheckBoxColumn easyDataGridViewCheckBoxColumn2;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn3;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn6;
        private EasyDataGridViewTextBoxColumn easyDataGridViewComboBoxColumn2;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn7;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn8;
        private EasyDataGridViewCheckBoxColumn colAnalogEnabled;
        private EasyDataGridViewTextBoxColumn colAnalogTriggerTag;
        private EasyDataGridViewTextBoxColumn colAnalogMinValue;
        private EasyDataGridViewTextBoxColumn colAnalogMaxValue;
        private EasyDataGridViewComboBoxColumn colAnalogCompareMode;
        private EasyDataGridViewTextBoxColumn colAnalogDelay;
        private EasyDataGridViewTextBoxColumn colAnalogDescription;
        private EasyDataGridViewCheckBoxColumn easyDataGridViewCheckBoxColumn1;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn1;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn2;
        private EasyDataGridViewComboBoxColumn easyDataGridViewComboBoxColumn1;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn4;
        private EasyDataGridViewTextBoxColumn easyDataGridViewTextBoxColumn5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private EasyNumericUpDown numDelayQuality;
        private EasyComboBox cobCompareModeQuality;
        private EasyComboBox cobQuality;
        private ThemedLabel themedLabel5;
        private ThemedLabel themedLabel3;
        private ThemedLabel themedLabel2;
        private ThemedLabel themedLabel1;
        private EasyCheckBox ckbEnabledQuality;
        private ThemedTextBox txbTriggerTagQuality;
        private ButtonSpecAny btnBrowseTriggerTagQuality;
        private ThemedLabel themedLabel4;
        private ThemedTextBox txbDescriptionQuality;
        private ThemedButton btnEditQuality;
        private ThemedButton btnSaveQuality;
        private ThemedButton btnCancelQuality;
    }
}