// ● check list control
/**
 * Base class for multi-select list controls with check boxes.
 *
 * Events:
 * - SelectionChanged
 */
tp.CheckListControl = class extends tp.ListControl {
    // ● constructor
    /**
     * Creates a check-list control.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataValueProperty = "SelectedValues";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fSelectedIndexes = [];
        this.fChangeHandler = this.FuncBind(this.HandleChange);
    }
    /**
     * Applies explicit create params to this check-list control.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        tp.Control.prototype.ApplyCreateParams.call(this, Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.ListValueField))
            this.ListValueField = Params.ListValueField;
        if (!tp.IsNil(Params.ListDisplayField))
            this.ListDisplayField = Params.ListDisplayField;
        if (!tp.IsNil(Params.ListSourceName))
            this.ListSourceName = Params.ListSourceName;
        if (!tp.IsNil(Params.ItemHeight))
            this.ItemHeight = Params.ItemHeight;
        if (!tp.IsNil(Params.List))
            this.fItems.AddRange(Params.List);
        if (!tp.IsNil(Params.ListItems))
            this.fItems.AddRange(Params.ListItems);
        if (!tp.IsNil(Params.Items))
            this.fItems.AddRange(Params.Items);
        if (!tp.IsNil(Params.ListSource))
            this.ListSource = Params.ListSource;
        if (!tp.IsNil(Params.SelectedIndexes))
            this.SelectedIndexes = Params.SelectedIndexes;
        else if (!tp.IsNil(Params.SelectedValues))
            this.SelectedValues = Params.SelectedValues;
        this.SetScrollerList();
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fChangeHandler)
            this.Handle.removeEventListener("change", this.fChangeHandler, false);
        this.fChangeHandler = null;
        super.DoDispose();
    }
    /**
     * Handles checkbox change events.
     * @protected
     * @param {Event} e The event.
     * @returns {void}
     */
    HandleChange(e) {
        var Element = e.target;
        var Info;
        var Parent;
        if (!(Element instanceof HTMLInputElement) || Element.type !== "checkbox" || !tp.HasElementInfo(Element))
            return;
        Info = tp.GetElementInfo(Element);
        this.CheckIndex(Info.Index, Element.checked === true);
        Parent = tp.Closest(Element, "label");
        if (Parent instanceof HTMLElement) {
            if (Element.checked === true)
                tp.AddClass(Parent, tp.Classes.Selected);
            else
                tp.RemoveClass(Parent, tp.Classes.Selected);
        }
        if (this.fScroller && this.fScroller.Viewport)
            this.fScroller.Viewport.focus();
    }
    /**
     * Notification from the item list after it changes.
     * @protected
     * @param {tp.ListEventArgs} Args The event arguments.
     * @returns {void}
     */
    ListChanged(Args) {
        switch (Args.Action) {
            case tp.ListChangeType.Remove:
                this.RemoveSelectedIndex(Args.Index);
                this.UpdateScroller();
                this.OnSelectionChanged();
                break;
            case tp.ListChangeType.Clear:
            case tp.ListChangeType.Assign:
                this.fSelectedIndexes.length = 0;
                this.SetScrollerList();
                this.OnSelectionChanged();
                break;
            case tp.ListChangeType.Insert:
            case tp.ListChangeType.Update:
            case tp.ListChangeType.AddRange:
                this.SetScrollerList();
                this.OnSelectionChanged();
                break;
        }
    }
    /**
     * Renders a virtual scroller row.
     * @protected
     * @param {*} Row The row item.
     * @param {number} RowIndex The row index.
     * @returns {HTMLElement} Returns the row element.
     */
    ItemRenderFunc(Row, RowIndex) {
        var Result = this.Document.createElement("label");
        var CheckBox = this.Document.createElement("input");
        var Text = this.Document.createTextNode(this.GetItemText(Row));
        var Index = this.Items.indexOf(Row);
        Index = Index === -1 ? RowIndex : Index;
        Result.className = tp.Classes.Item;
        Result.tabIndex = -1;
        tp.Data(Result, "index", Index);
        CheckBox.type = "checkbox";
        CheckBox.checked = this.IsChecked(Index);
        tp.SetElementInfo(CheckBox, {
            Item: Row,
            Index: Index
        });
        if (CheckBox.checked === true)
            tp.AddClass(Result, tp.Classes.Selected);
        Result.appendChild(CheckBox);
        Result.appendChild(Text);
        return Result;
    }
    /**
     * Virtual scroller callback before and after rendering.
     * @protected
     * @param {number} Phase The render phase. 1 is before, 2 is after.
     * @returns {void}
     */
    ScrollFunc(Phase) {
    }
    /**
     * Binds the control to its data source.
     * @protected
     * @returns {void}
     */
    Bind() {
        tp.Control.prototype.Bind.call(this);
        this.ReadDataValue();
    }
    /**
     * Reads the bound data value.
     * @protected
     * @returns {void}
     */
    ReadDataValue() {
        var Value;
        if (this.ReadingDataValue === true || this.WritingDataValue === true)
            return;
        this.fCanPostDataValue = false;
        try {
            if (this.IsDataBound && this.DataSource.Position >= 0) {
                this.ReadingDataValue = true;
                try {
                    Value = this.DataSource.Get(this.DataField);
                    this[this.DataValueProperty] = Value;
                } finally {
                    this.ReadingDataValue = false;
                }
            }
        } finally {
            this.fCanPostDataValue = true;
        }
    }
    /**
     * Removes one selected index.
     * @protected
     * @param {number} Index The selected index.
     * @returns {void}
     */
    RemoveSelectedIndex(Index) {
        var Result = [];
        var i;
        for (i = 0; i < this.fSelectedIndexes.length; i++) {
            if (this.fSelectedIndexes[i] < Index)
                Result.push(this.fSelectedIndexes[i]);
            else if (this.fSelectedIndexes[i] > Index)
                Result.push(this.fSelectedIndexes[i] - 1);
        }
        this.fSelectedIndexes = Result;
    }
    /**
     * Writes the selection to the bound data source when allowed.
     * @protected
     * @returns {void}
     */
    DoPost() {
        if (this.IsDataBound && this.fCanPostDataValue === true)
            this.WriteDataValue();
    }

    // ● public
    /**
     * Clears the control.
     * @returns {void}
     */
    Clear() {
        this.fItems.Clear();
        this.fSelectedIndexes.length = 0;
        this.SetScrollerList();
        this.OnSelectionChanged();
    }
    /**
     * Returns the index of a value, if any.
     * @param {*} Value The value.
     * @returns {number} Returns the index or -1.
     */
    IndexOfValue(Value) {
        var Index;
        var Item;
        for (Index = 0; Index < this.Items.length; Index++) {
            Item = this.Items[Index];
            if (this.GetItemValue(Item) === Value)
                return Index;
        }
        return -1;
    }
    /**
     * Checks or unchecks an item by index.
     * @param {number} Index The item index.
     * @param {boolean} Flag True to check.
     * @returns {void}
     */
    CheckIndex(Index, Flag) {
        var CurrentIndex = this.fSelectedIndexes.indexOf(Index);
        if (!tp.InRange(this.Items, Index))
            return;
        if (Flag === true && CurrentIndex === -1) {
            this.fSelectedIndexes.push(Index);
            this.OnSelectionChanged();
        } else if (Flag !== true && CurrentIndex !== -1) {
            this.fSelectedIndexes.splice(CurrentIndex, 1);
            this.OnSelectionChanged();
        }
    }
    /**
     * Checks or unchecks an item by value.
     * @param {*} Value The value.
     * @param {boolean} Flag True to check.
     * @returns {void}
     */
    CheckValue(Value, Flag) {
        this.CheckIndex(this.IndexOfValue(Value), Flag);
    }
    /**
     * Checks or unchecks an item.
     * @param {*} Item The item.
     * @param {boolean} Flag True to check.
     * @returns {void}
     */
    CheckItem(Item, Flag) {
        this.CheckIndex(this.Items.indexOf(Item), Flag);
    }
    /**
     * Returns true when an item index is checked.
     * @param {number} Index The item index.
     * @returns {boolean} Returns true when checked.
     */
    IsChecked(Index) {
        return this.fSelectedIndexes.indexOf(Index) !== -1;
    }
    /**
     * Returns true when an item value is checked.
     * @param {*} Value The value.
     * @returns {boolean} Returns true when checked.
     */
    IsValueChecked(Value) {
        return this.IsChecked(this.IndexOfValue(Value));
    }
    /**
     * Returns true when an item is checked.
     * @param {*} Item The item.
     * @returns {boolean} Returns true when checked.
     */
    IsItemChecked(Item) {
        return this.IsChecked(this.Items.indexOf(Item));
    }
    /**
     * Returns the selected items.
     * @returns {Array} Returns the selected items.
     */
    GetSelectedItems() {
        var Result = [];
        var Index;
        var i;
        for (i = 0; i < this.fSelectedIndexes.length; i++) {
            Index = this.fSelectedIndexes[i];
            if (tp.InRange(this.Items, Index))
                Result.push(this.Items[Index]);
        }
        return Result;
    }

    // ● properties
    /**
     * Gets or sets selected item indexes.
     * @returns {number[]} Returns selected indexes.
     */
    get SelectedIndexes() {
        return this.fSelectedIndexes.slice();
    }
    /**
     * Gets or sets selected item indexes.
     * @param {number[]} Value The selected indexes.
     * @returns {void}
     */
    set SelectedIndexes(Value) {
        var Index;
        this.fSelectedIndexes.length = 0;
        if (tp.IsArray(Value)) {
            for (Index = 0; Index < Value.length; Index++) {
                if (tp.InRange(this.Items, Value[Index]) && this.fSelectedIndexes.indexOf(Value[Index]) === -1)
                    this.fSelectedIndexes.push(Value[Index]);
            }
        }
        this.OnSelectionChanged();
    }
    /**
     * Gets or sets selected item values.
     * @returns {Array} Returns selected values.
     */
    get SelectedValues() {
        var Result = [];
        var List = this.GetSelectedItems();
        var Index;
        for (Index = 0; Index < List.length; Index++)
            Result.push(this.GetItemValue(List[Index]));
        return Result;
    }
    /**
     * Gets or sets selected item values.
     * @param {Array|null|undefined} Value The selected values.
     * @returns {void}
     */
    set SelectedValues(Value) {
        var Index;
        var ItemIndex;
        this.fSelectedIndexes.length = 0;
        if (tp.IsArray(Value)) {
            for (Index = 0; Index < Value.length; Index++) {
                ItemIndex = this.IndexOfValue(Value[Index]);
                if (ItemIndex !== -1 && this.fSelectedIndexes.indexOf(ItemIndex) === -1)
                    this.fSelectedIndexes.push(ItemIndex);
            }
        }
        this.OnSelectionChanged();
    }
    /**
     * Gets or sets the non data-bound items.
     * @returns {Array} Returns the items.
     */
    get Items() {
        return super.Items;
    }
    /**
     * Gets or sets the non data-bound items.
     * @param {Array|null|undefined} Value The items.
     * @returns {void}
     */
    set Items(Value) {
        this.Clear();
        if (tp.IsArray(Value))
            this.fItems.AddRange(Value);
        this.SetScrollerList();
    }

    // ● event triggers
    /**
     * Triggers the SelectionChanged event.
     * @protected
     * @returns {void}
     */
    OnSelectionChanged() {
        this.DoPost();
        this.SetScrollerList();
        this.Trigger("SelectionChanged", {});
    }
};

