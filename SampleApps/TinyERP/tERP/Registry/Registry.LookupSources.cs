namespace tERP;

static internal partial class Registry
{
    static void RegisterLookupSources_Enums()
    {
        //DataRegistry.AddLookupSource(typeof(TradeType));
        //DataRegistry.AddLookupSource(typeof(TradeStatus));
    }
    static void RegisterLookupSources_Tables()
    {
        //DataRegistry.AddLookupSource("Country");
        //DataRegistry.AddLookupSource("Category");
        
        List<TableItemDef> LookupTableItems = Db.LookupTableItemDefs.GetAllTables();
        foreach (TableItemDef Item in LookupTableItems)
            DataRegistry.AddLookupSource(Item.Name);
    }
    
    static public void RegisterLookupSources()
    {
        RegisterLookupSources_Tables();
        RegisterLookupSources_Enums();
    }
}