/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Desktop FactBox control that displays item module structure information.
/// </summary>
[TypeStore]
public class ItemStructureFactBoxControl: ItemFactBoxControl
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemStructureFactBoxControl()
    {
    }

    // ● protected
    /// <summary>
    /// Creates a text block.
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <param name="IsBold">True for bold text.</param>
    /// <returns>The created text block.</returns>
    protected virtual SelectableTextBlock CreateText(string Text, bool IsBold = false)
    {
        return new SelectableTextBlock
        {
            Text = Text ?? string.Empty,
            FontWeight = IsBold ? FontWeight.SemiBold : FontWeight.Normal,
            TextWrapping = TextWrapping.Wrap
        };
    }
    /// <summary>
    /// Creates a display row.
    /// </summary>
    /// <param name="Name">The row name.</param>
    /// <param name="Value">The row value.</param>
    /// <returns>The created row.</returns>
    protected virtual Control CreateRow(string Name, object Value)
    {
        Grid Result = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(new GridLength(1, GridUnitType.Star))
            }
        };

        SelectableTextBlock Key = CreateText(Name, true);
        Key.Margin = new Thickness(0, 0, 8, 0);
        Grid.SetColumn(Key, 0);
        Result.Children.Add(Key);

        SelectableTextBlock Text = CreateText(Convert.ToString(Value, CultureInfo.CurrentCulture));
        Grid.SetColumn(Text, 1);
        Result.Children.Add(Text);

        return Result;
    }
    /// <summary>
    /// Adds a cell to a grid.
    /// </summary>
    /// <param name="Grid">The grid.</param>
    /// <param name="Row">The row index.</param>
    /// <param name="Column">The column index.</param>
    /// <param name="Text">The cell text.</param>
    /// <param name="IsHeader">True when the cell is a header.</param>
    /// <param name="IsCentered">True when the cell text should be centered.</param>
    protected virtual void AddCell(Grid Grid, int Row, int Column, string Text, bool IsHeader = false, bool IsCentered = false)
    {
        Border Border = new()
        {
            BorderThickness = new Thickness(0, 0, 1, 1),
            Padding = new Thickness(4, 2)
        };
        Border.Bind(Border.BorderBrushProperty, new Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("SystemControlForegroundBaseMediumLowBrush"));
        SelectableTextBlock Block = CreateText(Text, IsHeader);
        if (IsCentered)
        {
            Block.TextAlignment = TextAlignment.Center;
            Block.HorizontalAlignment = HorizontalAlignment.Center;
        }
        Border.Child = Block;
        Grid.SetRow(Border, Row);
        Grid.SetColumn(Border, Column);
        Grid.Children.Add(Border);
    }
    /// <summary>
    /// Adds a row to the field grid.
    /// </summary>
    /// <param name="Grid">The grid.</param>
    /// <param name="Row">The row index.</param>
    /// <param name="Field">The field information.</param>
    protected virtual void AddFieldRow(Grid Grid, int Row, ItemStructureFieldInfo Field)
    {
        Grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        AddCell(Grid, Row, 0, Field.Title);
        AddCell(Grid, Row, 1, Field.Name);
        AddCell(Grid, Row, 2, Field.IsVisible ? "x" : string.Empty, IsCentered: true);
        AddCell(Grid, Row, 3, Field.IsVisible ? string.Empty : "x", IsCentered: true);
        AddCell(Grid, Row, 4, Field.DataType);
        AddCell(Grid, Row, 5, Field.IsRequired ? "x" : string.Empty, IsCentered: true);
        AddCell(Grid, Row, 6, Field.IsReadOnly ? "x" : string.Empty, IsCentered: true);
        AddCell(Grid, Row, 7, Field.LookupSource);
        AddCell(Grid, Row, 8, Field.Locator);
        AddCell(Grid, Row, 9, Field.Group);
        AddCell(Grid, Row, 10, Field.MaxLength > 0 ? Field.MaxLength.ToString(CultureInfo.CurrentCulture) : string.Empty);
        AddCell(Grid, Row, 11, Field.Decimals >= 0 ? Field.Decimals.ToString(CultureInfo.CurrentCulture) : string.Empty);
        AddCell(Grid, Row, 12, Field.DefaultValue);
        AddCell(Grid, Row, 13, Field.IsNullable ? "x" : string.Empty, IsCentered: true);
        AddCell(Grid, Row, 14, Field.DisplayWidth > 0 ? Field.DisplayWidth.ToString(CultureInfo.CurrentCulture) : string.Empty);
        AddCell(Grid, Row, 15, Field.Expression);
        AddCell(Grid, Row, 16, Field.CodeProvider);
        AddCell(Grid, Row, 17, Field.SnapshotOf);
        AddCell(Grid, Row, 18, Field.Flags);
    }
    /// <summary>
    /// Creates a table information panel.
    /// </summary>
    /// <param name="Table">The table information.</param>
    /// <returns>The created table information panel.</returns>
    protected virtual Control CreateTableInfoPanel(ItemStructureTableInfo Table)
    {
        StackPanel Result = new()
        {
            Spacing = 6,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Result.Children.Add(CreateRow("Alias", Table.Alias));
        Result.Children.Add(CreateRow("Master", Table.MasterName));
        Result.Children.Add(CreateRow("Details", string.Join(", ", Table.DetailNames)));
        Result.Children.Add(CreateRow("KeyField", Table.KeyField));
        if (Table.IsDetail)
        {
            Result.Children.Add(CreateRow("MasterField", Table.MasterField));
            Result.Children.Add(CreateRow("DetailField", Table.DetailField));
        }
        Result.Children.Add(CreateRow("OneToOne", Table.IsOneToOne));
        Result.Children.Add(CreateRow("Joins", Table.JoinCount));
        Result.Children.Add(CreateRow("Stocks", Table.StockCount));
        Result.Children.Add(CreateRow("Fields", $"{Table.VisibleFieldCount}/{Table.FieldCount} visible"));

        if (Table.Fields.Count > 0)
            Result.Children.Add(CreateFieldsGrid(Table.Fields));

        return Result;
    }
    /// <summary>
    /// Creates a table expander header.
    /// </summary>
    /// <param name="Text">The header text.</param>
    /// <returns>The created header control.</returns>
    protected virtual Control CreateTableExpanderHeader(string Text)
    {
        Border Result = new()
        {
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(6, 3),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child = new TextBlock
            {
                Text = Text ?? string.Empty,
                TextWrapping = TextWrapping.Wrap
            }
        };
        Result.Bind(Border.BackgroundProperty, new Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("SystemControlBackgroundChromeMediumLowBrush"));
        Result.Bind(Border.BorderBrushProperty, new Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("SystemControlForegroundBaseMediumLowBrush"));
        return Result;
    }
    /// <summary>
    /// Creates a table expander.
    /// </summary>
    /// <param name="Table">The table information.</param>
    /// <param name="Level">The tree level.</param>
    /// <param name="IsExpanded">True when the expander should be initially expanded.</param>
    /// <returns>The created expander.</returns>
    protected virtual Expander CreateTableExpander(ItemStructureTableInfo Table, int Level, bool IsExpanded)
    {
        string VisibleText = Table.IsUiVisible ? "visible" : "hidden";
        string DetailText = Table.IsDetail ? "detail" : "top";
        string HeaderText = $"{new string(' ', Level * 2)}{Table.Title} ({Table.Name}) - {VisibleText}, {DetailText}, fields {Table.VisibleFieldCount}/{Table.FieldCount}";
        Expander Result = new()
        {
            Header = CreateTableExpanderHeader(HeaderText),
            IsExpanded = IsExpanded,
            Margin = new Thickness(Level * 8, 4, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Content = CreateTableInfoPanel(Table)
        };

        return Result;
    }
    /// <summary>
    /// Adds table expanders recursively.
    /// </summary>
    /// <param name="Panel">The parent panel.</param>
    /// <param name="Table">The table information.</param>
    /// <param name="Level">The tree level.</param>
    /// <param name="ExpandTop">True when the top table should be expanded.</param>
    protected virtual void AddTableExpanders(StackPanel Panel, ItemStructureTableInfo Table, int Level, bool ExpandTop)
    {
        Panel.Children.Add(CreateTableExpander(Table, Level, Level == 0 && ExpandTop));
        foreach (ItemStructureTableInfo Detail in Table.Details)
            AddTableExpanders(Panel, Detail, Level + 1, ExpandTop);
    }
    /// <summary>
    /// Creates the fields grid.
    /// </summary>
    /// <param name="Fields">The fields.</param>
    /// <returns>The created fields grid.</returns>
    protected virtual Control CreateFieldsGrid(List<ItemStructureFieldInfo> Fields)
    {
        Grid Result = new() { Margin = new Thickness(0, 4, 0, 0) };
        string[] Headers = [
            "Title",
            "Name",
            "Visible",
            "Hidden",
            "DataType",
            "Required",
            "ReadOnly",
            "Lookup",
            "Locator",
            "Group",
            "Size",
            "Decimals",
            "Default",
            "Nullable",
            "Width",
            "Expression",
            "CodeProvider",
            "SnapshotOf",
            "Flags"
        ];

        Result.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (int Index = 0; Index < Headers.Length; Index++)
        {
            Result.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            AddCell(Result, 0, Index, Headers[Index], true);
        }

        for (int Index = 0; Index < Fields.Count; Index++)
        {
            AddFieldRow(Result, Index + 1, Fields[Index]);
        }

        return new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = Result
        };
    }

    // ● public
    /// <summary>
    /// Binds this control to FactBox data.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <param name="Data">The FactBox data.</param>
    public override void BindFactBox(ItemFactBoxContext Context, object Data)
    {
        base.BindFactBox(Context, Data);

        StackPanel Panel = new()
        {
            Margin = new Thickness(8),
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        if (Data is ItemStructureFactBoxData Structure)
        {
            Panel.Children.Add(CreateRow("Module", $"{Structure.ModuleTitle} ({Structure.ModuleName})"));
            Panel.Children.Add(CreateRow("Group", Structure.ModuleGroup));
            Panel.Children.Add(CreateRow("Module Class", Structure.ModuleClassName));
            Panel.Children.Add(CreateRow("Form Class", Structure.FormClassName));
            Panel.Children.Add(CreateRow("ItemPage Class", Structure.ItemPageClassName));
            Panel.Children.Add(CreateRow("Tables", $"{Structure.VisibleTableCount}/{Structure.TableCount} visible"));
            if (Structure.Table != null)
                AddTableExpanders(Panel, Structure.Table, 0, true);
        }

        Content = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = Panel
        };
    }
}
