using System;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;
using Tripous.Data;
 

namespace Tripous.Avalon;

/// <summary>
/// Handles a top data table, the one with the single row, i.e. a tblCustomer or tblProduct with just one <see cref="DataRow"/>.
/// </summary>
public class ItemBinder : ObservableObject
{
    // ● private
    private void RefreshCurrentRow()
    {
        DataRow newRow = GetCurrentRow();

        if (!ReferenceEquals(CurrentRow, newRow))
        {
            CurrentRow = newRow;
            OnPropertyChanged(string.Empty);   // refresh all bindings
            OnPropertyChanged(nameof(HasRow));
        }
    }
    private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
    {
        if (e.Action == DataRowAction.Add || e.Action == DataRowAction.Change)
            RefreshCurrentRow();
    }
    private void Table_RowDeleted(object sender, DataRowChangeEventArgs e)
    {
        RefreshCurrentRow();
    }
    private void Table_TableCleared(object sender, DataTableClearEventArgs e)
    {
        RefreshCurrentRow();
    }

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public ItemBinder(MemTable Table)
    {
        this.Table = Table ?? throw new ArgumentNullException(nameof(Table));
        CurrentRow = GetCurrentRow();

        this.Table.RowChanged += Table_RowChanged;
        this.Table.RowDeleted += Table_RowDeleted;
        this.Table.TableCleared += Table_TableCleared;
    }

    // ● row tracking
    /// <summary>
    /// Returns the current row.
    /// </summary>
    public DataRow GetCurrentRow() => Table.Rows.Count > 0 ? Table.Rows[0] : null;


