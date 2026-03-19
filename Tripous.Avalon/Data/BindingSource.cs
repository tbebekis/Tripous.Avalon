using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
 

namespace Tripous.Avalon.Data;

/*
/// <summary>
/// Delegate for the event triggered during the creation of a new inner object.
/// </summary>
public delegate void BindingSourceCreateEventHandler(object sender, BindingSourceCreateEventArgs e);

/// <summary>
/// Delegate for events involving a generic inner object.
/// </summary>
public delegate void BindingSourceObjectHandler(object sender, object innerObject);

/// <summary>
/// Delegate for standard data source events.
/// </summary>
public delegate void BindingSourceEventHandler(object sender, BindingSourceEventArgs e);

/// <summary>
/// Delegate for data source events that can be canceled.
/// </summary>
public delegate void BindingSourceCancelEventHandler(object sender, BindingSourceCancelEventArgs e);

/// <summary>
/// Delegate for events triggered when a data value is changing or has changed.
/// </summary>
public delegate void BindingSourceChangeEventHandler(object sender, BindingSourceChangeEventArgs e);
*/

/// <summary>
/// Provides a centralized mechanism for data binding, navigation, and CRUD operations 
/// between UI controls and various underlying data sources.
/// </summary>
public class BindingSource : INotifyPropertyChanged
{
    static private int Counter = 0;
    private BindingSourceRow fCurrent;
    private BindingSourceRowCollection<BindingSourceRow> fRows;
    private string fTitle;
    private Type fSourceItemType;

