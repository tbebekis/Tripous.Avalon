namespace Tripous.Data;

public class DataLinkLocal: DataLink
{
    private TableSet TableSet;
    
    public override async Task Initialize(string ModuleName)
    {
        if (!IsInitialized)
        {
            await Task.CompletedTask;
            IsInitialized = true;
            //this.ModuleDef = Db.DataRegistryLink.Modules.Get(ModuleName);  
            
            // TODO: EDW - εδώ πρέπει να γίνουν όλα.
            
        }
    }

    public override async Task Insert()
    {
        await Task.CompletedTask;
        TableSet.ProcessInsert();
    }
    public override async Task Load(object RowId)
    {
        await Task.CompletedTask;
        TableSet.Load(RowId);
    }
    public override async Task Delete(object RowId)
    {
        await Task.CompletedTask;
        TableSet.Delete(RowId);
    }
    public override async Task<object> Commit(bool Reselect)
    {
        await Task.CompletedTask;
        return TableSet.Commit(Reselect);
    }
    public override async Task<int> ListSelect(SelectDef SelectDef)
    {
        await Task.CompletedTask;
        return TableSet.ListSelect(tblList, SelectDef.SqlText);
    }
}