using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace EasyDriver.Workspace.Main
{
    public class BarItemAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EasyDriver.Service.BarManager.BarItemAlignment alignment)
            {
                return (BarItemAlignment)(int)alignment;
            }
            return BarItemAlignment.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
