/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● command tree view form
/**
 * Displays application commands in a tree view.
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
app.CommandTreeViewForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates a command tree view form.
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
        tp.AddClass(this.Handle, "app-command-tree-view");
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * The commands displayed by this view.
         * @type {tp.DefList|null}
         */
        this.Commands = null;
        /**
         * The view toolbar.
         * @type {tp.ToolBar|null}
         */
        this.ToolBar = null;
        /**
         * The tree view.
         * @type {tp.TreeView|null}
         */
        this.TreeView = null;
        /**
         * The selected tree node.
         * @type {tp.TreeNode|null}
         */
        this.SelectedNode = null;
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
    }
    /**
     * Called just after the context is assigned.
     * @returns {void}
     */
    SetupContext() {
        super.SetupContext();
        this.Commands = app.App.MenuCommands;
    }
    /**
     * Called just after form initialization.
     * @returns {void}
     */
    FormInitialized() {
        super.FormInitialized();
        this.CreateTreeViewNodes();
    }
    /**
     * Creates the toolbar and tree view.
     * @returns {void}
     */
    CreateControls() {
        var ToolBarElement = this.Handle.querySelector("[data-role='toolbar']");
        var TreeElement = this.Handle.querySelector("[data-role='tree']");
        var Button;
        if (!(ToolBarElement instanceof HTMLElement) || !(TreeElement instanceof HTMLElement))
            return;
        this.ToolBar = new tp.ToolBar(ToolBarElement);
        tp.AddClass(this.ToolBar.Handle, "app-command-tree-toolbar");
        Button = this.ToolBar.AddButton("Expand", "Expand", "Expand", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "arrow_out.png" });
        Button = this.ToolBar.AddButton("Collapse", "Collapse", "Collapse", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "arrow_in.png" });
        Button = this.ToolBar.AddButton("Execute", "Execute", "Execute", "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "lightning.png" });
        this.ToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
        this.TreeView = new tp.TreeView(TreeElement);
        tp.AddClass(this.TreeView.Handle, "app-command-tree");
        this.TreeView.On("NodeClick", this.HandleNodeClick, this);
        this.TreeView.On("NodeDoubleClick", this.HandleNodeDoubleClick, this);
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "Expand")
            this.ExpandAll();
        else if (Args.Command === "Collapse")
            this.CollapseAll();
        else if (Args.Command === "Execute")
            this.ExecuteSelectedCommand();
    }
    /**
     * Handles tree node clicks.
     * @param {tp.TreeViewEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleNodeClick(Args) {
        this.SelectedNode = Args ? Args.Node : null;
    }
    /**
     * Handles tree node double clicks.
     * @param {tp.TreeViewEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleNodeDoubleClick(Args) {
        this.SelectedNode = Args ? Args.Node : null;
        this.ExecuteSelectedCommand();
    }
    /**
     * Creates the tree view nodes.
     * @returns {void}
     */
    CreateTreeViewNodes() {
        var Index;
        var Command;
        if (!this.TreeView)
            return;
        this.TreeView.Clear();
        this.SelectedNode = null;
        if (!(this.Commands instanceof tp.DefList))
            return;
        for (Index = 0; Index < this.Commands.Count; Index++) {
            Command = this.Commands.Items[Index];
            this.AddCommandNode(this.TreeView, Command);
        }
        this.TreeView.CollapseAll();
    }
    /**
     * Adds a command node to a tree node or tree view.
     * @param {tp.TreeView|tp.TreeNode} Parent The parent tree item.
     * @param {tp.Command} Command The command.
     * @returns {tp.TreeNode|null} Returns the created node.
     */
    AddCommandNode(Parent, Command) {
        var Node;
        var ImageUrl;
        var Index;
        var ChildCommand;
        if (!(Parent instanceof tp.TreeView || Parent instanceof tp.TreeNode) || !(Command instanceof tp.Command))
            return null;
        if (Command.HasChildren && Command.Commands.Count === 0)
            return null;
        Node = Parent.AddNode(Command.Title);
        Node.Tag = Command.HasChildren ? null : Command;
        Node.ToolTip = Command.Title;
        if (Command.HasChildren) {
            ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "folder16.png" });
            if (!tp.IsBlankString(ImageUrl))
                Node.ImageUrl = ImageUrl;
            else
                Node.IcoClasses = "fa fa-folder";
            for (Index = 0; Index < Command.Commands.Count; Index++) {
                ChildCommand = Command.Commands.Items[Index];
                this.AddCommandNode(Node, ChildCommand);
            }
        } else {
            ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "item16.png" });
            if (!tp.IsBlankString(ImageUrl))
                Node.ImageUrl = ImageUrl;
            else
                Node.IcoClasses = app.App.GetCommandIconClasses(Command);
        }
        return Node;
    }

    // ● public
    /**
     * Expands all tree nodes.
     * @returns {void}
     */
    ExpandAll() {
        if (this.TreeView)
            this.TreeView.ExpandAll();
    }
    /**
     * Collapses all tree nodes.
     * @returns {void}
     */
    CollapseAll() {
        if (this.TreeView)
            this.TreeView.CollapseAll();
    }
    /**
     * Executes the selected command.
     * @returns {void}
     */
    ExecuteSelectedCommand() {
        var Command = this.SelectedNode instanceof tp.TreeNode ? this.SelectedNode.Tag : null;
        if (Command instanceof tp.Command)
            app.App.ExecuteCommand(Command);
    }
    /**
     * Refreshes tree nodes from the current command list.
     * @returns {void}
     */
    Refresh() {
        this.Commands = app.App.MenuCommands;
        this.CreateTreeViewNodes();
    }
};

// ● prototype
/**
 * The commands displayed by this view.
 * @type {tp.DefList|null}
 */
app.CommandTreeViewForm.prototype.Commands = null;
/**
 * The view toolbar.
 * @type {tp.ToolBar|null}
 */
app.CommandTreeViewForm.prototype.ToolBar = null;
/**
 * The tree view.
 * @type {tp.TreeView|null}
 */
app.CommandTreeViewForm.prototype.TreeView = null;
/**
 * The selected tree node.
 * @type {tp.TreeNode|null}
 */
app.CommandTreeViewForm.prototype.SelectedNode = null;
