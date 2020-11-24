using EasyDriver.ServicePlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyDriver.Service.ApplicationProperties
{
    [Service(int.MaxValue, true)]
    public class ApplicationPropertiesService : EasyServicePluginBase, IApplicationPropertiesService
    {
        public Dictionary<string, string> Properties { get; set; }
        public string ApplicationTitle { get; set; }
        public bool IsMainWindowExit { get; set; }
        public ServerConfiguration ServerConfiguration { get; protected set; }
        string AppDirectory { get; set; }
        string PropertiesPath { get; set; }
        string applicationDirectory;
        public string ApplicationDirectory
        {
            get => applicationDirectory;
            set
            {
                if (applicationDirectory != value)
                {
                    applicationDirectory = value;
                    ServerConfiguration = new ServerConfiguration(applicationDirectory + "\\server.ini");
                }
            }
        }

        public event EventHandler Saved;

        public ApplicationPropertiesService() : base()
        {
            Properties = new Dictionary<string, string>();
        }

        public override void EndInit()
        {
            Refresh();
        }

        public bool Contains(string key)
        {
            return Properties.ContainsKey(key);
        }

        public T GetOrAddValue<T>(string key, T value) where T : IConvertible
        {
            Properties[key] = value?.ToString();
            return value;
        }

        public T GetValue<T>(string key) where T : IConvertible
        {
            if (Contains(key))
            {
                Type valueType = typeof(T);
                if (valueType.IsEnum)
                    return (T)Enum.Parse(valueType, Properties[key]);
                else
                    return (T)Convert.ChangeType(Properties[key], typeof(T));
            }
            throw new ArgumentException("Key wasn't exist in parameters", key);
        }

        public void Refresh()
        {
            AppDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            PropertiesPath = AppDirectory + "\\properties.json";
            try
            {
                if (File.Exists(PropertiesPath))
                {
                    Properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(PropertiesPath));
                }
                else
                {
                    Properties = new Dictionary<string, string>();
                }
            }
            catch { Properties = new Dictionary<string, string>(); }
        }

        public void Save()
        {
            File.WriteAllText(PropertiesPath, JsonConvert.SerializeObject(Properties, Formatting.Indented));
            Saved?.Invoke(this, EventArgs.Empty);
        }

        public void SetValue(string key, string value)
        {
            Properties[key] = value;
        }

        public bool TryGetValue<T>(string key, out T value) where T : IConvertible
        {
            if (Contains(key))
            {
                value = GetValue<T>(key);
                return true;
            }
            value = default;
            return false;
        }
    }
}
