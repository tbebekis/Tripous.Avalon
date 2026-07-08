// ● select filter row
/**
 * A WebDesk select filter row.
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
tp.SelectFilterRow = class extends tp.Component {
    // ● constructor
     /**
     * Creates a select filter row.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
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
        this.SelectName = "";
        this.FilterName = "";
        this.Title = "";
        this.TitleKey = "";
        this.FieldName = "";
        this.FilterDataType = "String";
        this.BoolOp = "And";
        this.ConditionOp = "Equal";
        this.BoolOpCombo = null;
        this.ConditionOpCombo = null;
        this.ValueControl = null;
        this.ValueControl2 = null;
        this.elTextContainer = null;
        this.elCtrlContainer = null;
        this.elText = null;
        this.elBoolRow = null;
        this.elValueRow = null;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.SelectFilterRow);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.EnsureContent();
    }
    /**
     * Applies explicit create params to this component.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.SelectName))
            this.SelectName = String(Params.SelectName);
        if (!tp.IsNil(Params.FilterName))
            this.FilterName = String(Params.FilterName);
        else if (!tp.IsNil(Params.Name))
            this.FilterName = String(Params.Name);
        if (!tp.IsNil(Params.Title))
            this.Title = String(Params.Title);
        if (!tp.IsNil(Params.TitleKey))
            this.TitleKey = String(Params.TitleKey);
        if (!tp.IsNil(Params.FieldName))
            this.FieldName = String(Params.FieldName);
        if (!tp.IsNil(Params.FilterDataType))
            this.FilterDataType = String(Params.FilterDataType);
        if (!tp.IsNil(Params.BoolOp))
            this.BoolOp = String(Params.BoolOp);
        if (!tp.IsNil(Params.ConditionOp))
            this.ConditionOp = String(Params.ConditionOp);
        this.ApplyDefaults();
    }
    /**
     * Ensures the row markup and child controls exist.
     * @returns {void}
     */
    EnsureContent() {
        var Setup = this.GetSetup();
        if (!this.HasHandle)
            return;
        if (this.Handle.children.length === 0)
            this.BuildContent(Setup);
        else
            this.ResolveContent();
    }
    /**
     * Returns normalized row setup.
     * @returns {object} Returns the setup object.
     */
    GetSetup() {
        var Setup = this.CreateParams || {};
        return {
            FilterDataType: tp.IsString(Setup.FilterDataType) ? Setup.FilterDataType : this.FilterDataType,
            ConditionOp: tp.IsString(Setup.ConditionOp) ? Setup.ConditionOp : this.ConditionOp
        };
    }
    /**
     * Builds the row markup and child controls.
     * @param {object} Setup The normalized setup.
     * @returns {void}
     */
    BuildContent(Setup) {
        this.Handle.innerHTML =
            "<div class=\"" + tp.Classes.Ctrl + "\"></div>";
        this.ResolveContent();
        this.SetupControlLayout();
        this.elBoolRow = this.CreateControlRow();
        this.elText = this.Document.createElement("span");
        this.elText.style.flex = "1 1 auto";
        this.elText.style.minWidth = "0";
        this.elText.style.paddingLeft = "6px";
        this.elText.style.overflow = "hidden";
        this.elText.style.textOverflow = "ellipsis";
        this.elText.style.whiteSpace = "nowrap";
        this.elValueRow = this.CreateControlRow();
        this.BoolOpCombo = this.CreateCombo(this.elBoolRow, ["And", "Or"], 74);
        this.elBoolRow.appendChild(this.elText);
        this.ConditionOpCombo = this.CreateCombo(this.elBoolRow, this.GetConditionOps(Setup.FilterDataType), 118);
        this.ValueControl = this.CreateValueControl(false, Setup.FilterDataType, this.elValueRow);
        this.ValueControl2 = this.CreateValueControl(true, Setup.FilterDataType, this.elValueRow);
        this.ConditionOpCombo.On("SelectedIndexChanged", this.HandleConditionChanged, this);
    }
    /**
     * Sets the layout rules for the row control area.
     * @returns {void}
     */
    SetupControlLayout() {
        if (!(this.elCtrlContainer instanceof HTMLElement))
            return;
        this.Handle.style.display = "block";
        this.Handle.style.padding = "4px 4px 6px 4px";
        this.elCtrlContainer.style.display = "flex";
        this.elCtrlContainer.style.flexDirection = "column";
        this.elCtrlContainer.style.gap = "4px";
        this.elCtrlContainer.style.alignItems = "stretch";
    }
    /**
     * Creates an inner control row.
     * @returns {HTMLDivElement} Returns the created row.
     */
    CreateControlRow() {
        var Result = this.Document.createElement("div");
        Result.style.display = "flex";
        Result.style.flexWrap = "wrap";
        Result.style.gap = "3px";
        Result.style.alignItems = "center";
        Result.style.width = "100%";
        this.elCtrlContainer.appendChild(Result);
        return Result;
    }
    /**
     * Resolves child elements.
     * @returns {void}
     */
    ResolveContent() {
        this.elCtrlContainer = tp.Select(this.Handle, "." + tp.Classes.Ctrl);
        this.elTextContainer = null;
        this.elText = null;
    }
    /**
     * Applies default display and operator values.
     * @returns {void}
     */
    ApplyDefaults() {
        if (this.elText instanceof HTMLElement)
            this.elText.textContent = this.Title || this.FilterName;
        this.SetComboValue(this.BoolOpCombo, this.BoolOp || "And");
        this.SetComboValue(this.ConditionOpCombo, this.ConditionOp || "Equal");
        this.UpdateValueControlVisibility();
    }
    /**
     * Creates a combo box.
     * @param {HTMLElement} Parent The parent element.
     * @param {string[]} Items The combo items.
     * @param {number} Width The combo width.
     * @returns {tp.ComboBox} Returns the created combo.
     */
    CreateCombo(Parent, Items, Width) {
        var Result = new tp.ComboBox({
            Parent: Parent,
            List: Items,
            SelectedIndex: 0
        });
        this.SetControlFlex(Result, Width + "px", "0 0 " + Width + "px");
        this.ApplyAvaloniaControlStyle(Result);
        return Result;
    }
    /**
     * Creates a value control.
     * @param {boolean} IsSecond True to create the second value control.
     * @param {string} FilterDataType The filter data type.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.Component} Returns the created control.
     */
    CreateValueControl(IsSecond, FilterDataType, Parent) {
        var Params = { Parent: Parent };
        if (!IsSecond && this.IsBooleanType(FilterDataType))
            return this.SetupValueControl(new tp.ComboBox({
                Parent: Parent,
                List: ["All", "True", "False"],
                SelectedIndex: 0
            }), IsSecond);
        if (this.IsDateTimeType(FilterDataType))
            return this.SetupValueControl(new tp.HtmlDateBox(Params), IsSecond);
        return this.SetupValueControl(new tp.TextBox(Params), IsSecond);
    }
    /**
     * Applies layout to a value control.
     * @param {tp.Component} Control The control.
     * @param {boolean} IsSecond True when this is the second value control.
     * @returns {tp.Component} Returns the control.
     */
    SetupValueControl(Control, IsSecond) {
        this.SetControlFlex(Control, "100%", IsSecond ? "1 1 90px" : "1 1 120px");
        this.ApplyAvaloniaControlStyle(Control);
        return Control;
    }
    /**
     * Applies flex sizing to a child control.
     * @param {tp.Component|null} Control The child control.
     * @param {string} Width The CSS width.
     * @param {string} Flex The CSS flex value.
     * @returns {void}
     */
    SetControlFlex(Control, Width, Flex) {
        if (!Control || !(Control.Handle instanceof HTMLElement))
            return;
        Control.Handle.style.width = Width;
        Control.Handle.style.flex = Flex;
        Control.Handle.style.minWidth = "0";
    }
    /**
     * Applies filter-panel control styling closer to the desktop controls.
     * @param {tp.Component|null} Control The child control.
     * @returns {void}
     */
    ApplyAvaloniaControlStyle(Control) {
        var Strip;
        var TextInput;
        var Button;
        if (!Control || !(Control.Handle instanceof HTMLElement))
            return;
        Control.Handle.style.boxSizing = "border-box";
        Control.Handle.style.height = "31px";
        Control.Handle.style.minHeight = "31px";
        Control.Handle.style.border = "1px solid #8a8f96";
        Control.Handle.style.borderRadius = "3px";
        Control.Handle.style.backgroundColor = "#ffffff";
        Control.Handle.style.color = "#111111";
        Control.Handle.style.font = "inherit";
        Control.Handle.style.overflow = "hidden";
        if (Control.Handle instanceof HTMLInputElement) {
            Control.Handle.style.padding = "4px 8px";
            Control.Handle.style.outline = "none";
        }
        if (Control instanceof tp.ComboBox) {
            Strip = tp.Select(Control.Handle, "." + tp.Classes.Strip);
            TextInput = tp.Select(Control.Handle, "input." + tp.Classes.Text);
            Button = tp.Select(Control.Handle, "." + tp.Classes.Btn);
            if (Strip instanceof HTMLElement) {
                Strip.style.height = "100%";
                Strip.style.minHeight = "0";
            }
            if (TextInput instanceof HTMLInputElement) {
                TextInput.style.padding = "0 8px";
                TextInput.style.font = "inherit";
                TextInput.style.color = "#111111";
                TextInput.style.backgroundColor = "#ffffff";
                TextInput.style.outline = "none";
            }
            if (Button instanceof HTMLElement) {
                Button.style.width = "28px";
                Button.style.padding = "0";
                Button.style.borderLeft = "1px solid #c5c9cf";
                Button.style.backgroundColor = "#ffffff";
                Button.style.color = "#222222";
            }
        }
    }
    /**
     * Returns true when a data type is boolean.
     * @param {string} FilterDataType The filter data type.
     * @returns {boolean} Returns true when boolean.
     */
    IsBooleanType(FilterDataType) {
        return tp.IsSameText(FilterDataType, "Boolean");
    }
    /**
     * Returns true when a data type is date or datetime.
     * @param {string} FilterDataType The filter data type.
     * @returns {boolean} Returns true when date-like.
     */
    IsDateTimeType(FilterDataType) {
        return tp.IsSameText(FilterDataType, "Date") || tp.IsSameText(FilterDataType, "DateTime");
    }
    /**
     * Returns true when this row is a boolean filter.
     * @returns {boolean} Returns true when boolean.
     */
    IsBoolean() {
        return this.IsBooleanType(this.FilterDataType);
    }
    /**
     * Returns true when this row is a date or datetime filter.
     * @returns {boolean} Returns true when date-like.
     */
    IsDateTime() {
        return this.IsDateTimeType(this.FilterDataType);
    }
    /**
     * Returns the supported condition operators.
     * @param {string|null|undefined} FilterDataType The filter data type.
     * @returns {string[]} Returns the operators.
     */
    GetConditionOps(FilterDataType) {
        FilterDataType = FilterDataType || this.FilterDataType;
        if (tp.IsSameText(FilterDataType, "Boolean"))
            return ["Equal"];
        if (tp.IsSameText(FilterDataType, "String"))
            return ["Equal", "Contains", "StartsWith", "EndsWith"];
        return ["Equal", "GreaterOrEqual", "LessOrEqual", "Between"];
    }
    /**
     * Sets a combo selected value.
     * @param {tp.ComboBox|null} Combo The combo box.
     * @param {string} Value The value to select.
     * @returns {void}
     */
    SetComboValue(Combo, Value) {
        var Index;
        if (!(Combo instanceof tp.ComboBox))
            return;
        Index = Combo.Items.indexOf(Value);
        Combo.SelectedIndex = Index >= 0 ? Index : 0;
    }
    /**
     * Handles condition changes.
     * @returns {void}
     */
    HandleConditionChanged() {
        this.UpdateValueControlVisibility();
    }
    /**
     * Updates value control visibility.
     * @returns {void}
     */
    UpdateValueControlVisibility() {
        if (this.ConditionOpCombo instanceof tp.ComboBox)
            this.ConditionOp = this.ConditionOpCombo.SelectedValue || "Equal";
        if (this.ValueControl2)
            this.ValueControl2.Visible = !this.IsBoolean() && tp.IsSameText(this.ConditionOp, "Between");
        if (this.ConditionOpCombo)
            this.ConditionOpCombo.Visible = !this.IsBoolean();
        if (this.elValueRow instanceof HTMLElement)
            this.elValueRow.style.display = "flex";
    }
    /**
     * Reads a value from a value control.
     * @param {tp.Component|null} Control The value control.
     * @returns {*} Returns the value.
     */
    ReadValue(Control) {
        var Text;
        var Value;
        if (Control instanceof tp.HtmlDateBox)
            return tp.IsBlank(Control.Value) ? null : Control.Value;
        if (Control instanceof tp.TextBox) {
            Text = Control.Text || "";
            if (tp.IsBlank(Text))
                return null;
            if (tp.IsSameText(this.FilterDataType, "Integer"))
                return tp.StrToInt(Text, 0);
            if (["Decimal", "Decimal_", "Double"].indexOf(this.FilterDataType) >= 0)
                return Number(String(Text).replace(",", "."));
            return Text;
        }
        if (Control instanceof tp.ComboBox && this.IsBoolean()) {
            Value = Control.SelectedValue || "All";
            if (tp.IsSameText(Value, "True"))
                return 1;
            if (tp.IsSameText(Value, "False"))
                return 0;
        }
        return null;
    }

    // ● public
    /**
     * Clears the row filter values.
     * @returns {void}
     */
    Clear() {
        this.SetComboValue(this.BoolOpCombo, "And");
        this.SetComboValue(this.ConditionOpCombo, "Equal");
        if (this.ValueControl instanceof tp.TextBox)
            this.ValueControl.Text = "";
        if (this.ValueControl2 instanceof tp.TextBox)
            this.ValueControl2.Text = "";
        if (this.ValueControl instanceof tp.HtmlDateBox)
            this.ValueControl.Value = "";
        if (this.ValueControl2 instanceof tp.HtmlDateBox)
            this.ValueControl2.Value = "";
        if (this.ValueControl instanceof tp.ComboBox)
            this.ValueControl.SelectedIndex = 0;
        this.UpdateValueControlVisibility();
    }
    /**
     * Returns the active filter object, if the row has valid values.
     * @returns {object|null} Returns an active filter object or null.
     */
    GetActiveFilter() {
        var Result;
        var Value = this.ReadValue(this.ValueControl);
        var Value2 = this.ReadValue(this.ValueControl2);
        var ConditionOp = this.IsBoolean() ? "Equal" : (this.ConditionOpCombo.SelectedValue || "Equal");
        if (Value === null || Number.isNaN(Value))
            return null;
        if (tp.IsSameText(ConditionOp, "Between") && (Value2 === null || Number.isNaN(Value2)))
            return null;
        Result = {
            SelectName: this.SelectName,
            Name: this.FilterName,
            BoolOp: this.BoolOpCombo.SelectedValue || "And",
            ConditionOp: ConditionOp,
            Value: Value
        };
        if (tp.IsSameText(ConditionOp, "Between"))
            Result.Value2 = Value2;
        return Result;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.SelectFilterRow.prototype.tpClass = "tp.SelectFilterRow";
/**
 * The select name.
 * @type {string}
 */
tp.SelectFilterRow.prototype.SelectName = "";
/**
 * The filter name.
 * @type {string}
 */
tp.SelectFilterRow.prototype.FilterName = "";
/**
 * The display title.
 * @type {string}
 */
tp.SelectFilterRow.prototype.Title = "";
/**
 * The title key.
 * @type {string}
 */
tp.SelectFilterRow.prototype.TitleKey = "";
/**
 * The filter field name.
 * @type {string}
 */
tp.SelectFilterRow.prototype.FieldName = "";
/**
 * The filter data type.
 * @type {string}
 */
tp.SelectFilterRow.prototype.FilterDataType = "String";
/**
 * The default boolean operator.
 * @type {string}
 */
tp.SelectFilterRow.prototype.BoolOp = "And";
/**
 * The default condition operator.
 * @type {string}
 */
tp.SelectFilterRow.prototype.ConditionOp = "Equal";
/**
 * The boolean operator combo.
 * @type {tp.ComboBox|null}
 */
tp.SelectFilterRow.prototype.BoolOpCombo = null;
/**
 * The condition operator combo.
 * @type {tp.ComboBox|null}
 */
tp.SelectFilterRow.prototype.ConditionOpCombo = null;
/**
 * The first value control.
 * @type {tp.Component|null}
 */
tp.SelectFilterRow.prototype.ValueControl = null;
/**
 * The second value control.
 * @type {tp.Component|null}
 */
tp.SelectFilterRow.prototype.ValueControl2 = null;
/**
 * Caption text container.
 * @type {HTMLDivElement|null}
 */
tp.SelectFilterRow.prototype.elTextContainer = null;
/**
 * Control container.
 * @type {HTMLDivElement|null}
 */
tp.SelectFilterRow.prototype.elCtrlContainer = null;
/**
 * Label with the caption text.
 * @type {HTMLLabelElement|null}
 */
tp.SelectFilterRow.prototype.elText = null;
/**
 * Boolean operator row.
 * @type {HTMLDivElement|null}
 */
tp.SelectFilterRow.prototype.elBoolRow = null;
/**
 * Value control row.
 * @type {HTMLDivElement|null}
 */
tp.SelectFilterRow.prototype.elValueRow = null;

tp.Ui.RegisterType(["SelectFilterRow", "tp-SelectFilterRow"], tp.SelectFilterRow);
