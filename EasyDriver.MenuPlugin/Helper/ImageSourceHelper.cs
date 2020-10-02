using DevExpress.Xpf.Core;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EasyDriver.MenuPlugin
{
    public static class ImageSourceHelper
    {
        public static ImageSource GetImageSourceFromSvg(this Uri uri, int width, int height)
        {
            return (ImageSource)new SvgImageSourceExtension() { Uri = uri, Size = new Size(width, height) }.ProvideValue(null);
        }
    }
}
