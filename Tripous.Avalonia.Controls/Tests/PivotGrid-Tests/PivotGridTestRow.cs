// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Pivot.Tests;

/// <summary>
/// Represents a pivot grid test row.
/// </summary>
public class PivotGridTestRow
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridTestRow"/> class.
    /// </summary>
    public PivotGridTestRow()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets non-scalar custom data.
    /// </summary>
    public List<string> Tags { get; set; } = new();
    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    public string Region { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the quarter.
    /// </summary>
    public string Quarter { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the salesperson.
    /// </summary>
    public string Salesperson { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Gets or sets the unit count.
    /// </summary>
    public int Units { get; set; }
}
