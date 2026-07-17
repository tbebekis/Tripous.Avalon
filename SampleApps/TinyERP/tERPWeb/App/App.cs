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
        SysConfig.AppName = "tERPWeb";
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
        WebLib.Load();
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
        WebLib.RegisterDescriptors();
        RegisterAppWebForms();
    }
    /// <summary>
    /// Registers tERPWeb application forms.
    /// </summary>
    static void RegisterAppWebForms()
    {
        WebFormDef Form = WebDeskRegistry.AddOrUpdateForm("MainDashboard", TitleKey: "Dashboard", Module: string.Empty, ViewName: "/Views/WebForms/MainDashboard.cshtml", Group: "General", IsReadOnly: true);
        Form.IsCustom = true;
        Form.JsFormClassType = "app.MainDashboardForm";
        if (!Form.JavaScriptFiles.Contains("/js/forms/main-dashboard-form.js"))
            Form.JavaScriptFiles.Add("/js/forms/main-dashboard-form.js");

        Form = WebDeskRegistry.AddOrUpdateForm("CommandTreeView", TitleKey: "Commands", Module: string.Empty, ViewName: "/Views/WebForms/CommandTreeView.cshtml", Group: "General", IsReadOnly: true);
        Form.IsCustom = true;
        Form.JsFormClassType = "app.CommandTreeViewForm";
        if (!Form.JavaScriptFiles.Contains("/js/forms/command-tree-view-form.js"))
            Form.JavaScriptFiles.Add("/js/forms/command-tree-view-form.js");

        Form = WebDeskRegistry.AddOrUpdateForm("DatabaseWorkbench", TitleKey: "Database Workbench", Module: string.Empty, ViewName: "/Views/WebForms/DatabaseWorkbench.cshtml", Group: "General", IsReadOnly: true, SecurityLevel: UserLevel.Admin);
        Form.IsCustom = true;
        Form.JsFormClassType = "app.DatabaseWorkbenchForm";
        if (!Form.JavaScriptFiles.Contains("/js/forms/database-workbench-form.js"))
            Form.JavaScriptFiles.Add("/js/forms/database-workbench-form.js");
        if (!Form.CssFiles.Contains("/css/forms/database-workbench-form.css"))
            Form.CssFiles.Add("/css/forms/database-workbench-form.css");

        RegisterDataModuleWebForms();
    }
    /// <summary>
    /// Registers the Company form-level FactBox.
    /// </summary>
    /// <param name="Form">The web form definition.</param>
    static void RegisterCompanyWebFormFactBox(WebFormDef Form)
    {
        if (Form == null || Form.FactBoxes.Contains("CompanyWebFormInfo"))
            return;

        Form.FactBoxes.Add(new ItemFactBoxDef
        {
            Name = "CompanyWebFormInfo",
            TitleKey = "Company WebForm Info",
            ProviderClassName = typeof(FormScopeFactBoxProvider).FullName
        });
    }
    /// <summary>
    /// Configures web forms that require document-specific client behavior.
    /// </summary>
    static void ConfigureDocumentWebForms()
    {
        ConfigureDocumentWebForm("SalesOrder", "app.SalesOrderForm", "app.SalesDataModule");
        ConfigureDocumentWebForm("SalesDeliveryNote", "app.SalesDataForm", "app.SalesDataModule");
    }
    /// <summary>
    /// Configures a single document web form.
    /// </summary>
    /// <param name="FormName">The form name.</param>
    /// <param name="JsFormClassType">The JavaScript form class type.</param>
    /// <param name="JsDataModuleClassType">The JavaScript data module class type.</param>
    static void ConfigureDocumentWebForm(string FormName, string JsFormClassType, string JsDataModuleClassType)
    {
        WebFormDef Form = WebDeskRegistry.FindForm(FormName);
        if (Form == null)
            return;

        Form.JsFormClassType = JsFormClassType;
        Form.JsDataModuleClassType = JsDataModuleClassType;
        if (!Form.JavaScriptFiles.Contains("/js/forms/document-data-forms.js"))
            Form.JavaScriptFiles.Add("/js/forms/document-data-forms.js");
    }
    /// <summary>
    /// Registers standard WebDesk data forms from registered data modules.
    /// </summary>
    static void RegisterDataModuleWebForms()
    {
        WebFormDef Form;
        bool IsNewForm;
        foreach (ModuleDef Module in DataRegistry.Modules)
        {
            Form = WebDeskRegistry.FindForm(Module.Name);
            if (Form != null && Form.IsCustom)
                continue;
            IsNewForm = Form == null;

            Form = WebDeskRegistry.AddOrUpdateForm(
                Name: Module.Name,
                TitleKey: IsNewForm ? Module.TitleKey : null,
                Module: Module.Name,
                ViewName: "/Views/WebForms/WebDataForm.cshtml",
                ItemViewName: "/Views/WebForms/WebItemPage.cshtml",
                Group: IsNewForm ? Module.Group : null,
                IsReadOnly: IsNewForm ? false : null,
                SecurityLevel: IsNewForm ? Module.SecurityLevel : null);
            Form.JsFormClassType = "tp.WebDataForm";
            if (Module.Name.IsSameText("Company"))
                RegisterCompanyWebFormFactBox(Form);
        }
        ConfigureDocumentWebForms();
    }
    /// <summary>
    /// Registers Ajax request handlers.
    /// </summary>
    static void RegisterAjaxHandlers()
    {
        AjaxOperations.RegisterApplicationAssemblies();
        Tripous.WebDesk.WebFormProviders.RegisterApplicationAssemblies();
    }
    /// <summary>
    /// Initializes application libraries.
    /// </summary>
    static void InitializeLibraries()
    {
        CommonLib.Initialize();
        DataLib.Initialize();
        WebLib.Initialize();
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
