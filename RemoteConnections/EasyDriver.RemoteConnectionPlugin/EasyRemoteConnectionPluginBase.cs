using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.RemoteConnectionPlugin
{
    public abstract class EasyRemoteConnectionPluginBase : IEasyRemoteConnectionPlugin
    {
        public virtual event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;

        public virtual void Start(IStationCore remoteConnection)
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Dispose()
        {
        }

        public abstract IStationCore CreateRemoteConnection(IGroupItem parent);

        public abstract object GetCreateRemoteConnectionView(IGroupItem parent);

        public abstract object GetEditRemoteConnectionView(IStationCore remoteConnection);

        public virtual Task<WriteResponse> WriteAsync(WriteCommand cmd)
        {
            throw new NotImplementedException();
        }
    }
}
