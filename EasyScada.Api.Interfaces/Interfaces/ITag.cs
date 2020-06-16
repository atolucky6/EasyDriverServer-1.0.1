using System;
using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    public interface ITag : IPath
    {
        string Name { get; }
        string Address { get; }
        string DataType { get; }
        string Value { get; }
        Quality Quality { get; }
        int RefreshRate { get; }
        int RefreshInterval { get; }
        AccessPermission AccessPermission { get; }
        string Error { get; }
        DateTime TimeStamp { get; }
        Dictionary<string, object> Parameters { get; }
    }
}
