using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;


namespace EasyDriver.ProjectMenu
{
    /// <summary>
    /// Interaction logic for OpenProjectView.xaml
    /// </summary>
    public partial class RecentProjectsView : ThemedWindow
    {
        public RecentProjectsView()
        {
            InitializeComponent();
        }

        public RecentProjectsView(EasyProjectMenuPlugin menuPlugin)
        {
            InitializeComponent();
            DataContext = ViewModelSource.Create(() => new RecentProjectsViewModel(menuPlugin));
        }
    }
}
