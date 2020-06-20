namespace WindowsFormsApp6
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
            this.easyLabel2 = new EasyScada.Winforms.Controls.EasyLabel();
            this.SuspendLayout();
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyDriver.Client.Models.CommunicationMode.RequestToServer;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 1000;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // easyLabel1
            // 
            this.easyLabel1.AutoSize = true;
            this.easyLabel1.Connector = this.easyDriverConnector1;
            this.easyLabel1.Location = new System.Drawing.Point(207, 50);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.PathToTag = "Local Station/Channel1/Device1/Tag1";
            this.easyLabel1.Size = new System.Drawing.Size(61, 13);
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.Text = "easyLabel1";
            // 
            // easyLabel2
            // 
            this.easyLabel2.AutoSize = true;
            this.easyLabel2.Connector = this.easyDriverConnector1;
            this.easyLabel2.Location = new System.Drawing.Point(327, 50);
            this.easyLabel2.Name = "easyLabel2";
            this.easyLabel2.PathToTag = "Local Station/Channel1/Device2/Tag1";
            this.easyLabel2.Size = new System.Drawing.Size(61, 13);
            this.easyLabel2.TabIndex = 1;
            this.easyLabel2.Text = "easyLabel2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyLabel2);
            this.Controls.Add(this.easyLabel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EasyScada.Winforms.Connector.EasyDriverConnector easyDriverConnector1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel2;
    }
}

