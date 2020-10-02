using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.CopyPaste
{
    /// <summary>
    /// Interface hỗ trợ việc Copy/Cut/Paste đối tượng
    /// </summary>
    public interface ISupportCopyPaste
    {
        void Copy();
        bool CanCopy();
        void Cut();
        bool CanCut();
        void Paste();
        bool CanPaste();
        void Delete();
        bool CanDelete();
    }
}
