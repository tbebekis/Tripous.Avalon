namespace Tripous.Desktop;

/// <summary>
/// UI information regarding a single-row <see cref="TableDef"/> in an <see cref="ItemPage"/> form.
/// </summary>
public class UiTableInfo
{
    // ● public
    /// <summary>
    /// The table definition.
    /// </summary>
    public TableDef TableDef { get; set; }
    /// <summary>
    /// <see cref="FieldDef"/> to <see cref="Control"/> association list, for top tables and IsOneToOne = true single-row detail tables.
    /// </summary>
    public List<UiFieldInfo> FieldList { get; set; } = new();
    /// <summary>
    /// Visible one-to-one detail tables associated with this table tree.
    /// </summary>
    public List<UiTableInfo> OneToOneList { get; } = new();
    /// <summary>
    /// Visible multi-row detail tables associated with this table tree.
    /// </summary>
    public List<UiDetailTableInfo> DetailList { get; } = new();
    /// <summary>
    /// The table.
    /// </summary>
    public MemTable Table { get; set; }
}
