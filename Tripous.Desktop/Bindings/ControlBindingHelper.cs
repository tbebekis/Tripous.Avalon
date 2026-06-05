/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

static public class ControlBindingHelper
{
    // ● private
    static void EnsureLocatorFieldsCore(LocatorDef LocatorDef, FieldDef FieldDef)
    {
        if (LocatorDef == null || FieldDef?.TableDef == null || LocatorDef.Fields.Count > 0)
            return;

        TableDef JoinTable = FieldDef.TableDef.Joins.FirstOrDefault(item => item.MasterField.IsSameText(FieldDef.Name));
        if (JoinTable == null)
            return;

        List<FieldDef> Fields = JoinTable.Fields
            .Where(JoinField => JoinField.IsVisible && !JoinField.Name.IsSameText(JoinTable.KeyField))
            .OrderBy(JoinField =>
            {
                if (JoinField.Name.IsSameText("Code"))
                    return 0;
                if (JoinField.Name.IsSameText("Name"))
                    return 1;
                if (JoinField.Name.EndsWithText("Code"))
                    return 2;
                if (JoinField.Name.EndsWithText("Name"))
                    return 3;
                if (JoinField.DataType == DataFieldType.String)
                    return 4;
                return 5;
            })
            .ThenBy(JoinField => JoinField.Name)
            .Take(2)
            .ToList();

        foreach (FieldDef JoinField in Fields)
        {
            LocatorFieldDef LocatorField = new()
            {
                Name = JoinField.Name,
                Alias = JoinField.Alias,
                TargetField = JoinField.Alias,
                DataType = JoinField.DataType,
                IsVisible = JoinField.IsVisible,
                IsSearchable = JoinField.DataType == DataFieldType.String,
                DisplayWidth = JoinField.DisplayWidth
            };
            LocatorDef.Fields.Add(LocatorField);
        }
    }
    static DataRow GetCurrentRow(IRowProvider RowProvider)
    {
        return RowProvider?.CurrentRow;
    }
    static object GetValue(IRowProvider RowProvider, string FieldName)
    {
        DataRow Row = GetCurrentRow(RowProvider);
        if (Row == null)
            return null;

        object Result = Row[FieldName];
        return Result == DBNull.Value ? null : Result;
    }
    static void SetValue(IRowProvider RowProvider, string FieldName, object Value)
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

        object Current = Row[FieldName];

