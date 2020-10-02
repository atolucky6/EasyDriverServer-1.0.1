using DevExpress.Mvvm.POCO;
using EasyDriver.LayoutManager;
using EasyDriver.MenuPlugin;
using EasyDriver.ProjectManager;
using EasyDriver.Reversible;
using EasyDriver.SyncContext;
using EasyDriver.WorkspaceManager;
using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasyDriverServer.ViewModel
{
    public class IoC
    {
        #region Singleton

        public static IoC Instance { get; } = new IoC();

        #endregion

        #region Members

        internal IKernel Kernel { get; private set; } = new StandardKernel();
        internal IList<Type> RegisteredTypes { get; private set; } = new List<Type>();
        //public IProjectManagerService ProjectManagerService => Get<IProjectManagerService>();
        //public IServerBroadcastService ServerBroadcastService => Get<IServerBroadcastService>();
        //public ApplicationViewModel ApplicationViewModel => Get<ApplicationViewModel>();
        public List<string> OpcDaServerHosts { get; private set; }
        public string Key { get; set; }

        #endregion

        #region Constructors

        protected IoC()
        {
            //Task.Run(() =>
            //{
            //    OpcDaServerHosts = new List<string>();
            //    OpcServerEnumeratorAuto enumerator = new OpcServerEnumeratorAuto();
            //    OpcServerDescription[] serverDescriptions = enumerator.Enumerate("", OpcServerCategory.OpcDaServers);
            //    for (int i = 0; i < serverDescriptions.Length; i++)
            //    {
            //        OpcDaServerHosts.Add(serverDescriptions[i].ProgId);
            //    }
            //});
        }

        #endregion

        #region Methods

        public void Setup()
        {
            Kernel.Bind<IBarFactory>().ToConstant(BarFactory.Default);

            // Khởi tạo service container
            Kernel.Bind<EasyDriver.ServiceContainer.IServiceContainer>().ToConstant(new EasyDriver.ServiceContainer.ServiceContainer(Get));
            //Kernel.Bind<EasyDriver.ServiceContainer.IServiceContainer>().ToConstant(new EasyDriver.ServiceContainer.ServiceContainer((t) =>
            //{
            //    if (t == null)
            //        return null;


            //    string typeName = t.Name;
            //    var serviceDic = Get<ServiceModule>().ServiceDictionary;
            //    foreach (var kvp in serviceDic)
            //    {
            //        if (kvp.Key.Name == typeName)
            //        {
            //            return Kernel.Get(kvp.Key);
            //        }
            //    }
            //    return null;
            //}));

            // Khởi tạo các service đầu tiên
            BindToPOCOViewModel<ServiceModule>(true);
            GetPOCOViewModel<ServiceModule>();

            // Khởi tạo các menu
            BindToPOCOViewModel<MenuModule>(true);
            GetPOCOViewModel<MenuModule>();

            // Khởi tạo các workspace
            BindToPOCOViewModel<WorkspaceModule>(true);
            GetPOCOViewModel<WorkspaceModule>();

            // Chạy EndInit sau khi khởi tạo tất các các module
            foreach (var service in Get<ServiceModule>().Services)
                service.EndInit();
        }

        public T Get<T>()
        {
            return Kernel.Get<T>();
        }

        public object Get(Type type)
        {
            return Kernel.Get(type);
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

        public void BindToPOCOViewModel<T>(bool isUnique = false)
            where T : class
        {
            if (isUnique)
                Kernel.Bind<T>().To(GetPOCOType<T>()).InSingletonScope();
            else
                Kernel.Bind<T>().To(GetPOCOType<T>());
            RegisteredTypes.Add(typeof(T));
        }

        public void BindToPOCOViewModel(Type type, bool isUnique = false)
        {
            if (isUnique)
                Kernel.Bind(type).To(GetPOCOType(type)).InSingletonScope();
            else
                Kernel.Bind(type).To(GetPOCOType(type));
            RegisteredTypes.Add(type);
        }

        public void BindToConstant<T>(T obj)
        {
            Kernel.Bind<T>().ToConstant(obj);
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
