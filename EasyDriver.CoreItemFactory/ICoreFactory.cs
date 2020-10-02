using EasyDriverPlugin;
using System.Collections.Generic;

namespace EasyDriver.CoreItemFactory
{
    public interface ICoreFactory
    {
        IChannelCore ShowCreateChannelView(IGroupItem parent, out IEasyDriverPlugin driver);
        IDeviceCore ShowCreateDeviceView(IGroupItem parent, IEasyDriverPlugin driver, IDeviceCore template = null);
        List<ITagCore> ShowCreateTagView(IGroupItem parent, IEasyDriverPlugin driver, ITagCore template);

        void ShowEditChannelView(IChannelCore channel, IEasyDriverPlugin driver);
        void ShowEditDeviceView(IDeviceCore device, IEasyDriverPlugin driver);
        void ShowEditTagView(ITagCore tag, IEasyDriverPlugin driver);
    }
}
