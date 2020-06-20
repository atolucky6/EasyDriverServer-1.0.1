using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using EasyDriver.Client.Models;
using System.Windows;
using System.Windows.Controls;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ChannelTemplate { get; set; }
        public DataTemplate DeviceTemplate { get; set; }
        public DataTemplate TagTemplate { get; set; }
        public DataTemplate HubTemplate { get; set; }
        public DataTemplate StationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var row = (item as GridCellData).Row;
                if (row is IStationCore || row is Channel)
                    return StationTemplate;
                if (row is IChannelCore || row is Channel)
                    return ChannelTemplate;
                if (row is IDeviceCore || row is Device)
                    return DeviceTemplate;
                if (row is Tag)
                    return TagTemplate;
                if (row is HubModel)
                    return HubTemplate;
            }
            return DefaultTemplate;
        }
    }
}
