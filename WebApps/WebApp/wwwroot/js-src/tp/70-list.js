// ● enumerator
/**
 * A classic enumerator for array-like objects.
 * @example
 * var Enumerator = new tp.Enumerator([1, 2, 3]);
 * var Sum = 0;
 * while (Enumerator.MoveNext()) {
 *     Sum += Enumerator.Current;
 * }
 */
tp.Enumerator = class {
    // ● constructor
    /**
     * Creates an enumerator.
     * @param {Array|object|null|undefined} List The array-like object to enumerate.
     */
    constructor(List) {
        this.Index = -1;
        this.List = List || [];
    }

    // ● properties
    /**
     * Gets the current element.
     * @returns {*} Returns the current element.
     */
    get Current() {
        return this.GetCurrent();
    }

    // ● public
    /**
     * Advances this enumerator to the next element.
     * @returns {boolean} Returns true when there is a next element; otherwise, false.
     */
    MoveNext() {
        if (this.Index + 1 < this.List.length) {
            this.Index++;
            return true;
        }
        return false;
    }
    /**
     * Resets this enumerator to its initial position.
     * @returns {void}
     */
    Reset() {
        this.Index = -1;
    }
    /**
     * Returns the current element.
     * @returns {*} Returns the current element or null when the current position is invalid.
     */
    GetCurrent() {
        if (this.Index >= 0 && this.Index < this.List.length)
            return this.List[this.Index];
        return null;
    }
    /**
     * Returns a native JavaScript iterator.
     * @returns {Iterator} Returns a native JavaScript iterator.
     */
    [Symbol.iterator]() {
        var List = this.List;
        var Index = 0;
        return {
            /**
             * Returns the next iterator result.
             * @returns {{value: *, done: boolean}} Returns the next iterator result.
             */
            next: function () {
                if (Index < List.length)
                    return { value: List[Index++], done: false };
                return { value: undefined, done: true };
            }
        };
    }
};
/**
 * The current index.
 * @type {number}
 */
tp.Enumerator.prototype.Index = -1;
/**
 * The array-like object being enumerated.
 * @type {Array|object|null}
 */
tp.Enumerator.prototype.List = null;

// ● list change type
/**
 * Indicates what kind of change happened, or is about to happen, in a list.
 * @type {object}
 */
tp.ListChangeType = {
    /**
     * The list is cleared.
     * @type {string}
     */
    Clear: "Clear",
    /**
     * The list is assigned from another list.
     * @type {string}
     */
    Assign: "Assign",
    /**
     * A range of items is added.
     * @type {string}
     */
    AddRange: "AddRange",
    /**
     * An item is inserted.
     * @type {string}
     */
    Insert: "Insert",
    /**
     * An item is removed.
     * @type {string}
     */
    Remove: "Remove",
    /**
     * A batch update is completed.
     * @type {string}
     */
    Update: "Update"
};
Object.freeze(tp.ListChangeType);

// ● list event arguments
/**
 * Event arguments for tp.List events.
 */
tp.ListEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates list event arguments.
     * @param {string} Action One of the tp.ListChangeType constants.
     */
    constructor(Action) {
        super();
        this.Action = Action || "";
        this.Index = -1;
        this.Item = null;
        this.Source = null;
    }
};
/**
 * Indicates what kind of change happened, or is about to happen, in a list.
 * @type {string}
 */
tp.ListEventArgs.prototype.Action = "";
/**
 * The item index.
 * @type {number}
 */
tp.ListEventArgs.prototype.Index = -1;
/**
 * The affected item.
 * @type {*}
 */
tp.ListEventArgs.prototype.Item = null;
/**
 * The source list or array.
 * @type {Array|null}
 */
tp.ListEventArgs.prototype.Source = null;

// ● list
/**
 * A list class based on JavaScript Array.
 *
 * Events are disabled by default. Set EventsEnabled to true to receive Changing and Changed events.
 */
