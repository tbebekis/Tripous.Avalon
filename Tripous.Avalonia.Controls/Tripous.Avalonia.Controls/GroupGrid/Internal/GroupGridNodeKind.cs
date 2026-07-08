// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Specifies the kind of projected group grid node.
/// </summary>
internal enum GroupGridNodeKind
{
    /// <summary>
    /// The root node.
    /// </summary>
    Root,
    /// <summary>
    /// A data row node.
    /// </summary>
    DataRow,
    /// <summary>
    /// A group header row node.
    /// </summary>
    Group,
    /// <summary>
    /// A group summary row node.
    /// </summary>
    GroupSummary,
}
