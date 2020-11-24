using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public class ClientTagJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value != null)
            {
                if (value is ITagCore tag)
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
                    writer.WriteStartArray();
                    foreach (var child in (tag as IHaveTag).Tags)
                    {
                        writer.WriteRawValue(JsonConvert.SerializeObject(child, Formatting.Indented, this));
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
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
            if (serializer.Deserialize(reader) is JObject jObject)
            {
                ClientTag tag = new ClientTag();
                tag.Name = jObject["Name"].Value<string>();
                tag.Path = jObject["Path"].Value<string>();
                tag.Error = jObject["Error"].Value<string>();
                tag.Value = jObject["Value"].Value<string>();
                tag.TimeStamp = jObject["TimeStamp"].Value<DateTime>();
                tag.Quality = (Quality)Enum.Parse(typeof(Quality), jObject["Quality"].Value<string>());
                var tagChilds = jObject["Childs"];
                if (tagChilds != null)
                {
                    foreach (var child in tagChilds)
                    {
                        var tagChild = JsonConvert.DeserializeObject(child.ToString(), typeof(IClientTag), this);
                        if (tagChild != null)
                            tag.Childs.Add(tagChild as ClientTag);
                    }
                }
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
