/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns the registered read-only views.
/// </summary>
[AjaxOperation("App.GetReadOnlyViews")]
public class GetReadOnlyViews: AppAjaxOperation
{
    // ● private
    /// <summary>
    /// Creates a reduced filter packet.
    /// </summary>
    static object CreateFilterPacket(SqlFilterDef FilterDef)
    {
        return new
        {
            FilterDef.Name,
            FilterDef.TitleKey,
            FilterDef.Title,
            FilterDef.FieldName,
            FilterDataType = FilterDef.FilterDataType.ToString(),
            BoolOp = FilterDef.BoolOp.ToString(),
            ConditionOp = FilterDef.ConditionOp.ToString()
        };
    }
    /// <summary>
    /// Creates a reduced view packet.
    /// </summary>
    static object CreateViewPacket(SelectDef SelectDef)
    {
        return new
        {
            SelectDef.Name,
            SelectDef.TitleKey,
            SelectDef.Title,
            SelectDef.UseFilters,
            Filters = SelectDef.FilterDefs.Select(CreateFilterPacket).ToArray()
        };
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        Result["Views"] = tERPWeb.ReadOnlyViews.GetAll().Select(CreateViewPacket).ToArray();
        return Result;
    }
}

/// <summary>
/// Selects a registered read-only view.
/// </summary>
[AjaxOperation("App.SelectReadOnlyView")]
public class SelectReadOnlyView: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        string ViewName = GetStringParam(Request, "ViewName");
        JsonSelectFilters Filters = JsonSelectFilters.From(Request.GetParam("Filters"));
        string SqlText = tERPWeb.ReadOnlyViews.BuildSql(ViewName, Filters, out SelectDef SelectDef);
        DataTable Table = Db.DefaultStore.Select(SqlText);
        Table.TableName = SelectDef.Name;

        AjaxResponse Result = new(Request.OperationName);
        Result["ViewName"] = SelectDef.Name;
        Result["Title"] = SelectDef.Title;
        Result["Table"] = new JsonDataTable(Table);
        return Result;
    }
}
