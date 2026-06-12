/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Base class for all data modules.
/// <para>A data module represents a table tree, such as SalesOrder along with a list SELECT table.</para>
/// </summary>
[TypeStore]
public class DataModule
{
    // ● operation flags 
    /// <summary>
    /// Field
    /// </summary>
    protected int fIsInserting;
    /// <summary>
    /// Field
    /// </summary>
    protected int fIsEditing;
    /// <summary>
    /// Field
    /// </summary>
    protected int fIsDeleting;
    /// <summary>
    /// Field
    /// </summary>
    protected int fIsCommiting;

    /// <summary>
    /// Field
    /// </summary>
    protected Dictionary<string, object> fVariables;
    /// <summary>
    /// Field
    /// </summary>
    protected TableSet TableSet;
    
    // ● tableset event handlers  
    /// <summary>
    /// Gets a notification from the TableSet when deleting
    /// </summary>
    protected virtual void TableSet_TransactionStageDelete(object sender, TransactionEventArgs e)
    {
    }
    /// <summary>
    /// Gets a notification from the TableSet during commit transaction stages.
    /// </summary>
    protected virtual void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        // ** in web applications we do NOT have TableSet state
        // NOTE: We use the Post stage here because code assignment must run inside the transaction and just before changes are posted.
        // Using the Post stage, instead of Start, covers both cases: normal commits and batch commits.
        if (e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.Before && CodeProviderDef != null)
        {
            AssignCodeValue(e.Transaction);
        }
    }
 
    // ● overridables - code provider
    /// <summary>
    /// Returns the <see cref="CodeProviderDef"/>
    /// </summary>
    protected virtual CodeProviderDef GetCodeProviderDef() => CodeProviderDef;
    /// <summary>
    /// Assigns the <see cref="CodeProviderDef"/>
    /// </summary>
    protected virtual void AssignCodeProviderDef()
    {
        FieldDef FieldDef = ModuleDef.Table.Fields.Find("Code");
        if (FieldDef != null && !string.IsNullOrWhiteSpace(FieldDef.CodeProvider))
        {
           this.CodeProviderDef = DataRegistry.CodeProviders.Get(FieldDef.CodeProvider);
        }
    }
    /// <summary>
    /// Returns the next number using an atomic locked increment.
    /// Handles reset safely inside the same transaction.
    /// </summary>
    public virtual string GetNextCodeLocked(DbTransaction Transaction)
    {
        CodeProviderDef CPD = GetCodeProviderDef();
        
        if (CPD == null)
            throw new TripousDataException($"Cannot get next Code. {nameof(CodeProviderDef)} is null.");
        
        string CodeProviderName = CPD.Name;
        
        if (Transaction == null)
            throw new TripousDataException($"Cannot get next Code. {nameof(DbTransaction)} is null.");
 
        int Number = 1;
 
        DataRow Row = Store.Provider.SelectForUpdate(
            Transaction,
            DbConfig.SysNumberSeriesTableName,
            "Code",
            Store.ConnectionInfo.CommandTimeoutSeconds,
            CodeProviderName);

        if (Row == null)
            throw new TripousDataException($"{CodeProviderName} not found in {DbConfig.SysNumberSeriesTableName}");

        CodeProviderEntry CodeProviderEntry = new CodeProviderEntry(Row);

        string LastResetValue = Row.AsString("LastResetValue");
        int NextNumber = Row.AsInteger("NextNumber");
        string ResetValue = CodeProviderEntry.GetResetValue(DateTime.Today);
        bool RequiresReset = !string.IsNullOrWhiteSpace(ResetValue) && !LastResetValue.IsSameText(ResetValue);

        string SqlText = $"""
                          update {DbConfig.SysNumberSeriesTableName}
                          set NextNumber = :NextNumber,
                              LastResetValue = :LastResetValue
                          where Code = :Code
                          """;
        
        if (RequiresReset)
        {
            Number = 1;

            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Code"] = CodeProviderName,
                ["NextNumber"] = 2,
                ["LastResetValue"] = ResetValue,
            });
        }
        else
        {
            Number = NextNumber;

            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Code"] = CodeProviderName,
                ["NextNumber"] = NextNumber + 1,
                ["LastResetValue"] = LastResetValue,
            });
        }

        string Result = CodeProviderEntry.Format(DateTime.Today, Number);
        return Result;
    }
    /// <summary>
    /// Called from inside a commit transaction in order to assign the Code column
    /// </summary>
    protected virtual void AssignCodeValue(DbTransaction Transaction)
    {
        if (CodeProviderDef != null)
        {
            if (tblItem != null && tblItem.Rows.Count > 0 && tblItem.ContainsColumn("Code"))
            {
                foreach (DataRow Row in tblItem.Rows)
                {
                    bool IsInsertRow = Row.RowState == DataRowState.Added; // TableSet always adds a new DataRow in ProcessInsert()
                    bool IsCodeEmpty = Sys.IsNull(Row["Code"]) || string.IsNullOrEmpty(Sys.AsString(Row["Code"], string.Empty).Trim());

                    if (IsInsertRow && IsCodeEmpty)
                    {
                        Row["Code"] = GetNextCodeLocked(Transaction);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the default values, such as the default values for fields like CompanyId, CurrencyId, etc.
    /// </summary>
    protected virtual void LoadDefaultValues()
    {
    }
    
    /// <summary>
    /// Sets default values for all Tables.
    /// <para>It is called by the DoInsertAfter() and DoCommitBefore() </para>
    /// </summary>
    protected virtual void SetDefaultValues()
    {
        if (IsInserting || IsCommiting)
        {
            MemTable Table;
            List<TableDef> TableDefs = ModuleDef.GetTables();
            foreach (TableDef TableDef in TableDefs)
            {
                Table = GetTable(TableDef.Name);
                if (Table != null)
                {
                    SetDefaultValues(Table, TableDef);
                    SqlValueProviders.Process(Table, Store);
                }
            }
        }
    }
    /// <summary>
    /// Sets default values to the Table. It is called when a commit operation starts.
    /// </summary>
    protected virtual void SetDefaultValues(DataTable Table, TableDef TableDef)
    {
        if (IsInserting || IsCommiting)
        {
            foreach (DataRow Row in Table.Rows)
                SetDefaultValues(Table, Row, TableDef);
        }
    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected virtual void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        if (Row.RowState == DataRowState.Deleted)
            return;
        
        if (Table == tblItem)
            SetAuditDefaultValues(Table, Row, TableDef);

        Tuple<TableDef, FieldDef> Pair;
        FieldDef FieldDes;

        foreach (DataColumn Column in Row.Table.Columns)
        {
            if (!Column.ReadOnly)
            {
                if (Sys.IsNull(Row[Column]) || (Simple.SimpleTypeOf(Column.DataType).IsString() && (Row[Column].ToString() == string.Empty)))
                {
                    if (TableDef != null)
                    {
                        Pair = TableDef.FindAnyField(Column.ColumnName);
                        if (Pair != null)
                        {
                            FieldDes = Pair.Item2;

                            if (FieldDes != null)
                            {
                                // skip the column if the column descriptor is marked as read-only 
                                if (FieldDes.IsReadOnly)
                                    continue;

                                // DefaultValue
                                SqlValueProviders.Process(Row, Column, FieldDes.DefaultValue, Store);

                                // if still is null
                                if (Sys.IsNull(Row[Column]) && FieldDes.IsBoolean)
                                {
                                    Row[Column] = 0;
                                }
                            }
                        }

                    }

                    // if still is null
                    if (Sys.IsNull(Row[Column]) && (Column.DataType == typeof(System.Boolean)))
                        Row[Column] = false;
                    else if (Sys.IsNull(Row[Column]) || (Simple.SimpleTypeOf(Column.DataType).IsString() && (Row[Column].ToString() == string.Empty)))
                    {
                        if (Sys.IsSameText(DbConfig.CompanyFieldName, Column.ColumnName)) // ColumnName is CompanyId
                            Row[Column] = DbConfig.CompanyId;
                    }
                }

            }

        }

    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// <para>NOTE: This method is called only for the <see cref="tblItem"/> table.</para>
    /// <para>NOTE: This method sets the <c>CreatedBy, CreatedAt, ModifiedBy, ModifiedAt</c> field values, if they exist in the <see cref="tblItem"/>.</para>
    /// </summary>
    protected virtual void SetAuditDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem)
        {
            if (IsInserting)
            {
                Row.SetValue("CreatedBy", Sys.GetCurrentAppUserId());
                Row.SetValue("CreatedAt", DateTime.UtcNow);
            }
            
            Row.SetValue("ModifiedBy", Sys.GetCurrentAppUserId());
            Row.SetValue("ModifiedAt", DateTime.UtcNow);
        }
    }
    
    /// <summary>
    /// Called from inside <see cref="Commit"/>.
    /// <para>NOTE: It looks like, in some cases, we have to call EndEdit() for the DataRow(s) to post the changes.</para>
    /// </summary>
    protected virtual void EndEdit()
    {
        void EndEditInternal(MemTable Table)
        {
            foreach (MemTable tblChild in Table.Details)
                EndEditInternal(tblChild);
            
            foreach (DataRow Row in Table.Rows)
                Row.EndEdit();
        }

        EndEditInternal(tblItem);
    }
    
    /// <summary>
    /// Ensures that any TableDef is updated with the actual table schema from the database.
    /// </summary>
    protected virtual void UpdateTableSchema()
    {
        void UpdateSchema(TableDef T)
        {
            string TableName = T.Name;
            string StatementName = $"{this.GetType().FullName}.{Name}.{TableName}";
        
            DataTable SchemaTable = Store.GetNativeSchemaFromTableName(StatementName, TableName);
            T.UpdateFrom(SchemaTable);
            
            if (T.Details != null)
                foreach (var Item in T.Details)
                    UpdateSchema(Item);
        }

        UpdateSchema(ModuleDef.Table);
    }

    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Inserting()
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Inserted()
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Editing(object RowId)
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Edited(object RowId)
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Deleting(object RowId)
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Deleted(object RowId)
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Commiting(bool Reselect)
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual void Commited(bool Reselect, object RowId)
    {
    }
    /// <summary>
    /// Notification method
    /// </summary>
    protected virtual bool MustReselectAfterCommit() => CodeProviderDef != null;

    /// <summary>
    /// Event handler
    /// </summary>
    protected virtual void ColumnChanging(MemTable Table, DataColumnChangeEventArgs ea)
    {
    }
    /// <summary>
    /// Event handler
    /// </summary>
    protected virtual void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
    }
    /// <summary>
    /// Event handler
    /// WARNING: the new row is not in rows yet
    /// </summary>
    protected virtual void NewRowAdding(MemTable Table, DataTableNewRowEventArgs ea)
    {
    }
    /// <summary>
    /// Event handler
    /// </summary>
    protected virtual void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
    }


    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public DataModule()
    {
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => Name;

    // ● list
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="ModuleDef"></param>
    public virtual void Initialize(ModuleDef ModuleDef)
    {
        if (this.ModuleDef == null)
        {
            if (ModuleDef == null)
                throw new TripousArgumentNullException(nameof(ModuleDef));

            DataRegistry.UpdateLocatorReferences();
            
            this.ModuleDef = ModuleDef;
            ModuleDef.UpdateReferences();
            
            // ● Connection Info
            DbConnectionInfo ConnectionInfo = Db.GetConnectionInfo(ModuleDef.ConnectionName);

            if (ConnectionInfo == null)
                throw new DataModuleException($"Cannot initialize {nameof(DataModule)}. No {nameof(DbConnectionInfo)} found");
 
            // ● SqlStore
            Store = SqlStores.CreateSqlStore(ConnectionInfo);
            
            // ● Update the schema of all tables
            UpdateTableSchema();
            
            // ● DataSet
            DataSet = new DataSet("DS_" + ModuleDef.Name);
            tblList = new MemTable("List");
            DataSet.Tables.Add(tblList);
            
            List<TableDef> TableDefs = ModuleDef.GetTables();
            
            // ● get the sql generation flags
            // -----------------------------------------------------------
            BuildSqlFlags GetBuildSqlFlags()
            {
                BuildSqlFlags Result = BuildSqlFlags.None;

                if (ModuleDef.GuidOids)
                    Result |= BuildSqlFlags.GuidOids;
                else if (Store.Provider.OidMode == OidMode.Generator)
                    Result |= BuildSqlFlags.OidModeIsBefore;
         
                Result |= BuildSqlFlags.IncludeBlobFields;

                return Result;
            }
            // -----------------------------------------------------------
            BuildSqlFlags SqlFlags = GetBuildSqlFlags();
 
            // ● for all Tables of the module definition
            // - ensure that any TableDef is updated with the actual table schema from the database.
            // - create sql statements 
            // - create DataTable objects    
            
            MemTable Table;
            TableSqls Sqls;
            foreach (var TableDef in TableDefs)
            {
                Table = TableDef.CreateDescriptorTable(Store);  // TableDef.CreateDescriptorTable(Store, table => DataSet.Tables.Add(table));
                DataSet.Tables.Add(Table);
                ItemTables.Add(Table);
                
                Sqls = TableDef.BuildSql(SqlFlags);
                Table.Sqls.AssignFrom(Sqls);
                Table.AutoGenerateGuidKeys = ModuleDef.GuidOids;

                Table.ColumnChanging += (Sender, Args) => ColumnChanging(Sender as MemTable, Args);
                Table.ColumnChanged += (Sender, Args) => ColumnChanged(Sender as MemTable, Args);
                
                Table.NewRowAdding += (Sender, Args) => NewRowAdding(Sender as MemTable, Args);
                Table.NewRowAdded += (Sender, Args) => NewRowAdded(Sender as MemTable, Args);
            }
            
            tblItem = FindTable(ModuleDef.Table.Name);
            
            // ● details
            // -----------------------------------------------------------
            void CollectDetails(MemTable tblMaster, TableDef MasterDef)
            {
                MemTable tblDetail;

                foreach (TableDef DetailDef in MasterDef.Details)
                {
                    tblDetail = this.GetTable(DetailDef.Name);
                    tblDetail.Master = tblMaster;
                    tblMaster.Details.Add(tblDetail);
                    
                    // do a recursion to add detail Tables to this table
                    CollectDetails(tblDetail, DetailDef);
                }
            }
            // -----------------------------------------------------------
            CollectDetails(tblItem, ModuleDef.Table);
            tblItem.Details.Active = true;
            
            // ● DataColumn expressions - must be assigned after DataRelations are constructed
            // NOTE: we don't use DataRelations anymore
            DataColumn Field;
            foreach (var TableDef in TableDefs)
            {
                Table = this.GetTable(TableDef.Name);
                foreach (var FieldDef in TableDef.Fields)
                {
                    if (!string.IsNullOrEmpty(FieldDef.Expression))
                    {
                        Field = Table.GetColumn(FieldDef.Name);
                        Field.Expression = FieldDef.Expression;
                    }
                }
            }
            
            // ● Stocks - stock tables - creates the stock tables of the module 
            foreach (SelectDef StockDef in ModuleDef.Stocks)
            {
                if (string.IsNullOrWhiteSpace(StockDef.SqlText))
                    StockDef.SqlText = $"select * from {StockDef.Name}";
                    
                Table = GetTable(StockDef.Name);
                Table = new MemTable(StockDef.Name);
                DataSet.Tables.Add(Table);
                this.Stocks.Add(Table);
                
                Table.Sqls.SelectSql = StockDef.SqlText;
                Table.Sqls.DisplayLabels = StockDef.DisplayLabels;
            }

            // ● code provider
            AssignCodeProviderDef();
            
            // ● TableSet
            TableSetFlags TableSetFlags = TableSetFlags.None;
 
            if (!ModuleDef.CascadeDeletes)
                TableSetFlags |= TableSetFlags.NoCascadeDeletes;
            
            TableSet = new TableSet(this.Name, Store, tblList, tblItem, Stocks, TableSetFlags);

            TableSet.TransactionStageCommit += new EventHandler<TransactionEventArgs>(TableSet_TransactionStageCommit);
            TableSet.TransactionStageDelete += new EventHandler<TransactionEventArgs>(TableSet_TransactionStageDelete);

            // ● default values
            LoadDefaultValues();
        }
        
    }
 
    // ● list
    /// <summary>
    /// Selects the list table.
    /// </summary>
    public virtual void ListSelect(SelectDef SelectDef)
    {        
        if (SelectDef != null)
            ListSelect(SelectDef.SqlText);
    }
    /// <summary>
    /// Selects the list table.
    /// </summary>
    public virtual void ListSelect(string SqlText)
    {        
        if (!string.IsNullOrWhiteSpace(SqlText))
            TableSet.ListSelect(tblList, SqlText);
    }
    /// <summary>
    /// Saves the list table.
    /// </summary>
    public virtual void ListSave() => TableSet.ListSave();
    /// <summary>
    /// Rejects the changes in the list table.
    /// </summary>
    public virtual void ListCancel() => TableSet.ListCancel();
 
    // ● item
    /// <summary>
    /// Starts an insert operation. Valid with master modules only.
    /// </summary>
    public virtual void Insert()
    {
        IsInserting = true;
        try
        {
            CheckCanInsert();
            Inserting();
            TableSet.ProcessInsert();
            SetDefaultValues();
            Inserted();
        }
        finally
        {
            State = DataMode.Insert;
            IsInserting = false;
        }
    }
    /// <summary>
    /// Starts an edit operation. Valid with master modules only.
    /// </summary>
    public virtual void Edit(object RowId)
    {
        CheckCanEdit(RowId);

        IsEditing = true;
        try
        {
            Editing(RowId);
            TableSet.Load(RowId);
            LastEditedId = RowId;
            Edited(RowId);
        }
        finally
        {
            State = DataMode.Edit;
            IsEditing = false;
        }
    }
    /// <summary>
    /// Deletes a row. Valid with master modules only.
    /// </summary>
    public virtual void Delete(object RowId)
    {        
        CheckCanDelete(RowId);

        IsDeleting = true;
        try
        {
            Deleting(RowId);
            TableSet.Delete(RowId);
            LastDeletedId = RowId;
            Deleted(RowId);
        }
        finally
        {
            State = DataMode.None;
            IsDeleting = false;
        }
    }
    /// <summary>
    /// Commits changes after an insert or edit. Valid with master modules only.
    /// <para>Returns the row id of the tblItem commited row.</para>
    /// </summary>
    public virtual object Commit(bool Reselect = false)
    {
        object Result = null;
 
        IsCommiting = true;
        try
        {
            Reselect = Reselect || MustReselectAfterCommit();
            EndEdit();
            SetDefaultValues();
            EndEdit();
        
            CheckCanCommit(Reselect);
            
            Commiting(Reselect);
            Result = TableSet.Commit(Reselect);
            LastCommitedId = Result;
            Commited(Reselect, Result);
        }
        finally
        {
            State = DataMode.Edit;
            IsCommiting = false;
        }

        return Result;
    }
    /// <summary>
    /// Rejects the changes after an insert or edit. Valid with master modules only.
    /// </summary>
    public virtual void Cancel()
    {
        TableSet.RejectChanges();
        State = DataMode.Edit;
    }
    /// <summary>
    /// Returns true if <see cref="TableSet.ItemTable"/> table, or any of its details, in any depth, has changes.
    /// </summary>
    public virtual bool HasChanges() => TableSet.HasChanges();

    /// <summary>
    /// A Commit() version for batch operations.
    /// <para>Starts a transaction and keeps on calling <see cref="BatchCommitArgs.BeforeFunc()"/> while <see cref="BatchCommitArgs.AfterFunc()"/>  returns true.</para>
    /// <para>Commits the transaction each time the <see cref="BatchCommitArgs.TransLimit"/> is reached.</para>
    /// <para>NOTE: <see cref="BatchCommitArgs.BeforeFunc()"/> and <see cref="BatchCommitArgs.AfterFunc()"/> are optional.</para>
    /// </summary>
    public virtual void CommitBatch(BatchCommitArgs Args) => TableSet.CommitBatch(Args);
    
    /// <summary>
    /// Inserts rows from a source table using batch commit.
    /// </summary>
    public virtual void BatchInsert(DataTable tblSource)
    {
        if (tblSource == null)
            throw new TripousArgumentNullException(nameof(tblSource));

        DataRow SourceRow;
        BatchCommitArgs Args = null;

        var FieldList = ModuleDef.Table.Fields
            .Where(x => x.IsNativeField && !x.IsReadOnly && !x.IsNoInsertOrUpdate && string.IsNullOrWhiteSpace(x.CodeProvider))
            .ToList();

        var SourceColumns = tblSource.Columns
            .Cast<DataColumn>()
            .Where(x => FieldList.Any(y => y.Name.IsSameText(x.ColumnName)))
            .ToList();

        var TargetColumns = tblItem.Columns
            .Cast<DataColumn>()
            .Where(x => SourceColumns.Any(y => y.ColumnName.IsSameText(x.ColumnName)))
            .ToList();

        foreach (DataColumn SourceColumn in SourceColumns)
        {
            DataColumn TargetColumn = TargetColumns.FirstOrDefault(x => x.ColumnName.IsSameText(SourceColumn.ColumnName));

            if (TargetColumn == null)
                throw new TripousDataException($"Target column not found: {SourceColumn.ColumnName}");

            if (TargetColumn.DataType != SourceColumn.DataType)
                throw new TripousDataException($"Column type mismatch: {SourceColumn.ColumnName}");
        }

        // ---------------------------------------------
        Func<bool> BeforeFunc = delegate()
        {
            SourceRow = tblSource.Rows[Args.Counter];

            Insert();

            foreach (DataColumn SourceColumn in SourceColumns)
            {
                DataColumn TargetColumn = TargetColumns.First(x => x.ColumnName.IsSameText(SourceColumn.ColumnName));
                tblItem.Rows[0][TargetColumn] = SourceRow[SourceColumn];
            }

            return true;
        };
        // ---------------------------------------------
        Func<object, bool> AfterFunc = delegate(object LastId)
        {
            bool ShouldContinue = Args.Counter < tblSource.Rows.Count - 1;
            return ShouldContinue;
        };
        // ---------------------------------------------

        Args = new BatchCommitArgs(BeforeFunc, AfterFunc);
        CommitBatch(Args);
    }
    
    // ● item checks
    /// <summary>
    /// Called by the <see cref="Insert"/> and throws an exception if, for some reason,
    /// starting an insert operation is considered invalid.
    /// </summary>
    public virtual void CheckCanInsert()
    {
    }
    /// <summary>
    /// Called by the <see cref="Edit"/> and throws an exception if, for some reason,
    /// starting an edit operation is considered invalid.
    /// </summary>
    public virtual void CheckCanEdit(object RowId)
    {
        if (Sys.IsNull(RowId))
            throw new DataModuleException("Can not edit item. Invalid RowId");
    }
    /// <summary>
    /// Called by the <see cref="Delete"/> and throws an exception if, for some reason,
    /// deleting the row in the database is considered invalid.
    /// </summary>
    public virtual void CheckCanDelete(object RowId)
    {
        if (Sys.IsNull(RowId))
            throw new DataModuleException("Can not delete item. Invalid RowId");
    }
    /// <summary>
    /// Called by the <see cref="Commit"/> and throws an exception if, for some reason,
    /// commiting item is considered invalid.
    /// </summary>
    public virtual void CheckCanCommit(bool Reselect)
    {
    }

    /// <summary>
    /// True if a table exists, by name.
    /// </summary>
    public bool TableExists(string TableName) => FindTable(TableName) != null;
    /// <summary>
    /// Finds a table by name, if any, else null.
    /// </summary>
    public MemTable FindTable(string TableName) => Tables.FirstOrDefault(x => TableName.IsSameText(x.TableName)); 
    /// <summary>
    /// Gets a table by name, if any, else exception.
    /// </summary>
    public MemTable GetTable(string TableName)
    {
        MemTable Result = FindTable(TableName);

        if (Result == null)
            throw new DataModuleException($"Table {TableName} not found.");

        return Result;
    }
    
    // ● properties
    /// <summary>
    /// True if the module is initialized.
    /// </summary>
    public bool IsInitialized => ModuleDef != null;
    /// <summary>
    /// The module definition.
    /// </summary>
    public ModuleDef ModuleDef { get; protected set; }
    /// <summary>
    /// The code provider definition.
    /// </summary>
    public CodeProviderDef CodeProviderDef { get; protected set; }
    /// <summary>
    /// The <see cref="SqlStore"/> instance."/>
    /// </summary>
    public SqlStore Store { get; protected set; }
    /// <summary>
    /// An indexer with all tables
    /// </summary>
    /// <param name="TableName"></param>
    public MemTable this[string TableName] => GetTable(TableName);
    /// <summary>
    /// The DataSet of the module.
    /// </summary>
    public DataSet DataSet { get; protected set; }
    /// <summary>
    /// The list table, the one that has the list SELECT data
    /// </summary>
    public MemTable tblList { get; protected set; }
    /// <summary>
    /// The top table of the table tree. Is always a single-row table.
    /// </summary>
    public MemTable tblItem { get; protected set; }
    /// <summary>
    /// All tables, <see cref="tblList"/> included.
    /// </summary>
    public IEnumerable<MemTable> Tables => DataSet.Tables.Cast<MemTable>();
    /// <summary>
    /// The item tables only. <see cref="tblList"/> is not included."
    /// </summary>
    public List<MemTable> ItemTables = new();
    /// <summary>
    /// The stock tables.
    /// </summary>
    public List<MemTable> Stocks => new();
    /// <summary>
    /// The name of this module
    /// </summary>
    public string Name => ModuleDef.Name;
    /// <summary>
    /// When true then detail relationships are active.
    /// </summary>
    public bool DetailsActive
    {
        get => tblItem.DetailsActive;
        set => tblItem.DetailsActive = value;
    }
    /// <summary>
    /// The row provider host of this module. Typically the <see cref="tblItem"/>
    /// </summary>
    public IRowProviderHost RowProviderHost => tblItem;
    
    
    /// <summary>
    /// Returns the "data State" of the module. It could be Insert, Edit or None.
    /// <para>The State remains Insert or Edit after the Insert() or Edit() is called. 
    /// A call to Commit() sets the State to Edit. </para>
    /// </summary>
    public DataMode State { get; protected set; } = DataMode.None;
    /// <summary>
    /// True while inserting, that is while Insert() executes.
    /// </summary>
    public bool IsInserting
    {
        get { return fIsInserting > 0; }
        protected set
        {
            if (value)
                fIsInserting++;
            else
                fIsInserting--;

            if (fIsInserting < 0)
                fIsInserting = 0;
        }
    }
    /// <summary>
    /// True while loading, that is while Edit() executes.
    /// </summary>
    public bool IsEditing
    {
        get { return fIsEditing > 0; }
        protected set
        {
            if (value)
                fIsEditing++;
            else
                fIsEditing--;

            if (fIsEditing < 0)
                fIsEditing = 0;
        }
    }
    /// <summary>
    /// True while deleting, that is while Delete() executes.
    /// </summary>
    public bool IsDeleting
    {
        get { return fIsDeleting > 0; }
        protected set
        {
            if (value)
                fIsDeleting++;
            else
                fIsDeleting--;

            if (fIsDeleting < 0)
                fIsDeleting = 0;
        }
    }
    /// <summary>
    /// True while commiting, that is while Commit() executes.
    /// </summary>
    public bool IsCommiting
    {
        get { return fIsCommiting > 0; }
        protected set
        {
            if (value)
                fIsCommiting++;
            else
                fIsCommiting--;

            if (fIsCommiting < 0)
                fIsCommiting = 0;
        }
    }
    
    /// <summary>
    /// Gets the variables of the module.
    /// </summary>
    public Dictionary<string, object> Variables
    {
        get => fVariables ??= new Dictionary<string, object>();
        protected set => fVariables = value;
    }
    
    /// <summary>
    /// Returns the first row of the tblItem.
    /// <para>WARNING: Valid only in insert and edit mode.</para>
    /// </summary>
    public virtual DataRow CurrentRow => tblItem.CurrentRow;
    /// <summary>
    /// Returns the value of the Id field of the tblItem
    /// </summary>
    public virtual object Id =>  CurrentRow != null ? CurrentRow[tblItem.KeyFields[0]] : DBNull.Value;
    /// <summary>
    /// Returns the id of the item the last Edit() operation has loaded
    /// </summary>
    public object LastEditedId { get; protected set; }
    /// <summary>
    /// Returns the Id of the last commit
    /// </summary>
    public object LastCommitedId { get; protected set; }
    /// <summary>
    /// Returns the Id of the last delete
    /// </summary>
    public object LastDeletedId { get; protected set; }
 
}

 
