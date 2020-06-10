using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Forms;

namespace EasyScada.ServerApplication
{
    public class TextToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = "0";

            if (value != null)
                text = value.ToString();
            var size = TextRenderer.MeasureText(text, new System.Drawing.Font("SegoeUI", 12));
            return size.Width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
