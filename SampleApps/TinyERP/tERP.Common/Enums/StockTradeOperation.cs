/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the inventory operation performed by a stock transaction.
/// </summary>
[TypeStore]
public enum StockTradeOperation
{
    /// <summary>No stock operation is specified.</summary>
    None = 0,
    /// <summary>Moves stock between two warehouses without changing its cost.</summary>
    Transfer = 1,
    /// <summary>Receives stock into a warehouse using the entered cost.</summary>
    Receipt = 2,
    /// <summary>Issues stock from a warehouse using the current moving-average cost.</summary>
    Issue = 3,
}
