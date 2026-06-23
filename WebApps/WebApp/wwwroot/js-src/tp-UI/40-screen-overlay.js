// ● screen overlay
/**
 * Displays an overlay element over the viewport.
 */
tp.ScreenOverlay = class extends tp.Component {
    // ● constructor
    /**
     * Creates a screen overlay.
     * @param {HTMLElement|string|null|undefined} Parent The optional parent element. Defaults to document.body.
     */
    constructor(Parent) {
        super(tp.ScreenOverlay.CreateParams(Parent));
        this.BringToFront();
    }

    // ● protected
    /**
     * Creates component create params for a screen overlay.
     * @param {HTMLElement|string|null|undefined} Parent The optional parent element.
     * @returns {tp.CreateParams} Returns the create params.
     */
    static CreateParams(Parent) {
        var ParentElement = tp(Parent) || document.body;
        var Element = ParentElement.ownerDocument.createElement("div");
        return new tp.CreateParams({
            Handle: Element,
            Parent: ParentElement,
            Id: tp.SafeId(tp.Classes.ScreenOverlay),
            CssClasses: tp.Classes.ScreenOverlay
        });
    }

    // ● properties
    /**
     * Gets the overlay z-index.
     * @returns {number} Returns the z-index.
     */
    get ZIndex() {
        return tp.ToInt(this.Handle ? this.Handle.style.zIndex : 0);
    }
    /**
     * Sets the overlay z-index.
     * @param {number} Value The z-index.
     * @returns {void}
     */
    set ZIndex(Value) {
        if (this.Handle)
            this.Handle.style.zIndex = String(tp.ToInt(Value));
    }
    /**
     * Gets a value indicating whether the overlay is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle ? this.Handle.style.display !== "none" : false;
    }
    /**
     * Sets a value indicating whether the overlay is visible.
     * @param {boolean} Value True to show; false to hide.
     * @returns {void}
     */
    set Visible(Value) {
        if (this.Handle) {
            this.Handle.style.display = Value === true ? "flex" : "none";
            if (Value === true)
                this.BringToFront();
        }
    }

    // ● public
    /**
     * Brings the overlay to front.
     * @returns {number} Returns the assigned z-index.
     */
    BringToFront() {
        tp.ScreenOverlay.fLastZIndex++;
        this.ZIndex = tp.ScreenOverlay.fLastZIndex;
        return this.ZIndex;
    }
    /**
     * Shows the overlay.
     * @returns {HTMLElement|null} Returns the overlay handle.
     */
    Show() {
        this.Visible = true;
        return this.Handle;
    }
    /**
     * Hides the overlay.
     * @returns {HTMLElement|null} Returns the overlay handle.
     */
    Hide() {
        this.Visible = false;
        return this.Handle;
    }
};

// ● static fields
/**
 * Last overlay z-index.
 * @type {number}
 */
tp.ScreenOverlay.fLastZIndex = 10000;

// ● prototype
/**
 * Gets the class name.
 * @type {string}
 */
tp.ScreenOverlay.prototype.tpClass = "tp.ScreenOverlay";
