using System;
using System.Data;
using System.Linq;

namespace Tripous.Avalon;

/// <summary>
/// Implements the IDataLink interface to provide a bridge between a DataSource and a ADO.NET DataTable.
/// </summary>
public class DataTableLink : IDataLink
{
    private DataTable fTable;

    /// <summary>
    /// Initializes a new instance of the DataTableLink class with a specified DataTable.
    /// </summary>
    /// <param name="table">The DataTable to link to.</param>
    public DataTableLink(DataTable table)
    {
        this.fTable = table ?? throw new ArgumentNullException(nameof(table));
    }

    // --- Schema ---
    
    /// <summary>
    /// Gets the type of items contained in the link, which is DataRow for this implementation.
    /// </summary>
    public Type GetItemType() => typeof(DataRow);

    /// <summary>
    /// Returns an array of column names from the underlying DataTable.
    /// </summary>
    public string[] GetPropertyNames()
    {
        return fTable.Columns.Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .ToArray();
    }

    // --- Data Access ---
    
    /// <summary>
    /// Returns the collection of rows from the DataTable.
    /// </summary>
    public System.Collections.IEnumerable GetRows() => fTable.Rows;

    /// <summary>
    /// Gets a value from a specific column of a DataRow, converting DBNull to null.
    /// </summary>
    public object GetValue(object innerItem, string propertyName)
    {
        if (innerItem is DataRow row)
        {
            object value = row[propertyName];
            return (value == DBNull.Value) ? null : value;
        }
        return null;
    }

    /// <summary>
    /// Sets a value in a specific column of a DataRow, converting null to DBNull.
    /// </summary>
    public void SetValue(object innerItem, string propertyName, object value)
    {
        if (innerItem is DataRow row)
        {
            row[propertyName] = value ?? DBNull.Value;
        }
    }

    // --- CRUD ---
    
    /// <summary>
    /// Creates a new DataRow based on the schema of the linked DataTable. 
    /// Note: The row is created but not yet added to the table.
    /// </summary>
    public object CreateNew()
    {
        return fTable.NewRow();
    }

    /// <summary>
    /// Formally adds a DataRow to the underlying DataTable's row collection.
    /// </summary>
    public void AddToSource(object innerItem)
    {
        if (innerItem is DataRow row && row.Table == null)
        {
            fTable.Rows.Add(row);
        }
    }

    /// <summary>
    /// Removes (marks for deletion) a DataRow from the underlying DataTable.
    /// </summary>
    public void RemoveFromSource(object innerItem)
    {
        if (innerItem is DataRow row)
        {
            row.Delete(); 
        }
    }

    // --- Capabilities ---
    
    /// <summary>
    /// Gets a value indicating whether the data source has a fixed size.
    /// </summary>
    public bool IsFixedSize => false;

    /// <summary>
    /// Gets a value indicating whether the data source is read-only.
    /// </summary>
    public bool IsReadOnly => false;
}