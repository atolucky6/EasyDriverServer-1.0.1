using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.Core
{
    public interface ICheckable
    {
        bool? IsChecked { get; set; }
    }
}
