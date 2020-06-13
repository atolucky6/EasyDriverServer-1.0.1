using DevExpress.Xpf.Grid;
using EasyDriverPlugin;
using EasyScada.Core;
using System.Windows;
using System.Windows.Controls;

namespace EasyScada.ServerApplication
{
    public class ProjectTreeCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ChannelTemplate { get; set; }
        public DataTemplate DeviceTemplate { get; set; }
        public DataTemplate StationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var row = (item as GridCellData).Row;
                if (row is IStationCore)
                    return StationTemplate;
                if (row is IChannelCore)
                    return ChannelTemplate;
                if (row is IDeviceCore)
                    return DeviceTemplate;
            }
            return DefaultTemplate;
        }
    }
}
