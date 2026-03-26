namespace Tripous
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// A helper class for returning typed values from a DataRowView in a safe manner,
    /// by leveraging the existing DataRowExtensions.
    /// </summary>
    static public class DataRowViewExtensions
    {
        /* get column value by ColumnName */
        static public object AsObject(this DataRowView drv, string ColumnName, object Default) => drv.Row.AsObject(ColumnName, Default);
        static public int AsInteger(this DataRowView drv, string ColumnName, int Default) => drv.Row.AsInteger(ColumnName, Default);
        static public int AsInteger(this DataRowView drv, string ColumnName) => drv.Row.AsInteger(ColumnName);
        static public string AsString(this DataRowView drv, string ColumnName, string Default) => drv.Row.AsString(ColumnName, Default);
        static public string AsString(this DataRowView drv, string ColumnName) => drv.Row.AsString(ColumnName);
        static public double AsDouble(this DataRowView drv, string ColumnName, double Default) => drv.Row.AsDouble(ColumnName, Default);
        static public double AsDouble(this DataRowView drv, string ColumnName) => drv.Row.AsDouble(ColumnName);
        static public decimal AsDecimal(this DataRowView drv, string ColumnName, decimal Default) => drv.Row.AsDecimal(ColumnName, Default);
        static public decimal AsDecimal(this DataRowView drv, string ColumnName) => drv.Row.AsDecimal(ColumnName);
        static public bool AsBoolean(this DataRowView drv, string ColumnName, bool Default) => drv.Row.AsBoolean(ColumnName, Default);
        static public bool AsBoolean(this DataRowView drv, string ColumnName) => drv.Row.AsBoolean(ColumnName);
        static public DateTime AsDateTime(this DataRowView drv, string ColumnName, DateTime Default) => drv.Row.AsDateTime(ColumnName, Default);
        static public DateTime AsDateTime(this DataRowView drv, string ColumnName) => drv.Row.AsDateTime(ColumnName);

        /* get column value by ColumnIndex */
        static public object AsObject(this DataRowView drv, int ColumnIndex, object Default) => drv.Row.AsObject(ColumnIndex, Default);
        static public int AsInteger(this DataRowView drv, int ColumnIndex, int Default) => drv.Row.AsInteger(ColumnIndex, Default);
        static public int AsInteger(this DataRowView drv, int ColumnIndex) => drv.Row.AsInteger(ColumnIndex);
        static public string AsString(this DataRowView drv, int ColumnIndex, string Default) => drv.Row.AsString(ColumnIndex, Default);
        static public string AsString(this DataRowView drv, int ColumnIndex) => drv.Row.AsString(ColumnIndex);
        static public double AsDouble(this DataRowView drv, int ColumnIndex, double Default) => drv.Row.AsDouble(ColumnIndex, Default);
        static public double AsDouble(this DataRowView drv, int ColumnIndex) => drv.Row.AsDouble(ColumnIndex);
        static public decimal AsDecimal(this DataRowView drv, int ColumnIndex, decimal Default) => drv.Row.AsDecimal(ColumnIndex, Default);
        static public decimal AsDecimal(this DataRowView drv, int ColumnIndex) => drv.Row.AsDecimal(ColumnIndex);
        static public bool AsBoolean(this DataRowView drv, int ColumnIndex, bool Default) => drv.Row.AsBoolean(ColumnIndex, Default);
        static public bool AsBoolean(this DataRowView drv, int ColumnIndex) => drv.Row.AsBoolean(ColumnIndex);
        static public DateTime AsDateTime(this DataRowView drv, int ColumnIndex, DateTime Default) => drv.Row.AsDateTime(ColumnIndex, Default);
        static public DateTime AsDateTime(this DataRowView drv, int ColumnIndex) => drv.Row.AsDateTime(ColumnIndex);

        /* copy-append */
        static public void CopyTo(this DataRowView Source, DataRowView Dest)
        {
            Dest.BeginEdit();
            Source.Row.CopyTo(Dest.Row);
            Dest.EndEdit();
        }

        static public void SafeCopyTo(this DataRowView Source, DataRowView Dest)
        {
            Dest.BeginEdit();
            Source.Row.SafeCopyTo(Dest.Row);
            Dest.EndEdit();
        }

        /* blobs */
        static public void StreamToBlob(this DataRowView drv, string FieldName, Stream Stream)
        {
            drv.BeginEdit();
            drv.Row.StreamToBlob(FieldName, Stream);
            drv.EndEdit();
        }

        static public void BlobToStream(this DataRowView drv, string FieldName, Stream Stream) => drv.Row.BlobToStream(FieldName, Stream);
        static public MemoryStream BlobToStream(this DataRowView drv, string FieldName) => drv.Row.BlobToStream(FieldName);
        
        static public void LoadFromStream(this DataRowView drv, string BlobFieldName, Stream Stream)
        {
            drv.BeginEdit();
            drv.Row.LoadFromStream(BlobFieldName, Stream);
            drv.EndEdit();
        }

        static public void SaveToStream(this DataRowView drv, string BlobFieldName, Stream Stream) => drv.Row.SaveToStream(BlobFieldName, Stream);

        static public void StringToBlob(this DataRowView drv, string BlobFieldName, string Value)
        {
            drv.BeginEdit();
            drv.Row.StringToBlob(BlobFieldName, Value);
            drv.EndEdit();
        }

        static public string BlobToString(this DataRowView drv, string BlobFieldName) => drv.Row.BlobToString(BlobFieldName);

        /* miscs */
        static public bool TryGetValue(this DataRowView drv, string FieldName, out object Value) => drv.Row.TryGetValue(FieldName, out Value);
    }
}