/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = {};

// ● command tree view
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
 */
app.CommandTreeView = class extends tp.Component {
    // ● fields
    /**
     * The commands displayed by this view.
     * @type {tp.DefList|null}
     */
    Commands = null;
    /**
     * The view toolbar.
     * @type {tp.ToolBar|null}
     */
    ToolBar = null;
    /**
     * The tree view.
     * @type {tp.TreeView|null}
     */
    TreeView = null;
    /**
     * The selected tree node.
     * @type {tp.TreeNode|null}
     */
    SelectedNode = null;

    // ● constructor
    /**
     * Creates a command tree view.
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
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
    }
    /**
     * Applies explicit create params to this component.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (Params && Params.Commands instanceof tp.DefList)
            this.Commands = Params.Commands;
        this.CreateTreeViewNodes();
    }
    /**
     * Creates the toolbar and tree view.
     * @returns {void}
     */
    CreateControls() {
        var ToolBarElement = this.Document.createElement("div");
        var TreeElement = this.Document.createElement("div");
        var Button;
        this.Handle.appendChild(ToolBarElement);
        this.Handle.appendChild(TreeElement);
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
        this.TreeView.ExpandAll();
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
            ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "folder.png" });
            if (!tp.IsBlankString(ImageUrl))
                Node.ImageUrl = ImageUrl;
            else
                Node.IcoClasses = "fa fa-folder";
            for (Index = 0; Index < Command.Commands.Count; Index++) {
                ChildCommand = Command.Commands.Items[Index];
                this.AddCommandNode(Node, ChildCommand);
            }
        } else {
            ImageUrl = app.App.GetCommandImageUrl(Command);
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
};

// ● main page
/**
 * Represents the main application shell page.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
app.MainPage = class extends tp.Component {
    // ● fields
    /**
     * Main toolbar.
     * @type {tp.ToolBar|null}
     */
    MainToolBar = null;
    /**
     * Left sidebar tab control.
     * @type {tp.TabControl|null}
     */
    LeftTabControl = null;
    /**
     * Main vertical splitter.
     * @type {tp.Splitter|null}
     */
    MainSplitter = null;
    /**
     * Workspace tab control.
     * @type {tp.TabControl|null}
     */
    WorkspaceTabControl = null;
    /**
     * Log horizontal splitter.
     * @type {tp.Splitter|null}
     */
    LogSplitter = null;
    /**
     * Log panel element.
     * @type {HTMLElement|null}
     */
    LogPanel = null;
    /**
     * Log text area element.
     * @type {HTMLTextAreaElement|null}
     */
    LogTextArea = null;
    /**
     * Status bar.
     * @type {tp.StatusBar|null}
     */
    StatusBar = null;

    // ● constructor
    /**
     * Creates the main application shell page.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates child controls and stores useful shell element references.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.InitializeElements();
        this.InitializeControls();
        this.InitializeLog();
    }
    /**
     * Stores useful shell element references.
     * @returns {void}
     */
    InitializeElements() {
        this.LogPanel = tp("#LogPanel");
        this.LogTextArea = tp("#LogTextArea");
    }
    /**
     * Creates Tripous controls used by the shell.
     * @returns {void}
     */
    InitializeControls() {
        this.MainToolBar = new tp.ToolBar("#MainToolBar");
        this.PopulateToolBar();
        this.MainToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
        this.LeftTabControl = new tp.TabControl("#LeftTabControl");
        this.PopulateLeftSideBar();
        this.MainSplitter = new tp.Splitter("#MainSplitter");
        this.MainSplitter.Panel1MinSize = 40;
        this.MainSplitter.Panel2MinSize = 40;
        this.WorkspaceTabControl = new tp.TabControl("#WorkspaceTabControl");
        this.LogSplitter = new tp.Splitter("#LogSplitter");
        this.LogSplitter.IsHorizontal = true;
        this.LogSplitter.Panel1MinSize = 40;
        this.LogSplitter.Panel2MinSize = 40;
        this.StatusBar = new tp.StatusBar({
            ElementOrSelector: "#MainStatusBar",
            Items: [
                { Name: "Application", Text: "tERP Web", Width: "200px", TextAlign: "left" },
                { Name: "User", Text: "User: Admin", Width: "200px", TextAlign: "center" },
                { Name: "Role", Text: "Role: Admin", Width: "240px", TextAlign: "center" },
                { Name: "Message", Text: "Ready", Width: "1fr", TextAlign: "center" }
            ],
            DefaultItemName: "Message"
        });
    }
    /**
     * Initializes the log target.
     * @returns {void}
     */
    InitializeLog() {
        if (tp.LogBox) {
            tp.LogBox.Initialize(this.LogTextArea, { MaxLines: 1000 });
            tp.LogBox.AppendLine("Application Started.");
            tp.LogBox.AppendLine("Ready.");
        } else if (this.LogTextArea instanceof HTMLTextAreaElement) {
            this.LogTextArea.value = "Application Started.\nReady.";
        }
    }
    /**
     * Populates the main toolbar from registered commands.
     * @returns {void}
     */
    PopulateToolBar() {
        var Command;
        var Button;
        var Index;

        for (Index = 0; Index < app.App.ToolBarCommands.Count; Index++) {
            Command = app.App.ToolBarCommands.Items[Index];
            Button = this.MainToolBar.AddButton(Command.Name, Command.Title, Command.Title, app.App.GetCommandIconClasses(Command), "", false);
            Button.ImageUrl = app.App.GetCommandImageUrl(Command);
        }
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        var Command = Args ? app.App.FindCommand(Args.Command) : null;

        if (Command === null)
            return;

        app.App.ExecuteCommand(Command);
    }
    /**
     * Sends a ping Ajax request to the server.
     * @returns {Promise<void>} Returns a Promise.
     */
    async PingServerAsync() {
        var Packet;
        var Text;

        try {
            if (this.StatusBar)
                this.StatusBar.Message = "Pinging server...";

            Packet = await tp.AjaxRequest.ExecuteAsync("App.Ping");
            Text = "Ping response: " + JSON.stringify(Packet);

            if (tp.LogBox) {
                tp.LogBox.AppendLine("Ping succeeded.");
                tp.LogBox.AppendLine(Text);
            }
            if (this.StatusBar)
                this.StatusBar.Message = "Ping OK";
        } catch (e) {
            Text = "Ping failed: " + tp.ExceptionText(e);

            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (this.StatusBar)
                this.StatusBar.Message = Text;
        }
    }
    /**
     * Populates the left side bar tab control.
     * @returns {void}
     */
    PopulateLeftSideBar() {
        var Page;
        var ViewElement;
        if (!this.LeftTabControl)
            return;
        Page = this.LeftTabControl.AddPage("Commands");
        if (!Page)
            return;
        ViewElement = this.Document.createElement("div");
        Page.Handle.appendChild(ViewElement);
        new app.CommandTreeView({
            ElementOrSelector: ViewElement,
            Commands: app.App.MenuCommands
        });
    }

    // ● public
    /**
     * Shows or hides the log panel and its splitter.
     * @returns {void}
     */
    ToggleLog() {
        var IsVisible = this.LogPanel.style.display !== "none";
        this.LogPanel.style.display = IsVisible ? "none" : "";
        this.LogSplitter.Handle.style.display = IsVisible ? "none" : "";
    }

    // ● properties
    /**
     * Gets the main toolbar.
     * @returns {tp.ToolBar|null} Returns the main toolbar.
     */
    get ToolBar() {
        return this.MainToolBar;
    }
};

