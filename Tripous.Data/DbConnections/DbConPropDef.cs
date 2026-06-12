/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Defines a connection string property supported by a connection adapter.
/// </summary>
public class DbConPropDef
{
    // ● properties
    /// <summary>
    /// Gets or sets the connection property type.
    /// </summary>
    public DbConPropType PropType { get; set; }
    /// <summary>
    /// Gets or sets the display label used in the user interface.
    /// </summary>
    public string Label { get; set; } = "";
    /// <summary>
    /// Gets or sets a value indicating whether this property is required.
    /// </summary>
    public bool IsRequired { get; set; }
    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public string DefaultValue { get; set; } = "";
    /// <summary>
    /// Gets or sets the accepted connection string key aliases.
    /// </summary>
    public string[] Aliases { get; set; } = [];
    /// <summary>
    /// Gets or sets the list of valid values for this property.
    /// An empty array means that any value is accepted.
    /// </summary>
    public string[] ValidValues { get; set; } = [];
}