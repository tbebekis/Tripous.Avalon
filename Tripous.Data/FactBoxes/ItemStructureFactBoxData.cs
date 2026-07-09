/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Serializable data describing the structure of an item module.
/// </summary>
public class ItemStructureFactBoxData
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemStructureFactBoxData()
    {
    }

    // ● properties
    /// <summary>
    /// The module name.
    /// </summary>
    public string ModuleName { get; set; }
    /// <summary>
    /// The module title.
    /// </summary>
    public string ModuleTitle { get; set; }
    /// <summary>
    /// The module group.
    /// </summary>
    public string ModuleGroup { get; set; }
    /// <summary>
    /// The module class name.
    /// </summary>
    public string ModuleClassName { get; set; }
    /// <summary>
    /// The JavaScript module class name.
    /// </summary>
    public string ModuleJsClassName { get; set; }
    /// <summary>
    /// The form class name.
    /// </summary>
    public string FormClassName { get; set; }
    /// <summary>
    /// The JavaScript form class name.
    /// </summary>
    public string FormJsClassName { get; set; }
    /// <summary>
    /// The item page class name.
    /// </summary>
    public string ItemPageClassName { get; set; }
    /// <summary>
    /// The JavaScript item page class name.
    /// </summary>
    public string ItemPageJsClassName { get; set; }
    /// <summary>
    /// The number of tables in the item table tree.
    /// </summary>
    public int TableCount { get; set; }
    /// <summary>
    /// The number of UI visible tables in the item table tree.
    /// </summary>
    public int VisibleTableCount { get; set; }
    /// <summary>
    /// The top table.
    /// </summary>
    public ItemStructureTableInfo Table { get; set; }
}
