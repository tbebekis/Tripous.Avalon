namespace Tripous.Data;

/// <summary>
/// Result of a locator resolution operation.
/// </summary>
public class LocatorResult
{
    // ● private
    LocatorResultStatus fStatus;
    string fMessage;
    MemTable fTable;
    IList fObjectList;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorResult()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the result status.
    /// </summary>
    public LocatorResultStatus Status
    {
        get => fStatus;
        set => fStatus = value;
    }
    /// <summary>
    /// Gets or sets a message related to the result.
    /// </summary>
    public string Message
    {
        get => fMessage;
        set => fMessage = value;
    }
    /// <summary>
    /// Gets or sets the result table.
    /// </summary>
    public MemTable Table
    {
        get => fTable;
        set => fTable = value;
    }
    /// <summary>
    /// Gets the result table view.
    /// </summary>
    public DataView View => Table != null ? Table.DataView : null;
    /// <summary>
    /// Gets or sets the object result list.
    /// </summary>
    public IList ObjectList
    {
        get => fObjectList;
        set => fObjectList = value;
    }
    /// <summary>
    /// Gets the kind of result list returned by the operation.
    /// </summary>
    public LocatorResultListKind ListKind => Table != null ? LocatorResultListKind.MemTable : ObjectList != null ? LocatorResultListKind.ObjectList : LocatorResultListKind.None;
    /// <summary>
    /// Gets the result count.
    /// </summary>
    public int Count => Table != null ? Table.Rows.Count : ObjectList != null ? ObjectList.Count : 0;
    /// <summary>
    /// Gets true when the result has a single row.
    /// </summary>
    public bool HasSingleResult => Status == LocatorResultStatus.SingleResult && Count == 1;
    /// <summary>
    /// Gets true when the result has multiple rows.
    /// </summary>
    public bool HasMultipleResults => Status == LocatorResultStatus.MultipleResults && Count > 1;
    /// <summary>
    /// Gets true when the result is too broad.
    /// </summary>
    public bool HasTooManyResults => Status == LocatorResultStatus.TooManyResults;
    /// <summary>
    /// Gets true when the result is an error.
    /// </summary>
    public bool HasError => Status == LocatorResultStatus.Error;
}
