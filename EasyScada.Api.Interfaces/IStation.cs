using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    public interface IStation
    {
        string Name { get; }
        string IpAddress { get; }
        ushort Port { get; }
        string Error { get; }
        Dictionary<string, string> Parameters { get; }
        List<IChannel> Channels { get; }
    }
}
