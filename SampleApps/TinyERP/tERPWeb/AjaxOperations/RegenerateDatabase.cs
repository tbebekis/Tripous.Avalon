/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Deletes the sample SQLite database so it can be recreated on application restart.
/// </summary>
[AjaxOperation("App.RegenerateDatabase")]
public class RegenerateDatabase: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            throw new TripousException("Access denied.");

        string DatabaseFilePath = GetDefaultDatabaseFilePath();

        System.Data.SQLite.SQLiteConnection.ClearAllPools();
        if (File.Exists(DatabaseFilePath))
            File.Delete(DatabaseFilePath);

        AjaxResponse Result = new(Request.OperationName);
        Result["Success"] = true;
        Result["DatabaseFilePath"] = DatabaseFilePath;
        Result["Message"] = "The sample Sqlite database has been deleted. Restart the tERPWeb server process.";
        return Result;
    }
}
