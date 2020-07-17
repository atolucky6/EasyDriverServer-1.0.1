using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.ModbusRTU
{
    /// <summary>
    /// Interaction logic for ReadBlockSettingView.xaml
    /// </summary>
    public partial class ReadBlockSettingView : UserControl
    {
        #region Constructors

        public ReadBlockSettingView()
        {
            InitializeComponent();
        }

        #endregion

        #region Public members

        public AddressType AddressType { get; set; }
        public ObservableCollection<ReadBlockSetting> ReadBlockSettings { get; set; }

        #endregion

        #region Public methods

        public void InitBlockSettings(ObservableCollection<ReadBlockSetting> readBlockSettings, AddressType addressType)
        {
            ReadBlockSettings = readBlockSettings;
            grcBlock.ItemsSource = ReadBlockSettings;
            AddressType = addressType;
        }

        #endregion

        #region Events handlers

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            var mbr = DXMessageBox.Show("Do you want to clear all settings?", "Easy Driver Server", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                ReadBlockSettings.Clear();
            }
        }

        private void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (grcBlock.SelectedItem is ReadBlockSetting block)
            {
                int index = ReadBlockSettings.IndexOf(block);
                if (index < ReadBlockSettings.Count - 1)
                    ReadBlockSettings.Move(index, index + 1);
            }
        }

        private void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (grcBlock.SelectedItem is ReadBlockSetting block)
            {
                int index = ReadBlockSettings.IndexOf(block);
                if (index > 0)
                    ReadBlockSettings.Move(index, index - 1);
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (grcBlock.SelectedItem is ReadBlockSetting block)
            {
                int index = ReadBlockSettings.IndexOf(block);
                ReadBlockSettings.RemoveAt(index);
                if (ReadBlockSettings.Count > 0)
                {
                    if (index < ReadBlockSettings.Count)
                        grcBlock.SelectedItem = ReadBlockSettings[index];
                    else
                        grcBlock.SelectedItem = ReadBlockSettings.LastOrDefault();
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ReadBlockSetting newBlock = new ReadBlockSetting() { AddressType = this.AddressType, Enabled = true };
            ReadBlockSettings.Add(newBlock);
            grcBlock.SelectedItem = newBlock;
            grcBlock.View.ScrollIntoView(ReadBlockSettings.Count - 1);
        }

        #endregion
    }
}
