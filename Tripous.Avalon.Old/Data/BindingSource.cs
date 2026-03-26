using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Tripous.Avalon.Data;

 

/// <summary>
/// Provides a centralized mechanism for data binding, navigation, and CRUD operations 
/// between UI controls and various underlying data sources.
/// </summary>
public class BindingSource : INotifyPropertyChanged
{
    static private int Counter = 0;
    private BindingSourceRow fCurrent;
    private BindingSourceRowCollection<BindingSourceRow> fRows;
    private List<BindingSourceRow> fAllRows;
    private string fTitle;
    private List<BindingRelation> fRelations = new();
    private object[] LastMasterValues = null;
 


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
        this.fAllRows = new List<BindingSourceRow>();
        this.SourceItemType = DataSource.GetItemType();
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
        fAllRows.Clear();
        fRows.Clear();
        foreach (object Item in DataSource.GetRows())
        {
            fAllRows.Add(new BindingSourceRow(this, Item));
        }
        VisibleRowsChanged(null); 
        this.Current = (this.fRows.Count > 0) ? this.fRows[0] : null;
        this.OnLoaded?.Invoke(this, EventArgs.Empty);
    }    
    /// <summary>
    /// Clears all records from the row collection.
    /// </summary>
    public virtual void Clear()
    {
        if (fAllRows.Count == 0 && fRows.Count == 0) return;
        this.OnEmptying?.Invoke(this, EventArgs.Empty);
        this.fAllRows.Clear();
        this.fRows.Clear();
        this.LastMasterValues = null;
        this.Current = null;
        this.OnEmptied?.Invoke(this, EventArgs.Empty);
        this.OnPropertyChanged(nameof(Count));
        this.OnPropertyChanged(nameof(Position));
    }    
    
    /// <summary>
    /// Creates and returns a new <see cref="BindingSourceRow"/>.
    /// <para>The new row should be added to rows by calling <see cref="AddRow()"/></para>
    /// </summary>
    public virtual BindingSourceRow NewRow()
    {
        var CreateArgs = new BindingSourceCreateArgs();
        this.OnCreateInnerObject?.Invoke(this, CreateArgs);
        if (CreateArgs.Cancel) return null;
    
        object NewInnerItem = CreateArgs.NewInnerObject ?? DataSource.CreateNew();
    
        var InnerObjectArgs = new BindingSourceInnerObjectArgs(NewInnerItem);
        this.OnInnerObjectCreated?.Invoke(this, InnerObjectArgs);
    
        return new BindingSourceRow(this, NewInnerItem);
    }
    /// <summary>
    /// Adds a new record to the data source.
    /// </summary>
    public virtual void AddRow(BindingSourceRow Row)
    {
        if (Row == null) return;

        /* 1. Auto-assign Master keys ONLY if they are missing/null */
        if (IsDetail && LastMasterValues != null)
        {
            var Relation = Master.BindingRelations.FirstOrDefault(x => x.Child == this);
            if (Relation != null)
            {
                for (int i = 0; i < Relation.ChildPropertyNames.Length; i++)
                {
                    string childProp = Relation.ChildPropertyNames[i];
                
                    // If the user hasn't provided a value, we sync it from Master
                    if (Row[childProp] == null || Row[childProp] == DBNull.Value)
                    {
                        Row[childProp] = LastMasterValues[i];
                    }
                }
            }
        }

        /* 2. Standard Add Logic: Events and Collections */
        if (!RaiseOnAdding(Row)) return;
    
        DataSource.AddToSource(Row.InnerObject);
        this.fAllRows.Add(Row);

        /* 3. Visibility & Currency */
        bool ShouldBeVisible = IsInnerObjectVisible(Row.InnerObject);
        if (ShouldBeVisible)
        {
            this.fRows.Add(Row);
            this.Current = Row; 
        }

        if (IsDetail)
            UpdateVisibleRows();

        this.OnAdded?.Invoke(this, new BindingSourceArgs(Row));
    }
    /// <summary>
    /// Creates, initializes with the provided values, and adds a new row.
    /// Values are assigned based on the order of properties/columns in the DataSource.
    /// </summary>
    public virtual BindingSourceRow AddRow(params object[] Values)
    {
        /* 1. Create the 'offline' row */
        BindingSourceRow row = NewRow();
        if (row == null) return null;

        /* 2. Assign values by index */
        if (Values != null && Values.Length > 0)
        {
            string[] propNames = DataSource.GetPropertyNames();
            int count = Math.Min(Values.Length, propNames.Length);

            for (int i = 0; i < count; i++)
            {
                row[propNames[i]] = Values[i];
            }
        }

        /* 3. Let the standard AddRow logic handle the rest (Keys, Visibility, Events) */
        AddRow(row);
    
        return row;
    }  
    
    /// <summary>
    /// Deletes the specified record from the data source.
    /// </summary>
    public bool Delete(BindingSourceRow Row)
    {
        if (Row == null) return false;
    
        /* 1. Fire the Before-Delete event to allow cancellation */
        if (!RaiseOnDeleting(Row)) return false;

        try 
        {
            /* 2. Cascade Delete: Cleanup children first while the Master row still exists */
            if (this.CascadeDelete && fRelations.Count > 0)
            {
                foreach (var relation in fRelations)
                {
                    /* Get the master keys specifically from the row being deleted */
                    object[] masterKeys = this.GetMasterValues(relation, Row);
                
                    /* Instruct the child to delete all its records matching these keys */
                    relation.Child.DeleteByMasterValues(masterKeys);
                }
            }

            /* 3. Physical deletion from the underlying Data Source */
            DataSource.RemoveFromSource(Row.InnerObject);
        
            /* 4. Update local collections and position */
            int IndexInVisible = this.fRows.IndexOf(Row);
        
            this.fAllRows.Remove(Row);
            this.fRows.Remove(Row);

            /* If we deleted the current row, move the pointer to a valid neighbor */
            if (this.Current == Row)
            {
                if (this.fRows.Count == 0)
                    this.Position = -1;
                else
                    this.Position = Math.Clamp(IndexInVisible, 0, this.fRows.Count - 1);
            }

            /* 5. Notify observers */
            this.OnDeleted?.Invoke(this, new BindingSourceArgs(Row));
            return true;
        }
        catch (Exception Ex)
        {
            /* Provide context-rich error message */
            throw new ApplicationException($"DataSource: {Title} - Delete failed.", Ex);
        }
    }
    /// <summary>
    /// Deletes the current record.
    /// </summary>
    public bool Delete() => this.Delete(this.fCurrent);
    /// <summary>
    /// Deletes all rows from this BindingSource that match the provided master values,
    /// based on the master-detail relationship.
    /// </summary>
    internal void DeleteByMasterValues(object[] MasterValues)
    {
        /* 1. Guard clause: Ensure we have values to match and that this source is indeed a detail */
        if (MasterValues == null || DataSource.DetailPropertyNames == null)
            return;

        /* 2. Identify all rows in the underlying data storage that match the master keys.
           We use fAllRows to ensure we catch records even if they are currently filtered out of view. */
        var RowsToDelete = fAllRows.Where(row => 
            DataSource.PassesDetailCondition(row.InnerObject, MasterValues)
        ).ToList();

        /* 3. Iterate and remove each matching row */
        foreach (var row in RowsToDelete)
        {
            /* Recursive Step: If cascade delete is enabled, notify this row's own children */
            if (this.CascadeDelete && fRelations.Count > 0)
            {
                foreach (var relation in fRelations)
                {
                    /* Get the master keys specifically from the row being deleted */
                    object[] childKeys = this.GetMasterValues(relation, row);
                    relation.Child.DeleteByMasterValues(childKeys);
                }
            }

            /* 4. Physical deletion from Data Source and internal collections */
            DataSource.RemoveFromSource(row.InnerObject);
            fAllRows.Remove(row);
            fRows.Remove(row);
        
            /* Ensure Current is reset if it was the deleted row */
            if (this.Current == row)
                this.Current = fRows.FirstOrDefault();
        }
    }    
    
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

    // ● relations - master-detail 
    /// <summary>
    /// Adds a detail to this instance.
    /// </summary>
    public BindingRelation AddDetail(string Name, string ParentPropertyName, BindingSource Child, string ChildPropertyName)
    {
        return AddDetail(Name, new[] { ParentPropertyName }, Child, new[] { ChildPropertyName });
    }
    /// <summary>
    /// Adds a detail to this instance.
    /// </summary>
    public BindingRelation AddDetail(string Name, string[] ParentPropertyNames, BindingSource Child, string[] ChildPropertyNames)
    {
        if (fRelations.Any(x => x.Child == Child))
            throw new ApplicationException("The specified detail already exists.");
        
        if (Child == null)
            throw new ApplicationException("The specified detail cannot be null.");
        
        if (ChildPropertyNames == null ||  ChildPropertyNames.Length == 0 || ParentPropertyNames == null || ParentPropertyNames.Length == 0)
            throw  new ApplicationException("The specified detail Parent and Detail PropertyNames cannot be null or empty.");

        Child.Master = this;
        
        var relation = new BindingRelation(Name, this, ParentPropertyNames, Child, ChildPropertyNames);
        fRelations.Add(relation);
        Child.DataSource.DetailPropertyNames = ChildPropertyNames;
        return relation;
    }
    /// <summary>
    /// Removes a detail from this instance.
    /// </summary>
    public void RemoveDetail(BindingSource Child)
    {
        BindingRelation Relation = fRelations.FirstOrDefault(x => x.Child == Child);
        if (Relation != null)
            RemoveDetail(Relation);
    }
    /// <summary>
    /// Removes a detail from this instance.
    /// </summary>
    public void RemoveDetail(BindingRelation Relation)
    {
        if (fRelations.Contains(Relation))
        {
            Relation.Child.Master = null;
            Relation.Child.DataSource.DetailPropertyNames = null;
            fRelations.Remove(Relation);
        }
    }

    /// <summary>
    /// Sets whether this master propagates position changes to its details.
    /// </summary>
    public void ActivateDetails(bool Value, bool Propagate = true)
    {
        if (Value != this.DetailsActive)
        {
            this.DetailsActive = Value;

            /* 1. If activated, refresh the child views immediately */
            if (this.DetailsActive)
            {
                foreach (var relation in fRelations)
                {
                    /* Get current master values and push to child */
                    object[] masterValues = GetMasterValues(relation);
                    relation.Child.VisibleRowsChanged(masterValues);
                }
            }

            /* 2. Recursive propagation to the entire detail tree */
            if (Propagate)
            {
                foreach (var relation in fRelations)
                {
                    relation.Child.ActivateDetails(Value, Propagate);
                }
            }
            
            this.OnPropertyChanged(nameof(DetailsActive));
        }
    }
    bool IsInnerObjectVisible(object InnerObject)
    {
        // If this is a detail (has DetailPropertyNames) but we have no Master values, 
        // then it should probably be invisible.
        if (DataSource.DetailPropertyNames != null && LastMasterValues == null)
            return false;

        bool PassesDetail = (LastMasterValues == null) ? true : DataSource.PassesDetailCondition(InnerObject, LastMasterValues);
        return PassesDetail && DataSource.PassesFilterCondition(InnerObject);
    }
    
    /// <summary>
    /// Returns the master values according to <see cref="BindingRelation.ParentPropertyNames"/>
    /// </summary>
    internal object[] GetMasterValues1(BindingRelation Relation)
    {
        if (this.fCurrent == null || Relation == null) return null;
        object[] Values = new object[Relation.ParentPropertyNames.Length];
        for (int i = 0; i < Relation.ParentPropertyNames.Length; i++)
        {
            Values[i] = this.fCurrent[Relation.ParentPropertyNames[i]];
        }
        return Values;
    }
    /// <summary>
    /// Returns the master values according to <see cref="BindingRelation.ParentPropertyNames"/>.
    /// If Row is null, it uses the current row (fCurrent).
    /// </summary>
    internal object[] GetMasterValues(BindingRelation Relation, BindingSourceRow Row = null)
    {
        /* Use the provided row, or fallback to fCurrent if Row is null */
        BindingSourceRow TargetRow = Row ?? this.fCurrent;

        if (TargetRow == null || Relation == null) 
            return null;

        object[] Values = new object[Relation.ParentPropertyNames.Length];
        for (int i = 0; i < Relation.ParentPropertyNames.Length; i++)
        {
            /* Access the values via the row's indexer */
            Values[i] = TargetRow[Relation.ParentPropertyNames[i]];
        }
        return Values;
    }
    /// <summary>
    /// Called by a <see cref="BindingRelation"/> when this is a detail to a master and the master changes position.
    /// </summary>
    internal void VisibleRowsChanged(object[] MasterValues)
    {
        LastMasterValues = MasterValues;
        
        List<BindingSourceRow> ToAdd = new List<BindingSourceRow>();
        List<BindingSourceRow> ToRemove = new List<BindingSourceRow>();
        foreach (BindingSourceRow Row in fAllRows)
        {
            bool ShouldBeVisible = IsInnerObjectVisible(Row.InnerObject);
            bool IsVisible = fRows.Contains(Row);
            if (ShouldBeVisible && !IsVisible)
                ToAdd.Add(Row);
            else if (!ShouldBeVisible && IsVisible)
                ToRemove.Add(Row);
        }
        foreach (var Row in ToRemove)
            fRows.Remove(Row);
        foreach (var Row in ToAdd)
            fRows.Add(Row);
        if (fCurrent == null || !fRows.Contains(fCurrent))
            this.Position = fRows.Count > 0 ? 0 : -1;
        else
            OnPropertyChanged(nameof(Position));
    }

    public void UpdateVisibleRows()
    {
        VisibleRowsChanged(LastMasterValues);
    }
    
    // ● filter
    /// <summary>
    /// Sets the filter
    /// </summary>
    public void SetFilter(string PropertyName, object Value)
    {
        DataSource.SetFilter(PropertyName, Value);
        this.VisibleRowsChanged(this.LastMasterValues);
    }
    /// <summary>
    /// Clears the filter
    /// </summary>
    public void CancelFilter()
    {
        DataSource.ClearFilter();
        this.VisibleRowsChanged(this.LastMasterValues);
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
    public Type SourceItemType { get; private set; }
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
    /// The list of detail relations. Used when this is a master.
    /// </summary>
    public BindingRelation[] BindingRelations => fRelations.ToArray();

    public BindingSource Master { get; internal set; }

    /// <summary>
    /// When false the movement to another position does not propagated to details. Used when this is a master.
    /// </summary>
    public bool DetailsActive { get; private set; }

    public bool CascadeDelete { get; set; } = true;

    public bool IsDetail => Master != null;
    public bool IsFiltered =>
        !string.IsNullOrWhiteSpace(DataSource.FilterPropertyName) && (DataSource.FilterValue != null);


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

    
    // ● events
    /// <summary>
    /// Event triggered when a property of the DataSource itself changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    /// <summary>
    /// Occurs when a new inner object is requested.
    /// </summary>
    public event EventHandler<BindingSourceCreateArgs> OnCreateInnerObject;   
    /// <summary>
    /// Occurs when a new inner object has been successfully created.
    /// </summary>
    public event EventHandler<BindingSourceInnerObjectArgs> OnInnerObjectCreated; 
    /// <summary>
    /// Occurs before a record is added to the collection.
    /// </summary>
    public event EventHandler<BindingSourceCancelArgs> OnAdding;  
    /// <summary>
    /// Occurs after a record has been added to the collection.
    /// </summary>
    public event EventHandler<BindingSourceArgs> OnAdded; 
    /// <summary>
    /// Occurs before a record is deleted.
    /// </summary>
    public event EventHandler<BindingSourceCancelArgs> OnDeleting;   
    /// <summary>
    /// Occurs after a record has been deleted.
    /// </summary>
    public event EventHandler<BindingSourceArgs> OnDeleted;
    /// <summary>
    /// Occurs before a data value is changed.
    /// </summary>
    public event EventHandler<BindingSourceChangeArgs> OnChanging;   
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