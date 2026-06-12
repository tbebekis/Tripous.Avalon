/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Represents an individual item entry within a data lookup or selection component.
/// </summary>
public class LookupItem
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance of the LookupItem class with specified value identifiers and display text metrics.
    /// </summary>
    public LookupItem(object Value, string DisplayText, bool IsNullItem = false, DataRow Row = null)
    {
        this.Value = Value;
        this.DisplayText = DisplayText;
        this.IsNullItem = IsNullItem;
        this.Row = Row;
    }

    // ● public methods
    /// <summary>
    /// Returns the descriptive display text representing the lookup item item.
    /// </summary>
    public override string ToString() => DisplayText ?? string.Empty;

    // ● properties
    /// <summary>
    /// Gets the unique underlying backend key or identifier value of the lookup item.
    /// </summary>
    public object Value { get; }
    /// <summary>
    /// Gets the user-friendly descriptive title text label of the lookup item.
    /// </summary>
    public string DisplayText { get; }
    /// <summary>
    /// Gets a value indicating whether this entry represents a blank or null placeholder choice.
    /// </summary>
    public bool IsNullItem { get; }
    /// <summary>
    /// Gets the complete original relational source data row context if available.
    /// </summary>
    public DataRow Row { get; }
}