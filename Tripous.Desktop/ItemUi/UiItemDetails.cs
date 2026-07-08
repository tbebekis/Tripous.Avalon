/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Creates the UI for item page details, including detail tabs, one-to-one detail sections and detail grids.
/// </summary>
static public class UiItemDetails
{
    // ● details
    /// <summary>
    /// Creates the first-level detail tabs under the top table tab.
    /// </summary>
    static public void CreateFirstLevelDetails(UiItemContext context, Control ParentControl)
    {
        List<UiDetailTableInfo> Details = context.TopTableUiInfo.DetailList
            .Where(Detail => Detail.ParentTableDef == context.TopTableUiInfo.TableDef)
            .ToList();
        Details = OrderDetails(context, context.TopTableUiInfo.TableDef, Details);
        if (Details.Count == 0)
            return;
        TabControl TabControl = UiFactory.CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            TabControl.Items.Add(CreateDetailTabItem(context, Detail));
        UiFactory.AddChild(ParentControl, TabControl);
    }
    /// <summary>
    /// Returns the immediate child details of a detail table.
    /// </summary>
    static List<UiDetailTableInfo> GetChildDetails(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        List<UiDetailTableInfo> Result = context.TopTableUiInfo.DetailList
            .Where(Detail => Detail.ParentTableDef == DetailUiInfo.TableDef)
            .ToList();
        return OrderDetails(context, DetailUiInfo.TableDef, Result);
    }
    /// <summary>
    /// Orders sibling details according to the module detail order.
    /// </summary>
    static List<UiDetailTableInfo> OrderDetails(UiItemContext context, TableDef ParentTableDef, List<UiDetailTableInfo> Details)
    {
        if (Details.Count < 2 || ParentTableDef == null || !context.ModuleDef.DetailOrder.TryGetValue(ParentTableDef.Name, out List<string> DetailOrder))
            return Details;

        Dictionary<string, int> Order = new(StringComparer.OrdinalIgnoreCase);
        for (int Index = 0; Index < DetailOrder.Count; Index++)
        {
            string Name = DetailOrder[Index];
            if (!string.IsNullOrWhiteSpace(Name) && !Order.ContainsKey(Name))
                Order[Name] = Index;
        }

        return Details
            .Select((Detail, Index) => new
            {
                Detail,
                Index,
                Order = Order.TryGetValue(Detail.TableDef.Name, out int Value) ? Value : int.MaxValue
            })
            .OrderBy(Item => Item.Order)
            .ThenBy(Item => Item.Index)
            .Select(Item => Item.Detail)
            .ToList();
    }
    /// <summary>
    /// Creates a horizontal splitter between a parent detail and its child details.
    /// </summary>
    static GridSplitter CreateDetailSplitter()
    {
        return new GridSplitter
        {
            Height = 5,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            ResizeDirection = GridResizeDirection.Rows,
            ResizeBehavior = GridResizeBehavior.PreviousAndNext,
            Background = Brushes.LightGray
        };
    }
    /// <summary>
    /// Creates a panel for a single child detail.
    /// </summary>
    static Control CreateSingleChildDetail(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        Grid Result = new();
        Result.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Result.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        TextBlock Header = new()
        {
            Text = DetailUiInfo.TableDef.Title,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 8, 0, 4)
        };
        Control Content = CreateDetailBranch(context, DetailUiInfo, ApplyMinimumHeight: false);
        Avalonia.Controls.Grid.SetRow(Header, 0);
        Avalonia.Controls.Grid.SetRow(Content, 1);
        Result.Children.Add(Header);
        Result.Children.Add(Content);
        return Result;
    }
    /// <summary>
    /// Creates tabs for multiple child details.
    /// </summary>
    static Control CreateChildDetailTabs(UiItemContext context, List<UiDetailTableInfo> Details)
    {
        TabControl Result = UiFactory.CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            Result.Items.Add(CreateDetailTabItem(context, Detail, ApplyMinimumHeight: false));
        return Result;
    }
    /// <summary>
    /// Creates a detail branch recursively.
    /// </summary>
    static Control CreateDetailBranch(UiItemContext context, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        Grid Result = new();
        List<UiDetailTableInfo> Children = GetChildDetails(context, DetailUiInfo);
        if (Children.Count == 0)
        {
            CreateDetail(context, Result, DetailUiInfo, ApplyMinimumHeight);
            return Result;
        }

        if (ApplyMinimumHeight)
            Result.MinHeight = (Ui.Settings.DetailGridMinHeight * 2) + 5;
        Result.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        Result.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Result.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        GridSplitter Splitter = CreateDetailSplitter();
        Control ChildControl = Children.Count == 1
            ? CreateSingleChildDetail(context, Children[0])
            : CreateChildDetailTabs(context, Children);
        Grid ParentControl = CreateDetail(context, Result, DetailUiInfo, ApplyMinimumHeight: false);
        Avalonia.Controls.Grid.SetRow(ParentControl, 0);
        Avalonia.Controls.Grid.SetRow(Splitter, 1);
        Avalonia.Controls.Grid.SetRow(ChildControl, 2);
        Result.Children.Add(Splitter);
        Result.Children.Add(ChildControl);
        return Result;
    }
    /// <summary>
    /// Creates a tab item for a detail table.
    /// </summary>
    static public TabItem CreateDetailTabItem(UiItemContext context, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        TabItem Result = new()
        {
            Header = DetailUiInfo.TableDef.Title,
            Content = CreateDetailBranch(context, DetailUiInfo, ApplyMinimumHeight)
        };
        return Result;
    }
    /// <summary>
    /// Creates the container UI for a multi-row detail table.
    /// </summary>
    static public Grid CreateDetail(UiItemContext context, Control ParentControl, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        Grid Panel = new();
        Panel.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        GroupGrid Grid = CreateDetailDataGrid(context, DetailUiInfo, ApplyMinimumHeight);
        DetailUiInfo.Grid = Grid;
        Avalonia.Controls.Grid.SetRow(Grid, 0);
        Panel.Children.Add(Grid);
        CreateDetailGridReferenceMenus(context, DetailUiInfo);
        UiFactory.AddChild(ParentControl, Panel);
        return Panel;
    }
    /// <summary>
    /// Creates the field UI and binder used by a one-to-one detail table.
    /// </summary>
    static public void CreateOneToOneDetail(UiItemContext context, Control ParentControl, UiTableInfo TableUiInfo)
    {
        ItemBinder Binder = context.CreateOneToOneBinder(TableUiInfo.TableDef);
        Binder.TableInfo = TableUiInfo;
        context.Binders.Add(Binder);
        UiItemPage.CreateFieldGroups(context, ParentControl, TableUiInfo, Binder, context.ColumnCount);
    }
    /// <summary>
    /// Creates one-to-one detail controls under a specified parent table.
    /// </summary>
    static public void CreateOneToOneDetails(UiItemContext context, Control ParentControl, TableDef ParentTableDef)
    {
        foreach (UiTableInfo TableUiInfo in context.TopTableUiInfo.OneToOneList)
        {
            if (TableUiInfo.TableDef.Master != ParentTableDef)
                continue;
            CreateOneToOneDetail(context, ParentControl, TableUiInfo);
        }
    }
    /// <summary>
    /// Creates the reference menus for a detail data grid.
    /// </summary>
    static public void CreateDetailGridReferenceMenus(UiItemContext context, UiDetailTableInfo DetailUiInfo)
    {
        if (DetailUiInfo.Grid == null || DetailUiInfo.Table == null || context.GridHandler is not IReferenceContextMenuHost MenuHost)
            return;

        FormDef FormDef = (context.GridHandler as ItemPage)?.FormDef;
        foreach (GroupGridColumnBinding Binding in DetailUiInfo.Grid.GetInfoList())
        {
            if (Binding == null || !Binding.IsReference || Binding.GridColumn == null || Binding.GridColumn.IsReadOnly)
                continue;

            ReferenceContextMenu RefMenu = FormDef != null
                ? FormDef.CreateReferenceContextMenu()
                : new ReferenceContextMenu();
            RefMenu.Initialize(MenuHost, Binding);
        }

        DetailUiInfo.Grid.CellPointerPressed += DetailGrid_CellPointerPressed;
    }

    // ● detail grids
    /// <summary>
    /// Finds a display snapshot field for a lookup field.
    /// </summary>
    static public FieldDef FindLookupDisplaySnapshotField(TableDef TableDef, FieldDef LookupField)
    {
        if (TableDef == null || LookupField == null || !LookupField.IsLookup)
            return null;

        TableDef JoinTable = TableDef.FindJoinTableByMasterKeyField(LookupField.Name);
        string SourceName = JoinTable?.Alias;
        if (string.IsNullOrWhiteSpace(SourceName))
        {
            LookupDef LookupDef = DataRegistry.Lookups.Find(LookupField.LookupSource);
            SourceName = LookupDef?.TableName;
            if (string.IsNullOrWhiteSpace(SourceName))
                SourceName = LookupDef?.Name;
        }
        if (string.IsNullOrWhiteSpace(SourceName))
            return null;

        return TableDef.Fields.FirstOrDefault(Field =>
        {
            if (string.IsNullOrWhiteSpace(Field.SnapshotOf))
                return false;

            string[] Parts = Field.SnapshotOf.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return Parts.Length == 2 && Parts[0].IsSameText(SourceName) && Parts[1].IsSameText("Name");
        });
    }
    /// <summary>
    /// Creates a detail data grid.
    /// </summary>
    static public GroupGrid CreateDetailDataGrid(UiItemContext context, UiDetailTableInfo DetailUiInfo, bool ApplyMinimumHeight = true)
    {
        TableDef TableDef = DetailUiInfo.TableDef;
        GroupGrid Result = new()
        {
            Focusable = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsTabStop = true,
            MinHeight = ApplyMinimumHeight ? Ui.Settings.DetailGridMinHeight : 0,
            Margin = new Thickness(0, 8, 0, 8),
            IsToolBarVisible = true,
            IsGroupPanelVisible = false,
            IsFilterPanelVisible = false,
            IsTotalsSummaryVisible = false,
            IsSettingsMenuItemsVisible = true,
            SettingsSuggestedFileName = $"{context.ModuleDef.Name}-{TableDef.Name}-DetailGrid.json",
        };
        CreateDetailGridColumns(Result, TableDef);
        BindDetailGrid(context, Result, DetailUiInfo);
        return Result;
    }
    /// <summary>
    /// Creates the columns of a detail data grid.
    /// </summary>
    static public void CreateDetailGridColumns(GroupGrid Grid, TableDef TableDef)
    {
        HashSet<string> AddedFields = new(StringComparer.OrdinalIgnoreCase);
        foreach (FieldDef Field in TableDef.GetBindableFields())
        {
            if (!UiItemPage.IsDetailGridField(Field))
                continue;
            if (TableDef.IsLocatorSnapshotField(Field))
                continue;
            if (AddedFields.Contains(Field.Name))
                continue;

            foreach (GroupGridColumn Column in CreateDetailGridColumns(Field))
            {
                Grid.Columns.Add(Column);

                GroupGridColumnBinding Binding = Column.GetInfo();
                if (!string.IsNullOrWhiteSpace(Binding?.DisplayFieldName))
                    AddedFields.Add(Binding.DisplayFieldName);
                if (!string.IsNullOrWhiteSpace(Column.Name))
                    AddedFields.Add(Column.Name);
            }
            AddedFields.Add(Field.Name);
        }
    }
    /// <summary>
    /// Creates columns for a detail grid field.
    /// </summary>
    static public List<GroupGridColumn> CreateDetailGridColumns(FieldDef Field)
    {
        if (Field.IsLocator)
        {
            List<GroupGridColumn> LocatorColumns = CreateLocatorDetailGridColumns(Field);
            if (LocatorColumns.Count > 0)
                return LocatorColumns;
        }

        return [CreateDetailGridColumn(Field)];
    }
    /// <summary>
    /// Creates display columns for a locator detail grid field.
    /// </summary>
    static public List<GroupGridColumn> CreateLocatorDetailGridColumns(FieldDef Field)
    {
        List<GroupGridColumn> Result = [];
        if (Field.TableDef == null)
            return Result;

        LocatorDef locatorDef = DataRegistry.FindLocator(Field.Locator);
        if (locatorDef != null)
        {
            LocatorMapPlan MapPlan = new LocatorMapper().CreatePlan(locatorDef, Field.TableDef, Field);
            foreach (LocatorMapItem Item in MapPlan.Items)
            {
                if (Item.SourceField.IsSameText(locatorDef.KeyField))
                    continue;

                LocatorFieldDef LocatorField = locatorDef.Fields.Find(Item.SourceField);
                FieldDef TargetField = Field.TableDef.Fields.FirstOrDefault(x => x.Alias.IsSameText(Item.TargetField) || x.Name.IsSameText(Item.TargetField));
                if (LocatorField == null || TargetField == null)
                    continue;
                if (TargetField.IsLookup)
                    continue;
                if (!string.IsNullOrWhiteSpace(TargetField.SnapshotOf) && Field.TableDef.Fields.Any(x => x.IsLookup && FindLookupDisplaySnapshotField(Field.TableDef, x) == TargetField))
                    continue;

                Result.Add(GroupGridBinder.CreateLocatorColumn2(TargetField.Alias, TargetField.Title, Field, LocatorField, locatorDef, MapPlan));
            }
            return Result;
        }

        return Result;
    }
    /// <summary>
    /// Creates a column for a detail data grid.
    /// </summary>
    static public GroupGridColumn CreateDetailGridColumn(FieldDef Field)
    {
        if (Field.IsLookup)
            return GroupGridBinder.CreateLookupColumn(Field);
        return GroupGridBinder.CreateGridColumn(Field);
    }
    /// <summary>
    /// Updates group grid column bindings with runtime table information.
    /// </summary>
    static public void UpdateDetailGridColumnBindings(GroupGrid Grid, MemTable Table)
    {
        if (Grid == null || Table == null)
            return;

        foreach (GroupGridColumnBinding Binding in Grid.GetInfoList())
        {
            if (Binding.DataColumn != null)
                continue;

            Binding.DataColumn = Table.FindColumn(Binding.FieldName) ?? Table.FindColumn(Binding.DisplayFieldName);
        }
    }
    /// <summary>
    /// Handles committed detail grid cell values.
    /// </summary>
    static public void DetailGrid_CellValueCommitted(object Sender, GroupGridCellEditEventArgs Args)
    {
        if (Sender is not GroupGrid Grid || Args.Cell.Column?.Tag is not GroupGridColumnBinding Binding)
            return;
        if (Binding.FieldDef == null || !Binding.FieldDef.IsLookup || Binding.LookupSource == null)
            return;
        if (Grid.CurrentRow is not DataRowView RowView || RowView.Row == null)
            return;

        LookupItem LookupItem = Binding.LookupSource.FindItem(Args.Value);
        Binding.FieldDef.TableDef?.AssignLookupSnapshots(RowView.Row, Binding.FieldDef, Binding.LookupSource, LookupItem);
    }
    /// <summary>
    /// Handles detail grid cell pointer events.
    /// </summary>
    static public void DetailGrid_CellPointerPressed(object Sender, GroupGridCellPointerEventArgs Args)
    {
        if (Sender is not Control Control || Args == null || !Args.IsRightButton)
            return;
        if (Args.Column?.Tag is not GroupGridColumnBinding Binding)
            return;
        if (Binding.ReferenceContextMenu == null)
            return;

        Args.Handled = Binding.ReferenceContextMenu.Open(Control);
    }
    /// <summary>
    /// Handles committing detail grid cell values before they are written to the adapter.
    /// </summary>
    static public void DetailGrid_CellValueCommitting(object Sender, GroupGridCellEditEventArgs Args)
    {
        if (Sender is not GroupGrid Grid || Args.Cell.Column?.Tag is not GroupGridColumnBinding Binding)
            return;
        if (Binding.LocatorDef == null || Binding.LocatorMapPlan == null || Args.Value is not DataRow SourceRow)
            return;
        if (Grid.CurrentRow is not DataRowView RowView || RowView.Row == null)
            return;

        new LocatorMapper().Apply(Binding.LocatorMapPlan, SourceRow, RowView.Row);

        DataColumn Column = RowView.Row.Table.FindColumn(Binding.DisplayFieldName);
        Args.Value = Column != null ? RowView.Row[Column] : DBNull.Value;
    }
    /// <summary>
    /// Handles custom detail grid editor creation.
    /// </summary>
    static public void DetailGrid_CreateInplaceEditor(object Sender, GroupGridCreateInplaceEditorEventArgs Args)
    {
        if (Sender is not GroupGrid Grid || Args.Column?.Tag is not GroupGridColumnBinding Binding)
            return;
        if (Binding.LocatorDef == null || Binding.GridColumn.IsReadOnly)
            return;

        Args.Editor = new GroupGridLocatorInplaceEditor(Binding.LocatorDef, Binding.LocatorSourceFieldName, Grid.CurrentRow as DataRowView);
        Args.Handled = true;
    }
    /// <summary>
    /// Binds a detail data grid to the view of its table.
    /// </summary>
    static public void BindDetailGrid(UiItemContext context, GroupGrid Grid, UiDetailTableInfo DetailUiInfo)
    {
        TableDef TableDef = DetailUiInfo.TableDef;
        MemTable Table = context.Module.GetTable(TableDef.Name);
        DataView DataView = Table.DataView;
        UpdateDetailGridColumnBindings(Grid, Table);
        Grid.ItemsSource = DataView;

        GridCommand[] Commands = context.GridHandler?.GetGridCommands()?.Where(Command => Command.IsVisible).ToArray() ?? [];
        GridCommand AddCommand = Commands.FirstOrDefault(Command => Command.ActionType == GridActionType.Add);
        GridCommand DeleteCommand = Commands.FirstOrDefault(Command => Command.ActionType == GridActionType.Delete);
        Grid.IsInsertButtonVisible = AddCommand != null;
        Grid.IsDeleteButtonVisible = DeleteCommand != null;
        Grid.IsEditButtonVisible = false;

        DetailGridCommandContext CreateContext(GridCommand Command)
        {
            return new DetailGridCommandContext()
            {
                Command = Command,
                Grid = Grid,
                Table = Table,
                DetailInfo = DetailUiInfo,
                ItemContext = context
            };
        }

        void ExecuteCommand(GridCommand Command)
        {
            if (Command == null || context.GridHandler == null)
                return;

            DetailGridCommandContext CommandContext = CreateContext(Command);
            if (context.GridHandler.CanExecute(CommandContext))
                context.GridHandler.Execute(CommandContext);
        }

        void BestFitColumns()
        {
            Ui.Post(() => Grid.BestFitColumns());
        }

        void SelectFirstRow()
        {
            Ui.Post(() =>
            {
                if (DataView.Count > 0 && Grid.CurrentRow == null)
                {
                    GroupGridBinder.SelectRow(Grid, 0);
                    Table.CurrentRowView = DataView[0];
                }
            });
        }

        Grid.CurrentRowChanged += (Sender, Args) => Table.CurrentRowView = Grid.CurrentRow as DataRowView;
        Grid.CreateInplaceEditor += DetailGrid_CreateInplaceEditor;
        Grid.CellValueCommitting += DetailGrid_CellValueCommitting;
        Grid.CellValueCommitted += DetailGrid_CellValueCommitted;
        Grid.InsertingRow += (Sender, Args) =>
        {
            Args.Cancel = true;
            ExecuteCommand(AddCommand);
        };
        Grid.DeletingRow += (Sender, Args) =>
        {
            Args.Cancel = true;
            ExecuteCommand(DeleteCommand);
        };
        Grid.AddHandler(InputElement.KeyDownEvent, (Sender, Args) =>
        {
            foreach (GridCommand Command in Commands)
            {
                if (Command.KeyGesture != null && Command.KeyGesture.Matches(Args))
                {
                    ExecuteCommand(Command);
                    Args.Handled = true;
                    break;
                }
            }
        }, RoutingStrategies.Tunnel, handledEventsToo: true);
        DataView.ListChanged += (Sender, Args) =>
        {
            SelectFirstRow();
            BestFitColumns();
        };
        SelectFirstRow();
        BestFitColumns();
    }
}
