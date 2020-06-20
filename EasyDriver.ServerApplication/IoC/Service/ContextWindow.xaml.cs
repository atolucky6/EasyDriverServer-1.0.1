using DevExpress.Xpf.Core;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interaction logic for ContextWindow.xaml
    /// </summary>
    public partial class ContextWindow : ThemedWindow
    {
        public ContextWindow()
        {
            InitializeComponent();
        }

        public ContextWindow(object context)
        {
            InitializeComponent();
            if (context is System.Windows.Controls.UserControl wpfUserControl)
            {
                Panel.Children.Add(wpfUserControl);
            }
        }
    }
}
