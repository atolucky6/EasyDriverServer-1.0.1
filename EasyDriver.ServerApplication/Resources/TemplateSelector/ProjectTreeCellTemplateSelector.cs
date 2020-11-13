using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using System.Windows;
using System.Windows.Controls;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ChannelTemplate { get; set; }
        public DataTemplate DeviceTemplate { get; set; }
        public DataTemplate TagTemplate { get; set; }
        public DataTemplate HubTemplate { get; set; }
        public DataTemplate LocalStationTemplate { get; set; }
        public DataTemplate RemoteStationTemplate { get; set; }
        public DataTemplate GroupTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var row = (item as GridCellData).Row;
                if (row is IGroupItem coreItem)
                {
                    if (coreItem is IStationCore station)
                    {
                        if (station.StationType == "Local")
                            return LocalStationTemplate;
                        else
                            return RemoteStationTemplate;
                    }
                    else if (coreItem is IChannelCore)
                        return ChannelTemplate;
                    else if (coreItem is IDeviceCore)
                        return DeviceTemplate;
                    return GroupTemplate;
                }
                else if (row is IClientObject clientObject)
                {
                    switch (clientObject.ItemType)
                    {
                        case ItemType.ConnectionSchema:
                            break;
                        case ItemType.LocalStation:
                            return LocalStationTemplate;
                        case ItemType.RemoteStation:
                            return RemoteStationTemplate;
                        case ItemType.Channel:
                            return ChannelTemplate;
                        case ItemType.Device:
                            return DeviceTemplate;
                        case ItemType.Tag:
                            return TagTemplate;
                        case ItemType.Group:
                            return GroupTemplate;
                        default:
                            break;
                    }
                }
                else if (row is HubModel)
                    return HubTemplate;
            }
            return DefaultTemplate;
        }
    }
}
