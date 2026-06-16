/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the commercial nature of a trade document line.
/// </summary>
[TypeStore]
public enum TradeLineType
{
    /// <summary>No line type is specified.</summary>
    None = 0,
    /// <summary>A line representing a tangible product or material.</summary>
    Item = 1,
    /// <summary>A line representing a service.</summary>
    Service = 2,
}
