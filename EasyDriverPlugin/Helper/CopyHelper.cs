using EasyDriverPlugin;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Lớp hỗ trợ việc copy đối tượng
    /// </summary>
    public static class CopyHelper
    {
        /// <summary>
        /// Copy <see cref="ICoreItem"/> thành một <see cref="ICoreItem"/> mới 
        /// </summary>
        /// <param name="coreItem"></param>
        /// <returns></returns>
        public static ICoreItem DeepCopy(ICoreItem coreItem)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, coreItem);
                stream.Seek(0, SeekOrigin.Begin);

                return (ICoreItem)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Copy <see cref="T"/> thành một <see cref="T"/> mới 
        /// </summary>
        /// <param name="coreItem"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T item)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);

                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
