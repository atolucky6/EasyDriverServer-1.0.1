namespace WindowsFormsApp2
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
            EasyScada.Winforms.Controls.WriteTagCommand writeTagCommand2 = new EasyScada.Winforms.Controls.WriteTagCommand();
            EasyScada.Winforms.Controls.WriteTagCommand writeTagCommand3 = new EasyScada.Winforms.Controls.WriteTagCommand();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.easyButton3 = new EasyScada.Winforms.Controls.EasyButton();
            this.easyDriverConnector1 = new EasyScada.Winforms.Connector.EasyDriverConnector(this.components);
            this.easyButton2 = new EasyScada.Winforms.Controls.EasyButton();
            this.easyButton1 = new EasyScada.Winforms.Controls.EasyButton();
            this.easyTextBox1 = new EasyScada.Winforms.Controls.EasyTextBox();
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyWriteTagCommand1 = new EasyScada.Winforms.Controls.EasyWriteTagCommand();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // easyButton3
            // 
            this.easyButton3.AllowTransparentBackground = false;
            this.easyButton3.Connector = this.easyDriverConnector1;
            this.easyButton3.Location = new System.Drawing.Point(698, 150);
            this.easyButton3.Name = "easyButton3";
            this.easyButton3.Size = new System.Drawing.Size(90, 25);
            this.easyButton3.TabIndex = 4;
            this.easyButton3.Values.Text = "easyButton3";
            writeTagCommand1.Connector = this.easyDriverConnector1;
            writeTagCommand1.Enabled = true;
            writeTagCommand1.PathToTag = null;
            writeTagCommand1.WriteDelay = 0;
            writeTagCommand1.WriteValue = null;
            this.easyButton3.WriteTagCommands.AddRange(new EasyScada.Winforms.Controls.WriteTagCommand[] {
            writeTagCommand1});
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyScada.Winforms.Connector.CommunicationMode.RequestToServer;
            this.easyDriverConnector1.ConnectionStatus = EasyScada.Winforms.Connector.ConnectionStatus.Connecting;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 100;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // easyButton2
            // 
            this.easyButton2.AllowTransparentBackground = false;
            this.easyButton2.Connector = this.easyDriverConnector1;
            this.easyButton2.Location = new System.Drawing.Point(698, 119);
            this.easyButton2.Name = "easyButton2";
            this.easyButton2.Size = new System.Drawing.Size(90, 25);
            this.easyButton2.TabIndex = 3;
            this.easyButton2.Values.Text = "easyButton2";
            // 
            // easyButton1
            // 
            this.easyButton1.AllowTransparentBackground = false;
            this.easyButton1.Connector = this.easyDriverConnector1;
            this.easyButton1.Location = new System.Drawing.Point(571, 57);
            this.easyButton1.Name = "easyButton1";
            this.easyButton1.Size = new System.Drawing.Size(217, 56);
            this.easyButton1.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.easyButton1.StateCommon.Back.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyButton1.StateCommon.Border.Color1 = System.Drawing.Color.Yellow;
            this.easyButton1.StateCommon.Border.DrawBorders = ((EasyScada.Winforms.Controls.PaletteDrawBorders)((((EasyScada.Winforms.Controls.PaletteDrawBorders.Top | EasyScada.Winforms.Controls.PaletteDrawBorders.Bottom) 
            | EasyScada.Winforms.Controls.PaletteDrawBorders.Left) 
            | EasyScada.Winforms.Controls.PaletteDrawBorders.Right)));
            this.easyButton1.StateCommon.Border.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyButton1.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.easyButton1.StateCommon.Content.ShortText.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyButton1.StateCommon.Content.ShortText.Trim = EasyScada.Winforms.Controls.PaletteTextTrim.Inherit;
            this.easyButton1.TabIndex = 2;
            this.easyButton1.Values.Image = global::WindowsFormsApp2.Properties.Resources.workstation_48px;
            this.easyButton1.Values.Text = "easyButton1";
            writeTagCommand2.Connector = this.easyDriverConnector1;
            writeTagCommand2.Enabled = false;
            writeTagCommand2.PathToTag = "Local Station/Channel1/Device1/Tag4";
            writeTagCommand2.WriteDelay = 0;
            writeTagCommand2.WriteValue = "5";
            writeTagCommand3.Connector = this.easyDriverConnector1;
            writeTagCommand3.Enabled = false;
            writeTagCommand3.PathToTag = "Local Station/Channel1/Device1/Tag4";
            writeTagCommand3.WriteDelay = 0;
            writeTagCommand3.WriteValue = "400";
            this.easyButton1.WriteTagCommands.AddRange(new EasyScada.Winforms.Controls.WriteTagCommand[] {
            writeTagCommand2,
            writeTagCommand3});
            this.easyButton1.Click += new System.EventHandler(this.easyButton1_Click);
            // 
            // easyTextBox1
            // 
            this.easyTextBox1.AllowTransparentBackground = false;
            this.easyTextBox1.Connector = this.easyDriverConnector1;
            this.easyTextBox1.EnableScale = false;
            this.easyTextBox1.Gain = 1D;
            this.easyTextBox1.Location = new System.Drawing.Point(149, 150);
            this.easyTextBox1.Name = "easyTextBox1";
            this.easyTextBox1.Offset = 0D;
            this.easyTextBox1.PathToTag = "Local Station/Channel1/Device1/Tag1";
            this.easyTextBox1.Size = new System.Drawing.Size(136, 23);
            this.easyTextBox1.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.easyTextBox1.StateCommon.Border.DrawBorders = ((EasyScada.Winforms.Controls.PaletteDrawBorders)((((EasyScada.Winforms.Controls.PaletteDrawBorders.Top | EasyScada.Winforms.Controls.PaletteDrawBorders.Bottom) 
            | EasyScada.Winforms.Controls.PaletteDrawBorders.Left) 
            | EasyScada.Winforms.Controls.PaletteDrawBorders.Right)));
            this.easyTextBox1.StateCommon.Border.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyTextBox1.TabIndex = 1;
            this.easyTextBox1.WriteDelay = 0;
            this.easyTextBox1.WriteMode = EasyScada.Winforms.Controls.WriteMode.ValueChanged;
            // 
            // easyLabel1
            // 
            this.easyLabel1.AllowTransparentBackground = false;
            this.easyLabel1.Connector = this.easyDriverConnector1;
            this.easyLabel1.EnableScale = true;
            this.easyLabel1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.easyLabel1.Gain = 2D;
            this.easyLabel1.LabelStyle = EasyScada.Winforms.Controls.LabelStyle.BoldControl;
            this.easyLabel1.Location = new System.Drawing.Point(74, 74);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Offset = 100D;
            this.easyLabel1.PathToTag = "Local Station/Channel1/Device1/Tag1";
            this.easyLabel1.Size = new System.Drawing.Size(123, 50);
            this.easyLabel1.StateNormal.ShortText.Color1 = System.Drawing.Color.Red;
            this.easyLabel1.StateNormal.ShortText.ImageStyle = EasyScada.Winforms.Controls.PaletteImageStyle.Inherit;
            this.easyLabel1.StateNormal.ShortText.TextH = EasyScada.Winforms.Controls.PaletteRelativeAlign.Near;
            this.easyLabel1.StateNormal.ShortText.TextV = EasyScada.Winforms.Controls.PaletteRelativeAlign.Center;
            this.easyLabel1.StateNormal.ShortText.Trim = EasyScada.Winforms.Controls.PaletteTextTrim.Inherit;
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.Values.Image = global::WindowsFormsApp2.Properties.Resources.workstation_48px;
            this.easyLabel1.Values.Text = "easyLabel1";
            // 
            // easyWriteTagCommand1
            // 
            this.easyWriteTagCommand1.Connector = this.easyDriverConnector1;
            this.easyWriteTagCommand1.Enabled = false;
            this.easyWriteTagCommand1.PathToTag = "Local Station/Channel1/Device1/Tag1";
            this.easyWriteTagCommand1.WriteDelay = 0;
            this.easyWriteTagCommand1.WriteValue = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyButton3);
            this.Controls.Add(this.easyButton2);
            this.Controls.Add(this.easyButton1);
            this.Controls.Add(this.easyTextBox1);
            this.Controls.Add(this.easyLabel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form1";
            this.Text = "3";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EasyScada.Winforms.Connector.EasyDriverConnector easyDriverConnector1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
        private EasyScada.Winforms.Controls.EasyTextBox easyTextBox1;
        private System.Windows.Forms.ImageList imageList1;
        private EasyScada.Winforms.Controls.EasyWriteTagCommand easyWriteTagCommand1;
        private EasyScada.Winforms.Controls.EasyButton easyButton1;
        private EasyScada.Winforms.Controls.EasyButton easyButton2;
        private EasyScada.Winforms.Controls.EasyButton easyButton3;
    }
}

