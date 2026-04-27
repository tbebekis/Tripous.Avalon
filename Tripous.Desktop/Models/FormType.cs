namespace Tripous.Desktop;

/// <summary>
/// Indicates the structural type and capabilities of a data form.
/// </summary>
public enum FormType
{
    /// <summary>
    /// None
    /// </summary>
    None,
    /// <summary>
    /// Indicates a "list" form.
    /// A list form has a single part displaying a read-write grid.
    /// </summary>
    List,
    /// <summary>
    /// Indicates a "master list" form.
    /// Provides two parts: a list (read-only grid) and an item (data entry controls).
    /// Both parts are bound to the SAME DataTable.
    /// </summary>
    ListMaster,
    /// <summary>
    /// Indicates a "master" form.
    /// Provides two parts: a list (read-only grid) and an item (data entry controls).
    /// Each part is bound to DIFFERENT DataTable objects.
    /// </summary>
    Master,
}