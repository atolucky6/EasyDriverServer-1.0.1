using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class StringEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
