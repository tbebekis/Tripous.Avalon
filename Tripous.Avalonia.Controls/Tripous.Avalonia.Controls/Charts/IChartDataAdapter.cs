// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides indexed row and field value access for a chart data source.
/// </summary>
public interface IChartDataAdapter
{
    // ● public methods
    /// <summary>
    /// Returns the source row at a specified row index.
    /// </summary>
    /// <param name="RowIndex">The source row index.</param>
    /// <returns>The source row.</returns>
    object GetRow(int RowIndex);
    /// <summary>
    /// Returns a field value from a specified source row.
    /// </summary>
    /// <param name="RowIndex">The source row index.</param>
    /// <param name="FieldName">The field name.</param>
    /// <returns>The field value.</returns>
    object GetValue(int RowIndex, string FieldName);

    // ● properties
    /// <summary>
    /// Gets the number of source rows exposed by the adapter.
    /// </summary>
    int RowCount { get; }
    /// <summary>
    /// Gets the source fields exposed by the adapter.
    /// </summary>
    IReadOnlyList<ChartSourceField> SourceFields { get; }

    // ● events
    /// <summary>
    /// Occurs when rows or field values exposed by the adapter change.
    /// </summary>
    event EventHandler<ChartDataChangedEventArgs> Changed;
}
