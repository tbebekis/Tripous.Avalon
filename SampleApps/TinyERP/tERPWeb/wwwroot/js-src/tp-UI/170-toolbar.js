// ● button-ex ico mode
/**
 * Indicates the existence and position of a ButtonEx icon.
 * @enum {number}
 */
tp.ButtonExIcoMode = {
    None: 0,
    Left: 1,
    Top: 2
};
Object.freeze(tp.ButtonExIcoMode);

// ● toolbar event args
/**
 * Event arguments for toolbar button clicks.
 */
tp.ToolBarItemClickEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates toolbar item click event arguments.
     * @param {tp.Component|null|undefined} Item The clicked item.
     * @param {string|null|undefined} Command The item command.
     */
    constructor(Item, Command) {
        super(tp.Events.Click, null, null);
        this.Item = tp.IsNil(Item) ? null : Item;
        this.Button = this.Item;
        this.Command = tp.IsNil(Command) ? "" : String(Command);
    }
};

// ● prototype
/**
 * The clicked item.
 * @type {tp.Component|null}
 */
tp.ToolBarItemClickEventArgs.prototype.Item = null;
/**
 * The clicked button.
 * @type {tp.Component|null}
 */
tp.ToolBarItemClickEventArgs.prototype.Button = null;

// ● control toolbar button
/**
 * A compact button for control toolbars used by controls and specialized dialogs.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 * - Click
 *
 * @implements {tp.ICommandProperty}
 */