// ● command icon source
/**
 * Command icon source options.
 * @enum {string}
 */
app.CommandIconSource = {
    Image: "Image",
    FontAwesome: "FontAwesome"
};
Object.freeze(app.CommandIconSource);

// ● app
/**
 * Static application entry point.
 * @type {object}
 */
app.App = {
    // ● fields
    /**
     * Main page instance.
     * @type {app.MainPage|null}
     */
    MainPage: null,
    /**
     * Command icon source. Use "Image" for disk images or "FontAwesome" for CSS icons.
     * @type {string}
     */
    CommandIconSource: app.CommandIconSource.Image,
    /**
     * Registered menu commands.
     * @type {tp.DefList}
     */
    MenuCommands: new tp.DefList(tp.Command),
    /**
     * Registered toolbar commands.
     * @type {tp.DefList}
     */
    ToolBarCommands: new tp.DefList(tp.Command),
    /**
     * Available toolbar image file names.
     * @type {string[]}
     */
    CommandImageFileNames: [
        "arrow_in.png",
        "arrow_out.png",
        "bin.png",
        "change_password.png",
        "chart_bar.png",
        "database_edit.png",
        "database_refresh.png",
        "door_out.png",
        "error_log.png",
        "file_extension_log.png",
        "folder.png",
        "lightning.png",
        "setting_tools.png"
    ],

    // ● public
    /**
     * Registers client-side commands.
     * @returns {void}
     */
    RegisterCommands: function () {
        var cmdDashboard = new tp.Command({ Name: "Dashboard", ImageFileName: "chart_bar.png" });
        var cmdAppFolder = new tp.Command({ Name: "ShowAppFolder", ImageFileName: "folder.png" });
        var cmdApplicationSettings = new tp.Command({ Name: "Application Settings", ImageFileName: "setting_tools.png" });
        var cmdChangePassword = new tp.Command({ Name: "Change Password", ImageFileName: "change_password.png" });
        var cmdConnectionInfo = new tp.Command({ Name: "ConnectionInfo", ImageFileName: "database_edit.png" });
        var cmdRegenerateDatabase = new tp.Command({ Name: "Regenerate Database", ImageFileName: "database_refresh.png" });
        var cmdClearLog = new tp.Command({ Name: "Clear Log", ImageFileName: "bin.png" });
        var cmdToggleLog = new tp.Command({ Name: "Toggle Log", ImageFileName: "error_log.png" });
        var cmdToggleLogSqlStatements = new tp.Command({ Name: "Log Sql", ImageFileName: "file_extension_log.png", IsToggle: true });
        var cmdPing = new tp.Command({ Name: "App.Ping", Title: "Ping", ImageFileName: "lightning.png" });
        var cmdGeneral = new tp.Command("General");

        cmdGeneral.AddRange([cmdDashboard, cmdAppFolder, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdRegenerateDatabase]);
        this.MenuCommands.Add(cmdGeneral);
        this.ToolBarCommands.AddRange([cmdDashboard, cmdAppFolder, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdRegenerateDatabase, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdPing]);
    },
    /**
     * Finds a registered command.
     * @param {string} Name The command name.
     * @returns {tp.Command|null} Returns the command or null.
     */
    FindCommand: function (Name) {
        var Result = this.ToolBarCommands.Find(Name);
        if (Result !== null)
            return Result;

        return this.FindCommandInList(this.MenuCommands, Name);
    },
    /**
     * Finds a command in a command list.
     * @param {tp.DefList} List The command list.
     * @param {string} Name The command name.
     * @returns {tp.Command|null} Returns the command or null.
     */
    FindCommandInList: function (List, Name) {
        var Index;
        var Command;
        var Result;

        for (Index = 0; Index < List.Count; Index++) {
            Command = List.Items[Index];
            if (tp.IsSameText(Command.Name, Name))
                return Command;
            if (Command.HasChildren) {
                Result = this.FindCommandInList(Command.Commands, Name);
                if (Result !== null)
                    return Result;
            }
        }

        return null;
    },
    /**
     * Executes a command.
     * @param {tp.Command} Command The command.
     * @returns {void}
     */
    ExecuteCommand: function (Command) {
        if (!(Command instanceof tp.Command))
            return;
        if (Command.Name === "App.Ping" && this.MainPage)
            this.MainPage.PingServerAsync();
        else if (Command.Name === "Toggle Log" && this.MainPage)
            this.MainPage.ToggleLog();
        else if (tp.LogBox)
            tp.LogBox.AppendLine("Command executed: " + Command.Name);
    },
    /**
     * Returns icon CSS classes for a command.
     * @param {tp.Command} Command The command.
     * @returns {string} Returns icon CSS classes.
     */
    GetCommandIconClasses: function (Command) {
        if (this.CommandIconSource === app.CommandIconSource.Image && !tp.IsBlankString(this.GetCommandImageUrl(Command)))
            return "";
        if (Command.Name === "App.Ping")
            return "fa fa-bolt";
        if (Command.Name === "Toggle Log")
            return "fa fa-list";
        if (Command.Name === "Clear Log")
            return "fa fa-trash";
        if (Command.Name === "Log Sql")
            return "fa fa-file-lines";
        if (Command.Name === "Dashboard")
            return "fa fa-chart-simple";
        if (Command.Name === "ShowAppFolder")
            return "fa fa-folder-open";
        if (Command.Name === "Application Settings")
            return "fa fa-screwdriver-wrench";
        if (Command.Name === "Change Password")
            return "fa fa-key";
        if (Command.Name === "ConnectionInfo")
            return "fa fa-database";
        if (Command.Name === "Regenerate Database")
            return "fa fa-rotate";
        return "fa fa-circle";
    },
    /**
     * Returns image URL for a command.
     * @param {tp.Command} Command The command.
     * @returns {string} Returns the image URL.
     */
    GetCommandImageUrl: function (Command) {
        var ImageFileName = Command ? Command.ImageFileName : "";

        if (this.CommandIconSource !== app.CommandIconSource.Image)
            return "";

        if (tp.IsBlankString(ImageFileName))
            return "";

        if (!this.CommandImageFileNames.includes(ImageFileName))
            return "";

        return "/images/toolbar/" + ImageFileName;
    },
    /**
     * Initializes the application.
     * @returns {void}
     */
    Initialize: function () {
        this.RegisterCommands();
        this.MainPage = new app.MainPage("#AppShell");
    }
};

/**
 * Called by the Tripous runtime before ready listeners.
 * @returns {void}
 */
tp.AppInitializeBefore = function () {
    app.App.Initialize();
};
