using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace EasyScada.Core
{
    public class AnimateProperty<T> : AnimatePropertyBase
    {
        public AnimateProperty(object targetControl,T defaultValue)
        {
            TargetControl = targetControl;
            this.defaultValue = defaultValue;
        }

        public AnimateProperty()
        {

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
                IsDirty = true;
                this.value = value;
            }
        }

        public override void SetValue()
        {
            try
            {
                if (Enabled && TargetControl != null && AnimatePropertyInfo != null)
                {
                    if (TargetControl is Control winformControl)
                    {
                        winformControl.Invoke(new Action(() =>
                        {
                            if (!Equals(AnimatePropertyInfo.GetValue(TargetControl), Value))
                                AnimatePropertyInfo.SetMethod.Invoke(TargetControl, new object[] { Value });
                        }));
                    }
                    else
                    {
                        AnimatePropertyInfo.SetValue(TargetControl, Value);
                    }
                }
            }
            catch { }
        }

    }
}
