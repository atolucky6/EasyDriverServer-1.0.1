using System.Windows;

namespace EasyScada.ServerApplication
{
    public static class ClipboardManager
    {
        public static object Context { get; private set; }
        public static object ObjectToCopy { get; private set; }

        public static void CopyToClipboard(object objectToCopy, object context)
        {
            Clear();
            Context = context;
            ObjectToCopy = objectToCopy;
        }

        /// <summary>
        /// Hảm kiểm tra xem trong clipboard có chứa data hay không
        /// </summary>
        /// <returns></returns>
        public static bool ContainData()
        {
            return Context != null && ObjectToCopy != null;
        }

        /// <summary>
        /// Xóa tất cả thông tin có trong clipboard
        /// </summary>
        public static void Clear()
        {
            Clipboard.Clear();
            ObjectToCopy = null;
            Context = null;
        }
    }
}
