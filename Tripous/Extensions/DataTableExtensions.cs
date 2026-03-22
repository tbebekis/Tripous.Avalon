using System.Collections.ObjectModel;
using System.Dynamic;

namespace Tripous
{
    using System.Data;

    /// <summary>
    /// Extensions
    /// </summary>
    static public class DataTableExtensions
    {

        /// <summary>
        /// Returns the REAL number of rows in Table, not counting DataRowState.Deleted rows
        /// </summary>
        static public int GetRowCount(this DataTable Table)
        {
            int Result = 0;

            foreach (DataRow Row in Table.Rows)
            {
                if (Row.RowState != DataRowState.Deleted)
                    Result++;
            }

            return Result;
        }

        /// <summary>
        /// Returns the index of the DataColumn with FieldName in the Table, if any, else -1.
        /// </summary>
        static public int IndexOfColumn(this DataTable Table, string FieldName)
        {
            if ((Table != null) && !string.IsNullOrWhiteSpace(FieldName))
            {
                for (int i = 0; i < Table.Columns.Count; i++)
                    if (FieldName.IsSameText(Table.Columns[i].ColumnName))
                        return i;
            }
            return -1;
        }
        /// <summary>
        /// Returns true if Table contains a DataColumn with FieldName
        /// </summary>
        static public bool ContainsColumn(this DataTable Table, string FieldName)
        {
            return IndexOfColumn(Table, FieldName) > -1;
        }
        /// <summary>
        /// Returns the DataColumn with FileName, if exists, else null.
        /// </summary>
        static public DataColumn FindColumn(this DataTable Table, string FieldName)
        {
            if ((Table != null) && !string.IsNullOrWhiteSpace(FieldName))
            {
                for (int i = 0; i < Table.Columns.Count; i++)
                    if (FieldName.IsSameText(Table.Columns[i].ColumnName))
                        return Table.Columns[i];
            }

            return null;
        }


        /// <summary>
        /// A DataRow is marked as Deleted only if the DataRow.Delete() is called.
        /// <para>The DataRowCollection.Clear(), DataRowCollection.RemoveAt(), DataTable.Clear() etc, 
        /// do NOT set the DataRowState.Deleted flag.</para>
        /// <para>This method deletes all rows from a DataTable and sets the Deleted flag.</para>
        /// <para>WARNING: Only DataRowState.Unchanged rows (which is set by a DataTable.AcceptChanges() etc)
        /// are marked as DataRowState.Deleted.</para>
        /// </summary>
        static public void DeleteRows(this DataTable Table)
        {
            if ((Table != null) && (Table.Rows.Count > 0))
            {
                DataRow[] Rows = new DataRow[Table.Rows.Count];
                Table.Rows.CopyTo(Rows, 0);

                foreach (DataRow Row in Rows)
                    Row.Delete();
            }
        }

        /// <summary>
        /// Copies Source rows to Dest. 
        /// <para>WARNING: Assumes that Source and Dest are identical in schema.</para>
        /// </summary>
        static public void CopyTo(this DataTable Source, DataTable Dest, bool EmptyDest)
        {
            if (EmptyDest)
                Dest.Clear();
            DataRow DestRow = null;

            for (int i = 0; i < Source.Rows.Count; i++)
            {
                DestRow = Dest.NewRow();
                DataRowExtensions.CopyTo(Source.Rows[i], DestRow);
                Dest.Rows.Add(DestRow);
            }
        }
        /// <summary>
        /// Copies Source rows to Dest.
        /// <para>WARNING: Only data from columns with identical names to both tables are copied.</para>
        /// </summary>
        static public void SafeCopyTo(this DataTable Source, DataTable Dest, bool EmptyDest)
        {
            if (EmptyDest)
                Dest.Clear();
            DataRow DestRow = null;

            for (int i = 0; i < Source.Rows.Count; i++)
            {
                DestRow = Dest.NewRow();
                DataRowExtensions.SafeCopyTo(Source.Rows[i], DestRow);
                Dest.Rows.Add(DestRow);
            }
        }

        /// <summary>
        /// Copies Source rows to Dest. 
        /// <para>WARNING: Dest is emptied first.</para>
        /// <para>WARNING: Assumes that Source and Dest are identical in schema.</para>
        /// </summary>
        static public void CopyTo(this DataTable Source, DataTable Dest)
        {
            CopyTo(Source, Dest, true);
        }
        /// <summary>
        /// Copies Source rows to Dest.
        /// <para>WARNING: Dest is emptied first.</para>
        /// <para>WARNING: Only data from columns with identical names to both tables are copied.</para>
        /// </summary>
        static public void SafeCopyTo(this DataTable Source, DataTable Dest)
        {
            SafeCopyTo(Source, Dest, true);
        }

