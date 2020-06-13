using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyScada.Api.Interfaces
{
    public interface IEasyScadaClientApi
    {
        IStation GetStation(string stationName = "Local Station");
        Task<IStation> GetStationAsync(string stationName = "Local Station");

        IEnumerable<IStation> GetAllStations();
        Task<IEnumerable<IStation>> GetAllStationsAsync();

        IChannel GetChannel(string channelName, string stationName = "Local Station");
        Task<IChannel> GetChannelAsync(string channelName, string stationName = "Local Station");

        IEnumerable<IChannel> GetAllChannels(string stationName = "Local Station");
        Task<IEnumerable<IChannel>> GetAllChannelsAsync(string stationName = "Local Station");

        IDevice GetDevice(string deviceName, string channelName, string stationName = "Local Station");
        Task<IDevice> GetDeviceAsync(string deviceName, string channelName, string stationName = "Local Station");

        IEnumerable<IDevice> GetAllDevices(string channelName, string stationName = "Local Station");
        Task<IEnumerable<IDevice>> GetAllDevicesAsync(string channelName, string stationName = "Local Station");

        string SetChannelParameters(string jsonValue, string channelName, string stationName = "Local Station");
        string SetChannelParametersAsync(string jsonValue, string channelName, string stationName = "Local Station");

        string SetDeviceParameters(string jsonValue, string deviceName, string channelName, string stationName = "Local Station");
        Task<string> SetDeviceParametersAsync(string jsonValue, string deviceName, string channelName, string stationName = "Local Station");

        string SetTagParameters(string jsonValue, string tagName, string deviceName, string channelName, string stationName = "Local Station");
        Task<string> SetTagParametersAsync(string jsonValue, string tagName, string deviceName, string channelName, string stationName = "Local Station");

        string WriteTagValue(ITag tag, byte piorityLevel, WriteMode writeMode);
        Task<string> WriteTagValueAsync(ITag tag, byte piorityLevel = byte.MaxValue, WriteMode writeMode = WriteMode.WriteAllValue);

        string WriteTagValue(
            string tagName,
            string value, 
            string deviceName, 
            string channelName, 
            string stationName = "Local Station", 
            byte piorityLevel = byte.MaxValue, 
            WriteMode writeMode = WriteMode.WriteAllValue);

        Task<string> WriteTagValueAsync(
            string tagName,
            string value,
            string deviceName,
            string channelName,
            string stationName = "Local Station",
            byte piorityLevel = byte.MaxValue,
            WriteMode writeMode = WriteMode.WriteAllValue);
    }
}
