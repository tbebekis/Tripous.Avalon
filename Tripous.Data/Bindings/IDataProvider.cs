namespace Tripous.Data;

/// <summary>
/// Provides source-specific access to schema, rows, values, and change notifications.
/// </summary>
public interface IDataProvider
{
    // ● public
    /// <summary>
    /// Returns the field names exposed by the provider.
    /// </summary>
    string[] GetFieldNames();
    /// <summary>
    /// Returns the type of a field.
    /// </summary>
    Type GetFieldType(string FieldName);
    /// <summary>
    /// Returns the underlying source items.
    /// </summary>
    IEnumerable GetItems();
    /// <summary>
    /// Returns true when a field exists.
    /// </summary>
    bool ContainsField(string FieldName);
    /// <summary>
    /// Gets a field value from an underlying item.
    /// </summary>
    object GetValue(object InnerObject, string FieldName);
    /// <summary>
    /// Sets a field value on an underlying item.
    /// </summary>
    void SetValue(object InnerObject, string FieldName, object Value);
    /// <summary>
    /// Creates a new underlying item.
    /// </summary>
    object CreateItem();
    /// <summary>
    /// Adds an underlying item to the source.
    /// </summary>
    void AddItem(object InnerObject);
    /// <summary>
    /// Deletes an underlying item from the source.
    /// </summary>
    void DeleteItem(object InnerObject);
    /// <summary>
    /// Returns true when two underlying item references represent the same item.
    /// </summary>
    bool IsSameItem(object A, object B);

    // ● properties
    /// <summary>
    /// Gets a value indicating whether the provider is read-only.
    /// </summary>
    bool IsReadOnly { get; }
    /// <summary>
    /// Gets a value indicating whether the provider has a fixed size.
    /// </summary>
    bool IsFixedSize { get; }

    // ● events
    /// <summary>
    /// Occurs when an underlying item field changes outside the DataSourceRow setter.
    /// </summary>
    event EventHandler<DataProviderChangedEventArgs> ItemChanged;
    /// <summary>
    /// Occurs when the underlying item collection changes.
    /// </summary>
    event EventHandler ItemsChanged;
}
