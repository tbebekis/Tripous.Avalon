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
        this.CreateTreeViewNodes();
    }
};

// ● main dashboard form
/**
 * Displays the tERP dashboard.
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
app.MainDashboardForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates the main dashboard form.
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
        tp.AddClass(this.Handle, "app-dashboard");
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * The dashboard toolbar.
         * @type {tp.ToolBar|null}
         */
        this.ToolBar = null;
        /**
         * The dashboard tab control.
         * @type {tp.TabControl|null}
         */
        this.TabControl = null;
        /**
         * KPI value elements keyed by metric name.
         * @type {object}
         */
        this.MetricElements = {};
        /**
         * The customers grid.
         * @type {tp.Grid|null}
         */
        this.CustomersGrid = null;
        /**
         * The suppliers grid.
         * @type {tp.Grid|null}
         */
        this.SuppliersGrid = null;
        /**
         * The stock grid.
         * @type {tp.Grid|null}
         */
        this.StockGrid = null;
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
     * Creates the dashboard controls.
     * @returns {void}
     */
    CreateControls() {
        var ToolBarElement = this.Handle.querySelector("[data-role='toolbar']");
        var TabElement = this.Handle.querySelector(".app-dashboard-tabs");
        if (!ToolBarElement)
            return;
        this.ToolBar = new tp.ToolBar(ToolBarElement);
        tp.AddClass(this.ToolBar.Handle, "app-dashboard-toolbar");
        this.AddToolBarButton("Refresh", "table_refresh.png");
        this.AddToolBarButton("Close", "door_out.png");
        this.ToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
        this.CollectMetrics();
        this.CreateGrids(TabElement);
    }
    /**
     * Adds a toolbar button.
     * @param {string} Command The command.
     * @param {string} ImageFileName The image file name.
     * @returns {void}
     */
    AddToolBarButton(Command, ImageFileName) {
        var Button = this.ToolBar.AddButton(Command, Command, Command, "", "", false);
        Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: ImageFileName });
    }
    /**
     * Collects KPI metric value elements.
     * @returns {void}
     */
    CollectMetrics() {
        var Elements = this.Handle.querySelectorAll("[data-metric]");
        var Index;
        var Element;
        this.MetricElements = {};
        for (Index = 0; Index < Elements.length; Index++) {
            Element = Elements[Index];
            this.MetricElements[Element.dataset.metric] = Element;
        }
    }
    /**
     * Creates the grid tab control.
     * @param {HTMLElement} TabElement The tab control element.
     * @returns {void}
     */
    CreateGrids(TabElement) {
        var TabControl;
        var Page;
        if (!(TabElement instanceof HTMLElement))
            return;
        this.TabControl = new tp.TabControl(TabElement);
        TabControl = this.TabControl;
        Page = TabControl.AddPage("Top Customers");
        this.CustomersGrid = this.CreateGrid(Page.Handle);
        Page = TabControl.AddPage("Top Suppliers");
        this.SuppliersGrid = this.CreateGrid(Page.Handle);
        Page = TabControl.AddPage("Stock Snapshot");
        this.StockGrid = this.CreateGrid(Page.Handle);
    }
    /**
     * Creates a readonly dashboard grid.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.Grid} Returns the grid.
     */
    CreateGrid(Parent) {
        var Element = this.Document.createElement("div");
        var Grid;
        Parent.appendChild(Element);
        tp.AddClass(Element, "app-dashboard-grid");
        Grid = new tp.Grid({
            ElementOrSelector: Element,
            ReadOnly: true,
            AutoGenerateColumns: true,
            ToolBarVisible: false,
            GroupsVisible: false,
            FilterVisible: false,
            FooterVisible: false
        });
        return Grid;
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "Refresh")
            this.Refresh();
        else if (Args.Command === "Close")
            this.Close();
    }

    // ● public
    /**
     * Refreshes dashboard data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
        var Packet = await app.App.GetMainDashboardDataAsync();
        this.SetMetrics(new tp.DataTable(Packet.Metrics));
        this.CustomersGrid.DataSource = new tp.DataTable(Packet.Customers);
        this.SuppliersGrid.DataSource = new tp.DataTable(Packet.Suppliers);
        this.StockGrid.DataSource = new tp.DataTable(Packet.Stock);
        if (tp.LogBox)
            tp.LogBox.AppendLine("Dashboard refreshed.");
    }
    /**
     * Refreshes dashboard data without throwing.
     * @returns {void}
     */
    Refresh() {
        this.LoadData();
    }
    /**
     * Sets KPI metric values.
     * @param {tp.DataTable} Table The metric table.
     * @returns {void}
     */
    SetMetrics(Table) {
        var Formatter = new Intl.NumberFormat(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        var Index;
        var Row;
        var Name;
        var Value;
        if (!(Table instanceof tp.DataTable))
            return;
        for (Index = 0; Index < Table.Rows.length; Index++) {
            Row = Table.Rows[Index];
            Name = Row.Get("Name");
            Value = Row.Get("Value", 0);
            if (this.MetricElements[Name])
                this.MetricElements[Name].textContent = Formatter.format(Number(Value || 0));
        }
    }
    /**
     * Closes the dashboard tab.
     * @returns {void}
     */
    Close() {
        super.Close();
    }
};

