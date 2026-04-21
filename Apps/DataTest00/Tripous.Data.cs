using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Tripous.Data;

public enum EditStatus
{
    Browse,
    Edit,
    Insert,
}

public enum RowStatus
{
    Detached,
    Unchanged,
    Added,
    Modified,
    Deleted,
}

public enum RowViewKind
{
    Row,
    Group,
    Summary,
}

public class Row: INotifyPropertyChanged
{
    // ● private fields
    RowSet fRowSet;
    object fDataItem;
    EditStatus fEditStatus;
    RowStatus fRowStatus;
    Dictionary<string, object> fOriginalValues;
    Dictionary<string, object> fEditValues;

    // ● private methods
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    void NotifyValueChanged(Column Column, object OldValue, object NewValue)
    {
        ValueChanged?.Invoke(this, new RowValueChangedEventArgs(this, Column, OldValue, NewValue));

        if (Column != null)
            NotifyPropertyChanged(Column.Name);

        NotifyPropertyChanged(nameof(RowStatus));
    }
    Dictionary<string, object> CaptureValues()
    {
        Dictionary<string, object> Result = new(StringComparer.OrdinalIgnoreCase);

        foreach (Column Column in RowSet.Columns)
            Result[Column.Name] = RowSet.Adapter.GetValue(DataItem, Column);

        return Result;
    }
    void RestoreValues(Dictionary<string, object> Values)
    {
        if (Values == null)
            return;

        foreach (Column Column in RowSet.Columns)
        {
            if (Values.TryGetValue(Column.Name, out object Value))
                RowSet.Adapter.SetValue(DataItem, Column, Value);
        }
    }
    bool IsEqualToOriginal()
    {
        if (fOriginalValues == null || fOriginalValues.Count == 0)
            return false;

        foreach (Column Column in RowSet.Columns)
        {
            object CurrentValue = RowSet.Adapter.GetValue(DataItem, Column);
            object OriginalValue = fOriginalValues[Column.Name];

            if (!Equals(CurrentValue, OriginalValue))
                return false;
        }

        return true;
    }
    void UpdateRowStatusAfterValueChange()
    {
        if (RowStatus == RowStatus.Detached)
            return;

        if (RowStatus == RowStatus.Added)
            return;

        if (RowStatus == RowStatus.Deleted)
            return;

        RowStatus = IsEqualToOriginal() ? RowStatus.Unchanged : RowStatus.Modified;
    }

    // ● constructors
    public Row(RowSet RowSet, object DataItem)
    {
        fRowSet = RowSet ?? throw new ArgumentNullException(nameof(RowSet));
        fDataItem = DataItem;
        fEditStatus = EditStatus.Browse;
        fRowStatus = RowStatus.Detached;
    }

    // ● public methods
    public object GetValue(Column Column)
    {
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        return RowSet.Adapter.GetValue(DataItem, Column);
    }
    public object GetValue(string ColumnName)
    {
        Column Column = RowSet.Columns[ColumnName];
        if (Column == null)
            throw new ArgumentException($"Column not found: {ColumnName}", nameof(ColumnName));

        return GetValue(Column);
    }
    public void SetValue(Column Column, object Value)
    {
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        if (RowStatus == RowStatus.Deleted)
            throw new InvalidOperationException("Cannot set value on a deleted row.");

        object OldValue = GetValue(Column);
        if (Equals(OldValue, Value))
            return;

        RowSet.Adapter.SetValue(DataItem, Column, Value);
        UpdateRowStatusAfterValueChange();
        NotifyValueChanged(Column, OldValue, Value);
    }
    public void SetValue(string ColumnName, object Value)
    {
        Column Column = RowSet.Columns[ColumnName];
        if (Column == null)
            throw new ArgumentException($"Column not found: {ColumnName}", nameof(ColumnName));

        SetValue(Column, Value);
    }

    public void SetOriginalValues()
    {
        fOriginalValues = CaptureValues();
    }
    public void ClearOriginalValues()
    {
        fOriginalValues = null;
    }

    public void BeginEdit()
    {
        if (EditStatus == EditStatus.Edit)
            return;

        if (RowStatus == RowStatus.Detached)
            return;

        if (RowStatus == RowStatus.Deleted)
            return;

        fEditValues = CaptureValues();
        fEditStatus = EditStatus.Edit;
        NotifyPropertyChanged(nameof(EditStatus));
    }
    public void CommitEdit()
    {
        if (EditStatus != EditStatus.Edit)
            return;

        fEditValues = null;
        fEditStatus = EditStatus.Browse;
        NotifyPropertyChanged(nameof(EditStatus));
    }
    public void CancelEdit()
    {
        if (EditStatus != EditStatus.Edit)
            return;

        RestoreValues(fEditValues);
        fEditValues = null;
        fEditStatus = EditStatus.Browse;

        if (RowStatus == RowStatus.Unchanged || RowStatus == RowStatus.Modified)
            RowStatus = IsEqualToOriginal() ? RowStatus.Unchanged : RowStatus.Modified;

        NotifyPropertyChanged(nameof(EditStatus));
        NotifyPropertyChanged(nameof(RowStatus));
    }

    // ● properties
    public RowSet RowSet => fRowSet;
    public object DataItem => fDataItem;
    public EditStatus EditStatus => fEditStatus;
    public RowStatus RowStatus
    {
        get => fRowStatus;
        internal set => fRowStatus = value;
    }
    public object this[string ColumnName]
    {
        get => GetValue(ColumnName);
        set => SetValue(ColumnName, value);
    }
    
    public bool IsDetached => RowStatus == RowStatus.Detached;
    public bool IsUnchanged => RowStatus == RowStatus.Unchanged;
    public bool IsAdded => RowStatus == RowStatus.Added;
    public bool IsModified => RowStatus == RowStatus.Modified;
    public bool IsDeleted => RowStatus == RowStatus.Deleted;

