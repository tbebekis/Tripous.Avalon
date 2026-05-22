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
 
    static IDataTemplate CreateTextDisplayTemplate(string ColumnName, TextAlignment Alignment, string Format, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();

            object Value = GetValue(Item, ColumnName);
            Result.Text = FormatValue(Value, Format);

            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = Alignment;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateTextEditTemplate(string ColumnName, TextAlignment Alignment, string Format, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBox Result = new();
            bool IsLoading = true;

            object Value = GetValue(Item, ColumnName);
            Result.Text = FormatValue(Value, Format);

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
                    Result.Focus(NavigationMethod.Tab, KeyModifiers.None);
                    Dispatcher.UIThread.Post(() => Result.SelectAll(), DispatcherPriority.Input);
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;

            return Result;
        }, SupportsRecycling);
    }
    
    static IDataTemplate CreateLookupDisplayTemplate(string ColumnName, LookupSource LookupSource, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();

            object Value = GetValue(Item, ColumnName);
            LookupItem LookupItem = LookupSource.FindItem(Value);

            //Console.WriteLine($"LOOKUP DISPLAY: Column={ColumnName}, Value={Value}, ValueType={Value?.GetType().FullName}, Found={LookupItem?.DisplayText}");

            Result.Text = LookupItem?.DisplayText ?? string.Empty;
            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = TextAlignment.Left;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateLookupEditTemplate(string ColumnName, LookupSource LookupSource, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            ComboBox Result = new();
            bool IsLoading = true;

            object CurrentValue = GetValue(Item, ColumnName);
            Result.SelectedItem = LookupSource.FindItem(CurrentValue);

            Result.ItemsSource = LookupSource.GetList();
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
                    Result.SelectedItem = LookupSource.FindItem(CurrentValue);

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
                Result.SelectedItem = LookupSource.FindItem(CurrentValue);
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
            bool Flag = false;

            if (!Sys.IsNull(Value))
            {
                if (Value is bool B)
                    Flag = B;
                else
                    Flag = Convert.ToInt32(Value) != 0;
            }

            Result.Text = Flag ? "x" : string.Empty;
            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = TextAlignment.Center;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateBoolEditTemplate(string ColumnName, bool SupportsRecycling)
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

    static public string GetHeader(string ColumnName, string Header) => string.IsNullOrWhiteSpace(Header) ? ColumnName.SplitToWords() : Header;
    static public string FormatValue(object Value, string Format)
    {
        if (Value == null)
            return string.Empty;
        if (!string.IsNullOrWhiteSpace(Format))
            return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Value);
        return string.Format(CultureInfo.CurrentCulture, "{0}", Value);
    }
    static public string GetDateAwareFormat(string FieldName, Type DataType, string Format)
    {
        if (DataType != typeof(DateTime))
            return Format;
        if (FieldName.EndsWithText("Date"))
            return Sys.Settings.DateFormat;
        if (FieldName.EndsWithText("DateTime") || FieldName.EndsWithText("DT"))
            return Sys.Settings.DateTimeFormat;
        return Format;
    }
    static public string GetDateAwareFormat(FieldDef FieldDef)
    {
        string Format = FieldDef.DisplayFormat;
        if (!FieldDef.DataType.IsDateTime())
            return Format;
        if (FieldDef.Name.EndsWithText("Date"))
            return Sys.Settings.DateFormat;
        if (FieldDef.Name.EndsWithText("DateTime") || FieldDef.Name.EndsWithText("DT"))
            return Sys.Settings.DateTimeFormat;
        return Format;
    }
 
    // ● private - create columns
    static DataGridColumn CreateTextColumn(string ColumnName, string Header = "", string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        TextAlignment Align = Alignment ?? TextAlignment.Left;

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName.SplitToWords() : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateTextDisplayTemplate(ColumnName, Align, Format, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateTextEditTemplate(ColumnName, Align, Format, SupportsRecycling);

        return Result;
    }
    static DataGridColumn CreateBoolColumn(string ColumnName, string Header = "", bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName.SplitToWords() : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateBoolDisplayTemplate(ColumnName, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateBoolEditTemplate(ColumnName, SupportsRecycling);

        return Result;
    }
    static DataGridColumn CreateLookupColumn(string ColumnName, LookupSource LookupSource, string Header = "", bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName.SplitToWords() : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateLookupDisplayTemplate(ColumnName, LookupSource, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateLookupEditTemplate(ColumnName, LookupSource, SupportsRecycling);

        return Result;
    }
    
    // ● static public
    static public List<DataGridColumn> BindGrid(SelectDef SelectDef, DataGrid Grid, DataView DataView, bool SupportsRecycling = false, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();

        if (SelectDef != null)
        {
            foreach (DataColumn Column in DataColumns)
            {
                if (SelectDef.DisplayLabels.TryGetValue(Column.ColumnName, out string Label))
                    Column.Caption = Label;

                if (SelectDef.ColumnTypes.TryGetValue(Column.ColumnName, out DataColumnType ColumnType))
                    Column.ExtendedProperties["ColumnType"] = ColumnType;
            }
        }

        List<DataGridColumn> Result = CreateColumns(Grid, DataColumns, SupportsRecycling);

        Grid.ItemsSource = new DataViewItemsSource(DataView);

        if (GoToFirst && DataView.Count > 0)
            Grid.SelectedItem = DataView[0];

        return Result;
    }
    static public List<DataGridColumn> BindGrid(DataGrid Grid, DataView DataView, bool SupportsRecycling = false, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();

        List<DataGridColumn> Result = CreateColumns(Grid, DataColumns, SupportsRecycling);
 
        Grid.ItemsSource = new DataViewItemsSource(DataView);

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
        Format = GetDateAwareFormat(Column.ColumnName, Column.DataType, Format);

        DataColumnType ColumnType = Column.ExtendedProperties.ContainsKey("ColumnType")
            ? (DataColumnType)Column.ExtendedProperties["ColumnType"]
            : DataColumnType.None;

        bool IsBoolean = ColumnType.HasFlag(DataColumnType.Boolean)
                         || Column.DataType == typeof(bool);
        
        TextAlignment Align = TextAlignment.Left;
        if (Alignment.HasValue)
            Align = Alignment.Value;
        else
            Align = IsBoolean ? TextAlignment.Center : Column.DataType.GetTextAlignment();
        
        
        DataGridColumn Result = null;
        if (IsBoolean)
            Result = CreateBoolColumn(Column.ColumnName, Header: Texts.L(Column.Caption), IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);
        else
            Result = CreateTextColumn(Column.ColumnName, Header: Texts.L(Column.Caption), Format: Format, Alignment: Align, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);

        string Caption = Texts.L(Column.Caption);
        Result.Header = Caption.SplitToWords();
        Result.IsReadOnly = IsReadOnly;
        
        GridColumnBinding CI = new GridColumnBinding(Result, Column);
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
            Result = CreateTextColumn(FieldDef.Name, Header: FieldDef.Title, Format: GetDateAwareFormat(FieldDef), Alignment: Align, IsReadOnly: FieldDef.IsReadOnly, SupportsRecycling: SupportsRecycling);

        Result.Header = FieldDef.Title.SplitToWords();
        Result.IsReadOnly = FieldDef.IsReadOnly;
        
        GridColumnBinding CI = new GridColumnBinding(Result, FieldDef);
        Result.Tag = CI;

        return Result;                  
    }
    
    static public DataGridColumn CreateLookupColumn(DataColumn Column, LookupDef LookupDef, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        LookupSource LookupSource = LookupDef.Create();
        
        DataGridColumn Result = CreateLookupColumn(Column.ColumnName, LookupSource, Column.Caption, IsReadOnly, SupportsRecycling: SupportsRecycling);
        GridColumnBinding CI = new GridColumnBinding(Result, Column);
        CI.LookupSource = LookupSource;
        Result.Tag = CI; 
        return Result;
    }
    static public DataGridColumn CreateLookupColumn(FieldDef FieldDef, LookupDef LookupDef = null, bool SupportsRecycling = false)
    {
        LookupDef = LookupDef ?? DataRegistry.Lookups.Get(FieldDef.LookupSource);
        LookupSource LookupSource = LookupDef.Create();
        
        DataGridColumn Result = CreateLookupColumn(FieldDef.Name, LookupSource, FieldDef.Title, IsReadOnly: FieldDef.IsReadOnly, SupportsRecycling: SupportsRecycling);
        GridColumnBinding CI = new GridColumnBinding(Result, FieldDef);
        CI.LookupSource = LookupSource;
        Result.Tag = CI;    
        return Result;
    }

    static public GridColumnBinding GetInfo(this DataGridColumn Column) => Column != null ? Column.Tag as GridColumnBinding : null;

    static public List<GridColumnBinding> GetInfoList(this DataGrid Grid)
    {
        List<GridColumnBinding> Result = new();

        GridColumnBinding CI;
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
