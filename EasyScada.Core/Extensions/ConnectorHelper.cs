using System.Collections.Generic;
using System.Linq;

namespace EasyScada.Core
{
    public static class ConnectorHelper
    {
        public static IEnumerable<string> GetAllTagPath(this ConnectionSchema connector)
        {
            IEnumerable<Tag> result = GetAllTag(connector)?.ToList();
            if (result == null)
                return new List<string>();
            return result.Select(x => x.Path);
        }

        public static IEnumerable<Tag> GetAllTag(this ConnectionSchema connector)
        {
            if (connector != null && connector.Stations != null)
            {
                foreach (var station in connector.Stations)
                    foreach (var tag in GetAllTag(station))
                        yield return tag;
            }
        }

        public static IEnumerable<Tag> GetAllTag(this Station station)
        {
            if (station != null && station.Channels != null)
            {
                foreach (var channel in station.Channels)
                    foreach (var tag in GetAllTag(channel))
                        yield return tag;
            }

            if (station != null && station.RemoteStations != null)
            {
                foreach (var innerStation in station.RemoteStations)
                    foreach (var tag in GetAllTag(innerStation))
                        yield return tag;
            }
        }

        public static IEnumerable<Tag> GetAllTag(this Channel channel)
        {
            if (channel != null && channel.Devices != null)
            {
                foreach (var device in channel.Devices)
                    foreach (var tag in GetAllTag(device))
                        yield return tag;
            }
        }

        public static IEnumerable<Tag> GetAllTag(this Device device)
        {
            if (device != null && device.Tags != null)
            {
                foreach (var tag in device.Tags)
                    yield return tag;
            }
        }
    }
}
