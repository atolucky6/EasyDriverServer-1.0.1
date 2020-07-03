using DevExpress.Xpf.Core;
using Microsoft.Owin.Hosting;
using System.Windows;

namespace EasyScada.ServerApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            // Load custom theme
            var theme = new Theme("EasyDriverServerDarkTheme");
            theme.AssemblyName = "DevExpress.Xpf.Themes.EasyDriverServerDarkTheme.v19.1";
            Theme.RegisterTheme(theme);
            ApplicationThemeHelper.ApplicationThemeName = "EasyDriverServerDarkTheme";
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            IoC.Instance.Setup();
            string url = $"http://*:{IoC.Instance.Get<ApplicationViewModel>().ServerConfiguration.Port}";
            WebApp.Start(url);
            base.OnStartup(e);
        }
    }
}
