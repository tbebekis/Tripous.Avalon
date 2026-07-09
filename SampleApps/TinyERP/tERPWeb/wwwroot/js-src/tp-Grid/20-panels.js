// ● group panel
/**
 * Internal grid group panel.
 */
tp.GridGroupPanel = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid group panel.
     * @param {tp.Grid} Grid The owner grid.
     * @param {HTMLElement} Parent The parent element.
     */
    constructor(Grid, Parent) {
        super();
        this.Grid = Grid;
        this.Handle = Grid.Document.createElement("div");
        Parent.appendChild(this.Handle);
        this.Handle.className = tp.Classes.Groups;
        this.Handle.style.height = tp.px(this.Grid.GroupPanelHeight);
        this.Handle.style.minHeight = this.Handle.style.height;
        /**
         * The text element shown when the panel has no group columns.
         * @type {HTMLElement}
         */
        this.EmptyTextElement = Grid.Document.createElement("div");
        this.EmptyTextElement.className = tp.Classes.GroupPanelEmptyText;
        this.EmptyTextElement.style.boxSizing = "border-box";
        this.EmptyTextElement.style.width = "100%";
        this.EmptyTextElement.style.padding = "0 8px";
        this.EmptyTextElement.style.overflow = "hidden";
        this.EmptyTextElement.style.whiteSpace = "nowrap";
        this.EmptyTextElement.style.textOverflow = "ellipsis";
        this.EmptyTextElement.style.opacity = "0.65";
        this.EmptyTextElement.style.pointerEvents = "none";
        this.Handle.appendChild(this.EmptyTextElement);
        this.UpdateEmptyText();
        tp.On(this.Handle, tp.Events.DragEnter, this.FuncBind(this.DragEnter));
        tp.On(this.Handle, tp.Events.DragOver, this.FuncBind(this.DragOver));
        tp.On(this.Handle, tp.Events.DragLeave, this.FuncBind(this.DragLeave));
        tp.On(this.Handle, tp.Events.DragDrop, this.FuncBind(this.DragDrop));
    }

    // ● protected
    /**
     * Handles drag enter.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragEnter(e) {
    }
    /**
     * Handles drag over.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragOver(e) {
        var Column = this.Grid.DraggedColumn;
        if (Column) {
            if (e.preventDefault)
                e.preventDefault();
            if (e.dataTransfer)
                e.dataTransfer.dropEffect = "move";
            e.returnValue = false;
        }
    }
    /**
     * Handles drag leave.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragLeave(e) {
    }
    /**
     * Handles drop.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragDrop(e) {
        var Column = this.Grid.DraggedColumn;
        var RefColumn = null;
        var Point;
        var Rect;
        var Index;
        var GroupColumn;
        if (Column) {
            if (e.preventDefault)
                e.preventDefault();
            if (this.Grid.GroupColumnCount > 0) {
                Point = tp.Mouse.ToElement(e, this.Handle);
                for (Index = 0; Index < this.Grid.GroupColumnCount; Index++) {
                    GroupColumn = this.Grid.GroupColumnByIndex(Index);
                    Rect = tp.OffsetRect(GroupColumn.Handle);
                    if (Rect.Contains(Point)) {
                        RefColumn = GroupColumn;
                        break;
                    }
                }
            }
            this.Grid.ColumnGrouped(Column, RefColumn);
            e.returnValue = false;
        }
    }

    // ● public
    /**
     * Clears this panel.
     * @returns {void}
     */
    Clear() {
        super.Clear();
        if (this.Handle) {
            this.Handle.innerHTML = "";
            this.Handle.appendChild(this.EmptyTextElement);
            this.UpdateEmptyText();
        }
    }
    /**
     * Updates the empty group panel text.
     * @returns {void}
     */
    UpdateEmptyText() {
        if (!(this.EmptyTextElement instanceof HTMLElement))
            return;
        this.EmptyTextElement.textContent = this.Grid.EmptyGroupPanelText || "";
        this.EmptyTextElement.style.display = this.Grid.GroupColumnCount === 0 && !tp.IsBlank(this.Grid.EmptyGroupPanelText) ? "" : "none";
    }

    // ● properties
    /**
     * Gets or sets whether this panel is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle.style.display === "";
    }
    /**
     * Gets or sets whether this panel is visible.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        this.Handle.style.display = Value === true ? "" : "none";
    }
};

// ● column panel
/**
 * Internal grid column panel.
 */
