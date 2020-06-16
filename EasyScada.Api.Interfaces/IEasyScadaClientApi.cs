using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyScada.Api.Interfaces
{
    public interface IEasyDriverClientApi
    {
        string GetStation(string pathToStation = "Local Station");
        Task<string> GetStationAsync(string pathToStation = "Local Station");

        string GetAllStations();
        Task<string> GetAllStationsAsync();

        string GetChannel(string pathToChannel);
        Task<string> GetChannelAsync(string pathToChannel);

        string GetAllChannels(string pathToStation = "Local Station");
        Task<string> GetAllChannelsAsync(string pathToStation = "Local Station");

        string GetDevice(string pathToDevice);
        Task<string> GetDeviceAsync(string pathToDevice);

        string GetAllDevices(string pathToChannel);
        Task<string> GetAllDevicesAsync(string pathToChannel);

        string GetTag(string pathToTag);
        Task<string> GetTagAsync(string pathToTag);

        string GetAllTags(string pathToDevice);
        Task<string> GetAllTagsAsync(string pathToDevice);

        string WriteTagValue(
            string pathToTag,
            string value,
            byte piorityLevel = byte.MaxValue, 
            WriteMode writeMode = WriteMode.WriteAllValue);

        Task<string> WriteTagValueAsync(
            string pathToTag,
            string value,
            byte piorityLevel = byte.MaxValue,
            WriteMode writeMode = WriteMode.WriteAllValue);

        string SetChannelParameters(string jsonValue, string pathToChannel);
        string SetChannelParametersAsync(string jsonValue, string pathToChannel);

        string SetDeviceParameters(string jsonValue, string pathToDevice);
        Task<string> SetDeviceParametersAsync(string jsonValue, string pathToChannel);

        string SetTagParameters(string jsonValue, string pathToTag);
        Task<string> SetTagParametersAsync(string jsonValue, string pathToTag);

    }
}