    public bool IsEditing => EditStatus == EditStatus.Edit;

    public bool CanBeginEdit =>
        !IsDetached &&
        !IsDeleted &&
        !IsEditing;

    public bool CanDelete =>
        !IsDetached &&
        !IsDeleted;

    public bool CanSetValue =>
        !IsDeleted;
    
    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<RowValueChangedEventArgs> ValueChanged;
}

public class Column: INotifyPropertyChanged
{
    // ● private fields
    string fTitle;
    bool fIsReadOnly;
    bool fIsVisible;
    double fWidth;
    double fMinWidth;
    string fFormat;
    string fDisplayFormat;
    string fEditFormat;
    string fAlignment;

    // ● private methods
    void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    static bool IsNullableType(Type Type)
    {
        return !Type.IsValueType || Nullable.GetUnderlyingType(Type) != null;
    }
    static Type GetActualDataType(Type Type)
    {
        return Nullable.GetUnderlyingType(Type) ?? Type;
    }

    // ● constructors
    public Column(string Name, Type DataType)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentNullException(nameof(Name));

        this.Name = Name;
        this.DataType = DataType ?? typeof(string);
        this.IsNullable = IsNullableType(this.DataType);

        fIsVisible = true;
        fWidth = -1;
        fMinWidth = -1;
    }
    public Column(string Name, Type DataType, string SourceName)
        : this(Name, DataType)
    {
        this.SourceName = string.IsNullOrWhiteSpace(SourceName) ? Name : SourceName;
    }

    // ● public methods
    public override string ToString()
    {
        return Name;
    }

    // ● properties
    public string Name { get; }
    public Type DataType { get; }
    public bool IsNullable { get; init; }
    public object DefaultValue { get; init; }
    public string SourceName { get; init; }
    public string LookupSourceName { get; init; }
    public string ValueMember { get; init; }
    public string DisplayMember { get; init; }

    public bool IsReadOnly
    {
        get => fIsReadOnly;
        set
        {
            if (fIsReadOnly != value)
            {
                fIsReadOnly = value;
                NotifyPropertyChanged(nameof(IsReadOnly));
            }
        }
    }
    public string Title
    {
        get => string.IsNullOrWhiteSpace(fTitle) ? Name : fTitle;
        set
        {
            if (fTitle != value)
            {
                fTitle = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }
    }
    public bool IsVisible
    {
        get => fIsVisible;
        set
        {
            if (fIsVisible != value)
            {
                fIsVisible = value;
                NotifyPropertyChanged(nameof(IsVisible));
            }
        }
    }
    public double Width
    {
        get => fWidth;
        set
        {
            if (fWidth != value)
            {
                fWidth = value;
                NotifyPropertyChanged(nameof(Width));
            }
        }
    }
    public double MinWidth
    {
        get => fMinWidth;
        set
        {
            if (fMinWidth != value)
            {
                fMinWidth = value;
                NotifyPropertyChanged(nameof(MinWidth));
            }
        }
    }
    public string Format
    {
        get => fFormat;
        set
        {
            if (fFormat != value)
            {
                fFormat = value;
                NotifyPropertyChanged(nameof(Format));
            }
        }
    }
    public string DisplayFormat
    {
        get => fDisplayFormat;
        set
        {
            if (fDisplayFormat != value)
            {
                fDisplayFormat = value;
                NotifyPropertyChanged(nameof(DisplayFormat));
            }
        }
    }
    public string EditFormat
    {
        get => fEditFormat;
        set
        {
            if (fEditFormat != value)
            {
                fEditFormat = value;
                NotifyPropertyChanged(nameof(EditFormat));
            }
        }
    }
    public string Alignment
    {
        get => fAlignment;
        set
        {
            if (fAlignment != value)
            {
                fAlignment = value;
                NotifyPropertyChanged(nameof(Alignment));
            }
        }
    }

    public bool HasLookup => !string.IsNullOrWhiteSpace(LookupSourceName);
    public bool IsString => GetActualDataType(DataType) == typeof(string);
    public bool IsBoolean => GetActualDataType(DataType) == typeof(bool);
    public bool IsDateTime => GetActualDataType(DataType) == typeof(DateTime);
    public bool IsEnum => GetActualDataType(DataType).IsEnum;
    public bool IsNumeric
    {
        get
        {
            Type Type = GetActualDataType(DataType);

            return Type == typeof(byte)
                || Type == typeof(short)
                || Type == typeof(int)
                || Type == typeof(long)
                || Type == typeof(float)
                || Type == typeof(double)
                || Type == typeof(decimal);
        }
    }

    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}

