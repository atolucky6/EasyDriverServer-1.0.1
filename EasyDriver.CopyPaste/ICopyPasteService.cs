using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.CopyPaste
{
    public interface ICopyPasteService
    {
        object Context { get; }
        object ObjectToCopy { get; }
        void CopyToClipboard(object objToCopy, object context);
        void Clear();
        bool ContainData();
    }
}
