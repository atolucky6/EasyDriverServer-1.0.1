using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    public interface IChannel
    {
        string Name { get; }
        string DriverName { get; }
        ConnectionType ConnectionType { get; }
        ComunicationMode ComunicationMode { get; }
        int RefreshRate { get; }
        string Error { get; }
        List<IDevice> Devices { get; }
        Dictionary<string, object> Parameters { get; }
    }
}
