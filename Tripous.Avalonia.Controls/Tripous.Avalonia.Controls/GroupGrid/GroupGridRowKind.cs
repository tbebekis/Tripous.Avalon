// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Specifies the kind of logical row represented by a visible group grid row.
/// </summary>
public enum GroupGridRowKind
{
    /// <summary>
    /// No row.
    /// </summary>
    None,
    /// <summary>
    /// A data row.
    /// </summary>
    DataRow,
    /// <summary>
    /// A group header row.
    /// </summary>
    Group,
    /// <summary>
    /// A group summary row.
    /// </summary>
    GroupSummary,
    /// <summary>
    /// The total summary row in the grid footer.
    /// </summary>
    TotalSummary,
}
