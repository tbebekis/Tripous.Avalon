// ● html list control
/**
 * Base class for native HTML select controls.
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
tp.HtmlListControl = class extends tp.Control {
    // ● private
    /**
     * Creates html-list-control create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateHtmlListParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "select";
            return Args;
        }
        Args = tp.IsObject(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "select";
        return Args;
    }

    // ● constructor
    /**
     * Creates an html list control.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.HtmlListControl.CreateHtmlListParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDataBindMode = tp.ControlBindMode.List;
        this.fDataValueProperty = "SelectedIndex";
        this.fChangeHandler = this.FuncBind(this.HandleChange);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        if (this.Handle instanceof HTMLSelectElement)
            this.Handle.addEventListener("change", this.fChangeHandler, false);
    }
    /**
     * Applies explicit create params to this html list control.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Width = Params.Width;
        if (!tp.IsNil(Params.Height))
            this.Height = Params.Height;
        if (!tp.IsNil(Params.List))
            this.AddRange(Params.List);
        if (!tp.IsNil(Params.ListItems))
            this.AddRange(Params.ListItems);
        if (!tp.IsNil(Params.Items))
            this.AddRange(Params.Items);
        if (!tp.IsNil(Params.SelectedIndex))
            this.SelectedIndex = Params.SelectedIndex;
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
     * Handles change events.
     * @protected
     * @param {Event} e The event.
     * @returns {void}
     */
    HandleChange(e) {
        this.OnSelectedIndexChanged();
    }

    // ● public
    /**
     * Removes all options.
     * @returns {void}
     */
    Clear() {
        var Index;
        if (this.Handle instanceof HTMLSelectElement) {
            for (Index = this.Handle.options.length - 1; Index >= 0; Index--)
                this.Handle.remove(Index);
        }
    }
    /**
     * Adds and returns an option.
     * @param {string} Text The option text.
     * @param {string} Value The option value.
     * @returns {HTMLOptionElement|null} Returns the option or null.
     */
    Add(Text, Value) {
        var Result;
        if (this.Handle instanceof HTMLSelectElement) {
            Result = this.Handle.ownerDocument.createElement("option");
            Result.text = tp.IsNil(Text) ? "" : String(Text);
            Result.value = tp.IsNil(Value) ? "" : String(Value);
            this.Handle.add(Result);
            return Result;
        }
        return null;
    }
    /**
     * Inserts and returns an option.
     * @param {number} Index The item index.
     * @param {string} Text The option text.
     * @param {string} Value The option value.
     * @returns {HTMLOptionElement|null} Returns the option or null.
     */
    Insert(Index, Text, Value) {
        var Result;
        if (this.Handle instanceof HTMLSelectElement) {
            Result = this.Handle.ownerDocument.createElement("option");
            Result.text = tp.IsNil(Text) ? "" : String(Text);
            Result.value = tp.IsNil(Value) ? "" : String(Value);
            this.Handle.add(Result, tp.StrToInt(Index, this.Handle.options.length));
            return Result;
        }
        return null;
    }
    /**
     * Removes an option by index.
     * @param {number} Index The item index.
     * @returns {void}
     */
    RemoveAt(Index) {
        if (this.Handle instanceof HTMLSelectElement)
            this.Handle.remove(Index);
    }
    /**
     * Adds a range of options from an object list.
     * Each item may contain Id/Name or Value/Text properties.
     * @param {object[]} List The source list.
     * @returns {void}
     */
    AddRange(List) {
        var Index;
        var Item;
        var Text;
        var Value;
        if (!(this.Handle instanceof HTMLSelectElement) || !tp.IsArray(List))
            return;
        for (Index = 0; Index < List.length; Index++) {
            Item = List[Index];
            Value = "";
            Text = "";
            if (tp.IsObject(Item)) {
                if ("Id" in Item)
                    Value = Item.Id;
                else if ("Value" in Item)
                    Value = Item.Value;
                if ("Name" in Item)
                    Text = Item.Name;
                else if ("Text" in Item)
                    Text = Item.Text;
            } else if (!tp.IsNil(Item)) {
                Text = String(Item);
                Value = Text;
            }
            if (Text === "")
                Text = Index.toString();
            if (Value === "")
                Value = Text;
            this.Add(Text, Value);
        }
    }
    /**
     * Returns the index of an option by text.
     * @param {string} Text The text.
     * @returns {number} Returns the index or -1.
     */
    IndexOfText(Text) {
        var Index;
        var List;
        if (this.Handle instanceof HTMLSelectElement) {
            List = this.Handle.options;
            for (Index = 0; Index < List.length; Index++) {
                if (Text === List[Index].text)
                    return Index;
            }
        }
        return -1;
    }
    /**
     * Returns the index of an option by value.
     * @param {string} Value The value.
     * @returns {number} Returns the index or -1.
     */
    IndexOfValue(Value) {
        var Index;
        var List;
        if (this.Handle instanceof HTMLSelectElement) {
            Value = tp.IsNil(Value) ? "" : String(Value);
            List = this.Handle.options;
            for (Index = 0; Index < List.length; Index++) {
                if (Value === List[Index].value)
                    return Index;
            }
        }
        return -1;
    }
    /**
     * Returns an option by index.
     * @param {number} Index The item index.
     * @returns {HTMLOptionElement|null} Returns the option or null.
     */
    ItemAt(Index) {
        return this.Handle instanceof HTMLSelectElement ? this.Handle.options[Index] || null : null;
    }
    /**
     * Returns the text of an option.
     * @param {number} Index The item index.
     * @returns {string} Returns the option text.
     */
    GetTextAt(Index) {
        var Item = this.ItemAt(Index);
        return Item ? Item.text : "";
    }
    /**
     * Sets the text of an option.
     * @param {number} Index The item index.
     * @param {string} Text The text.
     * @returns {void}
     */
    SetTextAt(Index, Text) {
        var Item = this.ItemAt(Index);
        if (Item)
            Item.text = tp.IsNil(Text) ? "" : String(Text);
    }
    /**
     * Returns the value of an option.
     * @param {number} Index The item index.
     * @returns {string} Returns the option value.
     */
    GetValueAt(Index) {
        var Item = this.ItemAt(Index);
        return Item ? Item.value : "";
    }
    /**
     * Sets the value of an option.
     * @param {number} Index The item index.
     * @param {string} Value The value.
     * @returns {void}
     */
    SetValueAt(Index, Value) {
        var Item = this.ItemAt(Index);
        if (Item)
            Item.value = tp.IsNil(Value) ? "" : String(Value);
    }

    // ● properties
    /**
     * Gets the number of options.
     * @returns {number} Returns the option count.
     */
    get Count() {
        return this.Handle instanceof HTMLSelectElement ? this.Handle.length : 0;
    }
    /**
     * Gets or sets the selected index.
     * @returns {number} Returns the selected index.
     */
    get SelectedIndex() {
        return this.Handle instanceof HTMLSelectElement ? this.Handle.selectedIndex : -1;
    }
    /**
     * Gets or sets the selected index.
     * @param {number} Value The selected index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
        if (this.Handle instanceof HTMLSelectElement)
            this.Handle.selectedIndex = tp.StrToInt(Value, -1);
    }
    /**
     * Gets the selected option.
     * @returns {HTMLOptionElement|null} Returns the selected option or null.
     */
    get SelectedItem() {
        return this.Handle instanceof HTMLSelectElement && this.Handle.selectedIndex > -1 ? this.Handle.options[this.Handle.selectedIndex] : null;
    }
    /**
     * Gets the selected value.
     * @returns {string|null} Returns the selected value or null.
     */
    get SelectedValue() {
        return this.SelectedItem ? this.SelectedItem.value : null;
    }
    /**
     * Gets the option collection.
     * @returns {HTMLOptionsCollection|null} Returns the options or null.
     */
    get Items() {
        return this.Handle instanceof HTMLSelectElement ? this.Handle.options : null;
    }
    /**
     * Gets or replaces the list items.
     * @returns {HTMLOptionsCollection|null} Returns the options or null.
     */
    get List() {
        return this.Items;
    }
    /**
     * Gets or replaces the list items.
     * @param {object[]|string[]|null|undefined} Value The list items.
     * @returns {void}
     */
    set List(Value) {
        this.Clear();
        this.AddRange(Value);
    }
    /**
     * Gets or sets CSS width.
     * @returns {string} Returns the width.
     */
    get Width() {
        return this.Handle instanceof HTMLElement ? this.Handle.style.width || "" : "";
    }
    /**
     * Gets or sets CSS width.
     * @param {number|string} Value The width.
     * @returns {void}
     */
    set Width(Value) {
        if (this.Handle instanceof HTMLElement)
            this.Handle.style.width = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }
    /**
     * Gets or sets CSS height.
     * @returns {string} Returns the height.
     */
    get Height() {
        return this.Handle instanceof HTMLElement ? this.Handle.style.height || "" : "";
    }
    /**
     * Gets or sets CSS height.
     * @param {number|string} Value The height.
     * @returns {void}
     */
    set Height(Value) {
        if (this.Handle instanceof HTMLElement)
            this.Handle.style.height = tp.IsNumber(Value) ? tp.px(Value) : String(Value);
    }

    // ● event triggers
    /**
     * Triggers the SelectedIndexChanged event.
     * @protected
     * @returns {void}
     */
    OnSelectedIndexChanged() {
        this.Trigger("SelectedIndexChanged", {});
    }
};

