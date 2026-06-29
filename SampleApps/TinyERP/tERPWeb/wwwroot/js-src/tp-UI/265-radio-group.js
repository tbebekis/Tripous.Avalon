// ● radio group
/**
 * A radio-group control.
 *
 * Example markup:
 * <pre>
 *     <fieldset data-setup="{Name: 'group1', SelectedIndex: 0}">
 *         <legend>Radio Group Title</legend>
 *         <label><input type="radio" name="group1" value="Male" />Male</label>
 *         <label><input type="radio" name="group1" value="Female" />Female</label>
 *     </fieldset>
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
tp.RadioGroup = class extends tp.ListControl {
    // ● private
    /**
     * Creates radio-group create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateRadioGroupParams(CreateParams) {
        var Args = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "fieldset";
        return Args;
    }

    // ● constructor
    /**
     * Creates a radio group.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.RadioGroup.CreateRadioGroupParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "SelectedValue";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fLastName = "";
        this.fChangeHandler = this.FuncBind(this.HandleChange);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.EnsureContent();
        this.ImportMarkupItems();
    }
    /**
     * Applies explicit create params to this radio group.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        var HasParamItems;
        HasParamItems = Params && (!tp.IsNil(Params.List) || !tp.IsNil(Params.ListItems) || !tp.IsNil(Params.Items) || !tp.IsNil(Params.ListSource));
        if (HasParamItems === true)
            this.fItems.Clear();
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Name))
            this.Name = Params.Name;
        if (!tp.IsNil(Params.Text))
            this.Text = Params.Text;
        this.RenderItems();
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.RadioGroup);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fContainer && this.fChangeHandler)
            this.fContainer.removeEventListener("change", this.fChangeHandler, false);
        this.fChangeHandler = null;
        this.fLegend = null;
        this.fContainer = null;
        super.DoDispose();
    }
    /**
     * Ensures the expected fieldset content exists.
     * @protected
     * @returns {void}
     */
    EnsureContent() {
        var Legend = tp.Select(this.Handle, "legend");
        var Container = null;
        var Children = tp.ChildHTMLElements(this.Handle);
        var Index;
        this.fLegend = Legend instanceof HTMLLegendElement ? Legend : this.Document.createElement("legend");
        if (!this.fLegend.parentNode)
            this.Handle.insertBefore(this.fLegend, this.Handle.firstChild);
        for (Index = 0; Index < Children.length; Index++) {
            if (Children[Index] !== this.fLegend && !(Children[Index] instanceof HTMLLabelElement)) {
                Container = Children[Index];
                break;
            }
        }
        this.fContainer = Container || this.Document.createElement("div");
        tp.AddClass(this.fContainer, tp.Classes.List);
        if (!this.fContainer.parentNode)
            this.Handle.appendChild(this.fContainer);
        this.MoveMarkupLabelsToContainer();
        this.fContainer.addEventListener("change", this.fChangeHandler, false);
    }
    /**
     * Moves top-level radio labels into the radio container.
     * @protected
     * @returns {void}
     */
    MoveMarkupLabelsToContainer() {
        var Labels = tp.SelectAll(this.Handle, "label");
        var Index;
        for (Index = 0; Index < Labels.length; Index++) {
            if (Labels[Index].parentNode !== this.fContainer)
                this.fContainer.appendChild(Labels[Index]);
        }
    }
    /**
     * Imports existing markup radio labels into the item list.
     * @protected
     * @returns {void}
     */
    ImportMarkupItems() {
        var Radios = this.GetRadioList();
        var Labels;
        var Label;
        var Index;
        var Item;
        if (this.fItems.length > 0 || Radios.length === 0)
            return;
        Labels = tp.SelectAll(this.fContainer, "label");
        for (Index = 0; Index < Radios.length; Index++) {
            Label = Labels[Index];
            Item = {
                Id: Radios[Index].value,
                Name: Label instanceof HTMLLabelElement ? this.GetLabelText(Label) : Radios[Index].value
            };
            this.fItems.DoInsert(this.fItems.length, Item);
            if (Radios[Index].checked === true) {
                this.fSelectedIndex = Index;
                this.fSelectedValue = Item.Id;
                this.fSelectedItem = Item;
            }
        }
    }
    /**
     * Returns the label text excluding the radio input.
     * @protected
     * @param {HTMLLabelElement} Label The label.
     * @returns {string} Returns the label text.
     */
    GetLabelText(Label) {
        var Result = "";
        var Index;
        for (Index = 0; Index < Label.childNodes.length; Index++) {
            if (Label.childNodes[Index].nodeType === Node.TEXT_NODE)
                Result += Label.childNodes[Index].nodeValue;
        }
        return tp.Trim(Result);
    }
    /**
     * Handles radio input change events.
     * @protected
     * @param {Event} e The event.
     * @returns {void}
     */
    HandleChange(e) {
        var Radio = e.target;
        var Index;
        if (!(Radio instanceof HTMLInputElement) || Radio.type !== "radio" || Radio.checked !== true)
            return;
        Index = this.GetRadioList().indexOf(Radio);
        if (tp.InRange(this.Items, Index)) {
            this.fSelectedIndex = Index;
            this.DoSelectedIndexChanged();
        }
    }
    /**
     * Sets the visual text of the concrete control.
     * @protected
     * @param {string} Text The text.
     * @returns {void}
     */
    DoSetText(Text) {
    }
    /**
     * Called when SelectedIndex changes.
     * @protected
     * @returns {void}
     */
    DoSelectedIndexChanged() {
        var Radio = this.FindItemByIndex(this.SelectedIndex);
        if (Radio instanceof HTMLInputElement)
            Radio.checked = true;
        super.DoSelectedIndexChanged();
    }
    /**
     * Called when SelectedValue changes.
     * @protected
     * @returns {void}
     */
    DoSelectedValueChanged() {
        super.DoSelectedValueChanged();
        this.CheckRadioByIndex(this.SelectedIndex);
    }
    /**
     * Called when SelectedItem changes.
     * @protected
     * @returns {void}
     */
    DoSelectedItemChanged() {
        super.DoSelectedItemChanged();
        this.CheckRadioByIndex(this.SelectedIndex);
    }
    /**
     * Applies the current item list to the DOM.
     * @protected
     * @returns {void}
     */
    SetScrollerList() {
        this.RenderItems();
    }
    /**
     * Updates the DOM after list changes.
     * @protected
     * @returns {void}
     */
    UpdateScroller() {
        this.RenderItems();
    }
    /**
     * Renders radio labels from the item list.
     * @protected
     * @returns {void}
     */
    RenderItems() {
        var SelectedValue = this.SelectedValue;
        var Index;
        var Item;
        if (!this.fContainer)
            return;
        this.fContainer.innerHTML = "";
        for (Index = 0; Index < this.Items.length; Index++) {
            Item = this.Items[Index];
            this.fContainer.appendChild(this.CreateRadioLabel(Item, Index));
        }
        if (!tp.IsNil(SelectedValue))
            this.SelectedValue = SelectedValue;
        else if (this.SelectedIndex >= 0)
            this.CheckRadioByIndex(this.SelectedIndex);
    }
    /**
     * Creates a radio label for an item.
     * @protected
     * @param {*} Item The item.
     * @param {number} Index The item index.
     * @returns {HTMLLabelElement} Returns the label.
     */
    CreateRadioLabel(Item, Index) {
        var Label = this.Document.createElement("label");
        var Radio = this.Document.createElement("input");
        var Text = this.Document.createElement("span");
        Radio.type = "radio";
        Radio.name = this.Name;
        Radio.value = this.ItemValueToString(this.GetItemValue(Item));
        Radio.checked = Index === this.SelectedIndex;
        Text.className = tp.Classes.Text;
        Text.textContent = this.GetItemText(Item);
        Label.appendChild(Radio);
        Label.appendChild(Text);
        return Label;
    }
    /**
     * Converts an item value to radio value text.
     * @protected
     * @param {*} Value The item value.
     * @returns {string} Returns value text.
     */
    ItemValueToString(Value) {
        return tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Checks a radio by item index.
     * @protected
     * @param {number} Index The item index.
     * @returns {void}
     */
    CheckRadioByIndex(Index) {
        var Radio = this.FindItemByIndex(Index);
        if (Radio instanceof HTMLInputElement)
            Radio.checked = true;
    }

    // ● public
    /**
     * Returns the list of child radio buttons.
     * @returns {HTMLInputElement[]} Returns radio buttons.
     */
    GetRadioList() {
        var Result = [];
        var List = this.fContainer ? tp.SelectAll(this.fContainer, "input[type=radio]") : [];
        var Index;
        for (Index = 0; Index < List.length; Index++)
            Result.push(List[Index]);
        return Result;
    }
    /**
     * Finds and returns the checked radio button.
     * @returns {HTMLInputElement|null} Returns the checked radio button or null.
     */
    FindCheckedItem() {
        var List = this.GetRadioList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (List[Index].checked === true)
                return List[Index];
        }
        return null;
    }
    /**
     * Returns the value of the checked radio button.
     * @returns {string|null} Returns the checked value or null.
     */
    FindCheckedValue() {
        var Radio = this.FindCheckedItem();
        return Radio instanceof HTMLInputElement ? Radio.value : null;
    }
    /**
     * Finds and returns a radio button by value.
     * @param {*} Value The value.
     * @returns {HTMLInputElement|null} Returns the radio button or null.
     */
    FindItemByValue(Value) {
        var Text = this.ItemValueToString(Value);
        var List = this.GetRadioList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (List[Index].value === Text)
                return List[Index];
        }
        return null;
    }
    /**
     * Finds and returns a radio button by index.
     * @param {number} Index The index.
     * @returns {HTMLInputElement|null} Returns the radio button or null.
     */
    FindItemByIndex(Index) {
        var List = this.GetRadioList();
        return tp.InRange(List, Index) ? List[Index] : null;
    }
    /**
     * Checks a radio button by value.
     * @param {*} Value The value.
     * @returns {void}
     */
    CheckItemByValue(Value) {
        this.SelectedValue = Value;
    }
    /**
     * Checks a radio button by index.
     * @param {number} Index The index.
     * @returns {void}
     */
    CheckItemByIndex(Index) {
        this.SelectedIndex = Index;
    }
    /**
     * Removes all radio buttons from this group.
     * @returns {void}
     */
    Clear() {
        this.fLastName = !tp.IsBlank(this.Name) ? this.Name : this.fLastName;
        this.fSelectedIndex = -1;
        this.fSelectedValue = null;
        this.fSelectedItem = null;
        this.fItems.Clear();
        this.RenderItems();
    }
    /**
     * Adds a radio button to this group.
     * @param {*} Value The value.
     * @param {string} Text The display text.
     * @returns {HTMLInputElement|null} Returns the radio input.
     */
    AddItem(Value, Text) {
        return this.InsertItem(this.Count, Value, Text);
    }
    /**
     * Inserts a radio button to this group.
     * @param {number} Index The index.
     * @param {*} Value The value.
     * @param {string} Text The display text.
     * @returns {HTMLInputElement|null} Returns the radio input.
     */
    InsertItem(Index, Value, Text) {
        var Item = { Id: Value, Name: Text };
        this.fItems.Insert(Index, Item);
        return this.FindItemByIndex(Index);
    }
    /**
     * Returns the title text of a radio button by index.
     * @param {number} Index The index.
     * @returns {string} Returns the title text.
     */
    GetTitleAt(Index) {
        var Item = this.Items[Index];
        return !tp.IsNil(Item) ? this.GetItemText(Item) : "";
    }
    /**
     * Sets the title text of a radio button by index.
     * @param {number} Index The index.
     * @param {string} Text The text.
     * @returns {void}
     */
    SetTitleAt(Index, Text) {
        var Item = this.Items[Index];
        if (tp.IsNil(Item))
            return;
        if (tp.IsPrimitive(Item))
            this.fItems[Index] = Text;
        else
            Item[this.GetListDisplayField() || "Name"] = Text;
        this.RenderItems();
    }
    /**
     * Clears all radio buttons and loads from an enum-like object.
     * @param {object} EnumType The enum-like object.
     * @returns {void}
     */
    LoadFromEnumType(EnumType) {
        var List = [];
        var Name;
        var Value;
        if (!tp.IsObject(EnumType))
            return;
        for (Name in EnumType) {
            if (!tp.IsFunction(EnumType[Name])) {
                Value = EnumType[Name];
                if (tp.IsNumber(Value) || (tp.IsString(Value) && tp.TryStrToInt(Value).Result === true)) {
                    List.push({
                        Id: tp.ToInt(Value),
                        Name: Name
                    });
                }
            }
        }
        this.LoadFrom(List);
    }
    /**
     * Clears all radio buttons and loads from an object array.
     * @param {object[]} List A list of objects with Id and Name properties.
     * @returns {void}
     */
    LoadFrom(List) {
        this.Clear();
        if (tp.IsArray(List))
            this.fItems.AddRange(List);
        if (this.Items.length > 0)
            this.SelectedIndex = 0;
    }

    // ● properties
    /**
     * Gets or sets the title of the group.
     * @returns {string} Returns the title.
     */
    get Text() {
        return this.fLegend instanceof HTMLLegendElement ? this.fLegend.textContent || "" : "";
    }
    /**
     * Gets or sets the title of the group.
     * @param {string} Value The title.
     * @returns {void}
     */
    set Text(Value) {
        if (this.fLegend instanceof HTMLLegendElement)
            this.fLegend.textContent = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the group name used by all radio buttons.
     * @returns {string} Returns the group name.
     */
    get Name() {
        var List = this.GetRadioList();
        if (List.length > 0 && !tp.IsBlank(List[0].name))
            return List[0].name;
        return !tp.IsBlank(this.fLastName) ? this.fLastName : this.Id || "group";
    }
    /**
     * Gets or sets the group name used by all radio buttons.
     * @param {string} Value The group name.
     * @returns {void}
     */
    set Name(Value) {
        var List = this.GetRadioList();
        var Index;
        this.fLastName = tp.IsBlank(Value) ? this.fLastName : String(Value);
        for (Index = 0; Index < List.length; Index++)
            List[Index].name = this.fLastName;
    }
};

tp.Ui.RegisterType(["RadioGroup", "tp-RadioGroup"], tp.RadioGroup);
