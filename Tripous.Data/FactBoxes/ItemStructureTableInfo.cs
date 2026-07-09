/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Serializable table information for an item structure FactBox.
/// </summary>
public class ItemStructureTableInfo
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemStructureTableInfo()
    {
    }

    // ● properties
    /// <summary>
    /// The table name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The table title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// The table alias.
    /// </summary>
    public string Alias { get; set; }
    /// <summary>
    /// The primary key field.
    /// </summary>
    public string KeyField { get; set; }
    /// <summary>
    /// The master field.
    /// </summary>
    public string MasterField { get; set; }
    /// <summary>
    /// The detail field.
    /// </summary>
    public string DetailField { get; set; }
    /// <summary>
    /// The master table name.
    /// </summary>
    public string MasterName { get; set; }
    /// <summary>
    /// The direct detail table names.
    /// </summary>
    public List<string> DetailNames { get; set; } = [];
    /// <summary>
    /// True when this is a detail table.
    /// </summary>
    public bool IsDetail { get; set; }
    /// <summary>
    /// True when this table is UI visible.
    /// </summary>
    public bool IsUiVisible { get; set; }
    /// <summary>
    /// True when this is a one-to-one detail table.
    /// </summary>
    public bool IsOneToOne { get; set; }
    /// <summary>
    /// The field count.
    /// </summary>
    public int FieldCount { get; set; }
    /// <summary>
    /// The visible field count.
    /// </summary>
    public int VisibleFieldCount { get; set; }
    /// <summary>
    /// The join table count.
    /// </summary>
    public int JoinCount { get; set; }
    /// <summary>
    /// The stock select count.
    /// </summary>
    public int StockCount { get; set; }
    /// <summary>
    /// The fields.
    /// </summary>
    public List<ItemStructureFieldInfo> Fields { get; set; } = [];
    /// <summary>
    /// The detail tables.
    /// </summary>
    public List<ItemStructureTableInfo> Details { get; set; } = [];
}
