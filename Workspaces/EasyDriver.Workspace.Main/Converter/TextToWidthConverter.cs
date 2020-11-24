using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EasyDriver.Workspace.Main
{
    public class TextToWidthConverter : IValueConverter
    {
        FormattedText formattedText;
        FontFamily fontFamily = (FontFamily)(new FontFamilyConverter().ConvertFrom("SegeoUI"));
        FontStyle fontStyle = (FontStyle)(new FontStyleConverter().ConvertFrom("Normal"));
        FontWeight fontWeight = (FontWeight)(new FontWeightConverter().ConvertFrom("Normal"));
        FontStretch fontStrech = (FontStretch)(new FontStretchConverter().ConvertFrom("Normal"));


        public TextToWidthConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = "0";
            if (value != null)
                text = value.ToString();

            formattedText = new FormattedText(
                "",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStrech),
                14,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return formattedText.Width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
