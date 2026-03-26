namespace Tripous.Data;

public abstract class DataBroker
{
    // ● list
    public abstract void ListSelect(DataModule Module, object SelectDef);
    public abstract void ListSave(DataModule Module);
 
    // ● item
    public abstract object Save(DataModule Module, bool ReLoad = false);
    public abstract void Load(DataModule Module, object RowId);
    public abstract void Delete(DataModule Module, object RowId);
}