namespace Tripous.Desktop;

static public class ControlBindingHelper
{
    // ● private
    static private DataRow GetCurrentRow(IRowProvider RowProvider)
    {
        return RowProvider?.CurrentRow;
    }
    static private object GetValue(IRowProvider RowProvider, string FieldName)
    {
        DataRow Row = GetCurrentRow(RowProvider);
        if (Row == null)
            return null;

        object Result = Row[FieldName];
        return Result == DBNull.Value ? null : Result;
    }
    static private void SetValue(IRowProvider RowProvider, string FieldName, object Value)
    {
        DataRow Row = GetCurrentRow(RowProvider);
        if (Row == null)
            return;

        DataColumn Column = Row.Table.Columns[FieldName];
        Type DataType = Column.DataType;

        if (Value == null || Value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(Value)))
        {
            Row[FieldName] = DBNull.Value;
            return;
        }

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

        Row[FieldName] = Result;
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
    static private void RefreshTextBox(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not TextBox Box)
            return;

        object Value = GetValue(RowProvider, Binding.ColumnName);
        string Text = Value == null ? string.Empty : Convert.ToString(Value, CultureInfo.CurrentCulture);

        Binding.IsRefreshing = true;
        try
        {
            if (!string.Equals(Box.Text, Text, StringComparison.Ordinal))
                Box.Text = Text;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static private void RefreshComboBox(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not ComboBox Box)
            return;

        object Value = GetValue(RowProvider, Binding.ColumnName);
        LookupItem Item = FindLookupItem(Binding.LookupSource, Value);

        Binding.IsRefreshing = true;
        try
        {
            if (!ReferenceEquals(Box.SelectedItem, Item))
                Box.SelectedItem = Item;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static private FuncDataTemplate<LookupItem> CreateLookupItemTemplate()
    {
        return new FuncDataTemplate<LookupItem>((Item, _) =>
        {
            TextBlock Result = new();

            Result.Text = Item?.DisplayText ?? string.Empty;
            Result.VerticalAlignment = VerticalAlignment.Center;

            return Result;
        }, true);
    }

    // ● static public
    static public void Refresh(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (RowProvider == null || Binding == null)
            return;

        if (Binding.Control is TextBox)
            RefreshTextBox(RowProvider, Binding);
        else if (Binding.Control is ComboBox)
            RefreshComboBox(RowProvider, Binding);
        else if (Binding.Control is CheckBox cb)
        {
            bool Value = GetValue(RowProvider, Binding.ColumnName) is bool b && b;
            Binding.IsRefreshing = true;
            try { cb.IsChecked = Value; }
            finally { Binding.IsRefreshing = false; }
        }
        else if (Binding.Control is DatePicker dp)
        {
            Binding.IsRefreshing = true;
            try { dp.SelectedDate = GetValue(RowProvider, Binding.ColumnName) as DateTime?; }
            finally { Binding.IsRefreshing = false; }
        }
        else if (Binding.Control is NumericUpDown nu)
        {
            Binding.IsRefreshing = true;
            try { nu.Value = GetValue(RowProvider, Binding.ColumnName) as decimal?; }
            finally { Binding.IsRefreshing = false; }
        }
    }
    
    static public ControlBinding Bind(IRowProvider RowProvider, TextBox Box, FieldDef FieldDef)
    {
        if (FieldDef == null)
            throw new ArgumentNullException(nameof(FieldDef));

        return Bind(RowProvider, Box, FieldDef.Name, FieldDef);
    }
    static public ControlBinding Bind(IRowProvider RowProvider, TextBox Box, string FieldName, FieldDef FieldDef = null)
    {
        if (RowProvider == null)
            throw new ArgumentNullException(nameof(RowProvider));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException(nameof(FieldName));

        ControlBinding Result = new(Box, FieldName)
        {
            FieldDef = FieldDef,
        };

        Box.IsReadOnly = FieldDef != null && FieldDef.Flags.HasFlag(FieldFlags.ReadOnlyUI);

        EventHandler<TextChangedEventArgs> TextChangedHandler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.Text);
        };

        EventHandler<KeyEventArgs> KeyDownHandler = (Sender, Args) =>
        {
            if (Args.Key != Key.Escape)
                return;

            DataRow Row = GetCurrentRow(RowProvider);
            Row?.CancelEdit();

            Refresh(RowProvider, Result);
            Args.Handled = true;
        };

        Box.TextChanged += TextChangedHandler;
        Box.KeyDown += KeyDownHandler;

        Result.DisposeAction = () =>
        {
            Box.TextChanged -= TextChangedHandler;
            Box.KeyDown -= KeyDownHandler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    static public ControlBinding BindMemo(IRowProvider RowProvider, TextBox Box, string FieldName, FieldDef FieldDef = null)
    {
        Box.AcceptsReturn = true;
        Box.TextWrapping = TextWrapping.Wrap;
        Box.Height = 80; // ή auto later
        return Bind(RowProvider, Box, FieldName, FieldDef);
    }
    
    static public ControlBinding Bind(IRowProvider RowProvider, CheckBox Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = new(Box, FieldName) { FieldDef = FieldDef };

        EventHandler<RoutedEventArgs> Handler = (s, e) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.IsChecked == true);
        };

        Box.IsCheckedChanged += Handler;

        Result.DisposeAction = () =>
        {
            Box.IsCheckedChanged -= Handler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    static public ControlBinding Bind(IRowProvider RowProvider, DatePicker Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = new(Box, FieldName) { FieldDef = FieldDef };

        EventHandler<DatePickerSelectedValueChangedEventArgs> Handler = (s, e) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.SelectedDate);
        };

        Box.SelectedDateChanged += Handler;

        Result.DisposeAction = () =>
        {
            Box.SelectedDateChanged -= Handler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    
    static public ControlBinding Bind(IRowProvider RowProvider, ComboBox Box, string FieldName, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = new(Box, FieldName) { FieldDef = FieldDef };

        Box.ItemsSource = Items;

        EventHandler<SelectionChangedEventArgs> Handler = (s, e) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.SelectedItem);
        };

        Box.SelectionChanged += Handler;

        Result.DisposeAction = () =>
        {
            Box.SelectionChanged -= Handler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    static public ControlBinding Bind(IRowProvider RowProvider, ListBox Box, string FieldName, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = new(Box, FieldName) { FieldDef = FieldDef };

        Box.ItemsSource = Items;

        EventHandler<SelectionChangedEventArgs> Handler = (s, e) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.SelectedItem);
        };

        Box.SelectionChanged += Handler;

        Result.DisposeAction = () =>
        {
            Box.SelectionChanged -= Handler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    
    static public ControlBinding Bind(IRowProvider RowProvider, NumericUpDown Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = new(Box, FieldName) { FieldDef = FieldDef };

        EventHandler<NumericUpDownValueChangedEventArgs> Handler = (s, e) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.Value);
        };

        Box.ValueChanged += Handler;

        Result.DisposeAction = () =>
        {
            Box.ValueChanged -= Handler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    
    static public ControlBinding BindLookup(IRowProvider RowProvider, ComboBox Box, string FieldName, FieldDef FieldDef)
    {
        if (FieldDef == null)
            throw new ArgumentNullException(nameof(FieldDef));
        if (string.IsNullOrWhiteSpace(FieldDef.LookupSource))
            throw new InvalidOperationException($"FieldDef '{FieldDef.Name}' has no LookupSource.");

        return BindLookup(RowProvider, Box, FieldName, FieldDef.LookupSource, FieldDef);
    }
    static public ControlBinding BindLookup(IRowProvider RowProvider, ComboBox Box, string FieldName, string LookupSourceName, FieldDef FieldDef = null)
    {
        if (RowProvider == null)
            throw new ArgumentNullException(nameof(RowProvider));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException(nameof(FieldName));
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new ArgumentNullException(nameof(LookupSourceName));

        ILookupSource LookupSource = LookUpSource.GetLookupSource(LookupSourceName);

        ControlBinding Result = new(Box, FieldName)
        {
            FieldDef = FieldDef,
            LookupSource = LookupSource,
        };

        Box.ItemsSource = LookupSource.List;
        Box.ItemTemplate = CreateLookupItemTemplate();
        Box.IsEnabled = FieldDef == null || !FieldDef.Flags.HasFlag(FieldFlags.ReadOnlyUI);

        EventHandler<SelectionChangedEventArgs> SelectionChangedHandler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;

            if (Box.SelectedItem is LookupItem Item)
                SetValue(RowProvider, FieldName, Item.Value);
            else
                SetValue(RowProvider, FieldName, null);
        };

        EventHandler<KeyEventArgs> KeyDownHandler = (Sender, Args) =>
        {
            if (Args.Key != Key.Escape)
                return;

            DataRow Row = GetCurrentRow(RowProvider);
            Row?.CancelEdit();

            Refresh(RowProvider, Result);
            Args.Handled = true;
        };

        Box.SelectionChanged += SelectionChangedHandler;
        Box.KeyDown += KeyDownHandler;

        Result.DisposeAction = () =>
        {
            Box.SelectionChanged -= SelectionChangedHandler;
            Box.KeyDown -= KeyDownHandler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
}