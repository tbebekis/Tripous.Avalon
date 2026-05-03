namespace Tripous.Desktop;

/// <summary>
/// The item part of a <see cref="DataForm"/>
/// </summary>
public class ItemPage : UserControl
{
    // ● protected fields
    protected DataForm fDataForm;
    protected int fColumnCount = 2;

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
        return Field.IsBindable && !Field.IsMemo && !Field.IsImage;
    }
    /// <summary>
    /// Splits bindable fields into visual groups and columns.
    /// </summary>
    protected virtual Dictionary<string, List<List<FieldDef>>> SplitBindableGroups(TableDef TableDef, int ColumnCount)
    {
        Dictionary<string, List<List<FieldDef>>> Result = new();
        Dictionary<string, List<FieldDef>> Groups = TableDef.GetBindableGroups();
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
            Grid Grid = CreateColumnGrid();
            Grid.Margin = i == 0 ? new Thickness(0, 12, 0, 0) : new Thickness(16, 12, 0, 0);
            Grid.SetColumn(Grid, i);
            Root.Children.Add(Grid);
            Result.Add(Grid);
        }
        return Result;
    }

    // ● ui creation - fields
    /// <summary>
    /// Creates all field groups of a table.
    /// </summary>
    protected virtual void CreateFieldGroups(Control ParentControl, TableDef TableDef, ItemBinder Binder)
    {
        Dictionary<string, List<List<FieldDef>>> Groups = SplitBindableGroups(TableDef, fColumnCount);
        foreach (var Entry in Groups)
        {
            Expander Expander = CreateExpander(ParentControl, Entry.Key);
            List<Grid> ColumnGrids = CreateGroupColumnGrids(Expander, fColumnCount);
            for (int i = 0; i < Entry.Value.Count; i++)
            {
                List<FieldDef> Fields = Entry.Value[i];
                Grid Grid = ColumnGrids[i];
                FieldDef FieldDef;
                int RowIndex;
                for (int j = 0; j < Fields.Count; j++)
                {
                    RowIndex = j;
                    FieldDef = Fields[j];
                    AddControlRow(Grid, RowIndex, FieldDef, Binder);
                }
            }
        }
    }
    /// <summary>
    /// Adds a field editor row.
    /// </summary>
    protected virtual void AddControlRow(Grid Grid, int RowIndex, FieldDef Field, ItemBinder Binder)
    {
        Grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        if (IsBooleanField(Field))
        {
            CheckBox Box = new()
            {
                Content = Field.Title,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 6)
            };
            Binder.Bind(Box, Field.Name, Field);
            Grid.SetRow(Box, RowIndex);
            Grid.SetColumn(Box, 0);
            Grid.SetColumnSpan(Box, 2);
            Grid.Children.Add(Box);
            return;
        }
        if (Field.IsImage)
        {
            Control ImageControl = CreateImageControl(Field, Binder);
            Grid.SetRow(ImageControl, RowIndex);
            Grid.SetColumn(ImageControl, 0);
            Grid.SetColumnSpan(ImageControl, 2);
            Grid.Children.Add(ImageControl);
            return;
        }
        TextBlock Label = CreateFieldLabel(Field);
        Control Editor = CreateEditor(Field, Binder);
        Grid.SetRow(Label, RowIndex);
        Grid.SetColumn(Label, 0);
        Grid.SetRow(Editor, RowIndex);
        Grid.SetColumn(Editor, 1);
        Grid.Children.Add(Label);
        Grid.Children.Add(Editor);
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
    /// Creates the detail area of a table.
    /// </summary>
    protected virtual void CreateDetails(Control ParentControl, TableDef ParentTable)
    {
        if (ParentTable.Details.Count == 0)
            return;
        if (ParentTable.Details.Count == 1)
        {
            CreateDetail(ParentControl, ParentTable.Details[0]);
            return;
        }
        TabControl TabControl = CreateDetailTabControl(ParentTable);
        AddChild(ParentControl, TabControl);
    }
    /// <summary>
    /// Creates a tab control for detail tables.
    /// </summary>
    protected virtual TabControl CreateDetailTabControl(TableDef ParentTable)
    {
        TabControl Result = new();
        foreach (TableDef Detail in ParentTable.Details)
            Result.Items.Add(CreateDetailTabItem(Detail));
        return Result;
    }
    /// <summary>
    /// Creates a tab item for a detail table.
    /// </summary>
    protected virtual TabItem CreateDetailTabItem(TableDef TableDef)
    {
        TabItem Result = new()
        {
            Header = TableDef.Title
        };
        StackPanel Panel = CreateStackPanel();
        Result.Content = Panel;
        CreateDetail(Panel, TableDef);
        return Result;
    }
    /// <summary>
    /// Creates a detail table UI.
    /// </summary>
    protected virtual void CreateDetail(Control ParentControl, TableDef TableDef)
    {
        if (TableDef.IsOneToOne)
        {
            CreateOneToOneDetail(ParentControl, TableDef);
            return;
        }
        Expander Expander = CreateExpander(ParentControl, TableDef.Title);
        StackPanel Panel = CreateStackPanel();
        Expander.Content = Panel;
        DataGrid Grid = CreateDetailDataGrid(TableDef);
        Panel.Children.Add(Grid);
        CreateDetails(Panel, TableDef);
    }
    /// <summary>
    /// Creates a one-to-one detail table UI.
    /// </summary>
    protected virtual void CreateOneToOneDetail(Control ParentControl, TableDef TableDef)
    {
        ItemBinder Binder = CreateOneToOneBinder(TableDef);
        Binders.Add(Binder);
        Expander Expander = CreateExpander(ParentControl, TableDef.Title);
        StackPanel Panel = CreateStackPanel();
        Expander.Content = Panel;
        CreateFieldGroups(Panel, TableDef, Binder);
        CreateDetails(Panel, TableDef);
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
        Grid.ItemsSource = Module[TableDef.Name].DataView;
    }

    // ● ui creation - top table
    /// <summary>
    /// Creates a single-page top table layout.
    /// </summary>
    protected virtual void CreateSinglePageLayout(Control ParentControl)
    {
        CreateFieldGroups(ParentControl, ModuleDef.Table, ItemBinder);
        CreateDetails(ParentControl, ModuleDef.Table);
    }
    /// <summary>
    /// Creates a tabbed top table layout.
    /// </summary>
    protected virtual void CreateTabbedTopLayout(Control ParentControl)
    {
        TabControl TabControl = new();
        TabItem TopTab = new()
        {
            Header = ModuleDef.Table.Title
        };
        StackPanel TopPanel = CreateStackPanel();
        TopTab.Content = TopPanel;
        CreateFieldGroups(TopPanel, ModuleDef.Table, ItemBinder);
        TabControl.Items.Add(TopTab);
        foreach (TableDef Detail in ModuleDef.Table.Details)
            TabControl.Items.Add(CreateDetailTabItem(Detail));
        AddChild(ParentControl, TabControl);
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
        ScrollViewer ScrollViewer = CreateScrollViewer();
        StackPanel Root = CreateStackPanel();
        ScrollViewer.Content = Root;
        Content = ScrollViewer;
        if (ModuleDef.Table.Details.Count > 1)
            CreateTabbedTopLayout(Root);
        else
            CreateSinglePageLayout(Root);
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