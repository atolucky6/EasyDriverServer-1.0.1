using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using EasyDriver.LayoutManager;
using EasyDriver.MenuPlugin;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace EasyDriver.LayoutMenu
{
    /// <summary>
    /// Interaction logic for ManageLayoutWindow.xaml
    /// </summary>
    public partial class ManageLayoutWindow : ThemedWindow
    {
        #region Members

        public ObservableCollection<IBarComponent> LayoutBarItems { get; set; }
        ILayoutManagerService LayoutManagerService { get; set; }
        public IBarComponent ContainerLayoutBarItem { get; set; }
        public IBarComponent SelectedLayoutBarItem { get; set; }

        public ICommand RenameCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    LayoutWindow layoutWindow = new LayoutWindow(SelectedLayoutBarItem, ContainerLayoutBarItem, LayoutManagerService);
                    layoutWindow.ShowDialog();

                }, () => SelectedLayoutBarItem != null);
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    string layoutPathToRemove = $"{LayoutManagerService.ActualSaveLayoutPath}/{SelectedLayoutBarItem.DisplayName}.xml";
                    if (File.Exists(layoutPathToRemove))
                        File.Delete(layoutPathToRemove);
                    ContainerLayoutBarItem.Remove(SelectedLayoutBarItem);
                    LayoutBarItems.Remove(SelectedLayoutBarItem);
                }, () => SelectedLayoutBarItem != null);
            }
        }

        #endregion

        #region Constructors

        public ManageLayoutWindow(IBarComponent containerLayoutBarItem, ILayoutManagerService layoutManagerService)
        {
            InitializeComponent();
            ContainerLayoutBarItem = containerLayoutBarItem;
            LayoutManagerService = layoutManagerService;
            LayoutBarItems = new ObservableCollection<IBarComponent>(ContainerLayoutBarItem.Find(x => true));
            DataContext = this;
        }

        #endregion

        #region Events

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        #endregion
    }
}
