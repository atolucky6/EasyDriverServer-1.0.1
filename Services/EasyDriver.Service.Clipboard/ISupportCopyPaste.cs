using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.Service.Clipboard
{
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
