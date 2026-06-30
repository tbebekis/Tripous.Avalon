/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Represents an object that handles an <see cref="AjaxRequest"/>.
/// </summary>
public interface IAjaxRequestHandler
{
    // ● properties
    /// <summary>
    /// Gets the handler name.
    /// </summary>
    string Name { get; }

    // ● public
    /// <summary>
    /// Handles a specified request and returns a response when handled; otherwise null.
    /// </summary>
    AjaxResponse Handle(AjaxRequest Request, IViewToStringConverter ViewToStringConverter);
}
