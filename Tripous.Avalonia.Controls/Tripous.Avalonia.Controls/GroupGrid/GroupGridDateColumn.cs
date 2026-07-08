// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a date or date-time column in a group grid.
/// </summary>
public class GroupGridDateColumn: GroupGridColumn
{
    // ● private methods
    string GetDefaultEditFormat()
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridDateColumn"/> class.
    /// </summary>
    public GroupGridDateColumn()
    {
        ValueType = typeof(DateTime);
    }

    // ● public methods
    /// <summary>
    /// Formats a value for in-place editing.
    /// </summary>
    /// <param name="Value">The value to format.</param>
    /// <returns>The formatted edit text.</returns>
    public string FormatEditValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        string Format = string.IsNullOrWhiteSpace(EditFormat) ? GetDefaultEditFormat() : EditFormat;
        return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Value);
    }
    /// <inheritdoc />
    public override object ParseValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        if (Value is string Text)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return null;
            if (GroupGridDateTextNormalizer.TryParse(Text, CultureInfo.CurrentCulture, out DateTime Date))
                return Date;
        }

        return Convert.ToDateTime(Value, CultureInfo.CurrentCulture);
    }

    // ● properties
    /// <summary>
    /// Gets or sets the format used by the in-place editor.
    /// </summary>
    public string EditFormat { get; set; } = string.Empty;
}
