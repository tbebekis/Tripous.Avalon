/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

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
