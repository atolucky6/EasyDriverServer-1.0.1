using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDriver.RemoteConnectionPlugin
{
    public interface IEasyRemoteConnectionPlugin : IDisposable
    {
        void Start(IStationCore remoteConnection);
        void Stop();
        object GetCreateRemoteConnectionView(IGroupItem parent);
        object GetEditRemoteConnectionView(IStationCore remoteConnection);
        IStationCore CreateRemoteConnection(IGroupItem parent);
        event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;
        Task<WriteResponse> WriteAsync(WriteCommand cmd);
    }
}
