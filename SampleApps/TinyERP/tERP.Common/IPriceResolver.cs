/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines a price resolver that determines the unit price of a commercial document line.
/// </summary>
[TypeStore]
public interface IPriceResolver
{
    // ● public
    /// <summary>
    /// Resolves the applicable product price.
    /// </summary>
    PriceResult Resolve(PriceResolveArgs Args);
}

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

/// <summary>
/// Contains the selected product price and its source information.
/// </summary>
[TypeStore]
public class PriceResult
{
    // ● construction
    public PriceResult()
    {
    }

    // ● properties
    /// <summary>
    /// Indicates that an applicable price list row was found.
    /// </summary>
    public bool IsFound { get; set; }
    /// <summary>
    /// Selected price list row identifier.
    /// </summary>
    public string PriceListId { get; set; } = "";
    /// <summary>
    /// Selected price list type identifier.
    /// </summary>
    public string PriceListTypeId { get; set; } = "";
    /// <summary>
    /// Currency identifier of the selected price list type.
    /// </summary>
    public string CurrencyId { get; set; } = "";
    /// <summary>
    /// Discount category associated with the selected price row.
    /// </summary>
    public string DiscountCategoryId { get; set; } = "";
    /// <summary>
    /// Customer identifier of a customer-specific price row.
    /// </summary>
    public string CustomerId { get; set; } = "";
    /// <summary>
    /// Minimum quantity of the selected quantity break.
    /// </summary>
    public decimal MinQuantity { get; set; }
    /// <summary>
    /// Unit price stored in the selected price row.
    /// </summary>
    public decimal UnitPrice { get; set; }
    /// <summary>
    /// Indicates that the stored unit price includes indirect tax.
    /// </summary>
    public bool IsTaxIncluded { get; set; }
}
