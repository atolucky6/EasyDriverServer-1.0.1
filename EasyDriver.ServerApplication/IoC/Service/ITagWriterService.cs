using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface ITagWriterService
    {
        Task<WriteResponse> WriteTag(WriteCommand writeCommand);
    }
    public class TagWriterService : ITagWriterService
    {
        #region Constructors

        public TagWriterService(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IHubConnectionManagerService hubConnectionManagerService,
            IOpcDaClientManagerService opcDaClientManagerService)
        {
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            HubConnectionManagerService = hubConnectionManagerService;
            OpcDaClientManagerService = opcDaClientManagerService;
        }

        #endregion

        #region Injected services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IHubConnectionManagerService HubConnectionManagerService { get; set; }
        protected IOpcDaClientManagerService OpcDaClientManagerService { get; set; }

        #endregion

        public async Task<WriteResponse> WriteTag(WriteCommand writeCommand)
        {
            WriteResponse respone = new WriteResponse();
            respone.IsSuccess = false;
            respone.WriteCommand = writeCommand;
            respone.Error = "The tag not found";

            if (ProjectManagerService.CurrentProject == null)
            {
                respone.IsSuccess = false;
                respone.WriteCommand = writeCommand;
                respone.Error = "The server doesn't open project.";
            }
            else
            {
                if (writeCommand.PathToTag.StartsWith("Local Station"))
                {
                    if (ProjectManagerService.CurrentProject.GetItem<ITagClient>(writeCommand.PathToTag) is ITagCore tagCore)
                    {
                        var driver = DriverManagerService.GetDriver(tagCore);
                        if (driver != null)
                        {
                            Quality writeQuality = driver.Write(tagCore, writeCommand.Value);
                            respone.ExecuteTime = DateTime.Now;
                            respone.WriteCommand = writeCommand;
                            respone.IsSuccess = writeQuality == Quality.Good;
                        }
                        else
                        {
                            respone.IsSuccess = false;
                            respone.WriteCommand = writeCommand;
                            respone.Error = "Can't found driver of the tag";
                        }
                    }
                }
                else
                {
                    string[] pathSplit = writeCommand.PathToTag.Split('/');
                    if (pathSplit != null && pathSplit.Length > 0)
                    {
                        if (ProjectManagerService.CurrentProject.FirstOrDefault(x => x.Name == pathSplit[0]) is IStationCore stationCore)
                        {
                            if (stationCore.StationType == StationType.Remote)
                            {
                                if (HubConnectionManagerService.ConnectionDictonary.ContainsKey(stationCore))
                                {
                                    string oldPath = writeCommand.PathToTag;
                                    RemoteStationConnection remoteConnection = HubConnectionManagerService.ConnectionDictonary[stationCore];
                                    writeCommand.PathToTag = writeCommand.PathToTag.Remove(0, pathSplit[0].Length + 1);
                                    respone = await remoteConnection.WriteTagValue(writeCommand);
                                    respone.WriteCommand.PathToTag = oldPath;
                                }
                            }
                            else if (stationCore.StationType == StationType.OPC_DA)
                            {
                                if (OpcDaClientManagerService.ConnectionDictonary.ContainsKey(stationCore))
                                {
                                    string oldPath = writeCommand.PathToTag;
                                    OpcDaRemoteStationConnection remoteConnection = OpcDaClientManagerService.ConnectionDictonary[stationCore];
                                    writeCommand.PathToTag = writeCommand.PathToTag.Remove(0, pathSplit[0].Length + 1);
                                    respone = await remoteConnection.WriteTagValue(writeCommand);
                                    respone.WriteCommand.PathToTag = oldPath;
                                }
                            }
                        }
                    }
                }
            }
            respone.SendTime = DateTime.Now;
            return respone;
        }
    }
}
