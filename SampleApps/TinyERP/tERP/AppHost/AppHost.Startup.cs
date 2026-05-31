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

        Db.Settings.LogSqlStatements = true;
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
    /// Registers database schema versions
    /// </summary>
    static void RegisterSchemas() => Registry.RegisterSchemas();
    
    /// <summary>
    /// Creates database tables etc. based on the registered schemas
    /// </summary>
    static void ExecuteSchemas()
    {
        Schemas.Execute();
    }
 
    /// <summary>
    /// Register descriptors, i.e. commands, lookup sources, locators, modules and forms.
    /// </summary>
    static void RegisterDescriptors()
    {
         Registry.RegisterLookups();
         Registry.RegisterLookupSources();
         Registry.RegisterLocators();
 
         Registry.RegisterModules();
         Registry.RegisterForms();
  
         Registry.UpdateLookups();
         Registry.UpdateLocators();
         Registry.UpdateForms();
         Registry.UpdateModules();
         
         Registry.RegisterCommands();
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
        AppHost.AvaloniaDesktop = AvaloniaDesktop;
        Ui.MainWindow = AppHost.HiddenMainWindow;
        
        try
        {
            AppHost.MainWindow = new MainWindow();
            
            InitializeConfigs();

            await LoadConnectionStrings();
            CreateDatabases();

            RegisterSchemas(); 
            ExecuteSchemas();

            Store = SqlStores.CreateDefaultSqlStore();
            Ui.MainWindow = AppHost.MainWindow;
            
            LoadLibraries();
            TypeStore.RegisterLoadedAssemblies();
            RegisterDescriptors();
            MainWindow.Show();
            
            AddDefaultInitialData();
            InitializeLibraries();
            
            
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await MessageBox.Error(e.Message, HiddenMainWindow);
            throw;
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