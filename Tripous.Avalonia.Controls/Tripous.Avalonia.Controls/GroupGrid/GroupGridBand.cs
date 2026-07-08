// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Specifies a visual band in a group grid layout.
/// </summary>
public enum GroupGridBand
{
    /// <summary>
    /// No band.
    /// </summary>
    None,
    /// <summary>
    /// The toolbar band.
    /// </summary>
    ToolBar,
    /// <summary>
    /// The grouping panel band.
    /// </summary>
    GroupPanel,
    /// <summary>
    /// The column captions band.
    /// </summary>
    ColumnHeader,
    /// <summary>
    /// The filter row band.
    /// </summary>
    FilterRow,
    /// <summary>
    /// The scrollable body band.
    /// </summary>
    Body,
    /// <summary>
    /// The footer summary band.
    /// </summary>
    FooterSummary,
}
