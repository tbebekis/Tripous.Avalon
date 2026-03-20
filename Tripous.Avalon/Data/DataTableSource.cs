using System;
using System.Data;
using System.Linq;

namespace Tripous.Avalon.Data;

/// <summary>
/// Implements the IDataLink interface to provide a bridge between a DataSource and a ADO.NET DataTable.
/// </summary>
public class DataTableSource : IDataSource
{
    private string[] fChildPropertyNames;
    private DataTable fTable;

    // ● construction 
    /// <summary>
    /// Initializes a new instance of the DataTableLink class with a specified DataTable.
    /// </summary>
    /// <param name="table">The DataTable to link to.</param>
    public DataTableSource(DataTable table)
    {
        this.fTable = table ?? throw new ArgumentNullException(nameof(table));
    }

    // ● Schema & Metadata
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

    // ● Data Operations
    /// <summary>
    /// Returns the collection of rows from the DataTable.
    /// </summary>
    public System.Collections.IEnumerable GetRows() => fTable.Rows;
    /// <summary>
    /// Gets a value from a specific column of a DataRow, converting DBNull to null.
    /// </summary>
    public object GetValue(object InnerObject, string propertyName)
    {
        if (InnerObject is DataRow row)
        {
            object value = row[propertyName];
            return (value == DBNull.Value) ? null : value;
        }
        return null;
    }
    /// <summary>
    /// Sets a value in a specific column of a DataRow, converting null to DBNull.
    /// </summary>
    public void SetValue(object InnerObject, string propertyName, object value)
    {
        if (InnerObject is DataRow row)
        {
            row[propertyName] = value ?? DBNull.Value;
        }
    }

    // ● master-detail and filter
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    public bool PassesDetailCondition(object InnerObject, object[] MasterValues)
    {
        if (fChildPropertyNames == null || MasterValues == null || fChildPropertyNames.Length != MasterValues.Length)
            return true;
        
        if (InnerObject is DataRow row)
        {
            for (int i = 0; i < fChildPropertyNames.Length; i++)
            {
                object Value = row[fChildPropertyNames[i]];
                if (Value == DBNull.Value) Value = null;
                if (!Equals(Value, MasterValues[i])) return false;
            }
        }
        return true;
    }

    
    /// <summary>
    /// Sets the filter
    /// </summary>
    public void SetFilter(string PropertyName, object Value)
    {
        this.FilterPropertyName = PropertyName;
        this.FilterValue = Value;
    }
    /// <summary>
    /// Clears the filter
    /// </summary>
    public void ClearFilter()
    {
        this.FilterPropertyName = null;
        this.FilterValue = null;
    }
    private string FormatValueForSql(object fValue)
    {
        if (fValue == null || fValue == DBNull.Value) return "NULL";
        if (fValue is string s) return $"'{s.Replace("'", "''")}'";
        if (fValue is DateTime d) return $"#{d:yyyy-MM-dd HH:mm:ss}#";
        if (fValue is bool b) return b ? "1" : "0";
        return fValue.ToString();
    }
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    public bool PassesFilterCondition1(object InnerObject)
    {
        // ΕΔΩ
        return true;
    }
    /// <summary>
    /// Returns true if the specified object should be included in the visible rows
    /// </summary>
    /* Implementation for DataTableSource */
    public bool PassesFilterCondition(object InnerObject)
    {
        /* 1. If no filter is defined, everything is visible */
        if (string.IsNullOrEmpty(FilterPropertyName)) 
            return true;

        if (InnerObject is DataRow row)
        {
            /* 2. Extract value and handle DBNull */
            object value = row[FilterPropertyName];
            if (value == DBNull.Value) 
                value = null;

            /* 3. Logic for string-based filtering */
            if (value is string strValue && FilterValue is string strFilter)
            {
                if (string.IsNullOrEmpty(strFilter)) 
                    return true;

                /* Wildcard handling: StartsWith (e.g., "Al*") */
                if (strFilter.EndsWith("*") && strFilter.Length > 1)
                {
                    string cleanFilter = strFilter.Substring(0, strFilter.Length - 1);
                    return strValue.StartsWith(cleanFilter, StringComparison.OrdinalIgnoreCase);
                }

                /* Standard partial match: Contains */
                return strValue.Contains(strFilter, StringComparison.OrdinalIgnoreCase);
            }

            /* 4. Fallback to exact match for other types (int, bool, DateTime, etc.) */
            return Equals(value, FilterValue);
        }

        return true;
    }
    
    // ● CRUD 
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
    public void AddToSource(object InnerObject)
    {
        if (InnerObject is DataRow row && row.Table == null)
        {
            fTable.Rows.Add(row);
        }
    }
    /// <summary>
    /// Removes (marks for deletion) a DataRow from the underlying DataTable.
    /// </summary>
    public void RemoveFromSource(object InnerObject)
    {
        if (InnerObject is DataRow row)
        {
            row.Delete(); 
        }
    }

    // ● properties 
    /// <summary>
    /// Gets a value indicating whether the data source has a fixed size.
    /// </summary>
    public bool IsFixedSize => false;
    /// <summary>
    /// Gets a value indicating whether the data source is read-only.
    /// </summary>
    public bool IsReadOnly => false;
    /// <summary>
    /// The property name the filter is applied on.
    /// </summary>
    public string FilterPropertyName { get; private set; }
    /// <summary>
    /// The filter value.
    /// </summary>
    public object FilterValue { get; private set; }
    /// <summary>
    /// The property names when this is a detail source.
    /// </summary>
    public string[] DetailPropertyNames
    {
        get  => fChildPropertyNames;
        set => fChildPropertyNames = value;
    }

}