using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyScada.ServerApplication
{
    public class IoC
    {
        #region Singleton

        public static IoC Instance { get; } = new IoC();

        #endregion

        #region Members

        public IKernel Kernel { get; private set; } = new StandardKernel();
        public IList<Type> RegisteredTypes { get; private set; } = new List<Type>();
        public IProjectManagerService ProjectManagerService => Get<IProjectManagerService>();

        #endregion

        #region Constructors

        protected IoC()
        {

        }

        #endregion

        #region Methods

        public void Setup()
        {
            Kernel.Bind<IDriverManagerService>().ToConstant(new DriverManagerService());
            Kernel.Bind<IProjectManagerService>().ToConstant(new ProjectManagerService());
            Kernel.Bind<IReverseService>().ToConstant(new ReverseService());
            Kernel.Bind<IWorkspaceManagerService>().ToConstant(new WorkspaceManagerService((token) =>
            {
                if (token is IDeviceCore)
                    return Kernel.GetPOCOViewModel<TagCollectionViewModel>();
                return null;
            }));
        }

        public T Get<T>()
        {
            return Kernel.Get<T>();
        }

        public object GetPOCOViewModel(Type type)
        {
            if (!RegisteredTypes.Contains(type))
            {
                Kernel.Bind(type).To(GetPOCOType(type));
                RegisteredTypes.Add(type);
            }
            return Kernel.Get(type);
        }

        public T GetPOCOViewModel<T>()
            where T : class
        {
            if (!RegisteredTypes.Contains(typeof(T)))
            {
                Kernel.Bind(typeof(T)).To(GetPOCOType(typeof(T)));
                RegisteredTypes.Add(typeof(T));
            }
            return (T)Kernel.Get(typeof(T));
        }

        public T GetPOCOViewModel<T>(object parentViewModel)
            where T : class
        {
            if (!RegisteredTypes.Contains(typeof(T)))
            {
                Kernel.Bind(typeof(T)).To(GetPOCOType(typeof(T)));
                RegisteredTypes.Add(typeof(T));
            }
            return (T)Kernel.Get(typeof(T)).SetParentViewModel(parentViewModel);
        }

        public void BindToPOCOViewModel<T>(IKernel kernel)
            where T : class
        {
            kernel.Bind<T>().To(GetPOCOType<T>());
            RegisteredTypes.Add(typeof(T));
        }

        public Type GetPOCOType<T>()
            where T : class
        {
            return ViewModelSource.GetPOCOType(typeof(T));
        }

        public Type GetPOCOType(Type type)
        {
            return ViewModelSource.GetPOCOType(type);
        }

        #endregion
    }

    public static class InjectionExtensions
    {
        public static Type GetPOCOViewModelType(this Type type) => ViewModelSource.GetPOCOType(type);
        public static Type GetPOCOViewModelType<T>() => ViewModelSource.GetPOCOType(typeof(T));

        public static IBindingWhenInNamedWithOrOnSyntax<T> BindPOCOViewModel<T>(this IKernel kernal)
        {
            return kernal.Bind<T>().To(GetPOCOViewModelType<T>());
        }

        public static T GetPOCOViewModel<T>(this IKernel kernal)
        {
            if (kernal.GetBindings(typeof(T)).Count() <= 0)
                kernal.BindPOCOViewModel<T>();
            return kernal.Get<T>();
        }

        public static object GetPOCOViewModel(this IKernel kernal, Type type)
        {
            if (kernal.GetBindings(type).Count() <= 0)
                kernal.Bind(type).To(GetPOCOViewModelType(type));
            return kernal.Get(type);
        }
    }
}
