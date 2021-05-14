using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using System.Collections.ObjectModel;

namespace EasyDriver.ModbusTCP
{
    public class ReadBlockSettingViewModel
    {
        #region Public properties
        public virtual AddressType AddressType { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> ReadBlockSettingSource { get; set; }
        public virtual ReadBlockSetting SelectedSetting { get; set; }
        #endregion

        #region Services
        protected ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }
        protected IMessageBoxService MessageBoxService { get => this.GetService<IMessageBoxService>(); }
        #endregion

        #region Constructors
        public ReadBlockSettingViewModel(AddressType addressType, ObservableCollection<ReadBlockSetting> source)
        {
            AddressType = addressType;
            ReadBlockSettingSource = source;
        }
        #endregion

        #region Commands
        public void AddSetting()
        {
            ReadBlockSetting newSetting = new ReadBlockSetting() { AddressType = this.AddressType };
            ReadBlockSettingSource.Add(newSetting);
        }
        public bool CanAddSetting()
        {
            return ReadBlockSettingSource != null;
        }

        public void RemoveSetting()
        {
            ReadBlockSettingSource.Remove(SelectedSetting);
        }

        public bool CanRemoveSetting()
        {
            return SelectedSetting != null;
        }

        public void MoveUpSetting()
        {
            int oldIndex = ReadBlockSettingSource.IndexOf(SelectedSetting);
            int newIndex = oldIndex - 1;
            if (newIndex >= 0)
                ReadBlockSettingSource.Move(oldIndex, newIndex);
        }

        public bool CanMoveUpSetting()
        {
            return SelectedSetting != null;
        }

        public void MoveDownSetting()
        {
            int oldIndex = ReadBlockSettingSource.IndexOf(SelectedSetting);
            int newIndex = oldIndex + 1;
            if (newIndex < ReadBlockSettingSource.Count)
                ReadBlockSettingSource.Move(oldIndex, newIndex);
        }

        public bool CanMoveDownSetting()
        {
            return SelectedSetting != null;
        }

        public void ClearSetting()
        {
            var mbr = MessageBoxService.ShowMessage("Do you want to clear all settings?", "Message", MessageButton.YesNo, MessageIcon.Question);
            if (mbr == MessageResult.Yes)
            {
                ReadBlockSettingSource.Clear();
            }
        }

        public bool CanClearSetting()
        {
            return ReadBlockSettingSource != null && ReadBlockSettingSource.Count > 0;
        }
        #endregion
    }
}
