using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DevExpress.Xpf.Docking;
using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;

namespace EasyDriver.LayoutManager
{
    public class LayoutManagerService : EasyServicePlugin, ILayoutManagerService
    {
        #region Members

        public string SaveLayoutPath { get; set; }
        public string ActualSaveLayoutPath { get => GetActualSaveLayoutPath(); }
        public DockLayoutManager DockLayoutManager { get; set; }
        string CurrentApplicationPath { get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }

        #endregion

        #region Constructors

        public LayoutManagerService(IServiceContainer serviceContainer) : base(serviceContainer)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Cài đặt giao diện
        /// </summary>
        /// <param name="layoutName"></param>
        public void ApplyLayout(string layoutName)
        {
            var layoutPath = GetSaveLayoutPath(layoutName);
            if (File.Exists(layoutPath))
                DockLayoutManager.RestoreLayoutFromXml(layoutPath);
        }

        /// <summary>
        /// Lấy tất cả các tên của Layout trong thư mục lưu layout
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLayouts()
        {
            CreateSaveLayoutPathIfNotExists();
            foreach (var filePath in Directory.GetFiles(GetActualSaveLayoutPath(), "*.xml"))
            {
                yield return Path.GetFileNameWithoutExtension(filePath);
            }
        }

        /// <summary>
        /// Xóa giao diện
        /// </summary>
        /// <param name="layoutName"></param>
        public void RemoveLayout(string layoutName)
        {
            string layoutPath = GetSaveLayoutPath(layoutName);
            if (File.Exists(layoutPath))
                File.Delete(layoutPath);
        }

        /// <summary>
        /// Phục hồi lại giao diện mặc định
        /// </summary>
        public void ResetToDefault()
        {
            string layoutPath = $"{CurrentApplicationPath}/DefaultLayout.xml";
            if (File.Exists(layoutPath))
                DockLayoutManager.RestoreLayoutFromXml(layoutPath);
        }

        /// <summary>
        /// Phục hồi lại giao diện như lúc lần cuối cùng tắt ứng dụng
        /// </summary>
        /// <returns></returns>
        public bool RestoreLastLayout()
        {
            try
            {
                string layoutPath = $"{CurrentApplicationPath}/LastLayout.xml";
                if (File.Exists(layoutPath))
                {
                    DockLayoutManager.RestoreLayoutFromXml(layoutPath);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Lưu giao diện
        /// </summary>
        /// <param name="layoutName"></param>
        public void SaveLayout(string layoutName)
        {
            CreateSaveLayoutPathIfNotExists();
            DockLayoutManager.SaveLayoutToXml(GetSaveLayoutPath(layoutName));
        }

        /// <summary>
        /// Cài đặt giao diện hiện tại làm giao diện mặc định
        /// </summary>
        public void UpdateDefaultLayout()
        {
            DockLayoutManager.SaveLayoutToXml($"{CurrentApplicationPath}/DefaultLayout.xml");
        }

        /// <summary>
        /// Cài đặt giao diện hiện tại là giao diện cuối
        /// </summary>
        public void UpdateLastLayout()
        {
            DockLayoutManager.SaveLayoutToXml($"{CurrentApplicationPath}/LastLayout.xml");
        }

        /// <summary>
        /// Hàm lấy đường dẫn thư mục lưu layout
        /// </summary>
        /// <returns></returns>
        string GetActualSaveLayoutPath()
        {
            if (!string.IsNullOrEmpty(SaveLayoutPath) || Directory.Exists(SaveLayoutPath))
                return SaveLayoutPath;
            else
                return $"{CurrentApplicationPath}/Layouts";
        }

        /// <summary>
        /// Hàm lấy đường dẫn lưu layout
        /// </summary>
        /// <param name="layoutName"></param>
        /// <returns></returns>
        string GetSaveLayoutPath(string layoutName)
        {
            return $"{GetActualSaveLayoutPath()}/{layoutName}.xml";
        }

        /// <summary>
        /// Hàm tạo đường dẫn lưu layout nếu nó chưa tồn tại
        /// </summary>
        void CreateSaveLayoutPathIfNotExists()
        {
            if (!Directory.Exists(GetActualSaveLayoutPath()))
                Directory.CreateDirectory(GetActualSaveLayoutPath());
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }

        #endregion
    }
}
