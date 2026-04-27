namespace Tripous.Desktop;

/// <summary>
/// The state of a data-form indicates the UI the form is currently displaying
/// </summary>
public enum DataFormState
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// List
    /// </summary>
    List = 1,
    /// <summary>
    /// Insert
    /// </summary>
    Insert = 2,
    /// <summary>
    /// Edit
    /// </summary>
    Edit = 4,
}
