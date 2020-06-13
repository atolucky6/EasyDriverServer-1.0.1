using System;
using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    public interface IDevice
    {
        string Name { get; }
        Dictionary<string, object> Parameters { get; }
        DateTime LastRefreshTime { get; }
        string Error { get; }
        List<ITag> Tags { get; }
    }
}
