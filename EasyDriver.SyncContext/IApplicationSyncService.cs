using EasyDriver.ServicePlugin;
using System.ComponentModel;

namespace EasyDriver.SyncContext
{
    public interface IApplicationSyncService : IEasyServicePlugin, INotifyPropertyChanged
    {
        bool IsBusy { get; set; }
        string MessageTitle { get; set; }
        string ApplicationTitle { get; set; }
    }
}