    // ● private
    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    void OnPropertyChanged(string PropertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));


    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the DataSource class using a specific data link.
    /// </summary>
    public BindingSource(IDataSource source)
    {
        this.DataSource = source ?? throw new ArgumentNullException(nameof(source));
        this.fRows = new BindingSourceRowCollection<BindingSourceRow>();
        this.fSourceItemType = DataSource.GetItemType();
        this.Load();
    }

    // ● static
    /// <summary>
    /// Creates a DataSource instance from a DataTable.
    /// </summary>
    public static BindingSource FromTable(DataTable Table) => new BindingSource(new DataTableSource(Table));

    /// <summary>
    /// Creates a DataSource instance from a list of items.
    /// </summary>
    public static BindingSource FromList<T>(IList<T> List) => new BindingSource(new ListDataSource<T>(List));

    // ● public
    /// <summary>
    /// Forces the UI to synchronize its selection with the current record.
    /// </summary>
    public virtual void ForceMoveToCurrent()
    {
        if (Rows.Count == 0) return;
        int Index = Rows.IndexOf(Current);
        
        if (Index <= 0) { Last(); First(); }
        else { First(); Position = Index; }
    }
    
    /// <summary>
    /// Loads the data from the underlying data link into the row collection.
    /// </summary>
    public virtual void Load()
    {
        this.OnLoading?.Invoke(this, EventArgs.Empty);

        var TempList = new List<BindingSourceRow>();
        foreach (object Item in DataSource.GetRows())
        {
            TempList.Add(new BindingSourceRow(this, Item));
        }

        this.fRows.LoadRange(TempList);
        this.Current = (this.fRows.Count > 0) ? this.fRows[0] : null;

        this.OnLoaded?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Clears all records from the row collection.
    /// </summary>
    public virtual void Clear()
    {
        if (Rows.Count == 0) return;
        this.OnEmptying?.Invoke(this, EventArgs.Empty);

        this.fRows.Clear();
        this.Current = null;

        this.OnEmptied?.Invoke(this, EventArgs.Empty);
        this.OnPropertyChanged(nameof(Count));
        this.OnPropertyChanged(nameof(Position));
    }    
    
    /// <summary>
    /// Adds a new record to the data source.
    /// </summary>
    public virtual BindingSourceRow Add()
    {
        var CreateArgs = new BindingSourceCreateArgs();
        this.OnCreateInnerObject?.Invoke(this, CreateArgs);
        if (CreateArgs.Cancel) return null;

        object NewInnerItem = CreateArgs.NewInnerObject ?? DataSource.CreateNew();
        var InnerObjectArgs = new BindingSourceInnerObjectArgs(NewInnerItem);
        this.OnInnerObjectCreated?.Invoke(this, InnerObjectArgs);

        var NewRow = new BindingSourceRow(this, NewInnerItem);
        if (!RaiseOnAdding(NewRow)) return null;

        // Commit to source via Link
        DataSource.AddToSource(NewInnerItem);
        this.fRows.Add(NewRow);
        this.Current = NewRow;

        this.OnAdded?.Invoke(this, new BindingSourceArgs(NewRow));
        return NewRow;
    }
    /// <summary>
    /// Deletes the specified record from the data source.
    /// </summary>
    public bool Delete(BindingSourceRow Row)
    {
        if (Row == null || this.fRows == null) return false;

        // RaiseOnDeleting is called here. If it returns false, we stop.
        if (!RaiseOnDeleting(Row)) return false;

        int Index = this.fRows.IndexOf(Row);
        
        if (Index < 0) return false;

        try 
        {
            DataSource.RemoveFromSource(Row.InnerObject);
            this.fRows.Remove(Row);

            if (this.Current == Row)
                this.Position = (this.fRows.Count > 0) ? Math.Max(0, Index - 1) : -1;

            this.OnDeleted?.Invoke(this, new BindingSourceArgs(Row));
            return true;
        }
        catch (Exception Ex)
        {
            throw new ApplicationException($"DataSource: {Title} - Delete failed.", Ex);
        }
    }
    /// <summary>
    /// Deletes the current record.
    /// </summary>
    public bool Delete() => this.Delete(this.fCurrent);

    /// <summary>
    /// Moves the current selection to the first record.
    /// </summary>
    public virtual BindingSourceRow First() { Position = 0; return Current; }
    /// <summary>
    /// Moves the current selection to the last record.
    /// </summary>
    public virtual BindingSourceRow Last() { Position = this.fRows.Count - 1; return Current; }

    // ● Current - get/set value
    /// <summary>
    /// Gets the value of a property from the current record as a specific type.
    /// </summary>
    public virtual T GetValue<T>(string PropertyName)
    {
        if (this.fCurrent == null) return default(T);
        return this.fCurrent.GetValue<T>(PropertyName);
    }
    /// <summary>
    /// Sets the value of a property from the current record as a specific type.. Returns true if successful.
    /// </summary>
    public bool SetValue<T>(string PropertyName, T Value)
    {
        if (this.fCurrent == null) return false;
        return this.fCurrent.SetValue<T>(PropertyName, Value);
    }
    
    /// <summary>
    /// Gets the value of a property from the current record as an object. 
    /// Useful for lookups where the type is unknown.
    /// </summary>
    public virtual object GetValue(string PropertyName)
    {
        return this.fCurrent?[PropertyName];
    }
    /// <summary>
    /// Sets the value of a property for the current record.
    /// </summary>
    public virtual bool SetValue(string PropertyName, object Value)
    {
        if (this.fCurrent == null) return false;
        this.fCurrent[PropertyName] = Value;
        return true;
    }
    
    /// <summary>
    /// Accesses the property value of the current record as a string.
    /// </summary>
    public string AsString(string PropertyName) => fCurrent?.AsString(PropertyName);
    /// <summary>
    /// Sets the property value of the current record as a string.
    /// </summary>
    public void AsString(string PropertyName, string Value) => fCurrent?.AsString(PropertyName, Value);

    /// <summary>
    /// Accesses the property value of the current record as an integer.
    /// </summary>
    public int AsInteger(string PropertyName) => fCurrent?.AsInteger(PropertyName) ?? 0;
    /// <summary>
    /// Sets the property value of the current record as an integer.
    /// </summary>
    public void AsInteger(string PropertyName, int Value) => fCurrent?.AsInteger(PropertyName, Value);

    /// <summary>
    /// Accesses the property value of the current record as a double.
    /// </summary>
    public double AsDouble(string PropertyName) => fCurrent?.AsDouble(PropertyName) ?? 0.0;
    /// <summary>
    /// Sets the property value of the current record as a double.
    /// </summary>
    public void AsDouble(string PropertyName, double Value) => fCurrent?.AsDouble(PropertyName, Value);

    /// <summary>
    /// Accesses the property value of the current record as a boolean.
    /// </summary>
    public bool AsBoolean(string PropertyName) => fCurrent?.AsBoolean(PropertyName) ?? false;
    /// <summary>
    /// Sets the property value of the current record as a boolean.
    /// </summary>
    public void AsBoolean(string PropertyName, bool Value) => fCurrent?.AsBoolean(PropertyName, Value);

    /// <summary>
    /// Accesses the property value of the current record as a DateTime.
    /// </summary>
    public DateTime AsDateTime(string PropertyName) => fCurrent?.AsDateTime(PropertyName) ?? DateTime.MinValue;
    /// <summary>
    /// Sets the property value of the current record as a DateTime.
    /// </summary>
    public void AsDateTime(string PropertyName, DateTime Value) => fCurrent?.AsDateTime(PropertyName, Value);

    // ● filter 
    // Για το απλό φίλτρο του χρήστη (δημόσιο, ένα πεδίο)
    public void ApplyFilter(string PropertyName, object Value)
    {
        DataSource.ApplyFilter(PropertyName, Value);
    }
    
    // ● event triggers
    /// <summary>
    /// Internal trigger to raise the OnAdding event.
    /// </summary>
    internal bool RaiseOnAdding(BindingSourceRow row) {
        var e = new BindingSourceCancelArgs(row);
        OnAdding?.Invoke(this, e);
        return !e.Cancel;
    }
    /// <summary>
    /// Internal trigger to raise the OnDeleting event.
    /// </summary>
    internal bool RaiseOnDeleting(BindingSourceRow row) {
        var e = new BindingSourceCancelArgs(row);
        OnDeleting?.Invoke(this, e);
        return !e.Cancel;
    }    
    /// <summary>
    /// Internal trigger to raise the OnChanging event.
    /// </summary>
    internal bool RaiseOnChanging(BindingSourceRow row, string propName, object oldVal, object newVal) {
        var e = new BindingSourceChangeArgs(row, propName, oldVal, newVal);
        OnChanging?.Invoke(this, e);
        return !e.Cancel;
    }
    /// <summary>
    /// Internal trigger to raise the OnChanged event.
    /// </summary>
    internal void RaiseOnChanged(BindingSourceRow row, string propName, object oldVal, object newVal) {
        OnChanged?.Invoke(this, new BindingSourceChangeArgs(row, propName, oldVal, newVal));
    }      
    
    // ● properties
    /// <summary>
    /// Gets the collection of records as DataSourceRow wrappers.
    /// </summary>
    public ObservableCollection<BindingSourceRow> Rows => this.fRows;
    /// <summary>
    /// Gets the total number of records.
    /// </summary>
    public int Count => Rows.Count;
    /// <summary>
    /// Gets a value indicating whether the underlying source has a fixed size.
    /// </summary>
    public bool IsFixedSize => DataSource.IsFixedSize;

    /// <summary>
    /// Gets the underlying data link.
    /// </summary>
    public IDataSource DataSource { get; protected set; }

    /// <summary>
    /// Gets the type of the items contained in the source.
    /// </summary>
    public Type SourceItemType => fSourceItemType;
    
    /// <summary>
    /// Gets or sets the currently selected record.
    /// </summary>
    public virtual BindingSourceRow Current
    {
        get => this.fCurrent;
        set
        {
            if (this.fCurrent != value)
            {
                this.fCurrent = value;
                this.OnPropertyChanged(nameof(Current));
                this.OnPropertyChanged(nameof(Position));
                this.OnCurrentPositionChanged?.Invoke(this, new BindingSourceArgs(this.fCurrent));
            }
        }
    }

    /// <summary>
    /// Gets or sets the zero-based index of the current record.
    /// </summary>
    public virtual int Position
    {
        get => (Rows.Count > 0 && Current != null) ? Rows.IndexOf(Current) : -1;
        set
        {
            if (Rows.Count == 0 || value < 0 || value >= Rows.Count) return;
            var TargetRow = Rows[value];
            if (TargetRow == fCurrent) return;

            var Args = new BindingSourceCancelArgs(fCurrent);
            this.OnCurrentPositionChanging?.Invoke(this, Args);

            if (Args.Cancel) { this.OnPropertyChanged(nameof(Position)); return; }
            this.Current = TargetRow;
        }
    }

    /// <summary>
    /// Gets or sets a descriptive title for the data source.
    /// </summary>
    public virtual string Title
    {
        get { if (string.IsNullOrWhiteSpace(fTitle)) fTitle = "DataSource " + (++Counter); return fTitle; }
        set => fTitle = value;
    }
    
    /// <summary>
    /// Event triggered when a property of the DataSource itself changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Occurs when a new inner object is requested.
    /// </summary>
    public event EventHandler<BindingSourceCreateArgs> OnCreateInnerObject;   // BindingSourceCreateEventHandler

    /// <summary>
    /// Occurs when a new inner object has been successfully created.
    /// </summary>
    public event EventHandler<BindingSourceInnerObjectArgs> OnInnerObjectCreated; // BindingSourceObjectHandler

    /// <summary>
    /// Occurs before a record is added to the collection.
    /// </summary>
    public event EventHandler<BindingSourceCancelArgs> OnAdding;  // BindingSourceCancelEventHandler

    /// <summary>
    /// Occurs after a record has been added to the collection.
    /// </summary>
    public event EventHandler<BindingSourceArgs> OnAdded; // BindingSourceEventHandler

    /// <summary>
    /// Occurs before a record is deleted.
    /// </summary>
    public event EventHandler<BindingSourceCancelArgs> OnDeleting;    // BindingSourceCancelEventHandler

    /// <summary>
    /// Occurs after a record has been deleted.
    /// </summary>
    public event EventHandler<BindingSourceArgs> OnDeleted;

    /// <summary>
    /// Occurs before a data value is changed.
    /// </summary>
    public event EventHandler<BindingSourceChangeArgs> OnChanging;    // BindingSourceChangeEventHandler

    /// <summary>
    /// Occurs after a data value has been changed.
    /// </summary>
    public event EventHandler<BindingSourceChangeArgs> OnChanged;

    /// <summary>
    /// Occurs before the current record position changes.
    /// </summary>
    public event EventHandler<BindingSourceCancelArgs> OnCurrentPositionChanging;

    /// <summary>
    /// Occurs after the current record position has changed.
    /// </summary>
    public event EventHandler<BindingSourceArgs> OnCurrentPositionChanged;

    /// <summary>
    /// Occurs when the loading process begins.
    /// </summary>
    public event EventHandler OnLoading;

    /// <summary>
    /// Occurs when the loading process is complete.
    /// </summary>
    public event EventHandler OnLoaded;

    /// <summary>
    /// Occurs when the clearing process begins.
    /// </summary>
    public event EventHandler OnEmptying;

    /// <summary>
    /// Occurs when the clearing process is complete.
    /// </summary>
    public event EventHandler OnEmptied;
}