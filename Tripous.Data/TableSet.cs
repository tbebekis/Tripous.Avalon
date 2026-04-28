namespace Tripous.Data;

public class TableSet
{
    // ● initialization
    /// <summary>
    /// Adds Table an all of its detail tables to TableTree.
    /// </summary>
    void AddTableToTree(MemTable Table)
    {
        if (TableTree.IndexOf(Table) == -1)
            TableTree.Add(Table);

        for (int i = 0; i < Table.Details.Count; i++)
            AddTableToTree(Table.Details[i]);
    }
    /// <summary>
    /// Constructs the table tree
    /// </summary>
    void ConstructTableTree()
    {
        AddTableToTree(TopTable);
    }
    /// <summary>
    /// Sets the MaxDetailLevel, that is the depth of the details.
    /// </summary>
    void SetMaxDetailLevel()
    {
        MaxDetailLevel = 0;

        foreach (MemTable Table in TableTree)
            MaxDetailLevel = Math.Max(MaxDetailLevel, Table.Level);

    }
    /// <summary>
    /// Stocks in TableSet is a list of DataTables used as look-ups etc. This method
    /// executes the SELECT Sql statement for each of those queries.
    /// </summary>
    void SelectStocks()
    {
        if (Stocks != null)
        {
            MemTable Table;
            for (int i = 0; i < Stocks.Count; i++)
            {
                Table = Stocks[i];

                if (string.IsNullOrWhiteSpace(Table.Sqls.SelectSql))
                    Table.Sqls.SelectSql = "select * from " + Table.TableName; 

                Store.SelectTo(Table, Table.Sqls.SelectSql);

                if (Table.Sqls.HasDisplayLabels)
                {
                    Table.SetColumnCaptionsFrom(Table.Sqls.DisplayLabels, true);
                }
                else
                {
                    for (int j = 0; j < Table.Columns.Count; j++)
                    {
                        Table.Columns[j].IsVisible(!Sys.IsSameText("ID", Table.Columns[j].ColumnName));
                    }
                }
            }
        }


    }
    /// <summary>
    /// Generates text statements
    /// </summary>
    void GenerateSqlStatements()
    {
        if (GenerateSql)
        {
            for (int i = 0; i < TableTree.Count; i++)
                SqlStatementBuilder.BuildSql(TableTree[i], Store, TableTree[i] == TopTable);
        }
    }    

