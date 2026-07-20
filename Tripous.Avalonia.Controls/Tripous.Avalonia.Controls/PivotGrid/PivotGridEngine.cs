// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents the non-visual state and projection of a pivot grid.
/// </summary>
public class PivotGridEngine
{
    // ● private fields
    readonly List<PivotGridAxisItem> fRowItems = new();
    readonly List<PivotGridAxisItem> fColumnItems = new();
    readonly List<PivotGridAxisNode> fVisibleRowNodes = new();
    readonly Dictionary<string, PivotGridValueCell> fCells = new();
    readonly Dictionary<string, PivotGridValueCell> fRowTotalCells = new();
    readonly Dictionary<string, PivotGridValueCell> fColumnTotalCells = new();
    readonly Dictionary<string, PivotGridValueCell> fGrandTotalCells = new();
    readonly Dictionary<string, HashSet<string>> fFilters = new(StringComparer.OrdinalIgnoreCase);
    PivotGridAxisNode fRowRoot;
    IPivotGridDataAdapter fDataAdapter;
    PivotGridFieldRole fSortRole;
    string fSortFieldName = string.Empty;
    PivotGridSortDirection fSortDirection;

    // ● private methods
    void DataAdapter_Changed(object Sender, PivotGridDataChangedEventArgs Args)
    {
        Rebuild();
    }
    void SetDataAdapter(IPivotGridDataAdapter Value)
    {
        if (ReferenceEquals(fDataAdapter, Value))
            return;

        if (fDataAdapter != null)
            fDataAdapter.Changed -= DataAdapter_Changed;

        fDataAdapter = Value;

        if (fDataAdapter != null)
            fDataAdapter.Changed += DataAdapter_Changed;

        Rebuild();
        DataAdapterChanged?.Invoke(this, EventArgs.Empty);
    }
    string FormatAxisItem(IReadOnlyList<PivotGridField> Fields, IReadOnlyList<object> Values)
    {
        List<string> Parts = new();
        for (int Index = 0; Index < Fields.Count; Index++)
            Parts.Add(Fields[Index].FormatValue(Values[Index]));

        return string.Join(" | ", Parts);
    }
    string CreateKey(IEnumerable<object> Values)
    {
        return string.Join("\u001F", Values.Select(CreateValueKey));
    }
    string CreateValueKey(object Value)
    {
        return Value == null || Value == DBNull.Value ? string.Empty : Convert.ToString(Value, CultureInfo.InvariantCulture);
    }
    string CreateCellKey(string RowKey, string ColumnKey, PivotGridMeasure Measure)
    {
        return RowKey + "\u001E" + ColumnKey + "\u001E" + (Measure == null ? string.Empty : Measure.Name);
    }
    string CreateTotalCellKey(string AxisKey, PivotGridMeasure Measure)
    {
        return AxisKey + "\u001E" + (Measure == null ? string.Empty : Measure.Name);
    }
    PivotGridAxisItem GetOrAddAxisItem(Dictionary<string, PivotGridAxisItem> Map, List<PivotGridAxisItem> Items, IReadOnlyList<PivotGridField> Fields, IReadOnlyList<object> Values)
    {
        string Key = CreateKey(Values);
        if (Map.TryGetValue(Key, out PivotGridAxisItem Result))
            return Result;

        Result = new PivotGridAxisItem(Key, FormatAxisItem(Fields, Values), Values.ToList());
        Map.Add(Key, Result);
        Items.Add(Result);
        return Result;
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
        if (PivotGridFieldRules.IsNumericType(Left.GetType()) && PivotGridFieldRules.IsNumericType(Right.GetType()))
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
    int GetSortedRowFieldIndex()
    {
        if (fSortRole != PivotGridFieldRole.Row || fSortDirection == PivotGridSortDirection.None || string.IsNullOrWhiteSpace(fSortFieldName))
            return -1;

        for (int Index = 0; Index < RowFields.Count; Index++)
            if (string.Equals(RowFields[Index].Name, fSortFieldName, StringComparison.OrdinalIgnoreCase))
                return Index;

        return -1;
    }
    int GetSortedColumnFieldIndex()
    {
        if (fSortRole != PivotGridFieldRole.Column || fSortDirection == PivotGridSortDirection.None || string.IsNullOrWhiteSpace(fSortFieldName))
            return -1;

        for (int Index = 0; Index < ColumnFields.Count; Index++)
            if (string.Equals(ColumnFields[Index].Name, fSortFieldName, StringComparison.OrdinalIgnoreCase))
                return Index;

        return -1;
    }
    int CompareRowNodes(PivotGridAxisNode Left, PivotGridAxisNode Right, int FieldIndex)
    {
        object LeftValue = Left?.Item?.Values.Count > FieldIndex ? Left.Item.Values[FieldIndex] : null;
        object RightValue = Right?.Item?.Values.Count > FieldIndex ? Right.Item.Values[FieldIndex] : null;
        int Result = CompareValues(LeftValue, RightValue);
        return fSortDirection == PivotGridSortDirection.Descending ? -Result : Result;
    }
    int CompareColumnItems(PivotGridAxisItem Left, PivotGridAxisItem Right, int FieldIndex)
    {
        object LeftValue = Left?.Values.Count > FieldIndex ? Left.Values[FieldIndex] : null;
        object RightValue = Right?.Values.Count > FieldIndex ? Right.Values[FieldIndex] : null;
        int Result = CompareValues(LeftValue, RightValue);
        return fSortDirection == PivotGridSortDirection.Descending ? -Result : Result;
    }
    void SortRowNodes(PivotGridAxisNode Node, int FieldIndex)
    {
        if (Node == null)
            return;

        if (Node.Level + 1 == FieldIndex && Node.Children is List<PivotGridAxisNode> Children)
            Children.Sort((Left, Right) => CompareRowNodes(Left, Right, FieldIndex));

        foreach (PivotGridAxisNode Child in Node.Children)
            SortRowNodes(Child, FieldIndex);
    }
    void ApplySorting()
    {
        int RowFieldIndex = GetSortedRowFieldIndex();
        if (RowFieldIndex >= 0)
            SortRowNodes(fRowRoot, RowFieldIndex);

        int ColumnFieldIndex = GetSortedColumnFieldIndex();
        if (ColumnFieldIndex >= 0)
            fColumnItems.Sort((Left, Right) => CompareColumnItems(Left, Right, ColumnFieldIndex));
    }
    PivotGridAxisNode FindChild(PivotGridAxisNode Parent, string Key)
    {
        foreach (PivotGridAxisNode Child in Parent.Children)
            if (Child.Item != null && Child.Item.Key == Key)
                return Child;

        return null;
    }
    PivotGridAxisNode GetOrAddRowNode(PivotGridAxisNode Parent, PivotGridField Field, object Value)
    {
        List<object> Values = Parent.Item == null
            ? new List<object> { Value }
            : Parent.Item.Values.Concat(new[] { Value }).ToList();
        string Key = CreateKey(Values);
        PivotGridAxisNode Result = FindChild(Parent, Key);
        if (Result != null)
            return Result;

        string Text = Field == null ? string.Empty : Field.FormatValue(Value);
        PivotGridAxisItem Item = new(Key, Text, Values);
        Result = new PivotGridAxisNode(Parent, Item, Parent.Level + 1);
        Parent.Add(Result);
        return Result;
    }
    List<PivotGridAxisNode> GetOrAddRowPath(IReadOnlyList<object> Values)
    {
        List<PivotGridAxisNode> Result = new();
        PivotGridAxisNode Parent = fRowRoot;
        if (RowFields.Count == 0)
        {
            Parent = GetOrAddRowNode(Parent, null, string.Empty);
            Result.Add(Parent);
            return Result;
        }

        for (int Index = 0; Index < RowFields.Count; Index++)
        {
            Parent = GetOrAddRowNode(Parent, RowFields[Index], Values[Index]);
            Result.Add(Parent);
        }

        return Result;
    }
    void UpdateVisibleRows()
    {
        fVisibleRowNodes.Clear();
        fRowRoot?.AddVisibleNodesTo(fVisibleRowNodes);
        fRowItems.Clear();
        foreach (PivotGridAxisNode Node in fVisibleRowNodes)
            fRowItems.Add(Node.Item);
    }
    void AddCollapsedRowKeys(PivotGridAxisNode Node, List<string> List)
    {
        if (Node == null)
            return;

        if (!Node.IsRoot && Node.HasChildren && !Node.IsExpanded && Node.Item != null)
            List.Add(Node.Item.Key);
        foreach (PivotGridAxisNode Child in Node.Children)
            AddCollapsedRowKeys(Child, List);
    }
    bool ApplyCollapsedRowKeys(PivotGridAxisNode Node, HashSet<string> Keys)
    {
        if (Node == null)
            return false;

        bool Result = false;
        if (!Node.IsRoot && Node.HasChildren && Node.Item != null)
        {
            bool NewExpanded = !Keys.Contains(Node.Item.Key);
            if (Node.IsExpanded != NewExpanded)
            {
                Node.IsExpanded = NewExpanded;
                Result = true;
            }
        }

        foreach (PivotGridAxisNode Child in Node.Children)
            Result = ApplyCollapsedRowKeys(Child, Keys) || Result;

        return Result;
    }
    bool SetRowExpandedRecursive(PivotGridAxisNode Node, bool IsExpanded)
    {
        if (Node == null)
            return false;

        bool Result = false;
        if (!Node.IsRoot && Node.HasChildren && Node.IsExpanded != IsExpanded)
        {
            Node.IsExpanded = IsExpanded;
            Result = true;
        }

        foreach (PivotGridAxisNode Child in Node.Children)
            Result = SetRowExpandedRecursive(Child, IsExpanded) || Result;

        return Result;
    }
    bool HasExpandableRow(PivotGridAxisNode Node, bool IsExpanded)
    {
        if (Node == null)
            return false;
        if (!Node.IsRoot && Node.HasChildren && Node.IsExpanded != IsExpanded)
            return true;

        return Node.Children.Any(Child => HasExpandableRow(Child, IsExpanded));
    }
    object AggregateValues(List<object> Values, PivotGridMeasure Measure)
    {
        if (Measure == null)
            return null;
        if (Measure.AggregateKind == PivotGridAggregateKind.Count)
            return Values.Count;

        List<object> NonEmptyValues = Values
            .Where(Value => Value != null && Value != DBNull.Value)
            .ToList();
        if (NonEmptyValues.Count == 0)
            return null;

        switch (Measure.AggregateKind)
        {
            case PivotGridAggregateKind.Sum:
                return SumValues(NonEmptyValues);
            case PivotGridAggregateKind.Min:
                return NonEmptyValues.OfType<IComparable>().OrderBy(Value => Value).FirstOrDefault();
            case PivotGridAggregateKind.Max:
                return NonEmptyValues.OfType<IComparable>().OrderByDescending(Value => Value).FirstOrDefault();
            case PivotGridAggregateKind.Average:
                return AverageValues(NonEmptyValues);
        }

        return null;
    }
    decimal SumValues(IEnumerable<object> Values)
    {
        decimal Result = 0;
        foreach (object Value in Values)
            Result += Convert.ToDecimal(Value, CultureInfo.CurrentCulture);

        return Result;
    }
    object AverageValues(IReadOnlyList<object> Values)
    {
        if (Values.Count == 0)
            return null;

        return SumValues(Values) / Values.Count;
    }
    bool RowPassesFilters(int RowIndex)
    {
        foreach (KeyValuePair<string, HashSet<string>> Entry in fFilters)
            if (!Entry.Value.Contains(CreateValueKey(fDataAdapter.GetValue(RowIndex, Entry.Key))))
                return false;

        return true;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridEngine"/> class.
    /// </summary>
    public PivotGridEngine()
    {
        fRowRoot = new PivotGridAxisNode(null, null, -1);
        RowFields.CollectionChanged += (Sender, Args) => Rebuild();
        ColumnFields.CollectionChanged += (Sender, Args) => Rebuild();
        Measures.CollectionChanged += (Sender, Args) => Rebuild();
    }

    // ● public methods
    /// <summary>
    /// Rebuilds the pivot projection from the current data adapter, axes, and measures.
    /// </summary>
    public void Rebuild()
    {
        fRowItems.Clear();
        fColumnItems.Clear();
        fVisibleRowNodes.Clear();
        fCells.Clear();
        fRowTotalCells.Clear();
        fColumnTotalCells.Clear();
        fGrandTotalCells.Clear();
        fRowRoot = new PivotGridAxisNode(null, null, -1);

        if (fDataAdapter == null || Measures.Count == 0)
        {
            ProjectionChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        Dictionary<string, PivotGridAxisItem> ColumnMap = new();
        Dictionary<string, List<object>> Buckets = new();
        Dictionary<string, List<object>> RowTotalBuckets = new();
        Dictionary<string, List<object>> ColumnTotalBuckets = new();
        Dictionary<string, List<object>> GrandTotalBuckets = new();
        Dictionary<string, (PivotGridAxisItem RowItem, PivotGridAxisItem ColumnItem, PivotGridMeasure Measure)> BucketInfo = new();
        Dictionary<string, (PivotGridAxisItem RowItem, PivotGridMeasure Measure)> RowTotalBucketInfo = new();
        Dictionary<string, (PivotGridAxisItem ColumnItem, PivotGridMeasure Measure)> ColumnTotalBucketInfo = new();
        Dictionary<string, PivotGridMeasure> GrandTotalBucketInfo = new();

        for (int RowIndex = 0; RowIndex < fDataAdapter.RowCount; RowIndex++)
        {
            if (!RowPassesFilters(RowIndex))
                continue;

            List<object> RowValues = RowFields.Select(Field => fDataAdapter.GetValue(RowIndex, Field.Name)).ToList();
            List<object> ColumnValues = ColumnFields.Select(Field => fDataAdapter.GetValue(RowIndex, Field.Name)).ToList();
            List<PivotGridAxisNode> RowPath = GetOrAddRowPath(RowValues);
            PivotGridAxisItem ColumnItem = GetOrAddAxisItem(ColumnMap, fColumnItems, ColumnFields, ColumnValues);

            foreach (PivotGridAxisNode RowNode in RowPath)
                foreach (PivotGridMeasure Measure in Measures)
                {
                    string CellKey = CreateCellKey(RowNode.Item.Key, ColumnItem.Key, Measure);
                    if (!Buckets.TryGetValue(CellKey, out List<object> Values))
                    {
                        Values = new List<object>();
                        Buckets.Add(CellKey, Values);
                        BucketInfo.Add(CellKey, (RowNode.Item, ColumnItem, Measure));
                    }

                    Values.Add(fDataAdapter.GetValue(RowIndex, Measure.SourceFieldName));

                    string RowTotalCellKey = CreateTotalCellKey(RowNode.Item.Key, Measure);
                    if (!RowTotalBuckets.TryGetValue(RowTotalCellKey, out List<object> RowTotalValues))
                    {
                        RowTotalValues = new List<object>();
                        RowTotalBuckets.Add(RowTotalCellKey, RowTotalValues);
                        RowTotalBucketInfo.Add(RowTotalCellKey, (RowNode.Item, Measure));
                    }
                    RowTotalValues.Add(fDataAdapter.GetValue(RowIndex, Measure.SourceFieldName));
                }

            foreach (PivotGridMeasure Measure in Measures)
            {
                object Value = fDataAdapter.GetValue(RowIndex, Measure.SourceFieldName);
                string ColumnTotalCellKey = CreateTotalCellKey(ColumnItem.Key, Measure);
                if (!ColumnTotalBuckets.TryGetValue(ColumnTotalCellKey, out List<object> ColumnTotalValues))
                {
                    ColumnTotalValues = new List<object>();
                    ColumnTotalBuckets.Add(ColumnTotalCellKey, ColumnTotalValues);
                    ColumnTotalBucketInfo.Add(ColumnTotalCellKey, (ColumnItem, Measure));
                }
                ColumnTotalValues.Add(Value);

                string GrandTotalCellKey = Measure.Name;
                if (!GrandTotalBuckets.TryGetValue(GrandTotalCellKey, out List<object> GrandTotalValues))
                {
                    GrandTotalValues = new List<object>();
                    GrandTotalBuckets.Add(GrandTotalCellKey, GrandTotalValues);
                    GrandTotalBucketInfo.Add(GrandTotalCellKey, Measure);
                }
                GrandTotalValues.Add(Value);
            }
        }

        ApplySorting();
        UpdateVisibleRows();
        foreach (KeyValuePair<string, List<object>> Entry in Buckets)
        {
            (PivotGridAxisItem RowItem, PivotGridAxisItem ColumnItem, PivotGridMeasure Measure) Info = BucketInfo[Entry.Key];
            fCells.Add(Entry.Key, new PivotGridValueCell(Info.RowItem, Info.ColumnItem, Info.Measure, AggregateValues(Entry.Value, Info.Measure)));
        }
        foreach (KeyValuePair<string, List<object>> Entry in RowTotalBuckets)
        {
            (PivotGridAxisItem RowItem, PivotGridMeasure Measure) Info = RowTotalBucketInfo[Entry.Key];
            fRowTotalCells.Add(Entry.Key, new PivotGridValueCell(Info.RowItem, null, Info.Measure, AggregateValues(Entry.Value, Info.Measure)));
        }
        foreach (KeyValuePair<string, List<object>> Entry in ColumnTotalBuckets)
        {
            (PivotGridAxisItem ColumnItem, PivotGridMeasure Measure) Info = ColumnTotalBucketInfo[Entry.Key];
            fColumnTotalCells.Add(Entry.Key, new PivotGridValueCell(null, Info.ColumnItem, Info.Measure, AggregateValues(Entry.Value, Info.Measure)));
        }
        foreach (KeyValuePair<string, List<object>> Entry in GrandTotalBuckets)
        {
            PivotGridMeasure Measure = GrandTotalBucketInfo[Entry.Key];
            fGrandTotalCells.Add(Entry.Key, new PivotGridValueCell(null, null, Measure, AggregateValues(Entry.Value, Measure)));
        }

        ProjectionChanged?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Toggles a row-axis node expanded state.
    /// </summary>
    /// <param name="VisibleRowIndex">The visible row index.</param>
    /// <returns>True if the node expanded state changed; otherwise, false.</returns>
    public bool ToggleRowExpanded(int VisibleRowIndex)
    {
        if (VisibleRowIndex < 0 || VisibleRowIndex >= fVisibleRowNodes.Count)
            return false;

        PivotGridAxisNode Node = fVisibleRowNodes[VisibleRowIndex];
        if (!Node.HasChildren)
            return false;

        Node.IsExpanded = !Node.IsExpanded;
        UpdateVisibleRows();
        ProjectionChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Returns collapsed row-axis node keys.
    /// </summary>
    /// <returns>The collapsed row-axis node keys.</returns>
    public IReadOnlyList<string> GetCollapsedRowKeys()
    {
        List<string> Result = new();
        AddCollapsedRowKeys(fRowRoot, Result);
        return Result;
    }
    /// <summary>
    /// Applies collapsed row-axis node keys.
    /// </summary>
    /// <param name="Keys">The collapsed row-axis node keys.</param>
    /// <returns>True if any expanded state changed; otherwise, false.</returns>
    public bool SetCollapsedRowKeys(IEnumerable<string> Keys)
    {
        HashSet<string> KeySet = new((Keys ?? Array.Empty<string>()).Where(Key => !string.IsNullOrWhiteSpace(Key)), StringComparer.Ordinal);
        bool Result = ApplyCollapsedRowKeys(fRowRoot, KeySet);
        if (!Result)
            return false;

        UpdateVisibleRows();
        ProjectionChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Expands all row-axis nodes.
    /// </summary>
    /// <returns>True if any expanded state changed; otherwise, false.</returns>
    public bool ExpandAllRows()
    {
        bool Result = SetRowExpandedRecursive(fRowRoot, true);
        if (!Result)
            return false;

        UpdateVisibleRows();
        ProjectionChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Collapses all row-axis nodes.
    /// </summary>
    /// <returns>True if any expanded state changed; otherwise, false.</returns>
    public bool CollapseAllRows()
    {
        bool Result = SetRowExpandedRecursive(fRowRoot, false);
        if (!Result)
            return false;

        UpdateVisibleRows();
        ProjectionChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Returns a value cell for a row item, column item, and measure.
    /// </summary>
    /// <param name="RowItem">The row axis item.</param>
    /// <param name="ColumnItem">The column axis item.</param>
    /// <param name="Measure">The measure.</param>
    /// <returns>The value cell, or null when no value exists.</returns>
    public PivotGridValueCell GetCell(PivotGridAxisItem RowItem, PivotGridAxisItem ColumnItem, PivotGridMeasure Measure)
    {
        if (RowItem == null || ColumnItem == null || Measure == null)
            return null;

        fCells.TryGetValue(CreateCellKey(RowItem.Key, ColumnItem.Key, Measure), out PivotGridValueCell Result);
        return Result;
    }
    /// <summary>
    /// Returns a row grand-total cell for a row item and measure.
    /// </summary>
    /// <param name="RowItem">The row axis item.</param>
    /// <param name="Measure">The measure.</param>
    /// <returns>The row total cell, or null when no value exists.</returns>
    public PivotGridValueCell GetRowTotalCell(PivotGridAxisItem RowItem, PivotGridMeasure Measure)
    {
        if (RowItem == null || Measure == null)
            return null;

        fRowTotalCells.TryGetValue(CreateTotalCellKey(RowItem.Key, Measure), out PivotGridValueCell Result);
        return Result;
    }
    /// <summary>
    /// Returns a column grand-total cell for a column item and measure.
    /// </summary>
    /// <param name="ColumnItem">The column axis item.</param>
    /// <param name="Measure">The measure.</param>
    /// <returns>The column total cell, or null when no value exists.</returns>
    public PivotGridValueCell GetColumnTotalCell(PivotGridAxisItem ColumnItem, PivotGridMeasure Measure)
    {
        if (ColumnItem == null || Measure == null)
            return null;

        fColumnTotalCells.TryGetValue(CreateTotalCellKey(ColumnItem.Key, Measure), out PivotGridValueCell Result);
        return Result;
    }
    /// <summary>
    /// Returns a grand-total cell for a measure.
    /// </summary>
    /// <param name="Measure">The measure.</param>
    /// <returns>The grand total cell, or null when no value exists.</returns>
    public PivotGridValueCell GetGrandTotalCell(PivotGridMeasure Measure)
    {
        if (Measure == null)
            return null;

        fGrandTotalCells.TryGetValue(Measure.Name, out PivotGridValueCell Result);
        return Result;
    }
    /// <summary>
    /// Toggles sorting for a row or column field.
    /// </summary>
    /// <param name="Role">The field role.</param>
    /// <param name="FieldName">The field name.</param>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ToggleSort(PivotGridFieldRole Role, string FieldName)
    {
        if ((Role != PivotGridFieldRole.Row && Role != PivotGridFieldRole.Column) || string.IsNullOrWhiteSpace(FieldName))
            return false;

        if (fSortRole != Role || !string.Equals(fSortFieldName, FieldName, StringComparison.OrdinalIgnoreCase))
        {
            fSortRole = Role;
            fSortFieldName = FieldName;
            fSortDirection = PivotGridSortDirection.Ascending;
        }
        else if (fSortDirection == PivotGridSortDirection.None)
        {
            fSortDirection = PivotGridSortDirection.Ascending;
        }
        else if (fSortDirection == PivotGridSortDirection.Ascending)
        {
            fSortDirection = PivotGridSortDirection.Descending;
        }
        else
        {
            fSortRole = PivotGridFieldRole.None;
            fSortFieldName = string.Empty;
            fSortDirection = PivotGridSortDirection.None;
        }

        Rebuild();
        SortingChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Sets sorting for a row or column field.
    /// </summary>
    /// <param name="Role">The field role.</param>
    /// <param name="FieldName">The field name.</param>
    /// <param name="Direction">The sort direction.</param>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool SetSort(PivotGridFieldRole Role, string FieldName, PivotGridSortDirection Direction)
    {
        if (Direction == PivotGridSortDirection.None)
            return ClearSort();
        if ((Role != PivotGridFieldRole.Row && Role != PivotGridFieldRole.Column) || string.IsNullOrWhiteSpace(FieldName))
            return false;
        if (fSortRole == Role
            && fSortDirection == Direction
            && string.Equals(fSortFieldName, FieldName, StringComparison.OrdinalIgnoreCase))
            return false;

        fSortRole = Role;
        fSortFieldName = FieldName;
        fSortDirection = Direction;
        Rebuild();
        SortingChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Clears active sorting.
    /// </summary>
    /// <returns>True if sorting changed; otherwise, false.</returns>
    public bool ClearSort()
    {
        if (fSortRole == PivotGridFieldRole.None && fSortDirection == PivotGridSortDirection.None && string.IsNullOrEmpty(fSortFieldName))
            return false;

        fSortRole = PivotGridFieldRole.None;
        fSortFieldName = string.Empty;
        fSortDirection = PivotGridSortDirection.None;
        Rebuild();
        SortingChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Sets a value-list filter for a source field.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <param name="Values">The accepted field values.</param>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool SetFieldFilter(string FieldName, IEnumerable<object> Values)
    {
        if (string.IsNullOrWhiteSpace(FieldName))
            return false;
        if (Values == null)
            return ClearFieldFilter(FieldName);

        HashSet<string> NewKeys = new(Values.Select(CreateValueKey));
        if (fFilters.TryGetValue(FieldName, out HashSet<string> CurrentKeys) && CurrentKeys.SetEquals(NewKeys))
            return false;

        fFilters[FieldName] = NewKeys;
        Rebuild();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Sets a value-list filter from accepted invariant value keys.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <param name="ValueKeys">The accepted invariant value keys.</param>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool SetFieldFilterKeys(string FieldName, IEnumerable<string> ValueKeys)
    {
        if (string.IsNullOrWhiteSpace(FieldName))
            return false;
        if (ValueKeys == null)
            return ClearFieldFilter(FieldName);

        HashSet<string> NewKeys = new(ValueKeys.Select(Value => Value ?? string.Empty));
        if (fFilters.TryGetValue(FieldName, out HashSet<string> CurrentKeys) && CurrentKeys.SetEquals(NewKeys))
            return false;

        fFilters[FieldName] = NewKeys;
        Rebuild();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Clears a value-list filter from a source field.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool ClearFieldFilter(string FieldName)
    {
        if (string.IsNullOrWhiteSpace(FieldName) || !fFilters.Remove(FieldName))
            return false;

        Rebuild();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Clears all value-list filters.
    /// </summary>
    /// <returns>True if filtering changed; otherwise, false.</returns>
    public bool ClearFilters()
    {
        if (fFilters.Count == 0)
            return false;

        fFilters.Clear();
        Rebuild();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }
    /// <summary>
    /// Returns true when a value is accepted by the source field filter.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <param name="Value">The field value.</param>
    /// <returns>True if the value is accepted; otherwise, false.</returns>
    public bool IsFilterValueAccepted(string FieldName, object Value)
    {
        return string.IsNullOrWhiteSpace(FieldName)
               || !fFilters.TryGetValue(FieldName, out HashSet<string> Keys)
               || Keys.Contains(CreateValueKey(Value));
    }
    /// <summary>
    /// Returns true when a field has a value-list filter.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <returns>True when the field has a filter; otherwise, false.</returns>
    public bool IsFieldFiltered(string FieldName)
    {
        return !string.IsNullOrWhiteSpace(FieldName) && fFilters.ContainsKey(FieldName);
    }
    /// <summary>
    /// Returns accepted invariant value keys for a filtered source field.
    /// </summary>
    /// <param name="FieldName">The source field name.</param>
    /// <returns>The accepted invariant value keys.</returns>
    public IReadOnlyList<string> GetFieldFilterKeys(string FieldName)
    {
        if (string.IsNullOrWhiteSpace(FieldName) || !fFilters.TryGetValue(FieldName, out HashSet<string> Keys))
            return Array.Empty<string>();

        return Keys.ToList();
    }

    // ● properties
    /// <summary>
    /// Gets the row axis fields.
    /// </summary>
    public ObservableCollection<PivotGridField> RowFields { get; } = new();
    /// <summary>
    /// Gets the column axis fields.
    /// </summary>
    public ObservableCollection<PivotGridField> ColumnFields { get; } = new();
    /// <summary>
    /// Gets the value measures.
    /// </summary>
    public ObservableCollection<PivotGridMeasure> Measures { get; } = new();
    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    public IPivotGridDataAdapter DataAdapter
    {
        get => fDataAdapter;
        set => SetDataAdapter(value);
    }
    /// <summary>
    /// Gets the projected row axis items.
    /// </summary>
    public IReadOnlyList<PivotGridAxisItem> RowItems => fRowItems;
    /// <summary>
    /// Gets the visible row-axis nodes.
    /// </summary>
    public IReadOnlyList<PivotGridAxisNode> VisibleRowNodes => fVisibleRowNodes;
    /// <summary>
    /// Gets the active sort role.
    /// </summary>
    public PivotGridFieldRole SortRole => fSortRole;
    /// <summary>
    /// Gets the active sort field name.
    /// </summary>
    public string SortFieldName => fSortFieldName;
    /// <summary>
    /// Gets the active sort direction.
    /// </summary>
    public PivotGridSortDirection SortDirection => fSortDirection;
    /// <summary>
    /// Gets a value indicating whether any source field filter is active.
    /// </summary>
    public bool HasFilters => fFilters.Count > 0;
    /// <summary>
    /// Gets a value indicating whether any row-axis node can be expanded.
    /// </summary>
    public bool CanExpandRows => HasExpandableRow(fRowRoot, true);
    /// <summary>
    /// Gets a value indicating whether any row-axis node can be collapsed.
    /// </summary>
    public bool CanCollapseRows => HasExpandableRow(fRowRoot, false);
    /// <summary>
    /// Gets the names of source fields with an active value-list filter.
    /// </summary>
    public IReadOnlyList<string> FilterFieldNames => fFilters.Keys.ToList();
    /// <summary>
    /// Gets the projected column axis items.
    /// </summary>
    public IReadOnlyList<PivotGridAxisItem> ColumnItems => fColumnItems;

    // ● events
    /// <summary>
    /// Occurs when the data adapter changes.
    /// </summary>
    public event EventHandler DataAdapterChanged;
    /// <summary>
    /// Occurs when the pivot projection changes.
    /// </summary>
    public event EventHandler ProjectionChanged;
    /// <summary>
    /// Occurs when sorting changes.
    /// </summary>
    public event EventHandler SortingChanged;
    /// <summary>
    /// Occurs when filtering changes.
    /// </summary>
    public event EventHandler FiltersChanged;
}
