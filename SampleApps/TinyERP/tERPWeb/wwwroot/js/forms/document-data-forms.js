/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● app form dialog
/**
 * Modal dialog window that hosts a WebDesk form.
 */
app.AppFormDialog = class extends tp.Window {
    // ● static public
    /**
     * Shows a WebDesk data form in a modal dialog.
     * @param {string} WebFormName The web form name.
     * @param {object|null|undefined} Options The dialog and form options.
     * @returns {Promise<app.AppFormDialog>} Returns a Promise resolving with the closed dialog.
     */
    static async ShowModalDataFormAsync(WebFormName, Options) {
        var Packet;
        var Form;
        var Dialog;
        var ShowSpinner = tp.IsFunction(tp.ShowSpinner);
        Options = Options || {};
        if (tp.IsBlankString(WebFormName))
            throw new Error("No WebForm name specified.");
        if (ShowSpinner)
            tp.ShowSpinner(true);
        try {
            Packet = await app.App.GetWebFormAsync(WebFormName);
            Form = Packet ? Packet.Form : null;
            if (!Form)
                throw new Error("WebForm not returned: " + WebFormName);
            Dialog = new app.AppFormDialog({
                Text: Options.Title || Form.Title || Form.Name || WebFormName,
                Width: Options.Width || "min(1180px, calc(100vw - 32px))",
                Height: Options.Height || "min(760px, calc(100vh - 32px))",
                ShowFooter: false,
                CloseBox: Options.CloseBox !== false,
                CssClasses: "app-form-dialog",
                WebFormName: WebFormName,
                WebFormPacket: Packet,
                WebFormOptions: Options
            });
            await Dialog.CreateHostedFormAsync();
        } finally {
            if (ShowSpinner)
                tp.ShowSpinner(false);
        }
        Dialog.ShowModal();
        return await Dialog.WaitClosedAsync();
    }

    // ● constructor
    /**
     * Creates the dialog.
     * @param {tp.WindowArgs|object|null|undefined} Args The window arguments.
     */
    constructor(Args) {
        super(Args);
        this.tpClass = "app.AppFormDialog";
    }

    // ● protected
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * The server web form packet.
         * @type {object|null}
         */
        this.WebFormPacket = null;
        /**
         * The hosted web form context.
         * @type {tp.WebFormContext|null}
         */
        this.FormContext = null;
        /**
         * The hosted web form.
         * @type {tp.WebForm|null}
         */
        this.HostedForm = null;
        /**
         * Result data returned by the hosted form.
         * @type {*}
         */
        this.ResultData = null;
        /**
         * Resolves the modal close Promise.
         * @type {Function|null}
         */
        this.fClosedResolve = null;
    }
    /**
     * Applies create params.
     * @param {tp.WindowArgs|object|null|undefined} Params The create params.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (Params) {
            this.WebFormName = Params.WebFormName || "";
            this.WebFormPacket = Params.WebFormPacket || null;
            this.WebFormOptions = Params.WebFormOptions || null;
        }
    }
    /**
     * Releases the hosted form.
     * @returns {void}
     */
    DoDispose() {
        if (this.HostedForm instanceof tp.WebForm) {
            this.HostedForm.Off("CloseRequested", this.HandleHostedFormCloseRequested, this);
            this.HostedForm.Dispose();
        }
        this.HostedForm = null;
        this.FormContext = null;
        this.WebFormPacket = null;
        this.WebFormOptions = null;
        super.DoDispose();
    }
    /**
     * Creates the client web form inside the dialog.
     * @returns {Promise<tp.WebForm>} Returns a Promise resolving with the created form.
     */
    async CreateHostedFormAsync() {
        var Form = this.WebFormPacket ? this.WebFormPacket.Form : null;
        var Options = this.WebFormOptions || {};
        var Element;
        if (!Form)
            throw new Error("WebForm packet has no Form.");
        if (!this.ContentWrapper) {
            this.CreateControls();
            this.SetupDragger();
            this.SetupPositionAndSize();
        }
        this.ContentWrapper.Handle.innerHTML = Form.Html || "";
        Element = this.FindHostedFormElement();
        if (!(Element instanceof HTMLElement))
            throw new Error("WebForm root element not found: " + (Form.Name || ""));
        this.FormContext = new tp.WebFormContext({
            FormId: Options.FormId || Form.Name || this.WebFormName,
            ClassName: Form.JsFormClassType,
            DisplayMode: tp.WebFormDisplayMode.Dialog,
            ParentControl: this,
            Title: Options.Title || Form.Title || Form.Name || this.WebFormName,
            WebFormDef: Form,
            Packet: this.WebFormPacket,
            Options: Options,
            CssFiles: Form.CssFiles || [],
            JavaScriptFiles: Form.JavaScriptFiles || []
        });
        this.HostedForm = await this.FormContext.CreateForm(Element);
        this.HostedForm.On("CloseRequested", this.HandleHostedFormCloseRequested, this);
        return this.HostedForm;
    }
    /**
     * Finds the root hosted web form element.
     * @returns {HTMLElement|null} Returns the form element or null.
     */
    FindHostedFormElement() {
        var Index;
        var Children = this.ContentWrapper && this.ContentWrapper.Handle ? this.ContentWrapper.Handle.children : [];
        for (Index = 0; Index < Children.length; Index++) {
            if (Children[Index] instanceof HTMLElement)
                return Children[Index];
        }
        return null;
    }
    /**
     * Handles hosted form close requests.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleHostedFormCloseRequested(Args) {
        var Context = Args && Args.Context instanceof tp.WebFormContext ? Args.Context : null;
        var Result = Context instanceof tp.WebFormContext ? Context.ModalResult : tp.DialogResult.None;
        if (Result === tp.DialogResult.None)
            Result = tp.DialogResult.Cancel;
        this.ResultData = Context instanceof tp.WebFormContext ? Context.ResultData : null;
        this.fDialogResult = Result;
        this.Close();
    }
    /**
     * Handles standard window clicks.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    WindowAnyClick(e) {
        var Command = e && e.type !== "dblclick" ? tp.Data(e.target, "command") : "";
        if (tp.IsSameText("Close", Command) && this.HostedForm instanceof tp.WebForm) {
            this.HostedForm.CloseForm();
            return;
        }
        super.WindowAnyClick(e);
    }
    /**
     * Called when the window closes.
     * @returns {void}
     */
    OnClosed() {
        super.OnClosed();
        if (this.fClosedResolve) {
            this.fClosedResolve(this);
            this.fClosedResolve = null;
        }
    }

    // ● public
    /**
     * Returns a Promise resolved when the dialog closes.
     * @returns {Promise<app.AppFormDialog>} Returns a Promise resolving with this dialog.
     */
    WaitClosedAsync() {
        if (!this.Visible)
            return Promise.resolve(this);
        return new Promise((Resolve) => {
            this.fClosedResolve = Resolve;
        });
    }
};

tp.ReferenceContextMenu.ShowDataFormModalAsync = async function (WebFormName, Options) {
    Options = Options || {};
    return await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
        FormId: WebFormName + "." + tp.Guid(),
        Title: Options.Title || WebFormName,
        InitialAction: Options.InitialAction || "List",
        InitialKeyValue: Options.InitialKeyValue
    });
};

// ● trade data module
/**
 * Client-side data module for commercial document modules.
 */
app.TradeDataModule = class extends tp.DataModule {
    // ● constructor
    /**
     * Creates the trade data module.
     * @param {string|object|null|undefined} NameOrSource The module name or a JsonDataModule source object.
     */
    constructor(NameOrSource) {
        super(NameOrSource);
        this.AttachTradeLineDefaults();
    }

    // ● protected
    /**
     * Returns true when the table is a trade line table.
     * @param {tp.DataTable} Table The table to check.
     * @returns {boolean} Returns true when the table is a trade line table.
     */
    IsTradeLineTable(Table) {
        return Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "TradeLine");
    }
    /**
     * Applies commercial document defaults to a newly added line.
     * @param {tp.DataTable} Table The detail table.
     * @param {tp.DataRow} Row The detail row.
     * @returns {void}
     */
    ApplyTradeLineDefaults(Table, Row) {
        var Header = this.Row;
        if (!this.IsTradeLineTable(Table) || !(Row instanceof tp.DataRow) || !(Header instanceof tp.DataRow))
            return;
        if (Table.IndexOfColumn("WarehouseId") >= 0 && tp.IsEmpty(Row.Get("WarehouseId")))
            Row.Set("WarehouseId", Header.Get("WarehouseId"));
    }
    /**
     * Attaches detail table default handlers.
     * @returns {void}
     */
    AttachTradeLineDefaults() {
        var Table;
        var Index;
        if (!(this.DataSet instanceof tp.DataSet))
            return;
        for (Index = 0; Index < this.DataSet.Tables.length; Index++) {
            Table = this.DataSet.Tables[Index];
            if (this.IsTradeLineTable(Table) && Table.fAppTradeDefaultsAttached !== true) {
                Table.fAppTradeDefaultsAttached = true;
                Table.On("RowAdding", function (Args) {
                    this.ApplyTradeLineDefaults(Args.Table, Args.Row);
                }, this);
            }
        }
    }

    // ● public
    /**
     * Assigns values from a JsonDataModule source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        super.Assign(Source);
        this.AttachTradeLineDefaults();
    }
};

// ● sales data module
/**
 * Client-side data module for sales document modules.
 */
app.SalesDataModule = class extends app.TradeDataModule {
    // ● constructor
    /**
     * Creates the sales data module.
     * @param {string|object|null|undefined} NameOrSource The module name or a JsonDataModule source object.
     */
    constructor(NameOrSource) {
        super(NameOrSource);
    }
};

// ● purchase data module
/**
 * Client-side data module for purchase document modules.
 */
app.PurchaseDataModule = class extends app.TradeDataModule {
    // ● constructor
    /**
     * Creates the purchase data module.
     * @param {string|object|null|undefined} NameOrSource The module name or a JsonDataModule source object.
     */
    constructor(NameOrSource) {
        super(NameOrSource);
    }
};

// ● payment data module
/**
 * Client-side data module for payment document modules.
 */
