namespace Tripous.Data;

/// <summary>
/// Provides data for DataSource position changes.
/// </summary>
public class DataSourcePositionEventArgs: EventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourcePositionEventArgs(int OldPosition, int NewPosition, DataSourceRow OldCurrent, DataSourceRow NewCurrent)
    {
        this.OldPosition = OldPosition;
        this.NewPosition = NewPosition;
        this.OldCurrent = OldCurrent;
        this.NewCurrent = NewCurrent;
    }

    // ● properties
    /// <summary>
    /// Gets the old position.
    /// </summary>
    public int OldPosition { get; }
    /// <summary>
    /// Gets the new position.
    /// </summary>
    public int NewPosition { get; }
    /// <summary>
    /// Gets the old current row.
    /// </summary>
    public DataSourceRow OldCurrent { get; }
    /// <summary>
    /// Gets the new current row.
    /// </summary>
    public DataSourceRow NewCurrent { get; }
}

/// <summary>
/// Provides cancelable data for DataSource position changes.
/// </summary>
public class DataSourcePositionCancelEventArgs: DataSourcePositionEventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourcePositionCancelEventArgs(int OldPosition, int NewPosition, DataSourceRow OldCurrent, DataSourceRow NewCurrent)
        : base(OldPosition, NewPosition, OldCurrent, NewCurrent)
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether the position change is canceled.
    /// </summary>
    public bool Cancel { get; set; }
}
