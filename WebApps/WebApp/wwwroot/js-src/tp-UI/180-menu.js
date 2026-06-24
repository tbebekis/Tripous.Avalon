// ● menu item type
/**
 * Menu item type values.
 * @enum {number}
 */
tp.MenuItemType = {
    Item: 1,
    Separator: 2
};
Object.freeze(tp.MenuItemType);

// ● menu event args
/**
 * Event arguments for menu item events.
 */
tp.MenuEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates menu event arguments.
     * @param {tp.MenuItem} MenuItem The menu item.
     * @param {MouseEvent|null|undefined} e The DOM event.
     */
    constructor(MenuItem, e) {
        super(tp.Events.Click, null, e);
        this.MenuItem = MenuItem;
        this.Command = MenuItem instanceof tp.MenuItem ? MenuItem.Command : "";
    }
};

// ● prototype
/**
 * The clicked menu item.
 * @type {tp.MenuItem|null}
 */
tp.MenuEventArgs.prototype.MenuItem = null;

// ● menu item list
/**
 * Internal menu item list.
 */
tp.MenuItemList = class {
    // ● constructor
    /**
     * Creates a menu item list.
     * @param {HTMLElement} Handle The item container element.
     * @param {tp.MenuBase|tp.MenuItem} Owner The owner.
     */
    constructor(Handle, Owner) {
        this.Handle = Handle;
        this.Owner = Owner;
        this.Items = [];
    }

    // ● public
    /**
     * Returns the item index.
     * @param {tp.MenuItemBase} Item The item.
     * @returns {number} Returns the item index or -1.
     */
    IndexOf(Item) {
        return this.Items.indexOf(Item);
    }
    /**
     * Returns an item by index.
     * @param {number} Index The index.
     * @returns {tp.MenuItemBase|null} Returns the item or null.
     */
    ByIndex(Index) {
        return Index >= 0 && Index < this.Items.length ? this.Items[Index] : null;
    }
    /**
     * Returns an item by command.
     * @param {string} Command The command.
     * @returns {tp.MenuItem|null} Returns the menu item or null.
     */
    ByCommand(Command) {
        var Index;
        var Result;
        for (Index = 0; Index < this.Items.length; Index++) {
            Result = this.Items[Index].ByCommand(Command);
            if (Result)
                return Result;
        }
        return null;
    }
    /**
     * Returns true when the list contains an item.
     * @param {tp.MenuItemBase} Item The item.
     * @returns {boolean} Returns true when contained.
     */
    Contains(Item) {
        return this.Items.indexOf(Item) !== -1;
    }
    /**
     * Updates owner CSS state after list changes.
     * @returns {void}
     */
    UpdateOwnerState() {
        if (this.Owner instanceof tp.MenuItem) {
            if (this.Count > 0)
                tp.AddClass(this.Owner.Handle, tp.Classes.HasChildren);
            else
                tp.RemoveClass(this.Owner.Handle, tp.Classes.HasChildren);
        }
    }
    /**
     * Adds an item.
     * @param {tp.MenuItemBase} Item The item.
     * @returns {void}
     */
    Add(Item) {
        this.Insert(this.Items.length, Item);
    }
    /**
     * Inserts an item.
     * @param {number} Index The index.
     * @param {tp.MenuItemBase} Item The item.
     * @returns {void}
     */
    Insert(Index, Item) {
        var RefNode;
        if (!(Item instanceof tp.MenuItemBase) || this.Contains(Item))
            return;
        Index = Math.max(0, Math.min(tp.ToInt(Index), this.Items.length));
        Item.Parent = this.Owner;
        if (Index === this.Items.length) {
            this.Items.push(Item);
            this.Handle.appendChild(Item.Handle);
        } else {
            this.Items.splice(Index, 0, Item);
            RefNode = this.Handle.children[Index];
            this.Handle.insertBefore(Item.Handle, RefNode);
        }
        this.UpdateOwnerState();
    }
    /**
     * Removes an item.
     * @param {tp.MenuItemBase} Item The item.
     * @returns {void}
     */
    Remove(Item) {
        if (!this.Contains(Item))
            return;
        tp.ListRemove(this.Items, Item);
        if (Item.Handle && Item.Handle.parentNode)
            Item.Handle.parentNode.removeChild(Item.Handle);
        Item.Parent = null;
        this.UpdateOwnerState();
    }
    /**
     * Removes all items.
     * @returns {void}
     */
    Clear() {
        while (this.Items.length > 0)
            this.Remove(this.Items[0]);
    }
    /**
     * Adds a menu item.
     * @param {string} Text The text.
     * @param {string|null|undefined} Command The command.
     * @returns {tp.MenuItem} Returns the menu item.
     */
    AddMenuItem(Text, Command) {
        var Result = tp.MenuItemBase.CreateMenuItem(Text);
        Result.Command = Command || "";
        this.Add(Result);
        return Result;
    }
    /**
     * Adds a separator.
     * @returns {tp.MenuSeparator} Returns the separator.
     */
    AddSeparator() {
        var Result = tp.MenuItemBase.CreateSeparator();
        this.Add(Result);
        return Result;
    }
    /**
     * Inserts a menu item.
     * @param {number} Index The index.
     * @param {string} Text The text.
     * @param {string|null|undefined} Command The command.
     * @returns {tp.MenuItem} Returns the menu item.
     */
    InsertMenuItem(Index, Text, Command) {
        var Result = tp.MenuItemBase.CreateMenuItem(Text);
        Result.Command = Command || "";
        this.Insert(Index, Result);
        return Result;
    }
    /**
     * Inserts a separator.
     * @param {number} Index The index.
     * @returns {tp.MenuSeparator} Returns the separator.
     */
    InsertSeparator(Index) {
        var Result = tp.MenuItemBase.CreateSeparator();
        this.Insert(Index, Result);
        return Result;
    }

    // ● properties
    /**
     * Returns the item count.
     * @returns {number} Returns the item count.
     */
    get Count() {
        return this.Items.length;
    }
};

