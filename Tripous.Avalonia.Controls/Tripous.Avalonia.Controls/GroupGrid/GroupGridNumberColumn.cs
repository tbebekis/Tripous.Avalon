// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a numeric column in a group grid.
/// </summary>
public class GroupGridNumberColumn: GroupGridColumn
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridNumberColumn"/> class.
    /// </summary>
    public GroupGridNumberColumn()
    {
        ValueType = typeof(decimal);
        HorizontalAlignment = GroupGridCellHorizontalAlignment.Right;
    }

    // ● public methods
    /// <inheritdoc />
    public override object ParseValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        if (ValueType == typeof(int))
            return Convert.ToInt32(Value, CultureInfo.CurrentCulture);
        if (ValueType == typeof(long))
            return Convert.ToInt64(Value, CultureInfo.CurrentCulture);
        if (ValueType == typeof(float))
            return Convert.ToSingle(Value, CultureInfo.CurrentCulture);
        if (ValueType == typeof(double))
            return Convert.ToDouble(Value, CultureInfo.CurrentCulture);
        if (ValueType == typeof(decimal))
            return Convert.ToDecimal(Value, CultureInfo.CurrentCulture);

        return Convert.ChangeType(Value, ValueType, CultureInfo.CurrentCulture);
    }
}
