using EasyDriverServer.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace EasyDriverServer.View
{
    public class ViewModelContainerExtension : MarkupExtension
    {
        private readonly Type _type;
        private readonly bool _isUnique;
        private object _dataContext;

        public ViewModelContainerExtension(Type type, bool isUnique)
        {
            _type = type;
            _isUnique = isUnique;
        }

        public ViewModelContainerExtension(Type type)
        {
            _type = type;
            _isUnique = false;
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
                IoC.Instance.BindToPOCOViewModel(_type, _isUnique);
                _dataContext = IoC.Instance.GetPOCOViewModel(_type);
            }
            catch { return null; }
            return _dataContext;
        }
    }
}
