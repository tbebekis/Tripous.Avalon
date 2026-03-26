namespace Tripous.Data;

/// <summary>
/// Indicates the mode of a form or business object
/// </summary>
[Flags]
public enum DataMode
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Browse
    /// </summary>
    Browse = 1,

    /// <summary>
    /// Insert
    /// </summary>
    Insert = 2,
    /// <summary>
    /// Edit
    /// </summary>
    Edit = 4,
    /// <summary>
    /// Delete
    /// </summary>
    Delete = 8,

    /// <summary>
    /// Commit
    /// </summary>
    Commit = 0x10,
    /// <summary>
    /// Cancel
    /// </summary>
    Cancel = 0x20,
 
}