using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class LoggedEventArgs : EventArgs
    {
        public LogProfile Profile { get; private set; }
        public LogColumn[] Columns { get; private set; }
        public int LogResult { get; private set; }
        public string LogMessage { get; private set; }

        public LoggedEventArgs(string message, int result, LogProfile profile, LogColumn[] columns)
        {
            Profile = profile;
            Columns = columns;
            LogMessage = message;
            LogResult = result;
        }
    }
}
