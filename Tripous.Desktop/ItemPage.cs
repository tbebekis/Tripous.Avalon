namespace Tripous.Desktop;
 
/// <summary>
/// UI information regarding a single-row <see cref="TableDef"/> in an <see cref="ItemPage"/> form.
/// </summary>
public class UiTableInfo
{
    // ● public
    /// <summary>
    /// The table definition.
    /// </summary>
    public TableDef TableDef { get; set; }
    /// <summary>
    /// <see cref="FieldDef"/> to <see cref="Control"/> association list, for top tables and IsOneToOne = true single-row detail tables.
    /// </summary>
    public List<UiFieldInfo> FieldList { get; set; } = new();
    /// <summary>
    /// When there are details having IsOneToOne = true and IsUiVisible = true, go here.
    /// </summary>
    public List<UiTableInfo> OneToOneList { get; } = new();
    /// <summary>
    /// Multi-row detail tables having IsUiVisible = true, go here.
    /// </summary>
    public List<UiDetailTableInfo> DetailList { get; } = new();
    /// <summary>
    /// The table.
    /// </summary>
    public MemTable Table { get; set; }
}

/// <summary>
/// Ui information regarding the associaton of a <see cref="FieldDef"/> and a <see cref="Control"/>
/// </summary>
public class UiFieldInfo
{
    // ● public
    /// <summary>
    /// The table definition.
    /// </summary>
    public TableDef TableDef { get; set; }
    /// <summary>
    /// The field definition
    /// </summary>
    public FieldDef FieldDef { get; set; }
    /// <summary>
    /// The control
    /// </summary>
    public Control Control { get; set; }
    /// <summary>
    /// The field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The table.
    /// </summary>
    public MemTable Table { get; set; }
}

/// <summary>
/// Information about a detail grid UI.
/// </summary>
public class UiDetailTableInfo 
{
    // ● public
    /// <summary>
    /// The toolbar panel of the detail grid.
    /// </summary>
    public StackPanel ToolBarPanel { get; set; }
    /// <summary>
    /// The detail grid.
    /// </summary>
    public DataGrid Grid { get; set; }
    /// <summary>
    /// The parent table definition.
    /// </summary>
    public TableDef ParentTableDef { get; set; }
    /// <summary>
    /// The table definition.
    /// </summary>
    public TableDef TableDef { get; set; }
    /// <summary>
    /// The table.
    /// </summary>
    public MemTable Table { get; set; }
}

/// <summary>
/// The item part of a <see cref="DataForm"/>
/// </summary>
public class ItemPage : UserControl
{
    // ● protected fields
    protected DataForm fDataForm;
    protected int fColumnCount = 2;
    protected UiTableInfo TopTableUiInfo = new();

    // ● row providers
    /// <summary>
    /// Returns the row provider host.
    /// </summary>
    protected virtual IRowProviderHost GetRowProviderHost()
    {
        return Module.RowProviderHost;
    }
    /// <summary>
    /// Returns the row provider of a table.
    /// </summary>
    protected virtual IRowProvider GetRowProvider(TableDef TableDef)
    {
        return GetRowProviderHost().GetRowProvider(TableDef.Name);
    }
    /// <summary>
    /// Creates a binder for a one-to-one detail table.
    /// </summary>
    protected virtual ItemBinder CreateOneToOneBinder(TableDef TableDef)
    {
        ItemBinder Result = new();
        Result.RowProvider = GetRowProvider(TableDef);
        return Result;
    }

