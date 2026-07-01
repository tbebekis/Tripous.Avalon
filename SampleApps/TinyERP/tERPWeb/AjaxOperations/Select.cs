/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Executes a SELECT SQL statement and returns a JSON data table.
/// </summary>
[AjaxOperation("Select")]
public class Select: AppAjaxOperation
{
    // ● private
    DataTable SelectTable(SqlTextItem Item)
    {
        string ConnectionName = !string.IsNullOrWhiteSpace(Item.ConnectionName) ? Item.ConnectionName : Sys.DEFAULT;
        SqlStore Store = ConnectionName.IsSameText(Sys.DEFAULT) ? Db.DefaultStore : SqlStores.CreateSqlStore(ConnectionName);
        DataTable Result = Store.Select(Item.SqlText);
        if (!string.IsNullOrWhiteSpace(Item.Name))
            Result.TableName = Item.Name;
        return Result;
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        SqlTextItem Item = new(
            SqlText: GetStringParam(Request, "SqlText"),
            ConnectionName: GetStringParam(Request, "ConnectionName"),
            Name: GetStringParam(Request, "Name"));

        if (string.IsNullOrWhiteSpace(Item.SqlText))
            Sys.Throw("No SQL statement specified.");

        JsonDataTable Table = new(SelectTable(Item));

        AjaxResponse Result = new(Request.OperationName);
        Result["Table"] = Table;
        Result["JsonDataTable"] = Table;
        return Result;
    }
}
