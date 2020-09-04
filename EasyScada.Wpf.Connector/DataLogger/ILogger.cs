using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Wpf.Connector
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
