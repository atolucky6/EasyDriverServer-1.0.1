namespace WindowsFormsApp9
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
            EasyScada.Core.WriteTagCommand writeTagCommand1 = new EasyScada.Core.WriteTagCommand();
            this.easyLabel1 = new EasyScada.Winforms.Controls.EasyLabel();
            this.easyTextBox1 = new EasyScada.Winforms.Controls.EasyTextBox();
            this.easyButton1 = new EasyScada.Winforms.Controls.EasyButton();
            this.SuspendLayout();
            // 
            // easyLabel1
            // 
            this.easyLabel1.AllowTransparentBackground = false;
            this.easyLabel1.Connector = null;
            this.easyLabel1.EnableScale = true;
            this.easyLabel1.Gain = 1D;
            this.easyLabel1.Location = new System.Drawing.Point(543, 90);
            this.easyLabel1.Name = "easyLabel1";
            this.easyLabel1.Offset = 0D;
            this.easyLabel1.PathToTag = "";
            this.easyLabel1.Size = new System.Drawing.Size(70, 20);
            this.easyLabel1.TabIndex = 0;
            this.easyLabel1.Values.Text = "easyLabel1";
            // 
            // easyTextBox1
            // 
            this.easyTextBox1.AllowTransparentBackground = false;
            this.easyTextBox1.Connector = null;
            this.easyTextBox1.EnableScale = false;
            this.easyTextBox1.Gain = 1D;
            this.easyTextBox1.Location = new System.Drawing.Point(334, 269);
            this.easyTextBox1.Name = "easyTextBox1";
            this.easyTextBox1.Offset = 0D;
            this.easyTextBox1.PathToTag = "";
            this.easyTextBox1.Size = new System.Drawing.Size(100, 23);
            this.easyTextBox1.TabIndex = 1;
            this.easyTextBox1.Text = "easyTextBox1";
            this.easyTextBox1.WriteDelay = 0;
            this.easyTextBox1.WriteTrigger = EasyScada.Core.WriteTrigger.OnEnter;
            // 
            // easyButton1
            // 
            this.easyButton1.AllowTransparentBackground = false;
            this.easyButton1.Connector = null;
            this.easyButton1.Location = new System.Drawing.Point(316, 84);
            this.easyButton1.Name = "easyButton1";
            this.easyButton1.Size = new System.Drawing.Size(90, 25);
            this.easyButton1.TabIndex = 2;
            this.easyButton1.Values.Text = "easyButton1";
            writeTagCommand1.Connector = null;
            writeTagCommand1.PathToTag = null;
            writeTagCommand1.WriteDelay = 0;
            writeTagCommand1.WriteValue = null;
            this.easyButton1.WriteTagCommands.AddRange(new EasyScada.Core.WriteTagCommand[] {
            writeTagCommand1});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyButton1);
            this.Controls.Add(this.easyTextBox1);
            this.Controls.Add(this.easyLabel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EasyScada.Winforms.Controls.EasyLabel easyLabel1;
        private EasyScada.Winforms.Controls.EasyTextBox easyTextBox1;
        private EasyScada.Winforms.Controls.EasyButton easyButton1;
    }
}

