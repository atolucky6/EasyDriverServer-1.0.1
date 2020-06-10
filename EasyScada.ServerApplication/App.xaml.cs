using System.Windows;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IoC.Instance.Setup();
            base.OnStartup(e);
        }
    }
}
