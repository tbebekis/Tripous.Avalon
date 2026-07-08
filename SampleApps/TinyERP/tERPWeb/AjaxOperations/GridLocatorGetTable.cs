/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns the table schema used by the grid locator demo.
/// </summary>
[AjaxOperation("Demo.GridLocator.GetTable")]
public class GridLocatorGetTable: AppAjaxOperation
{
    // ● private
    /// <summary>
    /// Returns the StockTradeLine table definition.
    /// </summary>
    TableDef GetTableDef()
    {
        ModuleDef ModuleDef = DataRegistry.Modules.Find("StockTrade");
        if (ModuleDef == null)
            Sys.Throw("Module not found: StockTrade");

        TableDef Result = ModuleDef.GetTables().FirstOrDefault(item => item.Name.IsSameText("StockTradeLine"));
        if (Result == null)
            Sys.Throw("Table not found: StockTradeLine");

        return Result;
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        TableDef TableDef = GetTableDef();
        MemTable Table = TableDef.CreateDescriptorTable(Db.DefaultStore);

        AjaxResponse Result = new(Request.OperationName);
        Result["Table"] = new JsonDataTable(Table, TableDef);
        return Result;
    }
}
