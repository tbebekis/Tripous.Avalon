// ● number box
/**
 * A text input control for numeric values.
 *
 * Events:
 * - ValueChanged
 */
tp.NumberBox = class extends tp.InputControl {
    // ● constructor
    /**
     * Creates a number box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fElementSubType = "text";
        this.fDataValueProperty = "Value";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDecimals = 0;
        this.fLastCommittedText = null;
        this.fFocusLostHandler = this.FuncBind(this.HandleFocusLost);
    }
    /**
     * Applies explicit create params to this number box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Decimals))
            this.Decimals = tp.ToInt(Params.Decimals);
        if (!tp.IsNil(Params.Value))
            this.Value = Params.Value;
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = String(Params.Placeholder);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        if (this.Handle instanceof HTMLInputElement) {
            this.Handle.inputMode = "decimal";
            this.Handle.autocomplete = "off";
            this.Handle.style.textAlign = "right";
            this.Handle.addEventListener("blur", this.fFocusLostHandler);
        }
        tp.AddClass(this.Handle, tp.Classes.NumberBox);
        super.OnHandleCreated();
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fFocusLostHandler)
            this.Handle.removeEventListener("blur", this.fFocusLostHandler);
        this.fFocusLostHandler = null;
        super.DoDispose();
    }
    /**
     * Handles lost focus.
     * @protected
     * @param {FocusEvent} e The DOM event.
     * @returns {void}
     */
    HandleFocusLost(e) {
        this.CommitValue();
    }
    /**
     * Commits the current text to the normalized numeric value.
     * @protected
     * @returns {void}
     */
    CommitValue() {
        this.NormalizeText();
        this.WriteDataValue();
        if (this.Text !== this.fLastCommittedText) {
            this.fLastCommittedText = this.Text;
            this.OnValueChanged();
        }
    }
    /**
     * Handles input and change DOM events.
     * @protected
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    HandleInputChanged(e) {
        if (e && e.type === "change")
            this.CommitValue();
    }
    /**
     * Returns a strict normalized numeric text.
     * @protected
     * @param {*} Value The value to normalize.
     * @returns {string|null} Returns normalized text or null.
     */
    NormalizeNumberText(Value) {
        var Text;
        var DecimalSep;
        var ThousandSep;
        var DecimalPattern;
        if (tp.IsNumber(Value))
            return String(Value);
        if (tp.IsBlankString(Value))
            return null;
        Text = String(Value).trim();
        DecimalSep = tp.GetDecimalSeparator();
        ThousandSep = tp.GetThousandSeparator();
        if (!tp.IsBlank(ThousandSep))
            Text = Text.replace(new RegExp(tp.RegExEscape(ThousandSep), "g"), "");
        if (DecimalSep !== ".")
            Text = Text.replace(DecimalSep, ".");
        DecimalPattern = /^[+-]?(\d+(\.\d*)?|\.\d+)$/;
        if (this.Decimals > 0)
            return DecimalPattern.test(Text) ? Text : null;
        return /^[+-]?\d+$/.test(Text) ? Text : null;
    }
    /**
     * Converts a data-source value to a number-box value.
     * @param {*} Value The data-source value.
     * @returns {number|null} Returns the number value.
     */
    DataValueToDataProperty(Value) {
        if (tp.IsNumber(Value))
            return Value;
        return this.Parse(Value);
    }
    /**
     * Converts a number-box value to a data-source value.
     * @param {*} Value The number-box value.
     * @returns {number|null} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return tp.IsNumber(Value) ? Value : this.Parse(Value);
    }

    // ● properties
    /**
     * Gets or sets the numeric value.
     * @returns {number|null} Returns the numeric value.
     */
    get Value() {
        return this.Parse(this.Text);
    }
    /**
     * Gets or sets the numeric value.
     * @param {number|null|undefined} Value The numeric value.
     * @returns {void}
     */
    set Value(Value) {
        this.Text = tp.IsNumber(Value) ? this.Format(Value) : "";
    }
    /**
     * Gets or sets the decimal places.
     * @returns {number} Returns the decimal places.
     */
    get Decimals() {
        return this.DataColumn instanceof tp.DataColumn ? this.DataColumn.Decimals : this.fDecimals;
    }
    /**
     * Gets or sets the decimal places.
     * @param {number} Value The decimal places.
     * @returns {void}
     */
    set Decimals(Value) {
        Value = tp.ToInt(Value);
        if (Value >= 0 && Value !== this.fDecimals) {
            this.fDecimals = Value;
            this.NormalizeText();
        }
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

    // ● public
    /**
     * Normalizes the displayed text using the current decimal places.
     * Non-empty invalid text is normalized to zero.
     * @returns {boolean} Returns true when text is normalized.
     */
    NormalizeText() {
        var Text = this.Text;
        var Value = this.Parse(Text);
        var NewText;
        if (tp.IsBlankString(Text))
            return false;
        if (!tp.IsNumber(Value))
            Value = 0;
        NewText = this.Format(Value);
        if (Text !== NewText)
            this.Text = NewText;
        return true;
    }
    /**
     * Formats a numeric value.
     * @param {*} Value The value to format.
     * @returns {string} Returns the formatted text.
     */
    Format(Value) {
        if (tp.IsNumber(Value)) {
            if (this.DataColumn instanceof tp.DataColumn)
                return this.DataColumn.Format(Value, false);
            return tp.FormatNumber2(Value, this.Decimals);
        }
        return "";
    }
    /**
     * Parses a value to a number.
     * @param {*} Value The value to parse.
     * @returns {number|null} Returns the parsed number or null.
     */
    Parse(Value) {
        var Text;
        var NumberValue;
        if (tp.IsNumber(Value))
            return Value;
        Text = this.NormalizeNumberText(Value);
        if (Text !== null) {
            NumberValue = this.Decimals > 0 ? Number.parseFloat(Text) : Number.parseInt(Text, 10);
            return Number.isFinite(NumberValue) ? NumberValue : null;
        }
        return null;
    }
};

tp.Ui.RegisterType(["NumberBox", "tp-NumberBox"], tp.NumberBox);

// ● html number box
/**
 * A native HTML number input control.
 *
 * Events:
 * - ValueChanged
 */
tp.HtmlNumberBox = class extends tp.InputControl {
    // ● constructor
    /**
     * Creates a HTML number box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fElementSubType = "number";
        this.fDataValueProperty = "Value";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        if (this.Handle instanceof HTMLInputElement) {
            this.Handle.min = tp.IsBlank(this.Handle.min) ? "" : this.Handle.min;
            this.Handle.max = tp.IsBlank(this.Handle.max) ? "" : this.Handle.max;
            this.Handle.step = tp.IsBlank(this.Handle.step) ? "0.1" : this.Handle.step;
            this.Handle.value = tp.IsBlank(this.Handle.value) ? "0" : this.Handle.value;
        }
    }
    /**
     * Applies explicit create params to this number box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Min))
            this.Min = Params.Min;
        if (!tp.IsNil(Params.Max))
            this.Max = Params.Max;
        if (!tp.IsNil(Params.Step))
            this.Step = Params.Step;
        if (!tp.IsNil(Params.Value))
            this.Value = Params.Value;
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = String(Params.Placeholder);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        if (this.Handle instanceof HTMLInputElement) {
            this.Handle.style.textAlign = "right";
        }
        tp.AddClass(this.Handle, tp.Classes.HtmlNumberBox);
        super.OnHandleCreated();
    }
    /**
     * Handles input and change DOM events.
     * @protected
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    HandleInputChanged(e) {
        if (this.Handle instanceof HTMLInputElement && !tp.IsBlank(this.Handle.value) && !Number.isFinite(this.Handle.valueAsNumber))
            this.Value = 0;
        super.HandleInputChanged(e);
    }
    /**
     * Converts a data-source value to a number-box value.
     * @param {*} Value The data-source value.
     * @returns {number|null} Returns the number value.
     */
    DataValueToDataProperty(Value) {
        if (tp.IsNumber(Value))
            return Value;
        if (!tp.IsBlankString(Value)) {
            Value = Number(Value);
            return Number.isFinite(Value) ? Value : null;
        }
        return null;
    }
    /**
     * Converts a number-box value to a data-source value.
     * @param {*} Value The number-box value.
     * @returns {number|null} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return tp.IsNumber(Value) ? Value : null;
    }

    // ● properties
    /**
     * Gets or sets the minimum value.
     * @returns {number|null} Returns the minimum value.
     */
    get Min() {
        return this.Handle instanceof HTMLInputElement && !tp.IsBlank(this.Handle.min) ? Number(this.Handle.min) : null;
    }
    /**
     * Gets or sets the minimum value.
     * @param {number|null|undefined} Value The minimum value.
     * @returns {void}
     */
    set Min(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.min = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the maximum value.
     * @returns {number|null} Returns the maximum value.
     */
    get Max() {
        return this.Handle instanceof HTMLInputElement && !tp.IsBlank(this.Handle.max) ? Number(this.Handle.max) : null;
    }
    /**
     * Gets or sets the maximum value.
     * @param {number|null|undefined} Value The maximum value.
     * @returns {void}
     */
    set Max(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.max = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the native step value.
     * @returns {string} Returns the step value.
     */
    get Step() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.step || "1" : "1";
    }
    /**
     * Gets or sets the native step value.
     * @param {string|number} Value The step value.
     * @returns {void}
     */
    set Step(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.step = tp.IsNil(Value) ? "1" : String(Value);
    }
    /**
     * Gets or sets the numeric value.
     * @returns {number|null} Returns the numeric value.
     */
    get Value() {
        if (this.Handle instanceof HTMLInputElement) {
            if (tp.IsBlank(this.Handle.value))
                return null;
            return Number.isFinite(this.Handle.valueAsNumber) ? this.Handle.valueAsNumber : null;
        }
        return null;
    }
    /**
     * Gets or sets the numeric value.
     * @param {number|null|undefined} Value The numeric value.
     * @returns {void}
     */
    set Value(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.value = tp.IsNumber(Value) ? String(Value) : "";
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
};

tp.Ui.RegisterType(["HtmlNumberBox", "tp-HtmlNumberBox"], tp.HtmlNumberBox);

// ● html number box ex
/**
 * A native HTML number input control with plus and minus buttons.
 *
 * Events:
 * - ValueChanged
 */
tp.HtmlNumberBoxEx = class extends tp.Control {
    // ● constructor
    /**
     * Creates a HTML number box with plus and minus buttons.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        var Params = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Params.ElementOrSelector))
            Params.ElementOrSelector = "div";
        super(Params);
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Value";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fTextBox = null;
        this.fMinus = null;
        this.fPlus = null;
        this.fTextChangeHandler = this.FuncBind(this.HandleTextChange);
        this.fMinusClickHandler = this.FuncBind(this.HandleMinusClick);
        this.fPlusClickHandler = this.FuncBind(this.HandlePlusClick);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateInnerControls();
    }
    /**
     * Applies explicit create params to this number box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Min))
            this.Min = Params.Min;
        if (!tp.IsNil(Params.Max))
            this.Max = Params.Max;
        if (!tp.IsNil(Params.Step))
            this.Step = Params.Step;
        if (!tp.IsNil(Params.Value))
            this.Value = Params.Value;
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = String(Params.Placeholder);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.NumberBox);
        tp.AddClass(this.Handle, tp.Classes.HtmlNumberBoxEx);
    }
    /**
     * Creates inner input and buttons.
     * @protected
     * @returns {void}
     */
    CreateInnerControls() {
        this.fTextBox = this.Document.createElement("input");
        this.fTextBox.type = "number";
        this.fTextBox.className = tp.Classes.Text;
        this.fTextBox.step = "0.1";
        this.fTextBox.value = "0";
        this.fTextBox.style.textAlign = "right";
        this.Handle.appendChild(this.fTextBox);
        this.fMinus = this.Document.createElement("div");
        this.fMinus.className = tp.Classes.Minus;
        this.fMinus.innerHTML = tp.HtmlNumberBoxEx.MinusSymbol;
        this.Handle.appendChild(this.fMinus);
        this.fPlus = this.Document.createElement("div");
        this.fPlus.className = tp.Classes.Plus;
        this.fPlus.innerHTML = tp.HtmlNumberBoxEx.PlusSymbol;
        this.Handle.appendChild(this.fPlus);
        this.fTextBox.addEventListener("change", this.fTextChangeHandler);
        this.fMinus.addEventListener("click", this.fMinusClickHandler);
        this.fPlus.addEventListener("click", this.fPlusClickHandler);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fTextBox)
            this.fTextBox.removeEventListener("change", this.fTextChangeHandler);
        if (this.fMinus)
            this.fMinus.removeEventListener("click", this.fMinusClickHandler);
        if (this.fPlus)
            this.fPlus.removeEventListener("click", this.fPlusClickHandler);
        this.fTextChangeHandler = null;
        this.fMinusClickHandler = null;
        this.fPlusClickHandler = null;
        this.fTextBox = null;
        this.fMinus = null;
        this.fPlus = null;
        super.DoDispose();
    }
    /**
     * Handles inner input change.
     * @protected
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    HandleTextChange(e) {
        this.WriteDataValue();
        this.OnValueChanged();
    }
    /**
     * Handles minus button click.
     * @protected
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleMinusClick(e) {
        this.StepDown();
        this.WriteDataValue();
        this.OnValueChanged();
    }
    /**
     * Handles plus button click.
     * @protected
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandlePlusClick(e) {
        this.StepUp();
        this.WriteDataValue();
        this.OnValueChanged();
    }
    /**
     * Converts a data-source value to a number-box value.
     * @param {*} Value The data-source value.
     * @returns {number|null} Returns the number value.
     */
    DataValueToDataProperty(Value) {
        if (tp.IsNumber(Value))
            return Value;
        if (!tp.IsBlankString(Value)) {
            Value = Number(Value);
            return Number.isFinite(Value) ? Value : null;
        }
        return null;
    }
    /**
     * Converts a number-box value to a data-source value.
     * @param {*} Value The number-box value.
     * @returns {number|null} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return tp.IsNumber(Value) ? Value : null;
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
     * Gets or sets the minimum value.
     * @returns {number|null} Returns the minimum value.
     */
    get Min() {
        return this.fTextBox instanceof HTMLInputElement && !tp.IsBlank(this.fTextBox.min) ? Number(this.fTextBox.min) : null;
    }
    /**
     * Gets or sets the minimum value.
     * @param {number|null|undefined} Value The minimum value.
     * @returns {void}
     */
    set Min(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.min = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the maximum value.
     * @returns {number|null} Returns the maximum value.
     */
    get Max() {
        return this.fTextBox instanceof HTMLInputElement && !tp.IsBlank(this.fTextBox.max) ? Number(this.fTextBox.max) : null;
    }
    /**
     * Gets or sets the maximum value.
     * @param {number|null|undefined} Value The maximum value.
     * @returns {void}
     */
    set Max(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.max = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the native step value.
     * @returns {string} Returns the step value.
     */
    get Step() {
        return this.fTextBox instanceof HTMLInputElement ? this.fTextBox.step || "1" : "1";
    }
    /**
     * Gets or sets the native step value.
     * @param {string|number} Value The step value.
     * @returns {void}
     */
    set Step(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.step = tp.IsNil(Value) ? "1" : String(Value);
    }
    /**
     * Gets or sets the numeric value.
     * @returns {number|null} Returns the numeric value.
     */
    get Value() {
        if (this.fTextBox instanceof HTMLInputElement) {
            if (tp.IsBlank(this.fTextBox.value))
                return null;
            return Number.isFinite(this.fTextBox.valueAsNumber) ? this.fTextBox.valueAsNumber : null;
        }
        return null;
    }
    /**
     * Gets or sets the numeric value.
     * @param {number|null|undefined} Value The numeric value.
     * @returns {void}
     */
    set Value(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.value = tp.IsNumber(Value) ? String(Value) : "";
    }
    /**
     * Gets or sets the placeholder text.
     * @returns {string} Returns the placeholder text.
     */
    get Placeholder() {
        return this.fTextBox instanceof HTMLInputElement ? this.fTextBox.placeholder || "" : "";
    }
    /**
     * Gets or sets the placeholder text.
     * @param {string} Value The placeholder text.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }

    // ● public
    /**
     * Increases the value by the native step.
     * @returns {void}
     */
    StepUp() {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.stepUp();
    }
    /**
     * Decreases the value by the native step.
     * @returns {void}
     */
    StepDown() {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.stepDown();
    }
};

/**
 * The plus button symbol.
 * @type {string}
 */
tp.HtmlNumberBoxEx.PlusSymbol = "▴";
/**
 * The minus button symbol.
 * @type {string}
 */
tp.HtmlNumberBoxEx.MinusSymbol = "▾";

tp.Ui.RegisterType(["HtmlNumberBoxEx", "tp-HtmlNumberBoxEx"], tp.HtmlNumberBoxEx);
