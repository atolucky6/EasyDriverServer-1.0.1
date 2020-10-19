using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    public class ConnectionSchemaJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ICoreItem result = null;
            if (serializer.Deserialize(reader) is JObject jObject)
            {
                ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), jObject["ItemType"].Value<string>());
                switch (itemType)
                {
                    case ItemType.ConnectionSchema:
                        result = new ConnectionSchema();
                        break;
                    case ItemType.Tag:
                        result = new Tag();
                        break;
                    default:
                        result = new CoreItem();
                        break;
                }

                if (result is ConnectionSchema schema)
                {
                    schema.ItemType = itemType;
                    schema.ServerAddress = jObject["ServerAddress"].Value<string>();
                    schema.Port = jObject["Port"].Value<ushort>();
                    schema.Username = jObject["Username"].Value<string>();
                    schema.Password = jObject["Password"].Value<string>();
                    schema.CommunicationMode = (CommunicationMode)Enum.Parse(typeof(CommunicationMode), jObject["CommunicationMode"].Value<string>());
                    schema.RefreshRate = jObject["RefreshRate"].Value<int>();
                    schema.CreatedDate = jObject["CreatedDate"].Value<DateTime>();
                    Dictionary<string, string> properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject["Properties"].ToString());
                    if (properties != null)
                        schema.Properties = properties;

                    var childItems = jObject["Childs"];
                    if (childItems != null)
                    {
                        foreach (var child in childItems)
                        {
                            var obj = JsonConvert.DeserializeObject(child.ToString(), typeof(ICoreItem), this);
                            if (obj is ICoreItem childCoreItem)
                                schema.Childs.Add(childCoreItem);
                        }
                    }
                }
                else if (result is CoreItem item)
                {
                    item.Name = jObject["Name"].Value<string>();
                    item.ItemType = itemType;
                    item.Path = jObject["Path"].Value<string>();
                    item.Description = jObject["Description"].Value<string>();
                    item.DisplayInfo = jObject["DisplayInfo"].Value<string>();
                    item.Error = jObject["Error"].Value<string>();
                    //if (result is Tag tag)
                    //{
                    //    //tag.Address = jObject["Address"].Value<string>();
                    //    //tag.DataType = jObject["DataType"].Value<string>();
                    //    //tag.RefreshRate = jObject["RefreshRate"].Value<int>();
                    //    //tag.AccessPermission = (AccessPermission)jObject["RefreshRate"].Value<int>();
                    //}
                    Dictionary<string, string> properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject["Properties"].ToString());
                    if (properties != null)
                        item.Properties = properties;

                    var childItems = jObject["Childs"];
                    if (childItems != null)
                    {
                        foreach (var child in childItems)
                        {
                            var obj = JsonConvert.DeserializeObject<ICoreItem>(child.ToString(), this);
                            if (obj is ICoreItem childCoreItem)
                                item.Childs.Add(childCoreItem);
                        }
                    }
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value is ConnectionSchema schema)
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
            else if (value is ICoreItem coreItem)
            {
                writer.WritePropertyName("ItemType");
                writer.WriteValue(coreItem.ItemType.ToString());

                writer.WritePropertyName("Name");
                writer.WriteValue(coreItem.Name);

                writer.WritePropertyName("Path");
                writer.WriteValue(coreItem.Path);

                writer.WritePropertyName("Description");
                writer.WriteValue(coreItem.Description);

                writer.WritePropertyName("DisplayInfo");
                writer.WriteValue(coreItem.DisplayInfo);

                writer.WritePropertyName("Error");
                writer.WriteValue(coreItem.Error);

                writer.WritePropertyName("Properties");
                writer.WriteRawValue(JsonConvert.SerializeObject(coreItem.Properties, Formatting.Indented));

                //if (coreItem is ITag tag)
                //{
                //    writer.WritePropertyName("Address");
                //    writer.WriteValue(tag.Address);

                //    writer.WritePropertyName("DataType");
                //    writer.WriteValue(tag.DataType);

                //    writer.WritePropertyName("RefreshRate");
                //    writer.WriteValue(tag.RefreshRate);

                //    writer.WritePropertyName("AccessPermission");
                //    writer.WriteValue(tag.AccessPermission.ToString());
                //}

                writer.WritePropertyName("Childs");
                writer.WriteStartArray();
                if (coreItem.Childs != null)
                {
                    foreach (var item in coreItem.Childs)
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
