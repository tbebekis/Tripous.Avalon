namespace Tripous.Avalon.Data;

/// <summary>
/// Provides data for standard events associated with a DataSourceRow.
/// </summary>
public class BindingSourceArgs : EventArgs
{
    /// <summary>
    /// Gets the row associated with the event.
    /// </summary>
    public BindingSourceRow Row { get; internal set; }

    /// <summary>
    /// Gets the underlying business object (InnerObject) associated with the row.
    /// </summary>
    public object InnerObject => Row?.InnerObject;
    
    /// <summary>
    /// Initializes a new instance of the DataSourceEventArgs class.
    /// </summary>
    /// <param name="row">The row involved in the event.</param>
    public BindingSourceArgs(BindingSourceRow row)
    {
        this.Row = row;
    }
}


/// <summary>
/// Provides data for events that can be canceled within a DataSource operation.
/// </summary>
public class BindingSourceCancelArgs : EventArgs
{
    /// <summary>
    /// Gets the row associated with the event.
    /// </summary>
    public BindingSourceRow Row { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether the operation should be canceled.
    /// </summary>
    public bool Cancel { get; set; } = false;

    /// <summary>
    /// Gets or sets the reason why the operation was canceled.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Initializes a new instance of the DataSourceCancelEventArgs class.
    /// </summary>
    /// <param name="row">The row involved in the event.</param>
    public BindingSourceCancelArgs(BindingSourceRow row)
    {
        this.Row = row;
    }
}


/// <summary>
/// Provides data for events involving a change in a data property value.
/// </summary>
public class BindingSourceChangeArgs : BindingSourceCancelArgs
{
    /// <summary>
    /// Gets the name of the property that is changing or has changed.
    /// </summary>
    public string PropertyName { get; internal set; }

    /// <summary>
    /// Gets the value of the property before the change.
    /// </summary>
    public object OldValue { get; internal set; }

    /// <summary>
    /// Gets the new value being assigned to the property.
    /// </summary>
    public object NewValue { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the DataSourceChangeEventArgs class.
    /// </summary>
    /// <param name="row">The row containing the property.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="oldValue">The original value.</param>
    /// <param name="newValue">The proposed new value.</param>
    public BindingSourceChangeArgs(BindingSourceRow row, string propertyName, object oldValue, object newValue) 
        : base(row)
    {
        this.PropertyName = propertyName;
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }
}



/// <summary>
/// Provides data for events triggered when a new inner object is being created within the DataSource.
/// </summary>
public class BindingSourceCreateArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the new inner object instance provided by the caller (e.g., a new Customer instance).
    /// </summary>
    public object NewInnerObject { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the creation process should be canceled.
    /// </summary>
    public bool Cancel { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the DataSourceCreateEventArgs class.
    /// </summary>
    public BindingSourceCreateArgs()
    {
        this.NewInnerObject = null;
    }
}


public class BindingSourceInnerObjectArgs: EventArgs
{
    public BindingSourceInnerObjectArgs(object InnerObject)
    {
        this.InnerObject = InnerObject;
    }
    
    public object InnerObject { get; }
}