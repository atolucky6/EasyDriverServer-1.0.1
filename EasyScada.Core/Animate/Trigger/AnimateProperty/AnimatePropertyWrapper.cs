using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace EasyScada.Core
{
    public abstract class AnimatePropertyWrapper
    {
        private object targetControl;
        [Browsable(false)]
        public virtual object TargetControl
        {
            get => targetControl;
            set
            {
                if (targetControl != value)
                {
                    targetControl = value;
                    Type targetType = value.GetType();

                    var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo propertyInfo in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (typeof(AnimatePropertyBase).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            foreach (var targetProp in targetProperties)
                            {
                                if (targetProp.Name == propertyInfo.Name)
                                {
                                    if (propertyInfo.GetValue(this) is AnimatePropertyBase animateProp)
                                    {
                                        animateProp.TargetControl = value;
                                        animateProp.SetAnimatePropertyInfo(targetProp);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public abstract void UpdateValue();
        public abstract void Reverse();

        public virtual IEnumerable<AnimatePropertyBase> GetAnimateProperties()
        {
            foreach (PropertyInfo propInfo in GetType().GetProperties())
            {
                if (typeof(AnimatePropertyBase).IsAssignableFrom(propInfo.PropertyType))
                    yield return propInfo.GetValue(this) as AnimatePropertyBase;
            }
        }
    }
}
