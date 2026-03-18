using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Tripous.Avalon;

/// <summary>
/// Delegate for the event triggered during the creation of a new inner object.
/// </summary>
public delegate void DataSourceCreateEventHandler(object sender, DataSourceCreateEventArgs e);

/// <summary>
/// Delegate for events involving a generic inner object.
/// </summary>
public delegate void DataSourceObjectHandler(object sender, object innerObject);

/// <summary>
/// Delegate for standard data source events.
/// </summary>
public delegate void DataSourceEventHandler(object sender, DataSourceEventArgs e);

/// <summary>
/// Delegate for data source events that can be canceled.
/// </summary>
public delegate void DataSourceCancelEventHandler(object sender, DataSourceCancelEventArgs e);

/// <summary>
/// Delegate for events triggered when a data value is changing or has changed.
/// </summary>
public delegate void DataSourceChangeEventHandler(object sender, DataSourceChangeEventArgs e);

/// <summary>
/// Provides a centralized mechanism for data binding, navigation, and CRUD operations 
/// between UI controls and various underlying data sources.
/// </summary>
public class DataSource : INotifyPropertyChanged
{
    static private int Counter = 0;
    private IDataLink fLink;
    private DataSourceRow fCurrent;
    private DataSourceRowCollection<DataSourceRow> fRows;
    private string fTitle;
    private Type fSourceItemType;

    /// <summary>
    /// Initializes a new instance of the DataSource class using a specific data link.
    /// </summary>
    public DataSource(IDataLink Link)
    {
        this.fLink = Link ?? throw new ArgumentNullException(nameof(Link));
        this.fRows = new DataSourceRowCollection<DataSourceRow>();
        this.fSourceItemType = fLink.GetItemType();
        this.Load();
    }

    /// <summary>
    /// Creates a DataSource instance from a DataTable.
    /// </summary>
    public static DataSource FromTable(DataTable Table) => new DataSource(new DataTableLink(Table));

