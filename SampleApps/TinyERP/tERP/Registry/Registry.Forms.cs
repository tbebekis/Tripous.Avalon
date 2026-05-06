namespace tERP;

static internal partial class Registry
{
    static void RegisterForms_Lookups()
    {
        List<TableItemDef> LookupTableItems = Db.LookupTableItemDefs.GetAllTables();
        foreach (TableItemDef Item in LookupTableItems)
            DesktopRegistry.AddForm(Item.Name, TitleKey: Item.TitleKey);
    }
    static void RegisterForms_Masters()
    {
        DesktopRegistry.AddForm("Company", TitleKey: "Company");
    }
    static void RegisterForms_Transactions()
    {
        
    }
    
    static public void RegisterForms()
    {
        RegisterForms_Lookups();
        RegisterForms_Masters();
        RegisterForms_Transactions();
        
        /*
        FormDef FormDef;
        
        FormDef = DesktopRegistry.AddForm("Country", TitleKey: "Countries");
        //FormDef.IsReadOnly = true;
        
        DesktopRegistry.AddForm("Category", TitleKey: "Categories");
        DesktopRegistry.AddForm("Customer", TitleKey: "Customers");
        */
    }
}