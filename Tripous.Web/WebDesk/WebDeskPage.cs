/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Base Razor page class for WebDesk views.
/// </summary>
public abstract class WebDeskPage<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
{
}

/// <summary>
/// Base Razor page class for WebDesk views.
/// </summary>
public abstract class WebDeskPage: WebDeskPage<dynamic>
{
}
