/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Represents this application.
/// </summary>
static internal partial class AppHost
{
    // ● private
    /// <summary>
    /// Initializes the <see cref="SysConfig"/> static class.
    /// </summary>
    static void InitializeConfigs()
    {
        SysConfig.ApplicationMode = ApplicationMode.Desktop;
        SysConfig.MainAssembly = typeof(AppHost).Assembly;
        
        //Db.Settings.LogSqlStatements = true;
    }
    /// <summary>
    /// Creates the default tERP SQLite connection.
    /// </summary>
    static DbConnectionInfo CreateDefaultConnectionInfo()
    {
        DbConnectionInfo Result = new DbConnectionInfo();
        Result.Name = Sys.DEFAULT;
        Result.DbServerType = DbServerType.Sqlite;
        Result.ConnectionString = string.Format(DbServerType.Sqlite.GetTemplateConnectionString(), "[Data]/tERP.db3");
        return Result;
    }
    /// <summary>
    /// Shows the database connection edit dialog.
    /// </summary>
    static async Task<bool> ShowDbConnectionEditDialog(DbConnectionInfo ConnectionInfo)
    {
        return await DbConnectionEditDialog.ShowModal(ConnectionInfo, Ui.MainWindow);
    }
    /// <summary>
    /// Loads database configuration settings.
    /// </summary>
    static async Task LoadConnectionStrings()
    {
        Db.Connections.Load();
        if (Db.Connections.List.Count == 0)
        {
            // The natural process is to call ShowDbConnectionEditDialog() and let the user enter a connection string.
            DbConnectionInfo CI = CreateDefaultConnectionInfo();
            Db.Connections.Add(CI);
            Db.Connections.Save();
            await MessageBox.Info($"A default SQLite connection has been created.{Environment.NewLine}{Environment.NewLine}{CI.ConnectionString}", HiddenMainWindow);
        }
    }
    /// <summary>
    /// Creates a database when it does not exist yet.
    /// </summary>
    static async Task CreateDatabase(DbConnectionInfo ConnectionInfo)
    {
        SqlProvider Provider = ConnectionInfo.GetSqlProvider();
        string ConnectionString = ConnectionInfo.ConnectionString;

        if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
        {
            Provider.CreateDatabase(ConnectionString);
            await MessageBox.Info($"An empty database has been created for connection '{ConnectionInfo.Name}'.{Environment.NewLine}{Environment.NewLine}{ConnectionString}", HiddenMainWindow);
        }
    }
    /// <summary>
    /// Creates any non-existing creatable database.
    /// </summary>
    static async Task CreateDatabases()
    {
        DbConnectionInfo DefaultConnectionInfo = Db.GetDefaultConnectionInfo();
        await CreateDatabase(DefaultConnectionInfo);

        foreach (var ConInfo in Db.Connections.List)
        {
            if (ConInfo != DefaultConnectionInfo)
            {
                await CreateDatabase(ConInfo);
            }
        }
    }

    static void LoadLibraries()
    {
        CommonLib.Load();
        DataLib.Load();
        DesktopLib.Load();
    }
    static void InitializeLibraries()
    {
        CommonLib.Initialize();
        DataLib.Initialize();
        DesktopLib.Initialize();
    }

