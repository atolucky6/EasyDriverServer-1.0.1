using System.Collections.Generic;

namespace EasyScada.Winforms.Connector
{
    public interface IComposite
    {
        List<object> Childs { get; }
    }
}
