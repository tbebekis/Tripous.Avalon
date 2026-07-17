/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a <see cref="DataModule"/> used by Tripous Web.
/// </summary>
public class JsonDataModule
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataModule()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataModule(DataModule Source)
    {
        if (Source == null)
            throw new TripousArgumentNullException(nameof(Source));

        ModuleDef ModuleDef = Source.ModuleDef;

        Name = Source.Name;
        State = (int)Source.State;

        if (ModuleDef != null)
        {
            Title = ModuleDef.Title;
            TitleKey = ModuleDef.TitleKey;
            Group = ModuleDef.Group;
            ClassName = ModuleDef.ClassName;
            ConnectionName = ModuleDef.ConnectionName;
            Description = ModuleDef.Description;
            IsSingleSelect = ModuleDef.IsSingleSelect;
            UseFilters = ModuleDef.UseFilters;
            SecurityLevel = (int)ModuleDef.SecurityLevel;
            GuidOids = ModuleDef.GuidOids;
            CascadeDeletes = ModuleDef.CascadeDeletes;
            ItemCaptionField = ModuleDef.ItemCaptionField;
            MainTableName = ModuleDef.Table.Name;
            QueryNames.AddRange(ModuleDef.SelectList.Select(item => item.Name));
            StockNames.AddRange(ModuleDef.Stocks.Select(item => item.Name));
        }

        if (Source.tblList != null)
            ListTableName = Source.tblList.TableName;
        if (Source.tblItem != null)
            ItemTableName = Source.tblItem.TableName;

        DataSet = Source.DataSet != null ? new JsonDataSet(Source.DataSet, ModuleDef) : new JsonDataSet();
    }

    // ● static public
    /// <summary>
    /// Converts a data module to a JSON object.
    /// </summary>
    static public JsonObject ToJObject(DataModule Source)
    {
        JsonDataModule Instance = new(Source);
        return Instance.ToJObject();
    }

    // ● public
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => !string.IsNullOrWhiteSpace(Name)? Name: base.ToString();
    /// <summary>
    /// Converts this instance to a JSON object.
    /// </summary>
    public JsonObject ToJObject()
    {
        string JsonText = Json.Serialize(this);
        return JsonNode.Parse(JsonText) as JsonObject;
    }
    /// <summary>
    /// Converts this instance to JSON text.
    /// </summary>
    public string ToJson() => Json.Serialize(this);

    // ● properties
    /// <summary>
    /// The module name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The module title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// The module title localization key.
    /// </summary>
    public string TitleKey { get; set; }
    /// <summary>
    /// The module group.
    /// </summary>
    public string Group { get; set; }
    /// <summary>
    /// The module class name.
    /// </summary>
    public string ClassName { get; set; }
    /// <summary>
    /// The connection name.
    /// </summary>
    public string ConnectionName { get; set; }
    /// <summary>
    /// The module description.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// True when the module has a fixed single select.
    /// </summary>
    public bool IsSingleSelect { get; set; }
    /// <summary>
    /// True when filters should be used by UI.
    /// </summary>
    public bool UseFilters { get; set; }
    /// <summary>
    /// The minimum security level required to access the module as a <see cref="UserLevel"/> integer value.
    /// </summary>
    public int SecurityLevel { get; set; }
    /// <summary>
    /// True when key values are Guid strings.
    /// </summary>
    public bool GuidOids { get; set; }
    /// <summary>
    /// True when deletes should cascade from details to master.
    /// </summary>
    public bool CascadeDeletes { get; set; }
    /// <summary>
    /// The field used as item caption.
    /// </summary>
    public string ItemCaptionField { get; set; }
    /// <summary>
    /// The main table name.
    /// </summary>
    public string MainTableName { get; set; }
    /// <summary>
    /// The list table name.
    /// </summary>
    public string ListTableName { get; set; }
    /// <summary>
    /// The item table name.
    /// </summary>
    public string ItemTableName { get; set; }
    /// <summary>
    /// The data module state as a <see cref="DataMode"/> integer value.
    /// </summary>
    public int State { get; set; }
    /// <summary>
    /// The select query names.
    /// </summary>
    public List<string> QueryNames { get; set; } = [];
    /// <summary>
    /// The stock table names.
    /// </summary>
    public List<string> StockNames { get; set; } = [];
    /// <summary>
    /// The data set.
    /// </summary>
    public JsonDataSet DataSet { get; set; } = new();
}
