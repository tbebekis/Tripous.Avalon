namespace PasswordManager;

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
    static DbConnectionInfo CreateDefaultConnectionInfo()
    {
        DbConnectionInfo Result = new();
        Result.Name = Sys.DEFAULT;
        Result.DbServerType = DbServerType.Sqlite;
        Result.ConnectionString = string.Format(DbServerType.Sqlite.GetTemplateConnectionString(), "[Data]/password-manager.db3");
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
    /// Loads or creates database connection settings.
    /// </summary>
    static async Task LoadConnectionStrings()
    {
        Db.Connections.Load();
        if (Db.Connections.List.Count == 0)
        {
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
    /// Creates or verifies the vault master password.
    /// </summary>
    static async Task<bool> UnlockVault()
    {
        if (!VaultService.HasMasterPassword())
            return await CreateMasterPasswordDialog.ShowModal(HiddenMainWindow);
        return await UnlockVaultDialog.ShowModal(HiddenMainWindow);
    }

    // ● static public
    /// <summary>
    /// Starts the application.
    /// </summary>
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
            Flag = await UnlockVault();

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
    static public void InitializeUi(AppFormPagerHandler SideBarHandler, AppFormPagerHandler ContentHandler)
    {
        if (AppHost.SideBarHandler == null)
        {
            AppHost.SideBarHandler = SideBarHandler;
            AppHost.ContentHandler = ContentHandler;
            ShowSideBarPages();
            if (Convert.ToBoolean(Config.GetValue("PasswordManager.AutoOpenCredentialList"), CultureInfo.InvariantCulture))
                ContentHandler.ShowDataForm("Credential");
        }
    }
}