public class ColumnCollection: IReadOnlyList<Column>
{
    // ● private fields
    readonly List<Column> fList = new();
    readonly Dictionary<string, Column> fMap = new(StringComparer.OrdinalIgnoreCase);
    // ● private methods
    void AddToMap(Column Item)
    {
        if (Item != null && !string.IsNullOrWhiteSpace(Item.Name))
            fMap[Item.Name] = Item;
    }
    // ● constructors
    public ColumnCollection()
    {
    }
    // ● public methods
    static public ColumnCollection Create(IEnumerable<Column> Items)
    {
        ColumnCollection Result = new();
        if (Items != null)
        {
            foreach (Column Item in Items)
                Result.Add(Item);
        }
        return Result;
    }
    public void Add(Column Item)
    {
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        fList.Add(Item);
        AddToMap(Item);
    }
    public bool Contains(string Name)
    {
        return !string.IsNullOrWhiteSpace(Name) && fMap.ContainsKey(Name);
    }
    public Column Find(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return null;
        fMap.TryGetValue(Name, out Column Result);
        return Result;
    }
    public Column this[string Name] => Find(Name);
    public Column this[int Index] => fList[Index];
    public IEnumerator<Column> GetEnumerator()
    {
        return fList.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    // ● properties
    public int Count => fList.Count;
}

public interface IRowSourceAdapter
{
    // ● public methods
    ColumnCollection CreateColumns(object DataSource);
    IEnumerable GetItems(object DataSource);
    object CreateItem(object DataSource);
    void AddItem(object DataSource, object DataItem);
    void DeleteItem(object DataSource, object DataItem);
    object GetValue(object DataItem, Column Column);
    void SetValue(object DataItem, Column Column, object Value);
    bool IsDeleted(object DataItem);
    bool CanRead(object DataSource);
    bool CanInsert(object DataSource);
    bool CanDelete(object DataSource);
    bool CanModify(object DataSource);
}

public class RowValueChangedEventArgs: EventArgs
{
    // ● constructors
    public RowValueChangedEventArgs(Row Row, Column Column, object OldValue, object NewValue)
    {
        this.Row = Row;
        this.Column = Column;
        this.OldValue = OldValue;
        this.NewValue = NewValue;
    }
    // ● properties
    public Row Row { get; }
    public Column Column { get; }
    public object OldValue { get; }
    public object NewValue { get; }
}

public class RowSetRowEventArgs: EventArgs
{
    // ● constructors
    public RowSetRowEventArgs(RowSet RowSet, Row Row)
    {
        this.RowSet = RowSet;
        this.Row = Row;
    }
    // ● properties
    public RowSet RowSet { get; }
    public Row Row { get; }
}

public class RowSetRowValueChangedEventArgs: EventArgs
{
    // ● constructors
    public RowSetRowValueChangedEventArgs(RowSet RowSet, Row Row, Column Column, object OldValue, object NewValue)
    {
        this.RowSet = RowSet;
        this.Row = Row;
        this.Column = Column;
        this.OldValue = OldValue;
        this.NewValue = NewValue;
    }
    // ● properties
    public RowSet RowSet { get; }
    public Row Row { get; }
    public Column Column { get; }
    public object OldValue { get; }
    public object NewValue { get; }
}

public class InitializingNewRowEventArgs: EventArgs
{
    // ● constructors
    public InitializingNewRowEventArgs(RowSet RowSet, Row Row)
    {
        this.RowSet = RowSet;
        this.Row = Row;
    }

    // ● properties
    public RowSet RowSet { get; }
    public Row Row { get; }
    public object DataItem => Row?.DataItem;
}

public class RowSet: IReadOnlyList<Row>
{
    // ● private fields
    readonly List<Row> fRows = new();
    readonly List<Relation> fRelations = new();
    object fDataSource;
    IRowSourceAdapter fAdapter;
    ColumnCollection fColumns;

    // ● private methods
    void HookRow(Row Row)
    {
        if (Row != null)
            Row.ValueChanged += Row_ValueChanged;
    }
    void UnhookRow(Row Row)
    {
        if (Row != null)
            Row.ValueChanged -= Row_ValueChanged;
    }
    void Row_ValueChanged(object Sender, RowValueChangedEventArgs e)
    {
        OnRowValueChanged(new RowSetRowValueChangedEventArgs(this, e.Row, e.Column, e.OldValue, e.NewValue));
    }
    void AddLoadedRow(object DataItem)
    {
        Row Row = new(this, DataItem);
        Row.SetOriginalValues();
        Row.RowStatus = RowStatus.Unchanged;
        fRows.Add(Row);
        HookRow(Row);
        OnRowAdded(new RowSetRowEventArgs(this, Row));
    }

    // ● protected methods
    protected virtual void OnInitializingNewRow(InitializingNewRowEventArgs e)
    {
        InitializingNewRow?.Invoke(this, e);
    }
    protected virtual void OnRowAdded(RowSetRowEventArgs e)
    {
        RowAdded?.Invoke(this, e);
    }
    protected virtual void OnRowDeleted(RowSetRowEventArgs e)
    {
        RowDeleted?.Invoke(this, e);
    }
    protected virtual void OnRowRemoved(RowSetRowEventArgs e)
    {
        RowRemoved?.Invoke(this, e);
    }
    protected virtual void OnRowValueChanged(RowSetRowValueChangedEventArgs e)
    {
        RowValueChanged?.Invoke(this, e);
    }

    // ● constructors
    public RowSet(object DataSource, IRowSourceAdapter Adapter)
    {
        fDataSource = DataSource;
        fAdapter = Adapter ?? throw new ArgumentNullException(nameof(Adapter));
        fColumns = Adapter.CreateColumns(DataSource) ?? new ColumnCollection();
        Load();
    }

    // ● static public methods
    static public RowSet Create(object DataSource, IRowSourceAdapter Adapter)
    {
        return new RowSet(DataSource, Adapter);
    }

    // ● public methods
    public void Load()
    {
        Clear();

        IEnumerable Items = Adapter.GetItems(DataSource);
        if (Items == null)
            return;

        foreach (object Item in Items)
            AddLoadedRow(Item);
    }
    public void Clear()
    {
        foreach (Row Row in fRows)
            UnhookRow(Row);

        fRows.Clear();
    }
    public Row NewRow()
    {
        object DataItem = Adapter.CreateItem(DataSource);
        Row Result = new(this, DataItem);
        OnInitializingNewRow(new InitializingNewRowEventArgs(this, Result));
        return Result;
    }
    public void AddRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        if (!ReferenceEquals(Row.RowSet, this))
            throw new ArgumentException("Row does not belong to this RowSet.", nameof(Row));

        if (fRows.Contains(Row))
            throw new InvalidOperationException("Row already belongs to this RowSet.");

        if (Row.RowStatus != RowStatus.Detached)
            throw new InvalidOperationException("Only detached rows can be added.");