    // ●  edit operation  
    /// <summary>
    /// Cancels any pending edit operation in the whole table tree.
    /// </summary>
    void InternalCancel()
    {
        int Level = MaxDetailLevel;

        // in reverse order
        while (Level >= TopTable.Level)
        {
            foreach (MemTable Table in TableTree)
            {
                if (Table.Level == Level)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Row.HasVersion(DataRowVersion.Proposed))
                        {
                            Row.EndEdit();
                        }
                    }
                }
            }

            Level--;
        }
    }

    // ● database SELECT tree  
    /// <summary>
    /// Executes the SELECT SqlText and appends the resulted rows to the DetailTable MemTable.
    /// </summary>
    void Select_DoAddToDetail(string SqlText, MemTable Detail)
    {
        DataTable Source = Store.Select(SqlText);

        if (Detail.Columns.Count == 0)
            Source.CopyStructureTo(Detail);

        Detail.BeginLoadData();
        try
        {
            string FieldName = Detail.KeyField;
            object Value;
            for (int i = 0; i < Source.Rows.Count; i++)
            {
                Value = Source.Rows[i][FieldName];
                if (Detail.Locate(FieldName, Value, LocateOptions.None) == null)
                    Source.Rows[i].AppendTo(Detail);
            }
        }
        finally
        {
            Detail.EndLoadData();
        }

    }
    /// <summary>
    /// Executes the SELECT of the DetailTable.  
    /// </summary>
    void Select_DoDetail(MemTable MasterTable, MemTable Detail)
    {
        string SqlText;

        if (!string.IsNullOrWhiteSpace(Detail.Sqls.SelectSql))
        {
            // 1. SqlText execution ===================================================
            if ((MasterTable.Rows.Count > 0) && (MasterTable.Columns.Contains(Detail.MasterField)))
            {
                SelectSql SS = new SelectSql(Detail.Sqls.SelectSql);
                SS.Where = "";

                //  limit the number of elements inside the in (...),  in order
                //    to avoid problems with database servers that have such a limit.   
                List<string> KeyValuesList = MasterTable.GetKeyValuesList(Detail.MasterField, 100);

                StringBuilder SB = new StringBuilder();
                for (int i = 0; i < KeyValuesList.Count; i++)
                {
                    SB.Clear();
                    SB.AppendLine(SS.Text);
                    SB.AppendLine($"where ");
                    SB.AppendLine($"{Detail.TableName}.{Detail.DetailField} in {KeyValuesList[i]}");

                    SqlText = SB.ToString();

                    Select_DoAddToDetail(SqlText, Detail);
                }
            }

            Detail.SetColumnCaptionsFrom(Detail.Sqls.DisplayLabels, HideUntitleDisplayLabels);

            if (!Detail.IsEmpty)
                Select_DoDetails(Detail);
        }
    }
    /// <summary>
    /// Executes the SELECT of Details of the MasterTable.
    /// </summary>
    void Select_DoDetails(MemTable MasterTable)
    {
        foreach (MemTable DetailTable in MasterTable.Details)
        {
            Select_DoDetail(MasterTable, DetailTable);
            DetailTable.AcceptChanges();
        }
    }
 
    // ●  Oids and Guid  
    /// <summary>
    /// Returns the next id value of a generator named after the TableName table.
    /// <para>It should be used only with databases that support generators or when a CustomOid object is used.</para>
    /// </summary>
    int NextId(string TableName)
    {
        if (Transaction != null)
            return Store.NextId(Transaction, TableName);
        return Store.NextId(TableName);
    }
    /// <summary>
    /// Returns the last id produced by an INSERT statement.
    /// <para>It should be used only with databases that support identity (auto-increment) columns</para>
    /// </summary>
    int LastId(string TableName)
    {
        if (Transaction != null)
            return Store.LastId(Transaction, TableName);
        return Store.LastId(TableName);
    }
    /// <summary>
    /// Creates and returns a new Guid NOT surrounded by {}
    /// </summary>
    string GetGuid()
    {
        return Sys.GenId();
    }

    /// <summary>
    /// Returns true if Oids are needed before commiting a row to the database
    /// </summary>
    bool OidIsBefore { get { return Store.Provider.OidMode == OidMode.Generator; } }
    /// <summary>
    /// Returns true if Oids are needed after commiting a row to the database
    /// </summary>
    bool OidIsAfter { get { return !OidIsBefore; } }

    // ●  event triggers  
    /// <summary>
    /// Triggers the TransactionDelete event.
    /// </summary>
    void OnTransactionStageDelete(TransactionStage Stage, ExecTime ExecTime, object RowId)
    {
        if (TransactionStageDelete != null)
            TransactionStageDelete(this, new TransactionStageEventArgs(Store, Transaction, Stage, ExecTime, RowId));
    }
    /// <summary>
    /// Triggers the TransactionCommit event.
    /// </summary>
    void OnTransactionStageCommit(TransactionStage Stage, ExecTime ExecTime)
    {
        if (TransactionStageCommit != null)
            TransactionStageCommit(this, new TransactionStageEventArgs(Store, Transaction, Stage, ExecTime, -1));
    }

    // ●  miscs  
    /// <summary>
    /// Removes all data rows from Table and its details
    /// </summary>
    void Empty(MemTable Table)
    {
        for (int i = 0; i < Table.Details.Count; i++)
            Empty(Table.Details[i]);

        Table.Rows.Clear();
        Table.AcceptChanges();
    }
    /// <summary>
    /// Puts Variable values into the S by replacing value placeholders.
    /// <para>The default prefix for a Variable inside CommandText text is :@</para>
    /// </summary>
    void ResolveSql(ref string SqlText)
    {
        SqlValueProviders.Process(ref SqlText, Store);
    }
    
    // ● database commit the whole tree: INSERT-UPDATE-DELETE  
    /// <summary>
    /// Processes the DELETE part of a commit
    /// </summary>
    private void PostDeletes()
    {
        //  a nested method in order to avoid a separate method, since this code is called twice, below  
        // ------------------------------------------------------------------
        void DeleteTable (MemTable Table)
        {

            DataTable Source = Table.GetDeletedRows();

            if (Source != null)
            {
                string SqlText = "";

                // delete the rows 
                List<string> KeyValuesList = Source.GetKeyValuesList(Table.KeyField, 100);

                foreach (string KeyValues in KeyValuesList)
                {
                    SqlText = string.Format("delete from {0} where {1} in {2}", Table.TableName, Table.KeyField, KeyValues);
                    Store.ExecSql(Transaction, SqlText);
                }
            }
        };
        // ------------------------------------------------------------------

        int Level;

        // deletes in reverse order
        if (CascadeDeletes)
        {
            Level = MaxDetailLevel;
            while (Level >= TopTable.Level)
            {
                foreach (MemTable Table in TableTree)
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
            Level = TopTable.Level;
            while (Level <= MaxDetailLevel)
            {
                foreach (MemTable Table in TableTree)
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
    /// </summary>
    private void PostUpdates()
    {
        int Level = TopTable.Level;

        // updates with normal order
        while (Level <= MaxDetailLevel)
        {
            foreach (MemTable Table in TableTree)
            {
                if ((Table.Level == Level) && !Table.IsEmpty && Table.Columns.Contains(Table.KeyField))
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Row.RowState == DataRowState.Modified)
                        {
                            if (!Row.IsNull(Table.KeyField))
                            {
                                Store.ExecSql(Transaction, Table.Sqls.UpdateRowSql, Row);
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
    /// </summary>
    private void PostInserts()
    {
        bool IsString;

        object Value;
        int OldId;
        int NewId;

        // inserts with normal order
        int Level = TopTable.Level;
        while (Level <= MaxDetailLevel)
        {
            foreach (MemTable Table in TableTree)
            {
                if (Table.Level == Level && !Table.IsEmpty && Table.Columns.Contains(Table.KeyField))
                {
                    IsString = Table.IsStringField(Table.KeyField);

                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Row.RowState == DataRowState.Added)
                        {
                            Value = !Row.IsNull(Table.KeyField) ? Row[Table.KeyField] : Sys.GenId();
                            //Value = Row[Table.KeyFields[0]];

                            // primary key is a Guid
                            if (IsString)
                            {
                                Store.ExecSql(Transaction, Table.Sqls.InsertRowSql, Row);
                            }
                            else // primary key is an integer, autoincremented or provided by a generator/sequencer
                            {
                                OldId = Sys.AsInteger(Value, -1);
                                NewId = OldId;

                                // generator/sequencer, so get the "correct" Id
                                if (OidIsBefore)
                                {
                                    NewId = NextId(Table.TableName);
                                    Row[Table.KeyFields[0]] = NewId;
                                }

                                try
                                {
                                    Store.ExecSql(Transaction, Table.Sqls.InsertRowSql, Row);
                                }
                                catch
                                {
                                    Row[Table.KeyFields[0]] = OldId;
                                    throw;
                                }

                                if (OidIsAfter)
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
 
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public TableSet(SqlStore Store, MemTable TopTable, List<MemTable> Stocks, TableSetFlags Flags = TableSetFlags.GenerateSql)
    {
        if (TopTable == null)
            throw new ArgumentNullException("TopTable");

        TopTable.CheckTopTableErrors();

        this.Store = Store;
        this.TopTable = TopTable;
        this.Stocks = Stocks;

        GenerateSql = TableSetFlags.GenerateSql.In(Flags);   
        PessimisticMode = TableSetFlags.PessimisticMode.In(Flags);    
        CascadeDeletes = !TableSetFlags.NoCascadeDeletes.In(Flags);     

        ConstructTableTree();
        SetMaxDetailLevel();
        GenerateSqlStatements();
        SelectStocks();
    }
    
    // ● public
    // ● database operations  
    /// <summary>
    /// Selects the whole table tree from the database starting from the top table (which is a single-row table).
    /// <para>RowId could be string or integer and is the primary key value of the top table.</para>
    /// </summary>
    public bool Load(object RowId)
    {
        if (RowId == null || RowId == DBNull.Value || (RowId is string && string.IsNullOrWhiteSpace(RowId.ToString())))
            return false;
        
        ProcessEmpty();

        TopTable.EventsDisabled = true;
        try
        {
            if (!string.IsNullOrWhiteSpace(TopTable.Sqls.SelectRowSql))
            {
                Store.SelectTo(TopTable, TopTable.Sqls.SelectRowSql, RowId);
            }

            // select stock tables
            if (RowId != null && TopTable.Rows.Count >= 1)
            {
                MemTable StockTable;
                for (int i = 0; i < TopTable.Stocks.Count; i++)
                {
                    StockTable = TopTable.Stocks[i];
                    Store.SelectTo(StockTable, StockTable.Sqls.SelectRowSql, TopTable.Rows[0]);
                }
            }

            Select_DoDetails(TopTable); 
            TopTable.SetColumnCaptionsFrom(TopTable.Sqls.DisplayLabels, HideUntitleDisplayLabels);
        }
        finally
        {
            TopTable.EventsDisabled = false;
        }

        IsInsert = false;

        return TopTable.Rows.Count >= 1;
    }
    /// <summary>
    /// Deletes the whole table tree to the database. The way this method process table deletes, depends on the cascadeDeletes flag.
    /// <para>RowId could be string or integer and is the primary key value of the top table.</para>
    /// </summary>
    public void Delete(object RowId)
    {
        if (RowId == null)
            return;

        // first, select the top table and the detail tables
        Load(RowId);

        // already deleted in database
        if (TopTable.Rows.Count == 0)
            return;

         

        TopTable.EventsDisabled = true;
        //TopTable.Details.Active = false;
        try
        {
            // delete the top row, which deletes all detail rows too.
            TopTable.Rows[0].Delete();

            // then, inside a Transaction
            OnTransactionStageDelete(TransactionStage.Start, ExecTime.Before, RowId);

            using (Transaction = Store.BeginTransaction())
            {
                OnTransactionStageDelete(TransactionStage.Start, ExecTime.After, RowId);
                try
                {
                    PostDeletes();

                    OnTransactionStageDelete(TransactionStage.Commit, ExecTime.Before, RowId);
                    Transaction.Commit();
                    OnTransactionStageDelete(TransactionStage.Commit, ExecTime.After, RowId);
                }
                catch
                {
                    TopTable.DataSet.RejectChanges();
                    OnTransactionStageDelete(TransactionStage.Rollback, ExecTime.Before, RowId);
                    Transaction.Rollback();
                    OnTransactionStageDelete(TransactionStage.Rollback, ExecTime.After, RowId);
                    throw;
                }
            }


        }
        finally
        {
            //TopTable.Details.Active = true;
            TopTable.EventsDisabled = false;
            Transaction = null;
        }

 

    }
    /// <summary>
    /// Commits the whole table tree to the database. It can be either an insert or an update.
    /// </summary>
    public object Commit(bool Reselect)
    {
        if (TopTable.Rows.Count == 0)
            throw new ApplicationException("Nothing to commit. Top table is empty.");
        
        TopTable.EventsDisabled = true;
        //TopTable.Details.Active = false;
        try
        {
            // inside a single Transaction
            OnTransactionStageCommit(TransactionStage.Start, ExecTime.Before);
 
            using (Transaction = Store.BeginTransaction())
            {
                OnTransactionStageCommit(TransactionStage.Start, ExecTime.After);

                try
                {
                    PostChanges();

                    OnTransactionStageCommit(TransactionStage.Commit, ExecTime.Before);
                    Transaction.Commit();
                    TopTable.DataSet.AcceptChanges();   // clear logs    
                    OnTransactionStageCommit(TransactionStage.Commit, ExecTime.After);
                }
                catch
                {
                    OnTransactionStageCommit(TransactionStage.Rollback, ExecTime.Before);
                    Transaction.Rollback();
                    OnTransactionStageCommit(TransactionStage.Rollback, ExecTime.After);
                    throw;
                }
            }
        }
        finally
        {
            //TopTable.Details.Active = true;
            TopTable.EventsDisabled = false;
            Transaction = null;
        }

        LastCommitedId = null;

        if (TopTable.Rows.Count > 0)
            LastCommitedId = TopTable.Rows[0][TopTable.KeyFields[0]];

        if (Reselect && !Sys.IsNull(LastCommitedId))
            Load(LastCommitedId);

        IsInsert = false;
        return LastCommitedId;

    }
    /// <summary>
    /// A Commit() version for batch operations.
    /// <para>Starts a transaction and keeps on calling CommitProc() while it returns true.</para>
    /// <para>It commits the transaction each time the TransLimit is reached.</para>
    /// <para>Info is a user defined object.</para>
    /// </summary>
    public void CommitBatch2(BatchCommitArgs Args)
    {
        TopTable.EventsDisabled = true;
        try
        {
            int Counter = 0;
            int PostCounter = 0;
            bool ShouldPost;

            while (true)
            {
                Args.Counter = Counter;
                Args.PostCounter = PostCounter;

                ShouldPost = Args.BeforeFunc != null? Args.BeforeFunc(): false;

                if (ShouldPost)
                {
                    if (Transaction == null)
                    {
                        Transaction = Store.BeginTransaction();
                    }

                    PostChanges();

                    TopTable.DataSet.AcceptChanges();   // clear logs   

                    LastCommitedId = null;

                    if (TopTable.Rows.Count > 0)
                        LastCommitedId = TopTable.Rows[0][TopTable.KeyFields[0]];
                }

                if (Args.AfterFunc != null && !Args.AfterFunc(LastCommitedId))
                    break;

                if (ShouldPost)
                {
                    PostCounter++;

                    if (PostCounter % Args.TransLimit == 0 && Transaction != null)
                    {
                        Transaction.Commit();
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }

                Counter++;
            }

            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
        }
        catch  
        {
            if (Transaction != null)
                Transaction.Rollback();
            throw;
        }
        finally
        {
            TopTable.EventsDisabled = false;
        }
    }
    /// <summary>
    /// A Commit() version for batch operations.
    /// Starts a transaction and keeps on calling BeforeFunc() while AfterFunc() returns true.
    /// Commits the transaction each time the TransLimit is reached.
    /// Info is a user defined object.
    /// </summary>
    public void CommitBatch(BatchCommitArgs Args)
    {
        // ---------------------------------------
        void CommitBatchTransaction()
        {
            if (Transaction == null)
                return;

            try
            {
                Transaction.Commit();
                TopTable.DataSet.AcceptChanges();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
            }
        }
        // ---------------------------------------
        void RollbackBatchTransaction()
        {
            if (Transaction == null)
                return;

            try
            {
                Transaction.Rollback();
                TopTable.DataSet.RejectChanges();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
            }
        }        
        // ---------------------------------------
        
        if (Args == null)
            throw new ArgumentNullException("Args");
        if (Args.BeforeFunc == null)
            throw new ApplicationException("Batch commit requires a BeforeFunc.");
        if (Args.AfterFunc == null)
            throw new ApplicationException("Batch commit requires an AfterFunc.");
        if (Args.TransLimit <= 0)
            throw new ApplicationException("Batch commit TransLimit must be greater than zero.");

        TopTable.EventsDisabled = true;
        try
        {
            int Counter = 0;
            int PostCounter = 0;
            bool ShouldPost;
            bool Continue = true;

            while (Continue)
            {
                Args.Counter = Counter;
                Args.PostCounter = PostCounter;
                ShouldPost = Args.BeforeFunc != null? Args.BeforeFunc(): false;

                if (ShouldPost)
                {
                    if (Transaction == null)
                        Transaction = Store.BeginTransaction();

                    PostChanges();

                    LastCommitedId = null;
                    if (TopTable.Rows.Count > 0)
                        LastCommitedId = TopTable.Rows[0][TopTable.KeyField];

                    PostCounter++;

                    if (PostCounter % Args.TransLimit == 0)
                        CommitBatchTransaction();
                }
                
                Continue = Args.AfterFunc != null && Args.AfterFunc(LastCommitedId);
                Counter++;
            }

            CommitBatchTransaction();
        }
        catch
        {
            RollbackBatchTransaction();
            throw;
        }
        finally
        {
            TopTable.EventsDisabled = false;
            Transaction = null;
        }
    }

    /// <summary>
    /// Posts any changes (deletes, updates, inserts) to the database
    /// </summary>
    public void PostChanges()
    {
        PostDeletes();
        PostUpdates();
        PostInserts();
    }
    /// <summary>
    /// Executes the SELECT SqlText and puts the returned data rows to the Table.
    /// <para>It is used when selecting for the List (browser) part of a data form.</para>
    /// <para>Normally the Table passed to this method is not part of the table tree of the TableSet.</para>
    /// </summary>
    public int ListSelect(MemTable Table, string SqlText)
    {
        int Result = 0;

        if (Table != null)
        {
            if (string.IsNullOrWhiteSpace(SqlText))
                SqlText = Table.Sqls.SelectSql;

            if (SqlText.Trim() != "")
            {
                Table.EventsDisabled = true;
                try
                {
                    Result = Store.SelectTo(Table, SqlText);
                    Table.SetColumnCaptionsFrom(Table.Sqls.DisplayLabels, HideUntitleDisplayLabels);
                }
                finally
                {
                    Table.EventsDisabled = false;
                }
            }
        }

        return Result;

    }
 
    // ● edit operation handling 
    /// <summary>
    /// Removes all data rows from all tables in the tableTree
    /// </summary>
    public void ProcessEmpty()
    {
        InternalCancel();
 
        TopTable.EventsDisabled = true;
        //TopTable.DetailsActive = false;
        try
        {
            Empty(TopTable);
        }
        finally
        {
            //TopTable.DetailsActive = true;
            TopTable.EventsDisabled = false;
        }
    }
    /// <summary>
    /// Prepares the TableSet for an insert operation (in the tables, NOT the database)
    /// </summary>
    public void ProcessInsert()
    {
        ProcessEmpty();

        TopTable.EventsDisabled = true;
        //TopTable.DetailsActive = false;
        try
        {
            if (TopTable.Rows.Count == 0)
            {
                DataRow Row = TopTable.NewRow();
                TopTable.Rows.Add(Row);
            }
        }
        finally
        {
            //TopTable.DetailsActive = true;
            TopTable.EventsDisabled = false;
        }

        IsInsert = true;
    }
    /// <summary>
    /// Cancels an edit operation and re-initializes the table tree.
    /// </summary>
    public void ProcessCancel()
    {
        if (IsInsert)
            ProcessInsert();
        else if (TopTable.Rows.Count > 0 && TopTable.Rows[0].RowState != DataRowState.Deleted)
            Load(TopTable.Rows[0][TopTable.KeyFields[0]]);
    }    
 
    // ● properties
    /// <summary>
    /// Returns the executor
    /// </summary>
    public SqlStore Store { get; private set; }
    /// <summary>
    /// Returns the current Transaction
    /// </summary>
    public DbTransaction Transaction { get; private set; }
    /// <summary>
    /// True when inserting
    /// </summary>
    public bool IsInsert { get; private set; }
    /// <summary>
    /// If true, then when SELECTing to a MemTable, hides any column not found in the table's SqlStatements.BrowseSelect.DisplayLabels  
    /// </summary>
    public bool HideUntitleDisplayLabels { get; set; }
    /// <summary>
    /// Returns the maximum detail level
    /// </summary>
    public int MaxDetailLevel { get; private set; }
    /// <summary>
    /// Returns the Id of the last commit
    /// </summary>
    public object LastCommitedId { get; private set; }
    
    /// <summary>
    /// Field
    /// </summary>
    public MemTable TopTable { get; private set; }
    /// <summary>
    /// Field
    /// </summary>
    public List<MemTable> TableTree { get; private set; } = new();
    /// <summary>
    /// Field
    /// </summary>
    public List<MemTable> Stocks { get; private set; } 

    // ● flags  
    /// <summary>
    /// Field
    /// </summary>
    public bool GenerateSql  { get; private set; } = false;
    /// <summary>
    /// Field
    /// </summary>
    public bool PessimisticMode  { get; private set; } = false;
    /// <summary>
    /// Field
    /// </summary>
    public bool CascadeDeletes { get; private set; } = false;

    // ● events
    /// <summary>
    /// Occurs when <see cref="Delete"/>(object RowId) method is called.
    /// </summary>
    public event EventHandler<TransactionStageEventArgs> TransactionStageDelete;
    /// <summary>
    /// Occurs when <see cref="Commit"/>() method is called.
    /// </summary>
    public event EventHandler<TransactionStageEventArgs> TransactionStageCommit;   
}