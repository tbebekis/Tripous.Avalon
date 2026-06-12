namespace tERP.Tests;

public sealed class TestDatabaseFixture: IAsyncLifetime
{
    // ● private fields
    readonly string fDatabaseFolder;
    readonly string fDatabasePath;

    // ● private
    void ConfigureApplication()
    {
        SysConfig.ApplicationMode = ApplicationMode.Service;
        SysConfig.MainAssembly = typeof(TestDatabaseFixture).Assembly;
        SysConfig.AppFolderPath = fDatabaseFolder;
        SysConfig.AppDataFolderPath = fDatabaseFolder;
        SysConfig.AppTempFolderPath = fDatabaseFolder;

        DbConfig.DefaultConnectionName = Sys.DEFAULT;
        Db.Connections.List.Clear();
        Db.Connections.List.Add(new DbConnectionInfo()
        {
            Name = Sys.DEFAULT,
            DbServerType = DbServerType.Sqlite,
            ConnectionString = $@"Data Source=""{fDatabasePath}""",
        });
    }
    void CreateDatabase()
    {
        DbConnectionInfo ConnectionInfo = Db.GetDefaultConnectionInfo();
        ConnectionInfo.GetSqlProvider().CreateDatabase(ConnectionInfo.ConnectionString);

        Registry.RegisterSchemas();
        Schemas.Execute();
    }
    void RegisterApplication()
    {
        CommonLib.Load();
        DataLib.Load();
        TypeStore.RegisterLoadedAssemblies();
        Registry.RegisterDescriptors();
        CommonLib.Initialize();
        DataLib.Initialize();
    }
    void CreateTestUser()
    {
        AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
        if (Module == null)
            throw new TripousDataException("Cannot create the AppUser module.");

        Module.AddUser("Test User", "test", "test", UserLevel.Admin);
        Sys.Context.CurrentUser = Module.LoadByUserName("test");
    }

    // ● construction
    public TestDatabaseFixture()
    {
        fDatabaseFolder = Path.Combine(Path.GetTempPath(), "terp-tests", Sys.GenId());
        fDatabasePath = Path.Combine(fDatabaseFolder, "terp-tests.db");
    }

    // ● public
    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(fDatabaseFolder);
        ConfigureApplication();
        CreateDatabase();
        RegisterApplication();
        CreateTestUser();
        await SampleData.AddSampleDataAsync(SampleData.GetNotAdded());
        Store = SqlStores.CreateDefaultSqlStore();
    }
    public Task DisposeAsync()
    {
        Sys.Context.CurrentUser = null;
        System.Data.SQLite.SQLiteConnection.ClearAllPools();

        if (Directory.Exists(fDatabaseFolder))
            Directory.Delete(fDatabaseFolder, true);

        return Task.CompletedTask;
    }

    // ● properties
    public string DatabasePath => fDatabasePath;
    public SqlStore Store { get; private set; }
}
