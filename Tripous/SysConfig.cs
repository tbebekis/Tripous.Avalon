namespace Tripous;

static public class SysConfig
{
    static private string fAppName;
    static private string fAppExeFolderPath;
    static private string fAppFolderPath;
    static private string fAppDataFolderPath;
    static private string fAppTempFolderPath;
    static int fDefaultRowLimit;
    static private int fDefaultCommandTimeoutSeconds = 300;

    /// <summary>
    /// Returns the application name, e.g. MyApp
    /// <para>NOTE: Valid with desktop apps only.</para>
    /// </summary>
    static public string AppName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(fAppName))
                fAppName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]); // valid for desktop apps only
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
    static public object CompanyId { get; set; } 
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
    /// <summary>
    /// The default RowLimit for browser SELECTs.
    /// </summary>
    static public int DefaultRowLimit
    {
        get
        {
            if (fDefaultRowLimit >= 100 && fDefaultRowLimit <= 1500)
                return fDefaultRowLimit;
            return 300;
        }
        set { fDefaultRowLimit = value; }
    }
    static public int DefaultCommandTimeoutSeconds
    {
        get => fDefaultCommandTimeoutSeconds >= 15 ? fDefaultCommandTimeoutSeconds : 15;
        set => fDefaultCommandTimeoutSeconds = value;
    }
} 