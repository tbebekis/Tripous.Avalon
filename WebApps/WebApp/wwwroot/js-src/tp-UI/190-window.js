// ● window settings
/**
 * Default visual settings for tp.Window.
 * @type {object}
 */
tp.WindowSettings = {
    BackColor: "white",
    Border: "1px solid #CCC",
    CaptionHeight: "30px",
    TextPadding: "2px 4px"
};

// ● dialog result
/**
 * Modal window result values.
 * @enum {number}
 */
tp.DialogResult = {
    None: 0,
    OK: 1,
    Cancel: 2,
    Abort: 3,
    Retry: 4,
    Ignore: 5,
    Yes: 6,
    No: 7
};
Object.freeze(tp.DialogResult);

// ● window args
/**
 * Arguments for the tp.Window constructor.
 */
tp.WindowArgs = class {
    // ● constructor
    /**
     * Creates a window arguments object.
     * @param {object|null|undefined} Source The optional source arguments to copy from.
     */
    constructor(Source) {
        var Defaults = tp.WindowArgs.prototype;
        var Name;
        for (Name in Defaults) {
            if (Object.prototype.hasOwnProperty.call(Defaults, Name))
                this[Name] = Defaults[Name];
        }
        if (tp.IsObject(Source))
            tp.Assign(this, Source);
    }

    // ● properties
    /**
     * Gets the dialog result after a modal dialog closes.
     * @returns {number} Returns a tp.DialogResult value.
     */
    get DialogResult() {
        return this.Window instanceof tp.Window ? this.Window.DialogResult : tp.DialogResult.None;
    }
};

// ● prototype
/**
 * Window initial left position. Ignored with small screens.
 * @type {number}
 */
tp.WindowArgs.prototype.X = 100;
/**
 * Window initial top position. Ignored with small screens.
 * @type {number}
 */
tp.WindowArgs.prototype.Y = 200;
/**
 * Window initial width. Ignored with small screens.
 * @type {number|string}
 */
tp.WindowArgs.prototype.Width = 800;
/**
 * Window initial height. Ignored with small screens.
 * @type {number|string}
 */
tp.WindowArgs.prototype.Height = 500;
/**
 * True initially centers the window in the viewport.
 * @type {boolean}
 */
tp.WindowArgs.prototype.CenterScreen = true;
/**
 * Window caption text.
 * @type {string}
 */
tp.WindowArgs.prototype.Text = "Window";
/**
 * True makes the window header visible.
 * @type {boolean}
 */
tp.WindowArgs.prototype.ShowHeader = true;
/**
 * True makes the window footer visible.
 * @type {boolean}
 */
tp.WindowArgs.prototype.ShowFooter = true;
/**
 * True shows the close button in the upper right corner.
 * @type {boolean}
 */
tp.WindowArgs.prototype.CloseBox = true;
/**
 * Edges used as resize handlers. Set to tp.Edge.None for a non-resizable window.
 * @type {number}
 */
tp.WindowArgs.prototype.ResizeEdges = tp.Edge.All;
/**
 * True indicates a movable window.
 * @type {boolean}
 */
tp.WindowArgs.prototype.Movable = true;
/**
 * Callback context for ShowFunc and CloseFunc.
 * @type {object|null}
 */
tp.WindowArgs.prototype.Creator = null;
/**
 * Callback called after the window is shown. Signature: function(Window: tp.Window): void.
 * @type {Function|null}
 */
tp.WindowArgs.prototype.ShowFunc = null;
/**
 * Callback called when the window is about to close. Signature: function(Window: tp.Window): void.
 * @type {Function|null}
 */
tp.WindowArgs.prototype.CloseFunc = null;
/**
 * The created window.
 * @type {tp.Window|null}
 */
tp.WindowArgs.prototype.Window = null;
/**
 * Element or selector with html content. An element that becomes the content of the window.
 * @type {string|HTMLElement|null}
 */
tp.WindowArgs.prototype.Content = null;
/**
 * Modal indication flag for inheritors. Use ShowModal() for modal behavior.
 * @type {boolean}
 */
tp.WindowArgs.prototype.AsModal = false;
/**
 * Dialog result returned when the close button is clicked or Escape is pressed.
 * @type {number}
 */
tp.WindowArgs.prototype.DefaultDialogResult = tp.DialogResult.Cancel;

// ● window
/**
 * The base class for windows and dialog boxes.
 *
 * Events:
 * - Showing
 * - Shown
 * - Closing
 * - Closed
 * - ContentResized
 */
