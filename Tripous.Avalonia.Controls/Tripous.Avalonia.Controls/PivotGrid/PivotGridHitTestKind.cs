// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines logical elements returned by pivot grid hit testing.
/// </summary>
public enum PivotGridHitTestKind
{
    /// <summary>
    /// No element.
    /// </summary>
    None,
    /// <summary>
    /// A blank band area.
    /// </summary>
    Band,
    /// <summary>
    /// An available source field.
    /// </summary>
    AvailableField,
    /// <summary>
    /// A row-axis field.
    /// </summary>
    RowField,
    /// <summary>
    /// A column-axis field.
    /// </summary>
    ColumnField,
    /// <summary>
    /// A measure field.
    /// </summary>
    MeasureField,
    /// <summary>
    /// A row-axis node expander.
    /// </summary>
    RowExpander,
    /// <summary>
    /// A row-axis header item.
    /// </summary>
    RowHeader,
    /// <summary>
    /// A column-axis header item.
    /// </summary>
    ColumnHeader,
    /// <summary>
    /// A value cell.
    /// </summary>
    ValueCell,
    /// <summary>
    /// A measure column resize handle.
    /// </summary>
    MeasureResizer,
    /// <summary>
    /// A row header resize handle.
    /// </summary>
    RowHeaderResizer,
}