tp.ControlToolButton = class extends tp.Component {
    // ● constructor
    /**
     * Creates a ControlToolButton instance.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.ControlToolButton.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.ControlToolButton";
        this.fClickHandler = this.FuncBind(this.HandleClick);
        tp.AddClass(this.Handle, tp.Classes.ControlToolButton);
        this.ApplyButtonParams(Params);
        this.ReadMarkupParams();
        this.UpdatePadding();
        this.Handle.addEventListener("click", this.fClickHandler);
    }

    // ● protected
    /**
     * Creates normalized control toolbar button create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        var Element;
        if (arguments.length > 1 && !tp.IsNil(Options)) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        Element = tp(Params.ElementOrSelector);
        if (!(Element instanceof HTMLElement))
            Params.ElementOrSelector = "div";
        return Params;
    }
    /**
     * Creates child elements after the handle is created.
     * @returns {void}
     */
    OnHandleCreated() {
        var TextNode;
        var Text;
        super.OnHandleCreated();
        TextNode = tp.FindTextNode(this.Handle);
        Text = TextNode ? TextNode.nodeValue || "" : "";
        if (TextNode)
            TextNode.nodeValue = "";
        this.fImageElement = this.Document.createElement("div");
        this.fTextElement = this.Document.createElement("div");
        this.Handle.appendChild(this.fImageElement);
        this.Handle.appendChild(this.fTextElement);
        this.fTextElement.textContent = Text.trim();
    }
    /**
     * Applies ControlToolButton-specific create params.
     * @param {tp.CreateParams|object|null|undefined} Params The create params.
     * @returns {void}
     */
    ApplyButtonParams(Params) {
        if (!Params)
            return;
        if (!tp.IsNil(Params.Command))
            this.Command = Params.Command;
        if (!tp.IsNil(Params.IcoClasses))
            this.IcoClasses = Params.IcoClasses;
        if (!tp.IsNil(Params.IcoChar))
            this.IcoChar = Params.IcoChar;
        if (!tp.IsNil(Params.ToolTip))
            this.ToolTip = Params.ToolTip;
        if (!tp.IsNil(Params.Text))
            this.Text = Params.Text;
    }
    /**
     * Reads button settings from data-* attributes.
     * @returns {void}
     */
    ReadMarkupParams() {
        var Value;
        if (tp.IsBlank(this.Command))
            this.Command = tp.Data(this.Handle, "command") || "";
        Value = tp.Data(this.Handle, "ico-classes");
        if (!tp.IsBlank(Value))
            this.IcoClasses = Value;
        Value = tp.Data(this.Handle, "tooltip") || this.Handle.getAttribute("title");
        if (!tp.IsBlank(Value))
            this.ToolTip = Value;
    }
    /**
     * Handles DOM click events.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleClick(e) {
        e.preventDefault();
        if (!this.Enabled)
            return;
        this.Trigger(tp.Events.Click, {
            e: e,
            el: e.target,
            Button: this,
            Command: this.Command
        });
        if (this.Parent instanceof tp.ControlToolBar)
            this.Parent.OnButtonClick(new tp.ToolBarItemClickEventArgs(this, this.Command));
    }
    /**
     * Updates text padding when both icon and text are visible.
     * @returns {void}
     */
    UpdatePadding() {
        var HasIcon = !tp.IsBlank(this.IcoClasses);
        var HasIconChar = !tp.IsBlank(this.IcoChar);
        var HasText = !tp.IsBlank(this.Text);
        if (this.fTextElement instanceof HTMLElement)
            this.fTextElement.style.paddingLeft = (HasIcon || HasIconChar) && HasText ? "4px" : "0";
    }

    // ● public
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.IsDisposed)
            return;
        if (this.HasHandle && this.fClickHandler)
            this.Handle.removeEventListener("click", this.fClickHandler);
        this.fClickHandler = null;
        this.fImageElement = null;
        this.fTextElement = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Gets or sets the visible button text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.fTextElement instanceof HTMLElement ? this.fTextElement.textContent : "";
    }
    /**
     * Gets or sets the visible button text.
     * @param {*} Value The text value.
     * @returns {void}
     */
    set Text(Value) {
        if (this.fTextElement instanceof HTMLElement) {
            this.fTextElement.textContent = tp.IsNil(Value) ? "" : String(Value);
            this.UpdatePadding();
        }
    }
    /**
     * Gets or sets the visible icon character.
     * @returns {string} Returns the icon character.
     */
    get IcoChar() {
        return this.fImageElement instanceof HTMLElement ? this.fImageElement.textContent : "";
    }
    /**
     * Gets or sets the visible icon character.
     * @param {*} Value The icon character.
     * @returns {void}
     */
    set IcoChar(Value) {
        if (this.fImageElement instanceof HTMLElement) {
            this.fImageElement.textContent = tp.IsNil(Value) ? "" : String(Value);
            this.UpdatePadding();
        }
    }
    /**
     * Gets or sets the icon CSS classes.
     * @returns {string} Returns the icon CSS classes.
     */
    get IcoClasses() {
        return this.fIcoClasses;
    }
    /**
     * Gets or sets the icon CSS classes.
     * @param {*} Value The icon CSS classes.
     * @returns {void}
     */
    set IcoClasses(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            tp.AddClasses(this.fImageElement, Value);
        }
        this.fIcoClasses = Value;
        this.UpdatePadding();
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.ControlToolButton.prototype.tpClass = "tp.ControlToolButton";
/**
 * Button command.
 * @type {string}
 */
tp.ControlToolButton.prototype.Command = "";
/**
 * A user-defined value.
 * @type {*}
 */
tp.ControlToolButton.prototype.Tag = null;
/**
 * Icon element.
 * @type {HTMLElement|null}
 */
tp.ControlToolButton.prototype.fImageElement = null;
/**
 * Text element.
 * @type {HTMLElement|null}
 */
tp.ControlToolButton.prototype.fTextElement = null;
/**
 * Icon CSS classes.
 * @type {string}
 */
tp.ControlToolButton.prototype.fIcoClasses = "";
/**
 * Click handler.
 * @type {Function|null}
 */
tp.ControlToolButton.prototype.fClickHandler = null;

