namespace WindowsFormsApp3
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
            this.easyDriverConnector1 = new EasyScada.Winforms.Connector.EasyDriverConnector(this.components);
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.SuspendLayout();
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyScada.Winforms.Connector.CommunicationMode.ReceiveFromServer;
            this.easyDriverConnector1.ConnectionStatus = EasyScada.Winforms.Connector.ConnectionStatus.Connecting;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 1000;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // easyLabel1
            // 
            this.easyLabel1.AllowTransparentBackground = false;
            this.easyLabel1.Connector = this.easyDriverConnector1;
            this.easyLabel1.EnableScale = false;
            this.easyLabel1.Gain = 1D;
            this.easyLabel1.Location = new System.Drawing.Point(807, 12);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Offset = 0D;
            this.easyLabel1.PathToTag = "Local Station/Channel1/Device1/Tag1";
            this.easyLabel1.Size = new System.Drawing.Size(70, 20);
            this.easyLabel1.TabIndex = 6;
            this.easyLabel1.Values.Text = "easyLabel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 560);
            this.Controls.Add(this.easyLabel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EasyScada.Winforms.Connector.EasyDriverConnector easyDriverConnector1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
    }
}

