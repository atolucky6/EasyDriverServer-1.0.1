using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDriver.RemoteConnectionManager
{
    public interface IRemoteConnection : IDisposable
    {
        bool Enabled { get; set; }
        bool IsDisposed { get; }
        IStationCore Station { get; }
        WriteResponse WriteTagValue(WriteCommand writeCommand);
        Task<WriteResponse> WriteTagValueAsync(WriteCommand wirteCommand);
        IEnumerable<WriteResponse> WriteMultiTagValue(IEnumerable<WriteCommand> writeCommands);
        Task<IEnumerable<WriteResponse>> WriteMultiTagValueAsync(IEnumerable<WriteCommand> writeCommands);
        void Reload();
    }
}