// ● control toolbar
/**
 * A compact toolbar used by controls and specialized dialogs.
 *
 * Events:
 * - ButtonClick
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
tp.ControlToolBar = class extends tp.Component {
    // ● constructor
    /**
     * Creates a control toolbar.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.ControlToolBar.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.ControlToolBar";
        this.ButtonClass = tp.ControlToolButton;
        tp.AddClass(this.Handle, tp.Classes.ControlToolBar);
        this.CreateMarkupButtons();
    }

    // ● protected
    /**
     * Creates normalized control toolbar create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        var Element;
        if (arguments.length > 1 && !tp.IsNil(Options)) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        Element = tp(Params.ElementOrSelector);
        if (!(Element instanceof HTMLElement))
            Params.ElementOrSelector = "div";
        return Params;
    }
    /**
     * Creates internal toolbar elements.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        this.fRightAligner = this.Document.createElement("div");
        this.fRightAligner.className = tp.Classes.FlexFill;
        this.Handle.appendChild(this.fRightAligner);
    }
    /**
     * Converts direct div children to ControlToolButton components.
     * @returns {void}
     */
    CreateMarkupButtons() {
        var List = this.GetElementList();
        var Index;
        var Element;
        for (Index = 0; Index < List.length; Index++) {
            Element = List[Index];
            if (Element === this.fRightAligner)
                continue;
            if (Element instanceof HTMLDivElement && !(tp.GetComponent(Element) instanceof tp.ControlToolButton))
                new this.ButtonClass(Element);
        }
    }
    // ● public
    /**
     * Adds and returns a new control toolbar button.
     * @param {string} Command The button command.
     * @param {string|null|undefined} Text Tooltip fallback text for compact icon buttons.
     * @param {string|null|undefined} ToolTip The tooltip.
     * @param {string|null|undefined} IcoClasses The icon CSS classes.
     * @param {string|null|undefined} CssClasses Extra CSS classes.
     * @param {boolean|null|undefined} ToRight True to align to the right.
     * @returns {tp.ControlToolButton} Returns the new button.
     */
    AddButton(Command, Text, ToolTip, IcoClasses, CssClasses, ToRight) {
        var Button = new this.ButtonClass({
            Text: "",
            ToolTip: ToolTip || Text || "",
            Command: Command || "",
            IcoClasses: IcoClasses || ""
        });
        tp.AddClass(Button.Handle, tp.Classes.ToolButton);
        if (!tp.IsBlank(CssClasses))
            tp.AddClasses(Button.Handle, CssClasses);
        this.AddItem(Button, ToRight === true);
        return Button;
    }
    /**
     * Adds a component to the toolbar.
     * @param {tp.Component} Control The component to add.
     * @param {boolean|null|undefined} ToRight True to align to the right.
     * @returns {void}
     */
    AddItem(Control, ToRight) {
        if (!(Control instanceof tp.Component))
            return;
        if (ToRight === true)
            this.Handle.appendChild(Control.Handle);
        else
            this.Handle.insertBefore(Control.Handle, this.fRightAligner);
    }
    /**
     * Triggers the ButtonClick event.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    OnButtonClick(Args) {
        this.Trigger("ButtonClick", Args);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.ControlToolBar.prototype.tpClass = "tp.ControlToolBar";
/**
 * Right aligner element.
 * @type {HTMLElement|null}
 */
tp.ControlToolBar.prototype.fRightAligner = null;
/**
 * Button class.
 * @type {Function|null}
 */
tp.ControlToolBar.prototype.ButtonClass = null;

// ● button-ex
/**
 * A button control with icon and text, built on an anchor element.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 * - Click
 *
 * @implements {tp.ICommandProperty}
 * @example
 * <a id="ButtonEx1" data-command="Home" data-ico-classes="fa fa-home">Home</a>
 * <script>
 *     var Button = new tp.ButtonEx("#ButtonEx1");
 * </script>
 */
