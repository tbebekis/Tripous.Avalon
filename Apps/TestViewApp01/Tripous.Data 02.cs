using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Tripous.Data;

public enum GridNodeType
{
    None,
    Group,
    Row,
    Footer,
}

public enum GridDataRowType
{
    Data,
    Group,
    Footer,
    GrandTotal,
}

public enum GridViewSourceChangeType
{
    None,
    Reset,
    ItemAdded,
    ItemRemoved,
    ItemChanged,
}

public class GridViewSummary
{
    // ● constructors
    public GridViewSummary()
    {
    }
    public GridViewSummary(string FieldName, AggregateType AggregateType, object Value)
    {
        this.FieldName = FieldName;
        this.AggregateType = AggregateType;
        this.Value = Value;
    }

    // ● public methods
    public override string ToString() => $"{FieldName}={Value}";

    // ● properties
    public string FieldName { get; set; }
    public AggregateType AggregateType { get; set; }
    public object Value { get; set; }
}

public class GridDataRow: INotifyPropertyChanged
{
    // ● private fields
    private object fDataItem;
    private int fLevel;
    private GridViewNode fNode;
    private GridDataRowType fRowType;

    // ● private methods
    private void NotifyPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    private static object ConvertToColumnValue(object Value, Type DataType)
    {
        if (Value == null)
            return DBNull.Value;

        if (Value == DBNull.Value)
            return DBNull.Value;

        if (Value is string S && string.IsNullOrWhiteSpace(S))
            return DBNull.Value;

        Type TargetType = Nullable.GetUnderlyingType(DataType) ?? DataType;

        if (TargetType.IsEnum)
            return Value is string EnumText
                ? Enum.Parse(TargetType, EnumText, true)
                : Enum.ToObject(TargetType, Value);

        if (TargetType == typeof(Guid))
            return Value is Guid ? Value : Guid.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture));

        if (TargetType == typeof(DateTime))
            return Value is DateTime ? Value : DateTime.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

        if (TargetType == typeof(DateTimeOffset))
            return Value is DateTimeOffset ? Value : DateTimeOffset.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

        if (!TargetType.IsAssignableFrom(Value.GetType()))
            return Convert.ChangeType(Value, TargetType, CultureInfo.InvariantCulture);

        return Value;
    }

    // ● constructors
    public GridDataRow()
    {
    }

    // ● public methods
    public override string ToString()
    {
        return RowType.ToString();
    }
    public object GetValue(string FieldName)
    {
        if (!IsData)
            return null;

        if (fDataItem == null || string.IsNullOrWhiteSpace(FieldName))
            return null;

        return GridViewEngine.GetValue(fDataItem, FieldName);
    }
    public bool SetValue(string FieldName, object Value)
    {
        if (!IsData)
            return false;

        if (fDataItem == null || string.IsNullOrWhiteSpace(FieldName))
            return false;

        if (fDataItem is DataRowView RowView)
        {
            DataColumn Column = RowView.DataView.Table.Columns[FieldName];
            RowView[FieldName] = ConvertToColumnValue(Value, Column != null ? Column.DataType : typeof(object));
            NotifyPropertyChanged(nameof(DataItem));
            return true;
        }

        if (fDataItem is DataRow Row)
        {
            DataColumn Column = Row.Table.Columns[FieldName];
            Row[FieldName] = ConvertToColumnValue(Value, Column != null ? Column.DataType : typeof(object));
            NotifyPropertyChanged(nameof(DataItem));
            return true;
        }

        PropertyInfo Prop = fDataItem.GetType().GetProperty(FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (Prop == null || !Prop.CanWrite)
            return false;

        if (Value == DBNull.Value)
            Value = null;

        Type TargetType = Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType;
        if (Value != null && !TargetType.IsAssignableFrom(Value.GetType()))
        {
            if (Value is string S && string.IsNullOrWhiteSpace(S) && Nullable.GetUnderlyingType(Prop.PropertyType) != null)
                Value = null;
            else if (TargetType.IsEnum)
                Value = Value is string EnumText
                    ? Enum.Parse(TargetType, EnumText, true)
                    : Enum.ToObject(TargetType, Value);
            else if (TargetType == typeof(Guid))
                Value = Value is Guid ? Value : Guid.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture));
            else if (TargetType == typeof(DateTime))
                Value = Value is DateTime ? Value : DateTime.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            else if (TargetType == typeof(DateTimeOffset))
                Value = Value is DateTimeOffset ? Value : DateTimeOffset.Parse(Convert.ToString(Value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            else
                Value = Convert.ChangeType(Value, TargetType, CultureInfo.InvariantCulture);
        }

        Prop.SetValue(fDataItem, Value);
        NotifyPropertyChanged(nameof(DataItem));
        return true;
    }

    // ● properties
    public object DataItem
    {
        get => fDataItem;
        set
        {
            if (!ReferenceEquals(fDataItem, value))
            {
                fDataItem = value;
                NotifyPropertyChanged(nameof(DataItem));
            }
        }
    }
    public int Level
    {
        get => fLevel;
        set
        {
            if (fLevel != value)
            {
                fLevel = value;
                NotifyPropertyChanged(nameof(Level));
            }
        }
    }
    public GridViewNode Node
    {
        get => fNode;
        set
        {
            if (!ReferenceEquals(fNode, value))
            {
                fNode = value;
                NotifyPropertyChanged(nameof(Node));
            }
        }
    }
    public GridDataRowType RowType
    {
        get => fRowType;
        set
        {
            if (fRowType != value)
            {
                fRowType = value;
                NotifyPropertyChanged(nameof(RowType));
                NotifyPropertyChanged(nameof(IsData));
                NotifyPropertyChanged(nameof(IsGroup));
                NotifyPropertyChanged(nameof(IsFooter));
                NotifyPropertyChanged(nameof(IsGrandTotal));
            }
        }
    }
    public bool IsData => RowType == GridDataRowType.Data;
    public bool IsGroup => RowType == GridDataRowType.Group;
    public bool IsFooter => RowType == GridDataRowType.Footer;
    public bool IsGrandTotal => RowType == GridDataRowType.GrandTotal;

    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}

