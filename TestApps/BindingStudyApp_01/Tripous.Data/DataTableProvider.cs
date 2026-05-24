namespace Tripous.Data;

/// <summary>
/// Provides DataSource access to a DataTable or DataView.
/// </summary>
public class DataTableProvider: IDataProvider
{
    // ● private fields
    DataTable fTable;
    DataView fView;

    // ● private
    object ConvertValue(string FieldName, object Value)
    {
        if (Value == null)
            return DBNull.Value;

        Type DataType = GetFieldType(FieldName);

        if (Value is string Text && string.IsNullOrWhiteSpace(Text))
            return DBNull.Value;

        if (DataType == typeof(string))
            return Value.ToString();
        if (DataType == typeof(int))
            return Convert.ToInt32(Value);
        if (DataType == typeof(decimal))
            return Convert.ToDecimal(Value);
        if (DataType == typeof(double))
            return Convert.ToDouble(Value);
        if (DataType == typeof(bool))
            return Convert.ToBoolean(Value);
        if (DataType == typeof(DateTime))
            return Convert.ToDateTime(Value);

        return Convert.ChangeType(Value, DataType);
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance using a DataTable.
    /// </summary>
    public DataTableProvider(DataTable Table)
    {
        fTable = Table;
        fView = Table.DefaultView;
        fTable.ColumnChanged += Table_ColumnChanged;
    }
    /// <summary>
    /// Initializes a new instance using a DataView.
    /// </summary>
    public DataTableProvider(DataView View)
    {
        fView = View;
        fTable = View.Table;
        fTable.ColumnChanged += Table_ColumnChanged;
    }
    void Table_ColumnChanged(object Sender, DataColumnChangeEventArgs e)
    {
        object OldValue = null;
        object NewValue = e.Row[e.Column];

        if (e.Row.HasVersion(DataRowVersion.Original))
            OldValue = e.Row[e.Column, DataRowVersion.Original];

        if (OldValue == DBNull.Value)
            OldValue = null;
        if (NewValue == DBNull.Value)
            NewValue = null;

        ItemChanged?.Invoke(this, new DataProviderChangedEventArgs(e.Row, e.Column.ColumnName, OldValue, NewValue));
    }

    // ● public
    /// <summary>
    /// Returns the column names.
    /// </summary>
    public string[] GetFieldNames()
    {
        return fTable.Columns.Cast<DataColumn>().Select(Column => Column.ColumnName).ToArray();
    }
    /// <summary>
    /// Returns the column data type.
    /// </summary>
    public Type GetFieldType(string FieldName)
    {
        return fTable.Columns.Contains(FieldName) ? fTable.Columns[FieldName].DataType : typeof(object);
    }
    /// <summary>
    /// Returns the rows exposed by the DataView.
    /// </summary>
    public IEnumerable GetItems()
    {
        foreach (DataRowView RowView in fView)
            yield return RowView;
    }
    /// <summary>
    /// Returns true when a column exists.
    /// </summary>
    public bool ContainsField(string FieldName)
    {
        return fTable.Columns.Contains(FieldName);
    }
    /// <summary>
    /// Gets a column value from a DataRowView or DataRow.
    /// </summary>
    public object GetValue(object InnerObject, string FieldName)
    {
        DataRowView RowView = InnerObject as DataRowView;
        DataRow Row = InnerObject as DataRow;
        object Result = null;

        if (!ContainsField(FieldName))
            return null;

        if (RowView != null)
            Result = RowView[FieldName];
        else if (Row != null)
            Result = Row[FieldName];

        return Result == DBNull.Value ? null : Result;
    }
    /// <summary>
    /// Sets a column value on a DataRowView or DataRow.
    /// </summary>
    public void SetValue(object InnerObject, string FieldName, object Value)
    {
        DataRowView RowView = InnerObject as DataRowView;
        DataRow Row = InnerObject as DataRow;

        if (!ContainsField(FieldName))
            return;

        if (RowView != null)
            RowView[FieldName] = ConvertValue(FieldName, Value);
        else if (Row != null)
            Row[FieldName] = ConvertValue(FieldName, Value);
    }
    /// <summary>
    /// Creates a new detached DataRow.
    /// </summary>
    public object CreateItem()
    {
        return fTable.NewRow();
    }
    /// <summary>
    /// Adds a detached DataRow to the DataTable.
    /// </summary>
    public void AddItem(object InnerObject)
    {
        if (InnerObject is DataRow Row && Row.RowState == DataRowState.Detached)
            fTable.Rows.Add(Row);
    }
    /// <summary>
    /// Deletes a DataRowView or DataRow.
    /// </summary>
    public void DeleteItem(object InnerObject)
    {
        if (InnerObject is DataRowView RowView)
            RowView.Delete();
        else if (InnerObject is DataRow Row)
            Row.Delete();
    }
    /// <summary>
    /// Returns true when two row references represent the same DataRow.
    /// </summary>
    public bool IsSameItem(object A, object B)
    {
        DataRow RowA = A is DataRowView RowViewA ? RowViewA.Row : A as DataRow;
        DataRow RowB = B is DataRowView RowViewB ? RowViewB.Row : B as DataRow;

        return RowA != null && ReferenceEquals(RowA, RowB);
    }

    // ● properties
    /// <summary>
    /// Gets a value indicating whether the source is read-only.
    /// </summary>
    public bool IsReadOnly => false;
    /// <summary>
    /// Gets a value indicating whether the source has a fixed size.
    /// </summary>
    public bool IsFixedSize => false;
    /// <summary>
    /// Gets the underlying DataTable.
    /// </summary>
    public DataTable Table => fTable;
    /// <summary>
    /// Gets the underlying DataView.
    /// </summary>
    public DataView View => fView;

    // ● events
    /// <summary>
    /// Occurs when a DataTable column value changes.
    /// </summary>
    public event EventHandler<DataProviderChangedEventArgs> ItemChanged;
}