tp.Window = class extends tp.Component {
    // ● constructor
    /**
     * Creates a window.
     * @param {tp.WindowArgs|object|null|undefined} Args The window arguments.
     */
    constructor(Args) {
        Args = new tp.WindowArgs(Args);
        Args.ElementOrSelector = Args.ElementOrSelector || "div";
        Args.Id = Args.Id || tp.SafeId(tp.Classes.Window);
        Args.CssClasses = tp.IsBlank(Args.CssClasses) ? tp.Classes.Window : Args.CssClasses + " " + tp.Classes.Window;
        super(Args);
        this.tpClass = "tp.Window";
        this.Args.Window = this;
        this.ProcessInitInfo();
        tp.Window.Windows.push(this);
    }

    // ● protected
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        this.CreateOverlay();
        this.IsElementResizeListener = true;
        this.Handle.tabIndex = -1;
        tp.Data(this.Handle, "tpClass", this.tpClass);
        this.fKeyDownHandler = this.FuncBind(this.HandleKeyDown);
        this.Handle.addEventListener("keydown", this.fKeyDownHandler);
    }
    /**
     * Disposes internal resources.
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fKeyDownHandler)
            this.Handle.removeEventListener("keydown", this.fKeyDownHandler);
        if (this.HeaderText && this.fWindowClickHandler)
            this.HeaderText.removeEventListener("dblclick", this.fWindowClickHandler);
        if (this.fDragger) {
            this.fDragger.Dispose();
            this.fDragger = null;
        }
        this.DisposeOverlay();
        super.DoDispose();
    }
    /**
     * Sets a style dimension value.
     * @param {string} Name The style property name.
     * @param {number|string} Value The value to set.
     * @returns {void}
     */
    SetDimension(Name, Value) {
        if (this.Handle)
            this.Handle.style[Name] = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
    /**
     * Creates the modal overlay.
     * @returns {void}
     */
    CreateOverlay() {
        this.fOverlay = new tp.ScreenOverlay();
        this.ZIndex = this.fOverlay.ZIndex + 1;
        this.fOverlay.Visible = false;
        this.fOverlay.Handle.appendChild(this.Handle);
    }
    /**
     * Disposes the modal overlay.
     * @returns {void}
     */
    DisposeOverlay() {
        if (this.fOverlay) {
            this.fOverlay.Dispose();
            this.fOverlay = null;
        }
    }
    /**
     * Creates a header button.
     * @param {string} Command The command.
     * @returns {HTMLSpanElement} Returns the button element.
     */
    AddHeaderButton(Command) {
        var Text = "\u00D7";
        var Result = this.Document.createElement("span");
        if (Command === "Maximize")
            Text = "\u25A1";
        else if (Command === "Restore")
            Text = "\u2750";
        Result.className = tp.Classes.WindowCaptionButton;
        Result.textContent = Text;
        Result.title = Command;
        tp.Data(Result, "command", Command);
        Result.addEventListener("click", this.fWindowClickHandler);
        this.HeaderButtonBar.appendChild(Result);
        return Result;
    }
    /**
     * Creates window controls.
     * @returns {void}
     */
    CreateControls() {
        this.fWindowClickHandler = this.FuncBind(this.WindowAnyClick);
        this.fMaximizeButton = null;
        this.fRestoreButton = null;
        this.fDragger = null;

        this.Header = this.Document.createElement("div");
        this.Header.id = tp.SafeId(tp.Classes.WindowCaption);
        this.Header.className = tp.Classes.WindowCaption;
        this.Handle.appendChild(this.Header);

        this.HeaderText = this.Document.createElement("span");
        this.HeaderText.className = tp.Classes.WindowCaptionText;
        this.HeaderText.innerHTML = this.Args.Text;
        this.HeaderText.addEventListener("dblclick", this.fWindowClickHandler);
        this.Header.appendChild(this.HeaderText);

        this.HeaderButtonBar = this.Document.createElement("div");
        this.HeaderButtonBar.id = tp.SafeId(tp.Classes.WindowCaptionButtonBar);
        this.HeaderButtonBar.className = tp.Classes.WindowCaptionButtonBar;
        this.Header.appendChild(this.HeaderButtonBar);

        if (!tp.Viewport.IsXSmall && this.Args.ResizeEdges !== tp.Edge.None) {
            this.fMaximizeButton = this.AddHeaderButton("Maximize");
            this.fRestoreButton = this.AddHeaderButton("Restore");
            this.fRestoreButton.style.display = "none";
        }
        if (this.Args.CloseBox === true)
            this.fCloseButton = this.AddHeaderButton("Close");
        if (this.Args.ShowHeader === false)
            this.Header.style.display = "none";

        this.ContentWrapper = new tp.Component({
            ElementOrSelector: "div",
            Parent: this,
            Id: tp.SafeId(tp.Classes.WindowContentContainer),
            CssClasses: tp.Classes.WindowContentContainer,
            IsElementResizeListener: true
        });
        this.ContentWrapper.On("ElementSizeChanged", this.ContentResized, this);

        this.Footer = this.Document.createElement("div");
        this.Footer.id = tp.SafeId(tp.Classes.WindowFooter);
        this.Footer.className = tp.Classes.WindowFooter;
        this.Handle.appendChild(this.Footer);

        this.fFooterFill = this.Document.createElement("div");
        this.fFooterFill.className = tp.Classes.FlexFill;
        this.Footer.appendChild(this.fFooterFill);
        if (this.Args.ShowFooter === false)
            this.Footer.style.display = "none";
    }
    /**
     * Sets up dragging and resizing.
     * @returns {void}
     */
    SetupDragger() {
        var DragHandle;
        var DragOnly;
        var Mode;
        if (!tp.Viewport.IsXSmall && (this.Args.ResizeEdges !== tp.Edge.None || this.Args.Movable === true)) {
            DragHandle = this.Args.Movable === true ? this.HeaderText : null;
            DragOnly = !tp.IsNil(DragHandle) && this.Args.ResizeEdges === tp.Edge.None;
            Mode = DragOnly ? tp.DraggerMode.Drag : tp.DraggerMode.Both;
            this.fDragger = new tp.Dragger(Mode, this.Handle, DragHandle);
            this.fDragger.Edges = this.Args.ResizeEdges;
            this.fDragger.MinHeight = 100;
            this.fDragger.MinWidth = 100;
        }
    }
    /**
     * Sets initial position and size.
     * @returns {void}
     */
    SetupPositionAndSize() {
        if (tp.Viewport.IsXSmall) {
            this.X = 1;
            this.Y = 1;
            this.Width = this.Document.documentElement.clientWidth - 3;
            this.Height = this.Document.documentElement.clientHeight - 3;
        } else {
            this.X = this.Args.X;
            this.Y = this.Args.Y;
            this.Width = this.Args.Width;
            this.Height = this.Args.Height;
            if (this.Args.CenterScreen === true)
                this.CenterInScreen();
        }
    }
    /**
     * Handles Escape.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Escape))
            this.EscapePressed(e);
    }

    // ● overridables
    /**
     * Processes custom initialization info.
     * @returns {void}
     */
    ProcessInitInfo() {
    }
    /**
     * Passes result values back to caller code.
     * @returns {void}
     */
    PassBackResult() {
    }
    /**
     * Moves item values to controls.
     * @returns {void}
     */
    ItemToControls() {
    }
    /**
     * Moves control values to the item.
     * @returns {void}
     */
    ControlsToItem() {
    }
    /**
     * Returns true when the current result is valid.
     * @returns {boolean} Returns true.
     */
    IsValidResult() {
        return true;
    }
    /**
     * Returns true when a dialog result is a positive result.
     * @param {number} DialogResult The dialog result.
     * @returns {boolean} Returns true for OK or Yes.
     */
    IsValidDialogResult(DialogResult) {
        return DialogResult === tp.DialogResult.OK || DialogResult === tp.DialogResult.Yes;
    }

    // ● properties
    /**
     * Gets the window arguments.
     * @returns {tp.WindowArgs} Returns the arguments.
     */
    get Args() {
        return this.CreateParams;
    }
    /**
     * Gets or sets visibility.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle ? this.Handle.style.display === "flex" : false;
    }
    /**
     * Gets or sets visibility.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        var OldValue = this.Visible;
        if (this.Handle)
            this.Handle.style.display = Value === true ? "flex" : "none";
        if (OldValue !== this.Visible)
            this.OnVisibleChanged();
    }
    /**
     * Gets or sets the title text.
     * @returns {string} Returns the title.
     */
    get Text() {
        return this.HeaderText ? this.HeaderText.innerHTML : "";
    }
    /**
     * Gets or sets the title text.
     * @param {string} Value The title.
     * @returns {void}
     */
    set Text(Value) {
        if (this.HeaderText)
            this.HeaderText.innerHTML = Value;
    }
    /**
     * Returns true when this window is modal.
     * @returns {boolean} Returns true when modal.
     */
    get Modal() {
        return this.fModal;
    }
    /**
     * Returns true when maximized.
     * @returns {boolean} Returns true when maximized.
     */
    get IsMaximized() {
        return this.fMaximized;
    }
    /**
     * Gets or sets the modal dialog result.
     * @returns {number} Returns a tp.DialogResult value.
     */
    get DialogResult() {
        return this.fDialogResult;
    }
    /**
     * Gets or sets the modal dialog result.
     * @param {number} Value A tp.DialogResult value.
     * @returns {void}
     */
    set DialogResult(Value) {
        if (this.Modal === true && tp.IsNumber(Value) && Value !== this.DialogResult && Value !== tp.DialogResult.None) {
            if (this.IsValidDialogResult(Value))
                this.ControlsToItem();
            this.fDialogResult = Value;
            this.Close();
        }
    }
    /**
     * Gets left position.
     * @returns {number} Returns left position.
     */
    get X() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().left : 0;
    }
    /**
     * Sets left position.
     * @param {number|string} Value The left position.
     * @returns {void}
     */
    set X(Value) {
        if (this.Handle)
            this.Handle.style.left = tp.px(Value);
    }
    /**
     * Gets top position.
     * @returns {number} Returns top position.
     */
    get Y() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().top : 0;
    }
    /**
     * Sets top position.
     * @param {number|string} Value The top position.
     * @returns {void}
     */
    set Y(Value) {
        if (this.Handle)
            this.Handle.style.top = tp.px(Value);
    }
    /**
     * Gets width.
     * @returns {number} Returns width.
     */
    get Width() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().width : 0;
    }
    /**
     * Sets width.
     * @param {number|string} Value The width.
     * @returns {void}
     */
    set Width(Value) {
        this.SetDimension("width", Value);
    }
    /**
     * Gets height.
     * @returns {number} Returns height.
     */
    get Height() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().height : 0;
    }
    /**
     * Sets height.
     * @param {number|string} Value The height.
     * @returns {void}
     */
    set Height(Value) {
        this.SetDimension("height", Value);
    }
    /**
     * Gets z-index.
     * @returns {number} Returns z-index.
     */
    get ZIndex() {
        return tp.ZIndex(this.Handle);
    }
    /**
     * Sets z-index.
     * @param {number} Value The z-index.
     * @returns {void}
     */
    set ZIndex(Value) {
        tp.ZIndex(this.Handle, Value);
    }

    // ● public
    /**
     * Brings this window to front.
     * @returns {number} Returns the assigned z-index.
     */
    BringToFront() {
        var Result = this.ZIndex;
        var Index;
        var Value;
        for (Index = 0; Index < tp.Window.Windows.length; Index++) {
            if (tp.Window.Windows[Index] !== this && !tp.Window.Windows[Index].Modal) {
                Value = tp.Window.Windows[Index].ZIndex;
                Result = Math.max(Result, Value + 1);
            }
        }
        this.ZIndex = Result;
        return Result;
    }
    /**
     * Centers the window in the viewport.
     * @returns {void}
     */
    CenterInScreen() {
        var ViewportSize = tp.Viewport.GetSize();
        var Width = this.Handle.offsetWidth;
        var Height = this.Handle.offsetHeight;
        this.X = Math.round((ViewportSize.Width - Width) / 2);
        this.Y = Math.round((ViewportSize.Height - Height) / 2);
    }
    /**
     * Creates and appends a footer button.
     * @param {string} Command The command.
     * @param {string} Title The button title.
     * @param {number} DialogResult Optional dialog result.
     * @param {boolean} ToLeft True to place the button at the left side.
     * @returns {tp.Button} Returns the button.
     */
    CreateFooterButton(Command, Title, DialogResult = tp.DialogResult.None, ToLeft = false) {
        var Button = new tp.Button({
            ElementOrSelector: "button",
            Text: Title,
            Id: tp.SafeId("tp-Window-FooterButton"),
            CssClasses: tp.Classes.Button
        });
        Button.Command = Command;
        Button.DialogResult = DialogResult;
        Button.On(tp.Events.Click, this.AnyClick, this);
        if (ToLeft === true)
            this.Footer.insertBefore(Button.Handle, this.Footer.firstChild);
        else
            this.Footer.appendChild(Button.Handle);
        return Button;
    }
    /**
     * Creates the window content component.
     * @returns {tp.Component} Returns the content component.
     */
    CreateContentElement() {
        if (!(this.Content instanceof tp.Component)) {
            this.Content = new tp.Component({
                ElementOrSelector: "div",
                Parent: this.ContentWrapper,
                Id: tp.SafeId(tp.Classes.WindowContent),
                CssClasses: tp.Classes.WindowContent
            });
        }
        return this.Content;
    }
    /**
     * Maximizes the window.
     * @returns {void}
     */
    Maximize() {
        if (!tp.Viewport.IsXSmall && !this.IsMaximized) {
            this.fLastRect = tp.Rect.FromClientRect(this.Handle);
            this.X = 1;
            this.Y = 1;
            this.Width = this.Document.documentElement.clientWidth - 3;
            this.Height = tp.Viewport.Height - 3;
            if (this.fMaximizeButton) {
                this.fMaximizeButton.style.display = "none";
                this.fRestoreButton.style.display = "";
            }
            if (this.fDragger)
                this.fDragger.Active = false;
            this.fMaximized = true;
        }
    }
    /**
     * Restores the window.
     * @returns {void}
     */
    Restore() {
        if (!tp.Viewport.IsXSmall && this.IsMaximized) {
            this.X = this.fLastRect.X;
            this.Y = this.fLastRect.Y;
            this.Width = this.fLastRect.Width;
            this.Height = this.fLastRect.Height;
            if (this.fMaximizeButton) {
                this.fMaximizeButton.style.display = "";
                this.fRestoreButton.style.display = "none";
            }
            if (this.fDragger)
                this.fDragger.Active = true;
            this.fMaximized = false;
        }
    }
    /**
     * Handles Escape key.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    EscapePressed(e) {
        if (this.Modal)
            this.DialogResult = this.Args.DefaultDialogResult;
    }
    /**
     * Shows the window.
     * @returns {void}
     */
    Show() {
        if (this.Modal) {
            if (this.Handle.parentNode !== this.fOverlay.Handle)
                this.fOverlay.Handle.appendChild(this.Handle);
            this.fOverlay.Visible = true;
        } else {
            if (this.Handle.parentNode !== this.Document.body)
                this.Document.body.appendChild(this.Handle);
            this.fOverlay.Visible = false;
        }
        this.OnShowing();
        this.Visible = true;
        if (!this.Header) {
            this.CreateControls();
            this.SetupDragger();
            this.SetupPositionAndSize();
        }
        this.BringToFront();
        this.Handle.focus();
        this.OnShown();
        tp.Call(this.Args.ShowFunc, this.Args.Creator, this);
        this.ItemToControls();
        if (!tp.Viewport.IsXSmall && this.Args.CenterScreen === true)
            requestAnimationFrame(this.FuncBind(this.CenterInScreen));
    }
    /**
     * Hides the window.
     * @returns {void}
     */
    Hide() {
        this.Visible = false;
        if (this.Modal)
            this.fOverlay.Visible = false;
    }
    /**
     * Shows the window as modal dialog.
     * @returns {void}
     */
    ShowModal() {
        this.fDialogResult = tp.DialogResult.None;
        this.fModal = true;
        this.Show();
    }
    /**
     * Closes and disposes this instance.
     * @returns {void}
     */
    Close() {
        tp.Call(this.Args.CloseFunc, this.Args.Creator, this);
        this.OnClosing();
        if (this.IsValidResult())
            this.PassBackResult();
        this.Hide();
        this.OnClosed();
        tp.ListRemove(tp.Window.Windows, this);
        this.Dispose();
    }

    // ● notifications
    /**
     * Called when the window is about to show itself.
     * @returns {void}
     */
    OnShowing() {
        this.Trigger("Showing", {});
    }
    /**
     * Called when the window is shown.
     * @returns {void}
     */
    OnShown() {
        this.Trigger("Shown", {});
    }
    /**
     * Called when the window is about to close.
     * @returns {void}
     */
    OnClosing() {
        this.Trigger("Closing", {});
    }
    /**
     * Called when the window closes.
     * @returns {void}
     */
    OnClosed() {
        this.Trigger("Closed", {});
    }

    // ● event handler
    /**
     * Handles content wrapper resize.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    ContentResized(Args) {
        this.Trigger("ContentResized", {});
    }
    /**
     * Handles standard window clicks.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    WindowAnyClick(e) {
        var Command;
        if (e.type === "dblclick") {
            if (this.IsMaximized)
                this.Restore();
            else
                this.Maximize();
        } else {
            Command = tp.Data(e.target, "command");
            if (tp.IsSameText("Maximize", Command))
                this.Maximize();
            else if (tp.IsSameText("Restore", Command))
                this.Restore();
            else if (tp.IsSameText("Close", Command)) {
                if (this.Modal)
                    this.DialogResult = this.Args.DefaultDialogResult;
                else
                    this.Close();
            }
        }
    }
    /**
     * Handles footer button clicks.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    AnyClick(Args) {
        var Button;
        if (this.Modal === true) {
            Button = Args.Sender;
            if (tp.IsNumber(Button.DialogResult))
                this.DialogResult = Button.DialogResult;
        }
    }
};

// ● static fields
/**
 * Active windows.
 * @type {tp.Window[]}
 */
