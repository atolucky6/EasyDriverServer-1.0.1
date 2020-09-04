using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EasyScada.Wpf.Controls
{
    public class DispatcherService
    {
        public static DispatcherService Instance { get; } = new DispatcherService();

        readonly Dispatcher dispatcher;
        readonly System.Timers.Timer monitorTimer;
        readonly ConcurrentDictionary<DispatcherOperation, object> operations;
        long operationsQueueCount;

        public int DispatcherQueueSize { get; set; } = 2500;
        public int DispatcherMonitorRate { get; set; } = 5000;

        public DispatcherService()
        {
            this.dispatcher = Application.Current.Dispatcher;
            operations = new ConcurrentDictionary<DispatcherOperation, object>();
            monitorTimer = new System.Timers.Timer();
            monitorTimer.Interval = DispatcherMonitorRate;
            monitorTimer.Elapsed += MonitorDispatcherQueue;
            monitorTimer.Start();
        }

        private void MonitorDispatcherQueue(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                monitorTimer.Start();
                if (operationsQueueCount > DispatcherQueueSize || operations.Count > DispatcherQueueSize)
                {
                    Application.Current.DoEvents();
                    operations.Clear();
                    Interlocked.Exchange(ref operationsQueueCount, 0);
                }
            }
            catch { }
            finally { monitorTimer.Start(); }
        }

        public void AddToDispatcherQueue(Delegate workItem)
        {
            Interlocked.Increment(ref operationsQueueCount);
            var operation = dispatcher.BeginInvoke(DispatcherPriority.Background, workItem);
            operations.TryAdd(operation, null);
            operation.Completed += (s, e) =>
            {
                Interlocked.Decrement(ref operationsQueueCount);
                operations.TryRemove((DispatcherOperation)s, out object t);
            };
        }
    }

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