public class GridViewNode
{
    // ● private fields
    private bool fIsExpanded;
    private List<GridViewNode> fItems;
    private List<GridViewSummary> fSummaries;

    // ● private methods
    private GridViewNode GetRootNode()
    {
        GridViewNode Node = this;
        while (Node.Parent != null)
            Node = Node.Parent;
        return Node;
    }

    // ● constructors
    public GridViewNode()
    {
        fIsExpanded = true;
        fItems = new();
        fSummaries = new();
    }

    // ● public methods
    public bool Expand(bool Flag)
    {
        if (!IsGroup || IsRoot)
            return false;

        if (fIsExpanded == Flag)
            return false;

        fIsExpanded = Flag;

        if (GetRootNode() is GridViewData Root)
        {
            Root.RebuildVisibleNodes();
            Root.RebuildRows();
        }

        return true;
    }
    public bool Toggle()
    {
        return Expand(!IsExpanded);
    }

    // ● properties
    public GridNodeType NodeType { get; internal set; }
    public GridViewNode Parent { get; internal set; }
    public object Key { get; internal set; }
    public string FieldName { get; internal set; }
    public int Level { get; internal set; } = -1;
    public object DataItem { get; internal set; }
    public GridViewNode OwnerGroup { get; internal set; }
    public List<GridViewNode> Items => fItems;
    public List<GridViewSummary> Summaries => fSummaries;
    public GridViewNode Footer { get; internal set; }
    public GridViewNode First => Items.Count > 0 ? Items[0] : null;
    public GridViewNode Last => Items.Count > 0 ? Items[Items.Count - 1] : null;
    public bool IsFirst => Parent != null && this == Parent.First;
    public bool IsLast => Parent != null && this == Parent.Last;
    public bool IsExpanded => fIsExpanded;
    public bool IsRoot => NodeType == GridNodeType.Group && Parent == null;
    public bool IsGroup => NodeType == GridNodeType.Group;
    public bool IsRow => NodeType == GridNodeType.Row;
    public bool IsFooter => NodeType == GridNodeType.Footer;
}

