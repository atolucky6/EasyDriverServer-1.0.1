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


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var row = (item as GridCellData).Row;
                if (row is IStationCore stationCore)
                {
                    if (stationCore.IsLocalStation)
                        return LocalStationTemplate;
                    return RemoteStationTemplate;
                }
                if (row is StationClient station)
                {
                    if (station.IsLocalStation)
                        return LocalStationTemplate;
                    return RemoteStationTemplate;
                }
                if (row is IChannelCore || row is ChannelClient)
                    return ChannelTemplate;
                if (row is IDeviceCore || row is DeviceClient)
                    return DeviceTemplate;
                if (row is TagClient)
                    return TagTemplate;
                if (row is HubModel)
                    return HubTemplate;
            }
            return DefaultTemplate;
        }
    }
}
