/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a connection string property value.
/// </summary>
public class DbConProp
{
    // ● public
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => $"{PropType}: {Value}";

    // ● properties
    /// <summary>
    /// Gets or sets the connection property type.
    /// </summary>
    public DbConPropType PropType { get; set; }
    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    public string Value { get; set; } = "";
}