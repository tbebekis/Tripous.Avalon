/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the business purpose of a postal address.
/// </summary>
[TypeStore]
public enum AddressType
{
    /// <summary>No address purpose is specified.</summary>
    None = 0,
    /// <summary>The primary general-purpose address.</summary>
    Main = 1,
    /// <summary>The address used for billing and invoicing.</summary>
    Billing = 2,
    /// <summary>The address used for shipping and delivery.</summary>
    Shipping = 3,
    /// <summary>An address with another business purpose.</summary>
    Other = 4
}
