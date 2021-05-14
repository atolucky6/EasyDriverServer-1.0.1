using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyDriver.OmronHostLink
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

        public ObservableCollection<ReadBlockSetting> ReadBlockSettings { get; set; }

        #endregion

        #region Public methods

        public void InitBlockSettings(ObservableCollection<ReadBlockSetting> readBlockSettings)
        {
            ReadBlockSettings = readBlockSettings;
            grcBlock.ItemsSource = ReadBlockSettings;
            grcBlock.ExpandAllGroups();
        }

        #endregion

        #region Events handlers

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            var mbr = DXMessageBox.Show("Do you want to clear all settings?", "Message", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                ReadBlockSettings.Clear();
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
            CreateBlockSettingWindow createBlock = new CreateBlockSettingWindow(this);
            createBlock.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            createBlock.ShowDialog();
        }

        #endregion
    }
}