        Adapter.AddItem(DataSource, Row.DataItem);
        Row.RowStatus = RowStatus.Added;
        fRows.Add(Row);
        HookRow(Row);
        OnRowAdded(new RowSetRowEventArgs(this, Row));
    }
    public void DeleteRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        if (!ReferenceEquals(Row.RowSet, this))
            throw new ArgumentException("Row does not belong to this RowSet.", nameof(Row));

        if (Row.RowStatus == RowStatus.Detached)
            return;

        if (Row.RowStatus == RowStatus.Deleted)
            return;

        Adapter.DeleteItem(DataSource, Row.DataItem);
        Row.RowStatus = RowStatus.Deleted;
        OnRowDeleted(new RowSetRowEventArgs(this, Row));
    }
    public void AddRelation(Relation Relation)
    {
        if (Relation == null)
            throw new ArgumentNullException(nameof(Relation));

        fRelations.Add(Relation);
    }
    public IEnumerator<Row> GetEnumerator()
    {
        return fRows.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Row FindByKey(RowSet rs, string ColumnName, object Value)
    {
        foreach (var r in rs)
        {
            object v = r[ColumnName];

            if (v == null && Value == null)
                return r;

            if (v != null && v.Equals(Value))
                return r;
        }

        return null;
    }
    
    // ● properties
    public object DataSource => fDataSource;
    public IRowSourceAdapter Adapter => fAdapter;
    public ColumnCollection Columns => fColumns;
    public IReadOnlyList<Relation> Relations => fRelations;
    public Row this[int Index] => fRows[Index];
    public int Count => fRows.Count;

    // ● events
    public event EventHandler<InitializingNewRowEventArgs> InitializingNewRow;
    public event EventHandler<RowSetRowEventArgs> RowAdded;
    public event EventHandler<RowSetRowEventArgs> RowDeleted;
    public event EventHandler<RowSetRowEventArgs> RowRemoved;
    public event EventHandler<RowSetRowValueChangedEventArgs> RowValueChanged;
}

public interface ILookupSource
{
    // ● properties
    string Name { get; }
    RowSet GetRowSet();
}

public class LookupRegistry
{
    // ● private fields
    readonly Dictionary<string, ILookupSource> fMap = new(StringComparer.OrdinalIgnoreCase);

    // ● constructors
    public LookupRegistry()
    {
    }

    // ● public methods
    public void Register(ILookupSource Source)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        fMap[Source.Name] = Source;
    }
    public void Register(string Name, RowSet RowSet)
    {
        Register(new RowSetLookupSource(Name, RowSet));
    }
    public bool Contains(string Name)
    {
        return !string.IsNullOrWhiteSpace(Name) && fMap.ContainsKey(Name);
    }
    public ILookupSource Find(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return null;

        fMap.TryGetValue(Name, out ILookupSource Result);
        return Result;
    }
    public RowSet ResolveRowSet(string Name)
    {
        return Find(Name)?.GetRowSet();
    }
    public LookupResolver CreateResolver(string SourceName, string ValueMember, string DisplayMember)
    {
        RowSet RowSet = ResolveRowSet(SourceName);
        if (RowSet == null)
            return null;

        return new LookupResolver(RowSet, ValueMember, DisplayMember);
    }
}

public class ListSourceAdapter<T>: IRowSourceAdapter where T : class, new()
{
    // ● private fields
    readonly Dictionary<string, PropertyInfo> fPropertyMap = new(StringComparer.OrdinalIgnoreCase);

    // ● private methods
    void EnsureProperties()
    {
        if (fPropertyMap.Count > 0)
            return;

        foreach (PropertyInfo Prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (Prop.CanRead)
                fPropertyMap[Prop.Name] = Prop;
        }
    }
    PropertyInfo FindProperty(Column Column)
    {
        EnsureProperties();

        if (Column == null)
            return null;

        string Name = !string.IsNullOrWhiteSpace(Column.SourceName) ? Column.SourceName : Column.Name;
        if (string.IsNullOrWhiteSpace(Name))
            return null;

        fPropertyMap.TryGetValue(Name, out PropertyInfo Result);
        return Result;
    }
    static object NormalizeValue(object Value, Type DestType)
    {
        if (DestType == null)
            return Value;

        Type ActualType = Nullable.GetUnderlyingType(DestType) ?? DestType;

        if (Value == null || Value == DBNull.Value)
        {
            if (Nullable.GetUnderlyingType(DestType) != null || !ActualType.IsValueType)
                return null;

            return Activator.CreateInstance(ActualType);
        }

        if (ActualType.IsEnum)
        {
            if (Value is string S)
                return Enum.Parse(ActualType, S, true);

            return Enum.ToObject(ActualType, Value);
        }

        if (ActualType == typeof(Guid))
        {
            if (Value is Guid)
                return Value;

            return Guid.Parse(Convert.ToString(Value));
        }

        if (ActualType.IsInstanceOfType(Value))
            return Value;

        return Convert.ChangeType(Value, ActualType);
    }

    // ● constructors
    public ListSourceAdapter()
    {
    }

    // ● public methods
    public ColumnCollection CreateColumns(object DataSource)
    {
        EnsureProperties();

        ColumnCollection Result = new();

        foreach (PropertyInfo Prop in fPropertyMap.Values)
        {
            Column Column = new(Prop.Name, Prop.PropertyType)
            {
                SourceName = Prop.Name,
                IsReadOnly = !Prop.CanWrite,
                IsNullable = !Prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(Prop.PropertyType) != null,
            };
            Result.Add(Column);
        }

        return Result;
    }
    public IEnumerable GetItems(object DataSource)
    {
        return DataSource as IEnumerable<T>;
    }
    public object CreateItem(object DataSource)
    {
        return new T();
    }
    public void AddItem(object DataSource, object DataItem)
    {
        if (DataSource is not IList<T> List)
            throw new InvalidOperationException("DataSource is not an IList<T>.");

        List.Add((T)DataItem);
    }
    public void DeleteItem(object DataSource, object DataItem)
    {
        if (DataSource is not IList<T> List)
            throw new InvalidOperationException("DataSource is not an IList<T>.");

        List.Remove((T)DataItem);
    }
    public object GetValue(object DataItem, Column Column)
    {
        if (DataItem == null)
            throw new ArgumentNullException(nameof(DataItem));

        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        PropertyInfo Prop = FindProperty(Column);
        if (Prop == null)
            throw new ArgumentException($"Property not found: {Column.Name}", nameof(Column));

