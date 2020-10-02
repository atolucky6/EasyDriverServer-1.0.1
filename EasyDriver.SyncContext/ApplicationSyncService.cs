using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using System.ComponentModel;

namespace EasyDriver.SyncContext
{
    public class ApplicationSyncService : EasyServicePlugin, IApplicationSyncService, INotifyPropertyChanged
    {
        public ApplicationSyncService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
        }

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBusy"));
                }
            }
        }

        public string MessageTitle { get; set; } = "Easy Scada";
        public string ApplicationTitle { get; set; } = "Easy Scada Server";

        public event PropertyChangedEventHandler PropertyChanged;

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }
    }
}
