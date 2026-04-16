using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace Tripous.Avalon;

static public class TripousAvalonExtensions
{
    // ● DataGrid
 
    /// <summary>
    /// Returns the path of a column, e.g. [Customer.Name]
    /// </summary>
    static public string GetColumnPath(this DataGridColumn Column)
    {
        if (Column is DataGridBoundColumn bound && bound.Binding is Binding b)
            return b.Path;

        return string.Empty;
    }
    /// <summary>
    /// Returns the name of a Property/FieldName a column is bound to.
    /// </summary>
    static public string GetPropertyName(this DataGridColumn Column)
    {
        if (Column != null && (Column is DataGridBoundColumn col))
        {
            // try get it from Binding
            if (col.Binding is Binding b && !string.IsNullOrWhiteSpace(b.Path))
            {
                // remove any brackets and get the last part
                string path = b.Path.Replace("[", "").Replace("]", "");
        
                if (path.Contains("."))
                {
                    return path.Split('.').Last();
                }
        
                return path;
            }

            // try get it from Tag
            if (Column.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
                return tag;
        }
        
        return string.Empty;
    }
    /// <summary>
    /// Returns a binding path suitable in binding to a ProDataGrid column.
    /// </summary>
    static public string GetBindingPath(string PropertyName)
    {
        if (string.IsNullOrEmpty(PropertyName))
            return PropertyName;

        if (PropertyName.Length > 1 && PropertyName[0] == '[' && PropertyName[PropertyName.Length - 1] == ']')
            return PropertyName;

        bool RequiresIndexer = false;

        foreach (char ch in PropertyName)
        {
            if (!char.IsLetterOrDigit(ch) && ch != '_')
            {
                RequiresIndexer = true;
                break;
            }
        }

        return RequiresIndexer ? $"[{PropertyName}]" : PropertyName;
    }
 
    /// <summary>
    /// Finds and returns a column by name (i.e. Header), if any, else null.
    /// <para>NOTE: <see cref="ColumnName"/> is the Header of the column.</para>
    /// </summary>
    static public DataGridColumn FindColumn(this DataGrid Grid, string ColumnName)
    {
        foreach (DataGridColumn Column in Grid.Columns)
            if (ColumnName.IsSameText(Column.Header.ToString()))
                return Column;
        return null;
    } 
    /// <summary>
    /// Sets all columns to be editable (i.e. not read-only)
    /// </summary>
    static public void SetAllColumnsEditable(this DataGrid Grid)
    {
        foreach (DataGridColumn Column in Grid.Columns)
            Column.IsReadOnly = false;    
    }
    /// <summary>
    /// Columns found in <see cref="ColumnNames"/> are set to be editable. All other columns are set to read-only.
    /// </summary>
    static public void SetEditableColumns(this DataGrid Grid, List<string> ColumnNames)
    {
        foreach (DataGridColumn Column in Grid.Columns)
            Column.IsReadOnly = !ColumnNames.ContainsText(Column.Header.ToString());
    }
    /// <summary>
    /// Columns found in <see cref="ColumnNames"/> are set to be visible. All other columns are hidden.
    /// </summary>
    static public void SetVisibleColumns(this DataGrid Grid, List<string> ColumnNames)
    {
        foreach (DataGridColumn Column in Grid.Columns)
            Column.IsVisible = ColumnNames.ContainsText(Column.Header.ToString());
    }
    /// <summary>
    /// Sets a column editable (non read-only) by name.
    /// <para>NOTE: <see cref="ColumnName"/> is the Header of the column.</para>
    /// </summary>
    static public void SetColumnEditable(this DataGrid Grid, string ColumnName, bool Value)
    {
        DataGridColumn Column = Grid.FindColumn(ColumnName);
        if (Column != null)
            Column.IsReadOnly = !Value;
    }
    /// <summary>
    /// Sets a column visible by name.
    /// <para>NOTE: <see cref="ColumnName"/> is the Header of the column.</para>
    /// </summary>
    static public void SetColumnVisible(this DataGrid Grid, string ColumnName, bool Value)
    {
        DataGridColumn Column = Grid.FindColumn(ColumnName);
        if (Column != null)
            Column.IsVisible = Value;
    }
 
    static public void ShowHideIdColumns(this DataGrid Grid, bool Value)
    {
        string PropertyName;
        foreach (var column in Grid.Columns)
        {
            if (column is DataGridBoundColumn Col)
            {
                PropertyName = Col.GetPropertyName();
                if (!string.IsNullOrWhiteSpace(PropertyName) && PropertyName.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    Col.IsVisible = Value;
                }
            }
        }
    }

    // ● control text
    static public string GetText(this TextBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this TextEditor Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this AutoCompleteBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this ComboBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;

    static public bool GetValue(this CheckBox Box) => Box != null && Box.IsChecked.HasValue? Box.IsChecked.Value : false;
    
    // ● Menu
    static public MenuItem AddMenuItem(this List<object> Items, string Header, EventHandler<RoutedEventArgs> Click = null, object Tag = null)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        Result.Click += Click;
        Items.Add(Result);
        return Result;
    }
    static public Separator AddSeparator(this List<object> Items)
    {
        Separator Result = new Separator();
        Items.Add(Result);
        return Result;
    }
    static public MenuItem AddMenuItem(this MenuItem MenuItem, string Header, EventHandler<RoutedEventArgs> Click = null, object Tag = null)
    {
        return MenuItem.Items.AddMenuItem(Header, Click, Tag);
    }
    static public Separator AddSeparator(this MenuItem MenuItem)
    {
        return MenuItem.Items.AddSeparator();
    }
    static public MenuItem AddMenuItem(this ItemCollection Items, string Header, EventHandler<RoutedEventArgs> Click = null, object Tag = null)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        Result.Click += Click;
        Items.Add(Result);
        return Result;
    }
    static public Separator AddSeparator(this ItemCollection Items)
    {
        Separator Result = new Separator();
        Items.Add(Result);
        return Result;
    }

    static public MenuItem AddCheckBoxMenuItem(this ItemCollection Items, string Header, bool IsChecked = true, EventHandler<RoutedEventArgs> Click = null, object Tag = null)
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
    static public MenuItem AddCheckBoxMenuItem(this IList<object> Items, string Header, bool IsChecked = true, EventHandler<RoutedEventArgs> Click = null, object Tag = null)
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
    
    // ● Button
    static public void PerformClick(this Button Button)
    {
        if (Button != null)
        {
            var clickArgs = new RoutedEventArgs(Button.ClickEvent);
            Button.RaiseEvent(clickArgs);
        }
    }
    
    // ● TabItem
    static public void TabItem_MiddleClick(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
        {
            if (sender is TabItem Page)
            {
                var Pager = Page.FindLogicalAncestorOfType<TabControl>();

                if (Pager != null)
                {
                    Pager.Items.Remove(Page);
                }
            }
        }
    }
    static public void Close(this TabItem Page)
    {
        if (Page != null)
        {
            var Pager = Page.FindLogicalAncestorOfType<TabControl>();

            if (Pager != null)
            {
                Pager.Items.Remove(Page);
            }
        }
    }
}

