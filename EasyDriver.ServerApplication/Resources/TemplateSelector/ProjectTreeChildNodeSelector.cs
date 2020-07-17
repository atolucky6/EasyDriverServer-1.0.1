using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using System.Collections;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeChildNodeSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item != null)
            {               
                if (item is IDeviceCore)
                    return null;
                if (item is IGroupItem groupItem)
                    return groupItem.Childs;
                if (item is HubModel hubModel)
                    return hubModel.Stations;
                if (item is StationClient station)
                {
                    if (station.IsLocalStation)
                        return station.Channels;
                    else
                        return station.RemoteStations;
                }
                if (item is ChannelClient channel)
                    return channel.Devices;
                if (item is DeviceClient device)
                    return device.Tags;
            }
            return null;
        }
    }
}
