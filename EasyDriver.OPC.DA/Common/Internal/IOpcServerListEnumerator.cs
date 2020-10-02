using System;
using System.Collections.Generic;

namespace EasyDriver.Opc.DA.Client.Common.Internal
{
    internal interface IOpcServerListEnumerator
    {
        List<OpcServerDescription> Enumerate(string host, Guid[] categoriesGuids);
        Guid CLSIDFromProgID(string progId);
        OpcServerDescription GetServerDescription(string host, Guid clsid);
        OpcServerDescription TryGetServerDescription(string host, Guid clsid);
    }
}