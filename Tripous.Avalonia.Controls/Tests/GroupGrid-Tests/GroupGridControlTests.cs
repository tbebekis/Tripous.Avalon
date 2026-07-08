// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Tests public group grid control APIs that do not require rendering.
/// </summary>
public class GroupGridControlTests
{
    // ● private types
    class RowWithIds
    {
        /// <summary>
        /// Gets or sets the row id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the customer id.
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the row name.
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// Test drop-down editor used to verify host callbacks.
    /// </summary>
    class TestDropDownEditor: GroupGridDropDownInplaceEditorBase
    {
        // ● protected methods
        /// <inheritdoc />
        protected override object GetDropDownSelectedValue()
        {
            return Value;
        }

        // ● public methods
        /// <inheritdoc />
        public override void SelectDropDownItem(object Item)
        {
            Value = Item;
        }
    }

    // ● private methods
    ObservableCollection<GridTestRow> CreateRows()
    {
        return new ObservableCollection<GridTestRow>
        {
            new() { Category = "A", Name = "Alpha", Quantity = 2, Amount = 10m },
            new() { Category = "A", Name = "Beta", Quantity = 5, Amount = 25m },
            new() { Category = "B", Name = "Gamma", Quantity = 1, Amount = 7m },
        };
    }
    GroupGrid CreateGrid(ObservableCollection<GridTestRow> Rows)
    {
        GroupGrid Result = new()
        {
            DataAdapter = new GroupGridListDataAdapter<GridTestRow>(Rows),
        };
        Result.Columns.Add(new GroupGridTextColumn { Name = nameof(GridTestRow.Category), Header = "Category", Width = 120 });
        Result.Columns.Add(new GroupGridTextColumn { Name = nameof(GridTestRow.Name), Header = "Name", Width = 180 });
        Result.Columns.Add(new GroupGridNumberColumn { Name = nameof(GridTestRow.Quantity), Header = "Quantity", Width = 90, TotalSummary = GroupGridAggregateKind.Sum });
        Result.Columns.Add(new GroupGridNumberColumn { Name = nameof(GridTestRow.Amount), Header = "Amount", Width = 100, TotalSummary = GroupGridAggregateKind.Sum });
        return Result;
    }
    GroupGridColumn Column(GroupGrid Grid, string Name)
    {
        return Grid.Columns.First(Item => Item.Name == Name);
    }
    string TempPath(string Extension)
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + "." + Extension);
    }

    // ● tests
    /// <summary>
    /// Verifies that settings capture layout, grouping, filters, summaries, sorting, and visibility state.
    /// </summary>
    [Fact]
    public void CreateSettings_WithConfiguredGrid_CapturesLayoutState()
    {
        GroupGrid Grid = CreateGrid(CreateRows());
        GroupGridColumn CategoryColumn = Column(Grid, nameof(GridTestRow.Category));
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        GroupGridColumn QuantityColumn = Column(Grid, nameof(GridTestRow.Quantity));
        Grid.IsToolBarVisible = false;
        Grid.IsEditButtonVisible = true;
        Grid.Engine.GroupColumn(CategoryColumn);
        Grid.Engine.SetColumnFilter(NameColumn, "ta");
        Grid.Engine.ToggleSort(QuantityColumn);
        QuantityColumn.Width = 140;
        QuantityColumn.GroupSummary = GroupGridAggregateKind.Sum;

        GroupGridSettings Settings = Grid.CreateSettings("Configured");

        Assert.Equal("Configured", Settings.Name);
        Assert.False(Settings.IsToolBarVisible);
        Assert.True(Settings.IsEditButtonVisible);
        Assert.Equal(nameof(GridTestRow.Quantity), Settings.SortColumnName);
        Assert.Equal(GroupGridSortDirection.Ascending, Settings.SortDirection);
        Assert.Equal(0, Settings.Columns.First(Item => Item.Name == nameof(GridTestRow.Category)).GroupIndex);
        Assert.Equal("ta", Settings.Columns.First(Item => Item.Name == nameof(GridTestRow.Name)).FilterText);
        Assert.Equal(140, Settings.Columns.First(Item => Item.Name == nameof(GridTestRow.Quantity)).Width);
        Assert.Equal(GroupGridAggregateKind.Sum, Settings.Columns.First(Item => Item.Name == nameof(GridTestRow.Quantity)).GroupSummary);
    }
    /// <summary>
    /// Verifies that applying settings restores layout state.
    /// </summary>
    [Fact]
    public void ApplySettings_WithSavedSettings_RestoresLayoutState()
    {
        GroupGrid Source = CreateGrid(CreateRows());
        GroupGridColumn SourceCategoryColumn = Column(Source, nameof(GridTestRow.Category));
        GroupGridColumn SourceNameColumn = Column(Source, nameof(GridTestRow.Name));
        GroupGridColumn SourceQuantityColumn = Column(Source, nameof(GridTestRow.Quantity));
        Source.IsColumnHeadersVisible = false;
        Source.Engine.GroupColumn(SourceCategoryColumn);
        Source.Engine.SetColumnFilter(SourceNameColumn, "ta");
        Source.Engine.ToggleSort(SourceQuantityColumn);
        SourceQuantityColumn.Width = 140;

        GroupGridSettings Settings = Source.CreateSettings();
        GroupGrid Target = CreateGrid(CreateRows());

        Target.ApplySettings(Settings);

        GroupGridColumn TargetCategoryColumn = Column(Target, nameof(GridTestRow.Category));
        GroupGridColumn TargetNameColumn = Column(Target, nameof(GridTestRow.Name));
        GroupGridColumn TargetQuantityColumn = Column(Target, nameof(GridTestRow.Quantity));
        Assert.False(Target.IsColumnHeadersVisible);
        Assert.True(Target.Engine.IsGroupedColumn(TargetCategoryColumn));
        Assert.Equal("ta", Target.Engine.GetColumnFilter(TargetNameColumn));
        Assert.Same(TargetQuantityColumn, Target.Engine.SortColumn);
        Assert.Equal(GroupGridSortDirection.Ascending, Target.Engine.SortDirection);
        Assert.Equal(140, TargetQuantityColumn.Width);
    }
    /// <summary>
    /// Verifies formatted settings save/load round-trips through a full file path.
    /// </summary>
    [Fact]
    public void SaveSettingsAndLoadSettings_WithFullFilePath_RoundTripsSettings()
    {
        string FilePath = TempPath("json");
        try
        {
            GroupGrid Source = CreateGrid(CreateRows());
            Source.IsFilterPanelVisible = false;
            Source.Engine.ToggleSort(Column(Source, nameof(GridTestRow.Name)));
            Source.SaveSettings(FilePath, "FileSettings");
            string Json = File.ReadAllText(FilePath);

            GroupGrid Target = CreateGrid(CreateRows());
            bool Loaded = Target.LoadSettings(FilePath);

            Assert.Contains(Environment.NewLine, Json);
            Assert.True(Loaded);
            Assert.False(Target.IsFilterPanelVisible);
            Assert.Equal("FileSettings", JsonDocument.Parse(Json).RootElement.GetProperty("Name").GetString());
            Assert.Same(Column(Target, nameof(GridTestRow.Name)), Target.Engine.SortColumn);
        }
        finally
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
    /// <summary>
    /// Verifies settings context menu properties expose their defaults and can be changed.
    /// </summary>
    [Fact]
    public void SettingsMenuProperties_WithDefaultGrid_ExposeDefaultsAndCanChange()
    {
        GroupGrid Grid = new();

        Assert.True(Grid.IsSettingsMenuItemsVisible);
        Assert.Equal(GroupGridDropDownPlacementMode.Auto, Grid.DropDownPlacementMode);
        Assert.Equal("group-grid-settings.json", Grid.SettingsSuggestedFileName);
        Assert.Equal("Drag a column header here to create a group", Grid.EmptyGroupPanelText);

        Grid.IsSettingsMenuItemsVisible = false;
        Grid.DropDownPlacementMode = GroupGridDropDownPlacementMode.Inline;
        Grid.SettingsSuggestedFileName = "orders-list-grid.json";
        Grid.EmptyGroupPanelText = "Drop here";

        Assert.False(Grid.IsSettingsMenuItemsVisible);
        Assert.Equal(GroupGridDropDownPlacementMode.Inline, Grid.DropDownPlacementMode);
        Assert.Equal("orders-list-grid.json", Grid.SettingsSuggestedFileName);
        Assert.Equal("Drop here", Grid.EmptyGroupPanelText);
    }
    /// <summary>
    /// Verifies drop-down geometry diagnostics expose default rectangles before layout.
    /// </summary>
    [Fact]
    public void DropDownGeometryDiagnostics_WithDefaultGrid_AreDefaultRects()
    {
        GroupGrid Grid = new();

        Assert.Equal(default, Grid.LastEditorRect);
        Assert.Equal(default, Grid.LastDropDownRect);
    }
    /// <summary>
    /// Verifies cell pointer event args expose hit-test, row, column, cell, and handled state.
    /// </summary>
    [Fact]
    public void GroupGridCellPointerEventArgs_WithHitTest_ExposeCellContext()
    {
        GroupGridColumn Column = new GroupGridTextColumn { Name = nameof(GridTestRow.Name) };
        GridTestRow Row = new() { Name = "Alpha" };
        GroupGridHitTestResult Hit = new()
        {
            Kind = GroupGridHitTestKind.BodyCell,
            RowKind = GroupGridRowKind.DataRow,
            RowIndex = 2,
            Column = Column,
        };
        GroupGridCellPointerEventArgs Args = new(Hit, Row, new Point(10, 20), null, true);

        Assert.Same(Hit, Args.HitTest);
        Assert.Equal(new GroupGridCell(2, Column), Args.Cell);
        Assert.Same(Column, Args.Column);
        Assert.Equal(2, Args.RowIndex);
        Assert.Same(Row, Args.Row);
        Assert.Equal(new Point(10, 20), Args.Position);
        Assert.True(Args.IsRightButton);
        Assert.False(Args.Handled);

        Args.Handled = true;

        Assert.True(Args.Handled);
    }
    /// <summary>
    /// Verifies settings load returns false when the full file path does not exist.
    /// </summary>
    [Fact]
    public void LoadSettings_WithMissingFullFilePath_ReturnsFalse()
    {
        string FilePath = TempPath("json");
        GroupGrid Grid = CreateGrid(CreateRows());

        Assert.False(Grid.LoadSettings(FilePath));
    }
    /// <summary>
    /// Verifies settings save rejects relative file paths.
    /// </summary>
    [Fact]
    public void SaveSettings_WithRelativeFilePath_Throws()
    {
        GroupGrid Grid = CreateGrid(CreateRows());

        Assert.Throws<ArgumentException>(() => Grid.SaveSettings("settings.json"));
    }
    /// <summary>
    /// Verifies export snapshot content from the current visible projection.
    /// </summary>
    [Fact]
    public void CreateExportSnapshot_WithFilteredGrid_ReturnsVisibleDataRows()
    {
        GroupGrid Grid = CreateGrid(CreateRows());
        Grid.Engine.SetColumnFilter(Column(Grid, nameof(GridTestRow.Name)), "a");

        GroupGridExportSnapshot Snapshot = Grid.CreateExportSnapshot();

        Assert.Equal(4, Snapshot.Columns.Count);
        Assert.Equal(3, Snapshot.GetDataRows().Count);
        Assert.Equal("Alpha", Snapshot.GetDataRows()[0].Cells.First(Cell => Cell.Column.Name == nameof(GridTestRow.Name)).Text);
        Assert.Equal("sum=8", Snapshot.TotalSummaryCells.First(Cell => Cell.Column.Name == nameof(GridTestRow.Quantity)).Text);
    }
    /// <summary>
    /// Verifies the public Id-column visibility policy.
    /// </summary>
    [Fact]
    public void AreIdColumnsVisible_WithIdColumnNames_HidesAndShowsIdColumns()
    {
        GroupGrid Grid = new();
        GroupGridColumn IdColumn = new GroupGridTextColumn { Name = "Id" };
        GroupGridColumn CustomerIdColumn = new GroupGridTextColumn { Name = "CustomerId" };
        GroupGridColumn LowerCaseIdColumn = new GroupGridTextColumn { Name = "orderid" };
        GroupGridColumn NameColumn = new GroupGridTextColumn { Name = "Name" };
        Grid.Columns.Add(IdColumn);
        Grid.Columns.Add(CustomerIdColumn);
        Grid.Columns.Add(NameColumn);

        Grid.AreIdColumnsVisible = false;
        Grid.Columns.Add(LowerCaseIdColumn);

        Assert.False(IdColumn.IsVisible);
        Assert.False(CustomerIdColumn.IsVisible);
        Assert.False(LowerCaseIdColumn.IsVisible);
        Assert.True(NameColumn.IsVisible);

        Grid.AreIdColumnsVisible = true;

        Assert.True(IdColumn.IsVisible);
        Assert.True(CustomerIdColumn.IsVisible);
        Assert.True(LowerCaseIdColumn.IsVisible);
        Assert.True(NameColumn.IsVisible);
    }
    /// <summary>
    /// Verifies by-name column visibility API.
    /// </summary>
    [Fact]
    public void SetColumnVisibleAndSetColumnsVisible_WithFieldNames_ChangeVisibility()
    {
        GroupGrid Grid = CreateGrid(CreateRows());

        Assert.True(Grid.SetColumnVisible(nameof(GridTestRow.Name), false));
        Assert.False(Column(Grid, nameof(GridTestRow.Name)).IsVisible);
        Assert.True(Grid.SetColumnsVisible(new[] { nameof(GridTestRow.Quantity), nameof(GridTestRow.Amount) }, false));
        Assert.False(Column(Grid, nameof(GridTestRow.Quantity)).IsVisible);
        Assert.False(Column(Grid, nameof(GridTestRow.Amount)).IsVisible);
        Assert.True(Grid.SetColumnsVisible(new[] { nameof(GridTestRow.Name), nameof(GridTestRow.Quantity) }, true));
        Assert.True(Column(Grid, nameof(GridTestRow.Name)).IsVisible);
        Assert.True(Column(Grid, nameof(GridTestRow.Quantity)).IsVisible);
    }
    /// <summary>
    /// Verifies by-name column read-only API.
    /// </summary>
    [Fact]
    public void SetColumnReadOnlyAndSetColumnsReadOnly_WithFieldNames_ChangeReadOnlyState()
    {
        GroupGrid Grid = CreateGrid(CreateRows());

        Assert.True(Grid.SetColumnReadOnly(nameof(GridTestRow.Name), true));
        Assert.True(Column(Grid, nameof(GridTestRow.Name)).IsReadOnly);
        Assert.True(Grid.SetColumnsReadOnly(new[] { nameof(GridTestRow.Quantity), nameof(GridTestRow.Amount) }, true));
        Assert.True(Column(Grid, nameof(GridTestRow.Quantity)).IsReadOnly);
        Assert.True(Column(Grid, nameof(GridTestRow.Amount)).IsReadOnly);
        Assert.True(Grid.SetColumnsReadOnly(new[] { nameof(GridTestRow.Name), nameof(GridTestRow.Quantity) }, false));
        Assert.False(Column(Grid, nameof(GridTestRow.Name)).IsReadOnly);
        Assert.False(Column(Grid, nameof(GridTestRow.Quantity)).IsReadOnly);
    }
    /// <summary>
    /// Verifies single-column best fit updates only the requested column width.
    /// </summary>
    [Fact]
    public void BestFitColumn_WithLongText_UpdatesRequestedColumnWidth()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        Rows[1].Name = "A very long customer display name";
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        GroupGridColumn AmountColumn = Column(Grid, nameof(GridTestRow.Amount));
        NameColumn.Width = 24;
        AmountColumn.Width = 100;

        bool Changed = Grid.BestFitColumn(NameColumn);

        Assert.True(Changed);
        Assert.True(NameColumn.Width > 24);
        Assert.Equal(100, AmountColumn.Width);
    }
    /// <summary>
    /// Verifies single-column best fit respects the minimum column width.
    /// </summary>
    [Fact]
    public void BestFitColumn_WithMinWidth_RespectsMinWidth()
    {
        GroupGrid Grid = CreateGrid(CreateRows());
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        NameColumn.Width = 24;
        NameColumn.MinWidth = 160;

        bool Changed = Grid.BestFitColumn(NameColumn);

        Assert.True(Changed);
        Assert.True(NameColumn.Width >= 160);
    }
    /// <summary>
    /// Verifies string-based best fit returns false for a missing column.
    /// </summary>
    [Fact]
    public void BestFitColumn_WithMissingColumnName_ReturnsFalse()
    {
        GroupGrid Grid = CreateGrid(CreateRows());

        Assert.False(Grid.BestFitColumn("Missing"));
    }
    /// <summary>
    /// Verifies all-column best fit returns the number of changed columns.
    /// </summary>
    [Fact]
    public void BestFitColumns_WithNarrowColumns_ReturnsChangedCount()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        Rows[0].Category = "Long category value";
        Rows[0].Name = "Long customer name";
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn CategoryColumn = Column(Grid, nameof(GridTestRow.Category));
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        GroupGridColumn AmountColumn = Column(Grid, nameof(GridTestRow.Amount));
        GroupGridColumn QuantityColumn = Column(Grid, nameof(GridTestRow.Quantity));
        CategoryColumn.Width = 24;
        NameColumn.Width = 24;
        QuantityColumn.Width = 24;
        AmountColumn.Width = 24;

        int ChangedCount = Grid.BestFitColumns();

        Assert.Equal(4, ChangedCount);
        Assert.True(CategoryColumn.Width > 24);
        Assert.True(NameColumn.Width > 24);
        Assert.True(QuantityColumn.Width > 24);
        Assert.True(AmountColumn.Width > 24);
    }
    /// <summary>
    /// Verifies the public current-row API follows the current cell row.
    /// </summary>
    [Fact]
    public void CurrentRowApi_WithCurrentCell_ReturnsRowAndRaisesRowChanged()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = CreateGrid(Rows);
        int EventCount = 0;
        Grid.CurrentRowChanged += (Sender, Args) => EventCount++;

        Assert.Equal(-1, Grid.CurrentRowIndex);
        Assert.Null(Grid.CurrentRow);

        Assert.True(Grid.SetCurrentCell(1, Column(Grid, nameof(GridTestRow.Name))));

        Assert.Equal(1, Grid.CurrentRowIndex);
        Assert.Same(Rows[1], Grid.CurrentRow);
        Assert.Equal(1, EventCount);

        Assert.True(Grid.SetCurrentCell(1, Column(Grid, nameof(GridTestRow.Quantity))));

        Assert.Equal(1, Grid.CurrentRowIndex);
        Assert.Same(Rows[1], Grid.CurrentRow);
        Assert.Equal(1, EventCount);

        Grid.ClearCurrentCell();

        Assert.Equal(-1, Grid.CurrentRowIndex);
        Assert.Null(Grid.CurrentRow);
        Assert.Equal(2, EventCount);
    }
    /// <summary>
    /// Verifies control-level cell committed event is raised after an edit commit.
    /// </summary>
    [Fact]
    public void CellValueCommitted_WithCommitEdit_FiresControlLevelEvent()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        GroupGridCellEditEventArgs CommittedArgs = null;
        Grid.CellValueCommitted += (Sender, Args) => CommittedArgs = Args;

        Assert.True(Grid.SetCurrentCell(0, NameColumn));
        Assert.True(Grid.BeginEdit());
        Assert.True(Grid.CommitEdit("Changed"));

        Assert.Equal("Changed", Rows[0].Name);
        Assert.NotNull(CommittedArgs);
        Assert.Equal(new GroupGridCell(0, NameColumn), CommittedArgs.Cell);
        Assert.Equal("Changed", CommittedArgs.Value);
    }
    /// <summary>
    /// Verifies control-level committing event can modify the value before it is written.
    /// </summary>
    [Fact]
    public void CellValueCommitting_WithModifiedValue_UpdatesCommittedValue()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        Grid.CellValueCommitting += (Sender, Args) => Args.Value = "Modified";

        Assert.True(Grid.SetCurrentCell(0, NameColumn));
        Assert.True(Grid.BeginEdit());
        Assert.True(Grid.CommitEdit("Changed"));

        Assert.Equal("Modified", Rows[0].Name);
    }
    /// <summary>
    /// Verifies control-level committing cancellation keeps editing active and leaves the value unchanged.
    /// </summary>
    [Fact]
    public void CellValueCommitting_WhenCanceled_KeepsEditingAndValueUnchanged()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        Grid.CellValueCommitting += (Sender, Args) => Args.Cancel = true;

        Assert.True(Grid.SetCurrentCell(0, NameColumn));
        Assert.True(Grid.BeginEdit());
        Assert.False(Grid.CommitEdit("Changed"));

        Assert.Equal("Alpha", Rows[0].Name);
        Assert.True(Grid.IsEditing);
    }
    /// <summary>
    /// Verifies control-level edit canceled event is raised when editing is canceled.
    /// </summary>
    [Fact]
    public void EditCanceled_WithCancelEdit_FiresControlLevelEvent()
    {
        GroupGrid Grid = CreateGrid(CreateRows());
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        GroupGridCellEditEventArgs CanceledArgs = null;
        Grid.EditCanceled += (Sender, Args) => CanceledArgs = Args;

        Assert.True(Grid.SetCurrentCell(0, NameColumn));
        Assert.True(Grid.BeginEdit());
        Assert.True(Grid.CancelEdit());

        Assert.NotNull(CanceledArgs);
        Assert.Equal(new GroupGridCell(0, NameColumn), CanceledArgs.Cell);
        Assert.Equal("Alpha", CanceledArgs.Value);
    }
    /// <summary>
    /// Verifies a custom drop-down editor can commit a value through the host callback.
    /// </summary>
    [Fact]
    public void DropDownEditorHost_WithCommitValue_CommitsEditorValue()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        TestDropDownEditor Editor = new();
        Grid.CreateInplaceEditor += (Sender, Args) =>
        {
            Args.Editor = Editor;
            Args.Handled = true;
        };

        Assert.True(Grid.SetCurrentCell(0, NameColumn));
        Assert.True(Grid.BeginEdit());
        Assert.NotNull(Editor.DropDownHost);
        Assert.True(Editor.DropDownHost.CommitDropDownValue("HostChanged"));

        Assert.Equal("HostChanged", Rows[0].Name);
        Assert.False(Grid.IsEditing);
    }
    /// <summary>
    /// Verifies a custom drop-down editor can cancel the drop-down without committing a value.
    /// </summary>
    [Fact]
    public void DropDownEditorHost_WithCancelDropDown_DoesNotCommitValue()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = CreateGrid(Rows);
        GroupGridColumn NameColumn = Column(Grid, nameof(GridTestRow.Name));
        TestDropDownEditor Editor = new();
        Grid.CreateInplaceEditor += (Sender, Args) =>
        {
            Args.Editor = Editor;
            Args.Handled = true;
        };

        Assert.True(Grid.SetCurrentCell(0, NameColumn));
        Assert.True(Grid.BeginEdit());
        Editor.Value = "Pending";

        Assert.True(Editor.DropDownHost.CancelDropDown());

        Assert.Equal("Alpha", Rows[0].Name);
        Assert.True(Grid.IsEditing);
    }
    /// <summary>
    /// Verifies row scrolling API uses adapter row indexes and respects the current projection.
    /// </summary>
    [Fact]
    public void ScrollToRow_WithVisibleAndFilteredRows_ReturnsExpectedResult()
    {
        GroupGrid Grid = CreateGrid(CreateRows());

        Assert.True(Grid.ScrollToRow(1));

        Grid.SetColumnFilter(Column(Grid, nameof(GridTestRow.Name)), "Alpha");

        Assert.True(Grid.ScrollToRow(0));
        Assert.False(Grid.ScrollToRow(1));
    }
    /// <summary>
    /// Verifies automatic columns and adapter creation for list item sources.
    /// </summary>
    [Fact]
    public void ItemsSource_WithAutoGenerateColumnsAndList_GeneratesColumnsAndAdapter()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGrid Grid = new()
        {
            AutoGenerateColumns = true,
            ItemsSource = Rows,
        };

        Assert.IsType<GroupGridListDataAdapter<GridTestRow>>(Grid.DataAdapter);
        Assert.Contains(Grid.Columns, Column => Column.Name == nameof(GridTestRow.Category) && Column is GroupGridTextColumn);
        Assert.Contains(Grid.Columns, Column => Column.Name == nameof(GridTestRow.Quantity) && Column is GroupGridNumberColumn);
        Assert.Equal("Alpha", Grid.DataAdapter.GetValue(0, Column(Grid, nameof(GridTestRow.Name))));
    }
    /// <summary>
    /// Verifies automatic columns and adapter creation for data table item sources.
    /// </summary>
    [Fact]
    public void ItemsSource_WithAutoGenerateColumnsAndDataTable_GeneratesColumnsAndAdapter()
    {
        DataTable Table = new();
        DataColumn NameColumn = Table.Columns.Add(nameof(GridTestRow.Name), typeof(string));
        DataColumn QuantityColumn = Table.Columns.Add(nameof(GridTestRow.Quantity), typeof(int));
        QuantityColumn.ReadOnly = true;
        NameColumn.Caption = "Customer Name";
        Table.Rows.Add("Alpha", 2);
        GroupGrid Grid = new()
        {
            AutoGenerateColumns = true,
            ItemsSource = Table,
        };

        GroupGridColumn GeneratedNameColumn = Column(Grid, nameof(GridTestRow.Name));
        GroupGridColumn GeneratedQuantityColumn = Column(Grid, nameof(GridTestRow.Quantity));
        Assert.IsType<GroupGridDataViewDataAdapter>(Grid.DataAdapter);
        Assert.IsType<GroupGridTextColumn>(GeneratedNameColumn);
        Assert.IsType<GroupGridNumberColumn>(GeneratedQuantityColumn);
        Assert.Equal("Customer Name", GeneratedNameColumn.Header);
        Assert.True(GeneratedQuantityColumn.IsReadOnly);
        Assert.Equal("Alpha", Grid.DataAdapter.GetValue(0, GeneratedNameColumn));
    }
    /// <summary>
    /// Verifies Id-column visibility policy during auto-generation.
    /// </summary>
    [Fact]
    public void ItemsSource_WithHiddenIdColumns_HidesGeneratedIdColumns()
    {
        List<RowWithIds> Rows = new()
        {
            new() { Id = 1, CustomerId = 2, Name = "Alpha" },
        };
        GroupGrid Grid = new()
        {
            AreIdColumnsVisible = false,
            AutoGenerateColumns = true,
            ItemsSource = Rows,
        };

        Assert.False(Column(Grid, nameof(RowWithIds.Id)).IsVisible);
        Assert.False(Column(Grid, nameof(RowWithIds.CustomerId)).IsVisible);
        Assert.True(Column(Grid, nameof(RowWithIds.Name)).IsVisible);
    }
}
