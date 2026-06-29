// ● mouse
/**
 * Mouse helper based on MouseEvent and PointerEvent.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent
 * See: https://developer.mozilla.org/en-US/docs/Web/API/PointerEvent
 * @type {object}
 */
tp.Mouse = {
    NONE: 0,
    LEFT: 1,
    RIGHT: 2,
    MID: 4,
    /**
     * Gets or sets the document body cursor.
     * @returns {string} Returns the document body cursor.
     */
    get Cursor() {
        return document.body ? document.body.style.cursor : "";
    },
    /**
     * Gets or sets the document body cursor.
     * @param {string} Value The cursor value.
     * @returns {void}
     */
    set Cursor(Value) {
        if (document.body)
            document.body.style.cursor = Value;
    },
    /**
     * Returns a Tripous mouse button constant from an event.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent/button
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {number} Returns one of the tp.Mouse button constants.
     */
    Button: function (e) {
        if (tp.IsNil(e) || !tp.IsNumber(e.button))
            return tp.Mouse.NONE;
        switch (e.button) {
            case 0: return tp.Mouse.LEFT;
            case 1: return tp.Mouse.MID;
            case 2: return tp.Mouse.RIGHT;
            default: return tp.Mouse.NONE;
        }
    },
    /**
     * Returns a bit-field with the mouse buttons currently pressed.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent/buttons
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {number} Returns a bit-field with pressed tp.Mouse button constants.
     */
    Buttons: function (e) {
        return !tp.IsNil(e) && tp.IsNumber(e.buttons) ? e.buttons : tp.Mouse.NONE;
    },
    /**
     * Returns true when the left mouse button is the event button.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {boolean} Returns true when the left button is the event button.
     */
    IsLeft: function (e) {
        return tp.Mouse.Button(e) === tp.Mouse.LEFT;
    },
    /**
     * Returns true when the middle mouse button is the event button.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {boolean} Returns true when the middle button is the event button.
     */
    IsMid: function (e) {
        return tp.Mouse.Button(e) === tp.Mouse.MID;
    },
    /**
     * Returns true when the right mouse button is the event button.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {boolean} Returns true when the right button is the event button.
     */
    IsRight: function (e) {
        return tp.Mouse.Button(e) === tp.Mouse.RIGHT;
    },
    /**
     * Returns the mouse position relative to the rendered document.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent/pageX
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {tp.Point} Returns the document position.
     */
    ToDocument: function (e) {
        if (tp.IsNil(e))
            return new tp.Point();
        return new tp.Point(tp.IsNumber(e.pageX) ? e.pageX : 0, tp.IsNumber(e.pageY) ? e.pageY : 0);
    },
    /**
     * Returns the mouse position relative to the viewport.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent/clientX
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {tp.Point} Returns the viewport position.
     */
    ToViewport: function (e) {
        if (tp.IsNil(e))
            return new tp.Point();
        return new tp.Point(tp.IsNumber(e.clientX) ? e.clientX : 0, tp.IsNumber(e.clientY) ? e.clientY : 0);
    },
    /**
     * Returns the mouse position relative to the physical screen.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent/screenX
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {tp.Point} Returns the screen position.
     */
    ToScreen: function (e) {
        if (tp.IsNil(e))
            return new tp.Point();
        return new tp.Point(tp.IsNumber(e.screenX) ? e.screenX : 0, tp.IsNumber(e.screenY) ? e.screenY : 0);
    },
    /**
     * Returns the mouse position relative to an element.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @param {Element|string|null|undefined} Selector The optional target selector or element.
     * @returns {tp.Point} Returns the element-relative position.
     */
    ToElement: function (e, Selector) {
        var Element = tp.IsNil(Selector) && !tp.IsNil(e) ? e.target : tp(Selector);
        var Rect;
        if (!tp.IsElement(Element) || tp.IsNil(e))
            return new tp.Point();
        Rect = Element.getBoundingClientRect();
        return new tp.Point(e.clientX - Rect.left, e.clientY - Rect.top);
    },
    /**
     * Returns true when the mouse position is inside an element.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @param {Element|string} Selector The target selector or element.
     * @returns {boolean} Returns true when the mouse is inside the element.
     */
    IsInElement: function (e, Selector) {
        var Element = tp(Selector);
        var Rect;
        if (!tp.IsElement(Element) || tp.IsNil(e))
            return false;
        Rect = Element.getBoundingClientRect();
        return e.clientX >= Rect.left && e.clientX <= Rect.right && e.clientY >= Rect.top && e.clientY <= Rect.bottom;
    },
    /**
     * Returns the topmost element under the mouse.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/Document/elementFromPoint
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {Element|null} Returns the element under the mouse.
     */
    ElementUnderMouse: function (e) {
        if (tp.IsNil(e))
            return null;
        return tp.Mouse.ElementAt(e.clientX, e.clientY);
    },
    /**
     * Returns the topmost element at viewport coordinates.
     * @param {number} X The viewport X coordinate.
     * @param {number} Y The viewport Y coordinate.
     * @returns {Element|null} Returns the element at the coordinates.
     */
    ElementAt: function (X, Y) {
        var Result = null;
        if (document.elementFromPoint)
            Result = document.elementFromPoint(X, Y);
        return tp.IsElement(Result) ? Result : null;
    }
};
Object.freeze(tp.Mouse);

