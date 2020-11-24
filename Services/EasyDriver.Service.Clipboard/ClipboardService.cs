using EasyDriver.ServicePlugin;

namespace EasyDriver.Service.Clipboard
{
    [Service(0, true)]
    public class ClipboardService : EasyServicePluginBase, IClipboardService
    {
        public object Context { get; set; }

        public object ObjectToCopy { get; set; }

        public void Clear()
        {
            ObjectToCopy = null;
            Context = null;
        }

        public bool ContainData()
        {
            return Context != null && ObjectToCopy != null;
        }

        public void CopyToClipboard(object objectToCopy, object context)
        {
            Context = context;
            ObjectToCopy = objectToCopy;
        }
    }
}
