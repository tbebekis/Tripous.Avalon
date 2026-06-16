/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a set of tables that are related to each other.
/// </summary>
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
                SqlStatementBuilder.BuildSql(ModuleName, TableTree[i], Store, TableTree[i] == ItemTable);
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
                            Row.CancelEdit();
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
    void Select_DoAddToDetail(string SqlText, MemTable tblDetail)
    {
        DataTable Source = Store.Select(SqlText);

        if (tblDetail.Columns.Count == 0)
            Source.CopyStructureTo(tblDetail);

        tblDetail.BeginLoadData();
        try
        {
            string FieldName = tblDetail.KeyField;
            object Value;
            for (int i = 0; i < Source.Rows.Count; i++)
            {
                Value = Source.Rows[i][FieldName];
                if (tblDetail.Locate(FieldName, Value, LocateOptions.None) == null)
                    Source.Rows[i].AppendTo(tblDetail);
            }
        }
        finally
        {
            tblDetail.EndLoadData();
        }

    }
    /// <summary>
    /// Executes the SELECT of the DetailTable.  
    /// </summary>
    void Select_DoDetail(MemTable tblMaster, MemTable tblDetail)
    {
        string SqlText;

        if (!string.IsNullOrWhiteSpace(tblDetail.Sqls.SelectSql))
        {
            // 1. SqlText execution ===================================================
            if ((tblMaster.Rows.Count > 0) && (tblMaster.Columns.Contains(tblDetail.MasterField)))
            {
                //  limit the number of elements inside the in (...),  in order
                //    to avoid problems with database servers that have such a limit.   
                List<string> KeyValuesList = tblMaster.GetKeyValuesList(tblDetail.MasterField, 100);

                for (int i = 0; i < KeyValuesList.Count; i++)
                {
                    SelectSql SS = new SelectSql(tblDetail.Sqls.SelectSql);
                    SS.Where = $"{tblDetail.TableName}.{tblDetail.DetailField} in ({KeyValuesList[i]})";
                    SqlText = SS.Text;

                    Select_DoAddToDetail(SqlText, tblDetail);
                }
            }

            tblDetail.SetColumnCaptionsFrom(tblDetail.Sqls.DisplayLabels, HideUntitledDisplayLabels);

            if (!tblDetail.IsEmpty)
                Select_DoDetails(tblDetail);
        }
    }
    /// <summary>
    /// Executes the SELECT of Details of the MasterTable.
    /// </summary>
    void Select_DoDetails(MemTable tblMaster)
    {
        foreach (MemTable tblDetail in tblMaster.Details)
        {
            Select_DoDetail(tblMaster, tblDetail);
            tblDetail.AcceptChanges();
        }
    }

    // ●  event triggers  
    /// <summary>
    /// Triggers the TransactionDelete event.
    /// </summary>
    void OnTransactionStageDelete(TransactionStage Stage, ExecTime ExecTime, object RowId)
    {
        if (TransactionStageDelete != null)
            TransactionStageDelete(this, new TransactionEventArgs(Store, Transaction, Stage, ExecTime, RowId));
    }
    /// <summary>
    /// Triggers the TransactionCommit event.
    /// </summary>
    void OnTransactionStageCommit(TransactionStage Stage, ExecTime ExecTime)
    {
        if (TransactionStageCommit != null)
            TransactionStageCommit(this, new TransactionEventArgs(Store, Transaction, Stage, ExecTime, -1));
    }

    // ●  miscs  
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
        
        DbOpContext Result = new(ModuleName,
            Store = this.Store, 
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
    public TableSet(string ModuleName, SqlStore Store, MemTable ListTable, MemTable ItemTable, List<MemTable> Stocks, TableSetFlags Flags = TableSetFlags.GenerateSql)
    {
        if (ItemTable == null)
            throw new TripousArgumentNullException("ItemTable");

        ItemTable.CheckTopTableErrors();

        this.ModuleName = ModuleName;
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
    /// <summary>
    /// Executes the SELECT SqlText and puts the returned data rows to the ListTable.
    /// </summary>
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
    
    /// <summary>
    /// Saves the ListTable to the database.
    /// </summary>
    public void ListSave() => ListSave(ListTable);
    /// <summary>
    /// Saves the Table to the database.
    /// </summary>
    public void ListSave(MemTable Table)
    {
        DbOpContext Context = CreateDbOpContext(Table);
        DbOps.PostChanges(Context);
    }
    
    /// <summary>
    /// Cancel any pending edit operation in the ListTable.
    /// </summary>
    public void ListCancel() => ListCancel(ListTable);
    /// <summary>
    /// Cancel any pending edit operation in the Table.
    /// </summary>
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

            Select_DoDetails(ItemTable); 
            ItemTable.SetColumnCaptionsFrom(ItemTable.Sqls.DisplayLabels, HideUntitledDisplayLabels);
            AcceptChanges();
        }
        finally
        {
            ItemTable.EventsDisabled = false;
        }

        IsInsert = false;
        ItemTable.UpdateCurrentRow();
        ItemTable.RefreshDetails();
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
        try
        {
            ItemTable.DeleteAll(AcceptChangesToo: false); 

            // then, inside a Transaction
            OnTransactionStageDelete(TransactionStage.Start, ExecTime.Before, RowId);

            using (SqlTransactionContext TransactionContext = Store.BeginTransactionContext())
            {
                Transaction = TransactionContext.Transaction;
                OnTransactionStageDelete(TransactionStage.Start, ExecTime.After, RowId);
                try
                {
                    OnTransactionStageDelete(TransactionStage.Post, ExecTime.Before, RowId);

                    DbOpContext Context = CreateDbOpContext(ItemTable);
                    DbOps.PostDeletes(Context);

                    OnTransactionStageDelete(TransactionStage.Post, ExecTime.After, RowId);

                    OnTransactionStageDelete(TransactionStage.Commit, ExecTime.Before, RowId);
                    TransactionContext.Commit();
                    AcceptChanges();
                    OnTransactionStageDelete(TransactionStage.Commit, ExecTime.After, RowId);
                }
                catch
                {
                    RejectChanges();
                    if (TransactionContext.IsActive)
                    {
                        OnTransactionStageDelete(TransactionStage.Rollback, ExecTime.Before, RowId);
                        TransactionContext.Rollback();
                        OnTransactionStageDelete(TransactionStage.Rollback, ExecTime.After, RowId);
                    }
                    throw;
                }
            }
        }
        finally
        {
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
        try
        {
            OnTransactionStageCommit(TransactionStage.Start, ExecTime.Before);

            using (SqlTransactionContext TransactionContext = Store.BeginTransactionContext())
            {
                Transaction = TransactionContext.Transaction;
                OnTransactionStageCommit(TransactionStage.Start, ExecTime.After);
                try
                {
                    OnTransactionStageCommit(TransactionStage.Post, ExecTime.Before);
                    PostChanges();
                    OnTransactionStageCommit(TransactionStage.Post, ExecTime.After);

                    OnTransactionStageCommit(TransactionStage.Commit, ExecTime.Before);
                    TransactionContext.Commit();
                    AcceptChanges();
                    OnTransactionStageCommit(TransactionStage.Commit, ExecTime.After);
                }
                catch
                {
                    if (TransactionContext.IsActive)
                    {
                        OnTransactionStageCommit(TransactionStage.Rollback, ExecTime.Before);
                        TransactionContext.Rollback();
                        OnTransactionStageCommit(TransactionStage.Rollback, ExecTime.After);
                    }
                    throw;
                }
            }
        }
        finally
        {
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
    /// Cancels any pending edit operation in the whole table tree.
    /// </summary>
    public void AcceptChanges() => ItemTable.AcceptChangesAll();
    /// <summary>
    /// Rejects any pending edit operation in the whole table tree.
    /// </summary>
    public void RejectChanges() => ItemTable.RejectChangesAll();
    /// <summary>
    /// Returns true if ItemTable table, or any of its details, in any depth, has changes.
    /// </summary>
    public bool HasChanges() => ItemTable.HasChangesAll();
    /// <summary>
    /// Posts any changes (deletes, updates, inserts) to the database
    /// <para><b>WARNING:</b> Does <b>NOT</b> call <see cref="MemTable.AcceptChangesAll"/> on item table. </para>
    /// </summary>
    public void PostChanges()
    {
        DbOpContext Context = CreateDbOpContext(ItemTable);
        DbOps.PostChanges(Context);
    }
    
    // ● item edit operation handling 
    /// <summary>
    /// Removes all data rows from all tables in the tableTree
    /// <para><b>WARNING:</b> Does <b>NOT</b> call <see cref="MemTable.AcceptChangesAll"/> on item table. </para>
    /// </summary>
    public void ProcessEmpty()
    {
        InternalCancel();
 
        ItemTable.EventsDisabled = true;
        //TopTable.DetailsActive = false;
        try
        {
            //Empty(ItemTable);
            ItemTable.DeleteAll(AcceptChangesToo: true);
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
    /// <para><b>WARNING:</b> Does <b>NOT</b> call <see cref="MemTable.AcceptChangesAll"/> on item table. </para>
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
    /// <para><b>WARNING:</b> Does <b>NOT</b> call <see cref="MemTable.AcceptChangesAll"/> on item table. </para>
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
    /// <para>Starts a transaction and keeps on calling <see cref="BatchCommitArgs.BeforeFunc()"/> while <see cref="BatchCommitArgs.AfterFunc()"/>  returns true.</para>
    /// <para>Commits the transaction each time the <see cref="BatchCommitArgs.TransLimit"/> is reached.</para>
    /// <para>NOTE: <see cref="BatchCommitArgs.BeforeFunc()"/> and <see cref="BatchCommitArgs.AfterFunc()"/> are optional.</para>
    /// </summary>
    public void CommitBatch(BatchCommitArgs Args)
    {
        SqlTransactionContext TransactionContext = null;

        // ---------------------------------------
        void CommitBatchTransaction()
        {
            if (TransactionContext == null)
                return;

            try
            {
                OnTransactionStageCommit(TransactionStage.Commit, ExecTime.Before);
                TransactionContext.Commit();
                ItemTable.DataSet.AcceptChanges();
                OnTransactionStageCommit(TransactionStage.Commit, ExecTime.After);
            }
            finally
            {
                TransactionContext.Dispose();
                TransactionContext = null;
                Transaction = null;
            }
        }
        // ---------------------------------------
        void RollbackBatchTransaction()
        {
            if (TransactionContext == null)
                return;

            try
            {
                OnTransactionStageCommit(TransactionStage.Rollback, ExecTime.Before);
                TransactionContext.Rollback();
                ItemTable.DataSet.RejectChanges();
                OnTransactionStageCommit(TransactionStage.Rollback, ExecTime.After);
            }
            finally
            {
                TransactionContext.Dispose();
                TransactionContext = null;
                Transaction = null;
            }
        }
        // ---------------------------------------

        if (Args == null)
            throw new TripousArgumentNullException("Args");
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
                ShouldPost = Args.BeforeFunc != null ? Args.BeforeFunc() : true;

                if (ShouldPost)
                {
                    if (TransactionContext == null)
                    {
                        OnTransactionStageCommit(TransactionStage.Start, ExecTime.Before);
                        TransactionContext = Store.BeginTransactionContext();
                        Transaction = TransactionContext.Transaction;
                        OnTransactionStageCommit(TransactionStage.Start, ExecTime.After);
                    }

                    OnTransactionStageCommit(TransactionStage.Post, ExecTime.Before);
                    PostChanges();
                    OnTransactionStageCommit(TransactionStage.Post, ExecTime.After);

                    LastCommitedId = null;
                    if (ItemTable.Rows.Count > 0)
                        LastCommitedId = ItemTable.Rows[0][ItemTable.KeyField];

                    PostCounter++;
                    Args.PostCounter = PostCounter;

                    if (PostCounter % Args.TransLimit == 0)
                        CommitBatchTransaction();
                }

                Continue = Args.AfterFunc != null ? Args.AfterFunc(LastCommitedId) : true;
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
    /// <para><b>WARNING:</b> The <see cref="ModuleName"/> and a TableName are used in constructing a unique StatementName.</para>
    /// <para>The StatementName is used with the <see cref="SqlStore.GetNativeSchemaFromTableName"/>
    /// so the <c>ModuleName.TableName</c> must construct a unique name because schema DataTables are stored in the <see cref="SqlCache"/> under that unique name. </para>
    /// </summary>
    public string ModuleName { get; }
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
    public event EventHandler<TransactionEventArgs> TransactionStageDelete;
    /// <summary>
    /// Occurs when <see cref="Commit"/>() method is called.
    /// </summary>
    public event EventHandler<TransactionEventArgs> TransactionStageCommit;   
}
