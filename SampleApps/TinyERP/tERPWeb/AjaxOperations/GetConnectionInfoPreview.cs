/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Builds a database connection string preview.
/// </summary>
[AjaxOperation("App.GetConnectionInfoPreview")]
public class GetConnectionInfoPreview: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        DbServerType ServerType = GetDbServerTypeParam(Request, ConnectionInfo.DbServerType);
        DbConAdapter Adapter = DbConAdapters.Get(ServerType);
        List<DbConProp> Props = GetConnectionProps(Request, Adapter);
        AjaxResponse Result = new(Request.OperationName);

        try
        {
            Result["Success"] = true;
            Result["ConnectionString"] = Adapter.Construct(Props);
            Result["Message"] = string.Empty;
        }
        catch (Exception e)
        {
            Result["Success"] = false;
            Result["ConnectionString"] = e.Message;
            Result["Message"] = e.Message;
        }

        return Result;
    }
}
