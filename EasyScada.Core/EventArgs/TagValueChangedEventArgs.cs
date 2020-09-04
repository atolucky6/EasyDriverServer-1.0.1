using System;

namespace EasyScada.Core
{
    /// <summary>
    /// Thàm số của sự kiện giá trị của Tag thay đổi
    /// </summary>
    [Serializable]
    public class TagValueChangedEventArgs : EventArgs
    {
        #region Members

        /// <summary>
        /// Tag xảy ra sự kiện này
        /// </summary>
        public ITag Tag { get; private set; }

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

        public TagValueChangedEventArgs(ITag tag, string oldValue, string newValue)
        {
            Tag = tag;
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion
    }
}
