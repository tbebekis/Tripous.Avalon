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
