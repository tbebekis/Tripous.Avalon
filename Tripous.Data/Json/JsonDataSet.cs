/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a data set used by Tripous Web.
/// </summary>
public class JsonDataSet
{
    // ● private
    static Dictionary<string, TableDef> CreateTableDefMap(ModuleDef ModuleDef)
    {
        Dictionary<string, TableDef> Result = new(StringComparer.OrdinalIgnoreCase);

        if (ModuleDef != null)
        {
            foreach (TableDef TableDef in ModuleDef.GetTables())
            {
                if (!string.IsNullOrWhiteSpace(TableDef.Name))
                    Result[TableDef.Name] = TableDef;
                if (!string.IsNullOrWhiteSpace(TableDef.Alias))
                    Result[TableDef.Alias] = TableDef;
            }
        }

        return Result;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataSet()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataSet(DataSet Source)
        : this(Source, null)
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataSet(DataSet Source, ModuleDef ModuleDef)
    {
        Dictionary<string, TableDef> TableDefMap = CreateTableDefMap(ModuleDef);

        Name = Source.DataSetName;

        foreach (DataTable SourceTable in Source.Tables)
        {
            TableDefMap.TryGetValue(SourceTable.TableName, out TableDef TableDef);
            Tables.Add(new JsonDataTable(SourceTable, TableDef));
        }
    }

    // ● static public
    /// <summary>
    /// Converts a data set to a JSON object.
    /// </summary>
    static public JsonObject ToJObject(DataSet Source, ModuleDef ModuleDef = null)
    {
        JsonDataSet Instance = new(Source, ModuleDef);
        return Instance.ToJObject();
    }

    // ● public
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
    /// The data set name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The tables.
    /// </summary>
    public List<JsonDataTable> Tables { get; set; } = [];
}
