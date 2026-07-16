/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns database connections available to the database workbench.
/// </summary>
[AjaxOperation("App.DatabaseWorkbench.GetConnections")]
public class DatabaseWorkbenchGetConnections: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        AjaxResponse Result = new(Request.OperationName);
        Result["Connections"] = Db.Connections.List.Select(item => new
        {
            item.Name,
            DbServerType = item.DbServerType.ToString(),
            item.CommandTimeoutSeconds
        }).ToArray();
        Result["ShowWarningOnExecStatements"] = Sys.AsBoolean(Config.GetValue(Config.SShowWarningOnExecStatements), true);
        return Result;
    }
}
