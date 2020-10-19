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
            EasyScada.Core.LogProfile logProfile1 = new EasyScada.Core.LogProfile();
            this.easyAlarmSetting1 = new EasyScada.Winforms.Controls.EasyAlarmSetting();
            this.easyDriverConnector1 = new EasyScada.Winforms.Controls.EasyDriverConnector(this.components);
            this.easyAlarmLogger2 = new EasyScada.Winforms.Controls.EasyAlarmLogger(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.easyDriverConnector1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyAlarmLogger2)).BeginInit();
            this.SuspendLayout();
            // 
            // easyAlarmSetting1
            // 
            this.easyAlarmSetting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyAlarmSetting1.Location = new System.Drawing.Point(0, 0);
            this.easyAlarmSetting1.Name = "easyAlarmSetting1";
            this.easyAlarmSetting1.Size = new System.Drawing.Size(800, 450);
            this.easyAlarmSetting1.TabIndex = 0;
            // 
            // easyDriverConnector1
            // 
            this.easyDriverConnector1.CommunicationMode = EasyScada.Core.CommunicationMode.ReceiveFromServer;
            this.easyDriverConnector1.Port = ((ushort)(8800));
            this.easyDriverConnector1.RefreshRate = 1000;
            this.easyDriverConnector1.ServerAddress = "127.0.0.1";
            // 
            // easyAlarmLogger2
            // 
            logProfile1.DatabaseName = "easyScada";
            logProfile1.DatabaseType = EasyScada.Core.DbType.MySql;
            logProfile1.DataSourceName = null;
            logProfile1.Enabled = true;
            logProfile1.IpAddress = "localhost";
            logProfile1.Password = "100100";
            logProfile1.Port = ((ushort)(3306));
            logProfile1.TableName = "testalarm";
            logProfile1.Username = "root";
            this.easyAlarmLogger2.Databases.Add(logProfile1);
            this.easyAlarmLogger2.Enabled = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.easyAlarmSetting1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.easyDriverConnector1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.easyAlarmLogger2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private EasyScada.Winforms.Controls.EasyAlarmSetting easyAlarmSetting1;
        private EasyScada.Winforms.Controls.EasyDriverConnector easyDriverConnector1;
        private EasyScada.Winforms.Controls.EasyAlarmLogger easyAlarmLogger2;
    }
}