using EasyDriverPlugin;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using DevExpress.Mvvm.POCO;
using EasyDriver.Core;

namespace EasyScada.ServerApplication
{
    public class EasyDriverServerHub : Hub
    {
        #region Members

        private readonly IoC ioc;

        #endregion

        #region Constructors

        public EasyDriverServerHub() : this(IoC.Instance) { }

        public EasyDriverServerHub(IoC ioc) { this.ioc = ioc; }

        #endregion

        #region Authentication 

        public bool Login(string password)
        {
            return false;
        }

        public bool Logout()
        {
            return true;
        }

        #endregion

        #region Subscribe/Unsubscribe

        public string Subscribe(string tagsJson, string communicationMode, int refreshRate)
        {
            List<string> subscribeTags = JsonConvert.DeserializeObject<List<string>>(tagsJson);
            if (subscribeTags != null && subscribeTags.Count > 0)
            {
                if (Enum.TryParse(communicationMode, out CommunicationMode comMode))
                {
                    ioc.ServerBroadcastService.AddEndpoint(Context.ConnectionId, subscribeTags, comMode, refreshRate);
                    return "Ok";
                }
            }
            return "Fail";
        }
        public async Task<string> SubscribeAsync(string stationsJson, string communicationMode, int refreshRate)
        {
            return await Task.Run(() => Subscribe(stationsJson, communicationMode, refreshRate));
        }

        public string Unsubscribe()
        {
            ioc.ServerBroadcastService.RemoveEndpoint(Context.ConnectionId);
            return "Ok";
        }
        public async Task<string> UnsubscribeAsync()
        {
            return await Task.Run(() => Unsubscribe());
        }

        public string GetSubscribedData()
        {
            if (ioc.ServerBroadcastService.BroadcastEndpoints.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId) is BroadcastEndpoint endpoint)
            {
                return endpoint.GetBroadcastResponse();
            }
            return string.Empty;
        }
        public async Task<string> GetSubscribedDataAsync()
        {
            return await Task.Run(() => GetSubscribedData());
        }

        #endregion

        public string GetAllElements()
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                CoreItemToClientJsonObjectConverter converter = new CoreItemToClientJsonObjectConverter();

                return JsonConvert.SerializeObject(ioc.ProjectManagerService.CurrentProject, Formatting.Indented, converter);
            }
            return null; 
        }

        public async Task<string> GetAllElementsAsync()
        {
            return await Task.Run(() => GetAllElements());
        }

        #region Tag

        public string GetTag(string pathToTag)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                //if (ioc.ProjectManagerService.CurrentProject.BrowseTag(pathToTag.Split('/'), 0) is ITagCore tagClient)
                //{
                //    return JsonConvert.SerializeObject(tagClient as IClientObject);
                //}
            }
            return null;
        }
        public async Task<string> GetTagAsync(string pathToTag)
        {
            return await Task.Run(() => GetTag(pathToTag));
        }

        public string GetAllTags(string pathToParent)
        {
            if (ioc.ProjectManagerService.CurrentProject != null)
            {
                //if (ioc.ProjectManagerService.CurrentProject.BrowseTag(pathToParent.Split('/'), 0) is IGroupItem groupItem)
                //{
                //    List<IClientObject> tags = new List<IClientObject>();
                //    var foundTags = groupItem.Find(x => x is ITagCore, true);
                //    if (foundTags != null && foundTags.Count() > 0)
                //        tags.AddRange(foundTags.Select(x => x as IClientObject));
                //    return JsonConvert.SerializeObject(tags);
                //}   
            }
            return null;
        }
        public async Task<string> GetAllTagsAsync(string pathToDevice)
        {
            return await Task.Run(() => GetAllTags(pathToDevice));
        }

        #endregion

        #region Write
        public async Task<WriteResponse> WriteTagValueAsync(WriteCommand writeCommand)
        {
            return await ioc.Get<ITagWriterService>().WriteTag(writeCommand); 
        }

        public async Task<List<WriteResponse>> WriteMultiTagAsync(List<WriteCommand> writeCommands)
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            var result =  await ioc.Get<ITagWriterService>().WriteMultiTag(writeCommands);
            sw.Stop();
            Debug.WriteLine($"Write take time: {sw.ElapsedMilliseconds}");
            return result;
        }
        #endregion

        #region Lifecycle handler

        public override Task OnConnected()
        {
            if (!ioc.ApplicationViewModel.ConnectedClients.Contains(Context.ConnectionId))
            {
                ioc.ApplicationViewModel.ConnectedClients.Add(Context.ConnectionId);
                ioc.ApplicationViewModel.RaisePropertyChanged(x => x.TotalConnectedClients);
            }
            Debug.WriteLine($"Client {Context.ConnectionId} connected");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (ioc.ApplicationViewModel.ConnectedClients.Contains(Context.ConnectionId))
            {
                ioc.ApplicationViewModel.ConnectedClients.Remove(Context.ConnectionId);
                ioc.ApplicationViewModel.RaisePropertyChanged(x => x.TotalConnectedClients);
            }
            Debug.WriteLine($"Client {Context.ConnectionId} disconnected");
            ioc.ServerBroadcastService.RemoveEndpoint(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            if (ioc.ApplicationViewModel.ConnectedClients.Contains(Context.ConnectionId))
            {
                ioc.ApplicationViewModel.ConnectedClients.Remove(Context.ConnectionId);
                ioc.ApplicationViewModel.RaisePropertyChanged(x => x.TotalConnectedClients);
            }
            Debug.WriteLine($"Client {Context.ConnectionId} reconnected");
            return base.OnReconnected();
        }

        #endregion
    }
}
