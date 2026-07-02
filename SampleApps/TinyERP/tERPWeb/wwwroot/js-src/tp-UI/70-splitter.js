// ● splitter
/**
 * A splitter bar placed between two sibling panels.
 * @example
 * <div id="Container" style="display: flex;">
 *     <div id="Panel1">Panel 1</div>
 *     <div class="tp-Splitter"></div>
 *     <div id="Panel2">Panel 2</div>
 * </div>
 * <script>
 *     var Splitter = new tp.Splitter(".tp-Splitter");
 * </script>
 */
tp.Splitter = class extends tp.Component {
    // ● constructor
    /**
     * Creates a splitter.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The splitter create parameters, handle, or selector.
     */
    constructor(CreateParams) {
        super(CreateParams);
        this.Initialize();
    }

    // ● protected
    /**
     * Initializes this splitter after handle creation and create parameter application.
     * @returns {void}
     */
    Initialize() {
        this.tpClass = "tp.Splitter";
        tp.AddClass(this.Handle, tp.Classes.Splitter);
        this.FindPanels();
        this.ValidatePanels();
        this.NormalizeSizeLimits();
        this.fDragContext = new tp.DragContext(this.Handle, this);
        this.SetHorizontal();
    }
    /**
     * Finds the sibling panels around the splitter handle.
     * @returns {void}
     */
    FindPanels() {
        var List;
        var Index;
        this.Panel1 = null;
        this.Panel2 = null;
        if (tp.IsHTMLElement(this.ParentHandle)) {
            List = tp.ChildHTMLElements(this.ParentHandle);
            Index = List.indexOf(this.Handle);
            if (Index !== -1) {
                if (Index - 1 >= 0)
                    this.Panel1 = List[Index - 1];
                if (this.UseBothPanels === true && Index + 1 <= List.length - 1)
                    this.Panel2 = List[Index + 1];
            }
        }
    }
    /**
     * Validates that required panels exist.
     * @returns {void}
     */
    ValidatePanels() {
        if (!tp.IsHTMLElement(this.Panel1))
            tp.Throw("Splitter Panel1 is not found.");
        if (this.UseBothPanels === true && !tp.IsHTMLElement(this.Panel2))
            tp.Throw("Splitter Panel2 is not found.");
    }
    /**
     * Normalizes panel size constraints.
     * @returns {void}
     */
    NormalizeSizeLimits() {
        this.Panel1MinSize = tp.IsNumber(this.Panel1MinSize) ? this.Panel1MinSize : 100;
        this.Panel1MaxSize = tp.IsNumber(this.Panel1MaxSize) ? this.Panel1MaxSize : 500;
        this.Panel2MinSize = tp.IsNumber(this.Panel2MinSize) ? this.Panel2MinSize : 100;
    }
    /**
     * Applies the orientation CSS classes.
     * @returns {void}
     */
    SetHorizontal() {
        if (this.HasHandle) {
            if (this.IsHorizontal) {
                tp.RemoveClass(this.Handle, tp.Classes.Vertical);
                tp.AddClass(this.Handle, tp.Classes.Horizontal);
            } else {
                tp.RemoveClass(this.Handle, tp.Classes.Horizontal);
                tp.AddClass(this.Handle, tp.Classes.Vertical);
            }
        }
    }
    /**
     * Returns true when the splitter can move.
     * @returns {boolean} Returns true when the splitter can move.
     */
    CanMoveSplitter() {
        var Result = tp.IsHTMLElement(this.Panel1);
        if (this.UseBothPanels === true)
            Result = Result && tp.IsHTMLElement(this.Panel2);
        if (Result) {
            if (this.IsHorizontal) {
                Result = this.Panel1.offsetHeight > this.Panel1MinSize;
                if (this.UseBothPanels === true)
                    Result = Result && this.Panel2.offsetHeight > this.Panel2MinSize;
                else
                    Result = Result && this.Panel1.offsetHeight < this.Panel1MaxSize;
            } else {
                Result = this.Panel1.offsetWidth > this.Panel1MinSize;
                if (this.UseBothPanels === true)
                    Result = Result && this.Panel2.offsetWidth > this.Panel2MinSize;
                else
                    Result = Result && this.Panel1.offsetWidth < this.Panel1MaxSize;
            }
        }
        return Result;
    }
    /**
     * Returns true when Panel1 can be resized to a specified mouse position.
     * @param {number} MousePos The mouse position inside the parent element.
     * @returns {boolean} Returns true when resizing is allowed.
     */
    CanResizePanel(MousePos) {
        var ParentSize;
        var SplitterSize = this.IsHorizontal ? this.Handle.offsetHeight : this.Handle.offsetWidth;
        var Result = MousePos > this.Panel1MinSize + SplitterSize;
        if (Result) {
            if (this.UseBothPanels === true) {
                ParentSize = this.IsHorizontal ? this.ParentHandle.offsetHeight : this.ParentHandle.offsetWidth;
                Result = ParentSize - MousePos > this.Panel2MinSize + SplitterSize;
            } else {
                Result = MousePos <= this.Panel1MaxSize;
            }
        }
        return Result;
    }

    // ● properties
    /**
     * Gets or sets a value indicating whether this splitter is horizontal.
     * @returns {boolean} Returns true when this splitter is horizontal.
     */
    get IsHorizontal() {
        return this.fIsHorizontal === true;
    }
    /**
     * Gets or sets a value indicating whether this splitter is horizontal.
     * @param {boolean} Value True for horizontal; false for vertical.
     * @returns {void}
     */
    set IsHorizontal(Value) {
        Value = Value === true;
        if (Value !== this.IsHorizontal) {
            this.fIsHorizontal = Value;
            this.SetHorizontal();
        }
    }

    // ● drag context listener
    /**
     * Called by tp.DragContext to decide if dragging should start.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {boolean} Returns true to start dragging.
     */
    IsDragStart(e) {
        return this.CanMoveSplitter();
    }
    /**
     * Called by tp.DragContext when dragging starts.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragStart(e) {
        tp.AddClass(this.Document.body, tp.Classes.UnSelectable);
        this.fOldCursor = this.Document.body.style.cursor;
        this.Document.body.style.cursor = this.IsHorizontal ? tp.Cursors.ResizeRow : tp.Cursors.ResizeCol;
    }
    /**
     * Called by tp.DragContext while dragging.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragMove(e) {
        var P = tp.Mouse.ToElement(e, this.ParentHandle);
        var Pos;
        if (this.IsHorizontal) {
            Pos = P.Y;
            if (this.CanResizePanel(Pos)) {
                this.Panel1.style.height = tp.px(Pos);
                this.Panel1.style.minHeight = this.Panel1.style.height;
            }
        } else {
            Pos = P.X;
            if (this.CanResizePanel(Pos)) {
                this.Panel1.style.width = tp.px(Pos);
                this.Panel1.style.minWidth = this.Panel1.style.width;
            }
        }
    }
    /**
     * Called by tp.DragContext when dragging ends.
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @returns {void}
     */
    DragEnd(e) {
        tp.RemoveClass(this.Document.body, tp.Classes.UnSelectable);
        this.Document.body.style.cursor = this.fOldCursor;
    }

    // ● public
    /**
     * Disposes this splitter.
     * @returns {void}
     */
    Dispose() {
        if (this.fDragContext) {
            this.fDragContext.Dispose();
            this.fDragContext = null;
        }
        super.Dispose();
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Splitter.prototype.tpClass = "tp.Splitter";
/**
 * Private field.
 * @type {tp.DragContext|null}
 */
tp.Splitter.prototype.fDragContext = null;
/**
 * Private field.
 * @type {boolean}
 */
tp.Splitter.prototype.fIsHorizontal = false;
/**
 * Private field.
 * @type {string}
 */
tp.Splitter.prototype.fOldCursor = "";
/**
 * True to constrain movement using both panels; false to constrain only Panel1.
 * @type {boolean}
 */
tp.Splitter.prototype.UseBothPanels = true;
/**
 * The panel before the splitter.
 * @type {HTMLElement|null}
 */
tp.Splitter.prototype.Panel1 = null;
/**
 * The panel after the splitter.
 * @type {HTMLElement|null}
 */
tp.Splitter.prototype.Panel2 = null;
/**
 * The minimum size of Panel1.
 * @type {number}
 */
tp.Splitter.prototype.Panel1MinSize = 40;
/**
 * The maximum size of Panel1 when UseBothPanels is false.
 * @type {number}
 */
tp.Splitter.prototype.Panel1MaxSize = 400;
/**
 * The minimum size of Panel2 when UseBothPanels is true.
 * @type {number}
 */
tp.Splitter.prototype.Panel2MinSize = 40;

tp.Ui.RegisterType(["Splitter", "tp-Splitter"], tp.Splitter);