// ● check list box
/**
 * A multi-select list box with check boxes.
 *
 * Events:
 * - SelectionChanged
 */
tp.CheckListBox = class extends tp.CheckListControl {
    // ● private
    /**
     * Creates check-list-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateCheckListBoxParams(CreateParams) {
        var Args = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }

    // ● constructor
    /**
     * Creates a check-list box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.CheckListBox.CreateCheckListBoxParams(CreateParams));
    }

    // ● protected
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
        tp.AddClass(this.Handle, tp.Classes.CheckListBox);
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
        this.Handle.addEventListener("change", this.fChangeHandler, false);
    }
    /**
     * Makes the check-list box the focused control.
     * @returns {void}
     */
    Focus() {
        if (this.fScroller && this.fScroller.Viewport)
            this.fScroller.Viewport.focus();
        else
            super.Focus();
    }
};

tp.Ui.RegisterType(["CheckListBox", "tp-CheckListBox"], tp.CheckListBox);

// ● check combo box
/**
 * A multi-select combo-box control with check boxes.
 *
 * Example markup:
 * <pre>
 *     <div data-setup="{ListValueField: 'Id', ListDisplayField: 'Name', List: [{Id: 100, Name: 'All'}, {Id: 0, Name: 'No stops'}], SelectedValues: [100]}"></div>
 * </pre>
 *
 * Events:
 * - SelectionChanged
 *
 * @implements {tp.IDropDownBoxListener}
 */
