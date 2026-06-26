// ● virtual scroller
/**
 * A virtual scroller helper for fixed-height rows.
 *
 * The scroller uses a viewport HTMLElement which contains and scrolls a child container.
 * The container is where rows are rendered.
 */
tp.VirtualScroller = class {
    // ● constructor
    /**
     * Creates a virtual scroller.
     * @param {HTMLElement} Viewport The scrolling viewport.
     * @param {HTMLElement|null|undefined} Container Optional row container.
     * @param {object[]|null|undefined} RowList Optional row list.
     */
    constructor(Viewport, Container, RowList) {
        if (!tp.IsHTMLElement(Viewport))
            tp.Throw("VirtualScroller requires a viewport HTMLElement.");
        this.fViewport = Viewport;
        if (!tp.IsHTMLElement(Container)) {
            Container = Viewport.ownerDocument.createElement("div");
            Container.tabIndex = -1;
            Viewport.appendChild(Container);
        }
        this.fContainer = Container;
        this.RowList = tp.IsArray(RowList) ? RowList : [];
        this.ContainerHeight = 0;
        this.LastScrollTop = 0;
        this.RowCache = {};
        this.IndexTop = 0;
        this.IndexBottom = 0;
        this.RowHeight = null;
        this.Context = null;
        this.RenderBind = this.Render.bind(this);
        this.fViewport.style.overflow = "auto";
        this.fContainer.style.position = "relative";
        this.fContainer.style.overflow = "hidden";
        this.fViewport.addEventListener("scroll", this.RenderBind, false);
    }

    // ● protected
    /**
     * Clears rendered row elements.
     * @returns {void}
     */
    ClearCache() {
        var Prop;
        var Element;
        for (Prop in this.RowCache) {
            if (Object.prototype.propertyIsEnumerable.call(this.RowCache, Prop)) {
                Element = this.RowCache[Prop];
                if (Element && Element.parentNode)
                    Element.parentNode.removeChild(Element);
                delete this.RowCache[Prop];
            }
        }
    }
    /**
     * Renders a row element.
     * @param {number} RowIndex The row index.
     * @returns {HTMLElement} Returns the row element.
     */
    RenderRow(RowIndex) {
        var Row = this.RowList[RowIndex];
        return tp.Call(this.RenderRowFunc, this.Context || this, Row, RowIndex, this.RowHeight);
    }
    /**
     * Renders visible rows.
     * @returns {void}
     */
    Render() {
        var Height;
        var TopPosition;
        var Rect;
        var BottomPosition;
        var Top;
        var Bottom;
        var Prop;
        var Element;
        var Index;
        var Length;
        if (!this.RowList || this.RowList.length === 0 || this.LastScrollTop === this.fViewport.scrollTop)
            return;
        tp.Call(this.ScrollFunc, this.Context || this, 1);
        this.LastScrollTop = this.fViewport.scrollTop;
        Height = this.ContainerHeight;
        TopPosition = this.fViewport.scrollTop;
        Rect = this.fViewport.getBoundingClientRect();
        BottomPosition = TopPosition + Rect.height;
        Top = Math.abs(Math.floor(TopPosition / this.RowHeight)) - 5;
        Top = Math.max(0, Top);
        Bottom = Math.abs(Math.ceil(BottomPosition / this.RowHeight)) + 5;
        Bottom = Math.min(Height / this.RowHeight, Bottom);
        for (Prop in this.RowCache) {
            if (Object.prototype.propertyIsEnumerable.call(this.RowCache, Prop)) {
                Index = Number(Prop);
                if (Index < Top || Index > Bottom) {
                    Element = this.RowCache[Prop];
                    if (Element && Element.parentNode)
                        Element.parentNode.removeChild(Element);
                    delete this.RowCache[Prop];
                }
            }
        }
        Length = this.RowList.length;
        for (Index = Top; Index <= Bottom; Index++) {
            if (Index >= 0 && Index <= Length - 1 && !this.RowCache[Index]) {
                Element = this.RenderRow(Index);
                Element.style.position = "absolute";
                Element.style.top = (Index * this.RowHeight) + "px";
                Element.style.height = this.RowHeight + "px";
                Element.style.width = "100%";
                this.fContainer.appendChild(Element);
                this.RowCache[Index] = Element;
            }
        }
        this.IndexTop = Top;
        this.IndexBottom = Bottom;
        tp.Call(this.ScrollFunc, this.Context || this, 2);
    }

    // ● overridables
    /**
     * Renders a row and returns an HTMLElement.
     * @param {*} Row The row item.
     * @param {number} RowIndex The row index.
     * @param {number} RowHeight The row height.
     * @returns {HTMLElement} Returns the row element.
     */
    RenderRowFunc(Row, RowIndex, RowHeight) {
        var Element = this.Viewport.ownerDocument.createElement("div");
        Element.tabIndex = -1;
        tp.SetStyle(Element, {
            "border-bottom": "1px dotted lightgray",
            left: "0",
            "font-size": "9pt",
            width: "100%",
            height: RowHeight + "px",
            padding: "4px"
        });
        Element.innerHTML = "row " + RowIndex;
        return Element;
    }
    /**
     * Callback called before and after rendering.
     * @param {number} Phase The render phase. 1 is before, 2 is after.
     * @returns {void}
     */
    ScrollFunc(Phase) {
    }

    // ● public
    /**
     * Sets the row list to display.
     * @param {object[]|null|undefined} RowList The row list.
     * @returns {void}
     */
    SetRowList(RowList) {
        var Element;
        this.ClearCache();
        this.IndexTop = 0;
        this.IndexBottom = 0;
        this.LastScrollTop = null;
        this.fViewport.scrollTop = 0;
        this.RowList = tp.IsArray(RowList) ? RowList : [];
        if (tp.IsEmpty(this.RowHeight) && this.RowList.length > 0) {
            Element = this.RenderRow(0);
            this.fContainer.appendChild(Element);
            this.RowHeight = Element.getBoundingClientRect().height;
            this.fContainer.removeChild(Element);
        }
        if (tp.IsEmpty(this.RowHeight))
            this.RowHeight = 32;
        if (!tp.IsEmpty(this.RowList)) {
            this.ContainerHeight = this.RowList.length * this.RowHeight;
            this.fContainer.style.height = this.ContainerHeight + "px";
            this.Render();
        }
    }
    /**
     * Returns a copy of the row list.
     * @returns {object[]} Returns the row list.
     */
    GetRowList() {
        return !tp.IsEmpty(this.RowList) ? this.RowList.slice() : [];
    }
    /**
     * Forces a full re-render.
     * @returns {void}
     */
    Update() {
        this.ClearCache();
        this.IndexTop = 0;
        this.IndexBottom = 0;
        this.LastScrollTop = null;
        this.fViewport.scrollTop = 0;
        if (!tp.IsEmpty(this.RowList)) {
            this.ContainerHeight = this.RowList.length * this.RowHeight;
            this.fContainer.style.height = this.ContainerHeight + "px";
            this.Render();
        }
    }
    /**
     * Releases event handlers and rendered rows.
     * @returns {void}
     */
    Dispose() {
        if (this.fViewport && this.RenderBind)
            this.fViewport.removeEventListener("scroll", this.RenderBind, false);
        this.ClearCache();
        this.RenderBind = null;
        this.fViewport = null;
        this.fContainer = null;
        this.RowList = [];
        this.RowCache = {};
    }

    // ● properties
    /**
     * Gets the viewport element.
     * @returns {HTMLElement|null} Returns the viewport.
     */
    get Viewport() {
        return this.fViewport;
    }
    /**
     * Gets the container element.
     * @returns {HTMLElement|null} Returns the container.
     */
    get Container() {
        return this.fContainer;
    }
    /**
     * Gets the row count.
     * @returns {number} Returns the row count.
     */
    get RowListCount() {
        return tp.IsArray(this.RowList) ? this.RowList.length : 0;
    }
};
