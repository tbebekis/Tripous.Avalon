// ● row
/**
 * A responsive row.
 * A row listens to its own element resize changes and propagates SizeMode changes to direct child components.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 *
 * @example
 * <div id="Row">
 *     <div class="tp-Col"></div>
 *     <div class="tp-Col"></div>
 * </div>
 * <script>
 *     var Row = new tp.Row({ ElementOrSelector: "#Row", Breakpoints: [400, 700, 1000, 1200, 1400] });
 * </script>
 */
tp.Row = class extends tp.Component {
    // ● constructor
    /**
     * Creates a responsive row.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The row create parameters.
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
        this.IsElementResizeListener = true;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.Row);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Row.prototype.tpClass = "tp.Row";

// ● column
/**
 * A responsive column.
 * WidthPercents contains one percent width for each size mode:
 * XSmall, Small, Medium, Large, XLarge, and XXLarge.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 *
 * @example
 * <div id="Col"></div>
 * <script>
 *     var Col = new tp.Col({ ElementOrSelector: "#Col", WidthPercents: [100, 100, 50, 33.33, 33.33, 25] });
 * </script>
 */
tp.Col = class extends tp.Component {
    // ● constructor
    /**
     * Creates a responsive column.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The column create parameters.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.Col);
    }
    /**
     * Applies explicit create params to this column.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (tp.IsArray(Params.WidthPercents))
            this.WidthPercents = this.NormalizePercents(Params.WidthPercents, this.WidthPercents);
        if (tp.IsArray(Params.ControlWidthPercents))
            this.ControlWidthPercents = this.NormalizePercents(Params.ControlWidthPercents, this.ControlWidthPercents);
    }
    /**
     * Normalizes a percent array to the number of supported size modes.
     * @param {number[]} Source The source percent array.
     * @param {number[]} Default The default percent array.
     * @returns {number[]} Returns a normalized percent array.
     */
    NormalizePercents(Source, Default) {
        var Result = Default.slice();
        var Index;
        if (tp.IsArray(Source)) {
            for (Index = 0; Index < Source.length && Index < Result.length; Index++) {
                if (tp.IsNumber(Source[Index]))
                    Result[Index] = Source[Index];
            }
        }
        return Result;
    }

    // ● public
    /**
     * Notification called by a parent component when its SizeMode changes.
     * @param {string} ParentSizeMode A tp.SizeMode value.
     * @returns {void}
     */
    ParentSizeModeChanged(ParentSizeMode) {
        var Index = tp.SizeModes.indexOf(ParentSizeMode);
        var Percent;
        var List;
        if (Index > 0) {
            Percent = this.WidthPercents[Index - 1];
            if (tp.IsNumber(Percent))
                this.Handle.style.width = Percent + "%";
            List = this.GetComponentList();
            List.forEach(function (Component) {
                if (tp.IsFunction(tp.CtrlRow) && Component instanceof tp.CtrlRow && tp.IsFunction(Component.SetControlPercentWidth)) {
                    Percent = this.ControlWidthPercents[Index - 1];
                    if (tp.IsNumber(Percent))
                        Component.SetControlPercentWidth(Percent + "%");
                }
            }, this);
        }
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Col.prototype.tpClass = "tp.Col";
/**
 * Percent widths to occupy from parent row according to size mode.
 * Values correspond to XSmall, Small, Medium, Large, XLarge, and XXLarge.
 * @type {number[]}
 */
tp.Col.prototype.WidthPercents = [100, 100, 50, 33.33, 33.33, 25];
/**
 * Percent widths for the control part of a child tp.CtrlRow according to size mode.
 * Values correspond to XSmall, Small, Medium, Large, XLarge, and XXLarge.
 * @type {number[]}
 */
tp.Col.prototype.ControlWidthPercents = [100, 100, 60, 65, 65, 65];

// ● control row
/**
 * A responsive control row with a label, required mark, and one child control.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 *
 * @example
 * <div class="tp-CtrlRow" data-setup="{Text: 'Trader', Control: { TypeName: 'TextBox', Id: 'Name', DataField: 'Name' } }"></div>
 */
tp.CtrlRow = class extends tp.Component {
    // ● constructor
    /**
     * Creates a control row.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The row create parameters.
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
        this.Control = null;
        this.elTextContainer = null;
        this.elCtrlContainer = null;
        this.elRequiredMark = null;
        this.elText = null;
        this.fText = "";
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.CtrlRow);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.EnsureContent();
    }
    /**
     * Applies explicit create params to this control row.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        var BaseParams;
        if (!Params) {
            super.ApplyCreateParams(Params);
            return;
        }
        BaseParams = new tp.CreateParams(Params);
        delete BaseParams.Text;
        super.ApplyCreateParams(BaseParams);
        if (!tp.IsNil(Params.Text)) {
            if (this.elText instanceof HTMLElement)
                this.Text = Params.Text;
            else
                this.fText = String(Params.Text);
        }
    }
    /**
     * Ensures the row markup and child control exist.
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
        Setup.Text = tp.IsString(Setup.Text) ? Setup.Text.trim() : this.fText;
        Setup.Control = tp.IsObject(Setup.Control) ? Setup.Control : {};
        return Setup;
    }
    /**
     * Builds the row markup and creates the child control.
     * @param {object} Setup The normalized setup.
     * @returns {void}
     */
    BuildContent(Setup) {
        var TypeName = Setup.Control.TypeName;
        var Type = tp.Ui.GetType(TypeName);
        var DataField;
        var Prefix;
        var Params;
        if (!tp.IsFunction(Type))
            tp.Throw("Control type name not registered in tp.Ui.Types: " + TypeName);
        DataField = tp.IsString(Setup.Control.DataField) ? Setup.Control.DataField.trim() : "";
        Prefix = !tp.IsBlank(DataField) ? tp.Prefix + "CtrlRow-" + DataField + "-" : tp.Prefix + "CtrlRow-";
        if (tp.IsBlank(this.Handle.id))
            this.Handle.id = tp.SafeId(Prefix);
        if (tp.IsBlank(Setup.Control.Id))
            Setup.Control.Id = tp.SafeId(tp.Prefix + TypeName + "-");
        this.Handle.innerHTML =
            "<div class=\"" + tp.Classes.CText + "\">" +
            "<label for=\"" + Setup.Control.Id + "\"></label>" +
            "<span class=\"" + tp.Classes.RequiredMark + "\" style=\"display: none;\">*</span>" +
            "</div>" +
            "<div class=\"" + tp.Classes.Ctrl + "\"></div>";
        this.ResolveContent();
        this.Text = Setup.Text;
        Params = new tp.CreateParams(Setup.Control);
        Params.Parent = this.elCtrlContainer;
        Params.elText = this.elText;
        Params.elRequiredMark = this.elRequiredMark;
        this.Control = new Type(Params);
    }
    /**
     * Resolves the row child elements.
     * @returns {void}
     */
    ResolveContent() {
        this.elCtrlContainer = tp.Select(this.Handle, "." + tp.Classes.Ctrl);
        this.elTextContainer = tp.Select(this.Handle, "." + tp.Classes.CText);
        this.elRequiredMark = tp.Select(this.elTextContainer, "." + tp.Classes.RequiredMark);
        this.elText = tp.Select(this.elTextContainer, "label");
        if (this.elText instanceof HTMLLabelElement)
            this.fText = this.elText.textContent || "";
    }

    // ● public
    /**
     * Sets the width of the control part of the row.
     * @param {string} Width The width to apply, e.g. "50%".
     * @returns {void}
     */
    SetControlPercentWidth(Width) {
        var ControlPercent = parseFloat(Width);
        var LabelPercent;
        if (!this.HasHandle || isNaN(ControlPercent))
            return;
        ControlPercent = Math.max(0, Math.min(100, ControlPercent));
        LabelPercent = 100 - ControlPercent;

        /*
         * Old flex layout code:
         * if (this.elCtrlContainer instanceof HTMLElement)
         *     this.elCtrlContainer.style.width = Width;
         *
         * CtrlRow now uses CSS grid, so the row controls the two columns.
         */
        this.Handle.style.gridTemplateColumns = "minmax(120px, " + LabelPercent + "%) minmax(0, " + ControlPercent + "%)";
    }

    // ● properties
    /**
     * Gets or sets the row label text.
     * @returns {string} Returns the label text.
     */
    get Text() {
        return this.elText instanceof HTMLElement ? this.elText.textContent || "" : this.fText;
    }
    /**
     * Gets or sets the row label text.
     * @param {*} Value The label text.
     * @returns {void}
     */
    set Text(Value) {
        this.fText = tp.IsNil(Value) ? "" : String(Value);
        if (this.elText instanceof HTMLElement)
            this.elText.textContent = this.fText;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.CtrlRow.prototype.tpClass = "tp.CtrlRow";
/**
 * The control hosted by this row.
 * @type {tp.Component|null}
 */
tp.CtrlRow.prototype.Control = null;
/**
 * Caption text container.
 * @type {HTMLDivElement|null}
 */
tp.CtrlRow.prototype.elTextContainer = null;
/**
 * Control container.
 * @type {HTMLDivElement|null}
 */
tp.CtrlRow.prototype.elCtrlContainer = null;
/**
 * Element with the required mark.
 * @type {HTMLSpanElement|null}
 */
tp.CtrlRow.prototype.elRequiredMark = null;
/**
 * Label with the caption text.
 * @type {HTMLLabelElement|null}
 */
tp.CtrlRow.prototype.elText = null;

// ● check-box row
/**
 * A responsive check-box control row.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 *
 * @example
 * <div class="tp-CheckBoxRow" data-setup="{Text: 'Some caption here', Control: { TypeName: 'CheckBox', Id: 'Flag', DataField: 'Flag' } }"></div>
 */
tp.CheckBoxRow = class extends tp.Component {
    // ● constructor
    /**
     * Creates a check-box row.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The row create parameters.
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
        this.Control = null;
        this.elRequiredMark = null;
        this.elText = null;
        this.fText = "";
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.CheckBoxRow);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.EnsureContent();
    }
    /**
     * Applies explicit create params to this check-box row.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        var BaseParams;
        if (!Params) {
            super.ApplyCreateParams(Params);
            return;
        }
        BaseParams = new tp.CreateParams(Params);
        delete BaseParams.Text;
        super.ApplyCreateParams(BaseParams);
        if (!tp.IsNil(Params.Text)) {
            if (this.elText instanceof HTMLElement)
                this.Text = Params.Text;
            else
                this.fText = String(Params.Text);
        }
    }
    /**
     * Ensures the row markup and child check-box exist.
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
        Setup.Text = tp.IsString(Setup.Text) ? Setup.Text.trim() : this.fText;
        Setup.Control = tp.IsObject(Setup.Control) ? Setup.Control : {};
        return Setup;
    }
    /**
     * Builds the row markup and creates the child check-box control.
     * @param {object} Setup The normalized setup.
     * @returns {void}
     */
    BuildContent(Setup) {
        var TypeName = !tp.IsBlank(Setup.Control.TypeName) ? Setup.Control.TypeName : "CheckBox";
        var Type = tp.Ui.GetType(TypeName) || tp.CheckBox;
        var DataField;
        var Prefix;
        var Params;
        var Element;
        DataField = tp.IsString(Setup.Control.DataField) ? Setup.Control.DataField.trim() : "";
        Prefix = !tp.IsBlank(DataField) ? tp.Prefix + "CheckBoxRow-" + DataField + "-" : tp.Prefix + "CheckBoxRow-";
        if (tp.IsBlank(this.Handle.id))
            this.Handle.id = tp.SafeId(Prefix);
        if (tp.IsBlank(Setup.Control.Id))
            Setup.Control.Id = tp.SafeId(tp.Prefix + TypeName + "-");
        this.Handle.innerHTML =
            "<label class=\"" + tp.Classes.CheckBox + "\">" +
            "<span class=\"" + tp.Classes.Ctrl + "\"><input type=\"checkbox\" /></span>" +
            "<span class=\"" + tp.Classes.RequiredMark + "\" style=\"display: none;\">*</span>" +
            "<span class=\"" + tp.Classes.Text + "\"></span>" +
            "</label>";
        this.ResolveContent();
        this.Text = Setup.Text;
        Params = new tp.CreateParams(Setup.Control);
        Params.Parent = this.Handle;
        Params.elText = this.elText;
        Params.elRequiredMark = this.elRequiredMark;
        Element = tp.Select(this.Handle, "label." + tp.Classes.CheckBox);
        this.Control = new Type(Object.assign(Params, { ElementOrSelector: Element }));
    }
    /**
     * Resolves the row child elements.
     * @returns {void}
     */
    ResolveContent() {
        this.elRequiredMark = tp.Select(this.Handle, "." + tp.Classes.RequiredMark);
        this.elText = tp.Select(this.Handle, "." + tp.Classes.Text);
        if (this.elText instanceof HTMLElement)
            this.fText = this.elText.textContent || "";
    }

    // ● properties
    /**
     * Gets or sets the check-box row text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.elText instanceof HTMLElement ? this.elText.textContent || "" : this.fText;
    }
    /**
     * Gets or sets the check-box row text.
     * @param {*} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        this.fText = tp.IsNil(Value) ? "" : String(Value);
        if (this.elText instanceof HTMLElement)
            this.elText.textContent = this.fText;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.CheckBoxRow.prototype.tpClass = "tp.CheckBoxRow";
/**
 * The check-box control hosted by this row.
 * @type {tp.Component|null}
 */
tp.CheckBoxRow.prototype.Control = null;
/**
 * Element with the required mark.
 * @type {HTMLSpanElement|null}
 */
tp.CheckBoxRow.prototype.elRequiredMark = null;
/**
 * Label with the caption text.
 * @type {HTMLElement|null}
 */
tp.CheckBoxRow.prototype.elText = null;

tp.Ui.RegisterType(["Row", "tp-Row"], tp.Row);
tp.Ui.RegisterType(["Col", "tp-Col"], tp.Col);
tp.Ui.RegisterType(["CtrlRow", "tp-CtrlRow"], tp.CtrlRow);
tp.Ui.RegisterType(["CheckBoxRow", "tp-CheckBoxRow"], tp.CheckBoxRow);
