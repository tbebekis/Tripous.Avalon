// ● input control
/**
 * Base class for input element controls with simple data binding.
 *
 * Events:
 * - ValueChanged
 */
tp.InputControl = class extends tp.Control {
    // ● private
    /**
     * Creates input-control create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateInputParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "input";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "input";
        return Args;
    }

    // ● constructor
    /**
     * Creates an input control.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.InputControl.CreateInputParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fElementType = "input";
        this.fDataBindMode = tp.ControlBindMode.Simple;
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        this.fInputChangedHandler = this.FuncBind(this.HandleInputChanged);
        if (this.Handle) {
            this.Handle.addEventListener("change", this.fInputChangedHandler);
            this.Handle.addEventListener("input", this.fInputChangedHandler);
        }
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fInputChangedHandler) {
            this.Handle.removeEventListener("change", this.fInputChangedHandler);
            this.Handle.removeEventListener("input", this.fInputChangedHandler);
        }
        this.fInputChangedHandler = null;
        super.DoDispose();
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
     * Called after ReadOnly changes.
     * @protected
     * @returns {void}
     */
    OnReadOnlyChanged() {
        if (this.Handle && "readOnly" in this.Handle)
            this.Handle.readOnly = this.ReadOnly;
        super.OnReadOnlyChanged();
    }
    /**
     * Called after Required changes.
     * @protected
     * @returns {void}
     */
    OnRequiredChanged() {
        this.SetRequiredMark(this.Handle);
        super.OnRequiredChanged();
    }
    /**
     * Handles input and change DOM events.
     * @protected
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    HandleInputChanged(e) {
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

    // ● properties
    /**
     * Gets or sets whether the input should receive focus on page load.
     * @returns {boolean} Returns true when autofocus is enabled.
     */
    get AutoFocus() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.autofocus === true : false;
    }
    /**
     * Gets or sets whether the input should receive focus on page load.
     * @param {boolean} Value True to enable autofocus.
     * @returns {void}
     */
    set AutoFocus(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.autofocus = Value === true;
    }
};
