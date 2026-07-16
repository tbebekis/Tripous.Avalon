/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● database workbench form
/**
 * Displays a database explorer and interactive SQL console.
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
app.DatabaseWorkbenchForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates a database workbench form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, "app-database-workbench");
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * Explorer toolbar.
         * @type {tp.ToolBar|null}
         */
        this.ExplorerToolBar = null;
        /**
         * SQL toolbar.
         * @type {tp.ToolBar|null}
         */
        this.SqlToolBar = null;
        /**
         * Schema tree view.
         * @type {tp.TreeView|null}
         */
        this.TreeView = null;
        /**
         * Selected tree node.
         * @type {tp.TreeNode|null}
         */
        this.SelectedNode = null;
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
         * Available connection packets.
         * @type {object[]}
         */
        this.Connections = [];
        /**
         * Schema packets keyed by connection name.
         * @type {object}
         */
        this.SchemaMap = {};
        /**
         * Active connection name.
         * @type {string}
         */
        this.ActiveConnectionName = "";
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
     * Creates form controls.
     * @returns {void}
     */
    CreateControls() {
        this.CreateExplorerToolBar();
        this.CreateSqlToolBar();
        this.CreateTreeView();
        this.EditorReady = this.CreateEditorAsync();
        this.CreateResults();
    }
    /**
     * Creates the explorer toolbar.
     * @returns {void}
     */
    CreateExplorerToolBar() {
        var Element = this.Handle.querySelector("[data-role='explorer-toolbar']");
        var Button;
        if (!(Element instanceof HTMLElement))
            return;
        this.ExplorerToolBar = new tp.ToolBar(Element);
        Button = this.ExplorerToolBar.AddButton("SqlEditor", "Interactive Sql", "Interactive Sql", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "script_lightning.png" });
        Button = this.ExplorerToolBar.AddButton("Connect", "Connect", "Connect", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "database_green.png" });
        Button = this.ExplorerToolBar.AddButton("Refresh", "Refresh", "Refresh", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "database_refresh.png" });
        this.ExplorerToolBar.On("ButtonClick", this.HandleExplorerToolBarButtonClick, this);
    }
    /**
     * Creates the SQL toolbar.
     * @returns {void}
     */
    CreateSqlToolBar() {
        var Element = this.Handle.querySelector("[data-role='sql-toolbar']");
        var Button;
        if (!(Element instanceof HTMLElement))
            return;
        this.SqlToolBar = new tp.ToolBar(Element);
        Button = this.SqlToolBar.AddButton("Prior", "Previous", "Previous", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "arrow_left.png" });
        Button = this.SqlToolBar.AddButton("Next", "Next", "Next", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "arrow_right.png" });
        Button = this.SqlToolBar.AddButton("Execute", "Execute (F5)", "Execute (F5)", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "lightning.png" });
        Button = this.SqlToolBar.AddButton("Close", "Close", "Close", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "door_out.png" });
        this.SqlToolBar.On("ButtonClick", this.HandleSqlToolBarButtonClick, this);
    }
    /**
     * Creates the tree view.
     * @returns {void}
     */
    CreateTreeView() {
        var Element = this.Handle.querySelector("[data-role='explorer-tree']");
        if (!(Element instanceof HTMLElement))
            return;
        this.TreeView = new tp.TreeView(Element);
        this.TreeView.On("NodeClick", this.HandleTreeNodeClick, this);
        this.TreeView.On("NodeDoubleClick", this.HandleTreeNodeDoubleClick, this);
    }
    /**
     * Creates the SQL editor.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateEditorAsync() {
        var Element = this.Handle.querySelector("[data-role='sql-editor']");
        if (!(Element instanceof HTMLElement))
            return;
        this.Editor = await tp.CodeEditor.CreateAsync({
            ElementOrSelector: Element,
            Mode: "sql",
            Theme: "chrome",
            FontSize: 14,
            ShowPrintMargin: false
        });
    }
    /**
     * Creates the result tabs and log page.
     * @returns {void}
     */
    CreateResults() {
        var Element = this.Handle.querySelector(".app-database-workbench-results");
        var Page;
        if (!(Element instanceof HTMLElement))
            return;
        this.ResultsTabControl = new tp.TabControl(Element);
        Page = this.ResultsTabControl.AddPage("Log");
        this.LogElement = this.Document.createElement("textarea");
        this.LogElement.className = "app-database-workbench-log";
        this.LogElement.readOnly = true;
        Page.Handle.appendChild(this.LogElement);
    }
    /**
     * Handles explorer toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleExplorerToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "SqlEditor")
            this.FocusEditor();
        else if (Args.Command === "Connect")
            this.LoadSelectedSchema();
        else if (Args.Command === "Refresh")
            this.RefreshSelectedSchema();
    }
    /**
     * Handles SQL toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleSqlToolBarButtonClick(Args) {
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
                tp.LogBox.AppendLine("SQL execution failed: " + tp.ExceptionText(ex));
        });
    }
    /**
     * Returns true when this workbench is the selected workspace page.
     * @returns {boolean} Returns true when active.
     */
    IsActivePage() {
        var Page = this.ParentControl;
        var Handler = Page ? Page.AppPageHandler : null;
        var TabControl = Handler ? Handler.TabControl : null;
        return Page instanceof tp.TabPage && TabControl instanceof tp.TabControl && TabControl.SelectedPage === Page;
    }
    /**
     * Handles tree node clicks.
     * @param {tp.TreeViewEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleTreeNodeClick(Args) {
        var Node = Args ? Args.Node : null;
        if (Node) {
            this.SelectedNode = Node;
            this.SelectConnectionFromNode(Node);
        }
    }
    /**
     * Handles tree node double clicks.
     * @param {tp.TreeViewEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleTreeNodeDoubleClick(Args) {
        var Node = Args ? Args.Node : null;
        if (!Node)
            return;
        this.SelectedNode = Node;
        this.SelectConnectionFromNode(Node);
        if (Node.Tag && (Node.Tag.NodeType === "Table" || Node.Tag.NodeType === "View")) {
            this.SetEditorText(Node.Tag.SelectSql || "");
            this.FocusEditor();
        } else if (Node.Tag && Node.Tag.NodeType === "Connection") {
            this.LoadSchemaNode(Node);
        }
    }
    /**
     * Creates a tree node.
     * @param {tp.TreeView|tp.TreeNode} Parent The parent.
     * @param {string} Text The node text.
     * @param {string} ImageFileName The image file name.
     * @param {object|null|undefined} Tag The node tag.
     * @returns {tp.TreeNode|null} Returns the created node.
     */
    AddTreeNode(Parent, Text, ImageFileName, Tag) {
        var Node;
        if (!(Parent instanceof tp.TreeView || Parent instanceof tp.TreeNode))
            return null;
        Node = Parent.AddNode(Text);
        Node.Tag = Tag || null;
        Node.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: ImageFileName });
        return Node;
    }
    /**
     * Adds a folder node when items exist.
     * @param {tp.TreeNode} Parent The parent node.
     * @param {string} Title The folder title.
     * @param {object[]} Items The item packets.
     * @param {string} NodeType The node type.
     * @returns {void}
     */
    AddItemFolder(Parent, Title, Items, NodeType) {
        var Folder;
        var Index;
        var Item;
        if (!(Parent instanceof tp.TreeNode) || !tp.IsArray(Items) || Items.length === 0)
            return;
        Folder = this.AddTreeNode(Parent, Title, "folder16.png", { NodeType: "Folder" });
        for (Index = 0; Index < Items.length; Index++) {
            Item = Items[Index];
            this.AddTreeNode(Folder, Item.DisplayText || Item.Name, "item16.png", {
                NodeType: NodeType,
                Item: Item
            });
        }
    }
    /**
     * Adds table or view columns under a node.
     * @param {tp.TreeNode} Parent The parent node.
     * @param {object[]} Columns The column packets.
     * @returns {void}
     */
    AddColumnFolder(Parent, Columns) {
        this.AddItemFolder(Parent, "Columns", Columns, "Column");
    }
    /**
     * Populates a schema node.
     * @param {tp.TreeNode} RootNode The connection node.
     * @param {object} Schema The schema packet.
     * @returns {void}
     */
    PopulateSchemaNode(RootNode, Schema) {
        var TablesFolder;
        var ViewsFolder;
        var Index;
        var Table;
        var View;
        var Node;
        RootNode.Clear();
        if (tp.IsArray(Schema.Tables) && Schema.Tables.length > 0) {
            TablesFolder = this.AddTreeNode(RootNode, "Tables", "folder16.png", { NodeType: "Folder" });
            for (Index = 0; Index < Schema.Tables.length; Index++) {
                Table = Schema.Tables[Index];
                Node = this.AddTreeNode(TablesFolder, Table.Name, "table.png", {
                    NodeType: "Table",
                    ConnectionName: Schema.ConnectionName,
                    SourceCode: Table.SourceCode,
                    FieldList: Table.FieldList,
                    SelectSql: Table.SelectSql
                });
                this.AddColumnFolder(Node, Table.Columns);
                this.AddItemFolder(Node, "Indexes", Table.Indexes, "Index");
                this.AddItemFolder(Node, "Constraints", Table.Constraints, "Constraint");
                this.AddItemFolder(Node, "Triggers", Table.Triggers, "Trigger");
            }
        }
        if (tp.IsArray(Schema.Views) && Schema.Views.length > 0) {
            ViewsFolder = this.AddTreeNode(RootNode, "Views", "folder16.png", { NodeType: "Folder" });
            for (Index = 0; Index < Schema.Views.length; Index++) {
                View = Schema.Views[Index];
                Node = this.AddTreeNode(ViewsFolder, View.Name, "table.png", {
                    NodeType: "View",
                    ConnectionName: Schema.ConnectionName,
                    SourceCode: View.SourceCode,
                    FieldList: View.FieldList,
                    SelectSql: View.SelectSql
                });
                this.AddColumnFolder(Node, View.Columns);
            }
        }
        RootNode.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "database_green.png" });
        RootNode.Expand();
    }
    /**
     * Loads a schema node.
     * @param {tp.TreeNode} Node The connection node.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadSchemaNode(Node) {
        var ConnectionName;
        var Packet;
        var Schema;
        if (!(Node instanceof tp.TreeNode) || !Node.Tag || Node.Tag.NodeType !== "Connection")
            return;
        ConnectionName = Node.Tag.ConnectionName;
        if (this.SchemaMap[ConnectionName]) {
            this.PopulateSchemaNode(Node, this.SchemaMap[ConnectionName]);
            return;
        }
        Packet = await tp.AjaxRequest.ExecuteAsync("App.DatabaseWorkbench.GetSchema", {
            ConnectionName: ConnectionName
        });
        Schema = Packet ? Packet.Schema : null;
        if (Schema) {
            this.SchemaMap[ConnectionName] = Schema;
            this.PopulateSchemaNode(Node, Schema);
            this.AppendLog("Schema loaded: " + ConnectionName);
        }
    }
    /**
     * Returns the selected tree node.
     * @returns {tp.TreeNode|null} Returns the selected node.
     */
    GetSelectedNode() {
        return this.SelectedNode;
    }
    /**
     * Selects the active connection from a tree node.
     * @param {tp.TreeNode} Node The tree node.
     * @returns {void}
     */
    SelectConnectionFromNode(Node) {
        var Current = Node;
        while (Current instanceof tp.TreeNode) {
            if (Current.Tag && Current.Tag.ConnectionName) {
                this.SetActiveConnection(Current.Tag.ConnectionName);
                return;
            }
            Current = Current.ParentTreeNode;
        }
    }
    /**
     * Loads the selected schema.
     * @returns {void}
     */
    LoadSelectedSchema() {
        var Node = this.GetSelectedNode();
        while (Node instanceof tp.TreeNode && (!Node.Tag || Node.Tag.NodeType !== "Connection"))
            Node = Node.ParentTreeNode;
        if (Node instanceof tp.TreeNode)
            this.LoadSchemaNode(Node);
    }
    /**
     * Refreshes the selected schema.
     * @returns {void}
     */
    RefreshSelectedSchema() {
        var Node = this.GetSelectedNode();
        var ConnectionName;
        while (Node instanceof tp.TreeNode && (!Node.Tag || Node.Tag.NodeType !== "Connection"))
            Node = Node.ParentTreeNode;
        if (!(Node instanceof tp.TreeNode))
            return;
        ConnectionName = Node.Tag.ConnectionName;
        delete this.SchemaMap[ConnectionName];
        Node.Clear();
        Node.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "database.png" });
        this.LoadSchemaNode(Node);
    }
    /**
     * Sets the active connection.
     * @param {string} ConnectionName The connection name.
     * @returns {void}
     */
    SetActiveConnection(ConnectionName) {
        if (tp.IsBlankString(ConnectionName) || ConnectionName === this.ActiveConnectionName)
            return;
        this.ActiveConnectionName = ConnectionName;
        this.AppendLog("Active connection changed to: " + ConnectionName);
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
        Message = "You are about to execute a non-SELECT SQL statement.\n\n" +
            "This may change data or database structure. Continue only if you accept responsibility for the result.\n\n" +
            "You can disable this warning from Application Settings by changing ShowWarningOnExecStatements.";
        return await tp.YesNoBoxAsync(Message);
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
        if (tp.IsBlankString(this.ActiveConnectionName)) {
            this.AppendLog("No connection selected.");
            return;
        }
        if (tp.IsBlankString(SqlText))
            return;
        if (await this.ConfirmExecStatements(SqlText) !== true)
            return;
        this.AddSqlHistory(SqlText);
        Packet = await tp.AjaxRequest.ExecuteAsync("App.DatabaseWorkbench.ExecuteSql", {
            ConnectionName: this.ActiveConnectionName,
            SqlText: SqlText
        });
        Results = Packet && tp.IsArray(Packet.Results) ? Packet.Results : [];
        for (Index = 0; Index < Results.length; Index++)
            this.HandleExecResult(Results[Index]);
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
            this.AppendLog("Statement " + Result.StatementCounter + " successfully executed.\nAffected rows: " + Result.AffectedRows + "\nSQL: " + Result.SqlText + "\n");
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
        Page = this.ResultsTabControl.AddPage("Result " + this.SelectCounter);
        GridElement = this.Document.createElement("div");
        GridElement.className = "app-database-workbench-result-grid";
        Page.Handle.appendChild(GridElement);
        Grid = new tp.Grid({
            ElementOrSelector: GridElement,
            ReadOnly: true,
            AutoGenerateColumns: true,
            ToolBarVisible: false,
            GroupsVisible: false,
            FilterVisible: true,
            FooterVisible: false,
            DataSource: Table
        });
        setTimeout(function () {
            if (!Grid.IsDisposed && tp.IsFunction(Grid.BestFitColumns))
                Grid.BestFitColumns();
        }, 0);
        this.AppendLog("Statement " + Result.StatementCounter + " successfully executed.\nReturned rows: " + Result.RowCount + "\nSQL: " + Result.SqlText + "\n");
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
     * Loads workbench data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
        var Packet;
        var Index;
        var Connection;
        var Node;
        await this.EditorReady;
        Packet = await tp.AjaxRequest.ExecuteAsync("App.DatabaseWorkbench.GetConnections");
        this.Connections = Packet && tp.IsArray(Packet.Connections) ? Packet.Connections : [];
        this.ShowWarningOnExecStatements = Packet ? Packet.ShowWarningOnExecStatements === true : true;
        this.TreeView.Clear();
        this.SelectedNode = null;
        for (Index = 0; Index < this.Connections.length; Index++) {
            Connection = this.Connections[Index];
            Node = this.AddTreeNode(this.TreeView, Connection.Name, "database.png", {
                NodeType: "Connection",
                ConnectionName: Connection.Name,
                Connection: Connection
            });
            if (Index === 0) {
                this.SelectedNode = Node;
                this.SetActiveConnection(Connection.Name);
            }
        }
    }
};
