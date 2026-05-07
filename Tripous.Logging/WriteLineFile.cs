namespace Tripous.Logging;

/// <summary>
/// Helper used in writing to files, line by line.
/// <para>It uses the fist line of a file as a column line, if one is passed.</para>
/// <para>Column names must be right padded with spaces according to a used line format.</para>
/// <para>A file may grow up to a defined maximum size (in kilobytes). </para>
/// <para>After that a new file is created.</para>
/// <para>The new file gets a name as <c>yyyy-MM-dd_HH_mm_ss__fff_DefaultFileName</c></para>
/// <para></para>
/// </summary>
public class WriteLineFile
{
    int fMaxSizeKiloBytes = 512;

    protected void BeginFile()
    {
        Size = 0;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        LastFileName = DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm_ss__fff_") + DefaultFileName;

        LastFilePath = Path.Combine(Folder, LastFileName);

        if (!string.IsNullOrWhiteSpace(ColumnLine) )
            File.WriteAllText(LastFilePath, ColumnLine);
    }

    // ● construction 
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="Folder">The folder where files are placed.</param>
    /// <param name="DefaultFileName">A file name to be used as template file name. 
    /// A date-time value is added in front of each new file name, i.e.<c>yyyy-MM-dd_HH_mm_ss__fff_DefaultFileName</c>
    /// </param>
    /// <param name="ColumnLine">The first line of a file may contain column names, as passed by this parameter. 
    /// Column names must be right padded with spaces according to a used line format. </param>
    /// <param name="MaxSizeKiloBytes">A file may grow up to a defined maximum size (in kilobytes). After that a new file is created.</param>
    public WriteLineFile(string Folder = "", string DefaultFileName = "", string ColumnLine = "", int MaxSizeKiloBytes = 512)
    {
        if (string.IsNullOrWhiteSpace(Folder))
            Folder = Logger.LogFolderPath;  

        if (string.IsNullOrWhiteSpace(DefaultFileName))
            DefaultFileName = "TripousLog";
        DefaultFileName = Path.ChangeExtension(DefaultFileName, ".log");
        
        if (string.IsNullOrWhiteSpace(ColumnLine))
             ColumnLine = Logger.GetLineCaptions();

        if (MaxSizeKiloBytes < 512)
            MaxSizeKiloBytes = Logger.MaxSizeKiloBytes;

        this.Folder = Folder;
        this.DefaultFileName = DefaultFileName;
        this.ColumnLine = ColumnLine;
        this.MaxSizeKiloBytes = MaxSizeKiloBytes;
    }

    // ● public 
    /// <summary>
    /// Writes a line to the <see cref="LastFilePath"/> file.
    /// </summary>
    /// <param name="Line"></param>
    public void WriteLine(string Line)
    {

        if (!string.IsNullOrWhiteSpace(Line))
        {
            if (string.IsNullOrWhiteSpace(LastFileName))
                BeginFile();

            Line = Line.Trim();
            Line += Environment.NewLine;
            File.AppendAllText(LastFilePath, Line);

            Size += System.Text.Encoding.Unicode.GetByteCount(Line);

            if (Size > (1024 * MaxSizeKiloBytes))
                LastFileName = null;
        }
    }
    /// <summary>
    /// Deletes files found in <see cref="Folder"/> older than a specified number of days.
    /// </summary>
    public void DeleteFilesOlderThan(int Days)
    {
        string[] FileList = Directory.GetFiles(Folder, "*.*");
        DateTime Now = DateTime.Now;
        DateTime FileCreationTime;
        int DaysDiff;

        foreach (string FilePath in FileList)
        {
            if (!Sys.IsSameText(LastFilePath, FilePath))
            {
                FileCreationTime = File.GetCreationTime(FilePath); // in local time
                if (Now > FileCreationTime)
                {
                    DaysDiff = (Now.Date - FileCreationTime.Date).Days;
                    if (DaysDiff > Days)
                    {
                        try
                        {
                            File.Delete(FilePath);
                        }
                        catch  
                        {
                        }
                    }
                }
            }

        }


    }

    // ● properties 
    /// <summary>
    /// The folder where files are placed. It is the folder of the file path specified in the constructor.
    /// </summary>
    public string Folder { get; }
    /// <summary>
    /// It is the file name of the file path specified in the constructor.
    /// </summary>
    public string DefaultFileName { get; }
    /// <summary>
    /// The full path to the current file, i.e. <see cref="Folder"/> + <see cref="LastFileName"/>
    /// </summary>
    public string LastFilePath { get; private set; }
    /// <summary>
    /// The current file. Its file name looks like <c>yyyy-MM-dd_HH_mm_ss__fff_DefaultFileName</c>.
    /// </summary>
    public string LastFileName { get; private set; }
    /// <summary>
    /// The first line of a file may contain column names, as passed by this parameter. Column names must be right padded with spaces according to a used line format. 
    /// </summary>
    public string ColumnLine { get; }
    /// <summary>
    /// The size in bytes of the current file.
    /// </summary>
    public int Size { get; private set; }
    /// <summary>
    /// The maximul allowed file size.
    /// </summary>
    public int MaxSizeKiloBytes
    {
        get
        {
            return fMaxSizeKiloBytes >= 512 ? fMaxSizeKiloBytes : 512;
        }
        set { fMaxSizeKiloBytes = value; }

    }
}

