using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.RemoteConnection.SignalR
{
    public class CreateRemoteConnectionViewModel
    {
        public IGroupItem Parent { get; set; }
        public CreateRemoteConnectionViewModel(IGroupItem parent)
        {
            Parent = parent;
        }

    }
}