public class GridViewSourceChangedEventArgs: EventArgs
{
    // ● constructors
    public GridViewSourceChangedEventArgs(GridViewSourceChangeType ChangeType, object Item = null)
    {
        this.ChangeType = ChangeType;
        this.Item = Item;
    }

    // ● properties
    public GridViewSourceChangeType ChangeType { get; }
    public object Item { get; }
}

public abstract class GridViewSource: IDisposable
{
    // ● private fields
    private bool fDisposed;

    // ● protected methods
    protected void OnSourceChanged(GridViewSourceChangedEventArgs e)
    {
        SourceChanged?.Invoke(this, e);
    }

    // ● constructors
    protected GridViewSource()
    {
    }

    // ● static public methods
    static public GridViewSource Create(DataView Source)
    {
        return Source != null ? new DataViewGridViewSource(Source) : null;
    }
    static public GridViewSource Create<T>(IEnumerable<T> Source)
    {
        return Source != null ? new PocoGridViewSource<T>(Source) : null;
    }

    // ● public methods
    public virtual void Dispose()
    {
        fDisposed = true;
    }

    // ● properties
    public bool IsDisposed => fDisposed;
    public abstract Type ItemType { get; }
    public abstract IEnumerable<object> Items { get; }

    // ● events
    public event EventHandler<GridViewSourceChangedEventArgs> SourceChanged;
}

public class DataViewGridViewSource: GridViewSource
{
    // ● private fields
    private DataView fSource;

    // ● private methods
    private void Source_ListChanged(object sender, ListChangedEventArgs e)
    {
        GridViewSourceChangeType ChangeType = e.ListChangedType switch
        {
            ListChangedType.ItemAdded => GridViewSourceChangeType.ItemAdded,
            ListChangedType.ItemDeleted => GridViewSourceChangeType.ItemRemoved,
            ListChangedType.ItemChanged => GridViewSourceChangeType.ItemChanged,
            _ => GridViewSourceChangeType.Reset,
        };

        object Item = null;
        if (e.NewIndex >= 0 && e.NewIndex < fSource.Count)
            Item = fSource[e.NewIndex];

        OnSourceChanged(new GridViewSourceChangedEventArgs(ChangeType, Item));
    }

    // ● constructors
    public DataViewGridViewSource(DataView Source)
    {
        fSource = Source ?? throw new ArgumentNullException(nameof(Source));
        fSource.ListChanged += Source_ListChanged;
    }

    // ● public methods
    public override void Dispose()
    {
        if (!IsDisposed && fSource != null)
            fSource.ListChanged -= Source_ListChanged;

        base.Dispose();
    }

    // ● properties
    public DataView Source => fSource;
    public override Type ItemType => typeof(DataRowView);
    public override IEnumerable<object> Items => fSource.Cast<DataRowView>().Cast<object>();
}

public class PocoGridViewSource<T>: GridViewSource
{
    // ● private fields
    private readonly Dictionary<INotifyPropertyChanged, object> fObservedItems = new();
    private INotifyCollectionChanged fNotifyCollectionChanged;
    private IEnumerable<T> fSource;

