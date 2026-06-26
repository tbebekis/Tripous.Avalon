// ● text box
/**
 * A single-line text input control.
 *
 * Events:
 * - ValueChanged
 */
tp.TextBox = class extends tp.InputControl {
    // ● constructor
    /**
     * Creates a text box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDataValueProperty = "Text";
        this.fAutocompleteList = null;
        this.SpellCheck = false;
        this.Autocomplete = false;
    }
    /**
     * Applies explicit create params to this text box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.MaxLength))
            this.MaxLength = tp.ToInt(Params.MaxLength);
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = String(Params.Placeholder);
        if (!tp.IsNil(Params.SpellCheck))
            this.SpellCheck = Params.SpellCheck === true;
        if (!tp.IsNil(Params.Autocomplete))
            this.Autocomplete = Params.Autocomplete === true;
        if (!tp.IsNil(Params.DOMAutocomplete))
            this.DOMAutocomplete = Params.DOMAutocomplete === true;
        if (!tp.IsNil(Params.RegexPattern))
            this.RegexPattern = String(Params.RegexPattern);
        if (!tp.IsNil(Params.AutocompleteList)) {
            this.AutocompleteList.DataList = Params.AutocompleteList;
            this.AutocompleteList.Active = true;
        }
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.type = "text";
        tp.AddClass(this.Handle, tp.Classes.TextBox);
        super.OnHandleCreated();
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fAutocompleteList) {
            this.fAutocompleteList.Dispose();
            this.fAutocompleteList = null;
        }
        super.DoDispose();
    }
    /**
     * Called after data-binding is completed.
     * @protected
     * @returns {void}
     */
    OnBindCompleted() {
        var Alignment;
        if (this.DataColumn instanceof tp.DataColumn) {
            Alignment = tp.DataType.DefaultAlignment(this.DataColumn.DataType);
            this.TextAlign = tp.Alignment.ToText(Alignment);
        }
        super.OnBindCompleted();
    }
    /**
     * Converts a data-source value to a text-box text value.
     * @param {*} Value The data-source value.
     * @returns {string|null} Returns the text value.
     */
    DataValueToDataProperty(Value) {
        return tp.IsString(Value) && tp.Db.NULL !== Value ? Value : null;
    }
    /**
     * Converts a text-box text value to a data-source value.
     * @param {*} Value The text-box text value.
     * @returns {*} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return Value;
    }

    // ● properties
    /**
     * Gets or sets the maximum text length.
     * @returns {number} Returns the maximum text length.
     */
    get MaxLength() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.maxLength : 0;
    }
    /**
     * Gets or sets the maximum text length.
     * @param {number} Value The maximum text length.
     * @returns {void}
     */
    set MaxLength(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.maxLength = tp.ToInt(Value);
    }
    /**
     * Gets or sets the placeholder text.
     * @returns {string} Returns the placeholder text.
     */
    get Placeholder() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.placeholder || "" : "";
    }
    /**
     * Gets or sets the placeholder text.
     * @param {string} Value The placeholder text.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets whether spelling checks are enabled.
     * @returns {boolean} Returns true when spelling checks are enabled.
     */
    get SpellCheck() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.spellcheck === true : false;
    }
    /**
     * Gets or sets whether spelling checks are enabled.
     * @param {boolean} Value True to enable spelling checks.
     * @returns {void}
     */
    set SpellCheck(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.spellcheck = Value === true;
    }
    /**
     * Gets or sets whether browser autocomplete is enabled.
     * @returns {boolean} Returns true when autocomplete is enabled.
     */
    get Autocomplete() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.autocomplete === "on" : false;
    }
    /**
     * Gets or sets whether browser autocomplete is enabled.
     * @param {boolean} Value True to enable autocomplete.
     * @returns {void}
     */
    set Autocomplete(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.autocomplete = Value === true ? "on" : "off";
    }
    /**
     * Gets or sets the validation regular expression pattern.
     * @returns {string} Returns the pattern.
     */
    get RegexPattern() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.pattern || "" : "";
    }
    /**
     * Gets or sets the validation regular expression pattern.
     * @param {string} Value The pattern.
     * @returns {void}
     */
    set RegexPattern(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.pattern = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the start position of the selected text.
     * @returns {number} Returns the selection start.
     */
    get SelectionStart() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.selectionStart || 0 : 0;
    }
    /**
     * Gets or sets the start position of the selected text.
     * @param {number} Value The selection start.
     * @returns {void}
     */
    set SelectionStart(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.selectionStart = tp.ToInt(Value);
    }
    /**
     * Gets or sets the end position of the selected text.
     * @returns {number} Returns the selection end.
     */
    get SelectionEnd() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.selectionEnd || 0 : 0;
    }
    /**
     * Gets or sets the end position of the selected text.
     * @param {number} Value The selection end.
     * @returns {void}
     */
    set SelectionEnd(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.selectionEnd = tp.ToInt(Value);
    }
    /**
     * Returns true when the selection direction is forward.
     * @returns {boolean} Returns true when the selection direction is forward.
     */
    get IsForwardSelection() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.selectionDirection === "forward" : false;
    }
    /**
     * Gets or sets whether browser autocomplete is enabled.
     * @returns {boolean} Returns true when autocomplete is enabled.
     */
    get DOMAutocomplete() {
        return this.Autocomplete;
    }
    /**
     * Gets or sets whether browser autocomplete is enabled.
     * @param {boolean} Value True to enable autocomplete.
     * @returns {void}
     */
    set DOMAutocomplete(Value) {
        this.Autocomplete = Value;
    }
    /**
     * Gets the Tripous autocomplete list associated with this text box.
     * @returns {tp.AutocompleteList|null} Returns the autocomplete list.
     */
    get AutocompleteList() {
        if (this.Handle instanceof HTMLInputElement) {
            if (tp.IsEmpty(this.fAutocompleteList))
                this.fAutocompleteList = new tp.AutocompleteList(this.Handle);
        }
        return this.fAutocompleteList;
    }
    /**
     * Gets or sets text alignment.
     * @returns {string} Returns the text-align value.
     */
    get TextAlign() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.style.textAlign || "" : "";
    }
    /**
     * Gets or sets text alignment.
     * @param {string} Value The text-align value.
     * @returns {void}
     */
    set TextAlign(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.style.textAlign = tp.IsNil(Value) ? "" : String(Value);
    }

    // ● public
    /**
     * Focuses the input and selects all text.
     * @returns {void}
     */
    Select() {
        if (this.Handle instanceof HTMLInputElement && tp.IsFunction(this.Handle.select)) {
            this.Handle.focus();
            this.Handle.select();
        }
    }
    /**
     * Sets the selected text range.
     * @param {number} Start The start position.
     * @param {number} End The end position.
     * @returns {void}
     */
    SetSelectionRange(Start, End) {
        if (this.Handle instanceof HTMLInputElement && tp.IsFunction(this.Handle.setSelectionRange)) {
            Start = tp.ToInt(Start);
            End = tp.ToInt(End);
            this.Handle.focus();
            this.Handle.setSelectionRange(Start, End);
        }
    }
};

tp.Ui.RegisterType(["TextBox", "tp-TextBox"], tp.TextBox);
