using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyScada.Core;
using System.IO.Ports;
using Newtonsoft.Json;
using System.IO;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyAlarmSettingDesigner))]
    public partial class EasyAlarmSetting : UserControl
    {
        #region Constructors
        public EasyAlarmSetting()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                InitializeEvents();
                easyNavigator1.SelectedIndex = 0;
                cobComport.Items.Clear();
                for (int i = 1; i <= 100; i++)
                {
                    cobComport.Items.Add($"COM{i}");
                }
                if (!DesignMode)
                {
                    InitializeAlarmSetting(null);
                }
            }
        }

        public EasyAlarmSetting(IServiceProvider serviceProvider = null)
        {
            this.serviceProvider = serviceProvider;
            InitializeComponent();
            InitializeEvents();
            InitializeAlarmSetting(serviceProvider);
            cobComport.Items.Clear();
            for (int i = 1; i <= 100; i++)
            {
                cobComport.Items.Add($"COM{i}");
            }
            easyNavigator1.SelectedIndex = 0;
        }
        #endregion

        #region Fields
        private IServiceProvider serviceProvider;
        private AlarmSetting alarmSetting;
        private BindingList<EmailSetting> emailSettings;
        private BindingList<SMSSetting> smsSettings;
        private BindingList<AlarmClass> alarmClasses;
        private BindingList<AlarmGroup> alarmGroups;
        private BindingList<DiscreteAlarm> discreteAlarms;
        private BindingList<AnalogAlarm> analogAlarms;
        private BindingList<QualityAlarm> qualityAlarms;
        #endregion

        #region Public properties
        public AlarmClass SelectedAlarmClass
        {
            get
            {
                if (gridViewClass.SelectedRows.Count > 0)
                {
                    if (gridViewClass.SelectedRows[0].DataBoundItem is AlarmClass alarmClass)
                        return alarmClass;
                }
                return null;
            }
        }

        public AlarmGroup SelectedAlarmGroup
        {
            get
            {
                if (gridViewGroup.SelectedRows.Count > 0)
                {
                    if (gridViewGroup.SelectedRows[0].DataBoundItem is AlarmGroup alarmGroup)
                        return alarmGroup;
                }
                return null;
            }
        }

        public DiscreteAlarm SelectedDiscreteAlarm
        {
            get
            {
                if (gridViewDiscrete.SelectedRows.Count > 0)
                {
                    if (gridViewDiscrete.SelectedRows[0].DataBoundItem is DiscreteAlarm discreteAlarm)
                        return discreteAlarm;
                }
                return null;
            }
        }

        public AnalogAlarm SelectedAnalogAlarm
        {
            get
            {
                if (gridViewAnalog.SelectedRows.Count > 0)
                {
                    if (gridViewAnalog.SelectedRows[0].DataBoundItem is AnalogAlarm analogAlarm)
                        return analogAlarm;
                }
                return null;
            }
        }

        public EmailSetting SelectedEmailSetting
        {
            get
            {
                if (gridViewEmail.SelectedRows.Count > 0)
                {
                    if (gridViewEmail.SelectedRows[0].DataBoundItem is EmailSetting emailSetting)
                        return emailSetting;
                }
                return null;
            }
        }

        public SMSSetting SelectedSMSSetting
        {
            get
            {
                if (gridViewSMS.SelectedRows.Count > 0)
                {
                    if (gridViewSMS.SelectedRows[0].DataBoundItem is SMSSetting smsSetting)
                        return smsSetting;
                }
                return null;
            }
        }

        public QualityAlarm SelectedQualityAlarm
        {
            get
            {
                if (gridViewQuality.SelectedRows.Count > 0)
                {
                    if (gridViewQuality.SelectedRows[0].DataBoundItem is QualityAlarm qualityAlarm)
                        return qualityAlarm;
                }
                return null;
            }
        }

        public bool IsChanged { get; private set; }
        #endregion

        #region Methods
        private void InitializeEvents()
        {
            btnImport.Click += BtnImport_Click;
            btnExport.Click += BtnExport_Click;
            btnSave.Click += BtnSave_Click;

            // Register alarm class events
            btnAddClass.Click += BtnAddClass_Click;
            btnDeleteClass.Click += BtnDeleteClass_Click;
            btnSaveClass.Click += BtnSaveClass_Click;
            btnClearClass.Click += BtnClearClass_Click;

            // Register alarm group events
            btnAddGroup.Click += BtnAddGroup_Click;
            btnDeleteGroup.Click += BtnDeleteGroup_Click;
            btnSaveGroup.Click += BtnSaveGroup_Click;
            btnClearGroup.Click += BtnClearGroup_Click;
            btnClearEmailSettingGroup.Click += BtnClearEmailSettingGroup_Click;
            btnClearSMSSettingGroup.Click += BtnClearSMSSettingGroup_Click;

            // Register email setting events
            btnAddEmail.Click += BtnAddEmail_Click;
            btnDeleteEmail.Click += BtnDeleteEmail_Click;
            btnSaveEmail.Click += BtnSaveEmail_Click;
            btnClearEmail.Click += BtnClearEmail_Click;

            // Register sms setting events
            btnAddSMS.Click += BtnAddSMS_Click;
            btnDeleteSMS.Click += BtnDeleteSMS_Click;
            btnSaveSMS.Click += BtnSaveSMS_Click;
            btnClearSMS.Click += BtnClearSMS_Click;

            // Register discrete alarm events
            btnAddDiscrete.Click += BtnAddDiscrete_Click;
            btnDeleteDiscrete.Click += BtnDeleteDiscrete_Click;
            btnSaveDiscrete.Click += BtnSaveDiscrete_Click;
            btnClearDiscrete.Click += BtnClearDiscrete_Click;
            btnClearAlarmClassDiscrete.Click += BtnClearAlarmClassDiscrete_Click;
            btnClearAlarmGroupDiscrete.Click += BtnClearAlarmGroupDiscrete_Click;
            btnBrowseTriggerTagDiscrete.Click += BtnBrowseTriggerTagDiscrete_Click;
            btnBrowseTriggerValueDiscrete.Click += BtnBrowseTriggerValueDiscrete_Click;
            btnImportDiscreteAlarm.Click += BtnImportDiscreteAlarm_Click;
            btnExportDiscreteAlarm.Click += BtnExportDiscreteAlarm_Click;

            // Register analog alarm events
            btnAddAnalog.Click += BtnAddAnalog_Click;
            btnDeleteAnalog.Click += BtnDeleteAnalog_Click;
            btnSaveAnalog.Click += BtnSaveAnalog_Click;
            btnClearAnalog.Click += BtnClearAnalog_Click;
            btnClearAlarmClassAnalog.Click += BtnClearAlarmClassAnalog_Click;
            btnClearAlarmGroupAnalog.Click += BtnClearAlarmGroupAnalog_Click;
            btnBrowseTriggerTagAnalog.Click += BtnBrowseTriggerTagAnalog_Click;
            btnBrowseLimitAnalog.Click += BtnBrowseLimitAnalog_Click;
            btnBrowseDeadbandAnalog.Click += BtnBrowseDeadbandAnalog_Click;
            btnImportAnalogAlarm.Click += BtnImportAnalogAlarm_Click;
            btnExportAnalogAlarm.Click += BtnExportAnalogAlarm_Click;

            // Register quality alarm events
            btnAddQuality.Click += BtnAddQuality_Click;
            btnDeleteQuality.Click += BtnDeleteQuality_Click;
            btnSaveQuality.Click += BtnSaveQuality_Click;
            btnClearClassQuality.Click += BtnClearClassQuality_Click;
            btnClearGroupQuality.Click += BtnClearGroupQuality_Click;
            btnBrowseTriggerTagQuality.Click += BtnBrowseTriggerTagQuality_Click;
            btnClearQuality.Click += BtnClearQuality_Click;
            btnImportQualityAlarm.Click += BtnImportQualityAlarm_Click;
            btnExportQualityAlarm.Click += BtnExportQualityAlarm_Click;

            // Register gridview events
            gridViewClass.SelectionChanged += GridViewClass_SelectionChanged;
            gridViewClass.RowsAdded += GridViewClass_RowsAdded;
            gridViewGroup.SelectionChanged += GridViewGroup_SelectionChanged;
            gridViewGroup.RowsAdded += GridViewGroup_RowsAdded;
            gridViewAnalog.SelectionChanged += GridViewAnalog_SelectionChanged;
            gridViewDiscrete.SelectionChanged += GridViewDiscrete_SelectionChanged;
            gridViewQuality.SelectionChanged += GridViewQuality_SelectionChanged;
            gridViewEmail.SelectionChanged += GridViewEmail_SelectionChanged;
            gridViewEmail.RowsAdded += GridViewEmail_RowsAdded;
            gridViewSMS.SelectionChanged += GridViewSMS_SelectionChanged;
            gridViewSMS.RowsAdded += GridViewSMS_RowsAdded;

            foreach (var item in Enum.GetValues(typeof(DeadbandMode)))
                cobDeadbandModeAnalog.Items.Add(item);
            foreach (var item in Enum.GetValues(typeof(LimitMode)))
                cobLimitModeAnalog.Items.Add(item);
            cobTriggerQuality.Items.Add(Quality.Bad);
            cobTriggerQuality.Items.Add(Quality.Good);
        } 

        private void InitializeMembers()
        {
            if (alarmSetting != null)
            {
                emailSettings = new BindingList<EmailSetting>(alarmSetting.EmailSettings);
                smsSettings = new BindingList<SMSSetting>(alarmSetting.SMSSettings);
                alarmClasses = new BindingList<AlarmClass>(alarmSetting.AlarmClasses);
                alarmGroups = new BindingList<AlarmGroup>(alarmSetting.AlarmGroups);
                discreteAlarms = new BindingList<DiscreteAlarm>(alarmSetting.DiscreteAlarms);
                analogAlarms = new BindingList<AnalogAlarm>(alarmSetting.AnalogAlarms);
                qualityAlarms = new BindingList<QualityAlarm>(alarmSetting.QualityAlarms);

                gridViewEmail.AutoGenerateColumns = false;
                gridViewSMS.AutoGenerateColumns = false;
                gridViewClass.AutoGenerateColumns = false;
                gridViewGroup.AutoGenerateColumns = false;
                gridViewDiscrete.AutoGenerateColumns = false;
                gridViewAnalog.AutoGenerateColumns = false;
                gridViewQuality.AutoGenerateColumns = false;

                gridViewEmail.DataSource = emailSettings;
                gridViewSMS.DataSource = smsSettings;
                gridViewClass.DataSource = alarmClasses;
                gridViewGroup.DataSource = alarmGroups;
                gridViewDiscrete.DataSource = discreteAlarms;
                gridViewAnalog.DataSource = analogAlarms;
                gridViewQuality.DataSource = qualityAlarms;

                foreach (DataGridViewColumn col in gridViewEmail.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                foreach (DataGridViewColumn col in gridViewSMS.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                foreach (DataGridViewColumn col in gridViewClass.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                foreach (DataGridViewColumn col in gridViewGroup.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                foreach (DataGridViewColumn col in gridViewDiscrete.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                foreach (DataGridViewColumn col in gridViewAnalog.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                foreach (DataGridViewColumn col in gridViewQuality.Columns)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;

            }
        }

        public void InitializeAlarmSetting(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            AlarmSetting alarmSetting = new AlarmSetting();
            this.alarmSetting = alarmSetting;
            InitializeMembers();

            try
            {
                string applicationPath = DesignerHelper.GetApplicationOutputPath(serviceProvider);
                string alarmSettingPath = applicationPath + "\\AlarmSetting.json";
                if (File.Exists(alarmSettingPath))
                {
                    LoadAlarmSetting(alarmSettingPath);
                }
                else
                {
                    emailSettings.Add(new EmailSetting() { ReadOnly = true, Enabled = true, Name = "Default" });
                    smsSettings.Add(new SMSSetting() { ReadOnly = true, Enabled = true, Name = "Default" });
                    alarmClasses.Add(new AlarmClass() { ReadOnly = true, Enabled = true, Name = "Errors" });
                    alarmClasses.Add(new AlarmClass() { ReadOnly = true, Enabled = true, Name = "Warnings", BackColorIncoming = "#FF7C4D" });
                    alarmClasses.Add(new AlarmClass() { ReadOnly = true, Enabled = true, Name = "Systems" });
                    alarmGroups.Add(new AlarmGroup() { ReadOnly = true, Enabled = true, Name = "Default" });
                }
            }
            catch { }

            DisplayItem(SelectedAlarmClass);
            DisplayItem(SelectedAlarmGroup);
            DisplayItem(SelectedEmailSetting);
            DisplayItem(SelectedSMSSetting);
            DisplayItem(SelectedDiscreteAlarm);
            DisplayItem(SelectedAnalogAlarm);
            DisplayItem(SelectedQualityAlarm);

            ReloadAlarmClassComboBoxSource();
            ReloadAlarmGroupComboBoxSource();
            ReloadEmailSettingComboBoxSource();
            ReloadSMSSettingComboBoxSource();

            UpdateButtonState(SelectedAlarmClass);
            UpdateButtonState(SelectedAlarmGroup);
            UpdateButtonState(SelectedAnalogAlarm);
            UpdateButtonState(SelectedDiscreteAlarm);
            UpdateButtonState(SelectedEmailSetting);
            UpdateButtonState(SelectedSMSSetting);
            UpdateButtonState(SelectedQualityAlarm);
        }

        private void ReloadAlarmClassComboBoxSource()
        {
            string selectedClass = cobClassAnalog.Text;
            var alarmClassNames = alarmClasses.Select(x => x.Name).ToArray();
            cobClassAnalog.Items.Clear();
            cobClassAnalog.Items.AddRange(alarmClassNames);
            if (alarmClassNames.Contains(selectedClass))
                cobClassAnalog.Text = selectedClass;
            else
                cobClassAnalog.Text = "";

            selectedClass = cobClassDiscrete.Text;
            cobClassDiscrete.Items.Clear();
            cobClassDiscrete.Items.AddRange(alarmClassNames);
            if (alarmClassNames.Contains(selectedClass))
                cobClassDiscrete.Text = selectedClass;
            else
                cobClassDiscrete.Text = "";

            selectedClass = cobClassQuality.Text;
            cobClassQuality.Items.Clear();
            cobClassQuality.Items.AddRange(alarmClassNames);
            if (alarmClassNames.Contains(selectedClass))
                cobClassQuality.Text = selectedClass;
            else
                cobClassQuality.Text = "";
        }

        private void ReloadAlarmGroupComboBoxSource()
        {
            string selectedGroup = cobGroupAnalog.Text;
            var alarmGroupNames = alarmGroups.Select(x => x.Name).ToArray();
            cobGroupAnalog.Items.Clear();
            cobGroupAnalog.Items.AddRange(alarmGroupNames);
            if (alarmGroupNames.Contains(selectedGroup))
                cobGroupAnalog.Text = selectedGroup;
            else
                cobGroupAnalog.Text = "";

            selectedGroup = cobGroupDiscrete.Text;
            cobGroupDiscrete.Items.Clear();
            cobGroupDiscrete.Items.AddRange(alarmGroupNames);
            if (alarmGroupNames.Contains(selectedGroup))
                cobGroupDiscrete.Text = selectedGroup;
            else
                cobGroupDiscrete.Text = "";

            selectedGroup = cobGroupQuality.Text;
            cobGroupQuality.Items.Clear();
            cobGroupQuality.Items.AddRange(alarmGroupNames);
            if (alarmGroupNames.Contains(selectedGroup))
                cobGroupQuality.Text = selectedGroup;
            else
                cobGroupQuality.Text = "";
        }

        private void ReloadEmailSettingComboBoxSource()
        {
            string selectedSetting = cobEmailGroup.Text;
            var emailSettingNames = emailSettings.Select(x => x.Name).ToArray();
            cobEmailGroup.Items.Clear();
            cobEmailGroup.Items.AddRange(emailSettingNames);
            if (emailSettingNames.Contains(selectedSetting))
                cobEmailGroup.Text = selectedSetting;
            else
                cobEmailGroup.Text = null;
        }

        private void ReloadSMSSettingComboBoxSource()
        {
            string selectedSetting = cobSMSGroup.Text;
            var smsSettingNames = smsSettings.Select(x => x.Name).ToArray();
            cobSMSGroup.Items.Clear();
            cobSMSGroup.Items.AddRange(smsSettingNames);
            if (smsSettingNames.Contains(selectedSetting))
                cobSMSGroup.Text = selectedSetting;
            else
                cobSMSGroup.Text = null;
        }

        private void UpdateAlarmClassName(string oldName, string newName)
        {
            foreach (var item in discreteAlarms)
            {
                if (item.AlarmClassName == oldName)
                    item.AlarmClassName = newName;
            }
            gridViewDiscrete.Refresh();
            if (cobClassDiscrete.Text == oldName)
                cobClassDiscrete.Text = newName;

            foreach (var item in analogAlarms)
            {
                if (item.AlarmClassName == oldName)
                    item.AlarmClassName = newName;
            }
            gridViewAnalog.Refresh();
            if (cobClassAnalog.Text == oldName)
                cobClassAnalog.Text = newName;

            foreach (var item in qualityAlarms)
            {
                if (item.AlarmClassName == oldName)
                    item.AlarmClassName = newName;
            }
            gridViewQuality.Refresh();
            if (cobClassQuality.Text == oldName)
                cobClassQuality.Text = newName;
        }

        private void UpdateAlarmGroupName(string oldName, string newName)
        {
            foreach (var item in discreteAlarms)
            {
                if (item.AlarmGroupName == oldName)
                    item.AlarmGroupName = newName;
            }
            gridViewDiscrete.Refresh();
            if (cobGroupDiscrete.Text == oldName)
                cobGroupDiscrete.Text = newName;

            foreach (var item in analogAlarms)
            {
                if (item.AlarmGroupName == oldName)
                    item.AlarmGroupName = newName;
            }
            gridViewAnalog.Refresh();
            if (cobGroupAnalog.Text == oldName)
                cobGroupAnalog.Text = newName;

            foreach (var item in qualityAlarms)
            {
                if (item.AlarmGroupName == oldName)
                    item.AlarmGroupName = newName;
            }
            gridViewQuality.Refresh();
            if (cobGroupQuality.Text == oldName)
                cobGroupQuality.Text = newName;
        }

        private void UpdateEmailSettingName(string oldName, string newName)
        {
            foreach (var item in alarmGroups)
            {
                if (item.EmailSettingName == oldName)
                    item.EmailSettingName = newName;
            }
            gridViewGroup.Refresh();
            if (cobEmailGroup.Items.Contains(oldName))
            {
                int index = cobEmailGroup.Items.IndexOf(oldName);
                if (string.IsNullOrEmpty(newName))
                    cobEmailGroup.Items.RemoveAt(index);
                else
                    cobEmailGroup.Items[index] = newName;
            }
            if (cobEmailGroup.Text == oldName)
                cobEmailGroup.Text = newName;
        }

        private void UpdateSMSSettingName(string oldName, string newName)
        {
            foreach (var item in alarmGroups)
            {
                if (item.SMSSettingName == oldName)
                    item.SMSSettingName = newName;
            }
            gridViewGroup.Refresh();

            if (cobSMSGroup.Items.Contains(oldName))
            {
                int index = cobSMSGroup.Items.IndexOf(oldName);
                if (string.IsNullOrEmpty(newName))
                    cobSMSGroup.Items.RemoveAt(index);
                else
                    cobSMSGroup.Items[index] = newName;
            }
            if (cobSMSGroup.Text == oldName)
                cobSMSGroup.Text = newName;
        }

        private void SetSelectedItem(DataGridView gridView, object selectedItem)
        {
            if (gridView != null)
            {
                foreach (DataGridViewRow row in gridView.Rows)
                {
                    if (Equals(row.DataBoundItem, selectedItem))
                    {
                        row.Selected = true;
                        return;
                    }
                }
                gridView.ClearSelection();
            }
        }

        private string GetObjectName(object item)
        {
            if (item is IUniqueNameItem nameItem)
                return nameItem.Name;
            throw new ArgumentException();
        }

        private bool ShowDeleteComfirm(string message = "Do you want to delete all selected items?")
        {
            var mbr = MessageBox.Show(message, "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return mbr == DialogResult.Yes;
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void DisplayItem<T>(T item) where T : class
        {
            Type itemType = typeof(T);
            if (itemType == typeof(AlarmClass))
            {
                if (item == null)
                {
                    ckbEnabledClass.Checked = false;
                    txbNameClass.Clear();
                    txbDescriptionClass.Clear();
                    btnBackColorAck.SelectedColor = Color.Transparent;
                    btnBackColorIncomming.SelectedColor = Color.Transparent;
                    btnBackColorOutgoing.SelectedColor = Color.Transparent;
                }
                else
                {
                    if (item is AlarmClass alarmClass)
                    {
                        ckbEnabledClass.Checked = alarmClass.Enabled;
                        txbNameClass.Text = alarmClass.Name;
                        txbDescriptionClass.Text = alarmClass.Description;
                        btnBackColorAck.SelectedColor = alarmClass.GetColorWinform(alarmClass.BackColorAcknowledged);
                        btnBackColorIncomming.SelectedColor = alarmClass.GetColorWinform(alarmClass.BackColorIncoming);
                        btnBackColorOutgoing.SelectedColor = alarmClass.GetColorWinform(alarmClass.BackColorOutgoing);
                    }
                }
            }
            else if (itemType == typeof(AlarmGroup))
            {
                if (item == null)
                {
                    ckbEnabledGroup.Checked = false;
                    txbNameGroup.Clear();
                    txbDescriptionGroup.Clear();
                    cobEmailGroup.Text = "";
                    cobSMSGroup.Text = "";
                }
                else
                {
                    if (item is AlarmGroup alarmGroup)
                    {
                        ckbEnabledGroup.Checked = alarmGroup.Enabled;
                        txbNameGroup.Text = alarmGroup.Name;
                        txbDescriptionGroup.Text = alarmGroup.Description;
                        cobEmailGroup.Text = alarmGroup.EmailSettingName;
                        cobSMSGroup.Text = alarmGroup.SMSSettingName;
                    }
                }
            }
            else if (itemType == typeof(DiscreteAlarm))
            {
                if (item == null)
                {
                    ckbEnabledDiscrete.Checked = false;
                    txbNameDiscrete.Clear();
                    cobClassDiscrete.Text = "";
                    cobGroupDiscrete.Text = "";
                    txbTriggerTagDiscrete.Clear();
                    txbTriggerValueDiscrete.Clear();
                    txbDescriptionDiscrete.Clear();
                    txbAlarmTextDiscrete.Clear();
                }
                else
                {
                    if (item is DiscreteAlarm discreteAlarm)
                    {
                        ckbEnabledDiscrete.Checked = discreteAlarm.Enabled;
                        txbNameDiscrete.Text = discreteAlarm.Name;
                        cobClassDiscrete.Text = discreteAlarm.AlarmClassName;
                        cobGroupDiscrete.Text = discreteAlarm.AlarmGroupName;
                        txbTriggerTagDiscrete.Text = discreteAlarm.TriggerTagPath;
                        txbTriggerValueDiscrete.Text = discreteAlarm.TriggerValue;
                        txbDescriptionDiscrete.Text = discreteAlarm.Description;
                        txbAlarmTextDiscrete.Text = discreteAlarm.AlarmText;
                    }
                }
            }
            else if (itemType == typeof(QualityAlarm))
            {
                if (item == null)
                {
                    ckbEnabledQuality.Checked = false;
                    txbNameQuality.Clear();
                    cobClassQuality.Text = "";
                    cobGroupQuality.Text = "";
                    txbTriggerTagQuality.Clear();
                    cobTriggerQuality.Text = "";
                    txbDescriptionQuality.Clear();
                    txbAlarmTextQuality.Clear();
                }
                else
                {
                    if (item is QualityAlarm qualityAlarm)
                    {
                        ckbEnabledQuality.Checked = qualityAlarm.Enabled;
                        txbNameQuality.Text = qualityAlarm.Name;
                        cobClassQuality.Text = qualityAlarm.AlarmClassName;
                        cobGroupQuality.Text = qualityAlarm.AlarmGroupName;
                        txbTriggerTagQuality.Text = qualityAlarm.TriggerTagPath;
                        cobTriggerQuality.Text = qualityAlarm.TriggerQuality.ToString();
                        txbDescriptionQuality.Text = qualityAlarm.Description;
                        txbAlarmTextQuality.Text = qualityAlarm.AlarmText;
                    }
                }
            }
            else if (itemType == typeof(AnalogAlarm))
            {
                if (item == null)
                {
                    ckbEnabledAnalog.Checked = false;
                    txbNameAnalog.Clear();
                    cobDeadbandModeAnalog.SelectedIndex = 0;
                    ckbDeadbandInPercentAnalog.Checked = false;
                    cobClassAnalog.Text = "";
                    cobGroupAnalog.Text = "";
                    cobLimitModeAnalog.SelectedIndex = 0;
                    txbTriggerTagAnalog.Clear();
                    txbLimitAnalog.Clear();
                    txbDescriptionAnalog.Clear();
                    txbDeadbandValueAnalog.Clear();
                    txbAlarmTextAnalog.Clear();
                }
                else
                {
                    if (item is AnalogAlarm analogAlarm)
                    {
                        ckbEnabledAnalog.Checked = analogAlarm.Enabled;
                        txbNameAnalog.Text = analogAlarm.Name;
                        cobDeadbandModeAnalog.SelectedItem = analogAlarm.DeadbandMode;
                        ckbDeadbandInPercentAnalog.Checked = analogAlarm.DeadbandInPercentage;
                        cobClassAnalog.Text = analogAlarm.AlarmClassName;
                        cobGroupAnalog.Text = analogAlarm.AlarmGroupName;
                        cobLimitModeAnalog.SelectedItem = analogAlarm.LimitMode;
                        txbTriggerTagAnalog.Text = analogAlarm.TriggerTagPath;
                        txbLimitAnalog.Text = analogAlarm.Limit;
                        txbDescriptionAnalog.Text = analogAlarm.Description;
                        txbDeadbandValueAnalog.Text = analogAlarm.DeadbandValue;
                        txbAlarmTextAnalog.Text = analogAlarm.AlarmText;
                    }
                }
            }
            else if (itemType == typeof(EmailSetting))
            {
                if (item == null)
                {
                    ckbEnableEmail.Checked = false;
                    txbNameEmail.Clear();
                    txbEmailTo.Clear();
                    txbHost.Clear();
                    txbPort.Clear();
                    txbCredentialEmail.Clear();
                    txbCredentialPassword.Clear();
                    txbTimeoutEmail.Clear();
                    txbCCTo.Clear();
                    ckbEnableSSL.Checked = false;
                }
                else
                {
                    if (item is EmailSetting emailSetting)
                    {
                        ckbEnableEmail.Checked = emailSetting.Enabled;
                        txbNameEmail.Text = emailSetting.Name;
                        txbEmailTo.Text = emailSetting.EmailsString;
                        txbHost.Text = emailSetting.Host;
                        txbPort.Text = emailSetting.Port.ToString();
                        txbCredentialEmail.Text = emailSetting.CredentialEmail;
                        txbCredentialPassword.Text = emailSetting.CredentialPassword;
                        txbTimeoutEmail.Text = emailSetting.Timeout.ToString();
                        txbCCTo.Text = emailSetting.CCString;
                        ckbEnableSSL.Checked = emailSetting.EnableSSL;             
                    }
                }
            }
            else if (itemType == typeof(SMSSetting))
            {
                if (item == null)
                {
                    ckbEnabledSMS.Checked = false;
                    txbNameSMS.Clear();
                    txbSmsTo.Clear();
                    cobComport.Text = "";
                    cobBaudrate.Text = "";
                    cobDatabits.Text = "";
                    cobParity.Text = "";
                    cobStopbits.Text = "";
                    txbTimeoutSMS.Text = "";
                }
                else
                {
                    if (item is SMSSetting smsSetting)
                    {
                        ckbEnabledSMS.Checked = smsSetting.Enabled;
                        txbNameSMS.Text = smsSetting.Name;
                        txbSmsTo.Text = smsSetting.PhoneNumbers;
                        cobComport.Text = smsSetting.ComPort;
                        cobBaudrate.Text = smsSetting.Baudrate.ToString();
                        cobDatabits.Text = smsSetting.DataBits.ToString();
                        cobParity.Text = smsSetting.Parity;
                        cobStopbits.Text = smsSetting.StopBits;
                        txbTimeoutSMS.Text = smsSetting.Timeout.ToString();
                    }
                }
            }
        }

        private string ValidateItem<T>(T item)
        {
            string result = string.Empty;
            Type itemType = typeof(T);
            if (itemType == typeof (AlarmClass))
            {
                if (item is AlarmClass alarmClass)
                {
                    result = txbNameClass.Text.ValidateName("alarm class");
                    if (string.IsNullOrEmpty(result))
                    {
                        if (!txbNameClass.Text.IsUniqueStringInCollection(alarmClasses.Where(x => x != alarmClass).ToArray(), GetObjectName))
                        {
                            result = $"The alarm class name '{txbNameClass.Text}' is already used.";
                        }
                    }
                }
            }
            else if (itemType == typeof(AlarmGroup))
            {
                if (item is AlarmGroup alarmGroup)
                {
                    result = txbNameGroup.Text.ValidateName("alarm group");
                    if (string.IsNullOrEmpty(result))
                    {
                        if (!txbNameGroup.Text.IsUniqueStringInCollection(alarmGroups.Where(x => x != alarmGroup).ToArray(), GetObjectName))
                        {
                            result = $"The alarm group name '{txbNameGroup.Text}' is already used.";
                        }
                    }
                }
            }
            else if (itemType == typeof(DiscreteAlarm))
            {

            }
            else if (itemType == typeof(AnalogAlarm))
            {

            }
            else if (itemType == typeof(QualityAlarm))
            {

            }
            else if (itemType == typeof(EmailSetting))
            {
                if (item is EmailSetting emailSetting)
                {
                    result = txbNameEmail.Text.ValidateName("email setting");
                    if (string.IsNullOrEmpty(result))
                    {
                        if (!txbNameEmail.Text.IsUniqueStringInCollection(emailSettings.Where(x => x != emailSetting).ToArray(), GetObjectName))
                        {
                            result = $"The email setting name '{txbNameEmail.Text}' is already used.";
                        }
                    }

                    if (!ushort.TryParse(txbPort.Text, out ushort port))
                    {
                        return $"The port valid port range is 1 - 65535"; 
                    }

                    if (!int.TryParse(txbTimeoutEmail.Text, out int timeout))
                    {
                        return $"The time out must be a number and larger than 1000";
                    }

                    if (timeout < 1000)
                    {
                        return $"The time out must be a number and larger than 1000";
                    }

                }
            }
            else if (itemType == typeof (SMSSetting))
            {
                if (item is SMSSetting smsSetting)
                {
                    result = txbNameSMS.Text.ValidateName("sms setting");
                    if (string.IsNullOrEmpty(result))
                    {
                        if (!txbNameSMS.Text.IsUniqueStringInCollection(smsSettings.Where(x => x != smsSetting).ToArray(), GetObjectName))
                        {
                            result = $"The sms setting name '{txbNameSMS.Text}' is already used.";
                        }
                    }
                    
                    if (!int.TryParse(txbTimeoutEmail.Text, out int timeout))
                    {
                        return $"The time out must be a number and larger than 1000";
                    }

                    if (timeout < 1000)
                    {
                        return $"The time out must be a number and larger than 1000";
                    }
                }
            }
            return result;
        }

        private void UpdateButtonState<T>(T item)
        {
            Type itemType = typeof(T);
            if (itemType == typeof(AlarmClass))
            {
                if (item is AlarmClass alarmClass)
                {
                    btnDeleteClass.Enabled = !alarmClass.ReadOnly;
                    btnSaveClass.Enabled = true;
                }
                else
                {
                    btnDeleteClass.Enabled = false;
                    btnSaveClass.Enabled = false;
                }
            }
            else if (itemType == typeof(AlarmGroup))
            {
                if (item is AlarmGroup alarmGroup)
                {
                    btnDeleteGroup.Enabled = !alarmGroup.ReadOnly;
                    btnSaveGroup.Enabled = true;
                }
                else
                {
                    btnDeleteGroup.Enabled = false;
                    btnSaveGroup.Enabled = false;
                }
            }
            else if (itemType == typeof(DiscreteAlarm))
            {
                if (item is DiscreteAlarm discreteAlarm)
                {
                    btnDeleteDiscrete.Enabled = true;
                    btnSaveDiscrete.Enabled = true;
                }
                else
                {
                    btnDeleteDiscrete.Enabled = false;
                    btnSaveDiscrete.Enabled = false;
                }
            }
            else if (itemType == typeof(AnalogAlarm))
            {
                if (item is AnalogAlarm analogAlarm)
                {
                    btnDeleteAnalog.Enabled = true;
                    btnSaveAnalog.Enabled = true;
                }
                else
                {
                    btnDeleteAnalog.Enabled = false;
                    btnSaveAnalog.Enabled = false;
                }
            }
            else if (itemType == typeof(QualityAlarm))
            {
                if (item is QualityAlarm qualityAlarm)
                {
                    btnDeleteQuality.Enabled = true;
                    btnSaveQuality.Enabled = true;
                }
                else
                {
                    btnDeleteQuality.Enabled = false;
                    btnSaveQuality.Enabled = false;
                }
            }
            else if (itemType == typeof(EmailSetting))
            {
                if (item is EmailSetting emailSetting)
                {
                    btnDeleteEmail.Enabled = !emailSetting.ReadOnly;
                    btnSaveEmail.Enabled = true;
                }
                else
                {
                    btnDeleteEmail.Enabled = false;
                    btnSaveEmail.Enabled = false;
                }
            }
            else if (itemType == typeof(SMSSetting))
            {
                if (item is SMSSetting smsSetting)
                {
                    btnDeleteSMS.Enabled = !smsSetting.ReadOnly;
                    btnSaveSMS.Enabled = true;
                }
                else
                {
                    btnDeleteSMS.Enabled = false;
                    btnSaveSMS.Enabled = false;
                }
            }
        }

        private void UpdateEditControlState<T>(T item) where T : class
        {
            Type itemType = typeof(T);
            if (itemType == typeof(AlarmClass))
            {
                if (item is AlarmClass alarmClass)
                {
                    if (alarmClass.ReadOnly)
                    {
                        txbNameClass.Enabled = false;
                        return;
                    }
                }
                txbNameClass.Enabled = true;
            }
            else if (itemType == typeof(AlarmGroup))
            {
                if (item is AlarmGroup alarmGroup)
                {
                    if (alarmGroup.ReadOnly)
                    {
                        txbNameGroup.Enabled = false;
                        return;
                    }
                }
                txbNameGroup.Enabled = true;
            }
            else if (itemType == typeof(EmailSetting))
            {
                if (item is EmailSetting emailSetting)
                {
                    if (emailSetting.ReadOnly)
                    {
                        txbNameEmail.Enabled = false;
                        return;
                    }
                }
                txbNameEmail.Enabled = true;
            }
            else if (itemType == typeof(SMSSetting))
            {
                if (item is SMSSetting smsSetting)
                {
                    if (smsSetting.ReadOnly)
                    {
                        txbNameSMS.Enabled = false;
                        return;
                    }
                }
                txbNameSMS.Enabled = true;
            }
        }

        private void UpdateBackColorAlarmClassRow(DataGridViewRow row)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.OwningColumn.HeaderText.Contains("Color"))
                {
                    if (cell.Value != null)
                    {
                        Color backColor = cell.Value.ToString().ToColor();
                        cell.Style.BackColor = backColor;
                        cell.Style.SelectionBackColor = backColor;
                    }
                }
            }
        }

        #endregion

        #region Event handlers

        #region Save
        private void BtnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
        #endregion

        #region Import & Export
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Alarm Setting File (*.json)|*.json";
                saveFileDialog1.Title = "Export";
                saveFileDialog1.FileName = "AlarmSetting.json";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string jsonRes = JsonConvert.SerializeObject(alarmSetting, Formatting.Indented);
                    File.WriteAllText(saveFileDialog1.FileName, jsonRes);
                    MessageBox.Show("Export successfully!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when export alarm setting. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Alarm Setting File (*.json)|*.json";
                openFileDialog1.Title = "Import";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    LoadAlarmSetting(openFileDialog1.FileName);
                    IsChanged = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when import alarm setting. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Alarm class event handlers
        private void BtnClearClass_Click(object sender, EventArgs e)
        {
            if (alarmClasses != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all alarm class item ?"))
                {
                    alarmClasses.Where(x => !x.ReadOnly).ToList().ForEach(x => alarmClasses.Remove(x));
                    ReloadAlarmClassComboBoxSource();
                    IsChanged = true;
                }
            }
        }

        private void BtnSaveClass_Click(object sender, EventArgs e)
        {
            if (SelectedAlarmClass != null)
            {
                string validateRes = ValidateItem(SelectedAlarmClass);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedAlarmClass.Enabled = ckbEnabledClass.Checked;
                    string oldName = SelectedAlarmClass.Name;
                    SelectedAlarmClass.Name = txbNameClass.Text?.Trim();
                    SelectedAlarmClass.Description = txbDescriptionClass.Text?.Trim();
                    SelectedAlarmClass.BackColorAcknowledged = btnBackColorAck.SelectedColor.ToHexString();
                    SelectedAlarmClass.BackColorIncoming = btnBackColorIncomming.SelectedColor.ToHexString();
                    SelectedAlarmClass.BackColorOutgoing = btnBackColorOutgoing.SelectedColor.ToHexString();
                    gridViewClass.Refresh();
                    foreach (DataGridViewRow row in gridViewClass.Rows)
                        UpdateBackColorAlarmClassRow(row);

                    if (oldName != SelectedAlarmClass.Name)
                        UpdateAlarmClassName(oldName, SelectedAlarmClass.Name);
                    ReloadAlarmClassComboBoxSource();
                    IsChanged = true;
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteClass_Click(object sender, EventArgs e)
        {
            if (SelectedAlarmClass != null)
            {
                if (!SelectedAlarmClass.ReadOnly)
                {
                    if (ShowDeleteComfirm())
                    {
                        int deleteIndex = alarmClasses.IndexOf(SelectedAlarmClass);
                        alarmClasses.Remove(SelectedAlarmClass);
                        ReloadAlarmClassComboBoxSource();
                        if (alarmClasses.Count > 0)
                        {
                            if (deleteIndex >= alarmClasses.Count)
                                SetSelectedItem(gridViewClass, alarmClasses[alarmClasses.Count - 1]);
                            else
                                SetSelectedItem(gridViewClass, alarmClasses[deleteIndex]);
                        }
                        IsChanged = true;
                    }
                }
            }
        }

        private void BtnAddClass_Click(object sender, EventArgs e)
        {
            AlarmClass alarmClass = new AlarmClass();
            alarmClass.Enabled = true;
            alarmClass.ReadOnly = false;
            alarmClass.Name = alarmClasses.GetUniqueNameInCollection(GetObjectName, "Alarm_class_1");
            alarmClasses.Add(alarmClass);
            SetSelectedItem(gridViewClass, alarmClass);
            ReloadAlarmClassComboBoxSource();
            IsChanged = true;
        }
        #endregion

        #region Alarm group event handlers
        private void BtnClearGroup_Click(object sender, EventArgs e)
        {
            if (alarmGroups != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all alarm group item ?"))
                {
                    alarmGroups.Where(x => !x.ReadOnly).ToList().ForEach(x => alarmGroups.Remove(x));
                    ReloadAlarmGroupComboBoxSource();
                    IsChanged = true;
                }
            }
        }

        private void BtnSaveGroup_Click(object sender, EventArgs e)
        {
            if (SelectedAlarmGroup != null)
            {
                string validateRes = ValidateItem(SelectedAlarmGroup);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedAlarmGroup.Enabled = ckbEnabledGroup.Checked;
                    string oldName = SelectedAlarmGroup.Name;
                    SelectedAlarmGroup.Name = txbNameGroup.Text?.Trim();
                    SelectedAlarmGroup.Description = txbDescriptionGroup.Text?.Trim();
                    SelectedAlarmGroup.SMSSettingName = cobSMSGroup.Text?.Trim();
                    SelectedAlarmGroup.EmailSettingName = cobEmailGroup.Text?.Trim();
                    gridViewGroup.Refresh();

                    if (oldName != SelectedAlarmGroup.Name)
                        UpdateAlarmGroupName(oldName, SelectedAlarmGroup.Name);
                    IsChanged = true;
                    ReloadAlarmGroupComboBoxSource();
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (SelectedAlarmGroup != null)
            {
                if (!SelectedAlarmGroup.ReadOnly)
                {
                    if (ShowDeleteComfirm())
                    {
                        int deleteIndex = alarmGroups.IndexOf(SelectedAlarmGroup);
                        alarmGroups.Remove(SelectedAlarmGroup);
                        ReloadAlarmGroupComboBoxSource();
                        if (alarmGroups.Count > 0)
                        {
                            if (deleteIndex >= alarmGroups.Count)
                                SetSelectedItem(gridViewGroup, alarmGroups[alarmGroups.Count - 1]);
                            else
                                SetSelectedItem(gridViewGroup, alarmGroups[deleteIndex]);
                        }
                        IsChanged = true;
                    }
                }
            }
        }

        private void BtnAddGroup_Click(object sender, EventArgs e)
        {
            AlarmGroup alarmGroup = new AlarmGroup();
            alarmGroup.Enabled = true;
            alarmGroup.ReadOnly = false;
            alarmGroup.Name = alarmGroups.GetUniqueNameInCollection(GetObjectName, "Alarm_group_1");
            alarmGroups.Add(alarmGroup);
            SetSelectedItem(gridViewGroup, alarmGroup);
            ReloadAlarmGroupComboBoxSource();
            IsChanged = true;
        }

        private void BtnClearSMSSettingGroup_Click(object sender, EventArgs e)
        {
            cobSMSGroup.Text = null;
        }

        private void BtnClearEmailSettingGroup_Click(object sender, EventArgs e)
        {
            cobEmailGroup.Text = null;
        }
        #endregion

        #region Email setting event handlers
        private void BtnClearEmail_Click(object sender, EventArgs e)
        {
            if (emailSettings != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all email setting item ?"))
                {
                    emailSettings.Where(x => !x.ReadOnly).ToList().ForEach(x => emailSettings.Remove(x));
                    ReloadEmailSettingComboBoxSource();
                    IsChanged = true;
                }
            }
        }

        private void BtnSaveEmail_Click(object sender, EventArgs e)
        {
            if (SelectedEmailSetting != null)
            {
                string validateRes = ValidateItem(SelectedEmailSetting);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedEmailSetting.Enabled = ckbEnableEmail.Checked;
                    string oldName = SelectedEmailSetting.Name;
                    SelectedEmailSetting.Name = txbNameEmail.Text?.Trim();
                    SelectedEmailSetting.EmailsString = txbEmailTo.Text?.Trim();
                    SelectedEmailSetting.CCString = txbCCTo.Text?.Trim();
                    SelectedEmailSetting.Host = txbHost.Text?.Trim();
                    SelectedEmailSetting.CredentialEmail = txbCredentialEmail.Text?.Trim();
                    SelectedEmailSetting.CredentialPassword = txbCredentialPassword.Text?.Trim();
                    SelectedEmailSetting.Port = int.Parse(txbPort.Text?.Trim());
                    SelectedEmailSetting.Timeout = int.Parse(txbTimeoutEmail.Text?.Trim());
                    SelectedEmailSetting.EnableSSL = ckbEnableSSL.Checked;


                    gridViewEmail.Refresh();
                    if (oldName != SelectedEmailSetting.Name)
                        UpdateEmailSettingName(oldName, SelectedEmailSetting.Name);
                    IsChanged = true;
                    ReloadEmailSettingComboBoxSource();
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteEmail_Click(object sender, EventArgs e)
        {
            if (SelectedEmailSetting != null)
            {
                if (!SelectedEmailSetting.ReadOnly)
                {
                    if (ShowDeleteComfirm())
                    {
                        int deleteIndex = emailSettings.IndexOf(SelectedEmailSetting);
                        string oldName = SelectedEmailSetting.Name;
                        emailSettings.Remove(SelectedEmailSetting);
                        ReloadEmailSettingComboBoxSource();
                        UpdateEmailSettingName(oldName, null);
                        if (emailSettings.Count > 0)
                        {
                            if (deleteIndex >= emailSettings.Count)
                                SetSelectedItem(gridViewEmail, emailSettings[emailSettings.Count - 1]);
                            else
                                SetSelectedItem(gridViewEmail, emailSettings[deleteIndex]);
                        }
                        IsChanged = true;
                    }
                }
            }
        }

        private void BtnAddEmail_Click(object sender, EventArgs e)
        {
            EmailSetting emailSetting = new EmailSetting();
            emailSetting.Enabled = true;
            emailSetting.ReadOnly = false;
            emailSetting.Name = emailSettings.GetUniqueNameInCollection(GetObjectName, "Email_setting_1");
            emailSettings.Add(emailSetting);
            SetSelectedItem(gridViewEmail, emailSetting);
            ReloadEmailSettingComboBoxSource();
            IsChanged = true;
        }
        #endregion

        #region SMS setting event handlers
        private void BtnClearSMS_Click(object sender, EventArgs e)
        {
            if (smsSettings != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all sms setting item ?"))
                {
                    smsSettings.Where(x => !x.ReadOnly).ToList().ForEach(x => smsSettings.Remove(x));
                    ReloadSMSSettingComboBoxSource();
                    IsChanged = true;
                }
            }
        }

        private void BtnSaveSMS_Click(object sender, EventArgs e)
        {
            if (SelectedSMSSetting != null)
            {
                string validateRes = ValidateItem(SelectedSMSSetting);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedSMSSetting.Enabled = ckbEnabledSMS.Checked;
                    string oldName = SelectedSMSSetting.Name;
                    SelectedSMSSetting.Name = txbNameSMS.Text?.Trim();
                    SelectedSMSSetting.PhoneNumbers = txbSmsTo.Text?.Trim();
                    SelectedSMSSetting.ComPort = cobComport.Text?.Trim();
                    SelectedSMSSetting.Baudrate = int.Parse(cobBaudrate.Text?.Trim());
                    SelectedSMSSetting.DataBits = int.Parse(cobDatabits.Text?.Trim());
                    SelectedSMSSetting.Parity = cobParity.Text?.Trim();
                    SelectedSMSSetting.StopBits = cobStopbits.Text?.Trim();
                    SelectedSMSSetting.Timeout = int.Parse(txbTimeoutSMS.Text?.Trim());
                    ReloadSMSSettingComboBoxSource();
                    gridViewEmail.Refresh();
                    if (oldName != SelectedSMSSetting.Name)
                        UpdateSMSSettingName(oldName, SelectedSMSSetting.Name);
                    IsChanged = true;
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteSMS_Click(object sender, EventArgs e)
        {
            if (SelectedSMSSetting != null)
            {
                if (!SelectedSMSSetting.ReadOnly)
                {
                    if (ShowDeleteComfirm())
                    {
                        int deleteIndex = smsSettings.IndexOf(SelectedSMSSetting);
                        string oldName = SelectedSMSSetting.Name;
                        smsSettings.Remove(SelectedSMSSetting);
                        ReloadSMSSettingComboBoxSource();
                        UpdateSMSSettingName(oldName, null);
                        if (smsSettings.Count > 0)
                        {
                            if (deleteIndex >= smsSettings.Count)
                                SetSelectedItem(gridViewSMS, smsSettings[smsSettings.Count - 1]);
                            else
                                SetSelectedItem(gridViewSMS, smsSettings[deleteIndex]);
                        }
                        IsChanged = true;
                    }
                }
            }
        }

        private void BtnAddSMS_Click(object sender, EventArgs e)
        {
            SMSSetting smsSetting = new SMSSetting();
            smsSetting.Enabled = true;
            smsSetting.ReadOnly = false;
            smsSetting.Name = smsSettings.GetUniqueNameInCollection(GetObjectName, "Sms_setting_1");
            smsSettings.Add(smsSetting);
            SetSelectedItem(gridViewSMS, smsSetting);
            ReloadSMSSettingComboBoxSource();
            IsChanged = true;
        }
        #endregion

        #region Discrete alarm event handlers
        private void BtnClearDiscrete_Click(object sender, EventArgs e)
        {
            if (discreteAlarms != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all discrete alarm ?"))
                {
                    discreteAlarms.ToList().ForEach(x => discreteAlarms.Remove(x));
                    IsChanged = true;
                }
            }
        }

        private void BtnSaveDiscrete_Click(object sender, EventArgs e)
        {
            if (SelectedDiscreteAlarm != null)
            {
                string validateRes = ValidateItem(SelectedDiscreteAlarm);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedDiscreteAlarm.Enabled = ckbEnabledDiscrete.Checked;
                    SelectedDiscreteAlarm.Name = txbNameDiscrete.Text?.Trim();
                    SelectedDiscreteAlarm.AlarmClassName = cobClassDiscrete.Text?.Trim();
                    SelectedDiscreteAlarm.AlarmGroupName = cobGroupDiscrete.Text?.Trim();
                    SelectedDiscreteAlarm.TriggerTagPath = txbTriggerTagDiscrete.Text?.Trim();
                    SelectedDiscreteAlarm.TriggerValue = txbTriggerValueDiscrete.Text?.Trim();
                    SelectedDiscreteAlarm.Description = txbDescriptionDiscrete.Text?.Trim();
                    SelectedDiscreteAlarm.AlarmText = txbAlarmTextDiscrete.Text?.Trim();
                    gridViewDiscrete.Refresh();
                    IsChanged = true;
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteDiscrete_Click(object sender, EventArgs e)
        {
            if (SelectedDiscreteAlarm != null)
            {
                if (ShowDeleteComfirm())
                {
                    int deleteIndex = discreteAlarms.IndexOf(SelectedDiscreteAlarm);
                    discreteAlarms.Remove(SelectedDiscreteAlarm);
                    if (discreteAlarms.Count > 0)
                    {
                        if (deleteIndex >= discreteAlarms.Count)
                            SetSelectedItem(gridViewDiscrete, discreteAlarms[discreteAlarms.Count - 1]);
                        else
                            SetSelectedItem(gridViewDiscrete, discreteAlarms[deleteIndex]);
                    }
                    IsChanged = true;
                }
            }
        }

        private void BtnAddDiscrete_Click(object sender, EventArgs e)
        {
            DiscreteAlarm discreteAlarm = new DiscreteAlarm();
            discreteAlarm.Enabled = true;
            discreteAlarm.Name = discreteAlarms.GetUniqueNameInCollection(GetObjectName, "Alarm_1");
            discreteAlarms.Add(discreteAlarm);
            SetSelectedItem(gridViewDiscrete, discreteAlarm);
            IsChanged = true;
        }

        private void BtnBrowseTriggerValueDiscrete_Click(object sender, EventArgs e)
        {
            if (serviceProvider.ShowSelectTag(out string selectedTag))
            {
                txbTriggerValueDiscrete.Text = selectedTag;
            }
        }

        private void BtnBrowseTriggerTagDiscrete_Click(object sender, EventArgs e)
        {
            if (serviceProvider.ShowSelectTag(out string selectedTag))
            {
                txbTriggerTagDiscrete.Text = selectedTag;
            }
        }

        private void BtnClearAlarmGroupDiscrete_Click(object sender, EventArgs e)
        {
            cobGroupDiscrete.Text = null;
        }

        private void BtnClearAlarmClassDiscrete_Click(object sender, EventArgs e)
        {
            cobClassDiscrete.Text = null;
        }

        private void BtnExportDiscreteAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Title = "Export";
                saveFileDialog1.Filter = "CSV file (*.csv)|*.csv";
                saveFileDialog1.FileName = "discrete_alarm_setting.csv";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string csv = discreteAlarms.ToCsvString();
                    File.WriteAllText(saveFileDialog1.FileName, csv);
                    MessageBox.Show("Export discrete alarms successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when export. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImportDiscreteAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "Import";
                openFileDialog1.Filter = "CSV file (*.csv)|*.csv";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var items = CsvHelper.ToList<DiscreteAlarm>(openFileDialog1.Filter);
                    int count = 0;
                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.Name))
                            {
                                if (!discreteAlarms.Any(x => x.Name == item.Name))
                                {
                                    discreteAlarms.Add(item);
                                    count++;
                                }
                            }
                        }
                    }
                    MessageBox.Show($"Import successfully. {count} items was added.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when import. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Analog alarm event handlers
        private void BtnClearAnalog_Click(object sender, EventArgs e)
        {
            if (analogAlarms != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all analog alarm ?"))
                {
                    analogAlarms.ToList().ForEach(x => analogAlarms.Remove(x));
                    IsChanged = true;
                }
            }
        }

        private void BtnSaveAnalog_Click(object sender, EventArgs e)
        {
            if (SelectedDiscreteAlarm != null)
            {
                string validateRes = ValidateItem(SelectedDiscreteAlarm);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedAnalogAlarm.Enabled = ckbEnabledAnalog.Checked;
                    SelectedAnalogAlarm.Name = txbNameAnalog.Text?.Trim();
                    SelectedAnalogAlarm.AlarmClassName = cobClassAnalog.Text?.Trim();
                    SelectedAnalogAlarm.AlarmGroupName = cobGroupAnalog.Text?.Trim();
                    SelectedAnalogAlarm.TriggerTagPath = txbTriggerTagAnalog.Text?.Trim();
                    SelectedAnalogAlarm.Limit = txbLimitAnalog.Text?.Trim();
                    SelectedAnalogAlarm.DeadbandValue = txbDeadbandValueAnalog.Text?.Trim();
                    SelectedAnalogAlarm.DeadbandInPercentage = ckbDeadbandInPercentAnalog.Checked;
                    SelectedAnalogAlarm.Description = txbDescriptionAnalog.Text?.Trim();
                    SelectedAnalogAlarm.DeadbandMode = (DeadbandMode)cobDeadbandModeAnalog.SelectedItem;
                    SelectedAnalogAlarm.LimitMode = (LimitMode)cobLimitModeAnalog.SelectedItem;
                    SelectedAnalogAlarm.AlarmText = txbAlarmTextAnalog.Text?.Trim();
                    gridViewAnalog.Refresh();
                    IsChanged = true;
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteAnalog_Click(object sender, EventArgs e)
        {
            if (SelectedAnalogAlarm != null)
            {
                if (ShowDeleteComfirm())
                {
                    int deleteIndex = analogAlarms.IndexOf(SelectedAnalogAlarm);
                    analogAlarms.Remove(SelectedAnalogAlarm);
                    if (analogAlarms.Count > 0)
                    {
                        if (deleteIndex >= analogAlarms.Count)
                            SetSelectedItem(gridViewAnalog, analogAlarms[analogAlarms.Count - 1]);
                        else
                            SetSelectedItem(gridViewAnalog, analogAlarms[deleteIndex]);
                    }
                    IsChanged = true;
                }
            }
        }

        private void BtnAddAnalog_Click(object sender, EventArgs e)
        {
            AnalogAlarm analogAlarm = new AnalogAlarm();
            analogAlarm.Enabled = true;
            analogAlarm.Name = analogAlarms.GetUniqueNameInCollection(GetObjectName, "Alarm_1");
            analogAlarms.Add(analogAlarm);
            SetSelectedItem(gridViewAnalog, analogAlarm);
            IsChanged = true;
        }

        private void BtnBrowseDeadbandAnalog_Click(object sender, EventArgs e)
        {
            if (serviceProvider.ShowSelectTag(out string selectedTag))
            {
                txbDeadbandValueAnalog.Text = selectedTag;
            }
        }

        private void BtnBrowseLimitAnalog_Click(object sender, EventArgs e)
        {
            if (serviceProvider.ShowSelectTag(out string selectedTag))
            {
                txbLimitAnalog.Text = selectedTag;
            }
        }

        private void BtnBrowseTriggerTagAnalog_Click(object sender, EventArgs e)
        {
            if (serviceProvider.ShowSelectTag(out string selectedTag))
            {
                txbTriggerTagAnalog.Text = selectedTag;
            }
        }

        private void BtnClearAlarmGroupAnalog_Click(object sender, EventArgs e)
        {
            cobGroupAnalog.Text = null;
        }

        private void BtnClearAlarmClassAnalog_Click(object sender, EventArgs e)
        {
            cobClassAnalog.Text = null;
        }

        private void BtnExportAnalogAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Title = "Export";
                saveFileDialog1.Filter = "CSV file (*.csv)|*.csv";
                saveFileDialog1.FileName = "analog_alarm_setting.csv";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string csv = analogAlarms.ToCsvString();
                    File.WriteAllText(saveFileDialog1.FileName, csv);
                    MessageBox.Show("Export analog alarms successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when export. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImportAnalogAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "Import";
                openFileDialog1.Filter = "CSV file (*.csv)|*.csv";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var items = CsvHelper.ToList<AnalogAlarm>(openFileDialog1.Filter);
                    int count = 0;
                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.Name))
                            {
                                if (!analogAlarms.Any(x => x.Name == item.Name))
                                {
                                    analogAlarms.Add(item);
                                    count++;
                                }
                            }
                        }
                    }
                    MessageBox.Show($"Import successfully. {count} items was added.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when import. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region Quality alarm event handlers
        private void BtnClearQuality_Click(object sender, EventArgs e)
        {
            if (qualityAlarms != null)
            {
                if (ShowDeleteComfirm("Do you want to clear all discrete alarm ?"))
                {
                    qualityAlarms.ToList().ForEach(x => qualityAlarms.Remove(x));
                    IsChanged = true;
                }
            }
        }

        private void BtnBrowseTriggerTagQuality_Click(object sender, EventArgs e)
        {
            if (serviceProvider.ShowSelectTag(out string selectedTag))
            {
                txbTriggerTagQuality.Text = selectedTag;
            }
        }

        private void BtnClearGroupQuality_Click(object sender, EventArgs e)
        {
            cobGroupQuality.Text = null;
        }

        private void BtnClearClassQuality_Click(object sender, EventArgs e)
        {
            cobClassQuality.Text = null;
        }

        private void BtnSaveQuality_Click(object sender, EventArgs e)
        {
            if (SelectedQualityAlarm != null)
            {
                string validateRes = ValidateItem(SelectedQualityAlarm);
                if (string.IsNullOrEmpty(validateRes))
                {
                    SelectedQualityAlarm.Enabled = ckbEnabledQuality.Checked;
                    SelectedQualityAlarm.Name = txbNameQuality.Text?.Trim();
                    SelectedQualityAlarm.AlarmClassName = cobClassQuality.Text?.Trim();
                    SelectedQualityAlarm.AlarmGroupName = cobGroupQuality.Text?.Trim();
                    SelectedQualityAlarm.TriggerTagPath = txbTriggerTagQuality.Text?.Trim();
                    Enum.TryParse(cobTriggerQuality.Text?.Trim(), out Quality triggerQuality);
                    SelectedQualityAlarm.TriggerQuality = triggerQuality;
                    SelectedQualityAlarm.Description = txbDescriptionQuality.Text?.Trim();
                    SelectedQualityAlarm.AlarmText = txbAlarmTextQuality.Text?.Trim();
                    gridViewQuality.Refresh();
                    IsChanged = true;
                }
                else
                {
                    ShowWarningMessage(validateRes);
                }
            }
        }

        private void BtnDeleteQuality_Click(object sender, EventArgs e)
        {
            if (SelectedQualityAlarm != null)
            {
                if (ShowDeleteComfirm())
                {
                    int deleteIndex = qualityAlarms.IndexOf(SelectedQualityAlarm);
                    qualityAlarms.Remove(SelectedQualityAlarm);
                    if (qualityAlarms.Count > 0)
                    {
                        if (deleteIndex >= qualityAlarms.Count)
                            SetSelectedItem(gridViewQuality, qualityAlarms[qualityAlarms.Count - 1]);
                        else
                            SetSelectedItem(gridViewQuality, qualityAlarms[deleteIndex]);
                    }
                    IsChanged = true;
                }
            }
        }

        private void BtnAddQuality_Click(object sender, EventArgs e)
        {
            QualityAlarm qualityAlarm = new QualityAlarm();
            qualityAlarm.Enabled = true;
            qualityAlarm.Name = qualityAlarms.GetUniqueNameInCollection(GetObjectName, "Alarm_1");
            qualityAlarm.TriggerQuality = Quality.Bad;
            qualityAlarms.Add(qualityAlarm);
            SetSelectedItem(gridViewQuality, qualityAlarm);
            IsChanged = true;
        }

        private void BtnExportQualityAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Title = "Export";
                saveFileDialog1.Filter = "CSV file (*.csv)|*.csv";
                saveFileDialog1.FileName = "quality_alarm_setting.csv";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string csv = qualityAlarms.ToCsvString();
                    File.WriteAllText(saveFileDialog1.FileName, csv);
                    MessageBox.Show("Export quality alarms successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when export. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImportQualityAlarm_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "Import";
                openFileDialog1.Filter = "CSV file (*.csv)|*.csv";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var items = CsvHelper.ToList<QualityAlarm>(openFileDialog1.Filter);
                    int count = 0;
                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            if (item != null && !string.IsNullOrEmpty(item.Name))
                            {
                                if (!qualityAlarms.Any(x => x.Name == item.Name))
                                {
                                    qualityAlarms.Add(item);
                                    count++;
                                }
                            }
                        }
                    }
                    MessageBox.Show($"Import successfully. {count} items was added.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when import. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region GridView event handlers
        private void GridViewSMS_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ReloadSMSSettingComboBoxSource();
        }

        private void GridViewEmail_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ReloadEmailSettingComboBoxSource();
        }

        private void GridViewClass_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var row = gridViewClass.Rows[e.RowIndex];
            UpdateBackColorAlarmClassRow(row);
            ReloadAlarmClassComboBoxSource();
        }

        private void GridViewGroup_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ReloadAlarmGroupComboBoxSource();
        }

        private void GridViewSMS_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedSMSSetting);
            UpdateButtonState(SelectedSMSSetting);
            UpdateEditControlState(SelectedSMSSetting);
        }

        private void GridViewEmail_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedEmailSetting);
            UpdateButtonState(SelectedEmailSetting);
            UpdateEditControlState(SelectedEmailSetting);
        }

        private void GridViewDiscrete_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedDiscreteAlarm);
            UpdateButtonState(SelectedDiscreteAlarm);
        }

        private void GridViewQuality_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedQualityAlarm);
            UpdateButtonState(SelectedQualityAlarm);
        }

        private void GridViewAnalog_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedAnalogAlarm);
            UpdateButtonState(SelectedAnalogAlarm);
        }

        private void GridViewGroup_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedAlarmGroup);
            UpdateButtonState(SelectedAlarmGroup);
            UpdateEditControlState(SelectedAlarmGroup);
        }

        private void GridViewClass_SelectionChanged(object sender, EventArgs e)
        {
            DisplayItem(SelectedAlarmClass);
            UpdateButtonState(SelectedAlarmClass);
            UpdateEditControlState(SelectedAlarmClass);
        }
        #endregion

        #endregion

        #region Methods
        private void LoadAlarmSetting(string alarmSettingPath)
        {
            string jsonRes = File.ReadAllText(alarmSettingPath);
            AlarmSetting alarmSetting = JsonConvert.DeserializeObject<AlarmSetting>(jsonRes);
            if (alarmSetting != null)
            {
                alarmSetting.Enabled = alarmSetting.Enabled;

                emailSettings.ToList().ForEach(x => emailSettings.Remove(x));
                if (alarmSetting.EmailSettings != null && alarmSetting.EmailSettings.Count > 0)
                {
                    foreach (var item in alarmSetting.EmailSettings)
                    {
                        if (item != null && !emailSettings.Any(x => x.Name == item.Name))
                            emailSettings.Add(item);
                    }
                }

                smsSettings.ToList().ForEach(x => smsSettings.Remove(x));
                if (alarmSetting.SMSSettings != null && alarmSetting.SMSSettings.Count > 0)
                {
                    foreach (var item in alarmSetting.SMSSettings)
                    {
                        if (item != null && !smsSettings.Any(x => x.Name == item.Name))
                            smsSettings.Add(item);
                    }
                }

                alarmClasses.ToList().ForEach(x => alarmClasses.Remove(x));
                if (alarmSetting.AlarmClasses != null && alarmSetting.AlarmClasses.Count > 0)
                {
                    foreach (var item in alarmSetting.AlarmClasses)
                    {
                        if (item != null && !alarmClasses.Any(x => x.Name == item.Name))
                            alarmClasses.Add(item);
                    }
                }

                alarmGroups.ToList().ForEach(x => alarmGroups.Remove(x));
                if (alarmSetting.AlarmGroups != null && alarmSetting.AlarmGroups.Count > 0)
                {
                    foreach (var item in alarmSetting.AlarmGroups)
                    {
                        if (item != null && !alarmGroups.Any(x => x.Name == item.Name))
                            alarmGroups.Add(item);
                    }
                }

                discreteAlarms.ToList().ForEach(x => discreteAlarms.Remove(x));
                if (alarmSetting.DiscreteAlarms != null && alarmSetting.DiscreteAlarms.Count > 0)
                {
                    foreach (var item in alarmSetting.DiscreteAlarms)
                    {
                        if (item != null && !discreteAlarms.Any(x => x.Name == item.Name))
                            discreteAlarms.Add(item);
                    }
                }

                analogAlarms.ToList().ForEach(x => analogAlarms.Remove(x));
                if (alarmSetting.AnalogAlarms != null && alarmSetting.AnalogAlarms.Count > 0)
                {
                    foreach (var item in alarmSetting.AnalogAlarms)
                    {
                        if (item != null && !analogAlarms.Any(x => x.Name == item.Name))
                            analogAlarms.Add(item);
                    }
                }

                qualityAlarms.ToList().ForEach(x => qualityAlarms.Remove(x));
                if (alarmSetting.QualityAlarms != null && alarmSetting.QualityAlarms.Count > 0)
                {
                    foreach (var item in alarmSetting.QualityAlarms)
                    {
                        if (item != null && !qualityAlarms.Any(x => x.Name == item.Name))
                            qualityAlarms.Add(item);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Can't load alarm setting at '{alarmSettingPath}'", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Save()
        {
            try
            {
                string applicationPath = DesignerHelper.GetApplicationOutputPath(serviceProvider);
                string alarmSettingPath = applicationPath + "\\AlarmSetting.json";
                string alarmSettingJson = JsonConvert.SerializeObject(alarmSetting, Formatting.Indented);
                File.WriteAllText(alarmSettingPath, alarmSettingJson);
                MessageBox.Show("Save successfully!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                IsChanged = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error occur when save alarm setting. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