    // ● private methods
    private void AttachItemHandlers()
    {
        DetachItemHandlers();

        foreach (T Item in fSource ?? Enumerable.Empty<T>())
        {
            if (Item is INotifyPropertyChanged NotifyItem)
            {
                NotifyItem.PropertyChanged += Item_PropertyChanged;
                fObservedItems[NotifyItem] = Item;
            }
        }
    }
    private void DetachItemHandlers()
    {
        foreach (INotifyPropertyChanged Item in fObservedItems.Keys.ToList())
            Item.PropertyChanged -= Item_PropertyChanged;

        fObservedItems.Clear();
    }
    private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            AttachItemHandlers();
            OnSourceChanged(new GridViewSourceChangedEventArgs(GridViewSourceChangeType.Reset));
            return;
        }

        if (e.OldItems != null)
        {
            foreach (object Item in e.OldItems)
            {
                if (Item is INotifyPropertyChanged NotifyItem && fObservedItems.ContainsKey(NotifyItem))
                {
                    NotifyItem.PropertyChanged -= Item_PropertyChanged;
                    fObservedItems.Remove(NotifyItem);
                }
            }
        }

        if (e.NewItems != null)
        {
            foreach (object Item in e.NewItems)
            {
                if (Item is INotifyPropertyChanged NotifyItem && !fObservedItems.ContainsKey(NotifyItem))
                {
                    NotifyItem.PropertyChanged += Item_PropertyChanged;
                    fObservedItems[NotifyItem] = Item;
                }
            }
        }

        GridViewSourceChangeType ChangeType = e.Action switch
        {
            NotifyCollectionChangedAction.Add => GridViewSourceChangeType.ItemAdded,
            NotifyCollectionChangedAction.Remove => GridViewSourceChangeType.ItemRemoved,
            NotifyCollectionChangedAction.Replace => GridViewSourceChangeType.ItemChanged,
            _ => GridViewSourceChangeType.Reset,
        };

        object ItemRef = e.NewItems?.Count > 0 ? e.NewItems[0] : e.OldItems?.Count > 0 ? e.OldItems[0] : null;
        OnSourceChanged(new GridViewSourceChangedEventArgs(ChangeType, ItemRef));
    }
    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnSourceChanged(new GridViewSourceChangedEventArgs(GridViewSourceChangeType.ItemChanged, sender));
    }

    // ● constructors
    public PocoGridViewSource(IEnumerable<T> Source)
    {
        fSource = Source ?? throw new ArgumentNullException(nameof(Source));
        fNotifyCollectionChanged = Source as INotifyCollectionChanged;

        if (fNotifyCollectionChanged != null)
            fNotifyCollectionChanged.CollectionChanged += Collection_CollectionChanged;

        AttachItemHandlers();
    }

    // ● public methods
    public override void Dispose()
    {
        DetachItemHandlers();

        if (!IsDisposed && fNotifyCollectionChanged != null)
            fNotifyCollectionChanged.CollectionChanged -= Collection_CollectionChanged;

        base.Dispose();
    }

    // ● properties
    public IEnumerable<T> Source => fSource;
    public override Type ItemType => typeof(T);
    public override IEnumerable<object> Items => fSource.Cast<object>();
}

public class GridViewData: GridViewNode
{
    // ● private fields
    private List<GridViewNode> fVisibleNodes;
    private ObservableCollection<GridDataRow> fRows;

    // ● private methods
    private void AddVisibleNode(GridViewNode Node)
    {
        if (Node == null)
            return;

        fVisibleNodes.Add(Node);

        if (Node.IsGroup && Node.IsExpanded)
        {
            foreach (GridViewNode Child in Node.Items)
                AddVisibleNode(Child);

            if (GroupFootersVisible && Node.Footer != null)
                fVisibleNodes.Add(Node.Footer);
        }
    }
    private static GridDataRowType GetRowType(GridViewNode Node)
    {
        if (Node == null)
            return GridDataRowType.Data;

        if (Node.IsGroup)
            return GridDataRowType.Group;

        if (Node.IsFooter)
            return Node.OwnerGroup == null ? GridDataRowType.GrandTotal : GridDataRowType.Footer;

        return GridDataRowType.Data;
    }

    // ● constructors
    public GridViewData()
        : base()
    {
        NodeType = GridNodeType.Group;
        fVisibleNodes = new();
        fRows = new();
    }

