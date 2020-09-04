using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    [Serializable]
    public class ConnectionSchema
    {
        public string ServerAddress { get; set; }
        public ushort Port { get; set; }
        public CommunicationMode CommunicationMode { get; set; }
        public int RefreshRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Station> Stations { get; set; }
        [field: NonSerialized]
        event EventHandler<TagValueChangedEventArgs> TagValueChanged;
        [field: NonSerialized]
        event EventHandler<TagQualityChangedEventArgs> TagQualityChanged;

        internal void RaiseTagValueChanged(object sender, TagValueChangedEventArgs e)
        {
            TagValueChanged?.Invoke(sender, e);
        }

        internal void RaiseTagQualityChanged(object sender, TagQualityChangedEventArgs e)
        {
            TagQualityChanged?.Invoke(sender, e);
        }

        internal void SetParentForChild()
        {
            foreach (var item in Stations)
            {
                item.SetParentForChild(this);
            }
        }
    }
}
