/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Serializable field information for an item structure FactBox.
/// </summary>
public class ItemStructureFieldInfo
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemStructureFieldInfo()
    {
    }

    // ● properties
    /// <summary>
    /// The field name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The field title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// The field data type.
    /// </summary>
    public string DataType { get; set; }
    /// <summary>
    /// The field group.
    /// </summary>
    public string Group { get; set; }
    /// <summary>
    /// The maximum field length.
    /// </summary>
    public int MaxLength { get; set; }
    /// <summary>
    /// The display width.
    /// </summary>
    public int DisplayWidth { get; set; }
    /// <summary>
    /// The number of decimals.
    /// </summary>
    public int Decimals { get; set; }
    /// <summary>
    /// True when the field is nullable.
    /// </summary>
    public bool IsNullable { get; set; }
    /// <summary>
    /// True when the field is visible.
    /// </summary>
    public bool IsVisible { get; set; }
    /// <summary>
    /// True when the field is required.
    /// </summary>
    public bool IsRequired { get; set; }
    /// <summary>
    /// True when the field is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// The lookup source name, if any.
    /// </summary>
    public string LookupSource { get; set; }
    /// <summary>
    /// The locator name, if any.
    /// </summary>
    public string Locator { get; set; }
    /// <summary>
    /// The default value.
    /// </summary>
    public string DefaultValue { get; set; }
    /// <summary>
    /// The expression.
    /// </summary>
    public string Expression { get; set; }
    /// <summary>
    /// The code provider name.
    /// </summary>
    public string CodeProvider { get; set; }
    /// <summary>
    /// The snapshot source field.
    /// </summary>
    public string SnapshotOf { get; set; }
    /// <summary>
    /// The field flags.
    /// </summary>
    public string Flags { get; set; }
}