    // ● ui info
    /// <summary>
    /// Creates the UI information tree for the top table.
    /// </summary>
    protected virtual UiTableInfo CreateTopTableUiInfo()
    {
        UiTableInfo Result = CreateUiTableInfo(ModuleDef.Table);
        AddDetailUiInfo(Result, ModuleDef.Table);
        return Result;
    }
    /// <summary>
    /// Creates UI information for a table.
    /// </summary>
    protected virtual UiTableInfo CreateUiTableInfo(TableDef TableDef)
    {
        UiTableInfo Result = new();
        Result.TableDef = TableDef;
        Result.Table = Module.GetTable(TableDef.Name);
        return Result;
    }
    /// <summary>
    /// Adds detail UI information recursively.
    /// </summary>
    protected virtual void AddDetailUiInfo(UiTableInfo RootUiInfo, TableDef ParentTableDef)
    {
        foreach (TableDef Detail in ParentTableDef.Details)
        {
            if (!Detail.IsUiVisible)
                continue;
            if (Detail.IsOneToOne)
                RootUiInfo.OneToOneList.Add(CreateUiTableInfo(Detail));
            else
                RootUiInfo.DetailList.Add(CreateDetailTableUiInfo(ParentTableDef, Detail));
            AddDetailUiInfo(RootUiInfo, Detail);
        }
    }
    /// <summary>
    /// Creates detail table UI information.
    /// </summary>
    protected virtual UiDetailTableInfo CreateDetailTableUiInfo(TableDef ParentTableDef, TableDef TableDef)
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
    protected virtual void AddFieldUiInfo(UiTableInfo TableUiInfo, FieldDef Field, Control Control)
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
    protected virtual int NormalizeColumnCount(int ColumnCount)
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
    protected virtual bool IsBooleanField(FieldDef Field)
    {
        return Field.Flags.HasFlag(FieldFlags.Boolean) || Field.DataType == DataFieldType.Boolean;
    }
    /// <summary>
    /// Returns true if a field can be displayed in a detail grid.
    /// </summary>
    protected virtual bool IsDetailGridField(FieldDef Field)
    {
        return Field.IsBindable && !Field.IsMemo && !Field.IsLargeMemo && !Field.IsImage;
    }
    /// <summary>
    /// Splits bindable fields into visual groups and columns.
    /// </summary>
    protected virtual Dictionary<string, List<List<FieldDef>>> SplitBindableGroups(TableDef TableDef, int ColumnCount)
    {
        Dictionary<string, List<List<FieldDef>>> Result = new();
        Dictionary<string, List<FieldDef>> Groups = TableDef.GetBindableFields()
            .Where(Field => !Field.IsLargeMemo)
            .GroupBy(Field => Field.Group)
            .ToDictionary(Group => Group.Key, Group => Group.ToList());
        foreach (var Entry in Groups)
        {
            List<FieldDef> Fields = Entry.Value;
            int FieldCount = Fields.Count;
            int ActualColumnCount;
            if (FieldCount <= 3)
                ActualColumnCount = 1;
            else if (FieldCount <= 6)
                ActualColumnCount = Math.Min(2, ColumnCount);
            else
                ActualColumnCount = ColumnCount;
            List<List<FieldDef>> Columns = new();
            for (int i = 0; i < ActualColumnCount; i++)
                Columns.Add(new List<FieldDef>());
            int ItemsPerColumn = (int)Math.Ceiling((double)FieldCount / ActualColumnCount);
            for (int i = 0; i < FieldCount; i++)
            {
                int ColumnIndex = i / ItemsPerColumn;
                Columns[ColumnIndex].Add(Fields[i]);
            }
            Result[Entry.Key] = Columns;
        }
        return Result;
    }

