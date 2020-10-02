using EasyDriver.ServiceContainer;
using System.Collections.Generic;

namespace EasyDriver.MenuPlugin
{
    public interface IEasyMenuPlugin
    {
        IServiceContainer ServiceContainer { get; }
        IEnumerable<IBarComponent> GetExtendContextMenuItems(object context);
        IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context);
        IEnumerable<IBarComponent> GetExtendApplicationToolBarItems(IBarComponent container, object context);
        IEnumerable<IBarComponent> GetExtendApplicationStatusBarItems(IBarComponent container, object context);
        IEnumerable<IBarComponent> GetExtendDocumentMenuItems(IBarComponent container, object context);
        IEnumerable<IBarComponent> GetExtendDocumentToolBarItems(IBarComponent container, object context);
        IEnumerable<IBarComponent> GetExtendDocumentStatusBarItems(IBarComponent container, object context);
    }
}
