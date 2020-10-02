using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriverServer.ViewModel
{
    public class ServiceModule
    {
        public ServiceModule()
        {
            string serviceDir = ApplicationHelper.GetApplicationPath() + "\\Services\\";
            Services = new List<IEasyServicePlugin>();
            ServiceDictionary = new Dictionary<Type, IEasyServicePlugin>();

            if (Directory.Exists(serviceDir))
            {
                var servicePaths = Directory.GetFiles(serviceDir, "*.dll").Select(x => Path.GetFileName(x)).ToList();
                string[] localServicePaths = Directory.GetFiles($"{ApplicationHelper.GetApplicationPath()}\\", "*.dll")
                    .Where(x => servicePaths.Contains(Path.GetFileName(x))).ToArray();

                List<string> assemblyFullNames = new List<string>();
                foreach (var servicePath in localServicePaths)
                {
                    string fullPath = Path.GetFullPath(servicePath);
                    Assembly loadedAssembly = Assembly.LoadFile(fullPath);
                    loadedAssembly = AppDomain.CurrentDomain.Load(loadedAssembly.GetName());
                    assemblyFullNames.Add(loadedAssembly.FullName);
                }

                Type serviceType = typeof(IEasyServicePlugin);
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (assemblyFullNames.Contains(assembly.FullName))
                        {
                            foreach (Type type in assembly.GetTypes())
                            {
                                Type instanceType = null;
                                if (serviceType.IsAssignableFrom(type) && type.IsClass)
                                {
                                    instanceType = type;
                                    InitializeService(instanceType);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public Dictionary<Type, IEasyServicePlugin> ServiceDictionary { get; private set; }
        public List<IEasyServicePlugin> Services { get; private set; }

        private void InitializeService(Type instanceType)
        {
            Type serviceType = typeof(IEasyServicePlugin);
            string instanceName = null;
            Type bindInterfaceType = null;

            if (instanceType != null)
            {
                instanceName = instanceType.Name; ;
                foreach (var interfaceType in instanceType.GetInterfaces())
                {
                    if (interfaceType.Name == $"I{instanceName}" && interfaceType != serviceType)
                    {
                        bindInterfaceType = interfaceType;
                        break;
                    }
                }
            }

            if (bindInterfaceType != null)
            {
                if (!IoC.Instance.RegisteredTypes.Contains(bindInterfaceType))
                {
                    IoC.Instance.Kernel.Bind(bindInterfaceType).To(instanceType);
                    if (IoC.Instance.Get(bindInterfaceType) is IEasyServicePlugin servicePlugin)
                    {
                        if (servicePlugin.IsUnique)
                            IoC.Instance.Kernel.Rebind(bindInterfaceType).ToConstant(servicePlugin);

                        servicePlugin = IoC.Instance.Get(bindInterfaceType) as IEasyServicePlugin;
                        servicePlugin.BeginInit();
                        ServiceDictionary[bindInterfaceType] = servicePlugin;
                        Services.Add(servicePlugin);
                    }
                }
            }
        }
    }
}
