using System;
using System.Collections;
using System.Collections.Generic;
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

public class GridViewSummary
{
    // ● construction
    public GridViewSummary()
    {
    }
    public GridViewSummary(string FieldName, AggregateType AggregateType, object Value)
    {
        this.FieldName = FieldName;
        this.AggregateType = AggregateType;
        this.Value = Value;
    }

    // ● public
    public override string ToString() => $"{FieldName}={Value}";

    // ● properties
    public string FieldName { get; set; }
    public AggregateType AggregateType { get; set; }
    public object Value { get; set; }
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
    /// <summary>
    /// Expands or collapses this node according to a specified flag.
    /// Valid only when this is a group node.
    /// Returns true only if operation was a valid one.
    /// </summary>
    public bool Expand(bool Flag)
    {
        if (!IsGroup || IsRoot)
            return false;

        if (fIsExpanded == Flag)
            return false;

        fIsExpanded = Flag;

        if (GetRootNode() is GridViewData Root)
            Root.RebuildVisibleNodes();

        return true;
    }
    /// <summary>
    /// Expands or collapses this node. Valid only when this is a group node.
    /// </summary>
    public bool Toggle()
    {
        return Expand(!IsExpanded);
    }

    // ● properties
    public GridNodeType NodeType { get; internal set; }
    public GridViewNode Parent { get; internal set; }
    /// <summary>
    /// Used when this is a group node. The value upon which this group is built.
    /// </summary>
    public object Key { get; internal set; }
    /// <summary>
    /// Used when this is a group node. The field name upon which this group is built.
    /// </summary>
    public string FieldName { get; internal set; }
    /// <summary>
    /// The level in the nodes tree.
    /// Root level is -1.
    /// </summary>
    public int Level { get; internal set; } = -1;
    /// <summary>
    /// Used when this is a row node.
    /// </summary>
    public object DataItem { get; internal set; }
    /// <summary>
    /// Used when this is a footer node.
    /// </summary>
    public GridViewNode OwnerGroup { get; internal set; }
    public List<GridViewNode> Items => fItems;
    public List<GridViewSummary> Summaries => fSummaries;
    public GridViewNode Footer { get; internal set; }
    /// <summary>
    /// Returns the first child node.
    /// </summary>
    public GridViewNode First => Items.Count > 0 ? Items[0] : null;
    /// <summary>
    /// Returns the last child node.
    /// </summary>
    public GridViewNode Last => Items.Count > 0 ? Items[Items.Count - 1] : null;
    /// <summary>
    /// Returns true if this is the first child node in its parent node.
    /// </summary>
    public bool IsFirst => Parent != null && this == Parent.First;
    /// <summary>
    /// Returns true if this is the last child node in its parent node.
    /// </summary>
    public bool IsLast => Parent != null && this == Parent.Last;
    public bool IsExpanded => fIsExpanded;
    public bool IsRoot => NodeType == GridNodeType.Group && Parent == null;
    public bool IsGroup => NodeType == GridNodeType.Group;
    public bool IsRow => NodeType == GridNodeType.Row;
    public bool IsFooter => NodeType == GridNodeType.Footer;
}

public class GridViewData: GridViewNode
{
    // ● private fields
    private List<GridViewNode> fVisibleNodes;

    // ● constructors
    public GridViewData()
        : base()
    {
        NodeType = GridNodeType.Group;
        fVisibleNodes = new();
    }

    // ● public methods
    public void RebuildVisibleNodes()
    {
        fVisibleNodes.Clear();

        foreach (GridViewNode Item in Items)
            AddVisibleNode(Item);
    }

    // ● properties
    public List<GridViewNode> VisibleNodes => fVisibleNodes;
    public bool GroupFootersVisible { get; internal set; } = true;

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
}

static public class GridViewEngine
{
    // ● private methods
    private static GridViewData ExecuteCore(IEnumerable<object> Source, GridViewDef Def)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        Def ??= new GridViewDef();

        GridViewData Result = new();
        List<object> RowList = Source.ToList();

        var GroupColumnList = Def.GetGroupColumns();

        if (GroupColumnList != null && GroupColumnList.Any())
            BuildGroups(Result, RowList, Def, 0);
        else
            Result.Items.AddRange(RowList.Select(x => CreateRowNode(Result, x)));

        PopulateSummaries(Result, Def);
        Result.RebuildVisibleNodes();

        return Result;
    }
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
    private static int GetCount(GridViewNode Node)
    {
        return GetLeafRows(Node).Count();
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
    private static object GetAverage(List<object> Values)
    {
        if (Values.Count == 0)
            return null;

        decimal Sum = Convert.ToDecimal(GetSum(Values), CultureInfo.InvariantCulture);
        return Sum / Values.Count;
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

    // ● static public methods
    static public GridViewData Execute<T>(IEnumerable<T> Source, GridViewDef Def)
    {
        IEnumerable<object> List = Source != null ? Source.Cast<object>() : null;
        Type SourceType = typeof(T);
        PropertyInfo[] Properties = SourceType.GetProperties();
        foreach (PropertyInfo Prop in Properties)
        {
            if (Def.Contains(Prop.Name))
                Def[Prop.Name].DataType = Prop.PropertyType;
        }
        return ExecuteCore(List, Def);
    }
    static public GridViewData Execute(DataView Source, GridViewDef Def)
    {
        IEnumerable<object> List = Source != null ? Source.Cast<DataRowView>().Cast<object>() : null;
        foreach (DataColumn DataColumn in Source.Table.Columns)
        {
            if (Def.Contains(DataColumn.ColumnName))
                Def[DataColumn.ColumnName].DataType = DataColumn.DataType;
        }
        return ExecuteCore(List, Def);
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
