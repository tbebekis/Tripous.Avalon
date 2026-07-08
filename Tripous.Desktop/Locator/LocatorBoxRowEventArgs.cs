/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Event arguments for locator row selection.
/// </summary>
public class LocatorBoxRowEventArgs: EventArgs
{
    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="Row">The selected source row.</param>
    public LocatorBoxRowEventArgs(DataRow Row)
    {
        this.Row = Row;
    }

    // ● properties
    /// <summary>
    /// The selected source row.
    /// </summary>
    public DataRow Row { get; }
}
