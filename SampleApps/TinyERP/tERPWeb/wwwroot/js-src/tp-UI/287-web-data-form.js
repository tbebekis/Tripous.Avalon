// ● web data form state
/**
 * Indicates the current state of a WebDesk data form.
 * @enum {string}
 */
tp.WebDataFormState = {
    None: "None",
    List: "List",
    Insert: "Insert",
    Edit: "Edit"
};
Object.freeze(tp.WebDataFormState);

// ● web data form
/**
 * Standard WebDesk data-entry form.
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
tp.WebDataForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates a WebDesk data form.
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
        tp.AddClass(this.Handle, tp.Classes.WebDataForm);
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * The server-side module name.
         * @type {string}
         */
        this.ModuleName = "";
        /**
         * True when the form is read-only.
         * @type {boolean}
         */
        this.IsReadOnly = false;
        /**
         * The data module proxy.
         * @type {tp.DataModule|null}
         */
        this.Module = null;
        /**
         * The current form state.
         * @type {string}
         */
        this.FormState = tp.WebDataFormState.None;
        /**
         * The main toolbar.
         * @type {tp.ControlBar|null}
         */
        this.ToolBar = null;
        /**
         * The select toolbar.
         * @type {tp.ControlBar|null}
         */
        this.SelectBar = null;
        /**
         * The select list combo box.
         * @type {tp.ComboBox|null}
         */
        this.SelectCombo = null;
        /**
         * The filter panel list.
         * @type {tp.PanelList|null}
         */
        this.FilterPanelList = null;
        /**
         * The list grid.
         * @type {tp.Grid|null}
         */
        this.ListGrid = null;
        /**
         * The filter pane element.
         * @type {HTMLElement|null}
         */
        this.FilterPane = null;
        /**
         * The filter splitter.
         * @type {tp.Splitter|null}
         */
        this.FilterSplitter = null;
        /**
         * True when the filter sidebar is visible.
         * @type {boolean}
         */
        this.SidebarVisible = false;
        /**
         * The list page element.
         * @type {HTMLElement|null}
         */
        this.ListPage = null;
        /**
         * The item page element.
         * @type {HTMLElement|null}
         */
        this.ItemPage = null;
        /**
         * Toolbar buttons keyed by command.
         * @type {object}
         */
        this.Buttons = {};
    }
    /**
     * Notification called after field initialization and before create params are applied.
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
        if (Params) {
            if (!tp.IsNil(Params.ModuleName))
                this.ModuleName = Params.ModuleName || "";
            if (!tp.IsNil(Params.Module))
                this.ModuleName = Params.Module || this.ModuleName;
            if (!tp.IsNil(Params.IsReadOnly))
                this.IsReadOnly = Params.IsReadOnly === true;
        }
    }
    /**
     * Called just after the context is assigned.
     * @returns {void}
     */
    SetupContext() {
        var Form;
        super.SetupContext();
        Form = this.Context instanceof tp.WebFormContext ? this.Context.WebFormDef : null;
        if (Form) {
            this.ModuleName = Form.Module || this.ModuleName || Form.Name || "";
            this.IsReadOnly = Form.IsReadOnly === true;
        }
    }
    /**
     * Executes the first form operation.
     * @returns {Promise<void>} Returns a Promise.
     */
    async StartAsync() {
        await this.InitializeDataModuleAsync();
        await this.SelectListAsync();
    }
    /**
     * Releases owned controls.
     * @returns {void}
     */
    DoDispose() {
        this.Buttons = {};
        this.ToolBar = null;
        this.SelectBar = null;
        this.SelectCombo = null;
        this.FilterPanelList = null;
        this.ListGrid = null;
        this.FilterPane = null;
        this.FilterSplitter = null;
        this.ListPage = null;
        this.ItemPage = null;
        this.Module = null;
        super.DoDispose();
    }
    /**
     * Creates child controls.
     * @returns {void}
     */
    CreateControls() {
        var ToolBarElement = this.FindRoleElement("toolbar");
        var SelectBarElement = this.FindRoleElement("select-bar");
        var FilterPanelListElement = this.FindRoleElement("filter-panel-list");
        var FilterSplitterElement = this.FindRoleElement("filter-splitter");
        var GridElement = this.FindRoleElement("list-grid");
        this.FilterPane = this.FindRoleElement("filters");
        this.ListPage = this.FindRoleElement("list-page");
        this.ItemPage = this.FindRoleElement("item-page");
        if (ToolBarElement instanceof HTMLElement)
            this.CreateToolBar(ToolBarElement);
        if (SelectBarElement instanceof HTMLElement)
            this.CreateSelectBar(SelectBarElement);
        if (FilterPanelListElement instanceof HTMLElement)
            this.CreateFilterPanelList(FilterPanelListElement);
        if (FilterSplitterElement instanceof HTMLElement)
            this.CreateFilterSplitter(FilterSplitterElement);
        if (GridElement instanceof HTMLElement)
            this.CreateListGrid(GridElement);
        this.SetFilterPaneVisible(false);
        this.ShowListPage();
    }
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        this.ToolBar = new tp.ControlBar(Element);
        tp.AddClass(this.ToolBar.Handle, tp.Classes.WebDataFormToolBar);
        this.AddToolBarButton("Home", "Home", "application_home.png");
        this.ToolBar.AddSeparator("HomeSeparator");
        this.AddToolBarButton("List", "List (F5)", "table.png");
        this.AddToolBarButton("RefreshList", "Refresh List (Ctrl+F5)", "table_refresh.png");
        this.AddToolBarButton("Find", "Find (Ctrl+F)", "find.png");
        this.AddToolBarButton("ToggleIds", "Toggle Ids", "table_select_row.png");
        this.ToolBar.AddSeparator("ListSeparator");
        this.AddToolBarButton("Insert", "Insert (Ctrl+Insert)", "table_add.png");
        this.AddToolBarButton("Edit", "Edit (Ctrl+Enter)", "table_edit.png");
        this.AddToolBarButton("Delete", "Delete (Ctrl+Delete)", "table_delete.png");
        this.AddToolBarButton("Refresh", "Refresh Item", "table_refresh.png");
        this.ToolBar.AddSeparator("EditSeparator");
        this.AddToolBarButton("Save", "Save (Ctrl+S)", "disk.png");
        this.ToolBar.AddSeparator("SaveSeparator");
        this.AddToolBarButton("Cancel", "Cancel (Escape)", "cancel.png");
        this.AddToolBarButton("Ok", "OK (Ctrl+Enter)", "accept.png");
        this.ToolBar.AddSeparator("CancelOkSeparator");
        this.AddToolBarButton("Close", "Close", "door_out.png");
        this.ToolBar.On("ButtonClick", this.HandleToolBarButtonClick, this);
        this.UpdateToolBar();
    }
    /**
     * Adds a toolbar button.
     * @param {string} Command The command.
     * @param {string} ToolTip The tooltip.
     * @param {string} ImageFileName The image file name.
     * @returns {tp.ButtonEx} Returns the created button.
     */
    AddToolBarButton(Command, ToolTip, ImageFileName) {
        var Button = this.ToolBar.AddButton(Command, Command, null, ToolTip, "", "", this.GetToolBarImageUrl(ImageFileName));
        this.Buttons[Command] = Button;
        return Button;
    }
    /**
     * Creates the select list toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateSelectBar(Element) {
        this.SelectBar = new tp.ControlBar(Element);
        tp.AddClass(this.SelectBar.Handle, tp.Classes.WebDataFormSelectBar);
        this.SelectCombo = this.SelectBar.AddComboBox([], 0, 180);
        this.SelectBar.AddButton("Execute", "Execute", null, "Execute", "", "", this.GetToolBarImageUrl("lightning.png"));
        this.SelectBar.AddButton("ClearFilter", "Clear Filter", null, "Clear Filter", "", "", this.GetToolBarImageUrl("textfield_clear.png"));
        this.SelectBar.On("ButtonClick", this.HandleSelectBarButtonClick, this);
    }
    /**
     * Creates the filter panel list.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateFilterPanelList(Element) {
        this.FilterPanelList = new tp.PanelList(Element);
        tp.AddClass(this.FilterPanelList.Handle, tp.Classes.WebDataFormFilterPanelList);
    }
    /**
     * Creates the filter splitter.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateFilterSplitter(Element) {
        this.FilterSplitter = new tp.Splitter(Element);
        this.FilterSplitter.Panel1MinSize = 260;
        this.FilterSplitter.Panel1MaxSize = 640;
        this.FilterSplitter.Panel2MinSize = 180;
        tp.AddClass(this.FilterSplitter.Handle, tp.Classes.WebDataFormFilterSplitter);
    }
    /**
     * Creates the list grid.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateListGrid(Element) {
        this.ListGrid = new tp.Grid({
            ElementOrSelector: Element,
            AutoGenerateColumns: true
        });
        tp.AddClass(this.ListGrid.Handle, tp.Classes.WebDataFormGrid);
    }
    /**
     * Finds an element by data-role.
     * @param {string} Role The role name.
     * @returns {HTMLElement|null} Returns the element or null.
     */
    FindRoleElement(Role) {
        return this.Handle.querySelector("[data-role='" + Role + "']");
    }
    /**
     * Returns an image URL for a toolbar image.
     * @param {string|null|undefined} ImageFileName The image file name.
     * @returns {string} Returns the image URL.
     */
    GetToolBarImageUrl(ImageFileName) {
        if (tp.IsBlankString(ImageFileName))
            return "";
        return "/images/toolbar/" + ImageFileName;
    }
    /**
     * Initializes the data module.
     * @returns {Promise<void>} Returns a Promise.
     */
    async InitializeDataModuleAsync() {
        if (tp.IsBlankString(this.ModuleName))
            throw new Error("No DataModule name specified for WebDataForm.");
        this.Module = new tp.DataModule(this.ModuleName);
        await this.Module.Initialize();
        this.FillSelectCombo();
        this.BuildFilterPanels();
        this.FormState = tp.WebDataFormState.List;
        this.UpdateToolBar();
    }
    /**
     * Fills the select combo box from available query names.
     * @returns {void}
     */
    FillSelectCombo() {
        var Names = this.Module && tp.IsArray(this.Module.QueryNames) ? this.Module.QueryNames.slice() : [];
        if (Names.length === 0)
            Names.push("Default");
        if (this.SelectCombo instanceof tp.ComboBox) {
            this.SelectCombo.Items = Names;
            this.SelectCombo.SelectedIndex = 0;
        }
    }
    /**
     * Builds the filter panel list from available select names.
     * @returns {void}
     */
    BuildFilterPanels() {
        var Names = this.Module && tp.IsArray(this.Module.QueryNames) ? this.Module.QueryNames.slice() : [];
        var Index;
        var Panel;
        if (!(this.FilterPanelList instanceof tp.PanelList))
            return;
        if (Names.length === 0)
            Names.push("Default");
        this.FilterPanelList.Associate = null;
        tp.RemoveChildren(this.FilterPanelList.Handle);
        for (Index = 0; Index < Names.length; Index++) {
            Panel = this.FilterPanelList.AddPanel();
            if (Panel instanceof HTMLElement)
                this.BuildFilterPanel(Panel, Names[Index]);
        }
        if (this.SelectCombo instanceof tp.ComboBox)
            this.FilterPanelList.Associate = this.SelectCombo;
        this.FilterPanelList.SelectedIndex = this.SelectCombo instanceof tp.ComboBox ? this.SelectCombo.SelectedIndex : 0;
    }
    /**
     * Builds a single filter panel.
     * @param {HTMLElement} Panel The panel element.
     * @param {string} SelectName The select name.
     * @returns {void}
     */
    BuildFilterPanel(Panel, SelectName) {
        var Title = Panel.ownerDocument.createElement("div");
        var Empty = Panel.ownerDocument.createElement("div");
        tp.AddClass(Title, tp.Classes.Title);
        Title.textContent = SelectName || "";
        Empty.textContent = "No filters";
        Panel.appendChild(Title);
        Panel.appendChild(Empty);
    }
    /**
     * Returns the selected select name.
     * @returns {string} Returns the selected select name.
     */
    GetSelectedSelectName() {
        if (this.SelectCombo instanceof tp.ComboBox && !tp.IsBlankString(this.SelectCombo.SelectedValue))
            return String(this.SelectCombo.SelectedValue);
        return this.Module && this.Module.QueryNames.length > 0 ? this.Module.QueryNames[0] : "";
    }
    /**
     * Returns the current filter WHERE text.
     * @returns {string} Returns the WHERE text.
     */
    GetWhereText() {
        return "";
    }
    /**
     * Selects and displays the list table.
     * @returns {Promise<void>} Returns a Promise.
     */
    async SelectListAsync() {
        var SelectName;
        var WhereText;
        var Table;
        if (!(this.Module instanceof tp.DataModule))
            return;
        SelectName = this.GetSelectedSelectName();
        WhereText = this.GetWhereText();
        await this.Module.SelectList(SelectName, WhereText);
        Table = this.Module.tblList;
        if (this.ListGrid instanceof tp.Grid && Table) {
            this.ListGrid.DataSource = Table;
            this.RefreshListGridLayout(true);
        }
        this.FormState = tp.WebDataFormState.List;
        this.ShowListPage();
        this.UpdateToolBar();
    }
    /**
     * Shows the list page.
     * @returns {void}
     */
    ShowListPage() {
        if (this.ListPage instanceof HTMLElement)
            this.ListPage.hidden = false;
        if (this.ItemPage instanceof HTMLElement)
            this.ItemPage.hidden = true;
    }
    /**
     * Shows the item page.
     * @returns {void}
     */
    ShowItemPage() {
        if (this.ListPage instanceof HTMLElement)
            this.ListPage.hidden = true;
        if (this.ItemPage instanceof HTMLElement)
            this.ItemPage.hidden = false;
    }
    /**
     * Shows or hides the filter pane.
     * @returns {void}
     */
    ToggleFilterPane() {
        this.SetFilterPaneVisible(!this.SidebarVisible);
    }
    /**
     * Shows or hides the filter pane and its splitter.
     * @param {boolean} Visible True to show.
     * @returns {void}
     */
    SetFilterPaneVisible(Visible) {
        var IsVisible = Visible === true;
        this.SidebarVisible = IsVisible;
        if (this.FilterPane instanceof HTMLElement) {
            this.FilterPane.hidden = !IsVisible;
            this.FilterPane.style.display = IsVisible ? "" : "none";
        }
        if (this.FilterSplitter instanceof tp.Splitter) {
            this.FilterSplitter.Handle.hidden = !IsVisible;
            this.FilterSplitter.Handle.style.display = IsVisible ? "" : "none";
        }
        this.RefreshListGridLayout(false);
    }
    /**
     * Refreshes the list grid after container size changes.
     * @param {boolean} BestFit True to best-fit columns too.
     * @returns {void}
     */
    RefreshListGridLayout(BestFit) {
        setTimeout(() => {
            if (this.ListGrid instanceof tp.Grid && !this.ListGrid.IsDisposed) {
                this.ListGrid.RefreshLayout();
                if (BestFit === true)
                    this.ListGrid.BestFitColumns();
            }
        }, 0);
    }
    /**
     * Handles main toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "Home")
            this.ShowListPage();
        else if (Command === "List")
            this.ShowListPage();
        else if (Command === "RefreshList")
            this.SelectListAsync();
        else if (Command === "Find")
            this.ToggleFilterPane();
        else if (Command === "Close")
            this.CloseForm();
    }
    /**
     * Handles select toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleSelectBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "Execute")
            this.SelectListAsync();
        else if (Command === "ClearFilter")
            this.ClearFilters();
    }
    /**
     * Clears filter controls.
     * @returns {void}
     */
    ClearFilters() {
    }
    /**
     * Updates toolbar state.
     * @returns {void}
     */
    UpdateToolBar() {
        this.SetButtonEnabled("List", true);
        this.SetButtonEnabled("RefreshList", true);
        this.SetButtonEnabled("Find", this.Module instanceof tp.DataModule && this.Module.UseFilters === true);
        this.SetButtonVisible("Ok", false);
        this.SetButtonEnabled("Home", false);
        this.SetButtonEnabled("ToggleIds", true);
        this.SetButtonEnabled("Insert", false);
        this.SetButtonEnabled("Edit", false);
        this.SetButtonEnabled("Delete", false);
        this.SetButtonEnabled("Refresh", false);
        this.SetButtonEnabled("Save", false);
        this.SetButtonEnabled("Cancel", false);
        this.SetButtonEnabled("Ok", false);
        this.SetButtonEnabled("Close", true);
    }
    /**
     * Enables or disables a toolbar button.
     * @param {string} Command The button command.
     * @param {boolean} Enabled True to enable.
     * @returns {void}
     */
    SetButtonEnabled(Command, Enabled) {
        var Button = this.Buttons[Command];
        if (Button instanceof tp.Component)
            Button.Enabled = Enabled === true;
    }
    /**
     * Shows or hides a toolbar button.
     * @param {string} Command The button command.
     * @param {boolean} Visible True to show.
     * @returns {void}
     */
    SetButtonVisible(Command, Visible) {
        var Button = this.Buttons[Command];
        if (Button instanceof tp.Component)
            Button.Visible = Visible === true;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.WebDataForm.prototype.tpClass = "tp.WebDataForm";
/**
 * The server-side module name.
 * @type {string}
 */
tp.WebDataForm.prototype.ModuleName = "";
/**
 * True when the form is read-only.
 * @type {boolean}
 */
tp.WebDataForm.prototype.IsReadOnly = false;
/**
 * The data module proxy.
 * @type {tp.DataModule|null}
 */
tp.WebDataForm.prototype.Module = null;
/**
 * The current form state.
 * @type {string}
 */
tp.WebDataForm.prototype.FormState = tp.WebDataFormState.None;
/**
 * The main toolbar.
 * @type {tp.ControlBar|null}
 */
tp.WebDataForm.prototype.ToolBar = null;
/**
 * The select toolbar.
 * @type {tp.ControlBar|null}
 */
tp.WebDataForm.prototype.SelectBar = null;
/**
 * The select list combo box.
 * @type {tp.ComboBox|null}
 */
tp.WebDataForm.prototype.SelectCombo = null;
/**
 * The filter panel list.
 * @type {tp.PanelList|null}
 */
tp.WebDataForm.prototype.FilterPanelList = null;
/**
 * The list grid.
 * @type {tp.Grid|null}
 */
tp.WebDataForm.prototype.ListGrid = null;
/**
 * The filter pane element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.FilterPane = null;
/**
 * The filter splitter.
 * @type {tp.Splitter|null}
 */
tp.WebDataForm.prototype.FilterSplitter = null;
/**
 * True when the filter sidebar is visible.
 * @type {boolean}
 */
tp.WebDataForm.prototype.SidebarVisible = false;
/**
 * The list page element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.ListPage = null;
/**
 * The item page element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.ItemPage = null;
/**
 * Toolbar buttons keyed by command.
 * @type {object|null}
 */
tp.WebDataForm.prototype.Buttons = null;

tp.Ui.RegisterType(["WebDataForm", "tp-WebDataForm"], tp.WebDataForm);
