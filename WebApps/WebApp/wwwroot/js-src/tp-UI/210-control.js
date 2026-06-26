// ● control
/**
 * The ultimate ancestor class of data-bindable controls.
 * Controls are components that may bind to a tp.DataSource through DataField and DataValueProperty.
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 */
tp.Control = class extends tp.Component {
    // ● constructor
    /**
     * Creates a control.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● static public
    /**
     * Adds a required mark beside a control.
     * @param {tp.Control} Control The control.
     * @param {HTMLElement|null|undefined} Mark The existing mark element.
     * @returns {HTMLElement|null} Returns the mark element.
     */
    static AddRequiredMark(Control, Mark) {
        if (!(Mark instanceof HTMLElement) && Control instanceof tp.Control && Control.ParentHandle instanceof HTMLElement) {
            Mark = Control.Document.createElement("span");
            Control.ParentHandle.appendChild(Mark);
            Mark.className = tp.Classes.RequiredMark;
            Mark.innerHTML = "*";
            tp.Display(Mark, "none");
        }
        return Mark instanceof HTMLElement ? Mark : null;
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDataBindMode = tp.ControlBindMode.None;
        this.fDataValueProperty = "";
        this.fDataSource = null;
        this.fDataField = "";
        this.fRequired = false;
        this.fReadOnly = false;
        this.TableName = "";
        this.elRequiredMark = null;
        this.ReadingDataValue = false;
        this.WritingDataValue = false;
    }
    /**
     * Applies explicit create params to this control.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.TableName))
            this.TableName = String(Params.TableName);
        if (!tp.IsNil(Params.elRequiredMark))
            this.elRequiredMark = Params.elRequiredMark;
        if (!tp.IsNil(Params.DataField))
            this.DataField = String(Params.DataField);
        if (!tp.IsNil(Params.Required))
            this.Required = Params.Required === true;
        if (!tp.IsNil(Params.ReadOnly))
            this.ReadOnly = Params.ReadOnly === true;
        if (!tp.IsNil(Params.DataSource))
            this.DataSource = Params.DataSource;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        // Old Tripous made controls part of the tab order by default.
        // -1 keeps an element focusable by code, 0 follows document tab order, > 0 follows explicit tab order.
        this.TabIndex = 0;
    }
    /**
     * Binds the control to its data source.
     * @protected
     * @returns {void}
     */
    Bind() {
    }
    /**
     * Displays or hides the required mark when Required changes.
     * This is intentionally minimal until tp.CtrlRow and tp.Ui exist.
     * @protected
     * @param {Element|null|undefined} Element The element whose value is required.
     * @returns {void}
     */
    SetRequiredMark(Element) {
        if (Element && "required" in Element)
            Element.required = this.Required === true;
        if (this.elRequiredMark instanceof HTMLElement)
            this.elRequiredMark.style.display = this.Required === true ? "" : "none";
    }

    // ● properties
    /**
     * Gets the property name used as the control data value.
     * @returns {string} Returns the data value property name.
     */
    get DataValueProperty() {
        return !tp.IsBlank(this.fDataValueProperty) ? this.fDataValueProperty : "";
    }
    /**
     * Gets the data-binding mode this control supports.
     * @returns {number} Returns a tp.ControlBindMode value.
     */
    get DataBindMode() {
        return this.fDataBindMode;
    }
    /**
     * Returns true when this control supports simple data binding.
     * @returns {boolean} Returns true for simple data binding.
     */
    get IsDataBindSimple() {
        return this.DataBindMode === tp.ControlBindMode.Simple;
    }
    /**
     * Returns true when this control supports list data binding.
     * @returns {boolean} Returns true for list data binding.
     */
    get IsDataBindList() {
        return this.DataBindMode === tp.ControlBindMode.List;
    }
    /**
     * Returns true when this control supports grid data binding.
     * @returns {boolean} Returns true for grid data binding.
     */
    get IsDataBindGrid() {
        return this.DataBindMode === tp.ControlBindMode.Grid;
    }
    /**
     * Gets or sets the control text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return super.Text;
    }
    /**
     * Gets or sets the control text.
     * @param {*} Value The text value.
     * @returns {void}
     */
    set Text(Value) {
        if (this.Handle)
            tp.val(this.Handle, "");
        super.Text = Value;
    }
    /**
     * Gets or sets the data source this control is bound to.
     * @returns {tp.DataSource|null} Returns the data source.
     */
    get DataSource() {
        return this.fDataSource;
    }
    /**
     * Gets or sets the data source this control is bound to.
     * @param {tp.DataSource|tp.DataTable|null|undefined} Value The data source or table.
     * @returns {void}
     */
    set DataSource(Value) {
        var WasDataBound;
        var IsRemoved;
        if (Value !== this.fDataSource) {
            WasDataBound = this.IsDataBound;
            this.OnDataSourceChanging(Value);
            if (Value instanceof tp.DataTable)
                Value = new tp.DataSource(Value);
            this.fDataSource = Value instanceof tp.DataSource ? Value : null;
            IsRemoved = !this.fDataSource && WasDataBound === true;
            if (IsRemoved)
                this.OnClearDataDisplay();
            this.OnDataSourceChanged();
            if (this.fDataSource) {
                this.Bind();
                if (this.IsDataBound === true)
                    this.OnBindCompleted();
            }
        }
    }
    /**
     * Gets or sets the bound data field name.
     * @returns {string} Returns the field name.
     */
    get DataField() {
        return this.fDataField;
    }
    /**
     * Gets or sets the bound data field name.
     * @param {string} Value The field name.
     * @returns {void}
     */
    set DataField(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (Value !== this.DataField) {
            this.fDataField = Value;
            this.OnDataFieldChanged();
            this.Bind();
        }
    }
    /**
     * Returns true if this control is bound to a data source.
     * @returns {boolean} Returns true when data-bound.
     */
    get IsDataBound() {
        if (!this.DataSource)
            return false;
        if (this.IsDataBindSimple || this.IsDataBindList)
            return !tp.IsBlank(this.DataField) && !tp.IsBlank(this.DataValueProperty) && this.DataColumn instanceof tp.DataColumn;
        if (this.IsDataBindGrid)
            return true;
        return false;
    }
    /**
     * Gets the data column this control is bound to.
     * @returns {tp.DataColumn|null} Returns the data column or null.
     */
    get DataColumn() {
        if (this.DataSource && !tp.IsBlank(this.DataField))
            return this.DataSource.Table.FindColumn(this.DataField);
        return null;
    }
    /**
     * Gets or sets whether the control requires a value.
     * @returns {boolean} Returns true when required.
     */
    get Required() {
        return this.fRequired === true;
    }
    /**
     * Gets or sets whether the control requires a value.
     * @param {boolean} Value True when required.
     * @returns {void}
     */
    set Required(Value) {
        Value = Value === true;
        if (this.Required !== Value) {
            this.fRequired = Value;
            this.OnRequiredChanged();
        }
    }
    /**
     * Gets or sets whether the control is read-only.
     * @returns {boolean} Returns true when read-only.
     */
    get ReadOnly() {
        return this.fReadOnly === true;
    }
    /**
     * Gets or sets whether the control is read-only.
     * @param {boolean} Value True when read-only.
     * @returns {void}
     */
    set ReadOnly(Value) {
        Value = Value === true;
        if (this.ReadOnly !== Value) {
            this.fReadOnly = Value;
            this.OnReadOnlyChanged();
        }
    }
    /**
     * Gets or sets a single character shortcut key.
     * @returns {string} Returns the access key.
     */
    get AccessKey() {
        return this.Handle ? this.Handle.accessKey : "";
    }
    /**
     * Gets or sets a single character shortcut key.
     * @param {string} Value The access key.
     * @returns {void}
     */
    set AccessKey(Value) {
        if (this.Handle)
            this.Handle.accessKey = Value;
    }

    // ● event triggers
    /**
     * Called before the data source is assigned.
     * @protected
     * @param {tp.DataSource|tp.DataTable|null|undefined} NewDataSource The new data source.
     * @returns {void}
     */
    OnDataSourceChanging(NewDataSource) {
        this.Trigger("DataSourceChanging", { NewDataSource: NewDataSource });
        if (this.DataSource)
            this.DataSource.RemoveDataListener(this);
    }
    /**
     * Called after the data source is assigned.
     * @protected
     * @returns {void}
     */
    OnDataSourceChanged() {
        if (this.DataSource) {
            this.DataSource.AddDataListener(this);
            this.Trigger("DataSourceChanged");
        }
    }
    /**
     * Called after DataField changes.
     * @protected
     * @returns {void}
     */
    OnDataFieldChanged() {
        this.Trigger("DataFieldChanged");
    }
    /**
     * Called when the control should clear its data display.
     * @protected
     * @returns {void}
     */
    OnClearDataDisplay() {
        this.Trigger("ClearDataDisplay");
    }
    /**
     * Called after binding completes.
     * @protected
     * @returns {void}
     */
    OnBindCompleted() {
        this.Trigger("BindCompleted");
    }
    /**
     * Called after Required changes.
     * @protected
     * @returns {void}
     */
    OnRequiredChanged() {
        this.Trigger("RequiredChanged");
    }
    /**
     * Called after ReadOnly changes.
     * @protected
     * @returns {void}
     */
    OnReadOnlyChanged() {
        this.Trigger("ReadOnlyChanged", {});
    }

    // ● data source listener
    /**
     * Notification called when a row is created.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowCreated(Table, Row) {
    }
    /**
     * Notification called when a row is added.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowAdded(Table, Row) {
        if (this.IsDataBindSimple || this.IsDataBindList)
            this.ReadDataValue();
    }
    /**
     * Notification called when a row is modified.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @param {tp.DataColumn} Column The data column.
     * @param {*} OldValue The old value.
     * @param {*} NewValue The new value.
     * @returns {void}
     */
    DataSourceRowModified(Table, Row, Column, OldValue, NewValue) {
        if (this.IsDataBindSimple || this.IsDataBindList)
            this.ReadDataValue();
    }
    /**
     * Notification called when a row is removed.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowRemoved(Table, Row) {
    }
    /**
     * Notification called when position changes.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The current row.
     * @param {number} Position The new position.
     * @returns {void}
     */
    DataSourcePositionChanged(Table, Row, Position) {
        if (this.IsDataBindSimple || this.IsDataBindList)
            this.ReadDataValue();
    }
    /**
     * Notification called after sorting.
     * @returns {void}
     */
    DataSourceSorted() {
    }
    /**
     * Notification called after filtering.
     * @returns {void}
     */
    DataSourceFiltered() {
    }
    /**
     * Notification called after datasource update.
     * @returns {void}
     */
    DataSourceUpdated() {
        if (this.IsDataBindSimple || this.IsDataBindList)
            this.ReadDataValue();
    }

    // ● public
    /**
     * Formats a value according to this control data column.
     * @param {*} Value The value to format.
     * @returns {string|*} Returns the formatted value.
     */
    Format(Value) {
        return this.DataColumn instanceof tp.DataColumn ? this.DataColumn.Format(Value, this.IsDataBindList || this.IsDataBindGrid) : "";
    }
    /**
     * Parses text according to this control data column.
     * @param {*} Value The value to parse.
     * @returns {*} Returns the parsed value.
     */
    Parse(Value) {
        if (this.DataColumn instanceof tp.DataColumn && !tp.IsEmpty(Value)) {
            Value = !tp.IsString(Value) ? Value.toString() : Value;
            return this.DataColumn.Parse(Value);
        }
        return null;
    }
    /**
     * Converts a data-source value to a control property value.
     * @param {*} Value The data-source value.
     * @returns {*} Returns the control property value.
     */
    DataValueToDataProperty(Value) {
        return Value;
    }
    /**
     * Converts a control property value to a data-source value.
     * @param {*} Value The control property value.
     * @returns {*} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return Value;
    }
    /**
     * Reads the value from the data source and assigns it to DataValueProperty.
     * @returns {void}
     */
    ReadDataValue() {
        var Value;
        if (this.ReadingDataValue === true || this.WritingDataValue === true)
            return;
        if (this.IsDataBound && this.DataSource.Position >= 0) {
            this.ReadingDataValue = true;
            try {
                Value = this.DataSource.Get(this.DataField);
                Value = this.DataValueToDataProperty(Value);
                this[this.DataValueProperty] = Value;
            } finally {
                this.ReadingDataValue = false;
            }
        }
    }
    /**
     * Writes DataValueProperty to the data source.
     * @returns {void}
     */
    WriteDataValue() {
        var Value;
        if (this.ReadingDataValue === true || this.WritingDataValue === true)
            return;
        if (this.IsDataBound && this.DataSource.Position >= 0) {
            this.WritingDataValue = true;
            try {
                Value = this[this.DataValueProperty];
                Value = this.DataPropertyToDataValue(Value);
                this.DataSource.Set(this.DataField, Value);
            } finally {
                this.WritingDataValue = false;
            }
        }
    }
    /**
     * Returns true if this control is valid.
     * @returns {boolean} Returns true when valid.
     */
    CheckValidity() {
        return tp.IsValidatableElement(this.Handle) ? this.Handle.checkValidity() : true;
    }
    /**
     * Sets a custom validation message.
     * @param {string} MessageText The validation message.
     * @returns {void}
     */
    SetValidationMessage(MessageText) {
        if (tp.IsValidatableElement(this.Handle))
            this.Handle.setCustomValidity(MessageText);
    }
};
