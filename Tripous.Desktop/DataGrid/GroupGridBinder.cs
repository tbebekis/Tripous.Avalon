/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides helper methods for binding group grids to data tables and definitions.
/// </summary>
static public class GroupGridBinder
{
    // ● private
    static GroupGridCellHorizontalAlignment ToGroupGridAlignment(TextAlignment Alignment)
    {
        if (Alignment == TextAlignment.Center)
            return GroupGridCellHorizontalAlignment.Center;
        if (Alignment == TextAlignment.Right)
            return GroupGridCellHorizontalAlignment.Right;
        return GroupGridCellHorizontalAlignment.Left;
    }
    static bool IsNumeric(Type DataType)
    {
        return DataType == typeof(byte)
            || DataType == typeof(sbyte)
            || DataType == typeof(short)
            || DataType == typeof(ushort)
            || DataType == typeof(int)
            || DataType == typeof(uint)
            || DataType == typeof(long)
            || DataType == typeof(ulong)
            || DataType == typeof(float)
            || DataType == typeof(double)
            || DataType == typeof(decimal);
    }
    static GroupGridColumn CreateColumn(string ColumnName, string Header, Type DataType, string Format, TextAlignment Alignment, bool IsBoolean, bool IsReadOnly)
    {
        GroupGridColumn Result;

        if (IsBoolean)
            Result = new GroupGridCheckBoxColumn();
        else if (DataType == typeof(DateTime))
            Result = new GroupGridDateColumn();
        else if (IsNumeric(DataType))
            Result = new GroupGridNumberColumn();
        else
            Result = new GroupGridTextColumn();

        Result.Name = ColumnName;
        Result.Header = Header;
        Result.ValueType = DataType;
        Result.DisplayFormat = Format ?? string.Empty;
        Result.HorizontalAlignment = IsBoolean ? GroupGridCellHorizontalAlignment.Center : ToGroupGridAlignment(Alignment);
        Result.IsReadOnly = IsReadOnly;

        return Result;
    }
    static GroupGridColumn GetFirstCurrentColumn(GroupGrid Grid)
    {
        return Grid?.GetVisibleValueColumns().FirstOrDefault() ?? Grid?.Columns.FirstOrDefault();
    }