tp.List = class extends Array {
    // ● constructor
    /**
     * Creates a list.
     * @param {Array|null|undefined} Source The optional source array.
     */
    constructor(Source) {
        super();
        this.fUpdateCounter = 0;
        this.fEventsEnabledCounter = 0;
        this.fEvents = null;
        if (!tp.IsNil(Source))
            this.Assign(Source);
    }

    // ● protected
    /**
     * Returns Array as the species constructor for native array methods.
     * @returns {ArrayConstructor} Returns the Array constructor.
     * @protected
     */
    static get [Symbol.species]() {
        return Array;
    }
    /**
     * Returns the normalized event name used as event map key.
     * @param {string} EventName The event name.
     * @returns {string} Returns the normalized event name.
     * @protected
     */
    NormalizeEventName(EventName) {
        return tp.IsBlank(EventName) ? "" : String(EventName).toUpperCase();
    }
    /**
     * Returns the invocation list for an event.
     * @param {string} EventName The event name.
     * @returns {tp.Listener[]} Returns the invocation list.
     * @protected
     */
    GetInvocationList(EventName) {
        var EventKey = this.NormalizeEventName(EventName);
        if (!this.fEvents || tp.IsBlank(EventKey))
            return [];
        return this.fEvents[EventKey] || [];
    }
    /**
     * Returns true when an event has listeners.
     * @param {string} EventName The event name.
     * @returns {boolean} Returns true when the event has listeners.
     * @protected
     */
    HasListeners(EventName) {
        return this.GetInvocationList(EventName).length > 0;
    }
    /**
     * Triggers the Changing event.
     * @param {tp.ListEventArgs} Args The event arguments.
     * @returns {void}
     * @protected
     */
    OnChanging(Args) {
        if (this.EventsEnabled === true && this.Updating === false)
            this.Trigger("Changing", Args);
    }
    /**
     * Triggers the Changed event.
     * @param {tp.ListEventArgs} Args The event arguments.
     * @returns {void}
     * @protected
     */
    OnChanged(Args) {
        if (this.EventsEnabled === true && this.Updating === false)
            this.Trigger("Changed", Args);
    }
    /**
     * Clears the list without triggering events.
     * @returns {void}
     * @protected
     */
    DoClear() {
        this.length = 0;
    }
    /**
     * Inserts an item without triggering events.
     * @param {number} Index The insert index.
     * @param {*} Item The item to insert.
     * @returns {*} Returns the inserted item.
     * @protected
     */
    DoInsert(Index, Item) {
        Index = tp.ToInt(Index);
        if (Index < 0)
            Index = 0;
        if (Index >= this.length)
            this.push(Item);
        else
            this.splice(Index, 0, Item);
        return Item;
    }
    /**
     * Removes an item at an index without triggering events.
     * @param {number} Index The item index.
     * @returns {void}
     * @protected
     */
    DoRemoveAt(Index) {
        this.splice(Index, 1);
    }
    /**
     * Appends a range without triggering events.
     * @param {Array} Items The items to append.
     * @returns {void}
     * @protected
     */
    DoAddRange(Items) {
        var Index;
        if (!Items)
            return;
        for (Index = 0; Index < Items.length; Index++)
            this.push(Items[Index]);
    }

    // ● events
    /**
     * Adds a listener to an event and returns the listener object.
     * @param {string} EventName The event name.
     * @param {Function} Func The callback function. Signature: function (Args: tp.EventArgs): void.
     * @param {object|null|undefined} Context The optional callback context.
     * @returns {tp.Listener|null} Returns the created listener or null.
     */
    On(EventName, Func, Context) {
        var EventKey = this.NormalizeEventName(EventName);
        var Listener;
        if (tp.IsBlank(EventKey) || !tp.IsFunction(Func))
            return null;
        if (!this.fEvents)
            this.fEvents = {};
        if (!(EventKey in this.fEvents))
            this.fEvents[EventKey] = [];
        Listener = new tp.Listener(Func, Context, false);
        this.fEvents[EventKey].push(Listener);
        return Listener;
    }
    /**
     * Removes a listener from an event.
     * @param {string} EventName The event name.
     * @param {tp.Listener|Function} ListenerOrFunc The listener object or callback function.
     * @returns {void}
     */
    Off(EventName, ListenerOrFunc) {
        var InvocationList = this.GetInvocationList(EventName);
        var Index;
        if (InvocationList.length === 0 || tp.IsNil(ListenerOrFunc))
            return;
        for (Index = InvocationList.length - 1; Index >= 0; Index--) {
            if (InvocationList[Index] === ListenerOrFunc || InvocationList[Index].Func === ListenerOrFunc)
                InvocationList.splice(Index, 1);
        }
    }
    /**
     * Triggers an event passing event arguments.
     * @param {string} EventName The event name.
     * @param {tp.EventArgs|object|null|undefined} Args The optional event arguments.
     * @returns {tp.EventArgs|null} Returns the event arguments or null.
     */
    Trigger(EventName, Args) {
        var InvocationList;
        var Listener;
        var Index;
        var EventKey = this.NormalizeEventName(EventName);
        if (!this.EventsEnabled || !this.fEvents || tp.IsBlank(EventKey))
            return null;
        InvocationList = this.GetInvocationList(EventKey).slice();
        if (InvocationList.length === 0)
            return null;
        Args = Args instanceof tp.EventArgs ? Args : new tp.EventArgs(Args || {});
        Args.EventName = tp.IsBlank(Args.EventName) ? String(EventName) : Args.EventName;
        Args.Sender = tp.IsNil(Args.Sender) ? this : Args.Sender;
        for (Index = 0; Index < InvocationList.length; Index++) {
            Listener = InvocationList[Index];
            Listener.Func.call(Listener.Context || this, Args);
            if (Listener.Once)
                this.Off(EventKey, Listener);
        }
        return Args;
    }

    // ● event handler
    /**
     * Implements the DOM EventListener interface.
     * @see {@link http://www.w3.org/TR/DOM-Level-2-Events/events.html#Events-EventListener|DOM Level 2 Events}
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    handleEvent(e) {
    }

    // ● public
    /**
     * Clears the list.
     * @returns {void}
     */
    Clear() {
        var Args = new tp.ListEventArgs(tp.ListChangeType.Clear);
        this.OnChanging(Args);
        if (Args.Cancel !== true) {
            this.DoClear();
            this.OnChanged(Args);
        }
    }
    /**
     * Removes all existing items and assigns a source array.
     * @param {Array|null|undefined} Source The source array.
     * @returns {void}
     */
    Assign(Source) {
        var Args;
        if (tp.IsNil(Source))
            return;
        Args = new tp.ListEventArgs(tp.ListChangeType.Assign);
        Args.Source = Source;
        this.OnChanging(Args);
        if (Args.Cancel !== true) {
            this.DoClear();
            this.DoAddRange(Source);
            this.OnChanged(Args);
        }
    }
    /**
     * Appends the items of a source array.
     * @param {Array|null|undefined} Items The source array.
     * @returns {void}
     */
    AddRange(Items) {
        var Args;
        if (tp.IsNil(Items))
            return;
        Args = new tp.ListEventArgs(tp.ListChangeType.AddRange);
        Args.Source = Items;
        this.OnChanging(Args);
        if (Args.Cancel !== true) {
            this.DoAddRange(Items);
            this.OnChanged(Args);
        }
    }
    /**
     * Adds an item.
     * @param {*} Item The item to add.
     * @returns {*} Returns the added item.
     */
    Add(Item) {
        return this.Insert(this.length, Item);
    }
    /**
     * Inserts an item at an index.
     * @param {number} Index The insert index.
     * @param {*} Item The item to insert.
     * @returns {*} Returns the inserted item.
     */
    Insert(Index, Item) {
        var Args = new tp.ListEventArgs(tp.ListChangeType.Insert);
        Args.Index = tp.ToInt(Index);
        Args.Item = Item;
        this.OnChanging(Args);
        if (Args.Cancel !== true) {
            Args.Item = this.DoInsert(Args.Index, Args.Item);
            this.OnChanged(Args);
        }
        return Args.Item;
    }
    /**
     * Removes an item.
     * @param {*} Item The item to remove.
     * @returns {void}
     */
    Remove(Item) {
        var Index = this.indexOf(Item);
        if (Index !== -1)
            this.RemoveAt(Index);
    }
    /**
     * Removes an item at an index.
     * @param {number} Index The item index.
     * @returns {void}
     */
    RemoveAt(Index) {
        var Args;
        Index = tp.ToInt(Index);
        if (Index < 0 || Index >= this.length)
            return;
        Args = new tp.ListEventArgs(tp.ListChangeType.Remove);
        Args.Index = Index;
        Args.Item = this[Index];
        this.OnChanging(Args);
        if (Args.Cancel !== true) {
            this.DoRemoveAt(Index);
            this.OnChanged(Args);
        }
    }
    /**
     * Returns true when this list contains an item.
     * @param {*} Item The item to check.
     * @returns {boolean} Returns true when this list contains the item.
     */
    Contains(Item) {
        return this.indexOf(Item) !== -1;
    }
    /**
     * Returns the index of an item.
     * @param {*} Item The item to check.
     * @returns {number} Returns the item index or -1.
     */
    IndexOf(Item) {
        return this.indexOf(Item);
    }
    /**
     * Returns a plain array containing all items.
     * @returns {Array} Returns a plain array containing all items.
     */
    ToArray() {
        return Array.prototype.slice.call(this);
    }
    /**
     * Finds an item by property value.
     * @param {string} Prop The property name.
     * @param {*} Value The property value.
     * @returns {*} Returns the found item or null.
     */
    FindBy(Prop, Value) {
        var Index = this.IndexBy(Prop, Value);
        return Index !== -1 ? this[Index] : null;
    }
    /**
     * Returns true when an item exists with a specified property value.
     * @param {string} Prop The property name.
     * @param {*} Value The property value.
     * @returns {boolean} Returns true when an item exists.
     */
    ContainsBy(Prop, Value) {
        return this.IndexBy(Prop, Value) !== -1;
    }
    /**
     * Returns the index of an item with a specified property value.
     * @param {string} Prop The property name.
     * @param {*} Value The property value.
     * @returns {number} Returns the item index or -1.
     */
    IndexBy(Prop, Value) {
        var Index;
        for (Index = 0; Index < this.length; Index++) {
            if (this[Index] && this[Index][Prop] === Value)
                return Index;
        }
        return -1;
    }
    /**
     * Removes an item with a specified property value.
     * @param {string} Prop The property name.
     * @param {*} Value The property value.
     * @returns {boolean} Returns true when an item was removed.
     */
    RemoveBy(Prop, Value) {
        var Index = this.IndexBy(Prop, Value);
        if (Index !== -1) {
            this.RemoveAt(Index);
            return true;
        }
        return false;
    }
    /**
     * Returns an enumerator for this list.
     * @returns {tp.Enumerator} Returns an enumerator for this list.
     */
    GetEnumerator() {
        return new tp.Enumerator(this);
    }

    // ● properties
    /**
     * Gets the number of items.
     * @returns {number} Returns the number of items.
     */
    get Count() {
        return this.length;
    }
    /**
     * Gets or sets a value indicating whether events are enabled.
     * @returns {boolean} Returns true when events are enabled.
     */
    get EventsEnabled() {
        return this.fEventsEnabledCounter > 0;
    }
    /**
     * Enables or disables events using a counter.
     * @param {boolean} Value True enables events; false disables events.
     * @returns {void}
     */
    set EventsEnabled(Value) {
        this.fEventsEnabledCounter += Value === true ? 1 : -1;
        if (this.fEventsEnabledCounter < 0)
            this.fEventsEnabledCounter = 0;
    }
    /**
     * Gets or sets a value indicating whether batch updating is in progress.
     * @returns {boolean} Returns true when batch updating is in progress.
     */
    get Updating() {
        return this.fUpdateCounter > 0;
    }
    /**
     * Starts or ends a batch update using a counter.
     * @param {boolean} Value True starts an update; false ends an update.
     * @returns {void}
     */
    set Updating(Value) {
        this.fUpdateCounter += Value === true ? 1 : -1;
        if (this.fUpdateCounter < 0)
            this.fUpdateCounter = 0;
        if (this.fUpdateCounter === 0) {
            var Args = new tp.ListEventArgs(tp.ListChangeType.Update);
            this.OnChanged(Args);
        }
    }
};
/**
 * The events-enabled counter.
 * @type {number}
 */
