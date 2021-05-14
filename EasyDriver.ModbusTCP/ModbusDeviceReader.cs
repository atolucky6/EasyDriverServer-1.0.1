using EasyDriverPlugin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyDriver.ModbusTCP
{
    //class ModbusDeviceReader : IDisposable
    //{
    //    public ModbusDeviceReader(ModbusTCPDriver driver, Device device)
    //    {
    //        this.driver = driver;
    //        Device = device;
    //        locker = new SemaphoreSlim(1, 1);
    //        stopwatch = new Stopwatch();
    //        DefaultPiorityWriteCommands = new ConcurrentDictionary<WriteCommand, WriteCommand>();
    //        driver.WriteQueue.Enqueued += OnWriteQueueAdded;
    //        refreshTask = Task.Factory.StartNew(Refresh, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    //        device.ParameterContainer.ParameterChanged += OnDeviceParameterChanged;

    //    }

    //    public Channel Channel => driver.Channel;
    //    public Device Device { get; private set; }
    //    private ModbusTCPClient mbClient;
    //    readonly Task refreshTask;
    //    readonly SemaphoreSlim locker;
    //    readonly Stopwatch stopwatch;
    //    readonly ModbusTCPDriver driver;
    //    public ConcurrentDictionary<WriteCommand, WriteCommand> DefaultPiorityWriteCommands { get; set; }
    //    WriteQueue WriteQueue => driver.WriteQueue;

    //    private void OnDeviceParameterChanged(object sender, ParameterChangedEventArgs e)
    //    {
    //        if (e.KeyValue.Key == "Port" || e.KeyValue.Key == "IpAddress")
    //        {
    //            locker.Wait();
    //            try
    //            {
    //                if (mbClient != null)
    //                    mbClient.Dispose();
    //                mbClient = new ModbusTCPClient();
    //                mbClient.Connect(Device.IpAddress, Device.Port, false);
    //            }
    //            catch { }
    //            finally { locker.Release(); }
    //        }
    //    }

    //    /// <summary>
    //    /// Bit xác định đối tượng này đã bị dispose hay chưa
    //    /// </summary>
    //    public bool IsDisposed { get; private set; }

    //    /// <summary>
    //    /// Hàm thực hiện việc đọc các thiết bị liên tục
    //    /// </summary>
    //    private void Refresh()
    //    {
    //        // Đảm bảo driver này chưa bị dispose
    //        while (!IsDisposed)
    //        {
    //            try
    //            {
    //                // Khởi động lại đồng hồ
    //                stopwatch.Restart();

    //                if (mbClient == null)
    //                {
    //                    InitMBClient();
    //                }

    //                if (!mbClient.Connected)
    //                {
    //                    try
    //                    {
    //                        locker.Wait();
    //                        mbClient.Connect(Device.IpAddress, Device.Port, false);
    //                    }
    //                    catch { }
    //                    finally { locker.Release(); }
    //                }

    //                if (mbClient.IsSocketNull)
    //                {
    //                    try
    //                    {
    //                        InitMBClient();
    //                    }
    //                    catch { }
    //                }

    //                if (mbClient != null)
    //                {
    //                    if (Device.Enabled)
    //                    {
    //                        if (mbClient.Connected)
    //                        {
    //                            byte unitId = (byte)Device.UnitId;
    //                            ByteOrder byteOrder = Device.ByteOrder;
    //                            // Lấy số lần thử đọc tối đa nếu đọc không thành công
    //                            int maxTryTimes = Device.TryReadWriteTimes;
    //                            // Thời gian timeout của việc đọc ghi
    //                            int timeout = Device.Timeout;

    //                            // Set lại thời gian timeout
    //                            mbClient.SetTimeout(timeout);

    //                            #region READ BLOCK FIRST

    //                            foreach (ReadBlockSetting block in Device.GetAllReadBlockSettings())
    //                            {
    //                                block.ReadResult = false;
    //                                if (block.IsValid && block.Enabled && block.AddressType != AddressType.Undefined)
    //                                {
    //                                    if (block.RegisterTags.Count > 0)
    //                                    {
    //                                        int readCount = 0;
    //                                        while (!block.ReadResult)
    //                                        {
    //                                            switch (block.AddressType)
    //                                            {
    //                                                case AddressType.InputContact:
    //                                                case AddressType.OutputCoil:
    //                                                    block.ReadResult = ReadBits(unitId, block.AddressType, block.StartOffset, block.BufferCount, ref block.BoolBuffer);
    //                                                    break;
    //                                                case AddressType.InputRegister:
    //                                                case AddressType.HoldingRegister:
    //                                                    block.ReadResult = ReadRegisters(unitId, block.AddressType, block.StartOffset, block.BufferCount, ref block.ByteBuffer);
    //                                                    break;
    //                                                default:
    //                                                    break;
    //                                            }
    //                                            readCount++;
    //                                            if (readCount >= Device.TryReadWriteTimes)
    //                                                break;
    //                                        }

    //                                        if (block.ReadResult)
    //                                        {
    //                                            switch (block.AddressType)
    //                                            {
    //                                                case AddressType.OutputCoil:
    //                                                case AddressType.InputContact:
    //                                                    {
    //                                                        foreach (var tag in block.RegisterTags.ToArray())
    //                                                        {
    //                                                            if (tag.Enabled)
    //                                                            {
    //                                                                tag.Quality = Quality.Good;
    //                                                                tag.TimeStamp = DateTime.Now;
    //                                                                tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                                                                tag.Value = block.BoolBuffer[tag.IndexOfDataInBlockSetting] ? (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
    //                                                            }
    //                                                        }
    //                                                        break;
    //                                                    }
    //                                                case AddressType.InputRegister:
    //                                                case AddressType.HoldingRegister:
    //                                                    {
    //                                                        foreach (var tag in block.RegisterTags.ToArray())
    //                                                        {
    //                                                            if (tag.Enabled)
    //                                                            {
    //                                                                tag.Quality = Quality.Good;
    //                                                                tag.TimeStamp = DateTime.Now;
    //                                                                tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                                                                tag.Value = tag.DataType.ConvertToValue(block.ByteBuffer, tag.Gain, tag.Offset, tag.IndexOfDataInBlockSetting, 0, Device.ByteOrder);
    //                                                            }
    //                                                        }
    //                                                        break;
    //                                                    }
    //                                                default:
    //                                                    break;
    //                                            }
    //                                        }
    //                                        else
    //                                        {
    //                                            foreach (var tag in block.RegisterTags.ToArray())
    //                                            {
    //                                                if (tag.Enabled)
    //                                                {
    //                                                    tag.Quality = Quality.Bad;
    //                                                    tag.TimeStamp = DateTime.Now;
    //                                                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                                                }
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }

    //                            #endregion

    //                            foreach (var childDevice in Device.UndefinedTags.ToArray())
    //                            {
    //                                // Đảm bảo tag phải là modbus tcp tag
    //                                if (!(childDevice is Tag tag))
    //                                    continue;

    //                                // Bỏ qua nếu tag là một internal tag hoặc không enabled
    //                                if (tag.IsInternalTag)
    //                                {
    //                                    tag.Quality = Quality.Good;
    //                                    tag.TimeStamp = DateTime.Now;
    //                                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                                    continue;
    //                                }

    //                                if (!tag.Enabled)
    //                                    continue;

    //                                // Nếu tag có kiểu địa chỉ xác định thì đọc nếu không thì set Quality thành Bad
    //                                if (tag.AddressType != AddressType.Undefined)
    //                                {
    //                                    if ((DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
    //                                    {
    //                                        bool readSuccess = false;
    //                                        switch (tag.AddressType)
    //                                        {
    //                                            case AddressType.InputContact:
    //                                                {
    //                                                    bool[] inputs = new bool[1];
    //                                                    readSuccess = ReadBits(
    //                                                        unitId,
    //                                                        AddressType.InputContact,
    //                                                        (ushort)tag.AddressOffset,
    //                                                        1, ref inputs);
    //                                                    if (readSuccess)
    //                                                        tag.Value = inputs[0] ?
    //                                                            (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
    //                                                    break;
    //                                                }
    //                                            case AddressType.OutputCoil:
    //                                                {
    //                                                    bool[] outputs = new bool[1];
    //                                                    readSuccess = ReadBits(
    //                                                        unitId,
    //                                                        AddressType.OutputCoil,
    //                                                        (ushort)tag.AddressOffset,
    //                                                        1, ref outputs);
    //                                                    if (readSuccess)
    //                                                        tag.Value = outputs[0] ?
    //                                                            (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
    //                                                    break;
    //                                                }
    //                                            case AddressType.InputRegister:
    //                                                {
    //                                                    byte[] inputRegs = new byte[tag.RequireByteLength];
    //                                                    readSuccess = ReadRegisters(
    //                                                        unitId,
    //                                                        AddressType.InputRegister,
    //                                                        (ushort)tag.AddressOffset,
    //                                                        (ushort)(tag.RequireByteLength / 2),
    //                                                        ref inputRegs);
    //                                                    if (readSuccess)
    //                                                        tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, Device.ByteOrder);
    //                                                    break;
    //                                                }
    //                                            case AddressType.HoldingRegister:
    //                                                {
    //                                                    byte[] holdingRegs = new byte[tag.RequireByteLength];
    //                                                    readSuccess = ReadRegisters(
    //                                                        unitId,
    //                                                        AddressType.HoldingRegister,
    //                                                        (ushort)tag.AddressOffset,
    //                                                        (ushort)(tag.RequireByteLength / 2),
    //                                                        ref holdingRegs);
    //                                                    if (readSuccess)
    //                                                        tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, Device.ByteOrder);
    //                                                    break;
    //                                                }
    //                                            default:
    //                                                break;
    //                                        }

    //                                        tag.Quality = readSuccess ? Quality.Good : Quality.Bad;
    //                                        tag.TimeStamp = DateTime.Now;
    //                                        tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    tag.TimeStamp = DateTime.Now;
    //                                    tag.Quality = Quality.Bad;
    //                                    tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                                }
    //                            }

    //                            if (mbClient.IsSocketNull)
    //                            {
    //                                SetAllTagBad();
    //                            }

    //                            // Thực hiện lệnh ghi tag
    //                            for (int i = 0; i < Device.MaxWritesCount; i++)
    //                            {
    //                                if (DefaultPiorityWriteCommands.Count > 0)
    //                                {
    //                                    var cmd = DefaultPiorityWriteCommands.Keys.FirstOrDefault();
    //                                    DefaultPiorityWriteCommands.TryRemove(cmd, out WriteCommand removeCmd);

    //                                    if (cmd != null)
    //                                    {
    //                                        WriteResponse response = ExecuteWriteCommand(cmd);
    //                                        CommandExecutedEventArgs args = new CommandExecutedEventArgs(cmd, response);
    //                                        cmd.OnExecuted(args);
    //                                    }
    //                                }
    //                                else { break; }
    //                            }

    //                            //#region READ BLOCK FIRST

    //                            //InitReadBlockSetting(Device, "ReadInputContactsBlockSetting", AddressType.InputContact);
    //                            //InitReadBlockSetting(Device, "ReadOutputCoilsBlockSetting", AddressType.OutputCoil);
    //                            //InitReadBlockSetting(Device, "ReadInputRegistersBlockSetting", AddressType.InputRegister);
    //                            //InitReadBlockSetting(Device, "ReadHoldingRegistersBlockSetting", AddressType.HoldingRegister);

    //                            //foreach (ReadBlockSetting block in ReadBlockSettings)
    //                            //{
    //                            //    block.ReadResult = false;
    //                            //    int readCount = 0;
    //                            //    while (!block.ReadResult)
    //                            //    {
    //                            //        switch (block.AddressType)
    //                            //        {
    //                            //            case AddressType.InputContact:
    //                            //            case AddressType.OutputCoil:
    //                            //                block.ReadResult = ReadBits(unitId, block.AddressType, block.StartOffset, block.Count, ref block.BoolBuffer);
    //                            //                break;
    //                            //            case AddressType.InputRegister:
    //                            //            case AddressType.HoldingRegister:
    //                            //                block.ReadResult = ReadRegisters(unitId, block.AddressType, block.StartOffset, block.Count, ref block.ByteBuffer);
    //                            //                break;
    //                            //            default:
    //                            //                break;
    //                            //        }
    //                            //        readCount++;
    //                            //        if (readCount >= maxTryTimes)
    //                            //            break;
    //                            //    }
    //                            //}

    //                            //#endregion

    //                            //foreach (var childDevice in Device.GetAllTags().ToArray())
    //                            //{
    //                            //    if (!(childDevice is ITagCore tag))
    //                            //        continue;

    //                            //    if (tag.IsInternalTag)
    //                            //    {
    //                            //        tag.Quality = Quality.Good;
    //                            //        tag.TimeStamp = DateTime.Now;
    //                            //        continue;
    //                            //    }

    //                            //    if (mbClient.IsSocketNull)
    //                            //        break;

    //                            //    if (!tag.ParameterContainer.Parameters.ContainsKey("LastAddress"))
    //                            //        tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;

    //                            //    AddressType addressType = AddressType.HoldingRegister;
    //                            //    bool decomposeSuccess = tag.Address.DecomposeAddress(tag.DataType, out addressType, out ushort offset, out byte stringLength);

    //                            //    if (!tag.ParameterContainer.Parameters.ContainsKey("AddressType") ||
    //                            //        tag.Address != tag.ParameterContainer.Parameters["LastAddress"].ToString())
    //                            //    {
    //                            //        if (decomposeSuccess)
    //                            //        {
    //                            //            tag.ParameterContainer.Parameters["AddressType"] = addressType.ToString();
    //                            //            tag.ParameterContainer.Parameters["Offset"] = offset.ToString();
    //                            //            tag.ParameterContainer.Parameters["IsValid"] = bool.TrueString;
    //                            //            tag.ParameterContainer.Parameters["LastAddress"] = tag.Address;
    //                            //            tag.ParameterContainer.Parameters["StringLength"] = stringLength.ToString();
    //                            //        }
    //                            //        else
    //                            //        {
    //                            //            tag.ParameterContainer.Parameters["IsValid"] = bool.FalseString;
    //                            //        }
    //                            //    }

    //                            //    if (tag.ParameterContainer.Parameters["IsValid"] == bool.TrueString &&
    //                            //        (DateTime.Now - tag.TimeStamp).TotalMilliseconds >= tag.RefreshRate)
    //                            //    {
    //                            //        bool readSuccess = false;
    //                            //        bool containInBlock = false;

    //                            //        int dtByteLength = 0;

    //                            //        if (tag.DataType.GetType() == typeof(ModbusTCP.String))
    //                            //        {
    //                            //            if (tag.ParameterContainer.Parameters.ContainsKey("StringLength"))
    //                            //            {
    //                            //                int.TryParse(tag.ParameterContainer.Parameters["StringLength"], out dtByteLength);
    //                            //                if (dtByteLength % 2 == 1)
    //                            //                    dtByteLength++;
    //                            //                tag.DataType.RequireByteLength = dtByteLength;
    //                            //            }
    //                            //        }
    //                            //        else
    //                            //        {
    //                            //            dtByteLength = tag.DataType.RequireByteLength;
    //                            //        }

    //                            //        foreach (var block in ReadBlockSettings)
    //                            //        {
    //                            //            if (block.AddressType == addressType)
    //                            //            {                                          
    //                            //                if (dtByteLength > 0 && dtByteLength <= 246)
    //                            //                {
    //                            //                    if (block.CheckTagIsInReadBlockRange(
    //                            //                        tag,
    //                            //                        block.AddressType,
    //                            //                        offset,
    //                            //                        dtByteLength,
    //                            //                        out int index))
    //                            //                    {
    //                            //                        containInBlock = true;
    //                            //                        readSuccess = block.ReadResult;
    //                            //                        if (readSuccess)
    //                            //                        {
    //                            //                            switch (block.AddressType)
    //                            //                            {
    //                            //                                case AddressType.InputContact:
    //                            //                                case AddressType.OutputCoil:
    //                            //                                    tag.Value = block.BoolBuffer[index] ?
    //                            //                                        (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
    //                            //                                    break;
    //                            //                                case AddressType.InputRegister:
    //                            //                                case AddressType.HoldingRegister:
    //                            //                                    tag.Value = tag.DataType.ConvertToValue(
    //                            //                                        block.ByteBuffer, tag.Gain, tag.Offset, index, 0, byteOrder);
    //                            //                                    break;
    //                            //                                default:
    //                            //                                    break;
    //                            //                            }
    //                            //                        }
    //                            //                        break;
    //                            //                    }
    //                            //                }
    //                            //            }

    //                            //        }

    //                            //        if (!containInBlock)
    //                            //        {
    //                            //            switch (addressType)
    //                            //            {
    //                            //                case AddressType.InputContact:
    //                            //                    {
    //                            //                        bool[] inputs = new bool[1];
    //                            //                        readSuccess = ReadBits(
    //                            //                            unitId,
    //                            //                            AddressType.InputContact,
    //                            //                            offset,
    //                            //                            1, ref inputs);
    //                            //                        if (readSuccess)
    //                            //                            tag.Value = inputs[0] ?
    //                            //                                (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
    //                            //                        break;
    //                            //                    }
    //                            //                case AddressType.OutputCoil:
    //                            //                    {
    //                            //                        bool[] outputs = new bool[1];
    //                            //                        readSuccess = ReadBits(
    //                            //                            unitId,
    //                            //                            AddressType.OutputCoil,
    //                            //                            offset,
    //                            //                            1, ref outputs);
    //                            //                        if (readSuccess)
    //                            //                            tag.Value = outputs[0] ?
    //                            //                                (tag.Gain + tag.Offset).ToString() : tag.Offset.ToString();
    //                            //                        break;
    //                            //                    }
    //                            //                case AddressType.InputRegister:
    //                            //                    {
    //                            //                        byte[] inputRegs = new byte[tag.DataType.RequireByteLength];
    //                            //                        readSuccess = ReadRegisters(
    //                            //                            unitId,
    //                            //                            AddressType.InputRegister,
    //                            //                            offset,
    //                            //                            (ushort)(tag.DataType.RequireByteLength / 2),
    //                            //                            ref inputRegs);
    //                            //                        if (readSuccess)
    //                            //                            tag.Value = tag.DataType.ConvertToValue(inputRegs, tag.Gain, tag.Offset, 0, 0, Device.ByteOrder);
    //                            //                        break;
    //                            //                    }
    //                            //                case AddressType.HoldingRegister:
    //                            //                    {
    //                            //                        byte[] holdingRegs = new byte[tag.DataType.RequireByteLength];
    //                            //                        readSuccess = ReadRegisters(
    //                            //                            unitId,
    //                            //                            AddressType.HoldingRegister, 
    //                            //                            offset,
    //                            //                            (ushort)(tag.DataType.RequireByteLength / 2),
    //                            //                            ref holdingRegs);
    //                            //                        if (readSuccess)
    //                            //                            tag.Value = tag.DataType.ConvertToValue(holdingRegs, tag.Gain, tag.Offset, 0, 0, Device.ByteOrder);
    //                            //                        break;
    //                            //                    }
    //                            //                default:
    //                            //                    break;
    //                            //            }
    //                            //        }

    //                            //        tag.Quality = readSuccess ? Quality.Good : Quality.Bad;
    //                            //        tag.TimeStamp = DateTime.Now;
    //                            //    }
    //                            //    else
    //                            //    {
    //                            //        tag.TimeStamp = DateTime.Now;
    //                            //        tag.Quality = Quality.Bad;
    //                            //        tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //                            //    }
    //                            //}
    //                        }
    //                        else
    //                        {
    //                            SetAllTagBad();
    //                        }

    //                        if (mbClient.IsSocketNull)
    //                        {
    //                            SetAllTagBad();
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    SetAllTagBad();
    //                }
    //            }
    //            catch { }
    //            finally
    //            {
    //                try
    //                {
    //                    mbClient?.Dispose();
    //                }
    //                catch { }

    //                stopwatch.Stop();
    //                int delay = (int)(Device.ScanRate - stopwatch.ElapsedMilliseconds);
    //                if (delay < 1)
    //                    delay = 1;
    //                Thread.Sleep(delay);
    //            }
    //        }
    //        // Dispose đối tượng phụ trách đọc ghi modbus
    //        mbClient?.Dispose();
    //    }

    //    /// <summary>
    //    /// Hàm đọc input và output
    //    /// </summary>
    //    /// <param name="unitId"></param>
    //    /// <param name="addressType"></param>
    //    /// <param name="offset"></param>
    //    /// <param name="count"></param>
    //    /// <param name="boolBuffer"></param>
    //    /// <returns></returns>
    //    private bool ReadBits(byte unitId, AddressType addressType, ushort offset, ushort count, ref bool[] boolBuffer)
    //    {
    //        locker.Wait();

    //        try
    //        {
    //            if (mbClient.IsSocketNull)
    //                InitMBClient();

    //            if (mbClient == null || mbClient.IsSocketNull)
    //                return false;

    //            byte[] result = null;
    //            switch (addressType)
    //            {
    //                case AddressType.InputContact:
    //                    mbClient.ReadDiscreteInputs(unitId, unitId, offset, count, ref result);
    //                    break;
    //                case AddressType.OutputCoil:
    //                    mbClient.ReadCoils(unitId, unitId, offset, count, ref result);
    //                    break;
    //                default:
    //                    break;
    //            }
    //            if (result != null)
    //            {
    //                for (int i = 0; i < result.Length; i++)
    //                {
    //                    for (int j = 0; j < 8; j++)
    //                    {
    //                        if ((8 * i + j) < count)
    //                            boolBuffer[8 * i + j] = Convert.ToBoolean((result[i] >> j) & 0x01);
    //                    }
    //                }
    //                return true;
    //            }
    //            return false;
    //        }
    //        catch { return false; }
    //        finally { Thread.Sleep(Device.DelayBetweenPool); locker.Release(); }
    //    }

    //    /// <summary>
    //    /// Hàm đọc input register và holding register
    //    /// </summary>
    //    /// <param name="unitId"></param>
    //    /// <param name="addressType"></param>
    //    /// <param name="offset"></param>
    //    /// <param name="count"></param>
    //    /// <param name="byteBuffer"></param>
    //    /// <returns></returns>
    //    private bool ReadRegisters(byte unitId, AddressType addressType, ushort offset, ushort count, ref byte[] byteBuffer)
    //    {
    //        locker.Wait();
    //        try
    //        {
    //            if (mbClient.IsSocketNull)
    //                InitMBClient();

    //            if (mbClient == null || mbClient.IsSocketNull)
    //            {
    //                return false;
    //            }

    //            byte[] result = null;
    //            switch (addressType)
    //            {
    //                case AddressType.InputRegister:
    //                    mbClient.ReadInputRegister(unitId, unitId, offset, count, ref result);
    //                    break;
    //                case AddressType.HoldingRegister:
    //                    mbClient.ReadHoldingRegister(unitId, unitId, offset, count, ref result);
    //                    break;
    //                default:
    //                    break;
    //            }
    //            if (result != null)
    //            {
    //                Array.Copy(result, byteBuffer, byteBuffer.Length);
    //                return true;
    //            }
    //            return false;
    //        }
    //        catch { return false; }
    //        finally { Thread.Sleep(Device.DelayBetweenPool); locker.Release(); }
    //    }

    //    /// <summary>
    //    /// Hàm set tất cả các tag trong device về trạng thái bad
    //    /// </summary>
    //    private void SetAllTagBad()
    //    {
    //        foreach (var childDevice in Device.GetAllTags().ToArray())
    //        {
    //            if (childDevice is ITagCore tag)
    //            {
    //                if (tag.IsInternalTag)
    //                    continue;
    //                tag.TimeStamp = DateTime.Now;
    //                tag.Quality = Quality.Bad;
    //                tag.RefreshInterval = (int)(DateTime.Now - tag.TimeStamp).TotalMilliseconds;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Hàm khởi tạo Modbus TCP Client 
    //    /// </summary>
    //    public void InitMBClient()
    //    {
    //        if (!IsDisposed)
    //        {
    //            locker.Wait();
    //            try
    //            {
    //                if (Device != null && Device.ParameterContainer != null)
    //                {
    //                    if (Device.ParameterContainer.Contains("IpAddress"))
    //                    {
    //                        if (mbClient != null)
    //                            mbClient.Dispose();
    //                        string ipAddress = Device.IpAddress;
    //                        ushort port = Device.Port;
    //                        mbClient = new ModbusTCPClient();
    //                        mbClient.Connect(ipAddress, port, false);
    //                    }
    //                }
    //            }
    //            catch { }
    //            finally { locker.Release(); }
    //        }
    //    }

    //    /// <summary>
    //    /// Hủy đối tượng này
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        IsDisposed = true;
    //    }

    //    public WriteResponse WriteCustom(WriteCommand cmd)
    //    {
    //        WriteResponse response = new WriteResponse()
    //        {
    //            WriteCommand = cmd,
    //            ExecuteTime = DateTime.Now
    //        };

    //        try
    //        {
    //            if (cmd == null)
    //            {
    //                response.Error = "The write command is null.";
    //            }
    //            else
    //            {
    //                string writeAddress = cmd.CustomWriteAddress?.Trim();
    //                if (string.IsNullOrWhiteSpace(writeAddress))
    //                {
    //                    response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
    //                }
    //                else
    //                {
    //                    if (cmd.EquivalentDevice == null)
    //                    {
    //                        response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
    //                    }
    //                    else
    //                    {
    //                        if (cmd.EquivalentDevice.Parent?.FindParent<IChannelCore>(x => x is IChannelCore) != Channel ||
    //                            !(cmd.EquivalentDevice is Device))
    //                        {
    //                            response.Error = $"The parent of tag doesn't match with driver.";
    //                        }
    //                        else
    //                        {
    //                            if (cmd.CustomWriteAddress.DecomposeAddress(out AddressType addressType, out ushort offset))
    //                            {
    //                                if (addressType == AddressType.Undefined)
    //                                {
    //                                    response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
    //                                }
    //                                else
    //                                {
    //                                    if (cmd.CustomWriteValue == null)
    //                                    {
    //                                        response.Error = $"The write value is null or invaid";
    //                                    }
    //                                    else
    //                                    {
    //                                        byte unitId = (byte)(cmd.EquivalentDevice as Device).UnitId;
    //                                        switch (addressType)
    //                                        {
    //                                            case AddressType.InputContact:
    //                                            case AddressType.InputRegister:
    //                                                {
    //                                                    response.Error = $"The write address doesn't support write function";
    //                                                    break;
    //                                                }
    //                                            case AddressType.OutputCoil:
    //                                                {
    //                                                    if (cmd.CustomWriteValue.Length == 0 ||
    //                                                    cmd.CustomWriteValue.Length >= 125)
    //                                                    {
    //                                                        response.Error = $"The write value is not valid";
    //                                                        return response;
    //                                                    }
    //                                                    Thread.Sleep(cmd.Delay);
    //                                                    locker.Wait();
    //                                                    try
    //                                                    {

    //                                                        byte[] result = null;
    //                                                        mbClient.WriteMultipleCoils(
    //                                                            unitId,
    //                                                            unitId,
    //                                                            offset,
    //                                                            (ushort)cmd.CustomWriteValue.Length, cmd.CustomWriteValue, ref result);
    //                                                        response.IsSuccess = result != null;

    //                                                        if (!response.IsSuccess)
    //                                                            response.Error = $"Write not success.";
    //                                                    }
    //                                                    catch { }
    //                                                    finally { locker.Release(); }
    //                                                    break;
    //                                                }
    //                                            case AddressType.HoldingRegister:
    //                                                {
    //                                                    if (cmd.CustomWriteValue.Length % 2 == 1 ||
    //                                                    cmd.CustomWriteValue.Length == 0 ||
    //                                                    cmd.CustomWriteValue.Length >= 125)
    //                                                    {
    //                                                        response.Error = $"The write value is not valid";
    //                                                        return response;
    //                                                    }
    //                                                    Thread.Sleep(cmd.Delay);
    //                                                    locker.Wait();
    //                                                    try
    //                                                    {
    //                                                        byte[] result = null;
    //                                                        mbClient.WriteMultipleRegister(
    //                                                            unitId,
    //                                                            unitId,
    //                                                            offset,
    //                                                            cmd.CustomWriteValue, ref result);
    //                                                        response.IsSuccess = result != null;

    //                                                        if (!response.IsSuccess)
    //                                                            response.Error = $"Write not success.";
    //                                                    }
    //                                                    catch { }
    //                                                    finally { locker.Release(); }
    //                                                    break;
    //                                                }
    //                                            default:
    //                                                break;
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                            else
    //                            {
    //                                response.Error = $"The write address {cmd.CustomWriteAddress} is not valid.";
    //                            }
    //                        }
    //                    }
    //                }

    //                if (cmd.NextCommands != null)
    //                {
    //                    if (cmd.NextCommands.Count > 0)
    //                    {
    //                        if (!cmd.AllowExecuteNextCommandsWhenFail ||
    //                            cmd.AllowExecuteNextCommandsWhenFail && response.IsSuccess)
    //                        {
    //                            foreach (var nextCmd in cmd.NextCommands)
    //                            {
    //                                ExecuteWriteCommand(nextCmd);
    //                            }
    //                        }
    //                    }
    //                }

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Error = $"Exception throw: {ex.ToString()}";
    //        }
    //        return response;
    //    }

    //    public WriteResponse WriteTag(WriteCommand cmd)
    //    {
    //        WriteResponse response = new WriteResponse()
    //        {
    //            WriteCommand = cmd,
    //            ExecuteTime = DateTime.Now
    //        };
    //        try
    //        {
    //            if (cmd == null)
    //            {
    //                response.Error = "The write command is null.";
    //            }
    //            else
    //            {
    //                if (cmd.EquivalentDevice == null)
    //                {
    //                    response.Error = $"Device not found";
    //                }
    //                else
    //                {
    //                    if (cmd.EquivalentDevice.Parent?.FindParent<IChannelCore>(x => x is IChannelCore) != Channel ||
    //                        !(cmd.EquivalentDevice is Device))
    //                    {
    //                        response.Error = $"The parent of tag doesn't match with driver.";
    //                    }
    //                    else
    //                    {
    //                        if (cmd.EquivalentTag != null && cmd.EquivalentTag is Tag tag)
    //                        {
    //                            if (tag.AddressType == AddressType.Undefined)
    //                            {
    //                                response.Error = $"The tag address {tag.Address} is not valid.";
    //                            }
    //                            else
    //                            {
    //                                if (tag.DataType == null)
    //                                {
    //                                    response.Error = $"The data type of tag is null";
    //                                }
    //                                else
    //                                {
    //                                    if (tag.DataType.TryParseToByteArray(cmd.Value, tag.Gain, tag.Offset, out byte[] writeBuffer, (cmd.EquivalentDevice as Device).ByteOrder))
    //                                    {
    //                                        byte unitId = (byte)(cmd.EquivalentDevice as Device).UnitId;
    //                                        switch (tag.AddressType)
    //                                        {
    //                                            case AddressType.InputContact:
    //                                            case AddressType.InputRegister:
    //                                                {
    //                                                    response.Error = $"The write address doesn't support write function";
    //                                                    break;
    //                                                }
    //                                            case AddressType.OutputCoil:
    //                                                {
    //                                                    Thread.Sleep(cmd.Delay);
    //                                                    locker.Wait();
    //                                                    try
    //                                                    {
    //                                                        bool? writeValue = null;
    //                                                        if (cmd.Value == "1")
    //                                                        {
    //                                                            writeValue = true;
    //                                                        }
    //                                                        else if (cmd.Value == "0")
    //                                                        {
    //                                                            writeValue = false;
    //                                                        }
    //                                                        else
    //                                                        {
    //                                                            response.IsSuccess = false;
    //                                                            response.Error = "Write value is not valid";
    //                                                            return response;
    //                                                        }

    //                                                        byte[] result = null;
    //                                                        mbClient.WriteSingleCoils(
    //                                                            unitId, unitId, (ushort)tag.AddressOffset, writeValue.Value, ref result);

    //                                                        response.IsSuccess = result != null;
    //                                                        if (!response.IsSuccess)
    //                                                            response.Error = $"Write not success.";

    //                                                    }
    //                                                    catch { }
    //                                                    finally { locker.Release(); }
    //                                                    break;
    //                                                }
    //                                            case AddressType.HoldingRegister:
    //                                                {
    //                                                    Thread.Sleep(cmd.Delay);
    //                                                    locker.Wait();
    //                                                    try
    //                                                    {
    //                                                        byte[] result = null;
    //                                                        mbClient.WriteMultipleRegister(
    //                                                            unitId, unitId, (ushort)tag.AddressOffset, writeBuffer, ref result);
    //                                                        response.IsSuccess = result != null;
    //                                                        if (!response.IsSuccess)
    //                                                            response.Error = $"Write not success.";
    //                                                    }
    //                                                    catch { }
    //                                                    finally { locker.Release(); }
    //                                                    break;
    //                                                }
    //                                            default:
    //                                                break;
    //                                        }
    //                                    }
    //                                    else
    //                                    {
    //                                        response.Error = $"The write value is not valid";
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        else
    //                        {
    //                            response.Error = $"Tag not found";
    //                        }
    //                    }
    //                }

    //                if (cmd.NextCommands != null)
    //                {
    //                    if (cmd.NextCommands.Count > 0)
    //                    {
    //                        if (!cmd.AllowExecuteNextCommandsWhenFail ||
    //                            cmd.AllowExecuteNextCommandsWhenFail && response.IsSuccess)
    //                        {
    //                            foreach (var nextCmd in cmd.NextCommands)
    //                            {
    //                                ExecuteWriteCommand(nextCmd);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            response.Error = $"Exception throw: {ex.ToString()}";
    //        }
    //        return response;
    //    }

    //    public WriteResponse ExecuteWriteCommand(WriteCommand cmd)
    //    {
    //        if (cmd == null)
    //        {
    //            return new WriteResponse()
    //            {
    //                WriteCommand = cmd,
    //                ExecuteTime = DateTime.Now,
    //                Error = "The write command is null."
    //            };
    //        }
    //        else
    //        {
    //            if (cmd.IsCustomWrite)
    //                return WriteCustom(cmd);
    //            else
    //                return WriteTag(cmd);
    //        }
    //    }

    //    private async void OnWriteQueueAdded(object sender, EventArgs e)
    //    {
    //        WriteCommand cmd = WriteQueue.GetCommand();
    //        if (cmd != null)
    //        {
    //            switch (cmd.WritePiority)
    //            {
    //                case WritePiority.Default:
    //                    cmd.Expired += OnWriteCommandExpired;
    //                    DefaultPiorityWriteCommands.TryAdd(cmd, cmd);
    //                    break;
    //                case WritePiority.High:
    //                    await Task.Run(() =>
    //                    {
    //                        WriteResponse response = ExecuteWriteCommand(cmd);
    //                        CommandExecutedEventArgs args = new CommandExecutedEventArgs(cmd, response);
    //                        cmd.OnExecuted(args);
    //                    });
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }

    //    private void OnWriteCommandExpired(object sender, EventArgs e)
    //    {
    //        if (sender is WriteCommand cmd)
    //        {
    //            cmd.Expired -= OnWriteCommandExpired;

    //            if (DefaultPiorityWriteCommands.ContainsKey(cmd))
    //            {
    //                DefaultPiorityWriteCommands.TryRemove(cmd, out WriteCommand removeCmd);
    //            }
    //        }
    //    }
    //}
}
