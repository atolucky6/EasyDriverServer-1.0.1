using System;
using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    public interface ITag
    {
        string Name { get; }
        string Address { get; }
        string DataType { get; }
        string Value { get; }
        string Quality { get; }
        int RefreshRate { get; }
        int RefreshInterval { get; }
        AccessPermission AccessPermission { get; }
        string Error { get; }
        DateTime LastRefreshTime { get; }
        Dictionary<string, object> Parameters { get; }
    }
}
