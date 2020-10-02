using EasyDriver.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class ChannelCoreToDriverNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is IChannelClient channel)
            //{
            //    if (string.IsNullOrEmpty(channel.DriverName))
            //        return string.Empty;
            //    return " - " + channel.DriverName;
            //}
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
