using DevExpress.Xpf.Docking;
using EasyDriver.ServicePlugin;
using System.Collections.Generic;

namespace EasyDriver.LayoutManager
{
    public interface ILayoutManagerService : IEasyServicePlugin
    {
        DockLayoutManager DockLayoutManager { get; set; }
        string SaveLayoutPath { get; set; }
        string ActualSaveLayoutPath { get; }

        void SaveLayout(string layoutName);
        void RemoveLayout(string layoutName);
        void ResetToDefault();
        void ApplyLayout(string layoutName);
        bool RestoreLastLayout();

        void UpdateLastLayout();
        void UpdateDefaultLayout();

        IEnumerable<string> GetLayouts();
    }
}
