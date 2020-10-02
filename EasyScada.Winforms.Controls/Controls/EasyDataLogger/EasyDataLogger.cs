using EasyScada.Core;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DbType = EasyScada.Core.DbType;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyDataLoggerDesigner))]
    public partial class EasyDataLogger : Component, ISupportInitialize
    {
        #region Constructors
        public EasyDataLogger()
        {
            InitializeComponent();
        }

        public EasyDataLogger(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
        #endregion

        #region Fields
        private bool enabled = false;
        private int interval = 60000;
        private LogProfileCollection profiles = new LogProfileCollection();
        private LogColumnCollection columns = new LogColumnCollection();

        private Stopwatch sw = new Stopwatch();
        private bool isStarted = false;
        private Task logTask;
        #endregion

        #region Properties

        [Browsable(true), Category("Easy Scada")]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (!DesignMode)
                    {
                        isStarted = true;
                    }
                }
            }
        }

        [Browsable(true), Category("Easy Scada")]
        public bool AllowLogWhenTagBad { get; set; }

        [Browsable(true), Category("Easy Scada")]
        public int Interval
        {
            get => interval;
            set
            {
                if (value != interval)
                {
                    if (value < 1000)
                        interval = 1000;
                    else
                        interval = value;
                }
            }
        }
        
        [Browsable(false)]
        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogColumnCollection Columns { get => columns; }

        [Browsable(false)]
        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogProfileCollection Databases { get => profiles; }

        #endregion

        #region Events

        public event EventHandler<LoggingEventArgs> Logging;
        public event EventHandler<LoggedEventArgs> Logged;
        public event EventHandler<LogErrorEventArgs> Failed;

        #endregion

        #region Methods

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            InitializeLogTask();
        }

        private void InitializeLogTask()
        {
            logTask = Task.Factory.StartNew(DoLog, TaskCreationOptions.LongRunning);
        }

        private void DoLog()
        {
            while (true)
            {
                sw.Start();
                try
                {
                    if (isStarted && EasyDriverConnectorProvider.GetEasyDriverConnector().IsStarted)
                    {
                        System.Action logAct = new System.Action(() =>
                        {
                            DateTime logTime = DateTime.Now;
                            LogColumn[] columns = this.columns.ToArray();
                            bool isTagBad = false;
                            foreach (LogColumn col in columns)
                            {
                                if (!(col.Quality == Quality.Good))
                                {
                                    isTagBad = true;
                                    break;
                                }
                            }

                            if (AllowLogWhenTagBad || (!AllowLogWhenTagBad && !isTagBad))
                            {
                                foreach (LogProfile profile in profiles)
                                {
                                    if (profile != null)
                                    {
                                        string insertQuery = profile.GetInsertQuery(logTime, columns);
                                        try
                                        {
                                            LoggingEventArgs args = new LoggingEventArgs(insertQuery, profile, columns);
                                            Logging?.Invoke(this, args);
                                            if (args.Cancel)
                                                continue;
                                            // Create database schema if not exists
                                            profile.GetCommand(out DbConnection conn, out DbCommand cmd, false);
                                            conn.Open();
                                            cmd.CommandText = profile.GetCreateSchemaQuery();
                                            cmd.ExecuteNonQuery();
                                            conn.Close();
                                            conn.Dispose();
                                            cmd.Dispose();
                                            // Create table if not exists
                                            profile.GetCommand(out conn, out cmd, out DbDataAdapter adp, true);
                                            conn.Open();
                                            cmd.CommandText = profile.GetCreateTableQuery(columns);
                                            int createTableRes = cmd.ExecuteNonQuery();

                                            // Create table result = 0. It means table already exists
                                            if (createTableRes == 0)
                                            {
                                                // We need to check the columns name
                                                cmd.CommandText = profile.GetSelectQuery(1);
                                                adp.SelectCommand = cmd;
                                                DataTable dt = new DataTable();
                                                adp.Fill(dt);
                                            }
                                            cmd.CommandText = insertQuery;
                                            int logRes = cmd.ExecuteNonQuery();
                                            Logged?.Invoke(this, new LoggedEventArgs(insertQuery, logRes, profile, columns));
                                        }
                                        catch (Exception ex)
                                        {
                                            Failed?.Invoke(this, new LogErrorEventArgs(insertQuery, ex, profile, columns));
                                        }
                                    }
                                }
                            }
                        });
                        Task.Factory.StartNew(logAct);
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    sw.Stop();
                    int nextInterval = Interval - (int)sw.ElapsedMilliseconds;
                    if (nextInterval < 1)
                        nextInterval = 1;
                    Thread.Sleep(nextInterval);
                }
            }
        }

        #endregion
    }
}
