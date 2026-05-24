namespace Tripous.Data;

/// <summary>
/// Defines how a DataSource handles detail rows when a master row is deleted.
/// </summary>
public enum CascadeDeleteRule
{
    /// <summary>
    /// Deletes the master row without checking detail rows.
    /// </summary>
    None,
    /// <summary>
    /// Blocks master deletion when matching detail rows exist.
    /// </summary>
    Restrict,
    /// <summary>
    /// Deletes matching detail rows before deleting the master row.
    /// </summary>
    Cascade
}
