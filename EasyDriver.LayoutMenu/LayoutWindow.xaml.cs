using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using EasyDriver.LayoutManager;
using EasyDriver.MenuPlugin;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace EasyDriver.LayoutMenu
{
    /// <summary>
    /// Interaction logic for LayoutWindow.xaml
    /// </summary>
    public partial class LayoutWindow : ThemedWindow, INotifyPropertyChanged, IDataErrorInfo
    {
        #region Members

        ILayoutManagerService LayoutManagerService { get; set; }
        IBarFactory BarFactory { get; set; }

        string _DisplayName;
        public string DisplayName
        {
            get => _DisplayName?.Trim();
            set
            {
                if (_DisplayName != value)
                {
                    _DisplayName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public IBarComponent LayoutBarItem { get; set; }
        public IBarComponent ContainerLayoutBarItem { get; set; }
        public bool IsNew { get; set; }

        public ICommand AcceptCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (IsNew)
                    {
                        LayoutManagerService.SaveLayout(DisplayName);
                        var barItem = BarFactory.CreateButton(DisplayName);
                        barItem.SetCommand(new DelegateCommand(() => LayoutManagerService.ApplyLayout(barItem.DisplayName)));
                        ContainerLayoutBarItem.Add(barItem);
                    }
                    else
                    {
                        string oldLayoutPath = $"{LayoutManagerService.ActualSaveLayoutPath}/{LayoutBarItem.DisplayName}.xml";
                        string newLayoutPath = $"{LayoutManagerService.ActualSaveLayoutPath}/{DisplayName}.xml";
                        File.Move(oldLayoutPath, newLayoutPath);
                        LayoutBarItem.DisplayName = DisplayName;
                    }
                    Close();
                });
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for create layout
        /// </summary>
        /// <param name="layoutManagerService"></param>
        /// <param name="containerLayoutBarItem"></param>
        public LayoutWindow(
            ILayoutManagerService layoutManagerService, 
            IBarComponent containerLayoutBarItem)
        {
            InitializeComponent();
            DataContext = this;
            LayoutManagerService = layoutManagerService;
            ContainerLayoutBarItem = containerLayoutBarItem;
            DisplayName = GetInvalidLayoutName();
            IsNew = true;
        }

        /// <summary>
        /// Constructor for edit layout
        /// </summary>
        /// <param name="layoutBarItemToEdit"></param>
        /// <param name="containerLayoutBarItem"></param>
        /// <param name="layoutManagerService"></param>
        public LayoutWindow(
            IBarComponent layoutBarItemToEdit, 
            IBarComponent containerLayoutBarItem, 
            ILayoutManagerService layoutManagerService)
        {
            InitializeComponent();
            DataContext = this;
            LayoutManagerService = layoutManagerService;
            ContainerLayoutBarItem = containerLayoutBarItem;
            LayoutBarItem = layoutBarItemToEdit;
            DisplayName = layoutBarItemToEdit.DisplayName;
            IsNew = false;
        }

        #endregion

        #region Methods

        public string GetInvalidLayoutName()
        {
            int index = 0;
            bool isValid = false;
            string name = null;
            do
            {
                index++;
                name = "New Layout " + index;
                if (ContainerLayoutBarItem.Find(x => x.DisplayName == name).FirstOrDefault() == null)
                    isValid = true;
            }
            while (!isValid);
            return name;
        }

        #endregion

        #region Events

        private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e) => Close();

        #endregion

        #region IDataErrorInfo

        public string Error { get; set; }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(DisplayName):
                        return ValidateDisplayName();
                    default:
                        return string.Empty;
                }
            }
        }

        string ValidateDisplayName()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return "The layout name don't allow empty value.";
            if (ContainerLayoutBarItem.Find(x => x.DisplayName == DisplayName && x != LayoutBarItem).FirstOrDefault() != null)
                return $"The layout name '{DisplayName}' is already exists.";
            return string.Empty;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyChanged = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }

        #endregion
    }
}
