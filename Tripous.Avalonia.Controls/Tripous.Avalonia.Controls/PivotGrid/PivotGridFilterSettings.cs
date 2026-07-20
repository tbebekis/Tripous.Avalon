// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents persisted pivot grid value-list filter settings.
/// </summary>
public class PivotGridFilterSettings
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridFilterSettings"/> class.
    /// </summary>
    public PivotGridFilterSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the filtered source field name.
    /// </summary>
    public string FieldName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets accepted invariant value keys.
    /// </summary>
    public List<string> AcceptedValueKeys { get; set; } = new();
}
