using DevExpress.Xpf.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System;
using System.Globalization;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class CoreItemToImageSourceConverter : IValueConverter
    {
        public ImageSource LocalStationImageSource { get; set; }

        public ImageSource RemoteStationImageSource { get; set; }

        public ImageSource ChannelImageSource { get; set; }

        public ImageSource DeviceImageSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BreadcrumbNode node)
            {
                if (node.Item is LocalStation)
                    return LocalStationImageSource;
                else if (node.Item is RemoteStation)
                    return RemoteStationImageSource;
                else if (node.Item is ChannelCore)
                    return ChannelImageSource;
                else if (node.Item is DeviceCore)
                    return DeviceImageSource;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