tp.Window.Windows = [];
/**
 * The three-lines icon as url-data.
 * @type {string}
 */
tp.Window.ICON_ThreeLines = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA8AAAANCAYAAAB2HjRBAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAACxJREFUOE9jcHBw+E8uZiBXI0gfZZr////PQC4mWyPIQsqcPRpgpKW2gQttALaGnxXL5WQ1AAAAAElFTkSuQmCC";

// ● prototype
tp.Window.prototype.tpClass = "tp.Window";
tp.Window.prototype.fOverlay = null;
tp.Window.prototype.fDragger = null;
tp.Window.prototype.fMaximizeButton = null;
tp.Window.prototype.fRestoreButton = null;
tp.Window.prototype.fCloseButton = null;
tp.Window.prototype.fFooterFill = null;
tp.Window.prototype.fLastRect = new tp.Rect(0, 0, 0, 0);
tp.Window.prototype.fMaximized = false;
tp.Window.prototype.fModal = false;
tp.Window.prototype.fDialogResult = tp.DialogResult.None;
tp.Window.prototype.Header = null;
tp.Window.prototype.HeaderText = null;
tp.Window.prototype.HeaderButtonBar = null;
tp.Window.prototype.Footer = null;
tp.Window.prototype.ContentWrapper = null;
tp.Window.prototype.Content = null;
tp.Window.prototype.fKeyDownHandler = null;
tp.Window.prototype.fWindowClickHandler = null;

