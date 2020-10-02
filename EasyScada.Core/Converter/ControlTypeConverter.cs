using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace EasyScada.Core
{
    public class ControlTypeConverter : StringConverter
    {

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                var userControlNames = GetType().Assembly.GetTypes().Where(x => x.Namespace == "System.Windows.Forms.UserControl").Select(x => x.Name);
                return new StandardValuesCollection(new string[] { "1" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return new StandardValuesCollection(new string[] { });
            }
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
