/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb;

/// <summary>
/// Test Ajax request handler for tERPWeb.
/// </summary>
public class TestAjaxRequestHandler: IAjaxRequestHandler
{
    // ● public
    /// <summary>
    /// Handles a specified request and returns a response when handled; otherwise null.
    /// </summary>
    public AjaxResponse Handle(AjaxRequest Request, IViewToStringConverter ViewToStringConverter)
    {
        if (!Sys.IsSameText(Request.OperationName, "App.Ping"))
            return null;

        AjaxResponse Result = new(Request.OperationName);
        Result["Message"] = "Pong";
        Result["ServerTimeUtc"] = DateTime.UtcNow;
        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets the handler name.
    /// </summary>
    public string Name => nameof(TestAjaxRequestHandler);
}
