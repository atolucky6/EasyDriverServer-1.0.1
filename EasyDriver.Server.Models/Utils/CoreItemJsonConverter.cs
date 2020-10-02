using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace EasyDriver.Core
{
    public class CoreItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IGroupItem result = null;
            if (serializer.Deserialize(reader) is JObject jObject)
            {
                ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), jObject["ItemType"].Value<string>());
                switch (itemType)
                {
                    case ItemType.LocalStation:
                        result = new LocalStation(null);
                        break;
                    case ItemType.RemoteStation:
                        result = new RemoteStation(null);
                        break;
                    case ItemType.Channel:
                        result = new ChannelCore(null);
                        break;
                    case ItemType.Device:
                        result = new DeviceCore(null);
                        break;
                    case ItemType.Group:
                        result = new GroupCore(null, false);
                        break;
                    case ItemType.Tag:
                        result = new TagCore(null);
                        break;
                    case ItemType.ConnectionSchema:
                    default:
                        break;
                }

                if (result != null)
                {
                    result.Name = jObject["Name"].Value<string>();
                    result.Description = jObject["Description"].Value<string>();
                    result.IsReadOnly = jObject["IsReadOnly"].Value<bool>();
                    result.Enabled = jObject["Enabled"].Value<bool>();
                    result.CreatedDate = jObject["CreatedDate"].Value<DateTime>();
                    result.ModifiedDate = jObject["ModifiedDate"].Value<DateTime>();

                    if (result is ISupportParameters supportParameters)
                    {
                        IParameterContainer parameterContainer = JsonConvert.DeserializeObject<ParameterContainer>(jObject["ParameterContainer"].ToString());
                        if (parameterContainer != null)
                            supportParameters.ParameterContainer = parameterContainer;
                    }

                    if (result is IHaveTag haveTag)
                    {
                        haveTag.HaveTags = jObject["HaveTags"].Value<bool>();

                        var tagItems = jObject["Tags"];
                        if (tagItems != null)
                        {
                            foreach (var item in tagItems)
                            {
                                var child = JsonConvert.DeserializeObject(item.ToString(), typeof(ICoreItem), this);
                                if (child != null)
                                    haveTag.Tags.Add(child);
                            }
                        }
                    }

                    switch (itemType)
                    {
                        case ItemType.LocalStation:
                        case ItemType.RemoteStation:
                            {
                                IStationCore station = result as IStationCore;
                                station.CommunicationMode = (CommunicationMode)Enum.Parse(typeof(CommunicationMode), jObject["CommunicationMode"].Value<string>());
                                station.StationType = jObject["StationType"].Value<string>();
                                station.RemoteAddress = jObject["RemoteAddress"].Value<string>();
                                station.Port = jObject["Port"].Value<ushort>();
                                station.OpcDaServerName = jObject["OpcDaServerName"].Value<string>();
                                station.RefreshRate = jObject["RefreshRate"].Value<int>();
                                station.ConnectionStatus = ConnectionStatus.Connected;
                                break;
                            }
                        case ItemType.Channel:
                            {
                                IChannelCore channel = result as IChannelCore;
                                channel.DriverPath = jObject["DriverPath"].Value<string>();
                                break;
                            }
                        case ItemType.Device:
                            {
                                IDeviceCore device = result as IDeviceCore;
                                device.ByteOrder = (ByteOrder)Enum.Parse(typeof(ByteOrder), jObject["ByteOrder"].Value<string>());
                                break;
                            }
                        case ItemType.Tag:
                            {
                                ITagCore tag = result as ITagCore;
                                tag.ByteOrder = (ByteOrder)Enum.Parse(typeof(ByteOrder), jObject["ByteOrder"].Value<string>());
                                tag.Address = jObject["Address"].Value<string>();
                                tag.DataTypeName = jObject["DataTypeName"].Value<string>();
                                tag.RefreshRate = jObject["RefreshRate"].Value<int>();
                                tag.AccessPermission = (AccessPermission)Enum.Parse(typeof(AccessPermission), jObject["AccessPermission"].Value<string>());
                                tag.Gain = jObject["Gain"].Value<double>();
                                tag.Offset = jObject["Offset"].Value<double>();
                                break;
                            }
                        default:
                            break;
                    }

                    var childItems = jObject["Childs"];
                    if (childItems != null)
                    {
                        foreach (var item in childItems)
                        {
                            var child = JsonConvert.DeserializeObject(item.ToString(), typeof(ICoreItem), this);
                            if (child != null)
                                result.Childs.Add(child);
                        }
                    }

                    result.UpdateParent(false);
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value != null && value is IGroupItem groupItem && value is IClientObject clientObject)
            {
                writer.WritePropertyName("Name");
                writer.WriteValue(groupItem.Name);

                writer.WritePropertyName("Description");
                writer.WriteValue(groupItem.Description);

                writer.WritePropertyName("ItemType");
                writer.WriteValue(clientObject.ItemType.ToString());

                writer.WritePropertyName("IsReadOnly");
                writer.WriteValue(groupItem.IsReadOnly);

                writer.WritePropertyName("Enabled");
                writer.WriteValue(groupItem.Enabled);

                writer.WritePropertyName("CreatedDate");
                writer.WriteValue(groupItem.CreatedDate);

                writer.WritePropertyName("ModifiedDate");
                writer.WriteValue(groupItem.ModifiedDate);

                if (value is ISupportParameters supportParameter)
                {
                    writer.WritePropertyName("ParameterContainer");
                    writer.WriteRawValue(JsonConvert.SerializeObject(supportParameter.ParameterContainer, Formatting.Indented));
                }

                if (value is IStationCore station)
                {
                    writer.WritePropertyName("CommunicationMode");
                    writer.WriteValue(station.CommunicationMode.ToString());

                    writer.WritePropertyName("StationType");
                    writer.WriteValue(station.StationType);

                    writer.WritePropertyName("RemoteAddress");
                    writer.WriteValue(station.RemoteAddress);

                    writer.WritePropertyName("Port");
                    writer.WriteValue(station.Port);

                    writer.WritePropertyName("OpcDaServerName");
                    writer.WriteValue(station.OpcDaServerName);

                    writer.WritePropertyName("RefreshRate");
                    writer.WriteValue(station.RefreshRate);
                }
                else if (value is IChannelCore channel)
                {
                    writer.WritePropertyName("DriverPath");
                    writer.WriteValue(channel.DriverPath);
                }
                else if (value is IDeviceCore device)
                {
                    writer.WritePropertyName("ByteOrder");
                    writer.WriteValue(device.ByteOrder.ToString());
                }
                else if (value is ITagCore tag)
                {
                    writer.WritePropertyName("Address");
                    writer.WriteValue(tag.Address);

                    writer.WritePropertyName("DataTypeName");
                    writer.WriteValue(tag.DataTypeName);

                    writer.WritePropertyName("RefreshRate");
                    writer.WriteValue(tag.RefreshRate);

                    writer.WritePropertyName("ByteOrder");
                    writer.WriteValue(tag.ByteOrder);

                    writer.WritePropertyName("AccessPermission");
                    writer.WriteValue(tag.AccessPermission.ToString());

                    writer.WritePropertyName("Offset");
                    writer.WriteValue(tag.Offset);

                    writer.WritePropertyName("Gain");
                    writer.WriteValue(tag.Gain);
                }

                writer.WritePropertyName("Childs");
                writer.WriteStartArray();
                foreach (var item in groupItem.Childs)
                {
                    writer.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented, this));
                }
                writer.WriteEndArray();

                if (value is IHaveTag haveTagObj)
                {
                    writer.WritePropertyName("HaveTags");
                    writer.WriteValue(haveTagObj.HaveTags);

                    writer.WritePropertyName("Tags");
                    writer.WriteStartArray();
                    if (haveTagObj.Tags != null)
                    {
                        foreach (var item in haveTagObj.Tags)
                        {
                            writer.WriteRawValue(JsonConvert.SerializeObject(item, Formatting.Indented, this));
                        }
                    }
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }
    }

}
