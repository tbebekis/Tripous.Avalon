namespace Tripous.Data;

public class SelectDefs: SettingsBase
{
    private DbConnectionInfo ConInfo;
    private ObservableCollection<SelectDef> fList;

    string GetFolderPath()
    {
        string Temp = Path.Combine(SysConfig.AppFolderPath, ConInfo.Name);
        Temp = Path.Combine(Temp, "SelectDefs");
        return Temp;
    }
    void CheckConnectionInfo()
    {
        if (ConInfo == null)
            throw new Exception("DbConnectionInfo not set");
    }
    
    protected override string FileName => "selectdefs.json"; // not used
    protected override string GetFilePath() => Path.Combine(GetFolderPath(), FileName); // not used
    protected override void LoadBefore()
    {
        CheckConnectionInfo();
        List.Clear();
    }
    protected override void SaveBefore()
    {
        CheckConnectionInfo();
        base.SaveBefore();
    }

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public SelectDefs()
    {
    }
    public SelectDefs(DbConnectionInfo ConInfo)
    {
        this.ConInfo = ConInfo;
    }
    
    // ● public
    /// <summary>
    /// Loads the security settings from disk.
    /// </summary>
    public override void Load()
    {
        LoadBefore();
            
        if (!Directory.Exists(GetFolderPath()))
            return;
 
        SelectDef Def;
        string[] FilePaths = Sys.GetFiles(GetFolderPath(), "*.json", false);
        foreach (var FilePath in FilePaths)
        {
            Def = new SelectDef();
            Def.Load(FilePath);
            List.Add(Def);
        }
            
        IsLoaded  = true;
        LoadAfter();
    }
    /// <summary>
    /// Saves the security settings to disk.
    /// </summary>
    public override void Save()
    {
        SaveBefore();
            
        string DirectoryPath = Path.GetDirectoryName(FilePath);

        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        foreach (SelectDef Def in List)
        {
            Def.Save(FilePath);
        }
        
        SaveAfter();
    }
 
    // ● public
    public SelectDef Find(string Name) => List.FirstOrDefault(x => Name.IsSameText(x.Name));
    public bool Contains(string Name) => List.Any(x => Name.IsSameText(x.Name));

    public SelectDef Add(string Name, string Title, string Category, string SqlText)
    {
        CheckConnectionInfo();
        
        var Result = Find(Name);

        Result = new();
        Result.Name = Name;
        Result.Category = Category;
        Result.SqlText = SqlText;
        return Add(Result);
    }
    public SelectDef Add(SelectDef Item)
    {
        var Result = Find(Item.Name);
        if (Result != null)
            return Result;
        
        Result = Item;
        List.Add(Result);
        Save();
        
        return Result;
    }
    
    public bool Remove(string Name)
    {
        var Item = Find(Name);
        if (Item != null)
        {
            Remove(Item);
            return true;
        }
        
        return false;
    }
    public bool Remove(SelectDef Item)
    {
        string FilePath = Item.GetFilePath();
        if (File.Exists(FilePath))
            File.Delete(FilePath);
        
        List.Remove(Item);
        return true;
    }
    
    // ● properties
    public ObservableCollection<SelectDef> List
    {
        get
        {
            if (fList == null)
                fList = new();
            return fList;
        }
        set 
        {
            fList = value;
        }
    }

}