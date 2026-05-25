namespace Tripous.Data;

/// <summary>
/// Represents a bindable row owned by a DataSource.
/// </summary>
public class DataSourceRow: INotifyPropertyChanged
{
    // ● private fields
    DataSource fSource;
    object fInnerObject;

    // ● private
    void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceRow(DataSource Source, object InnerObject)
    {
        fSource = Source;
        fInnerObject = InnerObject;
    }

    // ● public
    /// <summary>
    /// Returns true when a field exists in the source provider.
    /// </summary>
    public bool ContainsField(string FieldName)
    {
        return fSource.Provider.ContainsField(FieldName);
    }
    
    /// <summary>
    /// Gets a field value.
    /// </summary>
    public object GetValue(string FieldName)
    {
        return fSource.Provider.GetValue(fInnerObject, FieldName);
    }
    /// <summary>
    /// Gets a field value converted to the specified type.
    /// </summary>
    public T GetValue<T>(string FieldName)
    {
        object Value = GetValue(FieldName);

        if (Value == null || Value == DBNull.Value)
            return default;

        return (T)Convert.ChangeType(Value, typeof(T));
    }
    /// <summary>
    /// Sets a typed field value and returns true when successful.
    /// </summary>
    public bool SetValue<T>(string FieldName, T Value)
    {
        try
        {
            SetValue(FieldName, Value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Sets a field value.
    /// </summary>
    public void SetValue(string FieldName, object Value)
    {
        object OldValue = GetValue(FieldName);

        if (Equals(OldValue, Value))
            return;
        if (!fSource.RaiseChanging(this, FieldName, OldValue, Value))
        {
            NotifyFieldChanged(FieldName);
            return;
        }

        fSource.Provider.SetValue(fInnerObject, FieldName, Value);
        OnPropertyChanged(FieldName);
        OnPropertyChanged("Item");
        OnPropertyChanged("Item[]");
        fSource.RaiseChanged(this, FieldName, OldValue, GetValue(FieldName));

        if (fSource.IsFiltered && string.Equals(fSource.FilterFieldName, FieldName, StringComparison.OrdinalIgnoreCase))
            fSource.RefreshRows();
    }
    
    /// <summary>
    /// Returns true when a field value is null or DBNull.
    /// </summary>
    public bool IsNull(string FieldName)
    {
        object Value = GetValue(FieldName);
        return Value == null || Value == DBNull.Value;
    }
    /// <summary>
    /// Sets a field value to null.
    /// </summary>
    public bool SetToNull(string FieldName)
    {
        try
        {
            SetValue(FieldName, null);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Gets a field value as object.
    /// </summary>
    public object AsObject(string FieldName)
    {
        return this[FieldName];
    }
    /// <summary>
    /// Sets a field value as object.
    /// </summary>
    public void AsObject(string FieldName, object Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as string.
    /// </summary>
    public string AsString(string FieldName)
    {
        return Convert.ToString(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as string.
    /// </summary>
    public void AsString(string FieldName, string Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as int.
    /// </summary>
    public int AsInteger(string FieldName)
    {
        return Convert.ToInt32(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as int.
    /// </summary>
    public void AsInteger(string FieldName, int Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as Int32.
    /// </summary>
    public int AsInt32(string FieldName)
    {
        return AsInteger(FieldName);
    }
    /// <summary>
    /// Sets a field value as Int32.
    /// </summary>
    public void AsInt32(string FieldName, int Value)
    {
        AsInteger(FieldName, Value);
    }
    /// <summary>
    /// Gets a field value as Int64.
    /// </summary>
    public long AsInt64(string FieldName)
    {
        return Convert.ToInt64(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as Int64.
    /// </summary>
    public void AsInt64(string FieldName, long Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as double.
    /// </summary>
    public double AsDouble(string FieldName)
    {
        return Convert.ToDouble(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as double.
    /// </summary>
    public void AsDouble(string FieldName, double Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as decimal.
    /// </summary>
    public decimal AsDecimal(string FieldName)
    {
        return Convert.ToDecimal(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as decimal.
    /// </summary>
    public void AsDecimal(string FieldName, decimal Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as bool.
    /// </summary>
    public bool AsBoolean(string FieldName)
    {
        return Convert.ToBoolean(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as bool.
    /// </summary>
    public void AsBoolean(string FieldName, bool Value)
    {
        this[FieldName] = Value;
    }
    /// <summary>
    /// Gets a field value as DateTime.
    /// </summary>
    public DateTime AsDateTime(string FieldName)
    {
        return Convert.ToDateTime(this[FieldName]);
    }
    /// <summary>
    /// Sets a field value as DateTime.
    /// </summary>
    public void AsDateTime(string FieldName, DateTime Value)
    {
        this[FieldName] = Value;
    }
    
    /// <summary>
    /// Notifies bindings that a field value changed.
    /// </summary>
    public void NotifyFieldChanged(string FieldName)
    {
        OnPropertyChanged(FieldName);
        OnPropertyChanged("Item");
        OnPropertyChanged("Item[]");
    }

    // ● properties
    /// <summary>
    /// Gets the owning DataSource.
    /// </summary>
    public DataSource Source => fSource;
    /// <summary>
    /// Gets the underlying item.
    /// </summary>
    public object InnerObject => fInnerObject;
    /// <summary>
    /// Gets or sets a field value by field name.
    /// </summary>
    public object this[string FieldName]
    {
        get => GetValue(FieldName);
        set => SetValue(FieldName, value);
    }

    // ● events
    /// <summary>
    /// Occurs when a field value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}
