// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a calculated summary value for a group grid column.
/// </summary>
public readonly struct GroupGridSummaryValue
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridSummaryValue"/> struct.
    /// </summary>
    /// <param name="Column">The grid column.</param>
    /// <param name="AggregateKind">The aggregate kind.</param>
    /// <param name="Value">The calculated value.</param>
    public GroupGridSummaryValue(GroupGridColumn Column, GroupGridAggregateKind AggregateKind, object Value)
    {
        this.Column = Column;
        this.AggregateKind = AggregateKind;
        this.Value = Value;
    }

    // ● static public
    /// <summary>
    /// Gets an empty summary value.
    /// </summary>
    static public GroupGridSummaryValue Empty => new(null, GroupGridAggregateKind.None, null);

    // ● properties
    /// <summary>
    /// Gets the grid column.
    /// </summary>
    public GroupGridColumn Column { get; }
    /// <summary>
    /// Gets the aggregate kind.
    /// </summary>
    public GroupGridAggregateKind AggregateKind { get; }
    /// <summary>
    /// Gets the calculated value.
    /// </summary>
    public object Value { get; }
}
