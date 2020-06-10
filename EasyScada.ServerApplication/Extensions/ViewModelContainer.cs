using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace EasyScada.ServerApplication
{
    public class ViewModelContainerExtension : MarkupExtension
    {
        private readonly Type _type;
        private object _dataContext;

        public ViewModelContainerExtension(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            if (_type == null)
                throw new NullReferenceException("Type");

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return null;
            try
            {
                _dataContext = IoC.Instance.GetPOCOViewModel(_type);
            }
            catch { return null; }
            return _dataContext;
        }
    }
}
