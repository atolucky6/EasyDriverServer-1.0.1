using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace EasyDriver.Workspace.Main
{
    public class TagToImageSourceConverter : IValueConverter
    {
        public ImageSource InternalTagImageSource { get; set; }
        public ImageSource TagImageSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ITagCore tag)
            {
                if (tag.IsInternalTag)
                    return InternalTagImageSource;
            }
            return TagImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
