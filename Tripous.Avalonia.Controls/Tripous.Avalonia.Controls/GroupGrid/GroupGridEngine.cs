// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents the non-visual state, data adapter, columns, and row projection of a group grid.
/// </summary>
public class GroupGridEngine
{
    // ● private fields
    // ● projection state
    readonly GroupGridNodeProjection fProjection = new();
    readonly ObservableCollection<GroupGridColumn> fGroupColumns = new();
    readonly Dictionary<GroupGridColumn, string> fColumnFilters = new();

    // ● data state
    IGroupGridDataAdapter fDataAdapter;

    // ● cell state
    GroupGridCell fCurrentCell = GroupGridCell.Empty;
    GroupGridCell fSelectedCell = GroupGridCell.Empty;
    GroupGridCell fEditingCell = GroupGridCell.Empty;

    // ● viewport state
    GroupGridViewport fViewport = GroupGridViewport.Empty;
    double fBodyHeight;

    // ● operation state
    bool fIsReadOnly;
    GroupGridColumn fSortColumn;
    GroupGridSortDirection fSortDirection;

    // ● private methods
    // ● source and collection event handlers
    void Columns_CollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
    {
        if (Args.Action == NotifyCollectionChangedAction.Reset)
        {
            fGroupColumns.Clear();
            ClearFilters();
            ClearSort();
        }
        else if (Args.Action == NotifyCollectionChangedAction.Remove && Args.OldItems != null)
        {
            foreach (GroupGridColumn Column in Args.OldItems)
            {
                fGroupColumns.Remove(Column);
                ClearColumnFilter(Column);
                if (ReferenceEquals(fSortColumn, Column))
                    ClearSort();
            }
        }
        else if (Args.Action == NotifyCollectionChangedAction.Replace && Args.OldItems != null)
        {
            foreach (GroupGridColumn Column in Args.OldItems)
                if (!Columns.Contains(Column))
                {
                    fGroupColumns.Remove(Column);
                    ClearColumnFilter(Column);
                    if (ReferenceEquals(fSortColumn, Column))
                        ClearSort();
                }
        }

        ColumnsChanged?.Invoke(this, EventArgs.Empty);
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
    }
    void GroupColumns_CollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
    {
        GroupColumnsChanged?.Invoke(this, EventArgs.Empty);
        RebuildProjection();
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
    }
    void DataAdapter_Changed(object Sender, GroupGridDataChangedEventArgs Args)
    {
        if (Args.Kind == GroupGridDataChangeKind.Reset
            || Args.Kind == GroupGridDataChangeKind.RowAdded
            || Args.Kind == GroupGridDataChangeKind.RowRemoved
            || Args.Kind == GroupGridDataChangeKind.RowMoved
            || Args.Kind == GroupGridDataChangeKind.RowChanged
            || (Args.Kind == GroupGridDataChangeKind.CellChanged && IsGroupedColumnName(Args.ColumnName))
            || (Args.Kind == GroupGridDataChangeKind.CellChanged && IsFilteredColumnName(Args.ColumnName))
            || (Args.Kind == GroupGridDataChangeKind.CellChanged && IsSortedColumnName(Args.ColumnName)))
        {
            RebuildProjection();
            CoerceCurrentCell();
            CoerceSelectedCell();
            CoerceEditingCell();
        }
        else if (Args.Kind == GroupGridDataChangeKind.CellChanged)
        {
            CalculateSummaries();
            SummariesChanged?.Invoke(this, EventArgs.Empty);
        }

        DataChanged?.Invoke(this, Args);
    }
    void SetDataAdapter(IGroupGridDataAdapter Value)
    {
        if (ReferenceEquals(fDataAdapter, Value))
            return;

        if (fDataAdapter != null)
            fDataAdapter.Changed -= DataAdapter_Changed;

        fDataAdapter = Value;

        if (fDataAdapter != null)
            fDataAdapter.Changed += DataAdapter_Changed;

        RebuildProjection();
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
        DataAdapterChanged?.Invoke(this, EventArgs.Empty);
    }

    // ● cell validation helpers
    void CoerceCurrentCell()
    {
        if (fCurrentCell.IsEmpty)
            return;

        if (!IsValidValueCell(fCurrentCell))
            ClearCurrentCell();
    }
    void CoerceSelectedCell()
    {
        if (fSelectedCell.IsEmpty)
            return;

        if (!IsValidValueCell(fSelectedCell))
            ClearSelection();
    }
    void CoerceEditingCell()
    {
        if (fEditingCell.IsEmpty)
            return;

        if (!IsValidEditableCell(fEditingCell))
            CancelEdit();
    }
    bool IsValidValueCell(GroupGridCell Cell)
    {
        return !Cell.IsEmpty
               && fDataAdapter != null
               && Cell.RowIndex >= 0
               && Cell.RowIndex < fDataAdapter.RowCount
               && fProjection.IndexOfRow(Cell.RowIndex) >= 0
               && Columns.Contains(Cell.Column)
               && Cell.Column.IsVisible
               && !fGroupColumns.Contains(Cell.Column);
    }
    bool IsValidEditableCell(GroupGridCell Cell)
    {
        return !fIsReadOnly
               && IsValidValueCell(Cell)
               && !Cell.Column.IsReadOnly
               && CanSetValue(Cell.RowIndex, Cell.Column);
    }

    // ● column list helpers
    List<GroupGridColumn> GetVisibleColumnList()
    {
        return Columns.Where(Column => Column.IsVisible).ToList();
    }
    List<GroupGridColumn> GetVisibleValueColumnList()
    {
        return Columns
            .Where(Column => Column.IsVisible && !fGroupColumns.Contains(Column))
            .ToList();
    }
    GroupGridColumn GetColumnAt(IReadOnlyList<GroupGridColumn> Columns, double X, out int ColumnIndex, out double ColumnRight)
    {
        double Left = 0;
        ColumnIndex = -1;
        ColumnRight = 0;

        for (int Index = 0; Index < Columns.Count; Index++)
        {
            GroupGridColumn Column = Columns[Index];
            double Right = Left + Math.Max(Column.MinWidth, Column.Width);
            if (X >= Left && X < Right)
            {
                ColumnIndex = Index;
                ColumnRight = Right;
                return Column;
            }

            Left = Right;
        }

        return null;
    }

    // ● hit-test result helpers
    GroupGridHitTestResult CreateBandHitTestResult(double X, double Y, GroupGridBand Band, GroupGridHitTestKind Kind)
    {
        return new GroupGridHitTestResult
        {
            X = X,
            Y = Y,
            Band = Band,
            Kind = Kind,
        };
    }
    GroupGridHitTestResult CreateColumnHitTestResult(double X, double Y, GroupGridBand Band, GroupGridHitTestKind Kind, IReadOnlyList<GroupGridColumn> Columns)
    {
        GroupGridColumn Column = GetColumnAt(Columns, X, out int ColumnIndex, out double ColumnRight);
        if (Column == null)
            return CreateBandHitTestResult(X, Y, Band, Kind);

        GroupGridHitTestKind ResultKind = Kind;
        if (Band == GroupGridBand.ColumnHeader
            && Column.CanUserResize
            && ColumnRight - X <= LayoutMetrics.ColumnResizeHandleWidth)
            ResultKind = GroupGridHitTestKind.ColumnResizer;

        return new GroupGridHitTestResult
        {
            X = X,
            Y = Y,
            Band = Band,
            Kind = ResultKind,
            Column = Column,
            ColumnIndex = ColumnIndex,
        };
    }
    GroupGridHitTestResult CreateBodyHitTestResult(double X, double Y, double BodyTop)
    {
        if (fViewport.IsEmpty || LayoutMetrics.RowHeight <= 0)
            return CreateBandHitTestResult(X, Y, GroupGridBand.Body, GroupGridHitTestKind.BodyRow);

        int ViewportOffset = (int)((Y - BodyTop) / LayoutMetrics.RowHeight);
        int VisibleNodeIndex = fViewport.FirstVisibleNodeIndex + ViewportOffset;
        GroupGridRowInfo RowInfo = GetVisibleRowInfo(VisibleNodeIndex);
        if (RowInfo.IsEmpty)
            return CreateBandHitTestResult(X, Y, GroupGridBand.Body, GroupGridHitTestKind.BodyRow);

        GroupGridHitTestKind Kind = GroupGridHitTestKind.BodyRow;
        GroupGridColumn Column = null;
        int ColumnIndex = -1;

        if (RowInfo.IsGroup)
        {
            double ExpanderLeft = Math.Max(0, RowInfo.Level) * LayoutMetrics.GroupIndentWidth;
            double ExpanderRight = ExpanderLeft + LayoutMetrics.GroupExpanderWidth;
            if (X >= ExpanderLeft && X < ExpanderRight)
                Kind = GroupGridHitTestKind.GroupExpander;
        }
        else if (RowInfo.IsDataRow || RowInfo.IsGroupSummary)
        {
            Column = GetColumnAt(GetVisibleValueColumns(), X, out ColumnIndex, out double _);
            if (Column != null)
                Kind = GroupGridHitTestKind.BodyCell;
        }

        return new GroupGridHitTestResult
        {
            X = X,
            Y = Y,
            Band = GroupGridBand.Body,
            Kind = Kind,
            VisibleNodeIndex = VisibleNodeIndex,
            RowKind = RowInfo.Kind,
            RowIndex = RowInfo.RowIndex,
            Column = Column,
            ColumnIndex = ColumnIndex,
        };
    }

