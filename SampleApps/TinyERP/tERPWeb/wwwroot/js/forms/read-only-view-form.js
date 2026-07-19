/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● read-only view form
/**
 * Displays a registered read-only application view.
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
app.ReadOnlyViewForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates a read-only view form.
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
         * The selected read-only view name.
         * @type {string}
         */
        this.ViewName = "";
        /**
         * The selected read-only view title.
         * @type {string}
         */
        this.ViewTitle = "";
        /**
         * The toolbar.
         * @type {tp.ToolBar|null}
         */
        this.ToolBar = null;
        /**
         * The result grid.
         * @type {tp.Grid|null}
         */
        this.Grid = null;
        /**
         * The filter pane element.
         * @type {HTMLElement|null}
         */
        this.elFilterPane = null;
        /**
         * The filter panel list element.
         * @type {HTMLElement|null}
         */
        this.elFilterPanelList = null;
        /**
         * The active filter rows.
         * @type {tp.SelectFilterRow[]}
         */
        this.FilterRows = [];
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, "app-read-only-view");
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.ResolveElements();
        this.CreateControls();
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
            if (!tp.IsNil(Options.ViewName))
                this.ViewName = String(Options.ViewName);
            if (!tp.IsNil(Options.Title))
                this.ViewTitle = String(Options.Title);
        }
    }
    /**
     * Called just before form initialization.
     * @returns {void}
     */
    FormInitializing() {
        super.FormInitializing();
        if (!tp.IsBlankString(this.ViewTitle))
            this.TitleText = this.ViewTitle;
    }
    /**
     * Called just after form initialization.
     * @returns {void}
     */
    FormInitialized() {
        super.FormInitialized();
        this.ShowActiveFilterPanel();
    }
    /**
     * Resolves important DOM elements.
     * @returns {void}
     */
    ResolveElements() {
        this.elFilterPane = this.Handle.querySelector("[data-role='filters']");
        this.elFilterPanelList = this.Handle.querySelector("[data-role='filter-panel-list']");
    }
    /**
     * Creates toolbar and grid controls.
     * @returns {void}
     */
    CreateControls() {
        var ToolBarElement = this.Handle.querySelector("[data-role='toolbar']");
        var GridElement = this.Handle.querySelector("[data-role='grid']");
        var Button;
        if (ToolBarElement instanceof HTMLElement) {
            this.ToolBar = new tp.ToolBar(ToolBarElement);
            Button = this.ToolBar.AddButton("Refresh", tp._L("Refresh", "Refresh"), tp._L("Refresh", "Refresh"), "", "", false);
            Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "table_refresh.png" });
            Button = this.ToolBar.AddButton("Find", tp._L("Find", "Find"), tp._L("Find", "Find"), "", "", false);
            Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "lightning.png" });
            Button = this.ToolBar.AddButton("Clear", tp._L("Clear", "Clear"), tp._L("Clear", "Clear"), "", "", false);
            Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "bin.png" });
            Button = this.ToolBar.AddButton("Filters", tp._L("Filters", "Filters"), tp._L("Filters", "Filters"), "", "", false);
            Button.ImageUrl = app.App.GetCommandImageUrl({ ImageFileName: "setting_tools.png" });
            this.ToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
        }
        if (GridElement instanceof HTMLElement) {
            this.Grid = new tp.Grid({
                ElementOrSelector: GridElement,
                AutoGenerateColumns: true
            });
            this.Grid.ReadOnly = true;
        }
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "Refresh" || Args.Command === "Find")
            this.LoadData();
        else if (Args.Command === "Clear") {
            this.ClearFilters();
            this.LoadData();
        } else if (Args.Command === "Filters")
            this.ToggleFilters();
    }
    /**
     * Shows only the active view filter panel.
     * @returns {void}
     */
    ShowActiveFilterPanel() {
        var Index;
        var Panel;
        var Panels;
        var Rows;
        var Row;
        this.FilterRows.length = 0;
        if (!(this.elFilterPanelList instanceof HTMLElement))
            return;
        Panels = this.elFilterPanelList.children;
        for (Index = 0; Index < Panels.length; Index++) {
            Panel = Panels[Index];
            if (!(Panel instanceof HTMLElement))
                continue;
            Panel.hidden = !tp.IsSameText(Panel.getAttribute("data-select-name") || "", this.ViewName);
        }
        Rows = this.elFilterPanelList.querySelectorAll("[data-select-name='" + this.ViewName + "'] ." + tp.Classes.SelectFilterRow);
        for (Index = 0; Index < Rows.length; Index++) {
            Row = tp.GetComponent(Rows[Index]);
            if (!(Row instanceof tp.SelectFilterRow))
                Row = new tp.SelectFilterRow({ ElementOrSelector: Rows[Index] });
            this.FilterRows.push(Row);
        }
        if (this.elFilterPane instanceof HTMLElement)
            this.elFilterPane.hidden = this.FilterRows.length === 0;
    }
    /**
     * Returns active structured filter values.
     * @returns {object[]} Returns the active filters.
     */
    GetActiveFilters() {
        var Result = [];
        var Index;
        var Filter;
        for (Index = 0; Index < this.FilterRows.length; Index++) {
            Filter = this.FilterRows[Index].GetActiveFilter();
            if (Filter)
                Result.push(Filter);
        }
        return Result;
    }
    /**
     * Executes an async operation while showing the global spinner.
     * @param {Function} Func The async function to execute.
     * @returns {Promise<*>} Returns the function result.
     */
    async ExecuteWithSpinner(Func) {
        var ShowSpinner = tp.IsFunction(tp.ShowSpinner);
        if (ShowSpinner)
            tp.ShowSpinner(true);
        try {
            return await Func.call(this);
        } finally {
            if (ShowSpinner)
                tp.ShowSpinner(false);
        }
    }

    // ● public
    /**
     * Loads the read-only view data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
        var Packet;
        var Table;
        if (tp.IsBlankString(this.ViewName) || !(this.Grid instanceof tp.Grid))
            return;
        Packet = await this.ExecuteWithSpinner(async function () {
            return await app.App.SelectReadOnlyViewAsync(this.ViewName, this.GetActiveFilters());
        });
        if (Packet && tp.IsObject(Packet.Table)) {
            Table = new tp.DataTable(Packet.Table);
            this.Grid.DataSource = Table;
            this.Grid.ShowIdGridColumns(false);
            this.Grid.BestFitColumns();
        }
        if (tp.LogBox)
            tp.LogBox.AppendLine(tp._L("ReadOnlyViewSelected", "Read-only view selected") + ": " + this.ViewName);
    }
    /**
     * Toggles the filter pane.
     * @returns {void}
     */
    ToggleFilters() {
        if (this.elFilterPane instanceof HTMLElement)
            this.elFilterPane.hidden = !this.elFilterPane.hidden;
    }
    /**
     * Clears active filters.
     * @returns {void}
     */
    ClearFilters() {
        var Index;
        for (Index = 0; Index < this.FilterRows.length; Index++)
            this.FilterRows[Index].Clear();
    }
};

// ● prototype
/**
 * The selected read-only view name.
 * @type {string}
 */
app.ReadOnlyViewForm.prototype.ViewName = "";
/**
 * The selected read-only view title.
 * @type {string}
 */
app.ReadOnlyViewForm.prototype.ViewTitle = "";
/**
 * The toolbar.
 * @type {tp.ToolBar|null}
 */
app.ReadOnlyViewForm.prototype.ToolBar = null;
/**
 * The result grid.
 * @type {tp.Grid|null}
 */
app.ReadOnlyViewForm.prototype.Grid = null;
/**
 * The filter pane element.
 * @type {HTMLElement|null}
 */
app.ReadOnlyViewForm.prototype.elFilterPane = null;
/**
 * The filter panel list element.
 * @type {HTMLElement|null}
 */
app.ReadOnlyViewForm.prototype.elFilterPanelList = null;
/**
 * The active filter rows.
 * @type {tp.SelectFilterRow[]|null}
 */
app.ReadOnlyViewForm.prototype.FilterRows = null;
