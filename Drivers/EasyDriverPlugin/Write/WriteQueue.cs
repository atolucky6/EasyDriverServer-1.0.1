using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriverPlugin
{
    public class WriteQueue 
    {
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public ConcurrentQueue<WriteCommand> NormalCommandQueue { get; protected set; }
        public ConcurrentQueue<WriteCommand> HighCommandQueue { get; protected set; }

        public int Count { get => NormalCommandQueue.Count + HighCommandQueue.Count; }

        public event EventHandler<CommandExecutedEventArgs> CommandExecuted;

        public WriteQueue()
        {
            NormalCommandQueue = new ConcurrentQueue<WriteCommand>();
            HighCommandQueue = new ConcurrentQueue<WriteCommand>();
        }

        public WriteCommand GetCommand()
        {
            WriteCommand command = null;
            try
            {
                semaphore.Wait();
                HighCommandQueue.TryDequeue(out command);
                if (command == null)
                    NormalCommandQueue.TryDequeue(out command);
            }
            catch { }
            finally { semaphore.Release(); }
            return command;
        }

        public bool Add(WriteCommand command)
        {
            bool result = false;
            if (command != null)
            {
                try
                {
                    semaphore.Wait();
                    switch (command.WritePiority)
                    {
                        case WritePiority.Default:
                            {
                                if (command.WriteMode == WriteMode.WriteAllValue)
                                {
                                    HighCommandQueue.Enqueue(command);
                                    command.Executed += OnCommandExecuted;
                                    result = true;
                                }
                                else
                                {
                                    if (!HighCommandQueue.Any(x => x.Prefix == command.Prefix && x.TagName == command.TagName))
                                    {
                                        HighCommandQueue.Enqueue(command);
                                        command.Executed += OnCommandExecuted;
                                        result = true;
                                    }
                                }
                                break;
                            }
                        case WritePiority.High:
                            {
                                if (command.WriteMode == WriteMode.WriteAllValue)
                                {
                                    NormalCommandQueue.Enqueue(command);
                                    command.Executed += OnCommandExecuted;
                                    result = true;
                                }
                                else
                                {
                                    if (!NormalCommandQueue.Any(x => x.Prefix == command.Prefix && x.TagName == command.TagName))
                                    {
                                        NormalCommandQueue.Enqueue(command);
                                        command.Executed += OnCommandExecuted;
                                        result = true;
                                    }
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
                catch { }
                finally 
                {
                    semaphore.Release();
                    if (result)
                        Enqueued?.Invoke(this, EventArgs.Empty);
                }
            }
            return result;
        }

        private void OnCommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            CommandExecuted?.Invoke(sender, e);
        }

        public event EventHandler Enqueued;
    }
}
