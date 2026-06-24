// ● resize detector
/**
 * Detects size changes in an HTMLElement and sends notifications to a listener function.
 * Uses the ResizeObserver API.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/Resize_Observer_API|MDN Resize Observer API}
 */
tp.ResizeDetector = class {
    // ● constructor
    /**
     * Creates a resize detector.
     * @param {string|HTMLElement} SelectorOrElement The element to observe.
     * @param {Function} OnResizeFunc A callback receiving { Width: boolean, Height: boolean }.
     * @param {object|null|undefined} Context The callback context.
     * @param {boolean|null|undefined} ImmediateStart True to start observing immediately.
     */
    constructor(SelectorOrElement, OnResizeFunc, Context, ImmediateStart) {
        this.Element = tp.Select(SelectorOrElement);
        this.OnResizeFunc = OnResizeFunc;
        this.Context = Context || null;
        this.Observer = null;
        this.Width = 0;
        this.Height = 0;
        this.fObserving = false;
        if (!tp.IsHTMLElement(this.Element))
            tp.Throw("ResizeDetector requires an HTMLElement.");
        if (!tp.IsFunction(this.OnResizeFunc))
            tp.Throw("ResizeDetector requires a callback function.");
        if (typeof ResizeObserver === "undefined")
            tp.Throw("ResizeObserver is not available.");
        this.Observer = new ResizeObserver(this.ObserverCallback.bind(this));
        if (ImmediateStart !== false)
            this.Start();
    }

    // ● protected
    /**
     * Reads the size from a ResizeObserver entry.
     * @param {ResizeObserverEntry} Entry The observer entry.
     * @returns {tp.Size} Returns the observed size.
     * @protected
     */
    GetEntrySize(Entry) {
        var BoxSize;
        var Rect;
        if (Entry && Entry.borderBoxSize) {
            BoxSize = tp.IsArray(Entry.borderBoxSize) ? Entry.borderBoxSize[0] : Entry.borderBoxSize;
            if (BoxSize)
                return new tp.Size(BoxSize.inlineSize, BoxSize.blockSize);
        }
        Rect = Entry && Entry.contentRect ? Entry.contentRect : this.Element.getBoundingClientRect();
        return new tp.Size(Rect.width, Rect.height);
    }
    /**
     * Observer callback.
     * @param {ResizeObserverEntry[]} Entries The observer entries.
     * @param {ResizeObserver} Observer The observer instance.
     * @returns {void}
     * @protected
     */
    ObserverCallback(Entries, Observer) {
        var Size;
        var ResizeInfo;
        if (!Entries || Entries.length === 0)
            return;
        Size = this.GetEntrySize(Entries[0]);
        if (Size.Width !== this.Width || Size.Height !== this.Height) {
            ResizeInfo = {
                Width: Size.Width !== this.Width,
                Height: Size.Height !== this.Height
            };
            this.Width = Size.Width;
            this.Height = Size.Height;
            tp.Call(this.OnResizeFunc, this.Context, ResizeInfo);
        }
    }

    // ● public
    /**
     * Starts observing the element.
     * @returns {void}
     */
    Start() {
        var Options;
        if (!this.fObserving) {
            this.Width = this.Element.offsetWidth;
            this.Height = this.Element.offsetHeight;
            Options = { box: "border-box" };
            this.Observer.observe(this.Element, Options);
            this.fObserving = true;
        }
    }
    /**
     * Stops observing the element.
     * @returns {void}
     */
    Stop() {
        if (this.fObserving) {
            this.Observer.unobserve(this.Element);
            this.Observer.disconnect();
            this.fObserving = false;
        }
    }
    /**
     * Stops observing and clears references.
     * @returns {void}
     */
    Dispose() {
        this.Stop();
        this.Observer = null;
        this.Element = null;
        this.OnResizeFunc = null;
        this.Context = null;
    }

    // ● properties
    /**
     * Returns true while observing.
     * @returns {boolean} Returns true while observing.
     */
    get Observing() {
        return this.fObserving;
    }
};

// ● prototype
/**
 * The observer.
 * @type {ResizeObserver|null}
 */
tp.ResizeDetector.prototype.Observer = null;
/**
 * The observed element.
 * @type {HTMLElement|null}
 */
tp.ResizeDetector.prototype.Element = null;
/**
 * The resize callback.
 * @type {Function|null}
 */
tp.ResizeDetector.prototype.OnResizeFunc = null;
/**
 * The callback context.
 * @type {object|null}
 */
tp.ResizeDetector.prototype.Context = null;
/**
 * The last observed width.
 * @type {number}
 */
tp.ResizeDetector.prototype.Width = 0;
/**
 * The last observed height.
 * @type {number}
 */
tp.ResizeDetector.prototype.Height = 0;
/**
 * True while observing.
 * @type {boolean}
 */
tp.ResizeDetector.prototype.fObserving = false;