// ● prototype
/**
 * Container element.
 * @type {HTMLElement|null}
 */
tp.MenuItemList.prototype.Handle = null;
/**
 * Owner menu or item.
 * @type {tp.MenuBase|tp.MenuItem|null}
 */
tp.MenuItemList.prototype.Owner = null;
/**
 * Items.
 * @type {tp.MenuItemBase[]}
 */
tp.MenuItemList.prototype.Items = null;

// ● menu item base
/**
 * Base class for menu items and separators.
 */
tp.MenuItemBase = class extends tp.Object {
    // ● constructor
    /**
     * Creates a menu item base instance.
     * @param {number} Type The tp.MenuItemType value.
     * @param {HTMLElement|null|undefined} Handle The handle.
     */
    constructor(Type, Handle) {
        super();
        this.Type = Type;
        this.Handle = Handle instanceof HTMLElement ? Handle : document.createElement("div");
        this.NormalizeHandle();
    }

    // ● protected
    /**
     * Normalizes the item handle.
     * @returns {void}
     */
    NormalizeHandle() {
        var TextNode = tp.FindTextNode(this.Handle);
        var Text = TextNode ? TextNode.nodeValue || "" : "";
        var Children = tp.ToArray(this.Handle.children);
        var Index;
        if (TextNode)
            TextNode.nodeValue = "";
        Children.forEach(function (Child) {
            Child.parentNode.removeChild(Child);
        });
        this.Handle.__tpMenuItem = this;
        if (this.IsSeparator) {
            this.Handle.className = tp.Classes.MenuSeparator;
            this.fSeparatorElement = document.createElement("hr");
            this.Handle.appendChild(this.fSeparatorElement);
        } else {
            this.Handle.className = tp.Classes.MenuItem;
            this.CreateElements(Text.trim());
            this.ReadMarkupParams();
            for (Index = 0; Index < Children.length; Index++)
                this.Items.Add(tp.MenuItemBase.FromElement(Children[Index]));
        }
    }
    /**
     * Creates menu item child elements.
     * @param {string} Text The item text.
     * @returns {void}
     */
    CreateElements(Text) {
        this.fImageElement = document.createElement("div");
        this.fImageElement.className = tp.Classes.MenuItemImage;
        this.fTextElement = document.createElement("a");
        this.fTextElement.className = tp.Classes.MenuItemText;
        this.fTextElement.href = "javascript:void(0);";
        this.fTextElement.textContent = Text;
        this.fArrowElement = document.createElement("div");
        this.fArrowElement.className = tp.Classes.MenuItemArrow;
        this.fArrowElement.textContent = "›";
        this.fListElement = document.createElement("div");
        this.fListElement.className = tp.Classes.MenuItemList;
        this.fListElement.style.display = "none";
        this.Handle.appendChild(this.fImageElement);
        this.Handle.appendChild(this.fTextElement);
        this.Handle.appendChild(this.fArrowElement);
        this.Handle.appendChild(this.fListElement);
        this.Items = new tp.MenuItemList(this.fListElement, this);
    }
    /**
     * Reads menu item settings from data-* attributes.
     * @returns {void}
     */
    ReadMarkupParams() {
        var Value;
        this.Command = tp.Data(this.Handle, "command") || "";
        Value = tp.Data(this.Handle, "url");
        if (!tp.IsBlank(Value))
            this.Url = Value;
        Value = tp.Data(this.Handle, "ico-classes");
        if (!tp.IsBlank(Value))
            this.IcoClasses = Value;
        Value = tp.Data(this.Handle, "image-url");
        if (!tp.IsBlank(Value))
            this.ImageUrl = Value;
        Value = tp.Data(this.Handle, "enabled");
        if (!tp.IsBlank(Value))
            this.Enabled = !(tp.IsSameText(Value, "false") || tp.IsSameText(Value, "0"));
    }
    /**
     * Returns a menu item from an element.
     * @param {HTMLElement} Element The element.
     * @returns {tp.MenuItemBase} Returns a menu item or separator.
     */
    static FromElement(Element) {
        return tp.MenuItemBase.IsSeparator(Element) ? new tp.MenuSeparator(Element) : new tp.MenuItem(Element);
    }
    /**
     * Returns true when an element is a separator marker.
     * @param {HTMLElement} Element The element.
     * @returns {boolean} Returns true when the element is a separator marker.
     */
    static IsSeparator(Element) {
        var TextNode = tp.FindTextNode(Element);
        return !!TextNode && (TextNode.nodeValue || "").trim() === "-";
    }
    /**
     * Ensures item text.
     * @param {string|null|undefined} Text The source text.
     * @returns {string} Returns non-empty text.
     */
    static EnsureMenuItemText(Text) {
        if (tp.IsBlank(Text)) {
            Text = "MenuItem " + tp.MenuItemBase.MenuItemCounter;
            tp.MenuItemBase.MenuItemCounter++;
        }
        return Text;
    }
    /**
     * Creates a menu item.
     * @param {string} Text The item text.
     * @returns {tp.MenuItem} Returns the menu item.
     */
    static CreateMenuItem(Text) {
        var Element = document.createElement("div");
        Element.textContent = tp.MenuItemBase.EnsureMenuItemText(Text);
        return new tp.MenuItem(Element);
    }
    /**
     * Creates a separator.
     * @returns {tp.MenuSeparator} Returns the separator.
     */
    static CreateSeparator() {
        var Element = document.createElement("div");
        Element.textContent = "-";
        return new tp.MenuSeparator(Element);
    }

    // ● public
    /**
     * Returns an item by command.
     * @param {string} Command The command.
     * @returns {tp.MenuItem|null} Returns the item or null.
     */
    ByCommand(Command) {
        if (this.IsMenuItem && tp.IsSameText(this.Command, Command))
            return this;
        return this.Items ? this.Items.ByCommand(Command) : null;
    }
    /**
     * Adds a menu item.
     * @param {string} Text The text.
     * @param {string|null|undefined} Command The command.
     * @returns {tp.MenuItem} Returns the added item.
     */
    AddMenuItem(Text, Command) {
        return this.Items.AddMenuItem(Text, Command);
    }
    /**
     * Adds a separator.
     * @returns {tp.MenuSeparator} Returns the separator.
     */
    AddSeparator() {
        return this.Items.AddSeparator();
    }
    /**
     * Returns this item as string.
     * @returns {string} Returns the item text.
     */
    toString() {
        return this.Text;
    }

    // ● properties
    /**
     * Gets or sets item text.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.fTextElement instanceof HTMLElement ? this.fTextElement.textContent : "-";
    }
    /**
     * Gets or sets item text.
     * @param {string} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        if (this.fTextElement instanceof HTMLElement)
            this.fTextElement.textContent = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets item visibility.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.Handle.style.display !== "none";
    }
    /**
     * Gets or sets item visibility.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        this.Handle.style.display = Value === true ? "" : "none";
    }
    /**
     * Gets or sets enabled state.
     * @returns {boolean} Returns true when enabled.
     */
    get Enabled() {
        return !this.Handle.classList.contains(tp.Classes.Disabled);
    }
    /**
     * Gets or sets enabled state.
     * @param {boolean} Value True to enable.
     * @returns {void}
     */
    set Enabled(Value) {
        if (Value === true)
            tp.RemoveClass(this.Handle, tp.Classes.Disabled);
        else
            tp.AddClass(this.Handle, tp.Classes.Disabled);
    }
    /**
     * Returns true for menu items.
     * @returns {boolean} Returns true for menu items.
     */
    get IsMenuItem() {
        return !this.IsSeparator;
    }
    /**
     * Returns true for separators.
     * @returns {boolean} Returns true for separators.
     */
    get IsSeparator() {
        return this.Type === tp.MenuItemType.Separator;
    }
    /**
     * Returns true when item has children.
     * @returns {boolean} Returns true when item has children.
     */
    get HasChildren() {
        return this.Items && this.Items.Count > 0;
    }
    /**
     * Returns the child count.
     * @returns {number} Returns the child count.
     */
    get Count() {
        return this.Items ? this.Items.Count : 0;
    }
    /**
     * Gets or sets submenu visibility.
     * @returns {boolean} Returns true when visible.
     */
    get IsListVisible() {
        return this.fListElement instanceof HTMLElement && this.fListElement.style.display !== "none";
    }
    /**
     * Gets or sets submenu visibility.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set IsListVisible(Value) {
        if (this.fListElement instanceof HTMLElement)
            this.fListElement.style.display = Value === true ? "" : "none";
    }
};

// ● prototype
tp.MenuItemBase.prototype.Type = tp.MenuItemType.Item;
tp.MenuItemBase.prototype.Handle = null;
tp.MenuItemBase.prototype.Parent = null;
tp.MenuItemBase.prototype.Items = null;
tp.MenuItemBase.prototype.Command = "";
tp.MenuItemBase.prototype.Tag = null;
tp.MenuItemBase.prototype.fImageElement = null;
tp.MenuItemBase.prototype.fTextElement = null;
tp.MenuItemBase.prototype.fArrowElement = null;
tp.MenuItemBase.prototype.fListElement = null;
tp.MenuItemBase.prototype.fSeparatorElement = null;
tp.MenuItemBase.prototype.fIcoClasses = "";
tp.MenuItemBase.prototype.fImageUrl = "";
tp.MenuItemBase.MenuItemCounter = 0;

// ● menu item
/**
 * Represents a menu item.
 *
 * @implements {tp.ICommandProperty}
 */
tp.MenuItem = class extends tp.MenuItemBase {
    // ● constructor
    /**
     * Creates a menu item.
     * @param {HTMLElement|null|undefined} Handle The item handle.
     */
    constructor(Handle) {
        super(tp.MenuItemType.Item, Handle);
    }

    // ● properties
    /**
     * Gets or sets the item URL.
     * @returns {string} Returns the URL.
     */
    get Url() {
        if (this.fTextElement instanceof HTMLAnchorElement)
            return this.fTextElement.href !== "javascript:void(0);" ? this.fTextElement.href : "";
        return "";
    }
    /**
     * Gets or sets the item URL.
     * @param {string} Value The URL.
     * @returns {void}
     */
    set Url(Value) {
        if (this.fTextElement instanceof HTMLAnchorElement)
            this.fTextElement.href = tp.IsBlank(Value) ? "javascript:void(0);" : String(Value);
    }
    /**
     * Gets or sets icon CSS classes.
     * @returns {string} Returns icon CSS classes.
     */
    get IcoClasses() {
        return this.fIcoClasses;
    }
    /**
     * Gets or sets icon CSS classes.
     * @param {string} Value The icon CSS classes.
     * @returns {void}
     */
    set IcoClasses(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            this.fImageElement.style.background = "";
            this.fImageUrl = "";
            tp.AddClasses(this.fImageElement, Value);
        }
        this.fIcoClasses = Value;
    }
    /**
     * Gets or sets icon image URL.
     * @returns {string} Returns the image URL.
     */
    get ImageUrl() {
        return this.fImageUrl;
    }
    /**
     * Gets or sets icon image URL.
     * @param {string} Value The image URL.
     * @returns {void}
     */
    set ImageUrl(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            this.fImageElement.style.background = "";
            this.fIcoClasses = "";
            this.fImageElement.style.backgroundImage = tp.IsBlank(Value) ? "" : "url(\"" + Value + "\")";
            this.fImageElement.style.backgroundRepeat = "no-repeat";
            this.fImageElement.style.backgroundPosition = "center center";
            this.fImageElement.style.backgroundSize = "75%";
        }
        this.fImageUrl = Value;
    }
};

