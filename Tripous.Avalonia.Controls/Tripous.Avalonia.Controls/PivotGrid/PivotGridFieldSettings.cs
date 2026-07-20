// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents persisted pivot grid axis field settings.
/// </summary>
public class PivotGridFieldSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridFieldSettings"/> class.
    /// </summary>
    public PivotGridFieldSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display header.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the field width.
    /// </summary>
    public double Width { get; set; }
}
