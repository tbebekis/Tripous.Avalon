namespace Tripous;

static public class Sys
{
    // ●  constants  
    
    #region ISO DateTime formats array
    /// <summary>
    /// An array of ISO8601 datetime formats 
    /// </summary>
    static public readonly string[] ISODateTimeFormats = new string[] {
        "yyyy-MM-dd HH:mm:ss.fffffff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",
        "yyyyMMddHHmmss",
        "yyyyMMddHHmm",
        "yyyyMMddTHHmmssfffffff",
        "yyyy-MM-dd",
        "yy-MM-dd",
        "yyyyMMdd",
        "HH:mm:ss",
        "HH:mm",
        "THHmmss",
        "THHmm",
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss.ffffff",
        "HH:mm:ss.fff"
    };
    #endregion
    
   
    public const string None = "[none]";
    public const string NULL = "___null___"; 
    public const string DEFAULT = "Default";
    public const string SYSTEM = "System";
    public const string APPLICATION = "Application";
    public const string MASTER_KEY_FIELD_NAME = "MASTER_KEY_FIELD_NAME";
    public const string NamePathSep = ".";
    public const string FieldAliasSep = "__"; 
    public const string FromField = "_FROM";
    public const string ToField = "_TO";
    public const string StandardCompanyGuid = "74772779-BF08-4B22-8F87-196FB87EC7C2";
    public const string InvalidId = "27C15428-7892-4F7D-B28F-9BA059C94BA4";
    public const string EnId = "D4997C35-6E89-499A-87BF-D5750D0D3F06";
    public const string GrId = "92A158E7-25CA-4367-BA57-FB79C40D775C";
    
    
    // ●  public 
    /// <summary>
    /// Throws an Exception
    /// </summary>
    static public void Throw(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            Text = "Unknown error";

        throw (new Exception(Text));
    }
    /// <summary>
    /// Throws an Exception
    /// </summary>
    static public void Throw(string Text, params object[] Args)
    {
        if ((Args != null) && (Args.Length > 0))
            Text = string.Format(Text, Args);
        throw (new Exception(Text));
    }    
    
