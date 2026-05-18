namespace DesktopApp;

static public partial class AppHost
{
    static void RegisterCommands()
    {
        Command cmdExit = Command.Create("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdCountries = Command.Create("Countries", "globe_model.png", (c) => ContentHandler.ShowDataForm("Country"));
        Command cmdCustomers = Command.Create("Customers", "user.png", (c) => ContentHandler.ShowDataForm("Customer"));
        
        AppRegistry.ToolBarCommands.AddRange([cmdCountries, cmdCustomers, cmdExit]);
    }
    static void RegisterLookupSources()
    {
        DataRegistry.AddLookupSource(typeof(TradeType));
        DataRegistry.AddLookupSource(typeof(TradeStatus));
        
        DataRegistry.AddLookupWithTableName("Country", "Country");
        DataRegistry.AddLookupWithTableName("Category");
    }
    static void RegisterLocators()
    {
        // TODO: RegisterLocators()
    }
    static void RegisterModules()
    {
        ModuleDef Module;
        TableDef tblTop;
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
 
        tblTop = Module.Table;
        tblTop.AddId().Flags |= FieldFlags.Visible;
        tblTop.AddString("Name").Flags |= FieldFlags.Required | FieldFlags.Visible;
        //Table.AddStringLookupId("CountryId", "Country", TitleKey: "Country").Flags |= FieldFlags.Visible;  

        TableDef JoinTable = tblTop.AddJoin("CountryId", "Country");
        JoinTable.AddId().Flags |= FieldFlags.Visible;
        JoinTable.AddString("Name").Flags |= FieldFlags.Visible;
        
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