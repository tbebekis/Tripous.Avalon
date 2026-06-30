/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb;

/// <summary>
/// Provides application initialization for tERPWeb.
/// </summary>
static public partial class App
{
    static readonly object fLock = new();
    static bool fInitialized;
    static SqlStore Store;

    // ● private
    /// <summary>
    /// Initializes global Tripous configuration.
    /// </summary>
    static void InitializeConfigs(WebApplicationBuilder Builder)
    {
        SysConfig.ApplicationMode = ApplicationMode.Web;
        SysConfig.MainAssembly = typeof(App).Assembly;
        SysConfig.AppFolderPath = AppContext.BaseDirectory;
        SysConfig.AppDataFolderPath = Path.Combine(SysConfig.AppFolderPath, "Data");
    }
    /// <summary>
    /// Creates the default tERPWeb SQLite connection.
    /// </summary>
    static DbConnectionInfo CreateDefaultConnectionInfo()
    {
        DbConnectionInfo Result = new();
        Result.Name = Sys.DEFAULT;
        Result.DbServerType = DbServerType.Sqlite;
        Result.ConnectionString = string.Format(DbServerType.Sqlite.GetTemplateConnectionString(), "[Data]/tERP.db3");
        Result.CommandTimeoutSeconds = 300;
        return Result;
    }
    /// <summary>
    /// Loads database connection settings.
    /// </summary>
    static void LoadConnectionStrings()
    {
        Db.Connections.Load();
        if (Db.Connections.List.Count == 0)
        {
            DbConnectionInfo CI = CreateDefaultConnectionInfo();
            Db.Connections.Add(CI);
            Db.Connections.Save();
        }
    }
    /// <summary>
    /// Creates a database when it does not exist yet.
    /// </summary>
    static void CreateDatabase(DbConnectionInfo ConnectionInfo)
    {
        SqlProvider Provider = ConnectionInfo.GetSqlProvider();
        string ConnectionString = ConnectionInfo.ConnectionString;

        if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
            Provider.CreateDatabase(ConnectionString);
    }
    /// <summary>
    /// Creates any non-existing creatable database.
    /// </summary>
    static void CreateDatabases()
    {
        DbConnectionInfo DefaultConnectionInfo = Db.GetDefaultConnectionInfo();
        CreateDatabase(DefaultConnectionInfo);

        foreach (DbConnectionInfo ConInfo in Db.Connections.List)
        {
            if (ConInfo != DefaultConnectionInfo)
                CreateDatabase(ConInfo);
        }
    }
    /// <summary>
    /// Registers database schemas.
    /// </summary>
    static void RegisterSchemas()
    {
        Registry.RegisterSchemas();
    }
    /// <summary>
    /// Executes database schemas.
    /// </summary>
    static void ExecuteSchemas()
    {
        Schemas.Execute();
    }
    /// <summary>
    /// Creates the default SQL store.
    /// </summary>
    static void CreateDefaultSqlStore()
    {
        Store = SqlStores.CreateDefaultSqlStore();
    }
    /// <summary>
    /// Forces application libraries to load.
    /// </summary>
    static void LoadLibraries()
    {
        Tripous.Data.Db.Initialize();
        CommonLib.Load();
        DataLib.Load();
    }
    /// <summary>
    /// Registers discoverable types.
    /// </summary>
    static void RegisterTypes()
    {
        TypeStore.RegisterLoadedAssemblies();
    }
    /// <summary>
    /// Registers application descriptors.
    /// </summary>
    static void RegisterDescriptors()
    {
        Registry.RegisterDescriptors();
    }
    /// <summary>
    /// Registers Ajax request handlers.
    /// </summary>
    static void RegisterAjaxHandlers()
    {
        AjaxRequestHandlers.RegisterApplicationAssemblies();
    }
    /// <summary>
    /// Initializes application libraries.
    /// </summary>
    static void InitializeLibraries()
    {
        CommonLib.Initialize();
        DataLib.Initialize();
    }

    // ● static public
    /// <summary>
    /// Starts the tERPWeb application.
    /// </summary>
    static public void Start(WebApplicationBuilder Builder)
    {
        lock (fLock)
        {
            if (fInitialized)
                return;

            InitializeConfigs(Builder);
            LoadConnectionStrings();
            CreateDatabases();
            RegisterSchemas();
            ExecuteSchemas();
            CreateDefaultSqlStore();
            LoadLibraries();
            RegisterTypes();
            RegisterDescriptors();
            RegisterAjaxHandlers();
            InitializeLibraries();

            fInitialized = true;
        }
    }
}
