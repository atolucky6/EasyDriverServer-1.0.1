using System.Collections.Generic;

namespace EasyScada.Core
{
    public interface IComposite
    {
        List<object> Childs { get; }
    }
}