        /// <summary>
        /// Appends Source rows to Dest. 
        /// <para>WARNING: Assumes that Source and Dest are identical in schema.</para>
        /// </summary>
        static public void AppendTo(this DataTable Source, DataTable Dest)
        {
            CopyTo(Source, Dest, false);
        }
        /// <summary>
        /// Appends Source rows to Dest.
        /// <para>WARNING: Only data from columns with identical names to both tables are copied.</para>
        /// </summary>
        static public void SafeAppendTo(this DataTable Source, DataTable Dest)
        {
            SafeCopyTo(Source, Dest, false);
        }

        /// <summary>
        /// Returns a new empty DataTable with a schema identical to Source.
        /// <para>WARNING: It preserves the class type of the Source.
        /// That is the result table is of the same class type as the Source.</para>
        /// </summary>
        static public DataTable CopyStructure(this DataTable Source)
        {
            return Source.Clone();
        }
        /// <summary>
        /// Returns a new empty DataTable with a schema identical to Source.
        /// <para>If PreserveClassType is true then the result table is of the same class type as the Source.</para>
        /// <para>Else the result table is of the System.Data.DataTable type.</para>
        /// </summary>
        static public DataTable CopyStructure(this DataTable Source, bool PreserveClassType)
        {

            if (Source != null)
            {
                if (PreserveClassType)
                    return Source.Clone();
                DataTable Result = new DataTable();
                CopyStructureTo(Source, Result);
                return Result;

            }

            return new DataTable();
        }
        /// <summary>
        /// Copies the Source schema to Dest.
        /// <para>WARNING: Dest must be empty and no DataColumns defined.</para>
        /// </summary>
        static public void CopyStructureTo(this DataTable Source, DataTable Dest)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                string SourceTableName = Source.TableName;
                if (string.IsNullOrWhiteSpace(Source.TableName))
                    Source.TableName = ID.GenId();

                Source.WriteXmlSchema(MS);
                MS.Position = 0;

                string TableName = Dest.TableName;
                Dest.TableName = Source.TableName;

