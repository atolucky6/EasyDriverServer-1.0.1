using System;
using System.ComponentModel;
using System.Reflection;

namespace EasyScada.Core
{
    public class AnimateProperty<T> : AnimatePropertyBase
    {
        public AnimateProperty(object targetControl, PropertyInfo property, T defaultValue)
        {
            TargetControl = targetControl;
            AnimatePropertyInfo = property;
            this.defaultValue = defaultValue;
        }

        private T defaultValue;
        [Browsable(false)]
        public virtual T DefaultValue
        {
            get
            {
                try
                {
                    if (TargetControl != null && AnimatePropertyInfo != null)
                    {
                        return (T)AnimatePropertyInfo.GetValue(TargetControl);
                    }
                }
                catch { }
                return default;
            }
        }

        private T value;
        [Browsable(true), Category("Value")]
        public virtual T Value
        {
            get
            {
                if (IsDirty)
                    return value;
                return DefaultValue;
            }
            set
            {
                if (!Equals(value, this.value))
                {
                    this.value = value;
                    IsDirty = true;
                }
            }
        }

        public override void SetValue()
        {
            try
            {
                if (Enabled && TargetControl != null && AnimatePropertyInfo != null)
                    AnimatePropertyInfo.SetValue(TargetControl, Value);
            }
            catch { }
        }
    }
}
