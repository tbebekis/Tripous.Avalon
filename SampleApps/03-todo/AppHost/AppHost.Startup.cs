using System.Globalization;
using ToDo.Data;

namespace ToDo;

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
        Result.ConnectionString = string.Format(DbServerType.Sqlite.GetTemplateConnectionString(), "[Data]/todo.db3");
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
            RegisterCommands();
            MainWindow = new MainWindow();
            Ui.MainWindow = MainWindow;
            MainWindow.Show();
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
            if (Convert.ToBoolean(Config.GetValue("ToDo.AutoOpenTaskList"), CultureInfo.InvariantCulture))
                ContentHandler.ShowDataForm("TodoTask");
        }
    }
}
