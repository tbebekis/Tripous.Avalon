using System.Data;
using Avalonia.Controls;
using Avalonia.Data;

namespace Tripous.Avalon;

static public class TripousAvalonExtensions
{
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
    /// Returns a binding path suitable in binding to a ProDataGrid column.
    /// </summary>
    static public string GetBindingPath(string PropertyName)
    {
        if (string.IsNullOrEmpty(PropertyName))
            return PropertyName;

        if (PropertyName.Length > 1 && PropertyName[0] == '[' && PropertyName[PropertyName.Length - 1] == ']')
            return PropertyName;

        bool RequiresIndexer = false;

        foreach (char ch in PropertyName)
        {
            if (!char.IsLetterOrDigit(ch) && ch != '_')
            {
                RequiresIndexer = true;
                break;
            }
        }

        return RequiresIndexer ? $"[{PropertyName}]" : PropertyName;
    }
    /// <summary>
    /// Creates columns for a ProDataGrid based on a specified <see cref="DataTable"/>
    /// </summary>
    static public void CreateColumns(this DataGrid Grid, DataTable Table)
    {
        Grid.Columns.Clear();

        foreach (DataColumn Col in Table.Columns)
        {
            string BindingPath = GetBindingPath(Col.ColumnName);
            Type DataType = Col.DataType;
            Type CoreType = Nullable.GetUnderlyingType(DataType) ?? DataType;

            DataGridColumn column;

            /* Booleans -> CheckBox column */
            if (CoreType == typeof(bool))
            {
                column = new DataGridCheckBoxColumn
                {
                    Binding = new Binding(BindingPath),
                    IsThreeState = Col.AllowDBNull
                };
            }
            /* Enums -> ComboBox column */
            else if (CoreType.IsEnum)
            {
                column = new DataGridComboBoxColumn
                {
                    ItemsSource = Enum.GetValues(CoreType),
                    SelectedItemBinding = new Binding(BindingPath)
                };
            }
            /* Uri -> Hyperlink column */
            else if (CoreType == typeof(Uri))
            {
                column = new DataGridHyperlinkColumn
                {
                    Binding = new Binding(BindingPath),
                    ContentBinding = new Binding(BindingPath)
                };
            }
            /* Fallback -> text column */
            else
            {
                column = new DataGridTextColumn
                {
                    Binding = new Binding(BindingPath)
                };
            }

            column.Header = Col.ColumnName;
            column.IsReadOnly = Col.ReadOnly;

            Grid.Columns.Add(column);
        }
    }
    
}