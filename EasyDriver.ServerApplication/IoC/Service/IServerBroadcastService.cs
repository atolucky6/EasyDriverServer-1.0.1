using EasyDriver.Client.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface IServerBroadcastService
    {
        int BroadcastRate { get; set; }
        IHubConnectionContext<dynamic> Clients { get; }
        Dictionary<string, CommunicationMode> ClientToCommunicationMode { get; }
        Dictionary<string, List<Station>> ClientToSubscribedStations { get; }
    }

    public class ServerBroadcastService : IServerBroadcastService
    {
        readonly Timer broadcastTimer;
        readonly Stopwatch stopwatch;

        int broadcastRate = 1000;
        public int BroadcastRate
        {
            get => broadcastRate;
            set
            {
                broadcastRate = value;
                if (broadcastRate < 100)
                    broadcastRate = 100;
            }
        }

        public IHubConnectionContext<dynamic> Clients { get; private set; }

        public Dictionary<string, CommunicationMode> ClientToCommunicationMode { get; private set; }

        public Dictionary<string, List<Station>> ClientToSubscribedStations { get; private set; }

        public ServerBroadcastService()
        {
            Clients = GlobalHost.ConnectionManager.GetHubContext<EasyDriverServerHub>().Clients;
            ClientToCommunicationMode = new Dictionary<string, CommunicationMode>();
            ClientToSubscribedStations = new Dictionary<string, List<Station>>();
            stopwatch = new Stopwatch();
            broadcastTimer = new Timer(new TimerCallback(BroadcastCallback), Clients, 0, broadcastRate);
        }
        
        private async void BroadcastCallback(object state)
        {
            stopwatch.Restart();
            broadcastTimer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (state is IHubConnectionContext<dynamic> clients)
                {
                    List<string> clientsWillReceived = new List<string>();
                    foreach (var kvp in ClientToCommunicationMode)
                    {
                        if (kvp.Value == CommunicationMode.ReceiveFromServer)
                            clientsWillReceived.Add(kvp.Key);
                    }

                    string stationsJson = await GetAllStationsAsync();
                    Clients.Clients(clientsWillReceived).broadcastStations(stationsJson);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                stopwatch.Stop();
                long delay = BroadcastRate - stopwatch.ElapsedMilliseconds;
                broadcastTimer.Change(delay < 0 ? 0 : delay, 0);
            }
        }

        private string GetAllStations()
        {
            if (IoC.Instance.ProjectManagerService.CurrentProject != null)
            {
                var result = IoC.Instance.ProjectManagerService.CurrentProject.Childs?.Select(x => x as IStation)?.ToList();
                return JsonConvert.SerializeObject(result, Formatting.Indented);
            }
            return null;
        }

        private async Task<string> GetAllStationsAsync()
        {
            return await Task.Run(() => GetAllStations());
        }

        private async Task<List<Station>> GetListStationsAsync()
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<List<Station>>(GetAllStations()));
        }
    }
}
