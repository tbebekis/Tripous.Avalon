// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a boolean check box column in a group grid.
/// </summary>
public class GroupGridCheckBoxColumn: GroupGridColumn
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridCheckBoxColumn"/> class.
    /// </summary>
    public GroupGridCheckBoxColumn()
    {
        ValueType = typeof(bool);
        HorizontalAlignment = GroupGridCellHorizontalAlignment.Center;
    }

    // ● public methods
    /// <inheritdoc />
    public override string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        return Convert.ToBoolean(Value, CultureInfo.CurrentCulture) ? "x" : string.Empty;
    }
    /// <inheritdoc />
    public override object ParseValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return false;

        return Convert.ToBoolean(Value, CultureInfo.CurrentCulture);
    }
}
