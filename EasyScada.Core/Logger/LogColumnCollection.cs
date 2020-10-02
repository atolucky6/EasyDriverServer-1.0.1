using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class LogColumnCollection : TypedCollection<LogColumn>
    {
        public override void Add(LogColumn item)
        {
            if (item != null && string.IsNullOrEmpty(item.ColumnName))
            {
                item.ColumnName = GetUniqueName();
            }
            base.Add(item);
        }

        public override int Add(object value)
        {
            if (value is LogColumn col && string.IsNullOrEmpty(col.ColumnName))
            {
                col.ColumnName = GetUniqueName();
            }
            return base.Add(value);
        }

        public override void Insert(int index, LogColumn item)
        {
            if (item != null && string.IsNullOrEmpty(item.ColumnName))
            {
                item.ColumnName = GetUniqueName();
            }
            base.Insert(index, item);
        }

        public override void Insert(int index, object value)
        {
            if (value is LogColumn col && string.IsNullOrEmpty(col.ColumnName))
            {
                col.ColumnName = GetUniqueName();
            }
            base.Insert(index, value);
        }

        private string GetUniqueName()
        {
            int index = 1;
            while (true)
            {
                string name = $"Column{index}";
                if (this.FirstOrDefault(x => x.ColumnName == name) == null)
                    return name;
                index++;
            }
        }
    }
}
