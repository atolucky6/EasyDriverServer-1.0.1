using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class AlarmGroup : IUniqueNameItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Browsable(false)]
        public bool IsReadOnly { get; set; }

        public AlarmGroup()
        {

        }
    }
}
