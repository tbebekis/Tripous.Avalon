using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Tripous.Data;

/// <summary>
/// Represents the state of a DataSourceRow
/// </summary>
public enum DataSourceRowState
{
    Detached,
    Unchanged,
    Added,
    Modified,
    Deleted
}
/// <summary>
/// Minimal schema definition
/// </summary>
public class DataSourceColumn
{
    // ● private
    string fCaption;

    // ● constructor
    public DataSourceColumn(string Name, Type DataType, bool AllowsNull = true, bool ReadOnly = false, string Caption = null)
    {
        this.Name = !string.IsNullOrWhiteSpace(Name) ? Name : throw new ArgumentNullException(nameof(Name));
        this.DataType = DataType ?? typeof(object);
        this.AllowsNull = AllowsNull;
        this.ReadOnly = ReadOnly;
        fCaption = Caption;
    }

    // ● static public methods
    static public DataSourceColumn Create(string Name, Type DataType, bool AllowsNull = true, bool ReadOnly = false, string Caption = null)
    {
        return new DataSourceColumn(Name, DataType, AllowsNull, ReadOnly, Caption);
    }

    // ● public methods
    public override string ToString()
    {
        return Name;
    }

    // ● properties
    public string Name { get; }
    public string Caption => string.IsNullOrWhiteSpace(fCaption) ? Name : fCaption;
    public Type DataType { get; }
    public bool AllowsNull { get; }
    public bool ReadOnly { get; }
}
/// <summary>
/// Relation column mapping
/// </summary>
public class DataSourceRelationColumn
{
    // ● constructor
    public DataSourceRelationColumn()
    {
    }
    public DataSourceRelationColumn(string MasterColumnName, string DetailColumnName)
    {
        this.MasterColumnName = MasterColumnName;
        this.DetailColumnName = DetailColumnName;
    }

    // ● static public methods
    static public DataSourceRelationColumn Create(string MasterColumnName, string DetailColumnName)
    {
        return new DataSourceRelationColumn(MasterColumnName, DetailColumnName);
    }

    // ● properties
    public string MasterColumnName { get; set; }
    public string DetailColumnName { get; set; }
}
/// <summary>
/// Master-detail relation definition
/// </summary>
public class DataSourceRelation
{
    // ● constructor
    public DataSourceRelation(DataSource MasterSource, DataSource DetailSource)
    {
        this.MasterSource = MasterSource ?? throw new ArgumentNullException(nameof(MasterSource));
        this.DetailSource = DetailSource ?? throw new ArgumentNullException(nameof(DetailSource));
    }

    // ● static public methods
    static public DataSourceRelation Create(DataSource MasterSource, DataSource DetailSource)
    {
        return new DataSourceRelation(MasterSource, DetailSource);
    }

    // ● public methods
    public override string ToString()
    {
        return $"{MasterSource} -> {DetailSource}";
    }

    // ● properties
    public DataSource MasterSource { get; }
    public DataSource DetailSource { get; }
    public List<DataSourceRelationColumn> Columns { get; } = new();
}
/// <summary>
/// Row wrapper
/// </summary>
public class DataSourceRow
{
    // ● constructor
    internal DataSourceRow(DataSource Source, object SourceItem, DataSourceRowState State)
    {
        this.Source = Source ?? throw new ArgumentNullException(nameof(Source));
        this.SourceItem = SourceItem ?? throw new ArgumentNullException(nameof(SourceItem));
        this.State = State;
    }

    // ● public methods
    public T Get<T>(string ColumnName)
    {
        object Value = this[ColumnName];
        if (Value == null || Value == DBNull.Value)
            return default;
        if (Value is T TypedValue)
            return TypedValue;

        Type TargetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(Value, TargetType, CultureInfo.InvariantCulture);
    }
    public void Set<T>(string ColumnName, T Value)
    {
        this[ColumnName] = Value;
    }
    public bool IsNull(string ColumnName)
    {
        object Value = this[ColumnName];
        return Value == null || Value == DBNull.Value;
    }
    public override string ToString()
    {
        return $"{Source}[{State}]";
    }
 
