// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Adapts a <see cref="DataView"/> to the <see cref="IPivotGridDataAdapter"/> contract.
/// </summary>
public class PivotGridDataViewDataAdapter: IPivotGridDataAdapter, IDisposable
{
    // ● private fields
    readonly DataView fView;
    readonly List<PivotGridSourceField> fSourceFields;
    bool fDisposed;

    // ● private methods
    DataColumn FindColumn(string FieldName)
    {
        return string.IsNullOrWhiteSpace(FieldName) || fView.Table == null || !fView.Table.Columns.Contains(FieldName)
            ? null
            : fView.Table.Columns[FieldName];
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
    List<PivotGridSourceField> CreateSourceFields()
    {
        if (fView.Table == null)
            return new List<PivotGridSourceField>();

        return fView.Table.Columns
            .Cast<DataColumn>()
            .Select(Column => PivotGridFieldRules.CreateSourceField(Column.ColumnName, string.IsNullOrWhiteSpace(Column.Caption) ? Column.ColumnName : Column.Caption, Column.DataType))
            .Where(Field => Field.CanUseAsAxis || Field.CanUseAsMeasure)
            .ToList();
    }
    void View_ListChanged(object Sender, ListChangedEventArgs Args)
    {
        Changed?.Invoke(this, PivotGridDataChangedEventArgs.Reset());
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridDataViewDataAdapter"/> class.
    /// </summary>
    /// <param name="View">The source data view.</param>
    public PivotGridDataViewDataAdapter(DataView View)
    {
        fView = View ?? throw new ArgumentNullException(nameof(View));
        fSourceFields = CreateSourceFields();
        fView.ListChanged += View_ListChanged;
    }

    // ● public methods
    /// <inheritdoc />
    public object GetRow(int RowIndex) => GetRowView(RowIndex);
    /// <inheritdoc />
    public object GetValue(int RowIndex, string FieldName)
    {
        DataColumn Column = FindColumn(FieldName);
        DataRowView RowView = GetRowView(RowIndex);
        if (Column == null || RowView == null)
            return null;

        try
        {
            object Result = RowView[Column.ColumnName];
            return Result == DBNull.Value ? null : Result;
        }
        catch (RowNotInTableException)
        {
            return null;
        }
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
    /// <inheritdoc />
    public IReadOnlyList<PivotGridSourceField> SourceFields => fSourceFields;

    // ● events
    /// <inheritdoc />
    public event EventHandler<PivotGridDataChangedEventArgs> Changed;
}