    // ● ui creation - common
    /// <summary>
    /// Adds a child control to a parent control.
    /// </summary>
    protected virtual void AddChild(Control ParentControl, Control Child)
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
    protected virtual ScrollViewer CreateScrollViewer()
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
    protected virtual StackPanel CreateStackPanel()
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
    protected virtual Expander CreateExpander(Control ParentControl, string Caption)
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
    protected virtual TabControl CreateTabControl()
    {
        return new TabControl
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 8, 0, 0)
        };
    }

    // ● ui creation - columns
    /// <summary>
    /// Creates the root grid of a field group.
    /// </summary>
    protected virtual Grid CreateColumnRootGrid(int ColumnCount)
    {
        Grid Result = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        for (int i = 0; i < ColumnCount; i++)
            Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        return Result;
    }
    /// <summary>
    /// Creates a label-editor column grid.
    /// </summary>
    protected virtual Grid CreateColumnGrid()
    {
        Grid Result = new();
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(33, GridUnitType.Star)));
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(67, GridUnitType.Star)));
        return Result;
    }
    /// <summary>
    /// Creates the visual columns of a field group.
    /// </summary>
    protected virtual List<Grid> CreateGroupColumnGrids(Expander Expander, int ColumnCount)
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
    protected virtual void CreateFieldGroups(Control ParentControl, UiTableInfo TableUiInfo, ItemBinder Binder)
    {
        Dictionary<string, List<List<FieldDef>>> Groups = SplitBindableGroups(TableUiInfo.TableDef, fColumnCount);
        foreach (var Entry in Groups)
        {
            Expander Expander = CreateExpander(ParentControl, Entry.Key);
            List<Grid> ColumnGrids = CreateGroupColumnGrids(Expander, Entry.Value.Count);
            for (int i = 0; i < Entry.Value.Count; i++)
            {
                List<FieldDef> Fields = Entry.Value[i];
                Grid ColumnGrid = ColumnGrids[i];
                for (int j = 0; j < Fields.Count; j++)
                    AddControlRow(ColumnGrid, j, Fields[j], Binder, TableUiInfo);
            }
        }
        CreateLargeMemoGroups(ParentControl, TableUiInfo, Binder);
    }
    /// <summary>
    /// Creates all large memo field groups of a table.
    /// </summary>
    protected virtual void CreateLargeMemoGroups(Control ParentControl, UiTableInfo TableUiInfo, ItemBinder Binder)
    {
        List<FieldDef> Fields = TableUiInfo.TableDef.GetBindableFields().Where(Field => Field.IsLargeMemo).ToList();
        foreach (FieldDef Field in Fields)
        {
            Expander Expander = CreateExpander(ParentControl, Field.Title);
            Control Editor = CreateLargeMemoEditor(Field, Binder);
            Expander.Content = Editor;
            AddFieldUiInfo(TableUiInfo, Field, Editor);
        }
    }
    /// <summary>
    /// Adds a field editor row.
    /// </summary>
    protected virtual void AddControlRow(Grid Grid, int RowIndex, FieldDef Field, ItemBinder Binder, UiTableInfo TableUiInfo)
    {
        Grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Control Control;
        if (IsBooleanField(Field))
        {
            CheckBox Box = new()
            {
                Content = Field.Title,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 6)
            };
            Binder.Bind(Box, Field.Name, Field);
            Avalonia.Controls.Grid.SetRow(Box, RowIndex);
            Avalonia.Controls.Grid.SetColumn(Box, 0);
            Avalonia.Controls.Grid.SetColumnSpan(Box, 2);
            Grid.Children.Add(Box);
            AddFieldUiInfo(TableUiInfo, Field, Box);
            return;
        }
        if (Field.IsImage)
        {
            Control = CreateImageControl(Field, Binder);
            Avalonia.Controls.Grid.SetRow(Control, RowIndex);
            Avalonia.Controls.Grid.SetColumn(Control, 0);
            Avalonia.Controls.Grid.SetColumnSpan(Control, 2);
            Grid.Children.Add(Control);
            AddFieldUiInfo(TableUiInfo, Field, Control);
            return;
        }
        TextBlock Label = CreateFieldLabel(Field);
        Control Editor = CreateEditor(Field, Binder);
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
    protected virtual TextBlock CreateFieldLabel(FieldDef Field)
    {
        return new TextBlock
        {
            Text = Field.Title,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 6, 6)
        };
    }
    /// <summary>
    /// Creates a field editor.
    /// </summary>
    protected virtual Control CreateEditor(FieldDef Field, ItemBinder Binder)
    {
        Control Result;
        if (Field.IsLookup)
        {
            ComboBox Box = new();
            Binder.BindLookup(Box, Field.Name, Field);
            Result = Box;
        }
        else if (Field.IsDateTime)
        {
            DatePicker Box = new();
            Binder.Bind(Box, Field.Name, Field);
            Result = Box;
        }
        else
        {
            TextBox Box = new();
            if (Field.IsNumeric)
                Box.TextAlignment = TextAlignment.Right;
            if (Field.IsMemo)
                Binder.BindMemo(Box, Field.Name, Field);
            else
                Binder.Bind(Box, Field.Name, Field);
            Result = Box;
        }
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 0, 0, 6);
        return Result;
    }
    /// <summary>
    /// Creates a large memo editor.
    /// </summary>
    protected virtual Control CreateLargeMemoEditor(FieldDef Field, ItemBinder Binder)
    {
        TextBox Result = new();
        Result.AcceptsReturn = true;
        Result.TextWrapping = TextWrapping.NoWrap;
        Result.FontFamily = new FontFamily("Consolas");
        Result.MinHeight = 280;
        Result.MaxHeight = 500;
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 8, 0, 8);
        Binder.BindMemo(Result, Field.Name, Field);
        return Result;
    }
    /// <summary>
    /// Creates an image editor placeholder.
    /// </summary>
    protected virtual Control CreateImageControl(FieldDef Field, ItemBinder Binder)
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

    // ● ui creation - details
    /// <summary>
    /// Creates the first-level detail tabs under the top table tab.
    /// </summary>
    protected virtual void CreateFirstLevelDetails(Control ParentControl)
    {
        List<UiDetailTableInfo> Details = TopTableUiInfo.DetailList.Where(Detail => Detail.ParentTableDef == TopTableUiInfo.TableDef).ToList();
        if (Details.Count == 0)
            return;
        TabControl TabControl = CreateTabControl();
        foreach (UiDetailTableInfo Detail in Details)
            TabControl.Items.Add(CreateDetailTabItem(Detail, 1));
        AddChild(ParentControl, TabControl);
    }
    /// <summary>
    /// Creates all detail tabs from level two and deeper.
    /// </summary>
    protected virtual void CreateChildLevelDetails(TabControl ParentTabControl)
    {
        foreach (UiDetailTableInfo Detail in TopTableUiInfo.DetailList)
        {
            if (Detail.ParentTableDef == TopTableUiInfo.TableDef)
                continue;
            int Level = GetTableLevel(Detail.TableDef);
            ParentTabControl.Items.Add(CreateDetailTabItem(Detail, Level));
        }
    }
    /// <summary>
    /// Returns the table level in the detail tree.
    /// </summary>
    protected virtual int GetTableLevel(TableDef TableDef)
    {
        int Result = 0;
        TableDef Table = TableDef;
        while (Table != null && Table != ModuleDef.Table)
        {
            Result++;
            Table = Table.Master;
        }
        return Result;
    }
    /// <summary>
    /// Creates a tab item for a detail table.
    /// </summary>
    protected virtual TabItem CreateDetailTabItem(UiDetailTableInfo DetailUiInfo, int Level)
    {
        TabItem Result = new()
        {
            Header = DetailUiInfo.TableDef.Title
        };
        StackPanel Panel = CreateStackPanel();
        Result.Content = Panel;
        CreateDetail(Panel, DetailUiInfo);
        return Result;
    }
    /// <summary>
    /// Creates a detail table UI.
    /// </summary>
    protected virtual void CreateDetail(Control ParentControl, UiDetailTableInfo DetailUiInfo)
    {
        DockPanel Panel = new();
        Border ToolBarBorder = CreateDetailToolBarBorder(DetailUiInfo);
        DataGrid Grid = CreateDetailDataGrid(DetailUiInfo.TableDef);
        DetailUiInfo.Grid = Grid;
        Panel.Children.Add(ToolBarBorder);
        Panel.Children.Add(Grid);
        AddChild(ParentControl, Panel);
    }
    /// <summary>
    /// Creates a detail toolbar border.
    /// </summary>
    protected virtual Border CreateDetailToolBarBorder(UiDetailTableInfo DetailUiInfo)
    {
        Border Result = new();
        Result.Classes.Add("ToolbarContainer");
        DockPanel.SetDock(Result, Dock.Top);
        StackPanel ToolBarPanel = CreateStackPanel();
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
    protected virtual void CreateOneToOneDetail(Control ParentControl, UiTableInfo TableUiInfo)
    {
        ItemBinder Binder = CreateOneToOneBinder(TableUiInfo.TableDef);
        Binders.Add(Binder);
        CreateFieldGroups(ParentControl, TableUiInfo, Binder);
    }

    // ● ui creation - detail grids
    /// <summary>
    /// Creates a detail data grid.
    /// </summary>
    protected virtual DataGrid CreateDetailDataGrid(TableDef TableDef)
    {
        DataGrid Result = new()
        {
            AutoGenerateColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 8, 0, 8)
        };
        CreateDetailGridColumns(Result, TableDef);
        BindDetailGrid(Result, TableDef);
        return Result;
    }
    /// <summary>
    /// Creates the columns of a detail data grid.
    /// </summary>
    protected virtual void CreateDetailGridColumns(DataGrid Grid, TableDef TableDef)
    {
        foreach (FieldDef Field in TableDef.GetBindableFields())
        {
            if (!IsDetailGridField(Field))
                continue;
            Grid.Columns.Add(CreateDetailGridColumn(Field));
        }
    }
    /// <summary>
    /// Creates a column for a detail data grid.
    /// </summary>
    protected virtual DataGridColumn CreateDetailGridColumn(FieldDef Field)
    {
        if (Field.IsLookup)
            return DataGridBinder.CreateLookupColumn(Field);
        return DataGridBinder.CreateGridColumn(Field);
    }
    /// <summary>
    /// Binds a detail data grid.
    /// </summary>
    protected virtual void BindDetailGrid(DataGrid Grid, TableDef TableDef)
    {
        Grid.ItemsSource = Module.GetTable(TableDef.Name).DataView;
    }

    // ● ui creation - top table
    /// <summary>
    /// Creates a single-page top table layout.
    /// </summary>
    protected virtual void CreateSinglePageLayout(Control ParentControl)
    {
        CreateFieldGroups(ParentControl, TopTableUiInfo, ItemBinder);
        CreateOneToOneDetails(ParentControl, TopTableUiInfo.TableDef);
    }
    /// <summary>
    /// Creates a tabbed top table layout.
    /// </summary>
    protected virtual void CreateTabbedTopLayout(Control ParentControl)
    {
        TabControl RootTabControl = CreateTabControl();
        TabItem TopTab = CreateTopTableTabItem();
        RootTabControl.Items.Add(TopTab);
        CreateChildLevelDetails(RootTabControl);
        AddChild(ParentControl, RootTabControl);
    }
    /// <summary>
    /// Creates the top table tab item.
    /// </summary>
    protected virtual TabItem CreateTopTableTabItem()
    {
        TabItem Result = new()
        {
            Header = ModuleDef.Table.Title
        };
        StackPanel TopPanel = CreateStackPanel();
        Result.Content = TopPanel;
        CreateFieldGroups(TopPanel, TopTableUiInfo, ItemBinder);
        CreateOneToOneDetails(TopPanel, TopTableUiInfo.TableDef);
        CreateFirstLevelDetails(TopPanel);
        return Result;
    }
    /// <summary>
    /// Creates one-to-one detail controls under a specified parent table.
    /// </summary>
    protected virtual void CreateOneToOneDetails(Control ParentControl, TableDef ParentTableDef)
    {
        foreach (UiTableInfo TableUiInfo in TopTableUiInfo.OneToOneList)
        {
            if (TableUiInfo.TableDef.Master != ParentTableDef)
                continue;
            CreateOneToOneDetail(ParentControl, TableUiInfo);
        }
    }

    // ● binding
    /// <summary>
    /// Refreshes all binders.
    /// </summary>
    protected virtual void Refresh()
    {
        foreach (ItemBinder Binder in Binders)
            Binder.Refresh();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemPage()
    {
        ItemBinder = new();
        Binders = new();
        ItemBinder.CurrentRowChanging += (s, ea) => CurrentRowChanging?.Invoke(this, EventArgs.Empty);
        ItemBinder.CurrentRowChanged += (s, ea) => CurrentRowChanged?.Invoke(this, EventArgs.Empty);
    }

    // ● public methods
    /// <summary>
    /// Binds this instance.
    /// </summary>
    public virtual void Bind()
    {
        Bind(Ui.Settings.FormColumnCount);
    }
    /// <summary>
    /// Binds this instance.
    /// </summary>
    public virtual void Bind(int ColumnCount)
    {
        fColumnCount = NormalizeColumnCount(ColumnCount);
        Binders.Clear();
        Binders.Add(ItemBinder);
        ItemBinder.RowProvider = GetRowProvider(ModuleDef.Table);
        TopTableUiInfo = CreateTopTableUiInfo();
        ScrollViewer ScrollViewer = CreateScrollViewer();
        StackPanel Root = CreateStackPanel();
        ScrollViewer.Content = Root;
        Content = ScrollViewer;
        if (TopTableUiInfo.DetailList.Count == 0)
            CreateSinglePageLayout(Root);
        else
            CreateTabbedTopLayout(Root);
    }

    // ● properties
    /// <summary>
    /// The main item binder.
    /// </summary>
    public ItemBinder ItemBinder { get; }
    /// <summary>
    /// The binders of this instance.
    /// </summary>
    public List<ItemBinder> Binders { get; }
    /// <summary>
    /// The current data row.
    /// </summary>
    public DataRow CurrentRow => ItemBinder.CurrentRow;
    /// <summary>
    /// The parent form.
    /// </summary>
    public DataForm DataForm
    {
        get => fDataForm;
        set
        {
            fDataForm = value;
            ItemBinder.RowProvider = null;
            if (fDataForm != null)
                ItemBinder.RowProvider = GetRowProvider(ModuleDef.Table);
        }
    }
    /// <summary>
    /// Form context.
    /// </summary>
    public DataFormContext DataFormContext => DataForm.DataFormContext;
    /// <summary>
    /// The form definition.
    /// </summary>
    public FormDef FormDef => DataFormContext.FormDef;
    /// <summary>
    /// The module definition.
    /// </summary>
    public ModuleDef ModuleDef => DataFormContext.ModuleDef;
    /// <summary>
    /// The data module.
    /// </summary>
    public DataModule Module => DataFormContext.Module;
    /// <summary>
    /// Form actions the form is not allowed to execute.
    /// </summary>
    public DataFormAction InvalidActions => DataFormContext.InvalidActions;
    /// <summary>
    /// The first action the form should execute after initialization.
    /// </summary>
    public DataFormAction StartAction => DataFormContext.StartAction;

    // ● events
    /// <summary>
    /// Occurs before the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanging;
    /// <summary>
    /// Occurs after the current row changes.
    /// </summary>
    public event EventHandler CurrentRowChanged;
}
