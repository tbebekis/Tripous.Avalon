/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Provides the commercial context required for price resolution.
/// </summary>
[TypeStore]
public class PriceResolveArgs
{
    // ● construction
    public PriceResolveArgs()
    {
    }

    // ● properties
    /// <summary>
    /// Sales or purchase transaction direction.
    /// </summary>
    public TradeType TradeType { get; set; }
    /// <summary>
    /// Price list type identifier selected for the document.
    /// </summary>
    public string PriceListTypeId { get; set; } = "";
    /// <summary>
    /// Business party identifier used for customer-specific pricing.
    /// </summary>
    public string PersonId { get; set; } = "";
    /// <summary>
    /// Product identifier.
    /// </summary>
    public string ProductId { get; set; } = "";
    /// <summary>
    /// Transaction unit of measure identifier.
    /// </summary>
    public string UnitOfMeasureId { get; set; } = "";
    /// <summary>
    /// Quantity used for quantity-break pricing.
    /// </summary>
    public decimal Quantity { get; set; }
    /// <summary>
    /// Transaction date used for price validity selection.
    /// </summary>
    public DateTime TradeDate { get; set; }
    /// <summary>
    /// Document currency identifier.
    /// </summary>
    public string CurrencyId { get; set; } = "";
}
