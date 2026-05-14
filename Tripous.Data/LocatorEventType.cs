namespace Tripous.Data;

/// <summary>
/// Indicates the type of a locator event
/// </summary>
[Flags]
public enum LocatorEventType
{
    /// <summary>
    /// In both modes. Occurs when the SELECT is already constructed. 
    /// <para>Gives a chance to any client code to add special where to the passed SELECT just before execution</para>
    /// </summary>
    AddToWhere = 1,
    /// <summary>
    /// In both modes. Occurs when the locator needs to execute a SELECT.
    /// <para>The client may execute the passed SELECT, or any other statement,
    /// and assign the <see cref="Locator.SourceTable"/></para>
    /// </summary>
    SelectSourceTable = 2,
    /// <summary>
    /// Gives a chance to a client code to configure source table columns, titles, visibility etc.
    /// </summary>
    SetupSourceTable = 4,
    /// <summary>
    /// Occurs when the SourceTable.DefaultView must be filtered.
    /// </summary>
    FilterSourceTable = 8,
    /// <summary>
    /// Occurs when the DataValue property has changed its value
    /// </summary>
    DataValueChanged = 0x10,
 
}