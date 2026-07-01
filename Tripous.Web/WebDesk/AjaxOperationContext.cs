/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Provides context services to a WebDesk Ajax operation.
/// </summary>
public class AjaxOperationContext
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public AjaxOperationContext(IViewToStringConverter ViewToStringConverter)
    {
        this.ViewToStringConverter = ViewToStringConverter;
    }

    // ● properties
    /// <summary>
    /// Gets the view-to-string converter.
    /// </summary>
    public IViewToStringConverter ViewToStringConverter { get; }
}
