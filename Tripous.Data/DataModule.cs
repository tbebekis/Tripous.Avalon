namespace Tripous.Data;

public class DataModule
{
    // ● operation flags 
    /// <summary>
    /// Field
    /// </summary>
    protected int fInserting;
    /// <summary>
    /// Field
    /// </summary>
    protected int fLoading;
    /// <summary>
    /// Field
    /// </summary>
    protected int fDeleting;
    /// <summary>
    /// Field
    /// </summary>
    protected int fCommiting;

    private TableSet TableSet;
    
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

    public MemTable GetTable(string TableName)
    {
        MemTable Result = FindTable(TableName);

        if (Result == null)
            throw new ApplicationException($"Table {TableName} not found.");

        return Result;
    }
    public bool TableExists(string TableName)
    {
        return FindTable(TableName) != null;
    }
    public MemTable FindTable(string TableName)
    {
        foreach (DataTable Table in DataSet.Tables)
            if (string.Compare(TableName, Table.TableName, true) == 0)
                return Table as MemTable;
        return null;
    }
    public void SetAutoGenerateGuidKeys(bool Value)
    {
        foreach (DataTable Table in DataSet.Tables)
            (Table as MemTable).AutoGenerateGuidKeys = Value;
    }
    
    // ● properties
 
    public ModuleDef ModuleDef { get; set; }
    public MemTable this[string TableName] => GetTable(TableName);
    public DataSet DataSet { get; }
    public MemTable tblList { get; }
    public MemTable tblItem { get; }
    public string Name => ModuleDef.Name;
    public bool DetailsActive
    {
        get => tblItem.DetailsActive;
        set => tblItem.DetailsActive = value;
    }
    
    /// <summary>
    /// True if this is a list broker
    /// </summary>
    public virtual bool IsListModule { get; protected set; }
    /// <summary>
    /// True if this is a master broker.
    /// </summary>
    public virtual bool IsMasterModule { get { return !IsListModule; } }
    
    /// <summary>
    /// True while broker is in the initialization phase.
    /// </summary>
    public bool Initializing { get; protected set; }
    /// <summary>
    /// True after the broker is Initialized.
    /// </summary>
    public bool Initialized { get; protected set; }    
    
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
    public Dictionary<string, object> Variables { get; protected set; } = new Dictionary<string, object>();
    
    /// <summary>
    /// Returns the first row of the tblItem.
    /// <para>WARNING: Valid only in insert and edit mode.</para>
    /// </summary>
    public virtual DataRow Row => tblItem.CurrentRow;
    /// <summary>
    /// Returns the value of the Id field of the tblItem
    /// </summary>
    public virtual object Id { get { return Row != null ? Row[tblItem.KeyFields[0]] : DBNull.Value; } }
    /// <summary>
    /// Returns the id of the item the last Edit() operation has loaded
    /// </summary>
    public virtual object LastEditedId { get; protected set; }
    /// <summary>
    /// Returns the Id of the last commit
    /// </summary>
    public virtual object LastCommitedId { get; protected set; }
    /// <summary>
    /// Returns the Id of the last delete
    /// </summary>
    public virtual object LastDeletedId { get; protected set; }
}