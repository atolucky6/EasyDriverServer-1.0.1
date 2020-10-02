﻿using System;

namespace EasyDriver.Opc.DA.Client.Common
{
    /// <summary>
    /// Represents OPC server URL validation result.
    /// </summary>
    [Flags]
    public enum UrlValidationResult
    {
        Ok = 0,
        UrlIsNull = 1,
        MoreThanTwoSegments = 2,
        WrongScheme = 4
    }
}