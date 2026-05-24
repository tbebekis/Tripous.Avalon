namespace Tripous.Data;

/// <summary>
/// Provides DataSource access to an IList of objects.
/// </summary>
public class ListProvider<T>: IDataProvider where T: class, INotifyPropertyChanged, new()
{
    // ● private fields
    IList<T> fList;
    PropertyInfo[] fProperties;

    // ● private
    PropertyInfo FindProperty(string FieldName)
    {
        return fProperties.FirstOrDefault(Property => string.Equals(Property.Name, FieldName, StringComparison.OrdinalIgnoreCase));
    }
    void Subscribe(object Item)
    {
        if (Item is T Notifier)
            Notifier.PropertyChanged += Item_PropertyChanged;
    }
    void Unsubscribe(object Item)
    {
        if (Item is T Notifier)
            Notifier.PropertyChanged -= Item_PropertyChanged;
    }
    object ConvertValue(PropertyInfo Property, object Value)
    {
        if (Value == null)
            return null;

        Type PropertyType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;

        if (Value is string Text && string.IsNullOrWhiteSpace(Text))
            return PropertyType.IsValueType ? Activator.CreateInstance(PropertyType) : null;

        if (PropertyType == typeof(string))
            return Value.ToString();

        return Convert.ChangeType(Value, PropertyType);
    }
    void Item_PropertyChanged(object Sender, PropertyChangedEventArgs e)
    {
        object NewValue = GetValue(Sender, e.PropertyName);
        ItemChanged?.Invoke(this, new DataProviderChangedEventArgs(Sender, e.PropertyName, null, NewValue));
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ListProvider(IList<T> List)
    {
        fList = List;
        fProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (T Item in fList)
            Subscribe(Item);
    }

    // ● public
    /// <summary>
    /// Returns the public property names of the item type.
    /// </summary>
    public string[] GetFieldNames()
    {
        return fProperties.Select(Property => Property.Name).ToArray();
    }
    /// <summary>
    /// Returns the property type of a field.
    /// </summary>
    public Type GetFieldType(string FieldName)
    {
        PropertyInfo Property = FindProperty(FieldName);
        return Property != null ? Property.PropertyType : typeof(object);
    }
    /// <summary>
    /// Returns the list items.
    /// </summary>
    public IEnumerable GetItems()
    {
        foreach (T Item in fList)
            yield return Item;
    }
    /// <summary>
    /// Returns true when a public property exists.
    /// </summary>
    public bool ContainsField(string FieldName)
    {
        return FindProperty(FieldName) != null;
    }
    /// <summary>
    /// Gets a property value from an item.
    /// </summary>
    public object GetValue(object InnerObject, string FieldName)
    {
        PropertyInfo Property = FindProperty(FieldName);
        return Property != null ? Property.GetValue(InnerObject) : null;
    }
    /// <summary>
    /// Sets a property value on an item.
    /// </summary>
    public void SetValue(object InnerObject, string FieldName, object Value)
    {
        PropertyInfo Property = FindProperty(FieldName);

        if (Property == null || !Property.CanWrite)
            return;

        Property.SetValue(InnerObject, ConvertValue(Property, Value));
    }
    /// <summary>
    /// Creates a new item.
    /// </summary>
    public object CreateItem()
    {
        return new T();
    }
    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    public void AddItem(object InnerObject)
    {
        if (InnerObject is T Item && !fList.Contains(Item))
        {
            fList.Add(Item);
            Subscribe(Item);
        }
    }
    /// <summary>
    /// Deletes an item from the list.
    /// </summary>
    public void DeleteItem(object InnerObject)
    {
        if (InnerObject is T Item && fList.Contains(Item))
        {
            Unsubscribe(Item);
            fList.Remove(Item);
        }
    }
    /// <summary>
    /// Returns true when two item references are the same object.
    /// </summary>
    public bool IsSameItem(object A, object B)
    {
        return ReferenceEquals(A, B);
    }

    // ● properties
    /// <summary>
    /// Gets a value indicating whether the list is read-only.
    /// </summary>
    public bool IsReadOnly => fList.IsReadOnly;
    /// <summary>
    /// Gets a value indicating whether the list has a fixed size.
    /// </summary>
    public bool IsFixedSize => false;
    /// <summary>
    /// Gets the underlying list.
    /// </summary>
    public IList<T> List => fList;

    // ● events
    /// <summary>
    /// Occurs when a list item property changes.
    /// </summary>
    public event EventHandler<DataProviderChangedEventArgs> ItemChanged;
}
