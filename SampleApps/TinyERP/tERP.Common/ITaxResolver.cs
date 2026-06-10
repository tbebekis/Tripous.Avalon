/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines a tax resolver that determines the tax components of a commercial document line.
/// </summary>
[TypeStore]
public interface ITaxResolver
{
    // ● public
    /// <summary>
    /// Resolves the applicable tax rules and calculates the tax result.
    /// </summary>
    TaxResult Resolve(TaxResolveArgs Args);
}

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

/// <summary>
/// Represents one tax component produced by a tax rule.
/// </summary>
[TypeStore]
public class TaxComponent
{
    // ● construction
    public TaxComponent()
    {
    }

    // ● properties
    /// <summary>
    /// Tax rule identifier that produced this component.
    /// </summary>
    public string TaxRuleId { get; set; } = "";
    /// <summary>
    /// Tax rate identifier selected by the rule.
    /// </summary>
    public string TaxRateId { get; set; } = "";
    /// <summary>
    /// Tax jurisdiction identifier that imposed this component.
    /// </summary>
    public string TaxJurisdictionId { get; set; } = "";
    /// <summary>
    /// Tax clause identifier used by the component.
    /// </summary>
    public string TaxClauseId { get; set; } = "";
    /// <summary>
    /// Calculation order of the component.
    /// </summary>
    public int SequenceNo { get; set; }
    /// <summary>
    /// Calculation method used by the component.
    /// </summary>
    public TaxCalculationType TaxCalculationType { get; set; }
    /// <summary>
    /// Tax percentage stored as a calculation snapshot.
    /// </summary>
    public decimal TaxRatePercent { get; set; }
    /// <summary>
    /// Amount on which this component was calculated.
    /// </summary>
    public decimal TaxableAmount { get; set; }
    /// <summary>
    /// Calculated monetary value of this component.
    /// </summary>
    public decimal TaxAmount { get; set; }
    /// <summary>
    /// Indicates that this component is tax exempt.
    /// </summary>
    public bool IsExempt { get; set; }
    /// <summary>
    /// Indicates that this component uses reverse charge.
    /// </summary>
    public bool IsReverseCharge { get; set; }
    /// <summary>
    /// Printed legal explanation stored as a calculation snapshot.
    /// </summary>
    public string TaxClauseText { get; set; } = "";
}
