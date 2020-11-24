using EasyDriver.ServicePlugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyDriverServer.ModuleInjection
{
    public abstract class ModuleBase<T> : ISupportInitialize where  T : ISupportInitialize
    {
        public string ModuleName { get; protected set; }
        public Dictionary<int, Type> InitializeTypes { get; protected set; }
        public string ModuleDirectory { get; protected set; }
        public ServiceCollection Services { get; set; }

        public ModuleBase(string moduleDirectory, ServiceCollection services)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Services = services;
            ModuleName = GetType().Name;
            InitializeTypes = new Dictionary<int, Type>();
            ModuleDirectory = moduleDirectory;

            if (Directory.Exists(moduleDirectory))
            {
                Log("Begin find modules...");
                string[] paths = Directory.GetFiles(moduleDirectory, "*.dll");
                Log($"{paths.Length} module was found");
                foreach (var path in paths)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(path);
                        InitializeAssembly(assembly);
                    }
                    catch (Exception ex) 
                    {
                        Log($"Load module {Path.GetFileName(path)} fail: {ex.ToString()}");
                    }
                }
            }
            else { Log($"Directory '{moduleDirectory}' doesn't exist"); }

            sw.Stop();
            Log($"Intialize took: {sw.ElapsedMilliseconds}");
        }

        public virtual void BeginInit()
        {
            foreach (var item in InitializeTypes.OrderByDescending(x => x.Key))
            {

                ((T)IoC.Instance.ServiceProvider.GetService(item.Value)).BeginInit();
                Log($"Begin init {item.Value.GetType().Name}");
            }
        }

        public virtual void EndInit()
        {
            foreach (var item in InitializeTypes.OrderByDescending(x => x.Key))
            {
                ((T)IoC.Instance.ServiceProvider.GetService(item.Value)).EndInit();
                Log($"End init {item.Value.GetType().Name}");
            }
        }

        protected virtual void Log(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} -> {ModuleName}: {message}");
        }

        protected virtual void InitializeAssembly(Assembly assembly)
        {
            Type baseType = typeof(T);
            foreach (var instanceType in assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(instanceType) && instanceType.IsClass)
                {
                    Type bindType = null;
                    foreach (var interfaceType in instanceType.GetInterfaces())
                    {
                        if (interfaceType.Name == $"I{instanceType.Name}")
                        {
                            bindType = interfaceType;
                            break;
                        }
                    }

                    if (bindType != null)
                    {
                        ServiceAttribute seviceAttribute = instanceType.GetCustomAttribute(typeof(ServiceAttribute)) as ServiceAttribute;
                        if (seviceAttribute != null)
                        {
                            bool isUnique = seviceAttribute.IsUnique;
                            int intializePiority = seviceAttribute.InitializePiority;

                            if (isUnique)
                            {
                                do
                                {
                                    if (InitializeTypes.ContainsKey(intializePiority))
                                    {
                                        intializePiority--;
                                    }
                                    else
                                    {
                                        Services.AddSingleton(bindType, instanceType);
                                        InitializeTypes.Add(intializePiority, bindType);
                                        break;
                                    }
                                }
                                while (true);
                            }
                            else
                            {
                                Services.AddTransient(bindType, instanceType);
                            }

                            Log($"Load {instanceType.Name} success. IsUnique = {isUnique}, InitializePiority = {intializePiority}");
                        }
                        else
                        {
                            Log($"Load '{instanceType.Name}' fail need implement ServiceAttribute");
                        }
                        break;
                    }
                }
            }
        }
    }
}
