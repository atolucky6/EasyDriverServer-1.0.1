using EasyDriver.RemoteConnectionPlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.RemoteConnection.SignalR
{
    public class EditRemoteConnectionViewModel
    {
        public SignalRRemoteConnection RemoteConnection { get; set; }

        public EditRemoteConnectionViewModel(SignalRRemoteConnection remoteConnection)
        {
            RemoteConnection = remoteConnection;
        }
    }
}
