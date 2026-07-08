// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Base class for all group grid columns.
/// </summary>
public class GroupGridColumn
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridColumn"/> class.
    /// </summary>
    public GroupGridColumn()
    {
    }

    // ● public methods
    /// <summary>
    /// Formats a value for display.
    /// </summary>
    /// <param name="Value">The value to format.</param>
    /// <returns>The formatted display text.</returns>
    public virtual string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        if (!string.IsNullOrWhiteSpace(DisplayFormat))
            return string.Format(CultureInfo.CurrentCulture, $"{{0:{DisplayFormat}}}", Value);

        return string.Format(CultureInfo.CurrentCulture, "{0}", Value);
    }
    /// <summary>
    /// Parses an editor value before it is written to the adapter.
    /// </summary>
    /// <param name="Value">The editor value.</param>
    /// <returns>The parsed value.</returns>
    public virtual object ParseValue(object Value) => Value;
    /// <summary>
    /// Returns true when an aggregate kind is valid for this column.
    /// </summary>
    /// <param name="AggregateKind">The aggregate kind.</param>
    /// <returns>True when the aggregate kind is valid; otherwise, false.</returns>
    public virtual bool CanAggregate(GroupGridAggregateKind AggregateKind)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        bool IsNumeric = Type == typeof(byte)
                         || Type == typeof(sbyte)
                         || Type == typeof(short)
                         || Type == typeof(ushort)
                         || Type == typeof(int)
                         || Type == typeof(uint)
                         || Type == typeof(long)
                         || Type == typeof(ulong)
                         || Type == typeof(float)
                         || Type == typeof(double)
                         || Type == typeof(decimal);
        bool IsDate = Type == typeof(DateTime) || Type == typeof(DateTimeOffset);

        switch (AggregateKind)
        {
            case GroupGridAggregateKind.None:
            case GroupGridAggregateKind.Count:
                return true;
            case GroupGridAggregateKind.Sum:
                return IsNumeric;
            case GroupGridAggregateKind.Average:
                return IsNumeric || IsDate;
            case GroupGridAggregateKind.Min:
            case GroupGridAggregateKind.Max:
                return Type != typeof(bool) && typeof(IComparable).IsAssignableFrom(Type);
        }

        return false;
    }
    /// <summary>
    /// Formats a summary value for display.
    /// </summary>
    /// <param name="Summary">The summary value.</param>
    /// <returns>The formatted summary text.</returns>
    public virtual string FormatSummaryValue(GroupGridSummaryValue Summary)
    {
        if (Summary.AggregateKind == GroupGridAggregateKind.None)
            return string.Empty;
        if (Summary.AggregateKind == GroupGridAggregateKind.Count)
            return string.Format(CultureInfo.CurrentCulture, "count={0}", Summary.Value);

        string Prefix = string.Empty;
        switch (Summary.AggregateKind)
        {
            case GroupGridAggregateKind.Sum:
                Prefix = "sum";
                break;
            case GroupGridAggregateKind.Min:
                Prefix = "min";
                break;
            case GroupGridAggregateKind.Max:
                Prefix = "max";
                break;
            case GroupGridAggregateKind.Average:
                Prefix = "avg";
                break;
        }

        return string.IsNullOrEmpty(Prefix)
            ? FormatValue(Summary.Value)
            : string.Format(CultureInfo.CurrentCulture, "{0}={1}", Prefix, FormatValue(Summary.Value));
    }

    // ● properties
    /// <summary>
    /// Gets or sets the column name or key.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    public string Header { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    public double Width { get; set; } = 100;
    /// <summary>
    /// Gets or sets the minimum column width.
    /// </summary>
    public double MinWidth { get; set; } = 24;
    /// <summary>
    /// Gets or sets a value indicating whether the column is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the column is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether users may resize the column.
    /// </summary>
    public bool CanUserResize { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether users may reorder the column.
    /// </summary>
    public bool CanUserReorder { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether users may group by the column.
    /// </summary>
    public bool CanUserGroup { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether users may hide the column.
    /// </summary>
    public bool CanUserHide { get; set; } = true;
    /// <summary>
    /// Gets or sets the column value type.
    /// </summary>
    public Type ValueType { get; set; } = typeof(object);
    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the horizontal alignment used when drawing cell text.
    /// </summary>
    public GroupGridCellHorizontalAlignment HorizontalAlignment { get; set; }
    /// <summary>
    /// Gets or sets the group summary aggregate kind.
    /// </summary>
    public GroupGridAggregateKind GroupSummary { get; set; }
    /// <summary>
    /// Gets or sets the total summary aggregate kind.
    /// </summary>
    public GroupGridAggregateKind TotalSummary { get; set; }
    /// <summary>
    /// Gets or sets custom user data.
    /// </summary>
    public object Tag { get; set; }
}
