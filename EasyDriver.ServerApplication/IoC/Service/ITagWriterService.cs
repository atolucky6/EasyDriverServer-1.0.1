using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public interface ITagWriterService
    {
        Task<WriteResponse> WriteTag(WriteCommand writeCommand);
        Task<List<WriteResponse>> WriteMultiTag(List<WriteCommand> writeCommands);
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
            WriteResponse respone = new WriteResponse
            {
                ReceiveTime = DateTime.Now,
                IsSuccess = false,
                WriteCommand = writeCommand,
                Error = "The tag not found"
            };

            if (ProjectManagerService.CurrentProject == null)
            {
                respone.IsSuccess = false;
                respone.WriteCommand = writeCommand;
                respone.Error = "The server doesn't open project.";
                respone.ExecuteTime = DateTime.Now;
            }
            else
            {
                if (!string.IsNullOrEmpty(writeCommand.PathToTag))
                {
                    if (writeCommand.PathToTag.StartsWith("Local Station"))
                    {
                        string[] pathSplit = writeCommand.PathToTag.Split('/');
                        if (pathSplit != null && pathSplit.Length > 2)
                        {
                            string[] parentPath = new string[pathSplit.Length - 1];
                            Array.Copy(pathSplit, 0, parentPath, 0, parentPath.Length);
                            if (ProjectManagerService.CurrentProject.Browse(parentPath, 0) is IHaveTag parentOfTag)
                            {
                                IEasyDriverPlugin driver = DriverManagerService.GetDriver(parentOfTag as IGroupItem);

                                if (driver != null)
                                {
                                    ITagCore tag = parentOfTag.Tags.Find(pathSplit[parentPath.Length - 1]);
                                    if (tag != null)
                                    {
                                        respone.ExecuteTime = DateTime.Now;
                                        Quality writeQuality = driver.Write(tag, writeCommand.Value);
                                        respone.WriteCommand = writeCommand;
                                        respone.IsSuccess = writeQuality == Quality.Good;
                                    }
                                }
                                else
                                {
                                    respone.IsSuccess = false;
                                    respone.WriteCommand = writeCommand;
                                    respone.Error = "Can't found driver of the tag";
                                }
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
                                if (stationCore.StationType == "Remote")
                                {
                                    if (HubConnectionManagerService.ConnectionDictonary.ContainsKey(stationCore))
                                    {
                                        string oldPath = writeCommand.PathToTag;
                                        RemoteStationConnection remoteConnection = HubConnectionManagerService.ConnectionDictonary[stationCore];
                                        writeCommand.PathToTag = writeCommand.PathToTag.Remove(0, pathSplit[0].Length + 1);
                                        respone.ExecuteTime = DateTime.Now;
                                        respone = await remoteConnection.WriteTagValue(writeCommand, respone);
                                        respone.WriteCommand.PathToTag = oldPath;
                                    }
                                }
                                else if (stationCore.StationType == "OPC_DA")
                                {
                                    if (OpcDaClientManagerService.ConnectionDictonary.ContainsKey(stationCore))
                                    {
                                        string oldPath = writeCommand.PathToTag;
                                        OpcDaRemoteStationConnection remoteConnection = OpcDaClientManagerService.ConnectionDictonary[stationCore];
                                        writeCommand.PathToTag = writeCommand.PathToTag.Remove(0, pathSplit[0].Length + 1);
                                        respone.ExecuteTime = DateTime.Now;
                                        respone = await remoteConnection.WriteTagValue(writeCommand, respone);
                                        respone.WriteCommand.PathToTag = oldPath;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            respone.SendTime = DateTime.Now;
            return respone;
        }

        public async Task<List<WriteResponse>> WriteMultiTag(List<WriteCommand> writeCommands)
        {
            List<WriteResponse> writeResponses = new List<WriteResponse>();
            for (int i = 0; i < writeCommands.Count; i++)
            {
                writeResponses.Add(await WriteTag(writeCommands[i]));
            }
            return writeResponses;
        }
    }
}
