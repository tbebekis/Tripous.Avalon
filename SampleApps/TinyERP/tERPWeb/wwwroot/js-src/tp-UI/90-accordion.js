// ● accordion
/**
 * An accordion container.
 * Each child item is a div containing two divs: a title div and a content div.
 *
 * Events:
 * - ChildCreating
 * - ChildCreated
 *
 * @example
 * <div id="Accordion" class="tp-Accordion">
 *     <div>
 *         <div>Item 1</div>
 *         <div>Content of item 1</div>
 *     </div>
 * </div>
 */
tp.Accordion = class extends tp.Component {
    // ● constructor
    /**
     * Creates an accordion.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The accordion create parameters, handle, or selector.
     */
    constructor(CreateParams) {
        super(CreateParams);
        this.tpClass = "tp.Accordion";
        tp.AddClass(this.Handle, tp.Classes.Accordion);
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.Handle.addEventListener("click", this.fClickHandler);
    }

    // ● protected
    /**
     * Finds and returns the item whose title was clicked.
     * @param {Event} e The DOM event.
     * @returns {HTMLElement|null} Returns the clicked item or null.
     */
    FindClickedChild(e) {
        var List = this.GetElementList();
        var Index;
        var Item;
        var TitleElement;
        for (Index = 0; Index < List.length; Index++) {
            Item = List[Index];
            TitleElement = this.TitleElementOf(Item);
            if (tp.IsHTMLElement(TitleElement) && tp.ContainsEventTarget(TitleElement, e.target))
                return Item;
        }
        return null;
    }
    /**
     * Returns the title element of an item.
     * @param {HTMLElement|null|undefined} Item The item element.
     * @returns {HTMLElement|null} Returns the title element or null.
     */
    TitleElementOf(Item) {
        return tp.IsHTMLElement(Item) && Item.children.length > 0 && tp.IsHTMLElement(Item.children[0]) ? Item.children[0] : null;
    }
    /**
     * Returns the content element of an item.
     * @param {HTMLElement|null|undefined} Item The item element.
     * @returns {HTMLElement|null} Returns the content element or null.
     */
    ContentElementOf(Item) {
        return tp.IsHTMLElement(Item) && Item.children.length > 1 && tp.IsHTMLElement(Item.children[1]) ? Item.children[1] : null;
    }
    /**
     * Creates and returns an item element.
     * @param {string|null|undefined} Title Optional item title.
     * @returns {HTMLElement} Returns the created item.
     */
    CreateChild(Title) {
        var Args = this.OnChildCreating(Title);
        var Result = Args && tp.IsHTMLElement(Args.Child) ? Args.Child : null;
        var Element;
        if (!Result) {
            Result = this.Document.createElement("div");
            Element = this.Document.createElement("div");
            Element.innerHTML = tp.IsString(Title) && !tp.IsBlank(Title) ? Title : "no-name";
            Result.appendChild(Element);
            Element = this.Document.createElement("div");
            Result.appendChild(Element);
        }
        this.OnChildCreated(Result);
        return Result;
    }
    /**
     * Handles click events on item title elements.
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    HandleClick(e) {
        var Item = this.FindClickedChild(e);
        var Index;
        var IsExpanded;
        if (Item) {
            Index = this.IndexOfElement(Item);
            IsExpanded = tp.HasClass(Item, tp.Classes.Expanded);
            this.Expand(!IsExpanded, Index);
        }
    }
    /**
     * Event trigger called before a child item is created.
     * A listener may set Args.Child to an HTMLElement.
     * @param {string|null|undefined} Title Optional item title.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnChildCreating(Title) {
        return this.Trigger("ChildCreating", { Title: Title, Child: null });
    }
    /**
     * Event trigger called after a child item is created.
     * @param {HTMLElement} Child The created child.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnChildCreated(Child) {
        return this.Trigger("ChildCreated", { Child: Child });
    }

    // ● public
    /**
     * Expands or collapses one or all items.
     * @param {boolean} Flag True to expand; false to collapse.
     * @param {number|null|undefined} ChildIndex The item index, or -1 for all items.
     * @returns {void}
     */
    Expand(Flag, ChildIndex) {
        var List = this.GetElementList();
        var Index;
        var Item;
        ChildIndex = tp.IsNumber(ChildIndex) ? ChildIndex : -1;
        if (ChildIndex < 0) {
            for (Index = 0; Index < List.length; Index++)
                tp.RemoveClass(List[Index], tp.Classes.Expanded);
            if (Flag === true) {
                for (Index = 0; Index < List.length; Index++)
                    tp.AddClass(List[Index], tp.Classes.Expanded);
            }
        } else {
            if (!this.AllowMultiExpand) {
                for (Index = 0; Index < List.length; Index++)
                    tp.RemoveClass(List[Index], tp.Classes.Expanded);
            }
            Item = List[ChildIndex];
            if (tp.IsHTMLElement(Item)) {
                if (Flag === true)
                    tp.AddClass(Item, tp.Classes.Expanded);
                else
                    tp.RemoveClass(Item, tp.Classes.Expanded);
            }
        }
    }
    /**
     * Toggles the expansion of an item.
     * @param {number} Index The item index.
     * @returns {void}
     */
    Toggle(Index) {
        if (this.GetElementAt(Index))
            this.Expand(!this.IsExpanded(Index), Index);
    }
    /**
     * Returns true when an item is expanded.
     * @param {number} Index The item index.
     * @returns {boolean} Returns true when the item is expanded.
     */
    IsExpanded(Index) {
        var Item = this.GetElementAt(Index);
        return tp.IsHTMLElement(Item) && tp.HasClass(Item, tp.Classes.Expanded);
    }
    /**
     * Adds an item and returns it.
     * @param {string|null|undefined} Title Optional item title.
     * @returns {HTMLElement|null} Returns the added item or null.
     */
    AddItem(Title) {
        return this.InsertItem(this.Count, Title);
    }
    /**
     * Inserts an item and returns it.
     * @param {number} Index The insert index.
     * @param {string|null|undefined} Title Optional item title.
     * @returns {HTMLElement|null} Returns the inserted item or null.
     */
    InsertItem(Index, Title) {
        var List;
        var Child;
        var ReferenceChild;
        if (!this.HasHandle)
            return null;
        List = this.GetElementList();
        Child = this.CreateChild(Title);
        if (List.length === 0 || Index < 0 || Index >= List.length) {
            Index = List.length;
            this.Handle.appendChild(Child);
        } else {
            ReferenceChild = List[Index];
            this.Handle.insertBefore(Child, ReferenceChild);
        }
        this.Expand(true, Index);
        return Child;
    }
    /**
     * Returns the title element of an item.
     * @param {number} Index The item index.
     * @returns {HTMLElement|null} Returns the title element or null.
     */
    TitleElementAt(Index) {
        return this.TitleElementOf(this.GetElementAt(Index));
    }
    /**
     * Returns the content element of an item.
     * @param {number} Index The item index.
     * @returns {HTMLElement|null} Returns the content element or null.
     */
    ContentElementAt(Index) {
        return this.ContentElementOf(this.GetElementAt(Index));
    }
    /**
     * Returns the title text of an item.
     * @param {number} Index The item index.
     * @returns {string} Returns the title text.
     */
    GetTitleAt(Index) {
        var Element = this.TitleElementAt(Index);
        return tp.IsHTMLElement(Element) ? Element.innerHTML : "";
    }
    /**
     * Sets the title text of an item.
     * @param {number} Index The item index.
     * @param {string} Text The title text.
     * @returns {void}
     */
    SetTitleAt(Index, Text) {
        var Element = this.TitleElementAt(Index);
        if (tp.IsHTMLElement(Element))
            Element.innerHTML = Text;
    }
    /**
     * Returns all title elements.
     * @returns {HTMLElement[]} Returns title elements.
     */
    GetTitleElements() {
        var Result = [];
        this.GetElementList().forEach(function (Item) {
            var TitleElement = this.TitleElementOf(Item);
            if (tp.IsHTMLElement(TitleElement))
                Result.push(TitleElement);
        }, this);
        return Result;
    }
    /**
     * Returns all content panel elements.
     * @returns {HTMLElement[]} Returns content panel elements.
     */
    GetPanelElements() {
        var Result = [];
        this.GetElementList().forEach(function (Item) {
            var ContentElement = this.ContentElementOf(Item);
            if (tp.IsHTMLElement(ContentElement))
                Result.push(ContentElement);
        }, this);
        return Result;
    }
    /**
     * Disposes this accordion.
     * @returns {void}
     */
    Dispose() {
        if (this.HasHandle && this.fClickHandler)
            this.Handle.removeEventListener("click", this.fClickHandler);
        this.fClickHandler = null;
        super.Dispose();
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Accordion.prototype.tpClass = "tp.Accordion";
/**
 * True to allow multiple expanded items.
 * @type {boolean}
 */
tp.Accordion.prototype.AllowMultiExpand = false;
/**
 * The cached click handler.
 * @type {Function|null}
 */
tp.Accordion.prototype.fClickHandler = null;
