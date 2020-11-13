using System.Collections.Generic;
using EasyDriverPlugin;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using EasyDriver.Core;
using System.Windows.Input;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for CreateTagView.xaml
    /// </summary>
    public partial class CreateTagView : UserControl
    {
        public CreateTagView()
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
