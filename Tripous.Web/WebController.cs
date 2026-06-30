/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Web;

/// <summary>
/// Base MVC controller for Tripous web applications.
/// </summary>
public class WebController: Controller, IViewToStringConverter
{
    // ● protected
    /// <summary>
    /// Returns the exception text suitable for a response.
    /// </summary>
    protected virtual string GetExceptionText(Exception e) => e.Message;

    // ● public
    /// <summary>
    /// Renders a view to a string.
    /// </summary>
    public string ViewToString(string ViewName, object Model, IDictionary<string, object> PlusViewData = null)
    {
        return this.RenderPartialViewToString(ViewName, Model, PlusViewData);
    }
    /// <summary>
    /// Renders a view to a string.
    /// </summary>
    public string ViewToString(string ViewName, IDictionary<string, object> PlusViewData = null)
    {
        return this.RenderPartialViewToString(ViewName, PlusViewData);
    }
}
