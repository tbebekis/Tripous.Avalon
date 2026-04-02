namespace Tripous.Avalon;

/// <summary>
/// A bindable row adapter used by the pivot grid renderer.
/// </summary>
public class PivotGridRow
{
    private readonly PivotDataRow fRow;
    private readonly Dictionary<string, int> fIndexes;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PivotGridRow(PivotDataRow row, Dictionary<string, int> indexes)
    {
        fRow = row ?? throw new ArgumentNullException(nameof(row));
        fIndexes = indexes ?? throw new ArgumentNullException(nameof(indexes));
    }

    /// <summary>
    /// Gets a cell value by pivot column key.
    /// </summary>
    public object this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            return fIndexes.TryGetValue(key, out int index) ? fRow.Values[index] : null;
        }
    }

    /// <summary>
    /// The source pivot row.
    /// </summary>
    public PivotDataRow Row => fRow;

    /// <summary>
    /// The row type.
    /// </summary>
    public PivotDataRowType RowType => fRow.RowType;

    /// <summary>
    /// The subtotal level or -1.
    /// </summary>
    public int Level => fRow.Level;
}