using System;

namespace EasyDriver.Opc.DA.Client.Common
{
    public class RpcFailedEventArgs : EventArgs
    {
        public RpcFailedEventArgs(object userData, HRESULT error)
        {
            UserData = userData;
            Error = error;
        }

        public HRESULT Error { get; private set; }
        public object UserData { get; private set; }
    }
}