// ● mouse info
/**
 * Information captured from a mouse or pointer event.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/MouseEvent
 */
tp.MouseInfo = class {
    // ● constructor
    /**
     * Creates a mouse info instance.
     * @param {MouseEvent|PointerEvent|null|undefined} e The optional mouse or pointer event.
     */
    constructor(e) {
        this.Update(e);
    }

    // ● properties
    /**
     * Gets a value indicating whether the alt key was down.
     * @returns {boolean} Returns true when alt was down.
     */
    get Alt() {
        return this.fAlt;
    }
    /**
     * Gets a value indicating whether the control key was down.
     * @returns {boolean} Returns true when control was down.
     */
    get Ctrl() {
        return this.fCtrl;
    }
    /**
     * Gets a value indicating whether the shift key was down.
     * @returns {boolean} Returns true when shift was down.
     */
    get Shift() {
        return this.fShift;
    }
    /**
     * Gets a value indicating whether the meta key was down.
     * @returns {boolean} Returns true when meta was down.
     */
    get Meta() {
        return this.fMeta;
    }
    /**
     * Gets the event mouse button.
     * @returns {number} Returns one of the tp.Mouse button constants.
     */
    get Button() {
        return this.fButton;
    }
    /**
     * Gets the currently pressed mouse buttons bit-field.
     * @returns {number} Returns a bit-field of tp.Mouse button constants.
     */
    get Buttons() {
        return this.fButtons;
    }
    /**
     * Returns true when the left mouse button is the event button.
     * @returns {boolean} Returns true when the left button is the event button.
     */
    get IsLeft() {
        return this.Button === tp.Mouse.LEFT;
    }
    /**
     * Returns true when the middle mouse button is the event button.
     * @returns {boolean} Returns true when the middle button is the event button.
     */
    get IsMid() {
        return this.Button === tp.Mouse.MID;
    }
    /**
     * Returns true when the right mouse button is the event button.
     * @returns {boolean} Returns true when the right button is the event button.
     */
    get IsRight() {
        return this.Button === tp.Mouse.RIGHT;
    }
    /**
     * Gets the event position relative to the rendered document.
     * @returns {tp.Point} Returns the document position.
     */
    get Document() {
        return this.fDocument;
    }
    /**
     * Gets the event position relative to the viewport.
     * @returns {tp.Point} Returns the viewport position.
     */
    get Viewport() {
        return this.fViewport;
    }
    /**
     * Gets the event position relative to the physical screen.
     * @returns {tp.Point} Returns the screen position.
     */
    get Screen() {
        return this.fScreen;
    }

    // ● public
    /**
     * Updates this instance from a mouse or pointer event.
     * @param {MouseEvent|PointerEvent|null|undefined} e The mouse or pointer event.
     * @returns {void}
     */
    Update(e) {
        if (tp.IsNil(e))
            return;
        this.fAlt = e.altKey === true;
        this.fCtrl = e.ctrlKey === true;
        this.fShift = e.shiftKey === true;
        this.fMeta = e.metaKey === true;
        this.fButton = tp.Mouse.Button(e);
        this.fButtons = tp.Mouse.Buttons(e);
        this.fDocument = tp.Mouse.ToDocument(e);
        this.fViewport = tp.Mouse.ToViewport(e);
        this.fScreen = tp.Mouse.ToScreen(e);
    }
    /**
     * Returns the absolute viewport position difference between this instance and an event.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {tp.Point} Returns the absolute difference.
     */
    Dif(e) {
        var P = tp.Mouse.ToViewport(e);
        return new tp.Point(Math.abs(P.X - this.Viewport.X), Math.abs(P.Y - this.Viewport.Y));
    }
};

// ● prototype
/**
 * Private field.
 * @type {boolean}
 */
tp.MouseInfo.prototype.fAlt = false;
/**
 * Private field.
 * @type {boolean}
 */
tp.MouseInfo.prototype.fCtrl = false;
/**
 * Private field.
 * @type {boolean}
 */
tp.MouseInfo.prototype.fShift = false;
/**
 * Private field.
 * @type {boolean}
 */
tp.MouseInfo.prototype.fMeta = false;
/**
 * Private field.
 * @type {number}
 */
tp.MouseInfo.prototype.fButton = 0;
/**
 * Private field.
 * @type {number}
 */
tp.MouseInfo.prototype.fButtons = 0;
/**
 * Private field.
 * @type {tp.Point}
 */
tp.MouseInfo.prototype.fDocument = null;
/**
 * Private field.
 * @type {tp.Point}
 */
tp.MouseInfo.prototype.fViewport = null;
/**
 * Private field.
 * @type {tp.Point}
 */
tp.MouseInfo.prototype.fScreen = null;
