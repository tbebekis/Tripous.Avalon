/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Provides context services to a WebDesk form provider.
/// </summary>
public class WebFormProviderContext
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public WebFormProviderContext(AjaxRequest Request, WebFormDef Form, AjaxOperationContext AjaxContext)
    {
        this.Request = Request;
        this.Form = Form;
        this.AjaxContext = AjaxContext;
    }

    // ● properties
    /// <summary>
    /// Gets the Ajax request.
    /// </summary>
    public AjaxRequest Request { get; }
    /// <summary>
    /// Gets the web form definition.
    /// </summary>
    public WebFormDef Form { get; }
    /// <summary>
    /// Gets the Ajax operation context.
    /// </summary>
    public AjaxOperationContext AjaxContext { get; }
    /// <summary>
    /// Gets the view-to-string converter.
    /// </summary>
    public IViewToStringConverter ViewToStringConverter => AjaxContext.ViewToStringConverter;
}
