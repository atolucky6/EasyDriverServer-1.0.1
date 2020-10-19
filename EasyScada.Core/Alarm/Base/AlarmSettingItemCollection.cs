using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class AlarmSettingItemCollection<T> : UniqueItemCollection<T> where T : AlarmSettingItemBase
    {
        public AlarmSetting AlarmSetting { get; set; }

        public AlarmSettingItemCollection(AlarmSetting alarmSetting) : base()
        {
            AlarmSetting = alarmSetting;
        }

        protected override void InsertItem(int index, T item)
        {
            if (item != null)
            {
                item.AlarmSetting = AlarmSetting;
                base.InsertItem(index, item);
                item.AlarmStateChanged += Item_AlarmStateChanged;
            }

        }

        protected override void RemoveItem(int index)
        {
            var removeAlarm = Items[index];
            removeAlarm.AlarmStateChanged -= Item_AlarmStateChanged;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            foreach (var item in Items)
                item.AlarmStateChanged -= Item_AlarmStateChanged;
            base.ClearItems();
        }

        protected override void SetItem(int index, T item)
        {
            item.AlarmSetting = AlarmSetting;
            var currentItem = Items[index];
            currentItem.AlarmStateChanged -= Item_AlarmStateChanged;
            base.SetItem(index, item);
            item.AlarmStateChanged += Item_AlarmStateChanged;
        }

        private void Item_AlarmStateChanged(object sender, AlarmStateChangedEventArgs e)
        {
            AlarmSetting?.RaiseAlarmStateChanged(sender, e);
        }
    }
}
