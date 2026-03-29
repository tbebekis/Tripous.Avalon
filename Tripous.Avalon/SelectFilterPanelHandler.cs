namespace Tripous.Avalon;

/// <summary>
/// Handles a <see cref="StackPanel"/> where it creates filter controls according to a specified <see cref="SelectDef"/> descriptor.
/// </summary>
public class SelectFilterPanelHandler
{
    // ● private
    private StackPanel pnlFilters;
    Dictionary<SqlFilterDef, Control> ControlsDic = new();
    private const int GridHeight = 250;
    SelectDef SelectDef;
    private SqlStore fSqlStore;
    
    // ● private properties
    SqlStore SqlStore
    {
        get
        {
            if (fSqlStore is null)
                fSqlStore = SelectDef.GetSqlStore();
            return fSqlStore;
        }
    }
    Window ParentWindow => TopLevel.GetTopLevel(pnlFilters) as Window;
 
    // ● private
    DateTime CalculateInitialDate(DateRange Range)
    {
        if (Range == DateRange.Custom)
            return DateTime.Today;

        DateTime FromDate = DateTime.Today;
        DateTime ToDate = DateTime.Today;

        Range.ToDates(ref FromDate, ref ToDate);

        return FromDate; // return the start of the range
    }
    void AddGridContextMenu(DataGrid gridLookUp, DataTable tblLookUp, bool IsMultiple)
    {
        gridLookUp.ContextMenu = new ContextMenu();
        List<object> List = new();

        if (IsMultiple)
        {
            // toggle selection
            var mnuToggleSelection = new MenuItem
            {
                Name = "mnuToggleSelection",
                Header = "Select/Unselect (Double Click)"
            };
            mnuToggleSelection.Click += (_, __) =>
            {
                GridBinder Binder = gridLookUp.Tag as GridBinder;
                if (Binder.CurrentRow != null)
                {
                    bool Value = Binder.CurrentRow.AsBoolean("IsSelected");
                    Binder.CurrentRow["IsSelected"] = !Value;
                }
            };
            
            // select all
            var mnuSelectAll = new MenuItem
            {
                Name = "mnuSelectAll",
                Header = "Select All"
            };
            mnuSelectAll.Click += (_, __) =>
            {
                foreach (DataRowView Row in tblLookUp.DefaultView)
                    Row["IsSelected"] = true;
            };

            // de-select all
            var mnuDeselectAll = new MenuItem
            {
                Name = "mnuDeselectAll",
                Header = "Deselect All"
            };
            mnuDeselectAll.Click += (_, __) =>
            {
                foreach (DataRowView Row in tblLookUp.DefaultView)
                    Row["IsSelected"] = false;
            };
            
            List.Add(mnuToggleSelection);
            List.Add(new Separator());
            List.Add(mnuSelectAll);
            List.Add(mnuDeselectAll);
        }

        // toggle Id columns
  
        var mnuToggleIdColumns = new MenuItem
        {
            Name = "mnuToggleIdColumns",
            Header = "Show/Hide IDs"
        };
        mnuToggleIdColumns.Click += (_, __) =>
        {
            IdColumnsVisible = !IdColumnsVisible;
            gridLookUp.ShowHideIdColumns(IdColumnsVisible);
        };
        if (List.Count > 0)
            List.Add(new Separator());
        List.Add(mnuToggleIdColumns);

        gridLookUp.ContextMenu = new ContextMenu
        {
            ItemsSource = List.ToArray()
        };
    }
    Control CreateLookupControl(SqlFilterDef FilterDef, DataTable tblLookUp)
    {
        // grid
        var gridLookUp = new DataGrid
        {
            CanUserResizeColumns = true,
            Height = GridHeight,
            MaxHeight = 350,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HeadersVisibility = DataGridHeadersVisibility.All,
            SelectionMode = DataGridSelectionMode.Single,
            IsReadOnly = true,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.LightGray,
            GridLinesVisibility = DataGridGridLinesVisibility.All
        };
        
        // load data
        //DataTable tblLookUp = SqlStore.Select(FilterDef.LookUpSelectSqlText);
        
        // context menu
        AddGridContextMenu(gridLookUp, tblLookUp, FilterDef.IsMultiple);
        
        // when IsMultiple, then add the IsSelected as first column
        if (FilterDef.IsMultiple)
        {
            DataColumn Col = tblLookUp.Columns.Add("IsSelected", typeof(bool));
            Col.SetOrdinal(0);
            Col.Caption = "+/-";

            foreach (DataRow Row in tblLookUp.Rows)
                Row["IsSelected"] = false;
        }
        
        // bind the grid
        GridBinder Binder = new GridBinder(gridLookUp, tblLookUp);
        
        // Id columns
        gridLookUp.ShowHideIdColumns(IdColumnsVisible);
        
        // double click event handler
        gridLookUp.DoubleTapped += (s, e) => 
        {
            if (Binder.CurrentRow != null)
            {
                bool Value = Binder.CurrentRow.AsBoolean("IsSelected");
                Binder.CurrentRow["IsSelected"] = !Value;
            }
        };   
 
        return gridLookUp;
    }
    string GetControlValue(SqlFilterDef FilterDef)
    {
        string Result = "";
        
        if (!ControlsDic.ContainsKey(FilterDef))
            return Result;

        var ctrl = ControlsDic[FilterDef];

        // DATA GRID (Lookup filters)
        if (ctrl is DataGrid Grid)
        {
            //BindingSource BS = Grid.DataContext as BindingSource;
            GridBinder Binder = GridBinder.GetGridBinder(Grid);
            
            if (!FilterDef.IsMultiple)
            {
                Result = Binder.CurrentRow.Row.AsString(0);
            }
            else
            {
                string S;
                 
                // Column 0 = the "IsSelected" column
                // Column 1 = the Result column, the one with the Result Id
                List<string> List = new();
                foreach (DataRow Row in Binder.DataTable.Rows)
                {
                    if (Row.AsBoolean(0))  
                    {
                        S = Row.AsString(1);  
                        List.Add(S);
                    }
                }
                
                Result = string.Join(";", List);
            }

            return Result;
        }
 
        // 2. LIST BOX (Για Multi Enum)
        if (ctrl is ListBox lb)
        {
            var selected = lb.SelectedItems.Cast<object>().Select(x => x.ToString());
            return string.Join(";", selected);
        }

        // 3. COMBO BOX (Για Single Enum)
        if (ctrl is ComboBox cb)
        {
            return cb.SelectedItem?.ToString();
        }

        // 4. CALENDAR DATE PICKER (Για Ημερομηνίες)
        if (ctrl is CalendarDatePicker dp)
        {
            return dp.SelectedDate?.ToString("yyyy-MM-dd");
        }

        // 5. TEXT BOX (Για String, Int, Dec)
        if (ctrl is TextBox tb)
        {
            if (FilterDef.IsNumeric && string.IsNullOrWhiteSpace(tb.Text))
                return "0";
                
            return tb.Text;
        }

        return null;
    }
    
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public SelectFilterPanelHandler(SelectDef SelectDef, StackPanel pnlFilters, bool IdColumnsVisible)
    {
        this.SelectDef = SelectDef;
        this.pnlFilters = pnlFilters;
        this.IdColumnsVisible = IdColumnsVisible;
    }

