using EasyDriverPlugin;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.S7Ethernet
{
    /// <summary>
    /// Interaction logic for EditChannelView.xaml
    /// </summary>
    public partial class EditChannelView : UserControl
    {
        public EditChannelView()
        {
            InitializeComponent();
        }

        readonly S7EthernetDriver driver;

        public EditChannelView(S7EthernetDriver driver, IChannelCore channel)
        {
            this.driver = driver;
            Loaded += EditChannelView_Loaded;
        }

        private void EditChannelView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Tag = driver.Channel;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }
    }
}