    // ● public methods
    public void RebuildVisibleNodes()
    {
        fVisibleNodes.Clear();

        foreach (GridViewNode Item in Items)
            AddVisibleNode(Item);
    }
    public void RebuildRows()
    {
        fRows.Clear();

        foreach (GridViewNode Node in fVisibleNodes)
        {
            GridDataRow Row = new()
            {
                Node = Node,
                Level = Node.Level,
                RowType = GetRowType(Node),
                DataItem = Node.IsRow ? Node.DataItem : null,
            };

            fRows.Add(Row);
        }
    }

    // ● properties
    public GridViewDef ViewDef { get; internal set; }
    public GridViewSource Source { get; internal set; }
    public List<GridViewNode> VisibleNodes => fVisibleNodes;
    public ObservableCollection<GridDataRow> Rows => fRows;
    public bool GroupFootersVisible { get; internal set; } = true;
}

public class GridViewDataChangedEventArgs: EventArgs
{
    // ● constructors
    public GridViewDataChangedEventArgs(GridViewData Data, GridViewSourceChangedEventArgs SourceArgs = null)
    {
        this.Data = Data;
        this.SourceArgs = SourceArgs;
    }

    // ● properties
    public GridViewData Data { get; }
    public GridViewSourceChangedEventArgs SourceArgs { get; }
}

public class GridViewController: IDisposable
{
    // ● private fields
    private GridViewData fData;
    private bool fDisposed;
    private GridViewSource fSource;
    private bool fSuppressSourceChanged;
    private GridViewDef fViewDef;

    // ● private methods
    private void CloseSource()
    {
        if (fSource != null)
        {
            fSource.SourceChanged -= Source_SourceChanged;
            fSource.Dispose();
            fSource = null;
        }

        fData = null;
        fViewDef = null;
    }
    private void OnDataChanged(GridViewDataChangedEventArgs e)
    {
        DataChanged?.Invoke(this, e);
    }
    private void Refresh(GridViewSourceChangedEventArgs e = null)
    {
        if (fSource == null)
        {
            fData = null;
            OnDataChanged(new GridViewDataChangedEventArgs(fData, e));
            return;
        }

        fData = GridViewEngine.Execute(fSource, fViewDef);
        OnDataChanged(new GridViewDataChangedEventArgs(fData, e));
    }
    private void Source_SourceChanged(object sender, GridViewSourceChangedEventArgs e)
    {
        if (fSuppressSourceChanged)
            return;

        Refresh(e);
    }

    private GridViewColumnDef FindColumn(string FieldName)
    {
        if (fViewDef == null || string.IsNullOrWhiteSpace(FieldName))
            return null;

        return fViewDef.Columns.FirstOrDefault(x => string.Equals(x.FieldName, FieldName, StringComparison.OrdinalIgnoreCase));
    }
    private bool RequiresRefresh(string FieldName)
    {
        if (fViewDef == null || string.IsNullOrWhiteSpace(FieldName))
            return true;

        GridViewColumnDef Column = FindColumn(FieldName);
        if (Column == null)
            return true;

        if (Column.GroupIndex >= 0)
            return true;

        if (Column.Aggregate != AggregateType.None)
            return true;

        return true;
    }

    // ● constructors
    public GridViewController()
    {
    }
    public GridViewController(DataView Source, GridViewDef Def = null)
    {
        Open(Source, Def);
    }

    // ● public methods
    public void Close()
    {
        CloseSource();
    }
    public void Dispose()
    {
        if (!fDisposed)
        {
            CloseSource();
            fDisposed = true;
        }
    }
    public void Open(DataView Source, GridViewDef Def = null)
    {
        CloseSource();

        if (Source == null)
            return;

        fSource = new DataViewGridViewSource(Source);
        fViewDef = Def ?? GridViewEngine.CreateDefaultDef(Source);
        fSource.SourceChanged += Source_SourceChanged;

        Refresh();
    }
    public void Open<T>(IEnumerable<T> Source, GridViewDef Def = null)
    {
        CloseSource();

        if (Source == null)
            return;

        fSource = new PocoGridViewSource<T>(Source);
        fViewDef = Def ?? GridViewEngine.CreateDefaultDef(typeof(T));
        fSource.SourceChanged += Source_SourceChanged;

        Refresh();
    }
    public void Open(GridViewSource Source, GridViewDef Def = null)
    {
        CloseSource();

        if (Source == null)
            return;

        fSource = Source;
        fViewDef = Def ?? GridViewEngine.CreateDefaultDef(Source);
        fSource.SourceChanged += Source_SourceChanged;

        Refresh();
    }
    public void Refresh()
    {
        Refresh(null);
    }

