using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class LogErrorEventArgs : EventArgs
    {
        public LogProfile Profile { get; private set; }
        public LogColumn[] Columns { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public LogErrorEventArgs(string message, Exception ex, LogProfile profile, LogColumn[] columns)
        {
            Profile = profile;
            Columns = columns;
            Message = message;
            Exception = ex;
        }
    }
}
