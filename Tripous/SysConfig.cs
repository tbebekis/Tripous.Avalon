namespace Tripous
{
    static public class SysConfig
    {
        static private string fAppName;
        static private string fAppExeFolderPath;
        static private string fAppFolderPath;
        static private string fAppDataFolderPath;
        static private string fAppTempFolderPath;

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
 
    } 
}

