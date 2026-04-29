namespace Tripous.Data;

public class DataModule
{
    // ● operation flags 
    protected int fInserting;
    protected int fEditing;
    protected int fDeleting;
    protected int fCommiting;

    protected Dictionary<string, object> fVariables;
    protected TableSet TableSet;
    
    // ● tableset event handlers  
    /// <summary>
    /// Gets a notification from the TableSet when deleting
    /// </summary>
    protected virtual void TableSet_TransactionStageDelete(object sender, TransactionStageEventArgs e)
    {
    }
    /// <summary>
    /// Gets a notification from the TableSet when commiting
    /// </summary>
    protected virtual void TableSet_TransactionStageCommit(object sender, TransactionStageEventArgs e)
    {
        // ** in web applications we do NOT have TableSet state
        if (/*(TableSet.IsInsert) && */(e.Stage == TransactionStage.Start) && (e.ExecTime == ExecTime.After))
        {
            AssignCodeValue(e.Store, e.Transaction);
        }
    }
 
    // ● overridables
    protected virtual void AcceptChanges() => DataSet.AcceptChanges();
    protected virtual void RejectChanges() => DataSet.RejectChanges();
    
    /// <summary>
    /// Called from inside a commit transaction in order to assign the Code column
    /// </summary>
    protected virtual void AssignCodeValue(SqlStore Store, DbTransaction Transaction)
    {
        // TODO:
    }
 
