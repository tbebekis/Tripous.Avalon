// ● status bar item
/**
 * Descriptor for a tp.StatusBar panel.
 */
tp.StatusBarItem = class extends tp.NamedItem {
    // ● constructor
    /**
     * Creates a status bar item descriptor.
     * @param {object|string|null|undefined} Source The source item or item name.
     */
    constructor(Source) {
        super(tp.IsString(Source) ? Source : "");
        if (tp.IsString(Source)) {
            this.Text = Source;
        } else if (tp.IsObject(Source)) {
            this.Name = tp.IsBlank(Source.Name) ? "" : String(Source.Name);
            this.Text = tp.IsNil(Source.Text) ? "" : String(Source.Text);
            this.Title = tp.IsNil(Source.Title) ? "" : String(Source.Title);
            this.Width = tp.IsNil(Source.Width) ? "" : String(Source.Width);
            this.TextAlign = tp.IsNil(Source.TextAlign) ? "" : String(Source.TextAlign);
            this.CssClass = tp.IsNil(Source.CssClass) ? "" : String(Source.CssClass);
            if (tp.IsBlank(this.CssClass) && !tp.IsNil(Source.CssClasses))
                this.CssClass = String(Source.CssClasses);
            this.Visible = Source.Visible !== false;
            this.Tag = tp.IsNil(Source.Tag) ? null : Source.Tag;
        }
    }

    // ● public
    /**
     * Assigns a source item to this instance.
     * @param {tp.StatusBarItem|object|null|undefined} Source The source item.
     * @returns {void}
     */
    Assign(Source) {
        super.Assign(Source);
        if (Source) {
            this.Text = tp.IsNil(Source.Text) ? "" : String(Source.Text);
            this.Title = tp.IsNil(Source.Title) ? "" : String(Source.Title);
            this.Width = tp.IsNil(Source.Width) ? "" : String(Source.Width);
            this.TextAlign = tp.IsNil(Source.TextAlign) ? "" : String(Source.TextAlign);
            this.CssClass = tp.IsNil(Source.CssClass) ? "" : String(Source.CssClass);
            if (tp.IsBlank(this.CssClass) && !tp.IsNil(Source.CssClasses))
                this.CssClass = String(Source.CssClasses);
            this.Visible = Source.Visible !== false;
            this.Tag = tp.IsNil(Source.Tag) ? null : Source.Tag;
        }
    }
};

// ● prototype
/**
 * Item text.
 * @type {string}
 */
tp.StatusBarItem.prototype.Text = "";
/**
 * Item title.
 * @type {string}
 */
tp.StatusBarItem.prototype.Title = "";
/**
 * Item CSS grid width.
 * @type {string}
 */
tp.StatusBarItem.prototype.Width = "";
/**
 * CSS text-align value.
 * @type {string}
 */
tp.StatusBarItem.prototype.TextAlign = "";
/**
 * Additional CSS class applied to the item element.
 * @type {string}
 */
tp.StatusBarItem.prototype.CssClass = "";
/**
 * True when the item is visible.
 * @type {boolean}
 */
tp.StatusBarItem.prototype.Visible = true;
/**
 * User-defined value.
 * @type {*}
 */
tp.StatusBarItem.prototype.Tag = null;
/**
 * The rendered item element.
 * @type {HTMLElement|null}
 */
tp.StatusBarItem.prototype.Element = null;

// ● status bar
/**
 * Displays a horizontal status bar with named text panels.
 *
 * Create params and data-setup:
 * - Items: Array of strings or item objects.
 * - DefaultItemName: Name used by Text and Message.
 * - Compact: True for compact sizing.
 * - Text: Text for the default item.
 * - Message: Alias for Text.
 *
 * Events:
 * - None.
 */