    // ● public
    /// <summary>
    /// Binds a group grid to a data view using select definition metadata.
    /// </summary>
    /// <param name="SelectDef">The select definition.</param>
    /// <param name="Grid">The group grid.</param>
    /// <param name="DataView">The data view.</param>
    /// <param name="GoToFirst">True to select the first row.</param>
    /// <returns>The created group grid columns.</returns>
    static public List<GroupGridColumn> BindGrid(SelectDef SelectDef, GroupGrid Grid, DataView DataView, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();

        if (SelectDef != null)
        {
            foreach (DataColumn Column in DataColumns)
            {
                if (SelectDef.DisplayLabels.TryGetValue(Column.ColumnName, out string Label))
                    Column.Caption = Label;

                if (SelectDef.ColumnTypes.TryGetValue(Column.ColumnName, out DataColumnType ColumnType))
                    Column.ExtendedProperties["ColumnType"] = ColumnType;
            }
        }

        List<GroupGridColumn> Result = CreateColumns(Grid, DataColumns);
        Grid.ItemsSource = DataView;
        Grid.BestFitColumns();

        if (GoToFirst && DataView.Count > 0)
            SelectRow(Grid, 0);

        return Result;
    }
    /// <summary>
    /// Binds a group grid to a data view.
    /// </summary>
    /// <param name="Grid">The group grid.</param>
    /// <param name="DataView">The data view.</param>
    /// <param name="GoToFirst">True to select the first row.</param>
    /// <returns>The created group grid columns.</returns>
    static public List<GroupGridColumn> BindGrid(GroupGrid Grid, DataView DataView, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();
        List<GroupGridColumn> Result = CreateColumns(Grid, DataColumns);
        Grid.ItemsSource = DataView;
        Grid.BestFitColumns();

        if (GoToFirst && DataView.Count > 0)
            SelectRow(Grid, 0);

        return Result;
    }
    /// <summary>
    /// Unbinds a group grid.
    /// </summary>
    /// <param name="Grid">The group grid.</param>
    static public void UnBindGrid(GroupGrid Grid)
    {
        Grid.ItemsSource = null;
        Grid.Columns.Clear();
        Grid.ClearCurrentCell();
        Grid.ClearSelection();
    }
    /// <summary>
    /// Creates group grid columns from data columns.
    /// </summary>
    /// <param name="Grid">The group grid.</param>
    /// <param name="DataColumns">The data columns.</param>
    /// <returns>The created group grid columns.</returns>
    static public List<GroupGridColumn> CreateColumns(GroupGrid Grid, DataColumn[] DataColumns)
    {
        List<GroupGridColumn> Result = [];

        foreach (DataColumn Column in DataColumns)
            Result.Add(CreateGridColumn(Column, Format: Column.DataType.GetDefaultFormat(), Alignment: Column.DataType.GetTextAlignment(), IsReadOnly: Column.ReadOnly));

        foreach (GroupGridColumn Column in Result)
            Grid.Columns.Add(Column);

        return Result;
    }
    /// <summary>
    /// Creates a group grid column from a data column.
    /// </summary>
    /// <param name="Column">The data column.</param>
    /// <param name="Format">The display format.</param>
    /// <param name="Alignment">The text alignment.</param>
    /// <param name="IsReadOnly">True when the column is read-only.</param>
    /// <returns>The created group grid column.</returns>
    static public GroupGridColumn CreateGridColumn(DataColumn Column, string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false)
    {
        Format = DataGridBinder.GetDateAwareFormat(Column.ColumnName, Column.DataType, Format);

        DataColumnType ColumnType = Column.ExtendedProperties.ContainsKey("ColumnType")
            ? (DataColumnType)Column.ExtendedProperties["ColumnType"]
            : DataColumnType.None;

        if (ColumnType.HasFlag(DataColumnType.Integer))
            Format = "0";

        bool IsBoolean = ColumnType.HasFlag(DataColumnType.Boolean)
                         || Column.DataType == typeof(bool)
                         || Column.IsCheckBox();

        TextAlignment Align = Alignment ?? (IsBoolean ? TextAlignment.Center : Column.DataType.GetTextAlignment());
        string Caption = Texts.L(Column.Caption);
        Caption = DataGridBinder.GetHeader(Column.ColumnName, Caption);

        GroupGridColumn Result = CreateColumn(Column.ColumnName, Caption, Column.DataType, Format, Align, IsBoolean, IsReadOnly);
        Result.Tag = new GroupGridColumnBinding(Result, Column);

        return Result;
    }
    /// <summary>
    /// Creates a group grid column.
    /// </summary>
    /// <param name="ColumnName">The column name.</param>
    /// <param name="Header">The column header.</param>
    /// <param name="DataType">The data type.</param>
    /// <param name="Format">The display format.</param>
    /// <param name="IsReadOnly">True when the column is read-only.</param>
    /// <returns>The created group grid column.</returns>
    static public GroupGridColumn CreateGridColumn(string ColumnName, string Header, DataFieldType DataType, string Format = null, bool IsReadOnly = false)
    {
        bool IsBoolean = DataType == DataFieldType.Boolean;
        TextAlignment Align = IsBoolean ? TextAlignment.Center : DataType.GetTextAlignment();
        Type NetType = DataType.GetNetType();
        string Caption = DataGridBinder.GetHeader(ColumnName, Header);

        GroupGridColumn Result = CreateColumn(ColumnName, Caption, NetType, DataGridBinder.GetDateAwareFormat(ColumnName, NetType, Format), Align, IsBoolean, IsReadOnly);
        Result.Tag = new GroupGridColumnBinding(Result, ColumnName, NetType);

        return Result;
    }
    /// <summary>
    /// Creates a group grid column from a field definition.
    /// </summary>
    /// <param name="FieldDef">The field definition.</param>
    /// <returns>The created group grid column.</returns>
    static public GroupGridColumn CreateGridColumn(FieldDef FieldDef)
    {
        bool IsBoolean = FieldDef.IsBoolean;
        TextAlignment Align = IsBoolean ? TextAlignment.Center : FieldDef.DataType.GetTextAlignment();
        GroupGridColumn Result = CreateColumn(
            FieldDef.Name,
            DataGridBinder.GetHeader(FieldDef.Name, FieldDef.Title),
            FieldDef.DataType.GetNetType(),
            DataGridBinder.GetDateAwareFormat(FieldDef),
            Align,
            IsBoolean,
            FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI);

        Result.Tag = new GroupGridColumnBinding(Result, FieldDef);
        return Result;
    }
    /// <summary>
    /// Creates a lookup group grid column from a field definition.
    /// </summary>
    /// <param name="FieldDef">The field definition.</param>
    /// <param name="LookupDef">The lookup definition.</param>
    /// <returns>The created group grid column.</returns>
    static public GroupGridColumn CreateLookupColumn(FieldDef FieldDef, LookupDef LookupDef = null)
    {
        LookupDef = LookupDef ?? DataRegistry.Lookups.Get(FieldDef.LookupSource);
        LookupSource LookupSource = LookupDef.Create();

        GroupGridLookupColumn Result = new()
        {
            Name = FieldDef.Name,
            Header = DataGridBinder.GetHeader(FieldDef.Name, FieldDef.Title),
            ValueType = FieldDef.DataType.GetNetType(),
            IsReadOnly = FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI,
            LookupItemsSource = LookupSource.GetList(),
            DisplayMember = nameof(LookupItem.DisplayText),
            ValueMember = nameof(LookupItem.Value)
        };

        GroupGridColumnBinding Binding = new(Result, FieldDef)
        {
            LookupSource = LookupSource
        };
        Result.Tag = Binding;
        return Result;
    }
    /// <summary>
    /// Creates a locator display column for a group grid.
    /// </summary>
    /// <param name="ColumnName">The column name.</param>
    /// <param name="Header">The column header.</param>
    /// <param name="FieldDef">The field definition.</param>
    /// <param name="LocatorFieldDef">The locator field definition.</param>
    /// <param name="LocatorDef">The locator definition.</param>
    /// <param name="TargetFieldMap">The target field map.</param>
    /// <returns>The created group grid column.</returns>
    static public GroupGridColumn CreateLocatorColumn(string ColumnName, string Header, FieldDef FieldDef, LocatorFieldDef LocatorFieldDef, LocatorDef LocatorDef, Dictionary<string, string> TargetFieldMap)
    {
        // TODO: Adapt grid locator editing after Locator2 replaces the current locator stack.
        GroupGridColumn Result = CreateGridColumn(ColumnName, Header, LocatorFieldDef.DataType, IsReadOnly: true);
        GroupGridColumnBinding Binding = new(Result, FieldDef.Name, FieldDef.DataType.GetNetType())
        {
            FieldDef = FieldDef,
            DisplayFieldName = ColumnName,
            LocatorDef = LocatorDef,
            LocatorTargetFieldMap = TargetFieldMap
        };
        Result.Tag = Binding;
        return Result;
    }
    /// <summary>
    /// Creates a Locator2 display column for a group grid.
    /// </summary>
    /// <param name="ColumnName">The target display column name.</param>
    /// <param name="Header">The column header.</param>
    /// <param name="FieldDef">The reference field definition.</param>
    /// <param name="LocatorFieldDef">The Locator2 source field definition.</param>
    /// <param name="LocatorDef">The Locator2 definition.</param>
    /// <param name="MapPlan">The Locator2 mapping plan.</param>
    /// <returns>The created group grid column.</returns>
    static public GroupGridColumn CreateLocatorColumn2(string ColumnName, string Header, FieldDef FieldDef, LocatorFieldDef2 LocatorFieldDef, LocatorDef2 LocatorDef, LocatorMapPlan2 MapPlan)
    {
        GroupGridColumn Result = CreateGridColumn(ColumnName, Header, LocatorFieldDef.DataType, IsReadOnly: FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI);
        GroupGridColumnBinding Binding = new(Result, FieldDef.Name, FieldDef.DataType.GetNetType())
        {
            FieldDef = FieldDef,
            DisplayFieldName = ColumnName,
            LocatorDef2 = LocatorDef,
            LocatorMapPlan2 = MapPlan,
            LocatorSourceFieldName = LocatorFieldDef.Name
        };
        Result.Tag = Binding;
        return Result;
    }
    /// <summary>
    /// Selects a row by adapter row index.
    /// </summary>
    /// <param name="Grid">The group grid.</param>
    /// <param name="RowIndex">The adapter row index.</param>
    /// <returns>True if the row was selected; otherwise, false.</returns>
    static public bool SelectRow(GroupGrid Grid, int RowIndex)
    {
        if (Grid == null || RowIndex < 0 || RowIndex >= Grid.RowCount)
            return false;

        GroupGridColumn Column = GetFirstCurrentColumn(Grid);
        if (Column == null)
            return false;

        GroupGridCell Cell = new(RowIndex, Column);
        Grid.SetCurrentCell(Cell);
        Grid.SetSelectedCell(Cell);
        Grid.ScrollToRow(RowIndex);
        return true;
    }
    /// <summary>
    /// Returns the current row view of a group grid.
    /// </summary>
    /// <param name="Grid">The group grid.</param>
    /// <returns>The current row view, if any; otherwise, null.</returns>
    static public DataRowView GetCurrentRowView(GroupGrid Grid)
    {
        return Grid?.CurrentRow as DataRowView;
    }
    /// <summary>
    /// Returns the group grid column binding associated with a group grid column.
    /// </summary>
    /// <param name="Column">The group grid column.</param>
    /// <returns>The group grid column binding, if any; otherwise, null.</returns>
    static public GroupGridColumnBinding GetInfo(this GroupGridColumn Column) => Column != null ? Column.Tag as GroupGridColumnBinding : null;
    /// <summary>
    /// Returns the group grid column bindings associated with a group grid.
    /// </summary>
    /// <param name="Grid">The group grid.</param>
    /// <returns>The group grid column bindings.</returns>
    static public List<GroupGridColumnBinding> GetInfoList(this GroupGrid Grid)
    {
        List<GroupGridColumnBinding> Result = [];

        if (Grid != null && Grid.Columns.Count > 0)
        {
            foreach (GroupGridColumn GridColumn in Grid.Columns)
            {
                GroupGridColumnBinding Binding = GridColumn.GetInfo();
                if (Binding != null)
                    Result.Add(Binding);
            }
        }

        return Result;
    }
}
