using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using EasyDriver.Client.Models;
using EasyDriver.Server.Models;
using System.Collections;

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
                if (item is Station station)
                {
                    if (station.IsLocalStation)
                        return station.Channels;
                    else
                        return station.RemoteStations;
                }
                if (item is Channel channel)
                    return channel.Devices;
                if (item is Device device)
                    return device.Tags;
            }
            return null;
        }
    }
}
