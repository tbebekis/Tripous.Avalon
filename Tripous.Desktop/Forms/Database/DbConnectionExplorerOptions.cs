/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Options controlling the database connection explorer behavior.
/// </summary>
public class DbConnectionExplorerOptions
{
    // ● properties
    /// <summary>
    /// True to allow adding new connection definitions.
    /// </summary>
    public bool AllowAddConnections { get; set; }
    /// <summary>
    /// True to allow editing existing connection definitions.
    /// </summary>
    public bool AllowEditConnections { get; set; }
    /// <summary>
    /// True to allow deleting existing connection definitions.
    /// </summary>
    public bool AllowDeleteConnections { get; set; }
    /// <summary>
    /// True to allow creating physical databases when supported by the SQL provider.
    /// </summary>
    public bool AllowCreateDatabases { get; set; }
    /// <summary>
    /// True to persist connection definition changes to the configured storage.
    /// </summary>
    public bool PersistConnectionChanges { get; set; }
    /// <summary>
    /// True to show the toolbar.
    /// </summary>
    public bool ShowToolBar { get; set; } = true;
}
