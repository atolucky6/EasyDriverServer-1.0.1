using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EasyDriverPlugin
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// Load assembly and create instance with non argrument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadAndCreateInstance<T>(string path)
            where T : class
        {
            try
            {
                string driverPath = "";
                if (path.EndsWith(".dll"))
                {
                    driverPath = path;
                    if (!File.Exists(driverPath))
                        return null;
                }
                else
                {
                    driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Driver\\" + path + ".dll";
                }

                if (driverPath.EndsWith(".dll") && File.Exists(driverPath))
                {
                    Type interfaceType = typeof(T);
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    string fullPath = Path.GetFullPath(driverPath);
                    Assembly loadedAssembly = null;
                    foreach (var assembly in assemblies)
                    {
                        try
                        {
                            if (!assembly.IsDynamic)
                            {
                                if (assembly.Location == fullPath)
                                {
                                    loadedAssembly = assembly;
                                    break;
                                }
                            }
                        }
                        catch { }
                    }
                    if (loadedAssembly == null)
                        loadedAssembly = Assembly.LoadFile(fullPath);
                    Type instanceType = loadedAssembly.GetTypes().FirstOrDefault(t => interfaceType.IsAssignableFrom(t) && t.IsClass);
                    return (T)Activator.CreateInstance(instanceType);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// Load assembly and create instance with argruments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadAndCreateInstance<T>(string path, params object[] args)
            where T : class
        {
            try
            {
                if (path.EndsWith(".dll") && File.Exists(path))
                {
                    Type interfaceType = typeof(T);
                    Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.Location == Path.GetFullPath(path));
                    if (loadedAssembly == null)
                        loadedAssembly = Assembly.LoadFile(Path.GetFullPath(path));
                    Type instanceType = loadedAssembly.GetTypes().FirstOrDefault(t => interfaceType.IsAssignableFrom(t) && t.IsClass);
                    return (T)Activator.CreateInstance(instanceType, args);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
