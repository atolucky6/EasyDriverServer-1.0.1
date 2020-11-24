namespace TestScadaFormControls
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
            this.easyDriverConnector1 = new EasyScada.Winforms.Controls.EasyDriverConnector(this.components);
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyTextBox1 = new EasyScada.Winforms.Controls.EasyTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.easyDriverConnector1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyTextBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyScada.Core.CommunicationMode.ReceiveFromServer;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 1000;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // easyLabel1
            // 
            this.easyLabel1.Location = new System.Drawing.Point(38, 42);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Size = new System.Drawing.Size(100, 23);
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.TagPath = "RemoteStation1/Channel_0_User_Defined/Ramp/Ramp_Float";
            this.easyLabel1.Text = "easyLabel1";
            // 
            // easyTextBox1
            // 
            this.easyTextBox1.DropDownBackColor = System.Drawing.SystemColors.Control;
            this.easyTextBox1.DropDownBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.easyTextBox1.DropDownDirection = EasyScada.Winforms.Controls.DropDownDirection.Bottom;
            this.easyTextBox1.DropDownFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.easyTextBox1.DropDownForeColor = System.Drawing.SystemColors.ControlText;
            this.easyTextBox1.Location = new System.Drawing.Point(177, 42);
            this.easyTextBox1.Name = "easyTextBox1";
            this.easyTextBox1.Size = new System.Drawing.Size(100, 20);
            this.easyTextBox1.TabIndex = 1;
            this.easyTextBox1.TagPath = "RemoteStation1/Channel_1/Device_1/Tag_1";
            this.easyTextBox1.Text = "easyTextBox1";
            this.easyTextBox1.WriteDelay = 200;
            this.easyTextBox1.WriteTrigger = EasyScada.Core.WriteTrigger.OnEnter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyTextBox1);
            this.Controls.Add(this.easyLabel1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.easyDriverConnector1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyTextBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EasyScada.Winforms.Controls.EasyDriverConnector easyDriverConnector1;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
        private EasyScada.Winforms.Controls.EasyTextBox easyTextBox1;
    }
}