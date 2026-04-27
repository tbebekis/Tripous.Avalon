namespace Tripous.Data;

/// <summary>
/// Flags indicating the behavior of Sql statement generators
/// </summary>
[Flags]
public enum BuildSqlFlags
{
    /// <summary>
    /// Container flag
    /// </summary>
    None = 0,
    /// <summary>
    /// When is set indicates that the OIDs must be created before an INSERT statement execution.
    /// <para>This is the case with an Oracle sequencer or Firebird generator or when the OID is a Guid or anyother string.</para> 
    /// </summary>
    OidModeIsBefore = 1,
    /// <summary>
    /// When is set indicates that the OID is a Guid string.  
    /// </summary>
    GuidOids = 2,
    /// <summary>
    /// When is set indicates that blob fields must included into the SELECT of the List (browser) part.
    /// </summary>
    IncludeBlobFields = 4,
    /// <summary>
    /// When is set indicates that look up DataTables must created too, along with Sql generation
    /// </summary>
    CreateLookUpTables = 8,
 
}