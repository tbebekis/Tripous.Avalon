// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.Charts;

/// <summary>
/// Represents a sample sales row.
/// </summary>
public class SalesRow
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="SalesRow"/> class.
    /// </summary>
    public SalesRow()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the sales region.
    /// </summary>
    public string Region { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the salesperson.
    /// </summary>
    public string Salesperson { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string Category { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the quarter.
    /// </summary>
    public string Quarter { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public decimal Amount { get; set; }
}
