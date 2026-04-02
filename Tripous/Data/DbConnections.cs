using System.Collections.ObjectModel;

namespace Tripous.Data;

/// <summary>
/// A container of <see cref="DbConnectionInfo"/> objects.
/// <para>This object is saved to a JSON file.</para>
/// </summary>
public class DbConnections: SettingsBase
{
    private ObservableCollection<DbConnectionInfo> fList;
    
    protected override string FileName => "connections.json";
    protected override void LoadBefore()
    {
        List.Clear();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public DbConnections()
    {
    }
    
    // ● public
    public DbConnectionInfo Find(string Name) => List.FirstOrDefault(x => Name.IsSameText(x.Name));
    public bool Contains(string Name) => List.Any(x => Name.IsSameText(x.Name));

    public DbConnectionInfo Add(string Name, DbServerType dbServerType, string ConnectionString, int CommandTimeoutSeconds = DbConnectionInfo.DefaultCommandTimeoutSeconds)
    {
        var Result = Find(Name);

        Result = new();
        Result.Name = Name;
        Result.DbServerType = dbServerType;
        Result.ConnectionString = ConnectionString;
        Result.CommandTimeoutSeconds = CommandTimeoutSeconds;
        return Add(Result);
    }
    public DbConnectionInfo Add(DbConnectionInfo Item)
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
        var Result = Find(Name);
        if (Result != null)
        {
            List.Remove(Result);
            Save();
            return true;
        }
        
        return false;
    }
    public bool Remove(DbConnectionInfo Item)
    {
        return Remove(Item.Name);
    }
    
    // ● properties
    /// <summary>
    /// The list of connection info objects
    /// </summary>
    public ObservableCollection<DbConnectionInfo> List 
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