// ● web form page handler
/**
 * Opens and closes WebDesk form pages in the workspace tab control.
 */
app.WebFormPageHandler = class {
    // ● constructor
    /**
     * Creates a web form page handler.
     * @param {app.MainPage} MainPage The main page.
     */
    constructor(MainPage) {
        /**
         * The owner main page.
         * @type {app.MainPage}
         */
        this.MainPage = MainPage;
    }

    // ● protected
    /**
     * Returns the workspace tab control.
     * @returns {tp.TabControl|null} Returns the workspace tab control.
     */
    GetTabControl() {
        return this.MainPage ? this.MainPage.WorkspaceTabControl : null;
    }
    /**
     * Finds the root element of a web form inside a tab page.
     * @param {tp.TabPage} Page The tab page.
     * @returns {HTMLElement|null} Returns the form element or null.
     */
    FindFormElement(Page) {
        var Index;
        var Element;
        var Children = Page.Handle ? Page.Handle.children : [];
        for (Index = 0; Index < Children.length; Index++) {
            Element = Children[Index];
            if (Element instanceof HTMLElement)
                return Element;
        }
        return null;
    }
    /**
     * Creates a web form context for a tab page.
     * @param {tp.TabPage} Page The tab page.
     * @param {object} Form The server form packet.
     * @param {object} Packet The server packet.
     * @returns {tp.WebFormContext} Returns the web form context.
     */
    CreateFormContext(Page, Form, Packet) {
        return new tp.WebFormContext({
            FormId: Form.Name,
            ClassName: Form.JsFormClassType,
            DisplayMode: tp.WebFormDisplayMode.TabPage,
            ParentControl: Page,
            Title: !tp.IsBlankString(Form.Title) ? Form.Title : Form.Name,
            WebFormDef: Form,
            Packet: Packet,
            CssFiles: Form.CssFiles || [],
            JavaScriptFiles: Form.JavaScriptFiles || []
        });
    }
    /**
     * Creates the client component that handles a web form page.
     * @param {tp.TabPage} Page The tab page.
     * @param {tp.WebFormContext} Context The web form context.
     * @returns {Promise<tp.WebForm>} Returns a Promise resolving with the client form component.
     */
    async CreateFormComponent(Page, Context) {
        var Element = this.FindFormElement(Page);
        if (!(Element instanceof HTMLElement))
            throw new Error("WebForm root element not found: " + Context.FormId);
        return await Context.CreateForm(Element);
    }
    /**
     * Handles a web form close request.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleFormCloseRequested(Args) {
        if (Args && Args.Context instanceof tp.WebFormContext && Args.Context.ParentControl instanceof tp.TabPage)
            this.ClosePage(Args.Context.ParentControl);
    }

    // ● public
    /**
     * Finds a workspace page by web form name.
     * @param {string} WebFormName The web form name.
     * @returns {tp.TabPage|null} Returns the tab page or null.
     */
    FindPage(WebFormName) {
        return this.MainPage ? this.MainPage.FindWorkspacePage(WebFormName) : null;
    }
    /**
     * Opens a WebDesk form page.
     * @param {string} WebFormName The web form name.
     * @returns {Promise<tp.TabPage|null>} Returns a Promise with the opened page.
     */
    async OpenAsync(WebFormName) {
        var TabControl = this.GetTabControl();
        var Page;
        var Packet;
        var Form;
        var Component;
        var Context;
        if (!TabControl || tp.IsBlankString(WebFormName))
            return null;
        Page = this.FindPage(WebFormName);
        if (Page) {
            TabControl.SelectedPage = Page;
            if (Page.AppComponent instanceof tp.WebForm)
                Page.AppComponent.LoadData();
            else if (Page.AppComponent && tp.IsFunction(Page.AppComponent.Refresh))
                Page.AppComponent.Refresh();
            return Page;
        }
        Packet = await app.App.GetWebFormAsync(WebFormName);
        Form = Packet ? Packet.Form : null;
        if (!Form)
            throw new Error("WebForm not returned: " + WebFormName);
        Page = TabControl.AddPage(!tp.IsBlankString(Form.Title) ? Form.Title : Form.Name);
        Page.AppPageName = Form.Name;
        Page.AppPageHandler = this;
        Page.Handle.innerHTML = Form.Html || "";
        Context = this.CreateFormContext(Page, Form, Packet);
        Page.AppContext = Context;
        Component = await this.CreateFormComponent(Page, Context);
        Component.On("CloseRequested", this.HandleFormCloseRequested, this);
        Page.AppComponent = Component;
        if (Component instanceof tp.WebForm)
            Component.LoadData();
        else if (Component && tp.IsFunction(Component.Refresh))
            Component.Refresh();
        return Page;
    }
    /**
     * Opens a WebDesk form page and logs failures without throwing.
     * @param {string} WebFormName The web form name.
     * @returns {void}
     */
    Open(WebFormName) {
        this.OpenAsync(WebFormName).catch(function (e) {
            var Text = "Open web form failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        });
    }
    /**
     * Closes a web form page.
     * @param {tp.TabPage|null|undefined} Page The page to close.
     * @returns {void}
     */
    ClosePage(Page) {
        var TabControl = this.GetTabControl();
        var Component;
        if (!(TabControl && Page instanceof tp.TabPage))
            return;
        Component = Page.AppComponent;
        if (Component instanceof tp.WebForm && !Component.IsClosing) {
            if (Component.ClosableByUser)
                Component.CloseForm();
            return;
        }
        if (Component instanceof tp.Component) {
            Component.Dispose();
            Page.AppComponent = null;
        }
        Page.AppContext = null;
        TabControl.RemovePage(Page);
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
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * Main toolbar.
         * @type {tp.ToolBar|null}
         */
        this.MainToolBar = null;
        /**
         * Left sidebar tab control.
         * @type {tp.TabControl|null}
         */
        this.LeftTabControl = null;
        /**
         * Main vertical splitter.
         * @type {tp.Splitter|null}
         */
        this.MainSplitter = null;
        /**
         * Workspace tab control.
         * @type {tp.TabControl|null}
         */
        this.WorkspaceTabControl = null;
        /**
         * Log horizontal splitter.
         * @type {tp.Splitter|null}
         */
        this.LogSplitter = null;
        /**
         * Log panel element.
         * @type {HTMLElement|null}
         */
        this.LogPanel = null;
        /**
         * Log text area element.
         * @type {HTMLTextAreaElement|null}
         */
        this.LogTextArea = null;
        /**
         * Status bar.
         * @type {tp.StatusBar|null}
         */
        this.StatusBar = null;
        /**
         * Command tree view.
         * @type {app.CommandTreeView|null}
         */
        this.CommandTreeView = null;
        /**
         * Workspace web form page handler.
         * @type {app.WebFormPageHandler|null}
         */
        this.PageHandler = null;
    }
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
        this.WorkspaceTabControl = new tp.TabControl({
            ElementOrSelector: "#WorkspaceTabControl",
            CanClosePages: true,
            CanReorderPages: true
        });
        this.PageHandler = new app.WebFormPageHandler(this);
        this.WorkspaceTabControl.On("PageCloseRequested", this.HandleWorkspacePageCloseRequested, this);
        this.LogSplitter = new tp.Splitter("#LogSplitter");
        this.LogSplitter.IsHorizontal = true;
        this.LogSplitter.Panel1MinSize = 40;
        this.LogSplitter.Panel2MinSize = 40;
        this.StatusBar = new tp.StatusBar({
            ElementOrSelector: "#MainStatusBar",
            Items: [
                { Name: "Application", Text: app.App.GetApplicationName(), Width: "200px", TextAlign: "left" },
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
     * Handles workspace tab close requests.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleWorkspacePageCloseRequested(Args) {
        if (Args && this.PageHandler) {
            Args.Handled = true;
            this.PageHandler.ClosePage(Args.Page);
        }
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
     * Loads web forms from the server.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadWebFormsAsync() {
        var Count;
        var Text;

        try {
            if (this.StatusBar)
                this.StatusBar.Message = "Loading web forms...";

            Count = await app.App.LoadWebFormsAsync();
            Text = "Loaded web forms: " + Count.toString();

            if (this.CommandTreeView)
                this.CommandTreeView.Refresh();
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (this.StatusBar)
                this.StatusBar.Message = Text;
        } catch (e) {
            Text = "Load web forms failed: " + tp.ExceptionText(e);

            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (this.StatusBar)
                this.StatusBar.Message = Text;
        }
    }
    /**
     * Loads web forms from the server and logs failures without throwing.
     * @returns {void}
     */
    LoadWebForms() {
        this.LoadWebFormsAsync();
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
        this.CommandTreeView = new app.CommandTreeView({
            ElementOrSelector: ViewElement,
            Commands: app.App.MenuCommands
        });
    }
    /**
     * Finds a workspace page by application page name.
     * @param {string} Name The application page name.
     * @returns {tp.TabPage|null} Returns the page or null.
     */
    FindWorkspacePage(Name) {
        var Pages;
        var Index;
        if (!this.WorkspaceTabControl)
            return null;
        Pages = this.WorkspaceTabControl.GetPageList();
        for (Index = 0; Index < Pages.length; Index++) {
            if (tp.IsSameText(Pages[Index].AppPageName, Name))
                return Pages[Index];
        }
        return null;
    }
    /**
     * Closes a workspace page.
     * @param {tp.TabPage|null|undefined} Page The page to close.
     * @returns {void}
     */
    CloseWorkspacePage(Page) {
        if (this.PageHandler)
            this.PageHandler.ClosePage(Page);
        else if (this.WorkspaceTabControl && Page instanceof tp.TabPage)
            this.WorkspaceTabControl.RemovePage(Page);
    }
    /**
     * Shows the dashboard page.
     * @returns {void}
     */
    ShowDashboard() {
        if (this.PageHandler)
            this.PageHandler.Open("MainDashboard");
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
     * True when client commands have been registered.
     * @type {boolean}
     */
    CommandsRegistered: false,
    /**
     * Startup information returned by the server.
     * @type {object|null}
     */
    StartupInfo: null,
    /**
     * Application name.
     * @type {string}
     */
    ApplicationName: "",
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
     * Web form metadata loaded from the server.
     * @type {object[]}
     */
    WebForms: [],
    /**
     * Menu command group names created from web forms.
     * @type {string[]}
     */
    WebFormGroupCommandNames: [],
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
        "folder16.png",
        "item16.png",
        "lightning.png",
        "setting_tools.png",
        "table.png",
        "table_refresh.png"
    ],

    // ● commands
    /**
     * Registers client-side commands.
     * @returns {void}
     */
    RegisterCommands: function () {
        if (this.CommandsRegistered === true)
            return;

        var cmdDashboard = new tp.Command({ Name: "Dashboard", ImageFileName: "chart_bar.png", Form: "MainDashboard", Type: "Ui", IsSingleInstance: true });
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
        this.CommandsRegistered = true;
    },

    // ● startup
    /**
     * Updates the startup page message.
     * @param {string} Text The message text.
     * @returns {void}
     */
    SetStartupMessage: function (Text) {
        var Element = tp("#AppStartupMessage");
        if (Element)
            Element.textContent = Text;
    },
    /**
     * Returns the application name.
     * @returns {string} Returns the application name.
     */
    GetApplicationName: function () {
        if (!tp.IsBlankString(this.ApplicationName))
            return this.ApplicationName;
        if (document.body && !tp.IsBlankString(document.body.dataset.appName))
            this.ApplicationName = document.body.dataset.appName;
        return this.ApplicationName;
    },
    /**
     * Loads startup information from the server.
     * @returns {Promise<object>} Returns the startup information.
     */
    LoadStartupInfoAsync: async function () {
        var Packet = await tp.AjaxRequest.ExecuteAsync("App.GetStartupInfo");
        this.StartupInfo = Packet || {};
        if (this.StartupInfo && !tp.IsBlankString(this.StartupInfo.ApplicationName))
            this.ApplicationName = this.StartupInfo.ApplicationName;
        return this.StartupInfo;
    },

    // ● dialogs
    /**
     * Sets the message text of a startup dialog.
     * @param {tp.Window} Window The dialog window.
     * @param {string} Text The message text.
     * @returns {void}
     */
    SetStartupDialogMessage: function (Window, Text) {
        var Element = Window && Window.Handle ? Window.Handle.querySelector("[data-role='message']") : null;
        if (Element)
            Element.textContent = Text || "";
    },
    /**
     * Collects values from a startup dialog.
     * @param {tp.Window} Window The dialog window.
     * @returns {object} Returns a value object.
     */
    CollectStartupDialogData: function (Window) {
        var Result = {};
        var Elements = Window && Window.Handle ? Window.Handle.querySelectorAll("input[name], select[name]") : [];
        var Index;
        var Element;
        for (Index = 0; Index < Elements.length; Index++) {
            Element = Elements[Index];
            Result[Element.name] = Element.value;
        }
        return Result;
    },
    /**
     * Handles startup dialog key presses.
     * @param {KeyboardEvent} e The keyboard event.
     * @param {tp.Window} Window The dialog window.
     * @returns {void}
     */
    HandleStartupDialogKeyDown: function (e, Window) {
        if (tp.IsKey(e, tp.Keys.Enter)) {
            e.preventDefault();
            Window.DialogResult = tp.DialogResult.OK;
        }
    },
    /**
     * Shows a startup dialog as a modal content window.
     * @param {string} Html The dialog HTML.
     * @param {string} Title The dialog title.
     * @param {number} Width The dialog width.
     * @param {number} Height The dialog height.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null when cancelled.
     */
    ShowStartupDialogAsync: async function (Html, Title, Width, Height, Message) {
        var Self = this;
        var Args = {
            Text: Title,
            Width: Width,
            Height: Height,
            ResizeEdges: tp.Edge.None,
            InitialFocusSelector: "input[autofocus], input",
            ShowFunc: function (Window) {
                Self.SetStartupDialogMessage(Window, Message);
                Window.StartupDialogKeyDownHandler = function (e) {
                    Self.HandleStartupDialogKeyDown(e, Window);
                };
                Window.Handle.addEventListener("keydown", Window.StartupDialogKeyDownHandler);
            },
            CloseFunc: function (Window) {
                if (Window.StartupDialogKeyDownHandler)
                    Window.Handle.removeEventListener("keydown", Window.StartupDialogKeyDownHandler);
                if (Window.DialogResult === tp.DialogResult.OK)
                    Window.ResultData = Self.CollectStartupDialogData(Window);
            }
        };
        var Window = await tp.ContentWindow.ShowModalAsync(Html, Args);
        return Window.DialogResult === tp.DialogResult.OK ? Window.ResultData : null;
    },
    /**
     * Shows the first run administrator dialog.
     * @param {object} Info The startup information.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null.
     */
    ShowFirstRunDialogAsync: function (Info, Message) {
        return this.ShowStartupDialogAsync(Info.FirstRunHtml, "First Application Run", 420, 400, Message);
    },
    /**
     * Shows the login dialog.
     * @param {object} Info The startup information.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null.
     */
    ShowLoginDialogAsync: function (Info, Message) {
        return this.ShowStartupDialogAsync(Info.LoginHtml, "Login", 400, 300, Message);
    },

    // ● startup flow
    /**
     * Starts the application bootstrap flow.
     * @returns {Promise<void>} Returns a Promise.
     */
    Start: async function () {
        var Info;
        var DialogData;
        var Packet;
        var Message = "";

        if (tp("#AppShell")) {
            this.Initialize();
            return;
        }

        if (!tp("#AppStartup"))
            return;

        try {
            while (true) {
                this.SetStartupMessage("Checking startup state...");
                Info = await this.LoadStartupInfoAsync();

                if (Info.RequiresFirstRun === true) {
                    this.SetStartupMessage("First run setup is required.");
                    DialogData = await this.ShowFirstRunDialogAsync(Info, Message);
                    if (DialogData === null) {
                        this.SetStartupMessage("No Admin user. Terminating...");
                        return;
                    }
                    Packet = await tp.AjaxRequest.ExecuteAsync("App.CreateFirstRunAdmin", DialogData);
                    Message = Packet && Packet.Message ? Packet.Message : "";
                    if (Packet && Packet.Success === true) {
                        Message = "";
                        continue;
                    }
                    continue;
                }

                if (Info.UseUsers === true && Info.IsAuthenticated !== true) {
                    this.SetStartupMessage("Login is required.");
                    DialogData = await this.ShowLoginDialogAsync(Info, Message);
                    if (DialogData === null) {
                        this.SetStartupMessage("Login cancelled.");
                        return;
                    }
                    Packet = await tp.AjaxRequest.ExecuteAsync("App.Login", DialogData);
                    Message = Packet && Packet.Message ? Packet.Message : "";
                    if (Packet && Packet.Success === true) {
                        Message = "";
                        continue;
                    }
                    continue;
                }

                this.SetStartupMessage("Opening main page...");
                tp.NavigateTo("/Home/MainPage");
                return;
            }
        } catch (e) {
            this.SetStartupMessage("Startup failed: " + tp.ExceptionText(e));
        }
    },

    // ● data
    /**
     * Executes a SELECT statement and returns a data table.
     * @param {string} Name The table name.
     * @param {string} SqlText The SQL text.
     * @param {string|null|undefined} ConnectionName Optional connection name.
     * @returns {Promise<tp.DataTable>} Returns a Promise with the data table.
     */
    SelectAsync: async function (Name, SqlText, ConnectionName) {
        var Packet = await tp.AjaxRequest.ExecuteAsync("Select", {
            Name: Name,
            SqlText: SqlText,
            ConnectionName: ConnectionName || ""
        });
        return new tp.DataTable(Packet.Table || Packet.JsonDataTable);
    },

    // ● web forms
    /**
     * Returns a server-rendered web form packet.
     * @param {string} WebFormName The web form name.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    GetWebFormAsync: async function (WebFormName) {
        return await tp.AjaxRequest.ExecuteAsync("App.GetWebForm", {
            WebFormName: WebFormName
        });
    },
    /**
     * Returns main dashboard data.
     * @returns {Promise<object>} Returns a Promise with dashboard data.
     */
    GetMainDashboardDataAsync: async function () {
        return await tp.AjaxRequest.ExecuteAsync("App.MainDashboard.GetData");
    },
    /**
     * Loads web forms from the server and creates command groups.
     * @returns {Promise<number>} Returns the number of loaded web forms.
     */
    LoadWebFormsAsync: async function () {
        var Packet = await tp.AjaxRequest.ExecuteAsync("App.GetWebForms");
        var Forms = Packet && tp.IsArray(Packet.WebForms) ? Packet.WebForms : [];

        this.WebForms = Forms;
        this.BuildWebFormCommands(Forms);
        return Forms.length;
    },
    /**
     * Builds command groups from web form metadata.
     * @param {object[]} Forms The web form metadata array.
     * @returns {void}
     */
    BuildWebFormCommands: function (Forms) {
        var Index;
        var Form;
        var GroupName;
        var GroupCommandName;
        var GroupCommand;
        var Groups = {};

        for (Index = 0; Index < this.WebFormGroupCommandNames.length; Index++)
            this.MenuCommands.Remove(this.WebFormGroupCommandNames[Index]);
        this.WebFormGroupCommandNames.length = 0;

        for (Index = 0; Index < Forms.length; Index++) {
            Form = Forms[Index];
            if (Form.IsCustom === true)
                continue;
            GroupName = !tp.IsBlankString(Form.Group) ? Form.Group : "General Forms";
            GroupCommandName = "WebForms." + GroupName;
            GroupCommand = Groups[GroupCommandName];

            if (!GroupCommand) {
                GroupCommand = new tp.Command({ Name: GroupCommandName, Title: GroupName, ImageFileName: "folder.png" });
                Groups[GroupCommandName] = GroupCommand;
                this.WebFormGroupCommandNames.push(GroupCommandName);
                this.MenuCommands.Add(GroupCommand);
            }

            GroupCommand.Add({
                Name: "WebForm." + Form.Name,
                TitleKey: Form.TitleKey,
                Title: Form.Title,
                ImageFileName: "table.png",
                Form: Form.Name,
                Type: "Ui",
                IsSingleInstance: true,
                Params: {
                    Module: Form.Module,
                    ViewName: Form.ViewName,
                    ItemViewName: Form.ItemViewName,
                    JsFormClassType: Form.JsFormClassType,
                    IsReadOnly: Form.IsReadOnly === true
                }
            });
        }
    },
    /**
     * Resolves a dotted JavaScript class type name.
     * @param {string} ClassType The class type name.
     * @returns {Function|null} Returns the class constructor or null.
     */
    ResolveClassType: function (ClassType) {
        var Parts;
        var Index;
        var Result = window;
        if (tp.IsBlankString(ClassType))
            return null;
        Parts = ClassType.split(".");
        for (Index = 0; Index < Parts.length; Index++) {
            Result = Result ? Result[Parts[Index]] : null;
            if (!Result)
                return null;
        }
        return tp.IsFunction(Result) ? Result : null;
    },

    // ● command lookup
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

    // ● command execution
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
        else if (Command.Name === "Dashboard" && this.MainPage)
            this.MainPage.ShowDashboard();
        else if (Command.Name === "Toggle Log" && this.MainPage)
            this.MainPage.ToggleLog();
        else if (Command.IsUiCommand() && tp.IsBlankString(Command.Params ? Command.Params.JsFormClassType : ""))
            this.ReportCommandNotAvailable(Command);
        else if (Command.IsUiCommand() && this.MainPage && this.MainPage.PageHandler)
            this.MainPage.PageHandler.Open(Command.Form);
        else if (tp.LogBox)
            tp.LogBox.AppendLine("Command executed: " + Command.Name);
    },
    /**
     * Reports a command that is visible but not yet implemented.
     * @param {tp.Command} Command The command.
     * @returns {void}
     */
    ReportCommandNotAvailable: function (Command) {
        var Text = "Web form is not available yet: " + (Command.Title || Command.Name);
        if (tp.LogBox)
            tp.LogBox.AppendLine(Text);
        if (this.MainPage && this.MainPage.StatusBar)
            this.MainPage.StatusBar.Message = Text;
    },

    // ● command icons
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

    // ● lifecycle
    /**
     * Initializes the application.
     * @returns {void}
     */
    Initialize: function () {
        if (this.MainPage !== null)
            return;
        this.RegisterCommands();
        this.MainPage = new app.MainPage("#AppShell");
        this.MainPage.LoadWebForms();
    }
};

/**
 * Called by the Tripous runtime before ready listeners.
 * @returns {void}
 */
tp.AppInitializeBefore = function () {
    app.App.Start();
};