    static async Task<bool> EnsureAdminUser()
    {
        bool Result = Store.TableExists(DbConfig.SysAppUserTableName) && !Store.TableIsEmpty(DbConfig.SysAppUserTableName);
        if (!Result)
        {
            FirstRunBoxData BoxData = await FirstRunDialog.ShowModal(Ui.MainWindow);
            Result = BoxData.Result;
            
            if (!Result)
            {
                await MessageBox.Error("No Admin user. Terminating...", Ui.MainWindow);
            }
            else
            {
                AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
                Module.AddUser(FullName: BoxData.FullName, UserName: BoxData.UserName, PlainTextPassword: BoxData.Password, UserLevel: UserLevel.Admin);
                Result = true;
            }
        }
        
        return Result;
    }
    static async Task<bool> LoginUser()
    {
        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;

        LoginBoxData BoxData = new();
        BoxData.SupportedCultures = DataLib.SupportedCultures;

        for (int i = 0; i < 3; i++)
        {
            BoxData = await LoginDialog.ShowModal(BoxData, Ui.MainWindow);

            if (!BoxData.Result)
                return false;

            AppUser User = Module.LoadByUserName(BoxData.UserName);

            if (User == null)
            {
                BoxData.Message = "Invalid user name or password.";
                continue;
            }

            if (!User.IsActive)
            {
                BoxData.Message = "User account is disabled.";
                continue;
            }

            string Password = User.Properties["Password"] as string;
            string Salt = User.Properties["Salt"] as string;

            bool Flag = Sec.VerifyPassword(BoxData.Password, Password, Salt, 100_000);

            if (!Flag)
            {
                BoxData.Message = "Invalid user name or password.";
                continue;
            }

            User.CultureCode = BoxData.CultureCode;

            Sys.Context.CurrentUser = User;
             

            return true;
        }

        return false;
    }
    static bool UseUsers()
    {
        string Value = Config.GetValue(DataLib.SUseUsers);
        return !string.IsNullOrWhiteSpace(Value) && Convert.ToBoolean(Value);
    }
    static string GetAutoLoginUserName()
    {
        if (!string.IsNullOrWhiteSpace(DataLib.DebugUserName))
            return DataLib.DebugUserName;
        object Result = Store.SelectResult("""
            select UserName
            from SYS_APP_USER
            where IsActive = 1
            order by UserLevelId desc, UserName
            """, string.Empty);
        return Result == null || Result == DBNull.Value ? string.Empty : Result.ToString();
    }
    static async Task<bool> AutoLoginUser()
    {
        string UserName = GetAutoLoginUserName();
        if (string.IsNullOrWhiteSpace(UserName))
            return false;

        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;

        AppUser User = Module.LoadByUserName(UserName);

        if (User == null)
            throw new TripousException($"Auto-login user not found: {UserName}");

        if (!User.IsActive)
            throw new TripousException($"Auto-login user is inactive: {UserName}");

        Sys.Context.CurrentUser = User;

        await Task.CompletedTask;
        return true;
    }
 
    
    // ● public
    /// <summary>
    /// Starts this application.
    /// <para>This method is called from the <see cref="App.OnFrameworkInitializationCompleted"/> method.</para>
    /// <para>The whole initialization takes place having a hidden window as the main window.</para>
    /// <para>After the initialization is done the real <see cref="MainWindow"/> becomes the main window.</para>
    /// <para>This method loads connection strings, creates databases and schemas and registers all descriptors such as commands, lookups, modules and forms.</para>
    /// </summary>
    static public async Task Start(IClassicDesktopStyleApplicationLifetime AvaloniaDesktop)
    {
        Db.Settings.LogSqlStatements = false;
        bool Flag = true;
        
        AppHost.AvaloniaDesktop = AvaloniaDesktop;
        Ui.MainWindow = AppHost.HiddenMainWindow;
        
        try
        {
            InitializeConfigs();

            await LoadConnectionStrings();
            await CreateDatabases();
           
            Registry.RegisterSchemas();                 // Registers database schema versions
            Schemas.Execute();                          // Creates database tables etc. based on the registered schemas

            Store = SqlStores.CreateDefaultSqlStore();
            
            LoadLibraries();
            TypeStore.RegisterLoadedAssemblies();
            Registry.RegisterDescriptors();             // Register data descriptors, i.e. commands, lookup sources, locators and modules.
            DesktopLib.RegisterDescriptors();           // Register desktop descriptors, i.e. forms.
            
            InitializeLibraries();
            
            Flag = await EnsureAdminUser();

            if (Flag)
                Flag = UseUsers() ? await LoginUser() : await AutoLoginUser();
            
            if (Flag)
            {
                RegisterCommands();
                AppHost.MainWindow = new MainWindow();
                Ui.MainWindow = AppHost.MainWindow;
                MainWindow.Show();
            }
 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await MessageBox.Error(e.Message, Ui.MainWindow);
            Flag = false;
        }

        if (!Flag)
        {
            Ui.MainWindow.Close();
            return;
        }
        
        DesktopExceptionHandler.Initialize();
    }
    /// <summary>
    /// Initializes the ui of this application.
    /// <para>This method is called from the <see cref="MainWindow.WindowInitialize"/> method.</para>
    /// </summary>
    static public void InitializeUi(AppFormPagerHandler SideBarHandler, AppFormPagerHandler ContentHandler)
    {
        if (AppHost.SideBarHandler == null)
        {
            AppHost.SideBarHandler = SideBarHandler;
            AppHost.ContentHandler = ContentHandler;

            ShowSideBarPages();
        }
    }
}
