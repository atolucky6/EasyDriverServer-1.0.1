using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Controls;
using EasyDriver.Core;
using EasyDriverPlugin;
using System.Windows;
using System.Windows.Media;

namespace EasyScada.ServerApplication
{
    public class FindImageForBreadcrumb : Behavior<BreadcrumbControl>
    {
        public ImageSource LocalStationImageSource
        {
            get { return (ImageSource)GetValue(LocalStationImageSourceProperty); }
            set { SetValue(LocalStationImageSourceProperty, value); }
        }
        public static readonly DependencyProperty LocalStationImageSourceProperty =
            DependencyProperty.Register("LocalStationImageSource", typeof(ImageSource), typeof(FindImageForBreadcrumb), new PropertyMetadata(null));

        public ImageSource RemoteStationImageSource
        {
            get { return (ImageSource)GetValue(RemoteStationImageSourceProperty); }
            set { SetValue(RemoteStationImageSourceProperty, value); }
        }
        public static readonly DependencyProperty RemoteStationImageSourceProperty =
            DependencyProperty.Register("RemoteStationImageSource", typeof(ImageSource), typeof(FindImageForBreadcrumb), new PropertyMetadata(null));

        public ImageSource ChannelImageSource
        {
            get { return (ImageSource)GetValue(ChannelImageSourceProperty); }
            set { SetValue(ChannelImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ChannelImageSourceProperty =
            DependencyProperty.Register("ChannelImageSource", typeof(ImageSource), typeof(FindImageForBreadcrumb), new PropertyMetadata(null));

        public ImageSource DeviceImageSource
        {
            get { return (ImageSource)GetValue(DeviceImageSourceProperty); }
            set { SetValue(DeviceImageSourceProperty, value); }
        }
        public static readonly DependencyProperty DeviceImageSourceProperty =
            DependencyProperty.Register("DeviceImageSource", typeof(ImageSource), typeof(FindImageForBreadcrumb), new PropertyMetadata(null));

        public ImageSource GroupImageSource
        {
            get { return (ImageSource)GetValue(GroupImageSourceProperty); }
            set { SetValue(GroupImageSourceProperty, value); }
        }
        public static readonly DependencyProperty GroupImageSourceProperty =
            DependencyProperty.Register("GroupImageSource", typeof(ImageSource), typeof(FindImageForBreadcrumb), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CustomImage += GetCustomImage;
        }

        private void GetCustomImage(object sender, BreadcrumbCustomImageEventArgs e)
        {
            if (e.Item is IStationCore station)
            {
                if (station.Name?.Replace(" ", "") == "LocalStation")
                    e.Image = LocalStationImageSource;
                else
                    e.Image = RemoteStationImageSource;
            }
            else if (e.Item is IChannelCore)
                e.Image = ChannelImageSource;
            else if (e.Item is IDeviceCore)
                e.Image = DeviceImageSource;
            else if (e.Item is GroupItemBase)
                e.Image = GroupImageSource;
        }
    }
}
