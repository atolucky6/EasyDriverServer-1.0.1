using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class RoleCollection : TypedCollection<Role>
    {
        public override int Add(object value)
        {
            (value as Role).Source = this;
            return base.Add(value);
        }

        public override void Add(Role item)
        {
            item.Source = this;
            base.Add(item);
        }

        public override void AddRange(Role[] itemArray)
        {
            foreach (var item in itemArray)
            {
                item.Source = this;
            }
            base.AddRange(itemArray);
        }

        public override void Insert(int index, object value)
        {
            (value as Role).Source = this;
            base.Insert(index, value);
        }

        public override void Insert(int index, Role item)
        {
            item.Source = this;
            base.Insert(index, item);
        }
    }
}
