using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace EasyScada.Core
{
    public class LogProfile : ICloneable
    {
        public bool Enabled { get; set; } = true;
        [TypeConverter(typeof(OdbcDriverNameSourceConverter))]
        public string DataSourceName { get; set; }
        public DbType DatabaseType { get; set; } = DbType.MySql;
        public string TableName { get; set; } = "table1";
        public string DatabaseName { get; set; } = "easyScada";
        public string IpAddress { get; set; } = "192.168.1.10";
        public ushort Port { get; set; } = 3306;
        public string Username { get; set; } = "root";
        public string Password { get; set; } = "100100";

        public virtual string GetConnectionString(bool includeDatabaseName = true)
        {
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    if (includeDatabaseName)
                        return $"Server = {IpAddress},{Port}; Database = {DatabaseName}; User Id = {Username}; Password = {Password}; Trusted_Connection=True;";
                    else
                        return $"Server = {IpAddress},{Port};  User Id = {Username}; Password = {Password}; Trusted_Connection=True;";
                case DbType.MySql:
                    if (includeDatabaseName)
                        return $"Server={IpAddress};Port={Port};Database={DatabaseName};Uid={Username};Pwd={Password};";
                    else
                        return $"Server={IpAddress};Port={Port};Uid={Username};Pwd={Password};";
                case DbType.ODBC:
                    if (includeDatabaseName)
                        return $"Driver={DataSourceName};Server={IpAddress},{Port};Database={DatabaseName};Uid={Username};Pwd={Password};";
                    else
                        return $"Driver={DataSourceName};Server={IpAddress},{Port};Uid={Username};Pwd={Password};";
                default:
                    break;
            }
            return null;
        }

        public virtual string GetCreateTableQuery(LogColumn[] columns, string timeColumnName = "DateTime")
        {
            if (columns == null)
                return null;
            if (columns.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder();
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    {
                        sb.AppendLine($"if not exists (select * from sys.objects where object_id = OBJECT_ID(N'[dbo].[{TableName}]') and type in (N'U'))");
                        sb.AppendLine("BEGIN");
                        sb.Append($"create table [dbo].[{TableName}] ({timeColumnName}  DATETIME");
                        foreach (LogColumn column in columns)
                        {
                            sb.Append($", {column.ColumnName?.Replace(" ", "")} NVARCHAR(200)");
                        }
                        sb.AppendLine(");");
                        sb.Append("END");
                        break;
                    }
                case DbType.MySql:
                    {
                        sb.Append($"create table if not exists {TableName} ({timeColumnName} Datetime not null");
                        foreach (LogColumn column in columns)
                        {
                            sb.Append($", `{column.ColumnName?.Replace(" ", "")}` varchar(200)");
                        }
                        sb.Append(");");
                        break;
                    }
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        public virtual string GetSelectQuery(string where = null)
        {
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    return $"select * from {TableName} {where}";
                case DbType.MySql:
                    return $"select * from {TableName} {where}";
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
            return null;
        }

        public virtual string GetSelectQuery(string[] columns, string where = null)
        {
            StringBuilder sb = new StringBuilder();
            switch (DatabaseType)
            {
                case DbType.MySql:
                case DbType.MSSQL:
                    sb.Append("select ");
                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (i == columns.Length - 1)
                            sb.Append($"{columns[i]} from {TableName} {where}");
                        else
                            sb.Append($"{columns[i]}, ");
                    }
                    return sb.ToString();
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
            return null;
        }

        public virtual string GetSelectQuery(int maxCount)
        {
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    return $"select * from {TableName} limit {maxCount}";
                case DbType.MySql:
                    return $"select * from {TableName} limit {maxCount}";
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
            return null;
        }

        public virtual string GetInsertQuery(DateTime time, LogColumn[] columns)
        {
            StringBuilder sb = new StringBuilder();
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    {
                        if (columns != null && columns.Length > 0)
                        {
                            sb.Append($"insert into {TableName} (DateTime, ");
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (i == columns.Length - 1)
                                    sb.Append($"{columns[i].ColumnName})");
                                else
                                    sb.Append($"{columns[i].ColumnName}, ");
                            }
                            sb.Append($" values ('{time.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (i == columns.Length - 1)
                                    sb.Append($"'{columns[i].Value}')");
                                else
                                    sb.Append($"'{columns[i].Value}', ");
                            }
                        }
                        break;
                    }
                case DbType.MySql:
                    {
                        if (columns != null && columns.Length > 0)
                        {
                            sb.Append($"insert into {TableName} (DateTime, ");
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (i == columns.Length - 1)
                                    sb.Append($"{columns[i].ColumnName})");
                                else
                                    sb.Append($"{columns[i].ColumnName}, ");
                            }
                            sb.Append($" values ('{time.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (i == columns.Length - 1)
                                    sb.Append($"'{columns[i].Value}')");
                                else
                                    sb.Append($"'{columns[i].Value}', ");
                            }
                        }
                        break;
                    }
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        public virtual string GetCreateSchemaQuery()
        {
            StringBuilder sb = new StringBuilder();
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    {
                        sb.AppendLine($"if not exists(select 1 from sys.databases where name = '{DatabaseName}')");
                        sb.Append($"create database {DatabaseName};");
                        break;
                    }
                case DbType.MySql:
                    {
                        sb.AppendLine($"create database if not exists {DatabaseName}");
                        break;
                    }
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        public virtual bool TestConnection()
        {
            try
            {
                GetCommand(out DbConnection conn, out DbCommand cmd, true);
                conn.Open();
                cmd.CommandText = "select 1";
                cmd.ExecuteNonQuery();
                return true;
            }
            catch { return false; }
        }

        public override string ToString()
        {
            return $"{DatabaseType.ToString()} - (Db:{DatabaseName}, Table:{TableName}, Address:{IpAddress}:{Port})";
        }

        public virtual void GetCommand(out DbConnection connection, out DbCommand command, bool includeDatabaseName = true)
        {
            connection = null;
            command = null;
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    connection = new SqlConnection(GetConnectionString(includeDatabaseName));
                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    break;
                case DbType.MySql:
                    connection = new MySqlConnection(GetConnectionString(includeDatabaseName));
                    command = new MySqlCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    break;
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
        }

        public virtual void GetCommand(out DbConnection connection, out DbCommand command, out DbDataAdapter adapter, bool includeDatabaseName = true)
        {
            connection = null;
            command = null;
            adapter = null;
            switch (DatabaseType)
            {
                case DbType.MSSQL:
                    connection = new SqlConnection(GetConnectionString(includeDatabaseName));
                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    adapter = new SqlDataAdapter();
                    break;
                case DbType.MySql:
                    connection = new MySqlConnection(GetConnectionString(includeDatabaseName));
                    command = new MySqlCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    adapter = new MySqlDataAdapter();
                    break;
                case DbType.ODBC:
                    break;
                default:
                    break;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
