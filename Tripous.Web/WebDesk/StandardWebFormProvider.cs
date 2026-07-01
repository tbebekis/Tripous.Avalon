/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Default provider for standard WebDesk forms.
/// </summary>
public class StandardWebFormProvider: WebFormProvider
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
