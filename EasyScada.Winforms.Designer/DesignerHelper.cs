using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyScada.Winforms.Designer
{
    public static class DesignerHelper
    {
        /// <summary>
        /// The method to get a <see cref="PropertyDescriptor"/> of the control by property name
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static PropertyDescriptor GetPropertyByName(this object control, [CallerMemberName]string propName = null)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(control)[propName];
            if (null == prop)
                throw new ArgumentException("Matching ColorLabel property not found!", propName);
            else
                return prop;
        }

        /// <summary>
        /// Set the value for property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        public static void SetValue(this object control, object value = null, [CallerMemberName]string propName = null)
        {
            control.GetPropertyByName(propName).SetValue(control, value);
        }
    }
}
