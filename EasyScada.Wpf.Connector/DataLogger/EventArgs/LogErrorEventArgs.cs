using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Wpf.Connector
{
    public class LogErrorEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public LogErrorEventArgs(string message, Exception ex)
        {
            Message = message;
            Exception = ex;
        }
    }
}
