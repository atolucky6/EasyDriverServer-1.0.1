using System.Collections.ObjectModel;

namespace EasyDriver.ModbusRTU
{
    public class DeviceBlockSetting
    {
        public virtual ObservableCollection<ReadBlockSetting> InputContactReadBlockSettings { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> OutputCoilsReadBlockSettings { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> InputRegisterReadBlockSettings { get; set; }
        public virtual ObservableCollection<ReadBlockSetting> HoldingRegisterReadBlockSettings { get; set; }
    }
}
