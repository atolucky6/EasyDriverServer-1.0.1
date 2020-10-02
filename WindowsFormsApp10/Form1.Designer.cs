namespace WindowsFormsApp10
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
            EasyScada.Core.LogProfile logProfile1 = new EasyScada.Core.LogProfile();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.historicalTrend1 = new EasyScada.Winforms.Controls.Charts.HistoricalTrend();
            this.historicalTrendToolbox1 = new EasyScada.Winforms.Controls.HistoricalTrendToolbox();
            ((System.ComponentModel.ISupportInitialize)(this.historicalTrendToolbox1)).BeginInit();
            this.SuspendLayout();
            // 
            // historicalTrend1
            // 
            this.historicalTrend1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            logProfile1.DatabaseName = "easyScada";
            logProfile1.DatabaseType = EasyScada.Core.DbType.MySql;
            logProfile1.DataSourceName = null;
            logProfile1.Enabled = true;
            logProfile1.IpAddress = "localhost";
            logProfile1.Password = "100100";
            logProfile1.Port = ((ushort)(3306));
            logProfile1.TableName = "table1";
            logProfile1.Username = "root";
            this.historicalTrend1.Database = logProfile1;
            this.historicalTrend1.GridColumnWidth = 100;
            this.historicalTrend1.GridLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.historicalTrend1.GridRowCount = 6;
            this.historicalTrend1.LeftAxisMaxValue = 3000F;
            this.historicalTrend1.Lines.Add(((EasyScada.Winforms.Controls.TrendLine)(resources.GetObject("historicalTrend1.Lines"))));
            this.historicalTrend1.Lines.Add(((EasyScada.Winforms.Controls.TrendLine)(resources.GetObject("historicalTrend1.Lines1"))));
            this.historicalTrend1.Lines.Add(((EasyScada.Winforms.Controls.TrendLine)(resources.GetObject("historicalTrend1.Lines2"))));
            this.historicalTrend1.Location = new System.Drawing.Point(54, 108);
            this.historicalTrend1.MeasureTextColor = System.Drawing.Color.Yellow;
            this.historicalTrend1.Name = "historicalTrend1";
            this.historicalTrend1.RightAxisMaxValue = 3000F;
            this.historicalTrend1.Size = new System.Drawing.Size(852, 478);
            this.historicalTrend1.TabIndex = 0;
            this.historicalTrend1.Text = "historicalTrend1";
            this.historicalTrend1.TooltipBorderColor = System.Drawing.Color.HotPink;
            this.historicalTrend1.TooltipDataVisible = true;
            this.historicalTrend1.TooltipForeColor = System.Drawing.Color.Cyan;
            this.historicalTrend1.TooltipHorizontalLineVisible = true;
            this.historicalTrend1.TooltipTimeVisible = true;
            this.historicalTrend1.TooltipVerticalLineVisible = true;
            this.historicalTrend1.TooltipVisible = true;
            // 
            // historicalTrendToolbox1
            // 
            this.historicalTrendToolbox1.HistoricalTrend = this.historicalTrend1;
            this.historicalTrendToolbox1.Location = new System.Drawing.Point(90, 34);
            this.historicalTrendToolbox1.Name = "historicalTrendToolbox1";
            this.historicalTrendToolbox1.Size = new System.Drawing.Size(827, 40);
            this.historicalTrendToolbox1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 598);
            this.Controls.Add(this.historicalTrendToolbox1);
            this.Controls.Add(this.historicalTrend1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.historicalTrendToolbox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private EasyScada.Winforms.Controls.Charts.HistoricalTrend historicalTrend1;
        private EasyScada.Winforms.Controls.HistoricalTrendToolbox historicalTrendToolbox1;
    }
}