namespace Tripous.Desktop;

public static class DataViewGridColumnFactory
{
    // ● private
    static private Thickness GetCellPadding()
    {
        return new Thickness(6, 2, 6, 2);
    }
    
    static private object GetValue(DataRowView RowView, string ColumnName)
    {
        if (RowView == null)
            return null;

        object Result = RowView[ColumnName];
        return Result == DBNull.Value ? null : Result;
    }
    static private void SetValue(DataRowView RowView, string ColumnName, object Value)
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

            RowView[ColumnName] = Result;
        }
        catch
        {
            // ● ignore invalid input for now
        }
    }
    static private LookupItem FindLookupItem(ILookupSource Source, object Value)
    {
        if (Source == null)
            return null;

        if (Value == DBNull.Value)
            Value = null;

        foreach (LookupItem Item in Source.List)
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
 
    static private IDataTemplate CreateTextDisplayTemplate(string ColumnName, TextAlignment Alignment, string Format)
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
        }, true);
    }
    static private IDataTemplate CreateTextEditTemplate(string ColumnName, TextAlignment Alignment)
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
        }, true);
    }
    
    static private IDataTemplate CreateLookupDisplayTemplate(string ColumnName, ILookupSource Source)
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
        }, true);
    }
    static private IDataTemplate CreateLookupEditTemplate(string ColumnName, ILookupSource Source)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            ComboBox Result = new();
            bool IsLoading = true;

            object CurrentValue = GetValue(Item, ColumnName);
            Result.SelectedItem = FindLookupItem(Source, CurrentValue);

            Result.ItemsSource = Source.List;
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
            }, true);

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
        }, true);
    }
    
    static private IDataTemplate CreateBoolDisplayTemplate(string ColumnName)
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
        }, true);
    }
    static private IDataTemplate CreateBoolEditTemplate(string ColumnName)
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
        }, true);
    }

    // ● static public
    static public DataGridColumn CreateTextColumn(string ColumnName, string Header = "", string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false)
    {
        DataGridTemplateColumn Result = new();

        TextAlignment Align = Alignment ?? TextAlignment.Left;

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateTextDisplayTemplate(ColumnName, Align, Format);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateTextEditTemplate(ColumnName, Align);

        return Result;
    }
    static public DataGridColumn CreateLookupColumn(string ColumnName, ILookupSource Source, string Header = "", bool IsReadOnly = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateLookupDisplayTemplate(ColumnName, Source);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateLookupEditTemplate(ColumnName, Source);

        return Result;
    }
    static public DataGridColumn CreateBoolColumn(string ColumnName, string Header = "", bool IsReadOnly = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = string.IsNullOrWhiteSpace(Header) ? ColumnName : Header;
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateBoolDisplayTemplate(ColumnName);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateBoolEditTemplate(ColumnName);

        return Result;
    }
}