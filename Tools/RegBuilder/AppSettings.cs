namespace RegBuilder;

public class AppSettings
{
    readonly string FolderPath = AppContext.BaseDirectory;
    readonly string FileName = "AppSettings.json";
    string FilePath;
    TripousList<RegBuilderProject> fProjects;
    
    public AppSettings()
    {
        FilePath = Path.Combine(FolderPath, FileName);
    }

    public void Load() => Json.LoadFromFile(this, FilePath);
    public void Save() => Json.SaveToFile(this, FilePath);

    public TripousList<RegBuilderProject> Projects
    {
        get
        {
            if (fProjects == null)
                fProjects = new();
            return fProjects;
        }
        set => fProjects = value;
    }
}