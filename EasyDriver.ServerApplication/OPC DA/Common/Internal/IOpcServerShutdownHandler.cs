namespace EasyDriver.Opc.Client.Common.Internal
{
    internal interface IOpcServerShutdownHandler
    {
        void HandleShutdown(string reason);
    }
}