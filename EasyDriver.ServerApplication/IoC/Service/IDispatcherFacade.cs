using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public interface IDispatcherFacade
    {
        void AddToDispatcherQueue(Delegate workItem);
    }

    public class DispatcherFacade : IDispatcherFacade
    {
        readonly Dispatcher dispatcher;
        readonly ConcurrentDictionary<DispatcherOperation, object> operations;
        long operationsQueueCount;

        public int DispatcherQueueSize { get; set; } = 2500;
        public int DispatcherMonitorRate { get; set; } = 5000;

        public DispatcherFacade(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            operations = new ConcurrentDictionary<DispatcherOperation, object>();
            Observable.Interval(TimeSpan.FromMilliseconds(DispatcherMonitorRate)).Subscribe(MonitorDispatcherQueue);
        }

        private void MonitorDispatcherQueue(long l)
        {
            try
            {
                if (operationsQueueCount > DispatcherQueueSize
                  || operations.Count > DispatcherQueueSize)
                {
                    Application.Current.DoEvents();
                    operations.Clear();
                    Interlocked.Exchange(ref operationsQueueCount, 0);
                }
            }
            catch { }
        }

        public void AddToDispatcherQueue(Delegate workItem)
        {
            Interlocked.Increment(ref operationsQueueCount);
            var operation = dispatcher.BeginInvoke(DispatcherPriority.Background, workItem);
            operation.Completed += (s, e) =>
            {
                Interlocked.Decrement(ref operationsQueueCount);
                operations.TryRemove((DispatcherOperation)s, out object t);
            };
            operations.TryAdd(operation, null);
        }
    }


}
