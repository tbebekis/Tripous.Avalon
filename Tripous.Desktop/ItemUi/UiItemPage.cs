namespace Tripous.Desktop;
 
/// <summary>
/// Builds the main layout of an item page, including field groups, field rows and top-table layouts.
/// </summary>
static public class UiItemPage
{
    // ● layout calculation
    /// <summary>
    /// Normalizes the requested visual column count to the supported range.
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
    /// Splits bindable fields into visual groups and distributes them across layout columns.
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
    /// Creates the root grid that hosts the visual columns of a field group.
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
    /// Creates a column grid containing the label, editor and trailing spacing columns.
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
    /// Creates the grouped field UI for a table.
    /// </summary>
    static public void CreateFieldGroups(UiItemContext context, Control ParentControl, UiTableInfo TableUiInfo, ItemBinder Binder, int ColumnCount)
    {
        Dictionary<string, List<List<FieldDef>>> Groups = UiItemPage.SplitBindableGroups(TableUiInfo.TableDef, ColumnCount);
        foreach (var Entry in Groups)
        {
            Expander Expander = UiFactory.CreateExpander(ParentControl, Entry.Key);
            List<Grid> ColumnGrids = UiItemPage.CreateGroupColumnGrids(Expander, Entry.Value.Count);
            for (int i = 0; i < Entry.Value.Count; i++)
            {
                List<FieldDef> Fields = Entry.Value[i];
                Grid ColumnGrid = ColumnGrids[i];
                for (int j = 0; j < Fields.Count; j++)
                    AddControlRow(context, ColumnGrid, j, Fields[j], Binder, TableUiInfo);
            }
        }
        CreateLargeMemoGroups(ParentControl, TableUiInfo, Binder);
    }
    /// <summary>
    /// Creates the standalone groups used by large memo fields of a table.
    /// </summary>
    static public void CreateLargeMemoGroups(Control ParentControl, UiTableInfo TableUiInfo, ItemBinder Binder)
    {
        List<FieldDef> Fields = TableUiInfo.TableDef.GetBindableFields().Where(Field => Field.IsLargeMemo).ToList();
        foreach (FieldDef Field in Fields)
        {
            Expander Expander = UiFactory.CreateExpander(ParentControl, Field.Title);
            Control Editor = UiFactory.CreateLargeMemoEditor(Field, Binder);
            Expander.Content = Editor;
            UiItemInfo.AddFieldUiInfo(TableUiInfo, Field, Editor);
        }
    }
    /// <summary>
    /// Adds the row used to display and bind a single field.
    /// </summary>
    static public void AddControlRow(UiItemContext context, Grid Grid, int RowIndex, FieldDef Field, ItemBinder Binder, UiTableInfo TableUiInfo)
    {
        Grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Control Control;
        if (UiItemPage.IsBooleanField(Field))
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
            UiItemInfo.AddFieldUiInfo(TableUiInfo, Field, Box);
            return;
        }
        if (Field.IsImage)
        {
            Control = UiFactory.CreateImageControl(Field, Binder);
            Avalonia.Controls.Grid.SetRow(Control, RowIndex);
            Avalonia.Controls.Grid.SetColumn(Control, 0);
            Avalonia.Controls.Grid.SetColumnSpan(Control, 2);
            Grid.Children.Add(Control);
            UiItemInfo.AddFieldUiInfo(TableUiInfo, Field, Control);
            return;
        }
        TextBlock Label = UiFactory.CreateFieldLabel(Field);
        Control Editor = context.CreateEditorFunc(Field, Binder);
        Avalonia.Controls.Grid.SetRow(Label, RowIndex);
        Avalonia.Controls.Grid.SetColumn(Label, 0);
        Avalonia.Controls.Grid.SetRow(Editor, RowIndex);
        Avalonia.Controls.Grid.SetColumn(Editor, 1);
        Grid.Children.Add(Label);
        Grid.Children.Add(Editor);
        UiItemInfo.AddFieldUiInfo(TableUiInfo, Field, Editor);
    }
 
    // ● ui creation - top table
    /// <summary>
    /// Creates the tab item used by the top table in a tabbed item page layout.
    /// </summary>
    static public TabItem CreateTopTableTabItem(UiItemContext context)
    {
        TabItem Result = new()
        {
            Header = context.ModuleDef.Table.Title
        };
        StackPanel TopPanel = UiFactory.CreateStackPanel();
        Result.Content = TopPanel;
        CreateFieldGroups(context, TopPanel, context.TopTableUiInfo, context.ItemBinder, context.ColumnCount);
        UiItemDetails.CreateOneToOneDetails(context, TopPanel, context.TopTableUiInfo.TableDef);
        UiItemDetails.CreateFirstLevelDetails(context, TopPanel);
        return Result;
    }
    
    // ● ui creation - entry point
    /// <summary>
    /// Creates a single-page layout for the top table and its one-to-one details.
    /// </summary>
    static public void CreateSinglePageLayout(UiItemContext context)  
    {
        CreateFieldGroups(context, context.ParentControl, context.TopTableUiInfo, context.ItemBinder, context.ColumnCount);
        UiItemDetails.CreateOneToOneDetails(context, context.ParentControl, context.TopTableUiInfo.TableDef);
    }
    /// <summary>
    /// Creates a tabbed layout for the top table and all multi-row detail tables.
    /// </summary>
    static public void CreateTabbedTopLayout(UiItemContext context)
    {
        TabControl RootTabControl = UiFactory.CreateTabControl();
        TabItem TopTab = CreateTopTableTabItem(context);
        RootTabControl.Items.Add(TopTab);
        UiItemDetails.CreateChildLevelDetails(context, RootTabControl);
        UiFactory.AddChild(context.ParentControl, RootTabControl);
    }
}
