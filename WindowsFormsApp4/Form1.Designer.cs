namespace WindowsFormsApp4
{
    partial class Form1
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
            EasyScada.Winforms.Controls.WriteTagCommand writeTagCommand1 = new EasyScada.Winforms.Controls.WriteTagCommand();
            this.easyLabel3 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyDriverConnector1 = new EasyScada.Winforms.Connector.EasyDriverConnector(this.components);
            this.easyLabel2 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyTextBox1 = new EasyScada.Winforms.Controls.EasyTextBox();
            this.easyButton1 = new EasyScada.Winforms.Controls.EasyButton();
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.SuspendLayout();
            // 
            // easyLabel3
            // 
            this.easyLabel3.AllowTransparentBackground = false;
            this.easyLabel3.Connector = this.easyDriverConnector1;
            this.easyLabel3.EnableScale = false;
            this.easyLabel3.Gain = 1D;
            this.easyLabel3.Location = new System.Drawing.Point(29, 102);
            this.easyLabel3.Name = "easyLabel3";
            this.easyLabel3.Offset = 0D;
            this.easyLabel3.PathToTag = "RemoteStation1/Channel_1/Device_1/Tag_3";
            this.easyLabel3.Size = new System.Drawing.Size(169, 39);
            this.easyLabel3.StateNormal.ShortText.Color1 = System.Drawing.Color.Red;
            this.easyLabel3.StateNormal.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.easyLabel3.StateNormal.ShortText.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyLabel3.StateNormal.ShortText.Trim = EasyScada.Winforms.Controls.PaletteTextTrim.Inherit;
            this.easyLabel3.TabIndex = 4;
            this.easyLabel3.Values.Text = "easyLabel3";
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyScada.Winforms.Connector.CommunicationMode.RequestToServer;
            this.easyDriverConnector1.ConnectionStatus = EasyScada.Winforms.Connector.ConnectionStatus.Connecting;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 100;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // easyLabel2
            // 
            this.easyLabel2.AllowTransparentBackground = false;
            this.easyLabel2.Connector = this.easyDriverConnector1;
            this.easyLabel2.EnableScale = false;
            this.easyLabel2.Gain = 1D;
            this.easyLabel2.Location = new System.Drawing.Point(29, 57);
            this.easyLabel2.Name = "easyLabel2";
            this.easyLabel2.Offset = 0D;
            this.easyLabel2.PathToTag = "RemoteStation1/Channel_1/Device_1/Tag_2";
            this.easyLabel2.Size = new System.Drawing.Size(169, 39);
            this.easyLabel2.StateNormal.ShortText.Color1 = System.Drawing.Color.Red;
            this.easyLabel2.StateNormal.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.easyLabel2.StateNormal.ShortText.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyLabel2.StateNormal.ShortText.Trim = EasyScada.Winforms.Controls.PaletteTextTrim.Inherit;
            this.easyLabel2.TabIndex = 3;
            this.easyLabel2.Values.Text = "easyLabel2";
            // 
            // easyTextBox1
            // 
            this.easyTextBox1.AllowTransparentBackground = false;
            this.easyTextBox1.Connector = this.easyDriverConnector1;
            this.easyTextBox1.EnableScale = false;
            this.easyTextBox1.Gain = 1D;
            this.easyTextBox1.Location = new System.Drawing.Point(29, 163);
            this.easyTextBox1.Name = "easyTextBox1";
            this.easyTextBox1.Offset = 0D;
            this.easyTextBox1.PathToTag = "RemoteStation1/Channel_1/Device_1/Tag_1";
            this.easyTextBox1.Size = new System.Drawing.Size(195, 38);
            this.easyTextBox1.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.easyTextBox1.TabIndex = 2;
            this.easyTextBox1.Text = "easyTextBox1";
            this.easyTextBox1.WriteDelay = 0;
            this.easyTextBox1.WriteMode = EasyScada.Winforms.Controls.WriteMode.ValueChanged;
            // 
            // easyButton1
            // 
            this.easyButton1.AllowTransparentBackground = false;
            this.easyButton1.Connector = this.easyDriverConnector1;
            this.easyButton1.Location = new System.Drawing.Point(219, 26);
            this.easyButton1.Name = "easyButton1";
            this.easyButton1.PaletteMode = EasyScada.Winforms.Controls.PaletteMode.Office2010Silver;
            this.easyButton1.Size = new System.Drawing.Size(90, 25);
            this.easyButton1.TabIndex = 1;
            this.easyButton1.Values.Text = "easyButton1";
            writeTagCommand1.Connector = this.easyDriverConnector1;
            writeTagCommand1.PathToTag = "RemoteStation1/Channel_1/Device_1/Tag_1";
            writeTagCommand1.WriteDelay = 0;
            writeTagCommand1.WriteValue = "0";
            this.easyButton1.WriteTagCommands.AddRange(new EasyScada.Winforms.Controls.WriteTagCommand[] {
            writeTagCommand1});
            // 
            // easyLabel1
            // 
            this.easyLabel1.AllowTransparentBackground = false;
            this.easyLabel1.Connector = this.easyDriverConnector1;
            this.easyLabel1.EnableScale = false;
            this.easyLabel1.Gain = 1D;
            this.easyLabel1.Location = new System.Drawing.Point(29, 12);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Offset = 0D;
            this.easyLabel1.PathToTag = "RemoteStation1/Channel_1/Device_1/Tag_1";
            this.easyLabel1.Size = new System.Drawing.Size(169, 39);
            this.easyLabel1.StateNormal.ShortText.Color1 = System.Drawing.Color.Red;
            this.easyLabel1.StateNormal.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.easyLabel1.StateNormal.ShortText.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyLabel1.StateNormal.ShortText.Trim = EasyScada.Winforms.Controls.PaletteTextTrim.Inherit;
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.Values.Text = "easyLabel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyLabel3);
            this.Controls.Add(this.easyLabel2);
            this.Controls.Add(this.easyTextBox1);
            this.Controls.Add(this.easyButton1);
            this.Controls.Add(this.easyLabel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EasyScada.Winforms.Connector.EasyDriverConnector easyDriverConnector1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
        private EasyScada.Winforms.Controls.EasyButton easyButton1;
        private EasyScada.Winforms.Controls.EasyTextBox easyTextBox1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel2;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel3;
    }
}

