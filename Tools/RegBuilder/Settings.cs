namespace RegBuilder;

public class Settings
{
    readonly string FolderPath = AppContext.BaseDirectory;
    readonly string FileName = "Settings.json";
    string FilePath;
    
    public Settings()
    {
        FilePath = Path.Combine(FolderPath, FileName);
    }

    public void Load() => Json.LoadFromFile(this, FilePath);
    public void Save() => Json.SaveToFile(this, FilePath);

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(SourceFilePath)
            || !File.Exists(SourceFilePath)
            || SchemaVersion <= 0
           )
            return false;

        return true;
    }


    public string SourceFilePath { get; set; } = "/home/teo/Dev/CSharp/Tripous.Avalon/SampleApps/TinyERP/tERP.Data/Schema01.sql";
    public int SchemaVersion { get; set; } = 1;
    //public string StartFolderPath { get; set; } = "";
    //public string Text  { get; set; } = "";
}