    // ● navigation helpers
    List<GroupGridNode> GetVisibleDataRowNodes()
    {
        return fProjection.VisibleNodes
            .Where(Node => Node.IsDataRow)
            .ToList();
    }
    GroupGridColumn GetFirstValueColumn()
    {
        return GetVisibleValueColumnList().FirstOrDefault();
    }
    GroupGridColumn GetLastValueColumn()
    {
        return GetVisibleValueColumnList().LastOrDefault();
    }
    int IndexOfVisibleDataRow(IReadOnlyList<GroupGridNode> Rows, int RowIndex)
    {
        for (int Index = 0; Index < Rows.Count; Index++)
            if (Rows[Index].RowIndex == RowIndex)
                return Index;

        return -1;
    }
    bool MoveCurrentToDataRowColumn(GroupGridNode RowNode, GroupGridColumn Column)
    {
        return RowNode != null && Column != null && SetCurrentCell(RowNode.RowIndex, Column);
    }

    // ● projection dependency helpers
    bool IsGroupedColumnName(string ColumnName)
    {
        if (string.IsNullOrWhiteSpace(ColumnName))
            return false;

        foreach (GroupGridColumn Column in fGroupColumns)
            if (string.Equals(Column.Name, ColumnName, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }
    bool IsSortedColumnName(string ColumnName)
    {
        return fSortColumn != null
               && !string.IsNullOrWhiteSpace(ColumnName)
               && string.Equals(fSortColumn.Name, ColumnName, StringComparison.OrdinalIgnoreCase);
    }
    bool IsFilteredColumnName(string ColumnName)
    {
        if (string.IsNullOrWhiteSpace(ColumnName))
            return false;

        foreach (GroupGridColumn Column in fColumnFilters.Keys)
            if (string.Equals(Column.Name, ColumnName, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }

    // ● filter helpers
    bool ContainsFilterText(object Value, GroupGridColumn Column, string FilterText)
    {
        string Text = Column.FormatValue(Value);
        return CultureInfo.CurrentCulture.CompareInfo.IndexOf(Text, FilterText, CompareOptions.IgnoreCase) >= 0;
    }
    bool MatchesWildcardFilterText(object Value, GroupGridColumn Column, string FilterText)
    {
        string Text = Column.FormatValue(Value);
        string Pattern = FilterText.Replace('%', '*');
        if (!Pattern.Contains('*'))
            Pattern = "*" + Pattern + "*";

        string[] Parts = Pattern.Split('*');
        int Index = 0;
        bool StartsWithWildcard = Pattern.StartsWith("*", StringComparison.Ordinal);
        bool EndsWithWildcard = Pattern.EndsWith("*", StringComparison.Ordinal);

        foreach (string Part in Parts)
        {
            if (Part.Length == 0)
                continue;

            int FoundIndex = CultureInfo.CurrentCulture.CompareInfo.IndexOf(Text, Part, Index, CompareOptions.IgnoreCase);
            if (FoundIndex < 0)
                return false;
            if (Index == 0 && !StartsWithWildcard && FoundIndex != 0)
                return false;

            Index = FoundIndex + Part.Length;
        }

        string LastPart = Parts.LastOrDefault(Part => Part.Length > 0);
        return EndsWithWildcard
               || string.IsNullOrEmpty(LastPart)
               || Text.EndsWith(LastPart, StringComparison.CurrentCultureIgnoreCase);
    }
    bool TryParseFilterValue(string Text, GroupGridColumn Column, out object Value)
    {
        Value = null;
        if (Column == null)
            return false;

        try
        {
            Value = Column.ParseValue(Text);
            return true;
        }
        catch
        {
            // Invalid filter values do not match any rows.
            return false;
        }
    }
    bool MatchesComparisonFilter(object Value, GroupGridColumn Column, string Operator, string FilterText)
    {
        if (!TryParseFilterValue(FilterText, Column, out object FilterValue))
            return false;

        int CompareResult = CompareValues(Value, FilterValue);
        switch (Operator)
        {
            case ">":
                return CompareResult > 0;
            case ">=":
                return CompareResult >= 0;
            case "<":
                return CompareResult < 0;
            case "<=":
                return CompareResult <= 0;
            case "=":
                return CompareResult == 0;
            case "<>":
            case "!=":
                return CompareResult != 0;
        }

        return false;
    }
    bool TryReadFilterOperator(string FilterText, out string Operator, out string Operand)
    {
        Operator = string.Empty;
        Operand = string.Empty;
        string Text = FilterText.Trim();
        string[] Operators = { ">=", "<=", "<>", "!=", ">", "<", "=" };
        foreach (string Item in Operators)
            if (Text.StartsWith(Item, StringComparison.Ordinal))
            {
                Operator = Item;
                Operand = Text.Substring(Item.Length).Trim();
                return true;
            }

        return false;
    }
    bool MatchesFilter(object Value, GroupGridColumn Column, string FilterText)
    {
        if (string.IsNullOrWhiteSpace(FilterText))
            return true;

        if (TryReadFilterOperator(FilterText, out string Operator, out string Operand))
            return MatchesComparisonFilter(Value, Column, Operator, Operand);
        if (FilterText.Contains('%') || FilterText.Contains('*'))
            return MatchesWildcardFilterText(Value, Column, FilterText);

        return ContainsFilterText(Value, Column, FilterText);
    }
    bool RowPassesFilters(int RowIndex)
    {
        if (fColumnFilters.Count == 0)
            return true;

        foreach (KeyValuePair<GroupGridColumn, string> Entry in fColumnFilters)
            if (!MatchesFilter(GetValue(RowIndex, Entry.Key), Entry.Key, Entry.Value))
                return false;

        return true;
    }

    // ● sort helpers
    bool IsNumericValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return false;

        TypeCode TypeCode = Type.GetTypeCode(Value.GetType());
        return TypeCode == TypeCode.Byte
               || TypeCode == TypeCode.SByte
               || TypeCode == TypeCode.Int16
               || TypeCode == TypeCode.UInt16
               || TypeCode == TypeCode.Int32
               || TypeCode == TypeCode.UInt32
               || TypeCode == TypeCode.Int64
               || TypeCode == TypeCode.UInt64
               || TypeCode == TypeCode.Single
               || TypeCode == TypeCode.Double
               || TypeCode == TypeCode.Decimal;
    }
    int CompareValues(object Left, object Right)
    {
        if (Left == DBNull.Value)
            Left = null;
        if (Right == DBNull.Value)
            Right = null;
        if (Left == null && Right == null)
            return 0;
        if (Left == null)
            return -1;
        if (Right == null)
            return 1;
        if (IsNumericValue(Left) && IsNumericValue(Right))
            return Convert.ToDecimal(Left, CultureInfo.CurrentCulture).CompareTo(Convert.ToDecimal(Right, CultureInfo.CurrentCulture));
        if (Left.GetType().IsInstanceOfType(Right) && Left is IComparable SameTypeComparable)
            return SameTypeComparable.CompareTo(Right);
        if (Right.GetType().IsInstanceOfType(Left) && Right is IComparable ReverseComparable)
            return -ReverseComparable.CompareTo(Left);
        if (Left is IComparable Comparable)
        {
            try
            {
                return Comparable.CompareTo(Right);
            }
            catch (ArgumentException)
            {
            }
        }

        return string.Compare(Convert.ToString(Left, CultureInfo.CurrentCulture), Convert.ToString(Right, CultureInfo.CurrentCulture), StringComparison.CurrentCulture);
    }
    int CompareRows(int LeftRowIndex, int RightRowIndex)
    {
        int Result = CompareValues(GetValue(LeftRowIndex, fSortColumn), GetValue(RightRowIndex, fSortColumn));
        if (fSortDirection == GroupGridSortDirection.Descending)
            Result = -Result;

        return Result == 0 ? LeftRowIndex.CompareTo(RightRowIndex) : Result;
    }

    // ● projection helpers
    IReadOnlyList<int> GetProjectionRowIndexes()
    {
        if (fDataAdapter == null)
            return null;
        if (fColumnFilters.Count == 0 && (fSortColumn == null || fSortDirection == GroupGridSortDirection.None))
            return null;

        List<int> Result = Enumerable.Range(0, fDataAdapter.RowCount)
            .Where(RowPassesFilters)
            .ToList();
        if (fSortColumn != null && fSortDirection != GroupGridSortDirection.None)
            Result.Sort(CompareRows);

        return Result;
    }
    GroupGridRowInfo CreateRowInfo(GroupGridNode Node, int VisibleNodeIndex)
    {
        if (Node == null)
            return GroupGridRowInfo.Empty;

        GroupGridRowKind Kind = GroupGridRowKind.None;
        if (Node.IsDataRow)
            Kind = GroupGridRowKind.DataRow;
        else if (Node.IsGroup)
            Kind = GroupGridRowKind.Group;
        else if (Node.IsGroupSummary)
            Kind = GroupGridRowKind.GroupSummary;

        return new GroupGridRowInfo(Kind, VisibleNodeIndex, Node.RowIndex, Node.Level, Node.Column, Node.Key, Node.Row, Node.IsExpanded);
    }

    // ● summary helpers
    IEnumerable<GroupGridNode> EnumerateDataRows(GroupGridNode Node)
    {
        if (Node == null)
            yield break;

        if (Node.IsDataRow)
        {
            yield return Node;
            yield break;
        }

        foreach (GroupGridNode Child in Node.Children)
            foreach (GroupGridNode DataRowNode in EnumerateDataRows(Child))
                yield return DataRowNode;
    }
    object CalculateSummaryValue(IEnumerable<GroupGridNode> DataRowNodes, GroupGridColumn Column, GroupGridAggregateKind AggregateKind)
    {
        if (Column == null || !Column.CanAggregate(AggregateKind))
            return null;

        List<GroupGridNode> Nodes = DataRowNodes.ToList();

        if (AggregateKind == GroupGridAggregateKind.Count)
            return Nodes.Count;

        List<object> Values = Nodes
            .Select(Node => GetValue(Node.RowIndex, Column))
            .Where(Value => Value != null && Value != DBNull.Value)
            .ToList();

        if (Values.Count == 0)
            return null;

        switch (AggregateKind)
        {
            case GroupGridAggregateKind.Sum:
                return CalculateNumericSum(Values);
            case GroupGridAggregateKind.Min:
                return Values
                    .OfType<IComparable>()
                    .OrderBy(Value => Value)
                    .FirstOrDefault();
            case GroupGridAggregateKind.Max:
                return Values
                    .OfType<IComparable>()
                    .OrderByDescending(Value => Value)
                    .FirstOrDefault();
            case GroupGridAggregateKind.Average:
                return CalculateAverage(Values, Column);
        }

        return null;
    }
    bool TryConvertToDecimal(object Value, out decimal Result)
    {
        Result = 0;
        if (Value == null || Value == DBNull.Value || Value is DateTime || Value is DateTimeOffset)
            return false;

        try
        {
            Result = Convert.ToDecimal(Value, CultureInfo.CurrentCulture);
            return true;
        }
        catch
        {
            return false;
        }
    }
    object CalculateNumericSum(IEnumerable<object> Values)
    {
        List<decimal> Decimals = Values
            .Select(Value => TryConvertToDecimal(Value, out decimal Decimal) ? Decimal : (decimal?)null)
            .Where(Value => Value.HasValue)
            .Select(Value => Value.Value)
            .ToList();
        return Decimals.Count == 0 ? null : Decimals.Sum();
    }
    long CalculateAverageTicks(List<long> Ticks)
    {
        if (Ticks.Count == 0)
            return 0;

        long BaseTicks = Ticks[0];
        decimal OffsetSum = 0;
        foreach (long Value in Ticks)
            OffsetSum += Value - BaseTicks;

        return BaseTicks + (long)(OffsetSum / Ticks.Count);
    }
    object CalculateAverage(IEnumerable<object> Values, GroupGridColumn Column)
    {
        Type ValueType = Nullable.GetUnderlyingType(Column.ValueType) ?? Column.ValueType;
        if (ValueType == typeof(DateTime))
        {
            List<long> Ticks = Values
                .OfType<DateTime>()
                .Select(Value => Value.Ticks)
                .ToList();
            return Ticks.Count == 0 ? null : new DateTime(CalculateAverageTicks(Ticks), DateTimeKind.Unspecified);
        }

        if (ValueType == typeof(DateTimeOffset))
        {
            List<long> Ticks = Values
                .OfType<DateTimeOffset>()
                .Select(Value => Value.Ticks)
                .ToList();
            return Ticks.Count == 0 ? null : new DateTimeOffset(new DateTime(CalculateAverageTicks(Ticks), DateTimeKind.Unspecified));
        }

        List<decimal> Decimals = Values
            .Select(Value => TryConvertToDecimal(Value, out decimal Decimal) ? Decimal : (decimal?)null)
            .Where(Value => Value.HasValue)
            .Select(Value => Value.Value)
            .ToList();
        return Decimals.Count == 0 ? null : Decimals.Average();
    }
    void CalculateGroupSummaries(GroupGridNode Node)
    {
        if (Node == null)
            return;

        foreach (GroupGridNode Child in Node.Children)
            CalculateGroupSummaries(Child);

        if (!Node.IsGroup)
            return;

        List<GroupGridNode> DataRowNodes = EnumerateDataRows(Node).ToList();
        foreach (GroupGridColumn Column in Columns)
            if (Column.GroupSummary != GroupGridAggregateKind.None && Column.CanAggregate(Column.GroupSummary))
            {
                object Value = CalculateSummaryValue(DataRowNodes, Column, Column.GroupSummary);
                Node.SetSummary(new GroupGridSummaryValue(Column, Column.GroupSummary, Value));
            }
    }
    void CalculateTotalSummaries()
    {
        List<GroupGridNode> DataRowNodes = EnumerateDataRows(fProjection.Root).ToList();
        foreach (GroupGridColumn Column in Columns)
            if (Column.TotalSummary != GroupGridAggregateKind.None && Column.CanAggregate(Column.TotalSummary))
            {
                object Value = CalculateSummaryValue(DataRowNodes, Column, Column.TotalSummary);
                fProjection.Root.SetSummary(new GroupGridSummaryValue(Column, Column.TotalSummary, Value));
            }
    }
    void CalculateSummaries()
    {
        fProjection.Root.ClearSummaries();
        CalculateGroupSummaries(fProjection.Root);
        CalculateTotalSummaries();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridEngine"/> class.
    /// </summary>
    public GroupGridEngine()
    {
        Columns.CollectionChanged += Columns_CollectionChanged;
        fGroupColumns.CollectionChanged += GroupColumns_CollectionChanged;
    }

    // ● public methods
    // ● projection API
    /// <summary>
    /// Rebuilds the root-node and visible-node projection from the current data adapter.
    /// </summary>
    public void RebuildProjection()
    {
        fProjection.Rebuild(fDataAdapter, fGroupColumns, GetProjectionRowIndexes());
        CalculateSummaries();
        VisibleNodesChanged?.Invoke(this, EventArgs.Empty);
        SummariesChanged?.Invoke(this, EventArgs.Empty);
    }

    // ● current cell and selection API
    /// <summary>
    /// Clears the current cell.
    /// </summary>
    public void ClearCurrentCell()
    {
        SetCurrentCell(GroupGridCell.Empty);
    }
    /// <summary>
    /// Clears the selected cell.
    /// </summary>
    public void ClearSelection()
    {
        SetSelectedCell(GroupGridCell.Empty);
    }
    /// <summary>
    /// Sets the current cell.
    /// </summary>
    /// <param name="Cell">The current cell.</param>
    /// <returns>True if the current cell changed; otherwise, false.</returns>
    public bool SetCurrentCell(GroupGridCell Cell)
    {
        if (Cell.IsEmpty)
            Cell = GroupGridCell.Empty;

        if (Cell == fCurrentCell)
            return false;

        if (!Cell.IsEmpty && !IsValidValueCell(Cell))
            return false;

        int OldRowIndex = fCurrentCell.RowIndex;
        fCurrentCell = Cell;
        CurrentCellChanged?.Invoke(this, EventArgs.Empty);
        if (OldRowIndex != fCurrentCell.RowIndex)
            CurrentRowChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Sets the current cell by row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the current cell changed; otherwise, false.</returns>
    public bool SetCurrentCell(int RowIndex, GroupGridColumn Column) => SetCurrentCell(new GroupGridCell(RowIndex, Column));
    /// <summary>
    /// Sets the selected cell.
    /// </summary>
    /// <param name="Cell">The selected cell.</param>
    /// <returns>True if the selected cell changed; otherwise, false.</returns>
    public bool SetSelectedCell(GroupGridCell Cell)
    {
        if (Cell.IsEmpty)
            Cell = GroupGridCell.Empty;

        if (Cell == fSelectedCell)
            return false;

        if (!Cell.IsEmpty && !IsValidValueCell(Cell))
            return false;

        fSelectedCell = Cell;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Sets the selected cell by row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the selected cell changed; otherwise, false.</returns>
    public bool SetSelectedCell(int RowIndex, GroupGridColumn Column) => SetSelectedCell(new GroupGridCell(RowIndex, Column));
    /// <summary>
    /// Selects the current cell.
    /// </summary>
    /// <returns>True if the selected cell changed; otherwise, false.</returns>
    public bool SelectCurrentCell()
    {
        return SetSelectedCell(fCurrentCell);
    }
    /// <summary>
    /// Returns true when a cell is selected.
    /// </summary>
    /// <param name="Cell">The cell to check.</param>
    /// <returns>True if the cell is selected; otherwise, false.</returns>
    public bool IsSelectedCell(GroupGridCell Cell)
    {
        return !fSelectedCell.IsEmpty && fSelectedCell == Cell;
    }
    /// <summary>
    /// Returns true when an adapter row is selected.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <returns>True if the row is selected; otherwise, false.</returns>
    public bool IsSelectedRow(int RowIndex)
    {
        return !fSelectedCell.IsEmpty && fSelectedCell.RowIndex == RowIndex;
    }

    // ● editing API
    /// <summary>
    /// Begins editing a cell.
    /// </summary>
    /// <param name="Cell">The cell to edit.</param>
    /// <returns>True if editing started; otherwise, false.</returns>
    public bool BeginEdit(GroupGridCell Cell)
    {
        if (Cell == fEditingCell)
            return true;
        if (!IsValidEditableCell(Cell))
            return false;

        if (!fEditingCell.IsEmpty)
            CancelEdit();

        GroupGridCellEditEventArgs Args = new(Cell, GetValue(Cell.RowIndex, Cell.Column));
        BeginningEdit?.Invoke(this, Args);
        if (Args.Cancel)
            return false;

        fEditingCell = Cell;
        EditingCellChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Begins editing a cell by row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if editing started; otherwise, false.</returns>
    public bool BeginEdit(int RowIndex, GroupGridColumn Column) => BeginEdit(new GroupGridCell(RowIndex, Column));
    /// <summary>
    /// Begins editing the current cell.
    /// </summary>
    /// <returns>True if editing started; otherwise, false.</returns>
    public bool BeginEdit()
    {
        return BeginEdit(fCurrentCell);
    }
    /// <summary>
    /// Commits the edited value.
    /// </summary>
    /// <param name="Value">The editor value.</param>
    /// <returns>True if the edit was committed; otherwise, false.</returns>
    public bool CommitEdit(object Value)
    {
        if (fEditingCell.IsEmpty || !IsValidEditableCell(fEditingCell))
            return false;

        GroupGridCell Cell = fEditingCell;
        object ParsedValue = Cell.Column.ParseValue(Value);
        GroupGridCellEditEventArgs Args = new(Cell, ParsedValue);

        CellValidating?.Invoke(this, Args);
        if (Args.Cancel)
            return false;

        CellValueCommitting?.Invoke(this, Args);
        if (Args.Cancel)
            return false;

        SetValue(Cell.RowIndex, Cell.Column, Args.Value);
        fEditingCell = GroupGridCell.Empty;
        EditingCellChanged?.Invoke(this, EventArgs.Empty);
        CellValueCommitted?.Invoke(this, Args);
        return true;
    }
    /// <summary>
    /// Cancels the current edit.
    /// </summary>
    /// <returns>True if an edit was canceled; otherwise, false.</returns>
    public bool CancelEdit()
    {
        if (fEditingCell.IsEmpty)
            return false;

        GroupGridCell Cell = fEditingCell;
        object Value = IsValidValueCell(Cell) ? GetValue(Cell.RowIndex, Cell.Column) : null;
        fEditingCell = GroupGridCell.Empty;
        EditingCellChanged?.Invoke(this, EventArgs.Empty);
        EditCanceled?.Invoke(this, new GroupGridCellEditEventArgs(Cell, Value));
        return true;
    }

    // ● column query API
    /// <summary>
    /// Returns the visible columns in display order.
    /// </summary>
    /// <returns>The visible columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetVisibleColumns()
    {
        return GetVisibleColumnList();
    }
    /// <summary>
    /// Returns the visible column index of a specified column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>The visible column index, or -1 if not found.</returns>
    public int IndexOfVisibleColumn(GroupGridColumn Column)
    {
        if (Column == null || !Column.IsVisible)
            return -1;

        int Index = 0;
        foreach (GroupGridColumn Item in Columns)
        {
            if (!Item.IsVisible)
                continue;
            if (ReferenceEquals(Item, Column))
                return Index;

            Index++;
        }

        return -1;
    }
    /// <summary>
    /// Returns the visible value columns in display order.
    /// </summary>
    /// <returns>The visible value columns.</returns>
    public IReadOnlyList<GroupGridColumn> GetVisibleValueColumns()
    {
        return GetVisibleValueColumnList();
    }
    /// <summary>
    /// Returns true when a column is grouped.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the column is grouped; otherwise, false.</returns>
    public bool IsGroupedColumn(GroupGridColumn Column)
    {
        return Column != null && fGroupColumns.Contains(Column);
    }

    // ● column grouping and layout API
    /// <summary>
    /// Adds a column to the grouping list.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="GroupIndex">The group index, or -1 to append.</param>
    /// <returns>True if the column was grouped; otherwise, false.</returns>
    public bool GroupColumn(GroupGridColumn Column, int GroupIndex = -1)
    {
        if (Column == null || !Columns.Contains(Column) || !Column.CanUserGroup || fGroupColumns.Contains(Column))
            return false;

        if (GroupIndex < 0 || GroupIndex > fGroupColumns.Count)
            fGroupColumns.Add(Column);
        else
            fGroupColumns.Insert(GroupIndex, Column);

        return true;
    }
    /// <summary>
    /// Removes a column from the grouping list.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the column was ungrouped; otherwise, false.</returns>
    public bool UngroupColumn(GroupGridColumn Column)
    {
        return Column != null && fGroupColumns.Remove(Column);
    }
    /// <summary>
    /// Moves a grouped column to a new group index.
    /// </summary>
    /// <param name="Column">The grouped column.</param>
    /// <param name="GroupIndex">The new group index.</param>
    /// <returns>True if the column moved; otherwise, false.</returns>
    public bool MoveGroupedColumn(GroupGridColumn Column, int GroupIndex)
    {
        if (Column == null)
            return false;

        int OldIndex = fGroupColumns.IndexOf(Column);
        if (OldIndex < 0 || GroupIndex < 0 || GroupIndex >= fGroupColumns.Count || OldIndex == GroupIndex)
            return false;

        fGroupColumns.Move(OldIndex, GroupIndex);
        return true;
    }
    /// <summary>
    /// Moves a column to a new display index in the column collection.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="ColumnIndex">The new column index.</param>
    /// <returns>True if the column moved; otherwise, false.</returns>
    public bool MoveColumn(GroupGridColumn Column, int ColumnIndex)
    {
        if (Column == null || !Column.CanUserReorder)
            return false;

        int OldIndex = Columns.IndexOf(Column);
        if (OldIndex < 0 || ColumnIndex < 0 || ColumnIndex >= Columns.Count || OldIndex == ColumnIndex)
            return false;

        Columns.Move(OldIndex, ColumnIndex);
        return true;
    }
    /// <summary>
    /// Shows or hides a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="IsVisible">True to show the column; false to hide it.</param>
    /// <returns>True if the column visibility changed; otherwise, false.</returns>
    public bool SetColumnVisible(GroupGridColumn Column, bool IsVisible)
    {
        if (Column == null || !Columns.Contains(Column) || Column.IsVisible == IsVisible)
            return false;
        if (!IsVisible && !Column.CanUserHide)
            return false;

        Column.IsVisible = IsVisible;
        ColumnsChanged?.Invoke(this, EventArgs.Empty);

        if (!fCurrentCell.IsEmpty && ReferenceEquals(fCurrentCell.Column, Column) && !IsVisible)
            ClearCurrentCell();
        if (!fSelectedCell.IsEmpty && ReferenceEquals(fSelectedCell.Column, Column) && !IsVisible)
            ClearSelection();
        if (!fEditingCell.IsEmpty && ReferenceEquals(fEditingCell.Column, Column) && !IsVisible)
            CancelEdit();
        if (!IsVisible)
            ClearColumnFilter(Column);
        if (!IsVisible && ReferenceEquals(fSortColumn, Column))
            ClearSort();

        return true;
    }

    // ● filtering API
    /// <summary>
    /// Returns the text filter applied to a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>The filter text, or an empty string when no filter is applied.</returns>
    public string GetColumnFilter(GroupGridColumn Column)
    {
        return Column != null && fColumnFilters.TryGetValue(Column, out string Result) ? Result : string.Empty;
    }
    /// <summary>
    /// Sets the contains-text filter for a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="FilterText">The filter text.</param>
    /// <returns>True if the filter changed; otherwise, false.</returns>
    public bool SetColumnFilter(GroupGridColumn Column, string FilterText)
    {
        if (Column == null || !Columns.Contains(Column))
            return false;

        FilterText = FilterText ?? string.Empty;
        if (string.IsNullOrWhiteSpace(FilterText))
            return ClearColumnFilter(Column);

        if (fColumnFilters.TryGetValue(Column, out string OldText) && OldText == FilterText)
            return false;

        fColumnFilters[Column] = FilterText;
        RebuildProjection();
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Clears the text filter for a column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>True if the filter changed; otherwise, false.</returns>
    public bool ClearColumnFilter(GroupGridColumn Column)
    {
        if (Column == null || !fColumnFilters.Remove(Column))
            return false;

        RebuildProjection();
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Clears all column filters.
    /// </summary>
    /// <returns>True if any filter was cleared; otherwise, false.</returns>
    public bool ClearFilters()
    {
        if (fColumnFilters.Count == 0)
            return false;

        fColumnFilters.Clear();
        RebuildProjection();
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    // ● sorting API
    /// <summary>
    /// Clears the active column sorting.
    /// </summary>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ClearSort()
    {
        if (fSortColumn == null && fSortDirection == GroupGridSortDirection.None)
            return false;

        fSortColumn = null;
        fSortDirection = GroupGridSortDirection.None;
        RebuildProjection();
        SortingChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Toggles sorting for a column using the None, Ascending, Descending cycle.
    /// </summary>
    /// <param name="Column">The column to sort by.</param>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ToggleSort(GroupGridColumn Column)
    {
        if (Column == null || !Columns.Contains(Column))
            return false;

        if (!ReferenceEquals(fSortColumn, Column))
        {
            fSortColumn = Column;
            fSortDirection = GroupGridSortDirection.Ascending;
        }
        else if (fSortDirection == GroupGridSortDirection.None)
        {
            fSortDirection = GroupGridSortDirection.Ascending;
        }
        else if (fSortDirection == GroupGridSortDirection.Ascending)
        {
            fSortDirection = GroupGridSortDirection.Descending;
        }
        else
        {
            fSortColumn = null;
            fSortDirection = GroupGridSortDirection.None;
        }

        RebuildProjection();
        SortingChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    // ● row projection query API
    /// <summary>
    /// Returns the visible-node index of a specified adapter row index.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <returns>The visible-node index, or -1 if not found.</returns>
    public int IndexOfVisibleRow(int RowIndex)
    {
        return fProjection.IndexOfRow(RowIndex);
    }
    /// <summary>
    /// Returns information about a visible row.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>The visible row information.</returns>
    public GroupGridRowInfo GetVisibleRowInfo(int VisibleNodeIndex)
    {
        return CreateRowInfo(fProjection.NodeByVisibleIndex(VisibleNodeIndex), VisibleNodeIndex);
    }

    // ● navigation API
    /// <summary>
    /// Moves the current cell horizontally by a visible column delta.
    /// </summary>
    /// <param name="ColumnDelta">The visible column delta.</param>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentColumn(int ColumnDelta)
    {
        if (fCurrentCell.IsEmpty || ColumnDelta == 0)
            return false;

        List<GroupGridColumn> VisibleColumns = GetVisibleValueColumnList();
        int Index = VisibleColumns.IndexOf(fCurrentCell.Column);
        if (Index < 0)
            return false;

        int NewIndex = Index + ColumnDelta;
        if (NewIndex < 0 || NewIndex >= VisibleColumns.Count)
            return false;

        return SetCurrentCell(fCurrentCell.RowIndex, VisibleColumns[NewIndex]);
    }
    /// <summary>
    /// Moves the current cell vertically by a visible data-row delta.
    /// </summary>
    /// <param name="RowDelta">The visible row delta.</param>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentRow(int RowDelta)
    {
        if (fCurrentCell.IsEmpty || RowDelta == 0)
            return false;

        int VisibleNodeIndex = fProjection.IndexOfRow(fCurrentCell.RowIndex);
        if (VisibleNodeIndex < 0)
            return false;

        GroupGridNode Node = fProjection.FindDataRowNode(VisibleNodeIndex, RowDelta);
        if (Node == null)
            return false;

        return SetCurrentCell(Node.RowIndex, fCurrentCell.Column);
    }
    /// <summary>
    /// Moves the current cell by visible row and column deltas.
    /// </summary>
    /// <param name="RowDelta">The visible row delta.</param>
    /// <param name="ColumnDelta">The visible column delta.</param>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentCell(int RowDelta, int ColumnDelta)
    {
        bool Result = false;

        if (RowDelta != 0)
            Result = MoveCurrentRow(RowDelta);
        if (ColumnDelta != 0)
            Result = MoveCurrentColumn(ColumnDelta) || Result;

        return Result;
    }
    /// <summary>
    /// Moves the current cell to the first visible value column in the current row.
    /// </summary>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentToFirstColumn()
    {
        if (fCurrentCell.IsEmpty)
            return false;

        GroupGridColumn Column = GetFirstValueColumn();
        return Column != null && SetCurrentCell(fCurrentCell.RowIndex, Column);
    }
    /// <summary>
    /// Moves the current cell to the last visible value column in the current row.
    /// </summary>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentToLastColumn()
    {
        if (fCurrentCell.IsEmpty)
            return false;

        GroupGridColumn Column = GetLastValueColumn();
        return Column != null && SetCurrentCell(fCurrentCell.RowIndex, Column);
    }
    /// <summary>
    /// Moves the current cell to the first visible data row.
    /// </summary>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentToFirstRow()
    {
        List<GroupGridNode> Rows = GetVisibleDataRowNodes();
        GroupGridColumn Column = fCurrentCell.IsEmpty ? GetFirstValueColumn() : fCurrentCell.Column;
        return Rows.Count > 0 && MoveCurrentToDataRowColumn(Rows[0], Column);
    }
    /// <summary>
    /// Moves the current cell to the last visible data row.
    /// </summary>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentToLastRow()
    {
        List<GroupGridNode> Rows = GetVisibleDataRowNodes();
        GroupGridColumn Column = fCurrentCell.IsEmpty ? GetFirstValueColumn() : fCurrentCell.Column;
        return Rows.Count > 0 && MoveCurrentToDataRowColumn(Rows[Rows.Count - 1], Column);
    }
    /// <summary>
    /// Moves the current cell to the next editable visible cell.
    /// </summary>
    /// <param name="Forward">True to move forward; false to move backward.</param>
    /// <returns>True if the current cell moved; otherwise, false.</returns>
    public bool MoveCurrentToNextEditableCell(bool Forward)
    {
        List<GroupGridNode> Rows = GetVisibleDataRowNodes();
        List<GroupGridColumn> Columns = GetVisibleValueColumnList()
            .Where(Column => !Column.IsReadOnly)
            .ToList();
        if (Rows.Count == 0 || Columns.Count == 0)
            return false;

        int RowIndex = fCurrentCell.IsEmpty ? -1 : IndexOfVisibleDataRow(Rows, fCurrentCell.RowIndex);
        int ColumnIndex = fCurrentCell.IsEmpty ? -1 : Columns.IndexOf(fCurrentCell.Column);

        if (RowIndex < 0 || ColumnIndex < 0)
        {
            RowIndex = Forward ? 0 : Rows.Count - 1;
            ColumnIndex = Forward ? 0 : Columns.Count - 1;
        }
        else if (Forward)
        {
            ColumnIndex++;
            if (ColumnIndex >= Columns.Count)
            {
                ColumnIndex = 0;
                RowIndex++;
            }
        }
        else
        {
            ColumnIndex--;
            if (ColumnIndex < 0)
            {
                ColumnIndex = Columns.Count - 1;
                RowIndex--;
            }
        }

        while (RowIndex >= 0 && RowIndex < Rows.Count)
        {
            GroupGridCell Cell = new(Rows[RowIndex].RowIndex, Columns[ColumnIndex]);
            if (IsValidEditableCell(Cell))
                return SetCurrentCell(Cell);

            if (Forward)
            {
                ColumnIndex++;
                if (ColumnIndex >= Columns.Count)
                {
                    ColumnIndex = 0;
                    RowIndex++;
                }
            }
            else
            {
                ColumnIndex--;
                if (ColumnIndex < 0)
                {
                    ColumnIndex = Columns.Count - 1;
                    RowIndex--;
                }
            }
        }

        return false;
    }

    // ● group expansion API
    /// <summary>
    /// Sets the expanded state of a group node.
    /// </summary>
    /// <param name="Node">The group node.</param>
    /// <param name="IsExpanded">True to expand; false to collapse.</param>
    /// <returns>True if the group state changed; otherwise, false.</returns>
    internal bool SetGroupExpanded(GroupGridNode Node, bool IsExpanded)
    {
        if (Node == null || !Node.IsGroup)
            return false;

        if (Node.IsExpanded == IsExpanded)
            return false;

        Node.IsExpanded = IsExpanded;
        fProjection.UpdateVisibleNodes();
        VisibleNodesChanged?.Invoke(this, EventArgs.Empty);
        CoerceCurrentCell();
        CoerceSelectedCell();
        CoerceEditingCell();
        return true;
    }
    /// <summary>
    /// Sets the expanded state of a group node by visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <param name="IsExpanded">True to expand; false to collapse.</param>
    /// <returns>True if the group state changed; otherwise, false.</returns>
    public bool SetGroupExpanded(int VisibleNodeIndex, bool IsExpanded)
    {
        return SetGroupExpanded(fProjection.NodeByVisibleIndex(VisibleNodeIndex), IsExpanded);
    }
    /// <summary>
    /// Toggles the expanded state of a group node by visible-node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>True if the group state changed; otherwise, false.</returns>
    public bool ToggleGroupExpanded(int VisibleNodeIndex)
    {
        GroupGridNode Node = fProjection.NodeByVisibleIndex(VisibleNodeIndex);
        return Node != null && Node.IsGroup && SetGroupExpanded(Node, !Node.IsExpanded);
    }

    // ● summary and display API
    /// <summary>
    /// Returns a group summary value by visible-node index and column.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>The summary value.</returns>
    public GroupGridSummaryValue GetGroupSummary(int VisibleNodeIndex, GroupGridColumn Column)
    {
        GroupGridNode Node = fProjection.NodeByVisibleIndex(VisibleNodeIndex);
        if (Node == null)
            return GroupGridSummaryValue.Empty;

        if (Node.IsGroupSummary)
            Node = Node.Parent;

        return Node != null && Node.IsGroup
            ? Node.GetSummary(Column)
            : GroupGridSummaryValue.Empty;
    }
    /// <summary>
    /// Returns a total summary value by column.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>The summary value.</returns>
    public GroupGridSummaryValue GetTotalSummary(GroupGridColumn Column)
    {
        return fProjection.Root.GetSummary(Column);
    }
    /// <summary>
    /// Returns the display text for a group header row.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <returns>The group header display text.</returns>
    public string GetGroupHeaderText(int VisibleNodeIndex)
    {
        GroupGridRowInfo RowInfo = GetVisibleRowInfo(VisibleNodeIndex);
        if (!RowInfo.IsGroup || RowInfo.Column == null)
            return string.Empty;

        string Header = string.IsNullOrWhiteSpace(RowInfo.Column.Header)
            ? RowInfo.Column.Name
            : RowInfo.Column.Header;
        string ValueText = RowInfo.Column.FormatValue(RowInfo.Key);

        return string.IsNullOrWhiteSpace(Header)
            ? ValueText
            : string.Format(CultureInfo.CurrentCulture, "{0}: {1}", Header, ValueText);
    }
    /// <summary>
    /// Returns the display text for a visible body row and value column.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible-node index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>The display text.</returns>
    public string GetDisplayText(int VisibleNodeIndex, GroupGridColumn Column)
    {
        if (Column == null)
            return string.Empty;

        GroupGridNode Node = fProjection.NodeByVisibleIndex(VisibleNodeIndex);
        if (Node == null)
            return string.Empty;

        if (Node.IsDataRow)
            return Column.FormatValue(GetValue(Node.RowIndex, Column));
        if (Node.IsGroupSummary)
            return Column.FormatSummaryValue(GetGroupSummary(VisibleNodeIndex, Column));

        return string.Empty;
    }
    /// <summary>
    /// Returns the display text for a footer summary cell.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <returns>The footer summary display text.</returns>
    public string GetTotalSummaryText(GroupGridColumn Column)
    {
        return Column == null
            ? string.Empty
            : Column.FormatSummaryValue(GetTotalSummary(Column));
    }

    // ● hit-test API
    /// <summary>
    /// Returns hit-test information for a point in local grid coordinates.
    /// </summary>
    /// <param name="X">The local x-coordinate.</param>
    /// <param name="Y">The local y-coordinate.</param>
    /// <returns>The hit-test result.</returns>
    public GroupGridHitTestResult HitTest(double X, double Y)
    {
        if (X < 0 || Y < 0)
            return GroupGridHitTestResult.Empty;

        double Top = 0;
        double Bottom = Top + LayoutMetrics.ToolBarHeight;
        if (Y < Bottom)
            return CreateBandHitTestResult(X, Y, GroupGridBand.ToolBar, GroupGridHitTestKind.ToolBar);

        Top = Bottom;
        Bottom = Top + LayoutMetrics.GroupPanelHeight;
        if (Y < Bottom)
            return CreateColumnHitTestResult(X, Y, GroupGridBand.GroupPanel, GroupGridHitTestKind.GroupPanel, GroupColumns);

        Top = Bottom;
        Bottom = Top + LayoutMetrics.ColumnHeaderHeight;
        if (Y < Bottom)
            return CreateColumnHitTestResult(X, Y, GroupGridBand.ColumnHeader, GroupGridHitTestKind.ColumnHeader, GetVisibleValueColumns());

        Top = Bottom;
        Bottom = Top + LayoutMetrics.FilterRowHeight;
        if (Y < Bottom)
            return CreateColumnHitTestResult(X, Y, GroupGridBand.FilterRow, GroupGridHitTestKind.FilterCell, GetVisibleValueColumns());

        Top = Bottom;
        Bottom = Top + Math.Max(fBodyHeight, fViewport.Count * LayoutMetrics.RowHeight);
        if (Y < Bottom)
            return CreateBodyHitTestResult(X, Y, Top);

        Top = Bottom;
        Bottom = Top + LayoutMetrics.FooterSummaryHeight;
        if (Y < Bottom)
        {
            GroupGridHitTestResult Result = CreateColumnHitTestResult(X, Y, GroupGridBand.FooterSummary, GroupGridHitTestKind.FooterSummaryCell, GetVisibleValueColumns());
            Result.RowKind = GroupGridRowKind.TotalSummary;
            return Result;
        }

        return GroupGridHitTestResult.Empty;
    }

    // ● data access API
    /// <summary>
    /// Returns the source row at a visible node index.
    /// </summary>
    /// <param name="VisibleNodeIndex">The visible node index.</param>
    /// <returns>The source row, or null when the visible node is not a data row.</returns>
    public object GetVisibleRow(int VisibleNodeIndex)
    {
        GroupGridNode Node = fProjection.NodeByVisibleIndex(VisibleNodeIndex);
        return Node != null && Node.IsDataRow ? Node.Row : null;
    }
    /// <summary>
    /// Returns the cell value at a specified adapter row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>The cell value.</returns>
    public object GetValue(int RowIndex, GroupGridColumn Column)
    {
        return fDataAdapter == null ? null : fDataAdapter.GetValue(RowIndex, Column);
    }
    /// <summary>
    /// Sets the cell value at a specified adapter row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <param name="Value">The value to set.</param>
    public void SetValue(int RowIndex, GroupGridColumn Column, object Value)
    {
        if (!fIsReadOnly && fDataAdapter != null && fDataAdapter.CanSetValue(RowIndex, Column))
            fDataAdapter.SetValue(RowIndex, Column, Value);
    }
    /// <summary>
    /// Returns true when a value can be set at a specified adapter row index and column.
    /// </summary>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <param name="Column">The grid column.</param>
    /// <returns>True when the cell value can be set; otherwise, false.</returns>
    public bool CanSetValue(int RowIndex, GroupGridColumn Column)
    {
        return !fIsReadOnly && fDataAdapter != null && fDataAdapter.CanSetValue(RowIndex, Column);
    }

    // ● row command API
    /// <summary>
    /// Returns true when a new row can be inserted after the current row.
    /// </summary>
    /// <returns>True when a row can be inserted; otherwise, false.</returns>
    public bool CanInsertRow()
    {
        int RowIndex = fCurrentCell.IsEmpty ? -1 : fCurrentCell.RowIndex;
        return !fIsReadOnly && fDataAdapter != null && fDataAdapter.CanInsertRow(RowIndex);
    }
    /// <summary>
    /// Inserts a new row after the current row.
    /// </summary>
    /// <returns>True if a row was inserted; otherwise, false.</returns>
    public bool InsertRow()
    {
        int RowIndex = fCurrentCell.IsEmpty ? -1 : fCurrentCell.RowIndex;
        if (!CanInsertRow())
            return false;

        GroupGridRowOperationEventArgs Args = new(RowIndex, RowIndex >= 0 ? fDataAdapter.GetRow(RowIndex) : null);
        InsertingRow?.Invoke(this, Args);
        if (Args.Cancel)
            return false;

        int NewRowIndex = fDataAdapter.InsertRow(RowIndex);
        if (NewRowIndex < 0)
            return false;

        GroupGridColumn Column = fCurrentCell.IsEmpty ? GetFirstValueColumn() : fCurrentCell.Column;
        if (Column != null)
        {
            SetCurrentCell(NewRowIndex, Column);
            SelectCurrentCell();
        }

        RowInserted?.Invoke(this, new GroupGridRowOperationEventArgs(NewRowIndex, fDataAdapter.GetRow(NewRowIndex)));
        return true;
    }
    /// <summary>
    /// Returns true when the current row can be deleted.
    /// </summary>
    /// <returns>True when the current row can be deleted; otherwise, false.</returns>
    public bool CanDeleteCurrentRow()
    {
        return !fIsReadOnly && !fCurrentCell.IsEmpty && fDataAdapter != null && fDataAdapter.CanDeleteRow(fCurrentCell.RowIndex);
    }
    /// <summary>
    /// Deletes the current row.
    /// </summary>
    /// <returns>True if a row was deleted; otherwise, false.</returns>
    public bool DeleteCurrentRow()
    {
        if (!CanDeleteCurrentRow())
            return false;

        int RowIndex = fCurrentCell.RowIndex;
        object Row = fDataAdapter.GetRow(RowIndex);
        GroupGridRowOperationEventArgs Args = new(RowIndex, Row);
        DeletingRow?.Invoke(this, Args);
        if (Args.Cancel)
            return false;

        if (!fDataAdapter.DeleteRow(RowIndex))
            return false;

        ClearCurrentCell();
        ClearSelection();
        RowDeleted?.Invoke(this, new GroupGridRowOperationEventArgs(RowIndex, Row));
        return true;
    }
    /// <summary>
    /// Raises the edit requested event.
    /// </summary>
    public void RequestEdit()
    {
        EditRequested?.Invoke(this, EventArgs.Empty);
    }

    // ● viewport API
    /// <summary>
    /// Sets the viewport window into the logical visible-node list.
    /// </summary>
    /// <param name="Viewport">The viewport.</param>
    /// <returns>True if the viewport changed; otherwise, false.</returns>
    public bool SetViewport(GroupGridViewport Viewport)
    {
        return SetViewport(Viewport, fViewport.Count * LayoutMetrics.RowHeight);
    }
    /// <summary>
    /// Sets the viewport window and the rendered body height.
    /// </summary>
    /// <param name="Viewport">The viewport.</param>
    /// <param name="BodyHeight">The rendered body height.</param>
    /// <returns>True if the viewport or body height changed; otherwise, false.</returns>
    public bool SetViewport(GroupGridViewport Viewport, double BodyHeight)
    {
        if (Viewport == fViewport)
        {
            double NewBodyHeight = Math.Max(0, BodyHeight);
            if (Math.Abs(NewBodyHeight - fBodyHeight) < 0.1)
                return false;

            fBodyHeight = NewBodyHeight;
            ViewportChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        fViewport = Viewport;
        fBodyHeight = Math.Max(0, BodyHeight);
        ViewportChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    // ● properties
    // ● data and column properties
    /// <summary>
    /// Gets the grid columns.
    /// </summary>
    public ObservableCollection<GroupGridColumn> Columns { get; } = new();
    /// <summary>
    /// Gets the grouped columns in grouping order.
    /// </summary>
    public IReadOnlyList<GroupGridColumn> GroupColumns => fGroupColumns;
    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    public IGroupGridDataAdapter DataAdapter
    {
        get => fDataAdapter;
        set => SetDataAdapter(value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether editing is disabled for the whole grid.
    /// </summary>
    public bool IsReadOnly
    {
        get => fIsReadOnly;
        set
        {
            if (fIsReadOnly == value)
                return;

            fIsReadOnly = value;
            CoerceEditingCell();
        }
    }

    // ● current, selection, and editing properties
    /// <summary>
    /// Gets the current cell.
    /// </summary>
    public GroupGridCell CurrentCell => fCurrentCell;
    /// <summary>
    /// Gets the selected cell.
    /// </summary>
    public GroupGridCell SelectedCell => fSelectedCell;
    /// <summary>
    /// Gets the editing cell.
    /// </summary>
    public GroupGridCell EditingCell => fEditingCell;
    /// <summary>
    /// Gets a value indicating whether the grid has a current cell.
    /// </summary>
    public bool HasCurrentCell => !fCurrentCell.IsEmpty;
    /// <summary>
    /// Gets a value indicating whether the grid has a selected cell.
    /// </summary>
    public bool HasSelectedCell => !fSelectedCell.IsEmpty;
    /// <summary>
    /// Gets a value indicating whether the grid is editing a cell.
    /// </summary>
    public bool IsEditing => !fEditingCell.IsEmpty;
    /// <summary>
    /// Gets the current row index, or -1 when there is no current cell.
    /// </summary>
    public int CurrentRowIndex => fCurrentCell.RowIndex;
    /// <summary>
    /// Gets the selected row index, or -1 when there is no selected cell.
    /// </summary>
    public int SelectedRowIndex => fSelectedCell.RowIndex;
    /// <summary>
    /// Gets the current column, or null when there is no current cell.
    /// </summary>
    public GroupGridColumn CurrentColumn => fCurrentCell.Column;
    /// <summary>
    /// Gets the selected column, or null when there is no selected cell.
    /// </summary>
    public GroupGridColumn SelectedColumn => fSelectedCell.Column;

    // ● projection, sorting, and filtering properties
    /// <summary>
    /// Gets the number of visible projected nodes.
    /// </summary>
    public int VisibleNodeCount => fProjection.VisibleNodes.Count;
    /// <summary>
    /// Gets the number of grouped columns.
    /// </summary>
    public int GroupColumnCount => fGroupColumns.Count;
    /// <summary>
    /// Gets the sorted column, or null when sorting is not active.
    /// </summary>
    public GroupGridColumn SortColumn => fSortColumn;
    /// <summary>
    /// Gets the active sort direction.
    /// </summary>
    public GroupGridSortDirection SortDirection => fSortDirection;
    /// <summary>
    /// Gets the number of active column filters.
    /// </summary>
    public int FilterCount => fColumnFilters.Count;
    /// <summary>
    /// Gets a value indicating whether any column filter is active.
    /// </summary>
    public bool HasFilters => fColumnFilters.Count > 0;

    // ● viewport and layout properties
    /// <summary>
    /// Gets the viewport window into the logical visible-node list.
    /// </summary>
    public GroupGridViewport Viewport => fViewport;
    /// <summary>
    /// Gets the rendered body height used for hit testing.
    /// </summary>
    public double BodyHeight => fBodyHeight;
    /// <summary>
    /// Gets the layout metrics used by non-visual hit testing.
    /// </summary>
    public GroupGridLayoutMetrics LayoutMetrics { get; } = new();
    /// <summary>
    /// Gets the number of adapter rows.
    /// </summary>
    public int RowCount => fDataAdapter == null ? 0 : fDataAdapter.RowCount;
    /// <summary>
    /// Gets the internal node projection.
    /// </summary>
    internal GroupGridNodeProjection Projection => fProjection;

    // ● events
    // ● data and column events
    /// <summary>
    /// Occurs when the data adapter changes.
    /// </summary>
    public event EventHandler DataAdapterChanged;
    /// <summary>
    /// Occurs when the adapter reports a data change.
    /// </summary>
    public event EventHandler<GroupGridDataChangedEventArgs> DataChanged;
    /// <summary>
    /// Occurs when the columns collection changes.
    /// </summary>
    public event EventHandler ColumnsChanged;
    /// <summary>
    /// Occurs when the grouped columns list changes.
    /// </summary>
    public event EventHandler GroupColumnsChanged;
    /// <summary>
    /// Occurs when the logical flat list of visible nodes changes.
    /// This is the full row projection after grouping or expansion rules, not the viewport subset currently rendered on screen.
    /// </summary>
    public event EventHandler VisibleNodesChanged;

    // ● current, selection, and editing events
    /// <summary>
    /// Occurs when the current cell changes.
    /// </summary>
    public event EventHandler CurrentCellChanged;
    /// <summary>
    /// Occurs when the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanged;
    /// <summary>
    /// Occurs when the selected cell changes.
    /// </summary>
    public event EventHandler SelectionChanged;
    /// <summary>
    /// Occurs before cell editing begins.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> BeginningEdit;
    /// <summary>
    /// Occurs when the editing cell changes.
    /// </summary>
    public event EventHandler EditingCellChanged;
    /// <summary>
    /// Occurs before an edited value is committed.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> CellValidating;
    /// <summary>
    /// Occurs after validation and before the value is written to the adapter.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> CellValueCommitting;
    /// <summary>
    /// Occurs after the value is written to the adapter.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> CellValueCommitted;
    /// <summary>
    /// Occurs when editing is canceled.
    /// </summary>
    public event EventHandler<GroupGridCellEditEventArgs> EditCanceled;

    // ● viewport, summary, sort, and filter events
    /// <summary>
    /// Occurs when the viewport window into the logical visible-node list changes.
    /// </summary>
    public event EventHandler ViewportChanged;
    /// <summary>
    /// Occurs when summary values change.
    /// </summary>
    public event EventHandler SummariesChanged;
    /// <summary>
    /// Occurs when column sorting changes.
    /// </summary>
    public event EventHandler SortingChanged;
    /// <summary>
    /// Occurs when column filters change.
    /// </summary>
    public event EventHandler FiltersChanged;

    // ● row command events
    /// <summary>
    /// Occurs before a row is inserted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> InsertingRow;
    /// <summary>
    /// Occurs after a row is inserted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> RowInserted;
    /// <summary>
    /// Occurs before a row is deleted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> DeletingRow;
    /// <summary>
    /// Occurs after a row is deleted.
    /// </summary>
    public event EventHandler<GroupGridRowOperationEventArgs> RowDeleted;

    // ● command events
    /// <summary>
    /// Occurs when edit is requested by a toolbar command.
    /// </summary>
    public event EventHandler EditRequested;
}
