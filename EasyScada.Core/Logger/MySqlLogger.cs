using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace EasyScada.Core.Logger
{
    public class MySqlLogger : IDatabaseLogger
    {
        public MySqlLogger()
        {
            DbType = DbType.MySql;
            DbPort = 3306;
            DbIpAddress = "localhost";
            DbUsername = "root";
            DbPassword = "100100";
        }

        public bool Enabled { get; set; }
        public DbType DbType { get; protected set; }
        public string ConnectionString => $"Server={DbIpAddress};Port={DbPort};Database={DbName};Uid={DbUsername};Pwd={DbPassword};";
        public string Table { get; set; }
        public string DbIpAddress { get; set; }
        public ushort DbPort { get; set; }
        public string DbPassword { get; set; }
        public string DbName { get; set; }
        public string DbUsername { get; set; }

        public event EventHandler<LoggedEventArgs> Logged;
        public event EventHandler<LoggingEventArgs> Logging;
        public event EventHandler<LogErrorEventArgs> LogError;

        public int Log(string message)
        {
            try
            {
                if (Enabled)
                {
                    LoggingEventArgs loggingEventArgs = new LoggingEventArgs(message);
                    Logging?.Invoke(this, loggingEventArgs);
                    if (loggingEventArgs.Cancel)
                        return 0;
                    using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = message;
                            int res = cmd.ExecuteNonQuery();
                            Logged?.Invoke(this, new LoggedEventArgs(message, res));
                            return res;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                LogError?.Invoke(this, new LogErrorEventArgs(message, ex));
                return -2;
            }
        }
    }
}
