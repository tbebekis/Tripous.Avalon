using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tripous.Avalon.Data;

/// <summary>
/// A wrapper class that represents a single row in a DataSource. 
/// It provides a unified way to access data from different types of underlying objects (DataRow or POCO).
/// </summary>
public class BindingSourceRow : INotifyPropertyChanged
{
    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    void OnPropertyChanged([CallerMemberName] string PropertyName = null) =>
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>
    /// Initializes a new instance of the DataSourceRow class.
    /// </summary>
    /// <param name="bindingSource">The parent DataSource.</param>
    /// <param name="innerObject">The actual data object (e.g., DataRow or a class instance).</param>
    public BindingSourceRow(BindingSource bindingSource, object innerObject)
    {
        this.BindingSource = bindingSource;
        this.InnerObject = innerObject;
    }

    /// <summary>
    /// Gets a property value cast to the specified type.
    /// </summary>
    public T GetValue<T>(string PropertyName)
    {
        object Value = this[PropertyName];
        if (Value == null || Value == DBNull.Value)
        {
            return default(T);
        }
        return (T)Value;
    }

    /// <summary>
    /// Sets a property value. Returns true if successful.
    /// </summary>
    public bool SetValue<T>(string PropertyName, T Value)
    {
        try
        {
            this[PropertyName] = Value;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a property value is null or DBNull.
    /// </summary>
    public bool IsNull(string PropertyName)
    {
        object Value = this[PropertyName];
        return Value == null || Value == DBNull.Value;
    }

    /// <summary>
    /// Sets a property value to null or DBNull.
    /// </summary>
    public bool SetToNull(string PropertyName)
    {
        try
        {
            if (this.InnerObject is System.Data.DataRow Row)
            {
                Row[PropertyName] = DBNull.Value;
            }
            else
            {
                // For POCO we set null (works for reference types and nullable value types)
                this.InnerObject.GetType().GetProperty(PropertyName)?.SetValue(this.InnerObject, null);
            }
            
            this.OnPropertyChanged(PropertyName);
            this.OnPropertyChanged("Item");
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ● public Accessors

    /// <summary>
    /// Accesses the property as a generic object.
    /// </summary>
    public object AsObject(string PropertyName) => this[PropertyName];
    /// <summary>
    /// Sets the property value as a generic object.
    /// </summary>
    public void AsObject(string PropertyName, object Value) => this[PropertyName] = Value;

    /// <summary>
    /// Accesses the property value as a string.
    /// </summary>
    public string AsString(string PropertyName) => Convert.ToString(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a string.
    /// </summary>
    public void AsString(string PropertyName, string Value) => this[PropertyName] = Value;

    /// <summary>
    /// Accesses the property value as an integer.
    /// </summary>
    public int AsInteger(string PropertyName) => Convert.ToInt32(this[PropertyName]);
    /// <summary>
    /// Sets the property value as an integer.
    /// </summary>
    public void AsInteger(string PropertyName, int Value) => this[PropertyName] = Value;

    /// <summary>
    /// Accesses the property value as a double.
    /// </summary>
    public double AsDouble(string PropertyName) => Convert.ToDouble(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a double.
    /// </summary>
    public void AsDouble(string PropertyName, double Value) => this[PropertyName] = Value;

    /// <summary>
    /// Accesses the property value as a decimal.
    /// </summary>
    public decimal AsDecimal(string PropertyName) => Convert.ToDecimal(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a decimal.
    /// </summary>
    public void AsDecimal(string PropertyName, decimal Value) => this[PropertyName] = Value;

    /// <summary>
    /// Accesses the property value as a boolean.
    /// </summary>
    public bool AsBoolean(string PropertyName) => Convert.ToBoolean(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a boolean.
    /// </summary>
    public void AsBoolean(string PropertyName, bool Value) => this[PropertyName] = Value;

    /// <summary>
    /// Accesses the property value as a DateTime.
    /// </summary>
    public DateTime AsDateTime(string PropertyName) => Convert.ToDateTime(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a DateTime.
    /// </summary>
    public void AsDateTime(string PropertyName, DateTime Value) => this[PropertyName] = Value;
 
    /// <summary>
    /// Indexer to get or set values by property name. Handles both DataRow and POCO objects.
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    public object this[string PropertyName]
    {
        get
        {
            if (this.InnerObject is System.Data.DataRow Row)
                return Row.Table.Columns.Contains(PropertyName) ? Row[PropertyName] : null;
            
            return this.InnerObject.GetType().GetProperty(PropertyName)?.GetValue(this.InnerObject);
        }
        set
        {
            // 1. Get the old value for the event
            object OldValue = this[PropertyName];
            object NewValue = value;

            // If the value is the same, do nothing (Optimization)
            if (Equals(OldValue, NewValue)) return;

            // 2. OnChanging: (Cancelable) DbPark Validation
            // Call the internal method of the DataSource
            if (!this.BindingSource.RaiseOnChanging(this, PropertyName, OldValue, NewValue))
            {
                // If the user canceled the change, we stop here.
                // The TextBox will revert to the old value due to TwoWay Binding 
                // upon the next PropertyChanged refresh.
                this.OnPropertyChanged(PropertyName); 
                return;
            }

            // 3. Actual assignment of the value
            if (this.InnerObject is System.Data.DataRow Row) 
            {
                Row[PropertyName] = NewValue;
            }
            else 
            {
                var Prop = this.InnerObject.GetType().GetProperty(PropertyName);
                if (Prop != null && Prop.CanWrite)
                {
                    // Use Convert for type safety (e.g., string to int from a TextBox)
                    var TargetType = System.Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType;
                    var ConvertedValue = (NewValue == null) ? null : Convert.ChangeType(NewValue, TargetType);
                    Prop.SetValue(this.InnerObject, ConvertedValue);
                }
            }

            // 4. Notifications (To update the UI)
            this.OnPropertyChanged(PropertyName);
            this.OnPropertyChanged("Item");

            // 5. OnChanged: The change is finalized
            this.BindingSource.RaiseOnChanged(this, PropertyName, OldValue, NewValue);
        }
    }

    /// <summary>
    /// Gets the parent DataSource that owns this row.
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    public BindingSource BindingSource { get; private set; }

    /// <summary>
    /// Gets the underlying business object wrapped by this row.
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    public object InnerObject { get; private set; }
 
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}