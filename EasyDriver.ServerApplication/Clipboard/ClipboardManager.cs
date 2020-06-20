using EasyDriverPlugin;
using EasyDriver.Server.Models;
using System.Windows;


namespace EasyScada.ServerApplication
{
    public static class ClipboardManager
    {
        public static object Context { get; private set; }
        public static string CurrentFormat { get; private set; } = string.Empty;
        public static bool IsDevice { get; private set; }
        public static string DeviceType { get; private set; }

        public static T GetDataFromClipboard<T>()
            where T : class
        {
            IDataObject dataObj = Clipboard.GetDataObject();
            if (dataObj.GetDataPresent(CurrentFormat))
            {
                var retriveObj = dataObj.GetData(CurrentFormat);
                return (T)retriveObj;
            }
            return null;
        }

        public static void CopyToClipboard(object objectToCopy, object context)
        {
            Clear();
            IsDevice = objectToCopy is IDeviceCore;

            //if (objectToCopy is IComponent component)
            //    ModuleType = component.Owner.ModuleType;

            Context = context;
            CurrentFormat = objectToCopy.GetType().FullName;

            IDataObject dataObj = new DataObject();
            dataObj.SetData(CurrentFormat, objectToCopy, false);
            Clipboard.SetDataObject(dataObj, false);
        }

        public static bool ContainData<T>()
        {
            return Clipboard.ContainsData(typeof(T).FullName);
        }

        public static bool ContainData()
        {
            if (string.IsNullOrEmpty(CurrentFormat))
                return false;
            return Clipboard.ContainsData(CurrentFormat);
        }


        public static void Clear()
        {
         //   ModuleType = ModuleType.None;
            Clipboard.Clear();
        }
    }

}
