using System;

namespace EasyScada.Core.Logger
{
    public interface ILogger
    {
        bool Enabled { get; set; }
        int Log(string message);

        event EventHandler<LoggedEventArgs> Logged;
        event EventHandler<LoggingEventArgs> Logging;
        event EventHandler<LogErrorEventArgs> LogError;
    }
}