    // ● public
    /// <summary>
    /// Returns true if all controls have a value.
    /// </summary>
    public async Task<bool> CheckIsOk(bool ShowMessage)
    { 
        foreach(var filter in SelectDef.Filters)
        {
            if (string.IsNullOrWhiteSpace(GetControlValue(filter)))
            {
                string Message = $"Field '{filter.Label}' is required.";
                if (ShowMessage) await MessageBox.Info(Message, ParentWindow);
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Creates filter controls
    /// </summary>
    public async Task<int> ItemToControls()
    {
        DataTable tblLookUp = null;
        DataTable tblEnum = null;
        
        pnlFilters.Children.Clear();
        ControlsDic.Clear();

        int MinusY = 0;

        foreach (var filter in SelectDef.Filters)
        {
            var container = new StackPanel { Spacing = 2 };
            container.Children.Add(new TextBlock { Text = filter.Label, FontWeight = FontWeight.SemiBold, FontSize = 12 });

            Control ctrl = null;

            switch (filter.Type)
            {
                case SqlFilterType.Integer:
                case SqlFilterType.Decimal:
                    ctrl = new TextBox { 
                        Watermark = "0", 
                        HorizontalContentAlignment = HorizontalAlignment.Right,
                        Text = "0"
                    };
                    break;
                case SqlFilterType.Date:
                    var dp = new CalendarDatePicker { 
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        SelectedDateFormat = CalendarDatePickerFormat.Custom,
                        CustomDateFormatString = "yyyy-MM-dd",
                    };
                    // calculate date based on DateRange
                    dp.SelectedDate = CalculateInitialDate(filter.DateRange);
                    ctrl = dp;
                    break;

                case SqlFilterType.Enum:
                    var options = filter.Text?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                    tblEnum = new DataTable();
                    tblEnum.Columns.Add("Value");
                    foreach (var sValue in options)
                        tblEnum.Rows.Add(sValue);
                    ctrl = CreateLookupControl(filter, tblEnum);
                    /*
                    if (filter.IsMultiple) {
                        var lb = new ListBox { ItemsSource = options, SelectionMode = SelectionMode.Multiple, MaxHeight = 100 };
                        ctrl = lb;
                    } else {
                        var cb = new ComboBox { ItemsSource = options, HorizontalAlignment = HorizontalAlignment.Stretch };
                        if (options.Length > 0)
                            cb.SelectedIndex = 0;
                        ctrl = cb;
                    }
                    */
                    break;

                case SqlFilterType.Lookup:
                    tblLookUp = SqlStore.Select(filter.LookUpSelectSqlText);
                    ctrl = CreateLookupControl(filter, tblLookUp);
                    break;

                default: // String, Int, Dec
                    ctrl = new TextBox { Watermark = $"Enter {filter.Label}..." };
                    break;
            }
            
            
            if (ctrl != null)
            {
                ControlsDic[filter] = ctrl;
                container.Children.Add(ctrl);
                pnlFilters.Children.Add(container);

                switch (filter.Type)
                {
                    case SqlFilterType.Lookup:
                        MinusY += (GridHeight / 2);
                        break;
                    default:
                        MinusY += 30;
                        break;
                }
            }
        }

        return MinusY;
    }
    /// <summary>
    /// Gets the values from filter controls to the specified <see cref="SelectDef"/> filter items.
    /// </summary>
    public async Task ControlsToItem()
    {
        if (!await CheckIsOk(true)) return;

        foreach (var Entry in ControlsDic)
        {
            Entry.Key.Value = GetControlValue(Entry.Key);
        }
    }

    
    public bool IdColumnsVisible { get; set; }
}