using DevExpress.Mvvm.POCO;
using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;

namespace EasyDriver.CoreItemFactory
{
    public class CoreFactory : EasyServicePlugin, ICoreFactory
    {
        public CoreFactory(IServiceContainer serviceContainer) : base(serviceContainer)
        {
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }

        public IChannelCore ShowCreateChannelView(IGroupItem parent, out IEasyDriverPlugin driver)
        {
            driver = null;
            try
            {
                SelectDriverViewModel viewModel = ViewModelSource.Create(() => new SelectDriverViewModel(parent));
                SelectDriverWindow selectDriver = new SelectDriverWindow();
                selectDriver.DataContext = viewModel;
                if (selectDriver.ShowDialog() == true)
                {
                    if (selectDriver.Tag is List<object> array)
                    {
                        if (array.Count == 2)
                        {
                            if (array[1] is IEasyDriverPlugin driverPlugin)
                            {
                                driver = driverPlugin;
                                if (array[0] is IChannelCore channelCore)
                                    return channelCore;
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        public IDeviceCore ShowCreateDeviceView(IGroupItem parent, IEasyDriverPlugin driver, IDeviceCore template = null)
        {
            try
            {
                if (driver != null && parent != null)
                {
                    var createDeviceControl = driver.GetCreateDeviceControl(parent, template);
                    if (createDeviceControl != null)
                        return Helper.ShowContextWindow(createDeviceControl, "Add Device") as IDeviceCore;
                }
            }
            catch (Exception) { }
            return null;
        }

        public List<ITagCore> ShowCreateTagView(IGroupItem parent, IEasyDriverPlugin driver, ITagCore template)
        {
            try
            {
                if (driver != null && parent != null)
                {
                    var createTagControl = driver.GetCreateTagControl(parent, template);
                    if (createTagControl != null)
                        return Helper.ShowContextWindow(createTagControl, "Add Device") as List<ITagCore>;
                }
            }
            catch (Exception) { }
            return null;
        }

        public void ShowEditChannelView(IChannelCore channel, IEasyDriverPlugin driver)
        {
            try
            {
                if (driver != null && channel != null)
                {
                    var editChannelControl = driver.GetEditChannelControl(channel);
                    if (editChannelControl != null)
                        Helper.ShowContextWindow(editChannelControl, $"Edit Channel - {channel.Name}");
                }
            }
            catch (Exception) { }
        }

        public void ShowEditDeviceView(IDeviceCore device, IEasyDriverPlugin driver)
        {
            try
            {
                if (driver != null && device != null)
                {
                    var editDeviceControl = driver.GetEditDeviceControl(device);
                    if (editDeviceControl != null)
                        Helper.ShowContextWindow(editDeviceControl, $"Edit Device - {device.Name}");
                }
            }
            catch (Exception) { }
        }

        public void ShowEditTagView(ITagCore tag, IEasyDriverPlugin driver)
        {
            try
            {
                if (driver != null && tag != null)
                {
                    var editTagControl = driver.GetEditTagControl(tag);
                    if (editTagControl != null)
                        Helper.ShowContextWindow(editTagControl, $"Edit Tag - {tag.Name}");
                }
            }
            catch (Exception) { }
        }
    }
}
