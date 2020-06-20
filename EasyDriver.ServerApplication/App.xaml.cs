using Microsoft.Owin.Hosting;
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
            string url = "http://*:8800";
            WebApp.Start(url);
            base.OnStartup(e);
        }
    }
}
