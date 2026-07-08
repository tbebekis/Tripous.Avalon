// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Tests the list data adapter used by group grid list sources.
/// </summary>
public class GroupGridListDataAdapterTests
{
    // ● private methods
    ObservableCollection<GridTestRow> CreateRows()
    {
        return new ObservableCollection<GridTestRow>
        {
            new() { Category = "A", Name = "Alpha", Quantity = 2, Amount = 10m },
            new() { Category = "B", Name = "Beta", Quantity = 5, Amount = 25m },
        };
    }
    GroupGridColumn NameColumn()
    {
        return new GroupGridTextColumn { Name = nameof(GridTestRow.Name), Header = "Name" };
    }
    List<GroupGridDataChangedEventArgs> CaptureChanges(GroupGridListDataAdapter<GridTestRow> Adapter)
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
    public void GetValue_WithPropertyColumn_ReturnsPropertyValue()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGridListDataAdapter<GridTestRow> Adapter = new(Rows);

        Assert.Same(Rows[0], Adapter.GetRow(0));
        Assert.Equal("Alpha", Adapter.GetValue(0, NameColumn()));
    }
    /// <summary>
    /// Verifies that setting a writable value updates the source and raises a cell change.
    /// </summary>
    [Fact]
    public void SetValue_WithWritableProperty_UpdatesItemAndRaisesCellChanged()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGridListDataAdapter<GridTestRow> Adapter = new(Rows);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        Adapter.SetValue(0, NameColumn(), "Changed");

        Assert.Equal("Changed", Rows[0].Name);
        GroupGridDataChangedEventArgs Change = Assert.Single(Changes);
        Assert.Equal(GroupGridDataChangeKind.CellChanged, Change.Kind);
        Assert.Equal(0, Change.RowIndex);
        Assert.Equal(nameof(GridTestRow.Name), Change.ColumnName);
    }
    /// <summary>
    /// Verifies row property change notifications.
    /// </summary>
    [Fact]
    public void RowPropertyChanged_WithPropertyName_RaisesCellChanged()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGridListDataAdapter<GridTestRow> Adapter = new(Rows);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        Rows[1].Name = "Changed";

        GroupGridDataChangedEventArgs Change = Assert.Single(Changes);
        Assert.Equal(GroupGridDataChangeKind.CellChanged, Change.Kind);
        Assert.Equal(1, Change.RowIndex);
        Assert.Equal(nameof(GridTestRow.Name), Change.ColumnName);
    }
    /// <summary>
    /// Verifies observable collection add, remove, and move notifications.
    /// </summary>
    [Fact]
    public void ObservableCollectionChanges_RaiseRowNotifications()
    {
        ObservableCollection<GridTestRow> Rows = CreateRows();
        GroupGridListDataAdapter<GridTestRow> Adapter = new(Rows);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        Rows.Add(new GridTestRow { Category = "C", Name = "Gamma" });
        Rows.Move(2, 0);
        Rows.RemoveAt(1);

        Assert.Equal(GroupGridDataChangeKind.RowAdded, Changes[0].Kind);
        Assert.Equal(2, Changes[0].RowIndex);
        Assert.Equal(GroupGridDataChangeKind.RowMoved, Changes[1].Kind);
        Assert.Equal(2, Changes[1].OldRowIndex);
        Assert.Equal(0, Changes[1].RowIndex);
        Assert.Equal(GroupGridDataChangeKind.RowRemoved, Changes[2].Kind);
        Assert.Equal(1, Changes[2].RowIndex);
    }
    /// <summary>
    /// Verifies adapter insert and delete support.
    /// </summary>
    [Fact]
    public void InsertRowAndDeleteRow_WithMutableList_ChangeRows()
    {
        List<GridTestRow> Rows = CreateRows().ToList();
        GroupGridListDataAdapter<GridTestRow> Adapter = new(Rows);
        List<GroupGridDataChangedEventArgs> Changes = CaptureChanges(Adapter);

        int NewIndex = Adapter.InsertRow(0);
        bool Deleted = Adapter.DeleteRow(NewIndex);

        Assert.Equal(1, NewIndex);
        Assert.True(Deleted);
        Assert.Equal(2, Rows.Count);
        Assert.Equal(GroupGridDataChangeKind.RowAdded, Changes[0].Kind);
        Assert.Equal(GroupGridDataChangeKind.RowRemoved, Changes[1].Kind);
    }
}
