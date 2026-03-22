using System.Data;
using System.Data.Common;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridInteractions;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;
using AvaloniaEdit;
using Tripous.Avalon;
using Tripous.Avalon.Data;

namespace Tripous.Avalon.Controls;

static public class AvaloniaUiExtensions
{
 
    
    // ● control text
    static public string GetText(this TextBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this TextEditor Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this AutoCompleteBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this ComboBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    
    // ● bind  

    /// <summary>
    /// Binds a DataGrid to the data source.
    /// </summary>
    static public void Bind(this BindingSource BS, DataGrid Grid, bool CreateColumns = false)
    {
        if (Grid == null) return;
        Grid.UnBind(CreateColumns);
        Grid.DataContext = BS;
        Grid.Bind(DataGrid.ItemsSourceProperty, new Binding("Rows") { Mode = BindingMode.OneWay });
        Grid.Bind(DataGrid.SelectedItemProperty, new Binding("Current") { Mode = BindingMode.TwoWay });
        
        if (CreateColumns) BS.CreateGridColumns(Grid);
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Unbinds a grid
    /// </summary>
    static public void UnBind(this DataGrid Grid, bool RemoveColumns = false)
    {
        if (Grid == null) return;
 
        Grid.ClearValue(DataGrid.ItemsSourceProperty);
        Grid.ClearValue(DataGrid.SelectedItemProperty);
 
        Grid.DataContext = null;
 
        if (RemoveColumns)
            Grid.Columns.Clear();
    }
    /// <summary>
    /// Binds a TextBox to a specific property of the current record.
    /// </summary>
    static public void Bind(this BindingSource BS, TextBox Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrEmpty(PropertyName)) return;
        Box.DataContext = BS;
        Box.Bind(TextBox.TextProperty, new Binding(string.Format("Current[{0}]", PropertyName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
        });
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a combo box as a normal control, not a lookup combo box
    /// </summary>
    static public void Bind(this BindingSource BS, ComboBox Box, string PropertyName, object[] ItemsSource = null)
    {
        if (Box == null || string.IsNullOrEmpty(PropertyName)) return;
        Box.DataContext = BS;
        if (ItemsSource != null)
            Box.ItemsSource = ItemsSource;
        Box.Bind(TextBox.TextProperty, new Binding(string.Format("Current[{0}]", PropertyName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
        });
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a ComboBox as a lookup control.
    /// </summary>
    static public void Bind(this BindingSource BS, ComboBox Box, BindingSource LookupSource, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Box == null || LookupSource == null) return;

        // 1. Data Source
        Box.Bind(ComboBox.ItemsSourceProperty, new Binding("Rows") 
        { 
            Source = LookupSource, 
            Mode = BindingMode.OneWay 
        });

        // 2. Rendering: Use FuncDataTemplate to avoid "Cannot set both DisplayMemberBinding and ItemTemplate"
        // The row is the DataSourceRow contained in the LookupSource fRows
        Box.ItemTemplate = new FuncDataTemplate<BindingSourceRow>((row, namescope) => 
        {
            var Block = new TextBlock();
            Block.Bind(TextBlock.TextProperty, new Binding($"[{DisplayMember}]"));
            return Block;
        });

        // 3. From UI to DataSource
        Box.SelectionChanged += (s, e) =>
        {
            if (Box.SelectedItem is BindingSourceRow SelectedRow)
            {
                object NewValue = SelectedRow[ValueMember];
                object OldValue = BS.GetValue(TargetPropertyName);

                if (!object.Equals(OldValue, NewValue))
                {
                    BS.SetValue(TargetPropertyName, NewValue);
                }
            }
        };

        // 4. From DataSource to UI
        BS.OnCurrentPositionChanged += (s, e) =>
        {
            object CurrentValue = BS.GetValue(TargetPropertyName);
        
            if (CurrentValue == null)
            {
                Box.SelectedItem = null;
            }
            else
            {
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => 
                    object.Equals(r[ValueMember], CurrentValue));
            
                if (Box.SelectedItem != FoundRow)
                {
                    Box.SelectedItem = FoundRow;
                }
            }
        };
        
        // 5. Keyboard Handling: F4 for open, Enter for Tab behavior
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

        // ΠΡΟΣΘΗΚΗ: Listener για αλλαγή τιμής στο TargetPropertyName
        BS.OnChanged += (s, e) =>
        {
            if (e.PropertyName == TargetPropertyName)
            {
                // Επαναλαμβάνουμε τη λογική εύρεσης της σωστής γραμμής στο Lookup
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => 
                    object.Equals(r[ValueMember], e.NewValue));
            
                if (Box.SelectedItem != FoundRow)
                    Box.SelectedItem = FoundRow;
            }
        };
        
        // 6. Text search only (Select-only)
        Box.IsEditable = false; 
        Box.IsTextSearchEnabled = true;

        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a ListBox to a specific property of the current record.
    /// </summary>
    static public void Bind(this BindingSource BS, ListBox Box, BindingSource LookupSource, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Box == null || LookupSource == null) return;

        Box.Bind(ListBox.ItemsSourceProperty, new Binding("Rows") { Source = LookupSource });
    
        Box.ItemTemplate = new FuncDataTemplate<BindingSourceRow>((row, ns) => 
        {
            var tb = new TextBlock();
            tb.Bind(TextBlock.TextProperty, new Binding($"[{DisplayMember}]"));
            return tb;
        });

        // SelectedValue synchronization (using SelectionChanged like in ComboBox)
        Box.SelectionChanged += (s, e) =>
        {
            if (Box.SelectedItem is BindingSourceRow row)
                BS.SetValue(TargetPropertyName, row[ValueMember]);
        };

        BS.OnCurrentPositionChanged += (s, e) =>
        {
            object val = BS.GetValue(TargetPropertyName);
            Box.SelectedItem = LookupSource.Rows.FirstOrDefault(r => object.Equals(r[ValueMember], val));
        };
        
        // 3. ΠΡΟΣΘΗΚΗ: Από τον DataSource προς το UI (Αλλαγή τιμής στο ίδιο Row)
        BS.OnChanged += (s, e) =>
        {
            if (e.PropertyName == TargetPropertyName)
            {
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => object.Equals(r[ValueMember], e.NewValue));
                if (Box.SelectedItem != FoundRow)
                    Box.SelectedItem = FoundRow;
            }
        };
    
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a CheckBox to a boolean property of the current record.
    /// </summary>
    static public void Bind(this BindingSource BS, CheckBox Box, string PropertyName)
    {
        if (Box == null || string.IsNullOrEmpty(PropertyName)) return;
        Box.DataContext = BS;
        Box.Bind(CheckBox.IsCheckedProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a ToggleSwitch to a boolean property of the current record.
    /// </summary>
    static public void Bind(this BindingSource BS, ToggleSwitch ToggleSwitch, string PropertyName)
    {
        if (ToggleSwitch == null) return;
        ToggleSwitch.DataContext = BS;
        ToggleSwitch.Bind(ToggleSwitch.IsCheckedProperty, new Binding($"Current[{PropertyName}]") { Mode = BindingMode.TwoWay });
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a DatePicker to a date property of the current record.
    /// </summary>
    static public void Bind(this BindingSource BS, DatePicker DatePicker, string PropertyName)
    {
        if (DatePicker == null || string.IsNullOrEmpty(PropertyName)) return;
        DatePicker.DataContext = BS;
        DatePicker.Bind(DatePicker.SelectedDateProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        BS.ForceMoveToCurrent();
    }
    /// <summary>
    /// Binds a NumericUpDown to a numeric property of the current record.
    /// </summary>
    static public void Bind(this BindingSource BS, NumericUpDown NumericUpDown, string PropertyName)
    {
        if (NumericUpDown == null || string.IsNullOrEmpty(PropertyName)) return;
        NumericUpDown.DataContext = BS;
        NumericUpDown.Bind(NumericUpDown.ValueProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        BS.ForceMoveToCurrent();
    }
    
    // ● DataGrid
    /// <summary>
    /// Automatically generates grid columns based on the data source schema.
    /// </summary>
    static public void CreateGridColumns(this BindingSource BS, DataGrid Grid)
    {
        Grid.Columns.Clear();
        Grid.AutoGenerateColumns = false;

        string[] PropertyNames = BS.DataSource.GetPropertyNames();
        Type[] PropertyTypes = BS.DataSource.GetPropertyTypes();

        string PropertyName;
        Type PropertyType;
        DataGridBoundColumn GridColumn;
        for (int i = 0; i < PropertyNames.Length; i++)
        {
            PropertyName = PropertyNames[i];
            PropertyType = PropertyTypes[i];

            GridColumn = PropertyType == typeof(bool) ? new DataGridCheckBoxColumn() : new DataGridTextColumn();
            GridColumn.Header = PropertyName;
            GridColumn.Binding = new Binding(string.Format("[{0}]", PropertyName));
 
            Grid.Columns.Add(GridColumn);
        }
    }
    /// <summary>
    /// Returns the name of a Property/FieldName a column is bound to.
    /// </summary>
    static public string GetPropertyName(this DataGridBoundColumn column)
    {
        if (column == null) return null;

        // 1. Πρώτη προτεραιότητα στο Tag (το ελέγχουμε εμείς)
        if (column.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
            return tag;

        // 2. Δεύτερη προτεραιότητα στην ανάλυση του Binding
        if (column.Binding is Binding b && !string.IsNullOrWhiteSpace(b.Path))
        {
            // Καθαρίζουμε brackets και παίρνουμε το τελευταίο μέρος του path
            string path = b.Path.Replace("[", "").Replace("]", "");
        
            if (path.Contains("."))
            {
                return path.Split('.').Last();
            }
        
            return path;
        }

        return null;
    }
    /// <summary>
    /// Sets all columns to be editable (i.e. not read-only)
    /// </summary>
    static public void SetAllColumnsEditable(this DataGrid Grid)
    {
        foreach (DataGridBoundColumn Column in Grid.Columns)
            Column.IsReadOnly = false;    
    }
    /// <summary>
    /// Columns found in <see cref="PropertyNames"/> are set to be editable. All other columns are set to read-only.
    /// </summary>
    static public void SetEditableColumns(this DataGrid Grid, List<string> PropertyNames)
    {
        foreach (DataGridBoundColumn Column in Grid.Columns)
            Column.IsReadOnly = !PropertyNames.ContainsText(Column.GetPropertyName());
    }
    /// <summary>
    /// Columns found in <see cref="PropertyNames"/> are set to be visible. All other columns are hidden.
    /// </summary>
    static public void SetVisibleColumns(this DataGrid Grid, List<string> PropertyNames)
    {
        foreach (DataGridBoundColumn Column in Grid.Columns)
            Column.IsVisible = PropertyNames.ContainsText(Column.GetPropertyName());
    }
    /// <summary>
    /// Sets a column editable (non read-only) by name.
    /// <para>NOTE: <see cref="PropertyName"/> is the property given as the <see cref="Binding.Path"/>.</para>
    /// </summary>
    static public void SetColumnEditable(this DataGrid Grid, string PropertyName, bool Value)
    {
        DataGridBoundColumn Column = Grid.FindColumn(PropertyName);
        if (Column != null)
            Column.IsReadOnly = !Value;
    }
    /// <summary>
    /// Sets a column visible by name.
    /// <para>NOTE: <see cref="PropertyName"/> is the property given as the <see cref="Binding.Path"/>.</para>
    /// </summary>
    static public void SetColumnVisible(this DataGrid Grid, string PropertyName, bool Value)
    {
        DataGridBoundColumn Column = Grid.FindColumn(PropertyName);
        if (Column != null)
            Column.IsVisible = Value;
    }
    /// <summary>
    /// Finds and returns a column by name (i.e. Header), if any, else null.
    /// <para>NOTE: <see cref="PropertyName"/> is the Header of the column.</para>
    /// </summary>
    static public DataGridBoundColumn FindColumn(this DataGrid Grid, string PropertyName)
    {
        foreach (DataGridBoundColumn Column in Grid.Columns)
            if (PropertyName.IsSameText(Column.GetPropertyName()))
                return Column;
        return null;
    }
    /// <summary>
    /// Shows/Hides data bound grid columns according to a specified flag.
    /// </summary>
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
    
    
    /// <summary>
    /// Finds and returns the next focusable control in the visual tree.
    /// </summary>
    static public IInputElement GetNextControl(Visual current)
    {
        // 1. Find the TopLevel (Window) using VisualTreeHelper
        var topLevel = current.GetVisualRoot() as TopLevel;
        if (topLevel == null) return null;
        
        // 2. KeyboardNavigationHandler.GetNext expects IInputElement. 
        // Almost all Controls in Avalonia implement this interface.
        if (current is IInputElement element)
        {
            return KeyboardNavigationHandler.GetNext(element, NavigationDirection.Next);
        }
        
        return null;
    }
}