using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyDriverServer.Workspace.ProjectTree
{
    /// <summary>
    /// Interaction logic for ProjectTreeWorkspaceView.xaml
    /// </summary>
    public partial class ProjectTreeWorkspaceView : UserControl
    {
        public ProjectTreeWorkspaceView()
        {
            InitializeComponent();
        }
    }

    [ContentProperty("Items")]
    public class CustomPopupMenu : PopupMenu
    {
        public CustomPopupMenu() : base()
        {
            Items.Add(new BarButtonItem());
        }
    }
}
