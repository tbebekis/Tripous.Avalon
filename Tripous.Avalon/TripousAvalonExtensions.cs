namespace Tripous.Avalon;

static public class TripousAvalonExtensions
{
    // ● DataGrid
    static public Type GetColumnDataType(this DataGridColumn Column)
    {
        // 1. ΠΡΩΤΑ: GridColumnInfo στο Tag (κύρια πηγή αλήθειας)
        if (Column.Tag is GridColumnInfo Info)
        {
            return Info.UnderlyingType;
        }

        // 2. Fallback: γνωστοί τύποι column
        if (Column is DataGridCheckBoxColumn)
            return typeof(bool);

        if (Column is DataGridHyperlinkColumn)
            return typeof(Uri);

        if (Column is DataGridComboBoxColumn)
            return typeof(object); // enum ή lookup, δεν ξέρουμε σίγουρα εδώ

        // 3. Fallback: binding (best effort)
        if (Column is DataGridBoundColumn BoundColumn)
        {
            if (BoundColumn.Binding is Binding binding)
            {
                if (binding.Source != null)
                    return binding.Source.GetType();
            }
        }

        // 4. Τελευταίο fallback
        return typeof(object);
    }
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
    /// Creates columns for a ProDataGrid based on a specified <see cref="DataTable"/>
    /// </summary>
    static public void CreateColumns(this DataGrid Grid, DataTable Table)
    {
        Grid.AutoGenerateColumns = false;
        Grid.Columns.Clear();

        foreach (DataColumn TableColumn in Table.Columns)
        {
            string BindingPath = GetBindingPath(TableColumn.ColumnName);
            Type DataType = TableColumn.DataType;
            Type CoreType = Nullable.GetUnderlyingType(DataType) ?? DataType;

            DataGridColumn GridColumn;

            /* Booleans -> CheckBox column */
            if (CoreType == typeof(bool))
            {
                GridColumn = new DataGridCheckBoxColumn
                {
                    Binding = new Binding(BindingPath),
                    IsThreeState = TableColumn.AllowDBNull
                };
            }
            /* Enums -> ComboBox column */
            else if (CoreType.IsEnum)
            {
                GridColumn = new DataGridComboBoxColumn
                {
                    ItemsSource = Enum.GetValues(CoreType),
                    SelectedItemBinding = new Binding(BindingPath)
                };
            }
            /* Uri -> Hyperlink column */
            else if (CoreType == typeof(Uri))
            {
                GridColumn = new DataGridHyperlinkColumn
                {
                    Binding = new Binding(BindingPath),
                    ContentBinding = new Binding(BindingPath)
                };
            }
            /* Fallback -> text column */
            else
            {
                GridColumn = new DataGridTextColumn
                {
                    Binding = new Binding(BindingPath)
                };
            }

            GridColumn.Header = TableColumn.Caption;
            GridColumn.IsReadOnly = TableColumn.ReadOnly;
            GridColumn.ColumnKey = TableColumn.ColumnName;
            GridColumn.SortMemberPath = TableColumn.ColumnName;
            GridColumn.Tag = new GridColumnInfo(TableColumn, GridColumn);

            Grid.Columns.Add(GridColumn);
        }
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

    // ● Menu
    static public MenuItem AddMenuItem(this List<object> MenuItems, string Header, EventHandler<RoutedEventArgs> Click = null, object Tag = null)
    {
        MenuItem Result = new MenuItem() { Header =  Header, Tag = Tag };
        Result.Click += Click;
        MenuItems.Add(Result);
        return Result;
    }
    static public Separator AddSeparator(this List<object> MenuItems)
    {
        Separator Result = new Separator();
        MenuItems.Add(Result);
        return Result;
    }
    
}