        return Prop.GetValue(DataItem);
    }
    public void SetValue(object DataItem, Column Column, object Value)
    {
        if (DataItem == null)
            throw new ArgumentNullException(nameof(DataItem));

        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        PropertyInfo Prop = FindProperty(Column);
        if (Prop == null)
            throw new ArgumentException($"Property not found: {Column.Name}", nameof(Column));

        if (!Prop.CanWrite)
            throw new InvalidOperationException($"Property is read-only: {Prop.Name}");

        object ConvertedValue = NormalizeValue(Value, Prop.PropertyType);
        Prop.SetValue(DataItem, ConvertedValue);
    }
    public bool IsDeleted(object DataItem)
    {
        return false;
    }
    public bool CanRead(object DataSource)
    {
        return DataSource is IEnumerable<T>;
    }
    public bool CanInsert(object DataSource)
    {
        return DataSource is IList<T>;
    }
    public bool CanDelete(object DataSource)
    {
        return DataSource is IList<T>;
    }
    public bool CanModify(object DataSource)
    {
        return DataSource is IEnumerable<T>;
    }
}

public class DataTableSourceAdapter: IRowSourceAdapter
{
    // ● private methods
    static DataTable GetTable(object DataSource)
    {
        if (DataSource is not DataTable Table)
            throw new InvalidOperationException("DataSource is not a DataTable.");

        return Table;
    }
    static DataRow GetRow(object DataItem)
    {
        if (DataItem is not DataRow Row)
            throw new InvalidOperationException("DataItem is not a DataRow.");

        return Row;
    }
    static DataColumn FindColumn(DataTable Table, Column Column)
    {
        string Name = !string.IsNullOrWhiteSpace(Column.SourceName) ? Column.SourceName : Column.Name;
        if (string.IsNullOrWhiteSpace(Name))
            return null;

        return Table.Columns.Contains(Name) ? Table.Columns[Name] : null;
    }
    static object NormalizeValue(object Value)
    {
        return Value ?? DBNull.Value;
    }

    // ● constructors
    public DataTableSourceAdapter()
    {
    }

    // ● public methods
    public ColumnCollection CreateColumns(object DataSource)
    {
        DataTable Table = GetTable(DataSource);
        ColumnCollection Result = new();

        foreach (DataColumn Col in Table.Columns)
        {
            Column Item = new(Col.ColumnName, Col.DataType)
            {
                SourceName = Col.ColumnName,
                IsNullable = Col.AllowDBNull,
                IsReadOnly = Col.ReadOnly || Col.AutoIncrement,
                DefaultValue = Col.DefaultValue == DBNull.Value ? null : Col.DefaultValue,
            };
            Result.Add(Item);
        }

        return Result;
    }
    public IEnumerable GetItems(object DataSource)
    {
        DataTable Table = GetTable(DataSource);

        foreach (DataRow Row in Table.Rows)
        {
            if (Row.RowState != DataRowState.Deleted)
                yield return Row;
        }
    }
    public object CreateItem(object DataSource)
    {
        DataTable Table = GetTable(DataSource);
        return Table.NewRow();
    }
    public void AddItem(object DataSource, object DataItem)
    {
        DataTable Table = GetTable(DataSource);
        DataRow Row = GetRow(DataItem);

        if (!ReferenceEquals(Row.Table, Table))
            throw new InvalidOperationException("The DataRow does not belong to this DataTable.");

        if (Row.RowState == DataRowState.Detached)
            Table.Rows.Add(Row);
    }
    public void DeleteItem(object DataSource, object DataItem)
    {
        DataRow Row = GetRow(DataItem);

        if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            Row.Delete();
    }
    public object GetValue(object DataItem, Column Column)
    {
        DataRow Row = GetRow(DataItem);
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        DataColumn DataColumn = FindColumn(Row.Table, Column);
        if (DataColumn == null)
            throw new ArgumentException($"Column not found: {Column.Name}", nameof(Column));

        if (Row.RowState == DataRowState.Deleted)
            return Row[DataColumn, DataRowVersion.Original] is DBNull ? null : Row[DataColumn, DataRowVersion.Original];

        object Value = Row[DataColumn];
        return Value == DBNull.Value ? null : Value;
    }
    public void SetValue(object DataItem, Column Column, object Value)
    {
        DataRow Row = GetRow(DataItem);
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        DataColumn DataColumn = FindColumn(Row.Table, Column);
        if (DataColumn == null)
            throw new ArgumentException($"Column not found: {Column.Name}", nameof(Column));

        if (DataColumn.ReadOnly || DataColumn.AutoIncrement)
            throw new InvalidOperationException($"Column is read-only: {DataColumn.ColumnName}");

        if (Row.RowState == DataRowState.Deleted)
            throw new InvalidOperationException("Cannot set value on a deleted DataRow.");

        Row[DataColumn] = NormalizeValue(Value);
    }
    public bool IsDeleted(object DataItem)
    {
        DataRow Row = GetRow(DataItem);
        return Row.RowState == DataRowState.Deleted;
    }
    public bool CanRead(object DataSource)
    {
        return DataSource is DataTable;
    }
    public bool CanInsert(object DataSource)
    {
        return DataSource is DataTable;
    }
    public bool CanDelete(object DataSource)
    {
        return DataSource is DataTable;
    }
    public bool CanModify(object DataSource)
    {
        return DataSource is DataTable;
    }
}

