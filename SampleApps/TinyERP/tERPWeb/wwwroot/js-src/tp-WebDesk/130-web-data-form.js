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
         * True when the list needs to be reselected before showing it.
         * @type {boolean}
         */
        this.ListIsDirty = false;
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
         * The filter row controls.
         * @type {tp.SelectFilterRow[]}
         */
        this.SelectFilterRows = [];
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
         * The item page shell element.
         * @type {HTMLElement|null}
         */
        this.ItemPageShell = null;
        /**
         * The FactBox pane element.
         * @type {HTMLElement|null}
         */
        this.FactBoxPane = null;
        /**
         * The FactBox splitter element.
         * @type {HTMLElement|null}
         */
        this.FactBoxSplitter = null;
        /**
         * The FactBox tabs host element.
         * @type {HTMLElement|null}
         */
        this.FactBoxTabsHost = null;
        /**
         * The FactBox tab control.
         * @type {tp.TabControl|null}
         */
        this.FactBoxTabControl = null;
        /**
         * The current FactBox packets.
         * @type {object[]}
         */
        this.FactBoxes = [];
        /**
         * The Data FactBox table list element.
         * @type {HTMLSelectElement|null}
         */
        this.DataFactBoxTableList = null;
        /**
         * The Data FactBox grid.
         * @type {tp.Grid|null}
         */
        this.DataFactBoxGrid = null;
        /**
         * Data FactBox data sources keyed by table name.
         * @type {object}
         */
        this.DataFactBoxSources = {};
        /**
         * True when the FactBox pane is visible.
         * @type {boolean}
         */
        this.FactBoxPaneVisible = false;
        /**
         * The current FactBox pane width.
         * @type {number}
         */
        this.FactBoxPaneWidth = 420;
        /**
         * True while the FactBox pane is being resized.
         * @type {boolean}
         */
        this.FactBoxResizeActive = false;
        /**
         * The resize start mouse X coordinate.
         * @type {number}
         */
        this.FactBoxResizeStartX = 0;
        /**
         * The resize start pane width.
         * @type {number}
         */
        this.FactBoxResizeStartWidth = 0;
        /**
         * FactBox splitter mouse down handler.
         * @type {Function|null}
         */
        this.FactBoxSplitterMouseDownHandler = null;
        /**
         * FactBox splitter mouse move handler.
         * @type {Function|null}
         */
        this.FactBoxSplitterMouseMoveHandler = null;
        /**
         * FactBox splitter mouse up handler.
         * @type {Function|null}
         */
        this.FactBoxSplitterMouseUpHandler = null;
        /**
         * List grid double click handler.
         * @type {Function|null}
         */
        this.ListGridDoubleClickHandler = null;
        /**
         * The generated item page builder.
         * @type {tp.WebItemPageBuilder|null}
         */
        this.ItemPageBuilder = null;
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
        this.DisposeFactBoxSplitterResize();
        this.DisposeListGridDoubleClick();
        this.Buttons = {};
        this.ToolBar = null;
        this.SelectBar = null;
        this.SelectCombo = null;
        this.FilterPanelList = null;
        this.SelectFilterRows = [];
        this.ListGrid = null;
        this.FilterPane = null;
        this.FilterSplitter = null;
        this.ListPage = null;
        this.ItemPage = null;
        this.ItemPageShell = null;
        this.FactBoxPane = null;
        this.FactBoxSplitter = null;
        this.FactBoxTabsHost = null;
        this.FactBoxTabControl = null;
        this.ListGridDoubleClickHandler = null;
        this.FactBoxes = [];
        this.DataFactBoxTableList = null;
        this.DataFactBoxGrid = null;
        this.DataFactBoxSources = {};
        this.ItemPageBuilder = null;
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
        this.ItemPageShell = this.FindRoleElement("item-page");
        this.ItemPage = this.FindRoleElement("item-page-main") || this.ItemPageShell;
        this.FactBoxPane = this.FindRoleElement("factbox-pane");
        this.FactBoxSplitter = this.FindRoleElement("factbox-splitter");
        this.FactBoxTabsHost = this.FindRoleElement("factbox-tabs");
        this.ItemPageBuilder = new tp.WebItemPageBuilder(this);
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
        this.InitializeFactBoxSplitterResize();
        this.SetFilterPaneVisible(false);
        this.SetFactBoxPaneVisible(false);
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
        this.AddToolBarButton("FactBox", "Show FactBox", "information.png");
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
        Element.style.visibility = "hidden";
        this.ListGrid = new tp.Grid({
            ElementOrSelector: Element,
            AutoGenerateColumns: true
        });
        tp.AddClass(this.ListGrid.Handle, tp.Classes.WebDataFormGrid);
        this.InitializeListGridDoubleClick();
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
     * Returns the web form registration name.
     * @returns {string} Returns the web form name.
     */
    GetWebFormName() {
        var Form = this.Context instanceof tp.WebFormContext ? this.Context.WebFormDef : null;
        return Form ? Form.Name || "" : "";
    }
    /**
     * Initializes FactBox splitter resizing.
     * @returns {void}
     */
    InitializeFactBoxSplitterResize() {
        if (!(this.FactBoxSplitter instanceof HTMLElement) || !(this.FactBoxPane instanceof HTMLElement))
            return;
        this.FactBoxSplitterMouseDownHandler = this.HandleFactBoxSplitterMouseDown.bind(this);
        this.FactBoxSplitterMouseMoveHandler = this.HandleFactBoxSplitterMouseMove.bind(this);
        this.FactBoxSplitterMouseUpHandler = this.HandleFactBoxSplitterMouseUp.bind(this);
        this.FactBoxSplitter.addEventListener("mousedown", this.FactBoxSplitterMouseDownHandler);
    }
    /**
     * Releases FactBox splitter resize handlers.
     * @returns {void}
     */
    DisposeFactBoxSplitterResize() {
        if (this.FactBoxSplitter instanceof HTMLElement && this.FactBoxSplitterMouseDownHandler)
            this.FactBoxSplitter.removeEventListener("mousedown", this.FactBoxSplitterMouseDownHandler);
        if (this.FactBoxSplitterMouseMoveHandler)
            document.removeEventListener("mousemove", this.FactBoxSplitterMouseMoveHandler);
        if (this.FactBoxSplitterMouseUpHandler)
            document.removeEventListener("mouseup", this.FactBoxSplitterMouseUpHandler);
        this.FactBoxSplitterMouseDownHandler = null;
        this.FactBoxSplitterMouseMoveHandler = null;
        this.FactBoxSplitterMouseUpHandler = null;
        this.FactBoxResizeActive = false;
    }
    /**
     * Applies a width to the FactBox pane.
     * @param {number} Width The pane width.
     * @returns {void}
     */
    SetFactBoxPaneWidth(Width) {
        Width = Math.max(260, tp.ToInt(Width));
        this.FactBoxPaneWidth = Width;
        if (this.FactBoxPane instanceof HTMLElement) {
            this.FactBoxPane.style.width = Width + "px";
            this.FactBoxPane.style.flexBasis = Width + "px";
        }
    }
    /**
     * Handles FactBox splitter mouse down.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleFactBoxSplitterMouseDown(e) {
        if (!(this.FactBoxPane instanceof HTMLElement))
            return;
        this.FactBoxResizeActive = true;
        this.FactBoxResizeStartX = e.clientX;
        this.FactBoxResizeStartWidth = this.FactBoxPane.getBoundingClientRect().width;
        document.addEventListener("mousemove", this.FactBoxSplitterMouseMoveHandler);
        document.addEventListener("mouseup", this.FactBoxSplitterMouseUpHandler);
        document.body.classList.add(tp.Classes.UnSelectable);
        e.preventDefault();
    }
    /**
     * Handles FactBox splitter mouse move.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleFactBoxSplitterMouseMove(e) {
        var Delta;
        if (this.FactBoxResizeActive !== true)
            return;
        Delta = this.FactBoxResizeStartX - e.clientX;
        this.SetFactBoxPaneWidth(this.FactBoxResizeStartWidth + Delta);
        e.preventDefault();
    }
    /**
     * Handles FactBox splitter mouse up.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleFactBoxSplitterMouseUp(e) {
        this.FactBoxResizeActive = false;
        document.removeEventListener("mousemove", this.FactBoxSplitterMouseMoveHandler);
        document.removeEventListener("mouseup", this.FactBoxSplitterMouseUpHandler);
        document.body.classList.remove(tp.Classes.UnSelectable);
        e.preventDefault();
    }
    /**
     * Initializes list grid double click handling.
     * @returns {void}
     */
    InitializeListGridDoubleClick() {
        if (!(this.ListGrid instanceof tp.Grid))
            return;
        this.ListGridDoubleClickHandler = this.HandleListGridDoubleClick.bind(this);
        this.ListGrid.Handle.addEventListener("dblclick", this.ListGridDoubleClickHandler);
    }
    /**
     * Releases list grid double click handling.
     * @returns {void}
     */
    DisposeListGridDoubleClick() {
        if (this.ListGrid instanceof tp.Grid && this.ListGridDoubleClickHandler)
            this.ListGrid.Handle.removeEventListener("dblclick", this.ListGridDoubleClickHandler);
        this.ListGridDoubleClickHandler = null;
    }
    /**
     * Handles list grid double click.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleListGridDoubleClick(e) {
        var Element = e.target;
        if (!(this.ListGrid instanceof tp.Grid) || !(Element instanceof HTMLElement))
            return;
        if (!Element.closest("." + tp.Classes.GridRow))
            return;
        e.preventDefault();
        this.EditAsync();
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
        if (this.FilterPanelList.Count > 0) {
            if (this.SelectCombo instanceof tp.ComboBox)
                this.FilterPanelList.Associate = this.SelectCombo;
            this.FilterPanelList.SelectedIndex = this.SelectCombo instanceof tp.ComboBox ? this.SelectCombo.SelectedIndex : 0;
            this.InitializeFilterRows();
            return;
        }
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
        this.InitializeFilterRows();
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
     * Initializes select filter row controls from server-rendered placeholders.
     * @returns {void}
     */
    InitializeFilterRows() {
        var Elements;
        var Index;
        var Row;
        this.SelectFilterRows = [];
        if (!(this.FilterPanelList instanceof tp.PanelList))
            return;
        Elements = this.FilterPanelList.Handle.querySelectorAll("." + tp.Classes.SelectFilterRow);
        for (Index = 0; Index < Elements.length; Index++) {
            Row = tp.GetComponent(Elements[Index]);
            if (!(Row instanceof tp.SelectFilterRow))
                Row = new tp.SelectFilterRow({ ElementOrSelector: Elements[Index] });
            this.SelectFilterRows.push(Row);
        }
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
     * Returns active structured filter values.
     * @returns {object[]} Returns the active filters.
     */
    GetActiveFilters() {
        var Result = [];
        var Index;
        var SelectName = this.GetSelectedSelectName();
        var Filter;
        for (Index = 0; Index < this.SelectFilterRows.length; Index++) {
            if (!tp.IsSameText(this.SelectFilterRows[Index].SelectName, SelectName))
                continue;
            Filter = this.SelectFilterRows[Index].GetActiveFilter();
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
    /**
     * Selects and displays the list table.
     * @returns {Promise<void>} Returns a Promise.
     */
    async SelectListAsync() {
        var SelectName;
        var Filters;
        var Table;
        if (!(this.Module instanceof tp.DataModule))
            return;
        SelectName = this.GetSelectedSelectName();
        Filters = this.GetActiveFilters();
        await this.ExecuteWithSpinner(async function () {
            await this.Module.SelectList(SelectName, Filters);
            Table = this.Module.tblList;
            if (this.ListGrid instanceof tp.Grid && Table) {
                this.ListGrid.DataSource = Table;
                this.ListGrid.Handle.style.visibility = "";
                this.RefreshListGridLayout(true);
            }
            this.FormState = tp.WebDataFormState.List;
            this.ListIsDirty = false;
            this.ShowListPage();
            this.UpdateToolBar();
        });
    }
    /**
     * Starts an insert operation and displays the item page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async InsertAsync() {
        if (!(this.Module instanceof tp.DataModule) || this.IsReadOnly === true)
            return;
        await this.ExecuteWithSpinner(async function () {
            await this.Module.Insert();
            this.FormState = tp.WebDataFormState.Insert;
            await this.RenderItemPageAsync();
            await this.LoadFactBoxesAsync();
            this.ShowItemPage();
            this.UpdateToolBar();
        });
    }
    /**
     * Starts an edit operation for the selected list row and displays the item page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async EditAsync() {
        var Id;
        if (!(this.Module instanceof tp.DataModule) || this.IsReadOnly === true)
            return;
        Id = this.GetSelectedListId();
        if (tp.IsEmpty(Id))
            return;
        await this.ExecuteWithSpinner(async function () {
            await this.Module.Edit(Id);
            this.FormState = tp.WebDataFormState.Edit;
            this.UiLog("Loaded " + this.GetItemLogText(Id));
            await this.RenderItemPageAsync();
            await this.LoadFactBoxesAsync();
            this.ShowItemPage();
            this.UpdateToolBar();
        });
    }
    /**
     * Commits the current item, refreshes the list, and returns to the list page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async SaveAsync() {
        var Id;
        if (!(this.Module instanceof tp.DataModule) || this.IsReadOnly === true || this.IsItemState() !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                await this.Module.Commit();
                Id = this.Module.Id;
                this.UiLog("Saved " + this.GetItemLogText(Id));
                this.ListIsDirty = true;
                this.FormState = tp.WebDataFormState.Edit;
                await this.RenderItemPageAsync();
                await this.LoadFactBoxesAsync();
                this.ShowItemPage();
                this.UpdateToolBar();
            });
        } catch (e) {
            this.ReportError("Save failed: " + tp.ExceptionText(e));
        }
    }
    /**
     * Deletes the selected list item after confirmation.
     * @returns {Promise<void>} Returns a Promise.
     */
    async DeleteAsync() {
        var Id;
        var LogText;
        var Confirmed;
        if (!(this.Module instanceof tp.DataModule) || this.IsReadOnly === true || this.FormState !== tp.WebDataFormState.List)
            return;
        Id = this.GetSelectedListId();
        if (tp.IsEmpty(Id))
            return;
        LogText = this.GetItemLogText(Id);
        Confirmed = await tp.YesNoBoxAsync("Delete item: " + LogText + "?");
        if (Confirmed !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                await this.Module.Delete(Id);
                this.UiLog("Deleted " + LogText);
                await this.SelectListAsync();
            });
        } catch (e) {
            this.ReportError("Delete failed: " + tp.ExceptionText(e));
        }
    }
    /**
     * Cancels the current item operation and returns to the list page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CancelAsync() {
        if (this.IsItemState() !== true)
            return;
        this.ClearItemPage();
        this.FormState = tp.WebDataFormState.List;
        this.ShowListPage();
        this.UpdateToolBar();
    }
    /**
     * Shows the list page, refreshing it first when needed.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ListAsync() {
        if (this.ListIsDirty === true)
            await this.SelectListAsync();
        else {
            this.FormState = tp.WebDataFormState.List;
            this.ShowListPage();
            this.UpdateToolBar();
        }
    }
    /**
     * Renders the generated item page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async RenderItemPageAsync() {
        if (!(this.ItemPageBuilder instanceof tp.WebItemPageBuilder))
            this.ItemPageBuilder = new tp.WebItemPageBuilder(this);
        await this.ItemPageBuilder.BuildAsync();
    }
    /**
     * Loads FactBox packets for the current item page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadFactBoxesAsync() {
        var Packet;
        if (!(this.Module instanceof tp.DataModule))
            return;
        Packet = await tp.AjaxRequest.Execute("DataModule.GetFactBoxes", {
            ModuleName: this.ModuleName,
            WebFormName: this.GetWebFormName(),
            KeyValue: this.Module.Id,
            RowState: this.GetCurrentRowStateText()
        });
        this.FactBoxes = Packet && tp.IsArray(Packet.FactBoxes) ? Packet.FactBoxes : [];
        this.RenderFactBoxes(Packet ? Packet.Html || "" : "");
        this.SetFactBoxPaneVisible(Packet && Packet.ShowPane === true && Packet.FactBoxCount > 0);
    }
    /**
     * Renders the FactBox pane.
     * @param {string} Html The server-rendered FactBox HTML.
     * @returns {void}
     */
    RenderFactBoxes(Html) {
        var List;
        var Index;
        if (!(this.FactBoxTabsHost instanceof HTMLElement))
            return;
        this.FactBoxTabControl = null;
        this.DataFactBoxTableList = null;
        this.DataFactBoxGrid = null;
        this.DataFactBoxSources = {};
        this.FactBoxTabsHost.innerHTML = Html || "";
        if (tp.IsBlankString(Html))
            return;
        this.AppendDataFactBoxPage();
        this.FactBoxTabControl = new tp.TabControl(this.FactBoxTabsHost);
        List = this.FactBoxTabsHost.querySelectorAll(".tp-WebDataForm-FactBoxAccordion");
        for (Index = 0; Index < List.length; Index++) {
            List[Index].tpObject = new tp.Accordion({ ElementOrSelector: List[Index] });
            List[Index].tpObject.AllowMultiExpand = true;
        }
        this.InitializeDataFactBox();
    }
    /**
     * Returns true when the Data FactBox may be shown.
     * @returns {boolean} Returns true for administrator users.
     */
    CanShowDataFactBox() {
        return tp.CurrentUserIsAdmin === true;
    }
    /**
     * Appends the admin-only Data FactBox page to the current FactBox markup.
     * @returns {void}
     */
    AppendDataFactBoxPage() {
        var List;
        var TabHost;
        var PageHost;
        var Tab;
        var Page;
        var TableList;
        var GridHost;
        if (this.CanShowDataFactBox() !== true || !(this.FactBoxTabsHost instanceof HTMLElement))
            return;
        List = tp.ChildHTMLElements(this.FactBoxTabsHost);
        if (List.length !== 2)
            return;
        TabHost = List[0];
        PageHost = List[1];
        Tab = this.CreateElement("div", "", "Data");
        Page = this.CreateElement("div", "tp-WebDataForm-DataFactBoxPage");
        TableList = Page.ownerDocument.createElement("select");
        TableList.className = "tp-WebDataForm-DataFactBoxTableList";
        TableList.size = 6;
        GridHost = this.CreateElement("div", "tp-WebDataForm-DataFactBoxGridHost");
        Page.appendChild(TableList);
        Page.appendChild(GridHost);
        TabHost.appendChild(Tab);
        PageHost.appendChild(Page);
        this.DataFactBoxTableList = TableList;
    }
    /**
     * Returns the item data tables in tree order.
     * @returns {tp.DataTable[]} Returns the data tables.
     */
    GetDataFactBoxTables() {
        var Result = [];
        var AddTable;
        var TopTable = this.Module instanceof tp.DataModule ? this.Module.tblItem : null;
        AddTable = (Table) => {
            var Index;
            var Detail;
            if (!(Table instanceof tp.DataTable) || Result.indexOf(Table) >= 0)
                return;
            Result.push(Table);
            for (Index = 0; Index < Table.Details.length; Index++) {
                Detail = this.Module.FindTable(Table.Details[Index]);
                AddTable(Detail);
            }
        };
        AddTable(TopTable);
        return Result;
    }
    /**
     * Returns or creates a Data FactBox data source for a table.
     * @param {tp.DataTable} Table The table.
     * @returns {tp.DataSource|null} Returns the data source or null.
     */
    GetDataFactBoxSource(Table) {
        var Source;
        var MasterSource;
        if (!(Table instanceof tp.DataTable))
            return null;
        if (this.DataFactBoxSources[Table.Name] instanceof tp.DataSource)
            return this.DataFactBoxSources[Table.Name];
        Source = new tp.DataSource(Table);
        if (!tp.IsBlankString(Table.MasterTableName)) {
            MasterSource = this.GetDataFactBoxSource(this.Module.FindTable(Table.MasterTableName));
            if (MasterSource instanceof tp.DataSource) {
                Source.MasterKeyField = Table.MasterField;
                Source.DetailKeyField = Table.DetailField;
                Source.MasterSource = MasterSource;
            }
        }
        this.DataFactBoxSources[Table.Name] = Source;
        return Source;
    }
    /**
     * Creates all columns for the Data FactBox grid.
     * @param {tp.Grid} Grid The grid.
     * @param {tp.DataTable} Table The table.
     * @returns {void}
     */
    CreateDataFactBoxGridColumns(Grid, Table) {
        var Index;
        var Column;
        var GridColumn;
        if (!(Grid instanceof tp.Grid) || !(Table instanceof tp.DataTable))
            return;
        Grid.ClearColumns();
        for (Index = 0; Index < Table.Columns.length; Index++) {
            Column = Table.Columns[Index];
            GridColumn = Grid.AddColumn(Column.Name, Column.DisplayTitle);
            GridColumn.ReadOnly = true;
        }
    }
    /**
     * Binds the Data FactBox grid to a table.
     * @param {string} TableName The table name.
     * @returns {void}
     */
    BindDataFactBoxGrid(TableName) {
        var Table = this.Module instanceof tp.DataModule ? this.Module.FindTable(TableName) : null;
        var Source;
        if (!(this.DataFactBoxGrid instanceof tp.Grid) || !(Table instanceof tp.DataTable))
            return;
        Source = this.GetDataFactBoxSource(Table);
        this.CreateDataFactBoxGridColumns(this.DataFactBoxGrid, Table);
        this.DataFactBoxGrid.DataSource = Source;
        this.DataFactBoxGrid.BestFitColumns();
    }
    /**
     * Initializes the admin-only Data FactBox page.
     * @returns {void}
     */
    InitializeDataFactBox() {
        var Tables;
        var Index;
        var Option;
        var GridHost;
        if (!(this.DataFactBoxTableList instanceof HTMLSelectElement))
            return;
        GridHost = this.FactBoxTabsHost.querySelector(".tp-WebDataForm-DataFactBoxGridHost");
        if (!(GridHost instanceof HTMLElement))
            return;
        Tables = this.GetDataFactBoxTables();
        for (Index = 0; Index < Tables.length; Index++) {
            Option = this.DataFactBoxTableList.ownerDocument.createElement("option");
            Option.value = Tables[Index].Name;
            Option.textContent = Tables[Index].Name;
            this.DataFactBoxTableList.appendChild(Option);
        }
        this.DataFactBoxGrid = new tp.Grid({
            ElementOrSelector: GridHost,
            AutoGenerateColumns: false,
            ToolBarVisible: false,
            GroupsVisible: false,
            FilterVisible: false,
            FooterVisible: false,
            ReadOnly: true,
            AllowUserToAddRows: false,
            AllowUserToDeleteRows: false
        });
        this.DataFactBoxTableList.addEventListener("change", () => this.BindDataFactBoxGrid(this.DataFactBoxTableList.value));
        if (Tables.length > 0) {
            this.DataFactBoxTableList.selectedIndex = 0;
            this.BindDataFactBoxGrid(Tables[0].Name);
        }
    }
    /**
     * Renders a single FactBox page.
     * @param {object} FactBox The FactBox packet.
     * @returns {HTMLElement} Returns the page element.
     */
    RenderFactBoxPage(FactBox) {
        var Page = this.CreateElement("div", "tp-WebDataForm-FactBoxPage");
        var Data = FactBox ? FactBox.Data : null;
        if (Data && tp.IsObject(Data.Table))
            this.RenderStructureFactBox(Page, Data);
        else if (tp.IsObject(Data))
            this.RenderKeyValueObject(Page, Data);
        else
            Page.appendChild(this.CreateElement("pre", "tp-WebDataForm-FactBoxJson", JSON.stringify(Data, null, 2)));
        return Page;
    }
    /**
     * Renders a key/value object.
     * @param {HTMLElement} Parent The parent element.
     * @param {object} Data The data object.
     * @returns {void}
     */
    RenderKeyValueObject(Parent, Data) {
        var Name;
        for (Name in Data) {
            if (Object.prototype.hasOwnProperty.call(Data, Name))
                Parent.appendChild(this.CreateKeyValueRow(Name, Data[Name]));
        }
    }
    /**
     * Renders the standard Structure FactBox.
     * @param {HTMLElement} Parent The parent element.
     * @param {object} Data The structure data.
     * @returns {void}
     */
    RenderStructureFactBox(Parent, Data) {
        Parent.appendChild(this.CreateKeyValueRow("Module", (Data.ModuleTitle || "") + " (" + (Data.ModuleName || "") + ")"));
        Parent.appendChild(this.CreateKeyValueRow("Group", Data.ModuleGroup || ""));
        Parent.appendChild(this.CreateKeyValueRow("Module Class", this.FormatClassInfo(Data.ModuleJsClassName, Data.ModuleClassName)));
        Parent.appendChild(this.CreateKeyValueRow("Form Class", this.FormatClassInfo(Data.FormJsClassName, Data.FormClassName)));
        Parent.appendChild(this.CreateKeyValueRow("ItemPage Class", this.FormatClassInfo(Data.ItemPageJsClassName, Data.ItemPageClassName)));
        Parent.appendChild(this.CreateKeyValueRow("Tables", String(Data.VisibleTableCount || 0) + "/" + String(Data.TableCount || 0) + " visible"));
        if (tp.IsObject(Data.Table))
            this.RenderTableAccordion(Parent, Data.Table);
    }
    /**
     * Renders the table structure accordion.
     * @param {HTMLElement} Parent The parent element.
     * @param {object} Table The table data.
     * @returns {void}
     */
    RenderTableAccordion(Parent, Table) {
        var Element = this.CreateElement("div", "tp-WebDataForm-FactBoxAccordion");
        var Accordion;
        Parent.appendChild(Element);
        Accordion = new tp.Accordion({ ElementOrSelector: Element });
        Accordion.AllowMultiExpand = true;
        this.RenderTableAccordionItem(Accordion, Table, 0);
    }
    /**
     * Renders a table accordion item recursively.
     * @param {tp.Accordion} Accordion The accordion.
     * @param {object} Table The table data.
     * @param {number} Level The tree level.
     * @returns {void}
     */
    RenderTableAccordionItem(Accordion, Table, Level) {
        var Details = tp.IsArray(Table.Details) ? Table.Details : [];
        var DetailNames = tp.IsArray(Table.DetailNames) ? Table.DetailNames : [];
        var Detail;
        var Index;
        var ItemIndex = Accordion.GetElementList().length;
        var Title = (Table.Title || "") + " (" + (Table.Name || "") + ") - " + (Table.IsUiVisible ? "visible" : "hidden") + ", " + (Table.IsDetail ? "detail" : "top") + ", fields " + String(Table.VisibleFieldCount || 0) + "/" + String(Table.FieldCount || 0);
        var Item = Accordion.AddItem(Title);
        var TitleElement = Accordion.TitleElementOf(Item);
        var Body = Accordion.ContentElementOf(Item);
        if (TitleElement instanceof HTMLElement)
            TitleElement.style.paddingLeft = String(14 + (Level * 12)) + "px";
        if (!(Body instanceof HTMLElement))
            return;
        tp.AddClass(Body, "tp-WebDataForm-FactBoxTableBody");
        Accordion.Expand(Level === 0, ItemIndex);
        Body.appendChild(this.CreateKeyValueRow("Alias", Table.Alias || ""));
        Body.appendChild(this.CreateKeyValueRow("Master", Table.MasterName || ""));
        Body.appendChild(this.CreateKeyValueRow("Details", DetailNames.join(", ")));
        Body.appendChild(this.CreateKeyValueRow("KeyField", Table.KeyField || ""));
        if (Table.IsDetail === true) {
            Body.appendChild(this.CreateKeyValueRow("MasterField", Table.MasterField || ""));
            Body.appendChild(this.CreateKeyValueRow("DetailField", Table.DetailField || ""));
        }
        Body.appendChild(this.CreateKeyValueRow("OneToOne", Table.IsOneToOne === true));
        Body.appendChild(this.CreateKeyValueRow("Joins", Table.JoinCount || 0));
        Body.appendChild(this.CreateKeyValueRow("Stocks", Table.StockCount || 0));
        Body.appendChild(this.CreateKeyValueRow("Fields", String(Table.VisibleFieldCount || 0) + "/" + String(Table.FieldCount || 0) + " visible"));
        Body.appendChild(this.CreateFieldsTable(tp.IsArray(Table.Fields) ? Table.Fields : []));
        for (Index = 0; Index < Details.length; Index++) {
            Detail = Details[Index];
            this.RenderTableAccordionItem(Accordion, Detail, Level + 1);
        }
    }
    /**
     * Creates a fields table.
     * @param {object[]} Fields The field data.
     * @returns {HTMLElement} Returns the table element.
     */
    CreateFieldsTable(Fields) {
        var Wrap = this.CreateElement("div", "tp-WebDataForm-FactBoxFieldWrap");
        var Table = this.CreateElement("table", "tp-WebDataForm-FactBoxFieldTable");
        var Headers = ["Title", "Name", "Visible", "Hidden", "DataType", "Required", "ReadOnly", "Lookup", "Locator", "Group", "Size", "Decimals", "Default", "Nullable", "Width", "Expression", "CodeProvider", "SnapshotOf", "Flags"];
        var Field;
        var Index;
        var Row;
        var Values;
        Table.appendChild(this.CreateTableRow(Headers, true, []));
        for (Index = 0; Index < Fields.length; Index++) {
            Field = Fields[Index];
            Values = [
                Field.Title || "",
                Field.Name || "",
                Field.IsVisible === true ? "x" : "",
                Field.IsVisible === true ? "" : "x",
                Field.DataType || "",
                Field.IsRequired === true ? "x" : "",
                Field.IsReadOnly === true ? "x" : "",
                Field.LookupSource || "",
                Field.Locator || "",
                Field.Group || "",
                Field.MaxLength > 0 ? String(Field.MaxLength) : "",
                Field.Decimals >= 0 ? String(Field.Decimals) : "",
                Field.DefaultValue || "",
                Field.IsNullable === true ? "x" : "",
                Field.DisplayWidth > 0 ? String(Field.DisplayWidth) : "",
                Field.Expression || "",
                Field.CodeProvider || "",
                Field.SnapshotOf || "",
                Field.Flags || ""
            ];
            Row = this.CreateTableRow(Values, false, [2, 3, 5, 6, 13]);
            Table.appendChild(Row);
        }
        Wrap.appendChild(Table);
        return Wrap;
    }
    /**
     * Creates a table row.
     * @param {string[]} Values The cell values.
     * @param {boolean} IsHeader True when creating a header row.
     * @param {number[]} CenterIndexes Indexes of centered cells.
     * @returns {HTMLTableRowElement} Returns the row.
     */
    CreateTableRow(Values, IsHeader, CenterIndexes) {
        var Row = document.createElement("tr");
        var Index;
        var Cell;
        for (Index = 0; Index < Values.length; Index++) {
            Cell = document.createElement(IsHeader ? "th" : "td");
            Cell.textContent = Values[Index] || "";
            if (CenterIndexes.indexOf(Index) >= 0)
                Cell.className = "tp-Center";
            Row.appendChild(Cell);
        }
        return Row;
    }
    /**
     * Creates a key/value row.
     * @param {string} Key The key text.
     * @param {*} Value The value.
     * @returns {HTMLElement} Returns the row.
     */
    CreateKeyValueRow(Key, Value) {
        var Row = this.CreateElement("div", "tp-WebDataForm-FactBoxKeyValue");
        Row.appendChild(this.CreateElement("span", "tp-WebDataForm-FactBoxKey", Key));
        Row.appendChild(this.CreateElement("span", "tp-WebDataForm-FactBoxValue", String(tp.IsNil(Value) ? "" : Value)));
        return Row;
    }
    /**
     * Formats JavaScript class and server class/path information.
     * @param {string|null|undefined} JsName The JavaScript class name.
     * @param {string|null|undefined} ServerName The server class name or Razor path.
     * @returns {string} Returns the formatted text.
     */
    FormatClassInfo(JsName, ServerName) {
        JsName = JsName || "";
        ServerName = ServerName || "";
        if (!tp.IsBlankString(JsName) && !tp.IsBlankString(ServerName))
            return JsName + " (" + ServerName + ")";
        return JsName || ServerName;
    }
    /**
     * Returns the current item row state text.
     * @returns {string} Returns the row state text.
     */
    GetCurrentRowStateText() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        var State = Row instanceof tp.DataRow ? Row.State : null;
        var Name;
        if (tp.IsNumber(State)) {
            for (Name in tp.DataRowState) {
                if (Object.prototype.hasOwnProperty.call(tp.DataRowState, Name) && tp.DataRowState[Name] === State)
                    return Name;
            }
        }
        return this.FormState || "";
    }
    /**
     * Creates an element.
     * @param {string} TagName The tag name.
     * @param {string} CssClass The CSS class.
     * @param {string|null|undefined} Text Optional text.
     * @returns {HTMLElement} Returns the created element.
     */
    CreateElement(TagName, CssClass, Text) {
        var Result = document.createElement(TagName);
        if (!tp.IsBlankString(CssClass))
            Result.className = CssClass;
        if (!tp.IsNil(Text))
            Result.textContent = Text;
        return Result;
    }
    /**
     * Returns the id of the selected list row.
     * @returns {*} Returns the selected id or null.
     */
    GetSelectedListId() {
        var Source;
        var Row;
        var Table;
        if (!(this.ListGrid instanceof tp.Grid))
            return null;
        Source = this.ListGrid.DataSource;
        Row = Source instanceof tp.DataSource ? Source.Current : null;
        Table = Source instanceof tp.DataSource ? Source.Table : null;
        return Row && Table instanceof tp.DataTable ? Row.Get(Table.KeyField, null) : null;
    }
    /**
     * Returns true when the form is in an item editing state.
     * @returns {boolean} Returns true when in Insert or Edit state.
     */
    IsItemState() {
        return this.FormState === tp.WebDataFormState.Insert || this.FormState === tp.WebDataFormState.Edit;
    }
    /**
     * Selects a list row by id.
     * @param {*} Id The row id.
     * @returns {void}
     */
    SelectListRowById(Id) {
        var Source;
        var Table;
        var Row;
        if (tp.IsEmpty(Id) || !(this.ListGrid instanceof tp.Grid))
            return;
        Source = this.ListGrid.DataSource;
        Table = Source instanceof tp.DataSource ? Source.Table : null;
        Row = Table instanceof tp.DataTable ? Table.FindRow(Table.KeyField, Id) : null;
        if (Row instanceof tp.DataRow)
            this.ListGrid.SetFocusedRow(Row);
    }
    /**
     * Returns a text describing an item for logging purposes.
     * @param {*} Id The item id.
     * @returns {string} Returns the item log text.
     */
    GetItemLogText(Id) {
        var Parts = [];
        var Row = this.GetLogRow(Id);
        var FieldName = this.Module instanceof tp.DataModule ? this.Module.ItemCaptionField : "";
        var Code;
        var Caption;
        if (Row instanceof tp.DataRow) {
            if (!tp.IsSameText(FieldName, "Code") && Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn("Code") >= 0) {
                Code = Row.Get("Code", null);
                if (!tp.IsEmpty(Code))
                    Parts.push(String(Code));
            }
            if (!tp.IsBlank(FieldName) && Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn(FieldName) >= 0) {
                Caption = Row.Get(FieldName, null);
                if (!tp.IsEmpty(Caption))
                    Parts.push(String(Caption));
            }
        }
        if (Parts.length > 0)
            return Parts.join(" - ");
        if (!tp.IsEmpty(Id))
            return String(Id);
        return "Current item";
    }
    /**
     * Returns the row used for item logging.
     * @param {*} Id The item id.
     * @returns {tp.DataRow|null} Returns a row or null.
     */
    GetLogRow(Id) {
        var Table;
        var Row;
        if (this.Module instanceof tp.DataModule) {
            Table = this.Module.tblList;
            Row = Table instanceof tp.DataTable && !tp.IsEmpty(Id) ? Table.FindRow(Table.KeyField, Id) : null;
            if (Row instanceof tp.DataRow)
                return Row;
            Row = this.Module.Row;
            if (Row instanceof tp.DataRow)
                return Row;
        }
        return null;
    }
    /**
     * Writes a data form message to the UI log.
     * @param {string} Text The message text.
     * @returns {void}
     */
    UiLog(Text) {
        var Title = !tp.IsBlank(this.TitleText) ? this.TitleText : this.ModuleName;
        Title = String(Title || "").replace(/\s+/g, " ").trim();
        if (tp.LogBox && tp.LogBox.AppendLine)
            tp.LogBox.AppendLine("[" + Title + "] - " + Text);
    }
    /**
     * Clears the item page surface and FactBox content.
     * @returns {void}
     */
    ClearItemPage() {
        if (this.ItemPageBuilder instanceof tp.WebItemPageBuilder)
            this.ItemPageBuilder.Clear();
        if (this.FactBoxTabsHost instanceof HTMLElement)
            this.FactBoxTabsHost.innerHTML = "";
        this.FactBoxes = [];
        this.FactBoxTabControl = null;
        this.SetFactBoxPaneVisible(false);
    }
    /**
     * Reports an error through the standard WebDesk channels.
     * @param {string} Text The error text.
     * @returns {void}
     */
    ReportError(Text) {
        if (tp.LogBox && tp.LogBox.AppendLine)
            tp.LogBox.AppendLine(Text);
        if (tp.IsFunction(tp.ErrorNote))
            tp.ErrorNote(Text);
    }
    /**
     * Shows the list page.
     * @returns {void}
     */
    ShowListPage() {
        if (this.ListPage instanceof HTMLElement) {
            this.ListPage.hidden = false;
            this.ListPage.style.display = "";
        }
        if (this.ItemPageShell instanceof HTMLElement) {
            this.ItemPageShell.hidden = true;
            this.ItemPageShell.style.display = "none";
        }
    }
    /**
     * Shows the item page.
     * @returns {void}
     */
    ShowItemPage() {
        if (this.ListPage instanceof HTMLElement) {
            this.ListPage.hidden = true;
            this.ListPage.style.display = "none";
        }
        if (this.ItemPageShell instanceof HTMLElement) {
            this.ItemPageShell.hidden = false;
            this.ItemPageShell.style.display = "";
        }
    }
    /**
     * Toggles the FactBox pane.
     * @returns {void}
     */
    ToggleFactBoxPane() {
        this.SetFactBoxPaneVisible(!this.FactBoxPaneVisible);
    }
    /**
     * Shows or hides the FactBox pane.
     * @param {boolean} Visible True to show.
     * @returns {void}
     */
    SetFactBoxPaneVisible(Visible) {
        var IsVisible = Visible === true && this.FactBoxTabsHost instanceof HTMLElement && !tp.IsBlankString(this.FactBoxTabsHost.innerHTML);
        this.FactBoxPaneVisible = IsVisible;
        if (this.FactBoxPane instanceof HTMLElement) {
            this.FactBoxPane.hidden = !IsVisible;
            this.FactBoxPane.style.display = IsVisible ? "" : "none";
            if (IsVisible)
                this.SetFactBoxPaneWidth(this.FactBoxPaneWidth);
        }
        if (this.FactBoxSplitter instanceof HTMLElement) {
            this.FactBoxSplitter.hidden = !IsVisible;
            this.FactBoxSplitter.style.display = IsVisible ? "" : "none";
        }
        this.UpdateToolBar();
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
        else if (Command === "FactBox")
            this.ToggleFactBoxPane();
        else if (Command === "List")
            this.ListAsync();
        else if (Command === "RefreshList")
            this.SelectListAsync();
        else if (Command === "Find")
            this.ToggleFilterPane();
        else if (Command === "Insert")
            this.InsertAsync();
        else if (Command === "Edit")
            this.EditAsync();
        else if (Command === "Delete")
            this.DeleteAsync();
        else if (Command === "Save")
            this.SaveAsync();
        else if (Command === "Cancel")
            this.CancelAsync();
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
        var Index;
        var SelectName = this.GetSelectedSelectName();
        for (Index = 0; Index < this.SelectFilterRows.length; Index++) {
            if (!tp.IsSameText(this.SelectFilterRows[Index].SelectName, SelectName))
                continue;
            this.SelectFilterRows[Index].Clear();
        }
    }
    /**
     * Updates toolbar state.
     * @returns {void}
     */
    UpdateToolBar() {
        var IsItemState = this.IsItemState();
        var HasModule = this.Module instanceof tp.DataModule;
        this.SetButtonEnabled("List", true);
        this.SetButtonEnabled("RefreshList", true);
        this.SetButtonEnabled("Find", HasModule && this.Module.UseFilters === true);
        this.SetButtonVisible("FactBox", this.FactBoxTabsHost instanceof HTMLElement && !tp.IsBlankString(this.FactBoxTabsHost.innerHTML));
        this.SetButtonEnabled("FactBox", this.FactBoxTabsHost instanceof HTMLElement && !tp.IsBlankString(this.FactBoxTabsHost.innerHTML) && this.FormState !== tp.WebDataFormState.List);
        this.SetButtonVisible("Ok", false);
        this.SetButtonEnabled("Home", false);
        this.SetButtonEnabled("ToggleIds", true);
        this.SetButtonEnabled("Insert", HasModule && this.IsReadOnly !== true && IsItemState !== true);
        this.SetButtonEnabled("Edit", HasModule && this.Module.tblList instanceof tp.DataTable && this.Module.tblList.RowCount > 0 && this.IsReadOnly !== true && IsItemState !== true);
        this.SetButtonEnabled("Delete", HasModule && this.Module.tblList instanceof tp.DataTable && this.Module.tblList.RowCount > 0 && this.IsReadOnly !== true && this.FormState === tp.WebDataFormState.List);
        this.SetButtonEnabled("Refresh", false);
        this.SetButtonEnabled("Save", HasModule && this.IsReadOnly !== true && IsItemState === true);
        this.SetButtonEnabled("Cancel", IsItemState === true);
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
 * True when the list needs to be reselected before showing it.
 * @type {boolean}
 */
tp.WebDataForm.prototype.ListIsDirty = false;
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
 * The filter row controls.
 * @type {tp.SelectFilterRow[]|null}
 */
tp.WebDataForm.prototype.SelectFilterRows = null;
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
 * The item page shell element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.ItemPageShell = null;
/**
 * The FactBox pane element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.FactBoxPane = null;
/**
 * The FactBox splitter element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.FactBoxSplitter = null;
/**
 * The FactBox tabs host element.
 * @type {HTMLElement|null}
 */
tp.WebDataForm.prototype.FactBoxTabsHost = null;
/**
 * The FactBox tab control.
 * @type {tp.TabControl|null}
 */
tp.WebDataForm.prototype.FactBoxTabControl = null;
/**
 * The current FactBox packets.
 * @type {object[]|null}
 */
tp.WebDataForm.prototype.FactBoxes = null;
/**
 * The Data FactBox table list element.
 * @type {HTMLSelectElement|null}
 */
tp.WebDataForm.prototype.DataFactBoxTableList = null;
/**
 * The Data FactBox grid.
 * @type {tp.Grid|null}
 */
tp.WebDataForm.prototype.DataFactBoxGrid = null;
/**
 * Data FactBox data sources keyed by table name.
 * @type {object|null}
 */
tp.WebDataForm.prototype.DataFactBoxSources = null;
/**
 * True when the FactBox pane is visible.
 * @type {boolean}
 */
tp.WebDataForm.prototype.FactBoxPaneVisible = false;
/**
 * The current FactBox pane width.
 * @type {number}
 */
tp.WebDataForm.prototype.FactBoxPaneWidth = 420;
/**
 * True while the FactBox pane is being resized.
 * @type {boolean}
 */
tp.WebDataForm.prototype.FactBoxResizeActive = false;
/**
 * The resize start mouse X coordinate.
 * @type {number}
 */
tp.WebDataForm.prototype.FactBoxResizeStartX = 0;
/**
 * The resize start pane width.
 * @type {number}
 */
tp.WebDataForm.prototype.FactBoxResizeStartWidth = 0;
/**
 * FactBox splitter mouse down handler.
 * @type {Function|null}
 */
tp.WebDataForm.prototype.FactBoxSplitterMouseDownHandler = null;
/**
 * FactBox splitter mouse move handler.
 * @type {Function|null}
 */
tp.WebDataForm.prototype.FactBoxSplitterMouseMoveHandler = null;
/**
 * FactBox splitter mouse up handler.
 * @type {Function|null}
 */
tp.WebDataForm.prototype.FactBoxSplitterMouseUpHandler = null;
/**
 * List grid double click handler.
 * @type {Function|null}
 */
tp.WebDataForm.prototype.ListGridDoubleClickHandler = null;
/**
 * Toolbar buttons keyed by command.
 * @type {object|null}
 */
tp.WebDataForm.prototype.Buttons = null;

tp.Ui.RegisterType(["WebDataForm", "tp-WebDataForm"], tp.WebDataForm);
