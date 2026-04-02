using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PivotTestApp;

public class PivotDef
{
    // ● construction
    public PivotDef()
    {
    }
    
    // ● public
    public override string ToString()
    {
        return !string.IsNullOrWhiteSpace(Name)
            ? Name
            : base.ToString();
    }
    public void ClearLists()
    {
        RowFields.Clear();
        ColumnFields.Clear();
        ValueFields.Clear();
    }


    
    // ● properties
    /// <summary>
    /// The name of this pivot definition.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Field names placed on the row axis.
    /// </summary>
    public List<string> RowFields { get; set; } = new();
    /// <summary>
    /// Field names placed on the column axis.
    /// </summary>
    public List<string> ColumnFields { get; set; } = new();
    /// <summary>
    /// Value field definitions.
    /// </summary>
    public List<PivotValueDef> ValueFields { get; set; } = new();
    /// <summary>
    /// Indicates whether row subtotals are shown.
    /// </summary>
    public bool ShowSubtotals { get; set; } = true;
    /// <summary>
    /// Indicates whether grand totals are shown.
    /// </summary>
    public bool ShowGrandTotals { get; set; } = true;
    /// <summary>
    /// Indicates whether values are displayed on rows instead of columns.
    /// </summary>
    public bool ShowValuesOnRows { get; set; }
    [JsonIgnore]
    public object Tag { get; set; }
 
}