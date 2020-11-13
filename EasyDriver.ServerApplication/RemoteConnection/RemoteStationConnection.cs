using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Client.Transports;
using System.Collections;

namespace EasyScada.ServerApplication
{
    public class RemoteStationConnection : IRemoteConnection
    {
        #region Members

        public IStationCore Station { get; set; }
        public string RemoteAddress { get; private set; }
        public ushort Port { get; private set; }
        public bool IsDisposed { get; private set; }
        public bool IsSubscribed { get; set; }
        public HubConnection hubConnection;
        public IHubProxy hubProxy;

        private Task requestTask;
        readonly Hashtable cache;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructors

        public RemoteStationConnection(IStationCore stationCore, HubConnection hubConnection = null, IHubProxy hubPorxy = null)
        {
            Station = stationCore;
            RemoteAddress = Station.RemoteAddress;
            Port = Station.Port;
            cache = new Hashtable();
            InitializeConnection(hubConnection, hubPorxy);
        }

        ~RemoteStationConnection()
        {
        }

        #endregion

        #region Methods

        private async void InitializeConnection(HubConnection hubConnection = null, IHubProxy hubPorxy = null)
        {
            try
            {
                if (hubConnection != null && hubPorxy != null && hubConnection.State == ConnectionState.Connected)
                {
                    if (this.hubConnection == null)
                    {
                        this.hubConnection = hubConnection;
                        hubProxy = hubPorxy;
                        this.hubConnection.StateChanged += OnStateChanged;
                        this.hubConnection.Closed += OnDisconnected;
                        this.hubConnection.ConnectionSlow += OnConnectionSlow;

                        hubProxy.On("broadcastSubscribeData", (Action<string>)OnReceivedBroadcastMessage);
                        OnStateChanged(new StateChange(ConnectionState.Connecting, this.hubConnection.State));
                    }
                }
                else
                {
                    if (Station != null && Station.Parent.Childs.Contains(Station) && !IsDisposed)
                    {
                        if (this.hubConnection != null)
                        {
                            try
                            {
                                this.hubConnection.StateChanged -= OnStateChanged;
                                this.hubConnection.Closed -= OnDisconnected;
                                this.hubConnection.ConnectionSlow -= OnConnectionSlow;
                                if (this.hubConnection.State == ConnectionState.Connected)
                                    this.hubConnection.Stop();
                                this.hubConnection?.Dispose();
                            }
                            catch { }
                        }

                        this.hubConnection = new HubConnection($"http://{RemoteAddress}:{Port}/easyScada");
                        this.hubConnection.StateChanged += OnStateChanged;
                        this.hubConnection.Closed += OnDisconnected;
                        this.hubConnection.ConnectionSlow += OnConnectionSlow;
                        hubProxy = this.hubConnection.CreateHubProxy("EasyDriverServerHub");
                        hubProxy.On<string>("broadcastSubscribeData", OnReceivedBroadcastMessage);
                        await this.hubConnection.Start(new LongPollingTransport());
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Method to subscribe this connection to server
        /// </summary>
        /// <returns></returns>
        private bool Subscribe()
        {
            List<string> subscribeTags = new List<string>();
            foreach (var item in Station.GetAllTags())
            {
                subscribeTags.Add(item.Path.Remove(0, Station.Name.Length + 1));
            }
            string subscribeDataJson = JsonConvert.SerializeObject(subscribeTags);
            if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
            {
                var subTask = hubProxy.Invoke<string>("subscribe", subscribeDataJson, Station.CommunicationMode.ToString(), Station.RefreshRate);
                Task.WaitAll(subTask);
                if (subTask.IsCompleted)
                    return subTask.Result == "Ok";
            }
            return false;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public async Task<WriteResponse> WriteTagValue(WriteCommand writeCommand, WriteResponse response)
        {
            response.WriteCommand = writeCommand;
            if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
            {
                try
                {
                    await semaphore.WaitAsync();
                    var res = await hubProxy.Invoke<WriteResponse>("writeTagValueAsync", writeCommand);
                    response.Error = res.Error;
                    response.IsSuccess = res.IsSuccess;
                    return response;
                }
                catch (Exception)
                {
                    response.Error = "Some error occur when send write command to remote station.";
                    return response;
                }
                finally { semaphore.Release(); }
            }
            else
            {
                response.Error = "The connection state of remote station was disconnected";
            }
            return response;
        }

        #endregion

        #region Connection lifecycle handlers

        private void OnConnectionSlow()
        {

        }

        /// <summary>
        /// The method handler when hub connection is closed
        /// </summary>
        private void OnDisconnected()
        {
            if (!IsDisposed)
            {
                // Delay a little bit before reconnect
                Thread.Sleep(5000);
                // When connection disconnected we will init the new connection and tring to reconnect
                InitializeConnection();
            }
        }

        /// <summary>
        /// The method handler when state of hub connection is changed
        /// </summary>
        /// <param name="stateChanged">A change state</param>
        public void OnStateChanged(StateChange stateChanged)
        {
            Station.ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), stateChanged.NewState.ToString());
            if (stateChanged.NewState == ConnectionState.Connected && !IsDisposed)
            {
                // Delay a little bit before subscribe to server
                Thread.Sleep(1000);
                IsSubscribed = false;
                // Initialize the request task if it null
                if (requestTask == null)
                    requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else if (stateChanged.NewState == ConnectionState.Disconnected && !IsDisposed)
            {
                if (Station != null)
                {
                    foreach (var item in Station.GetAllTags())
                    {
                        if (item is ITagCore tagCore)
                            tagCore.Quality = Quality.Bad;
                    }
                }
            }
        }

        #endregion

        #region Request methods

        /// <summary>
        /// The method callback of refresh timer
        /// </summary>
        /// <param name="state"></param>
        private void RefreshData()
        {
            while (!IsDisposed)
            {
                int delay = 100;
                semaphore.Wait();
                try
                {
                    // Re subscribe if needed
                    if (!IsSubscribed && hubConnection != null && hubConnection.State == ConnectionState.Connected)
                    {
                        IsSubscribed = Subscribe();
                        delay = 100;
                    }

                    // Check requies condition to start get data from server
                    if (Station != null && !IsDisposed && Station.Parent.Childs.Contains(Station))
                    {
                        if (Station.CommunicationMode == CommunicationMode.RequestToServer &&
                            Station.Childs != null && Station.Childs.Count > 0 &&
                            hubConnection.State == ConnectionState.Connected)
                        {
                            try
                            {
                                // Get subscribed data from server
                                var getDataTask = hubProxy.Invoke<string>("getSubscribedDataAsync");
                                Task.WaitAll(getDataTask);
                                if (getDataTask.IsCompleted)
                                {
                                    string resJson = getDataTask.Result;
                                    List<IClientTag> clientObjects = JsonConvert.DeserializeObject<List<IClientTag>>(resJson);
                                    Hashtable cache = new Hashtable();
                                    clientObjects.ForEach(x => cache.Add(x.Path, x));
                                    var tags = Station.GetAllTags()?.ToList();
                                    if (tags != null)
                                    {
                                        tags.ForEach(x =>
                                        {
                                            if (cache.ContainsKey(x.Parent))
                                            {
                                                IClientTag clientTag = cache[x.Path] as IClientTag;
                                                x.Value = clientTag.Value;
                                                x.Quality = clientTag.Quality;
                                                x.TimeStamp = clientTag.TimeStamp;
                                                x.CommunicationError = clientTag.Error;
                                            }
                                            else
                                            {
                                                x.Quality = Quality.Bad;
                                                x.CommunicationError = "Object not found in server.";
                                                x.TimeStamp = DateTime.Now;
                                            }
                                        });
                                    }
                                }
                            }
                            catch
                            {
                                var tags = Station.GetAllTags()?.ToList();
                                if (tags != null)
                                {
                                    tags.ForEach(x =>
                                    {
                                        x.Quality = Quality.Bad;
                                        x.CommunicationError = "Some errors occur when get value from server.";
                                        x.TimeStamp = DateTime.Now;
                                    });
                                }
                            }
                            finally
                            {
                                Station.LastRefreshTime = DateTime.Now;
                            }
                            delay = Station.RefreshRate;
                        }
                        else { delay = 100; }
                    }
                }

                catch { }
                finally
                {
                    semaphore.Release();
                    Thread.Sleep(delay);
                }
            }

            hubConnection?.Dispose();
            semaphore.Dispose();
        }

        #endregion

        #region On message handlers

        /// <summary>
        /// The handler when the connection get broadcast message from server
        /// </summary>
        /// <param name="stationsJson"></param>
        private async void OnReceivedBroadcastMessage(string resJson)
        {
            await Task.Run(async () =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                await semaphore.WaitAsync();
                try
                {
                    // Check requires conditions 
                    if (Station != null &&
                        Station.Parent.Childs.Contains(Station) &&
                        !IsDisposed &&
                        Station.CommunicationMode == CommunicationMode.ReceiveFromServer)
                    {
                        try
                        {
                            List<IClientTag> clientObjects = JsonConvert.DeserializeObject<List<IClientTag>>(resJson, new ListClientTagJsonConverter());
                            Hashtable cache = new Hashtable();
                            clientObjects.ForEach(x => cache.Add(Station.Name + "/" + x.Path, x));
                            var tags = Station.GetAllTags()?.ToList();
                            if (tags != null)
                            {
                                tags.ForEach(x =>
                                {
                                    if (cache.ContainsKey(x.Path))
                                    {
                                        IClientTag clientTag = cache[x.Path] as IClientTag;
                                        x.Value = clientTag.Value;
                                        x.Quality = clientTag.Quality;
                                        x.TimeStamp = clientTag.TimeStamp;
                                        x.CommunicationError = clientTag.Error;
                                    }
                                    else
                                    {
                                        x.Quality = Quality.Bad;
                                        x.CommunicationError = "Object not found in server.";
                                        x.TimeStamp = DateTime.Now;
                                    }
                                });
                            }
                        }
                        catch
                        {
                            var tags = Station.GetAllTags()?.ToList();
                            if (tags != null)
                            {
                                tags.ForEach(x =>
                                {
                                    x.Quality = Quality.Bad;
                                    x.CommunicationError = "Some errors occur when get value from server.";
                                    x.TimeStamp = DateTime.Now;
                                });
                            }
                        }
                        finally { }
                        Station.LastRefreshTime = DateTime.Now;
                    }
                }
                catch (Exception) { }
                finally
                {
                    semaphore.Release();
                    sw.Stop();
                    Debug.WriteLine($"Handle broadcast message: {sw.ElapsedMilliseconds}");
                }
            });
        }

        #endregion
    }
}
