namespace Tripous.Desktop;
 
public static class DataGridBinder
{
    // ● private
    static Thickness GetCellPadding()
    {
        return new Thickness(6, 2, 6, 2);
    }
    
    static object GetValue(DataRowView RowView, string ColumnName)
    {
        if (RowView == null || RowView.Row == null || RowView.Row.RowState.In(DataRowState.Deleted))
            return null;

        object Result = RowView[ColumnName];
        return Result == DBNull.Value ? null : Result;
    }
    static void SetValue(DataRowView RowView, string ColumnName, object Value)
    {
        if (RowView == null)
            return;

        DataColumn Column = RowView.Row.Table.Columns[ColumnName];
        Type DataType = Column.DataType;

        if (Value == null || Value == DBNull.Value)
        {
            RowView[ColumnName] = DBNull.Value;
            return;
        }

        try
        {
            object Result;

            if (DataType == typeof(string))
                Result = Convert.ToString(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(int))
                Result = Convert.ToInt32(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(long))
                Result = Convert.ToInt64(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(decimal))
                Result = Convert.ToDecimal(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(double))
                Result = Convert.ToDouble(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(float))
                Result = Convert.ToSingle(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(bool))
                Result = Convert.ToBoolean(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(DateTime))
                Result = Convert.ToDateTime(Value, CultureInfo.CurrentCulture);
            else
                Result = Value;

            //RowView[ColumnName] = Result;
            object Current = RowView[ColumnName];

            if (!object.Equals(Current, Result))
                RowView[ColumnName] = Result;
        }
        catch
        {
            // ● ignore invalid input for now
        }
    }
    static LookupItem FindLookupItem(LookupSource Source, object Value)
    {
        if (Source == null)
            return null;

        if (Value == DBNull.Value)
            Value = null;

        foreach (LookupItem Item in Source.GetList())
        {
            if (Item.IsNullItem && Value == null)
                return Item;

            if (Item.Value == null && Value == null)
                return Item;

            if (Item.Value != null && Value != null)
            {
                if (Equals(Item.Value, Value))
                    return Item;

                if (Convert.ToString(Item.Value, CultureInfo.InvariantCulture) == Convert.ToString(Value, CultureInfo.InvariantCulture))
                    return Item;
            }
        }

        return null;
    }
 
