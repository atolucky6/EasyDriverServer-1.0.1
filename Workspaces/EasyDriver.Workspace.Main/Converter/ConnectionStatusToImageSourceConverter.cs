using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace EasyDriver.Workspace.Main
{
    public class ConnectionStatusToImageSourceConverter : IValueConverter
    {
        public ImageSource ConnectedImageSource { get; set; }
        public ImageSource DisconnectedImageSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectionStatus status)
            {
                if (status == ConnectionStatus.Connected)
                    return ConnectedImageSource;
            }
            return DisconnectedImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
