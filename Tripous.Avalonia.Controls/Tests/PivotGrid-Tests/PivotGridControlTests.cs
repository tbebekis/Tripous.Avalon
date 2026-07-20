// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Pivot.Tests;

/// <summary>
/// Tests public pivot grid control APIs that do not require rendering.
/// </summary>
public class PivotGridControlTests
{
    // ● private methods
    PivotGrid CreateGrid()
    {
        PivotGrid Result = new();
        Result.RowFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Region), Header = "Region" });
        Result.ColumnFields.Add(new PivotGridField { Name = nameof(PivotGridTestRow.Quarter), Header = "Quarter" });
        Result.Measures.Add(new PivotGridMeasure { Name = "Amount", SourceFieldName = nameof(PivotGridTestRow.Amount), AggregateKind = PivotGridAggregateKind.Sum });
        Result.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
        });
        return Result;
    }
    double MatrixTop(PivotGrid Grid)
    {
        return (Grid.ShowFieldPanel ? Grid.LayoutMetrics.FieldPanelHeight : 0) + Grid.LayoutMetrics.AxisPanelHeight;
    }
    bool InvokeNavigation(PivotGrid Grid, string MethodName, params object[] Args)
    {
        MethodInfo Method = typeof(PivotGrid).GetMethod(MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(Method);
        return (bool)Method.Invoke(Grid, Args);
    }
    int InvokeInt(PivotGrid Grid, string MethodName)
    {
        MethodInfo Method = typeof(PivotGrid).GetMethod(MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(Method);
        return (int)Method.Invoke(Grid, Array.Empty<object>());
    }

    // ● tests
    /// <summary>
    /// Verifies that hit testing returns row header information.
    /// </summary>
    [Fact]
    public void HitTest_WithRowHeaderPoint_ReturnsRowHeader()
    {
        PivotGrid Grid = CreateGrid();

        PivotGridHitTestResult Hit = Grid.HitTest(new Point(10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));

        Assert.Equal(PivotGridHitTestKind.RowHeader, Hit.Kind);
        Assert.Equal(0, Hit.RowIndex);
        Assert.Equal("North", Hit.RowItem.Text);
    }
    /// <summary>
    /// Verifies that hit testing returns value cell information.
    /// </summary>
    [Fact]
    public void HitTest_WithValueCellPoint_ReturnsCell()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + 10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridHitTestKind.ValueCell, Hit.Kind);
        Assert.Equal(0, Hit.RowIndex);
        Assert.Equal(0, Hit.ColumnIndex);
        Assert.Equal(10m, Hit.Cell.Value);
    }
    /// <summary>
    /// Verifies that hit testing returns a measure resizer at a value column boundary.
    /// </summary>
    [Fact]
    public void HitTest_WithMeasureBoundaryPoint_ReturnsMeasureResizer()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + Grid.LayoutMetrics.ValueCellWidth, MatrixTop(Grid) + 4);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridHitTestKind.MeasureResizer, Hit.Kind);
        Assert.Equal(0, Hit.MeasureIndex);
        Assert.Same(Grid.Measures[0], Hit.Measure);
    }
    /// <summary>
    /// Verifies that hit testing returns a row header resizer at the row header boundary.
    /// </summary>
    [Fact]
    public void HitTest_WithRowHeaderBoundaryPoint_ReturnsRowHeaderResizer()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridHitTestKind.RowHeaderResizer, Hit.Kind);
    }
    /// <summary>
    /// Verifies hiding the field panel moves the axis panel to the top.
    /// </summary>
    [Fact]
    public void HitTest_WithHiddenFieldPanel_UsesTopAxisPanel()
    {
        PivotGrid Grid = CreateGrid();
        Grid.ShowFieldPanel = false;
        Point Point = new(90, 10);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridBand.AxisPanel, Hit.Band);
        Assert.Equal(PivotGridHitTestKind.MeasureField, Hit.Kind);
    }
    /// <summary>
    /// Verifies tooltip text for row headers.
    /// </summary>
    [Fact]
    public void GetToolTipText_WithRowHeaderPoint_ReturnsRowHeaderText()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4);

        Assert.Equal("North", Grid.GetToolTipText(Point));
    }
    /// <summary>
    /// Verifies tooltip text for column headers.
    /// </summary>
    [Fact]
    public void GetToolTipText_WithColumnHeaderPoint_ReturnsColumnHeaderText()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + 10, MatrixTop(Grid) + 4);

        Assert.Equal("Q1", Grid.GetToolTipText(Point));
    }
    /// <summary>
    /// Verifies tooltip text for value cells.
    /// </summary>
    [Fact]
    public void GetToolTipText_WithValueCellPoint_ReturnsValueContextText()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + 10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4);

        string Text = Grid.GetToolTipText(Point);

        Assert.Contains("North", Text);
        Assert.Contains("Q1", Text);
        Assert.Contains("Amount (Sum)", Text);
        Assert.Contains("10", Text);
    }
    /// <summary>
    /// Verifies tooltip text can be disabled.
    /// </summary>
    [Fact]
    public void GetToolTipText_WithShowToolTipsFalse_ReturnsEmptyText()
    {
        PivotGrid Grid = CreateGrid();
        Grid.ShowToolTips = false;
        Point Point = new(Grid.ActualRowHeaderWidth + 10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4);

        Assert.Equal(string.Empty, Grid.GetToolTipText(Point));
    }
    /// <summary>
    /// Verifies changing a measure width through the public API.
    /// </summary>
    [Fact]
    public void SetMeasureWidth_WithValidIndex_UpdatesWidth()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.SetMeasureWidth(0, 160));
        Assert.Equal(160, Grid.Measures[0].Width);

        Assert.True(Grid.SetMeasureWidth(0, 10));
        Assert.Equal(40, Grid.Measures[0].Width);
        Assert.False(Grid.SetMeasureWidth(5, 120));
    }
    /// <summary>
    /// Verifies changing the row header width through the public API.
    /// </summary>
    [Fact]
    public void SetRowHeaderWidth_WithSmallWidth_ClampsToMinimum()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.SetRowHeaderWidth(80));
        Assert.Equal(120, Grid.LayoutMetrics.RowHeaderWidth);
        Assert.False(Grid.SetRowHeaderWidth(120));
    }
    /// <summary>
    /// Verifies resetting the row header width through the public API.
    /// </summary>
    [Fact]
    public void ResetRowHeaderWidth_WithManualWidth_RestoresDefaultWidth()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.SetRowHeaderWidth(360));
        Assert.True(Grid.ResetRowHeaderWidth());

        Assert.Equal(new PivotGridLayoutMetrics().RowHeaderWidth, Grid.LayoutMetrics.RowHeaderWidth);
        Assert.Equal(Grid.LayoutMetrics.RowHeaderWidth, Grid.CreateSettings().RowHeaderWidth);
        Assert.False(Grid.ResetRowHeaderWidth());
    }
    /// <summary>
    /// Verifies changing a measure aggregate recalculates pivot values.
    /// </summary>
    [Fact]
    public void SetMeasureAggregate_WithValidKind_RecalculatesValues()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North", Quarter = "Q1", Salesperson = "Bianca", Amount = 20m, Units = 4 },
        });

        Assert.True(Grid.SetMeasureAggregate(0, PivotGridAggregateKind.Average));

        Assert.Equal(PivotGridAggregateKind.Average, Grid.Measures[0].AggregateKind);
        Assert.Equal(15m, Grid.Engine.GetCell(Grid.Engine.RowItems[0], Grid.Engine.ColumnItems[0], Grid.Measures[0]).Value);
        Assert.False(Grid.SetMeasureAggregate(0, PivotGridAggregateKind.Average));
        Assert.False(Grid.SetMeasureAggregate(4, PivotGridAggregateKind.Sum));
    }
    /// <summary>
    /// Verifies changing a single visible value column width without changing all columns for the measure.
    /// </summary>
    [Fact]
    public void SetValueColumnWidth_WithVisibleColumn_UpdatesOnlyThatColumn()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
        });

        Assert.True(Grid.SetValueColumnWidth(0, 0, 160));
        Assert.Equal(110, Grid.Measures[0].Width);

        PivotGridHitTestResult FirstHit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 150, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));
        PivotGridHitTestResult SecondHit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 164, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));

        Assert.Equal(0, FirstHit.ColumnIndex);
        Assert.Equal("Q1", FirstHit.ColumnItem.Text);
        Assert.Equal(1, SecondHit.ColumnIndex);
        Assert.Equal("Q2", SecondHit.ColumnItem.Text);
    }
    /// <summary>
    /// Verifies shrinking a visible value column keeps following columns aligned.
    /// </summary>
    [Fact]
    public void SetValueColumnWidth_WithNarrowVisibleColumn_KeepsFollowingColumnAligned()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
        });

        Assert.True(Grid.SetValueColumnWidth(0, 0, 40));
        PivotGridHitTestResult FirstHit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 36, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));
        PivotGridHitTestResult SecondHit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 44, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));
        PivotGridHitTestResult SecondHeaderHit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 50, MatrixTop(Grid) + 4));

        Assert.Equal(0, FirstHit.ColumnIndex);
        Assert.Equal("Q1", FirstHit.ColumnItem.Text);
        Assert.Equal(1, SecondHit.ColumnIndex);
        Assert.Equal("Q2", SecondHit.ColumnItem.Text);
        Assert.Equal(1, SecondHeaderHit.ColumnIndex);
        Assert.Equal("Q2", SecondHeaderHit.ColumnItem.Text);
    }
    /// <summary>
    /// Verifies auto-fitting visible value columns creates width overrides.
    /// </summary>
    [Fact]
    public void AutoFitValueColumnWidths_WithVisibleColumns_CreatesWidthOverrides()
    {
        PivotGrid Grid = CreateGrid();
        Grid.Measures[0].Header = "Very Long Amount Header";
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 1000000m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
        });

        Assert.True(Grid.AutoFitValueColumnWidths());
        PivotGridSettings Settings = Grid.CreateSettings();

        Assert.Equal(3, Settings.ValueColumnWidths.Count);
        Assert.All(Settings.ValueColumnWidths.Values, Width => Assert.True(Width >= 40));
        Assert.False(Grid.AutoFitValueColumnWidths());
    }
    /// <summary>
    /// Verifies auto-fitting a single visible value column creates one width override.
    /// </summary>
    [Fact]
    public void AutoFitValueColumnWidth_WithVisibleColumn_CreatesSingleWidthOverride()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 1000000m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
        });

        Assert.True(Grid.AutoFitValueColumnWidth(0, 0));

        Assert.Single(Grid.CreateSettings().ValueColumnWidths);
        Assert.False(Grid.AutoFitValueColumnWidth(8, 0));
    }
    /// <summary>
    /// Verifies auto-fitting a single total value column creates one width override.
    /// </summary>
    [Fact]
    public void AutoFitValueColumnWidth_WithTotalColumn_CreatesSingleWidthOverride()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.AutoFitValueColumnWidth(Grid.Engine.ColumnItems.Count, 0));

        Assert.Single(Grid.CreateSettings().ValueColumnWidths);
    }
    /// <summary>
    /// Verifies auto-fitting value columns includes long column header text.
    /// </summary>
    [Fact]
    public void AutoFitValueColumnWidths_WithLongColumnHeader_FitsHeader()
    {
        PivotGrid Grid = CreateGrid();
        string LongQuarter = "Quarter with a very long caption that should not be clipped by auto fit";
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = LongQuarter, Salesperson = "Alex", Amount = 10m, Units = 2 },
        });

        Assert.True(Grid.AutoFitValueColumnWidths());
        double Width = Grid.CreateSettings().ValueColumnWidths.Values.First();

        Assert.True(Width > 360);
    }
    /// <summary>
    /// Verifies auto-fitting the row header uses visible row-axis content.
    /// </summary>
    [Fact]
    public void AutoFitRowHeaderWidth_WithLongVisibleRows_UpdatesRowHeaderWidth()
    {
        PivotGrid Grid = CreateGrid();
        Grid.LayoutMetrics.RowHeaderWidth = 120;
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "A very long sales region name", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
        });

        Assert.True(Grid.AutoFitRowHeaderWidth());

        Assert.True(Grid.LayoutMetrics.RowHeaderWidth > 120);
        Assert.Equal(Grid.LayoutMetrics.RowHeaderWidth, Grid.CreateSettings().RowHeaderWidth);
        Assert.False(Grid.AutoFitRowHeaderWidth());
    }
    /// <summary>
    /// Verifies auto-fitting the row header can shrink a manually widened row header.
    /// </summary>
    [Fact]
    public void AutoFitRowHeaderWidth_WithWideManualWidth_ShrinksRowHeaderWidth()
    {
        PivotGrid Grid = CreateGrid();
        Grid.LayoutMetrics.RowHeaderWidth = 600;

        Assert.True(Grid.AutoFitRowHeaderWidth());

        Assert.True(Grid.LayoutMetrics.RowHeaderWidth < 600);
        Assert.Equal(Grid.LayoutMetrics.RowHeaderWidth, Grid.CreateSettings().RowHeaderWidth);
    }
    /// <summary>
    /// Verifies clearing visible value column width overrides.
    /// </summary>
    [Fact]
    public void ClearValueColumnWidths_WithOverrides_RestoresDefaultWidths()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
        });
        Assert.True(Grid.SetValueColumnWidth(0, 0, 40));

        Assert.True(Grid.ClearValueColumnWidths());
        PivotGridHitTestResult Hit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 50, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));

        Assert.Equal(0, Hit.ColumnIndex);
        Assert.False(Grid.ClearValueColumnWidths());
    }
    /// <summary>
    /// Verifies setting and clearing the current value cell.
    /// </summary>
    [Fact]
    public void CurrentCell_WithValidCell_UpdatesIndexesAndRaisesEvent()
    {
        PivotGrid Grid = CreateGrid();
        int ChangeCount = 0;
        Grid.CurrentCellChanged += (Sender, Args) => ChangeCount++;

        Assert.True(Grid.SetCurrentCell(0, 0, 0));
        Assert.Equal(0, Grid.CurrentRowIndex);
        Assert.Equal(0, Grid.CurrentColumnIndex);
        Assert.Equal(0, Grid.CurrentMeasureIndex);
        Assert.Equal(1, ChangeCount);

        Assert.False(Grid.SetCurrentCell(2, 0, 0));
        Assert.Equal(1, ChangeCount);

        Assert.True(Grid.ClearCurrentCell());
        Assert.Equal(-1, Grid.CurrentRowIndex);
        Assert.Equal(-1, Grid.CurrentColumnIndex);
        Assert.Equal(-1, Grid.CurrentMeasureIndex);
        Assert.Equal(2, ChangeCount);
    }
    /// <summary>
    /// Verifies setting the current cell to total cells.
    /// </summary>
    [Fact]
    public void CurrentCell_WithTotalCells_UpdatesIndexes()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.SetCurrentCell(0, 1, 0));
        Assert.Equal(0, Grid.CurrentRowIndex);
        Assert.Equal(1, Grid.CurrentColumnIndex);

        Assert.True(Grid.SetCurrentCell(1, 0, 0));
        Assert.Equal(1, Grid.CurrentRowIndex);
        Assert.Equal(0, Grid.CurrentColumnIndex);

        Assert.True(Grid.SetCurrentCell(1, 1, 0));
        Assert.Equal(1, Grid.CurrentRowIndex);
        Assert.Equal(1, Grid.CurrentColumnIndex);
    }
    /// <summary>
    /// Verifies setting the current cell scrolls a far total cell into view.
    /// </summary>
    [Fact]
    public void SetCurrentCell_WithFarTotalCell_ScrollsCellIntoView()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "R1", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "R2", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "R3", Quarter = "Q3", Salesperson = "Alex", Amount = 30m, Units = 4 },
            new() { Region = "R4", Quarter = "Q4", Salesperson = "Alex", Amount = 40m, Units = 5 },
            new() { Region = "R5", Quarter = "Q5", Salesperson = "Alex", Amount = 50m, Units = 6 },
            new() { Region = "R6", Quarter = "Q6", Salesperson = "Alex", Amount = 60m, Units = 7 },
        });
        Grid.Measure(new Size(260, 180));
        Grid.Arrange(new Rect(0, 0, 260, 180));

        Assert.True(Grid.SetCurrentCell(Grid.Engine.VisibleRowNodes.Count, Grid.Engine.ColumnItems.Count, 0));

        Assert.True(Grid.VerticalOffset > 0);
        Assert.True(Grid.HorizontalOffset > 0);
    }
    /// <summary>
    /// Verifies the public scroll-into-view method restores visibility for the current cell.
    /// </summary>
    [Fact]
    public void ScrollCurrentCellIntoView_WithScrolledAwayCurrentCell_RestoresOffsets()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "R1", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "R2", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "R3", Quarter = "Q3", Salesperson = "Alex", Amount = 30m, Units = 4 },
            new() { Region = "R4", Quarter = "Q4", Salesperson = "Alex", Amount = 40m, Units = 5 },
            new() { Region = "R5", Quarter = "Q5", Salesperson = "Alex", Amount = 50m, Units = 6 },
            new() { Region = "R6", Quarter = "Q6", Salesperson = "Alex", Amount = 60m, Units = 7 },
        });
        Grid.Measure(new Size(260, 180));
        Grid.Arrange(new Rect(0, 0, 260, 180));
        Assert.True(Grid.SetCurrentCell(0, 0, 0));
        Assert.True(Grid.SetVerticalOffset(200));
        Assert.True(Grid.SetHorizontalOffset(200));

        Assert.True(Grid.ScrollCurrentCellIntoView());

        Assert.Equal(0, Grid.VerticalOffset);
        Assert.Equal(0, Grid.HorizontalOffset);
    }
    /// <summary>
    /// Verifies that current cell text is exposed for copying.
    /// </summary>
    [Fact]
    public void CurrentCellText_WithValueAndTotalCells_ReturnsDisplayText()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.SetCurrentCell(0, 0, 0));
        Assert.Equal("10", Grid.CurrentCellText);

        Assert.True(Grid.SetCurrentCell(1, 1, 0));
        Assert.Equal("10", Grid.CurrentCellText);
    }
    /// <summary>
    /// Verifies expanding and collapsing rows through the control API.
    /// </summary>
    [Fact]
    public void ExpandAllRowsAndCollapseAllRows_WithNestedRows_UpdatesEngine()
    {
        PivotGrid Grid = CreateGrid();
        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Row));

        Assert.True(Grid.CollapseAllRows());
        Assert.False(Grid.Engine.CanCollapseRows);
        Assert.True(Grid.Engine.CanExpandRows);

        Assert.True(Grid.ExpandAllRows());
        Assert.True(Grid.Engine.CanCollapseRows);
    }
    /// <summary>
    /// Verifies keyboard navigation helper behavior across row and grid boundaries.
    /// </summary>
    [Fact]
    public void KeyboardNavigation_WithMultipleCells_MovesToExpectedCells()
    {
        PivotGrid Grid = CreateGrid();
        Grid.Measures.Add(new PivotGridMeasure { Name = "Units", Header = "Units", SourceFieldName = nameof(PivotGridTestRow.Units), AggregateKind = PivotGridAggregateKind.Sum });
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "South", Quarter = "Q1", Salesperson = "Bianca", Amount = 7m, Units = 1 },
            new() { Region = "South", Quarter = "Q2", Salesperson = "Bianca", Amount = 8m, Units = 4 },
        });

        Assert.True(Grid.SetCurrentCell(0, 0, 0));
        Assert.True(InvokeNavigation(Grid, "MoveCurrentCellToRowEnd"));
        Assert.Equal(0, Grid.CurrentRowIndex);
        Assert.Equal(2, Grid.CurrentColumnIndex);
        Assert.Equal(1, Grid.CurrentMeasureIndex);

        Assert.True(InvokeNavigation(Grid, "MoveCurrentCellToRowStart"));
        Assert.Equal(0, Grid.CurrentColumnIndex);
        Assert.Equal(0, Grid.CurrentMeasureIndex);

        Assert.True(InvokeNavigation(Grid, "MoveCurrentCellToGridEnd"));
        Assert.Equal(2, Grid.CurrentRowIndex);
        Assert.Equal(2, Grid.CurrentColumnIndex);
        Assert.Equal(1, Grid.CurrentMeasureIndex);

        Assert.True(InvokeNavigation(Grid, "MoveCurrentCellToGridStart"));
        Assert.Equal(0, Grid.CurrentRowIndex);
        Assert.Equal(0, Grid.CurrentColumnIndex);
        Assert.Equal(0, Grid.CurrentMeasureIndex);
    }
    /// <summary>
    /// Verifies page navigation uses the arranged body height.
    /// </summary>
    [Fact]
    public void KeyboardPageNavigation_WithArrangedGrid_MovesByVisiblePage()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "R1", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "R2", Quarter = "Q1", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "R3", Quarter = "Q1", Salesperson = "Alex", Amount = 30m, Units = 4 },
            new() { Region = "R4", Quarter = "Q1", Salesperson = "Alex", Amount = 40m, Units = 5 },
            new() { Region = "R5", Quarter = "Q1", Salesperson = "Alex", Amount = 50m, Units = 6 },
            new() { Region = "R6", Quarter = "Q1", Salesperson = "Alex", Amount = 60m, Units = 7 },
        });
        Grid.Measure(new Size(360, 220));
        Grid.Arrange(new Rect(0, 0, 360, 220));

        Assert.True(Grid.SetCurrentCell(0, 0, 0));
        int PageRows = InvokeInt(Grid, "GetKeyboardPageRowCount");
        Assert.True(InvokeNavigation(Grid, "MoveCurrentCellPage", 1));

        Assert.Equal(PageRows, Grid.CurrentRowIndex);
        Assert.True(InvokeNavigation(Grid, "MoveCurrentCellPage", -1));
        Assert.Equal(0, Grid.CurrentRowIndex);
    }
    /// <summary>
    /// Verifies that hit testing returns the row total cell.
    /// </summary>
    [Fact]
    public void HitTest_WithRowTotalCellPoint_ReturnsTotalCell()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + Grid.LayoutMetrics.ValueCellWidth + 4, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridHitTestKind.ValueCell, Hit.Kind);
        Assert.Equal(0, Hit.RowIndex);
        Assert.Equal(1, Hit.ColumnIndex);
        Assert.Equal(10m, Hit.Cell.Value);
    }
    /// <summary>
    /// Verifies that hit testing returns the column total cell.
    /// </summary>
    [Fact]
    public void HitTest_WithColumnTotalCellPoint_ReturnsTotalCell()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + 10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + Grid.LayoutMetrics.RowHeight + 4);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridHitTestKind.ValueCell, Hit.Kind);
        Assert.Equal(1, Hit.RowIndex);
        Assert.Equal(0, Hit.ColumnIndex);
        Assert.Equal(10m, Hit.Cell.Value);
    }
    /// <summary>
    /// Verifies that hit testing returns the grand total cell.
    /// </summary>
    [Fact]
    public void HitTest_WithGrandTotalCellPoint_ReturnsTotalCell()
    {
        PivotGrid Grid = CreateGrid();
        Point Point = new(Grid.ActualRowHeaderWidth + Grid.LayoutMetrics.ValueCellWidth + 4, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + Grid.LayoutMetrics.RowHeight + 4);

        PivotGridHitTestResult Hit = Grid.HitTest(Point);

        Assert.Equal(PivotGridHitTestKind.ValueCell, Hit.Kind);
        Assert.Equal(1, Hit.RowIndex);
        Assert.Equal(1, Hit.ColumnIndex);
        Assert.Equal(10m, Hit.Cell.Value);
    }
    /// <summary>
    /// Verifies that horizontal scroll offset participates in value-cell hit testing.
    /// </summary>
    [Fact]
    public void HitTest_WithHorizontalOffset_ReturnsScrolledColumnCell()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "North", Quarter = "Q2", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "North", Quarter = "Q3", Salesperson = "Alex", Amount = 30m, Units = 4 },
        });
        Grid.Measure(new Size(260, 220));
        Grid.Arrange(new Rect(0, 0, 260, 220));

        Assert.True(Grid.SetHorizontalOffset(Grid.LayoutMetrics.ValueCellWidth));
        PivotGridHitTestResult Hit = Grid.HitTest(new Point(Grid.ActualRowHeaderWidth + 10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));

        Assert.Equal(PivotGridHitTestKind.ValueCell, Hit.Kind);
        Assert.Equal(1, Hit.ColumnIndex);
        Assert.Equal("Q2", Hit.ColumnItem.Text);
    }
    /// <summary>
    /// Verifies that vertical scroll offset participates in row-header hit testing.
    /// </summary>
    [Fact]
    public void HitTest_WithVerticalOffset_ReturnsScrolledRowHeader()
    {
        PivotGrid Grid = CreateGrid();
        Grid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "R1", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "R2", Quarter = "Q1", Salesperson = "Alex", Amount = 20m, Units = 3 },
            new() { Region = "R3", Quarter = "Q1", Salesperson = "Alex", Amount = 30m, Units = 4 },
            new() { Region = "R4", Quarter = "Q1", Salesperson = "Alex", Amount = 40m, Units = 5 },
            new() { Region = "R5", Quarter = "Q1", Salesperson = "Alex", Amount = 50m, Units = 6 },
            new() { Region = "R6", Quarter = "Q1", Salesperson = "Alex", Amount = 60m, Units = 7 },
        });
        Grid.Measure(new Size(360, 180));
        Grid.Arrange(new Rect(0, 0, 360, 180));

        Assert.True(Grid.SetVerticalOffset(Grid.LayoutMetrics.RowHeight));
        PivotGridHitTestResult Hit = Grid.HitTest(new Point(10, MatrixTop(Grid) + Grid.LayoutMetrics.ColumnHeaderHeight + 4));

        Assert.Equal(PivotGridHitTestKind.RowHeader, Hit.Kind);
        Assert.Equal(1, Hit.RowIndex);
        Assert.Equal("R2", Hit.RowItem.Text);
    }
    /// <summary>
    /// Verifies that available fields exclude fields already used by axes and measures.
    /// </summary>
    [Fact]
    public void AvailableFields_WithUsedFields_ExcludesUsedFieldNames()
    {
        PivotGrid Grid = CreateGrid();

        Assert.DoesNotContain(Grid.AvailableFields, Field => Field.Name == nameof(PivotGridTestRow.Region));
        Assert.DoesNotContain(Grid.AvailableFields, Field => Field.Name == nameof(PivotGridTestRow.Quarter));
        Assert.DoesNotContain(Grid.AvailableFields, Field => Field.Name == nameof(PivotGridTestRow.Amount));
        Assert.Contains(Grid.AvailableFields, Field => Field.Name == nameof(PivotGridTestRow.Salesperson));
    }
    /// <summary>
    /// Verifies that assigning DataTable through ItemsSource creates a default projection.
    /// </summary>
    [Fact]
    public void ItemsSource_WithDataTable_CreatesAdapterAndDefaultLayout()
    {
        DataTable Table = new("Sales");
        Table.Columns.Add("Region", typeof(string));
        Table.Columns.Add("Quarter", typeof(string));
        Table.Columns.Add("Amount", typeof(decimal));
        Table.Rows.Add("North", "Q1", 10m);

        PivotGrid Grid = new()
        {
            ItemsSource = Table,
        };

        Assert.NotNull(Grid.DataAdapter);
        Assert.NotEmpty(Grid.SourceFields);
        Assert.NotEmpty(Grid.RowFields);
        Assert.NotEmpty(Grid.ColumnFields);
        Assert.NotEmpty(Grid.Measures);
        Assert.Equal(10m, Grid.Engine.GetCell(Grid.Engine.RowItems[0], Grid.Engine.ColumnItems[0], Grid.Measures[0]).Value);
    }
    /// <summary>
    /// Verifies that hit testing returns available field information.
    /// </summary>
    [Fact]
    public void HitTest_WithAvailableFieldPoint_ReturnsAvailableField()
    {
        PivotGrid Grid = CreateGrid();

        PivotGridHitTestResult Hit = Grid.HitTest(new Point(136, 10));

        Assert.Equal(PivotGridHitTestKind.AvailableField, Hit.Kind);
        Assert.Equal(nameof(PivotGridTestRow.Salesperson), Hit.SourceField.Name);
    }
    /// <summary>
    /// Verifies moving a field from available fields to the row axis.
    /// </summary>
    [Fact]
    public void MoveField_WithAvailableFieldToRows_MovesField()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Row));

        Assert.Contains(Grid.RowFields, Field => Field.Name == nameof(PivotGridTestRow.Salesperson));
        Assert.DoesNotContain(Grid.AvailableFields, Field => Field.Name == nameof(PivotGridTestRow.Salesperson));
    }
    /// <summary>
    /// Verifies moving a field between axes.
    /// </summary>
    [Fact]
    public void MoveField_WithRowFieldToColumns_MovesFieldBetweenAxes()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Region), PivotGridFieldRole.Column));

        Assert.DoesNotContain(Grid.RowFields, Field => Field.Name == nameof(PivotGridTestRow.Region));
        Assert.Contains(Grid.ColumnFields, Field => Field.Name == nameof(PivotGridTestRow.Region));
    }
    /// <summary>
    /// Verifies that a numeric source field can move to values.
    /// </summary>
    [Fact]
    public void MoveField_WithNumericFieldToValues_AddsMeasure()
    {
        PivotGrid Grid = CreateGrid();
        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Amount), PivotGridFieldRole.Available));

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Amount), PivotGridFieldRole.Measure));

        Assert.Contains(Grid.Measures, Measure => Measure.SourceFieldName == nameof(PivotGridTestRow.Amount));
    }
    /// <summary>
    /// Verifies that a non-numeric source field cannot move to values.
    /// </summary>
    [Fact]
    public void MoveField_WithTextFieldToValues_DoesNotMove()
    {
        PivotGrid Grid = CreateGrid();

        Assert.False(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Measure));

        Assert.DoesNotContain(Grid.Measures, Measure => Measure.SourceFieldName == nameof(PivotGridTestRow.Salesperson));
    }
    /// <summary>
    /// Verifies moving an available row field to a specific insertion index.
    /// </summary>
    [Fact]
    public void MoveField_WithAvailableFieldToRowsAtIndex_InsertsField()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Row, 0));

        Assert.Equal(nameof(PivotGridTestRow.Salesperson), Grid.RowFields[0].Name);
        Assert.Equal(nameof(PivotGridTestRow.Region), Grid.RowFields[1].Name);
    }
    /// <summary>
    /// Verifies reordering fields inside the row axis.
    /// </summary>
    [Fact]
    public void MoveField_WithRowFieldToRowIndex_ReordersFields()
    {
        PivotGrid Grid = CreateGrid();
        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Row));

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Region), PivotGridFieldRole.Row, 2));

        Assert.Equal(nameof(PivotGridTestRow.Salesperson), Grid.RowFields[0].Name);
        Assert.Equal(nameof(PivotGridTestRow.Region), Grid.RowFields[1].Name);
    }
    /// <summary>
    /// Verifies reordering fields inside the column axis.
    /// </summary>
    [Fact]
    public void MoveField_WithColumnFieldToColumnIndex_ReordersFields()
    {
        PivotGrid Grid = CreateGrid();
        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Column));

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Column, 0));

        Assert.Equal(nameof(PivotGridTestRow.Salesperson), Grid.ColumnFields[0].Name);
        Assert.Equal(nameof(PivotGridTestRow.Quarter), Grid.ColumnFields[1].Name);
    }
    /// <summary>
    /// Verifies inserting numeric fields inside the value area.
    /// </summary>
    [Fact]
    public void MoveField_WithNumericFieldToMeasureIndex_InsertsMeasure()
    {
        PivotGrid Grid = CreateGrid();

        Assert.True(Grid.MoveField(nameof(PivotGridTestRow.Units), PivotGridFieldRole.Measure, 0));

        Assert.Equal(nameof(PivotGridTestRow.Units), Grid.Measures[0].SourceFieldName);
        Assert.Equal(nameof(PivotGridTestRow.Amount), Grid.Measures[1].SourceFieldName);
    }
    /// <summary>
    /// Verifies saving and loading pivot layout settings.
    /// </summary>
    [Fact]
    public void SaveSettings_ThenLoadSettings_RestoresLayout()
    {
        PivotGrid SourceGrid = CreateGrid();
        SourceGrid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
        {
            new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
            new() { Region = "South", Quarter = "Q1", Salesperson = "Bianca", Amount = 20m, Units = 4 },
        });
        Assert.True(SourceGrid.MoveField(nameof(PivotGridTestRow.Salesperson), PivotGridFieldRole.Row));
        SourceGrid.Measures[0].AggregateKind = PivotGridAggregateKind.Average;
        SourceGrid.Measures[0].DisplayFormat = "N1";
        SourceGrid.Measures[0].Width = 145;
        SourceGrid.ShowFieldPanel = false;
        SourceGrid.ShowRowGrandTotals = false;
        SourceGrid.ShowColumnGrandTotals = false;
        SourceGrid.ShowToolTips = false;
        SourceGrid.LayoutMetrics.RowHeaderWidth = 260;
        Assert.True(SourceGrid.SetValueColumnWidth(0, 0, 180));
        Assert.True(SourceGrid.SetSort(PivotGridFieldRole.Row, nameof(PivotGridTestRow.Region), PivotGridSortDirection.Descending));
        Assert.True(SourceGrid.SetFieldFilter(nameof(PivotGridTestRow.Region), new object[] { "North" }));
        int NorthIndex = SourceGrid.Engine.VisibleRowNodes.ToList().FindIndex(Node => Node.Item.Text == "North");
        Assert.True(SourceGrid.Engine.ToggleRowExpanded(NorthIndex));
        string FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");

        try
        {
            SourceGrid.SaveSettings(FilePath);
            PivotGrid TargetGrid = CreateGrid();
            TargetGrid.DataAdapter = new PivotGridListDataAdapter<PivotGridTestRow>(new List<PivotGridTestRow>
            {
                new() { Region = "North", Quarter = "Q1", Salesperson = "Alex", Amount = 10m, Units = 2 },
                new() { Region = "South", Quarter = "Q1", Salesperson = "Bianca", Amount = 20m, Units = 4 },
            });
            Assert.True(TargetGrid.LoadSettings(FilePath));

            Assert.Equal(new[] { nameof(PivotGridTestRow.Region), nameof(PivotGridTestRow.Salesperson) }, TargetGrid.RowFields.Select(Field => Field.Name));
            Assert.Equal(new[] { nameof(PivotGridTestRow.Quarter) }, TargetGrid.ColumnFields.Select(Field => Field.Name));
            Assert.Equal(PivotGridAggregateKind.Average, TargetGrid.Measures[0].AggregateKind);
            Assert.Equal("N1", TargetGrid.Measures[0].DisplayFormat);
            Assert.Equal(145, TargetGrid.Measures[0].Width);
            Assert.False(TargetGrid.ShowFieldPanel);
            Assert.False(TargetGrid.ShowRowGrandTotals);
            Assert.False(TargetGrid.ShowColumnGrandTotals);
            Assert.False(TargetGrid.ShowToolTips);
            Assert.Equal(260, TargetGrid.LayoutMetrics.RowHeaderWidth);
            PivotGridHitTestResult Hit = TargetGrid.HitTest(new Point(TargetGrid.ActualRowHeaderWidth + 170, MatrixTop(TargetGrid) + TargetGrid.LayoutMetrics.ColumnHeaderHeight + 4));
            Assert.Equal(0, Hit.ColumnIndex);
            Assert.Equal(PivotGridFieldRole.Row, TargetGrid.Engine.SortRole);
            Assert.Equal(PivotGridSortDirection.Descending, TargetGrid.Engine.SortDirection);
            Assert.True(TargetGrid.Engine.IsFieldFiltered(nameof(PivotGridTestRow.Region)));
            Assert.Equal(new[] { "North" }, TargetGrid.Engine.RowItems.Select(Item => Item.Text));
            Assert.False(TargetGrid.Engine.VisibleRowNodes[0].IsExpanded);
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