app.PaymentDataModule = class extends tp.DataModule {
    // ● constructor
    /**
     * Creates the payment data module.
     * @param {string|object|null|undefined} NameOrSource The module name or a JsonDataModule source object.
     */
    constructor(NameOrSource) {
        super(NameOrSource);
        /**
         * Latest payment amount adjustment message.
         * @type {string}
         */
        this.AmountAdjustmentMessage = "";
        /**
         * True while payment totals are being calculated.
         * @type {boolean}
         */
        this.fPaymentTotalsUpdating = false;
        this.AttachPaymentTotals();
    }

    // ● protected
    /**
     * Rounds payment amounts.
     * @param {*} Value The source value.
     * @returns {number} Returns the rounded amount.
     */
    RoundAmount(Value) {
        return Math.round(tp.StrToFloat(Value, 0) * 10000) / 10000;
    }
    /**
     * Returns true when this module is a payment cancellation document.
     * @returns {boolean} Returns true for payment cancellations.
     */
    IsPaymentCancellation() {
        return tp.IsSameText(this.Name, "CustomerReceiptCancellation")
            || tp.IsSameText(this.Name, "SupplierPaymentCancellation");
    }
    /**
     * Returns a table by name.
     * @param {string} TableName The table name.
     * @returns {tp.DataTable|null} Returns the table or null.
     */
    FindPaymentTable(TableName) {
        return this.DataSet instanceof tp.DataSet ? this.DataSet.FindTable(TableName) : null;
    }
    /**
     * Sets a row value when the field exists.
     * @param {tp.DataRow} Row The row.
     * @param {string} FieldName The field name.
     * @param {*} Value The field value.
     * @returns {void}
     */
    SetPaymentRowValue(Row, FieldName, Value) {
        if (Row instanceof tp.DataRow && Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn(FieldName) >= 0)
            Row.Set(FieldName, Value);
    }
    /**
     * Returns active settlement rows.
     * @returns {tp.DataRow[]} Returns active settlement rows.
     */
    GetActiveSettlements() {
        var Table = this.FindPaymentTable("PaymentSettlement");
        if (!(Table instanceof tp.DataTable))
            return [];
        return Table.Rows.filter(function (Row) {
            return Row instanceof tp.DataRow
                && Row.State !== tp.DataRowState.Deleted
                && Row.State !== tp.DataRowState.Detached;
        });
    }
    /**
     * Returns the settlement amount total.
     * @returns {number} Returns the settlement amount total.
     */
    GetSettlementTotal() {
        var Rows = this.GetActiveSettlements();
        var Result = 0;
        var Index;
        for (Index = 0; Index < Rows.length; Index++)
            Result += tp.StrToFloat(Rows[Index].Get("Amount"), 0);
        return this.RoundAmount(Result);
    }
    /**
     * Recalculates payment settlement totals.
     * @returns {void}
     */
    CalculateTotals() {
        var Row = this.Row;
        var SettledAmount;
        if (!(Row instanceof tp.DataRow) || this.fPaymentTotalsUpdating === true)
            return;
        this.fPaymentTotalsUpdating = true;
        try {
            SettledAmount = this.GetSettlementTotal();
            this.SetPaymentRowValue(Row, "SettledAmount", SettledAmount);
            this.SetPaymentRowValue(Row, "UnappliedAmount", this.RoundAmount(tp.StrToFloat(Row.Get("Amount"), 0) - SettledAmount));
        } finally {
            this.fPaymentTotalsUpdating = false;
        }
    }
    /**
     * Adjusts payment amount to settlement total when settlement lines exist.
     * @returns {void}
     */
    AdjustAmountToSettlementTotal() {
        var Row = this.Row;
        var SettledAmount;
        var Amount;
        this.AmountAdjustmentMessage = "";
        if (!(Row instanceof tp.DataRow) || this.IsPaymentCancellation() === true || this.GetActiveSettlements().length === 0)
            return;
        SettledAmount = this.GetSettlementTotal();
        Amount = this.RoundAmount(Row.Get("Amount"));
        if (Amount === SettledAmount)
            return;
        this.fPaymentTotalsUpdating = true;
        try {
            Row.Set("Amount", SettledAmount);
        } finally {
            this.fPaymentTotalsUpdating = false;
        }
        this.AmountAdjustmentMessage = "Payment amount was adjusted from " + Amount + " to " + SettledAmount + " to match settlement total.";
    }
    /**
     * Handles payment row changes.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandlePaymentRowModified(Args) {
        var Table = Args instanceof tp.DataTableEventArgs ? Args.Table : null;
        var Column = Args instanceof tp.DataTableEventArgs ? Args.Column : null;
        if (this.fPaymentTotalsUpdating === true || !(Column instanceof tp.DataColumn))
            return;
        if ((Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "Payment") && tp.IsSameText(Column.Name, "Amount"))
            || (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "PaymentSettlement") && tp.IsSameText(Column.Name, "Amount")))
            this.CalculateTotals();
    }
    /**
     * Attaches payment total handlers.
     * @returns {void}
     */
    AttachPaymentTotals() {
        var PaymentTable = this.FindPaymentTable("Payment");
        var SettlementTable = this.FindPaymentTable("PaymentSettlement");
        if (PaymentTable instanceof tp.DataTable && PaymentTable.fAppPaymentTotalsAttached !== true) {
            PaymentTable.fAppPaymentTotalsAttached = true;
            PaymentTable.On("RowModified", this.HandlePaymentRowModified, this);
        }
        if (SettlementTable instanceof tp.DataTable && SettlementTable.fAppPaymentTotalsAttached !== true) {
            SettlementTable.fAppPaymentTotalsAttached = true;
            SettlementTable.On("RowModified", this.HandlePaymentRowModified, this);
            SettlementTable.On("RowAdded", this.CalculateTotals, this);
            SettlementTable.On("RowRemoved", this.CalculateTotals, this);
        }
        this.CalculateTotals();
    }

    // ● public
    /**
     * Assigns values from a JsonDataModule source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        super.Assign(Source);
        this.AttachPaymentTotals();
    }
    /**
     * Commits the current item.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async Commit() {
        this.CalculateTotals();
        this.AdjustAmountToSettlementTotal();
        this.CalculateTotals();
        return await super.Commit();
    }
};

// ● document item page builder
/**
 * Builds item pages for document forms and applies document-specific detail row defaults.
 */
app.DocumentItemPageBuilder = class extends tp.WebItemPageBuilder {
    // ● constructor
    /**
     * Creates the document item page builder.
     * @param {tp.WebDataForm} Form The owner data form.
     */
    constructor(Form) {
        super(Form);
    }

    // ● protected
    /**
     * Returns true when the table is a trade line table.
     * @param {tp.DataTable} Table The table to check.
     * @returns {boolean} Returns true when the table is a trade line table.
     */
    IsTradeLineTable(Table) {
        return Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "TradeLine");
    }
    /**
     * Returns true when the table participates in server-side document calculation.
     * @param {tp.DataTable} Table The table to check.
     * @returns {boolean} Returns true when calculated.
     */
    IsServerCalculatedTable(Table) {
        return this.IsTradeLineTable(Table)
            || (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockTradeLine"))
            || (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockCountLine"))
            || (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "JournalEntryLine"));
    }
    /**
     * Returns true when a column should be read-only.
     * @param {tp.DataColumn} Column The data column.
     * @returns {boolean} Returns true when read-only.
     */
    IsReadOnlyColumn(Column) {
        if (this.Form && tp.IsFunction(this.Form.IsDocumentLocked) && this.Form.IsDocumentLocked())
            return true;
        return super.IsReadOnlyColumn(Column);
    }
    /**
     * Returns true when detail grid rows can be changed.
     * @returns {boolean} Returns true when detail grids are editable.
     */
    IsDetailGridEditable() {
        if (this.Form && tp.IsFunction(this.Form.IsDocumentLocked) && this.Form.IsDocumentLocked())
            return false;
        return super.IsDetailGridEditable();
    }
    /**
     * Applies the next display order to a new detail row.
     * @param {tp.DataTable} Table The detail table.
     * @param {tp.DataRow} Row The detail row.
     * @returns {void}
     */
    ApplyDisplayOrderDefault(Table, Row) {
        var Index;
        var MaxDisplayOrder = 0;
        var Value;
        if (!(Table instanceof tp.DataTable) || !(Row instanceof tp.DataRow))
            return;
        if (Table.IndexOfColumn("DisplayOrder") < 0 || !tp.IsEmpty(Row.Get("DisplayOrder")))
            return;
        for (Index = 0; Index < Table.Rows.length; Index++) {
            if (Table.Rows[Index] !== Row && Table.Rows[Index].State !== tp.DataRowState.Deleted) {
                Value = tp.StrToInt(Table.Rows[Index].Get("DisplayOrder"), 0);
                if (Value > MaxDisplayOrder)
                    MaxDisplayOrder = Value;
            }
        }
        Row.Set("DisplayOrder", MaxDisplayOrder + 10);
    }
    /**
     * Applies document-specific defaults to a newly created detail row.
     * @param {tp.DataTable} Table The detail table.
     * @param {tp.DataRow} Row The detail row.
     * @returns {void}
     */
    ApplyDetailRowDefaults(Table, Row) {
        var Header = this.Row;
        this.ApplyDisplayOrderDefault(Table, Row);
        if (Table instanceof tp.DataTable
            && tp.IsSameText(Table.Name, "StockTradeLine")
            && Row instanceof tp.DataRow
            && Header instanceof tp.DataRow
            && Table.IndexOfColumn("WarehouseId") >= 0
            && tp.IsEmpty(Row.Get("WarehouseId")))
            Row.Set("WarehouseId", Header.Get("WarehouseId"));
    }
    /**
     * Returns the field names to display in a document detail grid.
     * @param {tp.DataTable} Table The detail table.
     * @returns {string[]|null} Returns field names or null to use the default columns.
     */
    GetDetailGridFieldNames(Table) {
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockTradeLine")) {
            return [
                "DisplayOrder",
                "ProductCode",
                "ProductName",
                "WarehouseId",
                "UnitOfMeasureId",
                "UnitRatio",
                "Quantity",
                "PrimaryQuantity",
                "UnitCost",
                "CostAmount",
                "Remarks"
            ];
        }
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockCountLine")) {
            return [
                "DisplayOrder",
                "ProductCode",
                "ProductName",
                "UnitOfMeasureId",
                "SystemQuantity",
                "CountedQuantity",
                "DifferenceQuantity",
                "UnitCost",
                "DifferenceCostAmount",
                "Remarks"
            ];
        }
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "JournalEntryLine")) {
            return [
                "DisplayOrder",
                "AccountId",
                "DebitAmount",
                "CreditAmount",
                "CurrencyId",
                "ExchangeRate",
                "ReferenceNo",
                "Remarks"
            ];
        }
        return null;
    }
    /**
     * Returns the title for a detail grid column.
     * @param {tp.DataColumn} Column The data column.
     * @returns {string} Returns the grid column title.
     */
    GetDetailGridColumnTitle(Column) {
        var Name = Column instanceof tp.DataColumn ? Column.Name : "";
        if (tp.EndsWith(Name, "Id", true))
            return tp.SplitOnUpperCase(Name.substring(0, Name.length - 2));
        return Column instanceof tp.DataColumn ? Column.DisplayTitle : "";
    }
    /**
     * Applies form context to locator columns after table metadata has been bound.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {void}
     */
    ApplyDetailGridLocatorContext(Grid) {
        var Index;
        var Column;
        if (!(Grid instanceof tp.Grid))
            return;
        for (Index = 0; Index < Grid.Columns.length; Index++) {
            Column = Grid.Columns[Index];
            if (Column && Column.IsLocator && tp.IsBlank(Column.LocatorModuleName))
                Column.LocatorModuleName = this.Form ? this.Form.ModuleName : "";
        }
    }
    /**
     * Schedules best-fit column sizing for a detail grid.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {void}
     */
    ScheduleDetailGridBestFit(Grid) {
        if (!(Grid instanceof tp.Grid) || !tp.IsFunction(Grid.BestFitColumns))
            return;
        if (Grid.fDocumentBestFitTimer)
            clearTimeout(Grid.fDocumentBestFitTimer);
        Grid.fDocumentBestFitTimer = setTimeout(function () {
            Grid.fDocumentBestFitTimer = null;
            Grid.BestFitColumns();
        }, 50);
    }
    /**
     * Attaches best-fit refresh handlers to a detail grid data source.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {void}
     */
    AttachDetailGridBestFit(Grid) {
        var Source;
        if (!(Grid instanceof tp.Grid))
            return;
        Source = Grid.DataSource;
        if (!(Source instanceof tp.DataSource))
            return;
        if (Grid.fDocumentBestFitSource === Source)
            return;
        if (Grid.fDocumentBestFitSource instanceof tp.DataSource && Grid.fDocumentBestFitListener)
            Grid.fDocumentBestFitSource.Off("RowModified", Grid.fDocumentBestFitListener);
        Grid.fDocumentBestFitSource = Source;
        Grid.fDocumentBestFitListener = Source.On("RowModified", function () {
            this.ScheduleDetailGridBestFit(Grid);
        }, this);
    }
    /**
     * Returns true when a modified line column requires server-side document calculation.
     * @param {tp.DataColumn|null|undefined} Column The modified column.
     * @returns {boolean} Returns true when server calculation is required.
     */
    IsServerCalculatedLineColumn(Column) {
        var Name = Column instanceof tp.DataColumn ? Column.Name : "";
        var Table = Column instanceof tp.DataColumn ? Column.Table : null;
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockTradeLine")) {
            return tp.IsSameText(Name, "ProductId")
                || tp.IsSameText(Name, "UnitOfMeasureId")
                || tp.IsSameText(Name, "UnitRatio")
                || tp.IsSameText(Name, "Quantity")
                || tp.IsSameText(Name, "UnitCost");
        }
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockCountLine")) {
            return tp.IsSameText(Name, "ProductId")
                || tp.IsSameText(Name, "CountedQuantity")
                || tp.IsSameText(Name, "UnitCost");
        }
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "JournalEntryLine")) {
            return tp.IsSameText(Name, "DebitAmount")
                || tp.IsSameText(Name, "CreditAmount");
        }
        return tp.IsSameText(Name, "ProductId")
            || tp.IsSameText(Name, "TaxProductGroupId")
            || tp.IsSameText(Name, "UnitOfMeasureId")
            || tp.IsSameText(Name, "UnitRatio")
            || tp.IsSameText(Name, "Quantity")
            || tp.IsSameText(Name, "UnitPrice")
            || tp.IsSameText(Name, "DiscountPercent")
            || tp.IsSameText(Name, "DiscountAmount");
    }
    /**
     * Returns true when a modified header column requires server-side document calculation.
     * @param {tp.DataColumn|null|undefined} Column The modified column.
     * @returns {boolean} Returns true when server calculation is required.
     */
    IsServerCalculatedHeaderColumn(Column) {
        var Name = Column instanceof tp.DataColumn ? Column.Name : "";
        var Table = Column instanceof tp.DataColumn ? Column.Table : null;
        if (Table instanceof tp.DataTable && tp.IsSameText(Table.Name, "StockCount"))
            return tp.IsSameText(Name, "WarehouseId");
        return tp.IsSameText(Name, "PersonId")
            || tp.IsSameText(Name, "TradeDate")
            || tp.IsSameText(Name, "TradeTypeId")
            || tp.IsSameText(Name, "PriceListTypeId")
            || tp.IsSameText(Name, "CurrencyId")
            || tp.IsSameText(Name, "TaxBusinessGroupId")
            || tp.IsSameText(Name, "BranchId")
            || tp.IsSameText(Name, "OriginTaxJurisdictionId")
            || tp.IsSameText(Name, "DestinationTaxJurisdictionId")
            || tp.IsSameText(Name, "DiscountPercent")
            || tp.IsSameText(Name, "DiscountAmount")
            || tp.IsSameText(Name, "ChargesAmount")
            || tp.StartsWith(Name, "Billing", true)
            || tp.StartsWith(Name, "Shipping", true);
    }
    /**
     * Attaches server calculation handlers to the item data source.
     * @returns {void}
     */
    AttachItemServerCalculation() {
        var Source = this.DataSource;
        if (!(Source instanceof tp.DataSource))
            return;
        if (this.fDocumentItemCalculateSource === Source)
            return;
        if (this.fDocumentItemCalculateSource instanceof tp.DataSource && this.fDocumentItemCalculateListener)
            this.fDocumentItemCalculateSource.Off("RowModified", this.fDocumentItemCalculateListener);
        this.fDocumentItemCalculateSource = Source;
        this.fDocumentItemCalculateListener = Source.On("RowModified", function (Args) {
            if (this.Form && this.IsServerCalculatedHeaderColumn(Args ? Args.Column : null))
                this.Form.ScheduleDocumentCalculate(Source.Table ? Source.Table.Name : "", Args && Args.Column ? Args.Column.Name : "", Args ? Args.Row : null);
        }, this);
    }
    /**
     * Attaches server calculation handlers to a detail grid data source.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {void}
     */
    AttachDetailGridServerCalculation(Grid) {
        var Source;
        var Table;
        if (!(Grid instanceof tp.Grid))
            return;
        Source = Grid.DataSource;
        Table = Source instanceof tp.DataSource ? Source.Table : null;
        if (!(Source instanceof tp.DataSource) || !this.IsServerCalculatedTable(Table))
            return;
        if (Grid.fDocumentCalculateSource === Source)
            return;
        if (Grid.fDocumentCalculateSource instanceof tp.DataSource && Grid.fDocumentCalculateListener)
            Grid.fDocumentCalculateSource.Off("RowModified", Grid.fDocumentCalculateListener);
        Grid.fDocumentCalculateSource = Source;
        Grid.fDocumentCalculateListener = Source.On("RowModified", function (Args) {
            if (this.Form && this.IsServerCalculatedLineColumn(Args ? Args.Column : null))
                this.Form.ScheduleDocumentCalculate(Table ? Table.Name : "", Args && Args.Column ? Args.Column.Name : "", Args ? Args.Row : null);
        }, this);
    }

    // ● public
    /**
     * Creates columns for a detail grid.
     * @param {tp.Grid} Grid The detail grid.
     * @param {tp.DataTable|null|undefined} Table The optional detail table.
     * @returns {void}
     */
    CreateDetailGridColumns(Grid, Table) {
        var FieldNames;
        var Index;
        var Column;
        if (!(Grid instanceof tp.Grid))
            return;
        Table = Table instanceof tp.DataTable ? Table : Grid.DataSource instanceof tp.DataSource ? Grid.DataSource.Table : null;
        FieldNames = this.GetDetailGridFieldNames(Table);
        if (!tp.IsArray(FieldNames) || FieldNames.length === 0) {
            super.CreateDetailGridColumns(Grid, Table);
            return;
        }
        Grid.ClearColumns();
        for (Index = 0; Index < FieldNames.length; Index++) {
            Column = Table ? Table.FindColumn(FieldNames[Index]) : null;
            if (Column instanceof tp.DataColumn && this.CanRenderDetailGridColumn(Column, Table))
                this.AddDetailGridColumn(Grid, Column);
        }
    }
    /**
     * Creates a detail grid column for a data column.
     * @param {tp.Grid} Grid The detail grid.
     * @param {tp.DataColumn} Column The data column.
     * @returns {tp.GridColumn|null} Returns the created grid column or null.
     */
    AddDetailGridColumn(Grid, Column) {
        var ListSource;
        var Title;
        if (!(Grid instanceof tp.Grid) || !(Column instanceof tp.DataColumn))
            return null;
        Title = this.GetDetailGridColumnTitle(Column);
        if (Column.IsLocator)
            return Grid.AddLocatorColumn(Column.Name, Title, Column.Locator);
        if (Column.IsLookup) {
            ListSource = this.GetServerListSource(Column.LookupSource);
            if (ListSource instanceof tp.DataSource)
                return Grid.AddLookUpColumn(Column.Name, Title, "Id", "Name", ListSource);
        }
        return Grid.AddColumn(Column.Name, Title);
    }
    /**
     * Configures a detail grid for the current form state.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {void}
     */
    ConfigureDetailGrid(Grid) {
        super.ConfigureDetailGrid(Grid);
        this.ApplyDetailGridLocatorContext(Grid);
        this.AttachDetailGridBestFit(Grid);
        this.AttachDetailGridServerCalculation(Grid);
        this.ScheduleDetailGridBestFit(Grid);
    }
    /**
     * Builds the generated item page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async BuildAsync() {
        await super.BuildAsync();
        this.AttachItemServerCalculation();
    }
    /**
     * Adds a row to a detail grid and assigns the current master key to it.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {tp.DataRow|null} Returns the created row.
     */
    AddDetailGridRow(Grid) {
        var Source;
        var Table;
        var Row;
        var MasterSource;
        var MasterRow;
        var MasterValue;
        if (!this.CanExecuteDetailGridCommand(Grid, "GridRowInsert"))
            return null;
        Source = Grid.DataSource;
        Table = Source.Table;
        MasterSource = Source.MasterSource;
        if (MasterSource instanceof tp.DataSource && MasterSource.Current instanceof tp.DataRow) {
            MasterRow = MasterSource.Current;
            MasterValue = MasterRow.Get(Source.MasterKeyField);
            if (!tp.IsEmpty(MasterValue)) {
                Row = Table.NewRow();
                Row.SetByName(Source.DetailKeyField, MasterValue);
                this.ApplyDetailRowDefaults(Table, Row);
                Row = Table.AddRow(Row);
                Source.Update();
                Source.Current = Row;
                Grid.SetFocusedRow(Row);
                return Row;
            }
        }
        Row = Grid.InsertEmptyRow();
        if (Row instanceof tp.DataRow)
            this.ApplyDetailRowDefaults(Grid.DataSource.Table, Row);
        Source.Update();
        return Row;
    }
};

