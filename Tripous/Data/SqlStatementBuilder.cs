namespace Tripous.Data;

/// <summary>
/// Generates Sql statements for a <see cref="DataTable"/>.
/// </summary>
public static class SqlStatementBuilder
{
    // ● public
    /// <summary>
    /// Generates Sql statements for the Table.
    /// </summary>
    static public void BuildSql(string TableName, string PrimaryKeyField, SqlStore Store, TableSqls SqlStatements, bool IsTopTable)
    {
        string LB = Environment.NewLine;
        BuildSqlFlags Flags = BuildSqlFlags.None;

        if (Store.Provider.OidMode == OidMode.Generator)
            Flags |= BuildSqlFlags.OidModeIsBefore;

        DataTable SchemaTable = new DataTable("SchemaTable");
        Store.GetNativeSchema("", TableName, TableName, SchemaTable);

        DataColumn PkColumn = SchemaTable.FindColumn(PrimaryKeyField);
        if (PkColumn == null)
            Sys.Throw($"BuildSql(): Primary key field not found. Table: {TableName}, Field: {PrimaryKeyField}");

        string PkName = PkColumn.ColumnName;

        bool IsStringOid = Simple.SimpleTypeOf(PkColumn.DataType).IsString();
        bool OidModeIsBefore = !IsStringOid && ((Flags & BuildSqlFlags.OidModeIsBefore) == BuildSqlFlags.OidModeIsBefore);

        // delete (all rows)
        SqlStatements.DeleteSql = string.Format("delete from {0}", TableName);

        // delete row
        SqlStatements.DeleteRowSql = string.Format("delete from {0} where {1} = :{1}", TableName, PkName);

        // insert/update builders
        string S = "";
        string S2 = "";
        string S3 = "";
        string FieldName = "";

        for (int i = 0; i < SchemaTable.Columns.Count; i++)
        {
            FieldName = SchemaTable.Columns[i].ColumnName;

            if (!Sys.IsSameText(FieldName, PkName))
            {
                S += LB + "  " + FieldName + ", ";
                S2 += LB + "  :" + FieldName + ", ";
                S3 += LB + "  " + FieldName + " = :" + FieldName + ", ";
            }
            else if (IsStringOid || OidModeIsBefore)
            {
                S += LB + "  " + FieldName + ", ";
                S2 += LB + "  :" + FieldName + ", ";
            }
        }

        if (S.Length > 2) S = S.Remove(S.Length - 2, 2);
        if (S2.Length > 2) S2 = S2.Remove(S2.Length - 2, 2);
        if (S3.Length > 2) S3 = S3.Remove(S3.Length - 2, 2);

        // insert
        string SQL = "insert into {0} (" + LB + "{1}" + LB + ") values (" + LB + "{2}" + LB + ")";
        SqlStatements.InsertRowSql = string.Format(SQL, TableName, S, S2);

        // update
        if (!string.IsNullOrWhiteSpace(S3))
        {
            SQL = "update {0} " + LB + "set {1} " + LB + "where " + LB + "  {2} = :{2}";
            SqlStatements.UpdateRowSql = string.Format(SQL, TableName, S3, PkName);
        }
        else
        {
            SqlStatements.UpdateRowSql = string.Empty;
        }

        // select row
        if (IsTopTable && string.IsNullOrWhiteSpace(SqlStatements.SelectRowSql))
            SqlStatements.SelectRowSql = string.Format("select * from {0} where {1} = :{1}", TableName, PkName);

        // select all
        if (string.IsNullOrWhiteSpace(SqlStatements.SelectSql))
            SqlStatements.SelectSql = string.Format("select * from {0}", TableName);
    }
    
    /// <summary>
    /// Generates Sql statements for the Table.     
    /// </summary>
    static public void BuildSql(MemTable Table, SqlStore Store, bool IsTopTable)
    {
        BuildSql(Table.TableName, Table.KeyFields[0], Store, Table.SqlStatements, IsTopTable);
    }
    /// <summary>
    /// Generates Sql statements for the Table.     
    /// <para>WARNING: Assumes that Table primary key field is named Id.</para>
    /// </summary>
    static public TableSqls BuildSql(DataTable Table, SqlStore Store, bool IsTopTable)
    {
        TableSqls Result = new TableSqls();
        BuildSql(Table.TableName, "Id", Store, Result, IsTopTable);
        return Result;
    }
}