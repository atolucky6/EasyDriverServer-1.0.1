using System.ComponentModel;

namespace EasyScada.Wpf.Connector.VisualStudio.Design
{
    public class DesignerPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public DesignerPropertyChangedEventArgs(string property, object value) : base(property)
        {
            Value = value;
        }

        public object Value { get; protected set; }
    }
}