// ● menu separator
/**
 * Represents a menu separator.
 */
tp.MenuSeparator = class extends tp.MenuItemBase {
    // ● constructor
    /**
     * Creates a menu separator.
     * @param {HTMLElement|null|undefined} Handle The separator handle.
     */
    constructor(Handle) {
        super(tp.MenuItemType.Separator, Handle);
    }
};

// ● menu base
/**
 * Base class for main menus and context menus.
 *
 * Events:
 * - ItemClick
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
tp.MenuBase = class extends tp.Component {
    // ● constructor
    /**
     * Creates a menu base instance.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings.
     */
    constructor(CreateParams, Options) {
        var Params = tp.MenuBase.CreateParams(CreateParams, Options);
        super(Params);
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.fMouseOverHandler = this.FuncBind(this.HandleMouseOver);
        this.fDocumentClickHandler = this.FuncBind(this.HandleDocumentClick);
        this.Handle.addEventListener("click", this.fClickHandler);
        this.Handle.addEventListener("mouseover", this.fMouseOverHandler);
    }

    // ● protected
    /**
     * Creates normalized menu create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        var Element;
        if (arguments.length > 1) {
            Params = new tp.CreateParams(Options);
            Params.Handle = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        Element = tp(Params.Handle);
        if (!(Element instanceof HTMLElement))
            Params.Handle = document.createElement("div");
        return Params;
    }
    /**
     * Creates menu items from markup.
     * @returns {void}
     */
    OnHandleCreated() {
        var Children;
        var Index;
        super.OnHandleCreated();
        this.Handle.tabIndex = 0;
        Children = tp.ToArray(this.Handle.children);
        Children.forEach(function (Child) {
            Child.parentNode.removeChild(Child);
        });
        this.Items = new tp.MenuItemList(this.Handle, this);
        for (Index = 0; Index < Children.length; Index++)
            this.Items.Add(tp.MenuItemBase.FromElement(Children[Index]));
    }
    /**
     * Returns the menu item for an element.
     * @param {HTMLElement|null|undefined} Element The element.
     * @returns {tp.MenuItem|null} Returns the menu item or null.
     */
    FindMenuItemByElement(Element) {
        while (Element instanceof HTMLElement && Element !== this.Handle) {
            if (Element.__tpMenuItem instanceof tp.MenuItem)
                return Element.__tpMenuItem;
            Element = Element.parentElement;
        }
        return null;
    }
    /**
     * Hides all submenu lists.
     * @returns {void}
     */
    HideAllLists() {
        this.GetAllItems().forEach(function (Item) {
            if (Item instanceof tp.MenuItem)
                Item.IsListVisible = false;
        });
    }
    /**
     * Returns all menu items recursively.
     * @returns {tp.MenuItemBase[]} Returns the items.
     */
    GetAllItems() {
        var Result = [];
        var AddList = function (List) {
            if (!List)
                return;
            List.Items.forEach(function (Item) {
                Result.push(Item);
                if (Item.Items)
                    AddList(Item.Items);
            });
        };
        AddList(this.Items);
        return Result;
    }
    /**
     * Opens the submenu path for an item.
     * @param {tp.MenuItem} Item The item.
     * @returns {void}
     */
    OpenItemPath(Item) {
        var Parent = Item;
        while (Parent instanceof tp.MenuItem) {
            if (Parent.HasChildren)
                Parent.IsListVisible = true;
            Parent = Parent.Parent;
        }
    }
    /**
     * Positions a submenu list.
     * @param {tp.MenuItem} Item The item.
     * @returns {void}
     */
    PositionSubMenu(Item) {
        var IsTop = Item.Parent === this;
        if (!(Item instanceof tp.MenuItem) || !Item.HasChildren)
            return;
        if (IsTop && this.IsMenu) {
            Item.fListElement.style.left = "0";
            Item.fListElement.style.top = "calc(100% - 1px)";
        } else {
            Item.fListElement.style.left = "calc(100% - 2px)";
            Item.fListElement.style.top = "0";
        }
        tp.BringToFront(Item.fListElement);
    }
    /**
     * Handles item activation.
     * @param {tp.MenuItem} Item The item.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    ActivateItem(Item, e) {
        if (!(Item instanceof tp.MenuItem) || !Item.Enabled)
            return;
        if (Item.HasChildren) {
            this.HideSiblingLists(Item);
            if (Item.IsListVisible) {
                this.HideItemList(Item);
            } else {
                Item.IsListVisible = true;
                this.PositionSubMenu(Item);
                this.OpenItemPath(Item);
            }
        } else {
            if (tp.IsBlank(Item.Url))
                e.preventDefault();
            this.OnItemClick(e, Item);
            this.HideAfterItemClick();
        }
        e.stopPropagation();
    }
    /**
     * Hides menu UI after an item click.
     * @returns {void}
     */
    HideAfterItemClick() {
        this.HideAllLists();
    }
    /**
     * Handles DOM click events.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleClick(e) {
        var Item = this.FindMenuItemByElement(e.target);
        if (Item) {
            this.Handle.focus();
            this.ActivateItem(Item, e);
            this.HookDocumentClick();
        }
    }
    /**
     * Handles document clicks.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleDocumentClick(e) {
        if (!tp.ContainsEventTarget(this.Handle, e.target)) {
            this.HideAllLists();
            this.UnhookDocumentClick();
        }
    }
    /**
     * Handles mouse-over events.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleMouseOver(e) {
        var Item = this.FindMenuItemByElement(e.target);
        if (!(Item instanceof tp.MenuItem) || !Item.Enabled || !this.HasFocused)
            return;
        this.HideSiblingLists(Item);
        this.OpenItemPath(Item);
        if (Item.HasChildren) {
            Item.IsListVisible = true;
            this.PositionSubMenu(Item);
        }
    }
    /**
     * Hides an item submenu and all nested submenu lists.
     * @param {tp.MenuItem} Item The menu item.
     * @returns {void}
     */
    HideItemList(Item) {
        if (!(Item instanceof tp.MenuItem))
            return;
        Item.IsListVisible = false;
        if (Item.Items) {
            Item.Items.Items.forEach(function (Child) {
                this.HideItemList(Child);
            }, this);
        }
    }
    /**
     * Hides sibling submenu lists.
     * @param {tp.MenuItem} Item The active item.
     * @returns {void}
     */
    HideSiblingLists(Item) {
        var Parent = Item.Parent;
        if (!Parent || !Parent.Items)
            return;
        Parent.Items.Items.forEach(function (Sibling) {
            if (Sibling !== Item && Sibling instanceof tp.MenuItem)
                this.HideItemList(Sibling);
        }, this);
    }
    /**
     * Hooks the document click event.
     * @returns {void}
     */
    HookDocumentClick() {
        this.Document.addEventListener("click", this.fDocumentClickHandler);
    }
    /**
     * Unhooks the document click event.
     * @returns {void}
     */
    UnhookDocumentClick() {
        this.Document.removeEventListener("click", this.fDocumentClickHandler);
    }

    // ● public
    /**
     * Adds a menu item.
     * @param {string} Text The text.
     * @param {string|null|undefined} Command The command.
     * @returns {tp.MenuItem} Returns the added item.
     */
    AddMenuItem(Text, Command) {
        return this.Items.AddMenuItem(Text, Command);
    }
    /**
     * Adds a separator.
     * @returns {tp.MenuSeparator} Returns the separator.
     */
    AddSeparator() {
        return this.Items.AddSeparator();
    }
    /**
     * Returns an item by command.
     * @param {string} Command The command.
     * @returns {tp.MenuItem|null} Returns the item or null.
     */
    ByCommand(Command) {
        return this.Items.ByCommand(Command);
    }
    /**
     * Triggers the ItemClick event.
     * @param {MouseEvent} e The DOM event.
     * @param {tp.MenuItem} Item The item.
     * @returns {void}
     */
    OnItemClick(e, Item) {
        this.Trigger("ItemClick", new tp.MenuEventArgs(Item, e));
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.IsDisposed)
            return;
        if (this.HasHandle) {
            this.Handle.removeEventListener("click", this.fClickHandler);
            this.Handle.removeEventListener("mouseover", this.fMouseOverHandler);
        }
        this.UnhookDocumentClick();
        this.fClickHandler = null;
        this.fMouseOverHandler = null;
        this.fDocumentClickHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Returns true for main menus.
     * @returns {boolean} Returns true for main menus.
     */
    get IsMenu() {
        return false;
    }
    /**
     * Returns true for context menus.
     * @returns {boolean} Returns true for context menus.
     */
    get IsContextMenu() {
        return false;
    }
    /**
     * Returns the item count.
     * @returns {number} Returns the item count.
     */
    get Count() {
        return this.Items ? this.Items.Count : 0;
    }
};

