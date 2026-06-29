// ● autocomplete list
/**
 * Drop-down autocomplete list associated with a text input element.
 */
tp.AutocompleteList = class extends tp.DropDownBox {
    // ● constructor
    /**
     * Creates an autocomplete list.
     * @param {HTMLElement|string} Associate The associated input element.
     */
    constructor(Associate) {
        Associate = tp(Associate);
        if (!tp.IsHTMLElement(Associate))
            tp.Throw("No Associate defined for AutocompleteList.");
        super({ Associate: Associate });
        this.Associate = Associate;
        this.tpClass = "tp.AutocompleteList";
        tp.AddClass(this.Handle, tp.Classes.AutocompleteList);
        this.Active = false;
        this.ServerFunc = null;
        this.DataList = null;
        this.UseStartsWithFilter = false;
        this.AutocompleteCharCount = 3;
        this.fContainer = this.Document.createElement("div");
        this.fContainer.className = tp.Classes.List;
        this.fContainer.tabIndex = -1;
        this.Handle.appendChild(this.fContainer);
        this.fDropDownScroller = new tp.VirtualScroller(this.Handle, this.fContainer);
        this.fDropDownScroller.Context = this;
        this.fDropDownScroller.RenderRowFunc = this.ItemRenderFunc;
        this.fItemHeight = 0;
        this.fListDisplayField = null;
        this.fSelectedItem = null;
        this.fDisplayList = [];
        this.fAssociateKeyUpHandler = this.FuncBind(this.Associate_KeyUp);
        this.fAssociateKeyDownHandler = this.FuncBind(this.Associate_KeyDown);
        this.fContainerClickHandler = this.FuncBind(this.Container_Click);
        this.Associate.addEventListener("keyup", this.fAssociateKeyUpHandler, true);
        this.Associate.addEventListener("keydown", this.fAssociateKeyDownHandler, false);
        this.fContainer.addEventListener("click", this.fContainerClickHandler, false);
    }

    // ● protected
    /**
     * Renders a row item for the virtual scroller.
     * @param {*} Row The row item.
     * @param {number} RowIndex The row index.
     * @returns {HTMLElement} Returns the row element.
     */
    ItemRenderFunc(Row, RowIndex) {
        var Result = this.Document.createElement("div");
        Result.className = tp.Classes.Item;
        Result.tabIndex = -1;
        tp.SetElementInfo(Result, {
            Item: Row,
            Index: RowIndex
        });
        Result.innerHTML = this.GetItemText(Row);
        return Result;
    }
    /**
     * Returns the display text of an item.
     * @param {*} Item The item.
     * @returns {string} Returns item text.
     */
    GetItemText(Item) {
        if (!tp.IsEmpty(Item)) {
            if (tp.IsPrimitive(Item))
                return Item.toString();
            if (!tp.IsBlank(this.ListDisplayField) && this.ListDisplayField in Item)
                return Item[this.ListDisplayField];
            if (tp.IsFunction(Item.ToString))
                return Item.ToString();
            if (tp.IsFunction(Item.toString))
                return Item.toString();
        }
        return "";
    }
    /**
     * Updates the virtual scroller row list and opens or closes the list.
     * @param {string} Text The filter text.
     * @returns {void}
     */
    SetScrollerList(Text) {
        var List = this.fDisplayList;
        if (tp.IsEmpty(List) || tp.IsArray(List) && List.length === 0) {
            this.Close();
        } else if (tp.IsArray(List)) {
            if (List.length === 1 && Text === this.GetItemText(List[0])) {
                this.Close();
            } else if (List.length > 0) {
                this.fDropDownScroller.RowHeight = this.ItemHeight;
                this.fDropDownScroller.SetRowList(List);
                this.Open();
            }
        }
    }
    /**
     * Returns the selected row element, if any.
     * @returns {HTMLElement|null} Returns the selected row element.
     */
    GetItemWithSelectionIndication() {
        return tp.Select(this.fContainer, "." + tp.Classes.Selected);
    }
    /**
     * Sets selection indication to a row element.
     * @param {HTMLElement|null|undefined} Element The row element.
     * @returns {void}
     */
    SetSelectionIndicationTo(Element) {
        var Previous = tp.Select(this.fContainer, "." + tp.Classes.Selected);
        if (Previous)
            tp.RemoveClass(Previous, tp.Classes.Selected);
        if (tp.IsElement(Element) && tp.ContainsElement(this.fContainer, Element))
            tp.AddClass(Element, tp.Classes.Selected);
    }
    /**
     * Assigns an item to the associate element and closes the list.
     * @param {*} Item The selected item.
     * @returns {void}
     */
    SelectItem(Item) {
        this.fSelectedItem = Item;
        this.Close();
        tp.val(this.Associate, this.GetItemText(Item));
        this.Associate.focus();
    }
    /**
     * Handles container clicks.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    Container_Click(e) {
        var Element = e.target;
        if (this.Active !== true || this.Resizing === true)
            return;
        while (tp.IsHTMLElement(Element) && Element !== this.fContainer) {
            if (tp.HasClass(Element, tp.Classes.Item) && tp.HasElementInfo(Element)) {
                this.SelectItem(tp.GetElementInfo(Element).Item);
                return;
            }
            Element = Element.parentNode;
        }
    }
    /**
     * Handles key-up events on the associate element.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    Associate_KeyUp(e) {
        var Count;
        var Text;
        if (this.Active !== true || e.target !== this.Associate)
            return;
        Count = tp.IsNumber(this.AutocompleteCharCount) ? this.AutocompleteCharCount : 3;
        Text = tp.val(this.Associate) || "";
        if (Count > 0 && Text.length >= Count) {
            if (tp.IsPrintableKey(e))
                this.FilterAsync(Text);
        } else {
            this.Close();
        }
    }
    /**
     * Handles key-down events on the associate element.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    Associate_KeyDown(e) {
        var Element;
        if (this.Active !== true || e.target !== this.Associate || this.IsOpen !== true)
            return;
        if (tp.IsKey(e, tp.Keys.Up)) {
            Element = this.GetItemWithSelectionIndication();
            if (Element && tp.IsElement(Element.previousElementSibling))
                this.SetSelectionIndicationTo(Element.previousElementSibling);
            e.preventDefault();
        } else if (tp.IsKey(e, tp.Keys.Down)) {
            Element = this.GetItemWithSelectionIndication();
            if (Element && tp.IsElement(Element.nextElementSibling))
                this.SetSelectionIndicationTo(Element.nextElementSibling);
            else if (this.fContainer.children.length > 0)
                this.SetSelectionIndicationTo(this.fContainer.children[0]);
            e.preventDefault();
        } else if (tp.IsKey(e, tp.Keys.Enter)) {
            Element = this.GetItemWithSelectionIndication();
            if (Element && tp.HasElementInfo(Element))
                this.SelectItem(tp.GetElementInfo(Element).Item);
            e.preventDefault();
        } else if (tp.IsKey(e, tp.Keys.Escape)) {
            this.Close();
            e.preventDefault();
        }
    }
    /**
     * Called by Open() and Close() to notify the owner about a stage change.
     * @protected
     * @param {number} Stage The tp.DropDownBoxStage value.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnOwnerEvent(Stage) {
        var Count = this.fDisplayList ? Math.min(this.MaxDropdownItems, this.fDisplayList.length) : 0;
        this.Height = Count > 0 ? Count * this.ItemHeight + 5 : 0;
        return super.OnOwnerEvent(Stage);
    }

    // ● public
    /**
     * Filters the data list and displays the dropdown.
     * @param {string} Text The filter text.
     * @returns {Promise<void>} Returns a promise that completes after filtering.
     */
    async FilterAsync(Text) {
        var Data;
        var Args;
        var FilterFunc;
        var Index;
        var Item;
        var ItemText;
        this.fSelectedItem = null;
        this.fDisplayList = [];
        if (!tp.IsBlank(this.ServerFunc)) {
            if (!tp.Ajax || !tp.Ajax.PostAsync)
                tp.Throw("tp.Ajax.PostAsync is required for server autocomplete.");
            Data = {
                Text: Text,
                UseStartsWith: this.UseStartsWithFilter === true
            };
            Args = await tp.Ajax.PostAsync(this.ServerFunc, Data);
            this.fDisplayList = Args && tp.IsArray(Args.Packet) ? Args.Packet : [];
            this.SetScrollerList(Text);
            return;
        }
        if (tp.IsArray(this.DataList)) {
            FilterFunc = this.UseStartsWithFilter === true ? tp.StartsWith : tp.ContainsText;
            for (Index = 0; Index < this.DataList.length; Index++) {
                Item = this.DataList[Index];
                ItemText = this.GetItemText(Item);
                if (FilterFunc(ItemText, Text, true))
                    this.fDisplayList.push(Item);
            }
        }
        this.SetScrollerList(Text);
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.Associate) {
            this.Associate.removeEventListener("keyup", this.fAssociateKeyUpHandler, true);
            this.Associate.removeEventListener("keydown", this.fAssociateKeyDownHandler, false);
        }
        if (this.fContainer)
            this.fContainer.removeEventListener("click", this.fContainerClickHandler, false);
        if (this.fDropDownScroller) {
            this.fDropDownScroller.Dispose();
            this.fDropDownScroller = null;
        }
        this.fAssociateKeyUpHandler = null;
        this.fAssociateKeyDownHandler = null;
        this.fContainerClickHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Gets or sets item height.
     * @returns {number} Returns item height.
     */
    get ItemHeight() {
        if (tp.IsEmpty(this.fItemHeight) || this.fItemHeight <= 0)
            this.fItemHeight = tp.GetLineHeight(this.Associate);
        return this.fItemHeight;
    }
    /**
     * Gets or sets item height.
     * @param {number} Value The item height.
     * @returns {void}
     */
    set ItemHeight(Value) {
        this.fItemHeight = tp.ToInt(Value);
    }
    /**
     * Gets or sets the item display field name.
     * @returns {string} Returns the display field.
     */
    get ListDisplayField() {
        return this.fListDisplayField;
    }
    /**
     * Gets or sets the item display field name.
     * @param {string} Value The display field.
     * @returns {void}
     */
    set ListDisplayField(Value) {
        this.fListDisplayField = tp.IsBlank(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the maximum number of visible dropdown items.
     * @returns {number} Returns the maximum number of visible items.
     */
    get MaxDropdownItems() {
        var Result = this.fMaxDropdownItems || 10;
        return Result > 40 ? 40 : Result;
    }
    /**
     * Gets or sets the maximum number of visible dropdown items.
     * @param {number} Value The maximum visible item count.
     * @returns {void}
     */
    set MaxDropdownItems(Value) {
        this.fMaxDropdownItems = tp.ToInt(Value);
    }
    /**
     * Gets the selected item.
     * @returns {*} Returns the selected item.
     */
    get SelectedItem() {
        return this.fSelectedItem;
    }
    /**
     * Returns true if the selected item text matches the associate value.
     * @returns {boolean} Returns true when selected item is valid.
     */
    get IsSelectedItemValid() {
        var ItemText;
        var AssociateText;
        if (!tp.IsEmpty(this.SelectedItem)) {
            ItemText = this.GetItemText(this.SelectedItem);
            AssociateText = !tp.IsEmpty(this.Associate) ? tp.Trim(tp.val(this.Associate)) || "" : "";
            return !tp.IsBlank(AssociateText) && tp.IsSameText(ItemText, AssociateText);
        }
        return false;
    }
};
