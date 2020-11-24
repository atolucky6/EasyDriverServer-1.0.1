using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;

namespace EasyDriver.Service.ApplicationProperties
{
    public interface IApplicationPropertiesService : IEasyServicePlugin
    {
        string ApplicationDirectory { get; set; } 
        string ApplicationTitle { get; set; }
        bool IsMainWindowExit { get; set; }
        ServerConfiguration ServerConfiguration { get; }
        Dictionary<string, string> Properties { get; }
        T GetValue<T>(string key) where T : IConvertible;
        T GetOrAddValue<T>(string key, T value) where T : IConvertible;
        bool Contains(string key);
        void SetValue(string key, string value);
        bool TryGetValue<T>(string key, out T value) where T : IConvertible;
        void Save();
        void Refresh();
        event EventHandler Saved;
    }
}
