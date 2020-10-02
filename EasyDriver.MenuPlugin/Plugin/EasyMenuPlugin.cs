using System.Collections.Generic;
using EasyDriver.ServiceContainer;

namespace EasyDriver.MenuPlugin
{
    public abstract class EasyMenuPlugin : IEasyMenuPlugin
    {
        public virtual IServiceContainer ServiceContainer { get; protected set; }

        public EasyMenuPlugin(IServiceContainer serviceContainer)
        {
            ServiceContainer = serviceContainer;
        }

        public virtual IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context)
        {
            return null; 
        }

        public virtual IEnumerable<IBarComponent> GetExtendApplicationStatusBarItems(IBarComponent container, object context)
        {
            return null;
        }

        public virtual IEnumerable<IBarComponent> GetExtendApplicationToolBarItems(IBarComponent container, object context)
        {
            return null;
        }

        public virtual IEnumerable<IBarComponent> GetExtendContextMenuItems(object context)
        {
            return null;
        }

        public virtual IEnumerable<IBarComponent> GetExtendDocumentMenuItems(IBarComponent container, object context)
        {
            return null;
        }

        public virtual IEnumerable<IBarComponent> GetExtendDocumentStatusBarItems(IBarComponent container, object context)
        {
            return null;
        }

        public virtual IEnumerable<IBarComponent> GetExtendDocumentToolBarItems(IBarComponent container, object context)
        {
            return null;
        }
    }
}
