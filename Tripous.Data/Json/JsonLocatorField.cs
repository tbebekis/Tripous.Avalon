/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a locator field.
/// </summary>
public class JsonLocatorField
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorField()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonLocatorField(LocatorFieldDef Source)
    {
        if (Source != null)
        {
            Name = Source.Name;
            DataType = (int)Source.DataType;
        }
    }

    // ● properties
    /// <summary>
    /// The field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The field data type as a <see cref="DataFieldType"/> integer value.
    /// </summary>
    public int DataType { get; set; } = (int)DataFieldType.String;
}
