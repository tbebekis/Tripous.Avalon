/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns a ping response.
/// </summary>
[AjaxOperation("App.Ping")]
public class Ping: AjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        Result["Message"] = "Pong";
        Result["ServerTimeUtc"] = DateTime.UtcNow;
        return Result;
    }
}
