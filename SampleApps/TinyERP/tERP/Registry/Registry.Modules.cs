namespace tERP;

static internal partial class Registry
{
    static void RegisterMasterModule_Log()
    {
        ModuleDef Module;
        TableDef tblTop;
        SelectDef SelectDef;
        string SqlText;
        string TableName = "SYS_LOG";
        
/*
    Id  @NVARCHAR(40)  @NOT_NULL primary key
   ,Year int @NOT_NULL
   ,Month int @NOT_NULL
   ,DayOfMonth int @NOT_NULL
   ,LogTime @NVARCHAR(20) @NOT_NULL
   ,User @NVARCHAR(96) @NOT_NULL
   ,Host @NVARCHAR(96) @NOT_NULL
   ,Level @NVARCHAR(96) @NOT_NULL
   ,Source @NVARCHAR(512) @NOT_NULL
   ,Scope @NVARCHAR(512) @NOT_NULL
   ,EventId @NVARCHAR(96) @NOT_NULL
   ,Message @NBLOB_TEXT(96) @NOT_NULL 
 */        
        
        SqlText = $@"
select
    Id          
   ,Year        
   ,Month       
   ,DayOfMonth  
   ,LogTime     
   ,User        
   ,Host        
   ,Level       
   ,Source      
   ,Scope       
   ,EventId     
from
    {TableName}  
";
        Module = DataRegistry.AddModule(Name: "Log", ClassName: "DataModule", ListSelectSql: SqlText, IsSingleSelect: true);
        Module.GuidOids = true;


        
        tblTop = Module.Table;
        tblTop.Name = TableName;

        tblTop.AddId();
        tblTop.AddInteger("Year");
        tblTop.AddInteger("Month");
        tblTop.AddInteger("DayOfMonth");
        tblTop.AddString("LogTime");
        tblTop.AddString("User");
        tblTop.AddString("Host");
        tblTop.AddString("Level");
        tblTop.AddString("Source");
        tblTop.AddString("Scope");
        tblTop.AddString("EventId");
        tblTop.AddTextBlob("Message").Flags |= FieldFlags.LargeMemo;

        // filters
        string[] FilterFields = ["Year", "Month", "DayOfMonth", "User", "Host", "Level", "Source", "EventId"];
        SelectDef = Module.SelectList[0];
        foreach (string FieldName in FilterFields)
            SelectDef.AddFilter(tblTop.GetField(FieldName));
 
    }
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
        RegisterMasterModule_Log();
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