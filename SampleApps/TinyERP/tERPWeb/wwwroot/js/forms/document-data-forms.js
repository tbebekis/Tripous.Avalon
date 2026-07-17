/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

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
        this.ApplyDisplayOrderDefault(Table, Row);
    }
    /**
     * Returns the field names to display in a document detail grid.
     * @param {tp.DataTable} Table The detail table.
     * @returns {string[]|null} Returns field names or null to use the default columns.
     */
    GetDetailGridFieldNames(Table) {
        return null;
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
                this.Form.ScheduleDocumentCalculate(Source.Table ? Source.Table.Name : "", Args && Args.Column ? Args.Column.Name : "");
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
        if (!(Source instanceof tp.DataSource) || !this.IsTradeLineTable(Table))
            return;
        if (Grid.fDocumentCalculateSource === Source)
            return;
        if (Grid.fDocumentCalculateSource instanceof tp.DataSource && Grid.fDocumentCalculateListener)
            Grid.fDocumentCalculateSource.Off("RowModified", Grid.fDocumentCalculateListener);
        Grid.fDocumentCalculateSource = Source;
        Grid.fDocumentCalculateListener = Source.On("RowModified", function (Args) {
            if (this.Form && this.IsServerCalculatedLineColumn(Args ? Args.Column : null))
                this.Form.ScheduleDocumentCalculate(Table ? Table.Name : "", Args && Args.Column ? Args.Column.Name : "");
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
     * Updates toolbar state.
     * @returns {void}
     */
    UpdateToolBar() {
        super.UpdateToolBar();
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
                this.UiLog("Posted " + this.GetItemLogText(Id));
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
     * @returns {void}
     */
    ScheduleDocumentCalculate(TableName, FieldName) {
        if (this.fDocumentApplyingServerPacket === true || this.IsReadOnly === true || !(this.Module instanceof tp.DataModule))
            return;
        if (!tp.IsBlank(TableName) && !tp.IsBlank(FieldName) && (tp.IsBlank(this.fDocumentCalculateTableName) || tp.IsBlank(this.fDocumentCalculateFieldName))) {
            this.fDocumentCalculateTableName = String(TableName);
            this.fDocumentCalculateFieldName = String(FieldName);
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
        if (this.fDocumentApplyingServerPacket === true || this.IsReadOnly === true || !(this.Module instanceof tp.DataModule))
            return;
        if (this.fDocumentCalculating === true) {
            this.fDocumentCalculateAgain = true;
            return;
        }
        this.fDocumentCalculating = true;
        TableName = this.fDocumentCalculateTableName || "";
        FieldName = this.fDocumentCalculateFieldName || "";
        this.fDocumentCalculateTableName = "";
        this.fDocumentCalculateFieldName = "";
        try {
            Packet = await tp.AjaxRequest.Execute("App.DocumentDataModule.Calculate", {
                ModuleName: this.ModuleName,
                TableName: TableName,
                FieldName: FieldName,
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
     * Updates toolbar state.
     * @returns {void}
     */
    UpdateToolBar() {
        super.UpdateToolBar();
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
        var FormId;
        var PageHandler;
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
                FormId = WebFormName + "." + (DataModulePacket && DataModulePacket.DataSet ? this.Module.Id : tp.Guid());
                PageHandler = app.App && app.App.MainPage ? app.App.MainPage.PageHandler : null;
                if (!PageHandler || !tp.IsFunction(PageHandler.OpenAsync))
                    throw new Error("The WebForm page handler is not available.");
                await PageHandler.OpenAsync(WebFormName, {
                    FormId: FormId,
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
