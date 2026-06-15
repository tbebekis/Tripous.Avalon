/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class AppHost
{
    /// <summary>
    /// Returns the physical file path of the default SQLite database.
    /// </summary>
    static string GetDefaultDatabaseFilePath()
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        if (ConnectionInfo.DbServerType != DbServerType.Sqlite)
            throw new TripousException("Database regeneration is supported only for SQLite connections.");

        ConnectionStringBuilder Builder = new ConnectionStringBuilder(ConnectionInfo.ConnectionString);
        string Result = Builder.Database;
        Result = ConnectionStringBuilder.ReplacePathPlaceholders(Result);
        return Result;
    }
    /// <summary>
    /// Deletes the sample SQLite database and terminates the application so it can be recreated on next startup.
    /// </summary>
    static async Task RegenerateDatabase()
    {
        string DatabaseFilePath = GetDefaultDatabaseFilePath();
        string Message = $"This will delete and recreate the sample Sqlite database.{Environment.NewLine}{Environment.NewLine}{DatabaseFilePath}{Environment.NewLine}{Environment.NewLine}Continue?";
        bool Flag = await MessageBox.YesNo(Message, AppHost.MainWindow);
        if (!Flag)
            return;

        System.Data.SQLite.SQLiteConnection.ClearAllPools();

        if (File.Exists(DatabaseFilePath))
            File.Delete(DatabaseFilePath);

        await MessageBox.Info("The sample Sqlite database has been deleted. The application will now terminate. Please restart the application.", AppHost.MainWindow);
        AppHost.MainWindow.Close();
    }
    static object ShowFormFunc(Command Cmd)
    {            
        //FormDef FormDef = DesktopRegistry.Forms.Get(Cmd.Form);
        return AppHost.ContentHandler.ShowDataForm(Cmd.Form);
    }
    /// <summary>
    /// Opens the demo dashboard.
    /// </summary>
    static object ShowDashboardFunc(Command Cmd)
    {
        FormContext Context = FormContext.Create("Dashboard", typeof(DashboardForm).FullName, FormDisplayMode.TabItem, AppHost.MainWindow);
        Context.Title = "Dashboard";
        return AppHost.ContentHandler.ShowAppForm(Context);
    }
 
    static public void RegisterCommands()
    {
        // NOTE: ToolBar commands should define an ImageFileName.
        
        // ● commands  
        Command cmdDashboard = Command.Create("Dashboard", "chart_bar.png", ShowDashboardFunc);
        Command cmdExit = Command.Create("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdAppFolder = Command.Create("ShowAppFolder", "folder.png", (c) => { Sys.OpenFileExplorer(SysConfig.AppFolderPath); return 0; });
        Command cmdApplicationSettings = Command.CreateAsync("Application Settings", "setting_tools.png", async (c) => { await ConfigDialog.ShowModal(AppHost.MainWindow); return 0; });
        Command cmdConnectionInfo = Command.CreateAsync("ConnectionInfo", "database_edit.png", async (c) => { await ShowDbConnectionEditDialog(Db.GetDefaultConnectionInfo()); return 0; });
        Command cmdRegenerateDatabase = Command.CreateAsync("Regenerate Database", "database_refresh.png", async (c) => { await RegenerateDatabase(); return 0; });
        Command cmdClearLog = Command.Create("Clear Log", "bin.png", (c) => { LogBox.Clear(); return 0; });
        Command cmdToggleLog = Command.Create("Toggle Log", "error_log.png", (c) => { AppHost.MainWindow.ToggleLog(); return 0; });
        Command cmdToggleLogSqlStatements = Command.Create("Log Sql", "file_extension_log.png", (c) => { AppHost.MainWindow.ToggleLogSqlStatements(); return 0; });
        cmdToggleLogSqlStatements.IsToggle = true;
        Command cmdTest = Command.Create("Test", "lightning.png");
        
        // ● General commands  
        Command cmdGeneral = new ("General");
        cmdGeneral.Commands.AddRange([cmdDashboard, cmdAppFolder, cmdApplicationSettings, cmdConnectionInfo, cmdRegenerateDatabase, cmdExit]);

        // ● form commands  
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command cmdGroup = AppRegistry.FindCommand(FormDef.Group);
            if (cmdGroup == null)
            {
                cmdGroup = new Command(FormDef.Group);
                AppRegistry.MenuCommands.Add(cmdGroup);
            }

            Command Cmd = FormDef.CreateShowCommand(ShowFormFunc);
            cmdGroup.Commands.Add(Cmd);
        }
        RegisterReadOnlyViewCommands();
        AppRegistry.MenuCommands.Sort();
        AppRegistry.MenuCommands.Insert(0, cmdGeneral);
        
        // ● split commands to toolbar and menu commands
        AppRegistry.ToolBarCommands.AddRange([cmdDashboard, cmdAppFolder, cmdApplicationSettings, cmdConnectionInfo, cmdRegenerateDatabase, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdTest, cmdExit]);
        //AppRegistry.MenuCommands.AddRange(MasterCommandGroups);
    }
}
