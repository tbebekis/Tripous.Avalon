namespace tERP;

static public partial class AppHost
{
    static void RegisterCommands()
    {
        Command cmdExit = new ("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdCountries = new ("Countries", "globe_model.png", (c) => ContentHandler.ShowDataForm("Country"));
        Command cmdCustomers = new ("Customers", "user.png", (c) => ContentHandler.ShowDataForm("Customer"));
        
        AppRegistry.ToolBarCommands.AddRange([cmdCountries, cmdCustomers, cmdExit]);
        AppRegistry.MenuCommands.AddRange([cmdCountries, cmdCustomers, cmdExit]);
    }
    static void RegisterLookupSources()
    {
        //DataRegistry.AddLookupSource(typeof(TradeType));
        //DataRegistry.AddLookupSource(typeof(TradeStatus));
        
        DataRegistry.AddLookupSource("Country");
        DataRegistry.AddLookupSource("Category");
    }
    static void RegisterLocators()
    {
        // TODO: RegisterLocators()
    }
    static void RegisterModules()
    {
        ModuleDef Module;
        TableDef Table;
        string SqlText;
        
        // list modules
        DataRegistry.AddLookupListModule("Country");
        DataRegistry.AddLookupListModule("Category");
        
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
 
        Table = Module.Table;
        Table.AddId().Flags |= FieldFlags.Visible;
        Table.AddString("Name").Flags |= FieldFlags.Required | FieldFlags.Visible;
        Table.AddStringLookupId("CountryId", "Country", TitleKey: "Country").Flags |= FieldFlags.Visible;  
    }
    static void RegisterForms()
    {
        FormDef FormDef;
        FormDef = DesktopRegistry.AddForm("Country", TitleKey: "Countries");
        //FormDef.IsReadOnly = true;
        
        DesktopRegistry.AddForm("Category", TitleKey: "Categories");
        DesktopRegistry.AddForm("Customer", TitleKey: "Customers");
    }
}