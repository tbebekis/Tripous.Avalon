/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Desktop database explorer hosted in the left sidebar.
/// </summary>
public partial class DatabaseExplorerForm : AppForm
{
    // ● private
    void ConfigureExplorer()
    {
        Explorer.Options = new DbConnectionExplorerOptions
        {
            AllowAddConnections = false,
            AllowEditConnections = false,
            AllowDeleteConnections = false,
            AllowCreateDatabases = false,
            PersistConnectionChanges = false,
            ShowToolBar = true
        };
    }
    void OpenInteractiveSql(DbConnectionInfo ConnectionInfo, string SqlText = null)
    {
        if (ConnectionInfo == null)
            return;
        AppHost.OpenInteractiveSql(ConnectionInfo, SqlText);
    }

    // ● protected
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        TitleText = "Database Explorer";
        ConfigureExplorer();
        Explorer.SetConnections(Db.Connections.List);
        Explorer.OpenSqlRequested += (Sender, Args) => OpenInteractiveSql(Args.ConnectionInfo);
        Explorer.SqlTextRequested += (Sender, Args) => OpenInteractiveSql(Args.ConnectionInfo, Args.SqlText);
        Explorer.LogMessage += (Sender, Text) => AppHost.Log(Text);
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public DatabaseExplorerForm()
    {
        InitializeComponent();
    }
}
