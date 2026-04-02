using Avalonia.Styling;

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

    static public DataGridColumn CreateColumn(this DataGrid Grid, Type DataType, string FieldName, string Caption, bool IsReadOnly = true)
    {
        
        DataGridColumn GridColumn = null;
        Type CoreType = Nullable.GetUnderlyingType(DataType) ?? DataType;

        /* Booleans -> CheckBox column */
        if (CoreType == typeof(bool))
        {
            GridColumn = new DataGridCheckBoxColumn
            {
                Binding = new Binding(FieldName),
                IsThreeState = false
            };
        }
        /* Enums -> ComboBox column */
        else if (CoreType.IsEnum)
        {
            GridColumn = new DataGridComboBoxColumn
            {
                ItemsSource = Enum.GetValues(CoreType),
                SelectedItemBinding = new Binding(FieldName)
            };
        }
        /* Uri -> Hyperlink column */
        else if (CoreType == typeof(Uri))
        {
            GridColumn = new DataGridHyperlinkColumn
            {
                Binding = new Binding(FieldName),
                ContentBinding = new Binding(FieldName)
            };
        }
        /* Fallback -> text column */
        else
        {
            GridColumn = new DataGridTextColumn
            {
                Binding = new Binding(FieldName)
            };
        }
        
        GridColumn.Header = Caption;
        GridColumn.IsReadOnly = IsReadOnly;
        GridColumn.ColumnKey = FieldName;
        GridColumn.SortMemberPath = FieldName;

        Grid.Columns.Add(GridColumn);

        return GridColumn;
    }
    static public GridColumnInfo GetColumnInfo(this DataGridColumn Column) => Column.Tag as GridColumnInfo;
    
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
    
    // ● DataGrid - View related
    /// <summary>
    /// Creates a default <see cref="GridViewDef"/> based on a specified <see cref="DataView"/>.
    /// </summary>
    static public GridViewDef CreateViewDef(this DataView DataView)
    {
        GridViewDef Result = new();
        DataTable Table = DataView.Table;
 
        foreach (DataColumn Column in Table.Columns)
            Result.OrderList.AddRange(Column.ColumnName);
 
        return Result;
    }
    
    // ● DataGrid - Pivot related
    /// <summary>
    /// Converts a Tripous <see cref="PivotValueAggregateType"/> to a ProDataGrid <see cref="PivotAggregateType"/>
    /// </summary>
    static public PivotAggregateType ToPivotAggregateType(this PivotValueAggregateType AggregateType)
    {
        return AggregateType switch
        {
            PivotValueAggregateType.Sum => PivotAggregateType.Sum,
            PivotValueAggregateType.Avg => PivotAggregateType.Average,
            PivotValueAggregateType.Count => PivotAggregateType.Count,
            PivotValueAggregateType.Min => PivotAggregateType.Min,
            PivotValueAggregateType.Max => PivotAggregateType.Max,
            PivotValueAggregateType.StdDev => PivotAggregateType.StdDev,
            PivotValueAggregateType.StdDevP => PivotAggregateType.StdDevP,
            PivotValueAggregateType.Variance => PivotAggregateType.Variance,
            PivotValueAggregateType.VarianceP => PivotAggregateType.VarianceP,
            PivotValueAggregateType.CountDistinct => PivotAggregateType.CountDistinct,
            PivotValueAggregateType.Product => PivotAggregateType.Product,
            _ => PivotAggregateType.Count,
        };
    }
 
    static public bool IsPivotSupportedType(this Type T)
    {
        Type ActualType = Nullable.GetUnderlyingType(T) ?? T;
        return ActualType.IsString() || ActualType.IsNumeric() || ActualType.IsDateTime();
    }
    /// <summary>
    /// Creates a default <see cref="PivotDef"/> based on a specified <see cref="DataView"/>.
    /// </summary>
    static public PivotDef CreateDefaultPivotDef(this DataView DataView)
    {
        if (DataView == null)
            throw new ArgumentNullException(nameof(DataView));

        PivotDef Result = new();

        List<DataColumn> Columns = DataView.Table.Columns.Cast<DataColumn>().ToList();

        List<DataColumn> StringCols = Columns.Where(c => c.DataType == typeof(string)).ToList();
        List<DataColumn> DateCols = Columns.Where(c => c.DataType == typeof(DateTime)).ToList();
        List<DataColumn> NumericCols = Columns.Where(c => c.DataType.IsNumeric()).ToList();

        List<DataColumn> Eligible = Columns
            .Where(c => c.DataType == typeof(string) || c.DataType == typeof(DateTime) || c.DataType.IsNumeric())
            .ToList();

        foreach (DataColumn Col in Eligible)
        {
            Result.Columns.Add(new PivotColumnDef
            {
                FieldName = Col.ColumnName,
                Caption = Col.Caption ?? Col.ColumnName,
                Axis = PivotAxis.None,
                IsValue = false,
                ValueAggregateType = PivotValueAggregateType.None,
                SortByValue = true,
                SortDescending = false,
                Format = Col.DataType == typeof(DateTime) ? "d" : null
            });
        }

        PivotColumnDef FirstRow = null;
        PivotColumnDef SecondRow = null;
        PivotColumnDef FirstValue = null;

        DataColumn RowCol1 = StringCols.FirstOrDefault() ?? DateCols.FirstOrDefault();
        DataColumn RowCol2 = StringCols.Skip(1).FirstOrDefault() ?? DateCols.Skip(1).FirstOrDefault();

        if (RowCol1 != null)
        {
            FirstRow = Result.Columns.First(x => x.FieldName == RowCol1.ColumnName);
            FirstRow.Axis = PivotAxis.Row;
        }

        if (RowCol2 != null)
        {
            SecondRow = Result.Columns.First(x => x.FieldName == RowCol2.ColumnName);
            SecondRow.Axis = PivotAxis.Column;
        }

        DataColumn ValueCol = NumericCols.FirstOrDefault();

        if (ValueCol != null)
        {
            FirstValue = Result.Columns.First(x => x.FieldName == ValueCol.ColumnName);
            FirstValue.IsValue = true;
            FirstValue.ValueAggregateType = PivotValueAggregateType.Sum;
        }
        else
        {
            PivotColumnDef Fallback = Result.Columns.FirstOrDefault(x => x.Axis == PivotAxis.None);

            if (Fallback != null)
            {
                Fallback.IsValue = true;
                Fallback.ValueAggregateType = PivotValueAggregateType.Count;
            }
        }

        return Result;
    }
    /// <summary>
    /// Creates a list of <see cref="PivotColumnInfo"/> items based on a specified <see cref="DataView"/>.
    /// </summary>
    static public List<PivotColumnInfo> CreatePivotColumnInfoList(this DataView DataView)
    {
        List<PivotColumnInfo> Result = new();
        
        DataTable Table = DataView.Table;
  
        foreach (DataColumn Column in Table.Columns)
        {
            if (Column.DataType.IsPivotSupportedType())
                Result.Add(new PivotColumnInfo(Column));
        }
        
        return Result;
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
    
    // ● Miscs
    static public DataGridAggregateType ToAvalonia(this AggregateType AggregateType)
    {
        switch (AggregateType)
        {
            case AggregateType.Count: return DataGridAggregateType.Count;
            case AggregateType.Sum: return DataGridAggregateType.Sum;
            case AggregateType.Min: return DataGridAggregateType.Min;
            case AggregateType.Max: return DataGridAggregateType.Max;
            case AggregateType.Avg: return DataGridAggregateType.Average;
        }

        return DataGridAggregateType.None;
    }
}

