namespace Tripous.Data;

/// <summary>
/// Represents a named group of table definitions.
/// <para>Used in constructing menus for displaying forms.</para>
/// </summary>
public class TableItemDefGroup: BaseDef
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public TableItemDefGroup()
    {
    }
    
    // ● public
    /// <summary>
    /// Finds a table item by name. It adds a new table item if the item is not found.
    /// </summary>
    public TableItemDef FindOrdAdd(string Name) => Items.FindOrdAdd(Name);
    /// <summary>
    /// Finds table items by name. It adds a new table item if an item is not found.
    /// </summary>
    public List<TableItemDef> FindOrAddRange(string[] Names) => Items.FindOrAddRange(Names);
    
    /// <summary>
    /// Creates a <see cref="Command"/> for each table item.
    /// <para>Used in constructing menus.</para>
    /// </summary>
    public List<Command> AsCommandList()
    {
        List<Command> Result = [];
        Command Cmd;
 
        foreach (var Item in Items)
        {
            Cmd = new() { Name = Item.Name, TitleKey = Item.TitleKey, Tag = Item };
            Result.Add(Cmd);
        }

        return Result;
    }

    // ● properties
    /// <summary>
    /// The list of items
    /// </summary>
    public DefList<TableItemDef> Items { get; set; } = new();
}