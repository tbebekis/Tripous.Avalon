/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Marks a class as a WebDesk Ajax operation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AjaxOperationAttribute: Attribute
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public AjaxOperationAttribute(string Name)
    {
        this.Name = Name;
    }

    // ● properties
    /// <summary>
    /// Gets the operation name.
    /// </summary>
    public string Name { get; }
}
