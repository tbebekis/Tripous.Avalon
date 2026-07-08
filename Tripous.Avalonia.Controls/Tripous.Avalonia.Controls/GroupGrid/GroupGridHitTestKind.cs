// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Specifies the kind of group grid element found during hit testing.
/// </summary>
public enum GroupGridHitTestKind
{
    /// <summary>
    /// Nothing was hit.
    /// </summary>
    None,
    /// <summary>
    /// The toolbar was hit.
    /// </summary>
    ToolBar,
    /// <summary>
    /// The group panel was hit.
    /// </summary>
    GroupPanel,
    /// <summary>
    /// A column header was hit.
    /// </summary>
    ColumnHeader,
    /// <summary>
    /// A column resize handle was hit.
    /// </summary>
    ColumnResizer,
    /// <summary>
    /// A filter cell was hit.
    /// </summary>
    FilterCell,
    /// <summary>
    /// A body row was hit.
    /// </summary>
    BodyRow,
    /// <summary>
    /// A body cell was hit.
    /// </summary>
    BodyCell,
    /// <summary>
    /// A group expander was hit.
    /// </summary>
    GroupExpander,
    /// <summary>
    /// A footer summary cell was hit.
    /// </summary>
    FooterSummaryCell,
    /// <summary>
    /// A column header drag source was hit.
    /// </summary>
    ColumnDragSource,
    /// <summary>
    /// A column header drop target was hit.
    /// </summary>
    ColumnDropTarget,
    /// <summary>
    /// A group panel drop target was hit.
    /// </summary>
    GroupPanelDropTarget,
}
