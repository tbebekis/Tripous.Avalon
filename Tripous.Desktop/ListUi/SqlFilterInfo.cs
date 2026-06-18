/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Holds UI controls associated with a SQL filter definition.
/// </summary>
public class SqlFilterInfo
{
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFilterInfo"/> class.
    /// </summary>
    public SqlFilterInfo()
    {
    }
    
    // ● properties
    /// <summary>
    /// Gets or sets the SQL filter definition.
    /// </summary>
    public SqlFilterDef FilterDef { get; set; }
    /// <summary>
    /// Gets or sets the first filter value control.
    /// </summary>
    public Control Control  { get; set; }
    /// <summary>
    /// Gets or sets the second filter value control.
    /// </summary>
    public Control Control2  { get; set; }
    /// <summary>
    /// Gets or sets the boolean operator combo box.
    /// </summary>
    public ComboBox BoolOpCombo { get; set; }
    /// <summary>
    /// Gets or sets the condition operator combo box.
    /// </summary>
    public ComboBox ConditionOpCombo { get; set; }
    /// <summary>
    /// Gets or sets the boolean value combo box.
    /// </summary>
    public ComboBox BooleanValueCombo { get; set; }
}
