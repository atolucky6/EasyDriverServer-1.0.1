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
                    switch (station.StationType)
                    {
                        case StationType.Local:
                        case StationType.OPC_DA:
                            return station.Channels;
                        case StationType.Remote:
                            return station.RemoteStations;
                        default:
                            break;
                    }                        
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
