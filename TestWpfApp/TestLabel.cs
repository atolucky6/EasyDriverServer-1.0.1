using EasyScada.Winforms.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;

namespace TestWpfApp
{
    public class TestLabel : Label
    {
        public TestLabel() : base()
        {
        }

        public object Value { get;set; }


        [EditorBrowsable(EditorBrowsableState.Always)]
        [Editor(typeof(TestEditor), typeof(UITypeEditor))]
        public object MyProperty
        {
            get { return (object)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Editor(typeof(TestEditor), typeof(UITypeEditor))]
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(object), typeof(TestLabel), new PropertyMetadata(null));


    }

    public class TestLabelConverter : TypeConverter
    {
        public TestLabelConverter()
        {
        }   

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return base.CreateInstance(context, propertyValues);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "1", "2", "3" });
           // return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {

            return true;
        }
    }

    public class TestEditor : PropertyEditorBase
    {
        public TestEditor()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        protected override System.Windows.Forms.Control GetEditControl(string propertyName, object currentValue)
        {
            return new Form1();
        }


        protected override object GetEditedValue(System.Windows.Forms.Control editControl, string propertyName, object oldValue)
        {
            return "1";
        }
    }
}
