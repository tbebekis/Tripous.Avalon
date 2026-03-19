using System;

namespace Tripous.Avalon.Data;

/// <summary>
/// Provides data for standard events associated with a DataSourceRow.
/// </summary>
public class BindingSourceEventArgs : EventArgs
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
    public BindingSourceEventArgs(BindingSourceRow row)
    {
        this.Row = row;
    }
}