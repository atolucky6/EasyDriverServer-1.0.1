using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.Service.LayoutManager
{
    /// <summary>
    /// Đối tượng quản lý layout cho application
    /// </summary>
    public interface ILayoutManagerService : IEasyServicePlugin
    {
        /// <summary>
        /// Lưu layout vào thư mục {ApplicationBaseDir}/Layouts/{LayoutName}
        /// </summary>
        /// <param name="layoutName">Tên của layout</param>
        void SaveLayout(string layoutName);

        /// <summary>
        /// Xóa layout
        /// </summary>
        /// <param name="layoutName"></param>
        void RemoveLayout(string layoutName);

        /// <summary>
        /// Phục hồi lại layout mặc định
        /// </summary>
        void ResetToDefault();

        /// <summary>
        /// Áp dụng layout
        /// </summary>
        /// <param name="layoutName"></param>
        void ApplyLayout(string layoutName);

        /// <summary>
        /// Sử dụng layout lần cuối cùng sử dụng
        /// </summary>
        /// <returns></returns>
        void RestoreLastLayout();

        /// <summary>
        /// Lưu layout hiện tại thành layout sử dụng cuối cùng 
        /// </summary>
        void UpdateLastLayout();

        /// <summary>
        /// Lưu layout hiện tại thành layout mặc định
        /// </summary>
        void UpdateDefaultLayout();

        /// <summary>
        /// Lấy tất cả layouts
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetLayouts();
    }
}
