/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Executes SQL statements for the database workbench.
/// </summary>
[AjaxOperation("App.DatabaseWorkbench.ExecuteSql")]
public class DatabaseWorkbenchExecuteSql: AppAjaxOperation
{
    // ● private
    static readonly string[] StatementNames = ["select", "execute", "exec", "insert", "update", "delete", "create", "alter", "drop", "truncate"];

    static bool IsCommentLine(string Line)
    {
        Line = Line.TrimStart();
        return Line.StartsWith("--") || Line.StartsWith("//") || Line.StartsWith("##");
    }
    static string GetStatementName(string SqlText)
    {
        if (string.IsNullOrWhiteSpace(SqlText))
            return string.Empty;
        Match Match = Regex.Match(SqlText.TrimStart(), @"^([A-Za-z_][A-Za-z0-9_]*)");
        return Match.Success ? Match.Groups[1].Value.ToLowerInvariant() : string.Empty;
    }
    static bool IsStatementStart(string Line)
    {
        if (string.IsNullOrWhiteSpace(Line) || IsCommentLine(Line))
            return false;
        return StatementNames.Contains(GetStatementName(Line));
    }
    static List<string> ParseStatements(string SqlText)
    {
        List<string> Result = new();
        StringBuilder SB = new();
        foreach (string Line in (SqlText ?? string.Empty).Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            if (string.IsNullOrWhiteSpace(Line) || IsCommentLine(Line))
                continue;
            if (IsStatementStart(Line) && SB.Length > 0)
            {
                Result.Add(SB.ToString().Trim());
                SB.Clear();
            }
            SB.AppendLine(Line);
        }
        if (SB.Length > 0)
            Result.Add(SB.ToString().Trim());
        return Result;
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        string ConnectionName = GetStringParam(Request, "ConnectionName");
        string SqlText = GetStringParam(Request, "SqlText");
        if (string.IsNullOrWhiteSpace(ConnectionName))
            Sys.Throw("No connection specified.");
        if (string.IsNullOrWhiteSpace(SqlText))
            Sys.Throw("No SQL statement specified.");

        DbConnectionInfo ConnectionInfo = Db.Connections.Get(ConnectionName);
        SqlStore Store = new(ConnectionInfo);
        List<object> Results = new();
        int StatementCounter = 0;
        int SelectCounter = 0;

        foreach (string StatementText in ParseStatements(SqlText))
        {
            StatementCounter++;
            string StatementName = GetStatementName(StatementText);
            bool IsSelect = StatementName.IsSameText("select");
            if (IsSelect)
            {
                SelectCounter++;
                DataTable Table = Store.Select(StatementText);
                Results.Add(new
                {
                    Type = "Select",
                    StatementCounter,
                    SelectCounter,
                    StatementName,
                    SqlText = StatementText,
                    RowCount = Table.Rows.Count,
                    Table = new JsonDataTable(Table)
                });
            }
            else
            {
                int AffectedRows = Store.ExecSql(StatementText);
                Results.Add(new
                {
                    Type = "Exec",
                    StatementCounter,
                    StatementName,
                    SqlText = StatementText,
                    AffectedRows
                });
            }
        }

        AjaxResponse Result = new(Request.OperationName);
        Result["Results"] = Results;
        return Result;
    }
}
