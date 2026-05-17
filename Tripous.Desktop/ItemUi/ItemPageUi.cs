namespace Tripous.Desktop;
 
static public class ItemPageUi
{
    // ● ui creation - common
    /// <summary>
    /// Adds a child control to a parent control.
    /// </summary>
    static public void AddChild(Control ParentControl, Control Child)
    {
        if (ParentControl is Panel Panel)
        {
            Panel.Children.Add(Child);
            return;
        }
        if (ParentControl is ContentControl ContentControl)
        {
            ContentControl.Content = Child;
            return;
        }
        throw new ApplicationException("Invalid layout parent.");
    }
    /// <summary>
    /// Creates the root scroll viewer.
    /// </summary>
    static public ScrollViewer CreateScrollViewer()
    {
        return new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
    }
    /// <summary>
    /// Creates a vertical stack panel.
    /// </summary>
    static public StackPanel CreateStackPanel()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
    }
    /// <summary>
    /// Creates an expander.
    /// </summary>
    static public Expander CreateExpander(Control ParentControl, string Caption)
    {
        Expander Result = new()
        {
            Header = Caption,
            IsExpanded = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 8)
        };
        AddChild(ParentControl, Result);
        return Result;
    }
    /// <summary>
    /// Creates a tab control.
    /// </summary>
    static public TabControl CreateTabControl()
    {
        return new TabControl
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 8, 0, 0)
        };
    }
        
    // ● ui info
    /// <summary>
    /// Creates the UI information tree for the top table.
    /// </summary>
    static public UiTableInfo CreateTopTableUiInfo(DataModule Module)
    {
        UiTableInfo Result = CreateUiTableInfo(Module.ModuleDef.Table, Module);
        AddDetailUiInfo(Result, Module.ModuleDef.Table, Module);
        return Result;
    }
    /// <summary>
    /// Creates UI information for a table.
    /// </summary>
    static public UiTableInfo CreateUiTableInfo(TableDef TableDef, DataModule Module)
    {
        UiTableInfo Result = new();
        Result.TableDef = TableDef;
        Result.Table = Module.GetTable(TableDef.Name);
        return Result;
    }
    /// <summary>
    /// Adds detail UI information recursively.
    /// </summary>
    static public void AddDetailUiInfo(UiTableInfo RootUiInfo, TableDef ParentTableDef, DataModule Module)
    {
        foreach (TableDef Detail in ParentTableDef.Details)
        {
            if (!Detail.IsUiVisible)
                continue;
            if (Detail.IsOneToOne)
                RootUiInfo.OneToOneList.Add(CreateUiTableInfo(Detail, Module));
            else
                RootUiInfo.DetailList.Add(CreateDetailTableUiInfo(ParentTableDef, Detail, Module));
            AddDetailUiInfo(RootUiInfo, Detail, Module);
        }
    }
    /// <summary>
    /// Creates detail table UI information.
    /// </summary>
    static public UiDetailTableInfo CreateDetailTableUiInfo(TableDef ParentTableDef, TableDef TableDef, DataModule Module)
    {
        UiDetailTableInfo Result = new();
        Result.ParentTableDef = ParentTableDef;
        Result.TableDef = TableDef;
        Result.Table = Module.GetTable(TableDef.Name);
        return Result;
    }
    /// <summary>
    /// Adds field UI information.
    /// </summary>
    static public void AddFieldUiInfo(UiTableInfo TableUiInfo, FieldDef Field, Control Control)
    {
        TableUiInfo.FieldList.Add(new UiFieldInfo
        {
            TableDef = TableUiInfo.TableDef,
            FieldDef = Field,
            FieldName = Field.Name,
            Control = Control,
            Table = TableUiInfo.Table
        });
    }
    
    // ● layout calculation
    /// <summary>
    /// Normalizes a column count.
    /// </summary>
    static public int NormalizeColumnCount(int ColumnCount)
    {
        if (ColumnCount < 1)
            return 1;
        if (ColumnCount > 4)
            return 4;
        return ColumnCount;
    }
    /// <summary>
    /// Returns true if a field is boolean.
    /// </summary>
    static public bool IsBooleanField(FieldDef Field)
    {
        return Field.Flags.HasFlag(FieldFlags.Boolean) || Field.DataType == DataFieldType.Boolean;
    }
    /// <summary>
    /// Returns true if a field can be displayed in a detail grid.
    /// </summary>
    static public bool IsDetailGridField(FieldDef Field)
    {
        return Field.IsBindable && !Field.IsMemo && !Field.IsLargeMemo && !Field.IsImage;
    }
    /// <summary>
    /// Splits bindable fields into visual groups and columns.
    /// </summary>
    static public Dictionary<string, List<List<FieldDef>>> SplitBindableGroups(TableDef TableDef, int ColumnCount)
    {
        Dictionary<string, List<List<FieldDef>>> Result = new();
        Dictionary<string, List<FieldDef>> Groups = TableDef.GetBindableFields()
            .Where(Field => !Field.IsLargeMemo)
            .GroupBy(Field => Field.Group)
            .ToDictionary(Group => Group.Key, Group => Group.ToList());
        int VisualColumnCount = NormalizeColumnCount(ColumnCount);
        int MaxControlsPerColumn = Ui.Settings.FormMaxControlsPerColumn;
        foreach (var Entry in Groups)
        {
            List<List<FieldDef>> Columns = new();
            for (int i = 0; i < VisualColumnCount; i++)
                Columns.Add(new List<FieldDef>());
            List<FieldDef> Fields = Entry.Value;
            for (int i = 0; i < Fields.Count; i++)
            {
                int ColumnIndex = i / MaxControlsPerColumn;
                if (ColumnIndex >= VisualColumnCount)
                    ColumnIndex = VisualColumnCount - 1;
                Columns[ColumnIndex].Add(Fields[i]);
            }
            Result[Entry.Key] = Columns;
        }
        return Result;
    }
 
    // ● ui creation - columns
    /// <summary>
    /// Creates the root grid of a field group.
    /// </summary>
    static public Grid CreateColumnRootGrid(int ColumnCount)
    {
        Grid Result = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        double ColumnWidth = Ui.Settings.FormColumnWidth;
        for (int i = 0; i < ColumnCount; i++)
        {
            Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(ColumnWidth)));
        } 
            
        return Result;
    }
    /// <summary>
    /// Creates a label-editor column grid.
    /// </summary>
    static public Grid CreateColumnGrid()
    {
        Grid Result = new();
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(28, GridUnitType.Star)));
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70, GridUnitType.Star)));
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2, GridUnitType.Star)));
        return Result;
    }
    /// <summary>
    /// Creates the visual columns of a field group.
    /// </summary>
    static public List<Grid> CreateGroupColumnGrids(Expander Expander, int ColumnCount)
    {
        List<Grid> Result = new();
        Grid Root = CreateColumnRootGrid(ColumnCount);
        Expander.Content = Root;
        for (int i = 0; i < ColumnCount; i++)
        {
            Grid ColumnGrid = CreateColumnGrid();
            ColumnGrid.Margin = i == 0 ? new Thickness(0, 12, 0, 0) : new Thickness(16, 12, 0, 0);
            Avalonia.Controls.Grid.SetColumn(ColumnGrid, i);
            Root.Children.Add(ColumnGrid);
            Result.Add(ColumnGrid);
        }
        return Result;
    }
    
    // ● ui creation - fields
    /// <summary>
    /// Creates all field groups of a table.
    /// </summary>
    static public void CreateFieldGroups(ItemUiContext UiContext, Control ParentControl, UiTableInfo TableUiInfo, ItemBinder Binder, int ColumnCount)
    {
        Dictionary<string, List<List<FieldDef>>> Groups = ItemPageUi.SplitBindableGroups(TableUiInfo.TableDef, ColumnCount);
        foreach (var Entry in Groups)
        {
            Expander Expander = ItemPageUi.CreateExpander(ParentControl, Entry.Key);
            List<Grid> ColumnGrids = ItemPageUi.CreateGroupColumnGrids(Expander, Entry.Value.Count);
            for (int i = 0; i < Entry.Value.Count; i++)
            {
                List<FieldDef> Fields = Entry.Value[i];
                Grid ColumnGrid = ColumnGrids[i];
                for (int j = 0; j < Fields.Count; j++)
                    AddControlRow(UiContext, ColumnGrid, j, Fields[j], Binder, TableUiInfo);
            }
        }
        CreateLargeMemoGroups(ParentControl, TableUiInfo, Binder);
    }
    /// <summary>
    /// Creates all large memo field groups of a table.
    /// </summary>
    static public void CreateLargeMemoGroups(Control ParentControl, UiTableInfo TableUiInfo, ItemBinder Binder)
    {
        List<FieldDef> Fields = TableUiInfo.TableDef.GetBindableFields().Where(Field => Field.IsLargeMemo).ToList();
        foreach (FieldDef Field in Fields)
        {
            Expander Expander = ItemPageUi.CreateExpander(ParentControl, Field.Title);
            Control Editor = ItemPageUi.CreateLargeMemoEditor(Field, Binder);
            Expander.Content = Editor;
            AddFieldUiInfo(TableUiInfo, Field, Editor);
        }
    }
    /// <summary>
    /// Adds a field editor row.
    /// </summary>
    static public void AddControlRow(ItemUiContext UiContext, Grid Grid, int RowIndex, FieldDef Field, ItemBinder Binder, UiTableInfo TableUiInfo)
    {
        Grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Control Control;
        if (ItemPageUi.IsBooleanField(Field))
        {
            CheckBox Box = new()
            {
                Content = Field.Title,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 6)
            };
            
            DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);
            
            Binder.Bind(Box, Field.Name, DataColumn, Field);
            Avalonia.Controls.Grid.SetRow(Box, RowIndex);
            Avalonia.Controls.Grid.SetColumn(Box, 1);
            Grid.Children.Add(Box);
            AddFieldUiInfo(TableUiInfo, Field, Box);
            return;
        }
        if (Field.IsImage)
        {
            Control = ItemPageUi.CreateImageControl(Field, Binder);
            Avalonia.Controls.Grid.SetRow(Control, RowIndex);
            Avalonia.Controls.Grid.SetColumn(Control, 0);
            Avalonia.Controls.Grid.SetColumnSpan(Control, 2);
            Grid.Children.Add(Control);
            AddFieldUiInfo(TableUiInfo, Field, Control);
            return;
        }
        TextBlock Label = CreateFieldLabel(Field);
        Control Editor = UiContext.CreateEditorFunc(Field, Binder);
        Avalonia.Controls.Grid.SetRow(Label, RowIndex);
        Avalonia.Controls.Grid.SetColumn(Label, 0);
        Avalonia.Controls.Grid.SetRow(Editor, RowIndex);
        Avalonia.Controls.Grid.SetColumn(Editor, 1);
        Grid.Children.Add(Label);
        Grid.Children.Add(Editor);
        AddFieldUiInfo(TableUiInfo, Field, Editor);
    }
    /// <summary>
    /// Creates a field label.
    /// </summary>
    static public TextBlock CreateFieldLabel(FieldDef Field)
    {
        string Title = Field.Title;
        if (Field.IsLookup && Title.EndsWith(" Id", StringComparison.OrdinalIgnoreCase))
            Title = Title.Substring(0, Title.Length - 3);
             
        //if (Field.IsLookup)
        return new TextBlock
        {
            Text = Title,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 6, 6)
        };
    }
    
    
    // ● ui creation - details
    /// <summary>
    /// Creates the first-level detail tabs under the top table tab.
    /// </summary>
    static public void CreateFirstLevelDetails(ItemUiContext UiContext, Control ParentControl)
    {
        List<UiDetailTableInfo> Details = UiContext.TopTableUiInfo.DetailList.Where(Detail => Detail.ParentTableDef == UiContext.TopTableUiInfo.TableDef).ToList();
        if (Details.Count == 0)
            return;
        TabControl TabControl = ItemPageUi.CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            TabControl.Items.Add(CreateDetailTabItem(UiContext, Detail, 1));
        ItemPageUi.AddChild(ParentControl, TabControl);
    }
    /// <summary>
    /// Creates all detail tabs from level two and deeper.
    /// </summary>
    static public void CreateChildLevelDetails(ItemUiContext UiContext, TabControl ParentTabControl)
    {
        foreach (UiDetailTableInfo Detail in UiContext.TopTableUiInfo.DetailList)
        {
            if (Detail.ParentTableDef == UiContext.TopTableUiInfo.TableDef)
                continue;
            int Level = GetTableLevel(UiContext, Detail.TableDef);
            ParentTabControl.Items.Add(CreateDetailTabItem(UiContext, Detail, Level));
        }
    }
    /// <summary>
    /// Returns the table level in the detail tree.
    /// </summary>
    static public int GetTableLevel(ItemUiContext UiContext, TableDef TableDef)
    {
        int Result = 0;
        TableDef Table = TableDef;
        while (Table != null && Table != UiContext.ModuleDef.Table)
        {
            Result++;
            Table = Table.Master;
        }
        return Result;
    }
    /// <summary>
    /// Creates a tab item for a detail table.
    /// </summary>
    static public TabItem CreateDetailTabItem(ItemUiContext UiContext, UiDetailTableInfo DetailUiInfo, int Level)
    {
        TabItem Result = new()
        {
            Header = DetailUiInfo.TableDef.Title
        };
        StackPanel Panel = ItemPageUi.CreateStackPanel();
        Result.Content = Panel;
        CreateDetail(UiContext, Panel, DetailUiInfo);
        return Result;
    }
    /// <summary>
    /// Creates a detail table UI.
    /// </summary>
    static public void CreateDetail(ItemUiContext UiContext, Control ParentControl, UiDetailTableInfo DetailUiInfo)
    {
        DockPanel Panel = new();
        Border ToolBarBorder = CreateDetailToolBarBorder(DetailUiInfo);
        DataGrid Grid = CreateDetailDataGrid(UiContext, DetailUiInfo.TableDef);
        DetailUiInfo.Grid = Grid;
        Panel.Children.Add(ToolBarBorder);
        Panel.Children.Add(Grid);
        ItemPageUi.AddChild(ParentControl, Panel);
    }
    /// <summary>
    /// Creates a detail toolbar border.
    /// </summary>
    static public Border CreateDetailToolBarBorder(UiDetailTableInfo DetailUiInfo)
    {
        Border Result = new();
        Result.Classes.Add("ToolbarContainer");
        DockPanel.SetDock(Result, Dock.Top);
        StackPanel ToolBarPanel = ItemPageUi.CreateStackPanel();
        ToolBarPanel.Classes.Add("ToolBar");
        ToolBarPanel.Height = 32;
        ToolBarPanel.IsVisible = false;
        Result.Child = ToolBarPanel;
        DetailUiInfo.ToolBarPanel = ToolBarPanel;
        return Result;
    }
    /// <summary>
    /// Creates a one-to-one detail table UI.
    /// </summary>
    static public void CreateOneToOneDetail(ItemUiContext UiContext, Control ParentControl, UiTableInfo TableUiInfo)
    {
        ItemBinder Binder = UiContext.CreateOneToOneBinder(TableUiInfo.TableDef);
        Binder.TableInfo = TableUiInfo;
        UiContext.Binders.Add(Binder);
        CreateFieldGroups(UiContext, ParentControl, TableUiInfo, Binder, UiContext.ColumnCount);
    }

    // ● ui creation - detail grids
    /// <summary>
    /// Creates a detail data grid.
    /// </summary>
    static public DataGrid CreateDetailDataGrid(ItemUiContext UiContext,TableDef TableDef)
    {
        DataGrid Result = new()
        {
            AutoGenerateColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 8, 0, 8)
        };
        CreateDetailGridColumns(Result, TableDef);
        BindDetailGrid(UiContext, Result, TableDef);
        return Result;
    }
    /// <summary>
    /// Creates the columns of a detail data grid.
    /// </summary>
    static public void CreateDetailGridColumns(DataGrid Grid, TableDef TableDef)
    {
        foreach (FieldDef Field in TableDef.GetBindableFields())
        {
            if (!ItemPageUi.IsDetailGridField(Field))
                continue;
            Grid.Columns.Add(CreateDetailGridColumn(Field));
        }
    }
    /// <summary>
    /// Creates a column for a detail data grid.
    /// </summary>
    static public DataGridColumn CreateDetailGridColumn(FieldDef Field)
    {
        if (Field.IsLookup)
            return DataGridBinder.CreateLookupColumn(Field);
        return DataGridBinder.CreateGridColumn(Field);
    }
    /// <summary>
    /// Binds a detail data grid.
    /// </summary>
    static public void BindDetailGrid(ItemUiContext UiContext, DataGrid Grid, TableDef TableDef)
    {
        Grid.ItemsSource = UiContext.Module.GetTable(TableDef.Name).DataView;
    }
 
    // ● ui creation - top table
    /// <summary>
    /// Creates the top table tab item.
    /// </summary>
    static public TabItem CreateTopTableTabItem(ItemUiContext UiContext)
    {
        TabItem Result = new()
        {
            Header = UiContext.ModuleDef.Table.Title
        };
        StackPanel TopPanel = ItemPageUi.CreateStackPanel();
        Result.Content = TopPanel;
        CreateFieldGroups(UiContext, TopPanel, UiContext.TopTableUiInfo, UiContext.ItemBinder, UiContext.ColumnCount);
        CreateOneToOneDetails(UiContext, TopPanel, UiContext.TopTableUiInfo.TableDef);
        CreateFirstLevelDetails(UiContext, TopPanel);
        return Result;
    }
    /// <summary>
    /// Creates one-to-one detail controls under a specified parent table.
    /// </summary>
    static public void CreateOneToOneDetails(ItemUiContext UiContext, Control ParentControl, TableDef ParentTableDef)
    {
        foreach (UiTableInfo TableUiInfo in UiContext.TopTableUiInfo.OneToOneList)
        {
            if (TableUiInfo.TableDef.Master != ParentTableDef)
                continue;
            CreateOneToOneDetail(UiContext, ParentControl, TableUiInfo);
        }
    }
    
    // ● data input/display controls
    /// <summary>
    /// Creates a large memo editor.
    /// </summary>
    static public Control CreateLargeMemoEditor(FieldDef Field, ItemBinder Binder)
    {
        TextBox Result = new();
        Result.AcceptsReturn = true;
        Result.TextWrapping = TextWrapping.NoWrap;
        Result.FontFamily = new FontFamily("Consolas");
        Result.MinHeight = 280;
        Result.MaxHeight = 500;
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 8, 0, 8);
        DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);
        Binder.BindMemo(Result, Field.Name, DataColumn, Field);
        return Result;
    }
    /// <summary>
    /// Creates an image editor placeholder.
    /// </summary>
    static public Control CreateImageControl(FieldDef Field, ItemBinder Binder)
    {
        StackPanel Result = new();
        TextBlock Label = new()
        {
            Text = Field.Title,
            Margin = new Thickness(0, 0, 0, 4)
        };
        Border Border = new()
        {
            Height = Ui.Settings.FormImageHeight,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            Child = new TextBlock
            {
                Text = "No Image",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Result.Margin = new Thickness(0, 0, 0, 6);
        Result.Children.Add(Label);
        Result.Children.Add(Border);
        return Result;
    }
    
    // ● ui creation - entry point
    /// <summary>
    /// Creates a single-page top table layout.
    /// </summary>
    static public void CreateSinglePageLayout(ItemUiContext UiContext)  
    {
        CreateFieldGroups(UiContext, UiContext.ParentControl, UiContext.TopTableUiInfo, UiContext.ItemBinder, UiContext.ColumnCount);
        CreateOneToOneDetails(UiContext, UiContext.ParentControl, UiContext.TopTableUiInfo.TableDef);
    }
    /// <summary>
    /// Creates a tabbed top table layout.
    /// </summary>
    static public void CreateTabbedTopLayout(ItemUiContext UiContext)
    {
        TabControl RootTabControl = ItemPageUi.CreateTabControl();
        TabItem TopTab = CreateTopTableTabItem(UiContext);
        RootTabControl.Items.Add(TopTab);
        CreateChildLevelDetails(UiContext, RootTabControl);
        ItemPageUi.AddChild(UiContext.ParentControl, RootTabControl);
    }
}