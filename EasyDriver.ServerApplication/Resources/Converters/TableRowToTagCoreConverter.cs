using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyScada.ServerApplication
{
    public class TableRowToTagCoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ITagCore tagCore)
                return tagCore;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
