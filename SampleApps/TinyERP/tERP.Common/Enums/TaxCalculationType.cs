/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines how a tax rule calculates its monetary tax component.
/// </summary>
[TypeStore]
public enum TaxCalculationType
{
    /// <summary>No tax calculation method is specified.</summary>
    None = 0,
    /// <summary>Calculates tax as a percentage of the taxable amount.</summary>
    Percentage = 1,
    /// <summary>Calculates tax on the taxable amount including previously calculated tax components.</summary>
    TaxOnTax = 2,
}