// ● static public
/**
 * Displays a modal dialog box for editing an object.
 * @param {Object} Instance The object to edit.
 * @param {Function} WindowClass A tp.Window derived class.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {tp.Window} Returns the modal window.
 */
tp.Window.ShowModalFor = function (Instance, WindowClass, WindowArgs) {
    var Args = new tp.WindowArgs(WindowArgs);
    var Result;
    Args.Text = Args.Text || "Instance Editor";
    Args.AsModal = true;
    Args.DefaultDialogResult = tp.DialogResult.Cancel;
    Args.Instance = Instance;
    Result = new WindowClass(Args);
    Result.ShowModal();
    return Result;
};
/**
 * Displays a modal dialog box for editing an object and returns a Promise.
 * @param {Object} Instance The object to edit.
 * @param {Function} WindowClass A tp.Window derived class.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {Promise<tp.Window>} Returns a Promise resolving with the modal window.
 */
tp.Window.ShowModalForAsync = function (Instance, WindowClass, WindowArgs) {
    return new Promise(function (Resolve) {
        var Args = new tp.WindowArgs(WindowArgs);
        var CloseFunc = Args.CloseFunc;
        Args.Text = Args.Text || "Instance Editor";
        Args.CloseFunc = function (Window) {
            tp.Call(CloseFunc, Window.Args.Creator, Window);
            Resolve(Window);
        };
        tp.Window.ShowModalFor(Instance, WindowClass, Args);
    });
};

