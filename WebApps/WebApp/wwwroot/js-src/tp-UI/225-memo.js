// ● memo
/**
 * A multi-line text area control.
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
tp.Memo = class extends tp.Control {
    // ● private
    /**
     * Creates memo create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateMemoParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "textarea";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "textarea";
        return Args;
    }

    // ● constructor
    /**
     * Creates a memo.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.Memo.CreateMemoParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fElementType = "textarea";
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Text";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.Cols = 20;
        this.Rows = 2;
        this.SpellCheck = false;
        this.Autocomplete = false;
    }
    /**
     * Applies explicit create params to this memo.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Width = Params.Width;
        if (!tp.IsNil(Params.Height))
            this.Height = Params.Height;
        if (!tp.IsNil(Params.Cols))
            this.Cols = tp.ToInt(Params.Cols);
        if (!tp.IsNil(Params.Rows))
            this.Rows = tp.ToInt(Params.Rows);
        if (!tp.IsNil(Params.MaxLength))
            this.MaxLength = tp.ToInt(Params.MaxLength);
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = String(Params.Placeholder);
        if (!tp.IsNil(Params.SpellCheck))
            this.SpellCheck = Params.SpellCheck === true;
        if (!tp.IsNil(Params.Autocomplete))
            this.Autocomplete = Params.Autocomplete === true;
        if (!tp.IsNil(Params.WordWrap))
            this.WordWrap = Params.WordWrap === true;
        if (!tp.IsNil(Params.Resizable))
            this.Resizable = Params.Resizable === true;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.Memo);
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
        if (this.Handle instanceof HTMLTextAreaElement)
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
     * Converts a data-source value to a memo text value.
     * @param {*} Value The data-source value.
     * @returns {string|null} Returns the text value.
     */
    DataValueToDataProperty(Value) {
        return tp.IsString(Value) && tp.Db.NULL !== Value ? Value : null;
    }
    /**
     * Converts a memo text value to a data-source value.
     * @param {*} Value The memo text value.
     * @returns {*} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return Value;
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
     * Focuses the memo and selects all text.
     * @returns {void}
     */
    Select() {
        if (this.Handle instanceof HTMLTextAreaElement && tp.IsFunction(this.Handle.select)) {
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
        if (this.Handle instanceof HTMLTextAreaElement && tp.IsFunction(this.Handle.setSelectionRange)) {
            Start = tp.ToInt(Start);
            End = tp.ToInt(End);
            this.Handle.focus();
            this.Handle.setSelectionRange(Start, End);
        }
    }
    /**
     * Appends text to the memo.
     * @param {string} Text The text to append.
     * @returns {void}
     */
    Append(Text) {
        this.Text = this.Text + (tp.IsNil(Text) ? "" : String(Text));
    }
    /**
     * Appends text as a new line.
     * @param {string} Text The text to append.
     * @returns {void}
     */
    AppendLine(Text) {
        var S = this.Text;
        Text = tp.IsNil(Text) ? "" : String(Text);
        this.Text = tp.IsBlank(S) ? Text : S + "\n" + Text;
    }
    /**
     * Appends each line of a string array.
     * @param {string[]} StringList The string array.
     * @returns {void}
     */
    AppendLines(StringList) {
        if (tp.IsArray(StringList))
            StringList.forEach(function (Line) { this.AppendLine(Line); }, this);
    }
    /**
     * Returns memo text lines.
     * @param {boolean} RemoveEmptyLines True to remove empty lines.
     * @returns {string[]} Returns the memo text lines.
     */
    GetLines(RemoveEmptyLines) {
        return tp.Split(this.Text, "\n", RemoveEmptyLines === true);
    }

    // ● properties
    /**
     * Gets or sets the visible width in average character widths.
     * @returns {number} Returns the column count.
     */
    get Cols() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.cols : 0;
    }
    /**
     * Gets or sets the visible width in average character widths.
     * @param {number} Value The column count.
     * @returns {void}
     */
    set Cols(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.cols = tp.ToInt(Value);
    }
    /**
     * Gets or sets the number of visible text lines.
     * @returns {number} Returns the row count.
     */
    get Rows() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.rows : 0;
    }
    /**
     * Gets or sets the number of visible text lines.
     * @param {number} Value The row count.
     * @returns {void}
     */
    set Rows(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.rows = tp.ToInt(Value);
    }
    /**
     * Gets or sets whether the memo receives focus on page load.
     * @returns {boolean} Returns true when autofocus is enabled.
     */
    get AutoFocus() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.autofocus === true : false;
    }
    /**
     * Gets or sets whether the memo receives focus on page load.
     * @param {boolean} Value True to enable autofocus.
     * @returns {void}
     */
    set AutoFocus(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.autofocus = Value === true;
    }
    /**
     * Gets or sets the maximum text length.
     * @returns {number} Returns the maximum text length.
     */
    get MaxLength() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.maxLength : 0;
    }
    /**
     * Gets or sets the maximum text length.
     * @param {number} Value The maximum text length.
     * @returns {void}
     */
    set MaxLength(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.maxLength = tp.ToInt(Value);
    }
    /**
     * Gets or sets the placeholder text.
     * @returns {string} Returns the placeholder text.
     */
    get Placeholder() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.placeholder || "" : "";
    }
    /**
     * Gets or sets the placeholder text.
     * @param {string} Value The placeholder text.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets whether spelling checks are enabled.
     * @returns {boolean} Returns true when spelling checks are enabled.
     */
    get SpellCheck() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.spellcheck === true : false;
    }
    /**
     * Gets or sets whether spelling checks are enabled.
     * @param {boolean} Value True to enable spelling checks.
     * @returns {void}
     */
    set SpellCheck(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.spellcheck = Value === true;
    }
    /**
     * Gets or sets whether browser autocomplete is enabled.
     * @returns {boolean} Returns true when autocomplete is enabled.
     */
    get Autocomplete() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.autocomplete === "on" : false;
    }
    /**
     * Gets or sets whether browser autocomplete is enabled.
     * @param {boolean} Value True to enable autocomplete.
     * @returns {void}
     */
    set Autocomplete(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.autocomplete = Value === true ? "on" : "off";
    }
    /**
     * Gets or sets the start position of the selected text.
     * @returns {number} Returns the selection start.
     */
    get SelectionStart() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.selectionStart || 0 : 0;
    }
    /**
     * Gets or sets the start position of the selected text.
     * @param {number} Value The selection start.
     * @returns {void}
     */
    set SelectionStart(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.selectionStart = tp.ToInt(Value);
    }
    /**
     * Gets or sets the end position of the selected text.
     * @returns {number} Returns the selection end.
     */
    get SelectionEnd() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.selectionEnd || 0 : 0;
    }
    /**
     * Gets or sets the end position of the selected text.
     * @param {number} Value The selection end.
     * @returns {void}
     */
    set SelectionEnd(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.selectionEnd = tp.ToInt(Value);
    }
    /**
     * Returns true when the selection direction is forward.
     * @returns {boolean} Returns true when the selection direction is forward.
     */
    get IsForwardSelection() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.selectionDirection === "forward" : false;
    }
    /**
     * Gets or sets whether the memo applies word-wrap.
     * @returns {boolean} Returns true when word-wrap is enabled.
     */
    get WordWrap() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.wrap === "hard" || this.Handle.wrap === "soft" : true;
    }
    /**
     * Gets or sets whether the memo applies word-wrap.
     * @param {boolean} Value True to enable word-wrap.
     * @returns {void}
     */
    set WordWrap(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.wrap = Value === true ? "hard" : "off";
    }
    /**
     * Gets or sets whether the memo displays a resize handle.
     * @returns {boolean} Returns true when resizing is enabled.
     */
    get Resizable() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.style.resize !== "" && this.Handle.style.resize !== "none" : false;
    }
    /**
     * Gets or sets whether the memo displays a resize handle.
     * @param {boolean} Value True to enable resizing.
     * @returns {void}
     */
    set Resizable(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.style.resize = Value === true ? "both" : "none";
    }
    /**
     * Gets or sets CSS width.
     * @returns {string} Returns the width.
     */
    get Width() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.style.width || "" : "";
    }
    /**
     * Gets or sets CSS width.
     * @param {number|string} Value The width.
     * @returns {void}
     */
    set Width(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.style.width = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
    /**
     * Gets or sets CSS height.
     * @returns {string} Returns the height.
     */
    get Height() {
        return this.Handle instanceof HTMLTextAreaElement ? this.Handle.style.height || "" : "";
    }
    /**
     * Gets or sets CSS height.
     * @param {number|string} Value The height.
     * @returns {void}
     */
    set Height(Value) {
        if (this.Handle instanceof HTMLTextAreaElement)
            this.Handle.style.height = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
};

tp.Ui.RegisterType(["Memo", "tp-Memo"], tp.Memo);
