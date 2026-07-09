/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Provides simple form-level FactBox diagnostic information.
/// </summary>
public class FormScopeFactBoxProvider: ItemFactBoxProvider
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public FormScopeFactBoxProvider()
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
        string Platform = !string.IsNullOrWhiteSpace(Context?.FormJsClassName) ? "WebDesk" : "Desktop";

        return new Dictionary<string, object>
        {
            ["Registration Scope"] = "Form",
            ["Platform"] = Platform,
            ["FactBox"] = Context?.FactBoxDef?.Name ?? string.Empty,
            ["Form"] = Context?.FormName ?? string.Empty,
            ["Module"] = Context?.ModuleDef?.Name ?? string.Empty,
            ["Key"] = Context?.KeyValue ?? string.Empty,
            ["RowState"] = Context?.RowState ?? string.Empty
        };
    }
}
