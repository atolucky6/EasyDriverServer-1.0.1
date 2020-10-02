using EasyDriver.Opc.DA.Client.Interop.Da;

namespace EasyDriver.Opc.DA.Client.Da
{
    /// <summary>
    /// Represents OPC DA namespace type enumeration.
    /// </summary>
    public enum OpcDaNamespaceType
    {
        /// <summary>
        /// The hierarchial namespace.
        /// </summary>
        Hierarchial = OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL,

        /// <summary>
        /// The flat namespace.
        /// </summary>
        Flat = OPCNAMESPACETYPE.OPC_NS_FLAT
    }
}