    /// <summary>
    /// Creates a DataSource instance from a list of items.
    /// </summary>
    public static DataSource FromList<T>(IList<T> List) => new DataSource(new ListLink<T>(List));

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    void OnPropertyChanged(string PropertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>
    /// Automatically generates grid columns based on the data source schema.
    /// </summary>
    protected virtual void CreateGridColumns(DataGrid Grid)
    {
        Grid.Columns.Clear();
        Grid.AutoGenerateColumns = false;

        foreach (string PropName in fLink.GetPropertyNames())
        {
            // Optional: We could check for BrowsableAttribute here 
            // if the Link doesn't already perform filtering.
            var GridCol = new DataGridTextColumn
            {
                Header = PropName,
                Binding = new Binding(string.Format("[{0}]", PropName))
            };
            
            //string S = GridCol.Binding.
            Grid.Columns.Add(GridCol);
        }
    }

    /// <summary>
    /// Forces the UI to synchronize its selection with the current record.
    /// </summary>
    protected virtual void ForceMoveToCurrent()
    {
        if (Rows.Count == 0) return;
        int Index = Rows.IndexOf(Current);
        
        if (Index <= 0) { Last(); First(); }
        else { First(); Position = Index; }
    }

    /// <summary>
    /// Binds a DataGrid to the data source.
    /// </summary>
    public virtual void Bind(DataGrid Grid, bool CreateColumns = false)
    {
        if (Grid == null) return;
        Grid.DataContext = this;
        Grid.Bind(DataGrid.ItemsSourceProperty, new Binding("Rows") { Mode = BindingMode.OneWay });
        Grid.Bind(DataGrid.SelectedItemProperty, new Binding("Current") { Mode = BindingMode.TwoWay });
        
        if (CreateColumns) this.CreateGridColumns(Grid);
        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a TextBox to a specific property of the current record.
    /// </summary>
    public virtual void Bind(TextBox Edt, string PropertyName)
    {
        if (Edt == null || string.IsNullOrEmpty(PropertyName)) return;
        Edt.DataContext = this;
        Edt.Bind(TextBox.TextProperty, new Binding(string.Format("Current[{0}]", PropertyName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
        });
        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a ComboBox as a lookup control.
    /// </summary>
    public virtual void Bind(ComboBox Cbo, DataSource LookupSource, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Cbo == null || LookupSource == null) return;

        // 1. Data Source
        Cbo.Bind(ComboBox.ItemsSourceProperty, new Binding("Rows") 
        { 
            Source = LookupSource, 
            Mode = BindingMode.OneWay 
        });

        // 2. Rendering: Use FuncDataTemplate to avoid "Cannot set both DisplayMemberBinding and ItemTemplate"
        // The row is the DataSourceRow contained in the LookupSource fRows
        Cbo.ItemTemplate = new FuncDataTemplate<DataSourceRow>((row, namescope) => 
        {
            var Block = new TextBlock();
            Block.Bind(TextBlock.TextProperty, new Binding($"[{DisplayMember}]"));
            return Block;
        });

        // 3. From UI to DataSource
        Cbo.SelectionChanged += (s, e) =>
        {
            if (Cbo.SelectedItem is DataSourceRow SelectedRow)
            {
                object NewValue = SelectedRow[ValueMember];
                object OldValue = this.GetValue(TargetPropertyName);

                if (!object.Equals(OldValue, NewValue))
                {
                    this.SetValue(TargetPropertyName, NewValue);
                }
            }
        };

        // 4. From DataSource to UI
        this.OnCurrentPositionChanged += (s, e) =>
        {
            object CurrentValue = this.GetValue(TargetPropertyName);
        
            if (CurrentValue == null)
            {
                Cbo.SelectedItem = null;
            }
            else
            {
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => 
                    object.Equals(r[ValueMember], CurrentValue));
            
                if (Cbo.SelectedItem != FoundRow)
                {
                    Cbo.SelectedItem = FoundRow;
                }
            }
        };
        
        // 5. Keyboard Handling: F4 for open, Enter for Tab behavior
        Cbo.KeyDown += (s, e) =>
        {
            if (e.Key == Key.F4)
            {
                Cbo.IsDropDownOpen = !Cbo.IsDropDownOpen;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                GetNextControl(Cbo)?.Focus();
                e.Handled = true;
            }
        };

        // ΠΡΟΣΘΗΚΗ: Listener για αλλαγή τιμής στο TargetPropertyName
        this.OnChanged += (s, e) =>
        {
            if (e.PropertyName == TargetPropertyName)
            {
                // Επαναλαμβάνουμε τη λογική εύρεσης της σωστής γραμμής στο Lookup
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => 
                    object.Equals(r[ValueMember], e.NewValue));
            
                if (Cbo.SelectedItem != FoundRow)
                    Cbo.SelectedItem = FoundRow;
            }
        };
        
        // 6. Text search only (Select-only)
        Cbo.IsEditable = false; 
        Cbo.IsTextSearchEnabled = true;

        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a ListBox to a specific property of the current record.
    /// </summary>
    public virtual void Bind(ListBox Lst, DataSource LookupSource, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Lst == null || LookupSource == null) return;

        Lst.Bind(ListBox.ItemsSourceProperty, new Binding("Rows") { Source = LookupSource });
    
        Lst.ItemTemplate = new FuncDataTemplate<DataSourceRow>((row, ns) => 
        {
            var tb = new TextBlock();
            tb.Bind(TextBlock.TextProperty, new Binding($"[{DisplayMember}]"));
            return tb;
        });

        // SelectedValue synchronization (using SelectionChanged like in ComboBox)
        Lst.SelectionChanged += (s, e) =>
        {
            if (Lst.SelectedItem is DataSourceRow row)
                this.SetValue(TargetPropertyName, row[ValueMember]);
        };

        this.OnCurrentPositionChanged += (s, e) =>
        {
            object val = this.GetValue(TargetPropertyName);
            Lst.SelectedItem = LookupSource.Rows.FirstOrDefault(r => object.Equals(r[ValueMember], val));
        };
        
        // 3. ΠΡΟΣΘΗΚΗ: Από τον DataSource προς το UI (Αλλαγή τιμής στο ίδιο Row)
        this.OnChanged += (s, e) =>
        {
            if (e.PropertyName == TargetPropertyName)
            {
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => object.Equals(r[ValueMember], e.NewValue));
                if (Lst.SelectedItem != FoundRow)
                    Lst.SelectedItem = FoundRow;
            }
        };
    
        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a CheckBox to a boolean property of the current record.
    /// </summary>
    public virtual void Bind(CheckBox Chk, string PropertyName)
    {
        if (Chk == null || string.IsNullOrEmpty(PropertyName)) return;
        Chk.DataContext = this;
        Chk.Bind(CheckBox.IsCheckedProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a ToggleSwitch to a boolean property of the current record.
    /// </summary>
    public virtual void Bind(ToggleSwitch Sw, string PropertyName)
    {
        if (Sw == null) return;
        Sw.DataContext = this;
        Sw.Bind(ToggleSwitch.IsCheckedProperty, new Binding($"Current[{PropertyName}]") { Mode = BindingMode.TwoWay });
        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a DatePicker to a date property of the current record.
    /// </summary>
    public virtual void Bind(DatePicker Dt, string PropertyName)
    {
        if (Dt == null || string.IsNullOrEmpty(PropertyName)) return;
        Dt.DataContext = this;
        Dt.Bind(DatePicker.SelectedDateProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a NumericUpDown to a numeric property of the current record.
    /// </summary>
    public virtual void Bind(NumericUpDown Num, string PropertyName)
    {
        if (Num == null || string.IsNullOrEmpty(PropertyName)) return;
        Num.DataContext = this;
        Num.Bind(NumericUpDown.ValueProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        ForceMoveToCurrent();
    }
    
    /// <summary>
    /// Loads the data from the underlying data link into the row collection.
    /// </summary>
    public virtual void Load()
    {
        this.OnLoading?.Invoke(this, EventArgs.Empty);

        var TempList = new List<DataSourceRow>();
        foreach (object Item in fLink.GetRows())
        {
            TempList.Add(new DataSourceRow(this, Item));
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
    public virtual DataSourceRow Add()
    {
        var CreateArgs = new DataSourceCreateEventArgs();
        this.OnCreateInnerObject?.Invoke(this, CreateArgs);
        if (CreateArgs.Cancel) return null;

        object NewInnerItem = CreateArgs.NewInnerObject ?? fLink.CreateNew();
        this.OnInnerObjectCreated?.Invoke(this, NewInnerItem);

        var NewRow = new DataSourceRow(this, NewInnerItem);
        if (!RaiseOnAdding(NewRow)) return null;

        // Commit to source via Link
        fLink.AddToSource(NewInnerItem);
        this.fRows.Add(NewRow);
        this.Current = NewRow;

        this.OnAdded?.Invoke(this, new DataSourceEventArgs(NewRow));
        return NewRow;
    }

    /// <summary>
    /// Deletes the specified record from the data source.
    /// </summary>
    public bool Delete(DataSourceRow Row)
    {
        if (Row == null || this.fRows == null) return false;

        // RaiseOnDeleting is called here. If it returns false, we stop.
        if (!RaiseOnDeleting(Row)) return false;

        int Index = this.fRows.IndexOf(Row);
        
        if (Index < 0) return false;

        try 
        {
            fLink.RemoveFromSource(Row.InnerObject);
            this.fRows.Remove(Row);

            if (this.Current == Row)
                this.Position = (this.fRows.Count > 0) ? Math.Max(0, Index - 1) : -1;

            this.OnDeleted?.Invoke(this, new DataSourceEventArgs(Row));
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
    public virtual DataSourceRow First() { Position = 0; return Current; }

    /// <summary>
    /// Moves the current selection to the last record.
    /// </summary>
    public virtual DataSourceRow Last() { Position = this.fRows.Count - 1; return Current; }

    /// <summary>
    /// Gets the value of a property from the current record as a specific type.
    /// </summary>
    public virtual T GetValue<T>(string PropertyName)
    {
        if (this.fCurrent == null) return default(T);
        return this.fCurrent.GetValue<T>(PropertyName);
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
    
    // ● Accessors (Proxy to Current)

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

    /// <summary>
    /// Finds and returns the next focusable control in the visual tree.
    /// </summary>
    public static IInputElement GetNextControl(Visual current)
    {
        // 1. Find the TopLevel (Window) using VisualTreeHelper
        var topLevel = current.GetVisualRoot() as TopLevel;
        if (topLevel == null) return null;
        
        // 2. KeyboardNavigationHandler.GetNext expects IInputElement. 
        // Almost all Controls in Avalonia implement this interface.
        if (current is IInputElement element)
        {
            return KeyboardNavigationHandler.GetNext(element, NavigationDirection.Next);
        }
        
        return null;
    }
    
    /// <summary>
    /// Internal trigger to raise the OnAdding event.
    /// </summary>
    internal bool RaiseOnAdding(DataSourceRow row) {
        var e = new DataSourceCancelEventArgs(row);
        OnAdding?.Invoke(this, e);
        return !e.Cancel;
    }
    /// <summary>
    /// Internal trigger to raise the OnDeleting event.
    /// </summary>
    internal bool RaiseOnDeleting(DataSourceRow row) {
        var e = new DataSourceCancelEventArgs(row);
        OnDeleting?.Invoke(this, e);
        return !e.Cancel;
    }    
    /// <summary>
    /// Internal trigger to raise the OnChanging event.
    /// </summary>
    internal bool RaiseOnChanging(DataSourceRow row, string propName, object oldVal, object newVal) {
        var e = new DataSourceChangeEventArgs(row, propName, oldVal, newVal);
        OnChanging?.Invoke(this, e);
        return !e.Cancel;
    }
    /// <summary>
    /// Internal trigger to raise the OnChanged event.
    /// </summary>
    internal void RaiseOnChanged(DataSourceRow row, string propName, object oldVal, object newVal) {
        OnChanged?.Invoke(this, new DataSourceChangeEventArgs(row, propName, oldVal, newVal));
    }      
    
    /// <summary>
    /// Gets the collection of records as DataSourceRow wrappers.
    /// </summary>
    public ObservableCollection<DataSourceRow> Rows => this.fRows;

    /// <summary>
    /// Gets the total number of records.
    /// </summary>
    public int Count => Rows.Count;

    /// <summary>
    /// Gets a value indicating whether the underlying source has a fixed size.
    /// </summary>
    public bool IsFixedSize => fLink.IsFixedSize;

    /// <summary>
    /// Gets the underlying data link.
    /// </summary>
    public IDataLink Link => fLink;

    /// <summary>
    /// Gets the type of the items contained in the source.
    /// </summary>
    public Type SourceItemType => fSourceItemType;
    
    /// <summary>
    /// Gets or sets the currently selected record.
    /// </summary>
    public virtual DataSourceRow Current
    {
        get => this.fCurrent;
        set
        {
            if (this.fCurrent != value)
            {
                this.fCurrent = value;
                this.OnPropertyChanged(nameof(Current));
                this.OnPropertyChanged(nameof(Position));
                this.OnCurrentPositionChanged?.Invoke(this, new DataSourceEventArgs(this.fCurrent));
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

            var Args = new DataSourceCancelEventArgs(fCurrent);
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
    public event DataSourceCreateEventHandler OnCreateInnerObject;

    /// <summary>
    /// Occurs when a new inner object has been successfully created.
    /// </summary>
    public event DataSourceObjectHandler OnInnerObjectCreated;

    /// <summary>
    /// Occurs before a record is added to the collection.
    /// </summary>
    public event DataSourceCancelEventHandler OnAdding;

    /// <summary>
    /// Occurs after a record has been added to the collection.
    /// </summary>
    public event DataSourceEventHandler OnAdded;

    /// <summary>
    /// Occurs before a record is deleted.
    /// </summary>
    public event DataSourceCancelEventHandler OnDeleting;

    /// <summary>
    /// Occurs after a record has been deleted.
    /// </summary>
    public event DataSourceEventHandler OnDeleted;

    /// <summary>
    /// Occurs before a data value is changed.
    /// </summary>
    public event DataSourceChangeEventHandler OnChanging;

    /// <summary>
    /// Occurs after a data value has been changed.
    /// </summary>
    public event DataSourceChangeEventHandler OnChanged;

    /// <summary>
    /// Occurs before the current record position changes.
    /// </summary>
    public event DataSourceCancelEventHandler OnCurrentPositionChanging;

    /// <summary>
    /// Occurs after the current record position has changed.
    /// </summary>
    public event DataSourceEventHandler OnCurrentPositionChanged;

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