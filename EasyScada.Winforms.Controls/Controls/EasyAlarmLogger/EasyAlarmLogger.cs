using EasyScada.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyAlarmLoggerDesginer))]
    public partial class EasyAlarmLogger : Component, ISupportInitialize
    {
        #region Constructors
        public EasyAlarmLogger()
        {
            InitializeComponent();
        }

        public EasyAlarmLogger(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
        #endregion

        #region Fields
        private bool enabled = true;
        private LogProfileCollection profiles = new LogProfileCollection();
        private LogColumnCollection columns = new LogColumnCollection();
        private ConcurrentQueue<SentEmailMessage> sendEmailQueue = new ConcurrentQueue<SentEmailMessage>();
        private ConcurrentQueue<SentSMSMessage> sendSMSQueue = new ConcurrentQueue<SentSMSMessage>();
        private AlarmSetting alarmSetting;
        private Task refreshTask;
        private System.Timers.Timer sentEmailTimer;
        private System.Timers.Timer sentSMSTimer;
        private bool isDisposed;
        #endregion

        #region Properties
        [Browsable(false)]
        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogProfileCollection Databases { get => profiles; }

        [Category("Easy Scada")]
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [Browsable(false)]
        public AlarmSetting AlarmSetting { get => alarmSetting; }
        #endregion

        #region Events
        public event EventHandler<AlarmStateChangedEventArgs> AlarmStateChanged;
        #endregion

        #region Public methods
        public DataTable GetTopAlarmData(int limit = 100, string filterState = "")
        {
            DataTable dt = new DataTable();
            try
            {
                if (profiles.Count > 0 && limit > 0)
                {
                    profiles[0].GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adapter);
                    conn.Open();
                    if (profiles[0].DatabaseType == Core.DbType.MySql)
                    {
                        if (string.IsNullOrEmpty(filterState))
                            cmd.CommandText = $"select * from {profiles[0].TableName} order by IncommingTime desc limit {limit}";
                        else
                            cmd.CommandText = $"select * from {profiles[0].TableName} where State = '{filterState}' order by IncommingTime desc limit {limit}";
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();
                    }
                    else if (profiles[0].DatabaseType == Core.DbType.MSSQL)
                    {
                        if (string.IsNullOrEmpty(filterState))
                            cmd.CommandText = $"select top {limit} * from {profiles[0].TableName} order by IncommingTime desc";
                        else
                            cmd.CommandText = $"select top {limit} * from {profiles[0].TableName} where State = '{filterState}' order by IncommingTime desc";
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();
                    }
                }
            }
            catch { }
            return dt;
        }

        public DataTable GetAlarmData(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                if (profiles.Count > 0 && !string.IsNullOrEmpty(query))
                {
                    profiles[0].GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adapter);
                    conn.Open();
                    cmd.CommandText = query;
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dt);

                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
            catch { }
            return dt;
        }

        public List<AlarmItem> GetTopAlarmItems(int limit, string filterState = "")
        {
            try
            {
                return GetTopAlarmData(limit, filterState).ToList<AlarmItem>();
            }
            catch { }
            return new List<AlarmItem>();
        }

        public List<AlarmItem> GetAlarmItems(string query)
        {
            try
            {
                return GetAlarmData(query).ToList<AlarmItem>();
            }
            catch { }
            return new List<AlarmItem>();
        }

        public int AckAlarmItem(string alarmName, string alarmType, DateTime incommingTime)
        {
            int result = 0;
            try
            {
                string ackTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (LogProfile profile in profiles)
                {
                    try
                    {
                        if (profile.Enabled)
                        {
                            profile.GetCommand(out DbConnection conn, out DbCommand cmd);
                            conn.Open();
                            cmd.CommandText = $"update {profile.TableName} set State = 'Ack', AckTime = '{ackTime}' where Name = '{alarmName}' " +
                                $"and AlarmType = '{alarmType}' and State = 'Out' " +
                                $"and IncommingTime = '{incommingTime.ToString("yyyy-MM-dd HH:mm:ss")}'";
                            int res = cmd.ExecuteNonQuery();
                            if (result < res)
                                result = res;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return result;
        }

        public int AckAlarmItem(AlarmItem alarmItem)
        {
            int result = 0;
            try
            {
                if (alarmItem != null)
                {
                    result = AckAlarmItem(alarmItem.Name, alarmItem.AlarmType, alarmItem.IncommingTime);
                }
            }
            catch { }
            return result;
        }

        public int AckAll()
        {
            int result = 0;
            try
            {
                string ackTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (LogProfile profile in profiles)
                {
                    try
                    {
                        if (profile.Enabled)
                        {
                            profile.GetCommand(out DbConnection conn, out DbCommand cmd);
                            conn.Open();
                            cmd.CommandText = $"update {profile.TableName} set State = 'Ack', AckTime = '{ackTime}' where State = 'Out'";
                            int res = cmd.ExecuteNonQuery();
                            if (result < res)
                                result = res;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return result;
        }
        #endregion

        #region ISupportInitialize
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                if (refreshTask == null)
                {
                    columns = new LogColumnCollection();
                    columns.Add(new LogColumn() { ColumnName = "Name" });
                    columns.Add(new LogColumn() { ColumnName = "AlarmText" });
                    columns.Add(new LogColumn() { ColumnName = "AlarmClass" });
                    columns.Add(new LogColumn() { ColumnName = "AlarmGroup" });
                    columns.Add(new LogColumn() { ColumnName = "TriggerTag" });
                    columns.Add(new LogColumn() { ColumnName = "Value" });
                    columns.Add(new LogColumn() { ColumnName = "Limit" });
                    columns.Add(new LogColumn() { ColumnName = "CompareMode" });
                    columns.Add(new LogColumn() { ColumnName = "State" });
                    columns.Add(new LogColumn() { ColumnName = "OutgoingTime" });
                    columns.Add(new LogColumn() { ColumnName = "AckTime" });
                    columns.Add(new LogColumn() { ColumnName = "AlarmType" });

                    Disposed += OnDisposed;
                    alarmSetting = DesignerHelper.GetAlarmSetting(null);
                    if (alarmSetting != null)
                    {
                        alarmSetting.Enabled = enabled;
                        alarmSetting.AlarmStateChagned += OnAlarmStateChanged;
                        refreshTask = Task.Factory.StartNew(RefreshAlarm, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                        sentEmailTimer = new System.Timers.Timer();
                        sentEmailTimer.Interval = 1000;
                        sentEmailTimer.Elapsed += SentEmailTimer_Elapsed;
                        sentEmailTimer.Start();
                        sentSMSTimer = new System.Timers.Timer();
                        sentSMSTimer.Interval = 1000;
                        sentSMSTimer.Elapsed += SentSMSTimer_Elapsed;
                        sentSMSTimer.Start();
                    }
                }
            }
        }
        #endregion

        #region Methods
        private void SentSMSTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private void SentEmailTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sentEmailTimer.Stop();
            if (sendEmailQueue.TryDequeue(out SentEmailMessage message))
            {
                SendEmail(message);
            }
            sentEmailTimer.Start();
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            isDisposed = true;
        }

        private void OnAlarmStateChanged(object sender, AlarmStateChangedEventArgs e)
        {
            try
            {
                if (e.AlarmItem != null)
                {
                    string alarmTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string alarmType = "";
                    if (e.AlarmItem is DiscreteAlarm)
                    {
                        alarmType = "DiscreteAlarm";
                    }
                    else if (e.AlarmItem is AnalogAlarm)
                    {
                        alarmType = "AnalogAlarm";
                    }
                    else if (e.AlarmItem is QualityAlarm)
                    {
                        alarmType = "QualityAlarm";
                    }

                    bool needSentEmailOrSMS = false;

                    foreach (LogProfile profile in profiles)
                    {
                        if (profile.Enabled)
                        {
                            try
                            {
                                profile.GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adapter, true);
                                conn.Open();

                                if (e.NewState == AlarmState.Incoming)
                                {
                                    if (profile.DatabaseType == Core.DbType.MSSQL)
                                        cmd.CommandText = $"select top 1 * from {profile.TableName} where State = 'In' and Name = '{e.AlarmItem.Name}' and AlarmType = '{alarmType}'";
                                    else if (profile.DatabaseType == Core.DbType.MySql)
                                        cmd.CommandText = $"select * from {profile.TableName} where State = 'In' and Name = '{e.AlarmItem.Name}' and AlarmType = '{alarmType}' limit 1";
                                    adapter.SelectCommand = cmd;
                                    DataTable dt = new DataTable();
                                    adapter.Fill(dt);
                                    if (dt.Rows.Count == 0)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.Append($"insert into {profile.TableName} ");
                                        sb.Append("(`IncommingTime`, `Name`, `AlarmText`, `AlarmClass`, `AlarmGroup`, `TriggerTag`, `Value`, `Limit`, `CompareMode`, `State`, `AlarmType`) values (");
                                        sb.Append($"'{alarmTime}', '{e.AlarmItem.Name}', '{e.AlarmItem.AlarmText}', '{e.AlarmItem.AlarmClassName}', '{e.AlarmItem.AlarmGroupName}', '{e.AlarmItem.TriggerTagPath}', ");
                                        sb.Append($"'{e.Value}', '{e.Limit}', '{e.CompareMode}', 'In', '{alarmType}')");

                                        cmd.CommandText = sb.ToString();
                                        cmd.ExecuteNonQuery();

                                        needSentEmailOrSMS = true;
                                    }
                                }
                                else if (e.NewState == AlarmState.Outgoing)
                                {
                                    if (profile.DatabaseType == Core.DbType.MSSQL)
                                        cmd.CommandText = $"select top 1 * from {profile.TableName} where State = 'In' and Name = '{e.AlarmItem.Name}' and AlarmType = '{alarmType}'";
                                    else if (profile.DatabaseType == Core.DbType.MySql)
                                        cmd.CommandText = $"select * from {profile.TableName} where State = 'In' and Name = '{e.AlarmItem.Name}' and AlarmType = '{alarmType}' limit 1";
                                    adapter.SelectCommand = cmd;
                                    DataTable dt = new DataTable();
                                    adapter.Fill(dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        sb.Append($"update {profile.TableName} ");
                                        sb.Append($"set OutgoingTime = '{alarmTime}', State = 'Out' where ");
                                        sb.Append($"Name = '{e.AlarmItem.Name}' and AlarmType = '{alarmType}' and State = 'In'");
                                        cmd.CommandText = sb.ToString();
                                        int result = cmd.ExecuteNonQuery();
                                        needSentEmailOrSMS = true;
                                    }
                                }

                                conn.Close();
                                conn.Dispose();
                                cmd.Dispose();
                                adapter.Dispose();
                            }
                            catch { }
                        }
                    }

                    if (needSentEmailOrSMS)
                    {
                        if (e.AlarmItem.EmailSetting != null)
                        {
                            if (e.AlarmItem.EmailSetting.Enabled)
                                sendEmailQueue.Enqueue(new SentEmailMessage(e));
                        }

                        if (e.AlarmItem.SMSSetting != null)
                        {
                            if (e.AlarmItem.SMSSetting.Enabled)
                                sendSMSQueue.Enqueue(new SentSMSMessage(e));
                        }
                    }
                }
            }
            catch { }
            AlarmStateChanged?.Invoke(sender, e);
        }

        private void RefreshAlarm()
        {
            Dictionary<LogProfile, int> createSchemaResult = new Dictionary<LogProfile, int>();
            Dictionary<LogProfile, int> createTableResult = new Dictionary<LogProfile, int>();

            LogColumn[] columns = this.columns.ToArray();

            while (!isDisposed)
            {
                try
                {
                    foreach (LogProfile profile in profiles)
                    {
                        if (profile.Enabled)
                        {
                            try
                            {
                                bool needCreateSchema = false;
                                if (!createSchemaResult.ContainsKey(profile))
                                {
                                    needCreateSchema = true;
                                }
                                else
                                {
                                    if (createSchemaResult[profile] != 1)
                                        needCreateSchema = true;
                                }

                                if (needCreateSchema)
                                {
                                    // Create database schema if not exists
                                    profile.GetCommand(out DbConnection conn, out DbCommand cmd, false);
                                    conn.Open();
                                    cmd.CommandText = profile.GetCreateSchemaQuery();
                                    createSchemaResult[profile] = cmd.ExecuteNonQuery();
                                    conn.Close();
                                    conn.Dispose();
                                    cmd.Dispose();
                                }

                                bool needCreateTable = false;
                                if (!createTableResult.ContainsKey(profile))
                                {
                                    needCreateTable = true;
                                }
                                else
                                {
                                    if (createTableResult[profile] != 1)
                                        needCreateTable = true;
                                }

                                if (needCreateTable)
                                {
                                    // Create table if not exists
                                    profile.GetCommand(out DbConnection conn, out DbCommand cmd, out DbDataAdapter adp, true);
                                    conn.Open();
                                    cmd.CommandText = profile.GetCreateTableQuery(columns, "IncommingTime");
                                    int createTableRes = cmd.ExecuteNonQuery();
                                    createTableResult[profile] = createTableRes;
                                    // Create table result = 0. It means table already exists
                                    if (createTableRes == 0)
                                    {
                                        // We need to check the columns name
                                        cmd.CommandText = profile.GetSelectQuery(1);
                                        adp.SelectCommand = cmd;
                                        DataTable dt = new DataTable();
                                        adp.Fill(dt);
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    if (Enabled)
                    {
                        foreach (var item in alarmSetting.DiscreteAlarms)
                        {
                            if (item.Enabled)
                                item.Refresh();
                        }

                        foreach (var item in alarmSetting.AnalogAlarms)
                        {
                            if (item.Enabled)
                                item.Refresh();
                        }

                        foreach (var item in alarmSetting.QualityAlarms)
                        {
                            if (item.Enabled)
                                item.Refresh();
                        }
                    }
                }
                catch { }
                finally
                {
                    Thread.Sleep(50);
                }
            }
        }

        private async void SendEmail(SentEmailMessage message)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (message.TryCount < 10)
                    {
                        message.TryCount++;
                        MailMessage mailMessage = new MailMessage();
                        SmtpClient smtpClient = new SmtpClient();
                        EmailSetting emailSetting = message.EventArgs.AlarmItem.EmailSetting;
                        string credentialEmail = emailSetting.CredentialEmail;
                        string credentialPassword = emailSetting.CredentialPassword;
                        if (string.IsNullOrEmpty(credentialEmail) ||
                            string.IsNullOrEmpty(credentialPassword))
                        {
                            credentialEmail = "easyscada@gmail.com";
                            credentialPassword = "easyscada9999";
                        }

                        mailMessage.From = new MailAddress(credentialEmail);

                        foreach (var toEmail in emailSetting.GetEmails())
                        {
                            if (!string.IsNullOrEmpty(toEmail))
                            {
                                mailMessage.To.Add(toEmail);
                            }
                        }

                        foreach (var ccEmail in emailSetting.GetCC())
                        {
                            if (!string.IsNullOrEmpty(ccEmail))
                            {
                                mailMessage.CC.Add(ccEmail);
                            }
                        }

                        mailMessage.Subject = $"Alarm - {message.EventArgs.AlarmItem.Name}";

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Status: {message.EventArgs.NewState.ToString()}");
                        sb.AppendLine($"Class: {message.EventArgs.AlarmItem.AlarmClassName}");
                        sb.AppendLine($"Group: {message.EventArgs.AlarmItem.AlarmGroupName}");
                        sb.AppendLine($"{message.EventArgs.NewState.ToString()} time: {message.EventArgs.OccurTime.ToString("yyyy/MM/dd HH:mm:ss")}");
                        sb.AppendLine($"Value: {message.EventArgs.Value}");
                        sb.AppendLine($"Limit: {message.EventArgs.Limit}");
                        sb.AppendLine($"Compare mode: {message.EventArgs.CompareMode}");
                        sb.AppendLine($"Trigger tag: {message.EventArgs.AlarmItem.TriggerTagPath}");
                        sb.AppendLine($"Alarm type: {message.EventArgs.AlarmItem.GetType().Name}");
                        sb.AppendLine($"Description: {message.EventArgs.AlarmItem.Description}");

                        mailMessage.Body = sb.ToString();

                        smtpClient.Host = emailSetting.Host;
                        smtpClient.Port = emailSetting.Port;
                        smtpClient.EnableSsl = emailSetting.EnableSSL;
                        smtpClient.Timeout = emailSetting.Timeout;
                        smtpClient.Credentials = new NetworkCredential(credentialEmail, credentialPassword);
                        smtpClient.Send(mailMessage);
                        smtpClient.Dispose();
                        mailMessage.Dispose();
                    }
                }
                catch
                {
                    sendEmailQueue.Enqueue(message);
                }
            });
        }

        #endregion
    }
}
