// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Pivot.Tests;

/// <summary>
/// Tests the non-visual pivot grid engine behavior.
/// </summary>
public class PivotGridEngineTests
{
    // ● private methods
    List<PivotGridTestRow> CreateRows()
    {
        return new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m },
            new() { Region = "North", Quarter = "Q1", Salesperson = "Bianca", Amount = 15m },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m },
            new() { Region = "South", Quarter = "Q1", Salesperson = "Bianca", Amount = 7m },
        };
    }
    PivotGridEngine CreateEngine()
    {
        PivotGridEngine Result = new();
        Result.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Region), Header = "Region" });
        Result.ColumnFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Quarter), Header = "Quarter" });
        Result.Measures.Add(new PivotGridMeasure { Name = "Amount", Header = "Amount", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.Sum });
        Result.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(CreateRows());
        return Result;
    }

    // ● tests
    /// <summary>
    /// Verifies that the engine creates row and column axis items.
    /// </summary>
    [Fact]
    public void Rebuild_WithRowAndColumnFields_CreatesAxisItems()
    {
        PivotGridEngine Engine = CreateEngine();

        Assert.Equal(new[] { "North", "South" }, Engine.RowItems.Select(Item => Item.Text));
        Assert.Equal(new[] { "Q1", "Q2" }, Engine.ColumnItems.Select(Item => Item.Text));
    }
    /// <summary>
    /// Verifies that sum aggregation groups matching row and column axis values.
    /// </summary>
    [Fact]
    public void GetCell_WithSumMeasure_ReturnsAggregatedValue()
    {
        PivotGridEngine Engine = CreateEngine();
        PivotGridAxisItem North = Engine.RowItems.First(Item => Item.Text == "North");
        PivotGridAxisItem Q1 = Engine.ColumnItems.First(Item => Item.Text == "Q1");
        PivotGridValueCell Cell = Engine.GetCell(North, Q1, Engine.Measures[0]);

        Assert.NotNull(Cell);
        Assert.Equal(25m, Cell.Value);
    }
    /// <summary>
    /// Verifies that row, column, and grand totals are calculated.
    /// </summary>
    [Fact]
    public void Totals_WithSumMeasure_ReturnAggregatedValues()
    {
        PivotGridEngine Engine = CreateEngine();
        PivotGridAxisItem North = Engine.RowItems.First(Item => Item.Text == "North");
        PivotGridAxisItem Q1 = Engine.ColumnItems.First(Item => Item.Text == "Q1");
        PivotGridMeasure Measure = Engine.Measures[0];

        Assert.Equal(45m, Engine.GetRowTotalCell(North, Measure).Value);
        Assert.Equal(32m, Engine.GetColumnTotalCell(Q1, Measure).Value);
        Assert.Equal(52m, Engine.GetGrandTotalCell(Measure).Value);
    }
    /// <summary>
    /// Verifies extended numeric aggregate values.
    /// </summary>
    [Fact]
    public void GetCell_WithExtendedNumericAggregates_ReturnsAggregatedValues()
    {
        PivotGridEngine Engine = CreateEngine();
        Engine.Measures.Clear();
        Engine.Measures.Add(new PivotGridMeasure { Name = "Product", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.Product });
        Engine.Measures.Add(new PivotGridMeasure { Name = "Variance", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.Variance });
        Engine.Measures.Add(new PivotGridMeasure { Name = "VarianceP", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.VarianceP });
        Engine.Measures.Add(new PivotGridMeasure { Name = "StdDev", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.StdDev });
        Engine.Measures.Add(new PivotGridMeasure { Name = "StdDevP", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.StdDevP });
        Engine.Rebuild();
        PivotGridAxisItem North = Engine.RowItems.First(Item => Item.Text == "North");
        PivotGridAxisItem Q1 = Engine.ColumnItems.First(Item => Item.Text == "Q1");

        Assert.Equal(150m, Engine.GetCell(North, Q1, Engine.Measures[0]).Value);
        Assert.Equal(12.5m, Engine.GetCell(North, Q1, Engine.Measures[1]).Value);
        Assert.Equal(6.25m, Engine.GetCell(North, Q1, Engine.Measures[2]).Value);
        Assert.Equal(3.5355m, Math.Round((decimal)Engine.GetCell(North, Q1, Engine.Measures[3]).Value, 4));
        Assert.Equal(2.5m, Engine.GetCell(North, Q1, Engine.Measures[4]).Value);
    }
    /// <summary>
    /// Verifies default display format for decimal-heavy aggregates.
    /// </summary>
    [Fact]
    public void FormatValue_WithStatisticalAggregate_UsesTwoDecimalsByDefault()
    {
        PivotGridMeasure Measure = new()
        {
            AggregateKind = PivotGridAggregateKind.Variance
        };

        Assert.Equal((12.3456m).ToString("N2", CultureInfo.CurrentCulture), Measure.FormatValue(12.3456m));
    }
    /// <summary>
    /// Verifies distinct counting of non-empty values.
    /// </summary>
    [Fact]
    public void GetCell_WithCountDistinctMeasure_ReturnsDistinctValueCount()
    {
        PivotGridEngine Engine = CreateEngine();
        Engine.Measures.Clear();
        Engine.Measures.Add(new PivotGridMeasure { Name = "Salespeople", SourceFieldName = nameof(PivotGridTestRow.Salesperson), AggregateKind = PivotGridAggregateKind.CountDistinct });
        Engine.Rebuild();
        PivotGridAxisItem North = Engine.RowItems.First(Item => Item.Text == "North");
        PivotGridAxisItem Q1 = Engine.ColumnItems.First(Item => Item.Text == "Q1");

        Assert.Equal(2, Engine.GetCell(North, Q1, Engine.Measures[0]).Value);
        Assert.Equal(2, Engine.GetRowTotalCell(North, Engine.Measures[0]).Value);
    }
    /// <summary>
    /// Verifies that multiple row fields create an expandable row-axis tree with parent aggregates.
    /// </summary>
    [Fact]
    public void ToggleRowExpanded_WithNestedRows_CollapsesChildrenAndKeepsParentAggregate()
    {
        PivotGridEngine Engine = CreateEngine();
        Engine.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Salesperson), Header = "Salesperson" });
        PivotGridAxisNode NorthNode = Engine.VisibleRowNodes.First(Node => Node.Item.Text == "North");
        PivotGridAxisItem Q1 = Engine.ColumnItems.First(Item => Item.Text == "Q1");

        Assert.True(NorthNode.HasChildren);
        Assert.Contains(Engine.VisibleRowNodes, Node => Node.Item.Text == "Alex");
        Assert.Equal(25m, Engine.GetCell(NorthNode.Item, Q1, Engine.Measures[0]).Value);

        int NorthIndex = Engine.VisibleRowNodes.ToList().IndexOf(NorthNode);
        Assert.True(Engine.ToggleRowExpanded(NorthIndex));

        Assert.False(NorthNode.IsExpanded);
        Assert.DoesNotContain(Engine.VisibleRowNodes, Node => Node.Parent == NorthNode);
        Assert.Equal(25m, Engine.GetCell(NorthNode.Item, Q1, Engine.Measures[0]).Value);
    }
    /// <summary>
    /// Verifies collapsed row-axis keys can be saved and restored.
    /// </summary>
    [Fact]
    public void SetCollapsedRowKeys_WithSavedKeys_RestoresCollapsedRows()
    {
        PivotGridEngine Source = CreateEngine();
        Source.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Salesperson), Header = "Salesperson" });
        int NorthIndex = Source.VisibleRowNodes.ToList().FindIndex(Node => Node.Item.Text == "North");
        Assert.True(Source.ToggleRowExpanded(NorthIndex));
        IReadOnlyList<string> Keys = Source.GetCollapsedRowKeys();

        PivotGridEngine Target = CreateEngine();
        Target.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Salesperson), Header = "Salesperson" });
        Assert.True(Target.SetCollapsedRowKeys(Keys));

        PivotGridAxisNode NorthNode = Target.VisibleRowNodes.First(Node => Node.Item.Text == "North");
        Assert.False(NorthNode.IsExpanded);
        Assert.DoesNotContain(Target.VisibleRowNodes, Node => Node.Parent == NorthNode);
    }
    /// <summary>
    /// Verifies expanding and collapsing all row-axis nodes.
    /// </summary>
    [Fact]
    public void ExpandAllRowsAndCollapseAllRows_WithNestedRows_UpdatesVisibleRows()
    {
        PivotGridEngine Engine = CreateEngine();
        Engine.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Salesperson), Header = "Salesperson" });

        Assert.True(Engine.CanCollapseRows);
        Assert.True(Engine.CollapseAllRows());
        Assert.False(Engine.CanCollapseRows);
        Assert.True(Engine.CanExpandRows);
        Assert.Equal(new[] { "North", "South" }, Engine.VisibleRowNodes.Select(Node => Node.Item.Text));

        Assert.True(Engine.ExpandAllRows());
        Assert.True(Engine.CanCollapseRows);
        Assert.False(Engine.CanExpandRows);
        Assert.Contains(Engine.VisibleRowNodes, Node => Node.Item.Text == "Alex");
    }
    /// <summary>
    /// Verifies row field sort cycles and reorders row-axis siblings.
    /// </summary>
    [Fact]
    public void ToggleSort_WithRowField_CyclesAndSortsRows()
    {
        PivotGridEngine Engine = CreateEngine();

        Assert.True(Engine.ToggleSort(PivotGridFieldRole.Row, nameof(PivotGridTestRow.Region)));
        Assert.Equal(PivotGridSortDirection.Ascending, Engine.SortDirection);
        Assert.Equal(new[] { "North", "South" }, Engine.RowItems.Select(Item => Item.Text));

        Assert.True(Engine.ToggleSort(PivotGridFieldRole.Row, nameof(PivotGridTestRow.Region)));
        Assert.Equal(PivotGridSortDirection.Descending, Engine.SortDirection);
        Assert.Equal(new[] { "South", "North" }, Engine.RowItems.Select(Item => Item.Text));

        Assert.True(Engine.ToggleSort(PivotGridFieldRole.Row, nameof(PivotGridTestRow.Region)));
        Assert.Equal(PivotGridSortDirection.None, Engine.SortDirection);
    }
    /// <summary>
    /// Verifies column field sorting reorders column-axis items.
    /// </summary>
    [Fact]
    public void ToggleSort_WithColumnField_SortsColumns()
    {
        PivotGridEngine Engine = CreateEngine();

        Assert.True(Engine.ToggleSort(PivotGridFieldRole.Column, nameof(PivotGridTestRow.Quarter)));
        Assert.Equal(new[] { "Q1", "Q2" }, Engine.ColumnItems.Select(Item => Item.Text));

        Assert.True(Engine.ToggleSort(PivotGridFieldRole.Column, nameof(PivotGridTestRow.Quarter)));
        Assert.Equal(new[] { "Q2", "Q1" }, Engine.ColumnItems.Select(Item => Item.Text));
    }
    /// <summary>
    /// Verifies direct sort assignment and clearing.
    /// </summary>
    [Fact]
    public void SetSort_WithDirection_SortsAndClears()
    {
        PivotGridEngine Engine = CreateEngine();

        Assert.True(Engine.SetSort(PivotGridFieldRole.Column, nameof(PivotGridTestRow.Quarter), PivotGridSortDirection.Descending));
        Assert.Equal(PivotGridFieldRole.Column, Engine.SortRole);
        Assert.Equal(PivotGridSortDirection.Descending, Engine.SortDirection);
        Assert.Equal(new[] { "Q2", "Q1" }, Engine.ColumnItems.Select(Item => Item.Text));

        Assert.True(Engine.ClearSort());

        Assert.Equal(PivotGridFieldRole.None, Engine.SortRole);
        Assert.Equal(PivotGridSortDirection.None, Engine.SortDirection);
    }
    /// <summary>
    /// Verifies value-list filtering limits source rows used by the projection.
    /// </summary>
    [Fact]
    public void SetFieldFilter_WithAcceptedValues_FiltersProjection()
    {
        PivotGridEngine Engine = CreateEngine();

        Assert.True(Engine.SetFieldFilter(nameof(PivotGridTestRow.Region), new object[] { "North" }));

        Assert.True(Engine.HasFilters);
        Assert.True(Engine.IsFieldFiltered(nameof(PivotGridTestRow.Region)));
        Assert.Equal(new[] { "North" }, Engine.RowItems.Select(Item => Item.Text));
        PivotGridAxisItem North = Engine.RowItems.First(Item => Item.Text == "North");
        PivotGridAxisItem Q1 = Engine.ColumnItems.First(Item => Item.Text == "Q1");
        Assert.Equal(25m, Engine.GetCell(North, Q1, Engine.Measures[0]).Value);
    }
    /// <summary>
    /// Verifies clearing value-list filters restores the full projection.
    /// </summary>
    [Fact]
    public void ClearFilters_WithActiveFilters_RestoresProjection()
    {
        PivotGridEngine Engine = CreateEngine();
        Assert.True(Engine.SetFieldFilter(nameof(PivotGridTestRow.Region), new object[] { "North" }));

        Assert.True(Engine.ClearFilters());

        Assert.False(Engine.HasFilters);
        Assert.Equal(new[] { "North", "South" }, Engine.RowItems.Select(Item => Item.Text));
    }
    /// <summary>
    /// Verifies filter key export and import for persisted settings.
    /// </summary>
    [Fact]
    public void SetFieldFilterKeys_WithSavedKeys_RestoresFilter()
    {
        PivotGridEngine Source = CreateEngine();
        Assert.True(Source.SetFieldFilter(nameof(PivotGridTestRow.Region), new object[] { "North" }));
        IReadOnlyList<string> Keys = Source.GetFieldFilterKeys(nameof(PivotGridTestRow.Region));

        PivotGridEngine Target = CreateEngine();
        Assert.True(Target.SetFieldFilterKeys(nameof(PivotGridTestRow.Region), Keys));

        Assert.True(Target.HasFilters);
        Assert.Equal(new[] { "North" }, Target.RowItems.Select(Item => Item.Text));
    }
    /// <summary>
    /// Verifies that an empty accepted value list is an active filter that excludes all rows.
    /// </summary>
    [Fact]
    public void SetFieldFilter_WithNoAcceptedValues_ExcludesAllRows()
    {
        PivotGridEngine Engine = CreateEngine();

        Assert.True(Engine.SetFieldFilter(nameof(PivotGridTestRow.Region), Array.Empty<object>()));

        Assert.True(Engine.HasFilters);
        Assert.Empty(Engine.RowItems);
        Assert.Empty(Engine.ColumnItems);
    }
    /// <summary>
    /// Verifies that source field metadata ignores unsupported fields.
    /// </summary>
    [Fact]
    public void SourceFields_WithPocoAdapter_IgnoreUnsupportedFields()
    {
        PivotGridListDataAdapter<PivotGridTestRow> Adapter = new(CreateRows());

        Assert.Contains(Adapter.SourceFields, Field => Field.Name == nameof(PivotGridTestRow.Region) && Field.CanUseAsAxis);
        Assert.Contains(Adapter.SourceFields, Field => Field.Name == nameof(PivotGridTestRow.Amount) && Field.CanUseAsMeasure);
        Assert.DoesNotContain(Adapter.SourceFields, Field => Field.Name == nameof(PivotGridTestRow.Tags));
    }
    /// <summary>
    /// Verifies that a DataTable source is adapted through the same pivot contract.
    /// </summary>
    [Fact]
    public void DataAdapter_WithDataTable_ExposesFieldsAndValues()
    {
        DataTable Table = new("Sales");
        Table.Columns.Add("Region", typeof(string));
        Table.Columns.Add("Quarter", typeof(string));
        Table.Columns.Add("Amount", typeof(decimal));
        Table.Columns.Add("Payload", typeof(byte[]));
        Table.Rows.Add("North", "Q1", 10m, new byte[] { 1, 2 });

        PivotGridDataViewDataAdapter Adapter = new(Table.DefaultView);

        Assert.Contains(Adapter.SourceFields, Field => Field.Name == "Region" && Field.CanUseAsAxis);
        Assert.Contains(Adapter.SourceFields, Field => Field.Name == "Amount" && Field.CanUseAsMeasure);
        Assert.DoesNotContain(Adapter.SourceFields, Field => Field.Name == "Payload");
        Assert.Equal(10m, Adapter.GetValue(0, "Amount"));
    }
}
