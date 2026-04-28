namespace Tripous.Data;

/// <summary>
/// A set of flags that dictate the behavior of a <see cref="TableSet"/> object.
/// </summary>
[Flags]
public enum TableSetFlags
{
    /// <summary>
    /// None of the flags is set
    /// </summary>
    None = 0,
    /// <summary>
    /// Indicates that TableSet should use its own methods to generate Sql statements for the table tree.
    /// </summary>
    GenerateSql = 1,
    /// <summary>
    /// Indicates that pessimistic mode should be used. That is to flag a row in the database while it is selected
    /// and de-flag it when saved/deleted.
    /// </summary>
    PessimisticMode = 2,
    /// <summary>
    /// Indicates that deletes should happen top to bottom, so if any database foreign constraint exists, then let
    /// an exception to be thrown.
    /// </summary>
    NoCascadeDeletes = 4,
}