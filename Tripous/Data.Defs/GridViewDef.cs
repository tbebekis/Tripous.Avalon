 namespace Tripous.Data;

public class GridViewDef
{
    // ● construction
    public GridViewDef()
    {
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name: base.ToString();
    public string GetDescription()
    {
        StringBuilder SB = new();
        
        // group
        string S = string.Join(", ", GroupList);
        SB.AppendLine($"Group: [{S}]");
            
        // hidden columns
        S = string.Join(", ", HiddenList);
        SB.AppendLine($"Hidden: {S}");
            
        // summaries
        S = string.Join(", ", Summaries
            .Where(x => x.Value != AggregateType.None)
            .Select(x => $"{x.Key} = {x.Value}"));
        SB.AppendLine($"Summaries: {S}");
            
        // row filters
        S = RowFilters.Text;
        SB.AppendLine($"RowFilter: {S}");
            
        S = SB.ToString();
        return S;
    }
    public void ClearLists()
    {
        OrderList.Clear();
        HiddenList.Clear();
        GroupList.Clear();
        Summaries.Clear();
        RowFilters.Clear();
    }
    
    // ● properties
    /// <summary>
    /// The name of this grid view
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// All column names with the order they are displayed in the grid
    /// </summary>
    public List<string> OrderList { get; set; } = new();
    /// <summary>
    /// Column names of hidden columns, if any.
    /// </summary>
    public List<string> HiddenList { get; set; } = new();
    /// <summary>
    /// Column names with their order in the group, if any.
    /// </summary>
    public List<string> GroupList { get; set; } = new();
    /// <summary>
    /// The <see cref="AggregateType"/> of all column names, as they found in the <see cref="OrderList"/>.
    /// </summary>
    public Dictionary<string, AggregateType> Summaries { get; set; } = new();
    /// <summary>
    /// RowFilter definitions for columns participating in the DataView filtering.
    /// </summary>
    public RowFilterDefs RowFilters { get; set; } = new();
    
    [JsonIgnore]
    public object Tag { get; set; }
}