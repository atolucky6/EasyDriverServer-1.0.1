using EasyDriverPlugin;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyScada.ServerApplication
{
    public class TreeListRowToTagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ITagCore)
                return value;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
