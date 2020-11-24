using EasyDriver.ServicePlugin;
using EasyDriverPlugin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDriver.Service.TagWriter
{
    /// <summary>
    /// Cung cấp chức năng ghi giá trị cho tag
    /// </summary>
    public interface ITagWriterService : IEasyServicePlugin
    {
        Task<WriteResponse> WriteTag(WriteCommand writeCommand);
        Task<List<WriteResponse>> WriteMultiTag(List<WriteCommand> writeCommands);
    }
}
