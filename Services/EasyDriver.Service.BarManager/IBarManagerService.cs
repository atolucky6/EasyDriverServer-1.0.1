using EasyDriver.ServicePlugin;
using System.Collections.ObjectModel;

namespace EasyDriver.Service.BarManager
{
    public interface IBarManagerService : IEasyServicePlugin
    {
        ObservableCollection<IBarComponent> BarSource { get; }
        IBarComponent MainMenu { get; }
        IBarComponent Toolbar { get; }
        IBarComponent StatusBar { get; }
    }
}
