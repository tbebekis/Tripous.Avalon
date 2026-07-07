/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Base class for Tripous UI bindings.
/// </summary>
public class TripousBinding
{
    // ● private
    /// <summary>
    /// The explicitly assigned data type.
    /// </summary>
    Type fDataType;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public TripousBinding()
    {
    }
    
    // ● public
    /// <summary>
    /// Returns the bound field name.
    /// </summary>
    /// <returns>The bound field name.</returns>
    override public string ToString() => FieldName;
    
    /// <summary>
    /// Calls the <see cref="DisposeAction"/> handler.
    /// </summary>
    public virtual void Dispose()
    {
        DisposeAction?.Invoke();
    }

    // ● properties
    /// <summary>
    /// The bound field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The <see cref="DataColumn"/> of this binding.
    /// </summary>
    public DataColumn DataColumn { get; set; }
    /// <summary>
    /// The <see cref="MemTable"/> of this binding.
    /// </summary>
    public MemTable Table => DataColumn?.Table as MemTable;

    /// <summary>
    /// Optional field definition associated to the binding.
    /// </summary>
    public FieldDef FieldDef { get; set; }
    /// <summary>
    /// The data type of this binding.
    /// </summary>
    public virtual Type DataType
    {
        get
        {
            if (fDataType != null)
                return fDataType;

            if (DataColumn != null)
                return DataColumn.DataType;

            if (FieldDef != null)
                return FieldDef.DataType.GetNetType();

            return null;
        }
        protected set => fDataType = value;
    }
    /// <summary>
    /// The lookup source associated to the binding, if any, else null.
    /// </summary>
    public LookupSource LookupSource { get; set; }
    /// <summary>
    /// The locator, if any, else null.
    /// </summary>
    public Locator Locator { get; set; }

    /// <summary>
    /// Optional locator definition associated to the binding.
    /// </summary>
    public LocatorDef LocatorDef { get; set; }
    /// <summary>
    /// Optional Locator2 definition associated to the binding.
    /// </summary>
    public LocatorDef2 LocatorDef2 { get; set; }
    /// <summary>
    /// Maps locator field names to target row field names.
    /// </summary>
    public Dictionary<string, string> LocatorTargetFieldMap { get; set; }
    /// <summary>
    /// Optional Locator2 target field mapping plan.
    /// </summary>
    public LocatorMapPlan2 LocatorMapPlan2 { get; set; }
    /// <summary>
    /// Optional Locator2 source field name used by a display binding.
    /// </summary>
    public string LocatorSourceFieldName { get; set; }
    /// <summary>
    /// Optional reference context menu associated to the binding.
    /// </summary>
    public ReferenceContextMenu ReferenceContextMenu { get; set; }
    /// <summary>
    /// True while refreshing.
    /// </summary>
    public bool IsRefreshing { get; set; }
    /// <summary>
    /// Gets or sets the action called when this binding is disposed.
    /// </summary>
    public Action DisposeAction { get; set; }
}
