using DevExpress.Xpf.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasyDriver.ModbusTCP
{
    /// <summary>
    /// Interaction logic for CreateChannelView.xaml
    /// </summary>
    public partial class CreateChannelView : UserControl
    {
        #region Public members

        public IEasyDriverPlugin Driver { get; set; }
        public IGroupItem ParentChannel { get; set; }

        #endregion

        #region Constructors

        public CreateChannelView(IEasyDriverPlugin driver, IGroupItem parent, IChannelCore templateItem) 
        {
            Driver = driver;
            ParentChannel = parent;

            InitializeComponent();

            if (templateItem != null)
            {
                if (templateItem.ParameterContainer.Parameters.ContainsKey("Port"))
                    spnPort.Value = (ushort)templateItem.ParameterContainer.Parameters["Port"];
            }

            KeyDown += OnKeyDown;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOk_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                BtnCancel_Click(null, null);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Driver.Channel.ParameterContainer.DisplayName = "ModbusTCP Comunication Parameters";
            Driver.Channel.ParameterContainer.DisplayParameters = "ModbusTCP Comunication Parameters";
            Driver.Channel.ParameterContainer.Parameters["Port"] = spnPort.Value;

            Driver.Connect();
            ((Parent as FrameworkElement).Parent as Window).Tag = Driver.Channel;
            ((Parent as FrameworkElement).Parent as Window).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FrameworkElement).Parent as Window).Close();
        }
    }
}