tp.CheckComboBox = class extends tp.CheckListControl {
    // ● private
    /**
     * Creates check-combo-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateCheckComboBoxParams(CreateParams) {
        var Args = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }

    // ● constructor
    /**
     * Creates a check combo box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.CheckComboBox.CreateCheckComboBoxParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fListOnly = false;
        this.fMaxDropdownItems = 10;
        this.fLabels = [];
        this.fTextBoxInputHandler = this.FuncBind(this.HandleTextBoxInput);
        this.fTextBoxKeyDownHandler = this.FuncBind(this.HandleTextBoxKeyDown);
        this.fTextBoxFocusLostHandler = this.FuncBind(this.HandleTextBoxFocusLost);
        this.fStripClickHandler = this.FuncBind(this.HandleStripClick);
        this.fDocumentClickHandler = this.FuncBind(this.HandleDocumentClick);
        this.fContainerKeyDownHandler = this.FuncBind(this.HandleContainerKeyDown);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateInnerControls();
    }
    /**
     * Applies explicit create params to this check combo box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.ListOnly))
            this.ListOnly = Params.ListOnly === true;
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = Params.Placeholder;
        if (!tp.IsNil(Params.MaxDropdownItems))
            this.MaxDropdownItems = Params.MaxDropdownItems;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.ListControl);
        tp.AddClass(this.Handle, tp.Classes.CheckComboBox);
    }
    /**
     * Creates the inner controls and virtual scroller.
     * @protected
     * @returns {void}
     */
    CreateInnerControls() {
        if (this.fScroller)
            return;
        this.fControlContainer = this.Document.createElement("div");
        this.Handle.appendChild(this.fControlContainer);
        this.fControlContainer.className = tp.Classes.Strip;
        this.fTextBox = this.Document.createElement("input");
        this.fTextBox.type = "text";
        this.fTextBox.spellcheck = false;
        this.fTextBox.className = tp.Classes.Text;
        this.fTextBox.readOnly = this.ListOnly;
        this.fControlContainer.appendChild(this.fTextBox);
        this.fDropDownBox = new tp.DropDownBox(null, {
            Associate: this.fControlContainer,
            Owner: this,
            Parent: this.Handle
        });
        this.fContainer = this.Document.createElement("div");
        this.fDropDownBox.Handle.appendChild(this.fContainer);
        this.fContainer.className = tp.Classes.List;
        this.fContainer.tabIndex = -1;
        this.fScroller = new tp.VirtualScroller(this.fDropDownBox.Handle, this.fContainer);
        this.fScroller.RowHeight = this.ItemHeight;
        this.fScroller.Context = this;
        this.fScroller.RenderRowFunc = this.ItemRenderFunc;
        this.fScroller.ScrollFunc = this.ScrollFunc;
        this.fTextBox.addEventListener("input", this.fTextBoxInputHandler, false);
        this.fTextBox.addEventListener("keydown", this.fTextBoxKeyDownHandler, false);
        this.fTextBox.addEventListener("blur", this.fTextBoxFocusLostHandler, false);
        this.fControlContainer.addEventListener("click", this.fStripClickHandler, false);
        this.fContainer.addEventListener("change", this.fChangeHandler, false);
        this.fDropDownBox.Handle.addEventListener("keydown", this.fContainerKeyDownHandler, false);
        this.Document.addEventListener("click", this.fDocumentClickHandler, false);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fTextBox) {
            this.fTextBox.removeEventListener("input", this.fTextBoxInputHandler, false);
            this.fTextBox.removeEventListener("keydown", this.fTextBoxKeyDownHandler, false);
            this.fTextBox.removeEventListener("blur", this.fTextBoxFocusLostHandler, false);
        }
        if (this.fControlContainer)
            this.fControlContainer.removeEventListener("click", this.fStripClickHandler, false);
        if (this.fContainer)
            this.fContainer.removeEventListener("change", this.fChangeHandler, false);
        if (this.fDropDownBox && this.fDropDownBox.Handle)
            this.fDropDownBox.Handle.removeEventListener("keydown", this.fContainerKeyDownHandler, false);
        if (this.Document)
            this.Document.removeEventListener("click", this.fDocumentClickHandler, false);
        if (this.fDropDownBox) {
            this.fDropDownBox.Dispose();
            this.fDropDownBox = null;
        }
        this.fTextBoxInputHandler = null;
        this.fTextBoxKeyDownHandler = null;
        this.fTextBoxFocusLostHandler = null;
        this.fStripClickHandler = null;
        this.fDocumentClickHandler = null;
        this.fContainerKeyDownHandler = null;
        this.fTextBox = null;
        this.fControlContainer = null;
        this.fLabels = null;
        super.DoDispose();
    }
    /**
     * Handles text input for filtering.
     * @protected
     * @param {InputEvent} e The event.
     * @returns {void}
     */
    HandleTextBoxInput(e) {
        if (this.Enabled === true && this.ReadOnly !== true && this.ListOnly !== true)
            this.FilterScrollerList();
    }
    /**
     * Handles keyboard navigation in the text box.
     * @protected
     * @param {KeyboardEvent} e The event.
     * @returns {void}
     */
    HandleTextBoxKeyDown(e) {
        var List;
        var Item;
        var Index;
        if (this.Enabled !== true || this.ReadOnly === true)
            return;
        if (tp.IsKey(e, tp.Keys.Enter)) {
            if (this.fTextBox.value.length > 2) {
                List = this.fScroller.GetRowList();
                if (List.length > 0) {
                    Item = List[0];
                    Index = this.Items.indexOf(Item);
                    if (Index !== -1) {
                        tp.CancelEvent(e);
                        this.CheckIndex(Index, true);
                        this.fTextBox.value = "";
                        this.ResetScrollerList(this.Items, true);
                    }
                }
            }
        } else if (tp.IsKey(e, tp.Keys.Backspace)) {
            if (this.fTextBox.value.length === 0 && this.SelectedIndexes.length > 0) {
                tp.CancelEvent(e);
                Index = this.SelectedIndexes[this.SelectedIndexes.length - 1];
                this.CheckIndex(Index, false);
                this.FocusTextBox();
            }
        } else if (tp.IsKey(e, tp.Keys.Escape)) {
            tp.CancelEvent(e);
            this.Close();
        }
    }
    /**
     * Handles keyboard navigation in the drop-down list.
     * @protected
     * @param {KeyboardEvent} e The event.
     * @returns {void}
     */
    HandleContainerKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Escape)) {
            tp.CancelEvent(e);
            this.Close();
            this.FocusTextBox();
        }
    }
    /**
     * Handles text-box focus loss.
     * @protected
     * @param {FocusEvent} e The event.
     * @returns {void}
     */
    HandleTextBoxFocusLost(e) {
        if (this.Enabled === true && this.ReadOnly !== true) {
            this.fTextBox.value = "";
            this.ResetScrollerList(this.Items, false);
        }
    }
    /**
     * Handles clicks on the strip.
     * @protected
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleStripClick(e) {
        var Info;
        if (this.Enabled !== true || this.ReadOnly === true)
            return;
        if (tp.HasClass(e.target, tp.Classes.Close) && tp.HasElementInfo(e.target)) {
            tp.CancelEvent(e);
            Info = tp.GetElementInfo(e.target);
            this.CheckIndex(Info.Index, false);
            this.Close();
        } else {
            this.Open();
            this.FocusTextBox();
        }
    }
    /**
     * Handles document clicks.
     * @protected
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleDocumentClick(e) {
        if (this.Enabled === true && this.fDropDownBox && this.fDropDownBox.Resizing !== true && !tp.ContainsEventTarget(this.Handle, e.target))
            this.Close();
    }
    /**
     * Updates selected item labels in the strip.
     * @protected
     * @returns {void}
     */
    UpdateLabels() {
        var List = this.SelectedIndexes;
        var Index;
        var Item;
        var Label;
        var Text;
        var CloseButton;
        var i;
        if (!this.fControlContainer || !this.fTextBox)
            return;
        for (i = 0; i < this.fLabels.length; i++) {
            if (this.fLabels[i].parentNode)
                this.fLabels[i].parentNode.removeChild(this.fLabels[i]);
        }
        this.fLabels.length = 0;
        if (this.fTextBox.parentNode)
            this.fTextBox.parentNode.removeChild(this.fTextBox);
        for (i = 0; i < List.length; i++) {
            Index = List[i];
            Item = this.Items[Index];
            if (tp.IsNil(Item))
                continue;
            Label = this.Document.createElement("div");
            Label.className = tp.Classes.Item;
            Text = this.Document.createElement("div");
            Text.className = tp.Classes.Text;
            Text.textContent = this.GetItemText(Item);
            CloseButton = this.Document.createElement("div");
            CloseButton.className = tp.Classes.Close;
            CloseButton.textContent = "x";
            tp.SetElementInfo(CloseButton, { Index: Index });
            Label.appendChild(Text);
            Label.appendChild(CloseButton);
            this.fControlContainer.appendChild(Label);
            this.fLabels.push(Label);
        }
        this.fControlContainer.appendChild(this.fTextBox);
        if (this.IsOpen === true)
            setTimeout(this.FuncBind(function () { this.fDropDownBox.UpdateTop(); }), 0);
    }
    /**
     * Updates the drop-down height.
     * @protected
     * @returns {void}
     */
    UpdateDropdownHeight() {
        var Count = this.fScroller ? this.fScroller.RowListCount : 0;
        var RowCount = Count <= 0 ? 2 : Count < this.MaxDropdownItems ? Count + 1 : this.MaxDropdownItems;
        this.fDropDownBox.Height = RowCount * this.ItemHeight + 5;
    }
    /**
     * Returns items containing a specified text.
     * @protected
     * @param {string} Text The search text.
     * @returns {Array} Returns matching items.
     */
    GetItemsContainingText(Text) {
        var Result = [];
        var ItemText;
        var i;
        for (i = 0; i < this.Items.length; i++) {
            ItemText = this.GetItemText(this.Items[i]);
            if (!tp.IsBlank(ItemText) && tp.ContainsText(ItemText, Text, true))
                Result.push(this.Items[i]);
        }
        return Result;
    }
    /**
     * Filters the scroller list.
     * @protected
     * @returns {void}
     */
    FilterScrollerList() {
        var Text = this.fTextBox.value;
        var List;
        if (Text.length === 0 || Text.length >= 3) {
            List = tp.IsBlank(Text) ? this.Items : this.GetItemsContainingText(Text);
            this.Open();
            this.fScroller.SetRowList(List);
            this.UpdateDropdownHeight();
            this.UpdateScroller();
        }
    }
    /**
     * Resets the scroller list.
     * @protected
     * @param {Array} List The list.
     * @param {boolean} FocusToTextBox True to focus the text box.
     * @returns {void}
     */
    ResetScrollerList(List, FocusToTextBox) {
        this.fScroller.SetRowList(List);
        this.UpdateDropdownHeight();
        this.UpdateScroller();
        if (FocusToTextBox === true)
            this.FocusTextBox();
    }
    /**
     * Focuses the inner text box.
     * @protected
     * @returns {void}
     */
    FocusTextBox() {
        setTimeout(this.FuncBind(function () {
            if (this.fTextBox)
                this.fTextBox.focus();
        }), 0);
    }

    // ● public
    /**
     * Displays the drop-down box.
     * @returns {void}
     */
    Open() {
        if (this.ReadOnly !== true && this.Enabled === true)
            this.fDropDownBox.Open();
    }
    /**
     * Hides the drop-down box.
     * @returns {void}
     */
    Close() {
        if (this.ReadOnly !== true && this.Enabled === true)
            this.fDropDownBox.Close();
    }
    /**
     * Displays or hides the drop-down box.
     * @returns {void}
     */
    Toggle() {
        if (this.ReadOnly !== true && this.Enabled === true)
            this.fDropDownBox.Toggle();
    }
    /**
     * Called by the drop-down box to inform its owner about a stage change.
     * @param {tp.DropDownBox} Sender The sender.
     * @param {number} Stage One of the tp.DropDownBoxStage constants.
     * @returns {void}
     */
    OnDropDownBoxEvent(Sender, Stage) {
        switch (Stage) {
            case tp.DropDownBoxStage.Opening:
                tp.ZIndex(this.fScroller.Viewport, this.ZIndex + 1);
                this.fScroller.RowHeight = this.ItemHeight;
                break;
            case tp.DropDownBoxStage.Opened:
                this.UpdateDropdownHeight();
                this.UpdateScroller();
                if (this.ListOnly === true)
                    this.fScroller.Viewport.focus();
                break;
        }
    }
    /**
     * Makes the check combo box the focused control.
     * @returns {void}
     */
    Focus() {
        this.FocusTextBox();
    }

    // ● properties
    /**
     * Gets or sets a value indicating whether the text-box portion is read-only list-only input.
     * @returns {boolean} Returns true when list-only.
     */
    get ListOnly() {
        return this.fListOnly === true;
    }
    /**
     * Gets or sets a value indicating whether the text-box portion is read-only list-only input.
     * @param {boolean} Value True to make the text-box read-only.
     * @returns {void}
     */
    set ListOnly(Value) {
        this.fListOnly = Value === true;
        if (this.fTextBox)
            this.fTextBox.readOnly = this.fListOnly;
    }
    /**
     * Gets or sets the maximum number of items shown in the drop-down list.
     * @returns {number} Returns the maximum number of items.
     */
    get MaxDropdownItems() {
        var Result = tp.IsNumber(this.fMaxDropdownItems) ? this.fMaxDropdownItems : 10;
        return Result > 30 ? 30 : Result;
    }
    /**
     * Gets or sets the maximum number of items shown in the drop-down list.
     * @param {number} Value The maximum number of items.
     * @returns {void}
     */
    set MaxDropdownItems(Value) {
        this.fMaxDropdownItems = tp.IsNumber(Value) ? Value : 10;
    }
    /**
     * Returns true while the drop-down box is visible.
     * @returns {boolean} Returns true while open.
     */
    get IsOpen() {
        return this.fDropDownBox ? this.fDropDownBox.IsOpen : false;
    }
    /**
     * Gets or sets the text-box placeholder.
     * @returns {string} Returns the placeholder.
     */
    get Placeholder() {
        return this.fTextBox ? this.fTextBox.placeholder : "";
    }
    /**
     * Gets or sets the text-box placeholder.
     * @param {string} Value The placeholder.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.fTextBox)
            this.fTextBox.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }

    // ● event triggers
    /**
     * Triggers the SelectionChanged event.
     * @protected
     * @returns {void}
     */
    OnSelectionChanged() {
        this.UpdateLabels();
        super.OnSelectionChanged();
    }
};

tp.Ui.RegisterType(["CheckComboBox", "tp-CheckComboBox"], tp.CheckComboBox);
