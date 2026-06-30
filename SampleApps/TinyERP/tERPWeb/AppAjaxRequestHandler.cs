/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb;

/// <summary>
/// Ajax request handler for tERPWeb application operations.
/// </summary>
public class AppAjaxRequestHandler: IAjaxRequestHandler
{
    // ● private
    /// <summary>
    /// Handles the ping request.
    /// </summary>
    AjaxResponse HandlePing(AjaxRequest Request)
    {
        AjaxResponse Result = new(Request.OperationName);
        Result["Message"] = "Pong";
        Result["ServerTimeUtc"] = DateTime.UtcNow;
        return Result;
    }
    /// <summary>
    /// Handles the get web forms request.
    /// </summary>
    AjaxResponse HandleGetWebForms(AjaxRequest Request)
    {
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        var Forms = WebDeskRegistry.Forms
            .Where(Form => Form.CanAccess(User))
            .OrderBy(Form => Form.Group)
            .ThenBy(Form => Form.Title)
            .Select(Form => new
            {
                Form.Name,
                Form.TitleKey,
                Form.Title,
                Form.Module,
                Form.ViewName,
                Form.ItemViewName,
                Form.Group,
                Form.IsReadOnly
            })
            .ToArray();

        AjaxResponse Result = new(Request.OperationName);
        Result["WebForms"] = Forms;
        return Result;
    }

    // ● public
    /// <summary>
    /// Handles a specified request and returns a response when handled; otherwise null.
    /// </summary>
    public AjaxResponse Handle(AjaxRequest Request, IViewToStringConverter ViewToStringConverter)
    {
        if (Sys.IsSameText(Request.OperationName, "App.Ping"))
            return HandlePing(Request);
        if (Sys.IsSameText(Request.OperationName, "App.GetWebForms"))
            return HandleGetWebForms(Request);
        return null;
    }

    // ● properties
    /// <summary>
    /// Gets the handler name.
    /// </summary>
    public string Name => nameof(AppAjaxRequestHandler);
}
