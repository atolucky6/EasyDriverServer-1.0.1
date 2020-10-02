using System;
using EasyDriver.Opc.DA.Client.Interop.Da;

namespace EasyDriver.Opc.DA.Client.Da.Browsing
{
    /// <summary>
    /// Represents OPC DA browse filter flags.
    /// </summary>
    [Flags]
    public enum OpcDaBrowseFilter
    {
        /// <summary>
        /// Obtain branches and items.
        /// </summary>
        All = OPCBROWSEFILTER.OPC_BROWSE_FILTER_ALL,

        /// <summary>
        /// Obtain branches.
        /// </summary>
        Branches = OPCBROWSEFILTER.OPC_BROWSE_FILTER_BRANCHES,

        /// <summary>
        /// Obtain items.
        /// </summary>
        Items = OPCBROWSEFILTER.OPC_BROWSE_FILTER_ITEMS
    }
}