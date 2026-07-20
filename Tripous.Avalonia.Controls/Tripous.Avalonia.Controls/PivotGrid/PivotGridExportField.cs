// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes an axis field included in a pivot grid export snapshot.
/// </summary>
public class PivotGridExportField
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridExportField"/> class.
    /// </summary>
    /// <param name="Field">The source axis field.</param>
    public PivotGridExportField(PivotGridField Field)
    {
        Name = Field == null ? string.Empty : Field.Name;
        Header = Field == null || string.IsNullOrWhiteSpace(Field.Header) ? Name : Field.Header;
        DisplayFormat = Field == null ? string.Empty : Field.DisplayFormat;
    }

    // ● properties
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Gets the field header text.
    /// </summary>
    public string Header { get; }
    /// <summary>
    /// Gets the field display format.
    /// </summary>
    public string DisplayFormat { get; }
}
