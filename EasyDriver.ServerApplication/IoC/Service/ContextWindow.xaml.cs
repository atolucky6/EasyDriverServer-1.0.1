using DevExpress.Xpf.Core;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interaction logic for ContextWindow.xaml
    /// </summary>
    public partial class ContextWindow : ThemedWindow
    {
        private System.Windows.Controls.UserControl userControl;
        public ContextWindow()
        {
            InitializeComponent();
        }

        public ContextWindow(object context)
        {
            InitializeComponent();
            if (context is System.Windows.Controls.UserControl wpfUserControl)
            {
                userControl = wpfUserControl;
                Panel.Children.Add(userControl);
            }
        }

        public object GetTag()
        {
            return userControl?.Tag;
        }
    }
}
