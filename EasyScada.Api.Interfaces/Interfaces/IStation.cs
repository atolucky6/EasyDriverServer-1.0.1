using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    public interface IStation : IPath
    {
        string Name { get; }
        bool IsLocalStation { get; }
        string RemoteAddress { get; }
        CommunicationMode CommunicationMode { get; }
        int RefreshRate { get; }
        ushort Port { get; }
        string Error { get; }
        Dictionary<string, object> Parameters { get; }
        List<IChannel> Channels { get; }
        List<IStation> RemoteStations { get; }
    }
}