// ● prototype
tp.MenuBase.prototype.Items = null;
tp.MenuBase.prototype.fClickHandler = null;
tp.MenuBase.prototype.fMouseOverHandler = null;
tp.MenuBase.prototype.fDocumentClickHandler = null;

// ● main menu
/**
 * Represents a desktop-like main menu.
 */
tp.Menu = class extends tp.MenuBase {
    // ● constructor
    /**
     * Creates a menu.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings.
     */
    constructor(CreateParams, Options) {
        super(CreateParams, Options);
        this.tpClass = "tp.Menu";
        tp.AddClass(this.Handle, tp.Classes.Menu);
    }

    // ● properties
    /**
     * Returns true for main menus.
     * @returns {boolean} Returns true for main menus.
     */
    get IsMenu() {
        return true;
    }
};

// ● context menu
/**
 * Represents a desktop-like context menu.
 */
tp.ContextMenu = class extends tp.MenuBase {
    // ● constructor
    /**
     * Creates a context menu.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings.
     */
    constructor(CreateParams, Options) {
        super(CreateParams, Options);
        this.tpClass = "tp.ContextMenu";
        this.fDocumentKeyDownHandler = this.FuncBind(this.HandleDocumentKeyDown);
        tp.AddClass(this.Handle, tp.Classes.ContextMenu);
        this.Visible = false;
    }

    // ● protected
    /**
     * Handles document clicks.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleDocumentClick(e) {
        if (!tp.ContainsEventTarget(this.Handle, e.target))
            this.Visible = false;
    }
    /**
     * Handles document key-down events.
     * @param {KeyboardEvent} e The event.
     * @returns {void}
     */
    HandleDocumentKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Escape)) {
            this.Visible = false;
            e.preventDefault();
        }
    }
    /**
     * Hides menu UI after an item click.
     * @returns {void}
     */
    HideAfterItemClick() {
        this.Visible = false;
        this.HideAllLists();
    }

    // ● public
    /**
     * Shows the context menu at event coordinates.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    Show(e) {
        if (e instanceof MouseEvent) {
            e.preventDefault();
            this.ShowAt(e.clientX + 1, e.clientY + 1);
        }
    }
    /**
     * Shows the context menu at viewport coordinates.
     * @param {number} X The viewport X coordinate.
     * @param {number} Y The viewport Y coordinate.
     * @returns {void}
     */
    ShowAt(X, Y) {
        var Rect;
        if (!(this.Handle.parentNode instanceof HTMLElement))
            this.Document.body.appendChild(this.Handle);
        this.Position = "fixed";
        this.Handle.style.left = tp.px(X);
        this.Handle.style.top = tp.px(Y);
        this.Handle.style.zIndex = String(tp.MaxZIndexOf(this.Document.body) + 1);
        this.Visible = true;
        Rect = this.Handle.getBoundingClientRect();
        if (Rect.bottom > tp.Viewport.Height)
            this.Handle.style.top = tp.px(Math.max(0, Y - Rect.height));
        if (Rect.right > tp.Viewport.Width)
            this.Handle.style.left = tp.px(Math.max(0, tp.Viewport.Width - Rect.width));
        this.Handle.focus();
        setTimeout(function (Self) {
            Self.HookDocumentClick();
            Self.Document.addEventListener("keydown", Self.fDocumentKeyDownHandler);
        }, 0, this);
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        this.Document.removeEventListener("click", this.fDocumentClickHandler);
        this.Document.removeEventListener("keydown", this.fDocumentKeyDownHandler);
        this.fDocumentClickHandler = null;
        this.fDocumentKeyDownHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Returns true for context menus.
     * @returns {boolean} Returns true for context menus.
     */
    get IsContextMenu() {
        return true;
    }
    /**
     * Sets visibility and document hooks.
     * @param {boolean} Value True to show.
     * @returns {void}
     */
    set Visible(Value) {
        super.Visible = Value;
        if (Value !== true) {
            this.UnhookDocumentClick();
            this.Document.removeEventListener("keydown", this.fDocumentKeyDownHandler);
        }
    }
    /**
     * Gets visibility.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return super.Visible;
    }
};

// ● prototype
tp.ContextMenu.prototype.fDocumentKeyDownHandler = null;

// ● site menu event args
/**
 * Event arguments for tp.SiteMenu item clicks.
 */
tp.SiteMenuEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates site menu event arguments.
     * @param {HTMLElement} Element The clicked item element.
     * @param {MouseEvent|null|undefined} e The DOM event.
     */
    constructor(Element, e) {
        super("ItemClick", null, e);
        this.Element = Element instanceof HTMLElement ? Element : null;
        this.Command = this.Element ? tp.Data(this.Element, "command") || "" : "";
        this.ItemText = this.Element ? tp.SiteMenu.GetItemText(this.Element) : "";
    }
};

