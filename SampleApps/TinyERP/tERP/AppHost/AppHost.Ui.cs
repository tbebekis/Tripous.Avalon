/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class AppHost
{
    // ● public
    static public void ShowSideBarPages()
    {
        SideBarHandler.ShowAppForm(CommandTreeViewForm.CreateFormContext());
        ShowDatabaseExplorer();
        SideBarHandler.Pager.SelectedIndex = 0;
    }
    /// <summary>
    /// Shows the database explorer in the left sidebar.
    /// </summary>
    static public AppForm ShowDatabaseExplorer()
    {
        FormContext Context = FormContext.Create("DatabaseExplorer", typeof(DatabaseExplorerForm).FullName, FormDisplayMode.TabItem, AppHost.MainWindow);
        Context.Title = Texts.L("DatabaseExplorer", "Database Explorer");
        return SideBarHandler.ShowAppForm(Context);
    }
    /// <summary>
    /// Opens an interactive SQL tab for a database connection.
    /// </summary>
    /// <param name="ConnectionInfo">The database connection information.</param>
    /// <param name="SqlText">Optional SQL text.</param>
    /// <returns>The opened form.</returns>
    static public AppForm OpenInteractiveSql(DbConnectionInfo ConnectionInfo, string SqlText = null)
    {
        if (ConnectionInfo == null)
            return null;
        string FormId = "InteractiveSql." + ConnectionInfo.Name + "." + Sys.GenId();
        FormContext Context = FormContext.Create(FormId, typeof(InteractiveSqlForm).FullName, FormDisplayMode.TabItem, AppHost.MainWindow, ConnectionInfo);
        Context.Title = Texts.L("InteractiveSQL", "Interactive SQL") + " - " + ConnectionInfo.Name;
        if (!string.IsNullOrWhiteSpace(SqlText))
            Context.Params["SqlText"] = SqlText;
        return ContentHandler.ShowAppForm(Context);
    }
}
