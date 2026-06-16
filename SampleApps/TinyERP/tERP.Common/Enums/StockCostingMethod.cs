/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the method used to calculate inventory cost.
/// </summary>
[TypeStore]
public enum StockCostingMethod
{
    /// <summary>No stock costing method is specified.</summary>
    None = 0,
    /// <summary>Uses the continuously recalculated weighted average cost.</summary>
    MovingAverage = 1,
    /// <summary>Issues the oldest available stock cost first.</summary>
    Fifo = 2,
    /// <summary>Issues the newest available stock cost first.</summary>
    Lifo = 3,
    /// <summary>Uses a predefined standard unit cost.</summary>
    StandardCost = 4,
}
