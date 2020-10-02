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
            EasyScada.Core.LogColumn logColumn1 = new EasyScada.Core.LogColumn();
            EasyScada.Core.LogColumn logColumn2 = new EasyScada.Core.LogColumn();
            this.easyPictureBox1 = new EasyScada.Winforms.Controls.EasyPictureBox();
            this.easyDataLogger1 = new EasyScada.Winforms.Controls.EasyDataLogger(this.components);
            this.easyDataLogger2 = new EasyScada.Winforms.Controls.EasyDataLogger(this.components);
            this.easyDriverConnector1 = new EasyScada.Winforms.Controls.EasyDriverConnector(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.easyDataLogger1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyDataLogger2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyDriverConnector1)).BeginInit();
            this.SuspendLayout();
            // 
            // easyPictureBox1
            // 
            this.easyPictureBox1.FillMode = EasyScada.Winforms.Controls.ImageFillMode.Original;
            this.easyPictureBox1.FlipMode = EasyScada.Winforms.Controls.ImageFlipMode.None;
            this.easyPictureBox1.Image = null;
            this.easyPictureBox1.Location = new System.Drawing.Point(363, 214);
            this.easyPictureBox1.Name = "easyPictureBox1";
            this.easyPictureBox1.RotateAngle = 0;
            this.easyPictureBox1.ShadedColor = System.Drawing.Color.Gray;
            this.easyPictureBox1.Size = new System.Drawing.Size(75, 23);
            this.easyPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.easyPictureBox1.TabIndex = 7;
            this.easyPictureBox1.TagPath = "Local Station/Channel/MayMen/Setting8";
            this.easyPictureBox1.Text = "easyPictureBox1";
            // 
            // easyDataLogger1
            // 
            this.easyDataLogger1.AllowLogWhenTagBad = false;
            logColumn1.ColumnName = "Column1";
            logColumn1.DefaultValue = "";
            logColumn1.Description = "";
            logColumn1.Enabled = true;
            logColumn1.Mode = EasyScada.Core.LogColumnMode.UseDefaultValueWhenTagBad;
            logColumn1.TagPath = "";
            logColumn1.UseDefaultValueWhenQualityBad = false;
            this.easyDataLogger1.Columns.AddRange(new EasyScada.Core.LogColumn[] {
            logColumn1});
            this.easyDataLogger1.Enabled = false;
            this.easyDataLogger1.Interval = 60000;
            // 
            // easyDataLogger2
            // 
            this.easyDataLogger2.AllowLogWhenTagBad = false;
            logColumn2.ColumnName = "Column144";
            logColumn2.DefaultValue = "";
            logColumn2.Description = "";
            logColumn2.Enabled = true;
            logColumn2.Mode = EasyScada.Core.LogColumnMode.UseDefaultValueWhenTagBad;
            logColumn2.TagPath = "";
            logColumn2.UseDefaultValueWhenQualityBad = false;
            this.easyDataLogger2.Columns.AddRange(new EasyScada.Core.LogColumn[] {
            logColumn2});
            this.easyDataLogger2.Enabled = false;
            this.easyDataLogger2.Interval = 60000;
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyScada.Core.CommunicationMode.ReceiveFromServer;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 1000;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyPictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.easyDataLogger1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyDataLogger2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyDriverConnector1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private EasyScada.Winforms.Controls.EasyDataLogger easyDataLogger1;
        private EasyScada.Winforms.Controls.EasyDataLogger easyDataLogger2;
        private EasyScada.Winforms.Controls.EasyPictureBox easyPictureBox1;
        private EasyScada.Winforms.Controls.EasyDriverConnector easyDriverConnector1;
    }
}