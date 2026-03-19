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

    // --- Schema ---

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

    // --- Data Access ---

    /// <summary>
    /// Returns the collection of items from the underlying list.
    /// </summary>
    public System.Collections.IEnumerable GetRows() => fList;

    /// <summary>
    /// Gets the value of a specific property from an item in the list using reflection.
    /// </summary>
    public object GetValue(object innerItem, string propertyName)
    {
        var Prop = typeof(T).GetProperty(propertyName);
        return Prop?.GetValue(innerItem);
    }

    /// <summary>
    /// Sets the value of a specific property on an item in the list, handling Nullable types and type conversions.
    /// </summary>
    public void SetValue(object innerItem, string propertyName, object value)
    {
        var Prop = typeof(T).GetProperty(propertyName);
        if (Prop != null && Prop.CanWrite)
        {
            // Handle Nullable types and type conversion
            Type TargetType = Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType;
            object ConvertedValue = (value == null) ? null : Convert.ChangeType(value, TargetType);
            Prop.SetValue(innerItem, ConvertedValue);
        }
    }
    
    // Για το Master-Detail (εσωτερικό, με arrays για σύνθετα κλειδιά)
    public void ApplyChildSync(string[] PropertyNames, object[] Values)
    {
        // TODO:
    }
    
    // Για το απλό φίλτρο του χρήστη (δημόσιο, ένα πεδίο)
    public void ApplyFilter(string PropertyName, object Value)
    {
        // TODO:
    }

    // --- CRUD ---

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
    public void AddToSource(object innerItem)
    {
        if (innerItem is T item && !fList.Contains(item))
        {
            fList.Add(item);
        }
    }

    /// <summary>
    /// Removes an item from the underlying generic list.
    /// </summary>
    public void RemoveFromSource(object innerItem)
    {
        if (innerItem is T item)
        {
            fList.Remove(item);
        }
    }

    // --- Capabilities ---

    /// <summary>
    /// Gets a value indicating whether the data source has a fixed size (e.g., an Array).
    /// </summary>
    public bool IsFixedSize 
    {
        get 
        {
            // Arrays implement the non-generic IList, where IsFixedSize is defined
            if (fList is System.Collections.IList nonGenericList)
            {
                return nonGenericList.IsFixedSize;
            }
            return false; // Standard List<T> instances are not Fixed Size
        }
    }

    /// <summary>
    /// Gets a value indicating whether the underlying list is read-only.
    /// </summary>
    public bool IsReadOnly => fList.IsReadOnly;
}