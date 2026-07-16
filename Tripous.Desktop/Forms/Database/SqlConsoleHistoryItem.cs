/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Represents an SQL text entry in the SQL console history.
/// </summary>
public class SqlConsoleHistoryItem
{
    // ● private fields
    /// <summary>
    /// The parsed SQL statements.
    /// </summary>
    readonly List<SqlConsoleStatementItem> fSqlStatements = new();

    // ● private
    static bool IsCommentLine(string Line)
    {
        Line = Line.TrimStart();
        return Line.StartsWith("--") || Line.StartsWith("//") || Line.StartsWith("##");
    }
    static bool IsStatementStart(string Line)
    {
        if (string.IsNullOrWhiteSpace(Line) || IsCommentLine(Line))
            return false;
        Match Match = Regex.Match(Line.TrimStart(), @"^([A-Za-z_][A-Za-z0-9_]*)");
        if (!Match.Success)
            return false;
        string Name = Match.Groups[1].Value.ToLowerInvariant();
        return StatementNames.Contains(Name);
    }
    void ParseStatements()
    {
        StringBuilder SB = new();
        foreach (string Line in SqlText.Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            if (string.IsNullOrWhiteSpace(Line) || IsCommentLine(Line))
                continue;
            if (IsStatementStart(Line) && SB.Length > 0)
            {
                fSqlStatements.Add(new SqlConsoleStatementItem(SB.ToString().Trim()));
                SB.Clear();
            }
            SB.AppendLine(Line);
        }
        if (SB.Length > 0)
            fSqlStatements.Add(new SqlConsoleStatementItem(SB.ToString().Trim()));
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="SqlText">The SQL text.</param>
    public SqlConsoleHistoryItem(string SqlText)
    {
        this.SqlText = SqlText ?? string.Empty;
        ParseStatements();
    }

    // ● properties
    /// <summary>
    /// Gets the original SQL text.
    /// </summary>
    public string SqlText { get; }
    /// <summary>
    /// Gets the parsed SQL statements.
    /// </summary>
    public IReadOnlyList<SqlConsoleStatementItem> SqlStatements => fSqlStatements;
    /// <summary>
    /// Gets the known statement names.
    /// </summary>
    static public string[] StatementNames { get; } = ["select", "execute", "exec", "insert", "update", "delete", "create", "alter", "drop", "truncate"];
}
