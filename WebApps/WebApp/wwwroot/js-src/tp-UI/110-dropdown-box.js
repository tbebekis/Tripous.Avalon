// ● drop-down box listener
/**
 * Interface implemented by objects that listen to tp.DropDownBox stage changes.
 */
tp.IDropDownBoxListener = class {
    // ● public
    /**
     * Handles a drop-down box stage change.
     * @param {tp.DropDownBox} Sender The sender.
     * @param {number} Stage The tp.DropDownBoxStage value.
     * @returns {void}
     */
    OnDropDownBoxEvent(Sender, Stage) {
    }
};
/**
 * Returns true when a value implements tp.IDropDownBoxListener.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a drop-down box listener.
 */
tp.IsDropDownBoxListener = function (Value) {
    return !tp.IsEmpty(Value) && tp.IsFunction(Value.OnDropDownBoxEvent);
};

// ● drop-down box stage
/**
 * Indicates the stage of a drop-down box operation.
 * @enum {number}
 */
tp.DropDownBoxStage = {
    Opening: 1,
    Opened: 2,
    Closing: 4,
    Closed: 8
};
Object.freeze(tp.DropDownBoxStage);

// ● drop-down box
/**
 * A resizable fixed-position drop-down box associated with another element.
 * It keeps itself inside the viewport and closes when the user clicks outside or scrolls the page.
 *
 * Events:
 * - Opening
 * - Opened
 * - Closing
 * - Closed
 */
