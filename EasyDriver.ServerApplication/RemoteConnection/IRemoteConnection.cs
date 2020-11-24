using EasyDriverPlugin;
using System;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IRemoteConnection : IDisposable
    {
        IStationCore Station { get; set; }
        string RemoteAddress { get; }
        ushort Port { get; }
        Task<WriteResponse> WriteTagValue(WriteCommand writeCommand, WriteResponse response);
    }
}
