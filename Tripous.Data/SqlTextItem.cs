/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents an SQL statement request.
/// </summary>
public class SqlTextItem
{
    // ● private
    static int Counter;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SqlTextItem()
    {
        Name = NextName();
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public SqlTextItem(string SqlText, string ConnectionName = null, string Name = null)
    {
        this.SqlText = SqlText;
        this.ConnectionName = !string.IsNullOrWhiteSpace(ConnectionName) ? ConnectionName : Sys.DEFAULT;
        this.Name = !string.IsNullOrWhiteSpace(Name) ? Name : NextName();
    }

    // ● static public
    /// <summary>
    /// Returns the next default statement name.
    /// </summary>
    static public string NextName() => "Sql" + Counter++.ToString(CultureInfo.InvariantCulture);

    // ● properties
    /// <summary>
    /// The statement name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The SQL statement text.
    /// </summary>
    public string SqlText { get; set; }
    /// <summary>
    /// The connection name.
    /// </summary>
    public string ConnectionName { get; set; } = Sys.DEFAULT;
}
