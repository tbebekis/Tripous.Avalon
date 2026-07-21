// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines chart hit-test result kinds.
/// </summary>
public enum ChartHitTestKind
{
    /// <summary>
    /// No chart element was hit.
    /// </summary>
    None,
    /// <summary>
    /// A chart data point was hit.
    /// </summary>
    DataPoint,
    /// <summary>
    /// A legend item was hit.
    /// </summary>
    Legend,
}
