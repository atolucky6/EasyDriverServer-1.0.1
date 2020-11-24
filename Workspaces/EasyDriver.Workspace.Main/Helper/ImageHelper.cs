using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EasyDriver.Workspace.Main
{
    public static class ImageHelper
    {
        public static ImageSource GetImageSource(string name)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/EasyDriver.Workspace.Main;component/Resources/Images/{name}", UriKind.Absolute));
        }
    }
}
