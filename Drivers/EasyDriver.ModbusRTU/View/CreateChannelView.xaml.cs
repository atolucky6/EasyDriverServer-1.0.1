using System.Windows.Controls;
using System.Windows.Input;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateChannelView.xaml
    /// </summary>
    public partial class CreateChannelView : UserControl
    {
        public CreateChannelView()
        {
            InitializeComponent();
            PreviewKeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (btnOk.IsEnabled)
                    btnOk.Command.Execute(null);
            }
            else if (e.Key == Key.Escape)
            {
                btnCancel.Command.Execute(null);
            }
        }
    }
}
