namespace Tripous.Data;

public class DataModule
{

    // ● operation flags 
    protected int fInserting;
    protected int fLoading;
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
    /// <summary>
    /// Returns a bit field (set) of sql generation flags. Used when initializing Tables and
    /// their sql statements.
    /// </summary>
    protected virtual BuildSqlFlags GetBuildSqlFlags()
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
    /// <summary>
    /// Called from inside a commit transaction in order to assign the Code column
    /// </summary>
    protected virtual void AssignCodeValue(SqlStore Store, DbTransaction Transaction)
    {
        // TODO:
    }
 
    // ● construction
    public DataModule()
    {
        DataSet = new DataSet();
        tblList = new MemTable("List");
        tblItem = new MemTable("Item");
        DataSet.Tables.Add(tblList);
        DataSet.Tables.Add(tblItem);
    }
    
    // ● list
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
                throw new ApplicationException($"Cannot initialize {nameof(DataModule)}. No {nameof(DbConnectionInfo)} found");
 
            // ● SqlStore
            Store = SqlStores.CreateSqlStore(ConnectionInfo);
 
            // ● ensure that any TableDef is updated with the actual table schema from the database.
            ModuleDef.UpdateTableSchema(Store);
            
            // ● get the sql generation flags
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
                
                Table.Sqls.SelectSql = StockDef.SqlText;
                Table.Sqls.DisplayLabels = StockDef.DisplayLabels;
            }
            
            // ● TableSet
            TableSetFlags TableSetFlags = TableSetFlags.None;
 
            if (!ModuleDef.CascadeDeletes)
                TableSetFlags |= TableSetFlags.NoCascadeDeletes;
            
            TableSet = new TableSet(Store, tblItem, Stocks, TableSetFlags);

            TableSet.TransactionStageCommit += new EventHandler<TransactionStageEventArgs>(TableSet_TransactionStageCommit);
            TableSet.TransactionStageDelete += new EventHandler<TransactionStageEventArgs>(TableSet_TransactionStageDelete);
            
        }
        
    }
 
    // ● list
    public virtual void ListSelect(SelectDef SelectDef)
    {        
        if (SelectDef != null)
        {
            TableSet.ListSelect(tblList, SelectDef.SqlText);
        }
    }
    public virtual void ListSave()
    {
    }
 
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
            tblItem.ClearAll();
            tblItem.AddNewRow();
        }
        finally
        {
            State = DataMode.Insert;
            Inserting = false;
        }
    }
    /// <summary>
    /// Starts an edit operation. Valid with master brokers only.
    /// </summary>
    public virtual void Edit(object RowId)
    {
    }
    /// <summary>
    /// Deletes a row. Valid with master brokers only.
    /// </summary>
    public virtual void Delete(object RowId)
    {
    }
    /// <summary>
    /// Commits changes after an insert or edit. Valid with master brokers only.
    /// <para>Returns the row id of the tblItem commited row.</para>
    /// </summary>
    public virtual object Commit(bool Reselect = false)
    {
        return null;
    }
    /// <summary>
    /// Cancels changes after an insert or edit. Valid with master brokers only.
    /// </summary>
    public virtual void Cancel()
    {
    }
    
    // ● item checks
    /// <summary>
    /// Called by the <see cref="Insert"/> and throws an exception if, for some reason,
    /// starting an insert operation is considered invalid.
    /// </summary>
    public virtual void CheckCanInsert()
    {
        if (IsListModule)
            throw new ApplicationException("Can not insert item in a list module.");
    }
    /// <summary>
    /// Called by the <see cref="Edit"/> and throws an exception if, for some reason,
    /// starting an edit operation is considered invalid.
    /// </summary>
    public virtual void CheckCanEdit(object RowId)
    {
    }
    /// <summary>
    /// Called by the <see cref="Delete"/> and throws an exception if, for some reason,
    /// deleting the row in the database is considered invalid.
    /// </summary>
    public virtual void CheckCanDelete(object RowId)
    {
    }
    /// <summary>
    /// Called by the <see cref="Commit"/> and throws an exception if, for some reason,
    /// commiting item is considered invalid.
    /// </summary>
    public virtual void CheckCanCommit(bool Reselect)
    {
    }

    public bool TableExists(string TableName) => FindTable(TableName) != null;
    public MemTable FindTable(string TableName) => Tables.FirstOrDefault(x => TableName.IsSameText(x.TableName)); 
    public MemTable GetTable(string TableName)
    {
        MemTable Result = FindTable(TableName);

        if (Result == null)
            throw new ApplicationException($"Table {TableName} not found.");

        return Result;
    }
 
    public void SetAutoGenerateGuidKeys(bool Value)
    {
        foreach (DataTable Table in DataSet.Tables)
            (Table as MemTable).AutoGenerateGuidKeys = Value;
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
    /// True if this is a list broker
    /// </summary>
    public bool IsListModule => ModuleDef.IsListModule;
    /// <summary>
    /// True if this is a master broker.
    /// </summary>
    public bool IsMasterModule => !IsListModule;
    
    /// <summary>
    /// Returns the "data State" of the broker. It could be Insert, Edit or None.
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
    public bool Loading
    {
        get { return fLoading > 0; }
        protected set
        {
            if (value)
                fLoading++;
            else
                fLoading--;

            if (fLoading < 0)
                fLoading = 0;
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
    /// Gets the variables of the broker.
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