public class LookupResolver
{
    // ● private fields
    RowSet fRowSet;
    Column fValueColumn;
    Column fDisplayColumn;
    Dictionary<object, Row> fMap;

    // ● private methods
    static Type GetActualType(Type Type)
    {
        return Nullable.GetUnderlyingType(Type) ?? Type;
    }
    static object NormalizeValue(object Value, Type Type)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        Type ActualType = GetActualType(Type);

        if (ActualType == typeof(string))
            return Convert.ToString(Value);

        if (ActualType.IsEnum)
        {
            if (Value is string S)
                return Enum.Parse(ActualType, S, true);

            return Enum.ToObject(ActualType, Value);
        }

        if (ActualType == typeof(Guid))
        {
            if (Value is Guid)
                return Value;

            return Guid.Parse(Convert.ToString(Value));
        }

        if (ActualType == typeof(bool))
        {
            if (Value is string S)
                return bool.Parse(S);

            return Convert.ToBoolean(Value);
        }

        if (ActualType == typeof(DateTime))
        {
            if (Value is DateTime)
                return Value;

            return Convert.ToDateTime(Value);
        }

        if (ActualType.IsInstanceOfType(Value))
            return Value;

        return Convert.ChangeType(Value, ActualType);
    }
    object NormalizeKey(object Value)
    {
        return NormalizeValue(Value, ValueColumn.DataType);
    }
    void BuildMap()
    {
        fMap = new();

        foreach (Row Row in RowSet)
        {
            object Key = NormalizeKey(Row.GetValue(ValueColumn));
            if (Key != null && !fMap.ContainsKey(Key))
                fMap[Key] = Row;
        }
    }

    // ● constructors
    public LookupResolver(RowSet RowSet, string ValueMember, string DisplayMember)
    {
        fRowSet = RowSet ?? throw new ArgumentNullException(nameof(RowSet));
        fValueColumn = RowSet.Columns[ValueMember] ?? throw new ArgumentException($"Column not found: {ValueMember}", nameof(ValueMember));
        fDisplayColumn = RowSet.Columns[DisplayMember] ?? throw new ArgumentException($"Column not found: {DisplayMember}", nameof(DisplayMember));
        BuildMap();
    }
    public LookupResolver(RowSet RowSet, Column ValueColumn, Column DisplayColumn)
    {
        fRowSet = RowSet ?? throw new ArgumentNullException(nameof(RowSet));
        fValueColumn = ValueColumn ?? throw new ArgumentNullException(nameof(ValueColumn));
        fDisplayColumn = DisplayColumn ?? throw new ArgumentNullException(nameof(DisplayColumn));
        BuildMap();
    }

    // ● public methods
    public void Rebuild()
    {
        BuildMap();
    }
    public bool Contains(object Value)
    {
        object Key = NormalizeKey(Value);
        return Key != null && fMap.ContainsKey(Key);
    }
    public Row GetRow(object Value)
    {
        object Key = NormalizeKey(Value);
        if (Key == null)
            return null;

        fMap.TryGetValue(Key, out Row Result);
        return Result;
    }
    public object GetDisplayValue(object Value)
    {
        Row Row = GetRow(Value);
        return Row?.GetValue(DisplayColumn);
    }
    public string GetDisplayText(object Value)
    {
        object Result = GetDisplayValue(Value);
        return Result == null || Result == DBNull.Value ? string.Empty : Convert.ToString(Result);
    }

    // ● properties
    public RowSet RowSet => fRowSet;
    public Column ValueColumn => fValueColumn;
    public Column DisplayColumn => fDisplayColumn;
}

public class RowSetLookupSource: ILookupSource
{
    // ● private fields
    RowSet fRowSet;

    // ● constructors
    public RowSetLookupSource(string Name, RowSet RowSet)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentNullException(nameof(Name));

        fRowSet = RowSet ?? throw new ArgumentNullException(nameof(RowSet));
        this.Name = Name;
    }

    // ● public methods
    public RowSet GetRowSet()
    {
        return fRowSet;
    }

    // ● properties
    public string Name { get; }
}

public class Relation
{
    // ● constructors
    public Relation(string Name, RowSet MasterRowSet, RowSet DetailRowSet, IEnumerable<Column> MasterColumns, IEnumerable<Column> DetailColumns)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentNullException(nameof(Name));

        this.Name = Name;
        this.MasterRowSet = MasterRowSet ?? throw new ArgumentNullException(nameof(MasterRowSet));
        this.DetailRowSet = DetailRowSet ?? throw new ArgumentNullException(nameof(DetailRowSet));

        this.MasterColumns = new List<Column>(MasterColumns ?? throw new ArgumentNullException(nameof(MasterColumns)));
        this.DetailColumns = new List<Column>(DetailColumns ?? throw new ArgumentNullException(nameof(DetailColumns)));

        if (this.MasterColumns.Count != this.DetailColumns.Count)
            throw new ArgumentException("MasterColumns and DetailColumns count mismatch.");
    }

    // ● properties
    public string Name { get; }
    public RowSet MasterRowSet { get; }
    public RowSet DetailRowSet { get; }
    public List<Column> MasterColumns { get; }
    public List<Column> DetailColumns { get; }
}

public class RelationContext
{
    // ● private fields
    Relation fRelation;
    Row fCurrentMasterRow;
    RowSetView fMasterView;

    // ● private methods
    void SubscribeMasterView(RowSetView Value)
    {
        if (Value != null)
            Value.CurrentRowViewChanged += MasterView_CurrentRowViewChanged;
    }
    void UnsubscribeMasterView(RowSetView Value)
    {
        if (Value != null)
            Value.CurrentRowViewChanged -= MasterView_CurrentRowViewChanged;
    }
    void CheckMasterView(RowSetView Value)
    {
        if (Value == null)
            return;

        if (!ReferenceEquals(Value.RowSet, Relation.MasterRowSet))
            throw new ArgumentException("MasterView does not belong to Relation.MasterRowSet.", nameof(Value));
    }
    void MasterView_CurrentRowViewChanged(object Sender, EventArgs e)
    {
        CurrentMasterRow = MasterView?.CurrentRow;
    }

