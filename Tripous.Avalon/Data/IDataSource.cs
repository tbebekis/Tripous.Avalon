using System;

namespace Tripous.Avalon.Data;

/// <summary>
/// Defines a contract for a data bridge that abstracts the underlying data source (e.g., DataTable, List, etc.) 
/// allowing the DataSource class to operate independently of the specific data implementation.
/// </summary>
public interface IDataSource
{
    // ● Schema & Metadata
    /// <summary>
    /// Gets the type of the items managed by this link.
    /// </summary>
    Type GetItemType();
    /// <summary>
    /// Returns an array of property or column names available in the data source.
    /// </summary>
    string[] GetPropertyNames();
    
    // ● Data Operations
    /// <summary>
    /// Returns the collection of raw data objects from the underlying source.
    /// </summary>
    System.Collections.IEnumerable GetRows();
    /// <summary>
    /// Retrieves the value of a specific property from an underlying data item.
    /// </summary>
    object GetValue(object InnerObject, string propertyName);
    /// <summary>
    /// Sets the value of a specific property on an underlying data item.
    /// </summary>
    void SetValue(object InnerObject, string propertyName, object value);
    
    // ● master-detail and filter
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    bool PassesDetailCondition(object InnerObject, object[] MasterValues);
    
    /// <summary>
    /// Sets the filter
    /// </summary>
    void SetFilter(string PropertyName, object Value);
    /// <summary>
    /// Clears the filter
    /// </summary>
    void ClearFilter();
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    bool PassesFilterCondition(object InnerObject);
    
    // ● CRUD 
    /// <summary>
    /// Creates a new instance of the underlying data object (e.g., a new DataRow or a POCO instance).
    /// </summary>
    /// <returns>The newly created inner object.</returns>
    object CreateNew();
    /// <summary>
    /// Formally adds a newly created inner object to the original data source.
    /// </summary>
    void AddToSource(object InnerObject);
    /// <summary>
    /// Removes an inner object from the original data source.
    /// </summary>
    void RemoveFromSource(object InnerObject);
    
    // ● properties 
    /// <summary>
    /// Gets a value indicating whether the data source has a fixed size and cannot have items added or removed.
    /// </summary>
    bool IsFixedSize { get; }
    /// <summary>
    /// Gets a value indicating whether the data source is read-only.
    /// </summary>
    bool IsReadOnly { get; }
    /// <summary>
    /// The property name the filter is applied on.
    /// </summary>
    string FilterPropertyName { get;  }
    /// <summary>
    /// The filter value.
    /// </summary>
    object FilterValue { get;  }
    /// <summary>
    /// The property names when this is a detail source.
    /// </summary>
    string[] DetailPropertyNames { get; internal set; }
}
 