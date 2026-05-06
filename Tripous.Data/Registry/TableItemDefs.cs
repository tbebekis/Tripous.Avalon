namespace Tripous.Data;


/// <summary>
/// Tables by group.
/// <para>A client code may add one or more groups, and then add <see cref="TableItemDef"/> items to that group.</para>
/// <para>Used in constructing menus.</para>
/// </summary>
public class TableItemDefs
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public TableItemDefs()
    {
    }

    // ● public
    /// <summary>
    /// Finds a group item by name. It adds a new group item if the item is not found.
    /// </summary>
    public TableItemDefGroup FindOrdAddGroup(string Name) => Groups.FindOrdAdd(Name);
    /// <summary>
    /// Finds a group by name. It adds a new group item if an item is not found.
    /// <para>It then continues and adds a sequence of table names in that group, if not already added.</para>
    /// </summary>
    public TableItemDefGroup FindOrAddWithTableRange(string GroupName, string[] TableNames)
    {
        TableItemDefGroup Result = FindOrdAddGroup(GroupName);
        Result.FindOrAddRange(TableNames);
        return Result;
    }
    /// <summary>
    /// Finds a group by name. It adds a new group item if an item is not found.
    /// <para>It then continues and adds a table item in that group, if not already added.</para>
    /// </summary>
    public TableItemDef FindOrAddTable(string GroupName, string TableName)
    {
        TableItemDefGroup Group = FindOrdAddGroup(GroupName);
        TableItemDef Result = Group.FindOrdAdd(TableName);
        return Result;
    }
    

    /// <summary>
    /// Returns a flag list of all table items of all of the groups
    /// </summary>
    /// <returns></returns>
    public List<TableItemDef> GetAllTables()
    {
        List<TableItemDef> Result = new();
        foreach (TableItemDefGroup Group in Groups)
            Result.AddRange(Group.Items);
        return Result;
    }
    /// <summary>
    /// Creates a <see cref="Command"/> for each item.
    /// <para>Used in constructing menus.</para>
    /// </summary>
    public List<Command> AsCommandList()
    {
        List<Command> Result = [];
        Command Cmd;
        List<TableItemDef> TableList = GetAllTables();
        foreach (var Item in TableList)
        {
            Cmd = new() { Name = Item.Name, TitleKey = Item.TitleKey, Tag = Item };
            Result.Add(Cmd);
        }

        return Result;
    }
    
    // ● properties
    /// <summary>
    /// The list of groups.
    /// </summary>
    public DefList<TableItemDefGroup> Groups { get; set; } = new();
}