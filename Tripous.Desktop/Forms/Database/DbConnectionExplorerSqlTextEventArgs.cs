/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Event arguments carrying SQL text requested from the database explorer.
/// </summary>
public class DbConnectionExplorerSqlTextEventArgs: DbConnectionExplorerEventArgs
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="ConnectionInfo">The connection information.</param>
    /// <param name="SqlText">The SQL text.</param>
    public DbConnectionExplorerSqlTextEventArgs(DbConnectionInfo ConnectionInfo, string SqlText)
        : base(ConnectionInfo)
    {
        this.SqlText = SqlText;
    }

    // ● properties
    /// <summary>
    /// Gets the SQL text.
    /// </summary>
    public string SqlText { get; }
}
