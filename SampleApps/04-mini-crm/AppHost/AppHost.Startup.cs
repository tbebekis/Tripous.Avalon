using MiniCrm.Data;

namespace MiniCrm;

/// <summary>
/// Contains application startup logic.
/// </summary>
static public partial class AppHost
{
    // ● private
    /// <summary>
    /// Initializes Tripous system configuration.
    /// </summary>
    static void InitializeConfigs()
    {
        SysConfig.ApplicationMode = ApplicationMode.Desktop;
        SysConfig.MainAssembly = typeof(AppHost).Assembly;
    }
    /// <summary>
    /// Creates the default SQLite connection.
    /// </summary>
    /// <returns>The default connection information.</returns>
    static DbConnectionInfo CreateDefaultConnectionInfo()
    {
        DbConnectionInfo Result = new();
        Result.Name = Sys.DEFAULT;
        Result.DbServerType = DbServerType.Sqlite;
        Result.ConnectionString = string.Format(DbServerType.Sqlite.GetTemplateConnectionString(), "[Data]/mini-crm.db3");
        return Result;
    }
    /// <summary>
    /// Shows the database connection edit dialog.
    /// </summary>
    /// <param name="ConnectionInfo">The connection information to edit.</param>
    /// <returns>True when the dialog result is accepted.</returns>
    static async Task<bool> ShowDbConnectionEditDialog(DbConnectionInfo ConnectionInfo)
    {
        return await DbConnectionEditDialog.ShowModal(ConnectionInfo, Ui.MainWindow);
    }
    /// <summary>
    /// Loads or creates database connection settings.
    /// </summary>
    static async Task LoadConnectionStrings()
    {
        Db.Connections.Load();
        if (Db.Connections.List.Count == 0)
        {
            // ● Keep this sample automatic so the user reaches the data form immediately.
            DbConnectionInfo ConnectionInfo = CreateDefaultConnectionInfo();
            Db.Connections.Add(ConnectionInfo);
            Db.Connections.Save();
            await MessageBox.Info($"A default SQLite connection has been created.{Environment.NewLine}{Environment.NewLine}{ConnectionInfo.ConnectionString}", HiddenMainWindow);
        }
    }
    /// <summary>
    /// Creates the database when it does not exist.
    /// </summary>
    static async Task CreateDatabase()
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        SqlProvider Provider = ConnectionInfo.GetSqlProvider();
        string ConnectionString = ConnectionInfo.ConnectionString;

        if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
        {
            Provider.CreateDatabase(ConnectionString);
            await MessageBox.Info($"An empty SQLite database has been created.{Environment.NewLine}{Environment.NewLine}{ConnectionString}", HiddenMainWindow);
        }
    }
    /// <summary>
    /// Initializes application libraries.
    /// </summary>
    static void InitializeLibraries()
    {
        // ● In multi-assembly applications this is the right place to initialize central static classes from other assemblies.
        // ● Even a fake Initialize() method can force .NET to load an assembly, so TypeStore can discover its types.
    }
    /// <summary>
    /// Ensures that the application has an admin user.
    /// </summary>
    /// <returns>True when an admin user exists.</returns>
    static async Task<bool> EnsureAdminUser()
    {
        bool Result = Store.TableExists(DbConfig.SysAppUserTableName) && !Store.TableIsEmpty(DbConfig.SysAppUserTableName);
        if (!Result)
        {
            AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
            Module.AddUser(FullName: "Administrator", UserName: "admin", PlainTextPassword: "admin", UserLevel: UserLevel.Admin);
            Result = true;
        }

        await Task.CompletedTask;
        return Result;
    }
    /// <summary>
    /// Returns true when login users are enabled.
    /// </summary>
    /// <returns>True when users are enabled.</returns>
    static bool UseUsers()
    {
        string Value = Config.GetValue("UseUsers");
        return !string.IsNullOrWhiteSpace(Value) && Convert.ToBoolean(Value, CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// Returns the automatic login user name.
    /// </summary>
    /// <returns>The automatic login user name.</returns>
    static string GetAutoLoginUserName()
    {
        object Result = Store.SelectResult("""
            select UserName
            from SYS_APP_USER
            where IsActive = 1
            order by UserLevelId desc, UserName
            """, string.Empty);
        return Result == null || Result == DBNull.Value ? string.Empty : Result.ToString();
    }
    /// <summary>
    /// Logs in the first active user.
    /// </summary>
    /// <returns>True when login succeeds.</returns>
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
    /// <summary>
    /// Shows the login dialog and logs in a user.
    /// </summary>
    /// <returns>True when login succeeds.</returns>
    static async Task<bool> LoginUser()
    {
        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
        LoginBoxData BoxData = new();
        BoxData.SupportedCultures = ["en-US", "el-GR"];

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

    // ● static public
    /// <summary>
    /// Starts the application.
    /// <para>The application starts with a hidden window as owner for early dialogs.</para>
    /// <para>The real main window becomes the main window only after configuration, schema execution and registration are complete.</para>
    /// </summary>
    /// <param name="AvaloniaDesktop">The Avalonia desktop lifetime.</param>
    static public async Task Start(IClassicDesktopStyleApplicationLifetime AvaloniaDesktop)
    {
        bool Flag = true;
        AppHost.AvaloniaDesktop = AvaloniaDesktop;
        Ui.MainWindow = HiddenMainWindow;

        try
        {
            InitializeConfigs();
            await LoadConnectionStrings();
            await CreateDatabase();
            Registry.RegisterSchemas();
            Schemas.Execute();
            Store = SqlStores.CreateDefaultSqlStore();
            InitializeLibraries();
            TypeStore.RegisterLoadedAssemblies();
            Registry.RegisterDescriptors();
            Flag = await EnsureAdminUser();

            if (Flag)
                Flag = UseUsers() ? await LoginUser() : await AutoLoginUser();

            if (Flag)
            {
                RegisterCommands();
                MainWindow = new MainWindow();
                Ui.MainWindow = MainWindow;
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
    /// Initializes application UI handlers.
    /// </summary>
    /// <param name="SideBarHandler">The side bar handler.</param>
    /// <param name="ContentHandler">The content handler.</param>
    static public void InitializeUi(AppFormPagerHandler SideBarHandler, AppFormPagerHandler ContentHandler)
    {
        if (AppHost.SideBarHandler == null)
        {
            AppHost.SideBarHandler = SideBarHandler;
            AppHost.ContentHandler = ContentHandler;
            ShowSideBarPages();
            if (Convert.ToBoolean(Config.GetValue("MiniCrm.AutoOpenCustomerList"), CultureInfo.InvariantCulture))
                ContentHandler.ShowDataForm("Customer");
        }
    }
}
