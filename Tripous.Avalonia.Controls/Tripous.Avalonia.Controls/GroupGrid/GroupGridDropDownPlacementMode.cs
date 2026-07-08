// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines how a <see cref="GroupGrid"/> hosts in-place editor drop-down controls.
/// </summary>
public enum GroupGridDropDownPlacementMode
{
    /// <summary>
    /// Hosts the drop-down as an inline visual child of the grid.
    /// </summary>
    Inline,
    /// <summary>
    /// Hosts the drop-down in an Avalonia popup.
    /// </summary>
    Popup,
    /// <summary>
    /// Uses popup placement with platform constraint adjustment.
    /// </summary>
    Auto,
}
