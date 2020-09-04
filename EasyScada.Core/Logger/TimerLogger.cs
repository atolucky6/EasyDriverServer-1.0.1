using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.Core.Logger
{
    public class TimerLogger : ILogger, IDisposable
    {
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (value)
                    {
                        if (timer == null)
                            timer = new Timer(OnTimerCallback, null, Interval, Interval);
                        if (logTask == null)
                            logTask = Task.Factory.StartNew(DoLog, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    }
                    else
                    {
                        if (timer != null)
                            timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }
            }
        }

        private long interval = 30000;
        public long Interval
        {
            get { return interval; }
            set
            {
                if (value != interval)
                {
                    interval = value < 10 ? 10 : value;
                    if (timer != null)
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        timer.Change(interval, interval);
                    }
                }
            }
        }

        public bool IsDisposed { get; private set; }

        public Func<string, int> LogAction { get; set; }

        public TimerLogger()
        {
            sw = new Stopwatch();
            logQueue = new ConcurrentQueue<string>();
        }

        private readonly Stopwatch sw;
        private Task logTask;
        private Timer timer;


        private readonly ConcurrentQueue<string> logQueue;

        public event EventHandler<LoggedEventArgs> Logged;
        public event EventHandler<LoggingEventArgs> Logging;
        public event EventHandler<LogErrorEventArgs> LogError;

        public void OnTimerCallback(object state)
        {
            sw.Restart();
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            LoggingEventArgs loggingEventArgs = new LoggingEventArgs("");
            try
            {
                if (!Enabled)
                {
                    Logging?.Invoke(this, loggingEventArgs);
                    if (!loggingEventArgs.Cancel)
                        logQueue.Enqueue(loggingEventArgs.LogMessage);
                }
            }
            catch (Exception ex)
            {
                LogError?.Invoke(this, new LogErrorEventArgs(loggingEventArgs.LogMessage, ex));
            }
            finally
            {
                sw.Stop();
                long nextInterval = Interval - sw.ElapsedMilliseconds;
                nextInterval = nextInterval < 10 ? 10 : nextInterval;
                timer.Change(nextInterval, Interval);
            }
        }

        private void DoLog()
        {
            while (!IsDisposed)
            {
                string message = string.Empty;
                try
                {
                    if (!logQueue.IsEmpty && logQueue.TryDequeue(out message))
                        Log(message);
                }
                catch (Exception ex)
                {
                    LogError?.Invoke(this, new LogErrorEventArgs(message, ex));
                }
                finally { Thread.Sleep(10); }
            }
        }

        public void Dispose()
        {
            Enabled = false;
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }
        }

        public int Log(string message)
        {
            int res = LogAction.Invoke(message);
            Logged?.Invoke(this, new LoggedEventArgs(message, res));
            return res;
        }
    }
}
