// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a row included in a pivot grid export snapshot.
/// </summary>
public class PivotGridExportRow
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridExportRow"/> class.
    /// </summary>
    /// <param name="RowNode">The source row-axis node.</param>
    /// <param name="RowIndex">The visible row index.</param>
    /// <param name="HeaderText">The row header text.</param>
    /// <param name="RowTexts">The row-axis field display values.</param>
    /// <param name="Cells">The value cells.</param>
    /// <param name="IsColumnTotal">True when this is the column grand-total row.</param>
    public PivotGridExportRow(PivotGridAxisNode RowNode, int RowIndex, string HeaderText, IEnumerable<string> RowTexts, IEnumerable<PivotGridExportCell> Cells, bool IsColumnTotal)
    {
        this.RowNode = RowNode;
        this.RowIndex = RowIndex;
        this.HeaderText = HeaderText ?? string.Empty;
        this.RowTexts = new ReadOnlyCollection<string>((RowTexts ?? Array.Empty<string>()).ToList());
        this.Cells = new ReadOnlyCollection<PivotGridExportCell>((Cells ?? Array.Empty<PivotGridExportCell>()).ToList());
        this.IsColumnTotal = IsColumnTotal;
    }

    // ● properties
    /// <summary>
    /// Gets the source row-axis node.
    /// </summary>
    public PivotGridAxisNode RowNode { get; }
    /// <summary>
    /// Gets the visible row index.
    /// </summary>
    public int RowIndex { get; }
    /// <summary>
    /// Gets the row header text.
    /// </summary>
    public string HeaderText { get; }
    /// <summary>
    /// Gets the row-axis field display values.
    /// </summary>
    public IReadOnlyList<string> RowTexts { get; }
    /// <summary>
    /// Gets the exported value cells.
    /// </summary>
    public IReadOnlyList<PivotGridExportCell> Cells { get; }
    /// <summary>
    /// Gets a value indicating whether this is the column grand-total row.
    /// </summary>
    public bool IsColumnTotal { get; }
    /// <summary>
    /// Gets the row-axis level.
    /// </summary>
    public int Level => RowNode == null ? 0 : RowNode.Level;
    /// <summary>
    /// Gets a value indicating whether this row can be expanded.
    /// </summary>
    public bool HasChildren => RowNode != null && RowNode.HasChildren;
    /// <summary>
    /// Gets a value indicating whether this row is expanded.
    /// </summary>
    public bool IsExpanded => RowNode != null && RowNode.IsExpanded;
}
