using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Đại diện cho đối tượng chứa tag
    /// </summary>
    public interface IHaveTag
    {
        bool HaveTags { get; set; }
        NotifyCollection Tags { get; }
    }
}
