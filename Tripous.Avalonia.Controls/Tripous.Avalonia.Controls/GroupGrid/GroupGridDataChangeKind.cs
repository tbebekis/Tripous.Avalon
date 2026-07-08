// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Specifies the kind of data change reported by a group grid data adapter.
/// </summary>
public enum GroupGridDataChangeKind
{
    /// <summary>
    /// The full data projection should be rebuilt.
    /// </summary>
    Reset,
    /// <summary>
    /// A row has been added.
    /// </summary>
    RowAdded,
    /// <summary>
    /// A row has been removed.
    /// </summary>
    RowRemoved,
    /// <summary>
    /// A row has moved.
    /// </summary>
    RowMoved,
    /// <summary>
    /// A full row has changed.
    /// </summary>
    RowChanged,
    /// <summary>
    /// A single cell value has changed.
    /// </summary>
    CellChanged,
}
