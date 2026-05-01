namespace DesktopApp;

static public partial class AppHost
{
    static void RegisterCommands()
    {
        Command cmdExit = new ("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdCountries = new ("Countries", "globe_model.png", (c) => ContentHandler.ShowDataForm("Country"));
        Command cmdCustomers = new ("Customers", "user.png", (c) => ContentHandler.ShowDataForm("Customer"));
        
        AppRegistry.ToolBarCommands.AddRange([cmdCountries, cmdCustomers, cmdExit]);
    }
    static void RegisterLookupSources()
    {
        DataRegistry.AddLookupSource(typeof(TradeType));
        DataRegistry.AddLookupSource(typeof(TradeStatus));
        
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
        
        // list modules
        DataRegistry.AddLookupListModule("Country");
        DataRegistry.AddLookupListModule("Category");
        
        // Customer
        Module = DataRegistry.AddModule("Customer"); 
 
        Table = Module.Table;
        Table.AddId().Flags |= FieldFlags.Visible;
        Table.AddString("Name").Flags |= FieldFlags.Required | FieldFlags.Visible;
        Table.AddStringLookupId("CountryId", "Country", TitleKey: "Country").Flags |= FieldFlags.Visible;  
    }
    static void RegisterForms()
    {
        FormDef FormDef;
        FormDef = DesktopRegistry.AddForm("Country", TitleKey: "Countries");
        FormDef.IsReadOnly = true;
        
        DesktopRegistry.AddForm("Category", TitleKey: "Categories");
        DesktopRegistry.AddForm("Customer", TitleKey: "Customers");
    }
}