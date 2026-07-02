// ● selected index contract
/**
 * Interface-like base class for objects that provide a SelectedIndex property and a SelectedIndexChanged event.
 */
tp.ISelectedIndex = class {
    // ● constructor
    /**
     * Creates a selected-index contract instance.
     */
    constructor() {
    }

    // ● properties
    /**
     * Gets or sets the selected index.
     * @returns {number} Returns the selected index.
     */
    get SelectedIndex() {
        return -1;
    }
    /**
     * Gets or sets the selected index.
     * @param {number} Value The selected index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
    }
};
/**
 * Returns true when a value provides the selected-index contract.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value provides SelectedIndex and Tripous events.
 */
tp.IsISelectedIndex = function (Value) {
    return Value instanceof tp.Object && "SelectedIndex" in Value;
};

// ● panel list
/**
 * A list of panels where only one panel is visible at a time.
 * An Associate object with a SelectedIndex property and SelectedIndexChanged event may control the selected panel.
 *
 * Events:
 * - SelectedIndexChanged
 *
 * @implements {tp.ISelectedIndex}
 */
tp.PanelList = class extends tp.Component {
    // ● constructor
    /**
     * Creates a panel list.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The panel list create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.PanelList.CreateParams(CreateParams, Options);
        super(Params);
    }

    // ● protected
    /**
     * Creates normalized panel list create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        if (arguments.length > 1) {
            Params = new tp.CreateParams(Options);
            Params.ElementOrSelector = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        return Params;
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fUpdateAssociate = true;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.PanelList);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.InitializePanels();
    }
    /**
     * Applies explicit create params to this panel list.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Associate))
            this.Associate = Params.Associate;
        if (!tp.IsNil(Params.SelectedIndex))
            this.SelectedIndex = Params.SelectedIndex;
        else if (this.Count > 0)
            this.SelectedIndex = 0;
    }
    /**
     * Initializes child panel elements.
     * @returns {void}
     */
    InitializePanels() {
        var List = this.GetPanelList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            tp.AddClass(List[Index], tp.Classes.PanelListItem);
            List[Index].style.display = "none";
        }
    }
    /**
     * Resolves a value to a selected-index associate.
     * @param {*} Value The value to resolve.
     * @returns {tp.Object|null} Returns the associate or null.
     */
    ResolveAssociate(Value) {
        var Element;
        var Component;
        if (tp.IsISelectedIndex(Value))
            return Value;
        Element = tp(Value);
        Component = tp.GetComponent(Element);
        return tp.IsISelectedIndex(Component) ? Component : null;
    }
    /**
     * Unhooks the current associate listener.
     * @returns {void}
     */
    UnhookAssociate() {
        if (this.fAssociate instanceof tp.Object && this.fSelectedIndexListener instanceof tp.Listener)
            this.fAssociate.Off("SelectedIndexChanged", this.fSelectedIndexListener);
        this.fSelectedIndexListener = null;
    }
    /**
     * Sets the selected index.
     * @param {number} Index The index to set.
     * @returns {void}
     */
    SetSelectedIndex(Index) {
        var List = this.GetPanelList();
        var CurrentIndex = this.SelectedIndex;
        var NewIndex = tp.ToInt(Index);
        var IndexOfPanel;
        if (NewIndex < -1 || NewIndex >= List.length)
            return;
        for (IndexOfPanel = 0; IndexOfPanel < List.length; IndexOfPanel++) {
            tp.RemoveClass(List[IndexOfPanel], tp.Classes.Selected);
            List[IndexOfPanel].style.display = "none";
        }
        if (NewIndex >= 0) {
            tp.AddClass(List[NewIndex], tp.Classes.Selected);
            List[NewIndex].style.display = "";
        }
        if (tp.IsISelectedIndex(this.fAssociate) && this.fUpdateAssociate === true)
            this.fAssociate.SelectedIndex = NewIndex;
        this.OnSelectedIndexChanged(CurrentIndex, NewIndex);
    }
    /**
     * Handles the SelectedIndexChanged event of the associate.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {void}
     */
    Associate_SelectedIndexChanged(Args) {
        if (tp.IsISelectedIndex(this.fAssociate)) {
            this.fUpdateAssociate = false;
            try {
                this.SelectedIndex = this.fAssociate.SelectedIndex;
            } finally {
                this.fUpdateAssociate = true;
            }
        }
    }
    /**
     * Event trigger called after SelectedIndex changes.
     * @param {number} CurrentIndex The previous selected index.
     * @param {number} NewIndex The new selected index.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnSelectedIndexChanged(CurrentIndex, NewIndex) {
        return this.Trigger("SelectedIndexChanged", { CurrentIndex: CurrentIndex, NewIndex: NewIndex });
    }

    // ● public
    /**
     * Adds and returns a panel.
     * @returns {HTMLElement|null} Returns the newly added panel.
     */
    AddPanel() {
        return this.InsertPanel(this.Count);
    }
    /**
     * Inserts a panel at a specified index.
     * @param {number} Index The index where the panel is inserted.
     * @returns {HTMLElement|null} Returns the newly inserted panel.
     */
    InsertPanel(Index) {
        var Element = this.InsertElement(tp.ToInt(Index), "div");
        if (Element instanceof HTMLElement) {
            tp.AddClass(Element, tp.Classes.PanelListItem);
            Element.style.display = "none";
            this.SetSelectedIndex(this.IndexOfElement(Element));
        }
        return Element;
    }
    /**
     * Returns the panel element list.
     * @returns {HTMLElement[]} Returns the panel elements.
     */
    GetPanelList() {
        return this.GetElementList();
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        this.UnhookAssociate();
        this.fAssociate = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Gets or sets the object that controls SelectedIndex.
     * @returns {tp.Object|null} Returns the associate object.
     */
    get Associate() {
        return this.fAssociate;
    }
    /**
     * Gets or sets the object that controls SelectedIndex.
     * @param {*} Value The associate object, component, element, or selector.
     * @returns {void}
     */
    set Associate(Value) {
        var Associate;
        if (Value !== this.fAssociate) {
            this.UnhookAssociate();
            this.fAssociate = null;
            Associate = this.ResolveAssociate(Value);
            if (tp.IsISelectedIndex(Associate)) {
                this.fAssociate = Associate;
                if (Associate instanceof tp.Object)
                    this.fSelectedIndexListener = Associate.On("SelectedIndexChanged", this.Associate_SelectedIndexChanged, this);
                this.SelectedIndex = Associate.SelectedIndex;
            }
        }
    }
    /**
     * Gets or sets the selected index.
     * @returns {number} Returns the selected index.
     */
    get SelectedIndex() {
        var List = this.GetPanelList();
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.HasClass(List[Index], tp.Classes.Selected))
                return Index;
        }
        return -1;
    }
    /**
     * Gets or sets the selected index.
     * @param {number} Value The selected index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
        Value = tp.ToInt(Value);
        if (Value !== this.SelectedIndex)
            this.SetSelectedIndex(Value);
    }
    /**
     * Gets or sets the selected panel.
     * @returns {HTMLElement|null} Returns the selected panel or null.
     */
    get SelectedPanel() {
        return this.GetElementAt(this.SelectedIndex);
    }
    /**
     * Gets or sets the selected panel.
     * @param {HTMLElement|string|null|undefined} Value The selected panel.
     * @returns {void}
     */
    set SelectedPanel(Value) {
        var Element = tp(Value);
        var Index = this.GetPanelList().indexOf(Element);
        if (Index >= 0)
            this.SelectedIndex = Index;
    }
};

// ● prototype
/**
 * Private field.
 * @type {tp.Object|null}
 */
tp.PanelList.prototype.fAssociate = null;
/**
 * Private field.
 * @type {boolean}
 */
tp.PanelList.prototype.fUpdateAssociate = false;
/**
 * Private field.
 * @type {tp.Listener|null}
 */
tp.PanelList.prototype.fSelectedIndexListener = null;
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.PanelList.prototype.tpClass = "tp.PanelList";

tp.Ui.RegisterType(["PanelList", "tp-PanelList"], tp.PanelList);
