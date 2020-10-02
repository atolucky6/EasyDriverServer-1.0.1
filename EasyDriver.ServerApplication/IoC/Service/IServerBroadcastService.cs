using EasyDriver.Core;
using EasyDriverPlugin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IServerBroadcastService
    {
        BroadcastMode BroadcastMode { get; set; }
        int BroadcastRate { get; set; }
        IHubConnectionContext<dynamic> Clients { get; }
        List<BroadcastEndpoint> BroadcastEndpoints { get; }
        void AddEndpoint(string connectionId, List<string> subscribeTagPaths, CommunicationMode communicationMode, int refreshRate);
        void RemoveEndpoint(string connectionId);
        long BroadcastExecuteInterval { get; }
    }

    public class ServerBroadcastService : IServerBroadcastService, IDisposable
    {
        #region Members

        readonly Stopwatch stopwatch;
        readonly IProjectManagerService projectManagerService;
        readonly ApplicationViewModel applicationViewModel;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        readonly Task broadcastTask;

        public ConcurrentDictionary<string, string[]> PathCache = new ConcurrentDictionary<string, string[]>();
        public IHubConnectionContext<dynamic> Clients { get; private set; }
        public List<BroadcastEndpoint> BroadcastEndpoints { get; private set; }
        public bool IsDisposed { get; protected set; }
        public long BroadcastExecuteInterval { get; private set; }
        public BroadcastMode BroadcastMode { get; set; }
        public int BroadcastRate { get; set; }

        #endregion

        #region Constructors

        public ServerBroadcastService(
            IProjectManagerService projectManagerService, 
            ApplicationViewModel applicationViewModel,
            BroadcastMode broadcastMode,
            int broadcastRate)
        {
            BroadcastMode = broadcastMode;
            BroadcastRate = broadcastRate;
            this.projectManagerService = projectManagerService;
            this.applicationViewModel = applicationViewModel;
            Clients = GlobalHost.ConnectionManager.GetHubContext<EasyDriverServerHub>().Clients;
            BroadcastEndpoints = new List<BroadcastEndpoint>();
            stopwatch = new Stopwatch();
            broadcastTask = Task.Factory.StartNew(BroadcastToClients, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion

        #region Methods

        private void BroadcastToClients()
        {
            while (!IsDisposed)
            {
                stopwatch.Restart();
                try
                {
                    semaphore.Wait();

                    if (projectManagerService.CurrentProject != null && BroadcastEndpoints.Count > 0)
                    {
                        switch (BroadcastMode)
                        {
                            case BroadcastMode.SendAskedData:
                                foreach (var endpoint in BroadcastEndpoints)
                                    endpoint.Send();
                                break;
                            case BroadcastMode.SendAllData:
                                List<string> receivedClients = new List<string>(BroadcastEndpoints.Select(x => x.ConnectionId));
                                List<IClientTag> clientTags = projectManagerService.CurrentProject.GetAllTags()?.Select(x => x as IClientTag).ToList();
                                if (clientTags == null)
                                    clientTags = new List<IClientTag>();
                                Clients.Clients(receivedClients).broadcastSubscribeData(JsonConvert.SerializeObject(clientTags));
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch { }
                finally
                {
                    stopwatch.Stop();
                    BroadcastExecuteInterval = stopwatch.ElapsedMilliseconds;
                    long delay = BroadcastRate - stopwatch.ElapsedMilliseconds;
                    if (projectManagerService.CurrentProject == null)
                        delay = 100;
                    semaphore.Release();
                    Thread.Sleep((int)(delay < 0 ? 100 : delay));
                }
            }
        }

        private void BroadcastCallback(object state)
        {
            IsDisposed = true;
        }

        public void AddEndpoint(string connectionId, List<string> subscribeTagPaths, CommunicationMode communicationMode, int refreshRate)
        {
            semaphore.Wait();
            try
            {
                foreach (var item in subscribeTagPaths)
                {
                    if (!string.IsNullOrEmpty(item))
                        PathCache.AddOrUpdate(item, item.Split('/'), (k, v) => item.Split('/'));
                }

                if (BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == connectionId) is BroadcastEndpoint endpoint)
                {
                    endpoint.Update(communicationMode, refreshRate, subscribeTagPaths);
                }
                else
                {
                    endpoint = new BroadcastEndpoint(this, Clients, connectionId, projectManagerService)
                    {
                        CommunicationMode = communicationMode,
                        RereshRate = refreshRate,
                        SubscribeTagPaths = subscribeTagPaths,
                    };
                    BroadcastEndpoints.Add(endpoint);
                }
            }
            catch { }
            finally { semaphore.Release(); }         
        }

        public void RemoveEndpoint(string connectionId)
        {
            semaphore.Wait();
            try
            {
                BroadcastEndpoint endpoint = BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == connectionId);
                if (endpoint != null)
                    BroadcastEndpoints.Remove(endpoint);
            }
            catch { }
            finally { semaphore.Release(); }
        }

        private BroadcastEndpoint GetEndpoint(string connectionId)
        {
            return BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == connectionId);
        }

        public void Dispose()
        {
            IsDisposed = true;
            semaphore.Wait();
            broadcastTask?.Dispose();
            semaphore.Release();
            semaphore.Dispose();
        }

        #endregion
    }

    public class BroadcastEndpoint
    {
        public List<string> SubscribeTagPaths { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public DateTime LastBroadcastTime { get; set; }
        public int RereshRate { get; set; }
        public IHubConnectionContext<dynamic> Clients { get; private set; }
        public string ConnectionId { get; private set; }
        public IProjectManagerService ProjectManagerService { get; private set; }
        public long BroadcastExecuteInterval { get; private set; }
        readonly Stopwatch sw;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        readonly ServerBroadcastService serverBroadcastService;
       
        public BroadcastEndpoint(
            ServerBroadcastService boardCastService,
            IHubConnectionContext<dynamic> clients, 
            string connectionId, 
            IProjectManagerService projectManagerService)
        {
            serverBroadcastService = boardCastService;
            Clients = clients;
            ConnectionId = connectionId;
            ProjectManagerService = projectManagerService;
            sw = new Stopwatch();
        }

        public void Send()
        {
            sw.Restart();
            try
            {
                semaphore.Wait();
                if (CommunicationMode == CommunicationMode.ReceiveFromServer)
                {
                    if (Math.Abs((DateTime.Now - LastBroadcastTime).TotalMilliseconds) > RereshRate)
                    {
                        Clients.Client(ConnectionId).broadcastSubscribeData(GetBroadcastResponse());
                        LastBroadcastTime = DateTime.Now;
                    }
                }
            }
            catch { }
            finally
            {
                sw.Stop();
                BroadcastExecuteInterval = sw.ElapsedMilliseconds;
                semaphore.Release();
            }
        }

        public string GetBroadcastResponse()
        {
            List<IClientTag> tags = new List<IClientTag>();
            if (ProjectManagerService != null && ProjectManagerService.CurrentProject != null)
            {
                Dictionary<string, List<string>> groupTagPathDic = new Dictionary<string, List<string>>();
                foreach (var item in SubscribeTagPaths)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] paths = serverBroadcastService.PathCache.GetOrAdd(item, (x) => x.Split('/'));
                        if (paths.Length >= 3)
                        {
                            string group = "";
                            for (int i = 0; i < paths.Length - 1; i++)
                            {
                                if (i == paths.Length - 2)
                                    group += paths[i];
                                else
                                    group += paths[i] + "/";
                            }
                            if (groupTagPathDic.ContainsKey(group))
                            {
                                groupTagPathDic[group].Add(paths[paths.Length - 1]);
                            }
                            else
                            {
                                groupTagPathDic[group] = new List<string>();
                                groupTagPathDic[group].Add(paths[paths.Length - 1]);
                            }
                        }
                    }
                }

                foreach (var kvp in groupTagPathDic)
                {
                    if (ProjectManagerService.CurrentProject.Browse(kvp.Key.Split('/'), 0) is IHaveTag haveTagObj)
                    {
                        if (haveTagObj.HaveTags)
                        {
                            foreach (var tagName in kvp.Value)
                            {
                                if (haveTagObj.Tags.Find(tagName) is IClientTag clientTag)
                                    tags.Add(clientTag);
                            }
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(tags, Formatting.Indented, new ListClientTagJsonConverter());
        }

        public void Update(CommunicationMode communicationMode, int refreshRate, List<string> subscribeTagPaths)
        {
            semaphore.Wait();
            CommunicationMode = communicationMode;
            RereshRate = refreshRate;
            SubscribeTagPaths = subscribeTagPaths;
            semaphore.Release();
        }
    }
}