    public bool CanEdit(GridDataRow Row, string FieldName)
    {
        if (Row == null || !Row.IsData)
            return false;

        GridViewColumnDef Column = FindColumn(FieldName);
        if (Column == null)
            return false;

        return !Column.IsReadOnly;
    }
    public bool SetValue(GridDataRow Row, string FieldName, object Value)
    {
        if (!CanEdit(Row, FieldName))
            return false;

        bool Result = false;

        fSuppressSourceChanged = true;
        try
        {
            Result = Row.SetValue(FieldName, Value);
        }
        finally
        {
            fSuppressSourceChanged = false;
        }

        if (!Result)
            return false;

        if (RequiresRefresh(FieldName))
            Refresh();
        else
            Refresh();

        return true;
    }

    // ● properties
    public GridViewData Data => fData;
    public bool IsDisposed => fDisposed;
    public GridViewSource Source => fSource;
    public GridViewDef ViewDef => fViewDef;

    // ● events
    public event EventHandler<GridViewDataChangedEventArgs> DataChanged;
}

static public class GridViewEngine
{
    // ● private methods
    private static void BuildGroups(GridViewNode Parent, List<object> RowList, GridViewDef Def, int GroupIndex)
    {
        var GroupColumnList = Def.GetGroupColumns().ToList();

        string FieldName = GroupColumnList[GroupIndex].FieldName;
        List<GroupBucket> Buckets = CreateBuckets(RowList, FieldName);

        foreach (GroupBucket Bucket in Buckets)
        {
            GridViewNode GroupNode = CreateGroupNode(Parent, FieldName, Bucket.Key);
            Parent.Items.Add(GroupNode);

            if (GroupIndex < GroupColumnList.Count - 1)
                BuildGroups(GroupNode, Bucket.Rows, Def, GroupIndex + 1);
            else
                GroupNode.Items.AddRange(Bucket.Rows.Select(x => CreateRowNode(GroupNode, x)));
        }
    }
    private static GridViewNode CreateFooterNode(GridViewNode OwnerGroup)
    {
        GridViewNode Result = new()
        {
            Parent = OwnerGroup,
            NodeType = GridNodeType.Footer,
            Level = OwnerGroup.Level + 1,
            OwnerGroup = OwnerGroup,
        };

        Result.Summaries.AddRange(OwnerGroup.Summaries.Select(x => new GridViewSummary(x.FieldName, x.AggregateType, x.Value)));

        return Result;
    }
    private static GridViewNode CreateGroupNode(GridViewNode Parent, string FieldName, object Key)
    {
        return new GridViewNode()
        {
            Parent = Parent,
            NodeType = GridNodeType.Group,
            FieldName = FieldName,
            Key = Key,
            Level = Parent != null ? Parent.Level + 1 : 0,
        };
    }
    private static List<GroupBucket> CreateBuckets(List<object> RowList, string FieldName)
    {
        Dictionary<object, GroupBucket> Map = new();
        List<GroupBucket> Result = new();

        foreach (object Row in RowList)
        {
            object Key = GetValue(Row, FieldName);
            object DictionaryKey = Key ?? DBNull.Value;

            if (!Map.TryGetValue(DictionaryKey, out GroupBucket Bucket))
            {
                Bucket = new GroupBucket(Key);
                Map[DictionaryKey] = Bucket;
                Result.Add(Bucket);
            }

            Bucket.Rows.Add(Row);
        }

        return Result;
    }
    private static GridViewNode CreateRowNode(GridViewNode Parent, object DataItem)
    {
        return new GridViewNode()
        {
            Parent = Parent,
            NodeType = GridNodeType.Row,
            Level = Parent != null ? Parent.Level + 1 : 0,
            DataItem = DataItem,
        };
    }
    private static int Compare(object A, object B)
    {
        if (A == null && B == null)
            return 0;

        if (A == null)
            return -1;

        if (B == null)
            return 1;

        if (A is IComparable Comparable)
            return Comparable.CompareTo(B);

        return string.Compare(Convert.ToString(A, CultureInfo.InvariantCulture), Convert.ToString(B, CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }
    private static GridViewData ExecuteCore(IEnumerable<object> Source, GridViewDef Def, GridViewSource GridSource)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        if (Def == null)
            throw new ArgumentNullException(nameof(Def));

        GridViewData Result = new()
        {
            ViewDef = Def,
            Source = GridSource,
        };

        List<object> RowList = Source.ToList();
        var GroupColumnList = Def.GetGroupColumns();

        if (GroupColumnList != null && GroupColumnList.Any())
            BuildGroups(Result, RowList, Def, 0);
        else
            Result.Items.AddRange(RowList.Select(x => CreateRowNode(Result, x)));

        PopulateSummaries(Result, Def);
        Result.RebuildVisibleNodes();
        Result.RebuildRows();

        return Result;
    }
    private static object GetAverage(List<object> Values)
    {
        if (Values.Count == 0)
            return null;

        decimal Sum = Convert.ToDecimal(GetSum(Values), CultureInfo.InvariantCulture);
        return Sum / Values.Count;
    }
    private static int GetCount(GridViewNode Node)
    {
        return GetLeafRows(Node).Count();
    }
    private static IEnumerable<GridViewNode> GetLeafRows(GridViewNode Node)
    {
        foreach (GridViewNode Child in Node.Items)
        {
            if (Child.IsRow)
                yield return Child;
            else if (Child.IsGroup)
            {
                foreach (GridViewNode RowNode in GetLeafRows(Child))
                    yield return RowNode;
            }
        }
    }
    private static object GetMax(List<object> Values)
    {
        if (Values.Count == 0)
            return null;

        object Result = Values[0];

        foreach (object Value in Values)
        {
            if (Compare(Value, Result) > 0)
                Result = Value;
        }

        return Result;
    }
    private static object GetMin(List<object> Values)
    {
        if (Values.Count == 0)
            return null;

        object Result = Values[0];

        foreach (object Value in Values)
        {
            if (Compare(Value, Result) < 0)
                Result = Value;
        }

        return Result;
    }
    private static object GetSum(List<object> Values)
    {
        if (Values.Count == 0)
            return null;

        decimal Result = 0;

        foreach (object Value in Values)
            Result += Convert.ToDecimal(Value, CultureInfo.InvariantCulture);

        return Result;
    }
    private static object GetSummaryValue(GridViewNode Node, string FieldName, AggregateType AggregateType)
    {
        List<object> Values = GetLeafRows(Node)
            .Select(x => GetValue(x.DataItem, FieldName))
            .Where(x => x != null && x != DBNull.Value)
            .ToList();

        return AggregateType switch
        {
            AggregateType.Count => GetCount(Node),
            AggregateType.Sum => GetSum(Values),
            AggregateType.Avg => GetAverage(Values),
            AggregateType.Min => GetMin(Values),
            AggregateType.Max => GetMax(Values),
            _ => null,
        };
    }
    private static void PopulateSummaries(GridViewNode Node, GridViewDef Def)
    {
        if (!Node.IsGroup)
            return;

        foreach (GridViewNode Child in Node.Items)
            PopulateSummaries(Child, Def);

        var AggregateColumnList = Def.GetAggregateColumns();
        foreach (GridViewColumnDef Item in AggregateColumnList)
        {
            object Value = GetSummaryValue(Node, Item.FieldName, Item.Aggregate);
            Node.Summaries.Add(new GridViewSummary(Item.FieldName, Item.Aggregate, Value));
        }

        if (!Node.IsRoot && Node.Summaries.Count > 0)
            Node.Footer = CreateFooterNode(Node);
    }
    private static void UpdateColumnDataTypes(DataView Source, GridViewDef Def)
    {
        foreach (DataColumn DataColumn in Source.Table.Columns)
        {
            if (Def.Contains(DataColumn.ColumnName))
                Def[DataColumn.ColumnName].DataType = DataColumn.DataType;
        }
    }
    private static void UpdateColumnDataTypes(Type SourceType, GridViewDef Def)
    {
        PropertyInfo[] Properties = SourceType.GetProperties();
        foreach (PropertyInfo Prop in Properties)
        {
            if (Def.Contains(Prop.Name))
                Def[Prop.Name].DataType = Prop.PropertyType;
        }
    }

    // ● static public methods
    static public GridViewDef CreateDefaultDef(DataView Source)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        GridViewDef Def = new();

        foreach (DataColumn DataColumn in Source.Table.Columns)
        {
            GridViewColumnDef Column = new()
            {
                FieldName = DataColumn.ColumnName,
                Title = DataColumn.ColumnName,
                DataType = DataColumn.DataType,
            };

            Def.Columns.Add(Column);
            Column.VisibleIndex = Def.Columns.IndexOf(Column);
        }

        return Def;
    }
    static public GridViewDef CreateDefaultDef(Type ItemType)
    {
        if (ItemType == null)
            throw new ArgumentNullException(nameof(ItemType));

        GridViewDef Def = new();
        PropertyInfo[] Properties = ItemType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (PropertyInfo Prop in Properties)
        {
            GridViewColumnDef Column = new()
            {
                FieldName = Prop.Name,
                Title = Prop.Name,
                DataType = Prop.PropertyType,
            };

            Def.Columns.Add(Column);
            Column.VisibleIndex = Def.Columns.IndexOf(Column);
        }

        return Def;
    }
    static public GridViewDef CreateDefaultDef(GridViewSource Source)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        if (Source is DataViewGridViewSource DataViewSource)
            return CreateDefaultDef(DataViewSource.Source);

