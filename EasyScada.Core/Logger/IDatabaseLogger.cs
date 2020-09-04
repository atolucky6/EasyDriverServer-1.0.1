namespace EasyScada.Core.Logger
{
    public interface IDatabaseLogger : ILogger
    {
        DbType DbType { get; }
        string ConnectionString { get; }
        string Table { get; set; }
        string DbName { get; set; }
        string DbIpAddress { get; set; } 
        ushort DbPort { get; set; }
        string DbUsername { get; set; }
        string DbPassword { get; set; }
    }
}
