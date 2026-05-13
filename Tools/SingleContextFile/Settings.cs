namespace SingleContextFile;

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
 
    public string StartFolderPath { get; set; } = "";
    public string Text  { get; set; } = "";
}