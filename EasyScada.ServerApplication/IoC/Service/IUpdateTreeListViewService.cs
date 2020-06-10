using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid;

namespace EasyScada.ServerApplication
{
    public interface IUpdateTreeListViewService
    {
        void BeginUpdate();
        void EndUpdate();
    }

    public class UpdateTreeListViewService : ServiceBase, IUpdateTreeListViewService
    {
        public void BeginUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                TreeListControl.BeginDataUpdate();
            });
        }

        public void EndUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                TreeListControl.EndDataUpdate();
            });
        }

        protected TreeListControl TreeListControl { get => AssociatedObject as TreeListControl; }

        protected override void OnAttached()
        {
            base.OnAttached();
        }
    }
}
