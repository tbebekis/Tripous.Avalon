/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Provides the commercial, geographic, and monetary context required for tax resolution.
/// </summary>
[TypeStore]
public class TaxResolveArgs
{
    // ● construction
    public TaxResolveArgs()
    {
    }

    // ● properties
    /// <summary>
    /// Commercial document identifier, when available.
    /// </summary>
    public string TradeId { get; set; } = "";
    /// <summary>
    /// Commercial document line identifier, when available.
    /// </summary>
    public string TradeLineId { get; set; } = "";
    /// <summary>
    /// Document type identifier.
    /// </summary>
    public string DocumentTypeId { get; set; } = "";
    /// <summary>
    /// Sales or purchase transaction direction.
    /// </summary>
    public TradeType TradeType { get; set; }
    /// <summary>
    /// Transaction date used for effective tax rule selection.
    /// </summary>
    public DateTime TradeDate { get; set; }
    /// <summary>
    /// Business party identifier.
    /// </summary>
    public string PersonId { get; set; } = "";
    /// <summary>
    /// Business party tax classification identifier.
    /// </summary>
    public string TaxBusinessGroupId { get; set; } = "";
    /// <summary>
    /// Product identifier.
    /// </summary>
    public string ProductId { get; set; } = "";
    /// <summary>
    /// Product tax classification identifier.
    /// </summary>
    public string TaxProductGroupId { get; set; } = "";
    /// <summary>
    /// Explicit or previously resolved origin tax jurisdiction identifier.
    /// </summary>
    public string OriginTaxJurisdictionId { get; set; } = "";
    /// <summary>
    /// Explicit or previously resolved destination tax jurisdiction identifier.
    /// </summary>
    public string DestinationTaxJurisdictionId { get; set; } = "";
    /// <summary>
    /// Origin address used when the origin jurisdiction must be resolved.
    /// </summary>
    public PersonAddress OriginAddress { get; set; } = new();
    /// <summary>
    /// Destination address used when the destination jurisdiction must be resolved.
    /// </summary>
    public PersonAddress DestinationAddress { get; set; } = new();
    /// <summary>
    /// Net line amount on which tax is calculated.
    /// </summary>
    public decimal TaxableAmount { get; set; }
}
