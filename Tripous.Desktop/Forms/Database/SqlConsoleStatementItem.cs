/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Represents a parsed SQL statement.
/// </summary>
public class SqlConsoleStatementItem
{
    // ● private
    static string GetStatementName(string SqlText)
    {
        if (string.IsNullOrWhiteSpace(SqlText))
            return string.Empty;
        Match Match = Regex.Match(SqlText.TrimStart(), @"^([A-Za-z_][A-Za-z0-9_]*)");
        return Match.Success ? Match.Groups[1].Value.ToLowerInvariant() : string.Empty;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="SqlText">The SQL text.</param>
    public SqlConsoleStatementItem(string SqlText)
    {
        this.SqlText = SqlText ?? string.Empty;
        StatementName = GetStatementName(this.SqlText);
    }

    // ● properties
    /// <summary>
    /// Gets the SQL text.
    /// </summary>
    public string SqlText { get; }
    /// <summary>
    /// Gets the statement name.
    /// </summary>
    public string StatementName { get; }
    /// <summary>
    /// Returns true when this statement is a SELECT statement.
    /// </summary>
    public bool IsSelect => StatementName.IsSameText("select");
}
