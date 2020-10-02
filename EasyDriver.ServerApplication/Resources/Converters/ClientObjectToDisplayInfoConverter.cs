using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasyScada.ServerApplication
{
    public class ClientObjectToDisplayInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IClientObject clientObject)
            {
                if (!string.IsNullOrEmpty(clientObject.DisplayInfo))
                    return " - " + clientObject.DisplayInfo;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
