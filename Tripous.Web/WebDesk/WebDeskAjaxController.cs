/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Handles WebDesk Ajax requests.
/// </summary>
public class WebDeskAjaxController: WebDeskController
{
    // ● public
    /// <summary>
    /// Executes a WebDesk Ajax request.
    /// </summary>
    [HttpPost("/Ajax/Execute")]
    public async Task<JsonResult> AjaxExecute([FromBody] AjaxRequest Request)
    {
        await Task.CompletedTask;

        HttpPacketResult Result = new();

        try
        {
            AjaxResponse Response = AjaxRequestHandlers.Handle(Request, this);

            if (Response == null)
                throw new TripousException($"Ajax operation not supported: {Request.OperationName}");

            Result = HttpPacketResult.SetPacket(Response.GetPacketObject(), true);
        }
        catch (Exception e)
        {
            Result.ErrorText = GetExceptionText(e);
        }

        return Json(Result);
    }
}
