/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a data table used by Tripous Web.
/// </summary>
public class JsonDataTable
{
    // ● private
    static object ConvertValue(object Value, Type DataType)
    {
        if (Value == null)
            return DBNull.Value;
        if (Value is JsonElement Element)
        {
            if (Element.ValueKind == JsonValueKind.Null || Element.ValueKind == JsonValueKind.Undefined)
                return DBNull.Value;
            if (DataType == typeof(string))
                return Element.ValueKind == JsonValueKind.String ? Element.GetString() : Element.ToString();
            if (DataType == typeof(int))
            {
                if (Element.ValueKind == JsonValueKind.True)
                    return 1;
                if (Element.ValueKind == JsonValueKind.False)
                    return 0;
                return Element.GetInt32();
            }
            if (DataType == typeof(long))
            {
                if (Element.ValueKind == JsonValueKind.True)
                    return 1L;
                if (Element.ValueKind == JsonValueKind.False)
                    return 0L;
                return Element.GetInt64();
            }
            if (DataType == typeof(double))
                return Element.GetDouble();
            if (DataType == typeof(decimal))
                return Element.GetDecimal();
            if (DataType == typeof(bool))
            {
                if (Element.ValueKind == JsonValueKind.Number)
                    return Element.GetInt32() != 0;
                return Element.GetBoolean();
            }
            if (DataType == typeof(DateTime))
                return Element.GetDateTime();
            return Element.ToString();
        }
        if (Value == DBNull.Value)
            return DBNull.Value;
        if (DataType == typeof(string))
            return Convert.ToString(Value);
        return Convert.ChangeType(Value, DataType, CultureInfo.InvariantCulture);
    }
    static object[] ConvertData(JsonDataRow Row, DataTable Table)
    {
        object[] Result = new object[Table.Columns.Count];
        for (int i = 0; i < Result.Length; i++)
        {
            object Value = Row.Data != null && i < Row.Data.Length ? Row.Data[i] : null;
            Result[i] = ConvertValue(Value, Table.Columns[i].DataType);
        }
        return Result;
    }
    static FieldDef FindFieldDef(TableDef TableDef, DataColumn Column)
    {
        if (Column != null && Column.ExtendedProperties.ContainsKey("Descriptor"))
        {
            FieldDef FieldDef = Column.ExtendedProperties["Descriptor"] as FieldDef;
            if (FieldDef != null)
                return FieldDef;
        }

        if (TableDef != null && Column != null)
        {
            Tuple<TableDef, FieldDef> Pair = TableDef.FindAnyField(Column.ColumnName);
            return Pair != null ? Pair.Item2 : null;
        }

        return null;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataTable()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataTable(DataTable Source)
        : this(Source, null)
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataTable(DataTable Source, TableDef TableDef)
    {
        Name = Source.TableName;

        if (Source is MemTable Table)
        {
            KeyField = Table.KeyField;
            MasterField = Table.MasterField;
            DetailField = Table.DetailField;
            MasterTableName = Table.Master != null ? Table.Master.TableName : string.Empty;
            AutoGenerateGuidKeys = Table.AutoGenerateGuidKeys;
            Details.AddRange(Table.Details.Select(item => item.TableName));
        }
        else if (TableDef != null)
        {
            KeyField = TableDef.KeyField;
            MasterField = TableDef.MasterField;
            DetailField = TableDef.DetailField;
            MasterTableName = TableDef.Master != null ? TableDef.Master.Name : string.Empty;
            Details.AddRange(TableDef.Details.Select(item => item.Name));
        }

        if (TableDef != null)
            Locators = new JsonLocatorList(TableDef);

        foreach (DataColumn SourceColumn in Source.Columns)
            Columns.Add(new JsonDataColumn(SourceColumn, FindFieldDef(TableDef, SourceColumn)));

        foreach (DataRow SourceRow in Source.Rows)
        {
            if (SourceRow.RowState == DataRowState.Deleted)
                Deleted.Add(new JsonDataRow(SourceRow));
            else
                Rows.Add(new JsonDataRow(SourceRow));
        }
    }

    // ● static public
    /// <summary>
    /// Converts a data table to a JSON object.
    /// </summary>
    static public JsonObject ToJObject(DataTable Source, TableDef TableDef = null)
    {
        JsonDataTable Instance = new(Source, TableDef);
        return Instance.ToJObject();
    }

    // ● public
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => !string.IsNullOrWhiteSpace(Name)? Name: base.ToString();
    /// <summary>
    /// Returns the index of a column by name.
    /// </summary>
    public int IndexOfColumn(string ColumnName)
    {
        if (string.IsNullOrWhiteSpace(ColumnName) || Columns == null)
            return -1;
        for (int i = 0; i < Columns.Count; i++)
        {
            if (Columns[i] != null && ColumnName.IsSameText(Columns[i].Name))
                return i;
        }
        return -1;
    }
    /// <summary>
    /// Returns a row value by column name, using this JSON table for column lookup.
    /// </summary>
    public object GetValue(JsonDataRow Row, string ColumnName)
    {
        if (Row == null)
            return null;
        return Row.GetValue(this, ColumnName);
    }
    /// <summary>
    /// Returns a row value by column name, using a <see cref="DataTable"/> for column lookup.
    /// </summary>
    public object GetValue(JsonDataRow Row, DataTable Table, string ColumnName)
    {
        DataColumn Column = null;
        if (Row == null)
            return null;
        if (Table != null && !string.IsNullOrWhiteSpace(ColumnName))
        {
            for (int i = 0; i < Table.Columns.Count; i++)
            {
                if (ColumnName.IsSameText(Table.Columns[i].ColumnName))
                {
                    Column = Table.Columns[i];
                    break;
                }
            }
        }
        if (Column == null)
            return null;
        return ConvertValue(Row.GetValue(Column.Ordinal), Column.DataType);
    }

    /// <summary>
    /// Copies rows to a data table, preserving row state.
    /// </summary>
    public void RowsTo(DataTable Table)
    {
        DataRow Row;

        if (Rows != null && Rows.Count > 0)
        {
            foreach (JsonDataRow JsonRow in Rows)
            {
                Row = Table.NewRow();
                Row.ItemArray = ConvertData(JsonRow, Table);
                Table.Rows.Add(Row);
                Row.AcceptChanges();

                if (JsonRow.State == DataRowState.Added)
                    Row.SetAdded();
                else if (JsonRow.State == DataRowState.Modified)
                    Row.SetModified();
            }
        }

        if (Deleted != null && Deleted.Count > 0)
        {
            foreach (JsonDataRow JsonRow in Deleted)
            {
                Row = Table.NewRow();
                Row.ItemArray = ConvertData(JsonRow, Table);
                Table.Rows.Add(Row);
                Row.AcceptChanges();
                Row.Delete();
            }
        }
    }
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
    /// The table name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The key field name.
    /// </summary>
    public string KeyField { get; set; } = "Id";
    /// <summary>
    /// The master field name.
    /// </summary>
    public string MasterField { get; set; } = "Id";
    /// <summary>
    /// The detail field name.
    /// </summary>
    public string DetailField { get; set; } = "Id";
    /// <summary>
    /// The master table name.
    /// </summary>
    public string MasterTableName { get; set; } = string.Empty;
    /// <summary>
    /// True when key values should be generated as Guid strings.
    /// </summary>
    public bool AutoGenerateGuidKeys { get; set; } = true;
    /// <summary>
    /// The detail table names.
    /// </summary>
    public List<string> Details { get; set; } = [];
    /// <summary>
    /// The table-specific locator definitions.
    /// </summary>
    public JsonLocatorList Locators { get; set; } = new();
    /// <summary>
    /// The columns.
    /// </summary>
    public List<JsonDataColumn> Columns { get; set; } = [];
    /// <summary>
    /// The rows.
    /// </summary>
    public List<JsonDataRow> Rows { get; set; } = [];
    /// <summary>
    /// The deleted rows.
    /// </summary>
    public List<JsonDataRow> Deleted { get; set; } = [];
}
