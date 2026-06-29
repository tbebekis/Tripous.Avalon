// ● check box
/**
 * A check box control with a clickable wrapping label and word-wrapping text.
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 * - ValueChanged
 */
tp.CheckBox = class extends tp.Control {
    // ● private
    /**
     * Creates check-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateCheckBoxParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "label";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "label";
        return Args;
    }

    // ● constructor
    /**
     * Creates a check box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.CheckBox.CreateCheckBoxParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Checked";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
    }
    /**
     * Applies explicit create params to this check box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        var BaseParams;
        if (Params) {
            BaseParams = new tp.CreateParams(Params);
            delete BaseParams.Text;
            delete BaseParams.Checked;
        }
        super.ApplyCreateParams(BaseParams || Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.elText))
            this.elText = Params.elText;
        if (!tp.IsNil(Params.Text))
            this.Text = Params.Text;
        if (!tp.IsNil(Params.Checked))
            this.Checked = Params.Checked === true;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.CheckBox);
        this.EnsureContent();
        this.fChangeHandler = this.FuncBind(this.HandleChange);
        if (this.fCheckBox instanceof HTMLInputElement)
            this.fCheckBox.addEventListener("change", this.fChangeHandler);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fCheckBox instanceof HTMLInputElement && this.fChangeHandler)
            this.fCheckBox.removeEventListener("change", this.fChangeHandler);
        this.fChangeHandler = null;
        super.DoDispose();
    }
    /**
     * Ensures the check box has the expected internal markup.
     * @returns {void}
     */
    EnsureContent() {
        var ExistingHtml = this.Handle.innerHTML || "";
        var CtrlElement;
        if (!(this.Handle instanceof HTMLLabelElement))
            return;
        this.fCheckBox = tp.Select(this.Handle, "input[type=checkbox]");
        this.elText = tp.Select(this.Handle, "." + tp.Classes.Text);
        if (!(this.fCheckBox instanceof HTMLInputElement)) {
            this.Handle.innerHTML =
                "<span class=\"" + tp.Classes.Ctrl + "\"><input type=\"checkbox\" /></span>" +
                "<span class=\"" + tp.Classes.Text + "\">" + ExistingHtml + "</span>";
            this.fCheckBox = tp.Select(this.Handle, "input[type=checkbox]");
            this.elText = tp.Select(this.Handle, "." + tp.Classes.Text);
        } else {
            CtrlElement = tp.Closest(this.fCheckBox, "." + tp.Classes.Ctrl);
            if (!(CtrlElement instanceof HTMLElement)) {
                CtrlElement = this.Document.createElement("span");
                CtrlElement.className = tp.Classes.Ctrl;
                this.Handle.insertBefore(CtrlElement, this.fCheckBox);
                CtrlElement.appendChild(this.fCheckBox);
            }
            if (!(this.elText instanceof HTMLElement)) {
                this.elText = this.Document.createElement("span");
                this.elText.className = tp.Classes.Text;
                this.Handle.appendChild(this.elText);
            }
        }
    }
    /**
     * Binds the control to its data source.
     * @protected
     * @returns {void}
     */
    Bind() {
        super.Bind();
        this.ReadDataValue();
    }
    /**
     * Converts a data-source value to a checked property value.
     * @param {*} Value The data-source value.
     * @returns {boolean} Returns true when checked.
     */
    DataValueToDataProperty(Value) {
        return Value === true || Value === 1 || Value === "1" || Value === "true";
    }
    /**
     * Converts checked state to a data-source value.
     * @param {*} Value The checked value.
     * @returns {boolean} Returns true when checked.
     */
    DataPropertyToDataValue(Value) {
        return Value === true;
    }
    /**
     * Called after Required changes.
     * @protected
     * @returns {void}
     */
    OnRequiredChanged() {
        this.SetRequiredMark(this.fCheckBox);
        super.OnRequiredChanged();
    }
    /**
     * Called after ReadOnly changes.
     * @protected
     * @returns {void}
     */
    OnReadOnlyChanged() {
        if (this.fCheckBox instanceof HTMLInputElement)
            this.fCheckBox.disabled = this.ReadOnly === true;
        super.OnReadOnlyChanged();
    }
    /**
     * Handles input change events.
     * @protected
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    HandleChange(e) {
        this.WriteDataValue();
        this.OnValueChanged();
    }
    /**
     * Triggers the ValueChanged event.
     * @protected
     * @returns {void}
     */
    OnValueChanged() {
        if (this.ReadingDataValue !== true)
            this.Trigger("ValueChanged", {});
    }

    // ● public
    /**
     * Sets focus to the inner input element.
     * @returns {void}
     */
    Focus() {
        if (this.fCheckBox instanceof HTMLInputElement)
            this.fCheckBox.focus();
    }

    // ● properties
    /**
     * Gets or sets the check box text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.elText instanceof HTMLElement ? this.elText.innerHTML : "";
    }
    /**
     * Gets or sets the check box text.
     * @param {*} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        if (this.elText instanceof HTMLElement)
            this.elText.innerHTML = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the inner input name.
     * @returns {string} Returns the input name.
     */
    get Name() {
        return this.fCheckBox instanceof HTMLInputElement ? this.fCheckBox.name || "" : "";
    }
    /**
     * Gets or sets the inner input name.
     * @param {string} Value The input name.
     * @returns {void}
     */
    set Name(Value) {
        if (this.fCheckBox instanceof HTMLInputElement)
            this.fCheckBox.name = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets whether the check box is checked.
     * @returns {boolean} Returns true when checked.
     */
    get Checked() {
        return this.fCheckBox instanceof HTMLInputElement ? this.fCheckBox.checked === true : false;
    }
    /**
     * Gets or sets whether the check box is checked.
     * @param {boolean} Value True when checked.
     * @returns {void}
     */
    set Checked(Value) {
        if (this.fCheckBox instanceof HTMLInputElement)
            this.fCheckBox.checked = Value === true;
    }
};

// ● prototype
/**
 * Inner check box input.
 * @type {HTMLInputElement|null}
 */
tp.CheckBox.prototype.fCheckBox = null;
/**
 * Text element.
 * @type {HTMLElement|null}
 */
tp.CheckBox.prototype.elText = null;
/**
 * Change event handler.
 * @type {Function|null}
 */
tp.CheckBox.prototype.fChangeHandler = null;

tp.Ui.RegisterType(["CheckBox", "tp-CheckBox"], tp.CheckBox);
