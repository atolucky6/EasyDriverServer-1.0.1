using System;

namespace EasyScada.Winforms.Connector
{
    [Serializable]
    public enum CommunicationMode
    {
        ReceiveFromServer,
        RequestToServer,
    }
}
