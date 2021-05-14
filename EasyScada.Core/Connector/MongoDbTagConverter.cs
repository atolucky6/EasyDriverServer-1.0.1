using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    public class MongoDbTagConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (serializer.Deserialize(reader) is JArray jArray)
            {
                List<Tag> tags = new List<Tag>();
                foreach (var item in jArray.Children())
                {
                    if (item is JObject child)
                    {
                        Tag tag = new Tag();
                        tag.Path = child["p"].Value<string>();

                        string[] arr = tag.Path?.Split('/');
                        if (arr.Length >= 3)
                            tag.Name = arr[arr.Length - 1];

                        tag.Value = child["v"].Value<string>();
                        tag.TimeStamp = DateTime.Now;
                        int qualityNum = child["q"].Value<int>();
                        if (Enum.TryParse(qualityNum.ToString(), out Quality quality))
                        {
                            tag.Quality = quality;
                        }
                        else
                        {
                            tag.Quality = Quality.Bad;
                        }
                        tags.Add(tag);
                    }
                }

                return tags;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value is IEnumerable<ITag> tags)
            {
                foreach (var item in tags)
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("p");
                    writer.WriteValue(item.Path);

                    writer.WritePropertyName("v");
                    writer.WriteValue(item.Value);

                    writer.WritePropertyName("q");
                    writer.WriteValue((int)item.Quality);

                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();
        }
    }
}
