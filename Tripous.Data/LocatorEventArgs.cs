/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Used with a locator
/// </summary>
public class LocatorEventArgs : EventArgs
{
    // ● construction 
    /// <summary>
    /// Constructor
    /// </summary>
    public LocatorEventArgs()
    {
    }

    // ● properties  
    /// <summary>
    /// The type of the locator event, this EventArgs are used with
    /// </summary>
    public LocatorEventType EventType { get; init; }
    /// <summary>
    /// The locator
    /// </summary>
    public Locator Locator { get;  init; }
    /// <summary>
    /// The locator descriptor
    /// </summary>
    public LocatorDef LocatorDef => Locator.LocatorDef;
    /// <summary>
    /// The SELECT statement.
    /// <para>Used with the <see cref="LocatorEventType.AddToWhere"/> type and the <see cref="LocatorEventType.SelectSourceTable"/> only.</para>
    /// <para>With <see cref="LocatorEventType.AddToWhere"/>, client code may add to <see cref="SelectSql.Where"/>.</para>
    /// <para>With <see cref="LocatorEventType.SelectSourceTable"/>, client code executes the statement as the following
    /// <code>
    ///  select * from ({SelectSql.Text}) X where {UserWhere}
    /// </code>
    /// </para>
    /// </summary>
    public SelectSql SelectSql { get; init; }
    /// <summary>
    /// The WHERE clause constructed using the search term provided by the user.
    /// <para>It should be executed as the following
    /// <code>
    ///  select * from ({SelectSql.Text}) X where {UserWhere}
    /// </code>
    /// </para>
    /// </summary>
    public string UserWhere { get; init; }
    /// <summary>
    /// The filter to apply to SourceTable.DefaultView.RowFilter 
    /// </summary>
    public string SourceTableFilter { get; set; }
 
}