                Dest.ReadXmlSchema(MS);
                Dest.TableName = TableName;
                Source.TableName = SourceTableName;
            }
        }
        /// <summary>
        /// Copies column schema from Source to Dest. Only DataColumns that do not exist
        /// in Dest are copied.
        /// </summary>
        static public void MergeStructure(this DataTable Source, DataTable Dest)
        {
            for (int i = 0; i < Source.Columns.Count; i++)
                if (Dest.Columns.Contains(Source.Columns[i].ColumnName) == false)
                    DataColumnExtensions.CopyStructureTo(Source.Columns[i], Dest);
        }

        /// <summary>
        /// Copies tables from Source to Dest.
        /// <para>For each table copies Source data rows to Dest data rows, preserving the RowState.</para>
        /// <para>NOTE: It first clears any rows found in Dest tables.</para>
        /// <para>If CopySchemaToo is true, it deletes Columns from Dest tables and
        /// creates new Columns based on Source tables</para>
        /// </summary>
        static public void CopyExactState(this DataSet Source, DataSet Dest, bool CopySchemaToo)
        {
            if ((Source == null) || (Dest == null))
                return;

            Dest.DataSetName = Source.DataSetName;

            if (Dest.Tables.Count == 0)
            {
                foreach (DataTable SourceTable in Source.Tables)
                {
                    if (!Dest.Tables.Contains(SourceTable.TableName))
                        Dest.Tables.Add(SourceTable.TableName);
                }
            }

            for (int i = 0; i < Source.Tables.Count; i++)
            {
                CopyExactState(Source.Tables[i], Dest.Tables[i], CopySchemaToo);
            }

        }
        /// <summary>
        /// Copies tables from Source to Result.
        /// <para>For each table copies Source data rows to Result data rows, preserving the RowState.</para>
        /// </summary>
        static public DataSet CopyExactState(this DataSet Source)
        {
            DataSet Dest = new DataSet();
            CopyExactState(Source, Dest, true);
            return Dest;
        }

        /// <summary>
        /// Copies Source data rows to Dest data rows, preserving the RowState.
        /// <para>NOTE: It first clears any rows found in Dest.</para>
        /// <para>If CopySchemaToo is true, it deletes Columns from Dest and
        /// creates new Columns based on Source</para>
        /// </summary>
        static public void CopyExactState(this DataTable Source, DataTable Dest, bool CopySchemaToo)
        {
            if ((Source == null) || (Dest == null))
                return;

            if (string.IsNullOrWhiteSpace(Dest.TableName) && !string.IsNullOrWhiteSpace(Source.TableName))
            {
                Dest.TableName = Source.TableName;
            }

            if (Dest.Rows.Count > 0)
            {
                Dest.Clear();
                Dest.AcceptChanges();
            }

            if (CopySchemaToo)
            {
                Dest.Columns.Clear();
                Source.CopyStructureTo(Dest);
            }

            foreach (DataRow SourceRow in Source.Rows)
                Dest.ImportRow(SourceRow);

            DataTable Temp = Source.GetChanges(DataRowState.Deleted);

            if (Temp != null)
            {
                foreach (DataRow Row in Temp.Rows)
                    Dest.ImportRow(Row);
            }
        }
        /// <summary>
        /// Copies Source data rows to Result data rows, preserving the RowState.
        /// </summary>
        static public DataTable CopyExactState(this DataTable Source)
        {
            DataTable Dest = new DataTable();
            CopyExactState(Source, Dest, true);
            return Dest;
        }

        /// <summary>
        /// Returns a DataTable that contains copies of the DataRow objects, given an input IEnumerable of DataRow.
        /// <para>.NetStandard 2.1 contains the extension method CopyToDataTable() with the same functionality.</para>
        /// </summary>
        static public DataTable ToTable(this IEnumerable<DataRow> Rows)
        {
            if (Rows != null && Rows.Count() > 0)
            {
                DataTable Table = null;
                DataRow DestRow;
                foreach (DataRow Row in Rows)
                {
                    if (Table == null)
                    {
                        Table = Row.Table.CopyStructure();
                    }

                    DestRow = Table.NewRow();
                    Row.CopyTo(DestRow);
                    Table.Rows.Add(DestRow);
                }
            }

            return new DataTable();
        }

        /// <summary>
        /// Returns a DataTable with the deleted rows in the Source.
        /// <para>A DataRow is marked with the <see cref="DataViewRowState.Deleted"/> flag when it is deleted.
        /// After that is not possible to access the row data without an exception.</para>
        /// <para>By getting deleted rows of a table to another table, eliminates this problem.</para>
        /// </summary>
        static public DataTable GetDeletedRows(this DataTable Source)
        {
            DataView DataView = new DataView(Source, null, null, DataViewRowState.Deleted);
            return DataView.ToTable();
        }

        /// <summary>
        /// Splits a specified table's rows into chunks. Each chunk may have a specified row count.
        /// </summary>
        static public DataRow[][] SplitToChunks(this DataTable Table, int ChunkRowCount)
        {
            // copy rows to an array
            DataRow[] TableRows = new DataRow[Table.Rows.Count];
            Table.Rows.CopyTo(TableRows, 0);

            // split rows into manageable chunks
            int i = 0;

            DataRow[][] Chunks = TableRows.GroupBy(s => i++ / ChunkRowCount).Select(g => g.ToArray()).ToArray();
            return Chunks;
        }


        /// <summary>
        /// Creates a new row, adds the row to rows, and returns the row.
        /// </summary>
        static public DataRow AddNewRow(this DataTable Table)
        {
            DataRow Result = Table.NewRow();
            Table.Rows.Add(Result);
            return Result;
        }

        /// <summary>
        /// Sets Table column captions. Dictionary is a ColumnName=Caption list of pairs. If HideUntitle is true, 
        /// then any column not found in Dictionary is set to Visible = false in its ExtendedProperties.
        /// </summary>
        static public void SetColumnCaptionsFrom(this DataTable Table, IDictionary<string, string> Dictionary, bool HideUntitled)
        {
            if ((Dictionary == null) || (Dictionary.Count == 0))
                return;

            DataColumn Column;
            for (int i = 0; i < Table.Columns.Count; i++)
            {
                Column = Table.Columns[i];
                if (Column.ColumnName.IsSameText(Column.Caption))
                {
                    if (Dictionary.ContainsKey(Column.ColumnName))
                    {
                        Column.Caption = Dictionary[Column.ColumnName];
                        Column.IsVisible(true);
                    }
                    else
                    {
                        Column.IsVisible(HideUntitled ? false : true);
                    }

                }
            }
        }
        /// <summary>
        /// Sets the Visible "extended property" of all Table.Columns to Value.
        /// </summary>
        static public void SetColumnsVisible(this DataTable Table, bool Value)
        {
            foreach (DataColumn Column in Table.Columns)
                Column.IsVisible(Value);
        }


        /// <summary>
        /// Converts Value to int, if possible, else returns Default
        /// </summary>
        static private int AsInteger(object Value, int Default)
        {
            if (Value == null)
                return Default;

            try
            {
                return Convert.ToInt32(Value);
            }
            catch
            {
            }

            return Default;
        } 
        /// <summary>
        ///  Used in constructing SQL statements that contain a WHERE clause of the type
        ///  <para><c>  where FIELD_NAME in (...)</c></para>
        ///  This method limits the number of elements inside the in (...) according to the passed in ModValue, in order
        ///  to avoid problems with database servers that have such a limit.
        ///  <para>It returns a string array where each element contains no more than ModValue of the FieldName values from Table.</para>
        /// </summary>
        static public string[] GetKeyValuesList(this DataTable Table, string FieldName, int ModValue, bool DiscardBelowZeroes)
        {
            if ((Table != null) && !string.IsNullOrWhiteSpace(FieldName) && Table.Columns.Contains(FieldName))
            {
                List<object> List = new List<object>();
                foreach (DataRow R in Table.Rows)
                {
                    if (R.RowState != DataRowState.Deleted && !R.IsNull(FieldName))
                        List.Add(R[FieldName]);
                }

                return List.GetKeyValuesList(FieldName, ModValue, DiscardBelowZeroes);
            }

            return new string[0];

        }
        /// <summary>
        /// Returns true if FieldName is of type string.
        /// </summary>
        static public bool IsStringField(this DataTable Table, string FieldName)
        {
            return (Table.Columns.Contains(FieldName)) && (Table.Columns[FieldName].DataType == typeof(System.String));
        }

        /// <summary>
        /// Converts the rows of a DataTable to an observable collection.
        /// </summary>
        public static ObservableCollection<dynamic> ToObservableList(this DataTable table)
        {
            var list = new ObservableCollection<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                var expando = new ExpandoObject();
                var expandoDic = (IDictionary<string, object>)expando;

                foreach (DataColumn col in table.Columns)
                {
                    expandoDic[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                list.Add(expando);
            }
            return list;
        }


        /* grid column types */
        static void SetColumnsAsType(this DataTable Table, string ColumnNames, string TypeFlag)
        {
            if (Table != null)
            {
                string[] FieldNames = ColumnNames.Split(';');
                DataColumn Column;
                foreach (string FieldName in FieldNames)
                {
                    Column = Table.FindColumn(FieldName.Trim());
                    if (Column != null)
                    {
                        Column.ExtendedProperties[TypeFlag] = true;
                    }
                }
            }
        }
        /// <summary>
        /// Instructs grids to display grid columns of the designated type
        /// for the field names specified by ColumnNames.
        /// <para>ColumnNames is a list of strings separated by the character ;</para>
        /// </summary>
        static public void SetAsDateTimeColumns(this DataTable Table, string ColumnNames)
        {
            SetColumnsAsType(Table, ColumnNames, "IsDateTime");
        }
        /// <summary>
        /// Instructs grids to display grid columns of the designated type
        /// for the field names specified by ColumnNames.
        /// <para>ColumnNames is a list of strings separated by the character ;</para>
        /// </summary>
        static public void SetAsDateColumns(this DataTable Table, string ColumnNames)
        {
            SetColumnsAsType(Table, ColumnNames, "IsDate");
        }
        /// <summary>
        /// Instructs grids to display grid columns of the designated type
        /// for the field names specified by ColumnNames.
        /// <para>ColumnNames is a list of strings separated by the character ;</para>
        /// </summary>
        static public void SetAsTimeColumns(this DataTable Table, string ColumnNames)
        {
            SetColumnsAsType(Table, ColumnNames, "IsTime");
        }
        /// <summary>
        /// Instructs grids to display grid columns of the designated type
        /// for the field names specified by ColumnNames.
        /// <para>ColumnNames is a list of strings separated by the character ;</para>
        /// </summary>
        static public void SetAsCheckBoxColumns(this DataTable Table, string ColumnNames)
        {
            SetColumnsAsType(Table, ColumnNames, "IsCheckBox");
        }
        /// <summary>
        /// Instructs grids to display grid columns of the designated type
        /// for the field names specified by ColumnNames.
        /// <para>ColumnNames is a list of strings separated by the character ;</para>
        /// </summary>
        static public void SetAsMemoColumns(this DataTable Table, string ColumnNames)
        {
            SetColumnsAsType(Table, ColumnNames, "IsMemo");
        }
        /// <summary>
        /// Instructs grids to display grid columns of the designated type
        /// for the field names specified by ColumnNames.
        /// <para>ColumnNames is a list of strings separated by the character ;</para>
        /// </summary>
        static public void SetAsImageColumns(this DataTable Table, string ColumnNames)
        {
            SetColumnsAsType(Table, ColumnNames, "IsImage");
        }

    }
}
