/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

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
        /**
         * Dashboard grids in tab order.
         * @type {tp.Grid[]}
         */
        this.DashboardGrids = [];
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
        var Title = tp._L(Command, Command);
        var Button = this.ToolBar.AddButton(Command, Title, Title, "", "", false);
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
        Page = TabControl.AddPage(tp._L("TopCustomers", "Top Customers"));
        this.CustomersGrid = this.CreateGrid(Page.Handle);
        Page = TabControl.AddPage(tp._L("TopSuppliers", "Top Suppliers"));
        this.SuppliersGrid = this.CreateGrid(Page.Handle);
        Page = TabControl.AddPage(tp._L("StockSnapshot", "Stock Snapshot"));
        this.StockGrid = this.CreateGrid(Page.Handle);
        this.DashboardGrids = [this.CustomersGrid, this.SuppliersGrid, this.StockGrid];
        TabControl.On("SelectedIndexChanged", this.HandleSelectedTabChanged, this);
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
    /**
     * Handles dashboard tab selection changes.
     * @param {object} Args The event arguments.
     * @returns {void}
     */
    HandleSelectedTabChanged(Args) {
        var Index = Args && tp.IsNumber(Args.NewIndex) ? Args.NewIndex : -1;
        if (Index >= 0 && Index < this.DashboardGrids.length)
            this.BestFitGrid(this.DashboardGrids[Index]);
    }
    /**
     * Best fits a grid after pending rendering is complete.
     * @param {tp.Grid|null} Grid The grid.
     * @returns {void}
     */
    BestFitGrid(Grid) {
        if (!(Grid instanceof tp.Grid))
            return;
        setTimeout(function () {
            if (!Grid.IsDisposed && tp.IsFunction(Grid.BestFitColumns))
                Grid.BestFitColumns();
        }, 0);
    }
    /**
     * Best fits all dashboard grids.
     * @returns {void}
     */
    BestFitGrids() {
        var Index;
        for (Index = 0; Index < this.DashboardGrids.length; Index++)
            this.BestFitGrid(this.DashboardGrids[Index]);
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
        this.BestFitGrids();
        if (tp.LogBox)
            tp.LogBox.AppendLine(tp._L("DashboardRefreshed", "Dashboard refreshed."));
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
