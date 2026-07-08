// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Tests the data view adapter used by group grid table sources.
/// </summary>
public class GroupGridDataViewDataAdapterTests
{
    // ● private methods
    DataTable CreateTable()
    {
        DataTable Result = new();
        Result.Columns.Add(nameof(GridTestRow.Category), typeof(string));
        Result.Columns.Add(nameof(GridTestRow.Name), typeof(string));
        Result.Columns.Add(nameof(GridTestRow.Quantity), typeof(int));
        Result.Rows.Add("A", "Alpha", 2);
        Result.Rows.Add("B", "Beta", 5);
        return Result;
    }
    GroupGridColumn NameColumn()
    {
        return new GroupGridTextColumn { Name = nameof(GridTestRow.Name), Header = "Name" };
    }
    GroupGridColumn QuantityColumn()
    {
        return new GroupGridNumberColumn { Name = nameof(GridTestRow.Quantity), Header = "Quantity", ValueType = typeof(int) };
    }
    List<GroupGridDataChangedEventArgs> CaptureChanges(GroupGridDataViewDataAdapter Adapter)
    {
        List<GroupGridDataChangedEventArgs> Result = new();
        Adapter.Changed += (Sender, Args) => Result.Add(Args);
        return Result;
    }

    // ● tests
    /// <summary>
    /// Verifies indexed row and cell access.
    /// </summary>
    [Fact]
    public void GetValue_WithDataColumn_ReturnsColumnValue()
    {
        DataTable Table = CreateTable();
        GroupGridDataViewDataAdapter Adapter = new(Table.DefaultView);

        Assert.IsType<DataRowView>(Adapter.GetRow(0));
        Assert.Equal("Alpha", Adapter.GetValue(0, NameColumn()));
        Assert.Equal(2, Adapter.GetValue(0, QuantityColumn()));
    }
    /// <summary>
    /// Verifies value conversion and cell change notifications.
    /// </summary>
    [Fact]
    public void SetValue_WithConvertibleValue_UpdatesTableAndRaisesCellChanged()
    {
        DataTable Table = CreateTable();
        GroupGridDataViewDataAdapter Adapter = new(Table.DefaultView);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        Adapter.SetValue(0, QuantityColumn(), "9");

        Assert.Equal(9, Table.Rows[0][nameof(GridTestRow.Quantity)]);
        Assert.Contains(Changes, Change => Change.Kind == GroupGridDataChangeKind.CellChanged
                                           && Change.RowIndex == 0
                                           && Change.ColumnName == nameof(GridTestRow.Quantity));
    }
    /// <summary>
    /// Verifies table row insertion and deletion.
    /// </summary>
    [Fact]
    public void InsertRowAndDeleteRow_WithDataView_ChangeRows()
    {
        DataTable Table = CreateTable();
        GroupGridDataViewDataAdapter Adapter = new(Table.DefaultView);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        int NewIndex = Adapter.InsertRow(0);
        bool Deleted = Adapter.DeleteRow(NewIndex);

        Assert.True(NewIndex >= 0);
        Assert.True(Deleted);
        Assert.Equal(2, Adapter.RowCount);
        Assert.Contains(Changes, Change => Change.Kind == GroupGridDataChangeKind.RowAdded);
        Assert.Contains(Changes, Change => Change.Kind == GroupGridDataChangeKind.RowRemoved);
    }
    /// <summary>
    /// Verifies reset notification after changing the view sort.
    /// </summary>
    [Fact]
    public void DataViewSortChange_RaisesReset()
    {
        DataTable Table = CreateTable();
        GroupGridDataViewDataAdapter Adapter = new(Table.DefaultView);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        Table.DefaultView.Sort = nameof(GridTestRow.Name) + " DESC";

        Assert.Contains(Changes, Change => Change.Kind == GroupGridDataChangeKind.Reset);
    }
}