// ● prototype
/**
 * The clicked item element.
 * @type {HTMLElement|null}
 */
tp.SiteMenuEventArgs.prototype.Element = null;
/**
 * The clicked item command.
 * @type {string}
 */
tp.SiteMenuEventArgs.prototype.Command = "";
/**
 * The clicked item text.
 * @type {string}
 */
tp.SiteMenuEventArgs.prototype.ItemText = "";

// ● site menu
/**
 * Represents a document/site menu with a responsive toggle button and content columns.
 *
 * Events:
 * - ItemClick
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
tp.SiteMenu = class extends tp.Component {
    // ● constructor
    /**
     * Creates a site menu.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create parameters, handle, selector, or null.
     * @param {object|null|undefined} Options Optional settings.
     */
    constructor(CreateParams, Options) {
        var Params = arguments.length > 1 ? new tp.CreateParams(Options) : tp.Component.CreateParams(CreateParams);
        if (arguments.length > 1)
            Params.Handle = CreateParams;
        super(Params);
        this.tpClass = "tp.SiteMenu";
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.fMouseOverHandler = this.FuncBind(this.HandleMouseOver);
        this.fDocumentClickHandler = this.FuncBind(this.HandleDocumentClick);
        this.fWindowResizeHandler = this.FuncBind(this.HandleWindowResize);
        tp.AddClass(this.Handle, tp.Classes.SiteMenu);
        this.ReadMarkupParams();
        this.EnsureMarkup();
        this.Handle.addEventListener("click", this.fClickHandler);
        this.Handle.addEventListener("mouseover", this.fMouseOverHandler);
        this.Document.addEventListener("click", this.fDocumentClickHandler);
        window.addEventListener("resize", this.fWindowResizeHandler);
        this.UpdateScreenMode(true);
    }

    // ● protected
    /**
     * Reads menu settings from data-* attributes.
     * @returns {void}
     */
    ReadMarkupParams() {
        var Value = tp.Data(this.Handle, "break-point");
        if (!tp.IsBlank(Value))
            this.BreakPoint = tp.ToInt(Value);
    }
    /**
     * Ensures the required menu child elements.
     * @returns {void}
     */
    EnsureMarkup() {
        if (!this.ToggleElement) {
            this.fToggleElement = document.createElement("div");
            this.fToggleElement.className = tp.Classes.Toggle;
            this.fToggleElement.innerHTML = "<div class=\"" + tp.Classes.Btn + "\">Menu</div><div class=\"" + tp.Classes.FlexFill + "\"></div>";
            this.Handle.insertBefore(this.fToggleElement, this.Handle.firstChild);
        }
        if (!this.MenuStrip) {
            this.fMenuStrip = document.createElement("div");
            this.fMenuStrip.className = tp.Classes.Strip + " " + tp.Classes.Normal;
            this.Handle.appendChild(this.fMenuStrip);
        }
    }
    /**
     * Returns the closest site menu item for an event target.
     * @param {EventTarget|null|undefined} Target The event target.
     * @returns {HTMLElement|null} Returns the item element or null.
     */
    FindItemElement(Target) {
        var Element = Target;
        while (Element instanceof HTMLElement && Element !== this.Handle) {
            if (Element.classList.contains(tp.Classes.Item) && tp.ContainsElement(this.Handle, Element))
                return Element;
            Element = Element.parentElement;
        }
        return null;
    }
    /**
     * Returns true when an item is a top menu item.
     * @param {HTMLElement|null|undefined} Item The item.
     * @returns {boolean} Returns true when the item belongs directly to the strip.
     */
    IsTopItem(Item) {
        return Item instanceof HTMLElement && Item.parentElement === this.MenuStrip;
    }
    /**
     * Returns true when an item has a content panel.
     * @param {HTMLElement|null|undefined} Item The item.
     * @returns {boolean} Returns true when item has content.
     */
    HasContent(Item) {
        return this.GetItemContent(Item) instanceof HTMLElement;
    }
    /**
     * Returns an item content panel.
     * @param {HTMLElement|null|undefined} Item The item.
     * @returns {HTMLElement|null} Returns the content element or null.
     */
    GetItemContent(Item) {
        return Item instanceof HTMLElement ? tp.Select(Item, "." + tp.Classes.Content) : null;
    }
    /**
     * Sets an item as the active top item.
     * @param {HTMLElement} Item The item.
     * @returns {void}
     */
    SetActiveItem(Item) {
        this.ClearActiveItem();
        if (this.IsTopItem(Item) && this.HasContent(Item))
            tp.AddClass(Item, tp.Classes.Active);
    }
    /**
     * Clears the active item.
     * @returns {void}
     */
    ClearActiveItem() {
        var Item = this.ActiveItem;
        if (Item)
            tp.RemoveClass(Item, tp.Classes.Active);
    }
    /**
     * Toggles small-screen strip visibility.
     * @returns {void}
     */
    ToggleStrip() {
        this.ClearActiveItem();
        if (this.MenuStrip)
            tp.ToggleClass(this.MenuStrip, tp.Classes.Hide);
    }
    /**
     * Updates small-screen state.
     * @param {boolean} Force True to force update.
     * @returns {void}
     */
    UpdateScreenMode(Force) {
        var IsSmall = window.innerWidth <= this.BreakPoint;
        if (Force === true || IsSmall !== this.fIsSmallScreen) {
            this.fIsSmallScreen = IsSmall;
            this.ClearActiveItem();
            if (IsSmall) {
                tp.AddClass(this.Handle, tp.Classes.Small);
                tp.AddClass(this.MenuStrip, tp.Classes.Hide);
            } else {
                tp.RemoveClass(this.Handle, tp.Classes.Small);
                tp.RemoveClass(this.MenuStrip, tp.Classes.Hide);
            }
        }
    }
    /**
     * Handles DOM click events.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleClick(e) {
        var Item;
        if (this.ToggleElement && tp.ContainsEventTarget(this.ToggleElement, e.target)) {
            e.preventDefault();
            e.stopPropagation();
            this.ToggleStrip();
            return;
        }
        Item = this.FindItemElement(e.target);
        if (!Item)
            return;
        e.preventDefault();
        e.stopPropagation();
        if (Item.classList.contains(tp.Classes.Title))
            return;
        if (this.IsTopItem(Item) && this.HasContent(Item)) {
            if (Item === this.ActiveItem)
                this.ClearActiveItem();
            else
                this.SetActiveItem(Item);
        } else if (!this.IsSmallScreen) {
            this.ClearActiveItem();
        }
        this.OnItemClick(Item, e);
    }
    /**
     * Handles mouse-over events.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleMouseOver(e) {
        var Item = this.FindItemElement(e.target);
        if (!this.IsSmallScreen && this.ActiveItem && this.IsTopItem(Item) && Item !== this.ActiveItem)
            this.SetActiveItem(Item);
    }
    /**
     * Handles document click events.
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleDocumentClick(e) {
        if (!tp.ContainsEventTarget(this.Handle, e.target))
            this.ClearActiveItem();
    }
    /**
     * Handles window resize events.
     * @returns {void}
     */
    HandleWindowResize() {
        this.UpdateScreenMode(false);
    }
    /**
     * Triggers the ItemClick event.
     * @param {HTMLElement} Item The item.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    OnItemClick(Item, e) {
        this.Trigger("ItemClick", new tp.SiteMenuEventArgs(Item, e));
    }

    // ● public
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.IsDisposed)
            return;
        this.Handle.removeEventListener("click", this.fClickHandler);
        this.Handle.removeEventListener("mouseover", this.fMouseOverHandler);
        this.Document.removeEventListener("click", this.fDocumentClickHandler);
        window.removeEventListener("resize", this.fWindowResizeHandler);
        this.fClickHandler = null;
        this.fMouseOverHandler = null;
        this.fDocumentClickHandler = null;
        this.fWindowResizeHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Returns the active top item.
     * @returns {HTMLElement|null} Returns the active top item or null.
     */
    get ActiveItem() {
        return this.MenuStrip ? tp.Select(this.MenuStrip, "." + tp.Classes.Item + "." + tp.Classes.Active) : null;
    }
    /**
     * Returns the toggle element.
     * @returns {HTMLElement|null} Returns the toggle element.
     */
    get ToggleElement() {
        if (!this.fToggleElement && this.HasHandle)
            this.fToggleElement = tp.Select(this.Handle, "." + tp.Classes.Toggle);
        return this.fToggleElement;
    }
    /**
     * Returns the menu strip element.
     * @returns {HTMLElement|null} Returns the menu strip element.
     */
    get MenuStrip() {
        if (!this.fMenuStrip && this.HasHandle)
            this.fMenuStrip = tp.Select(this.Handle, "." + tp.Classes.Strip);
        return this.fMenuStrip;
    }
    /**
     * Returns true when the menu is in small-screen mode.
     * @returns {boolean} Returns true when small.
     */
    get IsSmallScreen() {
        return this.fIsSmallScreen === true;
    }
    /**
     * Gets or sets the responsive breakpoint.
     * @returns {number} Returns the breakpoint width.
     */
    get BreakPoint() {
        return this.fBreakPoint;
    }
    /**
     * Gets or sets the responsive breakpoint.
     * @param {number} Value The breakpoint width.
     * @returns {void}
     */
    set BreakPoint(Value) {
        this.fBreakPoint = Math.max(1, tp.ToInt(Value));
    }
    /**
     * Returns direct item text.
     * @param {HTMLElement} Item The item.
     * @returns {string} Returns the item text.
     */
    static GetItemText(Item) {
        var TextNode = tp.FindTextNode(Item);
        return TextNode ? (TextNode.nodeValue || "").trim() : "";
    }
};

// ● prototype
tp.SiteMenu.prototype.fToggleElement = null;
tp.SiteMenu.prototype.fMenuStrip = null;
tp.SiteMenu.prototype.fClickHandler = null;
tp.SiteMenu.prototype.fMouseOverHandler = null;
tp.SiteMenu.prototype.fDocumentClickHandler = null;
tp.SiteMenu.prototype.fWindowResizeHandler = null;
tp.SiteMenu.prototype.fIsSmallScreen = false;
tp.SiteMenu.prototype.fBreakPoint = 768;
