// ● combo box
/**
 * A virtual-scroller based combo-box control.
 *
 * Example markup:
 * <pre>
 *     <div data-setup="{ListOnly: true, ListValueField: 'Id', ListDisplayField: 'Name', List: [{Id: 100, Name: 'All'}, {Id: 0, Name: 'No stops'}], SelectedIndex: 0 }"></div>
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
 * - TextNotFound
 *
 * @implements {tp.IDropDownBoxListener}
 */
tp.ComboBox = class extends tp.ListControl {
    // ● private
    /**
     * Creates combo-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateComboBoxParams(CreateParams) {
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
     * Creates a combo box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.ComboBox.CreateComboBoxParams(CreateParams));
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
        this.fListOnly = true;
        this.fMaxDropdownItems = 10;
        this.fDropDownSelectedIndex = -1;
        this.fTextBoxChangeHandler = this.FuncBind(this.HandleTextBoxChange);
        this.fTextBoxKeyDownHandler = this.FuncBind(this.HandleTextBoxKeyDown);
        this.fTextBoxKeyPressHandler = this.FuncBind(this.HandleTextBoxKeyPress);
        this.fButtonClickHandler = this.FuncBind(this.HandleButtonClick);
        this.fContainerClickHandler = this.FuncBind(this.HandleContainerClick);
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
     * Applies explicit create params to this combo box.
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
        if (!tp.IsNil(Params.Text))
            this.Text = Params.Text;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.ListControl);
        tp.AddClass(this.Handle, tp.Classes.ComboBox);
    }
    /**
     * Creates the inner controls and virtual scroller.
     * @protected
     * @returns {void}
     */
    CreateInnerControls() {
        var ControlContainer;
        if (this.fScroller)
            return;
        ControlContainer = this.Document.createElement("div");
        this.Handle.appendChild(ControlContainer);
        ControlContainer.className = tp.Classes.Strip;
        this.fControlContainer = ControlContainer;
        this.fTextBox = this.Document.createElement("input");
        this.fTextBox.type = "text";
        this.fTextBox.spellcheck = false;
        this.fTextBox.className = tp.Classes.Text;
        this.fTextBox.readOnly = this.ListOnly;
        ControlContainer.appendChild(this.fTextBox);
        this.fButton = this.Document.createElement("div");
        this.fButton.className = tp.Classes.Btn;
        this.fButton.innerHTML = "&#9662;";
        ControlContainer.appendChild(this.fButton);
        this.fDropDownBox = new tp.DropDownBox(null, {
            Associate: ControlContainer,
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
        this.fTextBox.addEventListener("change", this.fTextBoxChangeHandler, false);
        this.fTextBox.addEventListener("keydown", this.fTextBoxKeyDownHandler, false);
        this.fTextBox.addEventListener("keypress", this.fTextBoxKeyPressHandler, false);
        this.fButton.addEventListener("click", this.fButtonClickHandler, false);
        this.fContainer.addEventListener("click", this.fContainerClickHandler, false);
        this.fDropDownBox.Handle.addEventListener("keydown", this.fContainerKeyDownHandler, false);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fTextBox) {
            this.fTextBox.removeEventListener("change", this.fTextBoxChangeHandler, false);
            this.fTextBox.removeEventListener("keydown", this.fTextBoxKeyDownHandler, false);
            this.fTextBox.removeEventListener("keypress", this.fTextBoxKeyPressHandler, false);
        }
        if (this.fButton)
            this.fButton.removeEventListener("click", this.fButtonClickHandler, false);
        if (this.fContainer) {
            this.fContainer.removeEventListener("click", this.fContainerClickHandler, false);
            this.fDropDownBox.Handle.removeEventListener("keydown", this.fContainerKeyDownHandler, false);
        }
        if (this.fDropDownBox) {
            this.fDropDownBox.Dispose();
            this.fDropDownBox = null;
        }
        this.fTextBoxChangeHandler = null;
        this.fTextBoxKeyDownHandler = null;
        this.fTextBoxKeyPressHandler = null;
        this.fButtonClickHandler = null;
        this.fContainerClickHandler = null;
        this.fContainerKeyDownHandler = null;
        this.fTextBox = null;
        this.fButton = null;
        this.fControlContainer = null;
        super.DoDispose();
    }
    /**
     * Sets a specified text.
     * @protected
     * @param {string} Text The text.
     * @returns {void}
     */
    DoSetText(Text) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.value = tp.IsNil(Text) ? "" : String(Text);
    }
    /**
     * Returns the text of an item starting with a specified text.
     * @protected
     * @param {string} Text The text.
     * @returns {string|null} Returns the item text or null.
     */
    GetItemTextStartingWith(Text) {
        var Index;
        var ItemText;
        for (Index = 0; Index < this.Items.length; Index++) {
            ItemText = this.GetItemText(this.Items[Index]);
            if (!tp.IsBlank(ItemText) && tp.StartsWith(ItemText, Text, true))
                return ItemText;
        }
        return null;
    }
    /**
     * Validates typed text against list items.
     * @protected
     * @returns {void}
     */
    CommitText() {
        var Text = tp.Trim(this.Text);
        var Index;
        if (!tp.IsBlank(Text)) {
            Index = this.IndexOfText(Text);
            if (Index >= 0) {
                this.SelectedIndex = Index;
            } else {
                if (this.IsDataBound)
                    this.DoClearValue(true);
                this.OnTextNotFound(Text);
            }
        } else if (this.IsDataBound) {
            this.DoClearValue(true);
        }
    }
    /**
     * Replaces selected text in the inner text box.
     * @protected
     * @param {string} Text The replacement text.
     * @returns {void}
     */
    ReplaceSelectedText(Text) {
        if (this.fTextBox instanceof HTMLInputElement && tp.IsFunction(this.fTextBox.setRangeText)) {
            this.fTextBox.setRangeText(Text, this.fTextBox.selectionStart, this.fTextBox.selectionEnd, "end");
        }
    }
    /**
     * Handles inner text box change.
     * @protected
     * @param {Event} e The event.
     * @returns {void}
     */
    HandleTextBoxChange(e) {
        tp.CancelEvent(e);
        this.CommitText();
    }
    /**
     * Handles drop-down button click.
     * @protected
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleButtonClick(e) {
        if (this.Enabled === true) {
            tp.CancelEvent(e);
            this.Toggle();
            if (this.IsOpen === false && this.fTextBox)
                this.fTextBox.focus();
        }
    }
    /**
     * Handles inner text box keydown.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleTextBoxKeyDown(e) {
        if (this.Enabled !== true || this.ReadOnly === true || e.target !== this.fTextBox)
            return;
        if (tp.IsKey(e, tp.Keys.Up) || tp.IsKey(e, tp.Keys.Down)) {
            tp.CancelEvent(e);
            if (e.altKey === true) {
                if (!this.IsOpen)
                    this.Open();
            } else if (this.IsOpen) {
                this.MoveDropDownSelectedIndex(tp.IsKey(e, tp.Keys.Up) ? -1 : 1);
            } else if (tp.IsKey(e, tp.Keys.Up)) {
                this.SelectedIndex = this.SelectedIndex - 1;
            } else {
                this.SelectedIndex = this.SelectedIndex + 1;
            }
        }
    }
    /**
     * Handles inner text box keypress for editable autocomplete behavior.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleTextBoxKeyPress(e) {
        var Element;
        var Start;
        var Text;
        if (this.Enabled !== true || this.ReadOnly === true || e.target !== this.fTextBox || this.ListOnly === true || !tp.IsPrintableKey(e))
            return;
        e.preventDefault();
        Element = this.fTextBox;
        this.ReplaceSelectedText(e.key);
        Start = Element.value.length;
        Text = this.GetItemTextStartingWith(Element.value);
        if (!tp.IsBlank(Text)) {
            Element.value = Text;
            Element.setSelectionRange(Start, Element.value.length);
        }
    }
    /**
     * Handles row container click.
     * @protected
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleContainerClick(e) {
        var Info;
        var Element = e.target;
        if (this.Enabled !== true || !this.fDropDownBox || this.fDropDownBox.Resizing === true)
            return;
        if (tp.ContainsEventTarget(this.fScroller.Container, Element)
            && tp.HasClass(Element, tp.Classes.Item)
            && tp.HasElementInfo(Element)) {
            Info = tp.GetElementInfo(Element);
            this.SelectedIndex = Info.Index;
            this.Close();
            this.fTextBox.focus();
        }
    }
    /**
     * Handles row container keydown.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleContainerKeyDown(e) {
        var Info;
        if (!(e instanceof KeyboardEvent))
            return;
        if (this.HandleScrollerKeyDown(e) === true)
            return;
        if (tp.ContainsEventTarget(this.fScroller.Container, e.target) && tp.HasClass(e.target, tp.Classes.Item)) {
            if (tp.IsKey(e, tp.Keys.Enter)) {
                tp.CancelEvent(e);
                Info = tp.GetElementInfo(e.target);
                this.SelectedIndex = Info.Index;
                this.Close();
                this.fTextBox.focus();
            } else if (tp.IsKey(e, tp.Keys.Escape)) {
                e.preventDefault();
                this.Close();
                this.fTextBox.focus();
            }
        }
    }
    /**
     * Moves the temporary dropdown selection by a delta.
     * @protected
     * @param {number} Delta The selection delta.
     * @returns {void}
     */
    MoveDropDownSelectedIndex(Delta) {
        if (!this.fScroller || this.Items.length === 0)
            return;
        this.SetScrollerIndexIndication(this.fDropDownSelectedIndex, false);
        this.fDropDownSelectedIndex = this.GetMovedScrollerIndex(this.fDropDownSelectedIndex, Delta);
        this.ScrollIndexIntoView(this.fDropDownSelectedIndex);
    }
    /**
     * Handles virtual scroller keyboard navigation while the dropdown is open.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {boolean} Returns true when handled.
     */
    HandleScrollerKeyDown(e) {
        if (!(e instanceof KeyboardEvent) || this.Enabled !== true || this.ReadOnly === true)
            return false;
        if (tp.IsKey(e, tp.Keys.Up)) {
            tp.CancelEvent(e);
            this.MoveDropDownSelectedIndex(-1);
            return true;
        }
        if (tp.IsKey(e, tp.Keys.Down)) {
            tp.CancelEvent(e);
            this.MoveDropDownSelectedIndex(1);
            return true;
        }
        if (tp.IsKey(e, tp.Keys.Enter) || tp.IsKey(e, tp.Keys.Space)) {
            if (this.AcceptScrollerSelection(e) === true) {
                tp.CancelEvent(e);
                return true;
            }
        }
        return false;
    }
    /**
     * Handles a keyboard request to accept the current scroller selection.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {boolean} Returns true when handled.
     */
    AcceptScrollerSelection(e) {
        if (tp.InRange(this.Items, this.fDropDownSelectedIndex))
            this.SelectedIndex = this.fDropDownSelectedIndex;
        this.Close();
        if (this.fTextBox)
            this.fTextBox.focus();
        return true;
    }
    /**
     * Virtual scroller callback before and after rendering.
     * @protected
     * @param {number} Phase The render phase. 1 is before, 2 is after.
     * @returns {void}
     */
    ScrollFunc(Phase) {
        if (this.IsOpen) {
            if (Phase === 1)
                this.SetScrollerIndexIndication(this.fDropDownSelectedIndex, false);
            else if (Phase === 2)
                this.SetScrollerIndexIndication(this.fDropDownSelectedIndex, true);
        } else {
            super.ScrollFunc(Phase);
        }
    }
    /**
     * Called after Required changes.
     * @protected
     * @returns {void}
     */
    OnRequiredChanged() {
        this.SetRequiredMark(this.fTextBox);
        super.OnRequiredChanged();
    }

    // ● public
    /**
     * Opens the drop-down list.
     * @returns {void}
     */
    Open() {
        if (this.ReadOnly !== true && this.Enabled === true && this.fDropDownBox)
            this.fDropDownBox.Open();
    }
    /**
     * Closes the drop-down list.
     * @returns {void}
     */
    Close() {
        if (this.ReadOnly !== true && this.Enabled === true && this.fDropDownBox)
            this.fDropDownBox.Close();
    }
    /**
     * Opens or closes the drop-down list.
     * @returns {void}
     */
    Toggle() {
        if (this.ReadOnly !== true && this.Enabled === true && this.fDropDownBox)
            this.fDropDownBox.Toggle();
    }
    /**
     * Called by the drop-down box when its stage changes.
     * @param {tp.DropDownBox} Sender The sender.
     * @param {number} Stage One of the tp.DropDownBoxStage values.
     * @returns {void}
     */
    OnDropDownBoxEvent(Sender, Stage) {
        var Count;
        var Height;
        var Index;
        if (Stage === tp.DropDownBoxStage.Opening) {
            this.fScroller.RowHeight = this.ItemHeight;
        } else if (Stage === tp.DropDownBoxStage.Opened) {
            Count = this.Items.length;
            Height = Count <= 0 ? 2 : (Count < this.MaxDropdownItems ? Count + 1 : this.MaxDropdownItems);
            this.fDropDownBox.Height = Height * this.ItemHeight + 5;
            this.UpdateScroller();
            this.SetScrollerIndexIndication(-1, false);
            this.fDropDownSelectedIndex = this.SelectedIndex;
            Index = this.fDropDownSelectedIndex;
            if (tp.InRange(this.Items, Index)) {
                this.fScroller.Viewport.scrollTop = Index * this.fScroller.RowHeight;
                this.fScroller.Render();
            }
            this.SetScrollerIndexIndication(this.fDropDownSelectedIndex, true);
            this.fScroller.Viewport.focus();
        }
    }
    /**
     * Returns true if this control is valid.
     * @returns {boolean} Returns true when valid.
     */
    CheckValidity() {
        return tp.IsValidatableElement(this.fTextBox) ? this.fTextBox.checkValidity() : true;
    }
    /**
     * Sets a custom validation message.
     * @param {string} MessageText The validation message.
     * @returns {void}
     */
    SetValidationMessage(MessageText) {
        if (tp.IsValidatableElement(this.fTextBox))
            this.fTextBox.setCustomValidity(MessageText);
    }

    // ● properties
    /**
     * Gets or sets whether typing is disabled and selection is list-only.
     * @returns {boolean} Returns true when list-only.
     */
    get ListOnly() {
        return this.fListOnly === true;
    }
    /**
     * Gets or sets whether typing is disabled and selection is list-only.
     * @param {boolean} Value True for list-only.
     * @returns {void}
     */
    set ListOnly(Value) {
        Value = Value === true;
        if (this.fListOnly !== Value) {
            this.fListOnly = Value;
            if (this.fTextBox)
                this.fTextBox.readOnly = this.fListOnly;
        }
    }
    /**
     * Gets or sets editable text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.fTextBox instanceof HTMLInputElement ? this.fTextBox.value : "";
    }
    /**
     * Gets or sets editable text.
     * @param {*} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        if (!this.ListOnly && this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.value = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets placeholder text.
     * @returns {string} Returns the placeholder.
     */
    get Placeholder() {
        return this.fTextBox instanceof HTMLInputElement ? this.fTextBox.placeholder : "";
    }
    /**
     * Gets or sets placeholder text.
     * @param {string} Value The placeholder.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the maximum number of visible dropdown items.
     * @returns {number} Returns the maximum item count.
     */
    get MaxDropdownItems() {
        var Result = this.fMaxDropdownItems || 10;
        return Result > 30 ? 30 : Result;
    }
    /**
     * Gets or sets the maximum number of visible dropdown items.
     * @param {number|string} Value The maximum item count.
     * @returns {void}
     */
    set MaxDropdownItems(Value) {
        this.fMaxDropdownItems = tp.StrToInt(Value, 10);
    }
    /**
     * Returns true while the dropdown box is visible.
     * @returns {boolean} Returns true when open.
     */
    get IsOpen() {
        return this.fDropDownBox ? this.fDropDownBox.IsOpen : false;
    }

    // ● event triggers
    /**
     * Triggers the TextNotFound event.
     * @protected
     * @param {string} Text The text that was not found.
     * @returns {void}
     */
    OnTextNotFound(Text) {
        this.Trigger("TextNotFound", { Text: Text });
    }
    /**
     * Triggers the SelectedIndexChanged event.
     * @protected
     * @returns {void}
     */
    OnSelectedIndexChanged() {
        this.Trigger("SelectedIndexChanged", {});
    }
};

tp.Ui.RegisterType(["ComboBox", "tp-ComboBox"], tp.ComboBox);
