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
                                    return true;
                                }
                                else
                                {
                                    if (!HighCommandQueue.Any(x => x.PathToTag == command.PathToTag))
                                    {
                                        HighCommandQueue.Enqueue(command);
                                        command.Executed += OnCommandExecuted;
                                        return true;
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
                                    return true;
                                }
                                else
                                {
                                    if (!NormalCommandQueue.Any(x => x.PathToTag == command.PathToTag))
                                    {
                                        NormalCommandQueue.Enqueue(command);
                                        command.Executed += OnCommandExecuted;
                                        return true;
                                    }
                                }
                                break;
                            }
                        default:
                            break;
                    }
                    return false;
                }
                catch { }
                finally { semaphore.Release(); }
            }
            return false;
        }

        private void OnCommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            CommandExecuted?.Invoke(sender, e);
        }
    }
}