    // ● public accessors
    /// <summary>
    /// Accesses the property as a generic object.
    /// </summary>
    public object AsObject(string ColumnName) => this[ColumnName];
    /// <summary>
    /// Sets the property value as a generic object.
    /// </summary>
    public void AsObject(string ColumnName, object value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a string.
    /// </summary>
    public string AsString(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToString(v) : null;
    }
    /// <summary>
    /// Sets the property value as a string.
    /// </summary>
    public void AsString(string ColumnName, string value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as an integer.
    /// </summary>
    public int AsInteger(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToInt32(v) : default;
    }
    /// <summary>
    /// Sets the property value as an integer.
    /// </summary>
    public void AsInteger(string ColumnName, int value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a nullable integer.
    /// </summary>
    public int? AsIntegerNullable(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToInt32(v) : (int?)null;
    }
    /// <summary>
    /// Sets the property value as a nullable integer.
    /// </summary>
    public void AsIntegerNullable(string ColumnName, int? value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a double.
    /// </summary>
    public double AsDouble(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToDouble(v) : default;
    }
    /// <summary>
    /// Sets the property value as a double.
    /// </summary>
    public void AsDouble(string ColumnName, double value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a nullable double.
    /// </summary>
    public double? AsDoubleNullable(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToDouble(v) : (double?)null;
    }
    /// <summary>
    /// Sets the property value as a nullable double.
    /// </summary>
    public void AsDoubleNullable(string ColumnName, double? value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a decimal.
    /// </summary>
    public decimal AsDecimal(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToDecimal(v) : default;
    }
    /// <summary>
    /// Sets the property value as a decimal.
    /// </summary>
    public void AsDecimal(string ColumnName, decimal value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a nullable decimal.
    /// </summary>
    public decimal? AsDecimalNullable(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToDecimal(v) : (decimal?)null;
    }
    /// <summary>
    /// Sets the property value as a nullable decimal.
    /// </summary>
    public void AsDecimalNullable(string ColumnName, decimal? value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a boolean.
    /// </summary>
    public bool AsBoolean(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToBoolean(v) : default;
    }
    /// <summary>
    /// Sets the property value as a boolean.
    /// </summary>
    public void AsBoolean(string ColumnName, bool value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a nullable boolean.
    /// </summary>
    public bool? AsBooleanNullable(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToBoolean(v) : (bool?)null;
    }
    /// <summary>
    /// Sets the property value as a nullable boolean.
    /// </summary>
    public void AsBooleanNullable(string ColumnName, bool? value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a DateTime.
    /// </summary>
    public DateTime AsDateTime(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToDateTime(v) : default;
    }
    /// <summary>
    /// Sets the property value as a DateTime.
    /// </summary>
    public void AsDateTime(string ColumnName, DateTime value) => this[ColumnName] = value;
    /// <summary>
    /// Accesses the property value as a nullable DateTime.
    /// </summary>
    public DateTime? AsDateTimeNullable(string ColumnName)
    {
        object v = this[ColumnName];
        return v != null && v != DBNull.Value ? Convert.ToDateTime(v) : (DateTime?)null;
    }
    /// <summary>
    /// Sets the property value as a nullable DateTime.
    /// </summary>
    public void AsDateTimeNullable(string ColumnName, DateTime? value) => this[ColumnName] = value;  
    
    // ● properties
    public DataSource Source { get; }
    public object SourceItem { get; }
    public DataSourceRowState State { get; internal set; }
    public bool IsDetached => State == DataSourceRowState.Detached;
    public object this[string ColumnName]
    {
        get => Source.GetValue(this, ColumnName);
        set => Source.SetValue(this, ColumnName, value);
    }
}
/// <summary>
/// Evaluates RowFilterDef / RowFilterDefs on DataSourceRow sequences
/// </summary>
static internal class DataSourceFilterEngine
{
    // ● private types
    enum StringOpType
    {
        Contains,
        StartsWith,
        EndsWith,
    }

    // ● private methods
    static private bool EvaluateRow(DataSourceRow Row, IList<RowFilterDef> Filters)
    {
        bool Result = true;
        bool First = true;

        foreach (RowFilterDef Filter in Filters)
        {
            bool Condition = EvaluateCondition(Row, Filter);

            if (First)
            {
                Result = Condition;
                First = false;
                continue;
            }

            switch (Filter.BoolOp)
            {
                case BoolOp.And:
                    Result = Result && Condition;
                    break;
                case BoolOp.Or:
                    Result = Result || Condition;
                    break;
                case BoolOp.AndNot:
                    Result = Result && !Condition;
                    break;
                case BoolOp.OrNot:
                    Result = Result || !Condition;
                    break;
                default:
                    Result = Condition;
                    break;
            }
        }

        return Result;
    }
    static private bool EvaluateCondition(DataSourceRow Row, RowFilterDef Filter)
    {
        object Value = Row[Filter.FieldName];
        bool IsNull = Value == null || Value == DBNull.Value;

        switch (Filter.ConditionOp)
        {
            case ConditionOp.Null:
                return IsNull;
            case ConditionOp.Equal:
                return Compare(Value, Filter.Value) == 0;
            case ConditionOp.NotEqual:
                return Compare(Value, Filter.Value) != 0;
            case ConditionOp.Greater:
                return Compare(Value, Filter.Value) > 0;
            case ConditionOp.GreaterOrEqual:
                return Compare(Value, Filter.Value) >= 0;
            case ConditionOp.Less:
                return Compare(Value, Filter.Value) < 0;
            case ConditionOp.LessOrEqual:
                return Compare(Value, Filter.Value) <= 0;
            case ConditionOp.Contains:
                return StringOp(Value, Filter.Value, StringOpType.Contains);
            case ConditionOp.StartsWith:
                return StringOp(Value, Filter.Value, StringOpType.StartsWith);
            case ConditionOp.EndsWith:
                return StringOp(Value, Filter.Value, StringOpType.EndsWith);
            case ConditionOp.Like:
                return StringOp(Value, Filter.Value, StringOpType.Contains);
            case ConditionOp.Between:
                return Compare(Value, Filter.Value) >= 0 && Compare(Value, Filter.Value2) <= 0;
            case ConditionOp.In:
                return InOp(Value, Filter.Value);
            default:
                return false;
        }
    }
    static private int Compare(object A, object B)
    {
        if (A == null || A == DBNull.Value)
            return (B == null || B == DBNull.Value) ? 0 : -1;
        if (B == null || B == DBNull.Value)
            return 1;
        if (A is string SA && B is string SB)
            return string.Compare(SA, SB, StringComparison.OrdinalIgnoreCase);

        try
        {
            Type TA = Nullable.GetUnderlyingType(A.GetType()) ?? A.GetType();
            Type TB = Nullable.GetUnderlyingType(B.GetType()) ?? B.GetType();

            if (TA.IsEnum)
                A = Convert.ToInt32(A, CultureInfo.InvariantCulture);
            if (TB.IsEnum)
                B = Convert.ToInt32(B, CultureInfo.InvariantCulture);

            if (A is IComparable Comparable)
            {
                if (A.GetType() != B.GetType())
                    B = Convert.ChangeType(B, A.GetType(), CultureInfo.InvariantCulture);
                return Comparable.CompareTo(B);
            }
        }
        catch
        {
        }

        string S1 = Convert.ToString(A, CultureInfo.InvariantCulture);
        string S2 = Convert.ToString(B, CultureInfo.InvariantCulture);
        return string.Compare(S1, S2, StringComparison.OrdinalIgnoreCase);
    }
    static private bool StringOp(object Value, object FilterValue, StringOpType Op)
    {
        if (Value == null || FilterValue == null)
            return false;

        string S1 = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
        string S2 = Convert.ToString(FilterValue, CultureInfo.InvariantCulture) ?? string.Empty;

        switch (Op)
        {
            case StringOpType.Contains:
                return S1.IndexOf(S2, StringComparison.OrdinalIgnoreCase) >= 0;
            case StringOpType.StartsWith:
                return S1.StartsWith(S2, StringComparison.OrdinalIgnoreCase);
            case StringOpType.EndsWith:
                return S1.EndsWith(S2, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
    static private bool InOp(object Value, object FilterValue)
    {
        if (FilterValue is IEnumerable List && FilterValue is not string)
        {
            foreach (object Item in List)
            {
                if (Compare(Value, Item) == 0)
                    return true;
            }
        }

        return false;
    }

    // ● static public methods
    static public List<DataSourceRow> Apply(List<DataSourceRow> Rows, RowFilterDefs Filters)
    {
        if (Rows == null || Rows.Count == 0)
            return Rows;
        if (Filters == null || Filters.Count == 0)
            return Rows;

        List<DataSourceRow> Result = new();

        foreach (DataSourceRow Row in Rows)
        {
            if (EvaluateRow(Row, Filters))
                Result.Add(Row);
        }

        return Result;
    }
}
/// <summary>
/// Core abstraction
/// </summary>
public abstract class DataSource
{
    // ● private
    List<DataSourceColumn> fColumns = new();
    List<DataSourceRow> fRows = new();
    List<DataSource> fDetails = new();
    List<DataSourceRelation> fRelations = new();
    List<string> fKeyColumns = new();
    RowFilterDefs fMasterDetailFilters = new();
    RowFilterDefs fUserFilters = new();
    LookupRegistry fLookups = new();
    int fCurrentIndex = -1;

    // ● protected methods
    protected DataSourceRow CreateRow(object SourceItem, DataSourceRowState State)
    {
        return CreateRowCore(SourceItem, State);
    }
    protected abstract object CreateNewItemCore();
    protected abstract DataSourceRow CreateRowCore(object SourceItem, DataSourceRowState State);
    protected abstract object GetValueCore(DataSourceRow Row, string ColumnName);
    protected abstract void SetValueCore(DataSourceRow Row, string ColumnName, object Value);
    protected abstract void AddCore(DataSourceRow Row);
    protected abstract void DeleteCore(DataSourceRow Row);
    protected abstract List<DataSourceColumn> LoadColumnsCore();
    protected abstract List<DataSourceRow> LoadRowsCore();
    protected abstract bool IsSameItemCore(object A, object B);
    protected virtual List<DataSourceRow> ApplyMasterDetailCore(List<DataSourceRow> Rows)
    {
        return DataSourceFilterEngine.Apply(Rows, MasterDetailFilters);
    }
    protected virtual List<DataSourceRow> ApplyUserFiltersCore(List<DataSourceRow> Rows)
    {
        return DataSourceFilterEngine.Apply(Rows, UserFilters);
    }
    protected virtual DataSourceRow FindRowByItem(object SourceItem)
    {
        return fRows.FirstOrDefault(Row => IsSameItem(Row.SourceItem, SourceItem));
    }
    protected virtual void SetCurrentInternal(DataSourceRow Row)
    {
        int NewIndex = Row == null ? -1 : fRows.IndexOf(Row);
        if (fCurrentIndex == NewIndex)
            return;

        fCurrentIndex = NewIndex;
        OnCurrentRowChanged();
    }
    protected virtual void BuildMasterDetailFilters(DataSource Detail, DataSourceRelation Relation)
    {
        Detail.MasterDetailFilters.Clear();

        if (CurrentRow == null || Relation == null || Relation.Columns.Count == 0)
            return;

        bool First = true;

        foreach (DataSourceRelationColumn RelationColumn in Relation.Columns)
        {
            RowFilterDef Filter = new();
            Filter.FieldName = RelationColumn.DetailColumnName;
            Filter.ConditionOp = ConditionOp.Equal;
            Filter.Value = CurrentRow[RelationColumn.MasterColumnName];

            if (!First)
                Filter.BoolOp = BoolOp.And;

            Detail.MasterDetailFilters.Add(Filter);
            First = false;
        }
    }
    protected virtual void RefreshDetailCore(DataSource Detail, DataSourceRelation Relation)
    {
        BuildMasterDetailFilters(Detail, Relation);
        Detail.Refresh();
    }
    protected virtual void OnCurrentRowChanged()
    {
        RefreshDetails();
        CurrentRowChanged?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnDataChanged()
    {
        DataChanged?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void OnRowStateChanged(DataSourceRow Row)
    {
        RowStateChanged?.Invoke(this, EventArgs.Empty);
    }

    // ● constructor
    protected DataSource()
    {
    }

    // ● static public methods
    static public DataSourceRowState ToDataSourceRowState(DataRowState RowState)
    {
        if ((RowState & DataRowState.Detached) == DataRowState.Detached)
            return DataSourceRowState.Detached;
        if ((RowState & DataRowState.Added) == DataRowState.Added)
            return DataSourceRowState.Added;
        if ((RowState & DataRowState.Modified) == DataRowState.Modified)
            return DataSourceRowState.Modified;
        if ((RowState & DataRowState.Deleted) == DataRowState.Deleted)
            return DataSourceRowState.Deleted;

        return DataSourceRowState.Unchanged;
    }
    static public DataRowState ToDataRowState(DataSourceRowState State)
    {
        return State switch
        {
            DataSourceRowState.Detached => DataRowState.Detached,
            DataSourceRowState.Added => DataRowState.Added,
            DataSourceRowState.Modified => DataRowState.Modified,
            DataSourceRowState.Deleted => DataRowState.Deleted,
            _ => DataRowState.Unchanged,
        };
    }

    // ● public methods
    public object GetValue(DataSourceRow Row, string ColumnName)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        return GetValueCore(Row, ColumnName);
    }
    public void SetValue(DataSourceRow Row, string ColumnName, object Value)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        SetValueCore(Row, ColumnName, Value);

        if (Row.State == DataSourceRowState.Unchanged)
        {
            Row.State = DataSourceRowState.Modified;
            OnRowStateChanged(Row);
        }

        OnDataChanged();
    }
    public bool IsSameItem(object A, object B)
    {
        if (ReferenceEquals(A, B))
            return true;
        if (A == null || B == null)
            return false;

        return IsSameItemCore(A, B);
    }
    public virtual bool MoveFirst()
    {
        return MoveTo(0);
    }
    public virtual bool MoveLast()
    {
        return MoveTo(fRows.Count - 1);
    }
    public virtual bool MoveNext()
    {
        return MoveTo(fCurrentIndex + 1);
    }
    public virtual bool MovePrevious()
    {
        return MoveTo(fCurrentIndex - 1);
    }
    public virtual bool MoveTo(int Index)
    {
        if (Index < 0 || Index >= fRows.Count)
            return false;

        SetCurrentInternal(fRows[Index]);
        return true;
    }
    public virtual bool SetCurrent(DataSourceRow Row)
    {
        if (Row == null)
        {
            SetCurrentInternal(null);
            return true;
        }
        if (Row.IsDetached)
            return false;
        if (!ReferenceEquals(Row.Source, this))
            return false;

        DataSourceRow AttachedRow = FindRowByItem(Row.SourceItem);
        if (AttachedRow == null)
            return false;

        SetCurrentInternal(AttachedRow);
        return true;
    }
    public virtual void RefreshCurrent()
    {
        if (CurrentRow == null)
            return;

        object SourceItem = CurrentRow.SourceItem;
        Refresh();
        SetCurrentInternal(FindRowByItem(SourceItem));
    }
    public virtual DataSourceRow CreateNew()
    {
        object SourceItem = CreateNewItemCore();
        return CreateRow(SourceItem, DataSourceRowState.Detached);
    }
    public virtual void Add(DataSourceRow Row)
    {
        if (Row == null)
            throw new ArgumentNullException(nameof(Row));
        if (!ReferenceEquals(Row.Source, this))
            throw new ArgumentException("The row does not belong to this DataSource.", nameof(Row));
        if (!Row.IsDetached)
            throw new InvalidOperationException("Only detached rows can be added.");

        AddCore(Row);
        Refresh();

        DataSourceRow AttachedRow = FindRowByItem(Row.SourceItem);
        if (AttachedRow == null)
            throw new InvalidOperationException("The added row could not be located after refresh.");

        AttachedRow.State = DataSourceRowState.Added;
        SetCurrentInternal(AttachedRow);
        OnRowStateChanged(AttachedRow);
        OnDataChanged();
    }
    public virtual void DeleteCurrent()
    {
        if (CurrentRow == null)
            return;

        DataSourceRow Row = CurrentRow;
        int OldIndex = fCurrentIndex;

        DeleteCore(Row);
        Row.State = DataSourceRowState.Deleted;
        Refresh();

        if (fRows.Count > 0)
        {
            int NewIndex = Math.Min(OldIndex, fRows.Count - 1);
            SetCurrentInternal(fRows[NewIndex]);
        }
        else
        {
            SetCurrentInternal(null);
        }

        OnRowStateChanged(Row);
        OnDataChanged();
    }
    public virtual void AddDetail(DataSource Detail)
    {
        if (Detail == null)
            throw new ArgumentNullException(nameof(Detail));
        if (!fDetails.Contains(Detail))
            fDetails.Add(Detail);
    }
    public virtual void RemoveDetail(DataSource Detail)
    {
        if (Detail == null)
            return;

        fDetails.Remove(Detail);
    }
    public virtual void AddRelation(DataSource Relation)
    {
        throw new NotSupportedException();
    }
    public virtual void AddRelation(DataSourceRelation Relation)
    {
        if (Relation == null)
            throw new ArgumentNullException(nameof(Relation));
        if (!ReferenceEquals(Relation.MasterSource, this))
            throw new InvalidOperationException("The relation does not belong to this master source.");
        if (!fRelations.Contains(Relation))
            fRelations.Add(Relation);
    }
    public virtual void RemoveRelation(DataSourceRelation Relation)
    {
        if (Relation == null)
            return;

        fRelations.Remove(Relation);
    }
    public virtual void Refresh()
    {
        object CurrentItem = CurrentRow?.SourceItem;

        fColumns = LoadColumnsCore() ?? new List<DataSourceColumn>();

        List<DataSourceRow> Rows = LoadRowsCore() ?? new List<DataSourceRow>();
        Rows = ApplyMasterDetailCore(Rows) ?? new List<DataSourceRow>();
        Rows = ApplyUserFiltersCore(Rows) ?? new List<DataSourceRow>();

        fRows = Rows;

        if (CurrentItem != null)
            SetCurrentInternal(FindRowByItem(CurrentItem));
        else if (fRows.Count > 0)
            SetCurrentInternal(fRows[0]);
        else
            SetCurrentInternal(null);

        RefreshDetails();
        OnDataChanged();
    }
    public virtual void RefreshDetails()
    {
        foreach (DataSource Detail in fDetails)
        {
            DataSourceRelation Relation = fRelations.FirstOrDefault(Item => ReferenceEquals(Item.DetailSource, Detail));
            if (Relation != null)
                RefreshDetailCore(Detail, Relation);
            else
                Detail.Refresh();
        }
    }
    public virtual void AcceptChanges()
    {
    }
    public virtual void RejectChanges()
    {
    }
    public override string ToString()
    {
        return GetType().Name;
    }
    
    // ● properties
    public IReadOnlyList<DataSourceColumn> Columns => fColumns;
    public IReadOnlyList<DataSourceRow> Rows => fRows;
    public DataSourceRow CurrentRow => fCurrentIndex >= 0 && fCurrentIndex < fRows.Count ? fRows[fCurrentIndex] : null;
    public object CurrentItem => CurrentRow?.SourceItem;
    public IList<string> KeyColumns => fKeyColumns;
    public IReadOnlyList<DataSourceRelation> Relations => fRelations;
    public IReadOnlyList<DataSource> Details => fDetails;
    public RowFilterDefs MasterDetailFilters => fMasterDetailFilters;
    public RowFilterDefs UserFilters => fUserFilters;
    public LookupRegistry Lookups => fLookups;
    public object this[string ColumnName]
    {
        get => CurrentRow != null ? CurrentRow[ColumnName] : null;
        set
        {
            if (CurrentRow != null)
                CurrentRow[ColumnName] = value;
        }
    }

    // ● properties
    public bool IsBof => fRows.Count == 0 || fCurrentIndex <= 0;
    public bool IsEof => fRows.Count == 0 || fCurrentIndex >= fRows.Count - 1;
    public bool IsEmpty => fRows.Count == 0;

    // ● events
    public event EventHandler CurrentRowChanged;
    public event EventHandler DataChanged;
    public event EventHandler RowStateChanged;
}
/// <summary>
/// DataView-based implementation
/// </summary>
public class DataViewSource : DataSource
{
    // ● private
    DataView fView;

    // ● constructor
    public DataViewSource(DataView View)
    {
        fView = View ?? throw new ArgumentNullException(nameof(View));
        Refresh();
    }

    // ● static public methods
    static public DataViewSource Create(DataView View)
    {
        return new DataViewSource(View);
    }

    // ● public methods
    public override void AcceptChanges()
    {
        View.Table?.AcceptChanges();
        Refresh();
    }
    public override void RejectChanges()
    {
        View.Table?.RejectChanges();
        Refresh();
    }
    public override string ToString()
    {
        if (View?.Table != null && !string.IsNullOrWhiteSpace(View.Table.TableName))
            return View.Table.TableName;

        return base.ToString();
    }

    // ● properties
    public DataView View => fView;

    // ● protected methods
    protected override object CreateNewItemCore()
    {
        if (View.Table == null)
            throw new InvalidOperationException("The DataView has no DataTable.");

        return View.Table.NewRow();
    }
    protected override DataSourceRow CreateRowCore(object SourceItem, DataSourceRowState State)
    {
        return new DataSourceRow(this, SourceItem, State);
    }
    protected override object GetValueCore(DataSourceRow Row, string ColumnName)
    {
        DataRow DataRow = (DataRow)Row.SourceItem;
        if (Row.State == DataSourceRowState.Deleted)
            return DataRow[ColumnName, DataRowVersion.Original];

        return DataRow[ColumnName];
    }
    protected override void SetValueCore(DataSourceRow Row, string ColumnName, object Value)
    {
        DataRow DataRow = (DataRow)Row.SourceItem;
        DataRow[ColumnName] = Value ?? DBNull.Value;
        Row.State = ToDataSourceRowState(DataRow.RowState);
    }
    protected override void AddCore(DataSourceRow Row)
    {
        DataRow DataRow = (DataRow)Row.SourceItem;
        View.Table.Rows.Add(DataRow);
    }
    protected override void DeleteCore(DataSourceRow Row)
    {
        DataRow DataRow = (DataRow)Row.SourceItem;
        DataRow.Delete();
    }
    protected override List<DataSourceColumn> LoadColumnsCore()
    {
        List<DataSourceColumn> Result = new();

        if (View.Table == null)
            return Result;

        foreach (DataColumn Column in View.Table.Columns)
            Result.Add(new DataSourceColumn(Column.ColumnName, Column.DataType, Column.AllowDBNull, Column.ReadOnly, Column.Caption));

        return Result;
    }
    protected override List<DataSourceRow> LoadRowsCore()
    {
        List<DataSourceRow> Result = new();

        foreach (DataRowView RowView in View)
            Result.Add(CreateRow(RowView.Row, ToDataSourceRowState(RowView.Row.RowState)));

        return Result;
    }
    protected override bool IsSameItemCore(object A, object B)
    {
        return ReferenceEquals(A, B);
    }
}
/// <summary>
/// Object-based implementation
/// </summary>
public class ObjectDataSource<T> : DataSource where T : class, new()
{
    // ● private
    static readonly BindingFlags PropFlags = BindingFlags.Instance | BindingFlags.Public;
    static readonly PropertyInfo[] Props = typeof(T).GetProperties(PropFlags).Where(Item => Item.CanRead).ToArray();
    ObservableCollection<T> fList;
    List<T> fViewList = new();

    // ● private methods
    PropertyInfo GetProperty(string ColumnName)
    {
        PropertyInfo Prop = Props.FirstOrDefault(Item => string.Equals(Item.Name, ColumnName, StringComparison.Ordinal));
        if (Prop == null)
            throw new ArgumentException($"Column not found: {ColumnName}", nameof(ColumnName));

        return Prop;
    }
    static private object ConvertValue(object Value, Type TargetType)
    {
        if (TargetType == null)
            throw new ArgumentNullException(nameof(TargetType));

        if (Value == null || Value == DBNull.Value)
        {
            if (!TargetType.IsValueType || Nullable.GetUnderlyingType(TargetType) != null)
                return null;

            return Activator.CreateInstance(TargetType);
        }

        Type ActualType = Nullable.GetUnderlyingType(TargetType) ?? TargetType;

        if (ActualType.IsInstanceOfType(Value))
            return Value;

        if (ActualType.IsEnum)
        {
            if (Value is string S)
                return Enum.Parse(ActualType, S, true);

            return Enum.ToObject(ActualType, Value);
        }

        return Convert.ChangeType(Value, ActualType, CultureInfo.InvariantCulture);
    }

    // ● constructor
    public ObjectDataSource(ObservableCollection<T> List)
    {
        fList = List ?? throw new ArgumentNullException(nameof(List));
        Refresh();
    }

    // ● static public methods
    static public ObjectDataSource<T> Create(ObservableCollection<T> List)
    {
        return new ObjectDataSource<T>(List);
    }

    // ● public methods
    public override void Refresh()
    {
        base.Refresh();
        fViewList = Rows.Select(Item => (T)Item.SourceItem).ToList();
    }
    public override void AcceptChanges()
    {
        foreach (DataSourceRow Row in Rows)
            Row.State = DataSourceRowState.Unchanged;

        Refresh();
    }
    public override string ToString()
    {
        return typeof(T).Name;
    }

    // ● properties
    public ObservableCollection<T> List => fList;
    public IReadOnlyList<T> ViewList => fViewList;

    // ● protected methods
    protected override object CreateNewItemCore()
    {
        return new T();
    }
    protected override DataSourceRow CreateRowCore(object SourceItem, DataSourceRowState State)
    {
        return new DataSourceRow(this, SourceItem, State);
    }
    protected override object GetValueCore(DataSourceRow Row, string ColumnName)
    {
        PropertyInfo Prop = GetProperty(ColumnName);
        return Prop.GetValue(Row.SourceItem);
    }
    protected override void SetValueCore(DataSourceRow Row, string ColumnName, object Value)
    {
        PropertyInfo Prop = GetProperty(ColumnName);
        object ConvertedValue = ConvertValue(Value, Prop.PropertyType);
        Prop.SetValue(Row.SourceItem, ConvertedValue);
    }
    protected override void AddCore(DataSourceRow Row)
    {
        fList.Add((T)Row.SourceItem);
    }
    protected override void DeleteCore(DataSourceRow Row)
    {
        fList.Remove((T)Row.SourceItem);
    }
    protected override List<DataSourceColumn> LoadColumnsCore()
    {
        List<DataSourceColumn> Result = new();

        foreach (PropertyInfo Prop in Props)
        {
            Type PropType = Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType;
            bool AllowNull = !Prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(Prop.PropertyType) != null;
            bool ReadOnly = !Prop.CanWrite;

            Result.Add(new DataSourceColumn(Prop.Name, PropType, AllowNull, ReadOnly));
        }

        return Result;
    }
    protected override List<DataSourceRow> LoadRowsCore()
    {
        List<DataSourceRow> Result = new();

        foreach (T Item in fList)
            Result.Add(CreateRow(Item, DataSourceRowState.Unchanged));

        return Result;
    }
    protected override bool IsSameItemCore(object A, object B)
    {
        return ReferenceEquals(A, B);
    }
    protected override List<DataSourceRow> ApplyMasterDetailCore(List<DataSourceRow> Rows)
    {
        return DataSourceFilterEngine.Apply(Rows, MasterDetailFilters);
    }
    protected override List<DataSourceRow> ApplyUserFiltersCore(List<DataSourceRow> Rows)
    {
        return DataSourceFilterEngine.Apply(Rows, UserFilters);
    }
}
