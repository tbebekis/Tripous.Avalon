using System;

namespace Tripous.Avalon.Data;

/// <summary>
/// Defines a contract for a data bridge that abstracts the underlying data source (e.g., DataTable, List, etc.) 
/// allowing the DataSource class to operate independently of the specific data implementation.
/// </summary>
public interface IDataSource
{
    // --- Schema & Metadata ---

    /// <summary>
    /// Gets the type of the items managed by this link.
    /// </summary>
    Type GetItemType();

    /// <summary>
    /// Returns an array of property or column names available in the data source.
    /// </summary>
    string[] GetPropertyNames();
    
    // --- Data Operations ---

    /// <summary>
    /// Returns the collection of raw data objects from the underlying source.
    /// </summary>
    System.Collections.IEnumerable GetRows();

    /// <summary>
    /// Retrieves the value of a specific property from an underlying data item.
    /// </summary>
    object GetValue(object innerItem, string propertyName);

    /// <summary>
    /// Sets the value of a specific property on an underlying data item.
    /// </summary>
    void SetValue(object innerItem, string propertyName, object value);
    
    // --- CRUD ---

    /// <summary>
    /// Creates a new instance of the underlying data object (e.g., a new DataRow or a POCO instance).
    /// </summary>
    /// <returns>The newly created inner object.</returns>
    object CreateNew();

    /// <summary>
    /// Formally adds a newly created inner object to the original data source.
    /// </summary>
    void AddToSource(object innerItem);

    /// <summary>
    /// Removes an inner object from the original data source.
    /// </summary>
    void RemoveFromSource(object innerItem);
    
    // --- Capabilities ---

    /// <summary>
    /// Gets a value indicating whether the data source has a fixed size and cannot have items added or removed.
    /// </summary>
    bool IsFixedSize { get; }

    /// <summary>
    /// Gets a value indicating whether the data source is read-only.
    /// </summary>
    bool IsReadOnly { get; }
}