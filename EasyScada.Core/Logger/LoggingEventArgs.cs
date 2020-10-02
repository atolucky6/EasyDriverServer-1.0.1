using System;

namespace EasyScada.Core
{
    public class LoggingEventArgs : EventArgs
    {
        public LogProfile Profile { get; private set; }
        public LogColumn[] Columns { get; private set; }
        public string LogMessage { get; set; }
        public bool Cancel { get; set; }

        public LoggingEventArgs(string message, LogProfile profile, LogColumn[] columns)
        {
            LogMessage = message;
            Profile = profile;
            Columns = columns;
        }
    }
}
