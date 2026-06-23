// ● screen mode
/**
 * Screen size mode constants.
 * @type {object}
 */
tp.ScreenMode = {
    None: 0,
    XSmall: 1,
    Small: 2,
    Medium: 4,
    Large: 8,
    XLarge: 16,
    XXLarge: 32
};
Object.freeze(tp.ScreenMode);

// ● screen widths
/**
 * Maximum viewport widths for screen modes.
 * @type {object}
 */
tp.ScreenWidthsMax = {
    XSmall: 575.98,
    Small: 767.98,
    Medium: 991.98,
    Large: 1199.98,
    XLarge: 1399.98
};
Object.freeze(tp.ScreenWidthsMax);

// ● viewport
/**
 * Viewport size and screen mode helper.
 * There are two viewport concepts: layout viewport and visual viewport.
 * See: https://developer.mozilla.org/en-US/docs/Glossary/Layout_viewport
 * See: https://developer.mozilla.org/en-US/docs/Glossary/Visual_Viewport
 * See: https://www.quirksmode.org/mobile/viewports.html
 * @type {object}
 */
tp.Viewport = {
    Initialized: false,
    OldMode: tp.ScreenMode.None,
    Listeners: [],
    /**
     * Initializes viewport resize tracking.
     * @returns {void}
     */
    Initialize: function () {
        if (!tp.Viewport.Initialized) {
            tp.Viewport.Initialized = true;
            tp.Viewport.OldMode = tp.Viewport.Mode;
            window.addEventListener("resize", function () {
                tp.Viewport.ScreenSizeChanged();
            }, false);
        }
    },
    /**
     * Returns the viewport size.
     * @returns {tp.Size} Returns the viewport size.
     */
    GetSize: function () {
        return new tp.Size(tp.Viewport.Width, tp.Viewport.Height);
    },
    /**
     * Returns the viewport top-left offset relative to the rendered document.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/Window/scrollX
     * @returns {tp.Point} Returns the page offset.
     */
    GetPageOffset: function () {
        return new tp.Point(
            window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft || 0,
            window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0
        );
    },
    /**
     * Centers an element in the viewport.
     * The element position should be fixed or absolute.
     * @param {Element|string} Selector The target selector or element.
     * @returns {void}
     */
    CenterInWindow: function (Selector) {
        var Element = tp(Selector);
        var Rect;
        var Left;
        var Top;
        var Style;
        if (!tp.IsElement(Element))
            return;
        Rect = Element.getBoundingClientRect();
        Left = Math.round((tp.Viewport.Width / 2) - (Rect.width / 2));
        Top = Math.round((tp.Viewport.Height / 2) - (Rect.height / 2));
        Style = tp.GetComputedStyle(Element);
        if (Style && Style.position === "absolute")
            Top += tp.Viewport.GetPageOffset().Y;
        Element.style.left = Left + "px";
        Element.style.top = Top + "px";
    },
    /**
     * Notifies viewport listeners that the viewport size changed.
     * @returns {void}
     */
    ScreenSizeChanged: function () {
        var NewMode = tp.Viewport.Mode;
        var ModeChanged = NewMode !== tp.Viewport.OldMode;
        var List = tp.Viewport.Listeners.slice();
        var Listener;
        var i;
        if (ModeChanged)
            tp.Viewport.OldMode = NewMode;
        for (i = 0; i < List.length; i++) {
            Listener = List[i];
            if (Listener && tp.IsFunction(Listener.Func))
                Listener.Func.call(Listener.Context || null, ModeChanged);
        }
    },
    /**
     * Adds a viewport resize listener.
     * @param {Function} Func The callback to execute. It receives a boolean indicating whether the screen mode changed.
     * @param {object|null|undefined} Context The optional callback context.
     * @returns {tp.Listener} Returns the listener object.
     */
    AddListener: function (Func, Context) {
        var Listener;
        if (!tp.IsFunction(Func))
            tp.Throw("Func is not a function.");
        Listener = new tp.Listener(Func, Context || null, false);
        tp.Viewport.Listeners.push(Listener);
        return Listener;
    },
    /**
     * Removes a viewport resize listener.
     * @param {tp.Listener} Listener The listener to remove.
     * @returns {void}
     */
    RemoveListener: function (Listener) {
        var Index = tp.Viewport.Listeners.indexOf(Listener);
        if (Index !== -1)
            tp.Viewport.Listeners.splice(Index, 1);
    },
    /**
     * Gets the current screen mode.
     * @returns {number} Returns a tp.ScreenMode value.
     */
    get Mode() {
        var Width = tp.Viewport.Width;
        if (Width <= tp.ScreenWidthsMax.XSmall)
            return tp.ScreenMode.XSmall;
        if (Width <= tp.ScreenWidthsMax.Small)
            return tp.ScreenMode.Small;
        if (Width <= tp.ScreenWidthsMax.Medium)
            return tp.ScreenMode.Medium;
        if (Width <= tp.ScreenWidthsMax.Large)
            return tp.ScreenMode.Large;
        if (Width <= tp.ScreenWidthsMax.XLarge)
            return tp.ScreenMode.XLarge;
        return tp.ScreenMode.XXLarge;
    },
    /**
     * Returns true when the viewport is extra small.
     * @returns {boolean} Returns true when the viewport is extra small.
     */
    get IsXSmall() {
        return tp.Viewport.Mode === tp.ScreenMode.XSmall;
    },
    /**
     * Returns true when the viewport is small.
     * @returns {boolean} Returns true when the viewport is small.
     */
    get IsSmall() {
        return tp.Viewport.Mode === tp.ScreenMode.Small;
    },
    /**
     * Returns true when the viewport is medium.
     * @returns {boolean} Returns true when the viewport is medium.
     */
    get IsMedium() {
        return tp.Viewport.Mode === tp.ScreenMode.Medium;
    },
    /**
     * Returns true when the viewport is large.
     * @returns {boolean} Returns true when the viewport is large.
     */
    get IsLarge() {
        return tp.Viewport.Mode === tp.ScreenMode.Large;
    },
    /**
     * Returns true when the viewport is extra large.
     * @returns {boolean} Returns true when the viewport is extra large.
     */
    get IsXLarge() {
        return tp.Viewport.Mode === tp.ScreenMode.XLarge;
    },
    /**
     * Returns true when the viewport is extra extra large.
     * @returns {boolean} Returns true when the viewport is extra extra large.
     */
    get IsXXLarge() {
        return tp.Viewport.Mode === tp.ScreenMode.XXLarge;
    },
    /**
     * Gets the layout viewport width.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/Window/innerWidth
     * @returns {number} Returns the viewport width.
     */
    get Width() {
        var Body = document.body || document.getElementsByTagName("body")[0];
        return window.innerWidth && document.documentElement.clientWidth ?
            Math.min(window.innerWidth, document.documentElement.clientWidth) :
            window.innerWidth ||
            document.documentElement.clientWidth ||
            (Body ? Body.clientWidth : 0);
    },
    /**
     * Gets the layout viewport height.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/Window/innerHeight
     * @returns {number} Returns the viewport height.
     */
    get Height() {
        var Body = document.body || document.getElementsByTagName("body")[0];
        return window.innerHeight && document.documentElement.clientHeight ?
            Math.min(window.innerHeight, document.documentElement.clientHeight) :
            window.innerHeight ||
            document.documentElement.clientHeight ||
            (Body ? Body.clientHeight : 0);
    }
};
