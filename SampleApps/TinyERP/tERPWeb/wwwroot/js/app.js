/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = {};

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
        this.MainToolBar.SetNoText(true);
        this.MainToolBar.SetIcoMode(tp.ButtonExIcoMode.Left);
        this.LeftTabControl = new tp.TabControl("#LeftTabControl");
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
                { Name: "Application", Text: "tERP Web", Width: "1fr", TextAlign: "left" },
                { Name: "User", Text: "User: Admin", Width: "150px", TextAlign: "center" },
                { Name: "Role", Text: "Role: Admin", Width: "150px", TextAlign: "center" },
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

    // ● public
    /**
     * Initializes the application.
     * @returns {void}
     */
    Initialize: function () {
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
