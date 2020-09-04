namespace WindowsFormsApp8
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
            this.userControl11 = new WindowsFormsApp8.UserControl1();
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.SuspendLayout();
            // 
            // userControl11
            // 
            this.userControl11.Location = new System.Drawing.Point(500, 142);
            this.userControl11.Name = "userControl11";
            this.userControl11.Size = new System.Drawing.Size(288, 185);
            this.userControl11.TabIndex = 2;
            this.userControl11.UseWaitCursor = true;
            // 
            // easyLabel1
            // 
            this.easyLabel1.AllowTransparentBackground = false;
            this.easyLabel1.Connector = null;
            this.easyLabel1.EnableScale = false;
            this.easyLabel1.Gain = 1D;
            this.easyLabel1.Location = new System.Drawing.Point(276, 85);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Offset = 0D;
            this.easyLabel1.PathToTag = "sgasdg";
            this.easyLabel1.Size = new System.Drawing.Size(70, 20);
            this.easyLabel1.TabIndex = 3;
            this.easyLabel1.Values.Text = "easyLabel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyLabel1);
            this.Controls.Add(this.userControl11);
            this.Name = "Form1";
            this.Text = "Form1";
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UserControl1 userControl11;
        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
    }
}

