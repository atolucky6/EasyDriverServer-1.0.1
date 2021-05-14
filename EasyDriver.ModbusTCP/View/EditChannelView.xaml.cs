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
    /// Interaction logic for EditChannelView.xaml
    /// </summary>
    public partial class EditChannelView : UserControl
    {
        public EditChannelView()
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

    ///// <summary>
    ///// Interaction logic for EditChannelView.xaml
    ///// </summary>
    //public partial class EditChannelView : UserControl
    //{
    //    #region Public members

    //    public IEasyDriverPlugin Driver { get; set; }
    //    public IChannelCore Channel { get; set; }

    //    #endregion

    //    #region Constructors

    //    public EditChannelView(IEasyDriverPlugin driver, IChannelCore channel)
    //    {
    //        Driver = driver;
    //        Channel = channel;

    //        InitializeComponent();

    //        KeyDown += OnKeyDown;

    //        btnOk.Click += BtnOk_Click;
    //        btnCancel.Click += BtnCancel_Click;
    //        Loaded += EditChannelView_Loaded;
    //    }

    //    #endregion

    //    private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    //    {
    //        if (e.Key == Key.Enter)
    //        {
    //            BtnOk_Click(null, null);
    //        }
    //        else if (e.Key == Key.Escape)
    //        {
    //            BtnCancel_Click(null, null);
    //        }
    //    }

    //    private void EditChannelView_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        if (Channel != null)
    //        {
    //            txbDescription.Text = Channel.Description;
    //            txbName.Text = Channel.Name;
    //            spnPort.EditValue = Convert.ToUInt16(Channel.ParameterContainer.Parameters["Port"]);
    //        }
    //    }

    //    private void BtnOk_Click(object sender, RoutedEventArgs e)
    //    {
    //        string validateResult = txbName.Text?.Trim().ValidateFileName("Channel");
    //        if (!string.IsNullOrWhiteSpace(validateResult))
    //        {
    //            DXMessageBox.Show(validateResult, "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
    //            return;
    //        }

    //        if (Channel.Parent.Childs.FirstOrDefault(x => x != Channel && (x as ICoreItem).Name == txbName.Text?.Trim()) != null)
    //        {
    //            DXMessageBox.Show($"The channel name '{txbName.Text?.Trim()}' is already in use.", "Easy Driver Server", MessageBoxButton.OK, MessageBoxImage.Warning);
    //            return;
    //        }

    //        ushort currentPort = Convert.ToUInt16(spnPort.EditValue);
    //        if (currentPort != (ushort)(spnPort.Value))
    //        {
    //            (Driver as ModbusTCPDriver).NotifyPortChanged();
    //        }

    //        Channel.Name = txbName.Text?.Trim();
    //        Driver.Channel.ParameterContainer.DisplayName = "ModbusTCP Comunication Parameters";
    //        Driver.Channel.ParameterContainer.DisplayParameters = "ModbusTCP Comunication Parameters";
    //        Driver.Channel.ParameterContainer.Parameters["Port"] = currentPort.ToString();
    //        Driver.Channel.Description = txbDescription.Text?.Trim();

    //        ((Parent as FrameworkElement).Parent as Window).Tag = Channel;
    //        ((Parent as FrameworkElement).Parent as Window).Close();
    //    }

    //    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    //    {
    //        ((Parent as FrameworkElement).Parent as Window).Close();
    //    }
    //}
}
