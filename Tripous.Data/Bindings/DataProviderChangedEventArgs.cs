namespace Tripous.Data;

/// <summary>
/// Provides data for provider-level item field changes.
/// </summary>
public class DataProviderChangedEventArgs: EventArgs
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataProviderChangedEventArgs(object InnerObject, string FieldName, object OldValue, object NewValue)
    {
        this.InnerObject = InnerObject;
        this.FieldName = FieldName;
        this.OldValue = OldValue;
        this.NewValue = NewValue;
    }

    // ● properties
    /// <summary>
    /// Gets the underlying item that changed.
    /// </summary>
    public object InnerObject { get; }
    /// <summary>
    /// Gets the changed field name.
    /// </summary>
    public string FieldName { get; }
    /// <summary>
    /// Gets the previous value when available.
    /// </summary>
    public object OldValue { get; }
    /// <summary>
    /// Gets the new value when available.
    /// </summary>
    public object NewValue { get; }
}