// ● document data form
/**
 * Base web data form for document modules.
 */
app.DocumentDataForm = class extends tp.WebDataForm {
    // ● constructor
    /**
     * Creates the document data form.
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
         * Server calculation debounce timer.
         * @type {number|null}
         */
        this.fDocumentCalculateTimer = null;
        /**
         * True while a server calculation request is running.
         * @type {boolean}
         */
        this.fDocumentCalculating = false;
        /**
         * True when another calculation is requested while one is running.
         * @type {boolean}
         */
        this.fDocumentCalculateAgain = false;
        /**
         * True while applying a server calculation packet.
         * @type {boolean}
         */
        this.fDocumentApplyingServerPacket = false;
        /**
         * Pending server calculation table name.
         * @type {string}
         */
        this.fDocumentCalculateTableName = "";
        /**
         * Pending server calculation field name.
         * @type {string}
         */
        this.fDocumentCalculateFieldName = "";
        /**
         * Pending server calculation row key.
         * @type {string}
         */
        this.fDocumentCalculateRowKey = "";
    }
    /**
     * Returns a row value by column name.
     * @param {tp.DataRow|null|undefined} Row The row.
     * @param {string} FieldName The field name.
     * @returns {*} Returns the value or an empty string.
     */
    GetDocumentRowValue(Row, FieldName) {
        return Row instanceof tp.DataRow && Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn(FieldName) >= 0
            ? Row.Get(FieldName)
            : "";
    }
    /**
     * Creates a document-posted notification packet from the current row.
     * @returns {object} Returns the notification packet.
     */
    CreateDocumentPostedArgs() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        return {
            ModuleName: this.ModuleName || "",
            DocumentId: this.GetDocumentRowValue(Row, "Id"),
            SourceId: this.GetDocumentRowValue(Row, "SourceId"),
            CancelsTradeId: this.GetDocumentRowValue(Row, "CancelsTradeId"),
            CancelledByTradeId: this.GetDocumentRowValue(Row, "CancelledByTradeId"),
            CancelsStockTradeId: this.GetDocumentRowValue(Row, "CancelsStockTradeId"),
            CancelledByStockTradeId: this.GetDocumentRowValue(Row, "CancelledByStockTradeId"),
            CancelledPaymentId: this.GetDocumentRowValue(Row, "CancelledPaymentId"),
            CancellationPaymentId: this.GetDocumentRowValue(Row, "CancellationPaymentId")
        };
    }
    /**
     * Returns true when a document-posted notification affects this form.
     * @param {object} Args The notification arguments.
     * @returns {boolean} Returns true when affected.
     */
    IsAffectedByDocumentPosted(Args) {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        var DocumentId = this.GetDocumentRowValue(Row, "Id");
        if (tp.IsEmpty(DocumentId) || !tp.IsObject(Args))
            return false;
        return tp.IsSameText(DocumentId, Args.DocumentId)
            || tp.IsSameText(DocumentId, Args.SourceId)
            || tp.IsSameText(DocumentId, Args.CancelsTradeId)
            || tp.IsSameText(DocumentId, Args.CancelledByTradeId)
            || tp.IsSameText(DocumentId, Args.CancelsStockTradeId)
            || tp.IsSameText(DocumentId, Args.CancelledByStockTradeId)
            || tp.IsSameText(DocumentId, Args.CancelledPaymentId)
            || tp.IsSameText(DocumentId, Args.CancellationPaymentId)
            || (tp.IsArray(Args.AffectedDocumentIds) && Args.AffectedDocumentIds.some((Id) => tp.IsSameText(DocumentId, Id)));
    }
    /**
     * Handles a document-posted notification.
     * @param {tp.EventArgs} Args The notification arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleDocumentPostedAsync(Args) {
        if (!this.IsAffectedByDocumentPosted(Args) || this.FormState !== tp.WebDataFormState.Edit)
            return;
        if (this.HasChanges() === true) {
            this.UiLog("Document changed by another form; refresh is skipped because this form has unsaved changes.");
            return;
        }
        await this.RefreshAsync();
    }
    /**
     * Returns a stable row key for preserving row state across calculation packets.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {string} Returns the row key.
     */
    GetDocumentRowStateKey(Table, Row) {
        var Value;
        if (!(Table instanceof tp.DataTable) || !(Row instanceof tp.DataRow) || Table.KeyFieldIndex < 0)
            return "";
        Value = Row.GetByIndex(Table.KeyFieldIndex);
        return tp.IsEmpty(Value) ? "" : String(Value);
    }
    /**
     * Returns a stable row key for a calculation request.
     * @param {tp.DataRow|null|undefined} Row The row.
     * @returns {string} Returns the row key.
     */
    GetDocumentCalculateRowKey(Row) {
        return Row instanceof tp.DataRow && Row.Table instanceof tp.DataTable
            ? this.GetDocumentRowStateKey(Row.Table, Row)
            : "";
    }
    /**
     * Captures non-unchanged row states before a calculation packet is applied.
     * @returns {object} Returns states by table and row key.
     */
    CaptureDocumentRowStates() {
        var Result = {};
        var TableIndex;
        var RowIndex;
        var Table;
        var Row;
        var Key;
        var TableStates;
        if (!(this.Module instanceof tp.DataModule) || !(this.Module.DataSet instanceof tp.DataSet))
            return Result;
        for (TableIndex = 0; TableIndex < this.Module.DataSet.Tables.length; TableIndex++) {
            Table = this.Module.DataSet.Tables[TableIndex];
            if (!(Table instanceof tp.DataTable))
                continue;
            for (RowIndex = 0; RowIndex < Table.Rows.length; RowIndex++) {
                Row = Table.Rows[RowIndex];
                if (!(Row instanceof tp.DataRow) || Row.State === tp.DataRowState.Unchanged)
                    continue;
                if (!Result[Table.Name])
                    Result[Table.Name] = { ByKey: {}, ByIndex: {} };
                TableStates = Result[Table.Name];
                TableStates.ByIndex[String(RowIndex)] = Row.State;
                Key = this.GetDocumentRowStateKey(Table, Row);
                if (!tp.IsBlank(Key))
                    TableStates.ByKey[Key] = Row.State;
            }
        }
        return Result;
    }
    /**
     * Restores dirty row states after a calculation packet has been applied.
     * @param {object} States The captured states.
     * @returns {void}
     */
    RestoreDocumentRowStates(States) {
        var TableName;
        var TableStates;
        var Key;
        var RowIndexText;
        var RowIndex;
        var Table;
        var Row;
        var State;
        if (!tp.IsObject(States) || !(this.Module instanceof tp.DataModule))
            return;
        for (TableName in States) {
            if (!Object.prototype.hasOwnProperty.call(States, TableName))
                continue;
            Table = this.Module.FindTable(TableName);
            TableStates = States[TableName];
            if (!(Table instanceof tp.DataTable) || !tp.IsObject(TableStates))
                continue;
            if (tp.IsObject(TableStates.ByKey)) {
                for (Key in TableStates.ByKey) {
                    if (!Object.prototype.hasOwnProperty.call(TableStates.ByKey, Key))
                        continue;
                    Row = Table.FindRow(Table.KeyField, Key);
                    if (!(Row instanceof tp.DataRow))
                        Row = Table.FindRow(Table.KeyField, tp.StrToInt(Key, Key));
                    State = TableStates.ByKey[Key];
                    if (Row instanceof tp.DataRow && Row.State === tp.DataRowState.Unchanged && State !== tp.DataRowState.Unchanged)
                        Row.State = State;
                }
            }
            if (tp.IsObject(TableStates.ByIndex)) {
                for (RowIndexText in TableStates.ByIndex) {
                    if (!Object.prototype.hasOwnProperty.call(TableStates.ByIndex, RowIndexText))
                        continue;
                    RowIndex = tp.StrToInt(RowIndexText, -1);
                    Row = tp.InRange(Table.Rows, RowIndex) ? Table.Rows[RowIndex] : null;
                    State = TableStates.ByIndex[RowIndexText];
                    if (Row instanceof tp.DataRow && Row.State === tp.DataRowState.Unchanged && State !== tp.DataRowState.Unchanged)
                        Row.State = State;
                }
            }
        }
    }
    /**
     * Applies a data module packet returned by a server-side calculation.
     * @param {object} Packet The Ajax response packet.
     * @returns {void}
     */
    ApplyDocumentCalculatePacket(Packet) {
        var Index;
        var Grid;
        var Source;
        var Tables;
        var RowStates;
        if (!Packet || !Packet.DataModule || !(this.Module instanceof tp.DataModule))
            return;
        RowStates = this.CaptureDocumentRowStates();
        this.fDocumentApplyingServerPacket = true;
        try {
            Tables = Packet.DataModule.DataSet && tp.IsArray(Packet.DataModule.DataSet.Tables) ? Packet.DataModule.DataSet.Tables : null;
            if (this.Module.DataSet instanceof tp.DataSet && tp.IsArray(Tables)) {
                this.Module.DataSet.AssignRows(Tables, true);
                this.RestoreDocumentRowStates(RowStates);
            }
        } finally {
            this.fDocumentApplyingServerPacket = false;
        }
        if (this.ItemPageBuilder instanceof app.DocumentItemPageBuilder) {
            if (this.ItemPageBuilder.DataSource instanceof tp.DataSource)
                this.ItemPageBuilder.DataSource.Update();
            for (Index = 0; Index < this.ItemPageBuilder.DetailSources.length; Index++) {
                Source = this.ItemPageBuilder.DetailSources[Index];
                if (Source instanceof tp.DataSource)
                    Source.Update();
            }
            for (Index = 0; Index < this.ItemPageBuilder.DetailGrids.length; Index++) {
                Grid = this.ItemPageBuilder.DetailGrids[Index];
                this.ItemPageBuilder.ScheduleDetailGridBestFit(Grid);
            }
        }
    }

    // ● protected
    /**
     * Creates the item page builder.
     * @returns {tp.WebItemPageBuilder} Returns the item page builder.
     */
    CreateItemPageBuilder() {
        return new app.DocumentItemPageBuilder(this);
    }
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("Post", "Post Document", "document_mark_as_final.png");
        if (this.ToolBar && this.Buttons.Save && this.Buttons.Post)
            this.ToolBar.PlaceControlAfter(this.Buttons.Save, this.Buttons.Post);
        this.UpdateToolBar();
    }
    /**
     * Returns the current document lifecycle status.
     * @returns {number} Returns the current document lifecycle status.
     */
    GetDocumentStatus() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        if (!(Row instanceof tp.DataRow))
            return 0;
        if (Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn("TradeStatusId") >= 0)
            return tp.StrToInt(Row.Get("TradeStatusId"), 0);
        if (Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn("StatusId") >= 0)
            return tp.StrToInt(Row.Get("StatusId"), 0);
        return 0;
    }
    /**
     * Returns true when the current document is cancelled.
     * @returns {boolean} Returns true when the document is cancelled.
     */
    IsDocumentCancelled() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        if (!(Row instanceof tp.DataRow) || !(Row.Table instanceof tp.DataTable) || Row.Table.IndexOfColumn("IsCancelled") < 0)
            return false;
        return tp.StrToBool(Row.Get("IsCancelled"), false);
    }
    /**
     * Returns true when the current document is locked.
     * @returns {boolean} Returns true when the document is locked.
     */
    IsDocumentLocked() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        if (!(Row instanceof tp.DataRow))
            return false;
        if (Row.Table instanceof tp.DataTable && Row.Table.IndexOfColumn("IsLocked") >= 0)
            return tp.StrToBool(Row.Get("IsLocked"), false) || this.GetDocumentStatus() !== 1;
        return this.GetDocumentStatus() !== 1;
    }
    /**
     * Returns true when the current document can be posted immediately.
     * @returns {boolean} Returns true when the current document can be posted.
     */
    CanPost() {
        return this.IsReadOnly !== true
            && this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 1
            && this.IsDocumentCancelled() !== true
            && this.IsDocumentLocked() !== true;
    }
    /**
     * Returns true when posting may be attempted.
     * @returns {boolean} Returns true when posting may be attempted.
     */
    CanAttemptPost() {
        return this.IsReadOnly !== true
            && this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.GetDocumentStatus() === 1
            && this.IsDocumentCancelled() !== true
            && this.IsDocumentLocked() !== true;
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "Post")
            await this.PostAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("Post", true);
        this.SetButtonEnabled("Save", this.IsButtonExecutable("Save") && this.IsDocumentLocked() !== true);
        this.SetButtonEnabled("Post", this.CanPost());
    }
    /**
     * Renders the generated item page.
     * @returns {Promise<void>} Returns a Promise.
     */
    async RenderItemPageAsync() {
        if (!(this.ItemPageBuilder instanceof app.DocumentItemPageBuilder))
            this.ItemPageBuilder = this.CreateItemPageBuilder();
        await this.ItemPageBuilder.BuildAsync();
    }

    // ● public
    /**
     * Posts the current document.
     * @returns {Promise<void>} Returns a Promise.
     */
    async PostAsync() {
        var Code;
        var DocumentText;
        var Message;
        var Packet;
        var Id;
        if (this.CanAttemptPost() !== true) {
            this.UpdateToolBar();
            return;
        }
        Code = this.Module.Row.Get("Code", "");
        DocumentText = tp.IsBlankString(Code) ? "document" : "document: " + Code;
        Message = "Post " + DocumentText + "?\n\nAfter posting, the document can no longer be edited.";
        if (await tp.YesNoBoxAsync(Message) !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute("App.DocumentDataModule.Post", {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                if (Packet && Packet.DataModule) {
                    this.Module.Assign(Packet.DataModule);
                    this.Module.CaptureCancelSnapshot();
                }
                Id = this.Module.Id;
                tp.Broadcaster.Send("Document.Posted", this, Packet && Packet.PostedInfo ? Packet.PostedInfo : this.CreateDocumentPostedArgs());
                this.UiLog("Posted " + this.GetItemLogText(Id));
                this.ReportSuccess("Posted " + this.GetItemLogText(Id));
                this.ListIsDirty = true;
                this.FormState = tp.WebDataFormState.Edit;
                await this.RenderItemPageAsync();
                await this.LoadFactBoxesAsync();
                this.ShowItemPage();
                this.UpdateToolBar();
            });
        } catch (e) {
            this.ReportError("Post failed: " + tp.ExceptionText(e));
        }
    }
    /**
     * Schedules a server-side commercial document calculation.
     * @param {string|null|undefined} TableName The table name of the changed field.
     * @param {string|null|undefined} FieldName The changed field name.
     * @param {tp.DataRow|null|undefined} Row The changed row.
     * @returns {void}
     */
    ScheduleDocumentCalculate(TableName, FieldName, Row) {
        var RowKey;
        if (this.fDocumentApplyingServerPacket === true || this.IsReadOnly === true || !(this.Module instanceof tp.DataModule))
            return;
        if (!tp.IsBlank(TableName) && !tp.IsBlank(FieldName) && (tp.IsBlank(this.fDocumentCalculateTableName) || tp.IsBlank(this.fDocumentCalculateFieldName))) {
            this.fDocumentCalculateTableName = String(TableName);
            this.fDocumentCalculateFieldName = String(FieldName);
            RowKey = this.GetDocumentCalculateRowKey(Row);
            this.fDocumentCalculateRowKey = RowKey;
        }
        if (this.fDocumentCalculateTimer)
            clearTimeout(this.fDocumentCalculateTimer);
        this.fDocumentCalculateTimer = setTimeout(() => {
            this.fDocumentCalculateTimer = null;
            this.CalculateDocumentAsync();
        }, 250);
    }
    /**
     * Executes a server-side commercial document calculation.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CalculateDocumentAsync() {
        var Packet;
        var TableName;
        var FieldName;
        var RowKey;
        if (this.fDocumentApplyingServerPacket === true || this.IsReadOnly === true || !(this.Module instanceof tp.DataModule))
            return;
        if (this.fDocumentCalculating === true) {
            this.fDocumentCalculateAgain = true;
            return;
        }
        this.fDocumentCalculating = true;
        TableName = this.fDocumentCalculateTableName || "";
        FieldName = this.fDocumentCalculateFieldName || "";
        RowKey = this.fDocumentCalculateRowKey || "";
        this.fDocumentCalculateTableName = "";
        this.fDocumentCalculateFieldName = "";
        this.fDocumentCalculateRowKey = "";
        try {
            Packet = await tp.AjaxRequest.Execute("App.DocumentDataModule.Calculate", {
                ModuleName: this.ModuleName,
                TableName: TableName,
                FieldName: FieldName,
                RowKey: RowKey,
                DataModuleJson: JSON.stringify(tp.IsFunction(this.Module.toDataJSON) ? this.Module.toDataJSON() : this.Module.toJSON())
            });
            this.ApplyDocumentCalculatePacket(Packet);
            this.UiLog("Calculated " + (!tp.IsBlank(TableName) && !tp.IsBlank(FieldName) ? TableName + "." + FieldName : "document"));
        } catch (e) {
            if (tp.LogBox && tp.LogBox.AppendLine)
                tp.LogBox.AppendLine("Document calculation failed: " + tp.ExceptionText(e));
        } finally {
            this.fDocumentCalculating = false;
        }
        if (this.fDocumentCalculateAgain === true) {
            this.fDocumentCalculateAgain = false;
            this.ScheduleDocumentCalculate();
        }
    }
    /**
     * Handles a broadcaster event.
     * @param {string} EventName The event name.
     * @param {tp.EventArgs} Args The broadcaster arguments.
     * @returns {void}
     */
    HandleBroadcasterEvent(EventName, Args) {
        super.HandleBroadcasterEvent(EventName, Args);
        if (!tp.IsSameText(EventName, "Document.Posted"))
            return;
        this.HandleDocumentPostedAsync(Args).catch((e) => {
            this.ReportError("Document notification failed: " + tp.ExceptionText(e));
        });
    }
};

