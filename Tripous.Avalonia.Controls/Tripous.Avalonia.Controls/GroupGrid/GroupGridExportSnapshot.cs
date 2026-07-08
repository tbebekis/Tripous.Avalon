// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Contains the current group grid data prepared for export.
/// </summary>
public class GroupGridExportSnapshot
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridExportSnapshot"/> class.
    /// </summary>
    /// <param name="Columns">The exported columns.</param>
    /// <param name="Rows">The exported rows.</param>
    /// <param name="TotalSummaryCells">The total summary cells.</param>
    public GroupGridExportSnapshot(IEnumerable<GroupGridExportColumn> Columns, IEnumerable<GroupGridExportRow> Rows, IEnumerable<GroupGridExportCell> TotalSummaryCells)
    {
        this.Columns = new ReadOnlyCollection<GroupGridExportColumn>((Columns ?? Array.Empty<GroupGridExportColumn>()).ToList());
        this.Rows = new ReadOnlyCollection<GroupGridExportRow>((Rows ?? Array.Empty<GroupGridExportRow>()).ToList());
        this.TotalSummaryCells = new ReadOnlyCollection<GroupGridExportCell>((TotalSummaryCells ?? Array.Empty<GroupGridExportCell>()).ToList());
    }

    // ● public methods
    /// <summary>
    /// Returns data rows from the export snapshot.
    /// </summary>
    /// <returns>The data rows.</returns>
    public IReadOnlyList<GroupGridExportRow> GetDataRows()
    {
        return Rows.Where(Row => Row.IsDataRow).ToList();
    }

    // ● properties
    /// <summary>
    /// Gets the exported columns.
    /// </summary>
    public IReadOnlyList<GroupGridExportColumn> Columns { get; }
    /// <summary>
    /// Gets the exported rows.
    /// </summary>
    public IReadOnlyList<GroupGridExportRow> Rows { get; }
    /// <summary>
    /// Gets the total summary cells.
    /// </summary>
    public IReadOnlyList<GroupGridExportCell> TotalSummaryCells { get; }
}
