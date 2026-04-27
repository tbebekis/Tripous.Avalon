namespace Tripous.Desktop;

static public class MenuExtensions
{
    // ● Menu
    static public MenuItem AddMenuItem(this IList Items, string Header)
    {
        EventHandler<RoutedEventArgs> Click = null;
        return AddMenuItem(Items, Header, Click);
    }
    static public MenuItem AddMenuItem(this IList Items, string Header, EventHandler<RoutedEventArgs> Click)
    {
        object Tag = null;
        return AddMenuItem(Items, Header, Click, Tag);
    }
    static public MenuItem AddMenuItem(this IList Items, string Header, EventHandler<RoutedEventArgs> Click, object Tag)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        Result.Click += Click;
        Items.Add(Result);
        return Result;
    }
    static public MenuItem AddMenuItem(this IList Items, string Header, Action Action)
    {
        object Tag = null;
        return AddMenuItem(Items, Header, Action, Tag);
    }
    static public MenuItem AddMenuItem(this IList Items, string Header, Action Action, object Tag)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        if (Action != null)
            Result.Click += (sender, args) => Action();
        Items.Add(Result);
        return Result;
    }
    static public Separator AddSeparator(this IList Items)
    {
        Separator Result = new Separator();
        Items.Add(Result);
        return Result;
    }

    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header)
    {
        EventHandler<RoutedEventArgs> Click = null;
        return AddMenuItem(MenuItem, Header, Click);
    }
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, EventHandler<RoutedEventArgs> Click)
    {
        object Tag = null;
        return AddMenuItem(MenuItem, Header, Click, Tag);
    }
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, EventHandler<RoutedEventArgs> Click, object Tag)
    {
        return MenuItem.Items.AddMenuItem(Header, Click, Tag);
    }
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, Action Action)
    {
        object Tag = null;
        return AddMenuItem(MenuItem, Header, Action, Tag);
    }
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, Action Action, object Tag)
    {
        return MenuItem.Items.AddMenuItem(Header, Action, Tag);
    }
    static public Separator AddSeparator(this MenuItem MenuItem)
    {
        return MenuItem.Items.AddSeparator();
    }
 
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header)
    {
        bool IsChecked = false;
        return AddCheckBoxMenuItem(Items, Header, IsChecked);
    }
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked)
    {
        EventHandler<RoutedEventArgs> Click = null;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Click);
    }
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked, EventHandler<RoutedEventArgs> Click)
    {
        object Tag = null;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Click, Tag);
    }
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
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, Action Action)
    {
        bool IsChecked = false;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Action);
    }
    static public MenuItem AddCheckBoxMenuItem(this IList Items, string Header, bool IsChecked, Action Action)
    {
        object Tag = null;
        return AddCheckBoxMenuItem(Items, Header, IsChecked, Action, Tag);
    }
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