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
    /// Returns true when the current user may access a command.
    /// </summary>
    static bool CanAccess(Command Command)
    {
        return Command.CanAccess(Sys.Context.CurrentUser);
    }
    /// <summary>
    /// Returns the physical file path of the default SQLite database.
    /// </summary>
    static string GetDefaultDatabaseFilePath()
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        if (ConnectionInfo.DbServerType != DbServerType.Sqlite)
            throw new TripousException(Texts.L("DatabaseRegenerationOnlySqlite", "Database regeneration is supported only for SQLite connections."));

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
        string Message = string.Format(
            Texts.L("ConfirmRegenerateDatabase", "This will delete and recreate the sample Sqlite database.{0}{0}{1}{0}{0}Continue?"),
            Environment.NewLine,
            DatabaseFilePath);
        bool Flag = await MessageBox.YesNo(Message, AppHost.MainWindow);
        if (!Flag)
            return;

        System.Data.SQLite.SQLiteConnection.ClearAllPools();

        if (File.Exists(DatabaseFilePath))
            File.Delete(DatabaseFilePath);

        await MessageBox.Info(Texts.L("DatabaseDeletedApplicationWillTerminate", "The sample Sqlite database has been deleted. The application will now terminate. Please restart the application."), AppHost.MainWindow);
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
        Context.Title = Texts.L("Dashboard", "Dashboard");
        return AppHost.ContentHandler.ShowAppForm(Context);
    }
    /// <summary>
    /// Opens the resource translation editor.
    /// </summary>
    static object ShowResourceTranslationsFunc(Command Cmd)
    {
        FormContext Context = FormContext.Create("ResourceTranslations", typeof(ResourceTranslationsForm).FullName, FormDisplayMode.TabItem, AppHost.MainWindow);
        Context.Title = Texts.L("ResourceTranslations", "Resource Translations");
        return AppHost.ContentHandler.ShowAppForm(Context);
    }
    /// <summary>
    /// Selects the database explorer.
    /// </summary>
    static object ShowDatabaseWorkbenchFunc(Command Cmd)
    {
        AppForm Result = AppHost.ShowDatabaseExplorer();
        Result?.SetAsSelectedForm();
        return Result;
    }
    /// <summary>
    /// Changes the current user password.
    /// </summary>
    static async Task ChangePassword()
    {
        AppUser User = Sys.Context.CurrentUser;
        if (User == null)
            return;
        ChangePasswordDialog Dialog = await ChangePasswordDialog.ShowModal(User.UserName, AppHost.MainWindow);
        if (!Dialog.Result)
            return;
        try
        {
            AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
            Module.ChangePassword(User.UserName, Dialog.CurrentPassword, Dialog.NewPassword);
            await MessageBox.Info(Texts.L("PasswordChanged", "Password changed."), AppHost.MainWindow);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, AppHost.MainWindow);
        }
    }
 
    static public void RegisterCommands()
    {
        // NOTE: ToolBar commands should define an ImageFileName.
        
        // ● commands  
        Command cmdDashboard = Command.Create("Dashboard", "chart_bar.png", ShowDashboardFunc, "Dashboard");
        Command cmdExit = Command.Create("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdAppFolder = Command.Create("ShowAppFolder", "folder.png", (c) => { Sys.OpenFileExplorer(SysConfig.AppFolderPath); return 0; }, "ShowAppFolder");
        Command cmdApplicationSettings = Command.CreateAsync("Application Settings", "setting_tools.png", async (c) => { await ConfigDialog.ShowModal(AppHost.MainWindow); return 0; }, "ApplicationSettings");
        Command cmdChangePassword = Command.CreateAsync("Change Password", "change_password.png", async (c) => { await ChangePassword(); return 0; }, "ChangePassword");
        Command cmdConnectionInfo = Command.CreateAsync("ConnectionInfo", "database_edit.png", async (c) => { await ShowDbConnectionEditDialog(Db.GetDefaultConnectionInfo()); return 0; }, "ConnectionInfo");
        Command cmdDatabaseWorkbench = Command.Create("Database Explorer", "database.png", ShowDatabaseWorkbenchFunc, "DatabaseExplorer");
        Command cmdResourceTranslations = Command.Create("Resource Translations", "language.png", ShowResourceTranslationsFunc, "ResourceTranslations");
        Command cmdRegenerateDatabase = Command.CreateAsync("Regenerate Database", "database_refresh.png", async (c) => { await RegenerateDatabase(); return 0; }, "RegenerateDatabase");
        cmdConnectionInfo.SecurityLevel = UserLevel.Admin;
        cmdDatabaseWorkbench.SecurityLevel = UserLevel.Admin;
        cmdResourceTranslations.SecurityLevel = UserLevel.Admin;
        cmdRegenerateDatabase.SecurityLevel = UserLevel.Admin;
        Command cmdClearLog = Command.Create("Clear Log", "bin.png", (c) => { LogBox.Clear(); return 0; }, "ClearLog");
        Command cmdToggleLog = Command.Create("Toggle Log", "error_log.png", (c) => { AppHost.MainWindow.ToggleLog(); return 0; }, "ToggleLog");
        Command cmdToggleLogSqlStatements = Command.Create("Log Sql", "file_extension_log.png", (c) => { AppHost.MainWindow.ToggleLogSqlStatements(); return 0; }, "LogSql");
        cmdToggleLogSqlStatements.IsToggle = true;
        Command cmdTest = Command.Create("Test", "lightning.png", "Test");
        
        // ● General commands  
        Command cmdGeneral = new ("General") { TitleKey = "General" };
        cmdGeneral.Commands.AddRange(new Command[] { cmdDashboard, cmdAppFolder, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdDatabaseWorkbench, cmdResourceTranslations, cmdRegenerateDatabase, cmdExit }.Where(CanAccess));

        // ● form commands  
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command cmdGroup = AppRegistry.FindCommand(FormDef.Group);
            if (cmdGroup == null)
            {
                cmdGroup = new Command(FormDef.Group);
                AppRegistry.MenuCommands.Add(cmdGroup);
            }

            Command Cmd = FormDef.CreateShowCommand(ShowFormFunc, ImageFileName: "table.png");
            ModuleDef ModuleDef = DataRegistry.Modules.Find(FormDef.Module);
            if (ModuleDef != null && ModuleDef.SecurityLevel != UserLevel.None)
                Cmd.SecurityLevel = ModuleDef.SecurityLevel;
            if (CanAccess(Cmd))
                cmdGroup.Commands.Add(Cmd);
        }
        RegisterReadOnlyViewCommands();
        foreach (Command Command in AppRegistry.MenuCommands.ToArray())
        {
            if (Command.HasChildren && Command.Commands.Count == 0)
                AppRegistry.MenuCommands.Remove(Command);
        }
        AppRegistry.MenuCommands.Sort();
        AppRegistry.MenuCommands.Insert(0, cmdGeneral);
        
        // ● split commands to toolbar and menu commands
        AppRegistry.ToolBarCommands.AddRange(new Command[] { cmdDashboard, cmdAppFolder, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdDatabaseWorkbench, cmdResourceTranslations, cmdRegenerateDatabase, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdTest, cmdExit }.Where(CanAccess));
        //AppRegistry.MenuCommands.AddRange(MasterCommandGroups);
    }
}
