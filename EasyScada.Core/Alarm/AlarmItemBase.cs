using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public abstract class AlarmItemBase : IUniqueNameItem
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Text { get; set; }
        public virtual string Class { get; set; }
        public virtual string Group { get; set; }
        public virtual string Description { get; set; }
        public virtual string TriggerTag { get; set; }
        public virtual AlarmState AlarmState { get; set; }
    }
}
