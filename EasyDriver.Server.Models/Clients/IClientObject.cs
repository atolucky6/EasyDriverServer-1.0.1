using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyDriver.Core
{
    public interface IClientObject
    {
        ItemType ItemType { get; }
        string DisplayInfo { get; }
        string Name { get; }
        string Path { get; }
        string Description { get; }
        string Error { get; }
        ConnectionStatus ConnectionStatus { get; }
        List<IClientObject> Childs { get; }
        Dictionary<string, string> Properties { get; }
    }

    public class ClientObject : IClientObject, ICheckable, INotifyPropertyChanged
    {
        public ItemType ItemType { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public string DisplayInfo { get; set; }

        public string Error { get; set; }

        public ConnectionStatus ConnectionStatus { get; set; }

        private bool? isChecked;
        [JsonIgnore]
        public bool? IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }

        [JsonConverter(typeof(ListClientObjectJsonConverter))]
        public List<IClientObject> Childs { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        [JsonConstructor]
        public ClientObject()
        {
            Properties = new Dictionary<string, string>();
            Childs = new List<IClientObject>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ListClientObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<IClientObject> clientObjects = new List<IClientObject>();
            JArray jArray = (JArray)serializer.Deserialize(reader);

            foreach (var item in jArray.Children())
            {
                ClientObject obj = item.ToObject<ClientObject>(serializer);
                if (obj != null)
                    clientObjects.Add(obj);
            }
            return clientObjects;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
