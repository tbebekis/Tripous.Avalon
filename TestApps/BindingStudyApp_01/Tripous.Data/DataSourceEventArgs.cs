namespace Tripous.Data;

/// <summary>
/// Provides data for DataSource row events.
/// </summary>
public class DataSourceRowEventArgs: EventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceRowEventArgs(DataSourceRow Row)
    {
        this.Row = Row;
    }

    // ● properties
    /// <summary>
    /// Gets the row associated with the event.
    /// </summary>
    public DataSourceRow Row { get; }
    /// <summary>
    /// Gets the underlying item associated with the row.
    /// </summary>
    public object InnerObject => Row?.InnerObject;
}

/// <summary>
/// Provides cancelable event data for DataSource operations.
/// </summary>
public class DataSourceCancelEventArgs: EventArgs
{
    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether the operation is canceled.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets or sets the cancellation reason.
    /// </summary>
    public string Reason { get; set; }
}

/// <summary>
/// Provides cancelable event data for DataSource row operations.
/// </summary>
public class DataSourceRowCancelEventArgs: DataSourceRowEventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceRowCancelEventArgs(DataSourceRow Row)
        : base(Row)
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether the operation is canceled.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets or sets the cancellation reason.
    /// </summary>
    public string Reason { get; set; }
}

/// <summary>
/// Provides data for DataSource field value changes.
/// </summary>
public class DataSourceChangeEventArgs: DataSourceRowCancelEventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceChangeEventArgs(DataSourceRow Row, string FieldName, object OldValue, object NewValue)
        : base(Row)
    {
        this.FieldName = FieldName;
        this.OldValue = OldValue;
        this.NewValue = NewValue;
    }

    // ● properties
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string FieldName { get; }
    /// <summary>
    /// Gets the old field value.
    /// </summary>
    public object OldValue { get; }
    /// <summary>
    /// Gets the new field value.
    /// </summary>
    public object NewValue { get; }
}

/// <summary>
/// Provides data for DataSource item creation.
/// </summary>
public class DataSourceCreateEventArgs: EventArgs
{
    // ● properties
    /// <summary>
    /// Gets or sets a caller-provided underlying item.
    /// </summary>
    public object InnerObject { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether creation is canceled.
    /// </summary>
    public bool Cancel { get; set; }
    /// <summary>
    /// Gets or sets the cancellation reason.
    /// </summary>
    public string Reason { get; set; }
}

/// <summary>
/// Provides data for events involving an underlying item.
/// </summary>
public class DataSourceInnerObjectEventArgs: EventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceInnerObjectEventArgs(object InnerObject)
    {
        this.InnerObject = InnerObject;
    }

    // ● properties
    /// <summary>
    /// Gets the underlying item.
    /// </summary>
    public object InnerObject { get; }
}
