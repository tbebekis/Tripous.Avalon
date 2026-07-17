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
 * - ParentChangedMA
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
         * @type {app.CommandTreeViewForm|null}
         */
        this.CommandTreeView = null;
        /**
         * Workspace web form page handler.
         * @type {tp.WebFormPageHandler|null}
         */
        this.PageHandler = null;
        /**
         * Left sidebar web form page handler.
         * @type {tp.WebFormPageHandler|null}
         */
        this.SideBarHandler = null;
        /**
         * Global shell keydown handler.
         * @type {Function|null}
         */
        this.WindowKeyDownHandler = null;
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
        this.WindowKeyDownHandler = (e) => this.HandleWindowKeyDown(e);
        window.addEventListener("keydown", this.WindowKeyDownHandler, true);
    }
    /**
     * Releases owned controls and event handlers.
     * @returns {void}
     */
    DoDispose() {
        if (this.WindowKeyDownHandler) {
            window.removeEventListener("keydown", this.WindowKeyDownHandler, true);
            this.WindowKeyDownHandler = null;
        }
        super.DoDispose();
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
        this.SideBarHandler = new tp.WebFormPageHandler({
            TabControl: this.LeftTabControl,
            GetWebFormFunc: function (WebFormName) {
                return app.App.GetWebFormAsync(WebFormName);
            },
            ErrorFunc: function (Text) {
                if (tp.LogBox)
                    tp.LogBox.AppendLine(Text);
                if (app.App.MainPage && app.App.MainPage.StatusBar)
                    app.App.MainPage.StatusBar.Message = Text;
            }
        });
        this.PopulateLeftSideBar();
        this.MainSplitter = new tp.Splitter("#MainSplitter");
        this.MainSplitter.Panel1MinSize = 40;
        this.MainSplitter.Panel2MinSize = 40;
        this.WorkspaceTabControl = new tp.TabControl({
            ElementOrSelector: "#WorkspaceTabControl",
            CanClosePages: true,
            CanReorderPages: true
        });
        this.PageHandler = new tp.WebFormPageHandler({
            TabControl: this.WorkspaceTabControl,
            GetWebFormFunc: function (WebFormName) {
                return app.App.GetWebFormAsync(WebFormName);
            },
            ErrorFunc: function (Text) {
                if (tp.LogBox)
                    tp.LogBox.AppendLine(Text);
                if (app.App.MainPage && app.App.MainPage.StatusBar)
                    app.App.MainPage.StatusBar.Message = Text;
            }
        });
        this.WorkspaceTabControl.On("PageCloseRequested", this.HandleWorkspacePageCloseRequested, this);
        this.LogSplitter = new tp.Splitter("#LogSplitter");
        this.LogSplitter.IsHorizontal = true;
        this.LogSplitter.Panel1MinSize = 40;
        this.LogSplitter.Panel2MinSize = 40;
        this.StatusBar = new tp.StatusBar({
            ElementOrSelector: "#MainStatusBar",
            Items: [
                { Name: "Application", Text: app.App.GetApplicationName(), Width: "200px", TextAlign: "left" },
                { Name: "User", Text: "User: ", Width: "200px", TextAlign: "center" },
                { Name: "Role", Text: "Role: ", Width: "240px", TextAlign: "center" },
                { Name: "Message", Text: "Ready", Width: "1fr", TextAlign: "center" }
            ],
            DefaultItemName: "Message"
        });
        this.UpdateStatusInfo(app.App.StartupInfo);
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
     * Returns true when a keyboard event maps to a browser command reserved by the WebDesk shell.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {boolean} Returns true when the browser command should be canceled.
     */
    IsReservedBrowserShortcut(e) {
        var HasModifier = e.ctrlKey === true || e.metaKey === true;
        var Key = tp.IsString(e.key) ? e.key.toLowerCase() : "";

        if (tp.IsSameText(e.key, "F5") || e.code === "F5")
            return true;
        if (HasModifier !== true)
            return false;
        return Key === "r" || Key === "s" || Key === "f" || Key === "p" || Key === "o";
    }
    /**
     * Handles global shell keyboard shortcuts.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleWindowKeyDown(e) {
        if (!(e instanceof KeyboardEvent) || this.IsReservedBrowserShortcut(e) !== true)
            return;
        if (tp.IsFunction(e.preventDefault))
            e.preventDefault();
        e.returnValue = false;
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
     * Updates user-related status bar items.
     * @param {object|null|undefined} Info The startup/current user information.
     * @returns {void}
     */
    UpdateStatusInfo(Info) {
        Info = Info || {};
        if (!this.StatusBar)
            return;
        this.StatusBar.SetText("Application", app.App.GetApplicationName());
        this.StatusBar.SetText("User", "User: " + (Info.UserName || ""));
        this.StatusBar.SetText("Role", "Role: " + (Info.UserLevel || ""));
    }
    /**
     * Populates the left side bar tab control.
     * @returns {void}
     */
    PopulateLeftSideBar() {
        var Self = this;
        if (!this.SideBarHandler)
            return;
        this.SideBarHandler.OpenAsync("CommandTreeView").then(function (Page) {
            Self.CommandTreeView = Page && Page.AppComponent instanceof tp.WebForm ? Page.AppComponent : null;
        }).catch(function (e) {
            var Text = "Open sidebar failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (Self.StatusBar)
                Self.StatusBar.Message = Text;
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
        if (this.StatusBar)
            this.StatusBar.Message = IsVisible ? "Log hidden." : "Log visible.";
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
        "arrow_left.png",
        "arrow_in.png",
        "arrow_out.png",
        "arrow_right.png",
        "bin.png",
        "change_password.png",
        "chart_bar.png",
        "database.png",
        "database_edit.png",
        "database_green.png",
        "database_refresh.png",
        "door_out.png",
        "error_log.png",
        "file_extension_log.png",
        "folder.png",
        "folder16.png",
        "item16.png",
        "lightning.png",
        "script_lightning.png",
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
        var cmdApplicationSettings = new tp.Command({ Name: "Application Settings", ImageFileName: "setting_tools.png" });
        var cmdChangePassword = new tp.Command({ Name: "Change Password", ImageFileName: "change_password.png" });
        var cmdConnectionInfo = new tp.Command({ Name: "ConnectionInfo", ImageFileName: "database_edit.png" });
        var cmdDatabaseWorkbench = new tp.Command({
            Name: "Database Workbench",
            ImageFileName: "script_lightning.png",
            Form: "DatabaseWorkbench",
            Type: "Ui",
            IsSingleInstance: true,
            Params: {
                JsFormClassType: "app.DatabaseWorkbenchForm"
            }
        });
        var cmdRegenerateDatabase = new tp.Command({ Name: "Regenerate Database", ImageFileName: "database_refresh.png" });
        var cmdClose = new tp.Command({ Name: "Close", ImageFileName: "door_out.png" });
        var cmdClearLog = new tp.Command({ Name: "Clear Log", ImageFileName: "bin.png" });
        var cmdToggleLog = new tp.Command({ Name: "Toggle Log", ImageFileName: "error_log.png" });
        var cmdToggleLogSqlStatements = new tp.Command({ Name: "Log Sql", ImageFileName: "file_extension_log.png", IsToggle: true });
        var cmdPing = new tp.Command({ Name: "App.Ping", Title: "Ping", ImageFileName: "lightning.png" });
        var cmdGeneral = new tp.Command("General");
        var GeneralCommands = [cmdDashboard, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdDatabaseWorkbench, cmdRegenerateDatabase, cmdClose];
        var ToolBarCommands = [cmdDashboard, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdDatabaseWorkbench, cmdRegenerateDatabase, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdPing, cmdClose];

        cmdGeneral.AddRange(GeneralCommands);
        this.MenuCommands.Add(cmdGeneral);
        this.ToolBarCommands.AddRange(ToolBarCommands);
        this.CommandsRegistered = true;
    },

    // ● startup
    /**
     * Shows or hides the startup Login button.
     * @param {boolean} Flag True to show the button.
     * @returns {void}
     */
    SetStartupLoginVisible: function (Flag) {
        var Element = tp("#AppStartupLoginButton");
        if (Element) {
            if (Flag === true)
                tp.AddClass(Element, tp.Classes.Visible);
            else
                tp.RemoveClass(Element, tp.Classes.Visible);
        }
    },
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
     * Shows or hides the startup progress indicator.
     * @param {boolean} Flag True to show the progress indicator.
     * @returns {void}
     */
    SetStartupBusy: function (Flag) {
        var Element = tp("#AppStartupProgress");
        if (Element) {
            if (Flag === true)
                tp.AddClass(Element, tp.Classes.Visible);
            else
                tp.RemoveClass(Element, tp.Classes.Visible);
        }
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
        tp.CurrentUserIsAdmin = this.StartupInfo.IsAdmin === true;
        if (this.StartupInfo && !tp.IsBlankString(this.StartupInfo.ApplicationName))
            this.ApplicationName = this.StartupInfo.ApplicationName;
        return this.StartupInfo;
    },
    /**
     * Returns a user message for missing sample data versions.
     * @param {number[]} Versions The missing sample data versions.
     * @returns {string} Returns the message.
     */
    GetSampleDataMessage: function (Versions) {
        var Text = "The following versions of sample data are not added to the database yet.\n\n";
        Text += Versions.join("\n");
        Text += "\n\nDo you want to add those versions of sample data to the database?";
        return Text;
    },
    /**
     * Adds missing sample data versions.
     * @returns {Promise<object>} Returns the operation packet.
     */
    AddSampleDataAsync: async function () {
        return await tp.AjaxRequest.ExecuteAsync("App.AddSampleData");
    },
    /**
     * Ensures the user has a chance to add missing sample data before opening the main page.
     * @param {object} Info The startup information.
     * @returns {Promise<boolean>} Returns true when startup may continue.
     */
    EnsureSampleDataAsync: async function (Info) {
        var Versions = Info && tp.IsArray(Info.SampleDataVersions) ? Info.SampleDataVersions : [];
        var Confirmed;
        var Packet;
        var Message;

        if (Versions.length === 0)
            return true;

        this.SetStartupMessage("Sample data is missing.");
        Confirmed = await tp.YesNoBoxAsync(this.GetSampleDataMessage(Versions));
        if (Confirmed !== true) {
            this.SetStartupMessage("Sample data was not added.");
            return true;
        }

        this.SetStartupMessage("Adding sample data. Please wait...");
        this.SetStartupBusy(true);
        try {
            Packet = await this.AddSampleDataAsync();
        } finally {
            this.SetStartupBusy(false);
        }
        Message = Packet && Packet.Message ? Packet.Message : "Sample data added.";
        if (Packet && Packet.Success === true) {
            if (tp.IsFunction(tp.SuccessNote))
                tp.SuccessNote(Message);
            this.SetStartupMessage(Message);
            return true;
        }

        if (tp.IsFunction(tp.ErrorNote))
            tp.ErrorNote(Message);
        this.SetStartupMessage(Message);
        return false;
    },

    // ● dialogs
    /**
     * Shows the first run administrator dialog.
     * @param {object} Info The startup information.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null.
     */
    ShowFirstRunDialogAsync: async function (Info, Message) {
        var Dialog = null;
        try {
            Dialog = await this.CreateServerDialogAsync(Info.FirstRunHtml || "");
            return await Dialog.ShowAsync(Info, Message);
        } finally {
            this.ReleaseServerDialog(Dialog);
        }
    },
    /**
     * Shows the login dialog.
     * @param {object} Info The startup information.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null.
     */
    ShowLoginDialogAsync: async function (Info, Message) {
        var Dialog = null;
        try {
            Dialog = await this.CreateServerDialogAsync(Info.LoginHtml || "");
            return await Dialog.ShowAsync(Info, Message);
        } finally {
            this.ReleaseServerDialog(Dialog);
        }
    },
    /**
     * Opens the login dialog from the startup page.
     * @param {object} Info The startup information.
     * @param {string} Message The message text.
     * @returns {Promise<void>} Returns a Promise.
     */
    StartupLoginAsync: async function (Info, Message) {
        var DialogData = await this.ShowLoginDialogAsync(Info, Message);
        var Packet;
        var NoteText;
        if (DialogData === null) {
            this.SetStartupMessage("Login cancelled.");
            this.SetStartupLoginVisible(true);
            return;
        }
        Packet = await tp.AjaxRequest.ExecuteAsync("App.Login", DialogData);
        Message = Packet && Packet.Message ? Packet.Message : "";
        if (Packet && Packet.Success === true) {
            NoteText = Message || "Login succeeded.";
            if (tp.IsFunction(tp.SuccessNote))
                tp.SuccessNote(NoteText);
            this.SetStartupLoginVisible(false);
            await this.OpenMainPageAfterStartupAsync();
            return;
        }
        NoteText = Message || "Login failed.";
        if (tp.IsFunction(tp.ErrorNote))
            tp.ErrorNote(NoteText);
        this.SetStartupMessage(NoteText);
        this.SetStartupLoginVisible(true);
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
                    this.SetStartupMessage(Message || "Login is required.");
                    tp.On("#AppStartupLoginButton", tp.Events.Click, function () {
                        app.App.SetStartupLoginVisible(false);
                        app.App.StartupLoginAsync(Info, Message).catch(function (e) {
                            app.App.SetStartupMessage("Startup failed: " + tp.ExceptionText(e));
                            app.App.SetStartupLoginVisible(true);
                        });
                    });
                    this.SetStartupLoginVisible(false);
                    this.StartupLoginAsync(Info, Message).catch(function (e) {
                        app.App.SetStartupMessage("Startup failed: " + tp.ExceptionText(e));
                        app.App.SetStartupLoginVisible(true);
                    });
                    return;
                }

                await this.OpenMainPageAfterStartupAsync(Info);
                return;
            }
        } catch (e) {
            this.SetStartupMessage("Startup failed: " + tp.ExceptionText(e));
        }
    },
    /**
     * Completes startup checks and opens the main page.
     * @param {object|null|undefined} Info Optional startup information.
     * @returns {Promise<void>} Returns a Promise.
     */
    OpenMainPageAfterStartupAsync: async function (Info) {
        var CanContinue;

        Info = Info || await this.LoadStartupInfoAsync();
        CanContinue = await this.EnsureSampleDataAsync(Info);
        if (CanContinue !== true)
            return;

        this.SetStartupMessage("Opening main page...");
        setTimeout(function () {
            tp.NavigateTo("/Home/MainPage");
        }, 600);
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
     * Returns the application settings dialog HTML.
     * @param {string} Scope The selected config scope.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    GetApplicationSettingsDialogAsync: async function (Scope) {
        return await tp.AjaxRequest.ExecuteAsync("App.GetApplicationSettingsDialog", {
            Scope: Scope || "User"
        });
    },
    /**
     * Returns the change password dialog HTML.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    GetChangePasswordDialogAsync: async function () {
        return await tp.AjaxRequest.ExecuteAsync("App.GetChangePasswordDialog");
    },
    /**
     * Returns the connection info dialog HTML and metadata.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    GetConnectionInfoDialogAsync: async function () {
        return await tp.AjaxRequest.ExecuteAsync("App.GetConnectionInfoDialog");
    },
    /**
     * Returns a connection string preview.
     * @param {object} Data The connection info data.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    GetConnectionInfoPreviewAsync: async function (Data) {
        return await tp.AjaxRequest.ExecuteAsync("App.GetConnectionInfoPreview", Data || {});
    },
    /**
     * Tests database connection information.
     * @param {object} Data The connection info data.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    TestConnectionInfoAsync: async function (Data) {
        return await tp.AjaxRequest.ExecuteAsync("App.TestConnectionInfo", Data || {});
    },
    /**
     * Changes the current user password.
     * @param {object} Data The password data.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    ChangePasswordAsync: async function (Data) {
        return await tp.AjaxRequest.ExecuteAsync("App.ChangePassword", Data || {});
    },
    /**
     * Saves database connection information.
     * @param {object} Data The connection info data.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    SaveConnectionInfoAsync: async function (Data) {
        return await tp.AjaxRequest.ExecuteAsync("App.SaveConnectionInfo", Data || {});
    },
    /**
     * Saves application settings.
     * @param {string} Scope The selected config scope.
     * @param {object} Values The settings values.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    SaveApplicationSettingsAsync: async function (Scope, Values) {
        return await tp.AjaxRequest.ExecuteAsync("App.SaveApplicationSettings", {
            Scope: Scope || "User",
            Values: Values || {}
        });
    },
    /**
     * Logs out the current user.
     * @returns {Promise<object>} Returns a Promise with the server packet.
     */
    LogoutAsync: async function () {
        return await tp.AjaxRequest.ExecuteAsync("App.Logout");
    },

    // ● server dialogs
    /**
     * Creates a dialog helper from server-rendered dialog HTML.
     * @param {string} Html The server-rendered dialog HTML.
     * @returns {Promise<object>} Returns a Promise with the dialog helper instance.
     */
    CreateServerDialogAsync: async function (Html) {
        var Element = tp.HtmlToElement(Html || "");
        var Params;
        var CssFiles;
        var JavaScriptFiles;
        var ClassName;
        var Type;
        var Dialog;

        if (!(Element instanceof HTMLElement))
            throw new Error("Cannot create server dialog. No root element found.");

        Params = new tp.CreateParams(tp.GetDataSetupObject(Element) || {});
        CssFiles = tp.IsArray(Params.CssFiles) ? Params.CssFiles : (tp.IsArray(Params.CSS) ? Params.CSS : []);
        JavaScriptFiles = tp.IsArray(Params.JavaScriptFiles) ? Params.JavaScriptFiles : (tp.IsArray(Params.JS) ? Params.JS : []);
        ClassName = Params.ClassName || Params.DialogClassName || Params.SetupClass || "";

        await tp.StaticFiles.LoadCssFiles(CssFiles);
        await tp.StaticFiles.LoadJavascriptFiles(JavaScriptFiles);

        try {
            Type = tp.WebForm.ResolveGlobalName(ClassName);
            if (!tp.IsFunction(Type))
                throw new Error("Cannot create server dialog. No JavaScript class type is specified.");
            Dialog = new Type({
                Html: Html,
                RootElement: Element,
                Params: Params,
                CssFiles: CssFiles,
                JavaScriptFiles: JavaScriptFiles
            });
            if (!tp.IsFunction(Dialog.ShowAsync))
                throw new Error("Cannot create server dialog. The specified class has no ShowAsync() method.");
            return Dialog;
        } catch (e) {
            tp.StaticFiles.UnLoadJavascriptFiles(JavaScriptFiles);
            tp.StaticFiles.UnLoadCssFiles(CssFiles);
            throw e;
        }
    },
    /**
     * Releases static files loaded for a server dialog.
     * @param {object|null|undefined} Dialog The server dialog helper.
     * @returns {void}
     */
    ReleaseServerDialog: function (Dialog) {
        if (!Dialog || !Dialog.Params)
            return;
        tp.StaticFiles.UnLoadJavascriptFiles(Dialog.Params.JavaScriptFiles || []);
        tp.StaticFiles.UnLoadCssFiles(Dialog.Params.CssFiles || []);
    },
    /**
     * Shows the application settings dialog.
     * @returns {Promise<void>} Returns a Promise.
     */
    ShowApplicationSettingsDialogAsync: async function () {
        var Packet;
        var Dialog = null;
        try {
            Packet = await this.GetApplicationSettingsDialogAsync("User");
            Dialog = await this.CreateServerDialogAsync(Packet.Html || "");
            await Dialog.ShowAsync(Packet);
        } finally {
            this.ReleaseServerDialog(Dialog);
        }
    },
    /**
     * Shows the application settings dialog and logs failures without throwing.
     * @returns {void}
     */
    ShowApplicationSettingsDialog: function () {
        this.ShowApplicationSettingsDialogAsync().catch(function (e) {
            var Text = "Application settings failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        });
    },
    /**
     * Shows the change password dialog.
     * @returns {Promise<void>} Returns a Promise.
     */
    ShowChangePasswordDialogAsync: async function () {
        var Packet;
        var Dialog = null;
        try {
            Packet = await this.GetChangePasswordDialogAsync();
            Dialog = await this.CreateServerDialogAsync(Packet.Html || "");
            await Dialog.ShowAsync(Packet);
        } finally {
            this.ReleaseServerDialog(Dialog);
        }
    },
    /**
     * Shows the change password dialog and logs failures without throwing.
     * @returns {void}
     */
    ShowChangePasswordDialog: function () {
        this.ShowChangePasswordDialogAsync().catch(function (e) {
            var Text = "Change password failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        });
    },
    /**
     * Shows the connection info dialog.
     * @returns {Promise<void>} Returns a Promise.
     */
    ShowConnectionInfoDialogAsync: async function () {
        var Packet;
        var Dialog = null;
        try {
            Packet = await this.GetConnectionInfoDialogAsync();
            Dialog = await this.CreateServerDialogAsync(Packet.Html || "");
            await Dialog.ShowAsync(Packet);
        } finally {
            this.ReleaseServerDialog(Dialog);
        }
    },
    /**
     * Shows the connection info dialog and logs failures without throwing.
     * @returns {void}
     */
    ShowConnectionInfoDialog: function () {
        this.ShowConnectionInfoDialogAsync().catch(function (e) {
            var Text = "Connection info failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        });
    },
    /**
     * Deletes the sample SQLite database on the server.
     * @returns {Promise<object>} Returns the operation packet.
     */
    RegenerateDatabaseAsync: async function () {
        var Message = "This will delete and recreate the sample Sqlite database.\n\nContinue?";
        var Confirmed = await tp.YesNoBoxAsync(Message);
        if (Confirmed !== true)
            return null;
        return await tp.AjaxRequest.ExecuteAsync("App.RegenerateDatabase");
    },
    /**
     * Deletes the sample SQLite database and reports the result.
     * @returns {void}
     */
    RegenerateDatabase: function () {
        this.RegenerateDatabaseAsync().then(function (Packet) {
            var Text;
            if (Packet === null)
                return;
            Text = Packet.Message || "The sample Sqlite database has been deleted. Restart the tERPWeb server process.";
            if (tp.IsFunction(tp.InfoBox))
                tp.InfoBox(Text);
            if (tp.IsFunction(tp.InfoNote))
                tp.InfoNote(Text);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        }).catch(function (e) {
            var Text = "Regenerate database failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (tp.IsFunction(tp.ErrorNote))
                tp.ErrorNote(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        });
    },
    /**
     * Toggles SQL statement logging on the server.
     * @returns {Promise<object>} Returns the operation packet.
     */
    ToggleLogSqlAsync: async function () {
        return await tp.AjaxRequest.ExecuteAsync("App.ToggleLogSql");
    },
    /**
     * Toggles SQL statement logging and reports the result.
     * @returns {void}
     */
    ToggleLogSql: function () {
        this.ToggleLogSqlAsync().then(function (Packet) {
            var Text = Packet && Packet.Message ? Packet.Message : "SQL Statements Logging changed.";
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
            if (tp.IsFunction(tp.InfoNote))
                tp.InfoNote(Text);
        }).catch(function (e) {
            var Text = "Log Sql failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
            if (tp.IsFunction(tp.ErrorNote))
                tp.ErrorNote(Text);
        });
    },
    /**
     * Clears the application log.
     * @returns {void}
     */
    ClearLog: function () {
        if (tp.LogBox)
            tp.LogBox.Clear();
        if (this.MainPage && this.MainPage.StatusBar)
            this.MainPage.StatusBar.Message = "Log cleared.";
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
            if (tp.IsBlankString(Form.JsFormClassType))
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
        else if (Command.Name === "Application Settings")
            this.ShowApplicationSettingsDialog();
        else if (Command.Name === "Change Password")
            this.ShowChangePasswordDialog();
        else if (Command.Name === "ConnectionInfo")
            this.ShowConnectionInfoDialog();
        else if (Command.Name === "Regenerate Database")
            this.RegenerateDatabase();
        else if (Command.Name === "Close")
            this.CloseApplication();
        else if (Command.Name === "Toggle Log" && this.MainPage)
            this.MainPage.ToggleLog();
        else if (Command.Name === "Clear Log")
            this.ClearLog();
        else if (Command.Name === "Log Sql")
            this.ToggleLogSql();
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
    /**
     * Logs out and returns to the startup page.
     * @returns {void}
     */
    CloseApplication: function () {
        this.LogoutAsync().then(function () {
            tp.NavigateTo("/Home/Startup");
        }).catch(function (e) {
            var Text = "Close failed: " + tp.ExceptionText(e);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Text);
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Text;
        });
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
        if (Command.Name === "Application Settings")
            return "fa fa-screwdriver-wrench";
        if (Command.Name === "Close")
            return "fa fa-right-from-bracket";
        if (Command.Name === "Change Password")
            return "fa fa-key";
        if (Command.Name === "ConnectionInfo")
            return "fa fa-database";
        if (Command.Name === "Database Workbench")
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
        this.LoadStartupInfoAsync().then(function (Info) {
            if (app.App.MainPage)
                app.App.MainPage.UpdateStatusInfo(Info);
        }).catch(function (e) {
            if (tp.LogBox)
                tp.LogBox.AppendLine("Load startup info failed: " + tp.ExceptionText(e));
        });
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
