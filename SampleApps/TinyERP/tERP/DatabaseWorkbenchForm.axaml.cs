/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// tERP database workbench form.
/// </summary>
public partial class DatabaseWorkbenchForm : AppForm
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
            $"{Texts.L("ConfirmNonSelectSqlExecution", "You are about to execute a non-SELECT SQL statement.")}: {Statement.StatementName.ToUpperInvariant()}.{Environment.NewLine}{Environment.NewLine}" +
            $"{Texts.L("NonSelectSqlMayChangeData", "This may change data or database structure. Continue only if you accept responsibility for the result.")}{Environment.NewLine}{Environment.NewLine}" +
            Texts.L("DisableSqlWarningFromSettings", "You can disable this warning from Application Settings by changing ShowWarningOnExecStatements.");
        return await MessageBox.YesNo(Message, this);
    }

    // ● protected
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        TitleText = Texts.L("DatabaseWorkbench", "Database Workbench");
        Explorer.Options = new DbConnectionExplorerOptions
        {
            AllowAddConnections = false,
            AllowEditConnections = false,
            AllowDeleteConnections = false,
            AllowCreateDatabases = false,
            PersistConnectionChanges = false,
            ShowToolBar = true
        };
        Explorer.SetConnections(Db.Connections.List);
        Explorer.ConnectionSelected += (Sender, Args) => SqlConsole.SetConnection(Args.ConnectionInfo);
        Explorer.OpenSqlRequested += (Sender, Args) => SqlConsole.SetConnection(Args.ConnectionInfo);
        Explorer.SqlTextRequested += (Sender, Args) =>
        {
            SqlConsole.SetConnection(Args.ConnectionInfo);
            SqlConsole.ShowSqlText(Args.SqlText);
        };
        SqlConsole.ConfirmExecStatementAsync = ConfirmExecStatement;
        SqlConsole.CloseRequested += (Sender, Args) => CloseForm();
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public DatabaseWorkbenchForm()
    {
        InitializeComponent();
    }
}
