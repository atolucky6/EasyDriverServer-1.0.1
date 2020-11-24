using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
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
                    IClientTag obj = JsonConvert.DeserializeObject<ClientTag>(item.ToString(), new ClientTagJsonConverter());
                    if (obj != null)
                        clientObjects.Add(obj);
                }
            }
            return clientObjects;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value is IEnumerable coreItems)
            {
                foreach (var item in coreItems)
                {
                    if (item is ITagCore tag)
                    {
                        writer.WriteStartObject();
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
                        writer.WriteStartArray();
                        foreach (var child in (tag as IHaveTag).Tags)
                        {
                            writer.WriteRawValue(JsonConvert.SerializeObject(child, Formatting.Indented, new ClientTagJsonConverter()));
                        }
                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                }
            }
            writer.WriteEndArray();
        }
    }
}
