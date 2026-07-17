/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a data row used by Tripous Web.
/// </summary>
public class JsonDataRow
{
    // ● private
    static object[] GetData(DataRow Source)
    {
        if (Source.RowState != DataRowState.Deleted)
            return Source.ItemArray;

        object[] Result = new object[Source.Table.Columns.Count];
        for (int i = 0; i < Result.Length; i++)
            Result[i] = Source[i, DataRowVersion.Original];
        return Result;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataRow()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataRow(DataRow Source)
    {
        Data = GetData(Source);
        State = Source.RowState;
    }

    // ● public
    /// <summary>
    /// Returns a value by column index.
    /// </summary>
    public object GetValue(int ColumnIndex)
    {
        if (Data == null || ColumnIndex < 0 || ColumnIndex >= Data.Length)
            return null;
        return Data[ColumnIndex];
    }
    /// <summary>
    /// Returns a value by column name, using a <see cref="JsonDataTable"/> for column lookup.
    /// </summary>
    public object GetValue(JsonDataTable Table, string ColumnName)
    {
        int Index = Table != null ? Table.IndexOfColumn(ColumnName) : -1;
        return GetValue(Index);
    }
    /// <summary>
    /// Returns a value by column name, using a <see cref="DataTable"/> for column lookup.
    /// </summary>
    public object GetValue(DataTable Table, string ColumnName)
    {
        int Index = -1;
        if (Table != null && !string.IsNullOrWhiteSpace(ColumnName))
        {
            for (int i = 0; i < Table.Columns.Count; i++)
            {
                if (ColumnName.IsSameText(Table.Columns[i].ColumnName))
                {
                    Index = i;
                    break;
                }
            }
        }
        return GetValue(Index);
    }

    // ● properties
    /// <summary>
    /// The row state.
    /// </summary>
    public DataRowState State { get; set; }
    /// <summary>
    /// The row data as an item array.
    /// </summary>
    public object[] Data { get; set; } = [];
}