    /// <summary>
    /// Unlocks all Code Pages (Greek, DOS, etc.) for the entire application.
    /// </summary>
    public static void RegisterAllEncodings()
    {
        // This line is the "magic" one.
        // Registers the CodePagesEncodingProvider, which contains
        // hundreds of legacy encodings that are not standard in .NET Core/5+.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    
    // ●  Convertions 
    /// <summary>
    /// Returns true if Value is null or DBNull
    /// </summary>
    static public bool IsNull(object Value)
    {
        return (Value == null) || (DBNull.Value == Value);
    }    
    
    /// <summary>
    /// Formats and returns a double value
    /// </summary>
    static public string DoubleToStr(double Value, int Digits = 4)
    {
        return Value.ToString("0." + new string('0', Digits));
    }
    /// <summary>
    /// Formats and returns a double value
    /// </summary>
    static public string DecimalToStr(decimal Value, int Digits = 4)
    {
        return Value.ToString("0." + new string('0', Digits));
    }
    /// <summary>
    /// Converts a datetime into a string
    /// </summary>
    static public string DateTimeToStr(DateTime Value, bool UseMSecs = false)
    {
        return UseMSecs ? Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : Value.ToString("yyyy-MM-dd HH:mm:ss");
    }
    /// <summary>
    /// Converts a date into a string
    /// </summary>
    static public string DateToStr(DateTime Value)
    {
        return Value.ToString("yyyy-MM-dd");
    }
    
    /// <summary>
    /// Returns the Value as a value of T. If is null returns Default.
    /// </summary>
    static public T AsValue<T>(object Value, T Default)
    {
        try
        {
            if (IsNull(Value))
                return Default;

            if (Default != null && Default.GetType().IsValueType)
            {
                Type DefaultType = Default.GetType();

                if (Value.GetType() == DefaultType)
                    return (T)Value;

                if (DefaultType.ImplementsInterface(typeof(IConvertible)))
                    return (T)System.Convert.ChangeType(Value, DefaultType, CultureInfo.InvariantCulture);                    
            }

            return (T)Value;

        }
        catch
        {
            return Default;
        }
    }

    /// <summary>
    /// Converts a value to string, if possible, else returns Default
    /// </summary>
    static public string AsString(object Value, string Default = "")
    {
        return !IsNull(Value) ? Value.ToString() : Default;
    }
    /// <summary>
    /// Converts a value to int, if possible, else returns Default
    /// </summary>
    static public int AsInteger(object Value, int Default = 0)
    {
        try
        {
            return !IsNull(Value) ? Convert.ToInt32(Value) : Default;
        }
        catch
        {
        }

        return Default;
    }
    /// <summary>
    /// Converts a value to bool, if possible, else returns Default
    /// </summary>
    static public bool AsBoolean(object Value, bool Default = false)
    {
        try
        {
            return !IsNull(Value) ? Convert.ToBoolean(Value) : Default;
        }
        catch
        {
        }

        return Default;

    }
    /// <summary>
    /// Converts a value to double, if possible, else returns Default
    /// </summary>
    static public double AsDouble(object Value, double Default = 0)
    {
        try
        {
            if (IsNull(Value))
                return 0;

            if (Value.GetType() == typeof(string))
                return Convert.ToDouble(Value.ToString(), CultureInfo.InvariantCulture);

            return Convert.ToDouble(Value);
        }
        catch
        {
        }


        return Default;
    }
    /// <summary>
    /// Converts a value to decimal, if possible, else returns Default
    /// </summary>
    static public decimal AsDecimal(object Value, decimal Default = 0)
    {
        try
        {
            if (IsNull(Value))
                return Default;

            if (Value.GetType() == typeof(string))
                return Convert.ToDecimal(Value.ToString(), CultureInfo.InvariantCulture);

            return Convert.ToDecimal(Value);
        }
        catch
        {
        }

        return Default;
    }
    /// <summary>
    /// Converts a value to DateTime, if possible, else returns Default
    /// </summary>
    static public DateTime AsDateTime(object Value, DateTime Default)
    {
        try
        {
            return !IsNull(Value) ? StrToDateTime(Value.ToString()): Default ;
        }
        catch
        {                
        }

        return Default;
    }

    /// <summary> 
    /// Converts a string to a DateTime value. The string must be defined in one of the ISODateTimeFormats
    /// </summary>
    public static DateTime StrToDateTime(string S)
    {
        try
        {
            return DateTime.Parse(S);
        }
        catch
        {
            return DateTime.ParseExact(S, ISODateTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
        }
    }
    /// <summary> 
    /// Converts a string to a DateTime value. The string must be defined in one of the ISODateTimeFormats
    /// </summary>
    public static DateTime StrToDateTime(string S, DateTime Default)
    {
        DateTime Result = DateTime.Now;
        if (TryStrToDateTime(S, out Result))
            return Result;
        return Default;
    }
    /// <summary> 
    /// Converts a string to a DateTime.Date value. The string must be defined in one of the ISODateTimeFormats
    /// </summary>
    public static DateTime StrToDate(string S, DateTime Default)
    {
        return StrToDateTime(S, Default).Date;
    }
    /// <summary>
    /// Converts a string into a DateTime. Returns true on success.
    /// </summary>
    public static bool TryStrToDateTime(string S, out DateTime Value)
    {
        Value = DateTime.MinValue;

        bool Result = DateTime.TryParse(S, out Value);
 
        if (!Result)
        {
            try
            {
                Value = DateTime.ParseExact(S, ISODateTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                Result = true;
            }
            catch
            {
            }
        }

        return Result;
    }    
    
    // ●  Paths and Files  
    /// <summary>
    /// Removes a trailing slash mark (e.g. c:\Temp\ ) from a file path.
    /// </summary>
    static public string RemoveTrailingSlash(string FilePath)
    {
        return string.IsNullOrWhiteSpace(FilePath)? string.Empty: FilePath.TrimEnd(new[] { '\\', '/',  });
    }

    /// <summary>
    /// Returns an array containing the characters that are not allowed in file names.
    /// </summary>
    static public char[] GetInvalidFileNameChars()
    {
        char[] InvalidFileChars = Path.GetInvalidFileNameChars();
        return InvalidFileChars;
    }
    /// <summary>
    /// Returns true if FileName is a valid file name, that is it just contains
    /// characters that are allowed in file names.
    /// </summary>
    static public bool IsValidFileName(string FileName)
    {
        char[] InvalidFileChars = GetInvalidFileNameChars();
        return FileName.IndexOfAny(InvalidFileChars) == -1;
    }
    /// <summary>
    /// Replaces any invalid file name characters from Source with spaces.
    /// </summary>
    static public string StrToValidFileName(string Source)
    {
        char[] InvalidFileChars = GetInvalidFileNameChars();
        StringBuilder SB = new StringBuilder(Source);
        foreach (char C in InvalidFileChars)
            SB.Replace(C, ' ');
        return SB.ToString();

    }

    /// <summary>
    /// Returns file names from the specified Folder that match the specified Filters.
    /// <para>Filters is a list of filters delimited by semicolon, i.e. *.gif;*.jpg;*.png;*.bmp; </para>
    /// <para> The returned file names include the full path.</para>
    /// </summary>
    static public string[] GetFiles(string Folder, string Filters, bool SearchSubFolders = false)
    {
        List<string> List = new List<string>();

        SearchOption SearchOption = SearchSubFolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
        string[] FilterItems = Filters.Split(';');

        foreach (string Filter in FilterItems)
        {
            List.AddRange(Directory.GetFiles(Folder, Filter, SearchOption));
        }

        return List.ToArray();
    }
    /// <summary>
    /// Displays the file explorer on a path
    /// </summary>
    public static void OpenFileExplorer(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        // Normalization of separators for the operating system
        path = Path.GetFullPath(path);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (File.Exists(path))
            {
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            }
            else if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", $"\"{path}\"");
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // On Linux (for xed, etc editors), we use xdg-open
            // Note: Linux usually opens the folder, it doesn't easily select a file
            string dir = File.Exists(path) ? Path.GetDirectoryName(path) : path;
            Process.Start("xdg-open", $"\"{dir}\"");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // On macOS we use the open command
            string args = File.Exists(path) ? $"-R \"{path}\"" : $"\"{path}\"";
            Process.Start("open", args);
        }
    }
    
    // ●  miscs 
    /// <summary>
    /// Creates and returns a new Guid.
    /// <para>If UseBrackets is true, the new guid is surrounded by {}</para>
    /// </summary>
    static public string GenId(bool UseBrackets)
    {
        string format = UseBrackets ? "B" : "D";
        return Guid.NewGuid().ToString(format).ToUpper();
    }
    /// <summary>
    /// Creates and returns a new Guid WITHOUT surrounding brackets, i.e. {}
    /// </summary>
    static public string GenId()
    {
        return GenId(false);
    }    
    /// <summary>
    /// Creates and returns a random string of a specified length, picking characters from a specified set of characters.
    /// </summary>
    static public string GenerateRandomString(int Length, string CharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        if (string.IsNullOrWhiteSpace(CharSet))
            CharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        char[] Buffer = new char[Length];
        Random R = new Random();

        for (int i = 0; i < Buffer.Length; i++)
        {
            Buffer[i] = CharSet[R.Next(CharSet.Length)];
        }

        string Result = new string(Buffer);
        return Result;
    }
    
    /// <summary>
    /// Case insensitive string equality.
    /// <para>Returns true if 1. both are null, 2. both are empty string or 3. they are the same string </para>
    /// </summary>
    static public bool IsSameText(string A, string B)
    {
        // Compare() returns true if 1. both are null, 2. both are empty string or 3. they are the same string
        return string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0;
    }

 
}   