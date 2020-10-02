namespace EasyDriver.Opc.DA.Client.Common.Internal
{
    internal interface IOpcServerShutdownHandler
    {
        void HandleShutdown(string reason);
    }
}