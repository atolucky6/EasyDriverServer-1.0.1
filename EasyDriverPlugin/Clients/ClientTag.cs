using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyDriverPlugin
{
    public class ClientTag : IClientTag
    {
        public string Value { get; set; }

        public Quality Quality { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Error { get; set; }

        [JsonConverter(typeof(ListClientTagJsonConverter))]
        public List<IClientTag> Childs { get; set; }

        [JsonConstructor]
        public ClientTag()
        {
            Childs = new List<IClientTag>();
        }
    }

    public class ListClientTagJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<IClientTag> clientObjects = new List<IClientTag>();

            if (serializer.Deserialize(reader) is JArray jArray)
            {
                foreach (var item in jArray.Children())
                {
                    IClientTag obj = JsonConvert.DeserializeObject<ClientTag>(item.ToString());
                    if (obj != null)
                        clientObjects.Add(obj);
                }
            }
            return clientObjects;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value is List<IClientTag> clientTags)
            {
                foreach (var item in clientTags)
                {
                    if (item != null)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("Name");
                        writer.WriteValue(item.Name);

                        writer.WritePropertyName("Path");
                        writer.WriteValue(item.Path);

                        writer.WritePropertyName("Error");
                        writer.WriteValue(item.Error);

                        writer.WritePropertyName("Value");
                        writer.WriteValue(item.Value);

                        writer.WritePropertyName("Quality");
                        writer.WriteValue(item.Quality);

                        writer.WritePropertyName("TimeStamp");
                        writer.WriteValue(item.TimeStamp);

                        writer.WritePropertyName("Childs");
                        writer.WriteRawValue(JsonConvert.SerializeObject(item.Childs, this));

                        writer.WriteEndObject();
                    }
                }
            }
            writer.WriteEndArray();
        }
    }

    public class ClientTagJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value != null)
            {
                if (value is IClientTag tag)
                {
                    writer.WritePropertyName("Name");
                    writer.WriteValue(tag.Name);

                    writer.WritePropertyName("Path");
                    writer.WriteValue(tag.Path);

                    writer.WritePropertyName("Error");
                    writer.WriteValue(tag.Error);

                    writer.WritePropertyName("Value");
                    writer.WriteValue(tag.Value);

                    writer.WritePropertyName("Quality");
                    writer.WriteValue(tag.Quality);

                    writer.WritePropertyName("TimeStamp");
                    writer.WriteValue(tag.TimeStamp);

                    writer.WritePropertyName("Childs");
                    serializer.Serialize(writer, tag.Childs);
                }
                else
                {
                    writer.WriteNull();
                }
            }
            else
            {
                writer.WriteNull();
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (serializer.Deserialize(reader) is JObject obj)
            {
                ClientTag tag = obj.ToObject<ClientTag>();
                return tag;
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
