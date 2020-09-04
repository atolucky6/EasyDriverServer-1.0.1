using System;

namespace EasyScada.Wpf.Connector
{
    public class LoggingEventArgs : EventArgs
    {
        public string LogMessage { get; set; }
        public bool Cancel { get; set; }

        public LoggingEventArgs(string message)
        {
            LogMessage = message;
        }
    }
}
