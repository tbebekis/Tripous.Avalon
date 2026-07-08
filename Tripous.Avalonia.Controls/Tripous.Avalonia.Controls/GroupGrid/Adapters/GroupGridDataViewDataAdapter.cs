// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Adapts a <see cref="DataView"/> to the <see cref="IGroupGridDataAdapter"/> contract.
/// </summary>
public class GroupGridDataViewDataAdapter: IGroupGridDataAdapter, IDisposable
{
    // ● private fields
    readonly DataView fView;
    bool fDisposed;

    // ● private methods
    DataColumn FindColumn(GroupGridColumn Column)
    {
        string Name = Column == null ? string.Empty : Column.Name;
        return string.IsNullOrWhiteSpace(Name) || fView.Table == null || !fView.Table.Columns.Contains(Name)
            ? null
            : fView.Table.Columns[Name];
    }
    DataRowView GetRowView(int RowIndex)
    {
        if (RowIndex < 0 || RowIndex >= fView.Count)
            return null;

        DataRowView RowView = fView[RowIndex];
        if (RowView == null || RowView.Row == null)
            return null;

        DataRowState State = RowView.Row.RowState;
        return State == DataRowState.Deleted || State == DataRowState.Detached ? null : RowView;
    }
    int IndexOfRow(DataRow Row)
    {
        if (Row == null)
            return -1;

        for (int Index = 0; Index < fView.Count; Index++)
            if (ReferenceEquals(fView[Index].Row, Row))
                return Index;

        return -1;
    }
    object ConvertValue(object Value, DataColumn DataColumn)
    {
        if (Value == null || Value == DBNull.Value)
            return DBNull.Value;

        Type DataType = DataColumn.DataType;
        if (DataType == typeof(string))
            return Convert.ToString(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(int))
            return Convert.ToInt32(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(long))
            return Convert.ToInt64(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(decimal))
            return Convert.ToDecimal(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(double))
            return Convert.ToDouble(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(float))
            return Convert.ToSingle(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(bool))
            return Convert.ToBoolean(Value, CultureInfo.CurrentCulture);
        if (DataType == typeof(DateTime))
            return Convert.ToDateTime(Value, CultureInfo.CurrentCulture);

        return Value;
    }
    void View_ListChanged(object Sender, ListChangedEventArgs Args)
    {
        switch (Args.ListChangedType)
        {
            case ListChangedType.ItemAdded:
                Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowAdded(Args.NewIndex));
                break;
            case ListChangedType.ItemDeleted:
                Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowRemoved(Args.NewIndex));
                break;
            case ListChangedType.ItemMoved:
                Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowMoved(Args.OldIndex, Args.NewIndex));
                break;
            case ListChangedType.ItemChanged:
                if (Args.PropertyDescriptor == null)
                    Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowChanged(Args.NewIndex));
                else
                    Changed?.Invoke(this, GroupGridDataChangedEventArgs.CellChanged(Args.NewIndex, Args.PropertyDescriptor.Name));
                break;
            default:
                Changed?.Invoke(this, GroupGridDataChangedEventArgs.Reset());
                break;
        }
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridDataViewDataAdapter"/> class.
    /// </summary>
    /// <param name="View">The source data view.</param>
    public GroupGridDataViewDataAdapter(DataView View)
    {
        fView = View ?? throw new ArgumentNullException(nameof(View));
        fView.ListChanged += View_ListChanged;
    }

    // ● public methods
    /// <inheritdoc />
    public object GetRow(int RowIndex) => GetRowView(RowIndex);
    /// <inheritdoc />
    public object GetValue(int RowIndex, GroupGridColumn Column)
    {
        DataColumn DataColumn = FindColumn(Column);
        DataRowView RowView = GetRowView(RowIndex);
        if (DataColumn == null || RowView == null)
            return null;

        try
        {
            object Result = RowView[DataColumn.ColumnName];
            return Result == DBNull.Value ? null : Result;
        }
        catch (RowNotInTableException)
        {
            return null;
        }
    }
    /// <inheritdoc />
    public void SetValue(int RowIndex, GroupGridColumn Column, object Value)
    {
        DataColumn DataColumn = FindColumn(Column);
        DataRowView RowView = GetRowView(RowIndex);
        if (DataColumn == null || RowView == null || DataColumn.ReadOnly || Column == null || Column.IsReadOnly)
            return;

        try
        {
            object OldValue = RowView[DataColumn.ColumnName];
            object ParsedValue = ConvertValue(Column.ParseValue(Value), DataColumn);
            if (Equals(OldValue, ParsedValue))
                return;

            RowView[DataColumn.ColumnName] = ParsedValue;
        }
        catch
        {
            // Invalid input is ignored by the adapter.
        }
    }
    /// <inheritdoc />
    public bool CanSetValue(int RowIndex, GroupGridColumn Column)
    {
        DataColumn DataColumn = FindColumn(Column);
        return DataColumn != null && !DataColumn.ReadOnly && Column != null && !Column.IsReadOnly;
    }
    /// <inheritdoc />
    public bool CanInsertRow(int RowIndex)
    {
        return fView.Table != null;
    }
    /// <inheritdoc />
    public int InsertRow(int RowIndex)
    {
        if (!CanInsertRow(RowIndex))
            return -1;

        DataTable Table = fView.Table;
        DataRow Row = Table.NewRow();
        int TableIndex = Table.Rows.Count;
        DataRowView RowView = GetRowView(RowIndex);
        if (RowView != null)
        {
            int CurrentTableIndex = Table.Rows.IndexOf(RowView.Row);
            if (CurrentTableIndex >= 0)
                TableIndex = CurrentTableIndex + 1;
        }

        Table.Rows.InsertAt(Row, TableIndex);
        return IndexOfRow(Row);
    }
    /// <inheritdoc />
    public bool CanDeleteRow(int RowIndex)
    {
        return GetRowView(RowIndex) != null;
    }
    /// <inheritdoc />
    public bool DeleteRow(int RowIndex)
    {
        DataRowView RowView = GetRowView(RowIndex);
        if (RowView == null)
            return false;

        RowView.Delete();
        return true;
    }
    /// <summary>
    /// Releases subscriptions held by this adapter.
    /// </summary>
    public void Dispose()
    {
        if (fDisposed)
            return;

        fView.ListChanged -= View_ListChanged;
        fDisposed = true;
    }

    // ● properties
    /// <inheritdoc />
    public int RowCount => fView.Count;

    // ● events
    /// <inheritdoc />
    public event EventHandler<GroupGridDataChangedEventArgs> Changed;
}
