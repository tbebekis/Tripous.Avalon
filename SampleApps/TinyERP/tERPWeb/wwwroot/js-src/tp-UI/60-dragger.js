// ● dragger mode
/**
 * Indicates the active operations of a dragger.
 * @enum {number}
 */
tp.DraggerMode = {
    Drag: 1,
    Resize: 2,
    Both: 1 | 2
};
Object.freeze(tp.DraggerMode);

// ● drag context listener
/**
 * Interface-like base class for tp.DragContext listeners.
 * @interface
 */
tp.IDragContextListener = class {
    // ● constructor
    /**
     * Creates a drag context listener.
     */
    constructor() {
    }

    // ● public
    /**
     * Called by tp.DragContext to decide if dragging should start.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {boolean} Returns true to start dragging.
     */
    IsDragStart(e) {
        return false;
    }
    /**
     * Called by tp.DragContext when dragging starts.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragStart(e) {
    }
    /**
     * Called by tp.DragContext while dragging.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragMove(e) {
    }
    /**
     * Called by tp.DragContext when dragging ends.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragEnd(e) {
    }
};

// ● drag context
/**
 * Tracks a mouse drag operation and delegates drag notifications to a listener.
 */
tp.DragContext = class {
    // ● constructor
    /**
     * Creates a drag context.
     * @param {string|Element} ElementOrSelector The element of this context.
     * @param {tp.IDragContextListener|object} Listener The drag context listener.
     */
    constructor(ElementOrSelector, Listener) {
        this.fElement = tp.Select(ElementOrSelector);
        this.fListener = Listener;
        if (!tp.IsHTMLElement(this.fElement))
            tp.Throw("tp.DragContext requires a valid HTMLElement.");
        if (tp.IsNil(Listener) || !tp.IsFunction(Listener.IsDragStart) || !tp.IsFunction(Listener.DragStart) || !tp.IsFunction(Listener.DragMove) || !tp.IsFunction(Listener.DragEnd))
            tp.Throw("tp.DragContext requires a valid listener.");
        this.fElement.addEventListener("mousedown", this);
        this.fElement.ownerDocument.addEventListener("mousemove", this, true);
        this.fElement.ownerDocument.addEventListener("mouseup", this, true);
    }

    // ● protected
    /**
     * Updates the stored mouse information.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    UpdateMouseInfo(e) {
        if (tp.IsNil(this.fMouseInfo))
            this.fMouseInfo = new tp.MouseInfo(e);
        else
            this.fMouseInfo.Update(e);
    }
    /**
     * Handles a mouse down event.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    OnMouseDown(e) {
        if (tp.Mouse.IsLeft(e)) {
            this.UpdateMouseInfo(e);
            this.fIsMouseDown = true;
            this.fDragging = this.fListener.IsDragStart(e) === true;
            if (this.Dragging)
                this.fListener.DragStart(e);
        }
    }
    /**
     * Handles a mouse move event.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    OnMouseMove(e) {
        if (this.fIsMouseDown && this.Dragging) {
            this.UpdateMouseInfo(e);
            this.fListener.DragMove(e);
        }
    }
    /**
     * Handles a mouse up event.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    OnMouseUp(e) {
        if (this.fIsMouseDown) {
            this.fIsMouseDown = false;
            if (this.Dragging) {
                this.UpdateMouseInfo(e);
                this.fDragging = false;
                this.fListener.DragEnd(e);
            }
        }
    }

    // ● properties
    /**
     * Returns true while dragging.
     * @returns {boolean} Returns true while dragging.
     */
    get Dragging() {
        return this.fDragging;
    }
    /**
     * Gets mouse information regarding the last handled mouse event.
     * @returns {tp.MouseInfo|null} Returns mouse information or null.
     */
    get MouseInfo() {
        return this.fMouseInfo;
    }
    /**
     * Returns true after Dispose() is called.
     * @returns {boolean} Returns true when this instance is disposed.
     */
    get IsDisposed() {
        return this.fIsDisposed;
    }

    // ● event handler
    /**
     * Handles DOM events.
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    handleEvent(e) {
        if (this.IsDisposed)
            return;
        if (tp.IsSameText("mousedown", e.type))
            this.OnMouseDown(e);
        else if (tp.IsSameText("mousemove", e.type))
            this.OnMouseMove(e);
        else if (tp.IsSameText("mouseup", e.type))
            this.OnMouseUp(e);
    }

    // ● public
    /**
     * Disposes this instance and removes event listeners.
     * @returns {void}
     */
    Dispose() {
        if (this.fIsDisposed === false && tp.IsHTMLElement(this.fElement)) {
            this.fElement.removeEventListener("mousedown", this);
            this.fElement.ownerDocument.removeEventListener("mousemove", this, true);
            this.fElement.ownerDocument.removeEventListener("mouseup", this, true);
            this.fElement = null;
            this.fListener = null;
            this.fMouseInfo = null;
            this.fIsDisposed = true;
        }
    }
};

// ● prototype
/**
 * Private field.
 * @type {boolean}
 */
tp.DragContext.prototype.fIsMouseDown = false;
/**
 * Private field.
 * @type {boolean}
 */
tp.DragContext.prototype.fDragging = false;
/**
 * Private field.
 * @type {tp.IDragContextListener|object|null}
 */
tp.DragContext.prototype.fListener = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.DragContext.prototype.fElement = null;
/**
 * Private field.
 * @type {tp.MouseInfo|null}
 */
tp.DragContext.prototype.fMouseInfo = null;
/**
 * Private field.
 * @type {boolean}
 */
tp.DragContext.prototype.fIsDisposed = false;

// ● dragger
/**
 * Moves and resizes an element with mouse interaction.
 *
 * Events:
 * - DragStart
 * - DragOver
 * - DragEnd
 *
 * @example
 * var Dragger = new tp.Dragger(tp.DraggerMode.Both, ".Box", ".Caption");
 */
tp.Dragger = class extends tp.Object {
    // ● constructor
    /**
     * Creates a dragger.
     * @param {number} Mode The active operations. A bit-field of tp.DraggerMode values.
     * @param {string|Element} ElementOrSelector The element to move or resize.
     * @param {string|Element|null|undefined} DragElementOrSelector Optional drag handle. When omitted, the main element is used.
     */
    constructor(Mode, ElementOrSelector, DragElementOrSelector) {
        var Element;
        super();
        this.fMode = tp.IsNumber(Mode) ? Mode : tp.DraggerMode.Both;
        this.fHandle = tp.Select(ElementOrSelector);
        if (!tp.IsHTMLElement(this.fHandle))
            tp.Throw("tp.Dragger requires a valid HTMLElement.");
        if (this.IsDraggable) {
            Element = tp.Select(DragElementOrSelector);
            this.fDragHandle = tp.IsHTMLElement(Element) ? Element : this.Handle;
        }
        this.fOldCursor = tp.Mouse.Cursor;
        this.Active = true;
    }

    // ● protected
    /**
     * Returns true when a width is inside the allowed limits.
     * @param {number} Value The width to check.
     * @returns {boolean} Returns true when the width is valid.
     */
    IsValidWidth(Value) {
        return Value >= this.MinWidth && Value <= this.MaxWidth;
    }
    /**
     * Returns true when a height is inside the allowed limits.
     * @param {number} Value The height to check.
     * @returns {boolean} Returns true when the height is valid.
     */
    IsValidHeight(Value) {
        return Value >= this.MinHeight && Value <= this.MaxHeight;
    }
    /**
     * Sets the cursor on the handle, its parent, and the document body.
     * @param {string} Cursor The CSS cursor value.
     * @returns {void}
     */
    SetCursor(Cursor) {
        var Body;
        if (tp.IsHTMLElement(this.Handle)) {
            Body = this.Handle.ownerDocument.body;
            if (Body)
                Body.style.cursor = Cursor;
            this.Handle.style.cursor = Cursor;
            if (tp.IsHTMLElement(this.Handle.parentNode))
                this.Handle.parentNode.style.cursor = Cursor;
        }
    }
    /**
     * Activates or deactivates this dragger.
     * @param {boolean} Value True to activate; false to deactivate.
     * @returns {void}
     */
    SetActive(Value) {
        Value = Value === true;
        if (Value !== this.Active) {
            if (Value) {
                this.fHandle.addEventListener("scroll", this, true);
                this.fHandle.addEventListener("mousedown", this, true);
                this.fHandle.addEventListener("mouseout", this, true);
                this.fHandle.ownerDocument.addEventListener("mousemove", this, true);
            } else {
                this.fHandle.removeEventListener("scroll", this, true);
                this.fHandle.removeEventListener("mousedown", this, true);
                this.fHandle.removeEventListener("mouseout", this, true);
                this.fHandle.ownerDocument.removeEventListener("mousemove", this, true);
                this.fHandle.ownerDocument.removeEventListener("mouseup", this, true);
                this.SetCursor(this.fOldCursor);
            }
            this.fActive = Value;
        }
    }
    /**
     * Starts the current drag or resize operation.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragStart(e) {
        var Parent;
        var Mouse;
        var Style;
        if (this.Dragging === true || this.Resizing === true) {
            Parent = this.fHandle.parentNode;
            Mouse = tp.Mouse.ToElement(e, Parent);
            Style = tp.GetComputedStyle(this.Handle);
            this.fDelta = new tp.Point(Mouse.X - tp.ExtractNumber(Style ? Style.left : 0), Mouse.Y - tp.ExtractNumber(Style ? Style.top : 0));
            this.OnDragStart(e);
        }
    }
    /**
     * Moves the current drag or resize operation.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragMove(e) {
        var L;
        var T;
        var W;
        var H;
        var Mouse;
        var Style;
        var Rect;
        if (this.IsDraggable && this.Dragging) {
            Mouse = tp.Mouse.ToElement(e, this.fHandle.parentNode);
            L = Mouse.X - this.fDelta.X;
            T = Mouse.Y - this.fDelta.Y;
            this.fHandle.style.left = tp.px(L);
            this.fHandle.style.top = tp.px(T);
            this.OnDragMove(e);
        } else if (this.IsResizable && this.Resizing) {
            Style = tp.GetComputedStyle(this.fHandle);
            Rect = this.fHandle.getBoundingClientRect();
            L = tp.ExtractNumber(Style ? Style.left : 0);
            T = tp.ExtractNumber(Style ? Style.top : 0);
            W = tp.ExtractNumber(Style ? Style.width : Rect.width);
            H = tp.ExtractNumber(Style ? Style.height : Rect.height);

            if (e.clientX < Rect.left) {
                L -= Rect.left - e.clientX;
                W += Rect.left - e.clientX;
            } else if (e.clientX > Rect.left && e.clientX < Rect.right) {
                if (tp.Edge.IsLeft(this.fEdge)) {
                    L += e.clientX - Rect.left;
                    W -= e.clientX - Rect.left;
                } else if (tp.Edge.IsRight(this.fEdge)) {
                    W -= Rect.right - e.clientX;
                }
            } else if (e.clientX > Rect.right) {
                W += e.clientX - Rect.right;
            }

            if (e.clientY < Rect.top) {
                T -= Rect.top - e.clientY;
                H += Rect.top - e.clientY;
            } else if (e.clientY > Rect.top && e.clientY < Rect.bottom) {
                if (tp.Edge.IsTop(this.fEdge)) {
                    T += e.clientY - Rect.top;
                    H -= e.clientY - Rect.top;
                } else if (tp.Edge.IsBottom(this.fEdge)) {
                    H -= Rect.bottom - e.clientY;
                }
            } else if (e.clientY > Rect.bottom) {
                H += e.clientY - Rect.bottom;
            }

            if (this.IsValidWidth(W) && this.IsValidHeight(H)) {
                this.fHandle.style.left = tp.px(L);
                this.fHandle.style.width = tp.px(W);
                this.fHandle.style.top = tp.px(T);
                this.fHandle.style.height = tp.px(H);
            }
            this.OnDragMove(e);
        }
    }
    /**
     * Ends the current drag or resize operation.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragEnd(e) {
        this.SetCursor(this.fOldCursor);
        this.OnDragEnd(e);
    }
    /**
     * Event trigger called when a drag or resize operation starts.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    OnDragStart(e) {
        this.Trigger(tp.Events.DragStart, { e: e });
    }
    /**
     * Event trigger called while a drag or resize operation moves.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    OnDragMove(e) {
        this.Trigger(tp.Events.DragOver, { e: e });
    }
    /**
     * Event trigger called when a drag or resize operation ends.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    OnDragEnd(e) {
        this.Trigger(tp.Events.DragEnd, { e: e });
    }

    // ● properties
    /**
     * Gets or sets a value indicating whether this instance is active.
     * @returns {boolean} Returns true when this instance is active.
     */
    get Active() {
        return this.fActive;
    }
    /**
     * Gets or sets a value indicating whether this instance is active.
     * @param {boolean} Value True to activate; false to deactivate.
     * @returns {void}
     */
    set Active(Value) {
        this.SetActive(Value);
    }
    /**
     * Gets the active operations of this dragger.
     * @returns {number} Returns a bit-field of tp.DraggerMode values.
     */
    get Mode() {
        return this.fMode;
    }
    /**
     * Returns true when this dragger can move its handle.
     * @returns {boolean} Returns true when moving is enabled.
     */
    get IsDraggable() {
        return tp.Bf.In(tp.DraggerMode.Drag, this.Mode);
    }
    /**
     * Returns true when this dragger can resize its handle.
     * @returns {boolean} Returns true when resizing is enabled.
     */
    get IsResizable() {
        return tp.Bf.In(tp.DraggerMode.Resize, this.Mode);
    }
    /**
     * Returns true while resizing.
     * @returns {boolean} Returns true while resizing.
     */
    get Resizing() {
        return this.fResizing;
    }
    /**
     * Returns true while dragging.
     * @returns {boolean} Returns true while dragging.
     */
    get Dragging() {
        return this.fDragging;
    }
    /**
     * Gets the element to move or resize.
     * @returns {HTMLElement|null} Returns the handle element.
     */
    get Handle() {
        return this.fHandle;
    }
    /**
     * Gets the element used as the drag handle.
     * @returns {HTMLElement|null} Returns the drag handle element.
     */
    get DragHandle() {
        return this.fDragHandle;
    }

    // ● event handler
    /**
     * Handles DOM events.
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    handleEvent(e) {
        var Edge;
        var Dif;
        if (this.Active !== true)
            return;
        if (tp.IsSameText("scroll", e.type) && (this.Resizing || this.Dragging)) {
            e.preventDefault();
            return;
        }
        if (tp.IsSameText("mousedown", e.type)) {
            if (tp.Mouse.IsLeft(e)) {
                Edge = tp.Edge.ResizeHitTest(e, this.fHandle, this.HandleSize);
                if (this.IsDraggable && Edge === tp.Edge.None && tp.ContainsEventTarget(this.fDragHandle, e.target)) {
                    this.fDragging = true;
                    this.SetCursor(tp.Cursors.Move);
                    this.DragStart(e);
                } else if (this.IsResizable && tp.Bf.In(Edge, this.Edges)) {
                    this.fResizing = true;
                    this.fEdge = Edge;
                    this.SetCursor(tp.Edge.ToCursor(Edge));
                    this.DragStart(e);
                }
                if (this.Dragging === true || this.Resizing === true) {
                    this.fMouseInfo = new tp.MouseInfo(e);
                    this.fHandle.ownerDocument.addEventListener("mouseup", this, true);
                }
            }
        } else if (tp.IsSameText("mousemove", e.type)) {
            if (!this.InMove) {
                this.InMove = true;
                try {
                    if (tp.Mouse.IsLeft(e) && (this.fDragging || this.fResizing)) {
                        Dif = this.fMouseInfo.Dif(e);
                        if (!(Math.abs(Dif.X) > 5 || Math.abs(Dif.Y) > 5))
                            return;
                        this.DragMove(e);
                    } else if (this.IsResizable) {
                        Edge = tp.Edge.ResizeHitTest(e, this.fHandle, this.HandleSize);
                        this.SetCursor(tp.Bf.In(Edge, this.Edges) ? tp.Edge.ToCursor(Edge) : this.fOldCursor);
                    }
                } finally {
                    this.InMove = false;
                }
            }
        } else if (tp.IsSameText("mouseout", e.type)) {
            if (!this.fResizing)
                this.SetCursor(this.fOldCursor);
        } else if (tp.IsSameText("mouseup", e.type)) {
            if (this.fDragging || this.fResizing) {
                this.fHandle.ownerDocument.removeEventListener("mouseup", this, true);
                this.DragEnd(e);
            }
            this.fDragging = false;
            this.fResizing = false;
            this.fEdge = tp.Edge.None;
            this.fMouseInfo = null;
        }
    }

    // ● public
    /**
     * Disposes this instance and removes event listeners.
     * @returns {void}
     */
    Dispose() {
        this.Active = false;
        this.fHandle = null;
        this.fDragHandle = null;
        this.fMouseInfo = null;
    }
};

// ● prototype
/**
 * Private field.
 * @type {boolean}
 */
tp.Dragger.prototype.fActive = false;
/**
 * Private field.
 * @type {number}
 */
tp.Dragger.prototype.fMode = tp.DraggerMode.Both;
/**
 * Private field.
 * @type {boolean}
 */
tp.Dragger.prototype.fDragging = false;
/**
 * Private field.
 * @type {boolean}
 */
tp.Dragger.prototype.fResizing = false;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.Dragger.prototype.fHandle = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.Dragger.prototype.fDragHandle = null;
/**
 * Private field.
 * @type {string}
 */
tp.Dragger.prototype.fOldCursor = "";
/**
 * Private field.
 * @type {number}
 */
tp.Dragger.prototype.fEdge = tp.Edge.None;
/**
 * Private field.
 * @type {tp.Point|null}
 */
tp.Dragger.prototype.fDelta = null;
/**
 * Private field.
 * @type {tp.MouseInfo|null}
 */
tp.Dragger.prototype.fMouseInfo = null;
/**
 * True while this instance handles a mouse move event.
 * @type {boolean}
 */
tp.Dragger.prototype.InMove = false;
/**
 * Bit-field with the edges to use as valid resize handles.
 * @type {number}
 */
tp.Dragger.prototype.Edges = tp.Edge.All;
/**
 * The minimum resize width.
 * @type {number}
 */
tp.Dragger.prototype.MinWidth = 50;
/**
 * The maximum resize width.
 * @type {number}
 */
tp.Dragger.prototype.MaxWidth = 6000;
/**
 * The minimum resize height.
 * @type {number}
 */
tp.Dragger.prototype.MinHeight = 50;
/**
 * The maximum resize height.
 * @type {number}
 */
tp.Dragger.prototype.MaxHeight = 6000;
/**
 * The resize handle hit-test size in pixels.
 * @type {number}
 */
tp.Dragger.prototype.HandleSize = 8;
