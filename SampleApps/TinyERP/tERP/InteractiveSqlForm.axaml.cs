/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Desktop interactive SQL form for a single database connection.
/// </summary>
public partial class InteractiveSqlForm : AppForm
{
    // ● private
    bool ShowWarningOnExecStatements()
    {
        string Value = Config.GetValue(Config.SShowWarningOnExecStatements);
        return Sys.AsBoolean(Value, true);
    }
    async Task<bool> ConfirmExecStatement(SqlConsoleStatementItem Statement)
    {
        if (!ShowWarningOnExecStatements())
            return true;
        string Message =
            $"You are about to execute a non-SELECT SQL statement: {Statement.StatementName.ToUpperInvariant()}.{Environment.NewLine}{Environment.NewLine}" +
            "This may change data or database structure. Continue only if you accept responsibility for the result." +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"You can disable this warning from Application Settings by changing {Config.SShowWarningOnExecStatements}.";
        return await MessageBox.YesNo(Message, this);
    }

    // ● protected
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        DbConnectionInfo ConnectionInfo = Context.Tag as DbConnectionInfo;
        if (ConnectionInfo != null)
        {
            TitleText = "Interactive SQL - " + ConnectionInfo.Name;
            SqlConsole.SetConnection(ConnectionInfo);
        }
        if (Context.Params.TryGetValue("SqlText", out object Value) && Value is string SqlText)
            SqlConsole.ShowSqlText(SqlText);
        SqlConsole.ConfirmExecStatementAsync = ConfirmExecStatement;
        SqlConsole.CloseRequested += (Sender, Args) => CloseForm();
        SqlConsole.LogMessage += (Sender, Text) => AppHost.Log(Text);
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public InteractiveSqlForm()
    {
        InitializeComponent();
    }
}
