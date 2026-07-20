// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a measure included in a pivot grid export snapshot.
/// </summary>
public class PivotGridExportMeasure
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridExportMeasure"/> class.
    /// </summary>
    /// <param name="Measure">The source measure.</param>
    public PivotGridExportMeasure(PivotGridMeasure Measure)
    {
        this.Measure = Measure;
        Name = Measure == null ? string.Empty : Measure.Name;
        Header = Measure == null || string.IsNullOrWhiteSpace(Measure.Header) ? Name : Measure.Header;
        SourceFieldName = Measure == null ? string.Empty : Measure.SourceFieldName;
        AggregateKind = Measure == null ? PivotGridAggregateKind.Sum : Measure.AggregateKind;
        DisplayFormat = Measure == null ? string.Empty : Measure.DisplayFormat;
    }

    // ● properties
    /// <summary>
    /// Gets the source measure.
    /// </summary>
    public PivotGridMeasure Measure { get; }
    /// <summary>
    /// Gets the measure name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Gets the measure header text.
    /// </summary>
    public string Header { get; }
    /// <summary>
    /// Gets the source field name.
    /// </summary>
    public string SourceFieldName { get; }
    /// <summary>
    /// Gets the aggregate kind.
    /// </summary>
    public PivotGridAggregateKind AggregateKind { get; }
    /// <summary>
    /// Gets the display format.
    /// </summary>
    public string DisplayFormat { get; }
}
