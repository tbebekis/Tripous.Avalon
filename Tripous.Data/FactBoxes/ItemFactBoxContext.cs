/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Provides the non-UI context passed to an item FactBox provider.
/// </summary>
public class ItemFactBoxContext
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemFactBoxContext()
    {
    }

    // ● properties
    /// <summary>
    /// The form registration name.
    /// </summary>
    public string FormName { get; set; }
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
    /// The FactBox definition.
    /// </summary>
    public ItemFactBoxDef FactBoxDef { get; set; }
    /// <summary>
    /// The data module.
    /// </summary>
    [JsonIgnore]
    public DataModule Module { get; set; }
    /// <summary>
    /// The module definition.
    /// </summary>
    [JsonIgnore]
    public ModuleDef ModuleDef => Module?.ModuleDef;
    /// <summary>
    /// The top table definition.
    /// </summary>
    [JsonIgnore]
    public TableDef TableDef => ModuleDef?.Table;
    /// <summary>
    /// The current item row.
    /// </summary>
    [JsonIgnore]
    public DataRow Row { get; set; }
    /// <summary>
    /// The current item row state.
    /// </summary>
    public string RowState { get; set; }
    /// <summary>
    /// The current item key value.
    /// </summary>
    public object KeyValue { get; set; }
}
