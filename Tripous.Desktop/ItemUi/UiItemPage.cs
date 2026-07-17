/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

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
        if (ColumnCount > 3)
            return 3;
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
        if (Field == null)
            return false;
        if (Field.TableDef != null && Field.TableDef.IsDetail && Field.Name.IsSameText(Field.TableDef.DetailField))
            return false;
        return Field.IsBindable && !Field.IsMemo && !Field.IsLargeMemo && !Field.IsImage;
    }
    /// <summary>
    /// Returns the visual column count that fits in a column root grid.
    /// </summary>
    static public int GetFittedColumnCount(Grid Root)
    {
        int RequestedCount = Root.Tag is int Tag ? NormalizeColumnCount(Tag) : NormalizeColumnCount(Root.ColumnDefinitions.Count);
        double Width = Root.Bounds.Width - 12;
        double ColumnWidth = 320;
        double Gap = 16;
        if (Width <= 0)
            return RequestedCount;
        int Result = (int)Math.Floor((Width + Gap) / (ColumnWidth + Gap));
        if (Result < 1)
            return 1;
        if (Result > RequestedCount)
            return RequestedCount;
        return Result;
    }
    /// <summary>
    /// Assigns star column definitions to a column root grid.
    /// </summary>
    static public void SetColumnRootDefinitions(Grid Root, int ColumnCount)
    {
        Root.ColumnDefinitions.Clear();
        for (int i = 0; i < ColumnCount; i++)
            Root.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
    }
    /// <summary>
    /// Splits bindable fields into visual groups and distributes them across layout columns.
    /// </summary>
    static public Dictionary<string, List<List<FieldDef>>> SplitBindableGroups(TableDef TableDef, int ColumnCount)
    {
        Dictionary<string, List<List<FieldDef>>> Result = new();
        Dictionary<string, List<FieldDef>> Groups = new();
        foreach (var Entry in TableDef.GetBindableGroups())
        {
            List<FieldDef> Fields = Entry.Value.Where(Field => !Field.IsLargeMemo).ToList();
            if (Fields.Count > 0)
                Groups[Entry.Key] = Fields;
        }
        int VisualColumnCount = NormalizeColumnCount(ColumnCount);
        foreach (var Entry in Groups)
        {
            List<List<FieldDef>> Columns = new();
            for (int i = 0; i < VisualColumnCount; i++)
                Columns.Add(new List<FieldDef>());
            List<FieldDef> Fields = Entry.Value;
            int RowsPerColumn = Math.Max(1, (int)Math.Ceiling((double)Fields.Count / VisualColumnCount));
            for (int i = 0; i < Fields.Count; i++)
            {
                int ColumnIndex = i / RowsPerColumn;
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

        ColumnCount = NormalizeColumnCount(ColumnCount);
        Result.Tag = ColumnCount;
        SetColumnRootDefinitions(Result, ColumnCount);
        Result.SizeChanged += (Sender, Args) => ApplyResponsiveColumnRootLayout(Result);
            
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
    /// <summary>
    /// Returns the visual column grids of a column root grid.
    /// </summary>
    static public List<Grid> GetVisualColumnGrids(Grid Root)
    {
        return Root.Children.OfType<Grid>().OrderBy(Avalonia.Controls.Grid.GetColumn).ToList();
    }
    /// <summary>
    /// Extracts existing control rows from a column root grid.
    /// </summary>
    static public List<List<Control>> ExtractControlRows(Grid Root)
    {
        List<List<Control>> Result = new();
        foreach (Grid ColumnGrid in GetVisualColumnGrids(Root))
        {
            int RowCount = ColumnGrid.RowDefinitions.Count;
            for (int RowIndex = 0; RowIndex < RowCount; RowIndex++)
            {
                List<Control> RowControls = ColumnGrid.Children
                    .Where(Item => Avalonia.Controls.Grid.GetRow(Item) == RowIndex)
                    .OrderBy(Avalonia.Controls.Grid.GetColumn)
                    .ToList();
                if (RowControls.Count > 0)
                    Result.Add(RowControls);
            }
            ColumnGrid.Children.Clear();
            ColumnGrid.RowDefinitions.Clear();
        }
        return Result;
    }
    /// <summary>
    /// Adds an existing control row to a visual column grid.
    /// </summary>
    static public void AddExistingControlRow(Grid ColumnGrid, int RowIndex, List<Control> Controls)
    {
        ColumnGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        foreach (Control Control in Controls)
        {
            Avalonia.Controls.Grid.SetRow(Control, RowIndex);
            ColumnGrid.Children.Add(Control);
        }
    }
    /// <summary>
    /// Rebuilds the visual columns of a column root grid.
    /// </summary>
    static public void RebuildColumnRootColumns(Grid Root, List<List<Control>> Rows, int ColumnCount)
    {
        List<Grid> ColumnGrids = new();
        int RowsPerColumn;
        ColumnCount = NormalizeColumnCount(ColumnCount);
        RowsPerColumn = Math.Max(1, (int)Math.Ceiling((double)Rows.Count / ColumnCount));
        Root.Children.Clear();
        SetColumnRootDefinitions(Root, ColumnCount);
        for (int i = 0; i < ColumnCount; i++)
        {
            Grid ColumnGrid = CreateColumnGrid();
            ColumnGrid.Margin = i == 0 ? new Thickness(0, 12, 0, 0) : new Thickness(16, 12, 0, 0);
            Avalonia.Controls.Grid.SetColumn(ColumnGrid, i);
            Root.Children.Add(ColumnGrid);
            ColumnGrids.Add(ColumnGrid);
        }
        for (int i = 0; i < Rows.Count; i++)
        {
            int ColumnIndex = i / RowsPerColumn;
            if (ColumnIndex >= ColumnCount)
                ColumnIndex = ColumnCount - 1;
            AddExistingControlRow(ColumnGrids[ColumnIndex], ColumnGrids[ColumnIndex].RowDefinitions.Count, Rows[i]);
        }
    }
    /// <summary>
    /// Applies responsive visual column layout to a column root grid.
    /// </summary>
    static public void ApplyResponsiveColumnRootLayout(Grid Root)
    {
        int ColumnCount = GetFittedColumnCount(Root);
        List<Grid> ColumnGrids = GetVisualColumnGrids(Root);
        List<List<Control>> Rows;
        if (ColumnGrids.Count == ColumnCount)
            return;
        Rows = ExtractControlRows(Root);
        if (Rows.Count == 0)
            return;
        RebuildColumnRootColumns(Root, Rows, ColumnCount);
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
            Expander.IsExpanded = Entry.Key.IsSameText(Sys.GENERAL);
            List<Grid> ColumnGrids = UiItemPage.CreateGroupColumnGrids(Expander, Entry.Value.Count);
            for (int i = 0; i < Entry.Value.Count; i++)
            {
                List<FieldDef> Fields = Entry.Value[i];
                Grid ColumnGrid = ColumnGrids[i];
                for (int j = 0; j < Fields.Count; j++)
                    AddControlRow(context, ColumnGrid, j, Fields[j], Binder, TableUiInfo);
            }
            if (ColumnGrids.Count > 0 && ColumnGrids[0].Parent is Grid Root)
                ApplyResponsiveColumnRootLayout(Root);
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
            Expander.IsExpanded = false;
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
        UiFactory.AddChild(context.ParentControl, RootTabControl);
    }
}
