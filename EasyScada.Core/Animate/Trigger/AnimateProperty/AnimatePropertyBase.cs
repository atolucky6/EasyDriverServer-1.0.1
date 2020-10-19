using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public abstract class AnimatePropertyBase
    {
        [Browsable(true)]
        public virtual bool Enabled { get; set; }

        [Browsable(false)]
        public bool IsDirty { get; set; }

        [field: NonSerialized]
        [Browsable(false)]
        public virtual object TargetControl { get; set; }

        [field: NonSerialized]
        protected PropertyInfo propertyInfo;

        [Browsable(false)]
        public virtual PropertyInfo AnimatePropertyInfo { get => propertyInfo; }

        public abstract void SetValue();

        public virtual void ResetToDefault()
        {
            Enabled = false;
            IsDirty = false;
        }

        public override string ToString()
        {
            return "";
        }

        public virtual void SetAnimatePropertyInfo(PropertyInfo propInfo)
        {
            propertyInfo = propInfo;
        }
    }
}