    /// <summary>
    /// Sets default values for all Tables.
    /// <para>It is called by the DoInsertAfter() and DoCommitBefore() </para>
    /// </summary>
    protected virtual void SetDefaultValues()
    {
        if (this.State == DataMode.Insert || (IsListModule && Commiting))
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
    /// Sets default values to the Table. It is called when an commit operation starts.
    /// </summary>
    protected virtual void SetDefaultValues(DataTable Table, TableDef TableDef)
    {
        if (this.State == DataMode.Insert || (IsListModule && Commiting))
        {
            foreach (DataRow Row in Table.Rows)
                SetDefaultValues(Table, Row, TableDef);
        }
    }
    /// <summary>
    /// Sets default values to the Row. It is called when an commit operation starts.
    /// </summary>
    protected virtual void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        if (Row.RowState == DataRowState.Deleted)
            return;

        if (ModuleDef.Table.Name.IsSameText(Row.Table.TableName))
        {
            if (IsListModule)
            {
                if (!Commiting)
                    return;
            }
            else if (this.State != DataMode.Insert)
            {
                return;
            }
        }

        bool Flag = (this.State == DataMode.Insert || (IsListModule && Commiting)) && (Row.RowState != DataRowState.Deleted);


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
                        if (Sys.IsSameText(SysConfig.CompanyFieldName, Column.ColumnName)) // ColumnName is CompanyId
                            Row[Column] = SysConfig.CompanyId;
                    }
                }

            }

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
    
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public DataModule()
    {
        DataSet = new DataSet();
        tblList = new MemTable("List");
        tblItem = new MemTable("Item");
        DataSet.Tables.Add(tblList);
        DataSet.Tables.Add(tblItem);
    }
    
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
                throw new ArgumentNullException(nameof(ModuleDef));
            
            this.ModuleDef = ModuleDef;
            ModuleDef.UpdateReferences();
            
            // ● Connection Info
            DbConnectionInfo ConnectionInfo = Db.GetConnectionInfo(ModuleDef.ConnectionName);

            if (ConnectionInfo == null)
                throw new DataModuleException($"Cannot initialize {nameof(DataModule)}. No {nameof(DbConnectionInfo)} found");
 
            // ● SqlStore
            Store = SqlStores.CreateSqlStore(ConnectionInfo);
 
            // ● ensure that any TableDef is updated with the actual table schema from the database.
            ModuleDef.UpdateTableSchema(Store);
            
            // ● get the sql generation flags
            // -----------------------------------------------------------
            BuildSqlFlags GetBuildSqlFlags()
            {
                BuildSqlFlags Result = BuildSqlFlags.None;

                if (ModuleDef.GuidOids)
                    Result |= BuildSqlFlags.GuidOids;
                else if (Store.Provider.OidMode == OidMode.Generator)
                    Result |= BuildSqlFlags.OidModeIsBefore;

                if (IsListModule)
                    Result |= BuildSqlFlags.IncludeBlobFields;

                return Result;
            }
            // -----------------------------------------------------------
            BuildSqlFlags SqlFlags = GetBuildSqlFlags();
            
            DataSet = new DataSet("DS_" + ModuleDef.Name);
            
            // ● for all Tables of the module definition
            // 1. create sql statements 
            // 2. create DataTable objects     
            List<TableDef> TableDefs = ModuleDef.GetTables();
            MemTable Table;
            TableSqls Sqls;
            foreach (var TableDef in TableDefs)
            {
                Table = TableDef.CreateDescriptorTable(Store);  // TableDef.CreateDescriptorTable(Store, table => DataSet.Tables.Add(table));
                Sqls = TableDef.BuildSql(SqlFlags);
                Table.Sqls.AssignFrom(Sqls);
                Table.AutoGenerateGuidKeys = ModuleDef.GuidOids;
                DataSet.Tables.Add(Table);
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
            
            // ● TableSet
            TableSetFlags TableSetFlags = TableSetFlags.None;
 
            if (!ModuleDef.CascadeDeletes)
                TableSetFlags |= TableSetFlags.NoCascadeDeletes;
            
            TableSet = new TableSet(Store, tblList, tblItem, Stocks, TableSetFlags);

            TableSet.TransactionStageCommit += new EventHandler<TransactionStageEventArgs>(TableSet_TransactionStageCommit);
            TableSet.TransactionStageDelete += new EventHandler<TransactionStageEventArgs>(TableSet_TransactionStageDelete);
        }
        
    }
 
    // ● list
    /// <summary>
    /// Selects the list table.
    /// </summary>
    public virtual void ListSelect(SelectDef SelectDef)
    {        
        if (SelectDef != null)
        {
            TableSet.ListSelect(tblList, SelectDef.SqlText);
        }
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
        CheckCanInsert();

        Inserting = true;
        try
        {
            TableSet.ItemProcessInsert();
            SetDefaultValues();
        }
        finally
        {
            State = DataMode.Insert;
            Inserting = false;
        }
    }
    /// <summary>
    /// Starts an edit operation. Valid with master modules only.
    /// </summary>
    public virtual void Edit(object RowId)
    {
        
        CheckCanEdit(RowId);

        Editing = true;
        try
        {
            TableSet.ItemLoad(RowId);
            LastEditedId = RowId;
            AcceptChanges();
        }
        finally
        {
            State = DataMode.Edit;
            Editing = false;
        }
    }
    /// <summary>
    /// Deletes a row. Valid with master modules only.
    /// </summary>
    public virtual void Delete(object RowId)
    {        
        CheckCanDelete(RowId);

        Deleting = true;
        try
        {
            TableSet.ItemDelete(RowId);
            LastDeletedId = RowId;
            AcceptChanges();
        }
        finally
        {
            State = DataMode.None;
            Deleting = false;
        }
    }
    /// <summary>
    /// Commits changes after an insert or edit. Valid with master modules only.
    /// <para>Returns the row id of the tblItem commited row.</para>
    /// </summary>
    public virtual object Commit(bool Reselect = false)
    {
        object Result = null;
         
        EndEdit();
        SetDefaultValues();
        EndEdit();
        
        CheckCanCommit(Reselect);

        Commiting = true;
        try
        {
            Result = TableSet.ItemCommit(Reselect);
            LastCommitedId = Result;
            AcceptChanges();
        }
        finally
        {
            State = DataMode.Edit;
            Commiting = false;
        }

        return Result;
    }
    /// <summary>
    /// Rejects the changes after an insert or edit. Valid with master modules only.
    /// </summary>
    public virtual void Cancel()
    {
        TableSet.ItemProcessCancel();
        RejectChanges();
        State = DataMode.Edit;
    }
    
    // ● item checks
    /// <summary>
    /// Called by the <see cref="Insert"/> and throws an exception if, for some reason,
    /// starting an insert operation is considered invalid.
    /// </summary>
    public virtual void CheckCanInsert()
    {
        if (IsListModule)
            throw new DataModuleException("Can not insert item in a list module.");
    }
    /// <summary>
    /// Called by the <see cref="Edit"/> and throws an exception if, for some reason,
    /// starting an edit operation is considered invalid.
    /// </summary>
    public virtual void CheckCanEdit(object RowId)
    {
        if (IsListModule)
            throw new DataModuleException("Can not edit item in a list module.");
        
        if (Sys.IsNull(RowId))
            throw new DataModuleException("Can not edit item. Invalid RowId");
    }
    /// <summary>
    /// Called by the <see cref="Delete"/> and throws an exception if, for some reason,
    /// deleting the row in the database is considered invalid.
    /// </summary>
    public virtual void CheckCanDelete(object RowId)
    {
        if (IsListModule)
            throw new DataModuleException("Can not delete item in a list module.");

        if (Sys.IsNull(RowId))
            throw new DataModuleException("Can not delete item. Invalid RowId");
    }
    /// <summary>
    /// Called by the <see cref="Commit"/> and throws an exception if, for some reason,
    /// commiting item is considered invalid.
    /// </summary>
    public virtual void CheckCanCommit(bool Reselect)
    {
        if (IsListModule)
            throw new DataModuleException("Can not commit item in a list module.");
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
    public bool IsInitialized => ModuleDef != null;
    public ModuleDef ModuleDef { get; protected set; }
    public SqlStore Store { get; protected set; }
    public MemTable this[string TableName] => GetTable(TableName);
    public DataSet DataSet { get; protected set; }
    public MemTable tblList { get; protected set; }
    public MemTable tblItem { get; protected set; }
    public IEnumerable<MemTable> Tables => DataSet.Tables.Cast<MemTable>();
    public List<MemTable> Stocks => new();
    public string Name => ModuleDef.Name;
    public bool DetailsActive
    {
        get => tblItem.DetailsActive;
        set => tblItem.DetailsActive = value;
    }

    /// <summary>
    /// True if this is a list module
    /// </summary>
    public bool IsListModule => ModuleDef.IsListModule;
    /// <summary>
    /// True if this is a master module.
    /// </summary>
    public bool IsMasterModule => !IsListModule;
    
    /// <summary>
    /// Returns the "data State" of the module. It could be Insert, Edit or None.
    /// <para>The State remains Insert or Edit after the Insert() or Edit() is called. 
    /// A call to Commit() sets the State to Edit. </para>
    /// </summary>
    public DataMode State { get; protected set; } = DataMode.None;
    /// <summary>
    /// True while inserting, that is while Insert() executes.
    /// </summary>
    public bool Inserting
    {
        get { return fInserting > 0; }
        protected set
        {
            if (value)
                fInserting++;
            else
                fInserting--;

            if (fInserting < 0)
                fInserting = 0;
        }
    }
    /// <summary>
    /// True while loading, that is while Edit() executes.
    /// </summary>
    public bool Editing
    {
        get { return fEditing > 0; }
        protected set
        {
            if (value)
                fEditing++;
            else
                fEditing--;

            if (fEditing < 0)
                fEditing = 0;
        }
    }
    /// <summary>
    /// True while deleting, that is while Delete() executes.
    /// </summary>
    public bool Deleting
    {
        get { return fDeleting > 0; }
        protected set
        {
            if (value)
                fDeleting++;
            else
                fDeleting--;

            if (fDeleting < 0)
                fDeleting = 0;
        }
    }
    /// <summary>
    /// True while commiting, that is while Commit() executes.
    /// </summary>
    public bool Commiting
    {
        get { return fCommiting > 0; }
        protected set
        {
            if (value)
                fCommiting++;
            else
                fCommiting--;

            if (fCommiting < 0)
                fCommiting = 0;
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
    public virtual DataRow Row => tblItem.CurrentRow;
    /// <summary>
    /// Returns the value of the Id field of the tblItem
    /// </summary>
    public virtual object Id =>  Row != null ? Row[tblItem.KeyFields[0]] : DBNull.Value;
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