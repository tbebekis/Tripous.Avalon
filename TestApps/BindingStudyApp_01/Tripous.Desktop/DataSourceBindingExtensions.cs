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
    /// Binds a CheckBox to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, CheckBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, CheckBox.IsCheckedProperty);
    }
    /// <summary>
    /// Binds a ToggleSwitch to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ToggleSwitch Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, ToggleSwitch.IsCheckedProperty);
    }
    /// <summary>
    /// Binds a DatePicker to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, DatePicker Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, DatePicker.SelectedDateProperty, UpdateSourceTrigger.LostFocus);
    }
    /// <summary>
    /// Binds a NumericUpDown to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, NumericUpDown Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, NumericUpDown.ValueProperty, UpdateSourceTrigger.LostFocus);
    }
    /// <summary>
    /// Binds a ComboBox selected value to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ComboBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, ComboBox.SelectedValueProperty);
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
    /// Binds a ListBox selected item to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, ListBox Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, ListBox.SelectedItemProperty);
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
    /// Binds a CalendarDatePicker to a DataSource field.
    /// </summary>
    static public DataSourceBinding Bind(this DataSource Source, CalendarDatePicker Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, CalendarDatePicker.SelectedDateProperty, UpdateSourceTrigger.LostFocus);
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
    /// Binds an Image source to a DataSource field.
    /// </summary>
    static public DataSourceBinding BindImage(this DataSource Source, Image Control, string FieldName)
    {
        return BindControl(Source, Control, FieldName, Image.SourceProperty);
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