    // ● bind - simple controls
    /// <summary>
    /// Binds a TextBox to a specific property of the current record.
    /// </summary>
    public void Bind(TextBox Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(TextBox.TextProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
    }
    /// <summary>
    /// Binds a normal ComboBox to the current record.
    /// </summary>
    public void Bind(ComboBox Box, string PropertyName, object[] ItemsSource = null)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;

        if (ItemsSource != null)
            Box.ItemsSource = ItemsSource;

        Box.Bind(SelectingItemsControl.SelectedItemProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
    }
    /// <summary>
    /// Binds a CheckBox to a boolean property of the current record.
    /// </summary>
    public void Bind(CheckBox Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(ToggleButton.IsCheckedProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
    }
    /// <summary>
    /// Binds a ToggleSwitch to a boolean property of the current record.
    /// </summary>
    public void Bind(ToggleSwitch Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(ToggleButton.IsCheckedProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
    }
    /// <summary>
    /// Binds a DatePicker to a date property of the current record.
    /// </summary>
    public void Bind(DatePicker Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(DatePicker.SelectedDateProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
    }
    /// <summary>
    /// Binds a NumericUpDown to a numeric property of the current record.
    /// </summary>
    public void Bind(NumericUpDown Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(NumericUpDown.ValueProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
    }
    /// <summary>
    /// Binds a MaskedTextBox to a string property of the current record.
    /// </summary>
    public void Bind(MaskedTextBox Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(TextBox.TextProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
    }
    /// <summary>
    /// Binds an AutoCompleteBox to a string property of the current record.
    /// </summary>
    public void Bind(AutoCompleteBox Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(AutoCompleteBox.TextProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
    }
    /// <summary>
    /// Binds a RadioButton to a boolean property of the current record.
    /// </summary>
    public void Bind(RadioButton Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrWhiteSpace(PropertyName))
            return;

        Box.DataContext = this;
        Box.Bind(ToggleButton.IsCheckedProperty, new Binding($"[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
    }

    // ● bind - lookup controls
    /// <summary>
    /// Binds a ListBox as a lookup control.
    /// </summary>
    public void Bind(ListBox Box, MemTable LookupTable, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        BindLookupCore(Box, LookupTable, DisplayMember, ValueMember, TargetPropertyName);
    }
    /// <summary>
    /// Binds a ComboBox as a lookup control.
    /// </summary>
    public void Bind(ComboBox Box, MemTable LookupTable, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        BindLookupCore(Box, LookupTable, DisplayMember, ValueMember, TargetPropertyName);

        Box.IsEditable = false;
        Box.IsTextSearchEnabled = true;

        Box.KeyDown += (s, e) =>
        {
            if (e.Key == Key.F4)
            {
                Box.IsDropDownOpen = !Box.IsDropDownOpen;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                GetNextControl(Box)?.Focus();
                e.Handled = true;
            }
        };
    }
    private void BindLookupCore(SelectingItemsControl Box, MemTable LookupTable, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Box == null || LookupTable == null)
            return;

        if (string.IsNullOrWhiteSpace(DisplayMember) ||
            string.IsNullOrWhiteSpace(ValueMember) ||
            string.IsNullOrWhiteSpace(TargetPropertyName))
            return;

        Box.ItemsSource = LookupTable.Rows;

        if (Box is HeaderedSelectingItemsControl or ComboBox or ListBox)
        {
            Box.ItemTemplate = new FuncDataTemplate<DataRow>((row, ns) =>
            {
                var block = new TextBlock();

                string text = string.Empty;
                if (row != null && LookupTable.Columns.Contains(DisplayMember))
                {
                    object v = row[DisplayMember];
                    text = v == null || v == DBNull.Value ? string.Empty : Convert.ToString(v);
                }

                block.Text = text;
                return block;
            });
        }

        void SyncSelection()
        {
            object currentValue = this[TargetPropertyName];

            if (currentValue == null || currentValue == DBNull.Value)
            {
                if (Box.SelectedItem != null)
                    Box.SelectedItem = null;

                return;
            }

            DataRow foundRow = null;

            foreach (DataRow row in LookupTable.Rows)
            {
                object rowValue = LookupTable.Columns.Contains(ValueMember) ? row[ValueMember] : null;
                if (Equals(rowValue, currentValue))
                {
                    foundRow = row;
                    break;
                }
            }

            if (!ReferenceEquals(Box.SelectedItem, foundRow))
                Box.SelectedItem = foundRow;
        }

        Box.SelectionChanged += (s, e) =>
        {
            if (Box.SelectedItem is DataRow selectedRow)
            {
                object newValue = LookupTable.Columns.Contains(ValueMember)
                    ? selectedRow[ValueMember]
                    : null;

                object oldValue = this[TargetPropertyName];

                if (!Equals(oldValue, newValue))
                    this[TargetPropertyName] = newValue;
            }
            else
            {
                if (!IsNull(TargetPropertyName))
                    SetToNull(TargetPropertyName);
            }
        };

        PropertyChanged += (s, e) =>
        {
            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == TargetPropertyName ||
                e.PropertyName == nameof(HasRow))
            {
                SyncSelection();
            }
        };

        LookupTable.RowChanged += (s, e) => SyncSelection();
        LookupTable.RowDeleted += (s, e) => SyncSelection();
        LookupTable.TableCleared += (s, e) => SyncSelection();

        if (Box is InputElement inputElement)
        {
            inputElement.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                    e.Handled = true;
            };
        }

        SyncSelection();
    }

    // ● value
    /// <summary>
    /// Gets a property value cast to the specified type.
    /// </summary>
    public T GetValue<T>(string PropertyName)
    {
        object value = this[PropertyName];
        if (value == null || value == DBNull.Value)
            return default(T);

        return (T)value;
    }
    /// <summary>
    /// Sets a property value.
    /// </summary>
    public void SetValue<T>(string PropertyName, T value)
    {
        this[PropertyName] = value;
    }
    /// <summary>
    /// Checks if a property value is null or DBNull.
    /// </summary>
    public bool IsNull(string PropertyName)
    {
        object value = this[PropertyName];
        return value == null || value == DBNull.Value;
    }
    /// <summary>
    /// Sets a property value to DBNull.
    /// </summary>
    public void SetToNull(string PropertyName)
    {
        this[PropertyName] = DBNull.Value;
    }

    // ● public accessors
    /// <summary>
    /// Accesses the property as a generic object.
    /// </summary>
    public object AsObject(string PropertyName) => this[PropertyName];
    /// <summary>
    /// Sets the property value as a generic object.
    /// </summary>
    public void AsObject(string PropertyName, object value) => this[PropertyName] = value;
    /// <summary>
    /// Accesses the property value as a string.
    /// </summary>
    public string AsString(string PropertyName) => Convert.ToString(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a string.
    /// </summary>
    public void AsString(string PropertyName, string value) => this[PropertyName] = value;
    /// <summary>
    /// Accesses the property value as an integer.
    /// </summary>
    public int AsInteger(string PropertyName) => Convert.ToInt32(this[PropertyName]);
    /// <summary>
    /// Sets the property value as an integer.
    /// </summary>
    public void AsInteger(string PropertyName, int value) => this[PropertyName] = value;
    /// <summary>
    /// Accesses the property value as a double.
    /// </summary>
    public double AsDouble(string PropertyName) => Convert.ToDouble(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a double.
    /// </summary>
    public void AsDouble(string PropertyName, double value) => this[PropertyName] = value;
    /// <summary>
    /// Accesses the property value as a decimal.
    /// </summary>
    public decimal AsDecimal(string PropertyName) => Convert.ToDecimal(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a decimal.
    /// </summary>
    public void AsDecimal(string PropertyName, decimal value) => this[PropertyName] = value;
    /// <summary>
    /// Accesses the property value as a boolean.
    /// </summary>
    public bool AsBoolean(string PropertyName) => Convert.ToBoolean(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a boolean.
    /// </summary>
    public void AsBoolean(string PropertyName, bool value) => this[PropertyName] = value;
    /// <summary>
    /// Accesses the property value as a DateTime.
    /// </summary>
    public DateTime AsDateTime(string PropertyName) => Convert.ToDateTime(this[PropertyName]);
    /// <summary>
    /// Sets the property value as a DateTime.
    /// </summary>
    public void AsDateTime(string PropertyName, DateTime value) => this[PropertyName] = value;

    /// <summary>
    /// Finds and returns the next focusable control in the visual tree.
    /// </summary>
    public static IInputElement GetNextControl(Visual current)
    {
        var topLevel = current.GetVisualRoot() as TopLevel;
        if (topLevel == null)
            return null;

        if (current is IInputElement element)
            return KeyboardNavigationHandler.GetNext(element, NavigationDirection.Next);

        return null;
    }

    // ● properties
    public MemTable Table { get; protected set; }
    public DataRow CurrentRow { get; protected set; }
    public bool HasRow => CurrentRow != null;

    /// <summary>
    /// Indexer to get or set values by property name.
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    public object this[string PropertyName]
    {
        get
        {
            if (CurrentRow == null)
                return null;

            return Table.Columns.Contains(PropertyName) ? CurrentRow[PropertyName] : null;
        }
        set
        {
            if (CurrentRow == null)
                return;

            if (!Table.Columns.Contains(PropertyName))
                return;

            object oldValue = this[PropertyName];
            object newValue = value ?? DBNull.Value;

            if (Equals(oldValue, newValue))
                return;

            CurrentRow[PropertyName] = newValue;
            OnPropertyChanged(PropertyName);
        }
    }
}