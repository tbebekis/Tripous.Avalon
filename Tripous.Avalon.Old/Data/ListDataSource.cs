using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tripous.Avalon.Data;

/// <summary>
/// Implements the IDataLink interface to provide a bridge between a DataSource and a generic IList of POCO (Plain Old CLR Objects).
/// </summary>
/// <typeparam name="T">The type of the business objects in the list.</typeparam>
public class ListDataSource<T> : IDataSource
{
    private IList<T> fList;
    private PropertyInfo[] fProperties;
    private string[] fChildPropertyNames;

    // ● construction 
    /// <summary>
    /// Initializes a new instance of the ListLink class with a specified list.
    /// </summary>
    /// <param name="list">The generic list to link to.</param>
    public ListDataSource(IList<T> list)
    {
        this.fList = list ?? throw new ArgumentNullException(nameof(list));
        // Cache properties for performance
        this.fProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    // ● Schema & Metadata
    /// <summary>
    /// Gets the type of items contained in the link.
    /// </summary>
    public Type GetItemType() => typeof(T);
    /// <summary>
    /// Returns an array of property names available on the generic type T.
    /// </summary>
    public string[] GetPropertyNames()
    {
        return fProperties.Select(p => p.Name).ToArray();
    }
    /// <summary>
    /// Returns an array of the property types
    /// </summary>
    public Type[] GetPropertyTypes()
    {
        return fProperties
            .Select(c => c.PropertyType)
            .ToArray();
    }
    /// <summary>
    /// Returns the <see cref="Type"/> of a specified property.
    /// </summary>
    public Type GetPropertyType(string PropertyName)
    {
        PropertyInfo Prop = fProperties.FirstOrDefault(x => x.Name.IsSameText(PropertyName)); 
        return Prop != null ? Prop.PropertyType : typeof(string);
    }
    /// <summary>
    /// Returns the <see cref="Type"/> of a property specified by its index in the properties.
    /// </summary>
    public Type GetPropertyType(int PropertyIndex)
    {
        return fProperties[PropertyIndex].PropertyType;
    }
    
    // ● Data Operations
    /// <summary>
    /// Returns the collection of items from the underlying list.
    /// </summary>
    public System.Collections.IEnumerable GetRows() => fList;
    /// <summary>
    /// Gets the value of a specific property from an item in the list using reflection.
    /// </summary>
    public object GetValue(object InnerObject, string propertyName)
    {
        var Prop = typeof(T).GetProperty(propertyName);
        return Prop?.GetValue(InnerObject);
    }
    /// <summary>
    /// Sets the value of a specific property on an item in the list, handling Nullable types and type conversions.
    /// </summary>
    public void SetValue(object InnerObject, string propertyName, object value)
    {
        var Prop = typeof(T).GetProperty(propertyName);
        if (Prop != null && Prop.CanWrite)
        {
            // Handle Nullable types and type conversion
            Type TargetType = Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType;
            object ConvertedValue = (value == null) ? null : Convert.ChangeType(value, TargetType);
            Prop.SetValue(InnerObject, ConvertedValue);
        }
    }
    
    // ● master-detail and filter
 
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    public bool PassesDetailCondition(object InnerObject, object[] MasterValues)
    {
        if (fChildPropertyNames == null || MasterValues == null || fChildPropertyNames.Length != MasterValues.Length)
            return true;
        for (int i = 0; i < fChildPropertyNames.Length; i++)
        {
            object Value = GetValue(InnerObject, fChildPropertyNames[i]);
            if (!Equals(Value, MasterValues[i])) return false;
        }
        return true;
    }

    /// <summary>
    /// Sets the filter
    /// </summary>
    public void SetFilter(string PropertyName, object Value)
    {
        this.FilterPropertyName = PropertyName;
        this.FilterValue = Value;
    }
    /// <summary>
    /// Clears the filter
    /// </summary>
    public void ClearFilter()
    {
        this.FilterPropertyName = null;
        this.FilterValue = null;
    }
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    public bool PassesFilterCondition1(object InnerObject)
    {
        if (string.IsNullOrEmpty(FilterPropertyName)) return true;
        object Value = GetValue(InnerObject, FilterPropertyName);

        if (Value is string strValue && FilterValue is string strFilter)
        {
            /* Wildcard: StartsWith check */
            if (strFilter.EndsWith("*") && strFilter.Length > 1)
            {
                string cleanFilter = strFilter.Substring(0, strFilter.Length - 1);
                return strValue.StartsWith(cleanFilter, StringComparison.OrdinalIgnoreCase);
            }

            /* Standard: Contains check */
            return strValue.Contains(strFilter, StringComparison.OrdinalIgnoreCase);
        }

        /* Fallback for non-string types */
        return Equals(Value, FilterValue);
    }
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    public bool PassesFilterCondition(object InnerObject)
    {
        if (string.IsNullOrEmpty(FilterPropertyName)) return true;
        object Value = GetValue(InnerObject, FilterPropertyName);
        return Equals(Value, FilterValue);
    }
    
    // ● CRUD 
    /// <summary>
    /// Creates a new instance of type T. This requires the type to have a parameterless constructor.
    /// </summary>
    /// <returns>A new instance of the business object.</returns>
    public object CreateNew()
    {
        // Create a new POCO (requires parameterless constructor)
        return Activator.CreateInstance<T>();
    }
    /// <summary>
    /// Adds a new item to the underlying generic list if it is not already present.
    /// </summary>
    public void AddToSource(object InnerObject)
    {
        if (InnerObject is T item && !fList.Contains(item))
        {
            fList.Add(item);
        }
    }
    /// <summary>
    /// Removes an item from the underlying generic list.
    /// </summary>
    public void RemoveFromSource(object InnerObject)
    {
        if (InnerObject is T item)
        {
            fList.Remove(item);
        }
    }

    // ● properties 
    /// <summary>
    /// Gets a value indicating whether the data source has a fixed size.
    /// </summary>
    public bool IsFixedSize => false;
    /// <summary>
    /// Gets a value indicating whether the data source is read-only.
    /// </summary>
    public bool IsReadOnly => false;
    /// <summary>
    /// The property name the filter is applied on.
    /// </summary>
    public string FilterPropertyName { get; private set; }
    /// <summary>
    /// The filter value.
    /// </summary>
    public object FilterValue { get; private set; }
    /// <summary>
    /// The property names when this is a detail source.
    /// </summary>
    public string[] DetailPropertyNames
    {
        get  => fChildPropertyNames;
        set => fChildPropertyNames = value;
    }
}