// ● html combo box
/**
 * A combo-box built on a native HTML select element.
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
tp.HtmlComboBox = class extends tp.HtmlListControl {
    // ● constructor
    /**
     * Creates an html combo box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.HtmlListControl);
        tp.AddClass(this.Handle, tp.Classes.HtmlComboBox);
    }
};

// ● html list box
/**
 * A single-select or multi-select list-box built on a native HTML select element.
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
tp.HtmlListBox = class extends tp.HtmlListControl {
    // ● constructor
    /**
     * Creates an html list box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        if (this.Handle instanceof HTMLSelectElement)
            this.Handle.size = 8;
    }
    /**
     * Applies explicit create params to this html list box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.VisibleItemCount))
            this.VisibleItemCount = Params.VisibleItemCount;
        if (!tp.IsNil(Params.MultiSelect))
            this.MultiSelect = Params.MultiSelect === true;
        if (!tp.IsNil(Params.SelectedIndexes))
            this.SelectedIndexes = Params.SelectedIndexes;
        if (!tp.IsNil(Params.SelectedValues))
            this.SelectedValues = Params.SelectedValues;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.HtmlListControl);
        tp.AddClass(this.Handle, tp.Classes.HtmlListBox);
    }

    // ● public
    /**
     * Selects or deselects all options.
     * @param {boolean} Flag True to select all.
     * @returns {void}
     */
    SelectAll(Flag) {
        var Index;
        var Items;
        if (this.Handle instanceof HTMLSelectElement) {
            Items = this.Handle.options;
            for (Index = 0; Index < Items.length; Index++)
                Items[Index].selected = Flag === true;
        }
    }
    /**
     * Returns all selected options.
     * @returns {HTMLOptionElement[]} Returns the selected options.
     */
    GetSelectedItems() {
        var Result = [];
        var Index;
        var Items;
        if (this.Handle instanceof HTMLSelectElement) {
            Items = this.Handle.options;
            for (Index = 0; Index < Items.length; Index++) {
                if (Items[Index].selected === true)
                    Result.push(Items[Index]);
            }
        }
        return Result;
    }

    // ● properties
    /**
     * Gets or sets the visible item count.
     * @returns {number} Returns the visible item count.
     */
    get VisibleItemCount() {
        return this.Handle instanceof HTMLSelectElement ? this.Handle.size : 0;
    }
    /**
     * Gets or sets the visible item count.
     * @param {number|string} Value The visible item count.
     * @returns {void}
     */
    set VisibleItemCount(Value) {
        if (this.Handle instanceof HTMLSelectElement)
            this.Handle.size = tp.StrToInt(Value, 0);
    }
    /**
     * Gets or sets whether multiple items can be selected.
     * @returns {boolean} Returns true when multi-select is enabled.
     */
    get MultiSelect() {
        return this.Handle instanceof HTMLSelectElement ? this.Handle.multiple : false;
    }
    /**
     * Gets or sets whether multiple items can be selected.
     * @param {boolean} Value True to enable multi-select.
     * @returns {void}
     */
    set MultiSelect(Value) {
        if (this.Handle instanceof HTMLSelectElement)
            this.Handle.multiple = Value === true;
    }
    /**
     * Gets or sets selected indexes.
     * @returns {number[]} Returns selected indexes.
     */
    get SelectedIndexes() {
        var Result = [];
        var Index;
        var Items;
        if (this.Handle instanceof HTMLSelectElement) {
            Items = this.Handle.options;
            for (Index = 0; Index < Items.length; Index++) {
                if (Items[Index].selected === true)
                    Result.push(Index);
            }
        }
        return Result;
    }
    /**
     * Gets or sets selected indexes.
     * @param {number[]} Value The selected indexes.
     * @returns {void}
     */
    set SelectedIndexes(Value) {
        var Index;
        var Item;
        if (this.Handle instanceof HTMLSelectElement) {
            this.SelectAll(false);
            Value = tp.IsArray(Value) ? Value : [];
            for (Index = 0; Index < Value.length; Index++) {
                Item = this.ItemAt(Value[Index]);
                if (Item)
                    Item.selected = true;
            }
        }
    }
    /**
     * Gets or sets selected values.
     * @returns {string[]} Returns selected values.
     */
    get SelectedValues() {
        var Result = [];
        var Index;
        var Items;
        if (this.Handle instanceof HTMLSelectElement) {
            Items = this.Handle.options;
            for (Index = 0; Index < Items.length; Index++) {
                if (Items[Index].selected === true)
                    Result.push(Items[Index].value);
            }
        }
        return Result;
    }
    /**
     * Gets or sets selected values.
     * @param {string[]} Value The selected values.
     * @returns {void}
     */
    set SelectedValues(Value) {
        var Index;
        var ItemIndex;
        var Item;
        if (this.Handle instanceof HTMLSelectElement) {
            this.SelectAll(false);
            Value = tp.IsArray(Value) ? Value : [];
            for (Index = 0; Index < Value.length; Index++) {
                ItemIndex = this.IndexOfValue(Value[Index]);
                Item = this.ItemAt(ItemIndex);
                if (Item)
                    Item.selected = true;
            }
        }
    }
};

tp.Ui.RegisterType(["HtmlComboBox", "tp-HtmlComboBox"], tp.HtmlComboBox);
tp.Ui.RegisterType(["HtmlListBox", "tp-HtmlListBox"], tp.HtmlListBox);