// ● content window
/**
 * A window for displaying existing html content.
 */
tp.ContentWindow = class extends tp.Window {
    // ● constructor
    /**
     * Creates a content window.
     * @param {tp.WindowArgs|object|null|undefined} Args The window arguments.
     */
    constructor(Args) {
        super(Args);
        this.tpClass = "tp.ContentWindow";
    }

    // ● protected
    /**
     * Disposes this instance.
     * @returns {void}
     */
    DoDispose() {
        if (tp.IsHTMLElement(this.Args.ContentParent) && tp.IsHTMLElement(this.Args.Content))
            this.Args.ContentParent.appendChild(this.Args.Content);
        super.DoDispose();
    }
    /**
     * Creates all window controls.
     * @returns {void}
     */
    CreateControls() {
        super.CreateControls();
        if (this.Args.AsModal === true) {
            this.CreateFooterButton("OK", "OK", tp.DialogResult.OK);
            this.CreateFooterButton("Cancel", "Cancel", tp.DialogResult.Cancel);
        }
        this.SetContent(this.Args.Content);
    }
    /**
     * Sets the content of the window.
     * @param {string|HTMLElement} Content The element, selector, HTML markup, or plain text to use as content.
     * @returns {void}
     */
    SetContent(Content) {
        var Element = tp.IsHTMLElement(Content) ? Content : null;
        var ContentElement;
        if (tp.IsString(Content) && tp.IsHtml(Content)) {
            ContentElement = this.CreateContentElement();
            ContentElement.Handle.innerHTML = Content;
            return;
        }
        if (!tp.IsHTMLElement(Element) && tp.IsString(Content)) {
            try {
                Element = tp.Select(Content);
            } catch (e) {
                Element = null;
            }
        }
        if (tp.IsHTMLElement(Element)) {
            this.Args.Content = Element;
            this.Args.ContentParent = Element.parentNode;
            this.ContentWrapper.Handle.appendChild(Element);
        } else if (tp.IsString(Content)) {
            ContentElement = this.CreateContentElement();
            ContentElement.Handle.textContent = Content;
        }
    }
};

