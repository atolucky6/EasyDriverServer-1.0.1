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
        public DataTemplate RemoteOpcDaStationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var row = (item as GridCellData).Row;
                if (row is IStationCore stationCore)
                {
                    switch (stationCore.StationType)
                    {
                        case StationType.Local:
                            return LocalStationTemplate;
                        case StationType.Remote:
                            return RemoteStationTemplate;
                        case StationType.OPC_DA:
                            return RemoteOpcDaStationTemplate;
                        default:
                            break;
                    }
                }
                if (row is StationClient station)
                {
                    switch (station.StationType)
                    {
                        case StationType.Local:
                            return LocalStationTemplate;
                        case StationType.Remote:
                            return RemoteStationTemplate;
                        case StationType.OPC_DA:
                            return RemoteOpcDaStationTemplate;
                        default:
                            break;
                    }
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
