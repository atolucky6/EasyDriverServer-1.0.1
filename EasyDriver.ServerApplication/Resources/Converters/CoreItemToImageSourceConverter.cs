using DevExpress.Xpf.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System;
using System.Globalization;
using EasyDriver.Core;
using EasyDriverPlugin;

namespace EasyScada.ServerApplication
{
    public class CoreItemToImageSourceConverter : IValueConverter
    {
        public ImageSource LocalStationImageSource { get; set; }

        public ImageSource RemoteOpcDaStationImageSource { get; set; }

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
                    if (station.StationType == "Local")
                        return LocalStationImageSource;
                    else if (station.StationType == "Remote")
                        return RemoteStationImageSource;
                    else if (station.StationType == "OPC_DA")
                        return RemoteOpcDaStationImageSource;
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
