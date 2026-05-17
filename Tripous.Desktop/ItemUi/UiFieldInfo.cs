namespace Tripous.Desktop;

/// <summary>
/// Ui information regarding the associaton of a <see cref="FieldDef"/> and a <see cref="Control"/>
/// </summary>
public class UiFieldInfo
{
    // ● public
    /// <summary>
    /// The table definition.
    /// </summary>
    public TableDef TableDef { get; set; }
    /// <summary>
    /// The field definition
    /// </summary>
    public FieldDef FieldDef { get; set; }
    /// <summary>
    /// The control
    /// </summary>
    public Control Control { get; set; }
    /// <summary>
    /// The field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The table.
    /// </summary>
    public MemTable Table { get; set; }
}