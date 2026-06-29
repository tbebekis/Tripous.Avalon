// ● value slider
/**
 * A native HTML range input control.
 *
 * It can be used either as a numeric position selector, or as a value selector
 * backed by a ValueList where the range position selects an item index.
 *
 * Events:
 * - ValueChanged
 */
tp.ValueSlider = class extends tp.InputControl {
    // ● constructor
    /**
     * Creates a value slider.
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
        this.fElementSubType = "range";
        this.fDataValueProperty = "Value";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fValueList = null;
        this.GetValueFunc = null;
        this.SetValueFunc = null;
        if (this.Handle instanceof HTMLInputElement) {
            this.Handle.min = tp.IsBlank(this.Handle.min) ? "0" : this.Handle.min;
            this.Handle.max = tp.IsBlank(this.Handle.max) ? "100" : this.Handle.max;
            this.Handle.step = tp.IsBlank(this.Handle.step) ? "1" : this.Handle.step;
            this.Handle.value = tp.IsBlank(this.Handle.value) ? "0" : this.Handle.value;
        }
    }
    /**
     * Applies explicit create params to this slider.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.GetValueFunc))
            this.GetValueFunc = Params.GetValueFunc;
        if (!tp.IsNil(Params.SetValueFunc))
            this.SetValueFunc = Params.SetValueFunc;
        if (!tp.IsNil(Params.ValueList))
            this.ValueList = Params.ValueList;
        if (!tp.IsNil(Params.Min))
            this.Min = Params.Min;
        if (!tp.IsNil(Params.Max))
            this.Max = Params.Max;
        if (!tp.IsNil(Params.Step))
            this.Step = Params.Step;
        if (!tp.IsNil(Params.Value))
            this.Value = Params.Value;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        tp.AddClass(this.Handle, tp.Classes.ValueSlider);
        super.OnHandleCreated();
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
     * Returns the value selected by the slider.
     * @protected
     * @returns {*} Returns the selected value.
     */
    GetValue() {
        var Index;
        if (tp.IsFunction(this.GetValueFunc))
            return this.GetValueFunc(this);
        if (this.Handle instanceof HTMLInputElement) {
            Index = tp.ToInt(this.Handle.value);
            return this.ValueList[Index];
        }
        return null;
    }
    /**
     * Selects a value.
     * @protected
     * @param {*} Value The value to select.
     * @returns {void}
     */
    SetValue(Value) {
        var Index;
        if (tp.IsFunction(this.SetValueFunc)) {
            this.SetValueFunc(this, Value);
            this.OnValueChanged();
        } else if (this.Handle instanceof HTMLInputElement) {
            Index = this.ValueList.indexOf(Value);
            if (Index !== -1) {
                this.Handle.value = Index.toString();
                this.OnValueChanged();
            }
        }
    }

    // ● properties
    /**
     * Gets or sets the minimum range position.
     * @returns {number|string} Returns the minimum range position.
     */
    get Min() {
        return this.UsesValueList ? 0 : (this.Handle instanceof HTMLInputElement ? this.Handle.min : 0);
    }
    /**
     * Gets or sets the minimum range position.
     * @param {number|string} Value The minimum range position.
     * @returns {void}
     */
    set Min(Value) {
        if (!this.UsesValueList && this.Handle instanceof HTMLInputElement)
            this.Handle.min = tp.IsNil(Value) ? "0" : String(Value);
    }
    /**
     * Gets or sets the maximum range position.
     * @returns {number|string} Returns the maximum range position.
     */
    get Max() {
        return this.UsesValueList ? this.ValueList.length - 1 : (this.Handle instanceof HTMLInputElement ? this.Handle.max : 0);
    }
    /**
     * Gets or sets the maximum range position.
     * @param {number|string} Value The maximum range position.
     * @returns {void}
     */
    set Max(Value) {
        if (!this.UsesValueList && this.Handle instanceof HTMLInputElement)
            this.Handle.max = tp.IsNil(Value) ? "100" : String(Value);
    }
    /**
     * Gets or sets the step size.
     * @returns {number} Returns the step size.
     */
    get Step() {
        return this.Handle instanceof HTMLInputElement ? Number(this.Handle.step) : 1;
    }
    /**
     * Gets or sets the step size.
     * @param {number|string} Value The step size.
     * @returns {void}
     */
    set Step(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.step = tp.IsNil(Value) ? "1" : String(Value);
    }
    /**
     * Gets or sets the selected value.
     * @returns {*} Returns the selected value.
     */
    get Value() {
        if (this.Handle instanceof HTMLInputElement)
            return this.UsesValueList ? this.GetValue() : this.Handle.value;
        return null;
    }
    /**
     * Gets or sets the selected value.
     * @param {*} Value The value.
     * @returns {void}
     */
    set Value(Value) {
        var Min;
        var Max;
        var NumberValue;
        if (!(this.Handle instanceof HTMLInputElement))
            return;
        if (this.UsesValueList) {
            this.SetValue(Value);
            return;
        }
        Min = Number(this.Handle.min);
        Max = Number(this.Handle.max);
        NumberValue = Number(Value);
        if (Number.isFinite(NumberValue) && Min <= NumberValue && NumberValue <= Max) {
            this.Handle.value = NumberValue.toString();
            this.OnValueChanged();
        }
    }
    /**
     * Gets or sets the value list.
     * @returns {Array} Returns the value list.
     */
    get ValueList() {
        return tp.IsArray(this.fValueList) ? this.fValueList : [];
    }
    /**
     * Gets or sets the value list.
     * @param {Array|null|undefined} Value The value list.
     * @returns {void}
     */
    set ValueList(Value) {
        if (!(this.Handle instanceof HTMLInputElement))
            return;
        if (tp.IsArray(Value) && Value.length > 0) {
            this.fValueList = Value;
            this.Handle.min = "0";
            this.Handle.max = (Value.length - 1).toString();
            this.Handle.step = "1";
            if (tp.ToInt(this.Handle.value) > Value.length - 1)
                this.Handle.value = "0";
        } else {
            this.fValueList = null;
            this.Handle.min = "0";
            this.Handle.max = "100";
            this.Handle.step = "1";
        }
        this.OnValueChanged();
    }
    /**
     * Returns true when a value list is used.
     * @returns {boolean} Returns true when a value list is used.
     */
    get UsesValueList() {
        return tp.IsArray(this.fValueList) && this.fValueList.length > 0;
    }

    // ● public
    /**
     * Increments the slider position.
     * @param {number|null|undefined} Count The optional step count.
     * @returns {void}
     */
    StepUp(Count) {
        if (this.Handle instanceof HTMLInputElement) {
            if (tp.IsNil(Count))
                this.Handle.stepUp();
            else
                this.Handle.stepUp(tp.ToInt(Count));
            this.WriteDataValue();
            this.OnValueChanged();
        }
    }
    /**
     * Decrements the slider position.
     * @param {number|null|undefined} Count The optional step count.
     * @returns {void}
     */
    StepDown(Count) {
        if (this.Handle instanceof HTMLInputElement) {
            if (tp.IsNil(Count))
                this.Handle.stepDown();
            else
                this.Handle.stepDown(tp.ToInt(Count));
            this.WriteDataValue();
            this.OnValueChanged();
        }
    }
};

tp.Ui.RegisterType(["ValueSlider", "tp-ValueSlider"], tp.ValueSlider);
