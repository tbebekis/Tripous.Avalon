/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */
namespace Tripous.Data;

/// <summary>
/// A helper class for returning typed values from a DataRowView in a safe manner,
/// by leveraging the existing DataRowExtensions.
/// </summary>
static public class DataRowViewExtensions
{
    // ● get column value by ColumnName  
    /// <summary>
    /// Returns the value of the specified column as an <see cref="object"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public object AsObject(this DataRowView drv, string ColumnName, object Default) => drv.Row.AsObject(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as an <see cref="int"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public int AsInteger(this DataRowView drv, string ColumnName, int Default) => drv.Row.AsInteger(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as an <see cref="int"/>, or zero if the value is null or DBNull.
    /// </summary>
    static public int AsInteger(this DataRowView drv, string ColumnName) => drv.Row.AsInteger(ColumnName);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="string"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public string AsString(this DataRowView drv, string ColumnName, string Default) => drv.Row.AsString(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="string"/>, or an empty string if the value is null or DBNull.
    /// </summary>
    static public string AsString(this DataRowView drv, string ColumnName) => drv.Row.AsString(ColumnName);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="double"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public double AsDouble(this DataRowView drv, string ColumnName, double Default) => drv.Row.AsDouble(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="double"/>, or zero if the value is null or DBNull.
    /// </summary>
    static public double AsDouble(this DataRowView drv, string ColumnName) => drv.Row.AsDouble(ColumnName);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="decimal"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public decimal AsDecimal(this DataRowView drv, string ColumnName, decimal Default) => drv.Row.AsDecimal(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="decimal"/>, or zero if the value is null or DBNull.
    /// </summary>
    static public decimal AsDecimal(this DataRowView drv, string ColumnName) => drv.Row.AsDecimal(ColumnName);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="bool"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public bool AsBoolean(this DataRowView drv, string ColumnName, bool Default) => drv.Row.AsBoolean(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="bool"/>, or false if the value is null or DBNull.
    /// </summary>
    static public bool AsBoolean(this DataRowView drv, string ColumnName) => drv.Row.AsBoolean(ColumnName);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="DateTime"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public DateTime AsDateTime(this DataRowView drv, string ColumnName, DateTime Default) => drv.Row.AsDateTime(ColumnName, Default);
    /// <summary>
    /// Returns the value of the specified column as a <see cref="DateTime"/>, or <see cref="DateTime.MinValue"/> if the value is null or DBNull.
    /// </summary>
    static public DateTime AsDateTime(this DataRowView drv, string ColumnName) => drv.Row.AsDateTime(ColumnName);

    // ● get column value by ColumnIndex  
    /// <summary>
    /// Returns the value of the column at the specified index as an <see cref="object"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public object AsObject(this DataRowView drv, int ColumnIndex, object Default) => drv.Row.AsObject(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as an <see cref="int"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public int AsInteger(this DataRowView drv, int ColumnIndex, int Default) => drv.Row.AsInteger(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as an <see cref="int"/>, or zero if the value is null or DBNull.
    /// </summary>
    static public int AsInteger(this DataRowView drv, int ColumnIndex) => drv.Row.AsInteger(ColumnIndex);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="string"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public string AsString(this DataRowView drv, int ColumnIndex, string Default) => drv.Row.AsString(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="string"/>, or an empty string if the value is null or DBNull.
    /// </summary>
    static public string AsString(this DataRowView drv, int ColumnIndex) => drv.Row.AsString(ColumnIndex);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="double"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public double AsDouble(this DataRowView drv, int ColumnIndex, double Default) => drv.Row.AsDouble(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="double"/>, or zero if the value is null or DBNull.
    /// </summary>
    static public double AsDouble(this DataRowView drv, int ColumnIndex) => drv.Row.AsDouble(ColumnIndex);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="decimal"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public decimal AsDecimal(this DataRowView drv, int ColumnIndex, decimal Default) => drv.Row.AsDecimal(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="decimal"/>, or zero if the value is null or DBNull.
    /// </summary>
    static public decimal AsDecimal(this DataRowView drv, int ColumnIndex) => drv.Row.AsDecimal(ColumnIndex);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="bool"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public bool AsBoolean(this DataRowView drv, int ColumnIndex, bool Default) => drv.Row.AsBoolean(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="bool"/>, or false if the value is null or DBNull.
    /// </summary>
    static public bool AsBoolean(this DataRowView drv, int ColumnIndex) => drv.Row.AsBoolean(ColumnIndex);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="DateTime"/>, or <paramref name="Default"/> if the value is null or DBNull.
    /// </summary>
    static public DateTime AsDateTime(this DataRowView drv, int ColumnIndex, DateTime Default) => drv.Row.AsDateTime(ColumnIndex, Default);
    /// <summary>
    /// Returns the value of the column at the specified index as a <see cref="DateTime"/>, or <see cref="DateTime.MinValue"/> if the value is null or DBNull.
    /// </summary>
    static public DateTime AsDateTime(this DataRowView drv, int ColumnIndex) => drv.Row.AsDateTime(ColumnIndex);

    // ● copy-append  
    /// <summary>
    /// Copies the values of <paramref name="Source"/> to <paramref name="Dest"/>.
    /// </summary>
    static public void CopyTo(this DataRowView Source, DataRowView Dest)
    {
        Dest.BeginEdit();
        Source.Row.CopyTo(Dest.Row);
        Dest.EndEdit();
    }
    /// <summary>
    /// Safely copies the values of <paramref name="Source"/> to <paramref name="Dest"/>, copying only columns that exist in both rows.
    /// </summary>
    static public void SafeCopyTo(this DataRowView Source, DataRowView Dest)
    {
        Dest.BeginEdit();
        Source.Row.SafeCopyTo(Dest.Row);
        Dest.EndEdit();
    }

    // ● blobs  
    /// <summary>
    /// Reads the contents of <paramref name="Stream"/> and stores them in the specified blob field.
    /// </summary>
    static public void StreamToBlob(this DataRowView drv, string FieldName, Stream Stream)
    {
        drv.BeginEdit();
        drv.Row.StreamToBlob(FieldName, Stream);
        drv.EndEdit();
    }
    /// <summary>
    /// Writes the contents of the specified blob field to <paramref name="Stream"/>.
    /// </summary>
    static public void BlobToStream(this DataRowView drv, string FieldName, Stream Stream) => drv.Row.BlobToStream(FieldName, Stream);
    /// <summary>
    /// Returns the contents of the specified blob field as a new <see cref="MemoryStream"/>.
    /// </summary>
    static public MemoryStream BlobToStream(this DataRowView drv, string FieldName) => drv.Row.BlobToStream(FieldName);
    /// <summary>
    /// Reads the contents of <paramref name="Stream"/> and stores them in the specified blob field.
    /// </summary>
    static public void LoadFromStream(this DataRowView drv, string BlobFieldName, Stream Stream)
    {
        drv.BeginEdit();
        drv.Row.LoadFromStream(BlobFieldName, Stream);
        drv.EndEdit();
    }
    /// <summary>
    /// Writes the contents of the specified blob field to <paramref name="Stream"/>.
    /// </summary>
    static public void SaveToStream(this DataRowView drv, string BlobFieldName, Stream Stream) => drv.Row.SaveToStream(BlobFieldName, Stream);
    /// <summary>
    /// Converts <paramref name="Value"/> to bytes and stores it in the specified blob field.
    /// </summary>
    static public void StringToBlob(this DataRowView drv, string BlobFieldName, string Value)
    {
        drv.BeginEdit();
        drv.Row.StringToBlob(BlobFieldName, Value);
        drv.EndEdit();
    }
    /// <summary>
    /// Returns the contents of the specified blob field as a <see cref="string"/>.
    /// </summary>
    static public string BlobToString(this DataRowView drv, string BlobFieldName) => drv.Row.BlobToString(BlobFieldName);

    // ● miscs  
    /// <summary>
    /// Returns true and sets <paramref name="Value"/> to the value of the specified field, if the field exists; otherwise returns false.
    /// </summary>
    static public bool TryGetValue(this DataRowView drv, string FieldName, out object Value) => drv.Row.TryGetValue(FieldName, out Value);
}