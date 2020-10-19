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
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual string EmailSettingName { get; set; }
        public virtual string SMSSettingName { get; set; }

        [Browsable(false)]
        public bool ReadOnly { get; set; }

        public AlarmGroup()
        {
        }
    }
}
