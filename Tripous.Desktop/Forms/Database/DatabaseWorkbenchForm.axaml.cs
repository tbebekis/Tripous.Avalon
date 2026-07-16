/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// AppForm hosting a database explorer and an interactive SQL console.
/// </summary>
[TypeStore]
public partial class DatabaseWorkbenchForm : AppForm
{
    // ● protected
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        Explorer.SetConnections(Db.Connections.List);
        Explorer.ConnectionSelected += (Sender, Args) => SqlConsole.SetConnection(Args.ConnectionInfo);
        Explorer.OpenSqlRequested += (Sender, Args) => SqlConsole.SetConnection(Args.ConnectionInfo);
        Explorer.SqlTextRequested += (Sender, Args) =>
        {
            SqlConsole.SetConnection(Args.ConnectionInfo);
            SqlConsole.ShowSqlText(Args.SqlText);
        };
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public DatabaseWorkbenchForm()
    {
        InitializeComponent();
    }

    // ● properties
    /// <summary>
    /// Gets the database explorer control.
    /// </summary>
    public DbConnectionExplorerControl ConnectionExplorer => Explorer;
    /// <summary>
    /// Gets the SQL console control.
    /// </summary>
    public SqlConsoleControl InteractiveSql => SqlConsole;
}
