namespace Tripous.Data;

public class DbOpContext
{
    // ● construction
    public DbOpContext(string ModuleName, SqlStore Store, DbTransaction Transaction, MemTable TopTable, bool CascadeDeletes = true, bool GenerateSql = false)
    {
        this.ModuleName = ModuleName;
        this.Store = Store;
        this.Transaction = Transaction;
        this.TopTable = TopTable;

        // flat table list
        this.FlatList = TopTable.GetTreeAsFlatList();
        
        // generate sql
        if (GenerateSql)
        {
            foreach (MemTable Table in this.FlatList)
                SqlStatementBuilder.BuildSql(ModuleName, Table, Store, Table == TopTable);
        }
        
        // max detail level
        MaxDetailLevel = 0;
        foreach (MemTable Table in this.FlatList)
            MaxDetailLevel = Math.Max(MaxDetailLevel, Table.Level);
 
        // cascade deletes
        this.CascadeDeletes = CascadeDeletes;
    }
    
    // ● properties
    /// <summary>
    /// <para><b>WARNING:</b> The <see cref="ModuleName"/> and a TableName are used in constructing a unique StatementName.</para>
    /// <para>The StatementName is used with the <see cref="SqlStore.GetNativeSchemaFromTableName"/>
    /// so the <c>ModuleName.TableName</c> must construct a unique name because schema DataTables are stored in the <see cref="SqlCache"/> under that unique name. </para>
    /// </summary>
    public string ModuleName { get; }
    public SqlStore Store { get;  }
    public DbTransaction Transaction { get; }
    public MemTable TopTable { get;  }
    public List<MemTable> FlatList { get; } 

    public int MaxDetailLevel { get;  }
    public bool CascadeDeletes { get;  }
    
    /// <summary>
    /// Returns true if Oids are needed before commiting a row to the database
    /// </summary>
    public bool OidIsBefore => Store.Provider.OidMode == OidMode.Generator;
    /// <summary>
    /// Returns true if Oids are needed after commiting a row to the database
    /// </summary>
    public bool OidIsAfter => !OidIsBefore;
}


/// <summary>
/// A helper class that post changes, i.e. INSERT-UPDATE-DELETE, of a single <see cref="MemTable"/> table or a table tree to the database.
/// <para>All tables should have the <c>InsertRowSql</c> and the <c>UpdateRowSql</c> of their <see cref="MemTable.Sqls"/> defined. </para>
/// <para>WARNING: Only DataRows that have the <c>Added</c>, <c>Modified</c> or <c>Deleted</c> <see cref="DataRowState"/> defined are processed. </para>
/// </summary>
static public class DbOps
{
    // ● public
    /// <summary>
    /// Posts any changes (deletes, updates, inserts) to the database
    /// </summary>
    static public void PostChanges(DbOpContext Context)
    {
        PostDeletes(Context);
        PostUpdates(Context);
        PostInserts(Context);
    }
 