// ● stock trade form
/**
 * Web data form for Stock Transaction documents.
 */
app.StockTradeForm = class extends app.DocumentDataForm {
    // ● constructor
    /**
     * Creates the Stock Transaction form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateCancellation", "Create Stock Cancellation", "document_torn.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateCancellation)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateCancellation);
        this.UpdateToolBar();
    }
    /**
     * Returns true when the current Stock Transaction can create a cancellation.
     * @returns {boolean} Returns true when cancellation can be created.
     */
    CanCreateCancellation() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        return this.FormState === tp.WebDataFormState.Edit
            && Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true
            && tp.IsBlank(this.GetDocumentRowValue(Row, "CancelsStockTradeId"))
            && tp.IsBlank(this.GetDocumentRowValue(Row, "CancelledByStockTradeId"));
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateCancellation")
            await this.CreateCancellationAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateCancellation", true);
        this.SetButtonEnabled("CreateCancellation", this.CanCreateCancellation());
    }

    // ● public
    /**
     * Creates a Stock Transaction cancellation from the current Stock Transaction.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCancellationAsync() {
        var Code;
        var DocumentText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateCancellation() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        DocumentText = tp.IsBlankString(Code) ? "Stock Transaction" : "Stock Transaction: " + Code;
        if (await tp.YesNoBoxAsync("Create a cancellation for " + DocumentText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute("App.StockTrade.CreateCancellation", {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : "";
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (tp.IsBlankString(WebFormName))
                    WebFormName = DataModulePacket && DataModulePacket.Name ? DataModulePacket.Name : "";
                if (!DataModulePacket || tp.IsBlankString(WebFormName))
                    throw new Error("Stock Cancellation data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: "Stock Cancellation",
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created Stock Cancellation from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create Stock Cancellation failed: " + tp.ExceptionText(e));
        }
    }
};

// ● trade data form
/**
 * Base web data form for commercial trade document modules.
 */
