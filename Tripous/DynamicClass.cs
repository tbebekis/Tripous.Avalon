/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */
namespace Tripous;

/// <summary>
/// Represents a dynamic object whose properties are stored in a dictionary.
///
/// Supports dynamic member access, property change notifications,
/// custom type descriptors and JSON serialization.
/// </summary>
public class DynamicClass: DynamicObject, INotifyPropertyChanged, ICustomTypeDescriptor
{
    // ● private
    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    void OnPropertyChanged(string PropertyName)
    {
        if (PropertyChanged == null)
            return;

        var EventArgs = new PropertyChangedEventArgs(PropertyName);
        PropertyChanged(this, EventArgs);
    }
    /// <summary>
    /// Notifies listeners that all properties should be refreshed.
    /// </summary>
    void NotifyToRefreshAllProperties()
    {
        OnPropertyChanged(string.Empty);
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public DynamicClass()
    {
    }
    /// <summary>
    /// Constructs an instance from a JSON string.
    /// </summary>
    public DynamicClass(string JsonText)
    {
        FromJson(JsonText);
    }

    // ● overrides
    /// <summary>
    /// Gets a dynamic member value.
    /// </summary>
    public override bool TryGetMember(GetMemberBinder Binder, out object Result)
    {
        return Properties.TryGetValue(Binder.Name, out Result);
    }
    /// <summary>
    /// Sets a dynamic member value.
    /// </summary>
    public override bool TrySetMember(SetMemberBinder Binder, object Value)
    {
        Properties[Binder.Name] = Value;
        NotifyToRefreshAllProperties();
        return true;
    }

    // ● ICustomTypeDescriptor
    /// <summary>
    /// Returns the attributes of this instance.
    /// </summary>
    public AttributeCollection GetAttributes()
    {
        return TypeDescriptor.GetAttributes(this, true);
    }
    /// <summary>
    /// Returns the class name.
    /// </summary>
    public string GetClassName()
    {
        return GetType().Name;
    }
    /// <summary>
    /// Returns the component name.
    /// </summary>
    public string GetComponentName()
    {
        return TypeDescriptor.GetComponentName(this, true);
    }
    /// <summary>
    /// Returns the type converter.
    /// </summary>
    public TypeConverter GetConverter()
    {
        return TypeDescriptor.GetConverter(this, true);
    }
    /// <summary>
    /// Returns the default event.
    /// </summary>
    public EventDescriptor GetDefaultEvent()
    {
        return TypeDescriptor.GetDefaultEvent(this, true);
    }
    /// <summary>
    /// Returns the default property.
    /// </summary>
    public PropertyDescriptor GetDefaultProperty()
    {
        return null;
    }
    /// <summary>
    /// Returns an editor of the specified base type.
    /// </summary>
    public object GetEditor(Type EditorBaseType)
    {
        return TypeDescriptor.GetEditor(this, EditorBaseType, true);
    }
    /// <summary>
    /// Returns the events of this instance.
    /// </summary>
    public EventDescriptorCollection GetEvents()
    {
        return TypeDescriptor.GetEvents(this, true);
    }
    /// <summary>
    /// Returns the events of this instance matching the specified attributes.
    /// </summary>
    public EventDescriptorCollection GetEvents(Attribute[] Attributes)
    {
        return TypeDescriptor.GetEvents(this, Attributes, true);
    }
    /// <summary>
    /// Returns the dynamic properties of this instance.
    /// </summary>
    public PropertyDescriptorCollection GetProperties()
    {
        return GetProperties(new Attribute[0]);
    }
    /// <summary>
    /// Returns the dynamic properties of this instance matching the specified attributes.
    /// </summary>
    public PropertyDescriptorCollection GetProperties(Attribute[] Attributes)
    {
        DynamicPropertyDescriptor[] PropList = Properties
            .Select(Entry => new DynamicPropertyDescriptor(this, Entry.Key, Entry.Value?.GetType() ?? typeof(object), Attributes))
            .ToArray();

        return new PropertyDescriptorCollection(PropList);
    }
    /// <summary>
    /// Returns the owner of the specified property descriptor.
    /// </summary>
    public object GetPropertyOwner(PropertyDescriptor Pd)
    {
        return this;
    }

    // ● public
    /// <summary>
    /// Serializes this instance to JSON.
    /// </summary>
    public string ToJson()
    {
        return Json.Serialize(this);
    }
    /// <summary>
    /// Loads this instance from a JSON string.
    /// </summary>
    public void FromJson(string JsonText)
    {
        dynamic Dyn = Json.Deserialize<DynamicClass>(JsonText);
        DynamicClass Instance = Dyn as DynamicClass;
        this.Properties = Instance.Properties;
    }
    /// <summary>
    /// Removes all dynamic properties.
    /// </summary>
    public void RemoveAllProperties()
    {
        this.Properties.Clear();
    }

    // ● properties
    /// <summary>
    /// Gets or sets a dynamic property value by property name.
    /// </summary>
    [JsonIgnore]
    public object this[string PropName]
    {
        get { return Properties[PropName]; }
        set
        {
            object OldValue = null;

            if (Properties.ContainsKey(PropName))
                OldValue = Properties[PropName];

            Properties[PropName] = value;

            if (OldValue != value)
                OnPropertyChanged(PropName);
        }
    }
    /// <summary>
    /// Gets or sets the dictionary that stores dynamic properties.
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    /// <summary>
    /// Gets the number of dynamic properties.
    /// </summary>
    [JsonIgnore]
    public int PropertyCount { get { return Properties.Keys.Count; } }

    // ● events
    /// <summary>
    /// Occurs when a dynamic property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}

/// <summary>
/// Provides property descriptor support for a dynamic property
/// of a DynamicClass instance.
/// </summary>
public class DynamicPropertyDescriptor : PropertyDescriptor
{
    DynamicClass Instance;
    Type PropType;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public DynamicPropertyDescriptor(DynamicClass Instance, string PropName, Type PropType, Attribute[] PropAttributes)
        : base(PropName, PropAttributes)
    {
        this.Instance = Instance;
        this.PropType = PropType;
    }

    // ● overrides
    /// <summary>
    /// Gets the property value.
    /// </summary>
    public override object GetValue(object Component)
    {
        return Instance[Name];
    }
    /// <summary>
    /// Sets the property value.
    /// </summary>
    public override void SetValue(object Component, object Value)
    {
        Instance[Name] = Value;
    }
    /// <summary>
    /// Returns true when the property value can be reset.
    /// </summary>
    public override bool CanResetValue(object Component)
    {
        return true;
    }
    /// <summary>
    /// Resets the property value.
    /// </summary>
    public override void ResetValue(object Component)
    {
    }
    /// <summary>
    /// Returns true when the property value should be serialized.
    /// </summary>
    public override bool ShouldSerializeValue(object Component)
    {
        return false;
    }

    // ● properties
    /// <summary>
    /// Gets the component type.
    /// </summary>
    public override Type ComponentType { get { return Instance.GetType(); } }
    /// <summary>
    /// Gets a value indicating whether this property is read-only.
    /// </summary>
    public override bool IsReadOnly { get { return false; } }
    /// <summary>
    /// Gets the property type.
    /// </summary>
    public override Type PropertyType { get { return PropType; } }
}