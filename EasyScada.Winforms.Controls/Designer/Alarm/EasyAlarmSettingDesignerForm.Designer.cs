namespace EasyScada.Winforms.Controls
{
    partial class EasyAlarmSettingDesignerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EasyAlarmSettingDesignerForm));
            this.easyAlarmSetting1 = new EasyScada.Winforms.Controls.EasyAlarmSetting();
            this.SuspendLayout();
            // 
            // easyAlarmSetting1
            // 
            this.easyAlarmSetting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyAlarmSetting1.Location = new System.Drawing.Point(0, 0);
            this.easyAlarmSetting1.Name = "easyAlarmSetting1";
            this.easyAlarmSetting1.Size = new System.Drawing.Size(965, 599);
            this.easyAlarmSetting1.TabIndex = 1;
            // 
            // EasyAlarmSettingDesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 599);
            this.Controls.Add(this.easyAlarmSetting1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EasyAlarmSettingDesignerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alarm Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private EasyAlarmSetting easyAlarmSetting1;
    }
}