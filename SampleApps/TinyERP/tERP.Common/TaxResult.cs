/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Contains the aggregate tax result and all individual tax components.
/// </summary>
[TypeStore]
public class TaxResult
{
    // ● construction
    public TaxResult()
    {
    }

    // ● properties
    /// <summary>
    /// Resolved origin tax jurisdiction identifier.
    /// </summary>
    public string OriginTaxJurisdictionId { get; set; } = "";
    /// <summary>
    /// Resolved destination tax jurisdiction identifier.
    /// </summary>
    public string DestinationTaxJurisdictionId { get; set; } = "";
    /// <summary>
    /// Aggregate effective tax percentage of all components.
    /// </summary>
    public decimal TaxPercent { get; set; }
    /// <summary>
    /// Total calculated tax amount.
    /// </summary>
    public decimal TaxAmount { get; set; }
    /// <summary>
    /// Indicates that the resolved transaction is tax exempt.
    /// </summary>
    public bool IsExempt { get; set; }
    /// <summary>
    /// Indicates that at least one component uses reverse charge.
    /// </summary>
    public bool IsReverseCharge { get; set; }
    /// <summary>
    /// Individual tax components produced by the resolver.
    /// </summary>
    public List<TaxComponent> Components { get; set; } = [];
}
