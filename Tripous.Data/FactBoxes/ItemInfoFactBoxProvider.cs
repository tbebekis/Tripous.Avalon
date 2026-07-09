/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// A simple FactBox provider that returns diagnostic item information.
/// </summary>
public class ItemInfoFactBoxProvider: ItemFactBoxProvider
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemInfoFactBoxProvider()
    {
    }

    // ● public
    /// <summary>
    /// Creates serializable data for a FactBox.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The serializable FactBox data.</returns>
    public override object GetData(ItemFactBoxContext Context)
    {
        Dictionary<string, object> Result = new()
        {
            ["Form"] = Context?.FormName ?? string.Empty,
            ["Module"] = Context?.ModuleDef?.Name ?? string.Empty,
            ["Table"] = Context?.TableDef?.Name ?? string.Empty,
            ["Key"] = Context?.KeyValue ?? string.Empty,
            ["RowState"] = !string.IsNullOrWhiteSpace(Context?.RowState) ? Context.RowState : Context?.Row?.RowState.ToString() ?? string.Empty
        };
        return Result;
    }
}
