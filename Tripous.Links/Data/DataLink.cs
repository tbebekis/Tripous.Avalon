namespace Tripous.Data;

public abstract class DataLink
{
    // ● construction
    public DataLink()
    {
    }
    
    // ● public
    /// <summary>
    /// Initializes this instance
    /// <code>
    /// RegistryLink.GetModuleDef(ModuleName) 
    /// → ModuleDef.Create()
    /// → DataLink.Initialize(ModuleDef ModuleDef)
    /// → DataLink δημιουργεί/φορτώνει ItemTable/ListTable
    /// → DataModule παίρνει references από DataLink
    /// </code>
    /// </summary>
    public abstract Task Initialize(string ModuleName);
    
    public abstract Task Insert();
    public abstract Task Load(object RowId);
    public abstract Task Delete(object RowId);
    public abstract Task<object> Commit(bool Reselect);
    public abstract Task<int> ListSelect(SelectDef SelectDef);
    
    // ● properties
    public virtual bool IsInitialized { get; protected set; }
    
    public virtual ModuleDef ModuleDef { get; protected set; }
    public virtual MemTable tblItem { get; protected set; }
    public virtual MemTable tblList { get; protected set; }
}












 

 