using System;

namespace EasyDriver.Client.Models
{
    /// <summary>
    /// Thàm số của sự kiện giá trị của Tag thay đổi
    /// </summary>
    public class TagValueChangedEventArgs : EventArgs
    {
        #region Members

        /// <summary>
        /// Giá trị cũ
        /// </summary>
        public string OldValue { get; private set; }

        /// <summary>
        /// Giá trị mới
        /// </summary>
        public string NewValue { get; private set; }

        #endregion

        #region Constructors

        public TagValueChangedEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion
    }
}
