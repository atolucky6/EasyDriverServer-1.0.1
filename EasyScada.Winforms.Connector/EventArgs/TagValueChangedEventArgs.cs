﻿using System;

namespace EasyScada.Winforms.Connector
{
    /// <summary>
    /// Thàm số của sự kiện giá trị của Tag thay đổi
    /// </summary>
    [Serializable]
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
