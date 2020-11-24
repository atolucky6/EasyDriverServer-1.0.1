using EasyDriver.ServicePlugin;
using EasyDriver.WorkspacePlugin;
using EasyScada.WorkspaceManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyDriverServer.ModuleInjection
{
    public class WorkspaceModule : ModuleBase<WorkspacePanelBase>
    {
        protected IWorkspaceManagerService WorkspaceManagerService { get; set; }

        public WorkspaceModule(string moduleDirectory, ServiceCollection services) : base(moduleDirectory, services)
        {
        }

        protected override void InitializeAssembly(Assembly assembly)
        {
            Type baseType = typeof(WorkspacePanelBase);
            foreach (var bindType in assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(bindType) && bindType.IsClass && !bindType.IsAbstract)
                {
                    Type instanceType = bindType.GetPOCOViewModelType();
                    if (instanceType != null)
                    {
                        ServiceAttribute serviceAttribute = instanceType.GetCustomAttribute(typeof(ServiceAttribute)) as ServiceAttribute;
                        if (serviceAttribute != null)
                        {
                            bool isUnique = serviceAttribute.IsUnique;
                            int intializePiority = serviceAttribute.InitializePiority;

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
                            Log($"Load '{instanceType.Name}' fail, need implement ServiceAttribute");
                        }
                    }
                }
            }
        }

        public override void EndInit()
        {
            base.EndInit();
            WorkspaceManagerService = IoC.Instance.Get<IWorkspaceManagerService>();
            foreach (var item in InitializeTypes.OrderByDescending(x => x.Key))
            {
                WorkspaceManagerService.Workspaces.Add((WorkspacePanelBase)IoC.Instance.ServiceProvider.GetService(item.Value));
            }
        }
    }
}
