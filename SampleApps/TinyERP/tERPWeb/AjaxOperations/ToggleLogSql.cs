/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Toggles SQL statement logging.
/// </summary>
[AjaxOperation("App.ToggleLogSql")]
public class ToggleLogSql: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        bool Flag = !Db.Settings.LogSqlStatements;
        Db.Settings.LogSqlStatements = Flag;

        AjaxResponse Result = new(Request.OperationName);
        Result["Success"] = true;
        Result["Enabled"] = Flag;
        Result["Message"] = $"SQL Statements Logging is now: {(Flag ? "ON" : "OFF")}.";
        return Result;
    }
}
