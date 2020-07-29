﻿using System;
using EasyDriver.Opc.Client.Da.Wrappers;

namespace EasyDriver.Opc.Client.Da.Browsing.Internal
{
    static class OpcBrowseServerAddressSpaceExtensions
    {
        public static string TryGetItemId(this OpcBrowseServerAddressSpace opcBrowseServerAddressSpace, string itemName)
        {
            try
            {
                return opcBrowseServerAddressSpace.GetItemId(itemName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
