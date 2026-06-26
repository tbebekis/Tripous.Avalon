// ● list box
/**
 * A virtual-scroller based list-box control.
 *
 * Example markup:
 * <pre>
 *     <div data-setup="{ListValueField: 'Id', ListDisplayField: 'Name', List: [{Id: 100, Name: 'All'}, {Id: 0, Name: 'No stops'}], SelectedIndex: 0 }"></div>
 * </pre>
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 * - SelectedIndexChanged
 */
tp.ListBox = class extends tp.ListControl {
    // ● private
    /**
     * Creates list-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateListBoxParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "div";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }

    // ● constructor
    /**
     * Creates a list box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.ListBox.CreateListBoxParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDataBindMode = tp.ControlBindMode.List;
        this.fDataValueProperty = "SelectedValue";
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.fKeyDownHandler = this.FuncBind(this.HandleKeyDown);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateScroller();
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.ListControl);
        tp.AddClass(this.Handle, tp.Classes.ListBox);
    }
    /**
     * Creates the row container and virtual scroller.
     * @protected
     * @returns {void}
     */
    CreateScroller() {
        if (this.fScroller)
            return;
        this.fContainer = this.Document.createElement("div");
        this.Handle.appendChild(this.fContainer);
        this.fContainer.className = tp.Classes.List;
        this.fContainer.tabIndex = -1;
        this.fScroller = new tp.VirtualScroller(this.Handle, this.fContainer);
        this.fScroller.RowHeight = this.ItemHeight;
        this.fScroller.Context = this;
        this.fScroller.RenderRowFunc = this.ItemRenderFunc;
        this.fScroller.ScrollFunc = this.ScrollFunc;
        this.Handle.addEventListener("click", this.fClickHandler, false);
        this.Handle.addEventListener("keydown", this.fKeyDownHandler, false);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fClickHandler)
            this.Handle.removeEventListener("click", this.fClickHandler, false);
        if (this.Handle && this.fKeyDownHandler)
            this.Handle.removeEventListener("keydown", this.fKeyDownHandler, false);
        this.fClickHandler = null;
        this.fKeyDownHandler = null;
        super.DoDispose();
    }
    /**
     * Handles click events.
     * @protected
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleClick(e) {
        var Info;
        var Element = e.target;
        if (this.Enabled !== true || !this.fScroller)
            return;
        if (tp.ContainsEventTarget(this.fScroller.Container, Element)
            && tp.HasClass(Element, tp.Classes.Item)
            && tp.HasElementInfo(Element)) {
            Info = tp.GetElementInfo(Element);
            this.SelectedIndex = Info.Index;
            this.fScroller.Viewport.focus();
        }
    }
    /**
     * Handles keyboard events.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleKeyDown(e) {
        this.HandleScrollerKeyDown(e);
    }
};

tp.Ui.RegisterType(["ListBox", "tp-ListBox"], tp.ListBox);