tp.DropDownBox = class extends tp.Component {
    // ● constructor
    /**
     * Creates a drop-down box.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The drop-down box create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.DropDownBox.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.DropDownBox";
        tp.AddClass(this.Handle, tp.Classes.DropDownBox);
        this.fWindowScrollHandler = this.FuncBind(this.Window_Scroll);
        this.fDocumentClickHandler = this.FuncBind(this.Document_Click);
        this.Associate = this.CreateParams.Associate;
        this.Owner = this.CreateParams.Owner;
        this.ApplySizeParams(this.CreateParams);
        this.Handle.tabIndex = -1;
        this.Handle.style.position = "fixed";
        this.CreateDragger();
        if (tp.IsBlank(this.Id))
            this.Id = tp.SafeId("DropDown");
    }

    // ● protected
    /**
     * Creates normalized drop-down box create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        if (arguments.length > 1) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        if (!tp.IsHTMLElement(tp(Params.ElementOrSelector)))
            Params.ElementOrSelector = "div";
        return Params;
    }
    /**
     * Applies size create parameters.
     * @param {tp.CreateParams|object|null|undefined} Params The create parameters.
     * @returns {void}
     */
    ApplySizeParams(Params) {
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Width = Params.Width;
        if (!tp.IsNil(Params.Height))
            this.Height = Params.Height;
    }
    /**
     * Creates the internal dragger.
     * @returns {void}
     */
    CreateDragger() {
        this.fDragger = new tp.Dragger(tp.DraggerMode.Resize, this.Handle);
        this.fDragger.Edges = tp.Edge.Bottom | tp.Edge.Right;
        this.fDragger.MinHeight = 100;
        this.fDragger.MinWidth = 100;
        this.fDragger.On(tp.Events.DragStart, this.FuncBind(this.AnyDraggerEvent));
        this.fDragger.On(tp.Events.DragEnd, this.FuncBind(this.AnyDraggerEvent));
    }
    /**
     * Resolves a value to an owner listener.
     * @param {*} Value The value to resolve.
     * @returns {object|null} Returns the owner listener or null.
     */
    ResolveOwner(Value) {
        var Element;
        var Component;
        if (tp.IsDropDownBoxListener(Value))
            return Value;
        Element = tp(Value);
        Component = tp.Component.GetComponent(Element);
        if (tp.IsDropDownBoxListener(Component))
            return Component;
        if (tp.IsDropDownBoxListener(Element))
            return Element;
        return null;
    }
    /**
     * Notifies the owner and triggers the matching Tripous event.
     * @param {number} Stage The tp.DropDownBoxStage value.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnOwnerEvent(Stage) {
        var EventName = this.StageToEventName(Stage);
        if (tp.IsDropDownBoxListener(this.Owner))
            this.Owner.OnDropDownBoxEvent(this, Stage);
        return EventName ? this.Trigger(EventName, { Stage: Stage }) : null;
    }
    /**
     * Returns the event name for a stage.
     * @param {number} Stage The tp.DropDownBoxStage value.
     * @returns {string} Returns the event name.
     */
    StageToEventName(Stage) {
        switch (Stage) {
            case tp.DropDownBoxStage.Opening: return "Opening";
            case tp.DropDownBoxStage.Opened: return "Opened";
            case tp.DropDownBoxStage.Closing: return "Closing";
            case tp.DropDownBoxStage.Closed: return "Closed";
            default: return "";
        }
    }
    /**
     * Handles dragger events.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    AnyDraggerEvent(Args) {
        if (Args.EventName === tp.Events.DragStart) {
            this.fResizing = true;
        } else if (Args.EventName === tp.Events.DragEnd) {
            setTimeout(function (Self) {
                Self.fResizing = false;
            }, 600, this);
        }
    }
    /**
     * Handles window scroll events.
     * @param {Event} e The event.
     * @returns {void}
     */
    Window_Scroll(e) {
        if (!tp.ContainsEventTarget(this.Handle, e.target)) {
            if (this.IsOpen && this.StyleProp("position") === "fixed")
                this.Close();
        }
    }
    /**
     * Handles document click events.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    Document_Click(e) {
        if (!tp.ContainsEventTarget(this.Handle, e.target)) {
            if (this.IsOpen) {
                this.Close();
                e.stopPropagation();
            }
        }
    }
    /**
     * Updates the z-index to place this box above existing body children.
     * @returns {void}
     */
    UpdateZIndex() {
        this.ZIndex = tp.MaxZIndexOf(this.Document.body) + 1;
    }
    /**
     * Repositions this box so it stays inside the viewport.
     * @returns {void}
     */
    KeepInsideViewport() {
        var Rect = this.Handle.getBoundingClientRect();
        var Left = Rect.left;
        var Top = Rect.top;
        if (Rect.bottom > tp.Viewport.Height)
            Top = Math.max(0, Rect.top - Rect.height);
        if (Rect.right > tp.Viewport.Width)
            Left = Math.max(0, tp.Viewport.Width - Rect.width);
        this.X = Left;
        this.Y = Top;
    }

    // ● public
    /**
     * Adds a div child element to the box.
     * @returns {HTMLElement|null} Returns the added element.
     */
    AddDivElement() {
        return this.AddElement("div");
    }
    /**
     * Opens the drop-down box.
     * @returns {void}
     */
    Open() {
        var Style;
        if (!this.IsOpen && tp.IsHTMLElement(this.Associate)) {
            if (!this.ParentHandle)
                this.Document.body.appendChild(this.Handle);
            this.OnOwnerEvent(tp.DropDownBoxStage.Opening);
            tp.AddClass(this.Handle, tp.Classes.Visible);
            this.UpdateTop();
            if (this.fIsFirstOpen === true) {
                Style = tp.GetComputedStyle(this.Handle);
                if (Style.position !== "fixed")
                    this.Position = "fixed";
                if (!this.Handle.style.width)
                    this.Width = Math.max(this.Associate.getBoundingClientRect().width, 100);
                this.fIsFirstOpen = false;
            }
            this.OnOwnerEvent(tp.DropDownBoxStage.Opened);
            this.KeepInsideViewport();
            this.UpdateZIndex();
            setTimeout(function (Self) {
                window.addEventListener("scroll", Self.fWindowScrollHandler);
                Self.Document.addEventListener("click", Self.fDocumentClickHandler, true);
            }, 0, this);
        }
    }
    /**
     * Closes the drop-down box.
     * @returns {void}
     */
    Close() {
        if (this.IsOpen && !this.Resizing) {
            this.OnOwnerEvent(tp.DropDownBoxStage.Closing);
            tp.RemoveClass(this.Handle, tp.Classes.Visible);
            this.OnOwnerEvent(tp.DropDownBoxStage.Closed);
            try {
                window.removeEventListener("scroll", this.fWindowScrollHandler);
            } catch (e) {
                //
            }
            try {
                this.Document.removeEventListener("click", this.fDocumentClickHandler, true);
            } catch (e) {
                //
            }
        }
    }
    /**
     * Opens or closes the drop-down box.
     * @returns {void}
     */
    Toggle() {
        if (this.IsOpen)
            this.Close();
        else
            this.Open();
    }
    /**
     * Updates the top-left position using the associate element.
     * @returns {void}
     */
    UpdateTop() {
        var Rect;
        if (this.IsOpen && tp.IsHTMLElement(this.Associate)) {
            Rect = this.Associate.getBoundingClientRect();
            this.X = Math.round(Rect.left);
            this.Y = Math.round(Rect.top + Rect.height);
        }
    }
    /**
     * Gets or sets a style property.
     * @param {string} Name The style property name.
     * @param {*} Value The optional value to set.
     * @returns {*} Returns the value.
     */
    StyleProp(Name, Value) {
        if (arguments.length < 2)
            return tp.StyleProp(this.Handle, Name);
        return tp.StyleProp(this.Handle, Name, Value);
    }
    /**
     * Disposes this drop-down box.
     * @returns {void}
     */
    Dispose() {
        this.fResizing = false;
        if (this.IsOpen)
            tp.RemoveClass(this.Handle, tp.Classes.Visible);
        try {
            window.removeEventListener("scroll", this.fWindowScrollHandler);
        } catch (e) {
            //
        }
        try {
            this.Document.removeEventListener("click", this.fDocumentClickHandler, true);
        } catch (e) {
            //
        }
        if (this.fDragger) {
            this.fDragger.Dispose();
            this.fDragger = null;
        }
        this.fWindowScrollHandler = null;
        this.fDocumentClickHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Gets or sets the associate element.
     * @returns {HTMLElement|null} Returns the associate element.
     */
    get Associate() {
        return this.fAssociate;
    }
    /**
     * Gets or sets the associate element.
     * @param {HTMLElement|string|null|undefined} Value The associate element or selector.
     * @returns {void}
     */
    set Associate(Value) {
        var Element = tp(Value);
        if (Element !== this.fAssociate) {
            this.fAssociate = tp.IsHTMLElement(Element) ? Element : null;
            this.fIsFirstOpen = true;
        }
    }
    /**
     * Gets or sets the owner listener.
     * @returns {object|null} Returns the owner listener.
     */
    get Owner() {
        return this.fOwner;
    }
    /**
     * Gets or sets the owner listener.
     * @param {*} Value The owner listener, component, element, or selector.
     * @returns {void}
     */
    set Owner(Value) {
        this.fOwner = this.ResolveOwner(Value);
    }
    /**
     * Returns true while the drop-down box is open.
     * @returns {boolean} Returns true when open.
     */
    get IsOpen() {
        return tp.HasClass(this.Handle, tp.Classes.Visible);
    }
    /**
     * Returns true while the drop-down box is resizing.
     * @returns {boolean} Returns true when resizing.
     */
    get Resizing() {
        return this.fResizing === true;
    }
    /**
     * Gets the dragger used for resizing.
     * @returns {tp.Dragger|null} Returns the dragger.
     */
    get Dragger() {
        return this.fDragger;
    }
    /**
     * Gets or sets the left coordinate.
     * @returns {number} Returns the left coordinate.
     */
    get X() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().left : 0;
    }
    /**
     * Gets or sets the left coordinate.
     * @param {number} Value The left coordinate.
     * @returns {void}
     */
    set X(Value) {
        if (this.HasHandle)
            this.Handle.style.left = tp.px(Math.round(tp.StrToFloat(Value, 0)));
    }
    /**
     * Gets or sets the top coordinate.
     * @returns {number} Returns the top coordinate.
     */
    get Y() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().top : 0;
    }
    /**
     * Gets or sets the top coordinate.
     * @param {number} Value The top coordinate.
     * @returns {void}
     */
    set Y(Value) {
        if (this.HasHandle)
            this.Handle.style.top = tp.px(Math.round(tp.StrToFloat(Value, 0)));
    }
    /**
     * Gets or sets the width.
     * @returns {number} Returns the width.
     */
    get Width() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().width : 0;
    }
    /**
     * Gets or sets the width.
     * @param {number|string} Value The width value.
     * @returns {void}
     */
    set Width(Value) {
        if (this.HasHandle)
            this.Handle.style.width = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
    /**
     * Gets or sets the height.
     * @returns {number} Returns the height.
     */
    get Height() {
        return this.HasHandle ? this.Handle.getBoundingClientRect().height : 0;
    }
    /**
     * Gets or sets the height.
     * @param {number|string} Value The height value.
     * @returns {void}
     */
    set Height(Value) {
        if (this.HasHandle)
            this.Handle.style.height = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
    /**
     * Gets or sets the z-index.
     * @returns {number} Returns the z-index.
     */
    get ZIndex() {
        return tp.ZIndex(this.Handle);
    }
    /**
     * Gets or sets the z-index.
     * @param {number} Value The z-index.
     * @returns {void}
     */
    set ZIndex(Value) {
        if (this.HasHandle)
            tp.ZIndex(this.Handle, Value);
    }
};

// ● prototype
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.DropDownBox.prototype.fAssociate = null;
/**
 * Private field.
 * @type {object|null}
 */
tp.DropDownBox.prototype.fOwner = null;
/**
 * Private field.
 * @type {tp.Dragger|null}
 */
tp.DropDownBox.prototype.fDragger = null;
/**
 * Private field.
 * @type {boolean}
 */
tp.DropDownBox.prototype.fResizing = false;
/**
 * Private field.
 * @type {boolean}
 */
tp.DropDownBox.prototype.fIsFirstOpen = true;
/**
 * Private field.
 * @type {Function|null}
 */
tp.DropDownBox.prototype.fWindowScrollHandler = null;
/**
 * Private field.
 * @type {Function|null}
 */
tp.DropDownBox.prototype.fDocumentClickHandler = null;
