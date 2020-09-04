using System;

namespace EasyScada.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TagAttribute : Attribute
    {
        public string StationName { get; private set; }
        public string ChannelName { get; private set; }
        public string DeviceName { get; private set; }
        public string TagName { get; private set; }
        public string TagPrefix { get; private set; }
        public bool UseTagName { get; private set; }

        public TagAttribute(string stationName, string channelName, string deviceName)
        {
            StationName = stationName;
            ChannelName = channelName;
            DeviceName = deviceName;
            TagPrefix = $"{StationName}/{ChannelName}/{DeviceName}/";
            UseTagName = false;
        }

        public TagAttribute(string stationName, string channelName, string deviceName, string tagName)
        {
            StationName = stationName;
            ChannelName = channelName;
            DeviceName = deviceName;
            TagName = tagName;
            TagPrefix = $"{StationName}/{ChannelName}/{DeviceName}/";
            UseTagName = true;
        }

        public TagAttribute(string tagPrefix)
        {
            TagPrefix = tagPrefix;
            UseTagName = false;
        }

        public TagAttribute(string tagPrefix, string tagName)
        {
            TagPrefix = tagPrefix;
            TagName = tagName;
            UseTagName = true;
        }
    }
}
