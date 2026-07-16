/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Event arguments carrying database connection information.
/// </summary>
public class DbConnectionExplorerEventArgs: EventArgs
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ConnectionInfo">The connection information.</param>
    public DbConnectionExplorerEventArgs(DbConnectionInfo ConnectionInfo)
    {
        this.ConnectionInfo = ConnectionInfo;
    }

    // ● properties
    /// <summary>
    /// Gets the connection information.
    /// </summary>
    public DbConnectionInfo ConnectionInfo { get; }
}