// ● prototype
tp.ContentWindow.prototype.tpClass = "tp.ContentWindow";

// ● static public
/**
 * Displays a content window, either modal or non-modal.
 * @param {boolean} Modal True to show modally.
 * @param {string|HTMLElement} Content The element or selector with html content.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {tp.ContentWindow} Returns the content window.
 */
tp.ContentWindow.Show = function (Modal, Content, WindowArgs) {
    var Args = new tp.WindowArgs(WindowArgs);
    var Result;
    Args.Text = Args.Text || "Content Window";
    Args.ShowFooter = Modal;
    Args.Content = Content;
    Args.AsModal = Modal;
    Args.DefaultDialogResult = tp.DialogResult.Cancel;
    Result = new tp.ContentWindow(Args);
    if (Modal)
        Result.ShowModal();
    else
        Result.Show();
    Result.CenterInScreen();
    return Result;
};
/**
 * Displays a modal content window.
 * @param {string|HTMLElement} Content The element or selector with html content.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {tp.ContentWindow} Returns the content window.
 */
tp.ContentWindow.ShowModal = function (Content, WindowArgs) {
    return tp.ContentWindow.Show(true, Content, WindowArgs);
};
/**
 * Displays a content window and returns a Promise.
 * @param {boolean} Modal True to show modally.
 * @param {string|HTMLElement} Content The element or selector with html content.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {Promise<tp.ContentWindow>} Returns a Promise resolving with the content window.
 */
