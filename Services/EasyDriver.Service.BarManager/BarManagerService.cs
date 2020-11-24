using EasyDriver.ServicePlugin;
using System.Collections.ObjectModel;

namespace EasyDriver.Service.BarManager
{
    [Service(int.MaxValue, true)]
    public class BarManagerService : EasyServicePluginBase, IBarManagerService
    {
        public ObservableCollection<IBarComponent> BarSource { get; protected set; }

        public IBarComponent MainMenu { get; protected set; }

        public IBarComponent Toolbar { get; protected set; }

        public IBarComponent StatusBar { get; protected set; }

        public BarManagerService() : base()
        {
            BarSource = new ObservableCollection<IBarComponent>();
            MainMenu = BarFactory.Default.CreateMainMenu("MainMenu");
            Toolbar = BarFactory.Default.CreateToolBar("Toolbar");
            StatusBar = BarFactory.Default.CreateStatusBar("StatusBar");
            BarSource.Add(MainMenu);
            BarSource.Add(Toolbar);
            BarSource.Add(StatusBar);
        }

        public override void EndInit()
        {
            base.EndInit();
        }

        public override void BeginInit()
        {
            base.BeginInit();
        }
    }
}
