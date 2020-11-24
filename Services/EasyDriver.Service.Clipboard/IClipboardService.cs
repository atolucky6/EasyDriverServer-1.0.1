using EasyDriver.ServicePlugin;
using System;

namespace EasyDriver.Service.Clipboard
{
    public interface IClipboardService : IEasyServicePlugin
    {
        object Context { get; }
        object ObjectToCopy { get; }
        void CopyToClipboard(object objectToCopy, object context);
        void Clear();
        bool ContainData();
    }
}
