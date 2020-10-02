using EasyDriverPlugin;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.S7Ethernet
{
    /// <summary>
    /// Interaction logic for CreateChannelView.xaml
    /// </summary>
    public partial class CreateChannelView : UserControl
    {
        public CreateChannelView()
        {
            InitializeComponent();
        }

        readonly S7EthernetDriver driver;

        public CreateChannelView(S7EthernetDriver driver, IGroupItem parent, IChannelCore templateItem = null)
        {
            this.driver = driver;
            driver.Channel.ParameterContainer.DisplayName = "S7Ethernet Comunication Parameters";
            driver.Channel.ParameterContainer.DisplayParameters = "S7Ethernet Comunication Parameters";
            Loaded += CreateChannelView_Loaded;
        }

        private void CreateChannelView_Loaded(object sender, RoutedEventArgs e)
        {
            driver.Connect();
            ((Parent as FrameworkElement).Parent as Window).Tag = driver.Channel;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }
    }
}
