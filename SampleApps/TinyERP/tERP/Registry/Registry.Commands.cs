namespace tERP;

static internal partial class Registry
{
    static object ShowFormFunc(Command Cmd)
    {            
        TableItemDef TableItemDef = Cmd.Tag as TableItemDef;
        if (TableItemDef != null)
            return AppHost.ContentHandler.ShowDataForm(TableItemDef.Name);
        return null;
    }
    static Command RegisterLookupCommands()
    {
        Command cmdLookups = new ("Lookups");
        
        var Commands = Db.LookupTableItemDefs.AsCommandList().OrderBy(x => x.Name);
        cmdLookups.Commands.AddRange(Commands);
 
        foreach (Command Cmd in cmdLookups.Commands)
            Cmd.ExecuteFunc = ShowFormFunc;

        return cmdLookups;
    }

    static List<Command> RegisterMasterCommands()
    {
        List<Command> MasterCommandGroups = [];
        
        foreach (TableItemDefGroup MasterGroup in Db.MasterTableItemDefs.Groups)
        {
            Command cmdGroup = new() { Name = MasterGroup.Name, TitleKey = MasterGroup.TitleKey, Tag = null };
            MasterCommandGroups.Add(cmdGroup);
            
            var Commands = MasterGroup.AsCommandList().OrderBy(x => x.Name);
            cmdGroup.Commands.AddRange(Commands);
            
            foreach (Command Cmd in cmdGroup.Commands)
                Cmd.ExecuteFunc = ShowFormFunc;
        }

        return MasterCommandGroups;
    }
    
    static public void RegisterCommands()
    {
        // NOTE: ToolBar commands should define an ImageFileName.
        
        // ● commands  
        Command cmdExit = new ("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdAppFolder = new ("ShowAppFolder", "folder.png", (c) => { Sys.OpenFileExplorer(SysConfig.AppFolderPath); return 0; });
        Command cmdConnectionInfo = new ("ConnectionInfo", "database_edit.png", async (c) => {  await DbConnectionEditDialog.ShowModal(Db.GetDefaultConnectionInfo()); return 0; });
        Command cmdClearLog = new ("ClearLog", "bin.png", (c) => { LogBox.Clear(); return 0; });
        Command cmdLog = new ("Error Log", "error_log.png", (c) => { AppHost.ContentHandler.ShowDataForm("Log"); return 0; });
        
        //Command cmdCountries = new ("Countries", "globe_model.png", (c) => AppHost.ContentHandler.ShowDataForm("Country"));
        //Command cmdCustomers = new ("Customers", "user.png", (c) => AppHost.ContentHandler.ShowDataForm("Customer"));
        
        // ● General commands  
        Command cmdGeneral = new ("General");
        cmdGeneral.Commands.AddRange([cmdAppFolder, cmdConnectionInfo, cmdLog, cmdExit]);  
        
        // ● Lookup table commands
        Command cmdLookups = RegisterLookupCommands();
        
        // ● Master table commands  
        List<Command> MasterCommandGroups = RegisterMasterCommands();
        
        // ● split commands to toolbar and menu commands
        AppRegistry.ToolBarCommands.AddRange([cmdAppFolder, cmdConnectionInfo, cmdClearLog, cmdExit]);
        AppRegistry.MenuCommands.AddRange([cmdGeneral, cmdLookups]);
        AppRegistry.MenuCommands.AddRange(MasterCommandGroups);
    }
}