using System;

namespace EasyDriverPlugin
{
    /// <summary>
    /// Tham số của sự kiện trạng thái Tag thay đổi
    /// </summary>
    public class TagQualityChangedEventArgs : EventArgs
    {
        #region Members

        /// <summary>
        /// Trạng thái cũ
        /// </summary>
        public Quality OldQuality { get; private set; }

        /// <summary>
        /// Trạng thái mới
        /// </summary>
        public Quality NewQuality { get; private set; }

        #endregion

        #region Constructors

        public TagQualityChangedEventArgs(Quality oldQuality, Quality newQuality)
        {
            OldQuality = oldQuality;
            NewQuality = newQuality;
        }

        #endregion
    }
}
