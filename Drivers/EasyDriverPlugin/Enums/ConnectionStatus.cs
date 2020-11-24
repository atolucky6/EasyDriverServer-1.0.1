namespace EasyDriverPlugin
{
    /// <summary>
    /// Trạng thái kết nối của <see cref="IStationClient"/>
    /// </summary>
    public enum ConnectionStatus
    {
        Connecting = 0,
        Connected = 1,
        Reconnecting = 2,
        Disconnected = 3
    }
}
