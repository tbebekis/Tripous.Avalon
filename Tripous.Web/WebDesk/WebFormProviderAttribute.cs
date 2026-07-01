/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Marks a class as a provider for a registered WebDesk form.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class WebFormProviderAttribute: Attribute
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public WebFormProviderAttribute(string WebFormName)
    {
        this.WebFormName = WebFormName;
    }

    // ● properties
    /// <summary>
    /// Gets the registered web form name handled by the provider.
    /// </summary>
    public string WebFormName { get; }
}
