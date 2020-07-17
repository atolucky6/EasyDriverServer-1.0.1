using EasyDriverPlugin;
using Newtonsoft.Json;
using System.IO;

namespace EasyDriver.Core
{
    public static class JsonUtils
    {

        public static void WriteProjectJson(this JsonTextWriter writer, IEasyScadaProject project)
        {
            if (project != null)
            {
                writer.WriteStartArray();
                // Write property 'Stations'
                foreach (var item in project.Childs)
                {
                    if (item is IStationCore childStation)
                        writer.WriteStationCoreJson(childStation);
                }
                writer.WriteEndArray();
            }
        }

        public static void WriteStationCoreJson(this JsonTextWriter writer, IStationCore stationCore)
        {
            if (stationCore != null)
            {
                // Start writing object
                writer.WriteStartObject();

                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(stationCore.Path);

                // Write property 'LastRefreshTime'
                writer.WritePropertyName("r");
                writer.WriteValue(stationCore.LastRefreshTime);

                // Write property 'ConnectionStatus'
                writer.WritePropertyName("s");
                writer.WriteValue(stationCore.ConnectionStatus.ToString());

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(stationCore.CommunicationError);

                // Write property 'Channels'
                writer.WritePropertyName("channels");
                writer.WriteStartArray();
                foreach (var item in stationCore.Childs)
                {
                    if (item is IChannelCore channelCore)
                        writer.WriteChannelCoreJson(channelCore);
                }
                writer.WriteEndArray();

                // Write property 'Stations'
                writer.WritePropertyName("stations");
                writer.WriteStartArray();
                foreach (var item in stationCore.Childs)
                {
                    if (item is IStationCore childStation)
                        writer.WriteStationCoreJson(childStation);
                }
                writer.WriteEndArray();

                // End
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Hàm ghi <see cref="IChannelCore"/> vào trong JsonWriter
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="channelCore"></param>
        public static void WriteChannelCoreJson(this JsonTextWriter writer, IChannelCore channelCore)
        {
            if (channelCore != null)
            {
                // Start writing object
                writer.WriteStartObject();

                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(channelCore.Path);

                // Write property 'LastRefreshTime'
                writer.WritePropertyName("r");
                writer.WriteValue(channelCore.LastRefreshTime);

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(channelCore.CommunicationError);

                // Write property 'Devices'
                writer.WritePropertyName("devices");
                writer.WriteStartArray();
                foreach (var item in channelCore.Childs)
                {
                    if (item is IDeviceCore deviceCore)
                        writer.WriteDeviceCoreJson(deviceCore);
                }
                writer.WriteEndArray();

                // End
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Hàm ghi <see cref="IDeviceCore"/> vào trong JsonWriter
        /// </summary>
        /// <param name="deviceCore"></param>
        /// <returns></returns>
        public static void WriteDeviceCoreJson(this JsonTextWriter writer, IDeviceCore deviceCore)
        {
            if (deviceCore != null)
            {
                // Start writing object
                writer.WriteStartObject();
                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(deviceCore.Path);

                // Write property 'LastRefreshTime'
                writer.WritePropertyName("r");
                writer.WriteValue(deviceCore.LastRefreshTime);

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(deviceCore.CommunicationError);

                // Write property 'Tags'
                writer.WritePropertyName("tags");
                writer.WriteStartArray();
                foreach (var item in deviceCore.Childs)
                {
                    if (item is ITagCore tagCore)
                        writer.WriteTagCoreJson(tagCore);
                }
                writer.WriteEndArray();

                // End
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Hàm ghi <see cref="ITagCore"/> vào trong JsonWriter
        /// </summary>
        /// <param name="tagCore">Đối tượng TagCore cần chuyển thành Json</param>
        /// <returns></returns>
        public static void WriteTagCoreJson(this JsonWriter writer, ITagCore tagCore)
        {
            if (tagCore != null)
            {
                // Start writing json object
                writer.WriteStartObject();

                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(tagCore.Path);

                // Write property 'Value'
                writer.WritePropertyName("v");
                writer.WriteValue(tagCore.Value);

                // Write property 'Quality'
                writer.WritePropertyName("q");
                writer.WriteValue(tagCore.Quality);

                // Write property 'TimeStamp'
                writer.WritePropertyName("t");
                writer.WriteValue(tagCore.TimeStamp);

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(tagCore.CommunicationError);

                // End
                writer.WriteEndObject();
            }
        }

        public static void WriteStationClientJson(this JsonTextWriter writer, StationClient station)
        {
            if (station != null)
            {
                // Start writing object
                writer.WriteStartObject();

                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(station.Path);

                // Write property 'LastRefreshTime'
                writer.WritePropertyName("r");
                writer.WriteValue(station.LastRefreshTime);

                // Write property 'ConnectionStatus'
                writer.WritePropertyName("s");
                writer.WriteValue(station.ConnectionStatus.ToString());

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(station.Error);

                // Write property 'Channels'
                writer.WritePropertyName("channels");
                writer.WriteStartArray();
                foreach (var item in station.Channels)
                    writer.WriteChannelClientJson(item);
                writer.WriteEndArray();

                // Write property 'Stations'
                writer.WritePropertyName("stations");
                writer.WriteStartArray();
                foreach (var item in station.RemoteStations)
                    writer.WriteStationClientJson(item);
                writer.WriteEndArray();

                // End
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Hàm ghi <see cref="ChannelClient"/> vào trong JsonWriter
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="channel"></param>
        public static void WriteChannelClientJson(this JsonTextWriter writer, ChannelClient channel)
        {
            if (channel != null)
            {
                // Start writing object
                writer.WriteStartObject();

                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(channel.Path);

                // Write property 'LastRefreshTime'
                writer.WritePropertyName("r");
                writer.WriteValue(channel.LastRefreshTime);

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(channel.Error);

                // Write property 'Devices'
                writer.WritePropertyName("devices");
                writer.WriteStartArray();
                foreach (var item in channel.Devices)
                    writer.WriteDeviceClientJson(item);
                writer.WriteEndArray();

                // End
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Hàm ghi <see cref="DeviceClient"/> vào trong JsonWriter
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static void WriteDeviceClientJson(this JsonTextWriter writer, DeviceClient device)
        {
            if (device != null)
            {
                // Start writing object
                writer.WriteStartObject();
                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(device.Path);

                // Write property 'LastRefreshTime'
                writer.WritePropertyName("r");
                writer.WriteValue(device.LastRefreshTime);

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(device.Error);

                // Write property 'Tags'
                writer.WritePropertyName("tags");
                writer.WriteStartArray();
                foreach (var item in device.Tags)
                    writer.WriteTagClientJson(item);
                writer.WriteEndArray();

                // End
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Hàm ghi <see cref="TagClient"/> vào trong JsonWriter
        /// </summary>
        /// <param name="tagCore">Đối tượng TagCore cần chuyển thành Json</param>
        /// <returns></returns>
        public static void WriteTagClientJson(this JsonWriter writer, TagClient tag)
        {
            if (tag != null)
            {
                // Start writing json object
                writer.WriteStartObject();

                // Write property 'Path'
                writer.WritePropertyName("p");
                writer.WriteValue(tag.Path);

                // Write property 'Value'
                writer.WritePropertyName("v");
                writer.WriteValue(tag.Value);

                // Write property 'Quality'
                writer.WritePropertyName("q");
                writer.WriteValue(tag.Quality);

                // Write property 'TimeStamp'
                writer.WritePropertyName("t");
                writer.WriteValue(tag.TimeStamp);

                // Write property 'CommunicationError'
                writer.WritePropertyName("e");
                writer.WriteValue(tag.Error);

                // End
                writer.WriteEndObject();
            }
        }
    }
}