    static IDataTemplate CreateTextDisplayTemplate(string ColumnName, TextAlignment Alignment, string Format, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();

            object Value = GetValue(Item, ColumnName);
            Result.Text = Value == null ? string.Empty : string.Format(CultureInfo.CurrentCulture, "{0}", Value);

            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = Alignment;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateTextEditTemplate(string ColumnName, TextAlignment Alignment, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBox Result = new();
            bool IsLoading = true;

            object Value = GetValue(Item, ColumnName);
            Result.Text = Value == null ? string.Empty : Convert.ToString(Value, CultureInfo.CurrentCulture);

            Result.Padding = GetCellPadding();
            Result.VerticalContentAlignment = VerticalAlignment.Center;
            Result.TextAlignment = Alignment;

            Result.TextChanged += (Sender, Args) =>
            {
                if (IsLoading)
                    return;

                SetValue(Item, ColumnName, Result.Text);
            };

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    Item?.BeginEdit();
                    IsLoading = false;
                    Result.Focus();
                    Result.SelectAll();
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;

            return Result;
        }, SupportsRecycling);
    }
    
    static IDataTemplate CreateLookupDisplayTemplate(string ColumnName, LookupSource Source, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();

            object Value = GetValue(Item, ColumnName);
            LookupItem LookupItem = FindLookupItem(Source, Value);

            //Console.WriteLine($"LOOKUP DISPLAY: Column={ColumnName}, Value={Value}, ValueType={Value?.GetType().FullName}, Found={LookupItem?.DisplayText}");

            Result.Text = LookupItem?.DisplayText ?? string.Empty;
            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = TextAlignment.Left;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateLookupEditTemplate(string ColumnName, LookupSource Source, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            ComboBox Result = new();
            bool IsLoading = true;

            object CurrentValue = GetValue(Item, ColumnName);
            Result.SelectedItem = FindLookupItem(Source, CurrentValue);

            Result.ItemsSource = Source.GetList();
            Result.Padding = new Thickness(0);
            Result.Margin = new Thickness(0);
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.VerticalAlignment = VerticalAlignment.Stretch;
            Result.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Result.VerticalContentAlignment = VerticalAlignment.Center;
            Result.MinHeight = 0;

            Result.ItemTemplate = new FuncDataTemplate<LookupItem>((LookupItem, _) =>
            {
                TextBlock Text = new();

                Text.Text = LookupItem?.DisplayText ?? string.Empty;
                Text.Padding = GetCellPadding();
                Text.TextAlignment = TextAlignment.Left;
                Text.VerticalAlignment = VerticalAlignment.Center;
                Text.HorizontalAlignment = HorizontalAlignment.Stretch;

                return Text;
            }, SupportsRecycling);

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    Item?.BeginEdit();

                    object CurrentValue = GetValue(Item, ColumnName);
                    Result.SelectedItem = FindLookupItem(Source, CurrentValue);

                    IsLoading = false;

                    Result.Focus();
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;
 
            Result.SelectionChanged += (Sender, Args) =>
            {
                if (IsLoading)
                    return;

                if (Result.SelectedItem is LookupItem SelectedItem)
                {
                    SetValue(Item, ColumnName, SelectedItem.Value);

                    DataGrid Grid = Result.FindAncestorOfType<DataGrid>();
                    Grid?.CommitEdit(DataGridEditingUnit.Cell, true);
                }
            };

            Result.DropDownOpened += (Sender, Args) =>
            {
                object CurrentValue = GetValue(Item, ColumnName);
                Result.SelectedItem = FindLookupItem(Source, CurrentValue);
            };

            Result.KeyDown += (Sender, Args) =>
            {
                if (Args.Key != Key.Escape)
                    return;

                if (Result.IsDropDownOpen)
                {
                    Result.IsDropDownOpen = false;
                    Args.Handled = true;
                    return;
                }

                DataGrid Grid = Result.FindAncestorOfType<DataGrid>();

                Item?.CancelEdit();
                Grid?.CancelEdit();

                Args.Handled = true;
            };

            return Result;
        }, SupportsRecycling);
    }
    
    static IDataTemplate CreateBoolDisplayTemplate(string ColumnName, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();

            object Value = GetValue(Item, ColumnName);
            bool Flag = Value is bool B && B;

            Result.Text = Flag ? "x" : string.Empty;
            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = TextAlignment.Center;

            return Result;
        }, SupportsRecycling);
    }
    static private IDataTemplate CreateBoolEditTemplate(string ColumnName, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            CheckBox Result = new();

            Result.Bind(ToggleButton.IsCheckedProperty, new Binding($"[{ColumnName}]", BindingMode.TwoWay));
            Result.HorizontalAlignment = HorizontalAlignment.Center;
            Result.VerticalAlignment = VerticalAlignment.Center;

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    Item?.BeginEdit();
                    Result.Focus();
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;

            Result.KeyDown += (Sender, Args) =>
            {
                if (Args.Key != Key.Escape)
                    return;

                DataGrid Grid = Result.FindAncestorOfType<DataGrid>();

                Item?.CancelEdit();
                Grid?.CancelEdit();

                Args.Handled = true;
            };

            return Result;
        }, SupportsRecycling);
    }

    // ● private - create columns
    static DataGridColumn CreateTextColumn(string ColumnName, string Header = "", string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        TextAlignment Align = Alignment ?? TextAlignment.Left;

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateTextDisplayTemplate(ColumnName, Align, Format, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateTextEditTemplate(ColumnName, Align, SupportsRecycling);

        return Result;
    }
    static DataGridColumn CreateBoolColumn(string ColumnName, string Header = "", bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateBoolDisplayTemplate(ColumnName, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateBoolEditTemplate(ColumnName, SupportsRecycling);

        return Result;
    }
    static DataGridColumn CreateLookupColumn(string ColumnName, LookupSource Source, string Header = "", bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateLookupDisplayTemplate(ColumnName, Source, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateLookupEditTemplate(ColumnName, Source, SupportsRecycling);

        return Result;
    }
    
    // ● static public
    static public List<DataGridColumn> BindGrid(DataGrid Grid, DataView DataView, bool SupportsRecycling = false, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();

        List<DataGridColumn> Result = CreateColumns(Grid, DataColumns, SupportsRecycling);
 
        Grid.ItemsSource = DataView;

        if (GoToFirst && DataView.Count > 0)
            Grid.SelectedItem = DataView[0];

        return Result;
    }
    static public void UnBindGrid(DataGrid Grid)
    {
        Grid.ItemsSource = null;
        Grid.Columns.Clear();
    }
    
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, DataTable Table, bool SupportsRecycling = false) => CreateColumns(Grid, Table.Columns.Cast<DataColumn>().ToArray(), SupportsRecycling);
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, DataColumn[] DataColumns, bool SupportsRecycling = false)
    {
        List<DataGridColumn> Result = new();
        DataGridColumn GridColumn;
        
        foreach (DataColumn Column in DataColumns)
        {
            GridColumn = CreateGridColumn(Column,
                Format: Column.DataType.GetDefaultFormat(),
                Alignment: Column.DataType.GetTextAlignment(),
                IsReadOnly: Column.ReadOnly,
                SupportsRecycling: SupportsRecycling);
            
            Result.Add(GridColumn);
        }
        
        Grid.Columns.AddRange(Result);
        return Result;
    }  
    
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, TableDef TableDef, bool SupportsRecycling = false) => CreateColumns(Grid, TableDef.Fields.ToArray(), SupportsRecycling);
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, FieldDef[] FieldDefs, bool SupportsRecycling = false)
    {
        List<DataGridColumn> Result = new();
        DataGridColumn GridColumn;
        
        foreach (FieldDef FieldDef in FieldDefs)
        {
            GridColumn = CreateGridColumn(FieldDef, SupportsRecycling: SupportsRecycling);
            Result.Add(GridColumn);
        }

        Grid.Columns.AddRange(Result);
        return Result;
    }

    static public DataGridColumn CreateGridColumn(DataColumn Column, string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        bool IsBoolean = Column.DataType == typeof(bool) 
                         || (Column.ExtendedProperties.ContainsKey("IsBoolean") && Convert.ToBoolean(Column.ExtendedProperties["IsBoolean"]));
        
        TextAlignment Align = TextAlignment.Left;
        if (Alignment.HasValue)
            Align = Alignment.Value;
        else
            Align = IsBoolean ? TextAlignment.Center : Column.DataType.GetTextAlignment();
        
        DataGridColumn Result = null;
        if (IsBoolean)
            Result = CreateBoolColumn(Column.ColumnName, Header: Column.Caption, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);
        else
            Result = CreateTextColumn(Column.ColumnName, Header: Column.Caption, Format: Format, Alignment: Align, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);

        Result.Header = Column.Caption;
        Result.IsReadOnly = IsReadOnly;
        
        GridColumnInfo CI = new GridColumnInfo(Result, Column);
        Result.Tag = CI;

        return Result;
    }
    static public DataGridColumn CreateGridColumn(FieldDef FieldDef, bool SupportsRecycling = false)
    {
        bool IsBoolean = FieldDef.IsBoolean;
        TextAlignment Align = IsBoolean ? TextAlignment.Center : FieldDef.DataType.GetTextAlignment();
 
        DataGridColumn Result = null;
        if (IsBoolean)
            Result = CreateBoolColumn(FieldDef.Name, Header: FieldDef.Title, IsReadOnly: FieldDef.IsReadOnly, SupportsRecycling: SupportsRecycling);
        else
            Result = CreateTextColumn(FieldDef.Name, Header: FieldDef.Title, Format: FieldDef.DisplayFormat, Alignment: Align, IsReadOnly: FieldDef.IsReadOnly, SupportsRecycling: SupportsRecycling);

        Result.Header = FieldDef.Title;
        Result.IsReadOnly = FieldDef.IsReadOnly;
        
        GridColumnInfo CI = new GridColumnInfo(Result, FieldDef);
        Result.Tag = CI;

        return Result;                  
    }
    
    static public DataGridColumn CreateLookupColumn(DataColumn Column, LookupSource Source, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridColumn Result = CreateLookupColumn(Column.ColumnName, Source, Column.Caption, IsReadOnly, SupportsRecycling: SupportsRecycling);
        GridColumnInfo CI = new GridColumnInfo(Result, Column);
        Result.Tag = CI; 
        return Result;
    }
    static public DataGridColumn CreateLookupColumn(FieldDef FieldDef, LookupSource Source = null, bool SupportsRecycling = false)
    {
        Source = Source ?? DataRegistry.LookupSources.Get(FieldDef.LookupSource);
        DataGridColumn Result = CreateLookupColumn(FieldDef.Name, Source, FieldDef.Title, IsReadOnly: FieldDef.IsReadOnly, SupportsRecycling: SupportsRecycling);
        GridColumnInfo CI = new GridColumnInfo(Result, FieldDef);
        Result.Tag = CI;    
        return Result;
    }

    static public GridColumnInfo GetInfo(this DataGridColumn Column) => Column != null ? Column.Tag as GridColumnInfo : null;

    static public List<GridColumnInfo> GetInfoList(this DataGrid Grid)
    {
        List<GridColumnInfo> Result = new();

        GridColumnInfo CI;
        if (Grid != null && Grid.Columns.Count > 0)
        {
            foreach (var GridColumn in Grid.Columns)
            {
                CI = GridColumn.GetInfo();
                if (CI != null)
                    Result.Add(CI);
            }
        }

        return Result;
    }
}