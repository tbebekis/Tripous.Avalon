// ● item bar render mode
/**
 * Indicates how an item bar renders its items.
 * @enum {number}
 */
tp.ItemBarRenderMode = {
    None: 0,
    Normal: 1,
    Toggle: 2,
    NextPrev: 4
};
Object.freeze(tp.ItemBarRenderMode);

// ● item bar
/**
 * A bar that displays selectable items.
 * Uses ResizeObserver through tp.ResizeDetector to detect whether items fit in the available width.
 *
 * Events:
 * - RenderModeChanged
 * - SelectedIndexChanging
 * - SelectedIndexChanged
 * - ItemClicked
 */
tp.ItemBar = class extends tp.Component {
    // ● constructor
    /**
     * Creates an item bar.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The item bar create parameters, handle, or selector.
     */
    constructor(CreateParams) {
        super(CreateParams);
        this.tpClass = "tp.ItemBar";
        tp.AddClass(this.Handle, tp.Classes.ItemBar);
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.fAuxClickHandler = this.FuncBind(this.HandleAuxClick);
        this.CreateControls();
        this.Handle.addEventListener("click", this.fClickHandler);
        this.Handle.addEventListener("auxclick", this.fAuxClickHandler);
        this.fResizeDetector = new tp.ResizeDetector(this.Handle, this.OnElementSizeChanged, this);
        this.RenderMode = tp.ItemBarRenderMode.Normal;
    }

    // ● protected
    /**
     * Creates the internal item bar elements and moves existing direct children into the item container.
     * @returns {void}
     */
    CreateControls() {
        var List = this.GetElementList();
        var Index;
        var Item;
        for (Index = 0; Index < List.length; Index++) {
            Item = List[Index];
            if (Item.parentNode)
                Item.parentNode.removeChild(Item);
        }

        this.ToggleContainer = this.Document.createElement("div");
        this.ToggleContainer.className = tp.Classes.Toggle;
        this.ToggleButton = this.Document.createElement("div");
        this.ToggleButton.className = tp.Classes.Btn;
        this.ToggleButtonIcon = this.Document.createElement("span");
        this.ToggleButtonIcon.className = tp.Classes.Ico;
        this.ToggleButtonIcon.textContent = "☰";
        this.ToggleButton.appendChild(this.ToggleButtonIcon);
        this.ToggleTextZone = this.Document.createElement("div");
        this.ToggleTextZone.className = tp.Classes.Text;
        this.ToggleContainer.appendChild(this.ToggleButton);
        this.ToggleContainer.appendChild(this.ToggleTextZone);
        this.Handle.appendChild(this.ToggleContainer);

        this.btnToLeft = this.Document.createElement("div");
        this.btnToLeft.className = tp.Classes.Prev;
        this.btnToLeft.textContent = "◂";
        this.Handle.appendChild(this.btnToLeft);

        this.ItemContainer = this.Document.createElement("div");
        this.ItemContainer.className = tp.Classes.ItemList;
        this.Handle.appendChild(this.ItemContainer);

        this.btnToRight = this.Document.createElement("div");
        this.btnToRight.className = tp.Classes.Next;
        this.btnToRight.textContent = "▸";
        this.Handle.appendChild(this.btnToRight);

        this.ToggleItemList = this.Document.createElement("div");
        this.ToggleItemList.className = tp.Classes.ToggleItemList;
        this.ToggleItemList.addEventListener("click", this.fClickHandler);

        for (Index = 0; Index < List.length; Index++)
            this.ItemContainer.appendChild(List[Index]);
    }
    /**
     * Handles element size changes.
     * @returns {void}
     */
    OnElementSizeChanged() {
        var ItemTotalWidth = this.GetItemTotalWidth();
        var ContainerWidth = this.Handle.offsetWidth;
        if (this.ChangingMode)
            return;
        if (this.RenderMode === tp.ItemBarRenderMode.Normal && ItemTotalWidth > ContainerWidth) {
            this.RenderMode = this.ResponsiveMode;
        } else if (this.RenderMode !== tp.ItemBarRenderMode.Normal && ContainerWidth > ItemTotalWidth) {
            this.RenderMode = tp.ItemBarRenderMode.Normal;
        } else if (this.RenderMode !== tp.ItemBarRenderMode.Normal && this.RenderMode !== this.ResponsiveMode) {
            this.RenderMode = this.ResponsiveMode;
        } else if (this.RenderMode === tp.ItemBarRenderMode.NextPrev) {
            this.Arrange();
        }
    }
    /**
     * Handles click events.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleClick(e) {
        var Index;
        var Item;
        if (tp.ContainsEventTarget(this.ToggleButton, e.target)) {
            this.ToggleClicked();
        } else if (this.ToggleItemList && tp.ContainsEventTarget(this.ToggleItemList, e.target)) {
            if (this.IsToggleDropDownBusy()) {
                e.preventDefault();
                e.stopPropagation();
                return;
            }
            Index = this.FindToggleItemIndex(e.target);
            if (Index !== -1) {
                Item = this.GetItemElementList()[Index];
                this.SelectedIndex = Index;
                this.CloseToggle();
                this.OnItemClicked(Item, e, tp.Mouse.LEFT);
            }
        } else if (tp.ContainsEventTarget(this.btnToLeft, e.target)) {
            if (this.CanShowNext())
                this.ShowNext();
        } else if (tp.ContainsEventTarget(this.btnToRight, e.target)) {
            if (this.CanHideNext())
                this.HideNext();
        } else {
            Item = this.FindClickedItem(e.target);
            if (Item) {
                Index = this.IndexOfItem(Item);
                this.SelectedIndex = Index;
                this.OnItemClicked(Item, e, tp.Mouse.LEFT);
            }
        }
    }
    /**
     * Handles auxclick events.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleAuxClick(e) {
        var Item = this.FindClickedItem(e.target);
        if (Item) {
            if (tp.Mouse.IsMid(e))
                this.OnItemClicked(Item, e, tp.Mouse.MID);
            else if (tp.Mouse.IsRight(e))
                this.OnItemClicked(Item, e, tp.Mouse.RIGHT);
        }
    }
    /**
     * Finds the clicked item.
     * @param {EventTarget} Target The event target.
     * @returns {HTMLElement|null} Returns the item or null.
     */
    FindClickedItem(Target) {
        var List = this.GetItemElementList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.ContainsEventTarget(List[Index], Target))
                return List[Index];
        }
        return null;
    }
    /**
     * Finds the clicked toggle item index.
     * @param {EventTarget} Target The event target.
     * @returns {number} Returns the item index or -1.
     */
    FindToggleItemIndex(Target) {
        var List = tp.ChildHTMLElements(this.ToggleItemList);
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.ContainsEventTarget(List[Index], Target))
                return Index;
        }
        return -1;
    }
    /**
     * Returns true when the toggle drop-down is resizing.
     * @returns {boolean} Returns true when toggle list clicks should be ignored.
     */
    IsToggleDropDownBusy() {
        return this.ToggleDropDownBox && this.ToggleDropDownBox.Resizing;
    }
    /**
     * Sets the selected item by index without triggering events.
     * @param {number} Index The item index.
     * @returns {void}
     */
    SetSelectedIndex(Index) {
        var List = this.GetItemElementList();
        var Item;
        var i;
        for (i = 0; i < List.length; i++)
            tp.RemoveClass(List[i], tp.Classes.Selected);
        Item = List[Index];
        if (tp.IsHTMLElement(Item)) {
            tp.AddClass(Item, tp.Classes.Selected);
            if (this.ToggleTextZone)
                this.ToggleTextZone.innerHTML = this.GetItemTextAt(Index);
        } else if (this.ToggleTextZone) {
            this.ToggleTextZone.innerHTML = "";
        }
    }
    /**
     * Returns true when an item is visible.
     * @param {HTMLElement} Element The item.
     * @returns {boolean} Returns true when visible.
     */
    IsItemVisible(Element) {
        return tp.IsHTMLElement(Element) && Element.style.display !== "none";
    }
    /**
     * Returns the gap between items.
     * @returns {number} Returns the item gap in pixels.
     */
    GetItemGap() {
        var Style = tp.GetComputedStyle(this.ItemContainer);
        return Style ? tp.ToInt(Style.gap) : 0;
    }
    /**
     * Returns total item width.
     * @returns {number} Returns total item width in pixels.
     */
    GetItemTotalWidth() {
        var List = this.GetItemElementList();
        var Gap = this.GetItemGap();
        var Total = 0;
        var ContainerDisplay = this.ItemContainer.style.display;
        var OldDisplay;
        var Index;
        var Item;
        this.ItemContainer.style.display = "";
        for (Index = 0; Index < List.length; Index++) {
            Item = List[Index];
            OldDisplay = Item.style.display;
            Item.style.display = "";
            Total += Item.offsetWidth + Gap;
            Item.style.display = OldDisplay;
        }
        this.ItemContainer.style.display = ContainerDisplay;
        return Total;
    }
    /**
     * Returns the total width of visible items.
     * @returns {number} Returns total visible item width in pixels.
     */
    GetVisibleItemTotalWidth() {
        var List = this.GetItemElementList();
        var Gap = this.GetItemGap();
        var Total = 0;
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (this.IsItemVisible(List[Index]))
                Total += List[Index].offsetWidth + Gap;
        }
        return Total;
    }
    /**
     * Returns true when an item can be hidden from the left edge.
     * @returns {boolean} Returns true when an item can be hidden.
     */
    CanHideNext() {
        var List = this.GetItemElementList();
        var Index;
        if (this.RenderMode !== tp.ItemBarRenderMode.NextPrev)
            return false;
        for (Index = 0; Index < List.length - 1; Index++) {
            if (this.IsItemVisible(List[Index]))
                return true;
        }
        return false;
    }
    /**
     * Returns true when a hidden item can be shown.
     * @returns {boolean} Returns true when a hidden item can be shown.
     */
    CanShowNext() {
        var List = this.GetItemElementList();
        var Index;
        if (this.RenderMode !== tp.ItemBarRenderMode.NextPrev)
            return false;
        for (Index = 0; Index < List.length; Index++) {
            if (!this.IsItemVisible(List[Index]))
                return true;
        }
        return false;
    }
    /**
     * Hides the next visible item from the left edge.
     * @returns {void}
     */
    HideNext() {
        var List = this.GetItemElementList();
        var Index;
        for (Index = 0; Index < List.length - 1; Index++) {
            if (this.IsItemVisible(List[Index])) {
                List[Index].style.display = "none";
                break;
            }
        }
    }
    /**
     * Shows the previous hidden item.
     * @returns {void}
     */
    ShowNext() {
        var List = this.GetItemElementList();
        var Index;
        for (Index = List.length - 1; Index >= 0; Index--) {
            if (!this.IsItemVisible(List[Index])) {
                List[Index].style.display = "";
                break;
            }
        }
    }
    /**
     * Arranges visible and hidden items for NextPrev mode.
     * @returns {void}
     */
    Arrange() {
        var List = this.GetItemElementList();
        var Index;
        var AvailableWidth;
        for (Index = 0; Index < List.length; Index++)
            List[Index].style.display = "";
        AvailableWidth = this.Handle.offsetWidth - this.btnToLeft.offsetWidth - this.btnToRight.offsetWidth;
        while (this.GetVisibleItemTotalWidth() > AvailableWidth && this.CanHideNext())
            this.HideNext();
    }
    /**
     * Opens or closes the toggle list.
     * @returns {void}
     */
    ToggleClicked() {
        if (this.ToggleDropDownBox && this.ToggleDropDownBox.IsOpen)
            this.CloseToggle();
        else
            this.OpenToggle();
    }
    /**
     * Opens the toggle list.
     * @returns {void}
     */
    OpenToggle() {
        var List = this.GetItemElementList();
        var Index;
        var ToggleItem;
        if (!this.ToggleDropDownBox) {
            this.ToggleDropDownBox = new tp.DropDownBox(null, {
                Associate: this.ToggleContainer,
                Width: this.ToggleContainer.getBoundingClientRect().width,
                Height: "auto"
            });
            this.ToggleDropDownBox.Handle.appendChild(this.ToggleItemList);
        }
        tp.RemoveChildren(this.ToggleItemList);
        for (Index = 0; Index < List.length; Index++) {
            ToggleItem = this.Document.createElement("div");
            ToggleItem.innerHTML = this.GetItemTextAt(Index);
            ToggleItem.classList.toggle(tp.Classes.Selected, Index === this.SelectedIndex);
            this.ToggleItemList.appendChild(ToggleItem);
        }
        this.ToggleDropDownBox.Width = Math.max(this.ToggleContainer.getBoundingClientRect().width, 180);
        this.ToggleDropDownBox.Height = "auto";
        this.ToggleDropDownBox.Open();
        this.ToggleDropDownBox.Height = Math.min(this.ToggleDropDownBox.Handle.getBoundingClientRect().height, 220);
    }
    /**
     * Closes the toggle list.
     * @returns {void}
     */
    CloseToggle() {
        if (this.IsToggleDropDownBusy())
            return;
        if (this.ToggleDropDownBox)
            this.ToggleDropDownBox.Close();
    }
    /**
     * Called when items are added or removed.
     * @returns {void}
     */
    ItemListChanged() {
        if (this.RenderMode === tp.ItemBarRenderMode.NextPrev)
            this.Arrange();
        else
            this.OnElementSizeChanged();
        this.SetSelectedIndex(this.SelectedIndex);
    }

    // ● properties
    /**
     * Gets or sets the render mode.
     * @returns {number} Returns a tp.ItemBarRenderMode value.
     */
    get RenderMode() {
        return this.fRenderMode;
    }
    /**
     * Gets or sets the render mode.
     * @param {number} Value The render mode.
     * @returns {void}
     */
    set RenderMode(Value) {
        var List;
        var Index;
        if (tp.IsNumber(Value) && Value !== this.fRenderMode && this.ChangingMode === false) {
            this.ChangingMode = true;
            try {
                this.fRenderMode = Value;
                this.CloseToggle();
                List = this.GetItemElementList();
                for (Index = 0; Index < List.length; Index++)
                    List[Index].style.display = "";
                this.ToggleContainer.style.display = Value === tp.ItemBarRenderMode.Toggle ? "" : "none";
                this.btnToLeft.style.display = Value === tp.ItemBarRenderMode.NextPrev ? "" : "none";
                this.btnToRight.style.display = Value === tp.ItemBarRenderMode.NextPrev ? "" : "none";
                this.ItemContainer.style.display = Value === tp.ItemBarRenderMode.Toggle ? "none" : "";
                if (Value === tp.ItemBarRenderMode.NextPrev)
                    this.Arrange();
                this.SetSelectedIndex(this.SelectedIndex);
                this.OnRenderModeChanged();
            } finally {
                this.ChangingMode = false;
            }
        }
    }
    /**
     * Gets or sets the responsive render mode to use when items exceed the available width.
     * @returns {number} Returns a tp.ItemBarRenderMode value.
     */
    get ResponsiveMode() {
        return this.fResponsiveMode;
    }
    /**
     * Gets or sets the responsive render mode to use when items exceed the available width.
     * @param {number} Value The responsive render mode.
     * @returns {void}
     */
    set ResponsiveMode(Value) {
        if (tp.IsNumber(Value) && Value !== this.fResponsiveMode) {
            this.fResponsiveMode = Value;
            if (this.RenderMode !== tp.ItemBarRenderMode.Normal)
                this.RenderMode = Value;
        }
    }
    /**
     * Gets or sets the selected item index.
     * @returns {number} Returns the selected item index.
     */
    get SelectedIndex() {
        var List = this.GetItemElementList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.HasClass(List[Index], tp.Classes.Selected))
                return Index;
        }
        return -1;
    }
    /**
     * Gets or sets the selected item index.
     * @param {number} Value The selected item index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
        var CurrentIndex = this.SelectedIndex;
        var Args;
        if (Value !== CurrentIndex) {
            Args = this.OnSelectedIndexChanging(CurrentIndex, Value);
            if (Args && Args.Cancel === true)
                return;
            this.SetSelectedIndex(Value);
            this.OnSelectedIndexChanged(CurrentIndex, Value);
        }
    }
    /**
     * Gets or sets the selected item.
     * @returns {HTMLElement|null} Returns the selected item.
     */
    get SelectedItem() {
        return this.GetItemElementList()[this.SelectedIndex] || null;
    }
    /**
     * Gets or sets the selected item.
     * @param {HTMLElement|tp.Component|null|undefined} Value The item or component.
     * @returns {void}
     */
    set SelectedItem(Value) {
        var Element = tp.IsHTMLElement(Value) ? Value : Value && tp.IsHTMLElement(Value.Handle) ? Value.Handle : null;
        var Index = this.IndexOfItem(Element);
        if (Index >= 0)
            this.SelectedIndex = Index;
    }

    // ● public
    /**
     * Returns the item elements.
     * @returns {HTMLElement[]} Returns item elements.
     */
    GetItemElementList() {
        return tp.IsHTMLElement(this.ItemContainer) ? tp.ChildHTMLElements(this.ItemContainer) : [];
    }
    /**
     * Returns the item text at an index.
     * @param {number} Index The item index.
     * @returns {string} Returns item text.
     */
    GetItemTextAt(Index) {
        var Item = this.GetItemElementList()[Index];
        var Component = tp.GetComponent(Item);
        if (Component && "Text" in Component)
            return Component.Text;
        return tp.IsHTMLElement(Item) ? Item.innerHTML : "";
    }
    /**
     * Adds an item.
     * @param {HTMLElement|tp.Component} Item The item element or component.
     * @returns {void}
     */
    AddItem(Item) {
        var Element = Item instanceof tp.Component ? Item.Handle : Item;
        if (tp.IsHTMLElement(Element)) {
            this.ItemContainer.appendChild(Element);
            this.ItemListChanged();
        }
    }
    /**
     * Adds a list of items.
     * @param {HTMLElement[]|tp.Component[]} ItemList The item list.
     * @returns {void}
     */
    AddRange(ItemList) {
        var Index;
        var Element;
        if (tp.IsArray(ItemList)) {
            for (Index = 0; Index < ItemList.length; Index++) {
                Element = ItemList[Index] instanceof tp.Component ? ItemList[Index].Handle : ItemList[Index];
                if (tp.IsHTMLElement(Element))
                    this.ItemContainer.appendChild(Element);
            }
            this.ItemListChanged();
        }
    }
    /**
     * Inserts an item.
     * @param {HTMLElement|tp.Component} Item The item element or component.
     * @param {number} Index The insert index.
     * @returns {void}
     */
    InsertItem(Item, Index) {
        var Element = Item instanceof tp.Component ? Item.Handle : Item;
        var List = this.GetItemElementList();
        var Reference = Index >= 0 && Index < List.length ? List[Index] : null;
        if (tp.IsHTMLElement(Element)) {
            if (Reference)
                this.ItemContainer.insertBefore(Element, Reference);
            else
                this.ItemContainer.appendChild(Element);
            this.ItemListChanged();
        }
    }
    /**
     * Removes an item at an index.
     * @param {number} Index The item index.
     * @returns {void}
     */
    RemoveItemAt(Index) {
        var Item = this.GetItemElementList()[Index];
        if (tp.IsHTMLElement(Item)) {
            this.ItemContainer.removeChild(Item);
            this.ItemListChanged();
        }
    }
    /**
     * Returns the index of an item.
     * @param {HTMLElement|null|undefined} Item The item element.
     * @returns {number} Returns the item index or -1.
     */
    IndexOfItem(Item) {
        return this.GetItemElementList().indexOf(Item);
    }
    /**
     * Disposes this item bar.
     * @returns {void}
     */
    Dispose() {
        if (this.fResizeDetector) {
            this.fResizeDetector.Dispose();
            this.fResizeDetector = null;
        }
        if (this.HasHandle) {
            this.Handle.removeEventListener("click", this.fClickHandler);
            this.Handle.removeEventListener("auxclick", this.fAuxClickHandler);
        }
        if (this.ToggleItemList)
            this.ToggleItemList.removeEventListener("click", this.fClickHandler);
        if (this.ToggleDropDownBox) {
            this.ToggleDropDownBox.Dispose();
            this.ToggleDropDownBox = null;
        }
        this.fClickHandler = null;
        this.fAuxClickHandler = null;
        super.Dispose();
    }

    // ● events
    /**
     * Event trigger called when RenderMode changes.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnRenderModeChanged() {
        return this.Trigger("RenderModeChanged", { RenderMode: this.RenderMode });
    }
    /**
     * Event trigger called before SelectedIndex changes.
     * @param {number} CurrentIndex The current index.
     * @param {number} NewIndex The new index.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnSelectedIndexChanging(CurrentIndex, NewIndex) {
        return this.Trigger("SelectedIndexChanging", { CurrentIndex: CurrentIndex, NewIndex: NewIndex });
    }
    /**
     * Event trigger called after SelectedIndex changes.
     * @param {number} CurrentIndex The previous index.
     * @param {number} NewIndex The new index.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnSelectedIndexChanged(CurrentIndex, NewIndex) {
        return this.Trigger("SelectedIndexChanged", { CurrentIndex: CurrentIndex, NewIndex: NewIndex });
    }
    /**
     * Event trigger called when an item is clicked.
     * @param {HTMLElement} Item The clicked item.
     * @param {MouseEvent} e The mouse event.
     * @param {number} MouseButton The tp.Mouse button value.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnItemClicked(Item, e, MouseButton) {
        var Args = new tp.EventArgs("ItemClicked", this, e);
        Args.Item = Item;
        Args.el = Item;
        Args.ItemIndex = this.IndexOfItem(Item);
        Args.MouseButton = MouseButton;
        return this.Trigger("ItemClicked", Args);
    }
};

// ● prototype
/**
 * True while changing render mode.
 * @type {boolean}
 */
tp.ItemBar.prototype.ChangingMode = false;
/**
 * Private field.
 * @type {number}
 */
tp.ItemBar.prototype.fRenderMode = tp.ItemBarRenderMode.None;
/**
 * Private field.
 * @type {number}
 */
tp.ItemBar.prototype.fResponsiveMode = tp.ItemBarRenderMode.NextPrev;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.ToggleContainer = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.ToggleButton = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.ToggleButtonIcon = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.ToggleTextZone = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.ToggleItemList = null;
/**
 * Private field.
 * @type {tp.DropDownBox|null}
 */
tp.ItemBar.prototype.ToggleDropDownBox = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.btnToLeft = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.btnToRight = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ItemBar.prototype.ItemContainer = null;
/**
 * Private field.
 * @type {tp.ResizeDetector|null}
 */
tp.ItemBar.prototype.fResizeDetector = null;
/**
 * Private field.
 * @type {Function|null}
 */
tp.ItemBar.prototype.fClickHandler = null;
/**
 * Private field.
 * @type {Function|null}
 */
tp.ItemBar.prototype.fAuxClickHandler = null;
