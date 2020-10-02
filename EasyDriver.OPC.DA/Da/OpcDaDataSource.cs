using EasyDriver.Opc.DA.Client.Interop.Da;

namespace EasyDriver.Opc.DA.Client.Da
{
    /// <summary>
    /// Represents data source to retrieving data.
    /// </summary>
    public enum OpcDaDataSource
    {
        /// <summary>
        /// The cache.
        /// </summary>
        Cache = OPCDATASOURCE.OPC_DS_CACHE,

        /// <summary>
        /// The device.
        /// </summary>
        Device = OPCDATASOURCE.OPC_DS_DEVICE
    }
}