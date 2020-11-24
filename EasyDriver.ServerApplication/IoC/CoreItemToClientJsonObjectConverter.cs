using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyScada.ServerApplication
{
    public class CoreItemToClientJsonObjectConverter : JsonConverter
    {
        public IEasyDriverPlugin currentDriver = null;
        public IGroupItem parent = null;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ClientObject result = null;
            if (serializer.Deserialize(reader) is JObject jObject)
            {
                result = new ClientObject();
                ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), jObject["ItemType"].Value<string>());
                result.ItemType = itemType;
                result.Name = jObject["Name"].Value<string>();
                result.Path = jObject["Path"].Value<string>();
                result.Description = jObject["Description"].Value<string>();
                result.DisplayInfo = jObject["DisplayInfo"].Value<string>();
                result.Error = jObject["Error"].Value<string>();
                result.ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), jObject["ConnectionStatus"].Value<string>());
                result.Properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject["Properties"].ToString());

                var childItems = jObject["Childs"];
                if (childItems != null)
                {
                    foreach (var item in childItems)
                    { 
                        var child = JsonConvert.DeserializeObject(item.ToString(), typeof(IClientObject), this);
                        if (child != null)
                            result.Childs.Add(child as IClientObject);
                    }
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value != null && value is IGroupItem groupItem)
            {
                writer.WritePropertyName("Name");
                writer.WriteValue(groupItem.Name);

                writer.WritePropertyName("Description");
                writer.WriteValue(groupItem.Description);

                writer.WritePropertyName("ItemType");
                writer.WriteValue(groupItem.ItemType.ToString());

                writer.WritePropertyName("DisplayInfo");
                writer.WriteValue(groupItem.DisplayInformation);

                writer.WritePropertyName("Path");
                writer.WriteValue(groupItem.Path);

                writer.WritePropertyName("Error");
                writer.WriteValue(groupItem.Error);

                if (value is ISupportParameters supportParameter)
                {
                    writer.WritePropertyName("Properties");
                    writer.WriteRawValue(JsonConvert.SerializeObject(supportParameter?.ParameterContainer?.Parameters, Formatting.Indented));
                }

                if (value is IStationCore station)
                {
                    writer.WritePropertyName("ConnectionStatus");
                    writer.WriteValue(station.ConnectionStatus.ToString());
                }
                else if (value is IChannelCore channel)
                {
                    writer.WritePropertyName("ConnectionStatus");
                    writer.WriteValue(channel.ConnectionStatus.ToString());
                }
                else if (value is IDeviceCore device)
                {
                    writer.WritePropertyName("ConnectionStatus");
                    writer.WriteValue(device.ConnectionStatus.ToString());
                }
                else if (value is ITagCore tag)
                {
                    writer.WritePropertyName("ConnectionStatus");
                    writer.WriteValue(tag.ConnectionStatus.ToString());
                }

                writer.WritePropertyName("Childs");
                writer.WriteStartArray();
                foreach (var item in groupItem.Childs)
                {
                    writer.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented, this));
                }

                if (value is IHaveTag haveTagObj)
                {
                    if (haveTagObj.Tags != null)
                    {
                        foreach (var item in haveTagObj.Tags)
                        {
                            writer.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented, this));
                        }
                    }
                }

                writer.WriteEndArray();

               
            }
            else if (value is ConnectionSchema schema)
            {
                writer.WritePropertyName("ItemType");
                writer.WriteValue(schema.ItemType.ToString());

                writer.WritePropertyName("ServerAddress");
                writer.WriteValue(schema.ServerAddress);

                writer.WritePropertyName("Port");
                writer.WriteValue(schema.Port);

                writer.WritePropertyName("CommunicationMode");
                writer.WriteValue(schema.CommunicationMode.ToString());

                writer.WritePropertyName("RefreshRate");
                writer.WriteValue(schema.RefreshRate);

                writer.WritePropertyName("CreatedDate");
                writer.WriteValue(schema.CreatedDate);

                writer.WritePropertyName("Username");
                writer.WriteValue(schema.Username);

                writer.WritePropertyName("Password");
                writer.WriteValue(schema.Password);

                writer.WritePropertyName("Properties");
                writer.WriteRawValue(JsonConvert.SerializeObject(schema.Properties, Formatting.Indented));

                writer.WritePropertyName("Childs");
                writer.WriteStartArray();
                if (schema.Childs != null)
                {
                    foreach (var item in schema.Childs)
                    {
                        writer.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented, this));
                    }
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
