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
 
    
    static void RegisterLookups()
    {
        
    }
    static void RegisterLookupSources()
    {
        
    }
    static void RegisterLocators()
    {
        
    }
    static void RegisterModules()
    {
        
    }
    static void RegisterForms()
    {
        
    }
}