    /// <summary>
    /// Processes the DELETE part of a commit
    /// <para>WARNING: DataRows must have the <c>Deleted</c> <see cref="DataRowState"/> defined. </para>
    /// </summary>
    static public void PostDeletes(DbOpContext Context)
    {
        //  a nested method in order to avoid a separate method, since this code is called twice, below  
        // ------------------------------------------------------------------
        void DeleteTable (MemTable Table)
        {
            DataTable Source = Table.GetDeletedRows();

            if (Source != null)
            {
                string SqlText;

                // delete the rows 
                List<string> KeyValuesList = Source.GetKeyValuesList(Table.KeyField, 100);

                foreach (string KeyValues in KeyValuesList)
                {
                    //SqlText = string.Format("delete from {0} where {1} in {2}", Table.TableName, Table.KeyField, KeyValues);
                    SqlText = $"delete from {Table.TableName} where {Table.KeyField} in {KeyValues}";
                    Context.Store.ExecSql(Context.Transaction, SqlText);
                }
            }
        };
        // ------------------------------------------------------------------

        int Level;

        // deletes in reverse order
        if (Context.CascadeDeletes)
        {
            Level = Context.MaxDetailLevel;
            while (Level >= Context.TopTable.Level)
            {
                foreach (MemTable Table in Context.FlatList)
                {
                    if (Table.Level == Level)
                    {
                        DeleteTable(Table);
                    }
                }

                Level--;
            }
        }
        else // deletes in normal order: let any constraint throw an exception
        {
            Level = Context.TopTable.Level;
            while (Level <= Context.MaxDetailLevel)
            {
                foreach (MemTable Table in Context.FlatList)
                {
                    if (Table.Level == Level)
                    {
                        DeleteTable(Table);
                    }
                }

                Level++;
            }
        }
    }
    /// <summary>
    /// Processes the UPDATE part of a commit
    /// <para>WARNING: DataRows must have the <c>Modified</c> <see cref="DataRowState"/> defined. </para>
    /// </summary>
    static public void PostUpdates(DbOpContext Context)
    {
        int Level = Context.TopTable.Level;

        // updates with normal order
        while (Level <= Context.MaxDetailLevel)
        {
            foreach (MemTable Table in Context.FlatList)
            {
                if ((Table.Level == Level) && !Table.IsEmpty && Table.Columns.Contains(Table.KeyField))
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Row.RowState == DataRowState.Modified)
                        {
                            if (!Row.IsNull(Table.KeyField))
                            {
                                Context.Store.ExecSql(Context.Transaction, Table.Sqls.UpdateRowSql, Row);
                            }
                        }
                    }
                }
            }

            Level++;
        }

    }
    /// <summary>
    /// Processes the INSERT part of a commit
    /// <para>WARNING: DataRows must have the <c>Added</c> <see cref="DataRowState"/> defined. </para>
    /// </summary>
    static public void PostInserts(DbOpContext Context)
    {
        //----------------------------------------------------------------------------------
        // Returns the next id value of a generator named after the TableName table.
        // It should be used only with databases that support generators or when a CustomOid object is used.
        int NextId(string TableName)
        {
            if (Context.Transaction != null)
                return Context.Store.NextId(Context.Transaction, TableName);
            return Context.Store.NextId(TableName);
        }
        // Returns the last id produced by an INSERT statement.
        // It should be used only with databases that support identity (auto-increment) columns
        int LastId(string TableName)
        {
            if (Context.Transaction != null)
                return Context.Store.LastId(Context.Transaction, TableName);
            return Context.Store.LastId(TableName);
        }
        //----------------------------------------------------------------------------------
        
        bool IsString;

        object Value;
        int OldId;
        int NewId;

        // inserts with normal order
        int Level = Context.TopTable.Level;
        while (Level <= Context.MaxDetailLevel)
        {
            foreach (MemTable Table in Context.FlatList)
            {
                if (Table.Level == Level && !Table.IsEmpty && Table.Columns.Contains(Table.KeyField))
                {
                    IsString = Table.IsStringField(Table.KeyField);

                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Row.RowState == DataRowState.Added)
                        {
                            Value = !Row.IsNull(Table.KeyField) ? Row[Table.KeyField] : Sys.GenId();

                            // primary key is a Guid
                            if (IsString)
                            {
                                Context.Store.ExecSql(Context.Transaction, Table.Sqls.InsertRowSql, Row);
                            }
                            else // primary key is an integer, autoincremented or provided by a generator/sequencer
                            {
                                OldId = Sys.AsInteger(Value, -1);
                                NewId = OldId;

                                // generator/sequencer, so get the "correct" Id
                                if (Context.OidIsBefore)
                                {
                                    NewId = NextId(Table.TableName);
                                    Row[Table.KeyFields[0]] = NewId;
                                }

                                try
                                {
                                    Context.Store.ExecSql(Context.Transaction, Table.Sqls.InsertRowSql, Row);
                                }
                                catch
                                {
                                    Row[Table.KeyFields[0]] = OldId;
                                    throw;
                                }

                                if (Context.OidIsAfter)
                                {
                                    NewId = LastId(Table.TableName);
                                    Row[Table.KeyFields[0]] = NewId;
                                }

                                // update Table detail tables with the "correct" master Id.
                                foreach (MemTable DetailTable in Table.Details)
                                {
                                    foreach (DataRow DetailRow in DetailTable.Rows)
                                    {
                                        if (!DetailRow.IsNull(DetailTable.DetailField) && (Sys.AsInteger(DetailRow[DetailTable.DetailFields[0]], -1) == OldId))
                                            DetailRow[DetailTable.DetailFields[0]] = NewId;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Level++;
        }
    }
}