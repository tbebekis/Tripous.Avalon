namespace Tripous.Data;

public class TableSet
{
    MemTable ListTable;
    MemTable ItemTable;
    List<MemTable> TableTree;
    List<MemTable> Stocks;
 
    bool IsInsert;
    int MaxDetailLevel;
    
    bool GenerateSql = false;
    bool CascadeDeletes  = false;
    
    // ● initialization
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
                SqlStatementBuilder.BuildSql(TableTree[i], Store, TableTree[i] == ItemTable);
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
        while (Level >= ItemTable.Level)
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

            Detail.SetColumnCaptionsFrom(Detail.Sqls.DisplayLabels, HideUntitledDisplayLabels);

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

 

    /// <summary>
    /// Creates a context for calling <see cref="DbOps"/>
    /// </summary>
    DbOpContext CreateDbOpContext(MemTable TopTable)
    {
 
        bool GenerateSqlFlag = string.IsNullOrWhiteSpace(TopTable.Sqls.InsertRowSql) || string.IsNullOrWhiteSpace(TopTable.Sqls.UpdateRowSql);
        
        DbOpContext Result = new(Store = this.Store, 
            Transaction, 
            TopTable, 
            CascadeDeletes, 
            GenerateSqlFlag
            );
        
        return Result;
    }
 
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public TableSet(SqlStore Store, MemTable ListTable, MemTable ItemTable, List<MemTable> Stocks, TableSetFlags Flags = TableSetFlags.GenerateSql)
    {
        if (ItemTable == null)
            throw new TripousArgumentNullException("ItemTable");

        ItemTable.CheckTopTableErrors();

        this.Store = Store;
        this.ListTable = ListTable;
        this.ItemTable = ItemTable;
        this.Stocks = Stocks;

        GenerateSql = TableSetFlags.GenerateSql.In(Flags);   
    
        CascadeDeletes = !TableSetFlags.NoCascadeDeletes.In(Flags);

        TableTree = ItemTable.GetTreeAsFlatList();
        SetMaxDetailLevel();
        GenerateSqlStatements();
        SelectStocks();
    }
    
    // ● public
    
