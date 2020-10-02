using System.IO;
using System.Reflection;

namespace EasyDriverServer.ViewModel
{
    public static class ApplicationHelper
    {
        public static string GetApplicationPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
