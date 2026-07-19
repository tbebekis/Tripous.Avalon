/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● database explorer form
/**
 * Displays database connections and schema metadata in the left sidebar.
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
app.DatabaseExplorerForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates a database explorer form.
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
         * Explorer toolbar.
         * @type {tp.ToolBar|null}
         */
        this.ToolBar = null;
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
         * Available connection packets.
         * @type {object[]}
         */
        this.Connections = [];
        /**
         * Explorer options.
         * @type {object}
         */
        this.Options = {};
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
         * Tree context menu.
         * @type {tp.ContextMenu|null}
         */
        this.mnuTree = null;
        /**
         * Expand menu item.
         * @type {tp.MenuItem|null}
         */
        this.mnuExpand = null;
        /**
         * Collapse menu item.
         * @type {tp.MenuItem|null}
         */
        this.mnuCollapse = null;
        /**
         * Show source code menu item.
         * @type {tp.MenuItem|null}
         */
        this.mnuShowSourceCode = null;
        /**
         * Show field list menu item.
         * @type {tp.MenuItem|null}
         */
        this.mnuShowFieldList = null;
        /**
         * Select table or view menu item.
         * @type {tp.MenuItem|null}
         */
        this.mnuSelectTableOrView = null;
        /**
         * Bound tree context menu handler.
         * @type {Function|null}
         */
        this.fTreeContextMenuHandler = this.HandleTreeContextMenu.bind(this);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, "app-database-explorer");
    }
    /**
     * Notification called after field initialization.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
    }
    /**
     * Releases resources.
     * @returns {void}
     */
    DoDispose() {
        if (this.TreeView && this.fTreeContextMenuHandler)
            this.TreeView.Handle.removeEventListener("contextmenu", this.fTreeContextMenuHandler, false);
        if (this.mnuTree)
            this.mnuTree.Dispose();
        this.fTreeContextMenuHandler = null;
        this.mnuTree = null;
        super.DoDispose();
    }
    /**
     * Creates form controls.
     * @returns {void}
     */
    CreateControls() {
        this.CreateToolBar();
        this.CreateTreeView();
        this.CreateContextMenu();
    }
    /**
     * Creates the explorer toolbar.
     * @returns {void}
     */
    CreateToolBar() {
        var Element = this.Handle.querySelector("[data-role='toolbar']");
        var Button;
        if (!(Element instanceof HTMLElement))
            return;
        this.ToolBar = new tp.ToolBar(Element);
        tp.AddClass(this.ToolBar.Handle, "app-database-explorer-toolbar");
        Button = this.ToolBar.AddButton("SqlEditor", "Interactive SQL", "Interactive SQL", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "script_lightning.png" });
        Button = this.ToolBar.AddButton("Connect", "Connect", "Connect", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "database_green.png" });
        Button = this.ToolBar.AddButton("Refresh", "Refresh", "Refresh", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "database_refresh.png" });
        Button = this.ToolBar.AddButton("Source", "Show Source Code", "Show Source Code", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "table.png" });
        Button = this.ToolBar.AddButton("Fields", "Show Field List", "Show Field List", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "table_select_column.png" });
        Button = this.ToolBar.AddButton("Select", "Select Table Or View", "Select Table Or View", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "lightning.png" });
        this.ToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
    }
    /**
     * Creates the tree view.
     * @returns {void}
     */
    CreateTreeView() {
        var Element = this.Handle.querySelector("[data-role='tree']");
        if (!(Element instanceof HTMLElement))
            return;
        this.TreeView = new tp.TreeView(Element);
        this.TreeView.On("NodeClick", this.HandleTreeNodeClick, this);
        this.TreeView.On("NodeDoubleClick", this.HandleTreeNodeDoubleClick, this);
        this.TreeView.Handle.addEventListener("contextmenu", this.fTreeContextMenuHandler, false);
    }
    /**
     * Creates the tree context menu.
     * @returns {void}
     */
    CreateContextMenu() {
        this.mnuTree = new tp.ContextMenu();
        this.mnuExpand = this.mnuTree.AddMenuItem("Expand", "Expand");
        this.mnuCollapse = this.mnuTree.AddMenuItem("Collapse", "Collapse");
        this.mnuTree.AddSeparator();
        this.mnuShowSourceCode = this.mnuTree.AddMenuItem("Show Source Code", "Source");
        this.mnuShowFieldList = this.mnuTree.AddMenuItem("Show Field List", "Fields");
        this.mnuTree.AddSeparator();
        this.mnuSelectTableOrView = this.mnuTree.AddMenuItem("Select Table Or View", "Select");
        this.mnuTree.On("ItemClick", this.HandleContextMenuItemClick, this);
    }
    /**
     * Handles tree context menu.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleTreeContextMenu(e) {
        var Node;
        var IsTableOrView;
        if (!(this.TreeView instanceof tp.TreeView))
            return;
        Node = this.TreeView.FindNodeFromEventTarget(e.target);
        if (!(Node instanceof tp.TreeNode))
            return;
        this.SelectedNode = Node;
        this.TreeView.SetFocusedNode(Node);
        this.SelectConnectionFromNode(Node);
        IsTableOrView = Node.Tag && (Node.Tag.NodeType === "Table" || Node.Tag.NodeType === "View");
        if (this.mnuExpand)
            this.mnuExpand.Enabled = Node.HasChildren;
        if (this.mnuCollapse)
            this.mnuCollapse.Enabled = Node.HasChildren;
        if (this.mnuShowSourceCode)
            this.mnuShowSourceCode.Enabled = IsTableOrView;
        if (this.mnuShowFieldList)
            this.mnuShowFieldList.Enabled = IsTableOrView;
        if (this.mnuSelectTableOrView)
            this.mnuSelectTableOrView.Enabled = IsTableOrView;
        this.mnuTree.Show(e);
    }
    /**
     * Handles tree context menu item clicks.
     * @param {tp.MenuEventArgs} Args The menu event arguments.
     * @returns {void}
     */
    HandleContextMenuItemClick(Args) {
        var Node = this.GetSelectedNode();
        if (!Args || !(Node instanceof tp.TreeNode))
            return;
        if (Args.Command === "Expand")
            Node.ExpandAll();
        else if (Args.Command === "Collapse")
            Node.CollapseAll();
        else if (Args.Command === "Source")
            this.OpenNodeSql("SourceCode");
        else if (Args.Command === "Fields")
            this.OpenNodeSql("FieldList");
        else if (Args.Command === "Select")
            this.OpenNodeSql("SelectSql");
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "SqlEditor")
            this.OpenSqlEditor();
        else if (Args.Command === "Connect")
            this.LoadSelectedSchema();
        else if (Args.Command === "Refresh")
            this.RefreshSelectedSchema();
        else if (Args.Command === "Source")
            this.OpenNodeSql("SourceCode");
        else if (Args.Command === "Fields")
            this.OpenNodeSql("FieldList");
        else if (Args.Command === "Select")
            this.OpenNodeSql("SelectSql");
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
        if (Node.Tag && (Node.Tag.NodeType === "Table" || Node.Tag.NodeType === "View"))
            this.OpenNodeSql("SelectSql");
        else if (Node.Tag && Node.Tag.NodeType === "Connection")
            this.LoadSchemaNode(Node);
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
            this.Log("Schema loaded: " + ConnectionName);
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
     * Returns the connection node that owns a tree node.
     * @param {tp.TreeNode|null|undefined} Node The starting tree node.
     * @returns {tp.TreeNode|null} Returns the connection node or null.
     */
    FindConnectionNode(Node) {
        while (Node instanceof tp.TreeNode) {
            if (Node.Tag && Node.Tag.NodeType === "Connection")
                return Node;
            Node = Node.ParentTreeNode;
        }
        return null;
    }
    /**
     * Returns true when the connection node schema is loaded.
     * @param {tp.TreeNode|null|undefined} Node The connection node.
     * @returns {boolean} Returns true when loaded.
     */
    IsConnectionNodeLoaded(Node) {
        return Node instanceof tp.TreeNode && Node.Tag && Node.Tag.NodeType === "Connection" && !!this.SchemaMap[Node.Tag.ConnectionName];
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
        this.Log("Active connection changed to: " + ConnectionName);
    }
    /**
     * Opens interactive SQL for the selected connection.
     * @returns {void}
     */
    OpenSqlEditor() {
        var Node = this.FindConnectionNode(this.GetSelectedNode());
        if (!this.IsConnectionNodeLoaded(Node))
            return;
        app.App.OpenInteractiveSql(Node.Tag.ConnectionName, "");
    }
    /**
     * Opens interactive SQL with SQL text from the selected table or view node.
     * @param {string} PropertyName The node tag SQL property name.
     * @returns {void}
     */
    OpenNodeSql(PropertyName) {
        var Node = this.GetSelectedNode();
        var SqlText;
        var ConnectionName;
        if (!(Node instanceof tp.TreeNode) || !Node.Tag || !(Node.Tag.NodeType === "Table" || Node.Tag.NodeType === "View")) {
            this.Log("No table or view selected.");
            return;
        }
        SqlText = Node.Tag[PropertyName] || "";
        ConnectionName = Node.Tag.ConnectionName || this.ActiveConnectionName;
        if (!tp.IsBlankString(SqlText) && !tp.IsBlankString(ConnectionName))
            app.App.OpenInteractiveSql(ConnectionName, SqlText);
    }
    /**
     * Writes a message to the application log.
     * @param {string} Text The message text.
     * @returns {void}
     */
    Log(Text) {
        if (tp.LogBox && !tp.IsBlankString(Text))
            tp.LogBox.AppendLine(Text);
    }

    // ● public
    /**
     * Loads explorer data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
        var Packet;
        var Index;
        var Connection;
        var Node;
        Packet = await tp.AjaxRequest.ExecuteAsync("App.DatabaseExplorer.GetConnections");
        this.Connections = Packet && tp.IsArray(Packet.Connections) ? Packet.Connections : [];
        this.Options = Packet && tp.IsObject(Packet.Options) ? Packet.Options : {};
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

// ● prototype
/**
 * Explorer toolbar.
 * @type {tp.ToolBar|null}
 */
app.DatabaseExplorerForm.prototype.ToolBar = null;
/**
 * Schema tree view.
 * @type {tp.TreeView|null}
 */
app.DatabaseExplorerForm.prototype.TreeView = null;
/**
 * Selected tree node.
 * @type {tp.TreeNode|null}
 */
app.DatabaseExplorerForm.prototype.SelectedNode = null;
/**
 * Available connection packets.
 * @type {object[]|null}
 */
app.DatabaseExplorerForm.prototype.Connections = null;
/**
 * Explorer options.
 * @type {object|null}
 */
app.DatabaseExplorerForm.prototype.Options = null;
/**
 * Schema packets keyed by connection name.
 * @type {object|null}
 */
app.DatabaseExplorerForm.prototype.SchemaMap = null;
/**
 * Active connection name.
 * @type {string}
 */
app.DatabaseExplorerForm.prototype.ActiveConnectionName = "";