tp.ContentWindow.ShowAsync = function (Modal, Content, WindowArgs) {
    return new Promise(function (Resolve) {
        var Args = new tp.WindowArgs(WindowArgs);
        var CloseFunc = Args.CloseFunc;
        Args.CloseFunc = function (Window) {
            tp.Call(CloseFunc, Window.Args.Creator, Window);
            Resolve(Window);
        };
        tp.ContentWindow.Show(Modal, Content, Args);
    });
};
/**
 * Displays a modal content window and returns a Promise.
 * @param {string|HTMLElement} Content The element or selector with html content.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {Promise<tp.ContentWindow>} Returns a Promise resolving with the content window.
 */
tp.ContentWindow.ShowModalAsync = function (Content, WindowArgs) {
    return tp.ContentWindow.ShowAsync(true, Content, WindowArgs);
};

// ● message dialog
/**
 * Internal message dialog used by the message box helper functions.
 */
tp.MessageDialog = class extends tp.Window {
    // ● constructor
    /**
     * Creates a message dialog.
     * @param {tp.WindowArgs|object|null|undefined} Args The window arguments.
     */
    constructor(Args) {
        super(Args);
        this.tpClass = "tp.MessageDialog";
        tp.AddClass(this.Handle, "tp-MessageDialog");
    }

    // ● protected
    /**
     * Processes initialization arguments.
     * @returns {void}
     */
    ProcessInitInfo() {
        super.ProcessInitInfo();
        this.BoxType = this.Args.BoxType || "";
        this.MessageText = this.Args.MessageText || "";
    }
    /**
     * Creates all controls.
     * @returns {void}
     */
    CreateControls() {
        super.CreateControls();
        switch (this.BoxType) {
            case "Information":
                this.Text = "Information";
                this.CreateFooterButton("Close", "Close", tp.DialogResult.Cancel);
                this.Args.DefaultDialogResult = tp.DialogResult.Cancel;
                break;
            case "Error":
                this.Text = "Error";
                this.CreateFooterButton("Close", "Close", tp.DialogResult.Cancel);
                this.Args.DefaultDialogResult = tp.DialogResult.Cancel;
                break;
            case "Question":
                this.Text = "Question";
                this.CreateFooterButton("Yes", "Yes", tp.DialogResult.Yes);
                this.CreateFooterButton("No", "No", tp.DialogResult.No);
                this.Args.DefaultDialogResult = tp.DialogResult.No;
                break;
        }
        this.edtMemo = new tp.Component({
            ElementOrSelector: "textarea",
            Parent: this.CreateContentElement(),
            CssClasses: "tp-Memo"
        });
        this.edtMemo.Handle.cols = 10;
        this.edtMemo.Handle.rows = 5;
        this.edtMemo.Handle.spellcheck = false;
        this.edtMemo.Text = this.MessageText;
        this.edtMemo.Handle.focus();
    }
};

// ● prototype
tp.MessageDialog.prototype.tpClass = "tp.MessageDialog";
/**
 * Message box type.
 * @type {string}
 */
tp.MessageDialog.prototype.BoxType = "";
/**
 * Message text.
 * @type {string}
 */
tp.MessageDialog.prototype.MessageText = "";
/**
 * Memo component.
 * @type {tp.Component|null}
 */
tp.MessageDialog.prototype.edtMemo = null;

// ● static public
/**
 * Shows a message dialog modally.
 * @param {string} MessageText The text to display.
 * @param {string} BoxType The dialog type: Information, Error, or Question.
 * @param {Function|null|undefined} CloseFunc Optional close callback. Signature: function(Window: tp.Window): void.
 * @param {object|null|undefined} Creator Optional callback context.
 * @returns {tp.MessageDialog} Returns the message dialog.
 */
tp.MessageDialog.Show = function (MessageText, BoxType, CloseFunc, Creator) {
    var Args = new tp.WindowArgs();
    var Result;
    Args.Width = 500;
    Args.Height = 300;
    Args.CloseFunc = CloseFunc;
    Args.Creator = Creator;
    Args.BoxType = BoxType;
    Args.MessageText = MessageText;
    Result = new tp.MessageDialog(Args);
    Result.ShowModal();
    return Result;
};

