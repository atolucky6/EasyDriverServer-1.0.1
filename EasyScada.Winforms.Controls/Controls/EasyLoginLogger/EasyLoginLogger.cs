using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    [Designer(typeof(EasyLoginLoggerDesigner))]
    public partial class EasyLoginLogger : Component, ISupportInitialize
    {
        #region Constructors
        public EasyLoginLogger()
        {
            InitializeComponent();
        }

        public EasyLoginLogger(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region Public properties
        [Browsable(true), Category(DesignerCategory.EASYSCADA), DefaultValue(true)]
        public bool Enabled { get; set; }

        LogProfileCollection _database = new LogProfileCollection();
        [Browsable(false)]
        [Category("Easy Scada"), TypeConverter(typeof(CollectionEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogProfileCollection Databases { get => _database; }

        #endregion

        #region Members
        private LogColumnCollection _logColumns;
        #endregion

        #region ISupportInitialize
        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (!DesignMode)
            {
                if (_logColumns == null)
                {
                    _logColumns = new LogColumnCollection();
                    _logColumns.Add(new LogColumn() { ColumnName = "User", Mode = LogColumnMode.UseDefaultValueWhenTagBad });
                    _logColumns.Add(new LogColumn() { ColumnName = "Role", Mode = LogColumnMode.UseDefaultValueWhenTagBad });
                    _logColumns.Add(new LogColumn() { ColumnName = "Action", Mode = LogColumnMode.UseDefaultValueWhenTagBad });

                    AuthenticateHelper.Logged += OnLogged;
                    AuthenticateHelper.Logouted += OnLogouted;
                }
            }
        }
        #endregion

        #region Event handlers
        private void OnLogouted(object sender, LogoutEventArgs e)
        {
            foreach (LogProfile profile in _database)
            {
                if (profile != null)
                {
                    _logColumns[0].DefaultValue = e.Username;
                    _logColumns[1].DefaultValue = e.Role;
                    _logColumns[2].DefaultValue = "Logout";

                    string insertQuery = profile.GetInsertQuery(e.LogoutTime, _logColumns.ToArray());
                    try
                    {
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
                        cmd.CommandText = profile.GetCreateTableQuery(_logColumns.ToArray());
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
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
                }
            }
        }

        private void OnLogged(object sender, LoginEventArgs e)
        {
            foreach (LogProfile profile in _database)
            {
                if (profile != null)
                {
                    _logColumns[0].DefaultValue = e.Username;
                    _logColumns[1].DefaultValue = e.Role;
                    _logColumns[2].DefaultValue = "Login";

                    string insertQuery = profile.GetInsertQuery(e.LoginTime, _logColumns.ToArray());
                    try
                    {
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
                        cmd.CommandText = profile.GetCreateTableQuery(_logColumns.ToArray());
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
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
                }
            }
        }
        #endregion

        #region Methods
        public void CreateTable()
        {
            LogColumn[] columns = _logColumns.ToArray();
            foreach (LogProfile profile in _database)
            {
                if (profile != null)
                {
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
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
