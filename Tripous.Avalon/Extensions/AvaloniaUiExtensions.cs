using System.Data;
using Avalonia.Controls;
using Avalonia.Data;
using AvaloniaEdit;

namespace Tripous.Avalon;

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
}