namespace EasyScada.Winforms.Controls
{
    partial class LogProfileConfigDesignerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogProfileConfigDesignerForm));
            this.easyPanel1 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easyPanel2 = new EasyScada.Winforms.Controls.EasyPanel();
            this.easySplitContainer1 = new EasyScada.Winforms.Controls.EasySplitContainer();
            this.easyHeaderGroup1 = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.easyLabel2 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.cobTableName = new EasyScada.Winforms.Controls.EasyComboBox();
            this.cobDataSourceName = new EasyScada.Winforms.Controls.EasyComboBox();
            this.easyLabel8 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.cobDatabaseType = new EasyScada.Winforms.Controls.EasyComboBox();
            this.txbPort = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.easyLabel4 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel7 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel6 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel5 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.easyLabel3 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.txbPassword = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.easyLabel1 = new EasyScada.Winforms.Controls.ThemedLabel();
            this.txbUser = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.btnTest = new EasyScada.Winforms.Controls.ThemedButton();
            this.txbIpAddress = new EasyScada.Winforms.Controls.ThemedTextBox();
            this.cobDatabase = new EasyScada.Winforms.Controls.EasyComboBox();
            this.btnOk = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnCancel = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnRemove = new EasyScada.Winforms.Controls.ThemedButton();
            this.btnAdd = new EasyScada.Winforms.Controls.ThemedButton();
            this.easyHeaderGroup2 = new EasyScada.Winforms.Controls.EasyHeaderGroup();
            this.lbDatabase = new EasyScada.Winforms.Controls.EasyListBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1.Panel)).BeginInit();
            this.easyHeaderGroup1.Panel.SuspendLayout();
            this.easyHeaderGroup1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cobTableName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobDataSourceName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobDatabaseType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobDatabase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2.Panel)).BeginInit();
            this.easyHeaderGroup2.Panel.SuspendLayout();
            this.easyHeaderGroup2.SuspendLayout();
            this.SuspendLayout();
            // 
            // easyPanel1
            // 
            this.easyPanel1.Controls.Add(this.btnAdd);
            this.easyPanel1.Controls.Add(this.btnOk);
            this.easyPanel1.Controls.Add(this.btnCancel);
            this.easyPanel1.Controls.Add(this.btnRemove);
            this.easyPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.easyPanel1.Location = new System.Drawing.Point(0, 271);
            this.easyPanel1.Name = "easyPanel1";
            this.easyPanel1.Padding = new System.Windows.Forms.Padding(4, 0, 4, 4);
            this.easyPanel1.Size = new System.Drawing.Size(738, 38);
            this.easyPanel1.TabIndex = 0;
            // 
            // easyPanel2
            // 
            this.easyPanel2.Controls.Add(this.easySplitContainer1);
            this.easyPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyPanel2.Location = new System.Drawing.Point(0, 0);
            this.easyPanel2.Name = "easyPanel2";
            this.easyPanel2.Padding = new System.Windows.Forms.Padding(4);
            this.easyPanel2.Size = new System.Drawing.Size(738, 271);
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
            this.easySplitContainer1.Panel1.Controls.Add(this.easyHeaderGroup2);
            // 
            // easySplitContainer1.Panel2
            // 
            this.easySplitContainer1.Panel2.Controls.Add(this.easyHeaderGroup1);
            this.easySplitContainer1.Size = new System.Drawing.Size(730, 263);
            this.easySplitContainer1.SplitterDistance = 243;
            this.easySplitContainer1.TabIndex = 0;
            // 
            // easyHeaderGroup1
            // 
            this.easyHeaderGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyHeaderGroup1.HeaderVisibleSecondary = false;
            this.easyHeaderGroup1.Location = new System.Drawing.Point(0, 0);
            this.easyHeaderGroup1.Name = "easyHeaderGroup1";
            // 
            // easyHeaderGroup1.Panel
            // 
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel2);
            this.easyHeaderGroup1.Panel.Controls.Add(this.cobTableName);
            this.easyHeaderGroup1.Panel.Controls.Add(this.cobDataSourceName);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel8);
            this.easyHeaderGroup1.Panel.Controls.Add(this.cobDatabaseType);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbPort);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel4);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel7);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel6);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel5);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel3);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbPassword);
            this.easyHeaderGroup1.Panel.Controls.Add(this.easyLabel1);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbUser);
            this.easyHeaderGroup1.Panel.Controls.Add(this.btnTest);
            this.easyHeaderGroup1.Panel.Controls.Add(this.txbIpAddress);
            this.easyHeaderGroup1.Panel.Controls.Add(this.cobDatabase);
            this.easyHeaderGroup1.Size = new System.Drawing.Size(482, 263);
            this.easyHeaderGroup1.TabIndex = 12;
            this.easyHeaderGroup1.ValuesPrimary.Heading = "Connection parameters";
            this.easyHeaderGroup1.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("easyHeaderGroup1.ValuesPrimary.Image")));
            // 
            // easyLabel2
            // 
            this.easyLabel2.Location = new System.Drawing.Point(88, 196);
            this.easyLabel2.Margin = new System.Windows.Forms.Padding(12, 4, 3, 3);
            this.easyLabel2.Name = "easyLabel2";
            this.easyLabel2.Size = new System.Drawing.Size(43, 20);
            this.easyLabel2.TabIndex = 27;
            this.easyLabel2.Values.Text = "Table:";
            // 
            // cobTableName
            // 
            this.cobTableName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cobTableName.DropDownWidth = 121;
            this.cobTableName.Location = new System.Drawing.Point(137, 195);
            this.cobTableName.MinimumSize = new System.Drawing.Size(0, 23);
            this.cobTableName.Name = "cobTableName";
            this.cobTableName.Size = new System.Drawing.Size(197, 23);
            this.cobTableName.TabIndex = 26;
            // 
            // cobDataSourceName
            // 
            this.cobDataSourceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cobDataSourceName.DropDownWidth = 121;
            this.cobDataSourceName.Location = new System.Drawing.Point(137, 14);
            this.cobDataSourceName.MinimumSize = new System.Drawing.Size(0, 23);
            this.cobDataSourceName.Name = "cobDataSourceName";
            this.cobDataSourceName.Size = new System.Drawing.Size(324, 23);
            this.cobDataSourceName.TabIndex = 25;
            // 
            // easyLabel8
            // 
            this.easyLabel8.Location = new System.Drawing.Point(38, 45);
            this.easyLabel8.Margin = new System.Windows.Forms.Padding(12, 4, 3, 3);
            this.easyLabel8.Name = "easyLabel8";
            this.easyLabel8.Size = new System.Drawing.Size(93, 20);
            this.easyLabel8.TabIndex = 24;
            this.easyLabel8.Values.Text = "Database Type:";
            // 
            // cobDatabaseType
            // 
            this.cobDatabaseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cobDatabaseType.DropDownWidth = 121;
            this.cobDatabaseType.Location = new System.Drawing.Point(137, 44);
            this.cobDatabaseType.MinimumSize = new System.Drawing.Size(0, 23);
            this.cobDatabaseType.Name = "cobDatabaseType";
            this.cobDatabaseType.Size = new System.Drawing.Size(324, 23);
            this.cobDatabaseType.TabIndex = 23;
            // 
            // txbPort
            // 
            this.txbPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPort.Location = new System.Drawing.Point(384, 75);
            this.txbPort.Margin = new System.Windows.Forms.Padding(3, 4, 4, 3);
            this.txbPort.Name = "txbPort";
            this.txbPort.Size = new System.Drawing.Size(77, 23);
            this.txbPort.TabIndex = 22;
            // 
            // easyLabel4
            // 
            this.easyLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLabel4.Location = new System.Drawing.Point(342, 76);
            this.easyLabel4.Margin = new System.Windows.Forms.Padding(4, 4, 3, 3);
            this.easyLabel4.Name = "easyLabel4";
            this.easyLabel4.Size = new System.Drawing.Size(36, 20);
            this.easyLabel4.TabIndex = 21;
            this.easyLabel4.Values.Text = "Port:";
            // 
            // easyLabel7
            // 
            this.easyLabel7.Location = new System.Drawing.Point(67, 166);
            this.easyLabel7.Margin = new System.Windows.Forms.Padding(12, 4, 3, 3);
            this.easyLabel7.Name = "easyLabel7";
            this.easyLabel7.Size = new System.Drawing.Size(64, 20);
            this.easyLabel7.TabIndex = 20;
            this.easyLabel7.Values.Text = "Database:";
            // 
            // easyLabel6
            // 
            this.easyLabel6.Location = new System.Drawing.Point(66, 136);
            this.easyLabel6.Margin = new System.Windows.Forms.Padding(12, 4, 3, 3);
            this.easyLabel6.Name = "easyLabel6";
            this.easyLabel6.Size = new System.Drawing.Size(65, 20);
            this.easyLabel6.TabIndex = 19;
            this.easyLabel6.Values.Text = "Password:";
            // 
            // easyLabel5
            // 
            this.easyLabel5.Location = new System.Drawing.Point(93, 106);
            this.easyLabel5.Margin = new System.Windows.Forms.Padding(12, 4, 3, 3);
            this.easyLabel5.Name = "easyLabel5";
            this.easyLabel5.Size = new System.Drawing.Size(38, 20);
            this.easyLabel5.TabIndex = 18;
            this.easyLabel5.Values.Text = "User:";
            // 
            // easyLabel3
            // 
            this.easyLabel3.Location = new System.Drawing.Point(70, 76);
            this.easyLabel3.Margin = new System.Windows.Forms.Padding(12, 4, 3, 3);
            this.easyLabel3.Name = "easyLabel3";
            this.easyLabel3.Size = new System.Drawing.Size(61, 20);
            this.easyLabel3.TabIndex = 17;
            this.easyLabel3.Values.Text = "Ip Server:";
            // 
            // txbPassword
            // 
            this.txbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPassword.Location = new System.Drawing.Point(137, 135);
            this.txbPassword.Margin = new System.Windows.Forms.Padding(3, 4, 4, 3);
            this.txbPassword.Name = "txbPassword";
            this.txbPassword.PasswordChar = '●';
            this.txbPassword.Size = new System.Drawing.Size(197, 23);
            this.txbPassword.TabIndex = 12;
            this.txbPassword.UseSystemPasswordChar = true;
            // 
            // easyLabel1
            // 
            this.easyLabel1.Location = new System.Drawing.Point(16, 15);
            this.easyLabel1.Margin = new System.Windows.Forms.Padding(12, 12, 3, 3);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Size = new System.Drawing.Size(115, 20);
            this.easyLabel1.TabIndex = 16;
            this.easyLabel1.Values.Text = "Data Source Name:";
            // 
            // txbUser
            // 
            this.txbUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbUser.Location = new System.Drawing.Point(137, 105);
            this.txbUser.Margin = new System.Windows.Forms.Padding(3, 4, 4, 3);
            this.txbUser.Name = "txbUser";
            this.txbUser.Size = new System.Drawing.Size(197, 23);
            this.txbUser.TabIndex = 11;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(340, 166);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(119, 52);
            this.btnTest.TabIndex = 15;
            this.btnTest.Values.Text = "Test";
            // 
            // txbIpAddress
            // 
            this.txbIpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbIpAddress.Location = new System.Drawing.Point(137, 75);
            this.txbIpAddress.Margin = new System.Windows.Forms.Padding(3, 4, 4, 3);
            this.txbIpAddress.Name = "txbIpAddress";
            this.txbIpAddress.Size = new System.Drawing.Size(197, 23);
            this.txbIpAddress.TabIndex = 13;
            // 
            // cobDatabase
            // 
            this.cobDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cobDatabase.DropDownWidth = 121;
            this.cobDatabase.Location = new System.Drawing.Point(137, 165);
            this.cobDatabase.MinimumSize = new System.Drawing.Size(0, 23);
            this.cobDatabase.Name = "cobDatabase";
            this.cobDatabase.Size = new System.Drawing.Size(197, 23);
            this.cobDatabase.TabIndex = 14;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(564, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(82, 29);
            this.btnOk.TabIndex = 17;
            this.btnOk.Values.Text = "Ok";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(652, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 29);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Values.Text = "Cancel";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(92, 2);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(82, 29);
            this.btnRemove.TabIndex = 15;
            this.btnRemove.Values.Text = "Remove";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(4, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(82, 29);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Values.Text = "Add";
            // 
            // easyHeaderGroup2
            // 
            this.easyHeaderGroup2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyHeaderGroup2.HeaderVisibleSecondary = false;
            this.easyHeaderGroup2.Location = new System.Drawing.Point(0, 0);
            this.easyHeaderGroup2.Name = "easyHeaderGroup2";
            // 
            // easyHeaderGroup2.Panel
            // 
            this.easyHeaderGroup2.Panel.Controls.Add(this.lbDatabase);
            this.easyHeaderGroup2.Size = new System.Drawing.Size(243, 263);
            this.easyHeaderGroup2.TabIndex = 0;
            this.easyHeaderGroup2.ValuesPrimary.Heading = "Databases";
            this.easyHeaderGroup2.ValuesPrimary.Image = ((System.Drawing.Image)(resources.GetObject("easyHeaderGroup2.ValuesPrimary.Image")));
            // 
            // lbDatabase
            // 
            this.lbDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDatabase.Location = new System.Drawing.Point(0, 0);
            this.lbDatabase.Name = "lbDatabase";
            this.lbDatabase.Size = new System.Drawing.Size(241, 231);
            this.lbDatabase.StateCommon.Border.DrawBorders = ((EasyScada.Winforms.Controls.PaletteDrawBorders)((((EasyScada.Winforms.Controls.PaletteDrawBorders.Top | EasyScada.Winforms.Controls.PaletteDrawBorders.Bottom) 
            | EasyScada.Winforms.Controls.PaletteDrawBorders.Left) 
            | EasyScada.Winforms.Controls.PaletteDrawBorders.Right)));
            this.lbDatabase.StateCommon.Border.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.lbDatabase.StateCommon.Border.Width = 0;
            this.lbDatabase.TabIndex = 14;
            // 
            // LogProfileConfigDesignerForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(738, 309);
            this.Controls.Add(this.easyPanel2);
            this.Controls.Add(this.easyPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogProfileConfigDesignerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Configuration";
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
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1.Panel)).EndInit();
            this.easyHeaderGroup1.Panel.ResumeLayout(false);
            this.easyHeaderGroup1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup1)).EndInit();
            this.easyHeaderGroup1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cobTableName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobDataSourceName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobDatabaseType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cobDatabase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2.Panel)).EndInit();
            this.easyHeaderGroup2.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.easyHeaderGroup2)).EndInit();
            this.easyHeaderGroup2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private EasyPanel easyPanel1;
        private ThemedButton btnAdd;
        private ThemedButton btnOk;
        private ThemedButton btnCancel;
        private ThemedButton btnRemove;
        private EasyPanel easyPanel2;
        private EasySplitContainer easySplitContainer1;
        private EasyHeaderGroup easyHeaderGroup2;
        private EasyListBox lbDatabase;
        private EasyHeaderGroup easyHeaderGroup1;
        private ThemedLabel easyLabel2;
        private EasyComboBox cobTableName;
        private EasyComboBox cobDataSourceName;
        private ThemedLabel easyLabel8;
        private EasyComboBox cobDatabaseType;
        private ThemedTextBox txbPort;
        private ThemedLabel easyLabel4;
        private ThemedLabel easyLabel7;
        private ThemedLabel easyLabel6;
        private ThemedLabel easyLabel5;
        private ThemedLabel easyLabel3;
        private ThemedTextBox txbPassword;
        private ThemedLabel easyLabel1;
        private ThemedTextBox txbUser;
        private ThemedButton btnTest;
        private ThemedTextBox txbIpAddress;
        private EasyComboBox cobDatabase;
    }
}