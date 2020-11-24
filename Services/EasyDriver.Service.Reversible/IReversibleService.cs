using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyDriver.Service.Reversible
{
    /// <summary>
    /// Cung cấp chức năng đảo ngược hoặc làm lại một hoạt động nào đó
    /// </summary>
    public interface IReversibleService : IEasyServicePlugin
    {
        /// <summary>
        /// Phiên làm việc của dịch vụ này
        /// </summary>
        ReversibleSession Session { get; }

        /// <summary>
        /// Bắt đầu một hoạt động nào đó mà ta muốn nó có thể đảo ngược hoặc làm lại 
        /// </summary>
        /// <returns>Trả về đại diện cho tiến trình xử lý hoạt động</returns>
        Transaction Begin();

        /// <summary>
        /// Bắt đầu một hoạt động nào đó mà ta muốn nó có thể đảo ngược hoặc làm lại 
        /// </summary>
        /// <param name="operationName">Tên của hoạt động đó</param>
        /// <returns>Trả về đại diện cho tiến trình xử lý hoạt động</returns>
        Transaction Begin(string operationName);

        /// <summary>
        /// Hàm đảo ngược lại một bước xảy ra trong <see cref="Transaction"/>
        /// </summary>
        void Undo();

        /// <summary>
        /// Hàm đảo ngược lại một hoặc nhiều bước xảy ra trong <see cref="Transaction"/>
        /// </summary>
        /// <param name="count">Số bước muốn đảo ngược. Tổi thiểu là 1</param>
        void Undo(int count);

        /// <summary>
        /// Hàm làm lại một bước mà ta mới đảo ngược trong <see cref="Transaction"/>
        /// </summary>
        void Redo();

        /// <summary>
        /// Hàm làm lại một hoặc nhiều bước mà ta mới đảo ngược trong <see cref="Transaction"/>
        /// </summary>
        void Redo(int count);

        /// <summary>
        /// Trả về kết quả kiểm tra xem rằng ta có thể thực hiện được <see cref="Undo"/> hay không
        /// </summary>
        /// <returns>True nghĩa là có thể hoặc ngược lại</returns>
        bool CanUndo();

        /// <summary>
        /// Trả về kết quả kiểm tra xem rằng ta có thể thực hiện được <see cref="Redo"/> hay không
        /// </summary>
        /// <returns>True nghĩa là có thể hoặc ngược lại</returns>
        bool CanRedo();

        /// <summary>
        /// Xóa tất cả các hoạt động xảy ra trong phiên làm việc này
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Sự kiện thông báo hoạt động xảy ra trong phiên làm việc thay đổi. 
        /// </summary>
        event EventHandler HistoryChanged;
    }
}
