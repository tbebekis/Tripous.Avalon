/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Keeps SQL console text history and navigation state.
/// </summary>
public class SqlConsoleHistory
{
    // ● private fields
    /// <summary>
    /// The history items.
    /// </summary>
    readonly List<SqlConsoleHistoryItem> fItems = new();
    /// <summary>
    /// The current history index.
    /// </summary>
    int fIndex = -1;
    /// <summary>
    /// The current SQL text.
    /// </summary>
    string fCurrentSqlText = string.Empty;

    // ● private
    void SetCurrentSqlText(string Value)
    {
        fCurrentSqlText = Value ?? string.Empty;
        CurrentSqlTextChanged?.Invoke(this, EventArgs.Empty);
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SqlConsoleHistory()
    {
    }

    // ● public
    /// <summary>
    /// Adds an SQL text to the history.
    /// </summary>
    /// <param name="SqlText">The SQL text.</param>
    public void Add(string SqlText)
    {
        if (string.IsNullOrWhiteSpace(SqlText))
            return;
        if (fItems.Count == 0 || !fItems[fItems.Count - 1].SqlText.IsSameText(SqlText))
            fItems.Add(new SqlConsoleHistoryItem(SqlText));
        fIndex = fItems.Count - 1;
        SetCurrentSqlText(SqlText);
    }
    /// <summary>
    /// Moves to the previous SQL text in history.
    /// </summary>
    public void Prior()
    {
        if (fItems.Count == 0)
            return;
        if (fIndex > 0)
            fIndex--;
        SetCurrentSqlText(Current?.SqlText);
    }
    /// <summary>
    /// Moves to the next SQL text in history.
    /// </summary>
    public void Next()
    {
        if (fItems.Count == 0)
            return;
        if (fIndex < fItems.Count - 1)
            fIndex++;
        SetCurrentSqlText(Current?.SqlText);
    }

    // ● properties
    /// <summary>
    /// Gets the current history item.
    /// </summary>
    public SqlConsoleHistoryItem Current => fIndex >= 0 && fIndex < fItems.Count ? fItems[fIndex] : null;
    /// <summary>
    /// True when the current item is the first history item.
    /// </summary>
    public bool Bof => fItems.Count == 0 || fIndex <= 0;
    /// <summary>
    /// True when the current item is the last history item.
    /// </summary>
    public bool Eof => fItems.Count == 0 || fIndex >= fItems.Count - 1;
    /// <summary>
    /// Gets the current SQL text.
    /// </summary>
    public string CurrentSqlText => fCurrentSqlText;

    // ● events
    /// <summary>
    /// Occurs when the current SQL text changes.
    /// </summary>
    public event EventHandler CurrentSqlTextChanged;
}
