using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Wpf.Connector
{
    public class LoggedEventArgs : EventArgs
    {
        public int LogResult { get; private set; }
        public string LogMessage { get; private set; }

        public LoggedEventArgs(string message, int result)
        {
            LogMessage = message;
            LogResult = result;
        }
    }
}
