using System;

namespace Tripous.Avalon.Data;

/// <summary>
/// Provides data for events involving a change in a data property value.
/// </summary>
public class BindingSourceChangeEventArgs : BindingSourceCancelEventArgs
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
    public BindingSourceChangeEventArgs(BindingSourceRow row, string propertyName, object oldValue, object newValue) 
        : base(row)
    {
        this.PropertyName = propertyName;
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }
}