        if (!object.Equals(Current, Result))
            Row[FieldName] = Result;
  
    }
    
    static void RefreshTextBox(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not TextBox Box)
            return;

        object Value = GetValue(RowProvider, Binding.FieldName);
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
    static void RefreshComboBox(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not ComboBox Box)
            return;
        object Value = GetValue(RowProvider, Binding.FieldName);
        LookupItem Item =  Binding.LookupSource.FindItem(Value);
        Binding.IsRefreshing = true;
        try
        {
            if (Box.ItemsSource == null && Binding.LookupSource != null)
                Box.ItemsSource = Binding.LookupSource.GetList();
            Box.SelectedItem = null;
            if (Item != null)
                Box.SelectedItem = Item;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static void RefreshImage(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not Image Box)
            return;
        object Value = GetValue(RowProvider, Binding.FieldName);
        IImage Source = null;
        if (Value is string FilePath && !string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
            Source = new Bitmap(FilePath);
        else if (Value is byte[] Bytes && Bytes.Length > 0)
        {
            using MemoryStream Stream = new(Bytes);
            Source = new Bitmap(Stream);
        }
        Binding.IsRefreshing = true;
        try
        {
            Box.Source = Source;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static void RefreshLocatorBox(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not LocatorBox Control)
            return;

        DataRow Row = RowProvider != null ? RowProvider.CurrentRow : null;
        if (Row == null)
        {
            Control.KeyValue = DBNull.Value;
            Control.ClearTargetBoxes();
            return;
        }

        if (Row.Table.Columns.Contains(Binding.FieldDef.Name))
            Control.KeyValue = Row[Binding.FieldDef.Name];

        Control.RefreshTargetBoxes(Row);
    }

    static void RefreshCheckBox(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not CheckBox Box)
            return;
        
        Binding.IsRefreshing = true;
        try
        {
          object V = GetValue(RowProvider, Binding.FieldName);
          bool Value = false;

          if (!Sys.IsNull(V))
          {
              if (V is bool B)
                  Value = B;
              else if (Binding.FieldDef != null && Binding.FieldDef.Flags.HasFlag(FieldFlags.Boolean))
                  Value = Convert.ToInt32(V) != 0;
          }
          
          Box.IsChecked = Value;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    
    // ● static public
    static public void Refresh(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (RowProvider == null || Binding == null)
            return;

        if (Binding.Control is TextBox)
        {
            RefreshTextBox(RowProvider, Binding);
        }
        else if (Binding.Control is ComboBox)
        {
            RefreshComboBox(RowProvider, Binding);
        }
        else if (Binding.Control is CheckBox)
        {
            RefreshCheckBox(RowProvider, Binding);
        }
        else if (Binding.Control is DatePicker dp)
        {
            Binding.IsRefreshing = true;
            try { dp.SelectedDate = GetValue(RowProvider, Binding.FieldName) as DateTime?; }
            finally { Binding.IsRefreshing = false; }
        }
        else if (Binding.Control is CalendarDatePicker cdp)
        {
            Binding.IsRefreshing = true;
            try { cdp.SelectedDate = GetValue(RowProvider, Binding.FieldName) as DateTime?; }
            finally { Binding.IsRefreshing = false; }
        }
        else if (Binding.Control is NumericUpDown nu)
        {
            Binding.IsRefreshing = true;
            try { nu.Value = GetValue(RowProvider, Binding.FieldName) as decimal?; }
            finally { Binding.IsRefreshing = false; }
        }
        else if (Binding.Control is Image)
        {
            RefreshImage(RowProvider, Binding);
        }
        else if (Binding.Control is LocatorBox)
        {
            RefreshLocatorBox(RowProvider, Binding);
        }
    }

    static public void EnsureLocatorFields(LocatorDef LocatorDef, FieldDef FieldDef)
    {
        EnsureLocatorFieldsCore(LocatorDef, FieldDef);
    }
    static public ControlBinding Bind(IRowProvider RowProvider, TextBox Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        if (RowProvider == null)
            throw new TripousArgumentNullException(nameof(RowProvider));
        if (Box == null)
            throw new TripousArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new TripousArgumentNullException(nameof(FieldName));

        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef,
        };

        Box.IsReadOnly = FieldDef != null && FieldDef.Flags.HasFlag(FieldFlags.ReadOnlyUI);

        EventHandler<TextChangedEventArgs> TextChangedHandler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.Text);
        };

        Box.TextChanged += TextChangedHandler;

        Result.DisposeAction = () =>
        {
            Box.TextChanged -= TextChangedHandler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    static public ControlBinding BindMemo(IRowProvider RowProvider, TextBox Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        Box.AcceptsReturn = true;
        Box.TextWrapping = TextWrapping.Wrap;
        Box.Height = 80; // ή auto later
        return Bind(RowProvider, Box, FieldName, DataColumn, FieldDef);
    }
    
    static public ControlBinding Bind(IRowProvider RowProvider, CheckBox Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

        Box.IsEnabled = FieldDef == null || !FieldDef.Flags.HasFlag(FieldFlags.ReadOnlyUI);

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
    static public ControlBinding Bind(IRowProvider RowProvider, DatePicker Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

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
    static public ControlBinding Bind(IRowProvider RowProvider, CalendarDatePicker Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

        void NormalizeOrRefresh(string Text = null)
        {
            if (Result.IsRefreshing)
                return;

            Text ??= Box.Text;
            if (string.IsNullOrWhiteSpace(Text))
            {
                SetValue(RowProvider, FieldName, null);
                return;
            }

            if (DateTextNormalizer.TryNormalize(Text, out string NormalizedText, out DateTime Date))
            {
                Result.IsRefreshing = true;
                try
                {
                    Box.Text = NormalizedText;
                    Box.SelectedDate = Date;
                }
                finally
                {
                    Result.IsRefreshing = false;
                }

                SetValue(RowProvider, FieldName, Date);
            }
            else
            {
                Refresh(RowProvider, Result);
            }
        }

        EventHandler<SelectionChangedEventArgs> Handler = (s, e) =>
        {
            if (Result.IsRefreshing)
                return;

            SetValue(RowProvider, FieldName, Box.SelectedDate);
        };
        EventHandler<RoutedEventArgs> LostFocusHandler = (s, e) => NormalizeOrRefresh();
        EventHandler<CalendarDatePickerDateValidationErrorEventArgs> DateValidationErrorHandler = (s, e) =>
        {
            e.ThrowException = false;
            NormalizeOrRefresh(e.Text);
        };
        EventHandler<KeyEventArgs> KeyDownHandler = (Sender, Args) =>
        {
            // CalendarDatePicker does not navigate correctly with Shift+Tab when its drop-down is closed.
            if (Args.Key == Key.Tab && Args.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                DataForm Form = Box.FindAncestorOfType<DataForm>();
                Args.Handled = Form?.FocusPreviousEditableControl(Box) == true;
            }
        };

        Box.SelectedDateChanged += Handler;
        Box.LostFocus += LostFocusHandler;
        Box.DateValidationError += DateValidationErrorHandler;
        Box.AddHandler(InputElement.KeyDownEvent, KeyDownHandler, RoutingStrategies.Tunnel);

        Result.DisposeAction = () =>
        {
            Box.SelectedDateChanged -= Handler;
            Box.LostFocus -= LostFocusHandler;
            Box.DateValidationError -= DateValidationErrorHandler;
            Box.RemoveHandler(InputElement.KeyDownEvent, KeyDownHandler);
        };

        Refresh(RowProvider, Result);
        return Result;
    }
    
    static public ControlBinding Bind(IRowProvider RowProvider, ComboBox Box, string FieldName, DataColumn DataColumn, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

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
    static public ControlBinding Bind(IRowProvider RowProvider, ListBox Box, string FieldName, DataColumn DataColumn, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

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
    
    static public ControlBinding Bind(IRowProvider RowProvider, NumericUpDown Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

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
    
    static public ControlBinding BindLookup(IRowProvider RowProvider, ComboBox Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef)
    {
        if (FieldDef == null)
            throw new TripousArgumentNullException(nameof(FieldDef));
        if (string.IsNullOrWhiteSpace(FieldDef.LookupSource))
            throw new InvalidOperationException($"FieldDef '{FieldDef.Name}' has no LookupSource.");

        return BindLookup(RowProvider, Box, FieldName, DataColumn, FieldDef.LookupSource, FieldDef);
    }
    static public ControlBinding BindLookup(IRowProvider RowProvider, ComboBox Box, string FieldName, DataColumn DataColumn, string LookupSourceName, FieldDef FieldDef = null)
    {
        if (RowProvider == null)
            throw new TripousArgumentNullException(nameof(RowProvider));
        if (Box == null)
            throw new TripousArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new TripousArgumentNullException(nameof(FieldName));
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new TripousArgumentNullException(nameof(LookupSourceName));

        LookupDef LookupDef = DataRegistry.Lookups.Get(LookupSourceName);  

        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef,
            LookupSource = LookupDef.Create(),
        };

        Box.ItemsSource = Result.LookupSource.GetList();
        
        // WARNING:
        // Do NOT use ItemTemplate or SelectionBoxItemTemplate on this ComboBox.
        // In Avalonia, the drop-down items and the selection box are rendered differently.
        // Using a DataTemplate here may cause the selected item text NOT to appear
        // in the closed (selection) part of the ComboBox, even though the correct item is selected.
        // Rely on LookupItem.ToString() instead for display.
        Box.ItemTemplate = null;                
        Box.SelectionBoxItemTemplate = null;    
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

        Box.SelectionChanged += SelectionChangedHandler;

        Result.DisposeAction = () =>
        {
            Box.SelectionChanged -= SelectionChangedHandler;
        };

        Refresh(RowProvider, Result);
        return Result;
    }

    static public ControlBinding BindImage(IRowProvider RowProvider, Image Box, string FieldName, DataColumn DataColumn, FieldDef FieldDef = null)
    {
        if (RowProvider == null)
            throw new TripousArgumentNullException(nameof(RowProvider));
        if (Box == null)
            throw new TripousArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new TripousArgumentNullException(nameof(FieldName));

        ControlBinding Result = new()
        {
            Control = Box,
            FieldName =  FieldName,
            DataColumn = DataColumn,
            FieldDef = FieldDef
        };

        Box.Height = Ui.Settings.FormImageHeight;
        Box.Stretch = Ui.Settings.FormImageStretch;

        Refresh(RowProvider, Result);
        return Result;
    }

    static public ControlBinding Bind(IRowProvider RowProvider, LocatorBox Box, FieldDef FieldDef)
    {
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (FieldDef == null)
            throw new ArgumentNullException(nameof(FieldDef));
        if (string.IsNullOrWhiteSpace(FieldDef.Locator))
            throw new TripousException($"Field '{FieldDef.Name}' has no locator.");

        LocatorDef LocatorDef = DataRegistry.Locators[FieldDef.Locator];
        if (LocatorDef == null)
            throw new TripousException($"LocatorDef not found. Locator: {FieldDef.Locator}");
        EnsureLocatorFields(LocatorDef, FieldDef);

        Locator Locator = TypeStore.CreateInstance<Locator>(LocatorDef.ClassName);
        Locator.Initialize(LocatorDef);

        Box.Locator = Locator;
        Box.IsReadOnly = FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI || LocatorDef.IsReadOnly;
        
        ControlBinding Binding = new()
        {
            Control = Box,
            FieldName =  FieldDef.Name,
            FieldDef = FieldDef,
            LocatorDef = LocatorDef,
            Locator = Locator
        };

        Box.RowSelected += (Sender, Args) =>
        {
            if (Binding.IsRefreshing)
                return;

            DataRow Row = RowProvider != null ? RowProvider.CurrentRow : null;
            if (Row == null)
                return;

            Binding.IsRefreshing = true;
            try
            {
                Locator.Assign(Args.Row, Row);
                SetLocatorBoxValue(RowProvider, Binding);
                RefreshLocatorBox(RowProvider, Binding);
            }
            finally
            {
                Binding.IsRefreshing = false;
            }
        };

        RefreshLocatorBox(RowProvider, Binding);

        return Binding;
    }

    /// <summary>
    /// Writes the locator box key value to the current row.
    /// </summary>
    static public void SetLocatorBoxValue(IRowProvider RowProvider, ControlBinding Binding)
    {
        if (Binding.Control is not LocatorBox Control)
            return;

        DataRow Row = RowProvider != null ? RowProvider.CurrentRow : null;
        if (Row == null)
            return;

        DataColumn Column = Row.Table.FindColumn(Binding.FieldDef.Name);
        if (Column == null || Column.ReadOnly)
            return;

        object OldValue = Row[Column];
        object NewValue = Sys.IsNull(Control.KeyValue) ? DBNull.Value : Control.KeyValue;

        if (!Equals(OldValue, NewValue))
            Row[Column] = NewValue;
    }
}
