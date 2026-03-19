using System;

namespace Tripous.Avalon.Data;

/// <summary>
/// Provides data for events triggered when a new inner object is being created within the DataSource.
/// </summary>
public class BindingSourceCreateEventArgs : EventArgs
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
    public BindingSourceCreateEventArgs()
    {
        this.NewInnerObject = null;
    }
}