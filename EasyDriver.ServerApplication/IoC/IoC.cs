﻿using DevExpress.Mvvm.POCO;
using EasyDriver.Core;
using EasyDriver.Opc.Client.Common;
using EasyDriverPlugin;
using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        public IServerBroadcastService ServerBroadcastService => Get<IServerBroadcastService>();
        public ApplicationViewModel ApplicationViewModel => Get<ApplicationViewModel>();
        public List<string> OpcDaServerHosts { get; private set; }
        public string Key { get; set; }

        #endregion

        #region Constructors

        protected IoC()
        {
            Task.Run(() =>
            {
                OpcDaServerHosts = new List<string>();
                OpcServerEnumeratorAuto enumerator = new OpcServerEnumeratorAuto();
                OpcServerDescription[] serverDescriptions = enumerator.Enumerate("", OpcServerCategory.OpcDaServers);
                for (int i = 0; i < serverDescriptions.Length; i++)
                {
                    OpcDaServerHosts.Add(serverDescriptions[i].ProgId);
                }
            });
        }

        #endregion

        #region Methods

        public void Setup()
        {
            Kernel.Bind<IInternalStorageService>().ToConstant(new InternalStorageService());
            Kernel.Bind<IDispatcherFacade>().ToConstant(new DispatcherFacade(Application.Current.Dispatcher));
            Kernel.Bind<IProjectManagerService>().ToConstant(new ProjectManagerService());
            Kernel.Bind<IWorkspaceManagerService>().ToConstant(new WorkspaceManagerService((token) =>
            {
                if (token is IHaveTag haveTags)
                {
                    if (haveTags.HaveTags && haveTags != null)
                    {
                        if (token is ICoreItem coreItem)
                        {
                            if (coreItem.FindParent<IStationCore>(x => x is RemoteStation) is RemoteStation parentStation)
                            {
                                if (haveTags.Tags.Count > 0)
                                    return Kernel.GetPOCOViewModel<TagCollectionViewModel>();
                            }
                            else
                            {
                                return Kernel.GetPOCOViewModel<TagCollectionViewModel>();
                            }
                        }
                        
                    }
                }
                return null;
            }));
            Kernel.BindPOCOViewModel<ApplicationViewModel>().InSingletonScope();
            var applicationViewModel = Kernel.Get<ApplicationViewModel>();
            Kernel.Bind<IHubFactory>().ToConstant(new HubFactory("EasyDriverServerHub"));
            Kernel.Bind<IRemoteConnectionManagerService>().ToConstant(new RemoteConnectionManagerService());
            Kernel.Bind<IDriverManagerService>().ToConstant(new DriverManagerService());
            Kernel.Bind<IReverseService>().ToConstant(new ReverseService());

            Kernel.Bind<IServerBroadcastService>().ToConstant(new ServerBroadcastService(
                this.Get<IProjectManagerService>(), 
                applicationViewModel, 
                applicationViewModel.ServerConfiguration.BroadcastMode,
                applicationViewModel.ServerConfiguration.BroadcastRate));
            Kernel.Bind<ITagWriterService>().ToConstant(new TagWriterService(
                ProjectManagerService,
                Get<IDriverManagerService>(),
                Get<IRemoteConnectionManagerService>(),
                Get<IInternalStorageService>()));
            Kernel.Bind<ILicenseManagerService>().ToConstant(new LicenseManagerService("EasyScada", Get<IProjectManagerService>()));
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
