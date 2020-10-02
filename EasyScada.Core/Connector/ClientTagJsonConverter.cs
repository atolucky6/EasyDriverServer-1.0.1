using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    class ClientTagJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ClientTag result = null;
            if (serializer.Deserialize(reader) is JObject jObject)
            {
                result = new ClientTag();
                result.Name = jObject["Name"].Value<string>();
                result.Path = jObject["Path"].Value<string>();
                result.Value = jObject["Value"].Value<string>();
                result.Quality = (Quality)jObject["Quality"].Value<int>();
                result.Error = jObject["Error"].Value<string>();

                var child = jObject["Childs"];
                if (child != null)
                {
                    result.Childs = new List<ClientTag>();
                    foreach (var item in child)
                    {
                        if (JsonConvert.DeserializeObject(item.ToString(), typeof(ClientTag), this) is ClientTag childTag)
                            result.Childs.Add(childTag);
                    }
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
