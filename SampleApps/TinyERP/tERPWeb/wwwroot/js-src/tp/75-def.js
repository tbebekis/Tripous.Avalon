// ● def
/**
 * Base class for Tripous descriptor-like client objects.
 */
tp.Def = class {
    // ● constructor
    /**
     * Creates a descriptor.
     * @param {object|string|null|undefined} Source Optional source descriptor or descriptor name.
     */
    constructor(Source = null) {
        if (tp.IsString(Source))
            this.Name = Source;
        else if (tp.IsObject(Source))
            this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (tp.IsNil(Source))
            return;

        this.Name = Source.Name || "";
        this.TitleKey = Source.TitleKey || "";
        this.Title = Source.Title || "";
        this.Params = {};

        if (tp.IsObject(Source.Params))
            tp.MergePropsShallow(this.Params, Source.Params);
    }
    /**
     * Returns the descriptor name.
     * @returns {string} Returns the descriptor name.
     */
    toString() {
        return this.Name;
    }

    // ● public
    /**
     * Descriptor name.
     * @type {string}
     */
    Name = "";
    /**
     * Localization key used for title text.
     * @type {string}
     */
    TitleKey = "";
    /**
     * User-defined parameters.
     * @type {object}
     */
    Params = {};

    // ● properties
    /**
     * Gets the display title.
     * @returns {string} Returns the display title.
     */
    get Title() {
        if (!tp.IsBlankString(this.fTitle))
            return this.fTitle;
        if (!tp.IsBlankString(this.TitleKey))
            return this.TitleKey;
        return this.Name;
    }
    /**
     * Sets the display title.
     * @param {string} Value The display title.
     * @returns {void}
     */
    set Title(Value) {
        this.fTitle = tp.IsNil(Value) ? "" : String(Value);
    }
};
/**
 * Explicit display title.
 * @type {string}
 */
tp.Def.prototype.fTitle = "";

// ● def list
/**
 * A list of descriptor-like objects.
 */
tp.DefList = class {
    // ● constructor
    /**
     * Creates a descriptor list.
     * @param {Function|null|undefined} ItemClass The item class.
     */
    constructor(ItemClass = null) {
        this.ItemClass = ItemClass || tp.Def;
        this.Items = [];
    }

    // ● protected
    /**
     * Normalizes a source item to the item class.
     * @param {object|string|tp.Def} Source The source item.
     * @returns {tp.Def} Returns the normalized item.
     */
    NormalizeItem(Source) {
        if (Source instanceof this.ItemClass)
            return Source;
        return new this.ItemClass(Source);
    }

    // ● public
    /**
     * Adds an item.
     * @param {object|string|tp.Def} Source The source item.
     * @returns {tp.Def} Returns the added item.
     */
    Add(Source) {
        var Item = this.NormalizeItem(Source);

        if (this.Contains(Item.Name))
            tp.Throw(this.ItemClass.name + " '" + Item.Name + "' is already registered.");

        this.Items.push(Item);
        return Item;
    }
    /**
     * Adds a range of items.
     * @param {Array<object|string|tp.Def>|null|undefined} Items The source items.
     * @returns {void}
     */
    AddRange(Items) {
        var Index;

        if (!tp.IsArray(Items))
            return;

        for (Index = 0; Index < Items.length; Index++)
            this.Add(Items[Index]);
    }
    /**
     * Clears the list.
     * @returns {void}
     */
    Clear() {
        this.Items.length = 0;
    }
    /**
     * Finds an item by name.
     * @param {string} Name The item name.
     * @returns {tp.Def|null} Returns the item or null.
     */
    Find(Name) {
        var Index;
        var Item;

        for (Index = 0; Index < this.Items.length; Index++) {
            Item = this.Items[Index];
            if (tp.IsSameText(Name, Item.Name))
                return Item;
        }

        return null;
    }
    /**
     * Returns true when an item exists.
     * @param {string} Name The item name.
     * @returns {boolean} Returns true when the item exists.
     */
    Contains(Name) {
        return this.Find(Name) !== null;
    }
    /**
     * Returns an item by name or throws.
     * @param {string} Name The item name.
     * @returns {tp.Def} Returns the item.
     */
    Get(Name) {
        var Result = this.Find(Name);
        if (Result === null)
            tp.Throw(this.ItemClass.name + " not found: " + Name);
        return Result;
    }
    /**
     * Removes an item by name.
     * @param {string} Name The item name.
     * @returns {boolean} Returns true when an item was removed.
     */
    Remove(Name) {
        var Index;

        for (Index = 0; Index < this.Items.length; Index++) {
            if (tp.IsSameText(Name, this.Items[Index].Name)) {
                this.Items.splice(Index, 1);
                return true;
            }
        }

        return false;
    }
    /**
     * Sorts items by title.
     * @returns {void}
     */
    Sort() {
        this.Items.sort(function (A, B) {
            return A.Title.localeCompare(B.Title);
        });
    }
    /**
     * Assigns this list from a source array.
     * @param {Array<object|string|tp.Def>|null|undefined} Items The source items.
     * @returns {void}
     */
    Assign(Items) {
        this.Clear();
        this.AddRange(Items);
    }
    /**
     * Returns a JSON-friendly array.
     * @returns {Array} Returns a JSON-friendly array.
     */
    toJSON() {
        return this.Items;
    }
    /**
     * Returns a native JavaScript iterator.
     * @returns {Iterator} Returns a native JavaScript iterator.
     */
    [Symbol.iterator]() {
        return this.Items[Symbol.iterator]();
    }

    // ● public
    /**
     * The item class.
     * @type {Function}
     */
    ItemClass = tp.Def;
    /**
     * The item array.
     * @type {tp.Def[]}
     */
    Items = [];

    // ● properties
    /**
     * Gets the item count.
     * @returns {number} Returns the item count.
     */
    get Count() {
        return this.Items.length;
    }
};
