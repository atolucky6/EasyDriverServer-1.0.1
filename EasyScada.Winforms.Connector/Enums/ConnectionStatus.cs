namespace EasyScada.Winforms.Connector
{
    /// <summary>
    /// Trạng thái kết nối của <see cref="IStation"/>
    /// </summary>
    public enum ConnectionStatus
    {
        Connecting = 0,
        Connected = 1,
        Reconnecting = 2,
        Disconnected = 3
    }
}