        return CreateDefaultDef(Source.ItemType);
    }
    static public GridViewData Execute(GridViewSource Source, GridViewDef Def = null)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        Def ??= CreateDefaultDef(Source);

        if (Source is DataViewGridViewSource DataViewSource)
            UpdateColumnDataTypes(DataViewSource.Source, Def);
        else
            UpdateColumnDataTypes(Source.ItemType, Def);

        return ExecuteCore(Source.Items, Def, Source);
    }
    static public GridViewData Execute<T>(IEnumerable<T> Source, GridViewDef Def = null)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        Def ??= CreateDefaultDef(typeof(T));
        UpdateColumnDataTypes(typeof(T), Def);

        return ExecuteCore(Source.Cast<object>(), Def, new PocoGridViewSource<T>(Source));
    }
    static public GridViewData Execute(DataView Source, GridViewDef Def = null)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        Def ??= CreateDefaultDef(Source);
        UpdateColumnDataTypes(Source, Def);

        return ExecuteCore(Source.Cast<DataRowView>().Cast<object>(), Def, new DataViewGridViewSource(Source));
    }
    internal static object GetValue(object Instance, string FieldName)
    {
        if (Instance == null)
            return null;

        if (Instance is DataRowView RowView)
            return RowView[FieldName];

        if (Instance is DataRow Row)
            return Row[FieldName];

        PropertyInfo Prop = Instance.GetType().GetProperty(FieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (Prop != null)
            return Prop.GetValue(Instance);

        throw new ApplicationException($"Field not found: {FieldName}");
    }

    // ● private fields
    private class GroupBucket
    {
        // ● constructors
        public GroupBucket(object Key)
        {
            this.Key = Key;
        }

        // ● properties
        public object Key { get; }
        public List<object> Rows { get; } = new();
    }
}
