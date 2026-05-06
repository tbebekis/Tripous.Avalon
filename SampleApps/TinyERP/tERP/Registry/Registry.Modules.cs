namespace tERP;

static internal partial class Registry
{
    static void RegisterMasterModule_Customer()
    {
        ModuleDef Module;
        TableDef tblTop;
        string SqlText;
        
        // Customer
        SqlText = $@"
select
    c.Id            as Id
    ,c.Name         as Customer
    ,co.Name        as Country    
from
    Customer c
        left join Country co on co.Id = c.CountryId
";
        Module = DataRegistry.AddModule("Customer", ListSelectSql: SqlText); 
 
        tblTop = Module.Table;
        tblTop.AddId().Flags |= FieldFlags.Visible;
        tblTop.AddString("Name").Flags |= FieldFlags.Required | FieldFlags.Visible;
        tblTop.AddStringLookupId("CountryId", "Country", TitleKey: "Country").Flags |= FieldFlags.Visible;  
    }
    static void RegisterMasterModule_Company()
    {
        ModuleDef Module;
        TableDef tblTop;
        string SqlText;
        
        SqlText = $@"
select
    Id
    ,Code
    ,Name
    ,Title
    ,TaxNumber
    ,Phone
from
    Company  
";
        Module = DataRegistry.AddModule("Company", ListSelectSql: SqlText); 
        
        tblTop = Module.Table;
        TableDef tblBranch = tblTop.AddDetail("CompanyBranch", "Id", "CompanyId");
        TableDef tblBankAccount = tblTop.AddDetail("CompanyBankAccount", "Id", "CompanyId");
    }
    
    static void RegisterModules_Lookups()
    {
        // list modules
        //DataRegistry.AddLookupListModule("Country");
        //DataRegistry.AddLookupListModule("Category");
        
        List<TableItemDef> LookupTableItems = Db.LookupTableItemDefs.GetAllTables();
        foreach (TableItemDef Item in LookupTableItems)
            DataRegistry.AddLookupListModule(Item.Name, TitleKey: Item.TitleKey);
    }
    static void RegisterModules_Masters()
    {
        RegisterMasterModule_Company();
    }
    static void RegisterModules_Transactions()
    {
        
    }
    
    static public void RegisterModules()
    {
        RegisterModules_Lookups();
        RegisterModules_Masters();
        RegisterModules_Transactions();
            

    }
}