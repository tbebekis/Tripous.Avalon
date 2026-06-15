/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Binding information for a grid column.
/// </summary>
public class GridColumnBinding: TripousBinding
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GridColumnBinding"/> class.
    /// </summary>
    /// <param name="GridColumn">The data grid column.</param>
    /// <param name="DataColumn">The data column.</param>
    public GridColumnBinding(DataGridColumn GridColumn, DataColumn DataColumn)
    {
        this.GridColumn = GridColumn;
        this.DataColumn = DataColumn;
        this.FieldName = DataColumn.ColumnName;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="GridColumnBinding"/> class.
    /// </summary>
    /// <param name="GridColumn">The data grid column.</param>
    /// <param name="FieldDef">The field definition.</param>
    public GridColumnBinding(DataGridColumn GridColumn, FieldDef FieldDef)
    {
        this.GridColumn = GridColumn;
        this.FieldDef = FieldDef;
        this.FieldName = FieldDef.Name;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="GridColumnBinding"/> class.
    /// </summary>
    /// <param name="GridColumn">The data grid column.</param>
    /// <param name="FieldName">The field name.</param>
    /// <param name="DataType">The data type.</param>
    public GridColumnBinding(DataGridColumn GridColumn, string FieldName, Type DataType)
    {
        this.GridColumn = GridColumn;
        this.FieldName = FieldName;
        this.DataType = DataType;
    }

    // ● properties
    /// <summary>
    /// Gets the data grid column.
    /// </summary>
    public DataGridColumn GridColumn { get; }
    /// <summary>
    /// Gets or sets the display field name.
    /// </summary>
    public string DisplayFieldName { get; set; }
    /// <summary>
    /// Gets or sets the active lookup combo box.
    /// </summary>
    public ComboBox ActiveLookupComboBox { get; set; }
    /// <summary>
    /// Gets a value indicating whether this binding points to a reference field.
    /// </summary>
    public bool IsReference => LookupSource != null || LocatorDef != null || (FieldDef != null && (FieldDef.IsLookup || !string.IsNullOrWhiteSpace(FieldDef.Locator)));
    /// <summary>
    /// Gets a value indicating whether this binding points to a plain ID field.
    /// </summary>
    public bool IsPlainId => FieldName.EndsWithText("Id") && !IsReference;
}
