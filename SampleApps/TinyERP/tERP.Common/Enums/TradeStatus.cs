/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the lifecycle status of a trade document.
/// </summary>
[TypeStore]
public enum TradeStatus
{
    /// <summary>No document status is specified.</summary>
    None = 0,
    /// <summary>The document is editable and has not been posted.</summary>
    Draft = 1,
    /// <summary>The document is finalized, posted, and locked.</summary>
    Posted = 2,
    /// <summary>The document has been cancelled.</summary>
    Cancelled = 3,
    /// <summary>The document has been fully executed or fulfilled.</summary>
    Completed = 4,
}