    // ● protected methods
    protected virtual void OnCurrentMasterRowChanged(EventArgs e)
    {
        CurrentMasterRowChanged?.Invoke(this, e);
    }
    protected virtual void OnMasterViewChanged(EventArgs e)
    {
        MasterViewChanged?.Invoke(this, e);
    }

    // ● constructors
    public RelationContext(Relation Relation)
    {
        fRelation = Relation ?? throw new ArgumentNullException(nameof(Relation));
    }

    // ● public methods
    public bool Accept(Row DetailRow)
    {
        if (CurrentMasterRow == null)
            return false;

        if (DetailRow == null)
            return false;

        if (!ReferenceEquals(DetailRow.RowSet, Relation.DetailRowSet))
            return false;

        int Count = Relation.MasterColumns.Count;
        for (int i = 0; i < Count; i++)
        {
            object MasterValue = CurrentMasterRow[Relation.MasterColumns[i].Name];
            object DetailValue = DetailRow[Relation.DetailColumns[i].Name];

            if (!Equals(MasterValue, DetailValue))
                return false;
        }

        return true;
    }

    // ● properties
    public Relation Relation => fRelation;
    public Row CurrentMasterRow
    {
        get => fCurrentMasterRow;
        set
        {
            if (!ReferenceEquals(fCurrentMasterRow, value))
            {
                fCurrentMasterRow = value;
                OnCurrentMasterRowChanged(EventArgs.Empty);
            }
        }
    }
    public RowSetView MasterView
    {
        get => fMasterView;
        set
        {
            if (!ReferenceEquals(fMasterView, value))
            {
                CheckMasterView(value);
                UnsubscribeMasterView(fMasterView);
                fMasterView = value;
                SubscribeMasterView(fMasterView);
                CurrentMasterRow = fMasterView?.CurrentRow;
                OnMasterViewChanged(EventArgs.Empty);
            }
        }
    }

    // ● events
    public event EventHandler CurrentMasterRowChanged;
    public event EventHandler MasterViewChanged;
}

public class RowView
{
    // ● private fields
    readonly List<RowView> fChildren = new();
    RowView fParent;
    RowViewKind fKind;
    Row fRow;
    int fLevel;
    object fKey;
    
    // ● constructors
    public RowView()
    {
    }
    
    // ● public methods
    public void AddChild(RowView Item)
    {
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        Item.fParent = this;
        Item.fLevel = this.Level + 1;
        fChildren.Add(Item);
    }
    public object GetValue(Column Column)
    {
        if (Kind == RowViewKind.Row && Row != null)
            return Row.GetValue(Column);
        return null;
    }
    
    // ● properties
    public RowView Parent => fParent;
    public IReadOnlyList<RowView> Children => fChildren;
    public RowViewKind Kind
    {
        get => fKind;
        set => fKind = value;
    }
    public Row Row
    {
        get => fRow;
        set => fRow = value;
    }
    public int Level
    {
        get => fLevel;
        set => fLevel = value;
    }
    public object Key
    {
        get => fKey;
        set => fKey = value;
    }
}

public class RowSetView: IReadOnlyList<RowView>
{
    // ● private fields
    readonly List<RowView> fRows = new();
    RowSet fRowSet;
    RelationContext fRelationContext;
    RowView fCurrentRowView;
    Func<Row, bool> fFilter;
    Comparison<Row> fSort;

    // ● private methods
    bool AcceptRow(Row Row)
    {
        if (Row == null)
            return false;

        if (Row.IsDeleted)
            return false;

        if (RelationContext != null && !RelationContext.Accept(Row))
            return false;

        if (Filter != null && !Filter(Row))
            return false;

        return true;
    }
    RowView CreateRowView(Row Row)
    {
        return new RowView()
        {
            Kind = RowViewKind.Row,
            Row = Row,
            Level = 0,
        };
    }
    int IndexOf(RowView RowView)
    {
        return RowView == null ? -1 : fRows.IndexOf(RowView);
    }
    int IndexOf(Row Row)
    {
        if (Row == null)
            return -1;

        for (int i = 0; i < fRows.Count; i++)
        {
            if (ReferenceEquals(fRows[i].Row, Row))
                return i;
        }

        return -1;
    }
    bool SetCurrentRowView(RowView Value)
    {
        if (Value != null && IndexOf(Value) < 0)
            throw new ArgumentException("RowView does not belong to this RowSetView.", nameof(Value));

        if (ReferenceEquals(fCurrentRowView, Value))
            return false;

        fCurrentRowView = Value;
        OnCurrentRowViewChanged(EventArgs.Empty);
        return true;
    }
    bool MoveToIndex(int Index)
    {
        if (fRows.Count == 0)
        {
            SetCurrentRowView(null);
            return false;
        }

        if (Index < 0 || Index >= fRows.Count)
            return false;

        SetCurrentRowView(fRows[Index]);
        return true;
    }
    void SubscribeRelationContext(RelationContext Value)
    {
        if (Value != null)
            Value.CurrentMasterRowChanged += RelationContext_CurrentMasterRowChanged;
    }
    void UnsubscribeRelationContext(RelationContext Value)
    {
        if (Value != null)
            Value.CurrentMasterRowChanged -= RelationContext_CurrentMasterRowChanged;
    }
    void CheckRelationContext(RelationContext Value)
    {
        if (Value == null)
            return;

        if (!ReferenceEquals(Value.Relation.DetailRowSet, RowSet))
            throw new ArgumentException("RelationContext does not belong to this RowSetView RowSet.", nameof(Value));
    }
    void RelationContext_CurrentMasterRowChanged(object Sender, EventArgs e)
    {
        Rebuild();
    }

