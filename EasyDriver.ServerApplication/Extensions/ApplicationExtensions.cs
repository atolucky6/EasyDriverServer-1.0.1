using System.Windows;
using System.Windows.Threading;

namespace EasyScada.ServerApplication
{
    public static class ApplicationExtensions
    {
        public static void DoEvents(this Application application)
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new ExitFrameHandler(frm => frm.Continue = false), frame);
        }

        private delegate void ExitFrameHandler(DispatcherFrame frame);
    }
}
