namespace EasyScada.Winforms.Controls
{
    partial class EasyHistoricalAlarm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnChangeAckComment = new System.Windows.Forms.Button();
            this.btnAckAll = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.alarmGrid = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.cobFindBy = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cobAlarmClass = new System.Windows.Forms.ComboBox();
            this.cobAlarmState = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cobAlarmType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cobType = new System.Windows.Forms.ComboBox();
            this.dtpToTime = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromTime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.dataGridViewLinkColumn1 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewLinkColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOutgoingTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAckTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAckComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnChangeAckComment);
            this.groupBox1.Controls.Add(this.btnAckAll);
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.alarmGrid);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cobFindBy);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cobAlarmClass);
            this.groupBox1.Controls.Add(this.cobAlarmState);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cobAlarmType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cobType);
            this.groupBox1.Controls.Add(this.dtpToTime);
            this.groupBox1.Controls.Add(this.dtpToDate);
            this.groupBox1.Controls.Add(this.dtpFromTime);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpFromDate);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(966, 616);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Title";
            // 
            // btnChangeAckComment
            // 
            this.btnChangeAckComment.Location = new System.Drawing.Point(617, 80);
            this.btnChangeAckComment.Name = "btnChangeAckComment";
            this.btnChangeAckComment.Size = new System.Drawing.Size(179, 23);
            this.btnChangeAckComment.TabIndex = 32;
            this.btnChangeAckComment.Text = "Change Ack Comment";
            this.btnChangeAckComment.UseVisualStyleBackColor = true;
            // 
            // btnAckAll
            // 
            this.btnAckAll.Location = new System.Drawing.Point(521, 80);
            this.btnAckAll.Name = "btnAckAll";
            this.btnAckAll.Size = new System.Drawing.Size(90, 23);
            this.btnAckAll.TabIndex = 31;
            this.btnAckAll.Text = "Ack All";
            this.btnAckAll.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(721, 36);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 30;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(521, 36);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 23);
            this.btnRefresh.TabIndex = 29;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // alarmGrid
            // 
            this.alarmGrid.AllowUserToAddRows = false;
            this.alarmGrid.AllowUserToDeleteRows = false;
            this.alarmGrid.AllowUserToOrderColumns = true;
            this.alarmGrid.AllowUserToResizeRows = false;
            this.alarmGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alarmGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.alarmGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.alarmGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.alarmGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAction,
            this.colName,
            this.colState,
            this.colInTime,
            this.colOutgoingTime,
            this.colAlarmText,
            this.colValue,
            this.colLimit,
            this.colClass,
            this.colAlarmType,
            this.colGroup,
            this.colAckTime,
            this.colAckComment});
            this.alarmGrid.Location = new System.Drawing.Point(6, 110);
            this.alarmGrid.Name = "alarmGrid";
            this.alarmGrid.Size = new System.Drawing.Size(954, 500);
            this.alarmGrid.TabIndex = 28;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Find by:";
            // 
            // cobFindBy
            // 
            this.cobFindBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobFindBy.FormattingEnabled = true;
            this.cobFindBy.Location = new System.Drawing.Point(20, 81);
            this.cobFindBy.Name = "cobFindBy";
            this.cobFindBy.Size = new System.Drawing.Size(124, 21);
            this.cobFindBy.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Alarm class:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(405, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Alarm state:";
            // 
            // cobAlarmClass
            // 
            this.cobAlarmClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobAlarmClass.FormattingEnabled = true;
            this.cobAlarmClass.Location = new System.Drawing.Point(150, 81);
            this.cobAlarmClass.Name = "cobAlarmClass";
            this.cobAlarmClass.Size = new System.Drawing.Size(110, 21);
            this.cobAlarmClass.TabIndex = 22;
            // 
            // cobAlarmState
            // 
            this.cobAlarmState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobAlarmState.FormattingEnabled = true;
            this.cobAlarmState.Location = new System.Drawing.Point(405, 81);
            this.cobAlarmState.Name = "cobAlarmState";
            this.cobAlarmState.Size = new System.Drawing.Size(110, 21);
            this.cobAlarmState.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(278, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Alarm type:";
            // 
            // cobAlarmType
            // 
            this.cobAlarmType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobAlarmType.FormattingEnabled = true;
            this.cobAlarmType.Location = new System.Drawing.Point(278, 81);
            this.cobAlarmType.Name = "cobAlarmType";
            this.cobAlarmType.Size = new System.Drawing.Size(121, 21);
            this.cobAlarmType.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(617, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Export type:";
            // 
            // cobType
            // 
            this.cobType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobType.FormattingEnabled = true;
            this.cobType.Location = new System.Drawing.Point(617, 37);
            this.cobType.Name = "cobType";
            this.cobType.Size = new System.Drawing.Size(98, 21);
            this.cobType.TabIndex = 18;
            // 
            // dtpToTime
            // 
            this.dtpToTime.CustomFormat = "HH:mm:ss";
            this.dtpToTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpToTime.Location = new System.Drawing.Point(405, 37);
            this.dtpToTime.Name = "dtpToTime";
            this.dtpToTime.ShowUpDown = true;
            this.dtpToTime.Size = new System.Drawing.Size(110, 20);
            this.dtpToTime.TabIndex = 14;
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(275, 37);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(124, 20);
            this.dtpToDate.TabIndex = 13;
            // 
            // dtpFromTime
            // 
            this.dtpFromTime.CustomFormat = "HH:mm:ss";
            this.dtpFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpFromTime.Location = new System.Drawing.Point(150, 37);
            this.dtpFromTime.Name = "dtpFromTime";
            this.dtpFromTime.ShowUpDown = true;
            this.dtpFromTime.Size = new System.Drawing.Size(110, 20);
            this.dtpFromTime.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(275, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "To:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "From:";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(20, 37);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(124, 20);
            this.dtpFromDate.TabIndex = 9;
            // 
            // dataGridViewLinkColumn1
            // 
            this.dataGridViewLinkColumn1.ActiveLinkColor = System.Drawing.Color.Blue;
            this.dataGridViewLinkColumn1.HeaderText = "Action";
            this.dataGridViewLinkColumn1.MinimumWidth = 50;
            this.dataGridViewLinkColumn1.Name = "dataGridViewLinkColumn1";
            this.dataGridViewLinkColumn1.ReadOnly = true;
            this.dataGridViewLinkColumn1.Text = "Ack";
            this.dataGridViewLinkColumn1.UseColumnTextForLinkValue = true;
            this.dataGridViewLinkColumn1.VisitedLinkColor = System.Drawing.Color.Blue;
            this.dataGridViewLinkColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 60;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Incomming Time";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 109;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "State";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 57;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Alarm Text";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 82;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Value";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 59;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Limit";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 53;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Class";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 57;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Group";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Width = 61;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Ack Comment";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            this.dataGridViewTextBoxColumn9.Width = 98;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Outgoing Time";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Width = 101;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.HeaderText = "Ack Time";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.Width = 77;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.HeaderText = "Ack Comment";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.Width = 98;
            // 
            // colAction
            // 
            this.colAction.ActiveLinkColor = System.Drawing.Color.Blue;
            this.colAction.HeaderText = "Action";
            this.colAction.MinimumWidth = 50;
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            this.colAction.Text = "Ack";
            this.colAction.VisitedLinkColor = System.Drawing.Color.Blue;
            this.colAction.Width = 50;
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 60;
            // 
            // colState
            // 
            this.colState.HeaderText = "State";
            this.colState.Name = "colState";
            this.colState.ReadOnly = true;
            this.colState.Width = 57;
            // 
            // colInTime
            // 
            this.colInTime.HeaderText = "Incomming Time";
            this.colInTime.Name = "colInTime";
            this.colInTime.ReadOnly = true;
            this.colInTime.Width = 109;
            // 
            // colOutgoingTime
            // 
            this.colOutgoingTime.HeaderText = "Outgoing Time";
            this.colOutgoingTime.Name = "colOutgoingTime";
            this.colOutgoingTime.Width = 101;
            // 
            // colAlarmText
            // 
            this.colAlarmText.HeaderText = "Alarm Text";
            this.colAlarmText.Name = "colAlarmText";
            this.colAlarmText.ReadOnly = true;
            this.colAlarmText.Width = 82;
            // 
            // colValue
            // 
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Width = 59;
            // 
            // colLimit
            // 
            this.colLimit.HeaderText = "Limit";
            this.colLimit.Name = "colLimit";
            this.colLimit.ReadOnly = true;
            this.colLimit.Width = 53;
            // 
            // colClass
            // 
            this.colClass.HeaderText = "Class";
            this.colClass.Name = "colClass";
            this.colClass.ReadOnly = true;
            this.colClass.Width = 57;
            // 
            // colAlarmType
            // 
            this.colAlarmType.HeaderText = "Type";
            this.colAlarmType.Name = "colAlarmType";
            this.colAlarmType.Width = 56;
            // 
            // colGroup
            // 
            this.colGroup.HeaderText = "Group";
            this.colGroup.Name = "colGroup";
            this.colGroup.ReadOnly = true;
            this.colGroup.Width = 61;
            // 
            // colAckTime
            // 
            this.colAckTime.HeaderText = "Ack Time";
            this.colAckTime.Name = "colAckTime";
            this.colAckTime.Width = 77;
            // 
            // colAckComment
            // 
            this.colAckComment.HeaderText = "Ack Comment";
            this.colAckComment.Name = "colAckComment";
            this.colAckComment.Width = 98;
            // 
            // EasyHistoricalAlarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "EasyHistoricalAlarm";
            this.Size = new System.Drawing.Size(972, 622);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cobType;
        private System.Windows.Forms.DateTimePicker dtpToTime;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpFromTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cobAlarmType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cobAlarmState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cobAlarmClass;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cobFindBy;
        private System.Windows.Forms.DataGridViewLinkColumn dataGridViewLinkColumn1;
        private System.Windows.Forms.DataGridView alarmGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.Button btnAckAll;
        private System.Windows.Forms.DataGridViewLinkColumn colAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colState;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutgoingTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmText;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAckTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAckComment;
        private System.Windows.Forms.Button btnChangeAckComment;
    }
}
