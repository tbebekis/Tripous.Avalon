using System.Data;
using System.Data.Common;
using Avalonia;
using Avalonia.Controls;
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
    /// <summary>
    /// Returns the index of Value in List, case insensitively, if exists, else -1.
    /// </summary>
    static int IndexOfText(this IList<string> List, string Value)
    {
        if (List != null)
        {
            for (int i = 0; i < List.Count; i++)
                if (string.Compare(List[i], Value, StringComparison.InvariantCultureIgnoreCase) == 0)  
                    return i;
        }
        return -1;
    }
    /// <summary>
    /// Returns trur if Value exists in List, case insensitively.
    /// </summary>
    static bool ContainsText(this IList<string> List, string Value)
    {
        return IndexOfText(List, Value) != -1;
    }
    /// <summary>
    /// Case insensitive string equality.
    /// <para>Returns true if 1. both are null, 2. both are empty string or 3. they are the same string </para>
    /// </summary>
    static bool IsSameText(this string A, string B)
    {
        //return (!string.IsNullOrWhiteSpace(A) && !string.IsNullOrWhiteSpace(B))&& (string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0);

        // Compare() returns true if 1. both are null, 2. both are empty string or 3. they are the same string
        return string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0;
    }
    
    // ● control text
    static public string GetText(this TextBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this TextEditor Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this AutoCompleteBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this ComboBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    
    // ● bind to DataSource
    /// <summary>
    /// Automatically generates grid columns based on the data source schema.
    /// </summary>
   static public void CreateGridColumns(this BindingSource bindingSource, DataGrid Grid)
    {
        Grid.Columns.Clear();
        Grid.AutoGenerateColumns = false;

        foreach (string PropName in bindingSource.DataSource.GetPropertyNames())
        {
            // Optional: We could check for BrowsableAttribute here 
            // if the Link doesn't already perform filtering.
            var GridCol = new DataGridTextColumn
            {
                Header = PropName,
                Binding = new Binding(string.Format("[{0}]", PropName))
            };
            
            //string S = GridCol.Binding.
            Grid.Columns.Add(GridCol);
        }
    }
    
    /// <summary>
    /// Binds a DataGrid to the data source.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, DataGrid Grid, bool CreateColumns = false)
    {
        if (Grid == null) return;
        Grid.DataContext = bindingSource;
        Grid.Bind(DataGrid.ItemsSourceProperty, new Binding("Rows") { Mode = BindingMode.OneWay });
        Grid.Bind(DataGrid.SelectedItemProperty, new Binding("Current") { Mode = BindingMode.TwoWay });
        
        if (CreateColumns) bindingSource.CreateGridColumns(Grid);
        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a TextBox to a specific property of the current record.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, TextBox Edt, string PropertyName)
    {
        if (Edt == null || string.IsNullOrEmpty(PropertyName)) return;
        Edt.DataContext = bindingSource;
        Edt.Bind(TextBox.TextProperty, new Binding(string.Format("Current[{0}]", PropertyName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
        });
        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a ComboBox as a lookup control.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, ComboBox Cbo, BindingSource LookupSource, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Cbo == null || LookupSource == null) return;

        // 1. Data Source
        Cbo.Bind(ComboBox.ItemsSourceProperty, new Binding("Rows") 
        { 
            Source = LookupSource, 
            Mode = BindingMode.OneWay 
        });

        // 2. Rendering: Use FuncDataTemplate to avoid "Cannot set both DisplayMemberBinding and ItemTemplate"
        // The row is the DataSourceRow contained in the LookupSource fRows
        Cbo.ItemTemplate = new FuncDataTemplate<BindingSourceRow>((row, namescope) => 
        {
            var Block = new TextBlock();
            Block.Bind(TextBlock.TextProperty, new Binding($"[{DisplayMember}]"));
            return Block;
        });

        // 3. From UI to DataSource
        Cbo.SelectionChanged += (s, e) =>
        {
            if (Cbo.SelectedItem is BindingSourceRow SelectedRow)
            {
                object NewValue = SelectedRow[ValueMember];
                object OldValue = bindingSource.GetValue(TargetPropertyName);

                if (!object.Equals(OldValue, NewValue))
                {
                    bindingSource.SetValue(TargetPropertyName, NewValue);
                }
            }
        };

        // 4. From DataSource to UI
        bindingSource.OnCurrentPositionChanged += (s, e) =>
        {
            object CurrentValue = bindingSource.GetValue(TargetPropertyName);
        
            if (CurrentValue == null)
            {
                Cbo.SelectedItem = null;
            }
            else
            {
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => 
                    object.Equals(r[ValueMember], CurrentValue));
            
                if (Cbo.SelectedItem != FoundRow)
                {
                    Cbo.SelectedItem = FoundRow;
                }
            }
        };
        
        // 5. Keyboard Handling: F4 for open, Enter for Tab behavior
        Cbo.KeyDown += (s, e) =>
        {
            if (e.Key == Key.F4)
            {
                Cbo.IsDropDownOpen = !Cbo.IsDropDownOpen;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                GetNextControl(Cbo)?.Focus();
                e.Handled = true;
            }
        };

        // ΠΡΟΣΘΗΚΗ: Listener για αλλαγή τιμής στο TargetPropertyName
        bindingSource.OnChanged += (s, e) =>
        {
            if (e.PropertyName == TargetPropertyName)
            {
                // Επαναλαμβάνουμε τη λογική εύρεσης της σωστής γραμμής στο Lookup
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => 
                    object.Equals(r[ValueMember], e.NewValue));
            
                if (Cbo.SelectedItem != FoundRow)
                    Cbo.SelectedItem = FoundRow;
            }
        };
        
        // 6. Text search only (Select-only)
        Cbo.IsEditable = false; 
        Cbo.IsTextSearchEnabled = true;

        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a ListBox to a specific property of the current record.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, ListBox Lst, BindingSource LookupSource, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        if (Lst == null || LookupSource == null) return;

        Lst.Bind(ListBox.ItemsSourceProperty, new Binding("Rows") { Source = LookupSource });
    
        Lst.ItemTemplate = new FuncDataTemplate<BindingSourceRow>((row, ns) => 
        {
            var tb = new TextBlock();
            tb.Bind(TextBlock.TextProperty, new Binding($"[{DisplayMember}]"));
            return tb;
        });

        // SelectedValue synchronization (using SelectionChanged like in ComboBox)
        Lst.SelectionChanged += (s, e) =>
        {
            if (Lst.SelectedItem is BindingSourceRow row)
                bindingSource.SetValue(TargetPropertyName, row[ValueMember]);
        };

        bindingSource.OnCurrentPositionChanged += (s, e) =>
        {
            object val = bindingSource.GetValue(TargetPropertyName);
            Lst.SelectedItem = LookupSource.Rows.FirstOrDefault(r => object.Equals(r[ValueMember], val));
        };
        
        // 3. ΠΡΟΣΘΗΚΗ: Από τον DataSource προς το UI (Αλλαγή τιμής στο ίδιο Row)
        bindingSource.OnChanged += (s, e) =>
        {
            if (e.PropertyName == TargetPropertyName)
            {
                var FoundRow = LookupSource.Rows.FirstOrDefault(r => object.Equals(r[ValueMember], e.NewValue));
                if (Lst.SelectedItem != FoundRow)
                    Lst.SelectedItem = FoundRow;
            }
        };
    
        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a CheckBox to a boolean property of the current record.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, CheckBox Chk, string PropertyName)
    {
        if (Chk == null || string.IsNullOrEmpty(PropertyName)) return;
        Chk.DataContext = bindingSource;
        Chk.Bind(CheckBox.IsCheckedProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a ToggleSwitch to a boolean property of the current record.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, ToggleSwitch Sw, string PropertyName)
    {
        if (Sw == null) return;
        Sw.DataContext = bindingSource;
        Sw.Bind(ToggleSwitch.IsCheckedProperty, new Binding($"Current[{PropertyName}]") { Mode = BindingMode.TwoWay });
        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a DatePicker to a date property of the current record.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, DatePicker Dt, string PropertyName)
    {
        if (Dt == null || string.IsNullOrEmpty(PropertyName)) return;
        Dt.DataContext = bindingSource;
        Dt.Bind(DatePicker.SelectedDateProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        bindingSource.ForceMoveToCurrent();
    }

    /// <summary>
    /// Binds a NumericUpDown to a numeric property of the current record.
    /// </summary>
    static public void Bind(this BindingSource bindingSource, NumericUpDown Num, string PropertyName)
    {
        if (Num == null || string.IsNullOrEmpty(PropertyName)) return;
        Num.DataContext = bindingSource;
        Num.Bind(NumericUpDown.ValueProperty, new Binding($"Current[{PropertyName}]")
        {
            Mode = BindingMode.TwoWay
        });
        bindingSource.ForceMoveToCurrent();
    }
    
    // ● DataGrid
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