tp.List.prototype.fEventsEnabledCounter = 0;
/**
 * The list event map.
 * @type {object|null}
 */
tp.List.prototype.fEvents = null;
/**
 * The update counter.
 * @type {number}
 */
tp.List.prototype.fUpdateCounter = 0;

// ● collection item
/**
 * Represents an object that belongs to a tp.Collection.
 */
tp.CollectionItem = class {
    // ● constructor
    /**
     * Creates a collection item.
     */
    constructor() {
        this.Collection = null;
    }
};
/**
 * The collection this item belongs to.
 * @type {tp.Collection|null}
 */
tp.CollectionItem.prototype.Collection = null;
/**
 * Type guard. Returns true when a value is a collection item.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a collection item.
 */
tp.IsCollectionItem = function (Value) {
    return !tp.IsNil(Value) && "Collection" in Value;
};

// ● collection
/**
 * A list of collection items.
 */
tp.Collection = class extends tp.List {
    // ● constructor
    /**
     * Creates a collection.
     * @param {Function|null|undefined} ItemClass The collection item class.
     * @param {Array|null|undefined} Source The optional source array.
     */
    constructor(ItemClass, Source) {
        super();
        this.ItemClass = ItemClass || tp.CollectionItem;
        if (!tp.IsNil(Source))
            this.Assign(Source);
    }

    // ● protected
    /**
     * Creates a new item.
     * @returns {tp.CollectionItem} Returns the new item.
     * @protected
     */
    CreateItem() {
        if (!tp.IsFunction(this.ItemClass))
            tp.Throw("Can not create a collection item. ItemClass is not a function.");
        return new this.ItemClass();
    }
    /**
     * Called after an item is inserted.
     * @param {tp.CollectionItem} Item The inserted item.
     * @param {tp.CollectionItem|null|undefined} SourceItem The optional source item.
     * @returns {void}
     * @protected
     */
    ItemInserted(Item, SourceItem) {
        if (tp.IsCollectionItem(Item))
            Item.Collection = this;
    }
    /**
     * Inserts an item without triggering events.
     * @param {number} Index The insert index.
     * @param {tp.CollectionItem|null|undefined} Item The item to insert.
     * @returns {tp.CollectionItem} Returns the inserted item.
     * @protected
     */
    DoInsert(Index, Item) {
        if (tp.IsNil(Item))
            Item = this.CreateItem();
        if (!tp.IsCollectionItem(Item))
            tp.Throw("Can not insert an item. Item should be a tp.CollectionItem instance.");
        Item = super.DoInsert(Index, Item);
        this.ItemInserted(Item, null);
        return Item;
    }
    /**
     * Appends a range without triggering events.
     * @param {Array} Items The items to append.
     * @returns {void}
     * @protected
     */
    DoAddRange(Items) {
        var Index;
        var SourceItem;
        var Item;
        if (!Items)
            return;
        for (Index = 0; Index < Items.length; Index++) {
            SourceItem = Items[Index];
            if (!tp.IsCollectionItem(SourceItem))
                tp.Throw("Can not insert an item. Item should be a tp.CollectionItem instance.");
            Item = this.CopyItem(SourceItem);
            super.DoInsert(this.length, Item);
            this.ItemInserted(Item, SourceItem);
        }
    }

    // ● public
    /**
     * Adds an item. When no item is passed, a new item is created.
     * @param {tp.CollectionItem|null|undefined} Item The optional item to add.
     * @returns {tp.CollectionItem} Returns the added item.
     */
    Add(Item) {
        return this.Insert(this.length, Item);
    }
    /**
     * Inserts an item. When no item is passed, a new item is created.
     * @param {number} Index The insert index.
     * @param {tp.CollectionItem|null|undefined} Item The optional item to insert.
     * @returns {tp.CollectionItem} Returns the inserted item.
     */
    Insert(Index, Item) {
        return super.Insert(Index, Item);
    }
    /**
     * Creates a copy of a source item.
     * @param {tp.CollectionItem} SourceItem The source item.
     * @returns {tp.CollectionItem} Returns the copied item.
     */
    CopyItem(SourceItem) {
        var Item;
        if (SourceItem && tp.IsFunction(SourceItem.Clone))
            Item = SourceItem.Clone();
        else {
            Item = this.CreateItem();
            if (Item && tp.IsFunction(Item.Assign))
                Item.Assign(SourceItem);
            else
                tp.Assign(Item, SourceItem);
        }
        if (!tp.IsCollectionItem(Item))
            tp.Throw("Can not copy an item. Item should be a tp.CollectionItem instance.");
        return Item;
    }
};
/**
 * The class of child items.
 * @type {Function|null}
 */
