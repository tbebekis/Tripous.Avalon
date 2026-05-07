namespace Tripous;

static public class SysConfig
{
    static string fAppName;
    static string fAppExeFolderPath;
    static string fAppFolderPath;
    static string fAppDataFolderPath;
    static string fAppTempFolderPath;

    
    /// <summary>
    /// The mode of the application (Desktop, Web, Service)
    /// </summary>
    static public ApplicationMode ApplicationMode { get; set; } = Tripous.ApplicationMode.Desktop;
    /// <summary>
    /// Gets the assembly of the main executable.
    /// <para> The user has to manually set the main assembly in Compact Framework. Otherwise those properties that use
    /// the main assembly in order to infer various paths will throw exceptions.</para> 
    /// </summary>
    static public Assembly MainAssembly { get; set; } = typeof(SysConfig).Assembly;

    /// <summary>
    /// Returns the application name, e.g. MyApp
    /// <para>NOTE: Valid with desktop apps only.</para>
    /// </summary>
    static public string AppName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(fAppName))
            {
                Assembly Assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                return Assembly.GetName().Name;
            }
            return fAppName;
        }
        set
        {
            fAppName = value;
        }
    }
    /// <summary>
    /// Returns the path to the folder with the executable.
    /// <para>NOTE: Valid with desktop apps only.</para>
    /// </summary>
    static public string AppExeFolderPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(fAppExeFolderPath))
                fAppExeFolderPath  = AppContext.BaseDirectory;
            return fAppExeFolderPath;
        }
        set
        {
            fAppExeFolderPath = value;
        }
    }
    /// <summary>
    /// Returns the path where the application uses to create files, such as setting files, etc.
    /// </summary>
    static public string AppFolderPath
    {
        get
        {
            if (string.IsNullOrEmpty(fAppFolderPath))
                fAppFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
            return fAppFolderPath;
        }
        set 
        {
            fAppFolderPath = value;
        }
    }
    /// <summary>
    /// Returns the <see cref="AppFolderPath"/> appending a <c>/Data</c> to it, e.g. <c>$HOME/.config/MyApp/Data</c>
    /// </summary>
    static public string AppDataFolderPath
    {
        get
        {
            if (string.IsNullOrEmpty(fAppDataFolderPath))
                fAppDataFolderPath = Path.Combine(AppFolderPath, "Data");
            return fAppDataFolderPath;
        }
        set
        {
            fAppDataFolderPath = value;
        }
    }
    /// <summary>
    /// The path for the Logs folder.
    /// </summary>
    static public string AppLogFolderPath => Path.Combine(AppFolderPath, "Logs");
    /// <summary>
    /// Returns a path the application can use as a temp folder path.
    /// </summary>
    static public string AppTempFolderPath
    {
        get
        {
            if (string.IsNullOrEmpty(fAppTempFolderPath))
                fAppDataFolderPath = Path.GetTempPath();
            return fAppTempFolderPath;
        }
        set
        {
            fAppTempFolderPath = value;
        }
    }
    
    /// <summary>
    /// When is set indicates that the Oids are Guid strings.  
    /// <para>Defaults to true.</para>
    /// </summary>
    static public bool GuidOids { get; set; } = true;
    /// <summary>
    /// Gets the variables prefix in Sql statements. Defaults to :@, e.g. :@Today
    /// </summary>
    static public string VariablesPrefix { get; set; } = ":@";
    
    /// <summary>
    /// The field name of the company field, used in various tables. 
    /// <para>Defaults to CompanyId</para>
    /// </summary>
    static public string CompanyFieldName { get; set; } = "CompanyId";
    /// <summary>
    /// The Id of the current company, if any, else null.
    /// </summary>
    static public object CompanyId { get; set; } = Sys.StandardCompanyGuid;
    /// <summary>
    /// ReadOnly. Returns the value of the CompanyId as a string for constructing Sql statements.
    /// </summary>
    static public string CompanyIdSql
    {
        get
        {

            if (CompanyId == null)
            {
                if (GuidOids)
                    return Sys.StandardCompanyGuid.QS();
                return "-1";
            }

            Type T = CompanyId.GetType();

            if ((T == typeof(System.String)) || (T == typeof(System.Guid)))
                return CompanyId.ToString().QS();
            else
                return CompanyId.ToString();
        }

    }
    /// <summary>
    /// ReadOnly. Returns the value of the CompanyId
    /// </summary>
    static public object CompanyIdValue
    {
        get
        {
            if (CompanyId == null)
            { 
                return GuidOids? (object)Sys.StandardCompanyGuid: -1;
            }
            else
            {
                return CompanyId;
            }
        }
    }
    
    /// <summary>
    /// Gets the default SimpleType data type for Id fields, based on the GuidOids setting in the Variables
    /// </summary>
    static public DataFieldType OidDataType => SysConfig.GuidOids ? DataFieldType.String : DataFieldType.Integer;
    /// <summary>
    /// Gets the size of a field for  the default SimpleType data type for Id fields
    /// </summary>
    static public int OidSize => OidDataType == DataFieldType.String ? 40 : 0; 
    
    /// <summary>
    /// The name of the default database connection
    /// </summary>
    static public string DefaultConnectionName { get; set; } = Sys.DEFAULT;

} 