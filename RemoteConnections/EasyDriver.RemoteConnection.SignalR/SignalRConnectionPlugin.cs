using DevExpress.Mvvm.POCO;
using EasyDriver.RemoteConnectionPlugin;
using EasyDriverPlugin;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.RemoteConnection.SignalR
{
    public class SignalRConnectionPlugin : EasyRemoteConnectionPluginBase
    {
        Task requestTask;
        HubConnection hubConnection;
        IHubProxy hubProxy;
        SignalRRemoteConnection remoteConnection;
        SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        bool isDisposed;
        string remoteAddress;
        ushort port;
        bool isStarted;
        bool isSubscribed;

        public SignalRConnectionPlugin() : base()
        {
        }

        public override void Start(IStationCore stationCore)
        {
            if (stationCore != null && stationCore is SignalRRemoteConnection)
            {
                this.remoteConnection = stationCore as SignalRRemoteConnection;
                remoteAddress = stationCore.RemoteAddress;
                port = stationCore.Port;
                InitializeConnection(hubConnection, hubProxy);
                requestTask = new Task(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning);
                requestTask.Start(TaskScheduler.Default);
                isStarted = true;
            }
        }

        public override void Stop()
        {
            isStarted = false;
        }

        public override void Dispose()
        {
            isDisposed = true;
        }

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
                    if (remoteConnection != null && remoteConnection.Parent.Childs.Contains(remoteConnection) && !isDisposed)
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

                        this.hubConnection = new HubConnection($"http://{remoteAddress}:{port}/easyScada");
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
            foreach (var item in remoteConnection.GetAllTags().ToArray())
            {
                subscribeTags.Add(item.Path.Remove(0, remoteConnection.Name.Length + 1));
            }
            string subscribeDataJson = JsonConvert.SerializeObject(subscribeTags);
            if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
            {
                var subTask = hubProxy.Invoke<string>("subscribe", subscribeDataJson, remoteConnection.CommunicationMode.ToString(), remoteConnection.RefreshRate);
                Task.WaitAll(subTask);
                if (subTask.IsCompleted)
                    return subTask.Result == "Ok";
            }
            return false;
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
            if (!isDisposed)
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
            remoteConnection.ConnectionStatus = (ConnectionStatus)Enum.Parse(typeof(ConnectionStatus), stateChanged.NewState.ToString());
            if (stateChanged.NewState == ConnectionState.Connected && !isDisposed)
            {
                // Delay a little bit before subscribe to server
                Thread.Sleep(1000);
                isSubscribed = false;
                // Initialize the request task if it null
                if (requestTask == null)
                    requestTask = Task.Factory.StartNew(RefreshData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else if (stateChanged.NewState == ConnectionState.Disconnected && !isDisposed)
            {
                if (remoteConnection != null)
                {
                    foreach (var item in remoteConnection.GetAllTags())
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
            while (!isDisposed)
            {
                int delay = 100;
                semaphore.Wait();
                if (isStarted)
                {
                    try
                    {
                        // Re subscribe if needed
                        if (!isSubscribed && hubConnection != null && hubConnection.State == ConnectionState.Connected)
                        {
                            isSubscribed = Subscribe();
                            delay = 100;
                        }

                        // Check requies condition to start get data from server
                        if (remoteConnection != null && !isDisposed && remoteConnection.Parent.Childs.Contains(remoteConnection))
                        {
                            if (remoteConnection.CommunicationMode == CommunicationMode.RequestToServer &&
                                remoteConnection.Childs != null && remoteConnection.Childs.Count > 0 &&
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
                                        var tags = remoteConnection.GetAllTags()?.ToList();
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
                                    var tags = remoteConnection.GetAllTags()?.ToList();
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
                                    remoteConnection.LastRefreshTime = DateTime.Now;
                                }
                                delay = remoteConnection.RefreshRate;
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
                    if (remoteConnection != null &&
                        remoteConnection.Parent.Childs.Contains(remoteConnection) &&
                        !isDisposed &&
                        remoteConnection.CommunicationMode == CommunicationMode.ReceiveFromServer)
                    {
                        try
                        {
                            List<IClientTag> clientObjects = JsonConvert.DeserializeObject<List<IClientTag>>(resJson, new ListClientTagJsonConverter());
                            Hashtable cache = new Hashtable();
                            clientObjects.ForEach(x => cache.Add(remoteConnection.Name + "/" + x.Path, x));
                            var tags = remoteConnection.GetAllTags()?.ToList();
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
                            var tags = remoteConnection.GetAllTags()?.ToList();
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
                        remoteConnection.LastRefreshTime = DateTime.Now;
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

        public override event EventHandler<CommandExecutedEventArgs> WriteCommandExecuted;

        public override IStationCore CreateRemoteConnection(IGroupItem parent)
        {
            return new SignalRRemoteConnection(parent);
        }

        public override async Task<WriteResponse> WriteAsync(WriteCommand cmd)
        {
            WriteResponse response = new WriteResponse();
            response.WriteCommand = cmd;
            if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
            {
                try
                {
                    var res = await hubProxy.Invoke<WriteResponse>("writeTagValueAsync", cmd);
                    response.Error = res.Error;
                    response.IsSuccess = res.IsSuccess;
                    WriteCommandExecuted?.Invoke(this, new CommandExecutedEventArgs(cmd, response));
                    return response;
                }
                catch (Exception ex)
                {
                    response.Error = $"Some error occur when send write command to remote station. {ex.Message}";
                    WriteCommandExecuted?.Invoke(this, new CommandExecutedEventArgs(cmd, response));
                    return response;
                }
                finally { }
            }
            else
            {
                response.Error = "The connection was disconnected or not initialized";
            }
            WriteCommandExecuted?.Invoke(this, new CommandExecutedEventArgs(cmd, response));
            return response;
        }

        public override object GetCreateRemoteConnectionView(IGroupItem parent)
        {
            CreateRemoteConnectionViewModel vm = ViewModelSource.Create(() => new CreateRemoteConnectionViewModel(parent));
            CreateRemoteConnectionView view = new CreateRemoteConnectionView() { DataContext = vm };
            return view;
        }

        public override object GetEditRemoteConnectionView(IStationCore stationCore)
        {
            if (!(stationCore is SignalRRemoteConnection))
                return null;
            EditRemoteConnectionViewModel vm = ViewModelSource.Create(() => new EditRemoteConnectionViewModel(stationCore as SignalRRemoteConnection));
            EditRemoteConnectionView view = new EditRemoteConnectionView() { DataContext = vm };
            return view;
        }
    }
}
