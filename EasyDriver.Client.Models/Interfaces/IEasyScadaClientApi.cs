using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDriver.Client.Models
{
    public interface IEasyDriverClientApi
    {
        string Subscribe(string stationsJson, string communicationMode, int refreshRate);
        Task<string> SubscribeAsync(string stationsJson, string communicationMode, int refreshRate);

        string Unsubscribe();
        Task<string> UnsubscribeAsync();

        string GetSubscribedData();
        Task<string> GetSubscribedDataAsync();

        string GetStation(string pathToStation = "Local Station");
        Task<string> GetStationAsync(string pathToStation = "Local Station");

        string GetAllStations();
        Task<string> GetAllStationsAsync();

        string GetStations(IEnumerable<string> pathToStations);
        Task<string> GetStationsAsync(IEnumerable<string> pathToStations);

        string GetChannel(string pathToChannel);
        Task<string> GetChannelAsync(string pathToChannel);

        string GetAllChannels(string pathToStation = "Local Station");
        Task<string> GetAllChannelsAsync(string pathToStation = "Local Station");

        string GetChannels(IEnumerable<string> pathToChannels);
        Task<string> GetChannelsAsync(IEnumerable<string> pathToChannels);

        string GetDevice(string pathToDevice);
        Task<string> GetDeviceAsync(string pathToDevice);

        string GetAllDevices(string pathToChannel);
        Task<string> GetAllDevicesAsync(string pathToChannel);

        string GetDevices(IEnumerable<string> pathToDevices);
        Task<string> GetDevicesAsync(IEnumerable<string> pathToDevices);

        string GetTag(string pathToTag);
        Task<string> GetTagAsync(string pathToTag);

        string GetAllTags(string pathToDevice);
        Task<string> GetAllTagsAsync(string pathToDevice);

        WriteResponse WriteTagValue(
            string pathToTag,
            string value,
            WritePiority writePiority = WritePiority.Default, 
            WriteMode writeMode = WriteMode.WriteAllValue);

        Task<WriteResponse> WriteTagValueAsync(
            string pathToTag,
            string value,
            WritePiority writePiority = WritePiority.Default,
            WriteMode writeMode = WriteMode.WriteAllValue);

        WriteResponse WriteTagValue(WriteCommand writeCommand);
        Task<WriteResponse> WriteTagValueAsync(WriteCommand writeCommand);

        string SetChannelParameters(string jsonValue, string pathToChannel);
        string SetChannelParametersAsync(string jsonValue, string pathToChannel);

        string SetDeviceParameters(string jsonValue, string pathToDevice);
        Task<string> SetDeviceParametersAsync(string jsonValue, string pathToChannel);

        string SetTagParameters(string jsonValue, string pathToTag);
        Task<string> SetTagParametersAsync(string jsonValue, string pathToTag);
    }
}
