using EasyDriverPlugin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public interface IDriverManagerService
    {
        Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; }
        IEasyDriverPlugin GetDriver(ICoreItem item);
        IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver);
        IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath);
        void RemoveDriver(IChannelCore channel);
    }

    public class DriverManagerService : IDriverManagerService
    {
        public DriverManagerService()
        {
            DriverPoll = new Dictionary<IChannelCore, IEasyDriverPlugin>();
            WriteTagQueueManagers = new List<WriteTagQueueManager>();
        }

        public Dictionary<IChannelCore, IEasyDriverPlugin> DriverPoll { get; private set; }

        public List<WriteTagQueueManager> WriteTagQueueManagers { get; set; }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, IEasyDriverPlugin driver)
        {
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            else
            {
                DriverPoll[channel] = driver;
                WriteTagQueueManagers.Add(new WriteTagQueueManager(driver, channel));
            }
            return driver;
        }

        public IEasyDriverPlugin AddDriver(IChannelCore channel, string driverPath)
        {
            if (GetDriver(channel) == null)
            {
                IEasyDriverPlugin driver = AssemblyHelper.LoadAndCreateInstance<IEasyDriverPlugin>(driverPath);
                if (driver != null)
                {
                    driver.Channel = channel;
                    return AddDriver(channel, driver);
                }
            }
            return null;
        }

        public IEasyDriverPlugin GetDriver(ICoreItem item)
        {
            IChannelCore channel = GetChannel(item);
            if (DriverPoll.ContainsKey(channel))
                return DriverPoll[channel];
            return null;
        }

        public async void RemoveDriver(IChannelCore channel)
        {
            await Task.Run(() =>
            {
                IEasyDriverPlugin driver = GetDriver(channel);
                if (driver != null)
                {
                    WriteTagQueueManagers.Remove(WriteTagQueueManagers.FirstOrDefault(x => x.Channel == channel));
                    DriverPoll.Remove(channel);
                    driver.Dispose();
                }
            });
        }

        private IChannelCore GetChannel(ICoreItem item)
        {
            if (item == null)
                return null;
            if (item is IChannelCore channel)
                return channel;
            return GetChannel(item.Parent);
        }
    }

    public class WriteTagQueueManager
    {
        public IEasyDriverPlugin Driver { get; set; }
        public IChannelCore Channel { get; set; }
        public List<WriteCommand> WriteQueue { get; set; }
        public int MaxWriteTimesPerScan { get; set; } = 10;
        readonly SemaphoreSlim semaphore;
        
        public WriteTagQueueManager(IEasyDriverPlugin driver, IChannelCore channel)
        {
            Driver = driver;
            Channel = channel;
            WriteQueue = new List<WriteCommand>();
            semaphore = new SemaphoreSlim(1, 1);
            Driver.Refreshed += Driver_Refreshed;
        }

        private void Driver_Refreshed(object sender, System.EventArgs e)
        {
            semaphore.Wait();
            try
            {
                int count = 0;
                for (int i = 0; i < MaxWriteTimesPerScan; i++)
                {
                    if (i < WriteQueue.Count)
                    {
                        count++;
                        //if ((Channel as IChannelClient).GetItem<IClientTag>(WriteQueue[i].PathToTag) is ITagCore tagCore)
                        //{
                        //    Driver.Write(tagCore, WriteQueue[i].PathToTag);
                        //}
                    }
                    else
                    {
                        break;
                    }
                }
                if (count > 0)
                    WriteQueue.RemoveRange(0, count);
            }
            catch { }
            finally { semaphore.Release(); }
        }

        public Quality WriteTagValue(WriteCommand writeCommand)
        {
            semaphore.Wait();
            try
            {

                if (writeCommand.WritePiority == WritePiority.Highest)
                {
                    //if ((Channel as IChannelClient).GetItem<IClientTag>(writeCommand.PathToTag) is ITagCore tagCore)
                    //{
                    //    return Driver.Write(tagCore, writeCommand.PathToTag);
                    //}
                }
                else if (writeCommand.WritePiority == WritePiority.High)
                {
                    if (writeCommand.WriteMode == WriteMode.WriteLatestValue)
                    {
                        if (WriteQueue.FirstOrDefault(x => x.PathToTag == writeCommand.PathToTag) is WriteCommand oldCmd)
                            WriteQueue.Remove(oldCmd);
                    }
                    else
                    {
                        WriteQueue.Add(writeCommand);
                    }

                    WriteQueue.Sort((x, y) =>
                    {
                        if ((int)x.WritePiority > (int)y.WritePiority)
                            return 1;
                        else if ((int)x.WritePiority < (int)y.WritePiority)
                            return -1;
                        else
                        {
                            if (x.SendTime > y.SendTime)
                                return 1;
                            else if (x.SendTime < y.SendTime)
                                return -1;
                        }
                        return 0;
                    });
                }
            }
            catch { }
            finally { semaphore.Release(); }
            return Quality.Uncertain;
        }
    }
}
