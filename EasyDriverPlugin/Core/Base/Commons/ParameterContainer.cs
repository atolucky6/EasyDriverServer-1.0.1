using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EasyDriverPlugin
{
    [Serializable]
    public class ParameterContainer : BindableCore, IParameterContainer
    {
        [ReadOnly(true)]
        [Display(AutoGenerateField = false)]
        public virtual string DisplayName { get => GetProperty<string>(); set => SetProperty(value); }

        [ReadOnly(true)]
        [Display(AutoGenerateField = false)]
        public virtual string DisplayParameters { get => GetProperty<string>(); set => SetProperty(value); }

        public Dictionary<string, string> Parameters { get; set; }

        [JsonIgnore]
        IReadOnlyDictionary<string, string> IParameterContainer.Parameters { get => Parameters; }

        [JsonConstructor]
        public ParameterContainer()
        {
            Parameters = new Dictionary<string, string>();
        }

        public event EventHandler<ParameterChangedEventArgs> ParameterChanged;

        public T GetValue<T>(string key) where T : IConvertible
        {
            if (Contains(key))
            {
                Type valueType = typeof(T);
                if (valueType.IsEnum)
                    return (T)Enum.Parse(valueType, Parameters[key]);
                else
                    return (T)Convert.ChangeType(Parameters[key], typeof(T));
            }
            throw new ArgumentException("Key wasn't exist in parameters", key);
        }

        public void SetValue(string key, string value)
        {
            if (Parameters.ContainsKey(key))
            {
                string oldValue = Parameters[key];
                Parameters[key] = value;
                if (oldValue != value)
                {
                    var kvp = Parameters.FirstOrDefault(x => x.Key == key);
                    ParameterChanged?.Invoke(this, new ParameterChangedEventArgs(kvp));
                }
            }
            else
            {
                Parameters[key] = value;
                var kvp = Parameters.FirstOrDefault(x => x.Key == key);
                ParameterChanged?.Invoke(this, new ParameterChangedEventArgs(kvp));
            }
        }

        public bool Contains(string key)
        {
            return Parameters.ContainsKey(key);
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
