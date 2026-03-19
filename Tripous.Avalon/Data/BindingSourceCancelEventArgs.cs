using System;

namespace Tripous.Avalon.Data;

/// <summary>
/// Provides data for events that can be canceled within a DataSource operation.
/// </summary>
public class BindingSourceCancelEventArgs : EventArgs
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
    public BindingSourceCancelEventArgs(BindingSourceRow row)
    {
        this.Row = row;
    }
}