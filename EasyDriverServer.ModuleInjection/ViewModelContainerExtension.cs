using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace EasyDriverServer.ModuleInjection
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
                _dataContext = IoC.Instance.RegisterAndGetPOCOViewModel(_type, _isUnique);
            }
            catch { return null; }
            return _dataContext;
        }
    }
}
