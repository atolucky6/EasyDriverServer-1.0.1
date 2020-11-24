using DevExpress.Xpf.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System;
using System.Globalization;
using EasyDriverPlugin;
using EasyDriver.RemoteConnectionPlugin;

namespace EasyDriver.Workspace.Main
{
    public class CoreItemToImageSourceConverter : IValueConverter
    {
        public ImageSource LocalStationImageSource { get; set; }

        public ImageSource RemoteStationImageSource { get; set; }

        public ImageSource ChannelImageSource { get; set; }

        public ImageSource DeviceImageSource { get; set; }

        public ImageSource GroupImageSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BreadcrumbNode node)
            {
                if (node.Item is IStationCore station)
                {
                    if (station is LocalStation)
                        return LocalStationImageSource;
                    return RemoteStationImageSource;
                }
                else if (node.Item is IChannelCore)
                    return ChannelImageSource;
                else if (node.Item is IDeviceCore)
                    return DeviceImageSource;
                else if (node.Item is IGroupItem)
                    return GroupImageSource;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