tp.GridColumnPanel = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid column panel.
     * @param {tp.Grid} Grid The owner grid.
     * @param {HTMLElement} Parent The parent element.
     */
    constructor(Grid, Parent) {
        super();
        this.Grid = Grid;
        this.Handle = Grid.Document.createElement("div");
        Parent.appendChild(this.Handle);
        this.Handle.className = tp.Classes.Columns;
        this.Handle.style.height = tp.px(this.Grid.ColumnHeight);
        this.Handle.style.minHeight = this.Handle.style.height;
        this.Content = this.Handle.ownerDocument.createElement("div");
        this.Handle.appendChild(this.Content);
        this.Content.className = tp.Classes.Content;
    }

    // ● public
    /**
     * Clears this panel.
     * @returns {void}
     */
    Clear() {
        super.Clear();
        if (this.Content)
            this.Content.innerHTML = "";
    }

    // ● properties
    /**
     * Gets or sets the horizontal scroll position.
     * @returns {number} Returns the scroll position.
     */
    get ScrollLeft() {
        return this.Handle.scrollLeft;
    }
    /**
     * Gets or sets the horizontal scroll position.
     * @param {number} Value The scroll position.
     * @returns {void}
     */
    set ScrollLeft(Value) {
        this.Handle.scrollLeft = Value;
    }
    /**
     * Gets or sets whether this panel is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle.style.display === "";
    }
    /**
     * Gets or sets whether this panel is visible.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        this.Handle.style.display = Value === true ? "" : "none";
    }
};

// ● filter panel
/**
 * Internal grid filter panel.
 */
tp.GridFilterPanel = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid filter panel.
     * @param {tp.Grid} Grid The owner grid.
     * @param {HTMLElement} Parent The parent element.
     */
    constructor(Grid, Parent) {
        super();
        this.Grid = Grid;
        this.Handle = Grid.Document.createElement("div");
        Parent.appendChild(this.Handle);
        this.Handle.className = tp.Classes.Filters;
        this.Handle.style.height = tp.px(this.Grid.RowHeight);
        this.Handle.style.minHeight = this.Handle.style.height;
        this.Content = this.Handle.ownerDocument.createElement("div");
        this.Handle.appendChild(this.Content);
        this.Content.className = tp.Classes.Content;
    }

    // ● public
    /**
     * Clears this panel.
     * @returns {void}
     */
    Clear() {
        super.Clear();
        if (this.Content)
            this.Content.innerHTML = "";
    }

    // ● properties
    /**
     * Gets or sets the horizontal scroll position.
     * @returns {number} Returns the scroll position.
     */
    get ScrollLeft() {
        return this.Handle.scrollLeft;
    }
    /**
     * Gets or sets the horizontal scroll position.
     * @param {number} Value The scroll position.
     * @returns {void}
     */
    set ScrollLeft(Value) {
        this.Handle.scrollLeft = Value;
    }
    /**
     * Gets or sets whether this panel is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle.style.display === "";
    }
    /**
     * Gets or sets whether this panel is visible.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        this.Handle.style.display = Value === true ? "" : "none";
    }
};

// ● viewport panel
/**
 * Internal grid viewport panel.
 *
 * Events:
 * - ElementSizeChanged
 */
