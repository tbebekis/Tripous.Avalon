/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Interactive SQL editor and result viewer control.
/// </summary>
public partial class SqlConsoleControl : UserControl
{
    // ● private fields
    /// <summary>
    /// The toolbar helper.
    /// </summary>
    ToolBar fToolBar;
    /// <summary>
    /// The SQL text history.
    /// </summary>
    SqlConsoleHistory fHistory;
    /// <summary>
    /// The active SQL store.
    /// </summary>
    SqlStore fStore;
    /// <summary>
    /// Select result counter.
    /// </summary>
    int fSelectCounter;
    /// <summary>
    /// Executed statement counter.
    /// </summary>
    int fStatementCounter;
    /// <summary>
    /// True while SQL text is executing.
    /// </summary>
    bool fExecuting;

    // ● private
    void CreateToolBar()
    {
        fToolBar = new ToolBar();
        fToolBar.Panel = pnlToolBar;
        fToolBar.AddButton("arrow_left.png", "Previous", () =>
        {
            fHistory.Prior();
            UpdateToolBar();
        });
        fToolBar.AddButton("arrow_right.png", "Next", () =>
        {
            fHistory.Next();
            UpdateToolBar();
        });
        fToolBar.AddButton("lightning.png", "Execute (F5)", async () => await Execute());
        fToolBar.AddButton("door_out.png", "Close", () => CloseRequested?.Invoke(this, EventArgs.Empty));
        UpdateToolBar();
    }
    void UpdateToolBar()
    {
        if (fToolBar?.Panel == null || fToolBar.Panel.Children.Count < 2)
            return;
        if (fToolBar.Panel.Children[0] is Button PriorButton)
            PriorButton.IsEnabled = !fHistory.Bof;
        if (fToolBar.Panel.Children[1] is Button NextButton)
            NextButton.IsEnabled = !fHistory.Eof;
    }
    void AppendLog(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;
        edtLog.Text += Text + Environment.NewLine;
        edtLog.CaretIndex = edtLog.Text.Length;
        LogMessage?.Invoke(this, Text);
    }
    async Task<bool> ConfirmStatement(SqlConsoleStatementItem Statement)
    {
        if (ConfirmExecStatementAsync != null)
            return await ConfirmExecStatementAsync(Statement);
        return true;
    }
    async Task ExecuteStatement(SqlConsoleStatementItem Statement)
    {
        fStatementCounter++;
        if (Statement.IsSelect)
        {
            fSelectCounter++;
            MemTable Table = await Task.Run(() => fStore.Select(Statement.SqlText));
            GroupGrid Grid = CreateResultGrid(Table);
            TabItem Page = new TabItem
            {
                Header = "Result " + fSelectCounter.ToString(CultureInfo.InvariantCulture),
                Content = Grid
            };
            pagerResults.Items.Add(Page);
            pagerResults.SelectedItem = Page;
            AppendLog($@"Statement {fStatementCounter} successfully executed.
Returned rows: {Table.Rows.Count}
SQL: {Statement.SqlText.Trim()}
");
        }
        else
        {
            if (!await ConfirmStatement(Statement))
            {
                AppendLog($"Statement {fStatementCounter}: canceled.");
                return;
            }
            int AffectedRows = await Task.Run(() => fStore.ExecSql(Statement.SqlText));
            AppendLog($@"Statement {fStatementCounter} successfully executed.
Affected rows: {AffectedRows}
SQL: {Statement.SqlText.Trim()}
");
        }
    }
    GroupGrid CreateResultGrid(MemTable Table)
    {
        GroupGrid Result = new GroupGrid();
        Result.IsReadOnly = true;
        Result.IsToolBarVisible = true;
        Result.IsGroupPanelVisible = false;
        Result.IsFilterPanelVisible = true;
        Result.IsTotalsSummaryVisible = false;
        GroupGridBinder.BindGrid(Result, Table.DataView, true);
        return Result;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SqlConsoleControl()
    {
        InitializeComponent();
        fHistory = new SqlConsoleHistory();
        fHistory.CurrentSqlTextChanged += (Sender, Args) => edtSql.Text = fHistory.CurrentSqlText;
        edtSql.KeyDown += async (Sender, Args) =>
        {
            if (Args.Key == Key.F5)
            {
                Args.Handled = true;
                await Execute();
            }
        };
        CreateToolBar();
    }

    // ● public
    /// <summary>
    /// Sets the active connection.
    /// </summary>
    /// <param name="ConnectionInfo">The active connection.</param>
    public void SetConnection(DbConnectionInfo ConnectionInfo)
    {
        if (ReferenceEquals(this.ConnectionInfo, ConnectionInfo))
            return;
        if (this.ConnectionInfo != null && ConnectionInfo != null && this.ConnectionInfo.Name.IsSameText(ConnectionInfo.Name))
            return;
        this.ConnectionInfo = ConnectionInfo;
        fStore = ConnectionInfo != null ? new SqlStore(ConnectionInfo) : null;
        AppendLog(ConnectionInfo != null ? $"Active connection changed to: {ConnectionInfo.Name}" : "Active connection cleared.");
    }
    /// <summary>
    /// Shows SQL text in the editor.
    /// </summary>
    /// <param name="SqlText">The SQL text.</param>
    public void ShowSqlText(string SqlText)
    {
        edtSql.Text = SqlText ?? string.Empty;
        edtSql.Focus();
    }
    /// <summary>
    /// Executes the editor SQL text.
    /// </summary>
    public async Task Execute()
    {
        if (fExecuting)
            return;
        if (ConnectionInfo == null || fStore == null)
        {
            AppendLog("No connection selected.");
            return;
        }
        string SqlText = edtSql.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(SqlText))
            return;
        fExecuting = true;
        try
        {
            fHistory.Add(SqlText);
            SqlConsoleHistoryItem Item = fHistory.Current;
            if (Item == null || Item.SqlStatements.Count == 0)
                return;
            foreach (SqlConsoleStatementItem Statement in Item.SqlStatements)
                await ExecuteStatement(Statement);
        }
        catch (Exception ex)
        {
            AppendLog(ex.Message);
            await MessageBox.Error(ex.Message, this);
        }
        finally
        {
            fExecuting = false;
            UpdateToolBar();
        }
    }

    // ● properties
    /// <summary>
    /// Gets the active connection.
    /// </summary>
    public DbConnectionInfo ConnectionInfo { get; private set; }
    /// <summary>
    /// Gets or sets the non-select statement confirmation callback.
    /// </summary>
    public Func<SqlConsoleStatementItem, Task<bool>> ConfirmExecStatementAsync { get; set; }

    // ● events
    /// <summary>
    /// Occurs when the control writes a log message.
    /// </summary>
    public event EventHandler<string> LogMessage;
    /// <summary>
    /// Occurs when the user requests closing the SQL console.
    /// </summary>
    public event EventHandler CloseRequested;
}
