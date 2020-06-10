using EasyDriverPlugin;
using System;
using System.Collections.ObjectModel;

namespace EasyScada.Core
{
    /// <summary>
    /// Định nghĩa thông tin lỗi của đối tượng
    /// </summary>
    [Serializable]
    public class ErrorInfo : IErrorInfo
    {
        #region Members

        /// <summary>
        /// Tên của thuộc tính chứa lỗi này
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Danh sách các lỗi con
        /// </summary>
        public ObservableCollection<IErrorInfo> Childs { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Tin của lỗi
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Kiểu lỗi
        /// </summary>
        public ErrorMessageType MessageType { get; set; }

        /// <summary>
        /// Đối tượng sở hữu lỗi này
        /// </summary>
        public object Owner { get; set; }

        #endregion

        #region Constructors

        public static IErrorInfo Create() => new ErrorInfo();

        internal ErrorInfo() => Childs = new ObservableCollection<IErrorInfo>();

        #endregion

        #region Methods

        /// <summary>
        /// Clone đối tượng này
        /// </summary>
        /// <returns></returns>
        public IErrorInfo Clone()
        {
            IErrorInfo result = MemberwiseClone() as IErrorInfo;
            result.Childs = new ObservableCollection<IErrorInfo>();
            return result;
        }

        /// <summary>
        /// Đếm số lỗi có trong danh sách lỗi con
        /// </summary>
        /// <returns></returns>
        public int CountErrors()
        {
            int errors = 0;
            if (MessageType == ErrorMessageType.Error)
                errors++;
            foreach (var item in Childs)
                errors += item.CountErrors();
            return errors;
        }

        /// <summary>
        /// Đếm số cảnh báo có trong danh sách lỗi con
        /// </summary>
        /// <returns></returns>
        public int CountWarning()
        {
            int warnings = 0;
            if (MessageType == ErrorMessageType.Warning)
                warnings++;
            foreach (var item in Childs)
                warnings += item.CountWarning();
            return warnings;
        }

        /// <summary>
        /// Cập nhật các thuộc tính của lỗi này bằng 1 <see cref="IErrorInfo"/> khác
        /// </summary>
        /// <param name="errorInfo"></param>
        public void Replace(IErrorInfo errorInfo)
        {
            if (errorInfo != null)
            {
                PropertyName = errorInfo.PropertyName;
                ErrorCode = errorInfo.ErrorCode;
                Message = errorInfo.Message;
                MessageType = errorInfo.MessageType;
            }
        }

        /// <summary>
        /// Tạo <see cref="IErrorInfo"/> mới và thêm vào danh sách con
        /// </summary>
        /// <returns></returns>
        public IErrorInfo CreateAndAddToChilds(object owner = null)
        {
            var errorInfo = new ErrorInfo() { Owner = owner };
            Childs.Add(errorInfo);
            return errorInfo;
        }

        /// <summary>
        /// Xóa tất cả thông tin của lỗi
        /// </summary>
        public void Clear()
        {
            Childs.Clear();
            PropertyName = string.Empty;
            ErrorCode = 0;
            Message = string.Empty;
            Owner = null;
            MessageType = ErrorMessageType.None;
        }

        #endregion
    }
}
