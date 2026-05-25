namespace Tripous.Data;

/// <summary>
/// Coordinates rows, current position, navigation, changes, and relations for a data provider.
/// </summary>
public class DataSource: INotifyPropertyChanged
{
    // ● private fields
    IDataProvider fProvider;
    ObservableCollection<DataSourceRow> fRows;
    List<DataSourceRow> fAllRows;
    DataSourceRow fCurrent;
    int fPosition = -1;
    DataSource fMaster;
    DataSourceRelation fMasterRelation;
    List<DataSourceRelation> fRelations;
    List<DataSource> fDetails;
    string fFilterFieldName;
    object fFilterValue;

    // ● private
    void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    void SetCurrentByPosition()
    {
        Current = fPosition >= 0 && fPosition < fRows.Count ? fRows[fPosition] : null;
    }
    bool RaisePositionChanging(int OldPosition, int NewPosition, DataSourceRow OldCurrent, DataSourceRow NewCurrent)
    {
        DataSourcePositionCancelEventArgs Args = new(OldPosition, NewPosition, OldCurrent, NewCurrent);
        PositionChanging?.Invoke(this, Args);
        return !Args.Cancel;
    }
    void RaisePositionChanged(int OldPosition, int NewPosition, DataSourceRow OldCurrent, DataSourceRow NewCurrent)
    {
        PositionChanged?.Invoke(this, new DataSourcePositionEventArgs(OldPosition, NewPosition, OldCurrent, NewCurrent));
    }
    internal bool RaiseChanging(DataSourceRow Row, string FieldName, object OldValue, object NewValue)
    {
        DataSourceChangeEventArgs Args = new(Row, FieldName, OldValue, NewValue);
        Changing?.Invoke(this, Args);
        return !Args.Cancel;
    }
    internal void RaiseChanged(DataSourceRow Row, string FieldName, object OldValue, object NewValue)
    {
        Changed?.Invoke(this, new DataSourceChangeEventArgs(Row, FieldName, OldValue, NewValue));
    }
    bool RaiseCreating(DataSourceCreateEventArgs Args)
    {
        Creating?.Invoke(this, Args);
        return !Args.Cancel;
    }
    void RaiseCreated(object InnerObject)
    {
        Created?.Invoke(this, new DataSourceInnerObjectEventArgs(InnerObject));
    }
    bool RaiseRowCancel(EventHandler<DataSourceRowCancelEventArgs> Event, DataSourceRow Row)
    {
        DataSourceRowCancelEventArgs Args = new(Row);
        Event?.Invoke(this, Args);
        return !Args.Cancel;
    }
    void RaiseRow(EventHandler<DataSourceRowEventArgs> Event, DataSourceRow Row)
    {
        Event?.Invoke(this, new DataSourceRowEventArgs(Row));
    }
    bool RaiseCancel(EventHandler<DataSourceCancelEventArgs> Event)
    {
        DataSourceCancelEventArgs Args = new();
        Event?.Invoke(this, Args);
        return !Args.Cancel;
    }
    void Provider_ItemChanged(object Sender, DataProviderChangedEventArgs e)
    {
        foreach (DataSourceRow Row in fAllRows)
        {
            if (fProvider.IsSameItem(Row.InnerObject, e.InnerObject))
            {
                Row.NotifyFieldChanged(e.FieldName);
                RaiseChanged(Row, e.FieldName, e.OldValue, e.NewValue);
                break;
            }
        }

        if (IsFiltered && string.Equals(fFilterFieldName, e.FieldName, StringComparison.OrdinalIgnoreCase))
            RebuildRows();
    }
    void Parent_PropertyChanged(object Sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Current) && fMaster != null && fMaster.DetailsActive)
            RebuildRows();
    }
    bool PassesMasterFilter(DataSourceRow Row)
    {
        if (fMasterRelation == null || fMaster == null || fMaster.Current == null)
            return true;

        for (int i = 0; i < fMasterRelation.ParentFieldNames.Length; i++)
        {
            object ParentValue = fMaster.Current[fMasterRelation.ParentFieldNames[i]];
            object ChildValue = Row[fMasterRelation.ChildFieldNames[i]];

            if (!Equals(ParentValue, ChildValue))
                return false;
        }

        return true;
    }
    bool PassesFilter(DataSourceRow Row)
    {
        if (!IsFiltered)
            return true;

        object Value = Row[fFilterFieldName];

        if (Value is string Text && fFilterValue is string FilterText)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;
            if (FilterText.EndsWith("*") && FilterText.Length > 1)
                return Text.StartsWith(FilterText.Substring(0, FilterText.Length - 1), StringComparison.OrdinalIgnoreCase);

            return Text.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
        }

        return Equals(Value, fFilterValue);
    }
    bool PassesRowFilters(DataSourceRow Row)
    {
        return PassesMasterFilter(Row) && PassesFilter(Row);
    }
    bool RowMatchesRelation(DataSourceRelation Relation, DataSourceRow ParentRow, DataSourceRow ChildRow)
    {
        for (int i = 0; i < Relation.ParentFieldNames.Length; i++)
        {
            object ParentValue = ParentRow[Relation.ParentFieldNames[i]];
            object ChildValue = ChildRow[Relation.ChildFieldNames[i]];

            if (!Equals(ParentValue, ChildValue))
                return false;
        }

        return true;
    }
    bool HasDetailRows(DataSourceRow ParentRow)
    {
        foreach (DataSourceRelation Relation in fRelations)
        {
            foreach (DataSourceRow ChildRow in Relation.Child.AllRows)
            {
                if (RowMatchesRelation(Relation, ParentRow, ChildRow))
                    return true;
            }
        }

        return false;
    }
    DataSourceRelation FindRelation(DataSource Child)
    {
        return fRelations.FirstOrDefault(Relation => ReferenceEquals(Relation.Child, Child));
    }
    DataSourceRelation FindRelation(string ChildName)
    {
        return fRelations.FirstOrDefault(Relation => string.Equals(Relation.Child.Name, ChildName, StringComparison.OrdinalIgnoreCase));
    }
    string CreateRelationName(DataSource Child)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ApplicationException("Master DataSource name is not specified.");
        if (Child == null)
            throw new ArgumentNullException(nameof(Child));
        if (string.IsNullOrWhiteSpace(Child.Name))
            throw new ApplicationException("Detail DataSource name is not specified.");

        return $"{Name}_TO_{Child.Name}";
    }
    void ValidateRelation(DataSource Child, string[] ParentFieldNames, string[] ChildFieldNames)
    {
        if (Child == null)
            throw new ArgumentNullException(nameof(Child));
        if (ReferenceEquals(Child, this))
            throw new ApplicationException("A DataSource cannot be a detail of itself.");
        if (ParentFieldNames == null || ParentFieldNames.Length == 0)
            throw new ArgumentException("Parent field names are not specified.", nameof(ParentFieldNames));
        if (ChildFieldNames == null || ChildFieldNames.Length == 0)
            throw new ArgumentException("Child field names are not specified.", nameof(ChildFieldNames));
        if (ParentFieldNames.Length != ChildFieldNames.Length)
            throw new ArgumentException("Parent and child field name counts do not match.", nameof(ChildFieldNames));
        if (fDetails.Contains(Child))
            throw new ApplicationException($"Detail DataSource already exists: {Child.Name}");
        if (Child.Master != null)
            throw new ApplicationException($"Detail DataSource already has a master: {Child.Name}");
    }
    void DeleteDetailRows(DataSourceRow ParentRow)
    {
        foreach (DataSourceRelation Relation in fRelations)
        {
            List<DataSourceRow> Rows = Relation.Child.AllRows
                .Where(ChildRow => RowMatchesRelation(Relation, ParentRow, ChildRow))
                .ToList();

            foreach (DataSourceRow Row in Rows)
                Relation.Child.DeleteRow(Row);
        }
    }
    void RebuildRows()
    {
        DataSourceRow OldCurrent = Current;

        fRows.Clear();

        foreach (DataSourceRow Row in fAllRows)
        {
            if (PassesRowFilters(Row))
                fRows.Add(Row);
        }

        if (OldCurrent != null && fRows.Contains(OldCurrent))
            Current = OldCurrent;
        else
            Position = fRows.Count > 0 ? 0 : -1;

        OnPropertyChanged(nameof(Count));
    }
    void SetMaster(DataSource Master, DataSourceRelation Relation)
    {
        ClearMaster();
        fMaster = Master;
        fMasterRelation = Relation;
        fMaster.PropertyChanged += Parent_PropertyChanged;
        RebuildRows();
    }
    void ClearMaster()
    {
        if (fMaster != null)
            fMaster.PropertyChanged -= Parent_PropertyChanged;

        fMaster = null;
        fMasterRelation = null;
        RebuildRows();
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSource(IDataProvider Provider)
    {
        fProvider = Provider;
        fProvider.ItemChanged += Provider_ItemChanged;
        fRows = new();
        fAllRows = new();
        fRelations = new();
        fDetails = new();
        DetailsActive = true;
        Load();
    }
    /// <summary>
    /// Initializes a new instance with a name.
    /// </summary>
    public DataSource(IDataProvider Provider, string Name) : this(Provider)
    {
        this.Name = Name;
    }

    // ● static public
    /// <summary>
    /// Creates a DataSource from a DataTable.
    /// </summary>
    static public DataSource FromTable(DataTable Table)
    {
        return new DataSource(new DataTableProvider(Table), Table.TableName);
    }
    /// <summary>
    /// Creates a DataSource from a DataTable.
    /// </summary>
    static public DataSource FromTable(DataTable Table, string Name)
    {
        return new DataSource(new DataTableProvider(Table), Name);
    }
    /// <summary>
    /// Creates a DataSource from a DataView.
    /// </summary>
    static public DataSource FromDataView(DataView View)
    {
        return new DataSource(new DataViewProvider(View), View.Table.TableName);
    }
    /// <summary>
    /// Creates a DataSource from a DataView.
    /// </summary>
    static public DataSource FromDataView(DataView View, string Name)
    {
        return new DataSource(new DataViewProvider(View), Name);
    }
    /// <summary>
    /// Creates a DataSource from a list of notifying objects.
    /// </summary>
    static public DataSource FromList<T>(IList<T> List) where T: class, INotifyPropertyChanged, new()
    {
        return new DataSource(new ListProvider<T>(List));
    }
    /// <summary>
    /// Creates a DataSource from a list of notifying objects.
    /// </summary>
    static public DataSource FromList<T>(IList<T> List, string Name) where T: class, INotifyPropertyChanged, new()
    {
        return new DataSource(new ListProvider<T>(List), Name);
    }

    // ● public
    /// <summary>
    /// Loads rows from the provider.
    /// </summary>
    public void Load()
    {
        if (!RaiseCancel(Loading))
            return;

        fRows.Clear();
        fAllRows.Clear();

        foreach (object Item in fProvider.GetItems())
            fAllRows.Add(new DataSourceRow(this, Item));

        RebuildRows();
        Loaded?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Clears all loaded rows from the DataSource.
    /// </summary>
    public void Clear()
    {
        if (!RaiseCancel(Clearing))
            return;

        fRows.Clear();
        fAllRows.Clear();
        Current = null;
        OnPropertyChanged(nameof(Count));
        Cleared?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Adds a detail relation using one parent field and one child field.
    /// </summary>
    public DataSourceRelation AddDetail(DataSource Child, string ParentFieldName, string ChildFieldName)
    {
        return AddDetail(Child, new[] { ParentFieldName }, new[] { ChildFieldName });
    }
    /// <summary>
    /// Adds a detail relation using parent and child field arrays.
    /// </summary>
    public DataSourceRelation AddDetail(DataSource Child, string[] ParentFieldNames, string[] ChildFieldNames)
    {
        ValidateRelation(Child, ParentFieldNames, ChildFieldNames);
        DataSourceRelation Relation = new(CreateRelationName(Child), this, Child, ParentFieldNames, ChildFieldNames);

        fRelations.Add(Relation);
        fDetails.Add(Child);
        Child.SetMaster(this, Relation);
        return Relation;
    }
    /// <summary>
    /// Removes a detail relation.
    /// </summary>
    public void RemoveDetail(DataSourceRelation Relation)
    {
        if (Relation == null)
            return;

        if (fRelations.Remove(Relation))
        {
            fDetails.Remove(Relation.Child);
            Relation.Child.ClearMaster();
        }
    }
    /// <summary>
    /// Removes a detail relation by child DataSource.
    /// </summary>
    public void RemoveDetail(DataSource Child)
    {
        RemoveDetail(FindRelation(Child));
    }
    /// <summary>
    /// Removes a detail relation by child DataSource name.
    /// </summary>
    public void RemoveDetail(string Name)
    {
        RemoveDetail(FindRelation(Name));
    }
    /// <summary>
    /// Finds a detail DataSource by name, or returns null.
    /// </summary>
    public DataSource FindDetail(string Name)
    {
        return fDetails.FirstOrDefault(Detail => string.Equals(Detail.Name, Name, StringComparison.OrdinalIgnoreCase));
    }
    /// <summary>
    /// Gets a detail DataSource by name, or throws an exception.
    /// </summary>
    public DataSource GetDetail(string Name)
    {
        DataSource Result = FindDetail(Name);

        if (Result == null)
            throw new ApplicationException($"Detail DataSource not found: {Name}");

        return Result;
    }
    
    /// <summary>
    /// Creates a new row without adding it to the source.
    /// </summary>
    public DataSourceRow NewRow()
    {
        DataSourceCreateEventArgs Args = new();

        if (!RaiseCreating(Args))
            return null;

        object InnerObject = Args.InnerObject ?? fProvider.CreateItem();
        DataSourceRow Row = new(this, InnerObject);
        RaiseCreated(InnerObject);

        if (fMasterRelation != null && fMaster != null && fMaster.Current != null)
        {
            for (int i = 0; i < fMasterRelation.ParentFieldNames.Length; i++)
                Row[fMasterRelation.ChildFieldNames[i]] = fMaster.Current[fMasterRelation.ParentFieldNames[i]];
        }

        return Row;
    }
    /// <summary>
    /// Adds a row to the source.
    /// </summary>
    public void AddRow(DataSourceRow Row)
    {
        if (Row == null)
            return;
        if (!RaiseRowCancel(Adding, Row))
            return;

        fProvider.AddItem(Row.InnerObject);
        fAllRows.Add(Row);

        if (PassesRowFilters(Row))
            fRows.Add(Row);

        Current = Row;
        OnPropertyChanged(nameof(Count));
        RaiseRow(Added, Row);
    }
    /// <summary>
    /// Creates and adds a new row.
    /// </summary>
    public DataSourceRow AppendRow()
    {
        DataSourceRow Row = NewRow();
        AddRow(Row);
        return Row;
    }
    /// <summary>
    /// Creates and adds a new row.
    /// </summary>
    public DataSourceRow AddNew()
    {
        return AppendRow();
    }
    
    /// <summary>
    /// Deletes the current row.
    /// </summary>
    public bool DeleteCurrent()
    {
        if (Current == null)
            return false;

        if (CascadeDeleteRule == CascadeDeleteRule.Restrict && HasDetailRows(Current))
            return false;
        if (CascadeDeleteRule == CascadeDeleteRule.Cascade)
            DeleteDetailRows(Current);

        return DeleteRow(Current);
    }
    /// <summary>
    /// Deletes the specified row.
    /// </summary>
    public bool DeleteRow(DataSourceRow Row)
    {
        if (Row == null)
            return false;
        if (!RaiseRowCancel(Deleting, Row))
            return false;

        int OldPosition = fRows.IndexOf(Row);

        fProvider.DeleteItem(Row.InnerObject);
        fRows.Remove(Row);
        fAllRows.Remove(Row);

        if (fRows.Count == 0)
            Current = null;
        else if (OldPosition < fRows.Count)
            Position = OldPosition;
        else
            Position = fRows.Count - 1;

        OnPropertyChanged(nameof(Count));
        RaiseRow(Deleted, Row);
        return true;
    }
    
    /// <summary>
    /// Moves to the first row.
    /// </summary>
    public bool MoveFirst()
    {
        if (fRows.Count == 0)
            return false;

        Position = 0;
        return true;
    }
    /// <summary>
    /// Moves to the last row.
    /// </summary>
    public bool MoveLast()
    {
        if (fRows.Count == 0)
            return false;

        Position = fRows.Count - 1;
        return true;
    }
    /// <summary>
    /// Moves to the next row.
    /// </summary>
    public bool MoveNext()
    {
        if (Position >= fRows.Count - 1)
            return false;

        Position++;
        return true;
    }
    /// <summary>
    /// Moves to the previous row.
    /// </summary>
    public bool MovePrevious()
    {
        if (Position <= 0)
            return false;

        Position--;
        return true;
    }
    
    /// <summary>
    /// Refreshes the current row notification.
    /// </summary>
    public void RefreshCurrent()
    {
        SetCurrentByPosition();
        OnPropertyChanged(nameof(Current));
    }
    /// <summary>
    /// Refreshes the visible rows.
    /// </summary>
    public void RefreshRows()
    {
        RebuildRows();
    }
    /// <summary>
    /// Sets a field/value filter.
    /// </summary>
    public void SetFilter(string FieldName, object Value)
    {
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentException("Filter field name is not specified.", nameof(FieldName));
        if (!Provider.ContainsField(FieldName))
            throw new ApplicationException($"Filter field not found: {FieldName}");

        fFilterFieldName = FieldName;
        fFilterValue = Value;
        RebuildRows();
        OnPropertyChanged(nameof(FilterFieldName));
        OnPropertyChanged(nameof(FilterValue));
        OnPropertyChanged(nameof(IsFiltered));
    }
    /// <summary>
    /// Clears the field/value filter.
    /// </summary>
    public void CancelFilter()
    {
        fFilterFieldName = null;
        fFilterValue = null;
        RebuildRows();
        OnPropertyChanged(nameof(FilterFieldName));
        OnPropertyChanged(nameof(FilterValue));
        OnPropertyChanged(nameof(IsFiltered));
    }
    /// <summary>
    /// Sets whether this DataSource propagates current-row changes to its details.
    /// </summary>
    public void ActivateDetails(bool Value)
    {
        ActivateDetails(Value, true);
    }
    /// <summary>
    /// Sets whether this DataSource propagates current-row changes to its details.
    /// </summary>
    public void ActivateDetails(bool Value, bool Propagate)
    {
        if (DetailsActive != Value)
        {
            DetailsActive = Value;

            if (DetailsActive)
            {
                foreach (DataSource Detail in fDetails)
                    Detail.RebuildRows();
            }

            if (Propagate)
            {
                foreach (DataSource Detail in fDetails)
                    Detail.ActivateDetails(Value, Propagate);
            }

            OnPropertyChanged(nameof(DetailsActive));
        }
    }
    /// <summary>
    /// Returns the DataSource name.
    /// </summary>
    public override string ToString()
    {
        return Name;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the DataSource name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the owner context.
    /// </summary>
    public object Owner { get; set; }
    /// <summary>
    /// Gets the provider.
    /// </summary>
    public IDataProvider Provider => fProvider;
    /// <summary>
    /// Gets the visible rows.
    /// </summary>
    public ObservableCollection<DataSourceRow> Rows => fRows;
    /// <summary>
    /// Gets all loaded rows.
    /// </summary>
    public IReadOnlyList<DataSourceRow> AllRows => fAllRows;
    /// <summary>
    /// Gets the master DataSource.
    /// </summary>
    public DataSource Master => fMaster;
    /// <summary>
    /// Gets the master relation.
    /// </summary>
    public DataSourceRelation MasterRelation => fMasterRelation;
    /// <summary>
    /// Gets the detail DataSources.
    /// </summary>
    public IReadOnlyList<DataSource> Details => fDetails;
    /// <summary>
    /// Gets the detail relations.
    /// </summary>
    public IReadOnlyList<DataSourceRelation> Relations => fRelations;
    /// <summary>
    /// Gets the visible row count.
    /// </summary>
    public int Count => fRows.Count;
    /// <summary>
    /// Gets or sets the current row position.
    /// </summary>
    public int Position
    {
        get => fPosition;
        set
        {
            int NewValue = value;

            if (NewValue < -1)
                NewValue = -1;
            if (NewValue >= fRows.Count)
                NewValue = fRows.Count - 1;

            int OldPosition = fPosition;
            DataSourceRow OldCurrent = fCurrent;
            DataSourceRow NewCurrent = NewValue >= 0 && NewValue < fRows.Count ? fRows[NewValue] : null;

            if (fPosition == NewValue)
            {
                if (!ReferenceEquals(fCurrent, NewCurrent))
                    SetCurrentByPosition();
                return;
            }

            if (!RaisePositionChanging(OldPosition, NewValue, OldCurrent, NewCurrent))
                return;

            fPosition = NewValue;
            SetCurrentByPosition();
            OnPropertyChanged(nameof(Position));
            RaisePositionChanged(OldPosition, fPosition, OldCurrent, fCurrent);
        }
    }
    /// <summary>
    /// Gets or sets the current row.
    /// </summary>
    public DataSourceRow Current
    {
        get => fCurrent;
        set
        {
            if (ReferenceEquals(fCurrent, value))
                return;

            fCurrent = value;
            fPosition = fCurrent != null ? fRows.IndexOf(fCurrent) : -1;
            OnPropertyChanged(nameof(Current));
            OnPropertyChanged(nameof(Position));
        }
    }
    /// <summary>
    /// Gets a value indicating whether the current position is at the first row.
    /// </summary>
    public bool IsBof => Count == 0 || Position <= 0;
    /// <summary>
    /// Gets a value indicating whether the current position is at the last row.
    /// </summary>
    public bool IsEof => Count == 0 || Position >= Count - 1;
    /// <summary>
    /// Gets a value indicating whether there are no visible rows.
    /// </summary>
    public bool IsEmpty => Count == 0;
    /// <summary>
    /// Gets a value indicating whether there are visible rows.
    /// </summary>
    public bool HasRows => Count > 0;
    /// <summary>
    /// Gets or sets the cascade delete rule.
    /// </summary>
    public CascadeDeleteRule CascadeDeleteRule { get; set; } = CascadeDeleteRule.Restrict;
    /// <summary>
    /// Gets a value indicating whether current-row changes are propagated to details.
    /// </summary>
    public bool DetailsActive { get; private set; }
    /// <summary>
    /// Gets the filter field name.
    /// </summary>
    public string FilterFieldName => fFilterFieldName;
    /// <summary>
    /// Gets the filter value.
    /// </summary>
    public object FilterValue => fFilterValue;
    /// <summary>
    /// Gets a value indicating whether a filter is active.
    /// </summary>
    public bool IsFiltered => !string.IsNullOrWhiteSpace(fFilterFieldName) && fFilterValue != null;

    // ● events
    /// <summary>
    /// Occurs when a DataSource property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    /// <summary>
    /// Occurs before the current position changes.
    /// </summary>
    public event EventHandler<DataSourcePositionCancelEventArgs> PositionChanging;
    /// <summary>
    /// Occurs after the current position changes.
    /// </summary>
    public event EventHandler<DataSourcePositionEventArgs> PositionChanged;
    /// <summary>
    /// Occurs before a field value changes.
    /// </summary>
    public event EventHandler<DataSourceChangeEventArgs> Changing;
    /// <summary>
    /// Occurs after a field value changes.
    /// </summary>
    public event EventHandler<DataSourceChangeEventArgs> Changed;
    /// <summary>
    /// Occurs before an underlying item is created.
    /// </summary>
    public event EventHandler<DataSourceCreateEventArgs> Creating;
    /// <summary>
    /// Occurs after an underlying item is created.
    /// </summary>
    public event EventHandler<DataSourceInnerObjectEventArgs> Created;
    /// <summary>
    /// Occurs before a row is added.
    /// </summary>
    public event EventHandler<DataSourceRowCancelEventArgs> Adding;
    /// <summary>
    /// Occurs after a row is added.
    /// </summary>
    public event EventHandler<DataSourceRowEventArgs> Added;
    /// <summary>
    /// Occurs before a row is deleted.
    /// </summary>
    public event EventHandler<DataSourceRowCancelEventArgs> Deleting;
    /// <summary>
    /// Occurs after a row is deleted.
    /// </summary>
    public event EventHandler<DataSourceRowEventArgs> Deleted;
    /// <summary>
    /// Occurs before rows are cleared.
    /// </summary>
    public event EventHandler<DataSourceCancelEventArgs> Clearing;
    /// <summary>
    /// Occurs after rows are cleared.
    /// </summary>
    public event EventHandler Cleared;
    /// <summary>
    /// Occurs before rows are loaded.
    /// </summary>
    public event EventHandler<DataSourceCancelEventArgs> Loading;
    /// <summary>
    /// Occurs after rows are loaded.
    /// </summary>
    public event EventHandler Loaded;
}
