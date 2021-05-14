using EasyScada.Core;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public static class ConnectorUtils
    {
        public async static Task<ConnectionSchema> GetConnectionSchema(string ipAddress, ushort port)
        {
            try
            {
                ConnectionSchema ConnectionSchema = null;
                using (HubConnection hubConnection = new HubConnection($"http://{ipAddress}:{port}/easyScada"))
                {
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    await hubConnection.Start();
                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        ConnectionSchema = new ConnectionSchema()
                        {
                            CreatedDate = DateTime.Now,
                            Port = port,
                            ServerAddress = ipAddress,
                        };

                        string resJson = await hubProxy.Invoke<string>("getAllElementsAsync");

                        List<ICoreItem> coreItems = new List<ICoreItem>();
                        if (JsonConvert.DeserializeObject(resJson) is JObject obj)
                        {
                            if (obj["Childs"] is JArray jArray)
                            {
                                foreach (var item in jArray)
                                {
                                    ICoreItem coreItem = JsonConvert.DeserializeObject<ICoreItem>(item.ToString(), new ConnectionSchemaJsonConverter());
                                    if (coreItem != null)
                                        coreItems.Add(coreItem);
                                }
                            }
                        }
                        if (coreItems != null)
                            coreItems.ForEach(x => ConnectionSchema.Childs.Add(x));
                    }
                }
                return ConnectionSchema;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task<ConnectionSchema> GetConnectionSchema(string url)
        {
            try
            {
                ConnectionSchema ConnectionSchema = null;
                using (HubConnection hubConnection = new HubConnection(url))
                {
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("EasyDriverServerHub");
                    await hubConnection.Start();
                    if (hubConnection.State == ConnectionState.Connected)
                    {
                        ConnectionSchema = new ConnectionSchema()
                        {
                            CreatedDate = DateTime.Now,
                            Url = url
                        };

                        string resJson = await hubProxy.Invoke<string>("getAllElementsAsync");

                        List<ICoreItem> coreItems = new List<ICoreItem>();
                        if (JsonConvert.DeserializeObject(resJson) is JObject obj)
                        {
                            if (obj["Childs"] is JArray jArray)
                            {
                                foreach (var item in jArray)
                                {
                                    ICoreItem coreItem = JsonConvert.DeserializeObject<ICoreItem>(item.ToString(), new ConnectionSchemaJsonConverter());
                                    if (coreItem != null)
                                        coreItems.Add(coreItem);
                                }
                            }
                        }
                        if (coreItems != null)
                            coreItems.ForEach(x => ConnectionSchema.Childs.Add(x));
                    }
                }
                return ConnectionSchema;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

    }
}
