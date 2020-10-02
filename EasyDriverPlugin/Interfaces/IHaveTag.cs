using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriverPlugin
{
    public interface IHaveTag
    {
        bool HaveTags { get; set; }
        TagCollection Tags { get; }
    }
}
