/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● interactive SQL form
/**
 * Displays an interactive SQL editor for a single database connection.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 * - CloseRequested
 */
app.InteractiveSqlForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates an interactive SQL form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * SQL toolbar.
         * @type {tp.ToolBar|null}
         */
        this.ToolBar = null;
        /**
         * SQL code editor.
         * @type {tp.CodeEditor|null}
         */
        this.Editor = null;
        /**
         * Promise resolved when editor creation completes.
         * @type {Promise|null}
         */
        this.EditorReady = null;
        /**
         * Results tab control.
         * @type {tp.TabControl|null}
         */
        this.ResultsTabControl = null;
        /**
         * Log text area.
         * @type {HTMLTextAreaElement|null}
         */
        this.LogElement = null;
        /**
         * Active connection name.
         * @type {string}
         */
        this.ConnectionName = "";
        /**
         * Initial SQL text.
         * @type {string}
         */
        this.InitialSqlText = "";
        /**
         * True to warn before non-select statements.
         * @type {boolean}
         */
        this.ShowWarningOnExecStatements = true;
        /**
         * SQL history list.
         * @type {string[]}
         */
        this.SqlHistory = [];
        /**
         * Current history index.
         * @type {number}
         */
        this.SqlHistoryIndex = -1;
        /**
         * Select result counter.
         * @type {number}
         */
        this.SelectCounter = 0;
        /**
         * Bound document keydown handler.
         * @type {Function}
         */
        this.fKeyDownHandler = this.FuncBind(this.HandleDocumentKeyDown);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, "app-interactive-sql");
    }
    /**
     * Notification called after field initialization.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
        document.addEventListener("keydown", this.fKeyDownHandler, true);
    }
    /**
     * Releases resources.
     * @returns {void}
     */
    DoDispose() {
        document.removeEventListener("keydown", this.fKeyDownHandler, true);
        this.fKeyDownHandler = null;
        super.DoDispose();
    }
    /**
     * Called just after the context is assigned.
     * @returns {void}
     */
    SetupContext() {
        var Options;
        super.SetupContext();
        Options = this.Context instanceof tp.WebFormContext ? this.Context.Options : null;
        if (Options) {
            if (!tp.IsNil(Options.ConnectionName))
                this.ConnectionName = String(Options.ConnectionName);
            if (!tp.IsNil(Options.InitialSqlText))
                this.InitialSqlText = String(Options.InitialSqlText);
            if (!tp.IsNil(Options.Title))
                this.TitleText = String(Options.Title);
        }
    }
    /**
     * Creates form controls.
     * @returns {void}
     */
    CreateControls() {
        this.CreateToolBar();
        this.EditorReady = this.CreateEditorAsync();
        this.CreateResults();
    }
    /**
     * Creates the SQL toolbar.
     * @returns {void}
     */
    CreateToolBar() {
        var Element = this.Handle.querySelector("[data-role='toolbar']");
        var Button;
        if (!(Element instanceof HTMLElement))
            return;
        this.ToolBar = new tp.ToolBar(Element);
        tp.AddClass(this.ToolBar.Handle, "app-interactive-sql-toolbar");
        Button = this.ToolBar.AddButton("Prior", tp._L("Previous", "Previous"), tp._L("Previous", "Previous"), "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "arrow_left.png" });
        Button = this.ToolBar.AddButton("Next", tp._L("Next", "Next"), tp._L("Next", "Next"), "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "arrow_right.png" });
        Button = this.ToolBar.AddButton("Execute", tp._L("ExecuteF5", "Execute (F5)"), tp._L("ExecuteF5", "Execute (F5)"), "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "lightning.png" });
        Button = this.ToolBar.AddButton("Close", tp._L("Close", "Close"), tp._L("Close", "Close"), "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "door_out.png" });
        this.ToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
    }
    /**
     * Creates the SQL editor.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateEditorAsync() {
        var Element = this.Handle.querySelector("[data-role='editor']");
        if (!(Element instanceof HTMLElement))
            return;
        this.Editor = await tp.CodeEditor.CreateAsync({
            ElementOrSelector: Element,
            Mode: "sql",
            Theme: "chrome",
            FontSize: 14,
            ShowPrintMargin: false
        });
        if (!tp.IsBlankString(this.InitialSqlText))
            this.Editor.Text = this.InitialSqlText;
    }
    /**
     * Creates the result tabs and log page.
     * @returns {void}
     */
    CreateResults() {
        var Element = this.Handle.querySelector("[data-role='results']");
        var Page;
        if (!(Element instanceof HTMLElement))
            return;
        this.ResultsTabControl = new tp.TabControl(Element);
        Page = this.ResultsTabControl.AddPage(tp._L("Log", "Log"));
        this.LogElement = this.Document.createElement("textarea");
        this.LogElement.className = "app-interactive-sql-log";
        this.LogElement.readOnly = true;
        Page.Handle.appendChild(this.LogElement);
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "Prior")
            this.ShowPriorSql();
        else if (Args.Command === "Next")
            this.ShowNextSql();
        else if (Args.Command === "Execute")
            this.ExecuteSql();
        else if (Args.Command === "Close")
            this.Close();
    }
    /**
     * Handles document key down events.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleDocumentKeyDown(e) {
        if (!e || e.key !== "F5" || this.IsActivePage() !== true)
            return;
        e.preventDefault();
        e.stopPropagation();
        this.ExecuteSql().catch(function (ex) {
            if (tp.LogBox)
                tp.LogBox.AppendLine(tp._L("SqlExecutionFailed", "SQL execution failed") + ": " + tp.ExceptionText(ex));
        });
    }
    /**
     * Returns true when this form is the selected workspace page.
     * @returns {boolean} Returns true when active.
     */
    IsActivePage() {
        var Page = this.ParentControl;
        var Handler = Page ? Page.AppPageHandler : null;
        var TabControl = Handler ? Handler.TabControl : null;
        return Page instanceof tp.TabPage && TabControl instanceof tp.TabControl && TabControl.SelectedPage === Page;
    }
    /**
     * Focuses the SQL editor.
     * @returns {void}
     */
    FocusEditor() {
        if (this.Editor && this.Editor.Handle)
            this.Editor.Handle.focus();
    }
    /**
     * Gets editor text.
     * @returns {string} Returns editor text.
     */
    GetEditorText() {
        return this.Editor ? this.Editor.Text || "" : "";
    }
    /**
     * Sets editor text.
     * @param {string} Text The editor text.
     * @returns {void}
     */
    SetEditorText(Text) {
        if (this.Editor)
            this.Editor.Text = Text || "";
    }
    /**
     * Adds SQL text to history.
     * @param {string} SqlText The SQL text.
     * @returns {void}
     */
    AddSqlHistory(SqlText) {
        if (tp.IsBlankString(SqlText))
            return;
        if (this.SqlHistory.length === 0 || !tp.IsSameText(this.SqlHistory[this.SqlHistory.length - 1], SqlText))
            this.SqlHistory.push(SqlText);
        this.SqlHistoryIndex = this.SqlHistory.length - 1;
    }
    /**
     * Shows previous SQL text.
     * @returns {void}
     */
    ShowPriorSql() {
        if (this.SqlHistory.length === 0)
            return;
        if (this.SqlHistoryIndex > 0)
            this.SqlHistoryIndex--;
        this.SetEditorText(this.SqlHistory[this.SqlHistoryIndex]);
    }
    /**
     * Shows next SQL text.
     * @returns {void}
     */
    ShowNextSql() {
        if (this.SqlHistory.length === 0)
            return;
        if (this.SqlHistoryIndex < this.SqlHistory.length - 1)
            this.SqlHistoryIndex++;
        this.SetEditorText(this.SqlHistory[this.SqlHistoryIndex]);
    }
    /**
     * Returns true when text contains non-select statements.
     * @param {string} SqlText The SQL text.
     * @returns {boolean} Returns true when non-select statements are detected.
     */
    HasExecStatements(SqlText) {
        var Lines = String(SqlText || "").split(/\r?\n/);
        var Index;
        var Text;
        var Match;
        for (Index = 0; Index < Lines.length; Index++) {
            Text = Lines[Index].trim();
            if (Text === "" || Text.indexOf("--") === 0 || Text.indexOf("//") === 0 || Text.indexOf("##") === 0)
                continue;
            Match = /^([A-Za-z_][A-Za-z0-9_]*)/.exec(Text);
            if (Match && !tp.IsSameText(Match[1], "select"))
                return true;
        }
        return false;
    }
    /**
     * Confirms non-select statement execution.
     * @param {string} SqlText The SQL text.
     * @returns {Promise<boolean>} Returns true to continue.
     */
    async ConfirmExecStatements(SqlText) {
        var Message;
        if (this.ShowWarningOnExecStatements !== true || this.HasExecStatements(SqlText) !== true)
            return true;
        Message = tp._L("ConfirmNonSelectSqlExecution", "You are about to execute a non-SELECT SQL statement.") + "\n\n" +
            tp._L("NonSelectSqlMayChangeData", "This may change data or database structure. Continue only if you accept responsibility for the result.") + "\n\n" +
            tp._L("DisableSqlWarningFromSettings", "You can disable this warning from Application Settings by changing ShowWarningOnExecStatements.");
        return await tp.YesNoBoxAsync(Message);
    }
    /**
     * Handles an execution result packet.
     * @param {object} Result The result packet.
     * @returns {void}
     */
    HandleExecResult(Result) {
        if (!Result)
            return;
        if (Result.Type === "Select")
            this.AddResultGrid(Result);
        else
            this.AppendLog(tp._L("Statement", "Statement") + " " + Result.StatementCounter + " " + tp._L("SuccessfullyExecuted", "successfully executed") + ".\n" + tp._L("AffectedRows", "Affected rows") + ": " + Result.AffectedRows + "\nSQL: " + Result.SqlText + "\n");
    }
    /**
     * Adds a select result grid.
     * @param {object} Result The result packet.
     * @returns {void}
     */
    AddResultGrid(Result) {
        var Page;
        var GridElement;
        var Grid;
        var Table = new tp.DataTable(Result.Table);
        this.SelectCounter++;
        Page = this.ResultsTabControl.AddPage(tp._L("Result", "Result") + " " + this.SelectCounter);
        GridElement = this.Document.createElement("div");
        GridElement.className = "app-interactive-sql-result-grid";
        Page.Handle.appendChild(GridElement);
        Grid = new tp.Grid({
            ElementOrSelector: GridElement,
            ReadOnly: true,
            AutoGenerateColumns: true,
            ToolBarVisible: false,
            GroupsVisible: true,
            FilterVisible: true,
            FooterVisible: true,
            DataSource: Table
        });
        setTimeout(function () {
            if (!Grid.IsDisposed && tp.IsFunction(Grid.BestFitColumns))
                Grid.BestFitColumns();
        }, 0);
        this.AppendLog(tp._L("Statement", "Statement") + " " + Result.StatementCounter + " " + tp._L("SuccessfullyExecuted", "successfully executed") + ".\n" + tp._L("ReturnedRows", "Returned rows") + ": " + Result.RowCount + "\nSQL: " + Result.SqlText + "\n");
    }
    /**
     * Appends a log line.
     * @param {string} Text The text.
     * @returns {void}
     */
    AppendLog(Text) {
        if (!(this.LogElement instanceof HTMLTextAreaElement) || tp.IsBlankString(Text))
            return;
        this.LogElement.value += Text + "\n";
        this.LogElement.scrollTop = this.LogElement.scrollHeight;
    }

    // ● public
    /**
     * Loads form data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
        var Packet;
        await this.EditorReady;
        Packet = await tp.AjaxRequest.ExecuteAsync("App.DatabaseExplorer.GetConnections");
        this.ShowWarningOnExecStatements = Packet ? Packet.ShowWarningOnExecStatements === true : true;
        if (!tp.IsBlankString(this.InitialSqlText))
            this.SetEditorText(this.InitialSqlText);
        this.AppendLog(tp._L("Connection", "Connection") + ": " + this.ConnectionName);
        this.FocusEditor();
    }
    /**
     * Executes SQL text.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ExecuteSql() {
        var SqlText = this.GetEditorText();
        var Packet;
        var Results;
        var Index;
        if (tp.IsBlankString(this.ConnectionName)) {
            this.AppendLog(tp._L("NoConnectionSelected", "No connection selected."));
            return;
        }
        if (tp.IsBlankString(SqlText))
            return;
        if (await this.ConfirmExecStatements(SqlText) !== true)
            return;
        this.AddSqlHistory(SqlText);
        Packet = await tp.AjaxRequest.ExecuteAsync("App.DatabaseWorkbench.ExecuteSql", {
            ConnectionName: this.ConnectionName,
            SqlText: SqlText
        });
        Results = Packet && tp.IsArray(Packet.Results) ? Packet.Results : [];
        for (Index = 0; Index < Results.length; Index++)
            this.HandleExecResult(Results[Index]);
    }
};

// ● prototype
/**
 * SQL toolbar.
 * @type {tp.ToolBar|null}
 */
app.InteractiveSqlForm.prototype.ToolBar = null;
/**
 * SQL code editor.
 * @type {tp.CodeEditor|null}
 */
app.InteractiveSqlForm.prototype.Editor = null;
/**
 * Promise resolved when editor creation completes.
 * @type {Promise|null}
 */
app.InteractiveSqlForm.prototype.EditorReady = null;
/**
 * Results tab control.
 * @type {tp.TabControl|null}
 */
app.InteractiveSqlForm.prototype.ResultsTabControl = null;
/**
 * Log text area.
 * @type {HTMLTextAreaElement|null}
 */
app.InteractiveSqlForm.prototype.LogElement = null;
/**
 * Active connection name.
 * @type {string}
 */
app.InteractiveSqlForm.prototype.ConnectionName = "";
/**
 * Initial SQL text.
 * @type {string}
 */
app.InteractiveSqlForm.prototype.InitialSqlText = "";
/**
 * True to warn before non-select statements.
 * @type {boolean}
 */
app.InteractiveSqlForm.prototype.ShowWarningOnExecStatements = true;
/**
 * SQL history list.
 * @type {string[]|null}
 */
app.InteractiveSqlForm.prototype.SqlHistory = null;
/**
 * Current history index.
 * @type {number}
 */
app.InteractiveSqlForm.prototype.SqlHistoryIndex = -1;
/**
 * Select result counter.
 * @type {number}
 */
app.InteractiveSqlForm.prototype.SelectCounter = 0;
/**
 * Bound document keydown handler.
 * @type {Function|null}
 */
app.InteractiveSqlForm.prototype.fKeyDownHandler = null;