    // ● protected methods
    protected virtual void OnCurrentRowViewChanged(EventArgs e)
    {
        CurrentRowViewChanged?.Invoke(this, e);
    }
    protected virtual void OnRebuilt(EventArgs e)
    {
        Rebuilt?.Invoke(this, e);
    }

    // ● constructors
    public RowSetView(RowSet RowSet)
    {
        fRowSet = RowSet ?? throw new ArgumentNullException(nameof(RowSet));
        Rebuild();
    }

    // ● public methods
    public void Rebuild()
    {
        Row OldCurrentRow = CurrentRow;
        List<Row> List = new();

        foreach (Row Row in RowSet)
        {
            if (AcceptRow(Row))
                List.Add(Row);
        }

        if (Sort != null)
            List.Sort(Sort);

        fRows.Clear();

        foreach (Row Row in List)
            fRows.Add(CreateRowView(Row));

        if (OldCurrentRow != null)
        {
            int Index = IndexOf(OldCurrentRow);
            if (Index >= 0)
                fCurrentRowView = fRows[Index];
            else
                fCurrentRowView = fRows.Count > 0 ? fRows[0] : null;
        }
        else
        {
            fCurrentRowView = fRows.Count > 0 ? fRows[0] : null;
        }

        OnRebuilt(EventArgs.Empty);
        OnCurrentRowViewChanged(EventArgs.Empty);
    }
    public void Refresh()
    {
        Rebuild();
    }
    public Row NewRow()
    {
        return RowSet.NewRow();
    }
    public void AddRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        RowSet.AddRow(Row);
        Rebuild();
        MoveTo(Row);
    }
    public void DeleteRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        int CurrentIndex = IndexOf(Row);
        if (CurrentIndex < 0)
        {
            RowSet.DeleteRow(Row);
            Rebuild();
            return;
        }

        int TargetIndex = CurrentIndex < Count - 1 ? CurrentIndex : CurrentIndex - 1;

        RowSet.DeleteRow(Row);
        Rebuild();

        if (TargetIndex >= 0 && TargetIndex < Count)
            MoveToIndex(TargetIndex);
        else
            ClearCurrent();
    }
    public void Delete()
    {
        if (CurrentRow != null)
            DeleteRow(CurrentRow);
    }
    public void BeginEditRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        Row.BeginEdit();
    }
    public void CancelEditRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        Row.CancelEdit();
        Rebuild();
        MoveTo(Row);
    }
    public void CommitEditRow(Row Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));

        Row.CommitEdit();
        Rebuild();
        MoveTo(Row);
    }
    public void BeginEdit()
    {
        if (CurrentRow != null)
            BeginEditRow(CurrentRow);
    }
    public void CancelEdit()
    {
        if (CurrentRow != null)
            CancelEditRow(CurrentRow);
    }
    public void CommitEdit()
    {
        if (CurrentRow != null)
            CommitEditRow(CurrentRow);
    }
    public bool First()
    {
        return MoveToIndex(0);
    }
    public bool Last()
    {
        return MoveToIndex(Count - 1);
    }
    public bool Next()
    {
        if (Count == 0)
            return false;

        if (CurrentRowView == null)
            return First();

        int Index = IndexOf(CurrentRowView);
        return MoveToIndex(Index + 1);
    }
    public bool Previous()
    {
        if (Count == 0)
            return false;

        if (CurrentRowView == null)
            return Last();

        int Index = IndexOf(CurrentRowView);
        return MoveToIndex(Index - 1);
    }
    public bool MoveTo(Row Row)
    {
        int Index = IndexOf(Row);
        return Index >= 0 && MoveToIndex(Index);
    }
    public void ClearCurrent()
    {
        SetCurrentRowView(null);
    }
    public bool ContainsRow(Row Row)
    {
        return IndexOf(Row) >= 0;
    }
    public RowView FindRowView(Row Row)
    {
        int Index = IndexOf(Row);
        return Index >= 0 ? fRows[Index] : null;
    }
    public IEnumerator<RowView> GetEnumerator()
    {
        return fRows.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // ● properties
    public RowSet RowSet => fRowSet;
    public RelationContext RelationContext
    {
        get => fRelationContext;
        set
        {
            if (!ReferenceEquals(fRelationContext, value))
            {
                CheckRelationContext(value);
                UnsubscribeRelationContext(fRelationContext);
                fRelationContext = value;
                SubscribeRelationContext(fRelationContext);
                Rebuild();
            }
        }
    }
    public RowView CurrentRowView
    {
        get => fCurrentRowView;
        set => SetCurrentRowView(value);
    }
    public Row CurrentRow => CurrentRowView?.Row;
    public bool IsBof => CurrentRowView == null || ReferenceEquals(CurrentRowView, fRows.Count > 0 ? fRows[0] : null);
    public bool IsEof => CurrentRowView == null || ReferenceEquals(CurrentRowView, fRows.Count > 0 ? fRows[fRows.Count - 1] : null);
    public bool IsEmpty => fRows.Count == 0;
    public RowView this[int Index] => fRows[Index];
    public int Count => fRows.Count;
    public Func<Row, bool> Filter
    {
        get => fFilter;
        set
        {
            if (!ReferenceEquals(fFilter, value))
            {
                fFilter = value;
                Rebuild();
            }
        }
    }
    public Comparison<Row> Sort
    {
        get => fSort;
        set
        {
            if (!ReferenceEquals(fSort, value))
            {
                fSort = value;
                Rebuild();
            }
        }
    }
    
    // ● events
    public event EventHandler CurrentRowViewChanged;
    public event EventHandler Rebuilt;
}