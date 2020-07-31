﻿using System;

namespace EasyScada.Winforms.Connector
{
    [Serializable]
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatus OldStatus { get; private set; }
        public ConnectionStatus NewStatus { get; private set; }
        public ConnectionStatusChangedEventArgs(ConnectionStatus oldStatus, ConnectionStatus newStatus)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}
