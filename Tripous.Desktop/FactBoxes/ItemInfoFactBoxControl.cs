/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// The built-in desktop FactBox control that displays item and structure information.
/// </summary>
[TypeStore]
public class ItemInfoFactBoxControl: ItemStructureFactBoxControl
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemInfoFactBoxControl()
    {
    }

    // ● public
    /// <summary>
    /// Binds this control to FactBox data.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <param name="Data">The FactBox data.</param>
    public override void BindFactBox(ItemFactBoxContext Context, object Data)
    {
        this.Context = Context;
        this.Data = Data;
        DataContext = Data;

        StackPanel Panel = new()
        {
            Margin = new Thickness(8),
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        if (Data is ItemStandardInfoFactBoxData StandardData)
        {
            AddItemInfo(Panel, StandardData.ItemInfo);
            AddStructureInfo(Panel, StandardData.Structure);
        }
        else if (Data is IReadOnlyDictionary<string, object> Dictionary)
        {
            AddItemInfo(Panel, Dictionary);
        }

        Content = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = Panel
        };
    }

    // ● protected
    /// <summary>
    /// Adds item row information.
    /// </summary>
    /// <param name="Panel">The parent panel.</param>
    /// <param name="Dictionary">The information dictionary.</param>
    protected virtual void AddItemInfo(StackPanel Panel, IReadOnlyDictionary<string, object> Dictionary)
    {
        if (Dictionary == null)
            return;

        foreach (KeyValuePair<string, object> Pair in Dictionary)
            Panel.Children.Add(CreateRow(Pair.Key, Pair.Value));
    }
    /// <summary>
    /// Adds module structure information.
    /// </summary>
    /// <param name="Panel">The parent panel.</param>
    /// <param name="Structure">The structure information.</param>
    protected virtual void AddStructureInfo(StackPanel Panel, ItemStructureFactBoxData Structure)
    {
        if (Structure == null)
            return;

        Panel.Children.Add(CreateRow("Group", Structure.ModuleGroup));
        Panel.Children.Add(CreateRow("Module Class", Structure.ModuleClassName));
        Panel.Children.Add(CreateRow("Form Class", Structure.FormClassName));
        Panel.Children.Add(CreateRow("ItemPage Class", Structure.ItemPageClassName));
        Panel.Children.Add(CreateRow("Tables", $"{Structure.VisibleTableCount}/{Structure.TableCount} visible"));
        if (Structure.Table != null)
            AddTableExpanders(Panel, Structure.Table, 0, true);
    }
}
