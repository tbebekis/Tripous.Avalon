/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Result of a locator search.
/// </summary>
public class LocatorSearchResult
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorSearchResult()
    {
    }
 
    // ● properties
    /// <summary>
    /// The locator
    /// </summary>
    public Locator Locator { get; init; }
    /// <summary>
    /// The search term text, just used by the locator in this last select;
    /// </summary>
    public string SearchTerm { get; init; }
    /// <summary>
    /// The last <see cref="SelectSql"/> just executed by the locator.
    /// </summary>
    public SelectSql SelectSql { get; init; }
    
    /// <summary>
    /// Returns true when returned rows exceed the allowed dropdown limit.
    /// </summary>
    public bool TooManyRows { get; init; }
    /// <summary>
    /// Optional result message.
    /// </summary>
    public string Message { get; init; }
    
    /// <summary>
    /// Locator definition.
    /// </summary>
    public LocatorDef LocatorDef => Locator.LocatorDef;
    /// <summary>
    /// Result table.
    /// </summary>
    public MemTable SourceTable => Locator.SourceTable;
    
    /// <summary>
    /// Result row count.
    /// </summary>
    public int RowCount => SourceTable?.Rows.Count ?? 0;
    /// <summary>
    /// Returns true when no rows exist.
    /// </summary>
    public bool IsEmpty => RowCount == 0;
    /// <summary>
    /// Returns true when exactly one row exists.
    /// </summary>
    public bool IsSingleRow => RowCount == 1;
    /// <summary>
    /// Returns true when dropdown display is allowed.
    /// </summary>
    public bool CanShowDropDown => !TooManyRows && !IsEmpty;

}