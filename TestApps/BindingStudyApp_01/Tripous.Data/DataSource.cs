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
    DataRelation fMasterRelation;
    List<DataRelation> fRelations;

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
    }
    void Parent_PropertyChanged(object Sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Current))
            RefreshDetailRows();
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
    bool RowMatchesRelation(DataRelation Relation, DataSourceRow ParentRow, DataSourceRow ChildRow)
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
        foreach (DataRelation Relation in fRelations)
        {
            foreach (DataSourceRow ChildRow in Relation.Child.AllRows)
            {
                if (RowMatchesRelation(Relation, ParentRow, ChildRow))
                    return true;
            }
        }

        return false;
    }
    void DeleteDetailRows(DataSourceRow ParentRow)
    {
        foreach (DataRelation Relation in fRelations)
        {
            List<DataSourceRow> Rows = Relation.Child.AllRows
                .Where(ChildRow => RowMatchesRelation(Relation, ParentRow, ChildRow))
                .ToList();

            foreach (DataSourceRow Row in Rows)
                Relation.Child.DeleteRow(Row);
        }
    }
    void RefreshDetailRows()
    {
        DataSourceRow OldCurrent = Current;

        fRows.Clear();

        foreach (DataSourceRow Row in fAllRows)
        {
            if (PassesMasterFilter(Row))
                fRows.Add(Row);
        }

        if (OldCurrent != null && fRows.Contains(OldCurrent))
            Current = OldCurrent;
        else
            Position = fRows.Count > 0 ? 0 : -1;

        OnPropertyChanged(nameof(Count));
    }
    void SetMaster(DataSource Master, DataRelation Relation)
    {
        ClearMaster();
        fMaster = Master;
        fMasterRelation = Relation;
        fMaster.PropertyChanged += Parent_PropertyChanged;
        RefreshDetailRows();
    }
    void ClearMaster()
    {
        if (fMaster != null)
            fMaster.PropertyChanged -= Parent_PropertyChanged;

        fMaster = null;
        fMasterRelation = null;
        RefreshDetailRows();
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
        Load();
    }

    // ● static public
    /// <summary>
    /// Creates a DataSource from a DataTable.
    /// </summary>
    static public DataSource FromTable(DataTable Table)
    {
        return new DataSource(new DataTableProvider(Table));
    }
    /// <summary>
    /// Creates a DataSource from a DataView.
    /// </summary>
    static public DataSource FromDataView(DataView View)
    {
        return new DataSource(new DataViewProvider(View));
    }
    /// <summary>
    /// Creates a DataSource from a list of notifying objects.
    /// </summary>
    static public DataSource FromList<T>(IList<T> List) where T: class, INotifyPropertyChanged, new()
    {
        return new DataSource(new ListProvider<T>(List));
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

        RefreshDetailRows();
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
    public DataRelation AddDetail(string Name, DataSource Child, string ParentFieldName, string ChildFieldName)
    {
        return AddDetail(Name, Child, new[] { ParentFieldName }, new[] { ChildFieldName });
    }
    /// <summary>
    /// Adds a detail relation using parent and child field arrays.
    /// </summary>
    public DataRelation AddDetail(string Name, DataSource Child, string[] ParentFieldNames, string[] ChildFieldNames)
    {
        DataRelation Relation = new(Name, this, Child, ParentFieldNames, ChildFieldNames);

        fRelations.Add(Relation);
        Child.SetMaster(this, Relation);
        return Relation;
    }
    /// <summary>
    /// Removes a detail relation.
    /// </summary>
    public void RemoveDetail(DataRelation Relation)
    {
        if (Relation == null)
            return;

        if (fRelations.Remove(Relation))
            Relation.Child.ClearMaster();
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

        if (PassesMasterFilter(Row))
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

    // ● properties
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
    /// Gets the detail relations.
    /// </summary>
    public IReadOnlyList<DataRelation> Relations => fRelations;
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
