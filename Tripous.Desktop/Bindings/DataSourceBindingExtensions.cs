namespace Tripous.Desktop;

/// <summary>
/// Provides Avalonia binding helpers for DataSource instances.
/// </summary>
static public class DataSourceBindingExtensions
{
    // ● private
    const string RowsPath = "Rows";
    const string CurrentPath = "Current";

    static void CheckArgs(DataSource Source, Control Control, string FieldName)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));
        if (Control == null)
            throw new ArgumentNullException(nameof(Control));
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException(nameof(FieldName));
    }
    static void CheckGridArgs(DataSource Source, DataGrid Grid)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));
    }
    static string GetFieldName(FieldDef Field)
    {
        if (Field == null)
            throw new ArgumentNullException(nameof(Field));
        if (string.IsNullOrWhiteSpace(Field.Name))
            throw new ArgumentNullException(nameof(Field.Name));

        return Field.Name;
    }
    static string GetCurrentFieldPath(string FieldName)
    {
        return CurrentPath + "[" + FieldName + "]";
    }
    static string GetRowFieldPath(string FieldName)
    {
        return "[" + FieldName + "]";
    }
    static Binding CreateCurrentBinding(string FieldName, UpdateSourceTrigger UpdateSourceTrigger = UpdateSourceTrigger.Default)
    {
        return new Binding(GetCurrentFieldPath(FieldName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger
        };
    }
    static Binding CreateRowBinding(string FieldName)
    {
        return new Binding(GetRowFieldPath(FieldName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
        };
    }
    static DataSourceBinding BindControl(DataSource Source, Control Control, string FieldName, AvaloniaProperty TargetProperty, UpdateSourceTrigger UpdateSourceTrigger = UpdateSourceTrigger.Default)
    {
        CheckArgs(Source, Control, FieldName);
        Control.DataContext = Source;
        IDisposable Subscription = Control.Bind(TargetProperty, CreateCurrentBinding(FieldName, UpdateSourceTrigger));
        return new DataSourceBinding(Source, Control, FieldName, TargetProperty, Subscription);
    }
    static bool AsBoolean(object Value)
    {
        if (Sys.IsNull(Value))
            return false;
        if (Value is bool BoolValue)
            return BoolValue;

        return Convert.ToInt32(Value) != 0;
    }
    static DataSourceBinding BindBooleanControl(DataSource Source, Control Control, string FieldName, AvaloniaProperty TargetProperty)
    {
        CheckArgs(Source, Control, FieldName);
        bool Updating = false;

        void Pull()
        {
            Updating = true;
            try
            {
                Control.SetValue(TargetProperty, Source.Current != null && AsBoolean(Source.Current[FieldName]));
            }
            finally
            {
                Updating = false;
            }
        }
        void Push(bool Value)
        {
            if (!Updating && Source.Current != null)
                Source.Current[FieldName] = Value;
        }
        void Source_PropertyChanged(object Sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentPath)
                Pull();
        }
        void Source_Changed(object Sender, DataSourceChangeEventArgs e)
        {
            if (ReferenceEquals(e.Row, Source.Current) && e.FieldName == FieldName)
                Pull();
        }
        void CheckBox_IsCheckedChanged(object Sender, RoutedEventArgs e)
        {
            if (Control is CheckBox Box)
                Push(Box.IsChecked == true);
        }
        void ToggleSwitch_IsCheckedChanged(object Sender, RoutedEventArgs e)
        {
            if (Control is ToggleSwitch Box)
                Push(Box.IsChecked == true);
        }

        Source.PropertyChanged += Source_PropertyChanged;
        Source.Changed += Source_Changed;

        if (Control is CheckBox CheckBox)
            CheckBox.IsCheckedChanged += CheckBox_IsCheckedChanged;
        else if (Control is ToggleSwitch ToggleSwitch)
            ToggleSwitch.IsCheckedChanged += ToggleSwitch_IsCheckedChanged;

        Pull();

        DataSourceBindingSubscription Subscription = new(() =>
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source.Changed -= Source_Changed;

            if (Control is CheckBox CheckBox)
                CheckBox.IsCheckedChanged -= CheckBox_IsCheckedChanged;
            else if (Control is ToggleSwitch ToggleSwitch)
                ToggleSwitch.IsCheckedChanged -= ToggleSwitch_IsCheckedChanged;
        });

        return new DataSourceBinding(Source, Control, FieldName, TargetProperty, Subscription);
    }
    static DateTime? AsDateTime(object Value)
    {
        if (Sys.IsNull(Value))
            return null;
        if (Value is DateTime DateTimeValue)
            return DateTimeValue;

        return Convert.ToDateTime(Value);
    }
    static DataSourceBinding BindDateControl(DataSource Source, Control Control, string FieldName, AvaloniaProperty TargetProperty)
    {
        CheckArgs(Source, Control, FieldName);
        bool Updating = false;

        void Pull()
        {
            Updating = true;
            try
            {
                Control.SetValue(TargetProperty, Source.Current != null ? AsDateTime(Source.Current[FieldName]) : null);
            }
            finally
            {
                Updating = false;
            }
        }
        void Push(DateTime? Value)
        {
            if (!Updating && Source.Current != null)
                Source.Current[FieldName] = Value;
        }
        void Source_PropertyChanged(object Sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentPath)
                Pull();
        }
        void Source_Changed(object Sender, DataSourceChangeEventArgs e)
        {
            if (ReferenceEquals(e.Row, Source.Current) && e.FieldName == FieldName)
                Pull();
        }
        void DatePicker_SelectedDateChanged(object Sender, DatePickerSelectedValueChangedEventArgs e)
        {
            if (Control is DatePicker Box)
                Push(Box.SelectedDate?.DateTime);
        }
        void CalendarDatePicker_SelectedDateChanged(object Sender, SelectionChangedEventArgs e)
        {
            if (Control is CalendarDatePicker Box)
                Push(Box.SelectedDate);
        }

        Source.PropertyChanged += Source_PropertyChanged;
        Source.Changed += Source_Changed;

        if (Control is DatePicker DatePicker)
            DatePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
        else if (Control is CalendarDatePicker CalendarDatePicker)
            CalendarDatePicker.SelectedDateChanged += CalendarDatePicker_SelectedDateChanged;

        Pull();

        DataSourceBindingSubscription Subscription = new(() =>
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source.Changed -= Source_Changed;

            if (Control is DatePicker DatePicker)
                DatePicker.SelectedDateChanged -= DatePicker_SelectedDateChanged;
            else if (Control is CalendarDatePicker CalendarDatePicker)
                CalendarDatePicker.SelectedDateChanged -= CalendarDatePicker_SelectedDateChanged;
        });

        return new DataSourceBinding(Source, Control, FieldName, TargetProperty, Subscription);
    }
    static decimal? AsDecimal(object Value)
    {
        if (Sys.IsNull(Value))
            return null;

        return Convert.ToDecimal(Value);
    }
    static DataSourceBinding BindNumericControl(DataSource Source, NumericUpDown Control, string FieldName)
    {
        CheckArgs(Source, Control, FieldName);
        bool Updating = false;

        void Pull()
        {
            Updating = true;
            try
            {
                Control.Value = Source.Current != null ? AsDecimal(Source.Current[FieldName]) : null;
            }
            finally
            {
                Updating = false;
            }
        }
        void Push(decimal? Value)
        {
            if (!Updating && Source.Current != null)
                Source.Current[FieldName] = Value;
        }
        void Source_PropertyChanged(object Sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentPath)
                Pull();
        }
        void Source_Changed(object Sender, DataSourceChangeEventArgs e)
        {
            if (ReferenceEquals(e.Row, Source.Current) && e.FieldName == FieldName)
                Pull();
        }
        void Control_ValueChanged(object Sender, NumericUpDownValueChangedEventArgs e)
        {
            Push(Control.Value);
        }

        Source.PropertyChanged += Source_PropertyChanged;
        Source.Changed += Source_Changed;
        Control.ValueChanged += Control_ValueChanged;
        Pull();

        DataSourceBindingSubscription Subscription = new(() =>
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source.Changed -= Source_Changed;
            Control.ValueChanged -= Control_ValueChanged;
        });

        return new DataSourceBinding(Source, Control, FieldName, NumericUpDown.ValueProperty, Subscription);
    }
    static DataSourceBinding BindLookupControl(DataSource Source, ComboBox Control, string FieldName, string LookupSourceName, FieldDef Field = null)
    {
        CheckArgs(Source, Control, FieldName);
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new ArgumentNullException(nameof(LookupSourceName));

        LookupDef LookupDef = DataRegistry.Lookups.Get(LookupSourceName);
        LookupSource LookupSource = LookupDef.Create();
        bool Updating = false;

        Control.ItemsSource = LookupSource.GetList();
        Control.ItemTemplate = null;
        Control.SelectionBoxItemTemplate = null;

        void Pull()
        {
            Updating = true;
            try
            {
                object Value = Source.Current != null ? Source.Current[FieldName] : null;
                Control.SelectedItem = LookupSource.FindItem(Value);
            }
            finally
            {
                Updating = false;
            }
        }
        void Push()
        {
            if (Updating || Source.Current == null)
                return;

            if (Control.SelectedItem is LookupItem Item)
                Source.Current[FieldName] = Item.Value;
            else
                Source.Current[FieldName] = null;
        }
        void Source_PropertyChanged(object Sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentPath)
                Pull();
        }
        void Source_Changed(object Sender, DataSourceChangeEventArgs e)
        {
            if (ReferenceEquals(e.Row, Source.Current) && e.FieldName == FieldName)
                Pull();
        }
        void Control_SelectionChanged(object Sender, SelectionChangedEventArgs e)
        {
            Push();
        }
        void Control_KeyDown(object Sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
                return;

            Pull();
            e.Handled = true;
        }

        Source.PropertyChanged += Source_PropertyChanged;
        Source.Changed += Source_Changed;
        Control.SelectionChanged += Control_SelectionChanged;
        Control.KeyDown += Control_KeyDown;
        Pull();

        DataSourceBindingSubscription Subscription = new(() =>
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source.Changed -= Source_Changed;
            Control.SelectionChanged -= Control_SelectionChanged;
            Control.KeyDown -= Control_KeyDown;
        });

        DataSourceBinding Binding = new(Source, Control, FieldName, ComboBox.SelectedItemProperty, Subscription, Field)
        {
            LookupSource = LookupSource
        };

        return Binding;
    }
    static DataSourceBinding BindLocatorControl(DataSource Source, LocatorBox Control, FieldDef Field)
    {
        CheckArgs(Source, Control, GetFieldName(Field));
        if (string.IsNullOrWhiteSpace(Field.Locator))
            throw new TripousException($"Field '{Field.Name}' has no locator.");

        LocatorDef LocatorDef = DataRegistry.Locators[Field.Locator];
        if (LocatorDef == null)
            throw new TripousException($"LocatorDef not found. Locator: {Field.Locator}");

        ControlBindingHelper.EnsureLocatorFields(LocatorDef, Field);
        Locator Locator = TypeStore.CreateInstance<Locator>(LocatorDef.ClassName);
        Locator.Initialize(LocatorDef);

        Control.Locator = Locator;
        Control.IsReadOnly = Field.IsReadOnly || Field.IsReadOnlyUI || LocatorDef.IsReadOnly;

        DataSourceBinding Binding = new(Source, Control, Field.Name, LocatorBox.KeyValueProperty, null, Field)
        {
            LocatorDef = LocatorDef,
            Locator = Locator
        };

        void Pull()
        {
            Binding.IsRefreshing = true;
            try
            {
                if (Source.Current == null)
                {
                    Control.KeyValue = DBNull.Value;
                    Control.ClearTargetBoxes();
                    return;
                }

                object Value = Source.Current[Field.Name];
                Control.KeyValue = Sys.IsNull(Value) ? DBNull.Value : Value;

                DataRow TargetRow = Source.Current.InnerObject is DataRowView RowView ? RowView.Row : Source.Current.InnerObject as DataRow;
                if (TargetRow != null)
                    Control.RefreshTargetBoxes(TargetRow);
            }
            finally
            {
                Binding.IsRefreshing = false;
            }
        }
        void Source_PropertyChanged(object Sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentPath)
                Pull();
        }
        void Source_Changed(object Sender, DataSourceChangeEventArgs e)
        {
            if (ReferenceEquals(e.Row, Source.Current) && e.FieldName == Field.Name)
                Pull();
        }
        void Control_RowSelected(object Sender, LocatorBoxRowEventArgs e)
        {
            if (Binding.IsRefreshing || Source.Current == null)
                return;

            Binding.IsRefreshing = true;
            try
            {
                Locator.Assign(e.Row, Source.Current, Field.Name, Binding.LocatorTargetFieldMap);
                Source.Current.NotifyFieldChanged(Field.Name);
                Pull();
            }
            finally
            {
                Binding.IsRefreshing = false;
            }
        }

        Source.PropertyChanged += Source_PropertyChanged;
        Source.Changed += Source_Changed;
        Control.RowSelected += Control_RowSelected;
        Pull();

        Binding.DisposeAction = () =>
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source.Changed -= Source_Changed;
            Control.RowSelected -= Control_RowSelected;
        };

        return Binding;
    }
    static DataSourceBinding AssignFieldDef(DataSourceBinding Binding, FieldDef Field)
    {
        Binding.FieldDef = Field;
        ApplyFieldMetadata(Binding);
        return Binding;
    }
    static void ApplyFieldMetadata(DataSourceBinding Binding)
    {
        if (Binding?.FieldDef == null || Binding.Control == null)
            return;

        bool IsReadOnly = Binding.FieldDef.Flags.HasFlag(FieldFlags.ReadOnlyUI);

        if (Binding.Control is TextBox TextBox)
        {
            TextBox.IsReadOnly = IsReadOnly;
            int MaxLength = Binding.DataColumn != null ? Binding.DataColumn.MaxLength : -1;

            if (MaxLength > 0)
                TextBox.MaxLength = MaxLength;
        }
        else if (Binding.Control is CheckBox CheckBox)
            CheckBox.IsEnabled = !IsReadOnly;
        else if (Binding.Control is ToggleSwitch ToggleSwitch)
            ToggleSwitch.IsEnabled = !IsReadOnly;
        else if (Binding.Control is DatePicker DatePicker)
            DatePicker.IsEnabled = !IsReadOnly;
        else if (Binding.Control is CalendarDatePicker CalendarDatePicker)
            CalendarDatePicker.IsEnabled = !IsReadOnly;
        else if (Binding.Control is NumericUpDown NumericUpDown)
            NumericUpDown.IsEnabled = !IsReadOnly;
    }

    // ● public
    /// <summary>
    /// Completes DataSource binding and posts the initial current row refresh.
    /// </summary>
    static public void BindingComplete(this DataSource Source)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        Dispatcher.UIThread.Post(() =>
        {
            if (Source.HasRows && Source.Current == null)
                Source.MoveFirst();
            else
                Source.RefreshCurrent();

            if (Source.Current == null)
                return;

            foreach (string FieldName in Source.Provider.GetFieldNames())
                Source.Current.NotifyFieldChanged(FieldName);
        }, DispatcherPriority.Background);
    }
    /// <summary>
    /// Binds a TextBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, TextBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, TextBox.TextProperty, UpdateSourceTrigger.LostFocus);
    }
    /// <summary>
    /// Binds a TextBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, TextBox Control, FieldDef Field)
    {
        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a CheckBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, CheckBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, CheckBox.IsCheckedProperty);
    }
    /// <summary>
    /// Binds a CheckBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, CheckBox Control, FieldDef Field)
    {
        if (Field != null && Field.Flags.HasFlag(FieldFlags.Boolean))
            return AssignFieldDef(BindBooleanControl(Source, Control, GetFieldName(Field), CheckBox.IsCheckedProperty), Field);

        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a ToggleSwitch to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ToggleSwitch Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, ToggleSwitch.IsCheckedProperty);
    }
    /// <summary>
    /// Binds a ToggleSwitch to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ToggleSwitch Control, FieldDef Field)
    {
        if (Field != null && Field.Flags.HasFlag(FieldFlags.Boolean))
            return AssignFieldDef(BindBooleanControl(Source, Control, GetFieldName(Field), ToggleSwitch.IsCheckedProperty), Field);

        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a DatePicker to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, DatePicker Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, DatePicker.SelectedDateProperty, UpdateSourceTrigger.LostFocus);
    }
    /// <summary>
    /// Binds a DatePicker to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, DatePicker Control, FieldDef Field)
    {
        return AssignFieldDef(BindDateControl(Source, Control, GetFieldName(Field), DatePicker.SelectedDateProperty), Field);
    }
    /// <summary>
    /// Binds a NumericUpDown to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, NumericUpDown Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, NumericUpDown.ValueProperty, UpdateSourceTrigger.LostFocus);
    }
    /// <summary>
    /// Binds a NumericUpDown to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, NumericUpDown Control, FieldDef Field)
    {
        return AssignFieldDef(BindNumericControl(Source, Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a ComboBox selected value to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ComboBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, ComboBox.SelectedValueProperty);
    }
    /// <summary>
    /// Binds a ComboBox selected value to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ComboBox Control, FieldDef Field)
    {
        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a lookup ComboBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindLookup(this DataSource Source, ComboBox Control, FieldDef Field)
    {
        if (Field == null)
            throw new ArgumentNullException(nameof(Field));
        if (string.IsNullOrWhiteSpace(Field.LookupSource))
            throw new InvalidOperationException($"FieldDef '{Field.Name}' has no LookupSource.");

        DataSourceBinding Binding = BindLookupControl(Source, Control, GetFieldName(Field), Field.LookupSource, Field);
        ApplyFieldMetadata(Binding);
        return Binding;
    }
    /// <summary>
    /// Binds a lookup ComboBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindLookup(this DataSource Source, ComboBox Control, string FieldName, string LookupSourceName)
    {
        return BindLookupControl(Source, Control, FieldName, LookupSourceName);
    }
    /// <summary>
    /// Binds a LocatorBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, LocatorBox Control, FieldDef Field)
    {
        return BindLocatorControl(Source, Control, Field);
    }
    /// <summary>
    /// Binds a ComboBox to an enum field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ComboBox Control, string FieldName, Type EnumType)
    {
        if (EnumType == null || !EnumType.IsEnum)
            throw new ArgumentException("EnumType must be an enum type.", nameof(EnumType));

        Control.ItemsSource = Enum.GetValues(EnumType);
        return Source.Bind(Control, FieldName);
    }
    /// <summary>
    /// Binds a ComboBox to an enum field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ComboBox Control, FieldDef Field, Type EnumType)
    {
        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field), EnumType), Field);
    }
    /// <summary>
    /// Binds a ListBox selected item to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ListBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, ListBox.SelectedItemProperty);
    }
    /// <summary>
    /// Binds a ListBox selected item to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ListBox Control, FieldDef Field)
    {
        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a ListBox to an enum field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ListBox Control, string FieldName, Type EnumType)
    {
        if (EnumType == null || !EnumType.IsEnum)
            throw new ArgumentException("EnumType must be an enum type.", nameof(EnumType));

        Control.ItemsSource = Enum.GetValues(EnumType);
        return Source.Bind(Control, FieldName);
    }
    /// <summary>
    /// Binds a ListBox to an enum field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ListBox Control, FieldDef Field, Type EnumType)
    {
        return AssignFieldDef(Source.Bind(Control, GetFieldName(Field), EnumType), Field);
    }
    /// <summary>
    /// Binds a CalendarDatePicker to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, CalendarDatePicker Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, CalendarDatePicker.SelectedDateProperty, UpdateSourceTrigger.LostFocus);
    }
    /// <summary>
    /// Binds a CalendarDatePicker to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, CalendarDatePicker Control, FieldDef Field)
    {
        return AssignFieldDef(BindDateControl(Source, Control, GetFieldName(Field), CalendarDatePicker.SelectedDateProperty), Field);
    }
    
    /// <summary>
    /// Binds a multiline TextBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindMemo(this DataSource Source, TextBox Control, string FieldName)
    {
        Control.AcceptsReturn = true;
        Control.TextWrapping = TextWrapping.Wrap;
        return Source.Bind(Control, FieldName);
    }
    /// <summary>
    /// Binds a multiline TextBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindMemo(this DataSource Source, TextBox Control, FieldDef Field)
    {
        return AssignFieldDef(Source.BindMemo(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds a TextEditor to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindEditor(this DataSource Source, TextEditor Control, string FieldName)
    {
        CheckArgs(Source, Control, FieldName);
        bool Updating = false;

        void Pull()
        {
            Updating = true;
            Control.Text = Source.Current != null ? Convert.ToString(Source.Current[FieldName]) : string.Empty;
            Updating = false;
        }
        void Source_PropertyChanged(object Sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentPath)
                Pull();
        }
        void Source_Changed(object Sender, DataSourceChangeEventArgs e)
        {
            if (ReferenceEquals(e.Row, Source.Current) && e.FieldName == FieldName)
                Pull();
        }
        void Control_TextChanged(object Sender, EventArgs e)
        {
            if (!Updating && Source.Current != null)
                Source.Current[FieldName] = Control.Text;
        }

        Source.PropertyChanged += Source_PropertyChanged;
        Source.Changed += Source_Changed;
        Control.TextChanged += Control_TextChanged;
        Pull();

        DataSourceBindingSubscription Subscription = new(() =>
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source.Changed -= Source_Changed;
            Control.TextChanged -= Control_TextChanged;
        });
        return new DataSourceBinding(Source, Control, FieldName, null, Subscription);
    }
    /// <summary>
    /// Binds a TextEditor to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindEditor(this DataSource Source, TextEditor Control, FieldDef Field)
    {
        return AssignFieldDef(Source.BindEditor(Control, GetFieldName(Field)), Field);
    }
    /// <summary>
    /// Binds an Image source to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindImage(this DataSource Source, Image Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, Image.SourceProperty);
    }
    /// <summary>
    /// Binds an Image source to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindImage(this DataSource Source, Image Control, FieldDef Field)
    {
        return AssignFieldDef(Source.BindImage(Control, GetFieldName(Field)), Field);
    }
    
    /// <summary>
    /// Binds a DataGrid to a DataSource.
    /// </summary>
    static public List<DataSourceBinding> Bind(this DataSource Source, DataGrid Grid, bool CreateColumns = true)
    {
        CheckGridArgs(Source, Grid);
        List<DataSourceBinding> Result = new();

        Grid.DataContext = Source;
        Grid.AutoGenerateColumns = false;
        Grid.IsReadOnly = false;

        IDisposable ItemsSubscription = Grid.Bind(DataGrid.ItemsSourceProperty, new Binding(RowsPath));
        IDisposable CurrentSubscription = Grid.Bind(DataGrid.SelectedItemProperty, new Binding(CurrentPath)
        {
            Mode = BindingMode.TwoWay
        });

        Result.Add(new DataSourceBinding(Source, Grid, string.Empty, DataGrid.ItemsSourceProperty, ItemsSubscription));
        Result.Add(new DataSourceBinding(Source, Grid, string.Empty, DataGrid.SelectedItemProperty, CurrentSubscription));

        if (CreateColumns)
            Result.AddRange(Source.CreateGridColumns(Grid));

        return Result;
    }
    /// <summary>
    /// Creates DataGrid columns for DataSource fields.
    /// </summary>
    static public List<DataSourceBinding> CreateGridColumns(this DataSource Source, DataGrid Grid)
    {
        CheckGridArgs(Source, Grid);
        List<DataSourceBinding> Result = new();

        Grid.Columns.Clear();

        foreach (string FieldName in Source.Provider.GetFieldNames())
            Result.Add(Source.AddGridColumn(Grid, FieldName));

        return Result;
    }
    /// <summary>
    /// Adds a DataGrid column for a DataSource field.
    /// </summary>
    static public DataSourceBinding AddGridColumn(this DataSource Source, DataGrid Grid, string FieldName)
    {
        CheckGridArgs(Source, Grid);
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException(nameof(FieldName));

        DataGridBoundColumn Column;
        Type FieldType = Source.Provider.GetFieldType(FieldName);

        if (FieldType == typeof(bool))
            Column = new DataGridCheckBoxColumn();
        else
            Column = new DataGridTextColumn();

        Column.Header = FieldName;
        Column.IsReadOnly = false;
        Column.Binding = CreateRowBinding(FieldName);
        Grid.Columns.Add(Column);
        return new DataSourceBinding(Source, Column, FieldName, null);
    }
}
