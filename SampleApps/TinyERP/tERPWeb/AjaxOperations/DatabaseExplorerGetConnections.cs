/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns database connections available to the database explorer.
/// </summary>
[AjaxOperation("App.DatabaseExplorer.GetConnections")]
public class DatabaseExplorerGetConnections: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        AjaxResponse Result = new(Request.OperationName);
        Result["Connections"] = new[]
        {
            new
            {
                ConnectionInfo.Name,
                DbServerType = ConnectionInfo.DbServerType.ToString(),
                ConnectionInfo.CommandTimeoutSeconds
            }
        };
        Result["Options"] = new
        {
            AllowAddConnections = false,
            AllowEditConnections = false,
            AllowDeleteConnections = false,
            AllowCreateDatabases = false,
            ShowToolBar = true
        };
        Result["ShowWarningOnExecStatements"] = Sys.AsBoolean(Config.GetValue(Config.SShowWarningOnExecStatements), true);
        return Result;
    }
}
