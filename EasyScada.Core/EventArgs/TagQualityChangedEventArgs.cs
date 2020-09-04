using System;

namespace EasyScada.Core
{
    /// <summary>
    /// Tham số của sự kiện trạng thái Tag thay đổi
    /// </summary>
    [Serializable]
    public class TagQualityChangedEventArgs : EventArgs
    {
        #region Members

        /// <summary>
        /// Tag xảy ra sự kiện này
        /// </summary>
        public ITag Tag { get; private set; }

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

        public TagQualityChangedEventArgs(ITag tag, Quality oldQuality, Quality newQuality)
        {
            Tag = tag;
            OldQuality = oldQuality;
            NewQuality = newQuality;
        }

        #endregion
    }
}