tp.ButtonEx = class extends tp.Component {
    // ● constructor
    /**
     * Creates a ButtonEx instance.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.ButtonEx.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.ButtonEx";
        this.fClickHandler = this.FuncBind(this.HandleClick);
        tp.AddClass(this.Handle, tp.Classes.ButtonEx);
        this.ApplyButtonParams(Params);
        this.ReadMarkupParams();
        this.ApplyIcoModeDefault();
        this.Handle.addEventListener("click", this.fClickHandler);
    }

    // ● protected
    /**
     * Creates normalized ButtonEx create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        var Element;
        if (arguments.length > 1 && !tp.IsNil(Options)) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        Element = tp(Params.ElementOrSelector);
        if (!(Element instanceof HTMLElement))
            Params.ElementOrSelector = "a";
        return Params;
    }
    /**
     * Creates child elements after the handle is created.
     * @returns {void}
     */
    OnHandleCreated() {
        var TextNode;
        var Text;
        super.OnHandleCreated();
        TextNode = tp.FindTextNode(this.Handle);
        Text = TextNode ? TextNode.nodeValue || "" : "";
        if (TextNode)
            TextNode.nodeValue = "";
        if (this.Handle instanceof HTMLAnchorElement && (tp.IsBlank(this.Handle.getAttribute("href")) || this.Handle.getAttribute("href") === "#"))
            this.Handle.href = "javascript:void(0);";
        this.fImageElement = this.Document.createElement("div");
        this.fTextElement = this.Document.createElement("div");
        this.Handle.appendChild(this.fImageElement);
        this.Handle.appendChild(this.fTextElement);
        this.fTextElement.textContent = Text.trim();
    }
    /**
     * Applies ButtonEx-specific create params.
     * @param {tp.CreateParams|object} Params The create params.
     * @returns {void}
     */
    ApplyButtonParams(Params) {
        if (!Params)
            return;
        if (!tp.IsNil(Params.Command))
            this.Command = Params.Command;
        if (!tp.IsNil(Params.Url))
            this.Url = Params.Url;
        if (!tp.IsNil(Params.IcoClasses))
            this.IcoClasses = Params.IcoClasses;
        if (!tp.IsNil(Params.ImageUrl))
            this.ImageUrl = Params.ImageUrl;
        if (!tp.IsNil(Params.IcoMode))
            this.IcoMode = Params.IcoMode;
        if (!tp.IsNil(Params.Ico))
            this.ApplyIcoParam(Params.Ico);
        if (!tp.IsNil(Params.NoText))
            this.NoText = Params.NoText;
    }
    /**
     * Reads ButtonEx-specific settings from data-* attributes.
     * @returns {void}
     */
    ReadMarkupParams() {
        var Value;
        if (tp.IsBlank(this.Command))
            this.Command = tp.Data(this.Handle, "command") || "";
        Value = tp.Data(this.Handle, "url");
        if (!tp.IsBlank(Value))
            this.Url = Value;
        Value = tp.Data(this.Handle, "ico-classes");
        if (!tp.IsBlank(Value))
            this.IcoClasses = Value;
        Value = tp.Data(this.Handle, "image-url");
        if (!tp.IsBlank(Value))
            this.ImageUrl = Value;
        Value = tp.Data(this.Handle, "ico");
        if (!tp.IsBlank(Value))
            this.ApplyIcoParam(Value);
        Value = tp.Data(this.Handle, "no-text");
        if (!tp.IsBlank(Value))
            this.NoText = tp.IsSameText(Value, "true") || tp.IsSameText(Value, "1");
    }
    /**
     * Applies a legacy Ico parameter value.
     * @param {string|number|null|undefined} Value The icon mode value.
     * @returns {void}
     */
    ApplyIcoParam(Value) {
        if (tp.IsNumber(Value)) {
            this.IcoMode = Value;
        } else if (tp.IsSameText(Value, "Top")) {
            this.IcoMode = tp.ButtonExIcoMode.Top;
        } else if (tp.IsSameText(Value, "No") || tp.IsSameText(Value, "NoIco") || tp.IsSameText(Value, "None")) {
            this.IcoMode = tp.ButtonExIcoMode.None;
        } else if (tp.IsSameText(Value, "Left")) {
            this.IcoMode = tp.ButtonExIcoMode.Left;
        }
    }
    /**
     * Applies the default icon mode based on icon values.
     * @returns {void}
     */
    ApplyIcoModeDefault() {
        if (tp.IsBlank(this.IcoClasses) && tp.IsBlank(this.ImageUrl))
            this.IcoMode = tp.ButtonExIcoMode.None;
    }
    /**
     * Handles DOM click events.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleClick(e) {
        var Args;
        e.preventDefault();
        if (!this.Enabled)
            return;
        Args = this.Trigger(tp.Events.Click, {
            e: e,
            el: e.target,
            Button: this,
            Command: this.Command
        });
        if (this.Parent instanceof tp.ToolBar)
            this.Parent.OnButtonClick(new tp.ToolBarItemClickEventArgs(this, Args && !tp.IsBlank(Args.Command) ? Args.Command : this.Command));
    }

    // ● public
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.IsDisposed)
            return;
        if (this.HasHandle && this.fClickHandler)
            this.Handle.removeEventListener("click", this.fClickHandler);
        this.fClickHandler = null;
        this.fImageElement = null;
        this.fTextElement = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Gets or sets the visible button text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.fTextElement instanceof HTMLElement ? this.fTextElement.textContent : "";
    }
    /**
     * Gets or sets the visible button text.
     * @param {string} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        if (this.fTextElement instanceof HTMLElement)
            this.fTextElement.textContent = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the anchor href.
     * @returns {string} Returns the URL.
     */
    get Url() {
        if (this.Handle instanceof HTMLAnchorElement)
            return this.Handle.href !== "javascript:void(0);" ? this.Handle.href : "";
        return "";
    }
    /**
     * Gets or sets the anchor href.
     * @param {string} Value The URL.
     * @returns {void}
     */
    set Url(Value) {
        if (this.Handle instanceof HTMLAnchorElement)
            this.Handle.href = tp.IsBlank(Value) ? "javascript:void(0);" : String(Value);
    }
    /**
     * Gets or sets the icon CSS classes.
     * @returns {string} Returns the icon CSS classes.
     */
    get IcoClasses() {
        return this.fIcoClasses;
    }
    /**
     * Gets or sets the icon CSS classes.
     * @param {string} Value The icon CSS classes.
     * @returns {void}
     */
    set IcoClasses(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            this.fImageElement.style.background = "";
            this.fImageUrl = "";
            tp.AddClasses(this.fImageElement, Value);
        }
        this.fIcoClasses = Value;
    }
    /**
     * Gets or sets the icon image URL.
     * @returns {string} Returns the image URL.
     */
    get ImageUrl() {
        return this.fImageUrl;
    }
    /**
     * Gets or sets the icon image URL.
     * @param {string} Value The image URL.
     * @returns {void}
     */
    set ImageUrl(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            this.fImageElement.style.background = "";
            this.fIcoClasses = "";
            this.fImageElement.style.backgroundImage = tp.IsBlank(Value) ? "" : "url(\"" + Value + "\")";
            this.fImageElement.style.backgroundRepeat = "no-repeat";
            this.fImageElement.style.backgroundPosition = "center center";
            this.fImageElement.style.backgroundSize = "90%";
        }
        this.fImageUrl = Value;
    }
    /**
     * Gets or sets the icon mode.
     * @returns {number} Returns a tp.ButtonExIcoMode value.
     */
    get IcoMode() {
        return this.fIcoMode;
    }
    /**
     * Gets or sets the icon mode.
     * @param {number} Value A tp.ButtonExIcoMode value.
     * @returns {void}
     */
    set IcoMode(Value) {
        this.fIcoMode = tp.IsNumber(Value) ? Value : tp.ButtonExIcoMode.Left;
        tp.RemoveClasses(this.Handle, tp.Classes.NoIco, tp.Classes.IcoTop);
        if (this.fIcoMode === tp.ButtonExIcoMode.None)
            tp.AddClass(this.Handle, tp.Classes.NoIco);
        else if (this.fIcoMode === tp.ButtonExIcoMode.Top)
            tp.AddClass(this.Handle, tp.Classes.IcoTop);
    }
    /**
     * Gets or sets whether text is hidden.
     * @returns {boolean} Returns true when text is hidden.
     */
    get NoText() {
        return this.fNoText === true;
    }
    /**
     * Gets or sets whether text is hidden.
     * @param {boolean} Value True to hide the text.
     * @returns {void}
     */
    set NoText(Value) {
        this.fNoText = Value === true;
        tp.RemoveClass(this.Handle, tp.Classes.NoText);
        if (this.fNoText)
            tp.AddClass(this.Handle, tp.Classes.NoText);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.ButtonEx.prototype.tpClass = "tp.ButtonEx";
/**
 * Button command.
 * @type {string}
 */
tp.ButtonEx.prototype.Command = "";
/**
 * A user-defined value.
 * @type {*}
 */
tp.ButtonEx.prototype.Tag = null;
/**
 * Icon element.
 * @type {HTMLElement|null}
 */
tp.ButtonEx.prototype.fImageElement = null;
/**
 * Text element.
 * @type {HTMLElement|null}
 */
tp.ButtonEx.prototype.fTextElement = null;
/**
 * Icon mode.
 * @type {number}
 */
tp.ButtonEx.prototype.fIcoMode = tp.ButtonExIcoMode.Left;
/**
 * True when text is hidden.
 * @type {boolean}
 */
tp.ButtonEx.prototype.fNoText = false;
/**
 * Icon CSS classes.
 * @type {string}
 */
tp.ButtonEx.prototype.fIcoClasses = "";
/**
 * Icon image URL.
 * @type {string}
 */
tp.ButtonEx.prototype.fImageUrl = "";
/**
 * Click handler.
 * @type {Function|null}
 */
tp.ButtonEx.prototype.fClickHandler = null;

// ● toolbar
/**
 * A toolbar containing ButtonEx items.
 *
 * Events:
 * - ButtonClick
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
tp.ToolBar = class extends tp.Component {
    // ● constructor
    /**
     * Creates a toolbar.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.ToolBar.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.ToolBar";
        tp.AddClass(this.Handle, tp.Classes.ToolBar);
        this.CreateMarkupButtons();
    }

    // ● protected
    /**
     * Creates normalized toolbar create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        var Element;
        if (arguments.length > 1 && !tp.IsNil(Options)) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        Element = tp(Params.ElementOrSelector);
        if (!(Element instanceof HTMLElement))
            Params.ElementOrSelector = "div";
        return Params;
    }
    /**
     * Creates internal toolbar elements.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        this.fRightAligner = this.Document.createElement("div");
        this.fRightAligner.className = tp.Classes.FlexFill;
        this.Handle.appendChild(this.fRightAligner);
    }
    /**
     * Converts direct anchor children to ButtonEx components.
     * @returns {void}
     */
    CreateMarkupButtons() {
        var List = this.GetElementList();
        var Index;
        var Element;
        var Button;
        for (Index = 0; Index < List.length; Index++) {
            Element = List[Index];
            if (Element === this.fRightAligner)
                continue;
            if (Element instanceof HTMLAnchorElement && !(tp.GetComponent(Element) instanceof tp.ButtonEx)) {
                Button = new tp.ButtonEx(Element);
                tp.AddClass(Button.Handle, tp.Classes.ToolButton);
                Button.NoText = this.NoText;
                Button.IcoMode = this.IcoMode;
            }
        }
    }

    // ● public
    /**
     * Adds and returns a new ButtonEx to the toolbar.
     * @param {string} Command The button command.
     * @param {string} Text The button text.
     * @param {string|null|undefined} ToolTip The tooltip.
     * @param {string|null|undefined} IcoClasses The icon CSS classes.
     * @param {string|null|undefined} CssClasses Extra CSS classes.
     * @param {boolean|null|undefined} ToRight True to align to the right.
     * @returns {tp.ButtonEx} Returns the new button.
     */
    AddButton(Command, Text, ToolTip, IcoClasses, CssClasses, ToRight) {
        var Button = new tp.ButtonEx({
            Text: Text || "",
            ToolTip: ToolTip || "",
            Command: Command || "",
            IcoClasses: IcoClasses || ""
        });
        tp.AddClass(Button.Handle, tp.Classes.ToolButton);
        Button.NoText = this.NoText;
        Button.IcoMode = this.IcoMode;
        if (!tp.IsBlank(CssClasses))
            tp.AddClasses(Button.Handle, CssClasses);
        this.AddItem(Button, ToRight === true);
        return Button;
    }
    /**
     * Adds a component to the toolbar.
     * @param {tp.Component} Control The component to add.
     * @param {boolean|null|undefined} ToRight True to align to the right.
     * @returns {void}
     */
    AddItem(Control, ToRight) {
        if (!(Control instanceof tp.Component))
            return;
        if (ToRight === true)
            this.Handle.appendChild(Control.Handle);
        else
            this.Handle.insertBefore(Control.Handle, this.fRightAligner);
    }
    /**
     * Sets the icon mode to all ButtonEx children.
     * @param {number} Value The tp.ButtonExIcoMode value.
     * @returns {void}
     */
    SetIcoMode(Value) {
        this.IcoMode = Value;
        Value = this.IcoMode;
        this.GetAllComponents().forEach(function (Item) {
            if (Item instanceof tp.ButtonEx)
                Item.IcoMode = Value;
        });
    }
    /**
     * Sets the NoText flag to all ButtonEx children.
     * @param {boolean} Value True to hide text.
     * @returns {void}
     */
    SetNoText(Value) {
        this.NoText = Value === true;
        Value = this.NoText;
        this.GetAllComponents().forEach(function (Item) {
            if (Item instanceof tp.ButtonEx)
                Item.NoText = Value;
        });
    }
    /**
     * Gets or sets the default icon mode for toolbar buttons.
     * @returns {number} Returns a tp.ButtonExIcoMode value.
     */
    get IcoMode() {
        return this.fIcoMode;
    }
    /**
     * Gets or sets the default icon mode for toolbar buttons.
     * @param {number} Value A tp.ButtonExIcoMode value.
     * @returns {void}
     */
    set IcoMode(Value) {
        this.fIcoMode = tp.IsNumber(Value) ? Value : tp.ButtonExIcoMode.Top;
    }
    /**
     * Gets or sets the default no-text flag for toolbar buttons.
     * @returns {boolean} Returns true when toolbar buttons hide text.
     */
    get NoText() {
        return this.fNoText === true;
    }
    /**
     * Gets or sets the default no-text flag for toolbar buttons.
     * @param {boolean} Value True to hide text.
     * @returns {void}
     */
    set NoText(Value) {
        this.fNoText = Value === true;
    }
    /**
     * Finds a child item by command.
     * @param {string} Command The command to find.
     * @returns {tp.ButtonEx|tp.Component|null} Returns the found item or null.
     */
    FindItemByCommand(Command) {
        var List = this.GetAllComponents();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.HasCommandProperty(List[Index]) && tp.IsSameText(List[Index].Command, Command))
                return List[Index];
        }
        return null;
    }
    /**
     * Triggers the ButtonClick event.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    OnButtonClick(Args) {
        this.Trigger("ButtonClick", Args);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.ToolBar.prototype.tpClass = "tp.ToolBar";
/**
 * Right aligner element.
 * @type {HTMLElement|null}
 */
tp.ToolBar.prototype.fRightAligner = null;
/**
 * Default icon mode for toolbar buttons.
 * @type {number}
 */
tp.ToolBar.prototype.fIcoMode = tp.ButtonExIcoMode.Top;
/**
 * True when toolbar buttons hide text by default.
 * @type {boolean}
 */
tp.ToolBar.prototype.fNoText = true;

tp.Ui.RegisterType(["ControlToolButton", "tp-ControlToolButton"], tp.ControlToolButton);
tp.Ui.RegisterType(["ControlToolBar", "tp-ControlToolBar"], tp.ControlToolBar);
tp.Ui.RegisterType(["ButtonEx", "tp-ButtonEx"], tp.ButtonEx);
tp.Ui.RegisterType(["ToolBar", "tp-ToolBar"], tp.ToolBar);
