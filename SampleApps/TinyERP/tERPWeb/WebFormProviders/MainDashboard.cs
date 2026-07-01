/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.WebFormProviders;

/// <summary>
/// Provides the main dashboard web form.
/// </summary>
[WebFormProvider("MainDashboard")]
public class MainDashboard: WebFormProvider
{
    // ● protected
    /// <summary>
    /// Returns the HTML markup for the form.
    /// </summary>
    protected override string GetHtml(WebFormProviderContext Context)
    {
        return Context.ViewToStringConverter.ViewToString(Context.Form.ViewName, Context.Form);
    }
}