tp.StatusBar = class extends tp.Component {
    // ● constructor
    /**
     * Creates a status bar.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes class metadata.
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.tpClass = "tp.StatusBar";
        this.fElementType = "div";
    }
    /**
     * Applies handle-only setup.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.StatusBar);
        this.Handle.setAttribute("role", "status");
        this.Handle.setAttribute("aria-live", "polite");
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fItems = new tp.NamedItems(tp.StatusBarItem);
        this.fDefaultItemName = "Message";
        this.fCompact = false;
        this.fItemAutoIndex = 0;
    }
    /**
     * Applies create params and data-setup values.
     * @param {tp.CreateParams|object|null|undefined} Params The create params.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        var BaseDefaultItemName = this.DefaultItemName;
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsBlank(Params.DefaultItemName)) {
            this.DefaultItemName = Params.DefaultItemName;
            if (!tp.IsSameText(BaseDefaultItemName, this.DefaultItemName) && this.ItemCount === 1 && this.Contains(BaseDefaultItemName))
                this.Remove(BaseDefaultItemName);
        }
        if (!tp.IsNil(Params.Compact))
            this.Compact = Params.Compact === true;
        if (tp.IsArray(Params.Items))
            this.AddRange(Params.Items);
        if (!tp.IsNil(Params.Message))
            this.Message = Params.Message;
        else if (!tp.IsNil(Params.Text))
            this.Text = Params.Text;
    }
    /**
     * Creates a status bar item from a supported source.
     * @param {tp.StatusBarItem|object|string|null|undefined} Source The source item.
     * @param {*|null|undefined} Text Optional text used when Source is a name.
     * @param {*|null|undefined} Width Optional width used when Source is a name.
     * @param {*|null|undefined} CssClass Optional CSS class used when Source is a name.
     * @param {*|null|undefined} TextAlign Optional text alignment used when Source is a name.
     * @returns {tp.StatusBarItem} Returns the normalized item.
     */
    NormalizeItem(Source, Text, Width, CssClass, TextAlign) {
        var Item;
        if (Source instanceof tp.StatusBarItem)
            Item = Source;
        else if (tp.IsString(Source))
            Item = new tp.StatusBarItem({ Name: Source, Text: tp.IsNil(Text) ? "" : Text, Width: Width, CssClass: CssClass, TextAlign: TextAlign });
        else
            Item = new tp.StatusBarItem(Source);
        if (tp.IsBlank(Item.Name)) {
            this.fItemAutoIndex++;
            Item.Name = "Item" + this.fItemAutoIndex;
        }
        return Item;
    }
    /**
     * Creates the element for an item.
     * @param {tp.StatusBarItem} Item The item to render.
     * @returns {HTMLElement} Returns the item element.
     */
    RenderItem(Item) {
        var Element = this.Document.createElement("div");
        Element.className = tp.Classes.StatusBarItem;
        Element.setAttribute("data-name", Item.Name);
        Item.Element = Element;
        this.ApplyItemElement(Item);
        return Element;
    }
    /**
     * Applies item state to the item element.
     * @param {tp.StatusBarItem} Item The item.
     * @returns {void}
     */
    ApplyItemElement(Item) {
        var Element = Item ? Item.Element : null;
        if (!(Element instanceof HTMLElement))
            return;
        Element.className = tp.Classes.StatusBarItem;
        Element.textContent = Item.Text;
        Element.title = Item.Title || Item.Text;
        Element.style.textAlign = Item.TextAlign || "";
        if (!tp.IsBlank(Item.CssClass))
            tp.AddClasses(Element, Item.CssClass);
        if (Item.Visible === false)
            tp.AddClass(Element, tp.Classes.StatusBarItemHidden);
    }
    /**
     * Rebuilds the status bar items.
     * @returns {void}
     */
    Render() {
        var Columns = [];
        if (!this.HasHandle)
            return;
        this.Handle.innerHTML = "";
        this.fItems.forEach(function (Item) {
            this.Handle.appendChild(this.RenderItem(Item));
            if (Item.Visible !== false)
                Columns.push(tp.IsBlank(Item.Width) ? "1fr" : Item.Width);
        }, this);
        this.Handle.style.gridTemplateColumns = Columns.length > 0 ? Columns.join(" ") : "";
    }

    // ● public
    /**
     * Adds or updates an item.
     * @param {tp.StatusBarItem|object|string} NameOrItem The item object or item name.
     * @param {*|null|undefined} Text Optional item text when NameOrItem is a name.
     * @param {*|null|undefined} Width Optional item width when NameOrItem is a name.
     * @param {*|null|undefined} CssClass Optional item CSS class when NameOrItem is a name.
     * @param {*|null|undefined} TextAlign Optional item text alignment when NameOrItem is a name.
     * @returns {tp.StatusBarItem} Returns the added or updated item.
     */
    Add(NameOrItem, Text, Width, CssClass, TextAlign) {
        var Item = this.NormalizeItem(NameOrItem, Text, Width, CssClass, TextAlign);
        var Existing = this.Find(Item.Name);
        if (Existing) {
            Existing.Text = Item.Text;
            Existing.Title = Item.Title;
            Existing.Width = Item.Width;
            Existing.TextAlign = Item.TextAlign;
            Existing.CssClass = Item.CssClass;
            Existing.Visible = Item.Visible;
            Existing.Tag = Item.Tag;
            Item = Existing;
        } else {
            this.fItems.Add(Item);
        }
        this.Render();
        return Item;
    }
    /**
     * Adds or updates multiple items.
     * @param {Array<tp.StatusBarItem|object|string>|null|undefined} Items The items to add.
     * @returns {void}
     */
    AddRange(Items) {
        if (!tp.IsArray(Items))
            return;
        Items.forEach(function (Item) {
            this.Add(Item);
        }, this);
    }
    /**
     * Removes all items.
     * @returns {void}
     */
    Clear() {
        this.fItems.forEach(function (Item) {
            Item.Element = null;
        });
        this.fItems.Clear();
        this.Render();
    }
    /**
     * Removes an item.
     * @param {string|number|tp.StatusBarItem} NameOrIndexOrItem The item name, index, or descriptor.
     * @returns {boolean} Returns true when an item was removed.
     */
    Remove(NameOrIndexOrItem) {
        var Index = -1;
        if (tp.IsNumber(NameOrIndexOrItem)) {
            Index = NameOrIndexOrItem;
        } else if (NameOrIndexOrItem instanceof tp.StatusBarItem) {
            Index = this.fItems.IndexOf(NameOrIndexOrItem);
        } else if (tp.IsString(NameOrIndexOrItem)) {
            Index = this.fItems.IndexOf(this.Find(NameOrIndexOrItem));
        }
        if (!tp.InRange(this.fItems, Index))
            return false;
        this.fItems[Index].Element = null;
        this.fItems.RemoveAt(Index);
        this.Render();
        return true;
    }
    /**
     * Finds an item by name.
     * @param {string} Name The item name.
     * @returns {tp.StatusBarItem|null} Returns the item or null.
     */
    Find(Name) {
        var Index;
        for (Index = 0; Index < this.fItems.Count; Index++) {
            if (tp.IsSameText(this.fItems[Index].Name, Name))
                return this.fItems[Index];
        }
        return null;
    }
    /**
     * Returns true when an item exists.
     * @param {string} Name The item name.
     * @returns {boolean} Returns true when found.
     */
    Contains(Name) {
        return this.Find(Name) !== null;
    }
    /**
     * Sets item text.
     * @param {string} Name The item name.
     * @param {*} Text The item text.
     * @returns {tp.StatusBarItem} Returns the item.
     */
    SetText(Name, Text) {
        var Item = this.Find(Name);
        if (!Item)
            Item = this.Add(Name, Text);
        else
            Item.Text = tp.IsNil(Text) ? "" : String(Text);
        this.ApplyItemElement(Item);
        return Item;
    }
    /**
     * Gets item text.
     * @param {string} Name The item name.
     * @returns {string} Returns the item text.
     */
    GetText(Name) {
        var Item = this.Find(Name);
        return Item ? Item.Text : "";
    }
    /**
     * Sets item title.
     * @param {string} Name The item name.
     * @param {*} Title The item title.
     * @returns {tp.StatusBarItem} Returns the item.
     */
    SetTitle(Name, Title) {
        var Item = this.Find(Name);
        if (!Item)
            Item = this.Add(Name, "");
        Item.Title = tp.IsNil(Title) ? "" : String(Title);
        this.ApplyItemElement(Item);
        return Item;
    }
    /**
     * Sets item visibility.
     * @param {string} Name The item name.
     * @param {boolean} Flag True to show the item.
     * @returns {tp.StatusBarItem} Returns the item.
     */
    SetVisible(Name, Flag) {
        var Item = this.Find(Name);
        if (!Item)
            Item = this.Add(Name, "");
        Item.Visible = Flag === true;
        this.Render();
        return Item;
    }
    /**
     * Sets item width.
     * @param {string} Name The item name.
     * @param {*} Width The CSS grid width.
     * @returns {tp.StatusBarItem} Returns the item.
     */
    SetWidth(Name, Width) {
        var Item = this.Find(Name);
        if (!Item)
            Item = this.Add(Name, "");
        Item.Width = tp.IsNil(Width) ? "" : String(Width);
        this.Render();
        return Item;
    }
    /**
     * Sets item text alignment.
     * @param {string} Name The item name.
     * @param {*} TextAlign The CSS text-align value.
     * @returns {tp.StatusBarItem} Returns the item.
     */
    SetTextAlign(Name, TextAlign) {
        var Item = this.Find(Name);
        if (!Item)
            Item = this.Add(Name, "");
        Item.TextAlign = tp.IsNil(TextAlign) ? "" : String(TextAlign);
        this.ApplyItemElement(Item);
        return Item;
    }

    // ● properties
    /**
     * Gets a copy of the item list.
     * @returns {tp.StatusBarItem[]} Returns a copy of the item list.
     */
    get Items() {
        return this.fItems.ToArray();
    }
    /**
     * Gets the number of items.
     * @returns {number} Returns the item count.
     */
    get ItemCount() {
        return this.fItems.Count;
    }
    /**
     * Gets or sets the default item name used by Text and Message.
     * @returns {string} Returns the default item name.
     */
    get DefaultItemName() {
        return this.fDefaultItemName;
    }
    /**
     * Gets or sets the default item name used by Text and Message.
     * @param {*} Value The item name.
     * @returns {void}
     */
    set DefaultItemName(Value) {
        if (!tp.IsBlank(Value))
            this.fDefaultItemName = String(Value);
    }
    /**
     * Gets or sets the default item text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.GetText(this.DefaultItemName);
    }
    /**
     * Gets or sets the default item text.
     * @param {*} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        this.SetText(this.DefaultItemName, Value);
    }
    /**
     * Gets or sets the status message.
     * @returns {string} Returns the message.
     */
    get Message() {
        return this.Text;
    }
    /**
     * Gets or sets the status message.
     * @param {*} Value The message.
     * @returns {void}
     */
    set Message(Value) {
        this.Text = Value;
    }
    /**
     * Gets or sets compact display mode.
     * @returns {boolean} Returns true when compact.
     */
    get Compact() {
        return this.fCompact === true;
    }
    /**
     * Gets or sets compact display mode.
     * @param {boolean} Value True for compact display.
     * @returns {void}
     */
    set Compact(Value) {
        this.fCompact = Value === true;
        if (this.HasHandle) {
            if (this.fCompact)
                tp.AddClass(this.Handle, tp.Classes.StatusBarCompact);
            else
                tp.RemoveClass(this.Handle, tp.Classes.StatusBarCompact);
        }
    }
};
