using System;
using System.Collections.Generic;

namespace EasyScada.Wpf.Connector
{
    public class DataLogger : ILogger
    {
        #region Constructors
        public DataLogger()
        {

        }
        #endregion

        #region Public properties
        public bool Enabled { get; set; } = true;
        public string TableName { get; set; }
        public DataBaseType DataBaseType { get; set; } = DataBaseType.MySql;
        public string DataBaseName { get; set; } = "easy_scada";
        public string IpAddress { get; set; } = "localhost";
        public ushort Port { get; set; } = 3306;
        public string Password { get; set; } = "100100";
        public string Username { get; set; } = "root";
        public string ConnectionString { get => $"Server={IpAddress};Port={Port};Database={DataBaseName};Uid={Username};Pwd={Password};"; }
        public uint LogInterval { get; set; } = 60000;
        public List<Column> Columns { get; set; }
        #endregion

        #region Events
        public event EventHandler<LoggedEventArgs> Logged;
        public event EventHandler<LoggingEventArgs> Logging;
        public event EventHandler<LogErrorEventArgs> LogError;

        #endregion

        #region Fields

        #endregion

        #region Methods
        public override string ToString()
        {
            return TableName;
        }
        public int Log(string message)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