    // ● list database operations  
    public int ListSelect(string SqlText) => ListSelect(ListTable, SqlText);
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
                    Table.SetColumnCaptionsFrom(Table.Sqls.DisplayLabels, HideUntitledDisplayLabels);
                }
                finally
                {
                    Table.EventsDisabled = false;
                }
            }
        }

        return Result;

    }
    
    public void ListSave() => ListSave(ListTable);
    public void ListSave(MemTable Table)
    {
        DbOpContext Context = CreateDbOpContext(ListTable);
        DbOps.PostChanges(Context);
    }
    
    public void ListCancel() => ListCancel(ListTable);
    public void ListCancel(MemTable Table) => Table.RejectChanges();
    
    // ● item database operations  
    /// <summary>
    /// Selects the whole table tree from the database starting from the top table (which is a single-row table).
    /// <para>RowId could be string or integer and is the primary key value of the top table.</para>
    /// </summary>
    public bool Load(object RowId)
    {
        if (RowId == null || RowId == DBNull.Value || (RowId is string && string.IsNullOrWhiteSpace(RowId.ToString())))
            return false;
        
        ProcessEmpty();

        ItemTable.EventsDisabled = true;
        try
        {
            if (!string.IsNullOrWhiteSpace(ItemTable.Sqls.SelectRowSql))
            {
                Store.SelectTo(ItemTable, ItemTable.Sqls.SelectRowSql, RowId);
            }

            // select stock tables
            if (RowId != null && ItemTable.Rows.Count >= 1)
            {
                MemTable StockTable;
                for (int i = 0; i < ItemTable.Stocks.Count; i++)
                {
                    StockTable = ItemTable.Stocks[i];
                    Store.SelectTo(StockTable, StockTable.Sqls.SelectRowSql, ItemTable.Rows[0]);
                }
            }

            Select_DoDetails(ItemTable); 
            ItemTable.SetColumnCaptionsFrom(ItemTable.Sqls.DisplayLabels, HideUntitledDisplayLabels);
        }
        finally
        {
            ItemTable.EventsDisabled = false;
        }

        IsInsert = false;
        ItemTable.UpdateCurrentRow();
        return ItemTable.Rows.Count >= 1;
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
        if (ItemTable.Rows.Count == 0)
            return;

        ItemTable.EventsDisabled = true;
        //TopTable.Details.Active = false;
        try
        {
            // delete the top row, which deletes all detail rows too.
            ItemTable.DeleteAll(); // ItemTable.Rows[0].Delete();

            // then, inside a Transaction
            OnTransactionStageDelete(TransactionStage.Start, ExecTime.Before, RowId);

            using (Transaction = Store.BeginTransaction())
            {
                OnTransactionStageDelete(TransactionStage.Start, ExecTime.After, RowId);
                try
                {
                    DbOpContext Context = CreateDbOpContext(ItemTable);
                    DbOps.PostDeletes(Context);

                    OnTransactionStageDelete(TransactionStage.Commit, ExecTime.Before, RowId);
                    Transaction.Commit();
                    OnTransactionStageDelete(TransactionStage.Commit, ExecTime.After, RowId);
                }
                catch
                {
                    ItemTable.DataSet.RejectChanges();
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
            ItemTable.EventsDisabled = false;
            Transaction = null;
        }
        
        ItemTable.UpdateCurrentRow();
    }
    /// <summary>
    /// Commits the whole table tree to the database. It can be either an insert or an update.
    /// </summary>
    public object Commit(bool Reselect)
    {
        if (ItemTable.Rows.Count == 0)
            throw new TableSetException("Nothing to commit. Top table is empty.");
        
        ItemTable.EventsDisabled = true;
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
                    ItemTable.DataSet.AcceptChanges();   // clear logs    
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
            ItemTable.EventsDisabled = false;
            Transaction = null;
        }

        LastCommitedId = null;

        if (ItemTable.Rows.Count > 0)
            LastCommitedId = ItemTable.Rows[0][ItemTable.KeyFields[0]];

        if (Reselect && !Sys.IsNull(LastCommitedId))
            Load(LastCommitedId);

        IsInsert = false;
        ItemTable.UpdateCurrentRow();
        return LastCommitedId;

    }

    /// <summary>
    /// Returns true if ItemTable table, or any of its details, in any depth, has changes.
    /// </summary>
    public bool HasChanges() => ItemTable.TreeHasChanges();
    /// <summary>
    /// Posts any changes (deletes, updates, inserts) to the database
    /// </summary>
    public void PostChanges()
    {
        DbOpContext Context = CreateDbOpContext(ItemTable);
        DbOps.PostChanges(Context);
    }
    
    // ● item edit operation handling 
    /// <summary>
    /// Removes all data rows from all tables in the tableTree
    /// </summary>
    public void ProcessEmpty()
    {
        InternalCancel();
 
        ItemTable.EventsDisabled = true;
        //TopTable.DetailsActive = false;
        try
        {
            Empty(ItemTable);
        }
        finally
        {
            //TopTable.DetailsActive = true;
            ItemTable.EventsDisabled = false;
        }
        
        ItemTable.UpdateCurrentRow();
    }
    /// <summary>
    /// Prepares the TableSet for an insert operation (in the tables, NOT the database)
    /// </summary>
    public void ProcessInsert()
    {
        ProcessEmpty();

        ItemTable.EventsDisabled = true;
        //TopTable.DetailsActive = false;
        try
        {
            if (ItemTable.Rows.Count == 0)
            {
                DataRow Row = ItemTable.NewRow();
                ItemTable.Rows.Add(Row);
            }
        }
        finally
        {
            //TopTable.DetailsActive = true;
            ItemTable.EventsDisabled = false;
        }

        ItemTable.UpdateCurrentRow();
        IsInsert = true;
    }
    /// <summary>
    /// Cancels an edit operation and re-initializes the table tree.
    /// </summary>
    public void ProcessCancel()
    {
        if (IsInsert)
            ProcessInsert();
        else if (ItemTable.Rows.Count > 0 && ItemTable.Rows[0].RowState != DataRowState.Deleted)
            Load(ItemTable.Rows[0][ItemTable.KeyFields[0]]);
    }    
    
    // ● batch database operations 
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
                ItemTable.DataSet.AcceptChanges();
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
                ItemTable.DataSet.RejectChanges();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
            }
        }        
        // ---------------------------------------
        
        if (Args == null)
            throw new TripousArgumentNullException("Args");
        if (Args.BeforeFunc == null)
            throw new TableSetException("Batch commit requires a BeforeFunc.");
        if (Args.AfterFunc == null)
            throw new TableSetException("Batch commit requires an AfterFunc.");
        if (Args.TransLimit <= 0)
            throw new TableSetException("Batch commit TransLimit must be greater than zero.");

        ItemTable.EventsDisabled = true;
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
                    if (ItemTable.Rows.Count > 0)
                        LastCommitedId = ItemTable.Rows[0][ItemTable.KeyField];

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
            ItemTable.EventsDisabled = false;
            Transaction = null;
        }
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
    /// Returns the Id of the last commit
    /// </summary>
    public object LastCommitedId { get; private set; }
    /// <summary>
    /// If true, then when SELECTing to a MemTable, hides any column not found in the table's Sqls.DisplayLabels.  
    /// </summary>
    public bool HideUntitledDisplayLabels { get; set; }

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