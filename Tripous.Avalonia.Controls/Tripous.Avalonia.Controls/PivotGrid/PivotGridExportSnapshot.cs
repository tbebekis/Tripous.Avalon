// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Contains the current pivot grid projection prepared for export.
/// </summary>
public class PivotGridExportSnapshot
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridExportSnapshot"/> class.
    /// </summary>
    /// <param name="RowFields">The exported row-axis fields.</param>
    /// <param name="ColumnFields">The exported column-axis fields.</param>
    /// <param name="Measures">The exported measures.</param>
    /// <param name="ValueColumns">The exported value columns.</param>
    /// <param name="Rows">The exported rows.</param>
    public PivotGridExportSnapshot(IEnumerable<PivotGridExportField> RowFields, IEnumerable<PivotGridExportField> ColumnFields, IEnumerable<PivotGridExportMeasure> Measures, IEnumerable<PivotGridExportValueColumn> ValueColumns, IEnumerable<PivotGridExportRow> Rows)
    {
        this.RowFields = new ReadOnlyCollection<PivotGridExportField>((RowFields ?? Array.Empty<PivotGridExportField>()).ToList());
        this.ColumnFields = new ReadOnlyCollection<PivotGridExportField>((ColumnFields ?? Array.Empty<PivotGridExportField>()).ToList());
        this.Measures = new ReadOnlyCollection<PivotGridExportMeasure>((Measures ?? Array.Empty<PivotGridExportMeasure>()).ToList());
        this.ValueColumns = new ReadOnlyCollection<PivotGridExportValueColumn>((ValueColumns ?? Array.Empty<PivotGridExportValueColumn>()).ToList());
        this.Rows = new ReadOnlyCollection<PivotGridExportRow>((Rows ?? Array.Empty<PivotGridExportRow>()).ToList());
    }

    // ● properties
    /// <summary>
    /// Gets the exported row-axis fields.
    /// </summary>
    public IReadOnlyList<PivotGridExportField> RowFields { get; }
    /// <summary>
    /// Gets the exported column-axis fields.
    /// </summary>
    public IReadOnlyList<PivotGridExportField> ColumnFields { get; }
    /// <summary>
    /// Gets the exported measures.
    /// </summary>
    public IReadOnlyList<PivotGridExportMeasure> Measures { get; }
    /// <summary>
    /// Gets the exported value columns.
    /// </summary>
    public IReadOnlyList<PivotGridExportValueColumn> ValueColumns { get; }
    /// <summary>
    /// Gets the exported rows.
    /// </summary>
    public IReadOnlyList<PivotGridExportRow> Rows { get; }
}