tp.Collection.prototype.ItemClass = null;

// ● named item
/**
 * A collection item with a Name property.
 */
tp.NamedItem = class extends tp.CollectionItem {
    // ● constructor
    /**
     * Creates a named item.
     * @param {string|null|undefined} Name The item name.
     */
    constructor(Name) {
        super();
        this.fName = tp.IsString(Name) ? Name : "";
    }

    // ● properties
    /**
     * Gets the name.
     * @returns {string} Returns the name.
     * @protected
     */
    get_Name() {
        return this.fName;
    }
    /**
     * Sets the name.
     * @param {string} Value The name.
     * @returns {void}
     * @protected
     */
    set_Name(Value) {
        this.fName = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the name.
     * @returns {string} Returns the name.
     */
    get Name() {
        return this.get_Name();
    }
    /**
     * Sets the name.
     * @param {string} Value The name.
     * @returns {void}
     */
    set Name(Value) {
        this.set_Name(Value);
    }

    // ● public
    /**
     * Assigns a source item to this instance.
     * @param {tp.NamedItem} Source The source item.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsNil(Source))
            this.Name = Source.Name;
    }
};
/**
 * The item name.
 * @type {string}
 */
tp.NamedItem.prototype.fName = "";

// ● named items
/**
 * A collection of named items.
 */
tp.NamedItems = class extends tp.Collection {
    // ● constructor
    /**
     * Creates a named item collection.
     * @param {Function|null|undefined} ItemClass The named item class.
     */
    constructor(ItemClass) {
        super(ItemClass || tp.NamedItem, null);
    }

    // ● protected
    /**
     * Creates a new item.
     * @param {string|null|undefined} Name The optional item name.
     * @returns {tp.NamedItem} Returns the new item.
     * @protected
     */
    CreateItem(Name) {
        var Item = super.CreateItem();
        if (!tp.IsBlank(Name))
            Item.Name = Name;
        return Item;
    }

    // ● public
    /**
     * Adds an item.
     * @param {string|tp.NamedItem|null|undefined} NameOrItem A name or named item.
     * @returns {tp.NamedItem} Returns the added item.
     */
    Add(NameOrItem) {
        var Item = tp.IsString(NameOrItem) ? this.CreateItem(NameOrItem) : NameOrItem;
        return super.Add(Item);
    }
    /**
     * Inserts an item at an index.
     * @param {number} Index The insert index.
     * @param {string|tp.NamedItem|null|undefined} NameOrItem A name or named item.
     * @returns {tp.NamedItem} Returns the inserted item.
     */
    Insert(Index, NameOrItem) {
        var Item = tp.IsString(NameOrItem) ? this.CreateItem(NameOrItem) : NameOrItem;
        return super.Insert(Index, Item);
    }
    /**
     * Removes an item.
     * @param {string|tp.NamedItem} NameOrItem A name or named item.
     * @returns {void}
     */
    Remove(NameOrItem) {
        var Index = this.IndexOf(NameOrItem);
        if (Index !== -1)
            this.RemoveAt(Index);
    }
    /**
     * Returns true when an item exists.
     * @param {string|tp.NamedItem} NameOrItem A name or named item.
     * @returns {boolean} Returns true when this collection contains the item.
     */
    Contains(NameOrItem) {
        return this.IndexOf(NameOrItem) !== -1;
    }
    /**
     * Returns the index of an item.
     * @param {string|tp.NamedItem} NameOrItem A name or named item.
     * @returns {number} Returns the item index or -1.
     */
    IndexOf(NameOrItem) {
        return tp.IsString(NameOrItem) ? this.IndexBy("Name", NameOrItem) : super.IndexOf(NameOrItem);
    }
    /**
     * Finds an item by name.
     * @param {string} Name The item name.
     * @returns {tp.NamedItem|null} Returns the item or null.
     */
    Find(Name) {
        return this.FindBy("Name", Name);
    }
};

// ● dictionary
/**
 * A generic dictionary backed by an ordered entry list.
 */
tp.Dictionary = class {
    // ● constructor
    /**
     * Creates a dictionary.
     */
    constructor() {
        this.fItems = [];
    }

    // ● protected
    /**
     * Finds an entry by key.
     * @param {*} Key The key.
     * @returns {{Key: *, Value: *}|null} Returns the entry or null.
     * @protected
     */
    FindEntry(Key) {
        var Index;
        for (Index = 0; Index < this.fItems.length; Index++) {
            if (this.fItems[Index].Key === Key)
                return this.fItems[Index];
        }
        return null;
    }

    // ● public
    /**
     * Sets a key and its value.
     * @param {*} Key The key.
     * @param {*} Value The value.
     * @returns {void}
     */
    Set(Key, Value) {
        var Entry = this.FindEntry(Key);
        if (Entry)
            Entry.Value = Value;
        else
            this.fItems.push({ Key: Key, Value: Value });
    }
    /**
     * Returns the value of a specified key.
     * @param {*} Key The key.
     * @returns {*} Returns the value or null.
     */
    Get(Key) {
        var Entry = this.FindEntry(Key);
        return Entry ? Entry.Value : null;
    }
    /**
     * Returns true when this dictionary contains a key.
     * @param {*} Key The key.
     * @returns {boolean} Returns true when this dictionary contains the key.
     */
    ContainsKey(Key) {
        return this.FindEntry(Key) !== null;
    }
    /**
     * Removes an entry by key.
     * @param {*} Key The key.
     * @returns {void}
     */
    Remove(Key) {
        var Entry = this.FindEntry(Key);
        var Index;
        if (Entry) {
            Index = this.fItems.indexOf(Entry);
            if (Index >= 0)
                this.fItems.splice(Index, 1);
        }
    }
    /**
     * Removes all entries.
     * @returns {void}
     */
    Clear() {
        this.fItems.length = 0;
    }
    /**
     * Returns the keys.
     * @returns {Array} Returns the keys.
     */
    Keys() {
        return this.fItems.map(function (Entry) {
            return Entry.Key;
        });
    }
    /**
     * Returns the values.
     * @returns {Array} Returns the values.
     */
    Values() {
        return this.fItems.map(function (Entry) {
            return Entry.Value;
        });
    }
    /**
     * Returns the values. Compatibility alias for the old API.
     * @returns {Array} Returns the values.
     */
    Value() {
        return this.Values();
    }
    /**
     * Returns the value of a key, or a default value when the key does not exist.
     * @param {*} Key The key.
     * @param {*} Default The default value.
     * @returns {*} Returns the value or the default value.
     */
    ValueOf(Key, Default) {
        var Entry = this.FindEntry(Key);
        return Entry ? Entry.Value : (tp.IsNil(Default) ? null : Default);
    }
    /**
     * Returns an enumerator for this dictionary.
     * @returns {tp.Enumerator} Returns an enumerator for this dictionary.
     */
    GetEnumerator() {
        return new tp.Enumerator(this.fItems);
    }
    /**
     * Returns a native JavaScript iterator over dictionary entries.
     * @returns {Iterator} Returns a native JavaScript iterator.
     */
    [Symbol.iterator]() {
        return this.fItems[Symbol.iterator]();
    }

    // ● properties
    /**
     * Gets the number of entries.
     * @returns {number} Returns the number of entries.
     */
    get Count() {
        return this.fItems.length;
    }
};
/**
 * The entry list.
 * @type {{Key: *, Value: *}[]}
 */
tp.Dictionary.prototype.fItems = [];

// ● name-value string list
/**
 * A string list where each line has the format Name=Value.
 */
tp.NameValueStringList = class {
    // ● constructor
    /**
     * Creates a name-value string list.
     * @param {string|string[]|tp.NameValueStringList|tp.Dictionary|null|undefined} Source The optional source.
     */
    constructor(Source) {
        this.fItems = [];
        if (!tp.IsNil(Source))
            this.Assign(Source);
    }

    // ● protected
    /**
     * Returns a line from a name and a value.
     * @param {string} Name The name.
     * @param {string} Value The value.
     * @returns {string} Returns the line.
     * @protected
     */
    Concat(Name, Value) {
        Name = tp.IsNil(Name) ? "" : String(Name);
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (!tp.IsBlank(Value))
            return !tp.IsBlank(Name) ? tp.Format("{0}={1}", Name, Value) : Value;
        return !tp.IsBlank(Name) ? Name : "";
    }
    /**
     * Splits a line into name and value parts.
     * @param {string} Line The line.
     * @returns {{Name: string, Value: string}} Returns the split result.
     * @protected
     */
    Split(Line) {
        var Result = { Name: "", Value: "" };
        var Index;
        if (!tp.IsBlank(Line)) {
            Line = String(Line);
            Index = Line.indexOf("=");
            if (Index === -1) {
                Result.Name = tp.Trim(Line);
            } else {
                Result.Name = tp.Trim(Line.substring(0, Index));
                Result.Value = tp.Trim(Line.substring(Index + 1));
            }
        }
        return Result;
    }

    // ● public
    /**
     * Removes all lines.
     * @returns {void}
     */
    Clear() {
        this.fItems.length = 0;
    }
    /**
     * Assigns the content of a source object to this instance.
     * @param {string|string[]|tp.NameValueStringList|tp.Dictionary|null|undefined} Source The source value.
     * @returns {void}
     */
    Assign(Source) {
        var Keys;
        var Values;
        var Index;
        this.Clear();
        if (tp.IsArray(Source)) {
            this.fItems = tp.ListClone(Source, false);
        } else if (Source instanceof tp.NameValueStringList) {
            this.fItems = tp.ListClone(Source.fItems, false);
        } else if (Source instanceof tp.Dictionary) {
            Keys = Source.Keys();
            Values = Source.Values();
            for (Index = 0; Index < Keys.length; Index++)
                this.Add(Keys[Index], Values[Index]);
        } else if (tp.IsString(Source)) {
            this.Text = Source;
        }
    }
    /**
     * Clones this instance.
     * @returns {tp.NameValueStringList} Returns the clone.
     */
    Clone() {
        var Result = new tp.NameValueStringList();
        Result.fItems = tp.ListClone(this.fItems, false);
        return Result;
    }
    /**
     * Adds a string line. It must have the format Name=Value.
     * @param {string} Line The line to add.
     * @returns {void}
     */
    AddLine(Line) {
        this.InsertLine(this.fItems.length, Line);
    }
    /**
     * Inserts a string line at an index. It must have the format Name=Value.
     * @param {number} Index The insert index.
     * @param {string} Line The line to insert.
     * @returns {void}
     */
    InsertLine(Index, Line) {
        var Item;
        if (tp.IsBlank(Line))
            tp.Throw("Line can not be null, empty, or white space.");
        Item = this.Split(Line);
        this.Insert(Index, Item.Name, Item.Value);
    }
    /**
     * Removes a line.
     * @param {string} Line The line to remove.
     * @returns {void}
     */
    RemoveLine(Line) {
        this.RemoveAt(this.IndexOfLine(Line));
    }
    /**
     * Returns the index of a line using case-insensitive matching.
     * @param {string} Line The line to search for.
     * @returns {number} Returns the line index or -1.
     */
    IndexOfLine(Line) {
        var Index;
        for (Index = 0; Index < this.fItems.length; Index++) {
            if (tp.IsSameText(this.fItems[Index], Line))
                return Index;
        }
        return -1;
    }
    /**
     * Returns true when a line exists using case-insensitive matching.
     * @param {string} Line The line to search for.
     * @returns {boolean} Returns true when this instance contains the line.
     */
    ContainsLine(Line) {
        return this.IndexOfLine(Line) !== -1;
    }
    /**
     * Adds a line by constructing it from a name and value.
     * @param {string} Name The name.
     * @param {string} Value The value.
     * @returns {void}
     */
    Add(Name, Value) {
        this.Insert(this.fItems.length, Name, Value);
    }
    /**
     * Inserts a line by constructing it from a name and value.
     * @param {number} Index The insert index.
     * @param {string} Name The name.
     * @param {string} Value The value.
     * @returns {void}
     */
    Insert(Index, Name, Value) {
        if (tp.IsBlank(Name))
            tp.Throw("Name can not be null, empty, or white space.");
        if (!tp.IsBlank(Value) && this.Contains(Name))
            tp.Throw(tp.Format("Name already exists in list: {0}", Name));
        tp.ListInsert(this.fItems, Index, this.Concat(Name, Value));
    }
    /**
     * Returns the index of a name using case-insensitive matching.
     * @param {string} Name The name.
     * @returns {number} Returns the name index or -1.
     */
    IndexOf(Name) {
        var Item;
        var Index;
        for (Index = 0; Index < this.fItems.length; Index++) {
            Item = this.Split(this.fItems[Index]);
            if (tp.IsSameText(Name, Item.Name))
                return Index;
        }
        return -1;
    }
    /**
     * Returns true when a name exists using case-insensitive matching.
     * @param {string} Name The name.
     * @returns {boolean} Returns true when this instance contains the name.
     */
    Contains(Name) {
        return this.IndexOf(Name) !== -1;
    }
    /**
     * Removes a line by name.
     * @param {string} Name The name.
     * @returns {void}
     */
    Remove(Name) {
        this.RemoveAt(this.IndexOf(Name));
    }
    /**
     * Removes a line at an index.
     * @param {number} Index The index.
     * @returns {void}
     */
    RemoveAt(Index) {
        if (tp.InRange(this.fItems, Index))
            tp.ListRemoveAt(this.fItems, Index);
    }
    /**
     * Returns the lines as an array.
     * @returns {string[]} Returns the lines as an array.
     */
    ToArray() {
        return tp.ListClone(this.fItems, false);
    }
    /**
     * Returns the lines as a dictionary.
     * @returns {tp.Dictionary} Returns the lines as a dictionary.
     */
    ToDictionary() {
        var Result = new tp.Dictionary();
        var Item;
        var Index;
        for (Index = 0; Index < this.fItems.length; Index++) {
            Item = this.Split(this.fItems[Index]);
            Result.Set(Item.Name, Item.Value);
        }
        return Result;
    }
    /**
     * Returns the name of a line at an index.
     * @param {number} Index The index.
     * @returns {string} Returns the name.
     */
    NameAt(Index) {
        return tp.InRange(this.fItems, Index) ? this.Split(this.fItems[Index]).Name : "";
    }
    /**
     * Returns the value of a line at an index.
     * @param {number} Index The index.
     * @returns {string} Returns the value.
     */
    ValueAt(Index) {
        return tp.InRange(this.fItems, Index) ? this.Split(this.fItems[Index]).Value : "";
    }
    /**
     * Returns the value of a line by name.
     * @param {string} Name The name.
     * @returns {string} Returns the value or empty string.
     */
    GetValue(Name) {
        var Index = this.IndexOf(Name);
        return tp.InRange(this.fItems, Index) ? this.ValueAt(Index) : "";
    }
    /**
     * Sets the value of a line by name.
     * @param {string} Name The name.
     * @param {*} Value The value.
     * @returns {void}
     */
    SetValue(Name, Value) {
        var Index = this.IndexOf(Name);
        if (Index === -1)
            this.Add(Name, tp.IsNil(Value) ? "" : String(Value));
        else
            this.fItems[Index] = this.Concat(Name, Value);
    }
    /**
     * Returns an enumerator for this instance.
     * @returns {tp.Enumerator} Returns an enumerator.
     */
    GetEnumerator() {
        return new tp.Enumerator(this.fItems);
    }

    // ● properties
    /**
     * Gets or sets the text content.
     * @returns {string} Returns the text content.
     */
    get Text() {
        return this.fItems.join("\n");
    }
    /**
     * Sets the text content.
     * @param {string} Value The text content.
     * @returns {void}
     */
    set Text(Value) {
        var Lines;
        var Index;
        this.Clear();
        if (!tp.IsBlank(Value)) {
            Lines = tp.ToLines(Value);
            for (Index = 0; Index < Lines.length; Index++) {
                if (!tp.IsBlank(Lines[Index]))
                    this.AddLine(Lines[Index]);
            }
        }
    }
    /**
     * Gets the number of lines.
     * @returns {number} Returns the number of lines.
     */
    get Count() {
        return this.fItems.length;
    }
    /**
     * Gets the lines joined by comma.
     * @returns {string} Returns the lines joined by comma.
     */
    get CommaText() {
        return this.fItems.join(",");
    }
    /**
     * Gets the names.
     * @returns {string[]} Returns the names.
     */
    get Names() {
        var Result = [];
        var Index;
        for (Index = 0; Index < this.fItems.length; Index++)
            Result.push(this.Split(this.fItems[Index]).Name);
        return Result;
    }
};
/**
 * The line list.
 * @type {string[]}
 */
tp.NameValueStringList.prototype.fItems = [];
