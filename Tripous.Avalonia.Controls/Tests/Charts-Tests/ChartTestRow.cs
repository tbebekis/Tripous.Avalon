// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Charts.Tests;

/// <summary>
/// Represents a row used by chart tests.
/// </summary>
public class ChartTestRow
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartTestRow"/> class.
    /// </summary>
    public ChartTestRow()
    {
    }

    // ● properties
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
}
