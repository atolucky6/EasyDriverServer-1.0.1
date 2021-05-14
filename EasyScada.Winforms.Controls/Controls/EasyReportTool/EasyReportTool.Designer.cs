namespace EasyScada.Winforms.Controls
{
    partial class EasyReportTool
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupReport = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cobType = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.dtpToTime = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromTime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupReport
            // 
            this.groupReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupReport.Controls.Add(this.label3);
            this.groupReport.Controls.Add(this.cobType);
            this.groupReport.Controls.Add(this.btnExport);
            this.groupReport.Controls.Add(this.dtpToTime);
            this.groupReport.Controls.Add(this.dtpToDate);
            this.groupReport.Controls.Add(this.dtpFromTime);
            this.groupReport.Controls.Add(this.label2);
            this.groupReport.Controls.Add(this.label1);
            this.groupReport.Controls.Add(this.dtpFromDate);
            this.groupReport.Location = new System.Drawing.Point(3, 3);
            this.groupReport.Name = "groupReport";
            this.groupReport.Size = new System.Drawing.Size(769, 110);
            this.groupReport.TabIndex = 0;
            this.groupReport.TabStop = false;
            this.groupReport.Text = "Report Title";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(534, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Export type:";
            // 
            // cobType
            // 
            this.cobType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobType.FormattingEnabled = true;
            this.cobType.Location = new System.Drawing.Point(534, 53);
            this.cobType.Name = "cobType";
            this.cobType.Size = new System.Drawing.Size(121, 28);
            this.cobType.TabIndex = 7;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(667, 52);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(81, 30);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // dtpToTime
            // 
            this.dtpToTime.CustomFormat = "HH:mm:ss";
            this.dtpToTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpToTime.Location = new System.Drawing.Point(412, 54);
            this.dtpToTime.Name = "dtpToTime";
            this.dtpToTime.ShowUpDown = true;
            this.dtpToTime.Size = new System.Drawing.Size(110, 26);
            this.dtpToTime.TabIndex = 5;
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(282, 54);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(124, 26);
            this.dtpToDate.TabIndex = 4;
            // 
            // dtpFromTime
            // 
            this.dtpFromTime.CustomFormat = "HH:mm:ss";
            this.dtpFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpFromTime.Location = new System.Drawing.Point(153, 54);
            this.dtpFromTime.Name = "dtpFromTime";
            this.dtpFromTime.ShowUpDown = true;
            this.dtpFromTime.Size = new System.Drawing.Size(110, 26);
            this.dtpFromTime.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(282, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "To:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "From:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(23, 54);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(124, 26);
            this.dtpFromDate.TabIndex = 0;
            // 
            // EasyReportTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupReport);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "EasyReportTool";
            this.Size = new System.Drawing.Size(775, 116);
            this.groupReport.ResumeLayout(false);
            this.groupReport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupReport;
        private System.Windows.Forms.DateTimePicker dtpFromTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.ComboBox cobType;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DateTimePicker dtpToTime;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
