using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid;

namespace EasyScada.ServerApplication
{
    public interface ITableViewUtilities
    {
        void ShowSearchPanel();
    }

    public class TableViewUtilities : ServiceBase, ITableViewUtilities
    {
        TableView TableView { get => AssociatedObject as TableView; }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        public void ShowSearchPanel()
        {
            TableView?.ShowSearchPanel(true);
        }
    }
}
