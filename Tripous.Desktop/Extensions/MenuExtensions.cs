/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides extension methods for creating menu items.
/// </summary>
static public class MenuExtensions
{
    // ● static public
    /// <summary>
    /// Adds a menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this IList Items, string Header)
    {
        EventHandler<RoutedEventArgs> Click = null;
        return AddMenuItem(Items, Header, Click);
    }
    /// <summary>
    /// Adds a menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Click">The click event handler.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this IList Items, string Header, EventHandler<RoutedEventArgs> Click)
    {
        object Tag = null;
        return AddMenuItem(Items, Header, Click, Tag);
    }
    /// <summary>
    /// Adds a menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Click">The click event handler.</param>
    /// <param name="Tag">The menu item tag.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this IList Items, string Header, EventHandler<RoutedEventArgs> Click, object Tag)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        Result.Click += Click;
        Items.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds a menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Action">The click action.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this IList Items, string Header, Action Action)
    {
        object Tag = null;
        return AddMenuItem(Items, Header, Action, Tag);
    }
    /// <summary>
    /// Adds a menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Action">The click action.</param>
    /// <param name="Tag">The menu item tag.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this IList Items, string Header, Action Action, object Tag)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        if (Action != null)
            Result.Click += (sender, args) => Action();
        Items.Add(Result);
        return Result;
    }
    /// <summary>
    /// Adds a separator to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <returns>The created separator.</returns>
    static public Separator AddSeparator(this IList Items)
    {
        Separator Result = new Separator();
        Items.Add(Result);
        return Result;
    }

    /// <summary>
    /// Adds a child menu item to a menu item.
    /// </summary>
    /// <param name="MenuItem">The parent menu item.</param>
    /// <param name="Header">The menu item header.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header)
    {
        EventHandler<RoutedEventArgs> Click = null;
        return AddMenuItem(MenuItem, Header, Click);
    }
    /// <summary>
    /// Adds a child menu item to a menu item.
    /// </summary>
    /// <param name="MenuItem">The parent menu item.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Click">The click event handler.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, EventHandler<RoutedEventArgs> Click)
    {
        object Tag = null;
        return AddMenuItem(MenuItem, Header, Click, Tag);
    }
    /// <summary>
    /// Adds a child menu item to a menu item.
    /// </summary>
    /// <param name="MenuItem">The parent menu item.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Click">The click event handler.</param>
    /// <param name="Tag">The menu item tag.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, EventHandler<RoutedEventArgs> Click, object Tag)
    {
        return MenuItem.Items.AddMenuItem(Header, Click, Tag);
    }
    /// <summary>
    /// Adds a child menu item to a menu item.
    /// </summary>
    /// <param name="MenuItem">The parent menu item.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Action">The click action.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, Action Action)
    {
        object Tag = null;
        return AddMenuItem(MenuItem, Header, Action, Tag);
    }
    /// <summary>
    /// Adds a child menu item to a menu item.
    /// </summary>
    /// <param name="MenuItem">The parent menu item.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Action">The click action.</param>
    /// <param name="Tag">The menu item tag.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, Action Action, object Tag)
    {
        return MenuItem.Items.AddMenuItem(Header, Action, Tag);
    }
    /// <summary>
    /// Adds a separator to a menu item.
    /// </summary>
    /// <param name="MenuItem">The parent menu item.</param>
    /// <returns>The created separator.</returns>
    static public Separator AddSeparator(this MenuItem MenuItem)
    {
        return MenuItem.Items.AddSeparator();
    }
 
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header)
    {
        bool IsChecked = false;
        return AddCheckBoxMenuItem(Items, Header, IsChecked);
    }
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="IsChecked">True when the menu item is checked.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked)
    {
        EventHandler<RoutedEventArgs> Click = null;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Click);
    }
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="IsChecked">True when the menu item is checked.</param>
    /// <param name="Click">The click event handler.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked, EventHandler<RoutedEventArgs> Click)
    {
        object Tag = null;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Click, Tag);
    }
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="IsChecked">True when the menu item is checked.</param>
    /// <param name="Click">The click event handler.</param>
    /// <param name="Tag">The menu item tag.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked, EventHandler<RoutedEventArgs> Click, object Tag)
    {
        MenuItem Result = new();
        Result.Header = Header;
        Result.Tag = Tag;
        Result.ToggleType = MenuItemToggleType.CheckBox;
        
        Result.Click += Click;
        Items.Add(Result);
        Result.IsChecked = IsChecked;

        return Result;
    }
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="Action">The click action.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, Action Action)
    {
        bool IsChecked = false;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Action);
    }
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="IsChecked">True when the menu item is checked.</param>
    /// <param name="Action">The click action.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked, Action Action)
    {
        object Tag = null;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Action, Tag);
    }
    /// <summary>
    /// Adds a check box menu item to an item list.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Header">The menu item header.</param>
    /// <param name="IsChecked">True when the menu item is checked.</param>
    /// <param name="Action">The click action.</param>
    /// <param name="Tag">The menu item tag.</param>
    /// <returns>The created menu item.</returns>
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked, Action Action, object Tag)
    {
        MenuItem Result = new();
        Result.Header = Header;
        Result.Tag = Tag;
        Result.ToggleType = MenuItemToggleType.CheckBox;
        
        if (Action != null)
            Result.Click += (sender, args) => Action();
        Items.Add(Result);
        Result.IsChecked = IsChecked;

        return Result;
 
    }
}
