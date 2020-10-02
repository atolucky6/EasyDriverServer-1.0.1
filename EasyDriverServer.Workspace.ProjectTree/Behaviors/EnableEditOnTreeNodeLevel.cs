using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows;

namespace EasyDriverServer.Workspace.ProjectTree
{
    public class EnableEditOnTreeNodeLevel : Behavior<TreeListView>
    {
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(EnableEditOnTreeNodeLevel), new PropertyMetadata(0));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ShowingEditor += AssociatedObject_ShowingEditor;
        }

        private void AssociatedObject_ShowingEditor(object sender, DevExpress.Xpf.Grid.TreeList.TreeListShowingEditorEventArgs e)
        {
            if (e.Node.Level != Level)
            {
                e.Cancel = true;
            }
        }
    }
}
