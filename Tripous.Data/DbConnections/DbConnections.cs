/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// A container of <see cref="DbConnectionInfo"/> objects.
/// <para>This object is saved to a JSON file.</para>
/// </summary>
public class DbConnections: SettingsBase
{
    ObservableCollection<DbConnectionInfo> fList;
    
    /// <summary>
    /// Gets the file name used for persistence.
    /// </summary>
    protected override string FileName => "DbConnections.json";
    /// <summary>
    /// Clears the current list before loading from storage.
    /// </summary>
    protected override void LoadBefore()
    {
        List.Clear();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public DbConnections()
    {
    }
    
    // ● public
    /// <summary>
    /// Finds a connection by name.
    /// </summary>
    public DbConnectionInfo Find(string Name) => List.FirstOrDefault(x => Name.IsSameText(x.Name));
    /// <summary>
    /// Returns a connection by name or raises an exception when not found.
    /// </summary>
    public DbConnectionInfo Get(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousDataException($"Connection Name is empty");
        
        DbConnectionInfo Result  = List.FirstOrDefault(x => Name.IsSameText(x.Name));
        if (Result == null)
            throw new TripousDataException($"Cannot get {typeof(DbConnectionInfo)}: {Name}");
        return Result;
    }
    /// <summary>
    /// Returns true if a connection with the specified name exists.
    /// </summary>
    public bool Contains(string Name) => List.Any(x => Name.IsSameText(x.Name));
    /// <summary>
    /// Creates and adds a connection definition.
    /// </summary>
    public DbConnectionInfo Add(string Name, DbServerType dbServerType, string ConnectionString, int CommandTimeoutSeconds)
    {
        var Result = Find(Name);

        Result = new();
        Result.Name = Name;
        Result.DbServerType = dbServerType;
        Result.ConnectionString = ConnectionString;
        Result.CommandTimeoutSeconds = CommandTimeoutSeconds;
        return Add(Result);
    }
    /// <summary>
    /// Adds a connection definition if it does not already exist.
    /// </summary>
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
    /// <summary>
    /// Removes a connection definition by name.
    /// </summary>
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
    /// <summary>
    /// Removes a connection definition.
    /// </summary>
    public bool Remove(DbConnectionInfo Item)
    {
        return Remove(Item.Name);
    }
    
    // ● properties
    /// <summary>
    /// Gets or sets the list of connection definitions.
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