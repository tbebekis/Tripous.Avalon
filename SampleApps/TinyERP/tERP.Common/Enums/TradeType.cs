/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the business domain and direction of a trade document.
/// </summary>
[TypeStore]
public enum TradeType
{
    /// <summary>No trade type is specified.</summary>
    None = 0,
    /// <summary>A customer-facing sales transaction.</summary>
    Sales = 1,
    /// <summary>A supplier-facing purchase transaction.</summary>
    Purchases = 2,
    /// <summary>An internal warehouse or inventory transaction.</summary>
    Warehouse = 3,
    /// <summary>A financial transaction involving money or balances.</summary>
    Financial = 4,
    /// <summary>An accounting transaction recorded in the ledger.</summary>
    Accounting = 5,
}