app.TradeDataForm = class extends app.DocumentDataForm {
    // ● constructor
    /**
     * Creates the trade data form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }
};

// ● sales item page builder
/**
 * Builds item pages for sales document forms.
 */
app.SalesItemPageBuilder = class extends app.DocumentItemPageBuilder {
    // ● constructor
    /**
     * Creates the sales item page builder.
     * @param {tp.WebDataForm} Form The owner data form.
     */
    constructor(Form) {
        super(Form);
    }

    // ● protected
    /**
     * Returns the field names to display in a sales document detail grid.
     * @param {tp.DataTable} Table The detail table.
     * @returns {string[]|null} Returns field names or null to use the default columns.
     */
    GetDetailGridFieldNames(Table) {
        if (this.IsTradeLineTable(Table)) {
            return [
                "DisplayOrder",
                "LineTypeId",
                "ProductCode",
                "ProductName",
                "UnitOfMeasureName",
                "Quantity",
                "UnitPrice",
                "GrossAmount",
                "DiscountPercent",
                "DiscountAmount",
                "DocumentDiscountAmount",
                "NetAmount",
                "TaxPercent",
                "TaxAmount",
                "TotalAmount"
            ];
        }
        return super.GetDetailGridFieldNames(Table);
    }
    /**
     * Applies sales-specific defaults to a newly created detail row.
     * @param {tp.DataTable} Table The detail table.
     * @param {tp.DataRow} Row The detail row.
     * @returns {void}
     */
    ApplyDetailRowDefaults(Table, Row) {
        super.ApplyDetailRowDefaults(Table, Row);
        if (this.IsTradeLineTable(Table) && Table.IndexOfColumn("Quantity") >= 0)
            Row.Set("Quantity", 1);
    }
};

// ● sales data form
/**
 * Base web data form for sales document modules.
 */
app.SalesDataForm = class extends app.TradeDataForm {
    // ● constructor
    /**
     * Creates the sales data form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the item page builder.
     * @returns {tp.WebItemPageBuilder} Returns the item page builder.
     */
    CreateItemPageBuilder() {
        return new app.SalesItemPageBuilder(this);
    }
};

// ● sales order form
/**
 * Web data form for sales orders.
 */
app.SalesOrderForm = class extends app.SalesDataForm {
    // ● constructor
    /**
     * Creates the sales order form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateDeliveryNote", "Create Sales Delivery Note", "document_export.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateDeliveryNote)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateDeliveryNote);
        this.UpdateToolBar();
    }
    /**
     * Returns true when a sales delivery note can be created.
     * @returns {boolean} Returns true when a sales delivery note can be created.
     */
    CanCreateDeliveryNote() {
        return this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true;
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateDeliveryNote")
            await this.CreateDeliveryNoteAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateDeliveryNote", true);
        this.SetButtonEnabled("CreateDeliveryNote", this.CanCreateDeliveryNote());
    }

    // ● public
    /**
     * Creates a sales delivery note from the current sales order.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateDeliveryNoteAsync() {
        var Code;
        var OrderText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateDeliveryNote() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        OrderText = tp.IsBlankString(Code) ? "Sales Order" : "Sales Order: " + Code;
        if (await tp.YesNoBoxAsync("Create a Sales Delivery Note from " + OrderText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute("App.SalesOrder.CreateDeliveryNote", {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : "SalesDeliveryNote";
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (!DataModulePacket)
                    throw new Error("Sales Delivery Note data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: "Sales Delivery Note",
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created Sales Delivery Note from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create Sales Delivery Note failed: " + tp.ExceptionText(e));
        }
    }
};

// ● sales delivery note form
/**
 * Web data form for sales delivery notes.
 */
app.SalesDeliveryNoteForm = class extends app.SalesDataForm {
    // ● constructor
    /**
     * Creates the sales delivery note form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateReturn", "Create Sales Return", "document_redirect.png");
        this.AddToolBarButton("CreateInvoice", "Create Sales Invoice", "document_export.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateReturn)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateReturn);
        if (this.ToolBar && this.Buttons.CreateReturn && this.Buttons.CreateInvoice)
            this.ToolBar.PlaceControlAfter(this.Buttons.CreateReturn, this.Buttons.CreateInvoice);
        this.UpdateToolBar();
    }
    /**
     * Returns true when a document can be created from the current sales delivery note.
     * @returns {boolean} Returns true when the current delivery note can be transformed.
     */
    CanCreateFromDeliveryNote() {
        return this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true;
    }
    /**
     * Returns true when at least one delivery line has quantity available for the specified counters.
     * @param {string[]} CounterFieldNames The consumed quantity field names.
     * @returns {boolean} Returns true when remaining quantity exists.
     */
    HasRemainingDeliveryQuantity(CounterFieldNames) {
        var Table = this.Module instanceof tp.DataModule ? this.Module.FindTable("TradeLine") : null;
        var Index;
        var Row;
        var UsedQuantity;
        var FieldIndex;
        if (!(Table instanceof tp.DataTable) || !tp.IsArray(CounterFieldNames))
            return false;
        for (Index = 0; Index < Table.Rows.length; Index++) {
            Row = Table.Rows[Index];
            if (!(Row instanceof tp.DataRow) || Row.State === tp.DataRowState.Deleted || Row.State === tp.DataRowState.Detached)
                continue;
            UsedQuantity = 0;
            for (FieldIndex = 0; FieldIndex < CounterFieldNames.length; FieldIndex++)
                UsedQuantity += tp.StrToFloat(Row.Get(CounterFieldNames[FieldIndex], 0), 0);
            if (tp.StrToFloat(Row.Get("Quantity", 0), 0) > UsedQuantity)
                return true;
        }
        return false;
    }
    /**
     * Returns true when a sales return can be created from the current delivery note.
     * @returns {boolean} Returns true when return quantity remains.
     */
    CanCreateReturn() {
        return this.CanCreateFromDeliveryNote()
            && this.HasRemainingDeliveryQuantity(["ReturnedQuantity"]);
    }
    /**
     * Returns true when a sales invoice can be created from the current delivery note.
     * @returns {boolean} Returns true when invoice quantity remains.
     */
    CanCreateInvoice() {
        return this.CanCreateFromDeliveryNote()
            && this.HasRemainingDeliveryQuantity(["InvoicedQuantity"]);
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateReturn")
            await this.CreateReturnAsync();
        else if (Command === "CreateInvoice")
            await this.CreateInvoiceAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateReturn", true);
        this.SetButtonEnabled("CreateReturn", this.CanCreateReturn());
        this.SetButtonVisible("CreateInvoice", true);
        this.SetButtonEnabled("CreateInvoice", this.CanCreateInvoice());
    }
    /**
     * Creates a transformed document from the current sales delivery note.
     * @param {string} OperationName The Ajax operation name.
     * @param {string} DefaultWebFormName The default target web form name.
     * @param {string} TargetTitle The target document title.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateTransformedDocumentAsync(OperationName, DefaultWebFormName, TargetTitle) {
        var Code;
        var DeliveryText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateFromDeliveryNote() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        DeliveryText = tp.IsBlankString(Code) ? "Sales Delivery Note" : "Sales Delivery Note: " + Code;
        if (await tp.YesNoBoxAsync("Create a " + TargetTitle + " from " + DeliveryText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute(OperationName, {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : DefaultWebFormName;
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (!DataModulePacket)
                    throw new Error(TargetTitle + " data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: TargetTitle,
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created " + TargetTitle + " from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create " + TargetTitle + " failed: " + tp.ExceptionText(e));
        }
    }

    // ● public
    /**
     * Creates a sales return from the current sales delivery note.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateReturnAsync() {
        await this.CreateTransformedDocumentAsync("App.SalesDeliveryNote.CreateReturn", "SalesReturn", "Sales Return");
    }
    /**
     * Creates a sales invoice from the current sales delivery note.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateInvoiceAsync() {
        await this.CreateTransformedDocumentAsync("App.SalesDeliveryNote.CreateInvoice", "SalesInvoice", "Sales Invoice");
    }
};

// ● sales invoice form
/**
 * Web data form for sales invoices.
 */
app.SalesInvoiceForm = class extends app.SalesDataForm {
    // ● constructor
    /**
     * Creates the sales invoice form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateCustomerReceipt", "Create Customer Receipt", "coins_add.png");
        this.AddToolBarButton("CreateCreditNote", "Create Sales Credit Note", "document_redirect.png");
        this.AddToolBarButton("CreateCancellation", "Create Sales Cancellation", "document_torn.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateCustomerReceipt)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateCustomerReceipt);
        if (this.ToolBar && this.Buttons.CreateCustomerReceipt && this.Buttons.CreateCreditNote)
            this.ToolBar.PlaceControlAfter(this.Buttons.CreateCustomerReceipt, this.Buttons.CreateCreditNote);
        if (this.ToolBar && this.Buttons.CreateCreditNote && this.Buttons.CreateCancellation)
            this.ToolBar.PlaceControlAfter(this.Buttons.CreateCreditNote, this.Buttons.CreateCancellation);
        this.UpdateToolBar();
    }
    /**
     * Returns true when a document can be created from the current sales invoice.
     * @returns {boolean} Returns true when the current invoice can create related documents.
     */
    CanCreateFromInvoice() {
        return this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true;
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateCustomerReceipt")
            await this.CreateCustomerReceiptAsync();
        else if (Command === "CreateCreditNote")
            await this.CreateCreditNoteAsync();
        else if (Command === "CreateCancellation")
            await this.CreateCancellationAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateCustomerReceipt", true);
        this.SetButtonEnabled("CreateCustomerReceipt", this.CanCreateFromInvoice());
        this.SetButtonVisible("CreateCreditNote", true);
        this.SetButtonEnabled("CreateCreditNote", this.CanCreateFromInvoice());
        this.SetButtonVisible("CreateCancellation", true);
        this.SetButtonEnabled("CreateCancellation", this.CanCreateFromInvoice());
    }
    /**
     * Creates a related document from the current sales invoice.
     * @param {string} OperationName The Ajax operation name.
     * @param {string} DefaultWebFormName The default target web form name.
     * @param {string} TargetTitle The target document title.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateRelatedDocumentAsync(OperationName, DefaultWebFormName, TargetTitle) {
        var Code;
        var InvoiceText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateFromInvoice() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        InvoiceText = tp.IsBlankString(Code) ? "Sales Invoice" : "Sales Invoice: " + Code;
        if (await tp.YesNoBoxAsync("Create a " + TargetTitle + " from " + InvoiceText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute(OperationName, {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : DefaultWebFormName;
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (!DataModulePacket)
                    throw new Error(TargetTitle + " data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: TargetTitle,
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created " + TargetTitle + " from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create " + TargetTitle + " failed: " + tp.ExceptionText(e));
        }
    }

    // ● public
    /**
     * Creates a customer receipt from the current sales invoice.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCustomerReceiptAsync() {
        await this.CreateRelatedDocumentAsync("App.SalesInvoice.CreateCustomerReceipt", "CustomerReceipt", "Customer Receipt");
    }
    /**
     * Creates a sales credit note from the current sales invoice.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCreditNoteAsync() {
        await this.CreateRelatedDocumentAsync("App.SalesInvoice.CreateCreditNote", "SalesCreditNote", "Sales Credit Note");
    }
    /**
     * Creates a sales cancellation from the current sales invoice.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCancellationAsync() {
        await this.CreateRelatedDocumentAsync("App.SalesInvoice.CreateCancellation", "SalesCancellation", "Sales Cancellation");
    }
};

// ● purchase item page builder
/**
 * Builds item pages for purchase document forms.
 */
app.PurchaseItemPageBuilder = class extends app.DocumentItemPageBuilder {
    // ● constructor
    /**
     * Creates the purchase item page builder.
     * @param {tp.WebDataForm} Form The owner data form.
     */
    constructor(Form) {
        super(Form);
    }

    // ● protected
    /**
     * Returns the field names to display in a purchase document detail grid.
     * @param {tp.DataTable} Table The detail table.
     * @returns {string[]|null} Returns field names or null to use the default columns.
     */
    GetDetailGridFieldNames(Table) {
        if (this.IsTradeLineTable(Table)) {
            return [
                "DisplayOrder",
                "LineTypeId",
                "ProductCode",
                "ProductName",
                "UnitOfMeasureName",
                "Quantity",
                "UnitPrice",
                "GrossAmount",
                "DiscountPercent",
                "DiscountAmount",
                "DocumentDiscountAmount",
                "NetAmount",
                "TaxPercent",
                "TaxAmount",
                "TotalAmount"
            ];
        }
        return super.GetDetailGridFieldNames(Table);
    }
    /**
     * Applies purchase-specific defaults to a newly created detail row.
     * @param {tp.DataTable} Table The detail table.
     * @param {tp.DataRow} Row The detail row.
     * @returns {void}
     */
    ApplyDetailRowDefaults(Table, Row) {
        super.ApplyDetailRowDefaults(Table, Row);
        if (this.IsTradeLineTable(Table) && Table.IndexOfColumn("Quantity") >= 0)
            Row.Set("Quantity", 1);
    }
};

// ● purchase data form
/**
 * Base web data form for purchase document modules.
 */
app.PurchaseDataForm = class extends app.TradeDataForm {
    // ● constructor
    /**
     * Creates the purchase data form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the item page builder.
     * @returns {tp.WebItemPageBuilder} Returns the item page builder.
     */
    CreateItemPageBuilder() {
        return new app.PurchaseItemPageBuilder(this);
    }
};

// ● purchase order form
/**
 * Web data form for purchase orders.
 */
app.PurchaseOrderForm = class extends app.PurchaseDataForm {
    // ● constructor
    /**
     * Creates the purchase order form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateDeliveryNote", "Create Purchase Delivery Note", "document_export.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateDeliveryNote)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateDeliveryNote);
        this.UpdateToolBar();
    }
    /**
     * Returns true when a purchase delivery note can be created.
     * @returns {boolean} Returns true when a purchase delivery note can be created.
     */
    CanCreateDeliveryNote() {
        return this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true;
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateDeliveryNote")
            await this.CreateDeliveryNoteAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateDeliveryNote", true);
        this.SetButtonEnabled("CreateDeliveryNote", this.CanCreateDeliveryNote());
    }

    // ● public
    /**
     * Creates a purchase delivery note from the current purchase order.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateDeliveryNoteAsync() {
        var Code;
        var OrderText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateDeliveryNote() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        OrderText = tp.IsBlankString(Code) ? "Purchase Order" : "Purchase Order: " + Code;
        if (await tp.YesNoBoxAsync("Create a Purchase Delivery Note from " + OrderText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute("App.PurchaseOrder.CreateDeliveryNote", {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : "PurchaseDeliveryNote";
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (!DataModulePacket)
                    throw new Error("Purchase Delivery Note data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: "Purchase Delivery Note",
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created Purchase Delivery Note from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create Purchase Delivery Note failed: " + tp.ExceptionText(e));
        }
    }
};

// ● purchase delivery note form
/**
 * Web data form for purchase delivery notes.
 */
app.PurchaseDeliveryNoteForm = class extends app.PurchaseDataForm {
    // ● constructor
    /**
     * Creates the purchase delivery note form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateReturn", "Create Purchase Return", "document_redirect.png");
        this.AddToolBarButton("CreateInvoice", "Create Purchase Invoice", "document_export.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateReturn)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateReturn);
        if (this.ToolBar && this.Buttons.CreateReturn && this.Buttons.CreateInvoice)
            this.ToolBar.PlaceControlAfter(this.Buttons.CreateReturn, this.Buttons.CreateInvoice);
        this.UpdateToolBar();
    }
    /**
     * Returns true when a document can be created from the current purchase delivery note.
     * @returns {boolean} Returns true when the current delivery note can be transformed.
     */
    CanCreateFromDeliveryNote() {
        return this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true;
    }
    /**
     * Returns true when at least one delivery line has quantity available for the specified counters.
     * @param {string[]} CounterFieldNames The consumed quantity field names.
     * @returns {boolean} Returns true when remaining quantity exists.
     */
    HasRemainingDeliveryQuantity(CounterFieldNames) {
        var Table = this.Module instanceof tp.DataModule ? this.Module.FindTable("TradeLine") : null;
        var Index;
        var Row;
        var UsedQuantity;
        var FieldIndex;
        if (!(Table instanceof tp.DataTable) || !tp.IsArray(CounterFieldNames))
            return false;
        for (Index = 0; Index < Table.Rows.length; Index++) {
            Row = Table.Rows[Index];
            if (!(Row instanceof tp.DataRow) || Row.State === tp.DataRowState.Deleted || Row.State === tp.DataRowState.Detached)
                continue;
            UsedQuantity = 0;
            for (FieldIndex = 0; FieldIndex < CounterFieldNames.length; FieldIndex++)
                UsedQuantity += tp.StrToFloat(Row.Get(CounterFieldNames[FieldIndex], 0), 0);
            if (tp.StrToFloat(Row.Get("Quantity", 0), 0) > UsedQuantity)
                return true;
        }
        return false;
    }
    /**
     * Returns true when a purchase return can be created from the current delivery note.
     * @returns {boolean} Returns true when return quantity remains.
     */
    CanCreateReturn() {
        return this.CanCreateFromDeliveryNote()
            && this.HasRemainingDeliveryQuantity(["ReturnedQuantity"]);
    }
    /**
     * Returns true when a purchase invoice can be created from the current delivery note.
     * @returns {boolean} Returns true when invoice quantity remains.
     */
    CanCreateInvoice() {
        return this.CanCreateFromDeliveryNote()
            && this.HasRemainingDeliveryQuantity(["InvoicedQuantity"]);
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateReturn")
            await this.CreateReturnAsync();
        else if (Command === "CreateInvoice")
            await this.CreateInvoiceAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateReturn", true);
        this.SetButtonEnabled("CreateReturn", this.CanCreateReturn());
        this.SetButtonVisible("CreateInvoice", true);
        this.SetButtonEnabled("CreateInvoice", this.CanCreateInvoice());
    }
    /**
     * Creates a transformed document from the current purchase delivery note.
     * @param {string} OperationName The Ajax operation name.
     * @param {string} DefaultWebFormName The default target web form name.
     * @param {string} TargetTitle The target document title.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateTransformedDocumentAsync(OperationName, DefaultWebFormName, TargetTitle) {
        var Code;
        var DeliveryText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateFromDeliveryNote() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        DeliveryText = tp.IsBlankString(Code) ? "Purchase Delivery Note" : "Purchase Delivery Note: " + Code;
        if (await tp.YesNoBoxAsync("Create a " + TargetTitle + " from " + DeliveryText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute(OperationName, {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : DefaultWebFormName;
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (!DataModulePacket)
                    throw new Error(TargetTitle + " data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: TargetTitle,
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created " + TargetTitle + " from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create " + TargetTitle + " failed: " + tp.ExceptionText(e));
        }
    }

    // ● public
    /**
     * Creates a purchase return from the current purchase delivery note.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateReturnAsync() {
        await this.CreateTransformedDocumentAsync("App.PurchaseDeliveryNote.CreateReturn", "PurchaseReturn", "Purchase Return");
    }
    /**
     * Creates a purchase invoice from the current purchase delivery note.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateInvoiceAsync() {
        await this.CreateTransformedDocumentAsync("App.PurchaseDeliveryNote.CreateInvoice", "PurchaseInvoice", "Purchase Invoice");
    }
};

// ● purchase invoice form
/**
 * Web data form for purchase invoices.
 */
app.PurchaseInvoiceForm = class extends app.PurchaseDataForm {
    // ● constructor
    /**
     * Creates the purchase invoice form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateSupplierPayment", "Create Supplier Payment", "coins_delete.png");
        this.AddToolBarButton("CreateCreditNote", "Create Purchase Credit Note", "document_redirect.png");
        this.AddToolBarButton("CreateCancellation", "Create Purchase Cancellation", "document_torn.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateSupplierPayment)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateSupplierPayment);
        if (this.ToolBar && this.Buttons.CreateSupplierPayment && this.Buttons.CreateCreditNote)
            this.ToolBar.PlaceControlAfter(this.Buttons.CreateSupplierPayment, this.Buttons.CreateCreditNote);
        if (this.ToolBar && this.Buttons.CreateCreditNote && this.Buttons.CreateCancellation)
            this.ToolBar.PlaceControlAfter(this.Buttons.CreateCreditNote, this.Buttons.CreateCancellation);
        this.UpdateToolBar();
    }
    /**
     * Returns true when a document can be created from the current purchase invoice.
     * @returns {boolean} Returns true when the current invoice can create related documents.
     */
    CanCreateFromInvoice() {
        return this.FormState === tp.WebDataFormState.Edit
            && this.Module instanceof tp.DataModule
            && this.Module.Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true;
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateSupplierPayment")
            await this.CreateSupplierPaymentAsync();
        else if (Command === "CreateCreditNote")
            await this.CreateCreditNoteAsync();
        else if (Command === "CreateCancellation")
            await this.CreateCancellationAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateSupplierPayment", true);
        this.SetButtonEnabled("CreateSupplierPayment", this.CanCreateFromInvoice());
        this.SetButtonVisible("CreateCreditNote", true);
        this.SetButtonEnabled("CreateCreditNote", this.CanCreateFromInvoice());
        this.SetButtonVisible("CreateCancellation", true);
        this.SetButtonEnabled("CreateCancellation", this.CanCreateFromInvoice());
    }
    /**
     * Creates a related document from the current purchase invoice.
     * @param {string} OperationName The Ajax operation name.
     * @param {string} DefaultWebFormName The default target web form name.
     * @param {string} TargetTitle The target document title.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateRelatedDocumentAsync(OperationName, DefaultWebFormName, TargetTitle) {
        var Code;
        var InvoiceText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        if (this.CanCreateFromInvoice() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        InvoiceText = tp.IsBlankString(Code) ? "Purchase Invoice" : "Purchase Invoice: " + Code;
        if (await tp.YesNoBoxAsync("Create a " + TargetTitle + " from " + InvoiceText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute(OperationName, {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : DefaultWebFormName;
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (!DataModulePacket)
                    throw new Error(TargetTitle + " data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: TargetTitle,
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created " + TargetTitle + " from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create " + TargetTitle + " failed: " + tp.ExceptionText(e));
        }
    }

    // ● public
    /**
     * Creates a supplier payment from the current purchase invoice.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateSupplierPaymentAsync() {
        await this.CreateRelatedDocumentAsync("App.PurchaseInvoice.CreateSupplierPayment", "SupplierPayment", "Supplier Payment");
    }
    /**
     * Creates a purchase credit note from the current purchase invoice.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCreditNoteAsync() {
        await this.CreateRelatedDocumentAsync("App.PurchaseInvoice.CreateCreditNote", "PurchaseCreditNote", "Purchase Credit Note");
    }
    /**
     * Creates a purchase cancellation from the current purchase invoice.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCancellationAsync() {
        await this.CreateRelatedDocumentAsync("App.PurchaseInvoice.CreateCancellation", "PurchaseCancellation", "Purchase Cancellation");
    }
};

// ● payment item page builder
/**
 * Builds item pages for payment document forms.
 */
app.PaymentItemPageBuilder = class extends app.DocumentItemPageBuilder {
    // ● constructor
    /**
     * Creates the payment item page builder.
     * @param {tp.WebDataForm} Form The owner data form.
     */
    constructor(Form) {
        super(Form);
    }

    // ● public
    /**
     * Returns true when a detail grid add or delete command can execute.
     * @param {tp.Grid} Grid The detail grid.
     * @param {string} Command The command name.
     * @returns {boolean} Returns true when the command can execute.
     */
    CanExecuteDetailGridCommand(Grid, Command) {
        var Table = Grid instanceof tp.Grid && Grid.DataSource instanceof tp.DataSource ? Grid.DataSource.Table : null;
        if (this.Form instanceof app.PaymentDataForm
            && this.Form.IsPaymentCancellation() === true
            && Table instanceof tp.DataTable
            && tp.IsSameText(Table.Name, "PaymentSettlement")
            && Command === "GridRowInsert")
            return false;
        return super.CanExecuteDetailGridCommand(Grid, Command);
    }
};

// ● payment data form
/**
 * Base web data form for payment documents.
 */
app.PaymentDataForm = class extends app.DocumentDataForm {
    // ● constructor
    /**
     * Creates the payment data form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the item page builder.
     * @returns {tp.WebItemPageBuilder} Returns the item page builder.
     */
    CreateItemPageBuilder() {
        return new app.PaymentItemPageBuilder(this);
    }

    // ● public
    /**
     * Returns true when this payment form displays a cancellation document.
     * @returns {boolean} Returns true for payment cancellation forms.
     */
    IsPaymentCancellation() {
        return tp.IsSameText(this.ModuleName, "CustomerReceiptCancellation")
            || tp.IsSameText(this.ModuleName, "SupplierPaymentCancellation");
    }
};

// ● payment form
/**
 * Web data form for customer receipts and supplier payments.
 */
app.PaymentForm = class extends app.PaymentDataForm {
    // ● constructor
    /**
     * Creates the payment form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Creates the main toolbar.
     * @param {HTMLElement} Element The host element.
     * @returns {void}
     */
    CreateToolBar(Element) {
        super.CreateToolBar(Element);
        this.AddToolBarButton("CreateCancellation", "Create " + this.GetPaymentCancellationTitle(), "document_torn.png");
        if (this.ToolBar && this.Buttons.Post && this.Buttons.CreateCancellation)
            this.ToolBar.PlaceControlAfter(this.Buttons.Post, this.Buttons.CreateCancellation);
        this.UpdateToolBar();
    }
    /**
     * Returns the payment module name.
     * @returns {string} Returns the module name.
     */
    GetPaymentModuleName() {
        if (this.Module instanceof tp.DataModule && !tp.IsBlankString(this.Module.Name))
            return this.Module.Name;
        if (!tp.IsBlankString(this.ModuleName))
            return this.ModuleName;
        return this.GetWebFormName();
    }
    /**
     * Returns true when this form displays a supplier payment.
     * @returns {boolean} Returns true for supplier payment forms.
     */
    IsSupplierPayment() {
        return tp.IsSameText(this.GetPaymentModuleName(), "SupplierPayment");
    }
    /**
     * Returns the payment document title.
     * @returns {string} Returns the payment document title.
     */
    GetPaymentTitle() {
        return this.IsSupplierPayment() ? "Supplier Payment" : "Customer Receipt";
    }
    /**
     * Returns the payment cancellation document title.
     * @returns {string} Returns the payment cancellation document title.
     */
    GetPaymentCancellationTitle() {
        return this.IsSupplierPayment() ? "Supplier Payment Cancellation" : "Customer Receipt Cancellation";
    }
    /**
     * Returns true when the current payment can create a cancellation document.
     * @returns {boolean} Returns true when cancellation can be created.
     */
    CanCreateCancellation() {
        var Row = this.Module instanceof tp.DataModule ? this.Module.Row : null;
        return this.FormState === tp.WebDataFormState.Edit
            && Row instanceof tp.DataRow
            && this.HasChanges() !== true
            && this.GetDocumentStatus() === 2
            && this.IsDocumentCancelled() !== true
            && tp.IsBlank(this.GetDocumentRowValue(Row, "CancelledPaymentId"))
            && tp.IsBlank(this.GetDocumentRowValue(Row, "CancellationPaymentId"));
    }
    /**
     * Handles toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async HandleToolBarButtonClick(Args) {
        var Command = Args ? Args.Command : "";
        if (Command === "CreateCancellation")
            await this.CreateCancellationAsync();
        else
            await super.HandleToolBarButtonClick(Args);
    }
    /**
     * Enables or disables form commands.
     * @returns {void}
     */
    EnableCommands() {
        super.EnableCommands();
        this.SetButtonVisible("CreateCancellation", true);
        if (this.Buttons.CreateCancellation instanceof tp.ButtonEx)
            this.Buttons.CreateCancellation.ToolTip = "Create " + this.GetPaymentCancellationTitle();
        this.SetButtonEnabled("CreateCancellation", this.CanCreateCancellation());
    }

    // ● public
    /**
     * Saves the current payment and displays amount adjustment feedback.
     * @returns {Promise<void>} Returns a Promise.
     */
    async SaveAsync() {
        await super.SaveAsync();
        if (this.Module instanceof app.PaymentDataModule && !tp.IsBlankString(this.Module.AmountAdjustmentMessage) && this.HasChanges() !== true) {
            this.UiLog(this.Module.AmountAdjustmentMessage);
            tp.InfoNote(this.Module.AmountAdjustmentMessage);
            this.Module.AmountAdjustmentMessage = "";
        }
    }
    /**
     * Creates a payment cancellation from the current payment.
     * @returns {Promise<void>} Returns a Promise.
     */
    async CreateCancellationAsync() {
        var Code;
        var PaymentText;
        var Packet;
        var WebFormName;
        var DataModulePacket;
        var TargetTitle = this.GetPaymentCancellationTitle();
        if (this.CanCreateCancellation() !== true)
            return;
        Code = this.Module.Row.Get("Code", "");
        PaymentText = tp.IsBlankString(Code) ? this.GetPaymentTitle() : this.GetPaymentTitle() + ": " + Code;
        if (await tp.YesNoBoxAsync("Create a " + TargetTitle + " from " + PaymentText + "?") !== true)
            return;
        try {
            await this.ExecuteWithSpinner(async function () {
                Packet = await tp.AjaxRequest.Execute("App.Payment.CreateCancellation", {
                    ModuleName: this.ModuleName,
                    DataModule: this.Module.toDataJSON()
                });
                WebFormName = Packet && Packet.WebFormName ? Packet.WebFormName : "";
                DataModulePacket = Packet ? Packet.DataModule : null;
                if (tp.IsBlankString(WebFormName))
                    WebFormName = DataModulePacket && DataModulePacket.Name ? DataModulePacket.Name : "";
                if (!DataModulePacket || tp.IsBlankString(WebFormName))
                    throw new Error(TargetTitle + " data module was not returned.");
                await app.AppFormDialog.ShowModalDataFormAsync(WebFormName, {
                    FormId: WebFormName + "." + tp.Guid(),
                    Title: TargetTitle,
                    InitialDataModule: DataModulePacket,
                    InitialFormState: tp.WebDataFormState.Insert
                });
                this.UiLog("Created " + TargetTitle + " from " + this.GetItemLogText(this.Module.Id));
            });
        } catch (e) {
            this.ReportError("Create " + TargetTitle + " failed: " + tp.ExceptionText(e));
        }
    }
};

// ● payment cancellation form
/**
 * Web data form for payment cancellation documents.
 */
app.PaymentCancellationForm = class extends app.PaymentDataForm {
    // ● constructor
    /**
     * Creates the payment cancellation form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }
};