// ● message boxes
/**
 * Displays an information modal dialog.
 * @param {string} MessageText The text to display.
 * @param {Function|null|undefined} CloseFunc Optional close callback.
 * @param {object|null|undefined} Creator Optional callback context.
 * @returns {tp.MessageDialog} Returns the message dialog.
 */
tp.InfoBox = function (MessageText, CloseFunc, Creator) {
    return tp.MessageDialog.Show(MessageText, "Information", CloseFunc, Creator);
};
/**
 * Displays an error modal dialog.
 * @param {string} MessageText The text to display.
 * @param {Function|null|undefined} CloseFunc Optional close callback.
 * @param {object|null|undefined} Creator Optional callback context.
 * @returns {tp.MessageDialog} Returns the message dialog.
 */
tp.ErrorBox = function (MessageText, CloseFunc, Creator) {
    return tp.MessageDialog.Show(MessageText, "Error", CloseFunc, Creator);
};
/**
 * Displays a yes-no modal dialog.
 * @param {string} MessageText The text to display.
 * @param {Function|null|undefined} CloseFunc Optional close callback.
 * @param {object|null|undefined} Creator Optional callback context.
 * @returns {tp.MessageDialog} Returns the message dialog.
 */
tp.YesNoBox = function (MessageText, CloseFunc, Creator) {
    return tp.MessageDialog.Show(MessageText, "Question", CloseFunc, Creator);
};
/**
 * Displays an information modal dialog and returns a Promise.
 * @param {string} MessageText The text to display.
 * @returns {Promise<tp.MessageDialog>} Returns a Promise resolving with the dialog.
 */
tp.InfoBoxAsync = function (MessageText) {
    return new Promise(function (Resolve) {
        tp.InfoBox(MessageText, function (Dialog) {
            Resolve(Dialog);
        });
    });
};
/**
 * Displays an error modal dialog and returns a Promise.
 * @param {string} MessageText The text to display.
 * @returns {Promise<tp.MessageDialog>} Returns a Promise resolving with the dialog.
 */
tp.ErrorBoxAsync = function (MessageText) {
    return new Promise(function (Resolve) {
        tp.ErrorBox(MessageText, function (Dialog) {
            Resolve(Dialog);
        });
    });
};
/**
 * Displays a yes-no modal dialog and returns a Promise.
 * @param {string} MessageText The text to display.
 * @returns {Promise<boolean>} Returns true when Yes is clicked.
 */
tp.YesNoBoxAsync = function (MessageText) {
    return new Promise(function (Resolve) {
        tp.YesNoBox(MessageText, function (Dialog) {
            Resolve(Dialog.DialogResult === tp.DialogResult.Yes);
        });
    });
};

// ● frame box
/**
 * Displays a modal window with an iframe element.
 * @param {string} UrlOrHtmlContent The URL or HTML content to display.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {tp.Window} Returns the modal window.
 */
tp.FrameBox = function (UrlOrHtmlContent, WindowArgs) {
    var Args = new tp.WindowArgs(WindowArgs);
    var Window;
    var Frame;
    Args.Text = Args.Text || "Frame Box";
    Args.DefaultDialogResult = tp.DialogResult.Cancel;
    Window = new tp.Window(Args);
    Window.ShowModal();
    Frame = new tp.IFrame({
        ElementOrSelector: "iframe",
        Parent: Window.CreateContentElement(),
        CssClasses: tp.Classes.Frame,
        CssText: "width:100%;height:100%;",
        UseSpinner: true
    });
    Window.CreateFooterButton("Close", "Close", tp.DialogResult.Cancel);
    if (tp.IsHtml(UrlOrHtmlContent))
        Frame.Content = UrlOrHtmlContent;
    else
        Frame.Url = UrlOrHtmlContent;
    setTimeout(function () {
        Frame.HideLoadSpinner();
    }, 1000 * 30);
    return Window;
};
/**
 * Displays a modal window with an iframe element and returns a Promise.
 * @param {string} UrlOrHtmlContent The URL or HTML content to display.
 * @param {tp.WindowArgs|object|null|undefined} WindowArgs Optional window arguments.
 * @returns {Promise<tp.Window>} Returns a Promise resolving with the modal window.
 */
tp.FrameBoxAsync = function (UrlOrHtmlContent, WindowArgs) {
    return new Promise(function (Resolve) {
        var Args = new tp.WindowArgs(WindowArgs);
        var CloseFunc = Args.CloseFunc;
        Args.CloseFunc = function (Window) {
            tp.Call(CloseFunc, Args.Creator, Window);
            Resolve(Window);
        };
        tp.FrameBox(UrlOrHtmlContent, Args);
    });
};
