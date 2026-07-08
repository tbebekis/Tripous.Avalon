namespace Tripous.Data;

/// <summary>
/// The kind of result list returned by a locator resolution operation.
/// </summary>
public enum LocatorResultListKind
{
    /// <summary>
    /// No result list is assigned.
    /// </summary>
    None,
    /// <summary>
    /// The result list is a <see cref="MemTable"/>.
    /// </summary>
    MemTable,
    /// <summary>
    /// The result list is an object list.
    /// </summary>
    ObjectList,
}
