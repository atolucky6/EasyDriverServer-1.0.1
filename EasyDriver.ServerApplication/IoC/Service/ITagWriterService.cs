using EasyDriver.Core;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interface của dịch vụ ghi giá trị xuống tag
    /// </summary>
    public interface ITagWriterService
    {
        Task<WriteResponse> WriteTag(WriteCommand writeCommand);
        Task<List<WriteResponse>> WriteMultiTag(List<WriteCommand> writeCommands);
    }

    /// <summary>
    /// Dịch vụ ghi giá trị xuống Tag 
    /// </summary>
    public class TagWriterService : ITagWriterService
    {
        #region Constructors

        public TagWriterService(
            IProjectManagerService projectManagerService,
            IDriverManagerService driverManagerService,
            IRemoteConnectionManagerService remoteConnectionManagerService,
            IInternalStorageService internalStorageService)
        {
            ProjectManagerService = projectManagerService;
            DriverManagerService = driverManagerService;
            RemoteConnectionManagerService = remoteConnectionManagerService;
            InternalStorageService = internalStorageService;
        }

        #endregion

        #region Injected services

        protected IProjectManagerService ProjectManagerService { get; set; }
        protected IDriverManagerService DriverManagerService { get; set; }
        protected IRemoteConnectionManagerService RemoteConnectionManagerService { get; set; }
        protected IInternalStorageService InternalStorageService { get; set; }

        #endregion

        public async Task<WriteResponse> WriteTag(WriteCommand writeCommand)
        {
            // Gán thời gian nhận vào WriteCommand
            writeCommand.ReceiveTime = DateTime.Now;

            // Tạo kết quả trả về
            WriteResponse respone = new WriteResponse
            {
                ReceiveTime = DateTime.Now,
                IsSuccess = false,
                WriteCommand = writeCommand,
                Error = "Tag not found"
            };

            // Kiểm tra chương trình đã mở project
            if (ProjectManagerService.CurrentProject == null)
            {
                respone.IsSuccess = false;
                respone.WriteCommand = writeCommand;
                respone.Error = "The server doesn't open any project.";
            }
            // Nếu đã mở thì kiểm tra WriteCommand có hợp lệ hay không
            else
            {
                // Kiểm tra đường dẫn đến tag không trống
                if (!string.IsNullOrEmpty(writeCommand.PathToTag))
                {
                    // Kiểm tra xem tag ở local hay ở remote
                    if (writeCommand.PathToTag.StartsWith("LocalStation"))
                    {
                        string[] pathSplit = writeCommand.PathToTag.Split('/');
                        if (pathSplit != null && pathSplit.Length > 2)
                        {
                            // Lấy đường dẫn của đối tượng cha của tag
                            string[] parentPath = new string[pathSplit.Length - 1];
                            Array.Copy(pathSplit, 0, parentPath, 0, parentPath.Length);

                            // Tìm đối tượng cha của tag là IHaveTag
                            if (ProjectManagerService.CurrentProject.Browse(parentPath, 0) is IHaveTag parentOfTag)
                            {
                                // Lấy driver của tag
                                IEasyDriverPlugin driver = DriverManagerService.GetDriver(parentOfTag as IGroupItem);

                                // Kiểm tra driver của tag có tồn tại hay không
                                if (driver != null)
                                {
                                    // Nếu có driver thì tìm tag trong đối tượng cha của tag
                                    if (parentOfTag.Tags.Find(pathSplit[pathSplit.Length - 1]) is ITagCore tag)
                                    {
                                        // Kiểm tra xem tag có phải là internal tag hay không
                                        if (tag.IsInternalTag)
                                        {
                                            respone.IsSuccess = true;
                                            tag.Value = writeCommand.Value;
                                            respone.ExecuteTime = DateTime.Now;

                                            if (string.IsNullOrWhiteSpace(tag.GUID))
                                                tag.GUID = Guid.NewGuid().ToString();

                                            if (tag.Retain)
                                                InternalStorageService.AddOrUpdateInternalTag(tag);
                                        }
                                        else
                                        {
                                            // Tạo async delegate ghi tag
                                            WriteTagDelegate writeTagDelegate = new WriteTagDelegate(WriteTag);

                                            // Gọi hàm ghi tag async
                                            IAsyncResult result = writeTagDelegate.BeginInvoke(driver, writeCommand, null, null);

                                            // Đợi cho đến khi delegate ghi tag hoàn tất hoặc quá thời gian timeout
                                            result.AsyncWaitHandle.WaitOne(writeCommand.Timeout);

                                            // Lấy kết quả trả về từ delegate
                                            WriteResponse finalResult = writeTagDelegate.EndInvoke(result);

                                            // Tắt WaitHandle
                                            result.AsyncWaitHandle.Close();

                                            // Gán lại kết quả trả về 
                                            respone = finalResult;
                                        }
                                    }
                                    else
                                    {
                                        respone.Error = "Tag not found";
                                    }
                                }
                                // Nếu không tồn tại driver thì trả về kết quả luôn
                                else
                                {
                                    respone.IsSuccess = false;
                                    respone.WriteCommand = writeCommand;
                                    respone.Error = "Driver not found.";
                                }
                            }
                        }
                    }
                    // Nếu ở remote thì chuyển tiếp write command đến remote station đó
                    else
                    {
                        // Tách đường dẫn để lấy thông tin của remote station
                        string[] pathSplit = writeCommand.PathToTag.Split('/');
                        if (pathSplit != null && pathSplit.Length > 0)
                        {
                            // Tìm remote station thông qua tên
                            if (ProjectManagerService.CurrentProject.FirstOrDefault(x => x.Name == pathSplit[0]) is IStationCore stationCore)
                            {
                                if (RemoteConnectionManagerService.ConnectionDictonary.ContainsKey(stationCore))
                                {
                                    string oldPath = writeCommand.PathToTag;
                                    IRemoteConnection remoteConnection = RemoteConnectionManagerService.ConnectionDictonary[stationCore];
                                    writeCommand.PathToTag = writeCommand.PathToTag.Remove(0, pathSplit[0].Length + 1);
                                    respone = await remoteConnection.WriteTagValue(writeCommand, respone);
                                    respone.WriteCommand.PathToTag = oldPath;
                                }
                            }
                        }
                    }
                }
            }

            // Gán thời gian gửi xuống client
            respone.SendTime = DateTime.Now;

            // Trả về kết quảs
            return respone;
        }

        public WriteResponse WriteTag(IEasyDriverPlugin driver, WriteCommand cmd)
        {
            // Tạo lock để chờ cho đến khi write command được execute
            SemaphoreSlim waiter = new SemaphoreSlim(1, 1);
            // Bắt đầu chờ lần 1
            waiter.Wait();
            try
            {
                // Khởi tạo kết quả trả vể
                WriteResponse response = null;

                // Đăng ký sự kiện command executed để biết command được execute
                driver.WriteQueue.CommandExecuted += (s, e) =>
                {
                    // Nếu write command là command đang chờ thì thả lock và gán kết quả trả về
                    if (e.WriteCommandSource == cmd)
                    {
                        response = e.WriteResponse;
                        waiter.Release();
                    }
                };

                // Thêm command vào write queue của driver
                if (driver.WriteQueue.Add(cmd))
                {
                    // Nếu thêm thành công thì chờ lần 2
                    // chờ cho đến khi sử kiện CommandExecuted thực thi
                    waiter.Wait();
                    // Trả về kết quả
                    return response;
                }
                else
                {
                    // Nếu không thêm được thì trả về kết quả và lỗi
                    return new WriteResponse()
                    {
                        IsSuccess = false,
                        WriteCommand = cmd,
                        ReceiveTime = DateTime.Now,
                        Error = $"The command with tag path '{cmd.PathToTag}' is already exists."
                    };
                }
            }
            catch (Exception ex)
            {
                // Nếu có exeception thì trả về kết qua kèm với thông tin của exception
                return new WriteResponse()
                {
                    IsSuccess = false,
                    WriteCommand = cmd,
                    Error = $"Exception: {ex.Message}",
                    ReceiveTime = DateTime.Now,
                };
            }
            // Mở khóa lock
            finally { waiter.Release(); }
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

        public delegate WriteResponse WriteTagDelegate(IEasyDriverPlugin driver, WriteCommand cmd);
    }
}
