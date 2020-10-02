﻿using System;
using System.Runtime.InteropServices;
using EasyDriver.Opc.DA.Client.Common;
using EasyDriver.Opc.DA.Client.Da.Browsing;
using EasyDriver.Opc.DA.Client.Interop.Common;
using EasyDriver.Opc.DA.Client.Interop.Da;
using EasyDriver.Opc.DA.Client.Interop.Helpers;
using EasyDriver.Opc.DA.Client.Interop.System;

namespace EasyDriver.Opc.DA.Client.Da.Wrappers
{
    public class OpcBrowseServerAddressSpace : ComWrapper
    {
        public OpcBrowseServerAddressSpace(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCBrowseServerAddressSpace>",
                () => comObject.QueryInterface<IOPCBrowseServerAddressSpace>());
        }

        internal IOPCBrowseServerAddressSpace ComObject { get; set; }

        public OpcDaNamespaceType Organization
        {
            get
            {
                return DoComCall(ComObject, "IOPCBrowseServerAddressSpace::QueryOrganization", () =>
                {
                    OPCNAMESPACETYPE nameSpaceType;
                    ComObject.QueryOrganization(out nameSpaceType);
                    return (OpcDaNamespaceType) nameSpaceType;
                });
            }
        }

        public void ChangeBrowsePosition(OPCBROWSEDIRECTION browseDirection, string itemNameOrId = "")
        {
            DoComCall(ComObject, "IOPCBrowseServerAddressSpace::ChangeBrowsePosition",
                () => ComObject.ChangeBrowsePosition(browseDirection, itemNameOrId ?? ""), browseDirection, itemNameOrId);
        }

        public string[] BrowseOpcItemIds(OpcDaBrowseType browseFilterType, string filterCriteria = "",
            VarEnum dataTypeFilter = VarEnum.VT_EMPTY,
            OpcDaAccessRights accessRightsFilter = OpcDaAccessRights.Ignore)
        {
            string szFilterCriteria = filterCriteria ?? "";

            return DoComCall(ComObject, "IOPCBrowseServerAddressSpace::BrowseOpcItemIDs", () =>
            {
                IEnumString enumString;
                ComObject.BrowseOPCItemIDs((OPCBROWSETYPE) browseFilterType, szFilterCriteria,
                    (short) dataTypeFilter,
                    (OPCACCESSRIGHTS) accessRightsFilter, out enumString);
                return DoComCall(enumString, "IEnumString::EnumareateAllAndRelease",
                    () => enumString.EnumareateAllAndRelease(OpcConfiguration.BatchSize).ToArray(),
                    OpcConfiguration.BatchSize);
            }, browseFilterType, szFilterCriteria, dataTypeFilter,
                accessRightsFilter);
        }

        public string GetItemId(string itemName)
        {
            return DoComCall(ComObject, "IOPCBrowseServerAddressSpace::GetItemId", () =>
            {
                string szItemId;
                ComObject.GetItemID(itemName, out szItemId);
                return szItemId;
            }, itemName);
        }

        public string[] BrowseAccessPaths(string itemId)
        {
            return DoComCall(ComObject, "IOPCBrowseServerAddressSpace::BrowseAccessPaths", () =>
            {
                IEnumString enumString;
                ComObject.BrowseAccessPaths(itemId, out enumString);
                return DoComCall(enumString, "IEnumString::EnumareateAllAndRelease",
                    () => enumString.EnumareateAllAndRelease(OpcConfiguration.BatchSize).ToArray(),
                    OpcConfiguration.BatchSize);
            }, itemId);
        }
    }
}