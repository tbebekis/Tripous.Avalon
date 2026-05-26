/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Binding information for a <see cref="DataGridColumn"/>.
/// </summary>
public class GridColumnBinding: TripousBinding
{
    // ● constructor
    public GridColumnBinding(DataGridColumn GridColumn, DataColumn DataColumn)
    {
        this.GridColumn = GridColumn;
        this.DataColumn = DataColumn;
        this.FieldName = DataColumn.ColumnName;
    }
    public GridColumnBinding(DataGridColumn GridColumn, FieldDef FieldDef)
    {
        this.GridColumn = GridColumn;
        this.FieldDef = FieldDef;
        this.FieldName = FieldDef.Name;
    }
    public GridColumnBinding(DataGridColumn GridColumn, string FieldName, Type DataType)
    {
        this.GridColumn = GridColumn;
        this.FieldName = FieldName;
        this.DataType = DataType;
    }

    // ● properties
    public DataGridColumn GridColumn { get; }
    public string DisplayFieldName { get; set; }
    public ComboBox ActiveLookupComboBox { get; set; }
    public bool IsReference => LookupSource != null || LocatorDef != null || (FieldDef != null && (FieldDef.IsLookup || !string.IsNullOrWhiteSpace(FieldDef.Locator)));
    public bool IsPlainId => FieldName.EndsWithText("Id") && !IsReference;
}
