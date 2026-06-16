/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the family of indirect tax represented by a tax rate.
/// </summary>
[TypeStore]
public enum TaxType
{
    /// <summary>No indirect tax family is specified.</summary>
    None = 0,
    /// <summary>Value Added Tax, commonly used in Europe and many other countries.</summary>
    Vat = 1,
    /// <summary>Sales tax, commonly imposed by United States state and local authorities.</summary>
    SalesTax = 2,
    /// <summary>Goods and Services Tax, used in countries such as Canada and Australia.</summary>
    Gst = 3,
    /// <summary>Another indirect tax family not represented by the standard values.</summary>
    Other = 4,
}
