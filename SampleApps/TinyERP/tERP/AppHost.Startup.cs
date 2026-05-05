namespace tERP;

static public partial class AppHost
{
    // ● private
    static void InitializeSysConfig()
    {
        SysConfig.ApplicationMode = ApplicationMode.Desktop;
        SysConfig.MainAssembly = typeof(AppHost).Assembly;
    }
 
    /// <summary>
    /// Loads database configuration settings.
    /// </summary>
    static async Task LoadConnectionStrings()
    {
        Db.Connections.Load();
        if (Db.Connections.List.Count == 0)
        {
            DbConnectionInfo CI = new DbConnectionInfo();
            CI.Name = Sys.DEFAULT;
            bool Flag = await DbConnectionEditDialog.ShowModal(CI, HiddenMainWindow);

            if (Flag)
            {
                Db.Connections.Add(CI);
                Db.Connections.Save();
            }
            else
            {
                HiddenMainWindow.Close();
            }
        }
    }
    /// <summary>
    /// Creates any non-existing creatable database.
    /// </summary>
    static void CreateDatabases()
    {
        DbConnectionInfo DefaultConnectionInfo = Db.GetDefaultConnectionInfo();

        SqlProvider Provider = DefaultConnectionInfo.GetSqlProvider();
        string ConnectionString = DefaultConnectionInfo.ConnectionString;

        if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
        {
            Provider.CreateDatabase(ConnectionString);
        }

        foreach (var ConInfo in Db.Connections.List)
        {
            if (ConInfo != DefaultConnectionInfo)
            {
                Provider = ConInfo.GetSqlProvider();
                ConnectionString = ConInfo.ConnectionString;

                if (!Provider.DatabaseExists(ConnectionString) && Provider.CanCreateDatabases)
                {
                    Provider.CreateDatabase(ConnectionString);
                }
            }
        }
    }

    /// <summary>
    /// Creates database tables etc. based on the registered schemas
    /// </summary>
    static void ExecuteSchemas()
    {
        Schemas.Execute();
    }
    static void AddSampleData(int TradeCount)
    {
       // string SqlText = "select * from Country";
       if (Store.TableExists("Country") && Store.TableIsEmpty("Country"))
           SampleData.Execute(Store, TradeCount);
    }
    static void RegisterDescriptors()
    {
        RegisterCommands();

        RegisterLookupSources();
        RegisterLocators();

        RegisterModules();
        RegisterForms();
    }
    
    // ● public
    static public async Task Start(IClassicDesktopStyleApplicationLifetime AvaloniaDesktop)
    {
        AppHost.AvaloniaDesktop = AvaloniaDesktop;
        Ui.MainWindow = AppHost.HiddenMainWindow;
        
        try
        {
            AppHost.MainWindow = new MainWindow();
            
            InitializeSysConfig();

            await LoadConnectionStrings();
            CreateDatabases();

            RegisterSchemas();
            ExecuteSchemas();

            Store = SqlStores.CreateDefaultSqlStore();
            AddSampleData(10000);

            RegisterDescriptors();
            
            Ui.MainWindow = AppHost.MainWindow;
            MainWindow.Show();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await MessageBox.Error(e.Message, HiddenMainWindow);
            throw;
        }
        
        DesktopExceptionHandler.Initialize();
    }
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