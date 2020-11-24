namespace EasyDriver.ModbusRTU
{
    public enum AddressType
    {
        Undefined = int.MaxValue,
        OutputCoil = 0,
        InputContact = 1,
        InputRegister = 3,
        HoldingRegister = 4,
    }
}
