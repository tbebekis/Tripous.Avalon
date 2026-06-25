// ● command property
/**
 * Interface-like base class for objects that provide a Command string property.
 */
tp.ICommandProperty = class {
    // ● constructor
    /**
     * Creates a command-property contract instance.
     */
    constructor() {
    }
};
/**
 * Returns true when a value provides a Command property.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value provides a Command property.
 */
tp.HasCommandProperty = function (Value) {
    return Value instanceof tp.Object && "Command" in Value;
};
/**
 * Returns a command string from a value, if any.
 * @param {HTMLElement|Event|string|tp.EventArgs|object|null|undefined} Value The value to inspect.
 * @returns {string} Returns the command string or an empty string.
 */
tp.GetCommand = function (Value) {
    var Args = null;
    var EventValue = null;
    var Element = null;
    var Component = null;
    if (Value instanceof tp.EventArgs) {
        Args = Value;
        EventValue = Args.e;
    } else if (typeof Event !== "undefined" && Value instanceof Event) {
        EventValue = Value;
    } else if (Value instanceof HTMLElement) {
        Element = Value;
    } else if (tp.IsString(Value)) {
        Element = tp.Select(Value);
    }
    if (EventValue instanceof Event && EventValue.target instanceof HTMLElement)
        Element = EventValue.target;
    if (Args) {
        if (!tp.IsBlank(Args.Command))
            return Args.Command;
        if (tp.HasCommandProperty(Args.Sender) && !tp.IsBlank(Args.Sender.Command))
            return Args.Sender.Command;
        if (tp.HasCommandProperty(Args.Button) && !tp.IsBlank(Args.Button.Command))
            return Args.Button.Command;
        if (tp.HasCommandProperty(Args.MenuItem) && !tp.IsBlank(Args.MenuItem.Command))
            return Args.MenuItem.Command;
    }
    while (Element instanceof HTMLElement) {
        if (!tp.IsBlank(tp.Data(Element, "command")))
            return tp.Data(Element, "command");
        Component = tp.Component.GetComponent(Element);
        if (tp.HasCommandProperty(Component) && !tp.IsBlank(Component.Command))
            return Component.Command;
        Element = Element.parentElement;
    }
    return "";
};

// ● button
/**
 * A button control built on a button element.
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
 * <button id="Button1" data-command="TEST_COMMAND">Button1</button>
 * <script>
 *     var Button = new tp.Button("#Button1");
 * </script>
 */
tp.Button = class extends tp.Component {
    // ● constructor
    /**
     * Creates a button.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The button create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = arguments.length > 1 ? tp.Button.CreateParams(CreateParams, Options) : tp.Button.CreateParams(CreateParams);
        super(Params);
        this.tpClass = "tp.Button";
        this.fClickHandler = this.FuncBind(this.HandleClick);
        tp.AddClass(this.Handle, tp.Classes.Button);
        this.Handle.type = "button";
        if (!tp.IsNil(Params.Command))
            this.Command = Params.Command;
        this.ReadCommand();
        this.Handle.addEventListener("click", this.fClickHandler);
    }

    // ● protected
    /**
     * Creates normalized button create parameters.
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
            Params.ElementOrSelector = "button";
        return Params;
    }
    /**
     * Reads the command from data-command, when the property is empty.
     * @returns {void}
     */
    ReadCommand() {
        if (tp.IsBlank(this.Command))
            this.Command = tp.Data(this.Handle, "command") || "";
    }

    // ● event handler
    /**
     * Handles DOM click events.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleClick(e) {
        this.Trigger(tp.Events.Click, {
            e: e,
            el: e.target,
            Button: this,
            Command: this.Command
        });
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
        super.Dispose();
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Button.prototype.tpClass = "tp.Button";
/**
 * Button command.
 * @type {string}
 */
tp.Button.prototype.Command = "";
/**
 * A user-defined value.
 * @type {*}
 */
tp.Button.prototype.Tag = null;
/**
 * Click handler.
 * @type {Function|null}
 */
tp.Button.prototype.fClickHandler = null;
