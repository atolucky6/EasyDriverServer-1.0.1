using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;


namespace EasyDriver.ContextWindow
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
            if (context is UserControl wpfUserControl)
            {
                AddChild(wpfUserControl);
            }
        }
    }
}
