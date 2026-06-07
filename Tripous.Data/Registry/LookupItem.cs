/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class LookupItem
{
    // ● construction
    public LookupItem(object Value, string DisplayText, bool IsNullItem = false, DataRow Row = null)
    {
        this.Value = Value;
        this.DisplayText = DisplayText;
        this.IsNullItem = IsNullItem;
        this.Row = Row;
    }

    // ● public
    public override string ToString() =>  DisplayText ?? string.Empty;
 
    // ● properties
    public object Value { get; }
    public string DisplayText { get; }
    public bool IsNullItem { get; }
    public DataRow Row { get; }
}