tp.GridViewportPanel = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid viewport panel.
     * @param {tp.Grid} Grid The owner grid.
     * @param {HTMLElement} Parent The parent element.
     */
    constructor(Grid, Parent) {
        super();
        this.Grid = Grid;
        this.Handle = Grid.Document.createElement("div");
        Parent.appendChild(this.Handle);
        this.Handle.className = tp.Classes.Viewport;
        this.Content = this.Handle.ownerDocument.createElement("div");
        this.Handle.appendChild(this.Content);
        this.Content.className = tp.Classes.Content;
        this.fResizeDetector = new tp.ResizeDetector(this.Handle, this.OnElementSizeChanged, this, true);
    }

    // ● protected
    /**
     * Handles size changes.
     * @protected
     * @returns {void}
     */
    OnElementSizeChanged() {
        this.Trigger("ElementSizeChanged");
    }

    // ● public
    /**
     * Clears this panel.
     * @returns {void}
     */
    Clear() {
        super.Clear();
        if (this.Content)
            this.Content.innerHTML = "";
    }

    // ● properties
    /**
     * Gets or sets the vertical scroll position.
     * @returns {number} Returns the scroll position.
     */
    get ScrollTop() {
        return this.Handle.scrollTop;
    }
    /**
     * Gets or sets the vertical scroll position.
     * @param {number} Value The scroll position.
     * @returns {void}
     */
    set ScrollTop(Value) {
        this.Handle.scrollTop = Value;
    }
    /**
     * Gets or sets the horizontal scroll position.
     * @returns {number} Returns the scroll position.
     */
    get ScrollLeft() {
        return this.Handle.scrollLeft;
    }
    /**
     * Gets or sets the horizontal scroll position.
     * @param {number} Value The scroll position.
     * @returns {void}
     */
    set ScrollLeft(Value) {
        this.Handle.scrollLeft = Value;
    }
};

// ● summaries panel
/**
 * Internal grid summaries panel.
 */
tp.GridSummariesPanel = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid summaries panel.
     * @param {tp.Grid} Grid The owner grid.
     * @param {HTMLElement} Parent The parent element.
     */
    constructor(Grid, Parent) {
        super();
        this.Grid = Grid;
        this.Handle = Grid.Document.createElement("div");
        Parent.appendChild(this.Handle);
        this.Handle.className = tp.Classes.Summaries;
        this.Handle.style.height = tp.px(this.Grid.RowHeight);
        this.Handle.style.minHeight = this.Handle.style.height;
        this.Content = this.Handle.ownerDocument.createElement("div");
        this.Handle.appendChild(this.Content);
        this.Content.className = tp.Classes.Content;
    }

    // ● public
    /**
     * Clears this panel.
     * @returns {void}
     */
    Clear() {
        super.Clear();
        if (this.Content)
            this.Content.innerHTML = "";
    }

    // ● properties
    /**
     * Gets or sets whether this panel is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle.style.display === "";
    }
    /**
     * Gets or sets whether this panel is visible.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        this.Handle.style.display = Value === true ? "" : "none";
    }
};

// ● bottom panel
/**
 * Internal grid bottom panel.
 */
tp.GridBottomPanel = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid bottom panel.
     * @param {tp.Grid} Grid The owner grid.
     * @param {HTMLElement} Parent The parent element.
     */
    constructor(Grid, Parent) {
        super();
        this.Grid = Grid;
        this.Handle = Grid.Document.createElement("div");
        Parent.appendChild(this.Handle);
        this.Handle.className = tp.Classes.Bottom;
        this.Handle.style.height = tp.px(tp.Environment.ScrollbarSize.Height);
        this.Handle.style.minHeight = this.Handle.style.height;
        this.Content = this.Handle.ownerDocument.createElement("div");
        this.Handle.appendChild(this.Content);
        this.Content.className = tp.Classes.Content;
    }

    // ● public
    /**
     * Clears this panel.
     * @returns {void}
     */
    Clear() {
        super.Clear();
        if (this.Content)
            this.Content.innerHTML = "";
    }

    // ● properties
    /**
     * Gets or sets the horizontal scroll position.
     * @returns {number} Returns the scroll position.
     */
    get ScrollLeft() {
        return this.Handle.scrollLeft;
    }
    /**
     * Gets or sets the horizontal scroll position.
     * @param {number} Value The scroll position.
     * @returns {void}
     */
    set ScrollLeft(Value) {
        this.Handle.scrollLeft = Value;
    }
    /**
     * Gets or sets whether this panel is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle.style.display === "";
    }
    /**
     * Gets or sets whether this panel is visible.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        this.Handle.style.display = Value === true ? "" : "none";
    }
};
