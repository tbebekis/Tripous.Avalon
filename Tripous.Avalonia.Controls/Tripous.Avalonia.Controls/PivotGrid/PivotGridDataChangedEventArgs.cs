// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides data-change information for a pivot grid data adapter.
/// </summary>
public class PivotGridDataChangedEventArgs: EventArgs
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridDataChangedEventArgs"/> class.
    /// </summary>
    public PivotGridDataChangedEventArgs()
    {
    }

    // ● static public
    /// <summary>
    /// Creates a reset data-change notification.
    /// </summary>
    /// <returns>The data-change notification.</returns>
    static public PivotGridDataChangedEventArgs Reset() => new();
}
